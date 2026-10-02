<?php
/**
 * API: Báo cáo Tỷ trọng Chi tiêu theo Danh mục (Pie Chart Data)
 * Phương thức: GET
 * Params: ?month={m}&year={y}
 * Bảo mật: Bắt buộc kèm Bearer JWT Token
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

// 1. Xác thực người dùng qua JWT
$user_id = get_authenticated_user_id();

// 2. Lấy tháng và năm
$month = filter_input(INPUT_GET, 'month', FILTER_VALIDATE_INT) ?: (int)date('m');
$year  = filter_input(INPUT_GET, 'year', FILTER_VALIDATE_INT) ?: (int)date('Y');

$database = new Database();
$db = $database->getConnection();

try {
    // 3. Tính tổng toàn bộ chi tiêu trong tháng để tính tỷ trọng %
    $total_stmt = $db->prepare("
        SELECT COALESCE(SUM(amount), 0.00) AS grand_total
        FROM `transactions`
        WHERE `user_id` = :uid 
          AND `transaction_type` = 'EXPENSE'
          AND MONTH(`transaction_date`) = :month 
          AND YEAR(`transaction_date`) = :year
    ");
    $total_stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $total_stmt->bindParam(':month', $month, PDO::PARAM_INT);
    $total_stmt->bindParam(':year', $year, PDO::PARAM_INT);
    $total_stmt->execute();
    $grand_total = (float)$total_stmt->fetchColumn();

    // 4. Gom nhóm tổng chi theo từng danh mục
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
    $stmt->bindParam(':uid', $user_id, PDO::PARAM_INT);
    $stmt->bindParam(':month', $month, PDO::PARAM_INT);
    $stmt->bindParam(':year', $year, PDO::PARAM_INT);
    $stmt->execute();

    $categories = $stmt->fetchAll(PDO::FETCH_ASSOC);
    $chart_data = [];

    foreach ($categories as $cat) {
        $amount = (float)$cat['total_amount'];
        $percentage = ($grand_total > 0) ? round(($amount / $grand_total) * 100, 1) : 0.0;

        // Định dạng dữ liệu chuẩn trực tiếp tương thích thư viện react-native-chart-kit PieChart
        $chart_data[] = [
            "category_id"     => (int)$cat['category_id'],
            "name"            => $cat['category_name'],
            "population"      => $amount, // react-native-chart-kit yêu cầu trường population
            "amount"          => $amount,
            "percentage"      => $percentage,
            "color"           => $cat['color_code'],
            "icon_name"       => $cat['icon_name'],
            "legendFontColor" => "#2B4C7E",
            "legendFontSize"  => 12
        ];
    }

    send_json_response("success", "Lấy dữ liệu biểu đồ chi tiêu thành công!", [
        "user_id"     => $user_id,
        "month"       => (int)$month,
        "year"        => (int)$year,
        "grand_total" => $grand_total,
        "chart_data"  => $chart_data
    ], 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi CSDL khi truy vấn biểu đồ chi tiêu: " . $e->getMessage(), null, 500);
}
?>
