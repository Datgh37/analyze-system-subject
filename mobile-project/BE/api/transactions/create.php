<?php
/**
 * API: Tạo Giao dịch mới (Thu hoặc Chi)
 * Phương thức: POST (raw JSON)
 * Body: {
 *   "wallet_id": 1,
 *   "category_id": 1,
 *   "amount": 45000,
 *   "transaction_type": "EXPENSE",
 *   "note": "Ăn trưa cơm tấm",
 *   "transaction_date": "2026-09-28"
 * }
 * Bảo mật: Bắt buộc kèm Bearer JWT Token (ACID Transaction)
 * Tác giả: Trần Trọng Đại (UI & Business API) & Trịnh Thành Đạt (Lead)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

// 1. Xác thực người dùng qua JWT
$user_id = get_authenticated_user_id();

$input = get_json_input();

$wallet_id        = filter_var($input['wallet_id'] ?? null, FILTER_VALIDATE_INT);
$category_id      = filter_var($input['category_id'] ?? null, FILTER_VALIDATE_INT);
$amount           = isset($input['amount']) ? (float)$input['amount'] : 0.0;
$transaction_type = strtoupper(trim($input['transaction_type'] ?? ''));
$note             = trim($input['note'] ?? '');
$transaction_date = trim($input['transaction_date'] ?? date('Y-m-d'));

// Kiểm tra tính hợp lệ
if (!$wallet_id || !$category_id || $amount <= 0) {
    send_json_response("error", "Dữ liệu không hợp lệ. Số tiền phải lớn hơn 0 và cần chọn đầy đủ ví, danh mục.", null, 400);
}

if (!in_array($transaction_type, ['EXPENSE', 'INCOME'])) {
    send_json_response("error", "Loại giao dịch phải là EXPENSE hoặc INCOME.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    $db->beginTransaction();

    // 2. Khóa và kiểm tra ví sở hữu bởi chính user
    $check_wallet = $db->prepare("SELECT id, balance FROM `wallets` WHERE `id` = :wid AND `user_id` = :uid FOR UPDATE");
    $check_wallet->bindParam(':wid', $wallet_id, PDO::PARAM_INT);
    $check_wallet->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $check_wallet->execute();
    $wallet = $check_wallet->fetch(PDO::FETCH_ASSOC);

    if (!$wallet) {
        $db->rollBack();
        send_json_response("error", "Ví không tồn tại hoặc không thuộc quyền sở hữu của bạn.", null, 404);
    }

    // 3. Chèn bản ghi giao dịch mới
    $insert_stmt = $db->prepare("
        INSERT INTO `transactions` (`user_id`, `wallet_id`, `category_id`, `amount`, `transaction_type`, `note`, `transaction_date`, `created_at`)
        VALUES (:uid, :wid, :cid, :amt, :type, :note, :tdate, NOW())
    ");
    $insert_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $insert_stmt->bindParam(':wid', $wallet_id, PDO::PARAM_INT);
    $insert_stmt->bindParam(':cid', $category_id, PDO::PARAM_INT);
    $insert_stmt->bindParam(':amt', $amount);
    $insert_stmt->bindParam(':type', $transaction_type);
    $insert_stmt->bindParam(':note', $note);
    $insert_stmt->bindParam(':tdate', $transaction_date);
    $insert_stmt->execute();

    $new_transaction_id = (int)$db->lastInsertId();

    // 4. Tự động cập nhật số dư ví tương ứng
    if ($transaction_type === 'EXPENSE') {
        $update_wallet = $db->prepare("UPDATE `wallets` SET `balance` = `balance` - :amt WHERE `id` = :wid");
    } else {
        $update_wallet = $db->prepare("UPDATE `wallets` SET `balance` = `balance` + :amt WHERE `id` = :wid");
    }
    $update_wallet->bindParam(':amt', $amount);
    $update_wallet->bindParam(':wid', $wallet_id, PDO::PARAM_INT);
    $update_wallet->execute();

    // 5. Lấy số dư mới sau cập nhật
    $get_new_bal = $db->prepare("SELECT balance FROM `wallets` WHERE `id` = :wid");
    $get_new_bal->bindParam(':wid', $wallet_id, PDO::PARAM_INT);
    $get_new_bal->execute();
    $new_balance = (float)$get_new_bal->fetchColumn();

    $db->commit();

    // ⚡ 6. Kích hoạt thông báo biến động số dư Realtime tới Socket.IO Gateway
    $delta = ($transaction_type === 'EXPENSE') ? -$amount : $amount;
    realtime_balance_update($user_id, $wallet_id, $wallet['name'] ?? 'Tiền Mặt', $new_balance, $delta, $note ?: 'Giao dịch mới');

    send_json_response("success", "Tạo giao dịch thành công.", [
        "transaction_id"   => $new_transaction_id,
        "wallet_id"        => $wallet_id,
        "amount"           => $amount,
        "transaction_type" => $transaction_type,
        "note"             => $note,
        "transaction_date" => $transaction_date,
        "updated_balance"  => $new_balance
    ], 201);

} catch (PDOException $e) {
    if ($db->inTransaction()) {
        $db->rollBack();
    }
    send_json_response("error", "Lỗi CSDL khi tạo giao dịch: " . $e->getMessage(), null, 500);
}
?>
