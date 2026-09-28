<?php
/**
 * API: Báo cáo tỷ trọng chi tiêu theo danh mục (Biểu đồ tròn Pie Chart)
 * Phương thức: GET
 * Params: ?user_id={id}&month={m}&year={y}
 * Tác giả: Trịnh Thành Đạt (Lead)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

// Lấy tham số truy vấn
$user_id = filter_input(INPUT_GET, 'user_id', FILTER_VALIDATE_INT);
$month   = filter_input(INPUT_GET, 'month', FILTER_VALIDATE_INT) ?? (int)date('m');
$year    = filter_input(INPUT_GET, 'year', FILTER_VALIDATE_INT) ?? (int)date('Y');

if (!$user_id) {
    send_json_response("error", "Thiếu hoặc sai định dạng tham số user_id.", null, 400);
}

$database = new Database();
$db = $database->getConnection();

try {
    // 1. Tính tổng toàn bộ chi tiêu trong tháng để tính % tỷ trọng
    $total_stmt = $db->prepare("
        SELECT COALESCE(SUM(amount), 0) AS grand_total
        FROM `transactions`
        WHERE `user_id` = :uid 
          AND `transaction_type` = 'EXPENSE'
          AND MONTH(`transaction_date`) = :month 
          AND YEAR(`transaction_date`) = :year
    ");
    $total_stmt->bindParam(':uid', $user_id);
    $total_stmt->bindParam(':month', $month);
    $total_stmt->bindParam(':year', $year);
    $total_stmt->execute();
    $grand_total = (float)$total_stmt->fetchColumn();

    // 2. Gom nhóm tổng chi theo từng danh mục
    $stmt = $db->prepare("
        SELECT 
            c.id AS category_id,
            c.name AS category_name,
            c.icon_name,
            COALESCE(c.color_code, '#4D96FF') AS color_code,
            SUM(t.amount) AS total_amount
        FROM `transactions` t
        INNER JOIN `categories` c ON t.category_id = c.id
        WHERE t.user_id = :uid 
          AND t.transaction_type = 'EXPENSE'
          AND MONTH(t.transaction_date) = :month 
          AND YEAR(t.transaction_date) = :year
        GROUP BY c.id, c.name, c.icon_name, c.color_code
        ORDER BY total_amount DESC
    ");
    $stmt->bindParam(':uid', $user_id);
    $stmt->bindParam(':month', $month);
    $stmt->bindParam(':year', $year);
    $stmt->execute();

    $categories = $stmt->fetchAll();
    $chart_data = [];

    foreach ($categories as $cat) {
        $amount = (float)$cat['total_amount'];
        $percentage = ($grand_total > 0) ? round(($amount / $grand_total) * 100, 1) : 0;

        // Định dạng dữ liệu tương thích trực tiếp với thư viện react-native-chart-kit PieChart
        $chart_data[] = [
            "category_id"     => (int)$cat['category_id'],
            "name"            => $cat['category_name'],
            "population"      => $amount, // react-native-chart-kit dùng trường population làm giá trị
            "amount"          => $amount,
            "percentage"      => $percentage,
            "color"           => $cat['color_code'],
            "icon_name"       => $cat['icon_name'],
            "legendFontColor" => "#2B4C7E",
            "legendFontSize"  => 12
        ];
    }

    send_json_response("success", "Lấy dữ liệu biểu đồ chi tiêu thành công!", [
        "month"       => (int)$month,
        "year"        => (int)$year,
        "grand_total" => $grand_total,
        "chart_data"  => $chart_data
    ], 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi máy chủ khi truy vấn biểu đồ chi tiêu: " . $e->getMessage(), null, 500);
}
?>
