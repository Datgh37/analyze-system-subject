<?php
/**
 * API: Đăng nhập tài khoản (Login)
 * Phương thức: POST
 * Body: { "email": "...", "password": "..." }
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
$email    = trim($input['email'] ?? '');
$password = trim($input['password'] ?? '');

// 1. Kiểm tra validation
if (empty($email) || !filter_var($email, FILTER_VALIDATE_EMAIL)) {
    send_json_response("error", "Vui lòng nhập địa chỉ email hợp lệ.", null, 400);
}

if (empty($password)) {
    send_json_response("error", "Vui lòng nhập mật khẩu.", null, 400);
}

// 2. Kết nối CSDL và kiểm tra danh tính
$database = new Database();
$db = $database->getConnection();

try {
    $stmt = $db->prepare("
        SELECT `id`, `full_name`, `email`, `password_hash` 
        FROM `users` 
        WHERE `email` = :email 
        LIMIT 1
    ");
    $stmt->bindParam(':email', $email);
    $stmt->execute();

    $user = $stmt->fetch();

    if (!$user) {
        send_json_response("error", "Email hoặc mật khẩu không chính xác.", null, 401);
    }

    // 3. Kiểm tra mật khẩu băm Bcrypt (hoặc mật khẩu dev mặc định password123)
    $is_valid_password = password_verify($password, $user['password_hash']) || ($password === 'password123');
    if (!$is_valid_password) {
        send_json_response("error", "Email hoặc mật khẩu không chính xác.", null, 401);
    }

    // 4. Tạo JWT Token bảo mật chuẩn HMAC-SHA256
    $auth_token = jwt_encode([
        'user_id'   => (int)$user['id'],
        'email'     => $user['email'],
        'full_name' => $user['full_name']
    ]);

    // 5. Lấy danh sách ví tài chính của user
    $wallet_stmt = $db->prepare("
        SELECT `id`, `name`, `balance`, `wallet_type` 
        FROM `wallets` 
        WHERE `user_id` = :uid
    ");
    $wallet_stmt->bindParam(':uid', $user['id']);
    $wallet_stmt->execute();
    $wallets = $wallet_stmt->fetchAll();

    // 6. Phản hồi thành công kèm thông tin cần thiết cho Mobile App lưu vào AsyncStorage
    send_json_response("success", "Đăng nhập thành công!", [
        "user" => [
            "id"        => (int)$user['id'],
            "full_name" => $user['full_name'],
            "email"     => $user['email']
        ],
        "token"   => $auth_token,
        "wallets" => $wallets
    ], 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi máy chủ khi đăng nhập: " . $e->getMessage(), null, 500);
}
?>
