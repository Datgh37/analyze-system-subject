<?php
require_once __DIR__ . '/../config/database.php';

try {
    $db = (new Database())->getConnection();
    
    $sql = "CREATE TABLE IF NOT EXISTS `password_resets` (
      `id` INT AUTO_INCREMENT PRIMARY KEY,
      `email` VARCHAR(150) NOT NULL COMMENT 'Email người dùng yêu cầu đặt lại mật khẩu',
      `otp_code` VARCHAR(10) NOT NULL COMMENT 'Mã OTP 6 chữ số',
      `reset_token` VARCHAR(64) NULL COMMENT 'Token tạm thời cấp sau khi xác thực OTP thành công',
      `expires_at` DATETIME NOT NULL COMMENT 'Thời hạn hết hạn của mã OTP hoặc token',
      `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm tạo yêu cầu',
      INDEX `idx_pwd_reset_email` (`email`),
      INDEX `idx_pwd_reset_token` (`reset_token`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

    $db->exec($sql);
    echo "SUCCESS: Table password_resets created/verified in MySQL!\n";
} catch (Exception $e) {
    echo "ERROR: " . $e->getMessage() . "\n";
}
