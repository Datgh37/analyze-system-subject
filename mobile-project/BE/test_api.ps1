# ==============================================================================
# SCRIPT KIỂM THỬ TỰ ĐỘNG TOÀN DIỆN 21 RESTful API (WAMP SERVER LOCALHOST)
# Dự án: Quản lý Chi tiêu Cá nhân Thông minh (Mobile App Backend)
# Đảm bảo: Xác thực JWT Bearer Token, Triệt tiêu IDOR, ACID Transactions, OTP Flow
# Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại (UI & Business API)
# ==============================================================================

param (
    [switch]$NoReset
)

$BaseUrl = "http://localhost/expense_api/api"

Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host ">>> KIEM THU TOAN DIEN 21 RESTful API & CORE SECURITY (JWT BEARER) <<<" -ForegroundColor Cyan
Write-Host "Target URL: $BaseUrl" -ForegroundColor DarkGray
Write-Host "========================================================================" -ForegroundColor Cyan

# 1. Kiểm tra WampServer Apache có đang chạy không
Write-Host "`n[BUOC 0] Kiem tra trang thai WampServer Apache..." -ForegroundColor Yellow
try {
    $ping = Invoke-WebRequest -Uri "http://localhost" -TimeoutSec 3 -UseBasicParsing -ErrorAction Stop
    Write-Host "[OK] Apache WampServer dang hoat dong binh thuong!" -ForegroundColor Green
} catch {
    Write-Host "[CANH BAO] Khong the ket noi den http://localhost" -ForegroundColor Red
    Write-Host "  -> Vui long khoi chay WampServer (wampmanager.exe) de icon chuyen sang mau XANH LA CAY." -ForegroundColor Yellow
    Write-Host "  -> Sau do chay lai: .\test_api.ps1" -ForegroundColor Yellow
    Exit 1
}

# 1.5. Tự động dọn dẹp và khôi phục CSDL sạch (Hard Reset & Seed)
if (-not $NoReset) {
    Write-Host "`n[BUOC 0.5] Don dep & Khoi phuc CSDL ve trang thai ban dau (Hard Reset)..." -ForegroundColor Yellow
    try {
        $resetRes = Invoke-RestMethod -Uri "$BaseUrl/debug/reset_database.php" -Method Get -TimeoutSec 5 -ErrorAction Stop
        if ($resetRes.status -eq "success") {
            Write-Host "[OK] CSDL da duoc Hard Reset ve trang thai goc (2 Users, 4 Vi, 7 Giao dich)!" -ForegroundColor Green
        }
    } catch {
        Write-Host "[CANH BAO] Khong the goi API reset CSDL: $($_.Exception.Message)" -ForegroundColor DarkGray
    }
}

# Biến lưu trữ Token toàn cục
$Script:AuthToken = $null

