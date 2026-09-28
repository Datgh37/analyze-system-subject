<?php
/**
 * API: Lấy danh sách Danh mục Thu / Chi
 * Phương thức: GET
 * Params: ?user_id={id}&type={EXPENSE|INCOME} (type là tùy chọn)
 * Phụ trách: Trần Trọng Đại (UI & Business API) | Boilerplate by Trịnh Thành Đạt
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

// Lấy tham số truy vấn
$user_id = filter_input(INPUT_GET, 'user_id', FILTER_VALIDATE_INT);
$type    = isset($_GET['type']) ? strtoupper(trim($_GET['type'])) : null;

if (!$user_id) {
    send_json_response("error", "Thiếu hoặc sai định dạng tham số user_id.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    // Lấy danh mục hệ thống (user_id IS NULL) HOẶC danh mục do chính user tạo
    $sql = "
        SELECT id, user_id, name, type, icon_name, color_code, 
               (CASE WHEN user_id IS NULL THEN 1 ELSE 0 END) AS is_system
        FROM `categories`
        WHERE (user_id IS NULL OR user_id = :uid)
    ";

    if ($type && in_array($type, ['EXPENSE', 'INCOME'])) {
        $sql .= " AND `type` = :type";
    }

    $sql .= " ORDER BY (user_id IS NULL) DESC, `id` ASC";

    $stmt = $db->prepare($sql);
    $stmt->bindParam(':uid', $user_id);
    if ($type && in_array($type, ['EXPENSE', 'INCOME'])) {
        $stmt->bindParam(':type', $type);
    }
    $stmt->execute();
    $categories = $stmt->fetchAll(PDO::FETCH_ASSOC);

    send_json_response("success", "Lấy danh sách danh mục thành công.", $categories, 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL: " . $e->getMessage(), null, 500);
}
