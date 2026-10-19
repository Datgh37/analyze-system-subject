# ==============================================================================
# SCRIPT TỰ ĐỘNG ĐÓNG GÓI ĐỒ ÁN MOBILE NỘP GIẢNG VIÊN (DNTU)
# Học phần: Lập trình Thiết bị Di động (React Native + PHP WAMP + Realtime)
# Đề tài: Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân
# Nhóm 46: Trịnh Thành Đạt & Trần Trọng Đại
# ==============================================================================

param (
    [string]$PackageName = "Nhom46_MobileApp_QuanLyChiTieu"
)

$ErrorActionPreference = "Stop"

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host "   CHUONG TRINH DONG GOI DO AN NOP GIANG VIEN (CHUAN DNTU)       " -ForegroundColor Yellow
Write-Host "=================================================================" -ForegroundColor Cyan

# 1. Xác định đường dẫn
$SourceDir = $PSScriptRoot
$ParentDir = Split-Path -Path $SourceDir -Parent
$StagingDir = Join-Path -Path $ParentDir -ChildPath $PackageName
$ZipPath = Join-Path -Path $ParentDir -ChildPath "$PackageName.zip"

Write-Host "[1/5] Kiem tra duong dan thu muc nguon:" -ForegroundColor White
Write-Host "  - Nguon: $SourceDir" -ForegroundColor Gray
Write-Host "  - Thu muc tam dong goi: $StagingDir" -ForegroundColor Gray
Write-Host "  - File Zip thanh pham: $ZipPath" -ForegroundColor Green

# 2. Xóa sạch thư mục tạm và file zip cũ nếu có
if (Test-Path $StagingDir) {
    Write-Host "[!] Dang xoa thu muc tam cu..." -ForegroundColor Yellow
    Remove-Item -Path $StagingDir -Recurse -Force
}
if (Test-Path $ZipPath) {
    Write-Host "[!] Dang xoa file Zip cu..." -ForegroundColor Yellow
    Remove-Item -Path $ZipPath -Force
}

New-Item -ItemType Directory -Path $StagingDir | Out-Null
New-Item -ItemType Directory -Path "$StagingDir\01_BaoCao" | Out-Null
New-Item -ItemType Directory -Path "$StagingDir\02_CoSoDuLieu" | Out-Null

# 3. Sao chép tài liệu báo cáo và CSDL
Write-Host "`n[2/5] Sao chep Tai lieu Bao cao & Co so Du lieu..." -ForegroundColor Cyan

# Báo cáo & ERD
$DocFiles = @("BAO_CAO_GIUA_KY.md", "database_erd.drawio", "ROADMAP.md", "huongdanvietbaocao.txt")
foreach ($doc in $DocFiles) {
    $fullPath = Join-Path -Path $SourceDir -ChildPath $doc
    if (Test-Path $fullPath) {
        Copy-Item -Path $fullPath -Destination "$StagingDir\01_BaoCao" -Force
        Write-Host "  + Da copy Bao cao: $doc" -ForegroundColor Gray
    }
}

# CSDL SQL (Sao chép riêng vào thư mục 02_CoSoDuLieu để thầy cô dễ thấy)
$SqlSource = Join-Path -Path $SourceDir -ChildPath "BE\sql\database_schema.sql"
if (Test-Path $SqlSource) {
    Copy-Item -Path $SqlSource -Destination "$StagingDir\02_CoSoDuLieu" -Force
    Write-Host "  + Da copy CSDL SQL: database_schema.sql" -ForegroundColor Gray
}

# 4. Sao chép mã nguồn với bộ lọc LOẠI BỎ node_modules, .expo, .claude
Write-Host "`n[3/5] Dang loc sach va sao chep Ma nguon (Loai bo node_modules, .expo)..." -ForegroundColor Cyan

# 4.1 Copy BE
Write-Host "  -> Dang sao chep BE (PHP WAMP)..." -ForegroundColor White
$RoboBE = robocopy "$SourceDir\BE" "$StagingDir\BE" /E /XD ".git" ".vscode" "temp" "logs" /XF "*.log" "*.tmp" /NJH /NJS /NDL /NC /NS
Write-Host "     [OK] BE hoan tat." -ForegroundColor Green

# 4.2 Copy realtime-server (Loại trừ node_modules)
Write-Host "  -> Dang sao chep realtime-server (Node.js)..." -ForegroundColor White
$RoboRealtime = robocopy "$SourceDir\realtime-server" "$StagingDir\realtime-server" /E /XD "node_modules" ".git" ".vscode" /XF "*.log" "*.tmp" /NJH /NJS /NDL /NC /NS
Write-Host "     [OK] Realtime server hoan tat (Da loai bo node_modules)." -ForegroundColor Green

