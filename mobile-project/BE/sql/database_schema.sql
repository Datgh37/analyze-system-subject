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

-- 2.1. Thêm người dùng mẫu (Mật khẩu: password123 đã băm Bcrypt)
INSERT IGNORE INTO `users` (`id`, `full_name`, `email`, `password_hash`, `created_at`) VALUES
(1, 'Trịnh Thành Đạt', 'thanhdat@gmail.com', '$2y$10$WehxfePSQA7OFuP3.ooiCuIFiJyFE5FQZOAU.jasJiE.Ta1P2S7.i', NOW()),
(2, 'Trần Trọng Đại', 'trongdai@gmail.com', '$2y$10$WehxfePSQA7OFuP3.ooiCuIFiJyFE5FQZOAU.jasJiE.Ta1P2S7.i', NOW());

-- 2.2. Thêm ví tài chính mẫu
INSERT IGNORE INTO `wallets` (`id`, `user_id`, `name`, `balance`, `wallet_type`, `created_at`) VALUES
(1, 1, 'Tiền Mặt Trong Ví', 2500000.00, 'CASH', NOW()),
(2, 1, 'Tài Khoản Vietcombank', 18500000.00, 'BANK', NOW()),
(3, 2, 'Tiền Mặt', 1200000.00, 'CASH', NOW()),
(4, 2, 'Ngân Hàng MB Bank', 8500000.00, 'BANK', NOW());

-- 2.3. Danh mục mặc định của hệ thống (user_id = NULL)
INSERT IGNORE INTO `categories` (`id`, `user_id`, `name`, `type`, `icon_name`, `color_code`) VALUES
-- Danh mục CHI TIÊU (EXPENSE)
(1, NULL, 'Ăn uống & Cà phê', 'EXPENSE', 'fast-food-outline', '#FF6B6B'),
(2, NULL, 'Mua sắm & Đồ dùng', 'EXPENSE', 'cart-outline', '#4ECDC4'),
(3, NULL, 'Di chuyển & Xăng xe', 'EXPENSE', 'car-sport-outline', '#FFE66D'),
(4, NULL, 'Hóa đơn & Điện nước', 'EXPENSE', 'receipt-outline', '#1A535C'),
(5, NULL, 'Nhà ở & Tiền phòng', 'EXPENSE', 'home-outline', '#4D96FF'),
(6, NULL, 'Giáo dục & Sách vở', 'EXPENSE', 'book-outline', '#9B5DE5'),
(7, NULL, 'Sức khỏe & Thuốc men', 'EXPENSE', 'medkit-outline', '#F15BB5'),
-- Danh mục THU NHẬP (INCOME)
(8, NULL, 'Lương hàng tháng', 'INCOME', 'cash-outline', '#2EC4B6'),
(9, NULL, 'Thưởng & Làm thêm', 'INCOME', 'trending-up-outline', '#38B000'),
(10, NULL, 'Thu nhập khác', 'INCOME', 'wallet-outline', '#70E000');

-- 2.4. Giao dịch mẫu tháng hiện tại cho user 1
INSERT IGNORE INTO `transactions` (`id`, `user_id`, `wallet_id`, `category_id`, `amount`, `transaction_type`, `note`, `transaction_date`, `created_at`) VALUES
(1, 1, 2, 8, 15000000.00, 'INCOME', 'Nhận lương tháng qua chuyển khoản Vietcombank', '2026-09-05', NOW()),
(2, 1, 1, 1, 55000.00, 'EXPENSE', 'Ăn trưa cơm văn phòng', '2026-09-20', NOW()),
(3, 1, 1, 1, 35000.00, 'EXPENSE', 'Cà phê sáng cùng đồng nghiệp', '2026-09-21', NOW()),
(4, 1, 1, 3, 90000.00, 'EXPENSE', 'Đổ xăng xe máy Petrolimex', '2026-09-22', NOW()),
(5, 1, 2, 2, 650000.00, 'EXPENSE', 'Mua sắm nhu yếu phẩm siêu thị Co.opmart', '2026-09-25', NOW()),
(6, 1, 2, 4, 380000.00, 'EXPENSE', 'Thanh toán tiền điện và nước sinh hoạt', '2026-09-27', NOW()),
(7, 1, 1, 9, 500000.00, 'INCOME', 'Tiền thưởng dự án nhỏ', '2026-09-28', NOW());

-- 2.5. Ngân sách mẫu tháng 9/2026 cho user 1
INSERT IGNORE INTO `budgets` (`id`, `user_id`, `category_id`, `amount_limit`, `month`, `year`) VALUES
(1, 1, NULL, 6000000.00, 9, 2026), -- Ngân sách tổng chi cả tháng là 6 triệu
(2, 1, 1, 2500000.00, 9, 2026);   -- Hạn mức riêng cho ăn uống là 2.5 triệu

-- Kích hoạt lại kiểm tra khóa ngoại sau khi hoàn tất
SET FOREIGN_KEY_CHECKS = 1;
