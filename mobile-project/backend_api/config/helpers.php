<?php
/**
 * Module tiện ích dùng chung cho toàn bộ RESTful API
 * - Cấu hình Header CORS
 * - Xử lý Preflight Request (OPTIONS)
 * - Đọc và giải mã JSON payload
 * - Chuẩn hóa định dạng phản hồi (Standard JSON Response)
 * Tác giả: Trịnh Thành Đạt (Lead)
 */

// 1. Cấu hình Header CORS cho phép Client Mobile và Postman gọi vào
header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json; charset=UTF-8");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS, DELETE, PUT");
header("Access-Control-Allow-Headers: Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With");

// 2. Chặn và trả lời ngay các Preflight Request của trình duyệt/client
if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit();
}

/**
 * Lấy dữ liệu JSON gửi lên từ body của Request
 * @return array
 */
function get_json_input() {
    $raw_content = file_get_contents("php://input");
    $data = json_decode($raw_content, true);
    return is_array($data) ? $data : [];
}

/**
 * Trả về phản hồi JSON chuẩn hóa và kết thúc thực thi
 * @param string $status 'success' hoặc 'error'
 * @param string $message Thông điệp hiển thị cho người dùng
 * @param mixed $data Dữ liệu kèm theo
 * @param int $http_code Mã trạng thái HTTP (200, 201, 400, 401, 404, 500)
 */
function send_json_response($status, $message, $data = null, $http_code = 200) {
    http_response_code($http_code);
    $response = [
        "status"    => $status,
        "message"   => $message,
        "timestamp" => date("Y-m-d H:i:s")
    ];

    if ($data !== null) {
        $response["data"] = $data;
    }

    echo json_encode($response, JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
    exit();
}
?>
