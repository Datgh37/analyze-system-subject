<?php
/**
 * API: Đặt lại Mật khẩu Mới (Reset Password - Bước 3)
 * Phương thức: POST
 * Body: { "email": "user@example.com", "reset_token": "...", "new_password": "newpassword123" }
 * Chức năng:
 * - Kiểm tra reset_token hợp lệ và còn hạn
 * - Mã hóa mật khẩu mới bằng Bcrypt
 * - Cập nhật bảng users và xóa token đã sử dụng
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

$input = get_json_input();
$email        = trim($input['email'] ?? '');
$reset_token  = trim($input['reset_token'] ?? '');
$new_password = trim($input['new_password'] ?? '');

if (empty($email) || !filter_var($email, FILTER_VALIDATE_EMAIL)) {
    send_json_response("error", "Vui lòng cung cấp email hợp lệ.", null, 400);
}

if (empty($reset_token)) {
    send_json_response("error", "Thiếu mã reset_token xác thực.", null, 400);
}

if (empty($new_password) || strlen($new_password) < 6) {
    send_json_response("error", "Mật khẩu mới phải có tối thiểu từ 6 ký tự.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    // 1. Kiểm tra reset_token có hợp lệ và còn hạn không
    $stmt = $db->prepare("
        SELECT id, email 
        FROM `password_resets` 
        WHERE `email` = :email 
          AND `reset_token` = :token 
          AND `expires_at` > NOW() 
        LIMIT 1
    ");
    $stmt->bindParam(':email', $email);
    $stmt->bindParam(':token', $reset_token);
    $stmt->execute();
    $record = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$record) {
        send_json_response("error", "Mã xác thực đặt lại mật khẩu không hợp lệ hoặc đã hết hạn. Vui lòng thử lại từ bước đầu.", null, 400);
    }

    // 2. Băm mật khẩu mới an toàn bằng Bcrypt
    $password_hash = password_hash($new_password, PASSWORD_BCRYPT);

    $db->beginTransaction();

    // 3. Cập nhật mật khẩu mới cho người dùng
    $update_user = $db->prepare("UPDATE `users` SET `password_hash` = :hash WHERE `email` = :email");
    $update_user->bindParam(':hash', $password_hash);
    $update_user->bindParam(':email', $email);
    $update_user->execute();

    // 4. Xóa bản ghi reset_token đã sử dụng thành công (One-time token)
    $delete_token = $db->prepare("DELETE FROM `password_resets` WHERE `id` = :id");
    $delete_token->bindParam(':id', $record['id'], PDO::PARAM_INT);
    $delete_token->execute();

    $db->commit();

    send_json_response(
        "success", 
        "Đặt lại mật khẩu mới thành công! Bạn có thể dùng mật khẩu mới để đăng nhập ngay bây giờ.", 
        [ "email" => $email ], 
        200
    );

} catch (PDOException $e) {
    if ($db->inTransaction()) {
        $db->rollBack();
    }
    send_json_response("error", "Lỗi CSDL khi đặt lại mật khẩu: " . $e->getMessage(), null, 500);
}
?>
