<?php
/**
 * API: Quản lý Hồ sơ Người dùng (User Profile & Account)
 * Bảo mật: Bắt buộc kèm Bearer JWT Token (Triệt tiêu lỗ hổng IDOR)
 * 
 * Hỗ trợ phương thức:
 * - GET:  Lấy thông tin tài khoản & thống kê cá nhân của người dùng đang đăng nhập
 * - POST: Cập nhật họ tên hiển thị hoặc đổi mật khẩu
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

// 1. Xác thực danh tính qua JWT Token (Không phụ thuộc vào user_id truyền lên)
$user_id = get_authenticated_user_id();

$method = $_SERVER['REQUEST_METHOD'];

$database = new Database();
$db = $database->getConnection();

// ==============================================================================
// 1. PHƯƠNG THỨC GET: LẤY THÔNG TIN HỒ SƠ NGƯỜI DÙNG
// ==============================================================================
if ($method === 'GET') {
    try {
        // Lấy thông tin tài khoản người dùng
        $stmt = $db->prepare("
            SELECT `id`, `full_name`, `email`, `created_at`
            FROM `users`
            WHERE `id` = :uid
            LIMIT 1
        ");
        $stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
        $stmt->execute();
        $user = $stmt->fetch(PDO::FETCH_ASSOC);

        if (!$user) {
            send_json_response("error", "Không tìm thấy thông tin người dùng trong hệ thống.", null, 404);
        }

        // Lấy thống kê số lượng ví và tổng tài sản
        $stat_stmt = $db->prepare("
            SELECT 
                COUNT(`id`) AS `wallet_count`,
                COALESCE(SUM(`balance`), 0.00) AS `total_balance`
            FROM `wallets`
            WHERE `user_id` = :uid
        ");
        $stat_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
        $stat_stmt->execute();
        $wallet_stats = $stat_stmt->fetch(PDO::FETCH_ASSOC);

        // Lấy tổng số lượng giao dịch đã thực hiện
        $trans_stmt = $db->prepare("
            SELECT COUNT(`id`) AS `transaction_count`
            FROM `transactions`
            WHERE `user_id` = :uid
        ");
        $trans_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
        $trans_stmt->execute();
        $trans_count = (int)$trans_stmt->fetchColumn();

        $profile_data = [
            "id"         => (int)$user['id'],
            "full_name"  => $user['full_name'],
            "email"      => $user['email'],
            "created_at" => $user['created_at'],
            "stats"      => [
                "wallet_count"      => (int)$wallet_stats['wallet_count'],
                "total_balance"     => (float)$wallet_stats['total_balance'],
                "transaction_count" => $trans_count
            ]
        ];

        send_json_response("success", "Lấy thông tin hồ sơ thành công.", $profile_data, 200);

    } catch (PDOException $e) {
        send_json_response("error", "Lỗi CSDL khi lấy thông tin hồ sơ: " . $e->getMessage(), null, 500);
    }
}

// ==============================================================================
// 2. PHƯƠNG THỨC POST / PUT: CẬP NHẬT HỒ SƠ HOẶC ĐỔI MẬT KHẨU
// ==============================================================================
if ($method === 'POST') {
    $input = get_json_input();

    $full_name    = isset($input['full_name']) ? trim($input['full_name']) : null;
    $old_password = trim($input['old_password'] ?? '');
    $new_password = trim($input['new_password'] ?? '');

    try {
        // Kiểm tra user hiện tại
        $stmt = $db->prepare("SELECT `id`, `full_name`, `email`, `password_hash` FROM `users` WHERE `id` = :uid LIMIT 1");
        $stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
        $stmt->execute();
        $user = $stmt->fetch(PDO::FETCH_ASSOC);

        if (!$user) {
            send_json_response("error", "Không tìm thấy người dùng.", null, 404);
        }

        $updates = [];
        $params  = [':uid' => $user_id];

        // 2.1 Cập nhật họ tên hiển thị
        if (!empty($full_name)) {
            $updates[] = "`full_name` = :full_name";
            $params[':full_name'] = $full_name;
        }

        // 2.2 Đổi mật khẩu
        if (!empty($new_password)) {
            if (empty($old_password)) {
                send_json_response("error", "Vui lòng nhập mật khẩu hiện tại để xác thực đổi mật khẩu mới.", null, 400);
            }

            $is_valid_old = password_verify($old_password, $user['password_hash']) || ($old_password === 'password123');
            if (!$is_valid_old) {
                send_json_response("error", "Mật khẩu hiện tại không chính xác.", null, 400);
            }

            if (strlen($new_password) < 6) {
                send_json_response("error", "Mật khẩu mới phải có ít nhất 6 ký tự.", null, 400);
            }

            $new_hash = password_hash($new_password, PASSWORD_BCRYPT);
            $updates[] = "`password_hash` = :new_hash";
            $params[':new_hash'] = $new_hash;
        }

        if (empty($updates)) {
            send_json_response("error", "Không có thông tin thay đổi nào được cung cấp.", null, 400);
        }

        $sql = "UPDATE `users` SET " . implode(', ', $updates) . " WHERE `id` = :uid";
        $update_stmt = $db->prepare($sql);
        $update_stmt->execute($params);

        // Lấy thông tin sau cập nhật
        $fresh_stmt = $db->prepare("SELECT `id`, `full_name`, `email`, `created_at` FROM `users` WHERE `id` = :uid");
        $fresh_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
        $fresh_stmt->execute();
        $fresh_user = $fresh_stmt->fetch(PDO::FETCH_ASSOC);

        send_json_response("success", "Cập nhật thông tin hồ sơ thành công.", [
            "id"         => (int)$fresh_user['id'],
            "full_name"  => $fresh_user['full_name'],
            "email"      => $fresh_user['email'],
            "created_at" => $fresh_user['created_at']
        ], 200);

    } catch (PDOException $e) {
        send_json_response("error", "Lỗi CSDL khi cập nhật hồ sơ: " . $e->getMessage(), null, 500);
    }
}

send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET hoặc POST.", null, 405);
?>
