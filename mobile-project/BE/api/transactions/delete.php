<?php
/**
 * API: Xóa Giao dịch và Hoàn trả Số dư Ví
 * Phương thức: POST hoặc DELETE
 * Body: { "user_id": 1, "transaction_id": 5 } hoặc Params ?id=5&user_id=1
 * Phụ trách: Trần Trọng Đại (UI & Business API) | Boilerplate by Trịnh Thành Đạt
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

$method = $_SERVER['REQUEST_METHOD'];
if ($method !== 'DELETE' && $method !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST hoặc DELETE.", null, 405);
}

// Hỗ trợ cả Query Params và JSON Body
$input = get_json_input();
$transaction_id = filter_var($input['transaction_id'] ?? $_GET['id'] ?? null, FILTER_VALIDATE_INT);
$user_id        = filter_var($input['user_id'] ?? $_GET['user_id'] ?? null, FILTER_VALIDATE_INT);

if (!$transaction_id || !$user_id) {
    send_json_response("error", "Thiếu tham số transaction_id hoặc user_id.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    $db->beginTransaction();

    // 1. Kiểm tra giao dịch có tồn tại và thuộc về user không
    $stmt = $db->prepare("
        SELECT id, user_id, wallet_id, amount, transaction_type 
        FROM `transactions` 
        WHERE `id` = :tid AND `user_id` = :uid 
        FOR UPDATE
    ");
    $stmt->bindParam(':tid', $transaction_id);
    $stmt->bindParam(':uid', $user_id);
    $stmt->execute();
    $trans = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$trans) {
        $db->rollBack();
        send_json_response("error", "Giao dịch không tồn tại hoặc không thuộc quyền sở hữu của bạn.", null, 404);
    }

    $wallet_id = (int)$trans['wallet_id'];
    $amount    = (float)$trans['amount'];
    $type      = $trans['transaction_type'];

    // 2. Hoàn lại số dư ví tương ứng (Đảo ngược tác động ban đầu)
    if ($type === 'EXPENSE') {
        // Giao dịch chi tiêu bị xóa -> cộng lại tiền vào ví
        $update_wallet = $db->prepare("UPDATE `wallets` SET `balance` = `balance` + :amt WHERE `id` = :wid");
    } else {
        // Giao dịch thu nhập bị xóa -> trừ lại tiền khỏi ví
        $update_wallet = $db->prepare("UPDATE `wallets` SET `balance` = `balance` - :amt WHERE `id` = :wid");
    }
    $update_wallet->bindParam(':amt', $amount);
    $update_wallet->bindParam(':wid', $wallet_id);
    $update_wallet->execute();

    // 3. Xóa giao dịch khỏi bảng transactions
    $delete_stmt = $db->prepare("DELETE FROM `transactions` WHERE `id` = :tid");
    $delete_stmt->bindParam(':tid', $transaction_id);
    $delete_stmt->execute();

    // 4. Lấy số dư mới sau khi hoàn tiền
    $get_new_bal = $db->prepare("SELECT balance FROM `wallets` WHERE `id` = :wid");
    $get_new_bal->bindParam(':wid', $wallet_id);
    $get_new_bal->execute();
    $new_balance = (float)$get_new_bal->fetchColumn();

    $db->commit();

    send_json_response("success", "Xóa giao dịch và hoàn trả số dư ví thành công.", [
        "deleted_transaction_id" => $transaction_id,
        "wallet_id"              => $wallet_id,
        "updated_balance"        => $new_balance
    ], 200);

} catch (PDOException $e) {
    if ($db->inTransaction()) {
        $db->rollBack();
    }
    send_json_response("error", "Lỗi CSDL khi xóa giao dịch: " . $e->getMessage(), null, 500);
}
