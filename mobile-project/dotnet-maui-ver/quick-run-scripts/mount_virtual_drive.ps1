# ==============================================================================
# EXPENSE TRACKER - MOUNT VIRTUAL DRIVE SCRIPT (FIX MAX_PATH FOR ANDROID)
# ==============================================================================
# Script này tạo ổ đĩa ảo X: trỏ thẳng vào thư mục dotnet-maui-ver (Root dự án).
# Giúp giải quyết triệt để 100% lỗi đường dẫn quá dài của Android AAPT2.
# Hoàn toàn tự động nhận diện thư mục hiện tại ($PSScriptRoot), hoạt động trên
# mọi máy tính của giảng viên hoặc sinh viên khi giải nén bài tập.
#
# Ổ đĩa ảo này nằm trên bộ nhớ RAM phiên làm việc của bạn:
# - Tự động biến mất khi bạn khởi động lại máy tính (Reboot).
# - Hoặc bạn có thể chủ động gỡ bằng lệnh: subst X: /D
# ==============================================================================

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " 🚀 KHOI TAO O DIA AO X: DE FIX LOI DUONG DAN DAI ANDROID " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# Tu dong tim thu muc goc du an (thu muc chua ExpenseTracker.slnx, ho tro du nguoi dung giai nen vao bat ky dau)
$currentDir = $PSScriptRoot
$targetPath = $null
while ($currentDir) {
    if (Test-Path (Join-Path $currentDir "ExpenseTracker.slnx")) {
        $targetPath = $currentDir
        break
    }
    $parent = Split-Path $currentDir -Parent
    if ($parent -eq $currentDir) { break }
    $currentDir = $parent
}
if (-not $targetPath) {
    $targetPath = Split-Path $PSScriptRoot -Parent
}

# 1. Kiem tra xem o X: da duoc mount chua, neu mount vao thu muc khac thi go ra gan lai
$currentSubst = subst
$xMount = $currentSubst | Where-Object { $_ -like "X:\:*" }

if ($xMount) {
    if ($xMount -like "*$targetPath*") {
        Write-Host "`n[V] O dia ao X: da duoc gan dung thu muc du an:" -ForegroundColor Green
        Write-Host "    $xMount" -ForegroundColor Gray
    } else {
        Write-Host "`n[!] O dia X: dang gan vao thu muc khac, dang thuc hien gan lai..." -ForegroundColor Yellow
        subst X: /D
        subst X: $targetPath
        Write-Host "[V] Da gan lai thanh cong o dia ao X: tro vao: $targetPath" -ForegroundColor Green
    }
} else {
    Write-Host "`n[1/2] Dang gan o dia ao X: tro vao thu muc du an..." -ForegroundColor Yellow
    subst X: $targetPath
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[V] Da gan thanh cong o dia ao X: tro vao: $targetPath" -ForegroundColor Green
    } else {
        Write-Host "[X] Khong the gan o dia X:. Vui long kiem tra xem o X: da bi chiem chua." -ForegroundColor Red
        exit 1
    }
}

# 2. Huong dan su dung
Write-Host "`n[2/2] DUONG DAN DU AN TAI O DIA AO X::" -ForegroundColor Cyan
Write-Host "    Solution tong the : X:\ExpenseTracker.slnx" -ForegroundColor White
Write-Host "    Frontend MAUI     : X:\frontend\ExpenseTracker.Maui\ExpenseTracker.Maui.csproj" -ForegroundColor White
Write-Host "    Backend API       : X:\backend\ExpenseTracker.slnx" -ForegroundColor White

Write-Host "`n💡 BAN CO THE:" -ForegroundColor Yellow
Write-Host "  1. Mo Visual Studio truc tiep bang tep: X:\ExpenseTracker.slnx" -ForegroundColor Gray
Write-Host "  2. Chon net10.0-android va may ao Android tren thanh cong cu roi nhan F5 de chay." -ForegroundColor Gray
Write-Host "  3. Khi nao muon go o dia ao, chi can go lenh: subst X: /D" -ForegroundColor Gray
Write-Host "`n==========================================================" -ForegroundColor Cyan

Write-Host "`nNhan phim Enter de dong cua so nay..." -ForegroundColor Gray
Read-Host

