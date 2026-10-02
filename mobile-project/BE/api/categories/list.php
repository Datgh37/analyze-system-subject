<?php
/**
 * API: Lấy danh sách Danh mục Thu / Chi
 * Phương thức: GET
 * Params: ?type={EXPENSE|INCOME} (Tùy chọn)
 * Bảo mật: Bắt buộc kèm Bearer JWT Token
 * 
 * Tác giả: Trần Trọng Đại (UI & Business API) & Trịnh Thành Đạt (Lead)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

// 1. Xác thực người dùng
$user_id = get_authenticated_user_id();

// 2. Lấy bộ lọc loại danh mục (Thu hay Chi)
$type = isset($_GET['type']) ? strtoupper(trim($_GET['type'])) : null;

$database = new Database();
$db = $database->getConnection();

try {
    // 3. Lấy danh mục mặc định của hệ thống (user_id IS NULL) HOẶC danh mục do chính user tạo
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
    $stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    if ($type && in_array($type, ['EXPENSE', 'INCOME'])) {
        $stmt->bindParam(':type', $type);
    }
    $stmt->execute();
    $categories = $stmt->fetchAll(PDO::FETCH_ASSOC);

    send_json_response("success", "Lấy danh sách danh mục thành công.", $categories, 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi truy vấn danh mục: " . $e->getMessage(), null, 500);
}
?>
