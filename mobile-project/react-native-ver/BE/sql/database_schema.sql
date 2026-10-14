-- ==============================================================================
-- ĐỒ ÁN MÔN HỌC: LẬP TRÌNH THIẾT BỊ DI ĐỘNG (MOBILE APPLICATION DEVELOPMENT)
-- ĐỀ TÀI: ỨNG DỤNG QUẢN LÝ CHI TIÊU CÁ NHÂN (PERSONAL EXPENSE TRACKER)
-- NHÓM THỰC HIỆN: TRỊNH THÀNH ĐẠT & TRẦN TRỌNG ĐẠI
-- ==============================================================================
-- Tệp DDL Khởi tạo Cơ sở Dữ liệu & Dữ liệu mẫu (An toàn - Chống xung đột)
-- Tương thích 100%: Cả WAMP Server Local & Hosting Server được cấp (Không bị lỗi quyền)
-- Cơ chế an toàn: 
--   1. Không dùng lệnh CREATE/DROP DATABASE (Tránh lỗi phân quyền Access Denied trên Hosting)
--   2. Dùng CREATE TABLE IF NOT EXISTS (Không xóa đè bảng đang chạy)
--   3. Dùng INSERT IGNORE INTO (Chống lỗi Duplicate Key khi chạy lại nhiều lần)
-- ==============================================================================

-- Tạm thời vô hiệu hóa kiểm tra khóa ngoại để quá trình khởi tạo diễn ra trơn tru
SET FOREIGN_KEY_CHECKS = 0;

-- ==============================================================================
-- 1. KHỞI TẠO CÁC BẢNG QUAN HỆ (NẾU CHƯA TỒN TẠI)
-- ==============================================================================

-- BẢNG 1: NGƯỜI DÙNG (USERS)
CREATE TABLE IF NOT EXISTS `users` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `full_name` VARCHAR(100) NOT NULL COMMENT 'Họ và tên người dùng',
  `email` VARCHAR(150) NOT NULL UNIQUE COMMENT 'Email đăng nhập duy nhất',
  `password_hash` VARCHAR(255) NOT NULL COMMENT 'Mật khẩu băm Bcrypt an toàn',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm đăng ký tài khoản'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- BẢNG 2: VÍ TÀI CHÍNH (WALLETS)
CREATE TABLE IF NOT EXISTS `wallets` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `user_id` INT NOT NULL COMMENT 'Mã người dùng sở hữu ví',
  `name` VARCHAR(100) NOT NULL COMMENT 'Tên ví (Ví dụ: Tiền mặt, Vietcombank)',
  `balance` DECIMAL(15,2) DEFAULT 0.00 COMMENT 'Số dư khả dụng hiện tại',
  `wallet_type` ENUM('CASH', 'BANK') DEFAULT 'CASH' COMMENT 'Phân loại ví: Tiền mặt hoặc Ngân hàng',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT `fk_wallets_user` FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- BẢNG 3: DANH MỤC THU / CHI (CATEGORIES)
CREATE TABLE IF NOT EXISTS `categories` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `user_id` INT NULL COMMENT 'NULL nếu là danh mục mặc định của hệ thống; có ID nếu user tự tạo',
  `name` VARCHAR(100) NOT NULL COMMENT 'Tên danh mục (Ví dụ: Ăn uống, Tiền lương)',
  `type` ENUM('INCOME', 'EXPENSE') NOT NULL COMMENT 'Phân loại danh mục: Thu hoặc Chi',
  `icon_name` VARCHAR(50) DEFAULT 'wallet' COMMENT 'Tên Icon Material/Ionicons đại diện',
  `color_code` VARCHAR(20) DEFAULT '#2B4C7E' COMMENT 'Mã màu Hex phân biệt danh mục',
  CONSTRAINT `fk_categories_user` FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- BẢNG 4: GIAO DỊCH THU / CHI (TRANSACTIONS)
CREATE TABLE IF NOT EXISTS `transactions` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `user_id` INT NOT NULL COMMENT 'Mã người dùng thực hiện giao dịch',
  `wallet_id` INT NOT NULL COMMENT 'Ví nguồn phát sinh biến động tiền',
  `category_id` INT NOT NULL COMMENT 'Hạng mục phân loại thu/chi',
  `amount` DECIMAL(15,2) NOT NULL COMMENT 'Số tiền phát sinh giao dịch',
  `transaction_type` ENUM('INCOME', 'EXPENSE') NOT NULL COMMENT 'Loại giao dịch: Thu nhập hoặc Chi tiêu',
  `note` TEXT NULL COMMENT 'Ghi chú chi tiết giao dịch',
  `transaction_date` DATE NOT NULL COMMENT 'Ngày ghi nhận giao dịch',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm bấm lưu giao dịch',
  CONSTRAINT `fk_trans_user` FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_trans_wallet` FOREIGN KEY (`wallet_id`) REFERENCES `wallets`(`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_trans_category` FOREIGN KEY (`category_id`) REFERENCES `categories`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- BẢNG 5: HẠN MỨC NGÂN SÁCH THÁNG (BUDGETS)
CREATE TABLE IF NOT EXISTS `budgets` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `user_id` INT NOT NULL COMMENT 'Mã người dùng thiết lập ngân sách',
  `category_id` INT NULL COMMENT 'NULL nếu là ngân sách tổng thể của cả tháng; có ID nếu giới hạn riêng hạng mục',
  `amount_limit` DECIMAL(15,2) NOT NULL COMMENT 'Hạn mức chi tiêu tối đa trong tháng',
  `month` INT NOT NULL COMMENT 'Tháng áp dụng (1 - 12)',
  `year` INT NOT NULL COMMENT 'Năm áp dụng (Ví dụ: 2026)',
  CONSTRAINT `fk_budgets_user` FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_budgets_category` FOREIGN KEY (`category_id`) REFERENCES `categories`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- BẢNG 6: MÃ XÁC THỰC QUÊN MẬT KHẨU (PASSWORD_RESETS)
