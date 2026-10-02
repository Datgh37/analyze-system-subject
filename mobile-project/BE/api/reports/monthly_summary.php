<?php
/**
 * API: Báo cáo Tổng quan Tài chính theo Tháng (Monthly Summary)
 * Phương thức: GET
 * Params: ?month={m}&year={y}
 * Bảo mật: Bắt buộc kèm Bearer JWT Token
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

// 1. Xác thực người dùng qua JWT
$user_id = get_authenticated_user_id();

// 2. Lấy tham số truy vấn tháng và năm
$month = filter_input(INPUT_GET, 'month', FILTER_VALIDATE_INT) ?: (int)date('m');
$year  = filter_input(INPUT_GET, 'year', FILTER_VALIDATE_INT) ?: (int)date('Y');

$database = new Database();
$db = $database->getConnection();

try {
    // 3. Tính tổng Thu nhập và tổng Chi tiêu trong tháng
    $trans_stmt = $db->prepare("
        SELECT 
            transaction_type,
            COALESCE(SUM(amount), 0.00) AS total_amount
        FROM `transactions`
        WHERE `user_id` = :uid 
          AND MONTH(`transaction_date`) = :month 
          AND YEAR(`transaction_date`) = :year
        GROUP BY `transaction_type`
    ");
    $trans_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $trans_stmt->bindParam(':month', $month, PDO::PARAM_INT);
    $trans_stmt->bindParam(':year', $year, PDO::PARAM_INT);
    $trans_stmt->execute();

    $rows = $trans_stmt->fetchAll(PDO::FETCH_ASSOC);
    $total_income  = 0.00;
    $total_expense = 0.00;

    foreach ($rows as $r) {
        if ($r['transaction_type'] === 'INCOME') {
            $total_income = (float)$r['total_amount'];
        } elseif ($r['transaction_type'] === 'EXPENSE') {
            $total_expense = (float)$r['total_amount'];
        }
    }

    $net_balance = $total_income - $total_expense;

    // 4. Lấy hạn mức ngân sách tháng (nếu có thiết lập)
    $budget_stmt = $db->prepare("
        SELECT `amount_limit` 
        FROM `budgets` 
        WHERE `user_id` = :uid 
          AND `category_id` IS NULL 
          AND `month` = :month 
          AND `year` = :year 
        LIMIT 1
    ");
    $budget_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $budget_stmt->bindParam(':month', $month, PDO::PARAM_INT);
    $budget_stmt->bindParam(':year', $year, PDO::PARAM_INT);
    $budget_stmt->execute();

    $budget_row = $budget_stmt->fetch(PDO::FETCH_ASSOC);
    $budget_limit = $budget_row ? (float)$budget_row['amount_limit'] : 0.00;

    $spent_percentage = ($budget_limit > 0) ? round(($total_expense / $budget_limit) * 100, 1) : 0.0;

    // 5. Tính tổng số dư của toàn bộ ví khả dụng hiện tại
    $wallet_stmt = $db->prepare("
        SELECT COALESCE(SUM(balance), 0.00) AS total_balance 
        FROM `wallets` 
        WHERE `user_id` = :uid
    ");
    $wallet_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $wallet_stmt->execute();
    $total_wallet_balance = (float)$wallet_stmt->fetchColumn();

    // 6. Tính tỷ lệ tiết kiệm (Savings Rate)
    $savings_rate = ($total_income > 0) ? round(($net_balance / $total_income) * 100, 1) : 0.0;

    send_json_response("success", "Lấy dữ liệu tổng quan tháng thành công!", [
        "user_id"                 => $user_id,
        "month"                   => (int)$month,
        "year"                    => (int)$year,
        "total_income"            => $total_income,
        "total_expense"           => $total_expense,
        "net_balance"             => $net_balance,
        "savings_rate"            => $savings_rate,
        "budget_limit"            => $budget_limit,
        "budget_spent_percentage" => $spent_percentage,
        "total_wallet_balance"    => $total_wallet_balance
    ], 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi truy vấn báo cáo tháng: " . $e->getMessage(), null, 500);
}
?>
