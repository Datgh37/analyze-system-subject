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

    // 3. Tái tạo 2 người dùng giả lập thực tế (Mật khẩu: password123)
    $pwdHash = password_hash('password123', PASSWORD_BCRYPT);
    $quotedHash = $db->quote($pwdHash);
    $db->exec("
        INSERT INTO `users` (`id`, `full_name`, `email`, `password_hash`, `created_at`) VALUES
        (1, 'Nguyễn Văn An', 'nguyenvanan@gmail.com', $quotedHash, NOW()),
        (2, 'Trần Thị Mai', 'tranmai@gmail.com', $quotedHash, NOW());
    ");

    // 4. Tái tạo 4 ví tài chính mẫu với số dư thực tế sau tích lũy dòng tiền
    $db->exec("
        INSERT INTO `wallets` (`id`, `user_id`, `name`, `balance`, `wallet_type`, `created_at`) VALUES
        (1, 1, 'Ví Tiền Mặt', 3850000.00, 'CASH', NOW()),
        (2, 1, 'Tài Khoản Vietcombank', 36450000.00, 'BANK', NOW()),
        (3, 1, 'Ví Điện Tử MoMo', 2850000.00, 'CASH', NOW()),
        (4, 1, 'Sổ Tiết Kiệm Dự Phòng', 25000000.00, 'BANK', NOW()),
        (5, 2, 'Tiền Mặt', 2000000.00, 'CASH', NOW()),
        (6, 2, 'Tài Khoản MB Bank', 12500000.00, 'BANK', NOW());
    ");

    // 5. Tái tạo 13 danh mục hệ thống đa dạng (9 Chi tiêu, 4 Thu nhập)
    $db->exec("
        INSERT INTO `categories` (`id`, `user_id`, `name`, `type`, `icon_name`, `color_code`) VALUES
        -- Phân nhóm CHI TIÊU (EXPENSE)
        (1, NULL, 'Ăn uống & Cà phê', 'EXPENSE', 'fast-food-outline', '#FF6B6B'),
        (2, NULL, 'Mua sắm & Đồ dùng', 'EXPENSE', 'cart-outline', '#4ECDC4'),
        (3, NULL, 'Di chuyển & Xăng xe', 'EXPENSE', 'car-sport-outline', '#FFE66D'),
        (4, NULL, 'Hóa đơn & Điện nước', 'EXPENSE', 'receipt-outline', '#1A535C'),
        (5, NULL, 'Nhà ở & Tiền phòng', 'EXPENSE', 'home-outline', '#4D96FF'),
        (6, NULL, 'Giáo dục & Sách vở', 'EXPENSE', 'book-outline', '#9B5DE5'),
        (7, NULL, 'Sức khỏe & Thuốc men', 'EXPENSE', 'medkit-outline', '#F15BB5'),
        (11, NULL, 'Giải trí & Dịch vụ', 'EXPENSE', 'film-outline', '#FF9F1C'),
        (12, NULL, 'Thể thao & Thể hình', 'EXPENSE', 'barbell-outline', '#06D6A0'),
        -- Phân nhóm THU NHẬP (INCOME)
        (8, NULL, 'Lương hàng tháng', 'INCOME', 'cash-outline', '#2EC4B6'),
        (9, NULL, 'Thưởng & Làm thêm', 'INCOME', 'trending-up-outline', '#38B000'),
        (10, NULL, 'Thu nhập làm thêm (Freelance)', 'INCOME', 'laptop-outline', '#70E000'),
        (13, NULL, 'Đầu tư & Tiết kiệm', 'INCOME', 'wallet-outline', '#00BBF9');
    ");

    // 6. Tái tạo tập dữ liệu Stress Test thực tế trải dài 6 tháng (Tháng 5 -> Tháng 10/2026) cho User #1
    $db->exec("
        INSERT INTO `transactions` (`id`, `user_id`, `wallet_id`, `category_id`, `amount`, `transaction_type`, `note`, `transaction_date`, `created_at`) VALUES
        -- THÁNG 5/2026 (T5: Thu 22.0M, Chi 5.85M)
        (1, 1, 2, 5, 3500000.00, 'EXPENSE', 'Tiền thuê căn hộ mini tháng 05/2026', '2026-05-01', NOW()),
        (2, 1, 2, 8, 18500000.00, 'INCOME', 'Nhận lương tháng 5 chuyển khoản Vietcombank', '2026-05-05', NOW()),
        (3, 1, 2, 9, 3500000.00, 'INCOME', 'Thưởng hoàn thành cột mốc dự án Q2', '2026-05-08', NOW()),
        (4, 1, 1, 3, 85000.00, 'EXPENSE', 'Đổ xăng xe máy Petrolimex', '2026-05-10', NOW()),
        (5, 1, 3, 4, 420000.00, 'EXPENSE', 'Thanh toán tiền điện EVN & Nước sinh hoạt', '2026-05-12', NOW()),
        (6, 1, 2, 2, 650000.00, 'EXPENSE', 'Mua sắm quần áo mùa hè Uniqlo', '2026-05-16', NOW()),
        (7, 1, 1, 1, 380000.00, 'EXPENSE', 'Ăn uống liên hoan phòng cuối tuần', '2026-05-20', NOW()),
        (8, 1, 2, 6, 320000.00, 'EXPENSE', 'Mua sách chuyên ngành kiến trúc phần mềm', '2026-05-23', NOW()),
        (9, 1, 3, 11, 215000.00, 'EXPENSE', 'Xem phim rạp CGV & Bắp rang bơ', '2026-05-27', NOW()),
        (10, 1, 1, 1, 280000.00, 'EXPENSE', 'Cafe họp nhóm & cơm trưa văn phòng tuần', '2026-05-30', NOW()),

        -- THÁNG 6/2026 (T6: Thu 22.5M, Chi 7.42M)
        (11, 1, 2, 5, 3500000.00, 'EXPENSE', 'Tiền thuê căn hộ mini tháng 06/2026', '2026-06-01', NOW()),
        (12, 1, 2, 8, 18500000.00, 'INCOME', 'Nhận lương tháng 6 chuyển khoản Vietcombank', '2026-06-05', NOW()),
        (13, 1, 2, 10, 4000000.00, 'INCOME', 'Thu nhập freelance lập trình web ngoài giờ', '2026-06-08', NOW()),
        (14, 1, 1, 3, 90000.00, 'EXPENSE', 'Đổ xăng xe máy Petrolimex', '2026-06-09', NOW()),
        (15, 1, 3, 4, 620000.00, 'EXPENSE', 'Tiền điện máy lạnh mùa nắng nóng & Nước', '2026-06-12', NOW()),
        (16, 1, 2, 2, 780000.00, 'EXPENSE', 'Siêu thị WinMart thực phẩm & đồ dùng gia đình', '2026-06-15', NOW()),
        (17, 1, 2, 12, 900000.00, 'EXPENSE', 'Đăng ký gói tập gym & yoga 3 tháng', '2026-06-18', NOW()),
        (18, 1, 1, 1, 350000.00, 'EXPENSE', 'Ăn lẩu nướng cuối tuần cùng bạn bè', '2026-06-21', NOW()),
        (19, 1, 2, 7, 450000.00, 'EXPENSE', 'Khám răng và cạo vôi răng định kỳ', '2026-06-24', NOW()),
        (20, 1, 1, 3, 280000.00, 'EXPENSE', 'Bảo dưỡng định kỳ thay dầu xe máy Honda', '2026-06-27', NOW()),
        (21, 1, 1, 1, 450000.00, 'EXPENSE', 'Cà phê sáng & cơm trưa văn phòng 2 tuần cuối', '2026-06-30', NOW()),

        -- THÁNG 7/2026 (T7: Thu 23.0M, Chi 8.25M)
        (22, 1, 2, 5, 3500000.00, 'EXPENSE', 'Tiền thuê căn hộ mini tháng 07/2026', '2026-07-01', NOW()),
        (23, 1, 2, 8, 18500000.00, 'INCOME', 'Nhận lương tháng 7 chuyển khoản Vietcombank', '2026-07-05', NOW()),
        (24, 1, 2, 9, 4500000.00, 'INCOME', 'Thưởng KPI xuất sắc quý 2/2026', '2026-07-08', NOW()),
        (25, 1, 1, 3, 85000.00, 'EXPENSE', 'Đổ xăng xe máy Petrolimex', '2026-07-09', NOW()),
        (26, 1, 3, 4, 550000.00, 'EXPENSE', 'Tiền điện EVN và nước sinh hoạt', '2026-07-12', NOW()),
        (27, 1, 2, 11, 1350000.00, 'EXPENSE', 'Vé xe khứ hồi & khách sạn du lịch Vũng Tàu', '2026-07-15', NOW()),
        (28, 1, 1, 1, 850000.00, 'EXPENSE', 'Ăn hải sản liên hoan chuyến du lịch', '2026-07-16', NOW()),
        (29, 1, 2, 2, 620000.00, 'EXPENSE', 'Mua sắm phụ kiện & đồ dùng cá nhân', '2026-07-20', NOW()),
        (30, 1, 2, 6, 499000.00, 'EXPENSE', 'Mua khóa học Cloud Architecture trên Udemy', '2026-07-23', NOW()),
        (31, 1, 3, 7, 250000.00, 'EXPENSE', 'Mua thuốc cảm cúm & khẩu y tế', '2026-07-26', NOW()),
        (32, 1, 1, 1, 320000.00, 'EXPENSE', 'Cà phê Highland gặp gỡ đối tác', '2026-07-29', NOW()),
        (33, 1, 1, 1, 225000.00, 'EXPENSE', 'Cơm trưa văn phòng tuần cuối tháng', '2026-07-31', NOW()),

        -- THÁNG 8/2026 (T8: Thu 20.2M, Chi 6.15M)
        (34, 1, 2, 5, 3500000.00, 'EXPENSE', 'Tiền thuê căn hộ mini tháng 08/2026', '2026-08-01', NOW()),
        (35, 1, 2, 8, 18500000.00, 'INCOME', 'Nhận lương tháng 8 chuyển khoản Vietcombank', '2026-08-05', NOW()),
        (36, 1, 2, 13, 1700000.00, 'INCOME', 'Lãi định kỳ trái phiếu & sổ tiết kiệm', '2026-08-08', NOW()),
        (37, 1, 1, 3, 90000.00, 'EXPENSE', 'Đổ xăng xe máy Petrolimex', '2026-08-10', NOW()),
        (38, 1, 3, 4, 520000.00, 'EXPENSE', 'Hóa đơn tiền điện nước sinh hoạt tháng 8', '2026-08-12', NOW()),
        (39, 1, 2, 2, 590000.00, 'EXPENSE', 'Siêu thị Co.opmart mua thực phẩm khô', '2026-08-15', NOW()),
        (40, 1, 1, 1, 320000.00, 'EXPENSE', 'Cà phê sáng & ăn sáng bánh cuốn tuần 2', '2026-08-18', NOW()),
        (41, 1, 3, 7, 310000.00, 'EXPENSE', 'Mua thực phẩm chức năng bổ sung vitamin', '2026-08-22', NOW()),
        (42, 1, 1, 1, 200000.00, 'EXPENSE', 'Góp quỹ liên hoan sinh nhật đồng nghiệp', '2026-08-25', NOW()),
        (43, 1, 2, 6, 280000.00, 'EXPENSE', 'Mua sách tâm lý học hành vi Richard Thaler', '2026-08-28', NOW()),
        (44, 1, 1, 1, 340000.00, 'EXPENSE', 'Cơm trưa văn phòng & nước ép tuần cuối', '2026-08-31', NOW()),

        -- THÁNG 9/2026 (T9: Thu 23.5M, Chi 8.05M)
        (45, 1, 2, 5, 3500000.00, 'EXPENSE', 'Tiền thuê căn hộ mini tháng 09/2026', '2026-09-01', NOW()),
        (46, 1, 2, 8, 18500000.00, 'INCOME', 'Nhận lương tháng 9 chuyển khoản Vietcombank', '2026-09-05', NOW()),
        (47, 1, 2, 9, 5000000.00, 'INCOME', 'Thưởng hoàn thành xuất sắc hệ thống PTTK', '2026-09-08', NOW()),
        (48, 1, 1, 3, 85000.00, 'EXPENSE', 'Đổ xăng xe máy Petrolimex', '2026-09-09', NOW()),
        (49, 1, 2, 2, 1450000.00, 'EXPENSE', 'Nâng cấp bàn phím cơ không dây & Chuột Logitech', '2026-09-12', NOW()),
        (50, 1, 3, 4, 490000.00, 'EXPENSE', 'Thanh toán tiền điện EVN & Cước Internet', '2026-09-14', NOW()),
        (51, 1, 2, 2, 820000.00, 'EXPENSE', 'Siêu thị Mega Market tích trữ đồ gia dụng', '2026-09-17', NOW()),
        (52, 1, 1, 1, 450000.00, 'EXPENSE', 'Ăn uống liên hoan nghiệm thu đồ án giữa kỳ', '2026-09-20', NOW()),
        (53, 1, 2, 12, 450000.00, 'EXPENSE', 'Mua giày chạy bộ thể thao giảm giá', '2026-09-23', NOW()),
        (54, 1, 1, 2, 150000.00, 'EXPENSE', 'Cắt tóc & chăm sóc da mặt nam', '2026-09-25', NOW()),
        (55, 1, 3, 11, 180000.00, 'EXPENSE', 'Xem phim rạp chiếu phim Lotte Cinema', '2026-09-27', NOW()),
        (56, 1, 1, 1, 380000.00, 'EXPENSE', 'Cơm trưa văn phòng & trà trái cây tuần 4', '2026-09-29', NOW()),
        (57, 1, 1, 3, 90000.00, 'EXPENSE', 'Đổ đầy bình xăng xe máy chuẩn bị tháng mới', '2026-09-30', NOW()),

        -- THÁNG 10/2026 (THÁNG HIỆN TẠI - 35 GIAO DỊCH STRESS TEST TRẢI ĐỀU TỪ NGÀY 01 ĐẾN 14)
        (58, 1, 2, 5, 3500000.00, 'EXPENSE', 'Tiền thuê căn hộ mini tháng 10/2026', '2026-10-01', NOW()),
        (59, 1, 1, 1, 45000.00, 'EXPENSE', 'Cà phê sáng Highlands Coffee', '2026-10-01', NOW()),
        (60, 1, 1, 3, 80000.00, 'EXPENSE', 'Đổ xăng xe máy Petrolimex', '2026-10-02', NOW()),
        (61, 1, 1, 1, 45000.00, 'EXPENSE', 'Ăn trưa bún bò Huế gần công ty', '2026-10-02', NOW()),
        (62, 1, 2, 6, 220000.00, 'EXPENSE', 'Mua sách Thiết kế Hệ thống Thông tin Fahasa', '2026-10-03', NOW()),
        (63, 1, 1, 1, 40000.00, 'EXPENSE', 'Cơm trưa văn phòng', '2026-10-03', NOW()),
        (64, 1, 2, 2, 310000.00, 'EXPENSE', 'Siêu thị WinMart rau củ thịt cá tuần 1', '2026-10-04', NOW()),
        (65, 1, 1, 1, 25000.00, 'EXPENSE', 'Bánh mì que & Sữa đậu nành sáng', '2026-10-04', NOW()),
        (66, 1, 2, 8, 18500000.00, 'INCOME', 'Nhận lương tháng 10 chuyển khoản Vietcombank', '2026-10-05', NOW()),
        (67, 1, 3, 1, 65000.00, 'EXPENSE', 'Trà sữa The Alley khao bạn', '2026-10-05', NOW()),
        (68, 1, 3, 12, 450000.00, 'EXPENSE', 'Gia hạn gói tập thể hình Gym tháng 10', '2026-10-06', NOW()),
        (69, 1, 1, 1, 45000.00, 'EXPENSE', 'Cơm tấm sườn bì chả trưa', '2026-10-06', NOW()),
        (70, 1, 2, 2, 480000.00, 'EXPENSE', 'Mua áo sơ mi công sở An Phước', '2026-10-07', NOW()),
        (71, 1, 1, 1, 40000.00, 'EXPENSE', 'Cà phê phin Trung Nguyên Legend', '2026-10-07', NOW()),
        (72, 1, 2, 9, 2000000.00, 'INCOME', 'Thưởng nóng năng suất dự án AI', '2026-10-08', NOW()),
        (73, 1, 2, 1, 360000.00, 'EXPENSE', 'Ăn tối thịt nướng Sumo BBQ liên hoan', '2026-10-08', NOW()),
        (74, 1, 1, 3, 90000.00, 'EXPENSE', 'Đổ xăng xe máy Petrolimex', '2026-10-09', NOW()),
        (75, 1, 1, 1, 45000.00, 'EXPENSE', 'Hủ tiếu Nam Vang ăn sáng', '2026-10-09', NOW()),
        (76, 1, 3, 2, 160000.00, 'EXPENSE', 'Mua dầu gội & kem đánh răng Miniso', '2026-10-09', NOW()),
        (77, 1, 3, 10, 1500000.00, 'INCOME', 'Thù lao hỗ trợ kiểm thử hệ thống cho đối tác', '2026-10-10', NOW()),
        (78, 1, 1, 1, 55000.00, 'EXPENSE', 'Phở bò tái nạm & Cà phê sáng', '2026-10-10', NOW()),
        (79, 1, 3, 7, 210000.00, 'EXPENSE', 'Mua vitamin C & thuốc nhỏ mắt Pharmacity', '2026-10-10', NOW()),
        (80, 1, 3, 11, 140000.00, 'EXPENSE', 'Vé xem phim CGV cuối tuần', '2026-10-11', NOW()),
        (81, 1, 1, 11, 85000.00, 'EXPENSE', 'Bắp ngô & Nước ngọt CGV combo', '2026-10-11', NOW()),
        (82, 1, 1, 1, 95000.00, 'EXPENSE', 'Ăn tối mì cay Seoul cấp độ 2', '2026-10-11', NOW()),
        (83, 1, 2, 2, 420000.00, 'EXPENSE', 'Siêu thị WinMart thực phẩm tươi sống tuần 2', '2026-10-12', NOW()),
        (84, 1, 1, 2, 100000.00, 'EXPENSE', 'Cắt tóc gội đầu thư giãn tiệm 30Shine', '2026-10-12', NOW()),
        (85, 1, 3, 4, 480000.00, 'EXPENSE', 'Thanh toán tiền điện EVN Đồng Nai tháng 9', '2026-10-13', NOW()),
        (86, 1, 3, 4, 95000.00, 'EXPENSE', 'Thanh toán tiền nước sinh hoạt', '2026-10-13', NOW()),
        (87, 1, 3, 4, 220000.00, 'EXPENSE', 'Cước Internet cáp quang Viettel', '2026-10-13', NOW()),
        (88, 1, 1, 1, 50000.00, 'EXPENSE', 'Cơm trưa văn phòng cùng đồng nghiệp', '2026-10-14', NOW()),
        (89, 1, 1, 1, 55000.00, 'EXPENSE', 'Trà đào Phúc Long giải khát chiều', '2026-10-14', NOW()),
        (90, 1, 1, 1, 35000.00, 'EXPENSE', 'Bánh mì ngọt Tous Les Jours ăn xế', '2026-10-14', NOW()),
        (91, 1, 1, 3, 35000.00, 'EXPENSE', 'Vé gửi xe tháng tòa nhà văn phòng', '2026-10-14', NOW()),
        (92, 1, 2, 13, 500000.00, 'INCOME', 'Cổ tức nhận tiền mặt từ quỹ mở', '2026-10-14', NOW());
    ");

    // 7. Tái tạo ngân sách tháng 10/2026 cho User #1 (Tổng ngân sách + 9 Danh mục chi tiết)
    $monthNum = (int)date('n');
    $yearNum  = (int)date('Y');
    $db->exec("
        INSERT INTO `budgets` (`id`, `user_id`, `category_id`, `amount_limit`, `month`, `year`) VALUES
        -- Ngân sách tổng tháng 10: 12.000.000 đ
        (1, 1, NULL, 12000000.00, $monthNum, $yearNum),
        -- Ngân sách phân rã chi tiết từng danh mục:
        (2, 1, 1, 3500000.00, $monthNum, $yearNum),  -- Ăn uống & Cà phê
        (3, 1, 2, 2000000.00, $monthNum, $yearNum),  -- Mua sắm & Đồ dùng
        (4, 1, 3, 800000.00, $monthNum, $yearNum),   -- Di chuyển & Xăng xe
        (5, 1, 4, 1200000.00, $monthNum, $yearNum),  -- Hóa đơn & Điện nước
        (6, 1, 5, 3500000.00, $monthNum, $yearNum),  -- Nhà ở & Tiền phòng
        (7, 1, 6, 600000.00, $monthNum, $yearNum),   -- Giáo dục & Sách vở
        (8, 1, 7, 500000.00, $monthNum, $yearNum),   -- Sức khỏe & Thuốc men
        (9, 1, 11, 600000.00, $monthNum, $yearNum),  -- Giải trí & Dịch vụ
        (10, 1, 12, 600000.00, $monthNum, $yearNum); -- Thể thao & Thể hình
    ");

    // 8. Bật lại kiểm tra khóa ngoại
    $db->exec("SET FOREIGN_KEY_CHECKS = 1;");

    send_json_response(
        "success", 
        "Đã dọn dẹp và khôi phục CSDL về trạng thái sạch ban đầu với bộ dữ liệu Stress Test 6 tháng (92 giao dịch)! ", 
        [
            "environment"        => "Localhost (WAMP)",
            "tables_truncated"   => $tables,
            "sample_users"       => 2,
            "sample_wallets"     => 4,
            "default_categories" => 13,
            "sample_transactions"=> 92,
            "sample_budgets"     => 10,
            "applied_month"      => "$monthNum/$yearNum"
        ], 
        200
    );

} catch (PDOException $e) {
    $db->exec("SET FOREIGN_KEY_CHECKS = 1;");
    send_json_response("error", "Lỗi CSDL khi reset dữ liệu: " . $e->getMessage(), null, 500);
}
