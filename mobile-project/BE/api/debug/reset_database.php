<?php
/**
 * API NỘI BỘ: Khôi phục và Dọn dẹp Cơ sở Dữ liệu Mẫu (Database Hard Reset & Seed)
 * 
 * MỤC ĐÍCH: CHỈ DÀNH CHO DEBUG / TESTING NỘI BỘ (KHÔNG XUẤT HIỆN TRÊN GIAO DIỆN MOBILE APP)
 * 
 * PHƯƠNG THỨC: GET hoặc POST
 * ENDPOINT: /api/debug/reset_database.php
 * 
 * CHỨC NĂNG:
 * - Xóa thật (Hard Delete / TRUNCATE) toàn bộ dữ liệu phát sinh khi chạy test
 * - Đưa Auto-Increment ID của tất cả bảng về lại 1
 * - Tái tạo nguyên trạng dữ liệu mẫu chuẩn:
 *   + 2 Người dùng: Trịnh Thành Đạt (Lead) & Trần Trọng Đại (mật khẩu: password123)
 *   + 4 Ví ban đầu: Tiền mặt, Vietcombank, MB Bank với số dư gốc
 *   + 10 Danh mục hệ thống mặc định (7 Chi tiêu, 3 Thu nhập)
 *   + 7 Giao dịch mẫu tháng hiện tại
 *   + 2 Ngân sách mẫu tháng hiện tại (Tổng chi 6 triệu & Ăn uống 2.5 triệu)
 *   + Dọn sạch bảng password_resets
 * - Khóa an toàn: Chặn 100% nếu chạy trên Production Hosting của trường cấp
 * 
 * Tác giả: Trịnh Thành Đạt (Lead)
 * Đồ án: Quản lý Chi tiêu Cá nhân Thông minh - Đại học Công nghệ Đồng Nai (DNTU)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

$database = new Database();

// BẢO MẬT: Tuyệt đối chỉ cho phép chạy trên WAMP Server Localhost
if (!$database->isLocal()) {
    send_json_response("error", "API này bị khóa trên máy chủ Production nhằm bảo vệ toàn vẹn dữ liệu.", null, 403);
}

$db = $database->getConnection();

try {
    // 1. Tắt tạm thời kiểm tra khóa ngoại
    $db->exec("SET FOREIGN_KEY_CHECKS = 0;");

    // 2. Xóa thật (Hard Delete) và đưa ID tự tăng về 1
    $tables = ['password_resets', 'transactions', 'budgets', 'categories', 'wallets', 'users'];
    foreach ($tables as $table) {
        $db->exec("TRUNCATE TABLE `$table`;");
    }

    // 3. Tái tạo 2 người dùng ban đầu (Mật khẩu: password123)
    $pwdHash = password_hash('password123', PASSWORD_BCRYPT);
    $quotedHash = $db->quote($pwdHash);
    $db->exec("
        INSERT INTO `users` (`id`, `full_name`, `email`, `password_hash`, `created_at`) VALUES
        (1, 'Trịnh Thành Đạt', 'thanhdat@gmail.com', $quotedHash, NOW()),
        (2, 'Trần Trọng Đại', 'trongdai@gmail.com', $quotedHash, NOW());
    ");

    // 4. Tái tạo 4 ví tài chính mẫu với số dư gốc
    $db->exec("
        INSERT INTO `wallets` (`id`, `user_id`, `name`, `balance`, `wallet_type`, `created_at`) VALUES
        (1, 1, 'Tiền Mặt Trong Ví', 2500000.00, 'CASH', NOW()),
        (2, 1, 'Tài Khoản Vietcombank', 18500000.00, 'BANK', NOW()),
        (3, 2, 'Tiền Mặt', 1200000.00, 'CASH', NOW()),
        (4, 2, 'Ngân Hàng MB Bank', 8500000.00, 'BANK', NOW());
    ");

    // 5. Tái tạo 10 danh mục mặc định của hệ thống (user_id = NULL)
    $db->exec("
        INSERT INTO `categories` (`id`, `user_id`, `name`, `type`, `icon_name`, `color_code`) VALUES
        (1, NULL, 'Ăn uống & Cà phê', 'EXPENSE', 'fast-food-outline', '#FF6B6B'),
        (2, NULL, 'Mua sắm & Đồ dùng', 'EXPENSE', 'cart-outline', '#4ECDC4'),
        (3, NULL, 'Di chuyển & Xăng xe', 'EXPENSE', 'car-sport-outline', '#FFE66D'),
        (4, NULL, 'Hóa đơn & Điện nước', 'EXPENSE', 'receipt-outline', '#1A535C'),
        (5, NULL, 'Nhà ở & Tiền phòng', 'EXPENSE', 'home-outline', '#4D96FF'),
        (6, NULL, 'Giáo dục & Sách vở', 'EXPENSE', 'book-outline', '#9B5DE5'),
        (7, NULL, 'Sức khỏe & Thuốc men', 'EXPENSE', 'medkit-outline', '#F15BB5'),
        (8, NULL, 'Lương hàng tháng', 'INCOME', 'cash-outline', '#2EC4B6'),
        (9, NULL, 'Thưởng & Làm thêm', 'INCOME', 'trending-up-outline', '#38B000'),
        (10, NULL, 'Thu nhập khác', 'INCOME', 'wallet-outline', '#70E000');
    ");

    // 6. Tái tạo 7 giao dịch mẫu theo tháng hiện tại cho user 1
    $currentMonth = date('Y-m');
    $db->exec("
        INSERT INTO `transactions` (`id`, `user_id`, `wallet_id`, `category_id`, `amount`, `transaction_type`, `note`, `transaction_date`, `created_at`) VALUES
        (1, 1, 2, 8, 15000000.00, 'INCOME', 'Nhận lương tháng qua chuyển khoản Vietcombank', '$currentMonth-05', NOW()),
        (2, 1, 1, 1, 55000.00, 'EXPENSE', 'Ăn trưa cơm văn phòng', '$currentMonth-20', NOW()),
        (3, 1, 1, 1, 35000.00, 'EXPENSE', 'Cà phê sáng cùng đồng nghiệp', '$currentMonth-21', NOW()),
        (4, 1, 1, 3, 90000.00, 'EXPENSE', 'Đổ xăng xe máy Petrolimex', '$currentMonth-22', NOW()),
        (5, 1, 2, 2, 650000.00, 'EXPENSE', 'Mua sắm nhu yếu phẩm siêu thị Co.opmart', '$currentMonth-25', NOW()),
        (6, 1, 2, 4, 380000.00, 'EXPENSE', 'Thanh toán tiền điện và nước sinh hoạt', '$currentMonth-27', NOW()),
        (7, 1, 1, 9, 500000.00, 'INCOME', 'Tiền thưởng dự án nhỏ', '$currentMonth-28', NOW());
    ");

    // 7. Tái tạo ngân sách mẫu tháng hiện tại cho user 1
    $monthNum = (int)date('n');
    $yearNum  = (int)date('Y');
    $db->exec("
        INSERT INTO `budgets` (`id`, `user_id`, `category_id`, `amount_limit`, `month`, `year`) VALUES
        (1, 1, NULL, 6000000.00, $monthNum, $yearNum),
        (2, 1, 1, 2500000.00, $monthNum, $yearNum);
    ");

    // 8. Bật lại kiểm tra khóa ngoại
    $db->exec("SET FOREIGN_KEY_CHECKS = 1;");

    send_json_response(
        "success", 
        "Đã dọn dẹp và khôi phục CSDL về trạng thái sạch ban đầu thành công (Hard Reset & Seed Completed)!", 
        [
            "environment"        => "Localhost (WAMP)",
            "tables_truncated"   => $tables,
            "sample_users"       => 2,
            "sample_wallets"     => 4,
            "default_categories" => 10,
            "sample_transactions"=> 7,
            "sample_budgets"     => 2,
            "password_resets"    => 0,
            "applied_month"      => "$monthNum/$yearNum"
        ], 
        200
    );

} catch (PDOException $e) {
    $db->exec("SET FOREIGN_KEY_CHECKS = 1;");
    send_json_response("error", "Lỗi CSDL khi reset dữ liệu: " . $e->getMessage(), null, 500);
}
