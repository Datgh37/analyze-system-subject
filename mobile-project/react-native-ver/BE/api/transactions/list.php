<?php
/**
 * API: Lấy danh sách Lịch sử Giao dịch (Có Bộ lọc Đa tiêu chí)
 * Phương thức: GET
 * Params: 
 *   - month (int, 1-12)
 *   - year (int)
 *   - wallet_id (int)
 *   - category_id (int)
 *   - type (EXPENSE|INCOME)
 *   - keyword (string tìm kiếm trong ghi chú)
 *   - limit (int, mặc định 50)
 *   - offset (int, mặc định 0)
 * 
 * Bảo mật: Bắt buộc kèm Bearer JWT Token
 * Tác giả: Trần Trọng Đại (UI & Business API) & Trịnh Thành Đạt (Lead)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

// 1. Xác thực người dùng qua JWT
$user_id = get_authenticated_user_id();

// 2. Nhận các bộ lọc đa tiêu chí (UX & QOL)
$month       = filter_input(INPUT_GET, 'month', FILTER_VALIDATE_INT);
$year        = filter_input(INPUT_GET, 'year', FILTER_VALIDATE_INT);
$wallet_id   = filter_input(INPUT_GET, 'wallet_id', FILTER_VALIDATE_INT);
$category_id = filter_input(INPUT_GET, 'category_id', FILTER_VALIDATE_INT);
$type        = isset($_GET['type']) ? strtoupper(trim($_GET['type'])) : null;
$keyword     = trim($_GET['keyword'] ?? '');
$limit       = filter_input(INPUT_GET, 'limit', FILTER_VALIDATE_INT) ?: 50;
$offset      = filter_input(INPUT_GET, 'offset', FILTER_VALIDATE_INT) ?: 0;

if ($limit > 100) $limit = 100;
if ($offset < 0)  $offset = 0;

$database = new Database();
$db = $database->getConnection();

try {
    $where_clauses = ["t.user_id = :uid"];
    $params = [':uid' => $user_id];

    if ($month && $year) {
        $where_clauses[] = "MONTH(t.transaction_date) = :month AND YEAR(t.transaction_date) = :year";
        $params[':month'] = $month;
        $params[':year']  = $year;
    } elseif ($year) {
        $where_clauses[] = "YEAR(t.transaction_date) = :year";
        $params[':year']  = $year;
    }

    if ($wallet_id) {
        $where_clauses[] = "t.wallet_id = :wid";
        $params[':wid']  = $wallet_id;
    }

    if ($category_id) {
        $where_clauses[] = "t.category_id = :cid";
        $params[':cid']  = $category_id;
    }

    if ($type && in_array($type, ['EXPENSE', 'INCOME'])) {
        $where_clauses[] = "t.transaction_type = :ttype";
        $params[':ttype'] = $type;
    }

    if (!empty($keyword)) {
        $where_clauses[] = "t.note LIKE :keyword";
        $params[':keyword'] = "%" . $keyword . "%";
    }

    $where_sql = implode(' AND ', $where_clauses);

    $sql = "
        SELECT 
            t.id,
            t.user_id,
            t.wallet_id,
            w.name AS wallet_name,
            t.category_id,
            c.name AS category_name,
            c.icon_name AS category_icon,
            c.color_code AS category_color,
            t.amount,
            t.transaction_type,
            t.transaction_type AS `type`,
            t.note,
            t.transaction_date,
            t.created_at
        FROM `transactions` t
        LEFT JOIN `wallets` w ON t.wallet_id = w.id
        LEFT JOIN `categories` c ON t.category_id = c.id
        WHERE {$where_sql}
        ORDER BY t.transaction_date DESC, t.id DESC
        LIMIT :lim OFFSET :off
    ";

    $stmt = $db->prepare($sql);
    foreach ($params as $key => $val) {
        if (is_int($val)) {
            $stmt->bindValue($key, $val, PDO::PARAM_INT);
        } else {
            $stmt->bindValue($key, $val, PDO::PARAM_STR);
        }
    }
    $stmt->bindValue(':lim', (int)$limit, PDO::PARAM_INT);
    $stmt->bindValue(':off', (int)$offset, PDO::PARAM_INT);
    $stmt->execute();

    $transactions = $stmt->fetchAll(PDO::FETCH_ASSOC);

    foreach ($transactions as &$t) {
        $t['amount'] = (float)$t['amount'];
    }

    send_json_response("success", "Lấy danh sách giao dịch thành công.", [
        "count"        => count($transactions),
        "filters"      => [
            "month"       => $month,
            "year"        => $year,
            "wallet_id"   => $wallet_id,
            "category_id" => $category_id,
            "type"        => $type,
            "keyword"     => $keyword
        ],
        "transactions" => $transactions
    ], 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi lấy danh sách giao dịch: " . $e->getMessage(), null, 500);
}
?>