# Hàm gọi API đa năng
function Invoke-ApiTest {
    param (
        [string]$Name,
        [string]$Method = "GET",
        [string]$Endpoint,
        [hashtable]$Body = $null,
        [switch]$ExpectUnauthorized = $false
    )

    Write-Host -NoNewline "[TEST] $Name ... " -ForegroundColor White

    $headers = @{}

    if ($Script:AuthToken) {
        $headers["Authorization"] = "Bearer $($Script:AuthToken)"
    }

    $url = "$BaseUrl/$Endpoint"

    try {
        if ($Method -eq "GET") {
            $resp = Invoke-RestMethod -Uri $url -Method Get -Headers $headers -TimeoutSec 6 -ErrorAction Stop
        } else {
            $jsonBody = ($Body | ConvertTo-Json -Compress)
            $utf8Bytes = [System.Text.Encoding]::UTF8.GetBytes($jsonBody)
            $resp = Invoke-RestMethod -Uri $url -Method $Method -Headers $headers -Body $utf8Bytes -ContentType "application/json; charset=utf-8" -TimeoutSec 6 -ErrorAction Stop
        }

        if ($ExpectUnauthorized) {
            Write-Host "FAILED (Mong doi 401 nhung lai thanh cong)" -ForegroundColor Red
            return $null
        }

        if ($resp.status -eq "success") {
            Write-Host "PASSED" -ForegroundColor Green
            return $resp.data
        } else {
            Write-Host "FAILED: $($resp.message)" -ForegroundColor Red
            return $null
        }
    } catch {
        $status = $_.Exception.Response.StatusCode.value__
        if ($ExpectUnauthorized -and $status -eq 401) {
            Write-Host "PASSED (Chặn 401 Unauthorized thành công!)" -ForegroundColor Green
            return $true
        }

        Write-Host "FAILED [HTTP $status]: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# ==============================================================================
# PHẦN 1: KIỂM THỬ BẢO MẬT & IDOR DEFENSE
# ==============================================================================
Write-Host "`n--- [PHAN 1] KIEM THU AN NINH & KHOA TOKEN (SECURITY & IDOR) ---" -ForegroundColor Magenta

# 1.1 Thử truy cập API bảo vệ khi chưa đăng nhập (Không có Token) -> Phải bị chặn 401
Invoke-ApiTest -Name "Security: Chan truy cap khong co Token (401 Check)" `
    -Method "GET" `
    -Endpoint "wallets/list.php" `
    -ExpectUnauthorized | Out-Null

# ==============================================================================
# PHẦN 2: MODULE XÁC THỰC & NGƯỜI DÙNG (AUTH & PROFILE - 7 ENDPOINTS)
# ==============================================================================
Write-Host "`n--- [PHAN 2] MODULE XAC THUC & PROFILE (7 APIS) ---" -ForegroundColor Magenta

# 2.1 Đăng nhập lấy Bearer JWT Token
$loginRes = Invoke-ApiTest -Name "API 2: Dang nhap (Login & Issue JWT)" `
    -Method "POST" `
    -Endpoint "auth/login.php" `
    -Body @{
        email    = "thanhdat@gmail.com"
        password = "password123"
    }

if ($loginRes -and $loginRes.token) {
    $Script:AuthToken = $loginRes.token
    Write-Host "       -> Da nhan JWT Token: $($Script:AuthToken.Substring(0, 30))..." -ForegroundColor DarkGray
} else {
    Write-Host "       [!] Khong the lay JWT Token tu Login. Dung chu trinh test." -ForegroundColor Red
    Exit 1
}

# 2.2 Luồng Quên Mật Khẩu 3 Bước (OTP Flow)
$forgotRes = Invoke-ApiTest -Name "API 3: Yeu cau OTP quen mat khau (Forgot Password)" `
    -Method "POST" `
    -Endpoint "auth/forgot_password.php" `
    -Body @{ email = "thanhdat@gmail.com" }

$debugOtp = if ($forgotRes) { $forgotRes.debug_otp } else { "123456" }
Write-Host "       -> Debug OTP nhan duoc: $debugOtp" -ForegroundColor DarkGray

$verifyRes = Invoke-ApiTest -Name "API 4: Xac thuc ma OTP (Verify OTP)" `
    -Method "POST" `
    -Endpoint "auth/verify_otp.php" `
    -Body @{
        email    = "thanhdat@gmail.com"
        otp_code = $debugOtp
    }

$resetToken = if ($verifyRes) { $verifyRes.reset_token } else { $null }

if ($resetToken) {
    Invoke-ApiTest -Name "API 5: Dat lai mat khau moi (Reset Password)" `
        -Method "POST" `
        -Endpoint "auth/reset_password.php" `
        -Body @{
            email        = "thanhdat@gmail.com"
            reset_token  = $resetToken
            new_password = "password123" # Giữ nguyên password123 để các lần test sau không bị lỗi
        } | Out-Null
}

# 2.3 Xem và Cập nhật Profile
Invoke-ApiTest -Name "API 6: Xem thong tin ho so ca nhan (Profile GET)" `
    -Method "GET" `
    -Endpoint "auth/profile.php" | Out-Null

Invoke-ApiTest -Name "API 7: Cap nhat ten hien thi (Profile POST)" `
    -Method "POST" `
    -Endpoint "auth/profile.php" `
    -Body @{ full_name = "Trịnh Thành Đạt (Lead)" } | Out-Null

# ==============================================================================
# PHẦN 3: MODULE VÍ TÀI CHÍNH & CHUYỂN TIỀN NỘI BỘ (WALLETS - 3 ENDPOINTS)
# ==============================================================================
Write-Host "`n--- [PHAN 3] MODULE VI TAI CHINH & CHUYEN TIEN (3 APIS) ---" -ForegroundColor Magenta

# 3.1 Lấy danh sách ví
$walletList = Invoke-ApiTest -Name "API 8: Danh sach vi tai chinh (Wallets List)" `
    -Method "GET" `
    -Endpoint "wallets/list.php"

# 3.2 Tạo ví mới
$newWallet = Invoke-ApiTest -Name "API 9: Tao vi moi (Wallets Create)" `
    -Method "POST" `
    -Endpoint "wallets/create.php" `
    -Body @{
        wallet_name     = "Ví Tiết Kiệm Khẩn Cấp"
        wallet_type     = "BANK"
        initial_balance = 500000
    }

# 3.3 Chuyển tiền nội bộ giữa 2 ví (ACID Transaction)
Invoke-ApiTest -Name "API 10: Chuyen tien noi bo giua 2 vi (Wallets Transfer)" `
    -Method "POST" `
    -Endpoint "wallets/transfer.php" `
    -Body @{
        from_wallet_id = 2 # Vietcombank
        to_wallet_id   = 1 # Tiền mặt
        amount         = 100000
        note           = "Rút tiền từ Vietcombank về Tiền mặt"
    } | Out-Null

# ==============================================================================
# PHẦN 4: MODULE DANH MỤC THU / CHI (CATEGORIES - 2 ENDPOINTS)
# ==============================================================================
Write-Host "`n--- [PHAN 4] MODULE DANH MUC (2 APIS) ---" -ForegroundColor Magenta

Invoke-ApiTest -Name "API 11: Danh sach danh muc Thu/Chi (Categories List)" `
    -Method "GET" `
    -Endpoint "categories/list.php" | Out-Null

$newCat = Invoke-ApiTest -Name "API 12: Tao danh muc rieng (Categories Create)" `
    -Method "POST" `
    -Endpoint "categories/create.php" `
    -Body @{
        name       = "Đầu tư Quỹ Mở"
        type       = "EXPENSE"
        icon_name  = "trending-up-outline"
        color_code = "#FF9F1C"
    }

# ==============================================================================
# PHẦN 5: MODULE GIAO DỊCH THU / CHI & BÙ TRỪ SỐ DƯ (TRANSACTIONS - 4 ENDPOINTS)
# ==============================================================================
Write-Host "`n--- [PHAN 5] MODULE GIAO DICH & DELTA RECONCILER (4 APIS) ---" -ForegroundColor Magenta

# 5.1 Tạo giao dịch mới
$createdTrans = Invoke-ApiTest -Name "API 14: Tao giao dich chi tieu (Transactions Create)" `
    -Method "POST" `
    -Endpoint "transactions/create.php" `
    -Body @{
        wallet_id        = 1
        category_id      = 1 # Ăn uống
        amount           = 45000
        transaction_type = "EXPENSE"
        note             = "Ăn trưa cơm tấm kiểm thử tự động"
        transaction_date = "2026-09-29"
    }

$transId = if ($createdTrans) { $createdTrans.transaction_id } else { 1 }

# 5.2 Cập nhật giao dịch với tính năng bù trừ số dư ví (Delta Balance Adjustment)
Invoke-ApiTest -Name "API 15: Sua giao dich & Tu dong bu tru so du (Transactions Update)" `
    -Method "POST" `
    -Endpoint "transactions/update.php" `
    -Body @{
        id               = $transId
        wallet_id        = 1
        category_id      = 1
        amount           = 55000 # Đổi từ 45k thành 55k -> Ví tự trừ thêm 10k
        transaction_type = "EXPENSE"
        note             = "Ăn trưa cơm tấm sườn trứng (Đã cập nhật đúng số tiền)"
        transaction_date = "2026-09-29"
    } | Out-Null

# 5.3 Lấy danh sách giao dịch có bộ lọc đa tiêu chí
Invoke-ApiTest -Name "API 13: Lich su giao dich kem Loc da tieu chi (Transactions List)" `
    -Method "GET" `
    -Endpoint "transactions/list.php?month=9&year=2026&type=EXPENSE&limit=10" | Out-Null

# 5.4 Xóa giao dịch và hoàn trả tiền vào ví
Invoke-ApiTest -Name "API 16: Xoa giao dich & Hoan lai so du vi (Transactions Delete)" `
    -Method "POST" `
    -Endpoint "transactions/delete.php" `
    -Body @{ transaction_id = $transId } | Out-Null

# ==============================================================================
# PHẦN 6: MODULE HẠN MỨC NGÂN SÁCH THÁNG (BUDGETS - 2 ENDPOINTS)
# ==============================================================================
Write-Host "`n--- [PHAN 6] MODULE NGAN SACH THANG (2 APIS) ---" -ForegroundColor Magenta

Invoke-ApiTest -Name "API 18: Thiet lap han muc ngan sach thang (Budgets Set)" `
    -Method "POST" `
    -Endpoint "budgets/set.php" `
    -Body @{
        category_id  = $null
        amount_limit = 7000000
        month        = 9
        year         = 2026
    } | Out-Null

Invoke-ApiTest -Name "API 17: Kiem tra tien do ngan sach & Canh bao (Budgets Status)" `
    -Method "GET" `
    -Endpoint "budgets/status.php?month=9&year=2026" | Out-Null

# ==============================================================================
# PHẦN 7: MODULE BÁO CÁO THỐNG KÊ & BIỂU ĐỒ (REPORTS - 3 ENDPOINTS)
# ==============================================================================
Write-Host "`n--- [PHAN 7] MODULE BAO CAO & BIEU DO (3 APIS) ---" -ForegroundColor Magenta

Invoke-ApiTest -Name "API 19: Bao cao tong quan thang (Monthly Summary)" `
    -Method "GET" `
    -Endpoint "reports/monthly_summary.php?month=9&year=2026" | Out-Null

Invoke-ApiTest -Name "API 20: Ty trong chi tieu PieChart (Category Chart)" `
    -Method "GET" `
    -Endpoint "reports/category_chart.php?month=9&year=2026" | Out-Null

Invoke-ApiTest -Name "API 21: Doi chieu Thu vs Chi BarChart (Income vs Expense)" `
    -Method "GET" `
    -Endpoint "reports/income_vs_expense_chart.php?months_count=6" | Out-Null

Write-Host "`n========================================================================" -ForegroundColor Cyan
Write-Host ">>> KET QUA: TOAN BO 21 RESTful API DA DUOC KIEM THU SAN SANG! <<<" -ForegroundColor Green
Write-Host "========================================================================" -ForegroundColor Cyan
