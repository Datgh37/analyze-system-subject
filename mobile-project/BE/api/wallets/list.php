<?php
/**
 * API: Lấy danh sách Ví và Số dư của Người dùng
 * Phương thức: GET
 * Bảo mật: Bắt buộc kèm Bearer JWT Token
 * 
 * Tác giả: Trần Trọng Đại (UI & Business API) & Trịnh Thành Đạt (Lead)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

// Xác thực danh tính an toàn từ Token
$user_id = get_authenticated_user_id();

$database = new Database();
$db = $database->getConnection();

try {
    $stmt = $db->prepare("
        SELECT id, user_id, name, name AS wallet_name, wallet_type, balance, 'VND' AS currency, 'wallet' AS icon_name, created_at
        FROM `wallets`
        WHERE `user_id` = :uid
        ORDER BY `id` ASC
    ");
    $stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $stmt->execute();
    $wallets = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Tính tổng tài sản khả dụng (Total Net Balance)
    $total_balance = 0.0;
    foreach ($wallets as &$w) {
        $w['balance'] = (float)$w['balance'];
        $total_balance += $w['balance'];
    }

    $response_data = [
        "user_id"       => $user_id,
        "total_balance" => $total_balance,
        "wallets"       => $wallets
    ];

    send_json_response("success", "Lấy danh sách ví thành công.", $response_data, 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi truy vấn danh sách ví: " . $e->getMessage(), null, 500);
}
?>
