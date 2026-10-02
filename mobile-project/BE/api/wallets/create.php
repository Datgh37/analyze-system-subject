<?php
/**
 * API: Tạo Ví mới cho Người dùng
 * Phương thức: POST (raw JSON)
 * Body: { "wallet_name": "Ví MoMo", "wallet_type": "BANK", "initial_balance": 200000 }
 * Bảo mật: Bắt buộc kèm Bearer JWT Token
 * 
 * Tác giả: Trần Trọng Đại (UI & Business API) & Trịnh Thành Đạt (Lead)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

// Trích xuất user_id từ Token
$user_id = get_authenticated_user_id();

$input = get_json_input();

$wallet_name     = trim($input['wallet_name'] ?? '');
$wallet_type     = strtoupper(trim($input['wallet_type'] ?? 'CASH'));
$initial_balance = isset($input['initial_balance']) ? (float)$input['initial_balance'] : 0.0;

if (empty($wallet_name)) {
    send_json_response("error", "Tên ví không được để trống.", null, 400);
}

$allowed_types = ['CASH', 'BANK'];
if (!in_array($wallet_type, $allowed_types)) {
    $wallet_type = 'CASH';
}

$database = new Database();
$db = $database->getConnection();

try {
    $stmt = $db->prepare("
        INSERT INTO `wallets` (`user_id`, `name`, `wallet_type`, `balance`, `created_at`)
        VALUES (:uid, :wname, :wtype, :bal, NOW())
    ");
    $stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $stmt->bindParam(':wname', $wallet_name);
    $stmt->bindParam(':wtype', $wallet_type);
    $stmt->bindParam(':bal', $initial_balance);
    $stmt->execute();

    $new_id = (int)$db->lastInsertId();

    send_json_response("success", "Tạo ví mới thành công.", [
        "id"           => $new_id,
        "user_id"      => $user_id,
        "wallet_name"  => $wallet_name,
        "name"         => $wallet_name,
        "wallet_type"  => $wallet_type,
        "balance"      => $initial_balance
    ], 201);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi tạo ví mới: " . $e->getMessage(), null, 500);
}
?>
