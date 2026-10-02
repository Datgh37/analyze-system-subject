<?php
/**
 * API: Chuyển tiền Nội bộ giữa 2 Ví (Internal Wallet Transfer)
 * Phương thức: POST (raw JSON)
 * Body: {
 *   "from_wallet_id": 2,
 *   "to_wallet_id": 1,
 *   "amount": 500000,
 *   "note": "Rút tiền ATM về ví tiền mặt"
 * }
 * Bảo mật: Bắt buộc kèm Bearer JWT Token (ACID Transaction)
 * 
 * Tính năng QOL:
 * - Chuyển dịch dòng tiền nội bộ mà không làm sai lệch báo cáo thu - chi
 * - Khóa bi quan (FOR UPDATE) chống Race Condition
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

// 1. Xác thực người dùng
$user_id = get_authenticated_user_id();

$input = get_json_input();
$from_wallet_id = filter_var($input['from_wallet_id'] ?? null, FILTER_VALIDATE_INT);
$to_wallet_id   = filter_var($input['to_wallet_id'] ?? null, FILTER_VALIDATE_INT);
$amount         = isset($input['amount']) ? (float)$input['amount'] : 0.0;
$note           = trim($input['note'] ?? 'Chuyển tiền nội bộ giữa các ví');

// 2. Kiểm tra tính hợp lệ của tham số
if (!$from_wallet_id || !$to_wallet_id) {
    send_json_response("error", "Vui lòng chọn đầy đủ Ví nguồn và Ví đích.", null, 400);
}

if ($from_wallet_id === $to_wallet_id) {
    send_json_response("error", "Ví nguồn và Ví đích không được trùng nhau.", null, 400);
}

if ($amount <= 0) {
    send_json_response("error", "Số tiền chuyển phải lớn hơn 0.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    $db->beginTransaction();

    // 3. Khóa và kiểm tra Ví nguồn (Pessimistic Locking)
    $stmt_from = $db->prepare("
        SELECT id, name, balance 
        FROM `wallets` 
        WHERE `id` = :wid AND `user_id` = :uid 
        FOR UPDATE
    ");
    $stmt_from->bindParam(':wid', $from_wallet_id, PDO::PARAM_INT);
    $stmt_from->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $stmt_from->execute();
    $from_wallet = $stmt_from->fetch(PDO::FETCH_ASSOC);

    if (!$from_wallet) {
        $db->rollBack();
        send_json_response("error", "Ví nguồn không tồn tại hoặc không thuộc quyền sở hữu của bạn.", null, 404);
    }

    $from_balance = (float)$from_wallet['balance'];
    if ($from_balance < $amount) {
        $db->rollBack();
        send_json_response(
            "error", 
            "Số dư ví nguồn không đủ để thực hiện giao dịch (Hiện có: " . number_format($from_balance, 0, ',', '.') . " đ, Cần: " . number_format($amount, 0, ',', '.') . " đ).", 
            null, 
            400
        );
    }

    // 4. Khóa và kiểm tra Ví đích
    $stmt_to = $db->prepare("
        SELECT id, name, balance 
        FROM `wallets` 
        WHERE `id` = :wid AND `user_id` = :uid 
        FOR UPDATE
    ");
    $stmt_to->bindParam(':wid', $to_wallet_id, PDO::PARAM_INT);
    $stmt_to->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $stmt_to->execute();
    $to_wallet = $stmt_to->fetch(PDO::FETCH_ASSOC);

    if (!$to_wallet) {
        $db->rollBack();
        send_json_response("error", "Ví đích không tồn tại hoặc không thuộc quyền sở hữu của bạn.", null, 404);
    }

    // 5. Thực hiện trừ tiền ví nguồn và cộng tiền ví đích
    $update_from = $db->prepare("UPDATE `wallets` SET `balance` = `balance` - :amt WHERE `id` = :wid");
    $update_from->bindParam(':amt', $amount);
    $update_from->bindParam(':wid', $from_wallet_id, PDO::PARAM_INT);
    $update_from->execute();

    $update_to = $db->prepare("UPDATE `wallets` SET `balance` = `balance` + :amt WHERE `id` = :wid");
    $update_to->bindParam(':amt', $amount);
    $update_to->bindParam(':wid', $to_wallet_id, PDO::PARAM_INT);
    $update_to->execute();

    // 6. Lấy số dư mới của cả 2 ví
    $new_from_balance = $from_balance - $amount;
    $new_to_balance   = (float)$to_wallet['balance'] + $amount;

    $db->commit();

    send_json_response("success", "Chuyển tiền giữa các ví thành công!", [
        "amount"        => $amount,
        "note"          => $note,
        "from_wallet"   => [
            "id"          => $from_wallet_id,
            "name"        => $from_wallet['name'],
            "new_balance" => $new_from_balance
        ],
        "to_wallet"     => [
            "id"          => $to_wallet_id,
            "name"        => $to_wallet['name'],
            "new_balance" => $new_to_balance
        ]
    ], 200);

} catch (PDOException $e) {
    if ($db->inTransaction()) {
        $db->rollBack();
    }
    send_json_response("error", "Lỗi CSDL khi thực hiện chuyển tiền: " . $e->getMessage(), null, 500);
}
?>
