# ==============================================================================
# SCRIPT KIỂM THỬ TỰ ĐỘNG TOÀN BỘ BACKEND RESTful API (WAMP SERVER LOCALHOST)
# Dự án: Quản lý chi tiêu cá nhân (Mobile App Backend)
# Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại (UI & API)
# ==============================================================================

$BaseUrl = "http://localhost/expense_api/api"

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host ">>> DANG KIEM THU TOAN BO RESTful API TAI: $BaseUrl <<<" -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan

# Hàm gọi API và hiển thị kết quả kiểm thử
function Test-Endpoint {
    param (
        [string]$Name,
        [string]$Method = "GET",
        [string]$Url,
        [hashtable]$Body = $null
    )

    Write-Host -NoNewline "[TEST] $Name ... " -ForegroundColor White
    try {
        $headers = @{ "Content-Type" = "application/json" }
        if ($Method -eq "GET") {
            $response = Invoke-RestMethod -Uri $Url -Method Get -Headers $headers -TimeoutSec 5
        } else {
            $jsonBody = ($Body | ConvertTo-Json -Compress)
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Headers $headers -Body $jsonBody -TimeoutSec 5
        }

        if ($response.status -eq "success") {
            Write-Host "PASSED (HTTP 200/201 OK)" -ForegroundColor Green
            return $response.data
        } else {
            Write-Host "FAILED: $($response.message)" -ForegroundColor Red
            return $null
        }
    } catch {
        Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# 1. Kiểm tra WAMP có đang bật không
Write-Host "[1/8] Kiem tra ket noi den Apache WampServer..." -ForegroundColor Yellow
try {
    $ping = Invoke-WebRequest -Uri "http://localhost" -TimeoutSec 3 -UseBasicParsing
    Write-Host "[+] WampServer Apache dang hoat dong binh thuong!" -ForegroundColor Green
} catch {
    Write-Host "[-] KHONG THE KET NOI DEN http://localhost." -ForegroundColor Red
    Write-Host "    Vui long bat WampServer (wampmanager.exe) de icon chuyen sang mau XANH LA CAY truoc khi test." -ForegroundColor Yellow
    Exit 1
}

# 2. Test Đăng nhập
$loginData = Test-Endpoint -Name "Auth: Dang nhap (Login)" -Method "POST" -Url "$BaseUrl/auth/login.php" -Body @{
    email    = "thanhdat@gmail.com"
    password = "password123"
}

$userId = if ($loginData) { $loginData.user.id } else { 1 }

# 3. Test Lấy danh sách ví
Test-Endpoint -Name "Wallets: Danh sach vi (List)" -Method "GET" -Url "$BaseUrl/wallets/list.php?user_id=$userId" | Out-Null

# 4. Test Lấy danh sách danh mục
Test-Endpoint -Name "Categories: Danh muc (List)" -Method "GET" -Url "$BaseUrl/categories/list.php?user_id=$userId" | Out-Null

# 5. Test Báo cáo tổng quan tháng
Test-Endpoint -Name "Reports: Tong quan thang 9/2026 (Monthly Summary)" -Method "GET" -Url "$BaseUrl/reports/monthly_summary.php?user_id=$userId&month=2026-09" | Out-Null

# 6. Test Báo cáo tỷ trọng chi tiêu (Pie Chart)
Test-Endpoint -Name "Reports: Ty trong chi tieu PieChart (Category Chart)" -Method "GET" -Url "$BaseUrl/reports/category_chart.php?user_id=$userId&month=9&year=2026" | Out-Null

# 7. Test Báo cáo Thu vs Chi qua các tháng (Bar Chart)
Test-Endpoint -Name "Reports: Thu vs Chi BarChart (Income vs Expense)" -Method "GET" -Url "$BaseUrl/reports/income_vs_expense_chart.php?user_id=$userId&months_count=6" | Out-Null

# 8. Test Lấy lịch sử giao dịch
Test-Endpoint -Name "Transactions: Lich su giao dich (List)" -Method "GET" -Url "$BaseUrl/transactions/list.php?user_id=$userId&limit=5" | Out-Null

Write-Host "`n=================================================================" -ForegroundColor Cyan
Write-Host ">>> HOAN TAT KIEM THU BACKEND RESTful API! <<<" -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Cyan
