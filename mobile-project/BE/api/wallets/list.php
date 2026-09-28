<?php
/**
 * API: Lấy danh sách Ví và Số dư của Người dùng
 * Phương thức: GET
 * Params: ?user_id={id}
 * Phụ trách: Trần Trọng Đại (UI & Business API) | Boilerplate by Trịnh Thành Đạt
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

$user_id = filter_input(INPUT_GET, 'user_id', FILTER_VALIDATE_INT);

if (!$user_id) {
    send_json_response("error", "Thiếu hoặc sai định dạng tham số user_id.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    $stmt = $db->prepare("
        SELECT id, user_id, wallet_name, wallet_type, balance, currency, icon_name, created_at
        FROM `wallets`
        WHERE `user_id` = :uid
        ORDER BY `id` ASC
    ");
    $stmt->bindParam(':uid', $user_id);
    $stmt->execute();
    $wallets = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Tính tổng tài sản khả dụng (Total Balance)
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
    send_json_response("error", "Lỗi CSDL: " . $e->getMessage(), null, 500);
}
