<?php
/**
 * API: Tạo Giao dịch mới (Thu hoặc Chi)
 * Phương thức: POST (raw JSON)
 * Body: {
 *   "user_id": 1,
 *   "wallet_id": 1,
 *   "category_id": 1,
 *   "amount": 45000,
 *   "transaction_type": "EXPENSE",
 *   "note": "Ăn trưa cơm tấm",
 *   "transaction_date": "2026-09-28"
 * }
 * Phụ trách: Trần Trọng Đại (UI & Business API) | Boilerplate by Trịnh Thành Đạt
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

$input = get_json_input();

$user_id          = filter_var($input['user_id'] ?? null, FILTER_VALIDATE_INT);
$wallet_id        = filter_var($input['wallet_id'] ?? null, FILTER_VALIDATE_INT);
$category_id      = filter_var($input['category_id'] ?? null, FILTER_VALIDATE_INT);
$amount           = isset($input['amount']) ? (float)$input['amount'] : 0.0;
$transaction_type = strtoupper(trim($input['transaction_type'] ?? ''));
$note             = trim($input['note'] ?? '');
$transaction_date = trim($input['transaction_date'] ?? date('Y-m-d'));

// Kiểm tra tính hợp lệ của dữ liệu đầu vào
if (!$user_id || !$wallet_id || !$category_id || $amount <= 0) {
    send_json_response("error", "Dữ liệu không hợp lệ. Số tiền phải lớn hơn 0 và cần chọn ví, danh mục.", null, 400);
}

if (!in_array($transaction_type, ['EXPENSE', 'INCOME'])) {
    send_json_response("error", "Loại giao dịch phải là EXPENSE hoặc INCOME.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    // Sử dụng Database Transaction để đảm bảo tính toàn vẹn (ACID)
    $db->beginTransaction();

    // 1. Kiểm tra ví có tồn tại và thuộc về user không
    $check_wallet = $db->prepare("SELECT id, balance FROM `wallets` WHERE `id` = :wid AND `user_id` = :uid FOR UPDATE");
    $check_wallet->bindParam(':wid', $wallet_id);
    $check_wallet->bindParam(':uid', $user_id);
    $check_wallet->execute();
    $wallet = $check_wallet->fetch(PDO::FETCH_ASSOC);

    if (!$wallet) {
        $db->rollBack();
        send_json_response("error", "Ví không tồn tại hoặc không thuộc quyền sở hữu của bạn.", null, 404);
    }

    // 2. Chèn bản ghi giao dịch mới
    $insert_stmt = $db->prepare("
        INSERT INTO `transactions` (`user_id`, `wallet_id`, `category_id`, `amount`, `transaction_type`, `note`, `transaction_date`)
        VALUES (:uid, :wid, :cid, :amt, :type, :note, :tdate)
    ");
    $insert_stmt->bindParam(':uid', $user_id);
    $insert_stmt->bindParam(':wid', $wallet_id);
    $insert_stmt->bindParam(':cid', $category_id);
    $insert_stmt->bindParam(':amt', $amount);
    $insert_stmt->bindParam(':type', $transaction_type);
    $insert_stmt->bindParam(':note', $note);
    $insert_stmt->bindParam(':tdate', $transaction_date);
    $insert_stmt->execute();

    $new_transaction_id = (int)$db->lastInsertId();

    // 3. Tự động cập nhật số dư ví tương ứng
    if ($transaction_type === 'EXPENSE') {
        $update_wallet = $db->prepare("UPDATE `wallets` SET `balance` = `balance` - :amt WHERE `id` = :wid");
    } else {
        $update_wallet = $db->prepare("UPDATE `wallets` SET `balance` = `balance` + :amt WHERE `id` = :wid");
    }
    $update_wallet->bindParam(':amt', $amount);
    $update_wallet->bindParam(':wid', $wallet_id);
    $update_wallet->execute();

    // 4. Lấy số dư mới sau cập nhật
    $get_new_bal = $db->prepare("SELECT balance FROM `wallets` WHERE `id` = :wid");
    $get_new_bal->bindParam(':wid', $wallet_id);
    $get_new_bal->execute();
    $new_balance = (float)$get_new_bal->fetchColumn();

    // Commit Transaction
    $db->commit();

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
