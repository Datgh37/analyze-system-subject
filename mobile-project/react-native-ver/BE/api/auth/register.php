<?php
/**
 * API: Đăng ký tài khoản người dùng mới (Register)
 * Phương thức: POST
 * Body: { "full_name": "...", "email": "...", "password": "..." }
 * Tác giả: Trịnh Thành Đạt (Lead)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

// Chỉ chấp nhận method POST
if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng POST.", null, 405);
}

// Đọc payload JSON
$input = get_json_input();
$full_name = trim($input['full_name'] ?? '');
$email     = trim($input['email'] ?? '');
$password  = trim($input['password'] ?? '');

// 1. Kiểm tra tính hợp lệ của dữ liệu (Validation)
if (empty($full_name)) {
    send_json_response("error", "Họ và tên không được để trống.", null, 400);
}

if (empty($email) || !filter_var($email, FILTER_VALIDATE_EMAIL)) {
    send_json_response("error", "Địa chỉ email không hợp lệ.", null, 400);
}

if (empty($password) || strlen($password) < 6) {
    send_json_response("error", "Mật khẩu phải có độ dài tối thiểu từ 6 ký tự.", null, 400);
}

// 2. Kết nối CSDL và xử lý nghiệp vụ
$database = new Database();
$db = $database->getConnection();

try {
    // 2.1. Kiểm tra email đã tồn tại hay chưa
    $check_stmt = $db->prepare("SELECT `id` FROM `users` WHERE `email` = :email LIMIT 1");
    $check_stmt->bindParam(':email', $email);
    $check_stmt->execute();

    if ($check_stmt->rowCount() > 0) {
        send_json_response("error", "Email này đã được đăng ký trong hệ thống. Vui lòng chọn email khác.", null, 409);
    }

    // 2.2. Mã hóa mật khẩu bằng Bcrypt
    $password_hash = password_hash($password, PASSWORD_BCRYPT);

    // 2.3. Bắt đầu Transaction tạo user và 2 ví mặc định
    $db->beginTransaction();

    $insert_user_stmt = $db->prepare("
        INSERT INTO `users` (`full_name`, `email`, `password_hash`, `created_at`) 
        VALUES (:full_name, :email, :password_hash, NOW())
    ");
    $insert_user_stmt->bindParam(':full_name', $full_name);
    $insert_user_stmt->bindParam(':email', $email);
    $insert_user_stmt->bindParam(':password_hash', $password_hash);
    $insert_user_stmt->execute();

    $new_user_id = $db->lastInsertId();

    // 2.4. Tự động tạo 2 ví cơ bản cho người dùng mới (Tiền mặt & Tài khoản ngân hàng)
    $insert_wallet_stmt = $db->prepare("
        INSERT INTO `wallets` (`user_id`, `name`, `balance`, `wallet_type`, `created_at`) VALUES
        (:uid, 'Tiền Mặt', 0.00, 'CASH', NOW()),
        (:uid, 'Tài Khoản Ngân Hàng', 0.00, 'BANK', NOW())
    ");
    $insert_wallet_stmt->bindParam(':uid', $new_user_id);
    $insert_wallet_stmt->execute();

    $db->commit();

    // 2.5. Cấp ngay JWT Token để client có thể tự động đăng nhập liền mạch
    $auth_token = jwt_encode([
        'user_id'   => (int)$new_user_id,
        'email'     => $email,
        'full_name' => $full_name
    ]);

    // 3. Phản hồi thành công
    send_json_response("success", "Đăng ký tài khoản thành công!", [
        "user" => [
            "id"        => (int)$new_user_id,
            "full_name" => $full_name,
            "email"     => $email
        ],
        "token" => $auth_token
    ], 201);

} catch (PDOException $e) {
    if ($db->inTransaction()) {
        $db->rollBack();
    }
    send_json_response("error", "Lỗi máy chủ khi đăng ký tài khoản: " . $e->getMessage(), null, 500);
}
?>
