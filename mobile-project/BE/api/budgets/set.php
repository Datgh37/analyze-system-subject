<?php
/**
 * API: Thiết lập hoặc Cập nhật Hạn mức Ngân sách Tháng (Set Budget Limit)
 * Phương thức: POST (raw JSON)
 * Body: {
 *   "category_id": null, // null nếu là ngân sách tổng thể; int nếu giới hạn riêng danh mục
 *   "amount_limit": 6000000,
 *   "month": 9,
 *   "year": 2026
 * }
 * Bảo mật: Bắt buộc kèm Bearer JWT Token
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

// 1. Xác thực người dùng qua JWT
$user_id = get_authenticated_user_id();

$input = get_json_input();
$category_id  = isset($input['category_id']) && $input['category_id'] !== '' ? filter_var($input['category_id'], FILTER_VALIDATE_INT) : null;
if ($category_id === false) $category_id = null;

$amount_limit = isset($input['amount_limit']) ? (float)$input['amount_limit'] : 0.0;
$month        = filter_var($input['month'] ?? date('m'), FILTER_VALIDATE_INT);
$year         = filter_var($input['year'] ?? date('Y'), FILTER_VALIDATE_INT);

if ($amount_limit <= 0) {
    send_json_response("error", "Hạn mức ngân sách phải lớn hơn 0.", null, 400);
}

if ($month < 1 || $month > 12) {
    send_json_response("error", "Tháng áp dụng không hợp lệ (Phải từ 1 đến 12).", null, 400);
}

if ($year < 2020 || $year > 2050) {
    send_json_response("error", "Năm áp dụng không hợp lệ.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    // 2. Kiểm tra nếu có category_id thì danh mục đó có tồn tại không
    if ($category_id !== null) {
        $check_cat = $db->prepare("SELECT id, name FROM `categories` WHERE `id` = :cid AND (`user_id` IS NULL OR `user_id` = :uid)");
        $check_cat->bindParam(':cid', $category_id, PDO::PARAM_INT);
        $check_cat->bindParam(':uid', $user_id, PDO::PARAM_INT);
        $check_cat->execute();
        if (!$check_cat->fetch()) {
            send_json_response("error", "Danh mục không tồn tại hoặc không thuộc quyền sở hữu của bạn.", null, 404);
        }
    }

    // 3. Tìm xem đã có bản ghi ngân sách cho tháng/năm này chưa
    if ($category_id === null) {
        $check_stmt = $db->prepare("
            SELECT id FROM `budgets` 
            WHERE `user_id` = :uid AND `category_id` IS NULL AND `month` = :month AND `year` = :year 
            LIMIT 1
        ");
    } else {
        $check_stmt = $db->prepare("
            SELECT id FROM `budgets` 
            WHERE `user_id` = :uid AND `category_id` = :cid AND `month` = :month AND `year` = :year 
            LIMIT 1
        ");
        $check_stmt->bindParam(':cid', $category_id, PDO::PARAM_INT);
    }

    $check_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $check_stmt->bindParam(':month', $month, PDO::PARAM_INT);
    $check_stmt->bindParam(':year', $year, PDO::PARAM_INT);
    $check_stmt->execute();
    $existing = $check_stmt->fetch(PDO::FETCH_ASSOC);

    if ($existing) {
        // Cập nhật bản ghi hiện tại
        $budget_id = (int)$existing['id'];
        $update_stmt = $db->prepare("UPDATE `budgets` SET `amount_limit` = :amt WHERE `id` = :bid");
        $update_stmt->bindParam(':amt', $amount_limit);
        $update_stmt->bindParam(':bid', $budget_id, PDO::PARAM_INT);
        $update_stmt->execute();
        $action_msg = "Cập nhật hạn mức ngân sách thành công.";
    } else {
        // Thêm bản ghi ngân sách mới
        $insert_stmt = $db->prepare("
            INSERT INTO `budgets` (`user_id`, `category_id`, `amount_limit`, `month`, `year`) 
            VALUES (:uid, :cid, :amt, :month, :year)
        ");
        $insert_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
        $insert_stmt->bindParam(':cid', $category_id, $category_id === null ? PDO::PARAM_NULL : PDO::PARAM_INT);
        $insert_stmt->bindParam(':amt', $amount_limit);
        $insert_stmt->bindParam(':month', $month, PDO::PARAM_INT);
        $insert_stmt->bindParam(':year', $year, PDO::PARAM_INT);
        $insert_stmt->execute();
        $budget_id = (int)$db->lastInsertId();
        $action_msg = "Thiết lập hạn mức ngân sách mới thành công.";
    }

    send_json_response("success", $action_msg, [
        "id"           => $budget_id,
        "user_id"      => $user_id,
        "category_id"  => $category_id,
        "amount_limit" => $amount_limit,
        "month"        => $month,
        "year"         => $year
    ], 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi thiết lập ngân sách: " . $e->getMessage(), null, 500);
}
?>
