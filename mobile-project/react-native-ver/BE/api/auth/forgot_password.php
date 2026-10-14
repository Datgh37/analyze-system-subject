<?php
/**
 * API: Yêu cầu Đặt lại Mật khẩu (Forgot Password - Bước 1)
 * Phương thức: POST
 * Body: { "email": "user@example.com" }
 * Chức năng:
 * - Kiểm tra sự tồn tại của Email trong bảng users
 * - Sinh mã OTP ngẫu nhiên 6 chữ số (Thời hạn 15 phút)
 * - Lưu OTP vào bảng password_resets
 * - Trả về debug_otp khi ở môi trường Localhost WAMP để dễ dàng kiểm thử không phụ thuộc SMTP
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

$input = get_json_input();
$email = trim($input['email'] ?? '');

if (empty($email) || !filter_var($email, FILTER_VALIDATE_EMAIL)) {
    send_json_response("error", "Vui lòng nhập địa chỉ email hợp lệ.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    // 1. Kiểm tra email có tồn tại trong hệ thống hay không
    $stmt = $db->prepare("SELECT id, full_name, email FROM `users` WHERE `email` = :email LIMIT 1");
    $stmt->bindParam(':email', $email);
    $stmt->execute();
    $user = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$user) {
        send_json_response("error", "Không tìm thấy tài khoản nào gắn với email này.", null, 404);
    }

    // 2. Sinh mã OTP 6 chữ số bảo mật cao
    $otp_code = sprintf("%06d", random_int(100000, 999999));
    $expires_at = date('Y-m-d H:i:s', time() + (15 * 60)); // Hiệu lực 15 phút

    // 3. Xóa các mã OTP cũ chưa sử dụng của email này để tránh dư thừa
    $delete_stmt = $db->prepare("DELETE FROM `password_resets` WHERE `email` = :email");
    $delete_stmt->bindParam(':email', $email);
    $delete_stmt->execute();

    // 4. Lưu OTP mới vào bảng password_resets (Dùng DATE_ADD(NOW(), INTERVAL 15 MINUTE) để đồng bộ tuyệt đối với đồng hồ MySQL)
    $insert_stmt = $db->prepare("
        INSERT INTO `password_resets` (`email`, `otp_code`, `expires_at`, `created_at`) 
        VALUES (:email, :otp, DATE_ADD(NOW(), INTERVAL 15 MINUTE), NOW())
    ");
    $insert_stmt->bindParam(':email', $email);
    $insert_stmt->bindParam(':otp', $otp_code);
    $insert_stmt->execute();

    // 5. Chuẩn bị dữ liệu phản hồi
    $response_data = [
        "email"      => $email,
        "expires_in" => "15 phút"
    ];

    // Chế độ Localhost: Kèm theo debug_otp giúp Dev/Tester test trơn tru không lo kẹt SMTP
    if ($database->isLocal()) {
        $response_data["debug_otp"] = $otp_code;
    }

    send_json_response(
        "success", 
        "Mã xác thực OTP đã được tạo thành công. Vui lòng kiểm tra và nhập mã để tiếp tục.", 
        $response_data, 
        200
    );

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi xử lý yêu cầu quên mật khẩu: " . $e->getMessage(), null, 500);
}
?>
