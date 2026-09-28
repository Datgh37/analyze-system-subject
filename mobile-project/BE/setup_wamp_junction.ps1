# ==============================================================================
# SCRIPT TỰ ĐỘNG THIẾT LẬP NTFS DIRECTORY JUNCTION CHO WAMP SERVER
# Dự án: Quản lý chi tiêu cá nhân (Mobile App Backend)
# Tác giả: Nhóm Đồ án Lập trình Mobile (Đạt & Đại)
# ==============================================================================

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host ">>> DANG KHOI TAO CAMS LIEN KET WAMP SERVER (DIRECTORY JUNCTION) <<<" -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan

# 1. Xác định thư mục nguồn (Thư mục BE hiện tại)
$SourcePath = $PSScriptRoot
Write-Host "[1/3] Thu muc nguon (Target BE): $SourcePath" -ForegroundColor Yellow

# 2. Tự động phát hiện thư mục www của WampServer
$PossibleWampPaths = @(
    "D:\WampServer\www",
    "C:\wamp64\www",
    "C:\wamp\www",
    "D:\wamp64\www",
    "D:\wamp\www",
    "E:\WampServer\www",
    "E:\wamp64\www"
)

$WampWwwPath = $null
foreach ($path in $PossibleWampPaths) {
    if (Test-Path $path) {
        $WampWwwPath = $path
        break
    }
}

# Nếu không tự động tìm thấy, yêu cầu người dùng nhập
if (-not $WampWwwPath) {
    Write-Host "[!] Khong tim thay thu muc WampServer/www mac dinh." -ForegroundColor Yellow
    $UserWampInput = Read-Host "Vui long nhap duong dan thu muc www cua WampServer tren may ban (Vi du: D:\WampServer\www)"
    if (Test-Path $UserWampInput) {
        $WampWwwPath = $UserWampInput
    } else {
        Write-Host "[-] Duong dan khong hop le hoac khong ton tai. Da huy thao tac!" -ForegroundColor Red
        Exit 1
    }
}

Write-Host "[2/3] Da tim thay thu muc Wamp www: $WampWwwPath" -ForegroundColor Green

# 3. Tạo đường dẫn đích Junction
$JunctionPath = Join-Path -Path $WampWwwPath -ChildPath "expense_api"

# Kiểm tra nếu đã tồn tại
if (Test-Path $JunctionPath) {
    $existingItem = Get-Item $JunctionPath
    Write-Host "[!] Thu muc $JunctionPath da ton tai san (LinkType: $($existingItem.LinkType))." -ForegroundColor Yellow
    $confirm = Read-Host "Ban co muon xoa lien ket cu va tao lai khong? (Y/N)"
    if ($confirm -eq 'Y' -or $confirm -eq 'y') {
        Remove-Item $JunctionPath -Force -Recurse
        Write-Host "[+] Da xoa lien ket cu thanh cong." -ForegroundColor Green
    } else {
        Write-Host "[+] Giu nguyen lien ket hien tai. Hoan tat!" -ForegroundColor Green
        Exit 0
    }
}

# Tiến hành tạo Junction
try {
    Write-Host "[3/3] Dang tao Junction: $JunctionPath -> $SourcePath" -ForegroundColor Cyan
    New-Item -ItemType Junction -Path $JunctionPath -Target $SourcePath | Out-Null
    Write-Host "`n>>> KET QUA: THIET LAP THANH CONG 100%! <<<" -ForegroundColor Green
    Write-Host "Bay gio ban co the:" -ForegroundColor White
    Write-Host "1. Bat WampServer (Icon xanh la cay)." -ForegroundColor White
    Write-Host "2. Goi API tai dia chi: http://localhost/expense_api/api/reports/monthly_summary.php?user_id=1&month=2026-09" -ForegroundColor White
    Write-Host "3. Moi thay doi trong thu muc BE se tu dong duoc Apache nhan dien 0ms!" -ForegroundColor White
} catch {
    Write-Host "[-] Co loi xay ra khi tao Junction: $_" -ForegroundColor Red
    Write-Host "Goi y: Hay chay PowerShell duoi quyen Administrator neu he dieu hanh yeu cau." -ForegroundColor Yellow
}
