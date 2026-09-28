<?php
/**
 * Module kết nối Cơ sở dữ liệu MySQL bằng PDO
 * Hỗ trợ chuyển đổi đa môi trường: WAMP Server (Local) và Hosting Trường cấp (Production)
 * Tác giả: Trịnh Thành Đạt (Lead)
 */

class Database {
    // =========================================================================
    // CỜ CẤU HÌNH MÔI TRƯỜNG:
    // - TRUE: Chạy trên WAMP Server máy tính cá nhân (localhost)
    // - FALSE: Chạy trên Hosting của trường cấp (103.139.203.43 / nhom46.itimit.id.vn)
    // =========================================================================
    private $is_local = true;

    // Cấu hình WAMP Localhost
    private $local_host = "localhost";
    private $local_db   = "sql_nhom46_itimi";
    private $local_user = "root";
    private $local_pass = ""; // Mặc định WAMP là chuỗi rỗng

    // Cấu hình Máy chủ Trường cấp (Hosting)
    private $remote_host = "localhost";
    private $remote_db   = "sql_nhom46_itimi";
    private $remote_user = "sql_nhom46_itimi";
    private $remote_pass = "0a530d0c23153";

    public $conn;

    /**
     * Lấy kết nối PDO an toàn với Prepared Statements
     * @return PDO|null
     */
    public function getConnection() {
        $this->conn = null;

        $host = $this->is_local ? $this->local_host : $this->remote_host;
        $db   = $this->is_local ? $this->local_db   : $this->remote_db;
        $user = $this->is_local ? $this->local_user : $this->remote_user;
        $pass = $this->is_local ? $this->local_pass : $this->remote_pass;

        try {
            $dsn = "mysql:host=" . $host . ";dbname=" . $db . ";charset=utf8mb4";
            $options = [
                PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
                PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
                PDO::ATTR_EMULATE_PREPARES   => false,
            ];

            $this->conn = new PDO($dsn, $user, $pass, $options);
        } catch(PDOException $exception) {
            http_response_code(500);
            echo json_encode([
                "status"  => "error",
                "message" => "Lỗi kết nối Cơ sở dữ liệu: " . $exception->getMessage()
            ], JSON_UNESCAPED_UNICODE);
            exit();
        }

        return $this->conn;
    }
}
?>