CREATE TABLE IF NOT EXISTS `password_resets` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `email` VARCHAR(150) NOT NULL COMMENT 'Email người dùng yêu cầu đặt lại mật khẩu',
  `otp_code` VARCHAR(10) NOT NULL COMMENT 'Mã OTP 6 chữ số',
  `reset_token` VARCHAR(64) NULL COMMENT 'Token tạm thời cấp sau khi xác thực OTP thành công',
  `expires_at` DATETIME NOT NULL COMMENT 'Thời hạn hết hạn của mã OTP hoặc token',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm tạo yêu cầu',
  INDEX `idx_pwd_reset_email` (`email`),
  INDEX `idx_pwd_reset_token` (`reset_token`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==============================================================================
-- 2. DỮ LIỆU MẪU KHỞI TẠO (SEED DATA) - DÙNG INSERT IGNORE ĐỂ CHỐNG TRÙNG LẶP
-- ==============================================================================

-- 2.1. Thêm người dùng mẫu giả lập thực tế (Mật khẩu: password123 đã băm Bcrypt)
INSERT IGNORE INTO `users` (`id`, `full_name`, `email`, `password_hash`, `created_at`) VALUES
(1, 'Nguyễn Văn An', 'nguyenvanan@gmail.com', '$2y$10$WehxfePSQA7OFuP3.ooiCuIFiJyFE5FQZOAU.jasJiE.Ta1P2S7.i', NOW()),
(2, 'Trần Thị Mai', 'tranmai@gmail.com', '$2y$10$WehxfePSQA7OFuP3.ooiCuIFiJyFE5FQZOAU.jasJiE.Ta1P2S7.i', NOW());

-- 2.2. Thêm ví tài chính mẫu thực tế sau tích lũy dòng tiền
INSERT IGNORE INTO `wallets` (`id`, `user_id`, `name`, `balance`, `wallet_type`, `created_at`) VALUES
(1, 1, 'Ví Tiền Mặt', 3850000.00, 'CASH', NOW()),
(2, 1, 'Tài Khoản Vietcombank', 36450000.00, 'BANK', NOW()),
(3, 1, 'Ví Điện Tử MoMo', 2850000.00, 'CASH', NOW()),
(4, 1, 'Sổ Tiết Kiệm Dự Phòng', 25000000.00, 'BANK', NOW()),
(5, 2, 'Tiền Mặt', 2000000.00, 'CASH', NOW()),
(6, 2, 'Tài Khoản MB Bank', 12500000.00, 'BANK', NOW());

-- 2.3. Danh mục đa dạng của hệ thống (user_id = NULL)
INSERT IGNORE INTO `categories` (`id`, `user_id`, `name`, `type`, `icon_name`, `color_code`) VALUES
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

-- 2.4. Tập dữ liệu Stress Test thực tế 6 tháng (Tháng 5 -> Tháng 10/2026, 92 giao dịch) cho User #1
INSERT IGNORE INTO `transactions` (`id`, `user_id`, `wallet_id`, `category_id`, `amount`, `transaction_type`, `note`, `transaction_date`, `created_at`) VALUES
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

-- 2.5. Ngân sách mẫu tháng 10/2026 cho user 1 (Tổng ngân sách + 9 Danh mục chi tiết)
INSERT IGNORE INTO `budgets` (`id`, `user_id`, `category_id`, `amount_limit`, `month`, `year`) VALUES
(1, 1, NULL, 12000000.00, 10, 2026), -- Ngân sách tổng chi cả tháng là 12 triệu
(2, 1, 1, 3500000.00, 10, 2026),    -- Hạn mức Ăn uống & Cà phê
(3, 1, 2, 2000000.00, 10, 2026),    -- Hạn mức Mua sắm & Đồ dùng
(4, 1, 3, 800000.00, 10, 2026),     -- Hạn mức Di chuyển & Xăng xe
(5, 1, 4, 1200000.00, 10, 2026),    -- Hạn mức Hóa đơn & Điện nước
(6, 1, 5, 3500000.00, 10, 2026),    -- Hạn mức Nhà ở & Tiền phòng
(7, 1, 6, 600000.00, 10, 2026),     -- Hạn mức Giáo dục & Sách vở
(8, 1, 7, 500000.00, 10, 2026),     -- Hạn mức Sức khỏe & Thuốc men
(9, 1, 11, 600000.00, 10, 2026),    -- Hạn mức Giải trí & Dịch vụ
(10, 1, 12, 600000.00, 10, 2026);   -- Hạn mức Thể thao & Thể hình

-- Kích hoạt lại kiểm tra khóa ngoại sau khi hoàn tất
SET FOREIGN_KEY_CHECKS = 1;
