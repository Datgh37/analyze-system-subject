# ==============================================================================
# EXPENSE TRACKER - WINDOWS QUICK LAUNCHER & UNLOCK SCRIPT
# ==============================================================================
# Script này tự động giải phóng File Lock của Windows App SDK, hủy đăng ký Appx cũ,
# biên dịch sạch và khởi chạy ứng dụng .NET MAUI trên Windows Desktop.
# ==============================================================================

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " 🚀 KHOI CHAY EXPENSE TRACKER (.NET MAUI 10 WINDOWS APP)  " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Tat tien trinh cu neu dang chay de tranh khoa tap tin (File Lock)
Write-Host "`n[1/4] Kiem tra va dung tien trinh cu..." -ForegroundColor Yellow
Stop-Process -Name "ExpenseTracker.Maui" -Force -ErrorAction SilentlyContinue

# 2. Huy dang ky goi Appx cu de giai phong handle cua Windows AppModel tren thu muc bin\Debug
Write-Host "[2/4] Giai phong File Lock (Huy dang ky goi cu)..." -ForegroundColor Yellow
Get-AppxPackage *expensetracker* | Remove-AppxPackage -ErrorAction SilentlyContinue

# 3. Bien dich du an cho nen tang Windows
Write-Host "[3/4] Dang bien dich ExpenseTracker.Maui (net10.0-windows10.0.19041.0)..." -ForegroundColor Yellow
dotnet build frontend/ExpenseTracker.Maui/ExpenseTracker.Maui.csproj -f net10.0-windows10.0.19041.0

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n[X] Bien dich that bai! Vui long kiem tra loi phia tren." -ForegroundColor Red
    exit 1
}

# 4. Dang ky goi va khoi chay ung dung
Write-Host "[4/4] Dang ky goi ung dung vao Windows va khoi chay..." -ForegroundColor Green
$manifestPath = ".\frontend\ExpenseTracker.Maui\bin\Debug\net10.0-windows10.0.19041.0\win-x64\AppxManifest.xml"

if (Test-Path $manifestPath) {
    Add-AppxPackage -Register $manifestPath
    Start-Process "explorer.exe" -ArgumentList "shell:AppsFolder\com.companyname.expensetracker.maui_9zz4h110yvjzm!App"
    Write-Host "`n[V] UNG DUNG DA KHOI CHAY THANH CONG TREN MAN HINH!" -ForegroundColor Green
    Write-Host "    (Ban cung co the mo ung dung bat ky luc nao tu Start Menu: go 'ExpenseTracker.Maui')" -ForegroundColor Gray
} else {
    Write-Host "`n[X] Khong tim thay AppxManifest.xml tai: $manifestPath" -ForegroundColor Red
    exit 1
}

Write-Host "`nNhan phim Enter de dong cua so nay..." -ForegroundColor Gray
Read-Host

