<?php
/**
 * API: Xác thực Mã OTP (Verify OTP - Bước 2)
 * Phương thức: POST
 * Body: { "email": "user@example.com", "otp_code": "123456" }
 * Chức năng:
 * - Kiểm tra tính chính xác và thời hạn của OTP
 * - Nếu hợp lệ: Cấp reset_token tạm thời (64 ký tự hex) có hạn 15 phút
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

$input = get_json_input();
$email    = trim($input['email'] ?? '');
$otp_code = trim($input['otp_code'] ?? '');

if (empty($email) || !filter_var($email, FILTER_VALIDATE_EMAIL)) {
    send_json_response("error", "Vui lòng cung cấp email hợp lệ.", null, 400);
}

if (empty($otp_code) || strlen($otp_code) !== 6) {
    send_json_response("error", "Mã OTP phải gồm 6 chữ số.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    // 1. Tìm bản ghi OTP khớp và còn hạn sử dụng
    $stmt = $db->prepare("
        SELECT id, email, expires_at 
        FROM `password_resets` 
        WHERE `email` = :email 
          AND `otp_code` = :otp 
          AND `expires_at` > NOW() 
        ORDER BY id DESC 
        LIMIT 1
    ");
    $stmt->bindParam(':email', $email);
    $stmt->bindParam(':otp', $otp_code);
    $stmt->execute();
    $reset_record = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$reset_record) {
        send_json_response("error", "Mã OTP không chính xác hoặc đã hết thời gian hiệu lực.", null, 400);
    }

    // 2. Sinh reset_token ngẫu nhiên an toàn mật mã học
    $reset_token = bin2hex(random_bytes(32));

    // 3. Cập nhật reset_token vào bản ghi (Hiệu lực thêm 15 phút tính theo đồng hồ MySQL)
    $update_stmt = $db->prepare("
        UPDATE `password_resets` 
        SET `reset_token` = :token, 
            `expires_at` = DATE_ADD(NOW(), INTERVAL 15 MINUTE) 
        WHERE `id` = :id
    ");
    $update_stmt->bindParam(':token', $reset_token);
    $update_stmt->bindParam(':id', $reset_record['id'], PDO::PARAM_INT);
    $update_stmt->execute();

    // 4. Phản hồi thành công kèm reset_token cho màn hình Đặt lại Mật khẩu
    send_json_response("success", "Xác thực mã OTP thành công. Vui lòng đặt mật khẩu mới.", [
        "email"       => $email,
        "reset_token" => $reset_token
    ], 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi xác thực OTP: " . $e->getMessage(), null, 500);
}
?>
