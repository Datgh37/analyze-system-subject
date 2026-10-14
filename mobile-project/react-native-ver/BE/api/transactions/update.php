<?php
/**
 * API: Cập nhật Giao dịch và Tự động Bù trừ Số dư Ví (Delta Balance Reconciler)
 * Phương thức: POST (raw JSON)
 * Body: {
 *   "id": 5,
 *   "wallet_id": 1,
 *   "category_id": 2,
 *   "amount": 450000,
 *   "transaction_type": "EXPENSE",
 *   "note": "Tiền ăn trưa (Sửa lại số tiền đúng)",
 *   "transaction_date": "2026-09-28"
 * }
 * Bảo mật: Bắt buộc kèm Bearer JWT Token (ACID Transaction)
 * 
 * Tính năng QOL #2:
 * - Tự động tính chênh lệch Delta (new_amount - old_amount) và điều chỉnh số dư ví
 * - Xử lý chuyển đổi ví nguồn nếu người dùng chọn nhầm ví ban đầu
 * - Khóa bi quan FOR UPDATE chống xung đột đồng thời
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

// 1. Xác thực danh tính từ Bearer Token
$user_id = get_authenticated_user_id();

$input = get_json_input();
$transaction_id = filter_var($input['id'] ?? $input['transaction_id'] ?? null, FILTER_VALIDATE_INT);

if (!$transaction_id) {
    send_json_response("error", "Thiếu mã giao dịch (id) cần cập nhật.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    $db->beginTransaction();

    // 2. Khóa và lấy thông tin giao dịch hiện tại của user
    $find_stmt = $db->prepare("
        SELECT id, user_id, wallet_id, category_id, amount, transaction_type, note, transaction_date 
        FROM `transactions` 
        WHERE `id` = :tid AND `user_id` = :uid 
        FOR UPDATE
    ");
    $find_stmt->bindParam(':tid', $transaction_id, PDO::PARAM_INT);
    $find_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $find_stmt->execute();
    $old_trans = $find_stmt->fetch(PDO::FETCH_ASSOC);

    if (!$old_trans) {
        $db->rollBack();
        send_json_response("error", "Giao dịch không tồn tại hoặc không thuộc quyền sở hữu của bạn.", null, 404);
    }

    $old_wallet_id = (int)$old_trans['wallet_id'];
    $old_amount    = (float)$old_trans['amount'];
    $old_type      = $old_trans['transaction_type'];

    // 3. Chuẩn bị dữ liệu cập nhật
    $new_wallet_id = isset($input['wallet_id']) ? (int)$input['wallet_id'] : $old_wallet_id;
    $new_amount    = isset($input['amount']) ? (float)$input['amount'] : $old_amount;
    $new_type      = isset($input['transaction_type']) ? strtoupper(trim($input['transaction_type'])) : $old_type;
    $category_id   = isset($input['category_id']) ? (int)$input['category_id'] : (int)$old_trans['category_id'];
    $note          = isset($input['note']) ? trim($input['note']) : $old_trans['note'];
    $t_date        = isset($input['transaction_date']) ? trim($input['transaction_date']) : $old_trans['transaction_date'];

    if ($new_amount <= 0) {
        $db->rollBack();
        send_json_response("error", "Số tiền giao dịch mới phải lớn hơn 0.", null, 400);
    }

    if (!in_array($new_type, ['EXPENSE', 'INCOME'])) {
        $db->rollBack();
        send_json_response("error", "Loại giao dịch phải là EXPENSE hoặc INCOME.", null, 400);
    }

    // 4. Bù trừ số dư ví (Delta Balance Reconciler)
    if ($old_wallet_id === $new_wallet_id && $old_type === $new_type) {
        // Trường hợp 4.1: Cùng ví và cùng loại thu/chi -> Chỉ cần bù trừ Delta chênh lệch
        $delta = $new_amount - $old_amount;
        if ($delta != 0) {
            if ($new_type === 'EXPENSE') {
                // Chi tiêu tăng -> ví bị trừ thêm delta; chi tiêu giảm -> ví được cộng lại delta
                $update_wallet = $db->prepare("UPDATE `wallets` SET `balance` = `balance` - :delta WHERE `id` = :wid");
            } else {
                // Thu nhập tăng -> ví được cộng thêm delta; thu nhập giảm -> ví bị trừ bớt delta
                $update_wallet = $db->prepare("UPDATE `wallets` SET `balance` = `balance` + :delta WHERE `id` = :wid");
            }
            $update_wallet->bindParam(':delta', $delta);
            $update_wallet->bindParam(':wid', $old_wallet_id, PDO::PARAM_INT);
            $update_wallet->execute();
        }
    } else {
        // Trường hợp 4.2: Đổi ví HOẶC Đổi loại giao dịch (Thu <-> Chi)
        // Bước A: Hoàn trả trạng thái của giao dịch cũ tại ví cũ
        if ($old_type === 'EXPENSE') {
            $rollback_old = $db->prepare("UPDATE `wallets` SET `balance` = `balance` + :amt WHERE `id` = :wid");
        } else {
            $rollback_old = $db->prepare("UPDATE `wallets` SET `balance` = `balance` - :amt WHERE `id` = :wid");
        }
        $rollback_old->bindParam(':amt', $old_amount);
        $rollback_old->bindParam(':wid', $old_wallet_id, PDO::PARAM_INT);
        $rollback_old->execute();

        // Bước B: Kiểm tra quyền sở hữu của ví mới
        $check_new_wallet = $db->prepare("SELECT id, balance FROM `wallets` WHERE `id` = :wid AND `user_id` = :uid FOR UPDATE");
        $check_new_wallet->bindParam(':wid', $new_wallet_id, PDO::PARAM_INT);
        $check_new_wallet->bindParam(':uid', $user_id, PDO::PARAM_INT);
        $check_new_wallet->execute();
        $target_wallet = $check_new_wallet->fetch(PDO::FETCH_ASSOC);

        if (!$target_wallet) {
            $db->rollBack();
            send_json_response("error", "Ví mới được chọn không tồn tại hoặc không thuộc quyền sở hữu của bạn.", null, 404);
        }

        // Bước C: Tác động giao dịch mới vào ví mới
        if ($new_type === 'EXPENSE') {
            $apply_new = $db->prepare("UPDATE `wallets` SET `balance` = `balance` - :amt WHERE `id` = :wid");
        } else {
            $apply_new = $db->prepare("UPDATE `wallets` SET `balance` = `balance` + :amt WHERE `id` = :wid");
        }
        $apply_new->bindParam(':amt', $new_amount);
        $apply_new->bindParam(':wid', $new_wallet_id, PDO::PARAM_INT);
        $apply_new->execute();
    }

    // 5. Cập nhật bản ghi giao dịch
    $update_trans = $db->prepare("
        UPDATE `transactions` 
        SET `wallet_id`        = :wid, 
            `category_id`      = :cid, 
            `amount`           = :amt, 
            `transaction_type` = :type, 
            `note`             = :note, 
            `transaction_date` = :tdate 
        WHERE `id` = :tid AND `user_id` = :uid
    ");
    $update_trans->bindParam(':wid', $new_wallet_id, PDO::PARAM_INT);
    $update_trans->bindParam(':cid', $category_id, PDO::PARAM_INT);
    $update_trans->bindParam(':amt', $new_amount);
    $update_trans->bindParam(':type', $new_type);
    $update_trans->bindParam(':note', $note);
    $update_trans->bindParam(':tdate', $t_date);
    $update_trans->bindParam(':tid', $transaction_id, PDO::PARAM_INT);
    $update_trans->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $update_trans->execute();

    // 6. Lấy số dư mới của ví sau bù trừ
    $bal_stmt = $db->prepare("SELECT balance FROM `wallets` WHERE `id` = :wid");
    $bal_stmt->bindParam(':wid', $new_wallet_id, PDO::PARAM_INT);
    $bal_stmt->execute();
    $new_wallet_balance = (float)$bal_stmt->fetchColumn();

    $db->commit();

    send_json_response("success", "Cập nhật giao dịch và tự động bù trừ số dư ví thành công!", [
        "id"                 => $transaction_id,
        "wallet_id"          => $new_wallet_id,
        "category_id"        => $category_id,
        "amount"             => $new_amount,
        "transaction_type"   => $new_type,
        "note"               => $note,
        "transaction_date"   => $t_date,
        "new_wallet_balance" => $new_wallet_balance
    ], 200);

} catch (PDOException $e) {
    if ($db->inTransaction()) {
        $db->rollBack();
    }
    send_json_response("error", "Lỗi CSDL khi cập nhật giao dịch: " . $e->getMessage(), null, 500);
}
?>