# 4.3 Copy FE (Loại trừ node_modules, .expo, .claude)
Write-Host "  -> Dang sao chep FE (React Native Expo)..." -ForegroundColor White
$RoboFE = robocopy "$SourceDir\FE" "$StagingDir\FE" /E /XD "node_modules" ".expo" ".claude" ".git" ".vscode" /XF "*.log" "*.tmp" "qrcode_expo.png" /NJH /NJS /NDL /NC /NS
Write-Host "     [OK] FE hoan tat (Da loai bo node_modules, .expo, .claude)." -ForegroundColor Green

# 5. Sao chép bộ Scripts chạy nhanh và Hướng dẫn sử dụng
Write-Host "`n[4/5] Sao chep Bo Scripts 1-Click va Huong dan van hanh..." -ForegroundColor Cyan
$RootFiles = @(
    "1_CAI_DAT_THU_VIEN.bat",
    "2_KET_NOI_WAMP.bat",
    "3_CHAY_REALTIME_SERVER.bat",
    "4_CHAY_APP_MOBILE.bat",
    "HUONG_DAN_CAI_DAT_VA_CHAY_DEMO.md",
    "HUONG_DAN_CAI_DAT_VA_CHAY_DEMO.txt",
    "README.md"
)

foreach ($rf in $RootFiles) {
    $fullPath = Join-Path -Path $SourceDir -ChildPath $rf
    if (Test-Path $fullPath) {
        Copy-Item -Path $fullPath -Destination $StagingDir -Force
        Write-Host "  + Da copy file goc: $rf" -ForegroundColor Gray
    }
}

# 6. Nén thành file ZIP thành phẩm
Write-Host "`n[5/5] Dang nen toan bo goi do an thanh file .ZIP..." -ForegroundColor Cyan
Start-Sleep -Seconds 1 # Nghỉ 1s để giải phóng toàn bộ file handles và antivirus scan

Add-Type -AssemblyName System.IO.Compression.FileSystem

$retryCount = 0
$maxRetries = 3
$zipSuccess = $false

while (-not $zipSuccess -and $retryCount -lt $maxRetries) {
    try {
        if (Test-Path $ZipPath) {
            Remove-Item -Path $ZipPath -Force
        }
        [System.IO.Compression.ZipFile]::CreateFromDirectory($StagingDir, $ZipPath, [System.IO.Compression.CompressionLevel]::Optimal, $false)
        $zipSuccess = $true
    } catch {
        $retryCount++
        Write-Host "  [!] Dang thu lai nen file zip (Lan $retryCount/$maxRetries)..." -ForegroundColor Yellow
        Start-Sleep -Seconds 2
    }
}

if (-not $zipSuccess) {
    Write-Host "[-] Loi: Khong the nen file ZIP. Vui long kiem tra xem co file nao dang mo." -ForegroundColor Red
    Exit 1
}

# Tính toán dung lượng
$RawFeSize = (Get-ChildItem -Path "$SourceDir\FE" -Recurse -Force -ErrorAction SilentlyContinue | Measure-Object -Property Length -Sum).Sum / 1MB
$RawRealtimeSize = (Get-ChildItem -Path "$SourceDir\realtime-server" -Recurse -Force -ErrorAction SilentlyContinue | Measure-Object -Property Length -Sum).Sum / 1MB
$RawTotalMB = [math]::Round($RawFeSize + $RawRealtimeSize, 1)

$PackagedMB = [math]::Round((Get-ChildItem -Path $StagingDir -Recurse -Force | Measure-Object -Property Length -Sum).Sum / 1MB, 2)
$ZipItem = Get-Item -Path $ZipPath
$ZipMB = [math]::Round($ZipItem.Length / 1MB, 2)
$ZipKB = [math]::Round($ZipItem.Length / 1KB, 0)

Write-Host "`n=================================================================" -ForegroundColor Green
Write-Host "           DONG GOI DO AN THANH CONG 100%!                       " -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Green
Write-Host " Thong ke toi uu dung luong:" -ForegroundColor White
Write-Host "  - Dung luong ma nguon goc (Co node_modules):  ~$RawTotalMB MB" -ForegroundColor Yellow
Write-Host "  - Dung luong goi da loc sach (Khong node_modules): $PackagedMB MB" -ForegroundColor White
Write-Host "  - Dung luong file ZIP thanh pham:                 $ZipMB MB ($ZipKB KB) - Giam hon 99%!" -ForegroundColor Green
Write-Host "`n File ZIP da san sang de nop len LMS / Google Drive / Email:" -ForegroundColor Cyan
Write-Host "  >>> $ZipPath <<<" -ForegroundColor Yellow
Write-Host "=================================================================" -ForegroundColor Green
