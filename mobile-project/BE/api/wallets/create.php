<?php
/**
 * API: Tạo Ví mới cho Người dùng
 * Phương thức: POST (raw JSON)
 * Body: { "user_id": 1, "wallet_name": "Ví MoMo", "wallet_type": "E_WALLET", "initial_balance": 200000 }
 * Phụ trách: Trần Trọng Đại (UI & Business API) | Boilerplate by Trịnh Thành Đạt
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

$input = get_json_input();

$user_id         = filter_var($input['user_id'] ?? null, FILTER_VALIDATE_INT);
$wallet_name     = trim($input['wallet_name'] ?? '');
$wallet_type     = strtoupper(trim($input['wallet_type'] ?? 'OTHER'));
$initial_balance = isset($input['initial_balance']) ? (float)$input['initial_balance'] : 0.0;
$icon_name       = trim($input['icon_name'] ?? 'wallet');

if (!$user_id || empty($wallet_name)) {
    send_json_response("error", "Thiếu thông tin bắt buộc: user_id hoặc wallet_name.", null, 400);
}

$allowed_types = ['CASH', 'BANK', 'E_WALLET', 'OTHER'];
if (!in_array($wallet_type, $allowed_types)) {
    $wallet_type = 'OTHER';
}

$database = new Database();
$db = $database->getConnection();

try {
    $stmt = $db->prepare("
        INSERT INTO `wallets` (`user_id`, `wallet_name`, `wallet_type`, `balance`, `currency`, `icon_name`)
        VALUES (:uid, :wname, :wtype, :bal, 'VND', :icon)
    ");
    $stmt->bindParam(':uid', $user_id);
    $stmt->bindParam(':wname', $wallet_name);
    $stmt->bindParam(':wtype', $wallet_type);
    $stmt->bindParam(':bal', $initial_balance);
    $stmt->bindParam(':icon', $icon_name);
    $stmt->execute();

    $new_id = (int)$db->lastInsertId();

    send_json_response("success", "Tạo ví mới thành công.", [
        "id"           => $new_id,
        "user_id"      => $user_id,
        "wallet_name"  => $wallet_name,
        "wallet_type"  => $wallet_type,
        "balance"      => $initial_balance
    ], 201);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL: " . $e->getMessage(), null, 500);
}
