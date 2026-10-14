<?php
/**
 * API: Tạo Danh mục Thu / Chi Mới (Dành riêng cho Người dùng)
 * Phương thức: POST (raw JSON)
 * Body: {
 *   "name": "Đầu tư Chứng khoán",
 *   "type": "EXPENSE",
 *   "icon_name": "trending-up-outline",
 *   "color_code": "#FF9F1C"
 * }
 * Bảo mật: Bắt buộc kèm Bearer JWT Token
 * 
 * Tác giả: Trần Trọng Đại (UI & Business API) & Trịnh Thành Đạt (Lead)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

// 1. Xác thực người dùng
$user_id = get_authenticated_user_id();

$input = get_json_input();
$name       = trim($input['name'] ?? '');
$type       = strtoupper(trim($input['type'] ?? ''));
$icon_name  = trim($input['icon_name'] ?? 'pricetag-outline');
$color_code = trim($input['color_code'] ?? '#2B4C7E');

// 2. Kiểm tra tính hợp lệ
if (empty($name)) {
    send_json_response("error", "Tên danh mục không được để trống.", null, 400);
}

if (!in_array($type, ['EXPENSE', 'INCOME'])) {
    send_json_response("error", "Phân loại danh mục phải là EXPENSE hoặc INCOME.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    $stmt = $db->prepare("
        INSERT INTO `categories` (`user_id`, `name`, `type`, `icon_name`, `color_code`) 
        VALUES (:uid, :name, :type, :icon, :color)
    ");
    $stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $stmt->bindParam(':name', $name);
    $stmt->bindParam(':type', $type);
    $stmt->bindParam(':icon', $icon_name);
    $stmt->bindParam(':color', $color_code);
    $stmt->execute();

    $new_id = (int)$db->lastInsertId();

    send_json_response("success", "Tạo danh mục mới thành công.", [
        "id"         => $new_id,
        "user_id"    => $user_id,
        "name"       => $name,
        "type"       => $type,
        "icon_name"  => $icon_name,
        "color_code" => $color_code,
        "is_system"  => 0
    ], 201);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi tạo danh mục: " . $e->getMessage(), null, 500);
}
?>
