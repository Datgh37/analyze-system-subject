<?php
/**
 * API: Báo cáo đối chiếu Thu nhập vs Chi tiêu qua các tháng (Biểu đồ cột Bar Chart)
 * Phương thức: GET
 * Params: ?user_id={id}&months_count={6}
 * Tác giả: Trịnh Thành Đạt (Lead)
 */

require_once __DIR__ . '/../../config/helpers.php';
require_once __DIR__ . '/../../config/database.php';

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    send_json_response("error", "Phương thức không được hỗ trợ. Vui lòng dùng GET.", null, 405);
}

// Lấy tham số truy vấn
$user_id      = filter_input(INPUT_GET, 'user_id', FILTER_VALIDATE_INT);
$months_count = filter_input(INPUT_GET, 'months_count', FILTER_VALIDATE_INT) ?? 6;

if (!$user_id) {
    send_json_response("error", "Thiếu hoặc sai định dạng tham số user_id.", null, 400);
}

// Giới hạn số tháng truy vấn từ 3 đến 12 tháng
if ($months_count < 3 || $months_count > 12) {
    $months_count = 6;
}

$database = new Database();
$db = $database->getConnection();

try {
    // 1. Tạo danh sách N tháng gần nhất tính ngược từ tháng hiện tại
    $months_list = [];
    $current_date = new DateTime();
    
    for ($i = $months_count - 1; $i >= 0; $i--) {
        $dt = (clone $current_date)->modify("-$i month");
        $m = (int)$dt->format('n');
        $y = (int)$dt->format('Y');
        $key = sprintf("%04d-%02d", $y, $m);
        
        $months_list[$key] = [
            "month"         => $m,
            "year"          => $y,
            "label"         => "T" . $m,
            "full_label"    => "Thg " . $m . "/" . $y,
            "total_income"  => 0.0,
            "total_expense" => 0.0,
            "net_savings"   => 0.0
        ];
    }

    // 2. Truy vấn gom nhóm thu chi trong khoảng thời gian N tháng gần nhất
    $earliest_key = array_key_first($months_list);
    $start_date   = $earliest_key . "-01";

    $stmt = $db->prepare("
        SELECT 
            DATE_FORMAT(transaction_date, '%Y-%m') AS ym,
            transaction_type,
            COALESCE(SUM(amount), 0) AS total_amount
        FROM `transactions`
        WHERE `user_id` = :uid
          AND `transaction_date` >= :start_date
        GROUP BY ym, transaction_type
        ORDER BY ym ASC
    ");
    $stmt->bindParam(':uid', $user_id);
    $stmt->bindParam(':start_date', $start_date);
    $stmt->execute();
    $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // 3. Đổ dữ liệu truy vấn vào danh sách tháng
    foreach ($rows as $row) {
        $ym = $row['ym'];
        if (isset($months_list[$ym])) {
            $amount = (float)$row['total_amount'];
            if ($row['transaction_type'] === 'INCOME') {
                $months_list[$ym]['total_income'] = $amount;
            } elseif ($row['transaction_type'] === 'EXPENSE') {
                $months_list[$ym]['total_expense'] = $amount;
            }
        }
    }

    // 4. Định dạng cấu trúc sẵn sàng cho thư viện react-native-chart-kit
    $chart_labels  = [];
    $income_series = [];
    $expense_series= [];
    $summary_list  = [];

    foreach ($months_list as $item) {
        $item['net_savings'] = $item['total_income'] - $item['total_expense'];
        $chart_labels[]      = $item['label'];
        $income_series[]     = $item['total_income'];
        $expense_series[]    = $item['total_expense'];
        $summary_list[]      = $item;
    }

    $response_data = [
        "user_id"       => $user_id,
        "months_count"  => $months_count,
        "chart_data"    => [
            "labels"        => $chart_labels,
            "income_series" => $income_series,
            "expense_series"=> $expense_series
        ],
        "monthly_details" => $summary_list
    ];

    send_json_response("success", "Lấy dữ liệu biểu đồ Thu - Chi thành công.", $response_data, 200);

} catch (PDOException $e) {
    send_json_response("error", "Lỗi truy vấn CSDL: " . $e->getMessage(), null, 500);
}
