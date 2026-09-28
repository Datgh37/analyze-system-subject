<?php
/**
 * API: Báo cáo tổng quan tài chính theo tháng (Monthly Summary)
 * Phương thức: GET
 * Params: ?user_id={id}&month={m}&year={y}
 * Tác giả: Trịnh Thành Đạt (Lead)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

// Lấy tham số truy vấn
$user_id = filter_input(INPUT_GET, 'user_id', FILTER_VALIDATE_INT);
$month   = filter_input(INPUT_GET, 'month', FILTER_VALIDATE_INT) ?? (int)date('m');
$year    = filter_input(INPUT_GET, 'year', FILTER_VALIDATE_INT) ?? (int)date('Y');

if (!$user_id) {
    send_json_response("error", "Thiếu hoặc sai định dạng tham số user_id.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    // 1. Tính tổng Thu nhập và tổng Chi tiêu trong tháng
    $trans_stmt = $db->prepare("
        SELECT 
            transaction_type,
            COALESCE(SUM(amount), 0) AS total_amount
        FROM `transactions`
        WHERE `user_id` = :uid 
          AND MONTH(`transaction_date`) = :month 
          AND YEAR(`transaction_date`) = :year
        GROUP BY `transaction_type`
    ");
    $trans_stmt->bindParam(':uid', $user_id);
    $trans_stmt->bindParam(':month', $month);
    $trans_stmt->bindParam(':year', $year);
    $trans_stmt->execute();

    $rows = $trans_stmt->fetchAll();
    $total_income = 0.00;
    $total_expense = 0.00;

    foreach ($rows as $r) {
        if ($r['transaction_type'] === 'INCOME') {
            $total_income = (float)$r['total_amount'];
        } elseif ($r['transaction_type'] === 'EXPENSE') {
            $total_expense = (float)$r['total_amount'];
        }
    }

    $net_balance = $total_income - $total_expense;

    // 2. Lấy hạn mức ngân sách tháng (nếu có thiết lập)
    $budget_stmt = $db->prepare("
        SELECT `amount_limit` 
        FROM `budgets` 
        WHERE `user_id` = :uid 
          AND `category_id` IS NULL 
          AND `month` = :month 
          AND `year` = :year 
        LIMIT 1
    ");
    $budget_stmt->bindParam(':uid', $user_id);
    $budget_stmt->bindParam(':month', $month);
    $budget_stmt->bindParam(':year', $year);
    $budget_stmt->execute();

    $budget_row = $budget_stmt->fetch();
    $budget_limit = $budget_row ? (float)$budget_row['amount_limit'] : 0.00;

    $spent_percentage = ($budget_limit > 0) ? round(($total_expense / $budget_limit) * 100, 1) : 0.0;

    // 3. Tính tổng số dư của toàn bộ ví khả dụng hiện tại
    $wallet_stmt = $db->prepare("
        SELECT COALESCE(SUM(balance), 0) AS total_balance 
        FROM `wallets` 
        WHERE `user_id` = :uid
    ");
    $wallet_stmt->bindParam(':uid', $user_id);
    $wallet_stmt->execute();
    $total_wallet_balance = (float)$wallet_stmt->fetchColumn();

    // 4. Trả về kết quả JSON
    send_json_response("success", "Lấy dữ liệu tổng quan tháng thành công!", [
        "month"                   => (int)$month,
        "year"                    => (int)$year,
        "total_income"            => $total_income,
        "total_expense"           => $total_expense,
        "net_balance"             => $net_balance,
        "budget_limit"            => $budget_limit,
        "budget_spent_percentage" => $spent_percentage,
        "total_wallet_balance"    => $total_wallet_balance
    ], 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi máy chủ khi truy vấn báo cáo tháng: " . $e->getMessage(), null, 500);
}
?>
