<?php
/**
 * Module tiện ích dùng chung cho toàn bộ RESTful API
 * - Cấu hình Header CORS & Xử lý Preflight (OPTIONS)
 * - Đọc và giải mã JSON payload
 * - Chuẩn hóa định dạng phản hồi (Standard JSON Response)
 * - Lõi Bảo mật JWT (HMAC-SHA256) Pure PHP không phụ thuộc Composer
 * - Middleware xác thực Bearer Token (Chống tấn công IDOR)
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 * Đồ án: Quản lý Chi tiêu Cá nhân Thông minh - Đại học Công nghệ Đồng Nai (DNTU)
 */

// 0. Thiết lập Múi giờ chuẩn Việt Nam (UTC+7) đồng bộ toàn hệ thống
date_default_timezone_set('Asia/Ho_Chi_Minh');

// 1. Cấu hình Header CORS cho phép Mobile App (Expo) và Dev Tools gọi vào
header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json; charset=UTF-8");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS, DELETE, PUT");
header("Access-Control-Allow-Headers: Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With");

// 2. Chặn và trả lời ngay các Preflight Request của trình duyệt/client
if (isset($_SERVER['REQUEST_METHOD']) && $_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit();
}

// 3. Khóa bí mật ký số JWT (HMAC-SHA256 Secret Key)
if (!defined('JWT_SECRET')) {
    define('JWT_SECRET', 'DNTU_LTMB_NHOM46_SECURE_JWT_SECRET_KEY_2026');
}

/**
 * Đọc dữ liệu JSON gửi lên từ Request Body
 * @return array
 */
function get_json_input(): array {
    $raw_content = file_get_contents("php://input");
    $data = json_decode($raw_content, true);
    return is_array($data) ? $data : [];
}

/**
 * Trả về phản hồi JSON chuẩn hóa và dừng script
 * @param string $status 'success' hoặc 'error'
 * @param string $message Thông điệp rõ ràng cho client
 * @param mixed $data Dữ liệu kèm theo
 * @param int $http_code Mã trạng thái HTTP (200, 201, 400, 401, 403, 404, 409, 500)
 */
