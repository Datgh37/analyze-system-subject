<?php
/**
 * API: Lấy danh sách Lịch sử Giao dịch
 * Phương thức: GET
 * Params: ?user_id={id}&month={m}&year={y}&limit={50}
 * Phụ trách: Trần Trọng Đại (UI & Business API) | Boilerplate by Trịnh Thành Đạt
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

$user_id = filter_input(INPUT_GET, 'user_id', FILTER_VALIDATE_INT);
$month   = filter_input(INPUT_GET, 'month', FILTER_VALIDATE_INT);
$year    = filter_input(INPUT_GET, 'year', FILTER_VALIDATE_INT);
$limit   = filter_input(INPUT_GET, 'limit', FILTER_VALIDATE_INT) ?? 50;

if (!$user_id) {
    send_json_response("error", "Thiếu hoặc sai định dạng tham số user_id.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    $sql = "
        SELECT 
            t.id,
            t.user_id,
            t.wallet_id,
            w.wallet_name,
            t.category_id,
            c.name AS category_name,
            c.icon_name AS category_icon,
            c.color_code AS category_color,
            t.amount,
            t.transaction_type,
            t.note,
            t.transaction_date,
            t.created_at
        FROM `transactions` t
        LEFT JOIN `wallets` w ON t.wallet_id = w.id
        LEFT JOIN `categories` c ON t.category_id = c.id
        WHERE t.user_id = :uid
    ";

    if ($month && $year) {
        $sql .= " AND MONTH(t.transaction_date) = :month AND YEAR(t.transaction_date) = :year";
    }

    $sql .= " ORDER BY t.transaction_date DESC, t.id DESC LIMIT :lim";

    $stmt = $db->prepare($sql);
    $stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    if ($month && $year) {
        $stmt->bindParam(':month', $month, PDO::PARAM_INT);
        $stmt->bindParam(':year', $year, PDO::PARAM_INT);
    }
    $stmt->bindParam(':lim', $limit, PDO::PARAM_INT);
    $stmt->execute();
    $transactions = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Chuẩn hóa kiểu dữ liệu
    foreach ($transactions as &$t) {
        $t['amount'] = (float)$t['amount'];
    }

    send_json_response("success", "Lấy danh sách giao dịch thành công.", [
        "count"        => count($transactions),
        "transactions" => $transactions
    ], 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL: " . $e->getMessage(), null, 500);
}
