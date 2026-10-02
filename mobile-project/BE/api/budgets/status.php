<?php
/**
 * API: Kiểm tra Tiến độ Ngân sách Tháng & Cảnh báo Chi tiêu (Budget Progress & Alert)
 * Phương thức: GET
 * Params: ?month={m}&year={y}
 * Bảo mật: Bắt buộc kèm Bearer JWT Token
 * 
 * Tính năng UX #4:
 * - Tính toán tỷ lệ % ngân sách đã tiêu (Budget Progress Bar)
 * - Đưa ra mức cảnh báo trực quan: NORMAL (<80%), WARNING (80-100%), DANGER (>100%)
 * - Tính hạn mức chi tiêu an toàn mỗi ngày (Daily Safe Spending)
 * - Liệt kê chi tiết ngân sách theo từng danh mục
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

$month = filter_input(INPUT_GET, 'month', FILTER_VALIDATE_INT) ?: (int)date('m');
$year  = filter_input(INPUT_GET, 'year', FILTER_VALIDATE_INT) ?: (int)date('Y');

$database = new Database();
$db = $database->getConnection();

try {
    // 2. Lấy ngân sách tổng thể của cả tháng (category_id IS NULL)
    $stmt_budget = $db->prepare("
        SELECT id, amount_limit 
        FROM `budgets` 
        WHERE `user_id` = :uid 
          AND `category_id` IS NULL 
          AND `month` = :month 
          AND `year` = :year 
        LIMIT 1
    ");
    $stmt_budget->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $stmt_budget->bindParam(':month', $month, PDO::PARAM_INT);
    $stmt_budget->bindParam(':year', $year, PDO::PARAM_INT);
    $stmt_budget->execute();
    $overall_budget = $stmt_budget->fetch(PDO::FETCH_ASSOC);

    $total_limit = $overall_budget ? (float)$overall_budget['amount_limit'] : 0.00;

    // 3. Tính tổng chi tiêu thực tế trong tháng
    $stmt_spent = $db->prepare("
        SELECT COALESCE(SUM(amount), 0.00) AS total_spent 
        FROM `transactions` 
        WHERE `user_id` = :uid 
          AND `transaction_type` = 'EXPENSE' 
          AND MONTH(`transaction_date`) = :month 
          AND YEAR(`transaction_date`) = :year
    ");
    $stmt_spent->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $stmt_spent->bindParam(':month', $month, PDO::PARAM_INT);
    $stmt_spent->bindParam(':year', $year, PDO::PARAM_INT);
    $stmt_spent->execute();
    $total_spent = (float)$stmt_spent->fetchColumn();

    // 4. Tính toán phần trăm và mức cảnh báo
    $remaining_amount = $total_limit - $total_spent;
    $percentage = ($total_limit > 0) ? round(($total_spent / $total_limit) * 100, 1) : 0.0;

    if ($total_limit <= 0) {
        $status_level = "NOT_SET";
        $status_text  = "Chưa thiết lập ngân sách tổng cho tháng này.";
    } elseif ($percentage > 100) {
        $status_level = "DANGER";
        $status_text  = "Báo động: Bạn đã chi tiêu vượt quá hạn mức ngân sách đặt ra!";
    } elseif ($percentage >= 80) {
        $status_level = "WARNING";
        $status_text  = "Cảnh báo: Bạn đã chi tiêu gần chạm trần hạn mức ngân sách.";
    } else {
        $status_level = "NORMAL";
        $status_text  = "Chi tiêu đang nằm trong tầm kiểm soát an toàn.";
    }

    // 5. Tính toán Tốc độ chi tiêu an toàn mỗi ngày (Daily Safe Spending)
    $days_in_month = (int)date('t', strtotime("$year-$month-01"));
    $is_current_month = ((int)date('m') === $month && (int)date('Y') === $year);
    $current_day = $is_current_month ? (int)date('j') : $days_in_month;
    $days_remaining = max(1, $days_in_month - $current_day + 1);

    $daily_safe_spending = ($remaining_amount > 0) ? round($remaining_amount / $days_remaining) : 0.0;

    // 6. Lấy danh sách ngân sách chi tiết theo từng Danh mục
    $stmt_cat_budgets = $db->prepare("
        SELECT 
            b.id AS budget_id,
            b.category_id,
            c.name AS category_name,
            c.icon_name,
            c.color_code,
            b.amount_limit,
            COALESCE(SUM(t.amount), 0.00) AS spent_amount
        FROM `budgets` b
        INNER JOIN `categories` c ON b.category_id = c.id
        LEFT JOIN `transactions` t ON t.category_id = c.id 
            AND t.user_id = b.user_id 
            AND t.transaction_type = 'EXPENSE'
            AND MONTH(t.transaction_date) = b.month 
            AND YEAR(t.transaction_date) = b.year
        WHERE b.user_id = :uid 
          AND b.month = :month 
          AND b.year = :year
        GROUP BY b.id, b.category_id, c.name, c.icon_name, c.color_code, b.amount_limit
    ");
    $stmt_cat_budgets->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $stmt_cat_budgets->bindParam(':month', $month, PDO::PARAM_INT);
    $stmt_cat_budgets->bindParam(':year', $year, PDO::PARAM_INT);
    $stmt_cat_budgets->execute();
    $cat_budgets = $stmt_cat_budgets->fetchAll(PDO::FETCH_ASSOC);

    foreach ($cat_budgets as &$cb) {
        $cb['amount_limit'] = (float)$cb['amount_limit'];
        $cb['spent_amount'] = (float)$cb['spent_amount'];
        $cb['remaining']    = $cb['amount_limit'] - $cb['spent_amount'];
        $cb['percentage']   = ($cb['amount_limit'] > 0) ? round(($cb['spent_amount'] / $cb['amount_limit']) * 100, 1) : 0.0;
    }

    send_json_response("success", "Lấy dữ liệu tiến độ ngân sách thành công.", [
        "month"               => $month,
        "year"                => $year,
        "overall_budget"      => [
            "limit_amount"        => $total_limit,
            "spent_amount"        => $total_spent,
            "remaining_amount"    => $remaining_amount,
            "percentage"          => $percentage,
            "status_level"        => $status_level,
            "status_text"         => $status_text,
            "days_remaining"      => $days_remaining,
            "daily_safe_spending" => $daily_safe_spending
        ],
        "category_budgets"    => $cat_budgets
    ], 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi kiểm tra ngân sách: " . $e->getMessage(), null, 500);
}
?>