function send_json_response(string $status, string $message, $data = null, int $http_code = 200): void {
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

/**
 * Mã hóa chuỗi URL-Safe Base64 theo chuẩn RFC 7515 (JWT)
 */
function base64_url_encode(string $data): string {
    return rtrim(strtr(base64_encode($data), '+/', '-_'), '=');
}

/**
 * Giải mã chuỗi URL-Safe Base64 theo chuẩn RFC 7515 (JWT)
 */
function base64_url_decode(string $data): string {
    return base64_decode(str_pad(strtr($data, '-_', '+/'), strlen($data) % 4, '=', STR_PAD_RIGHT));
}

/**
 * Sinh mã JWT Token ký số HMAC-SHA256
 * @param array $payload Dữ liệu định danh người dùng (user_id, email, full_name, ...)
 * @param string $secret Khóa bí mật ký số
 * @param int $expiry_seconds Thời hạn sống của token (Mặc định 7 ngày = 604800 giây)
 * @return string JWT Token dạng header.payload.signature
 */
function jwt_encode(array $payload, string $secret = JWT_SECRET, int $expiry_seconds = 604800): string {
    $header = [
        'alg' => 'HS256',
        'typ' => 'JWT'
    ];

    $now = time();
    if (!isset($payload['iat'])) {
        $payload['iat'] = $now;
    }
    if (!isset($payload['exp'])) {
        $payload['exp'] = $now + $expiry_seconds;
    }

    $b64Header  = base64_url_encode(json_encode($header));
    $b64Payload = base64_url_encode(json_encode($payload));

    $signatureRaw = hash_hmac('sha256', "$b64Header.$b64Payload", $secret, true);
    $b64Signature = base64_url_encode($signatureRaw);

    return "$b64Header.$b64Payload.$b64Signature";
}

/**
 * Giải mã và xác minh tính hợp lệ của JWT Token
 * @param string $token JWT token
 * @param string $secret Khóa bí mật
 * @return array|null Trả về payload dạng array nếu hợp lệ, null nếu giả mạo hoặc hết hạn
 */
function jwt_decode(string $token, string $secret = JWT_SECRET): ?array {
    $parts = explode('.', $token);
    if (count($parts) !== 3) {
        return null;
    }

    list($b64Header, $b64Payload, $b64Signature) = $parts;

    $headerJson = base64_url_decode($b64Header);
    $header = json_decode($headerJson, true);
    if (!$header || ($header['alg'] ?? '') !== 'HS256') {
        return null;
    }

    $expectedSigRaw = hash_hmac('sha256', "$b64Header.$b64Payload", $secret, true);
    $expectedB64Sig = base64_url_encode($expectedSigRaw);

    // Kiểm tra chữ ký an toàn chống Timing Attack
    if (!hash_equals($expectedB64Sig, $b64Signature)) {
        return null;
    }

    $payloadJson = base64_url_decode($b64Payload);
    $payload = json_decode($payloadJson, true);
    if (!is_array($payload)) {
        return null;
    }

    // Kiểm tra thời hạn hết hạn token
    if (isset($payload['exp']) && $payload['exp'] < time()) {
        return null;
    }

    return $payload;
}

/**
 * Trích xuất chuỗi Bearer Token từ các biến môi trường và Header HTTP
 * Hỗ trợ chuẩn trên cả Apache WAMP và Hosting FastCGI
 * @return string|null
 */
function get_bearer_token(): ?string {
    $header = null;

    if (isset($_SERVER['HTTP_AUTHORIZATION'])) {
        $header = trim($_SERVER['HTTP_AUTHORIZATION']);
    } elseif (isset($_SERVER['REDIRECT_HTTP_AUTHORIZATION'])) {
        $header = trim($_SERVER['REDIRECT_HTTP_AUTHORIZATION']);
    } elseif (function_exists('apache_request_headers')) {
        $requestHeaders = apache_request_headers();
        $requestHeaders = array_combine(array_map('ucwords', array_keys($requestHeaders)), array_values($requestHeaders));
        if (isset($requestHeaders['Authorization'])) {
            $header = trim($requestHeaders['Authorization']);
        }
    }

    if (!empty($header)) {
        if (preg_match('/Bearer\s+(\S+)/i', $header, $matches)) {
            return $matches[1];
        }
    }

    return null;
}

/**
 * Middleware cốt tử: Bắt buộc yêu cầu xác thực JWT cho API
 * Chặn đứng hoàn toàn tấn công IDOR bằng cách trích xuất trực tiếp user_id từ chữ ký số
 * @return array Trả về thông tin payload đã xác thực (gồm user_id, email, full_name, ...)
 */
function require_authenticated_user(): array {
    $token = get_bearer_token();
    if (!$token) {
        send_json_response(
            "error", 
            "Truy cập bị từ chối: Vui lòng đăng nhập để tiếp tục (Thiếu Authorization Bearer Token).", 
            null, 
            401
        );
    }

    $payload = jwt_decode($token);
    if (!$payload || !isset($payload['user_id'])) {
        send_json_response(
            "error", 
            "Phiên làm việc không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại.", 
            null, 
            401
        );
    }

    return $payload;
}

/**
 * Tiện ích lấy nhanh user_id đã xác thực an toàn từ Bearer Token
 * @return int
 */
function get_authenticated_user_id(): int {
    $user = require_authenticated_user();
    return (int)$user['user_id'];
}

/**
 * ==============================================================================
 * ⚡ TIỆN ÍCH KẾT NỐI REALTIME SOCKET.IO GATEWAY (NODE.JS PORT 3001)
 * Kết nối 2 chiều giữa PHP WAMP Backend và App Mobile / Web Simulator
 * Cơ chế Safe Degradation: Nếu Realtime Server chưa bật, API PHP vẫn chạy 100% trơn tru.
 * ==============================================================================
 */

/**
 * Gửi yêu cầu HTTP POST kích hoạt sự kiện tới Realtime Gateway
 * @param string $endpoint Đường dẫn API (/api/notify/balance-update, v.v.)
 * @param array $payload Dữ liệu sự kiện JSON
 * @return bool
 */
function send_realtime_event(string $endpoint, array $payload): bool {
    $url = "http://127.0.0.1:3001" . $endpoint;
    $json = json_encode($payload);

    if (function_exists('curl_init')) {
        $ch = curl_init($url);
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
        curl_setopt($ch, CURLOPT_POSTFIELDS, $json);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_HTTPHEADER, [
            'Content-Type: application/json',
            'Content-Length: ' . strlen($json)
        ]);
        curl_setopt($ch, CURLOPT_TIMEOUT_MS, 400); // Tối đa 400ms không làm nghẽn PHP
        curl_setopt($ch, CURLOPT_CONNECTTIMEOUT_MS, 200);
        $result = @curl_exec($ch);
        $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        curl_close($ch);
        return ($http_code === 200);
    } else {
        $options = [
            'http' => [
                'header'  => "Content-type: application/json\r\n",
                'method'  => 'POST',
                'content' => $json,
                'timeout' => 0.4
            ]
        ];
        $context = stream_context_create($options);
        $result = @file_get_contents($url, false, $context);
        return ($result !== false);
    }
}

/**
 * Kích hoạt sự kiện đồng bộ số dư ví tức thời trên tất cả thiết bị
 */
function realtime_balance_update(int $userId, int $walletId, string $walletName, float $newBalance, float $delta, string $reason): void {
    @send_realtime_event("/api/notify/balance-update", [
        "userId" => $userId,
        "walletId" => $walletId,
        "walletName" => $walletName,
        "newBalance" => $newBalance,
        "delta" => $delta,
        "reason" => $reason
    ]);
}

/**
 * Kích hoạt sự kiện cảnh báo Cú hích chi tiêu (Nudge Alert) khi gần/vượt hạn mức ngân sách
 */
function realtime_budget_nudge(int $userId, string $categoryName, float $currentSpent, float $budgetLimit, string $severity = 'warning'): void {
    @send_realtime_event("/api/notify/budget-nudge", [
        "userId" => $userId,
        "categoryName" => $categoryName,
        "currentSpent" => $currentSpent,
        "budgetLimit" => $budgetLimit,
        "severity" => $severity
    ]);
}

/**
 * Kích hoạt sự kiện thu hồi phiên đăng nhập từ xa vì lý do an ninh
 */
function realtime_force_logout(int $userId, string $reason): void {
    @send_realtime_event("/api/notify/force-logout", [
        "userId" => $userId,
        "reason" => $reason
    ]);
}
