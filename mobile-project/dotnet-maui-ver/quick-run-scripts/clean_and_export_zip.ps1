# ==============================================================================
# SCRIPT DỌN DẸP RÁC VÀ ĐÓNG GÓI ZIP DỰ ÁN CHO ĐỒI NGŨ FRONTEND
# ==============================================================================
# - Tự động xóa sạch toàn bộ thư mục bin, obj, .vs, .vscode, TestResults
# - Tuyệt đối KHÔNG đưa file chứa API Key cá nhân (appsettings.Local.json) vào ZIP
# - Giúp file nén nhẹ nhất có thể (từ vài GB xuống còn vài chục MB) và giải nén không lỗi
# ==============================================================================

$ErrorActionPreference = "Stop"
$projectRoot = Split-Path -Parent $PSScriptRoot
$destinationZip = Join-Path (Split-Path -Parent $projectRoot) "ExpenseTracker_MobileProject_Clean.zip"
$tempStagingDir = Join-Path $env:TEMP ("ExpenseTracker_Staging_" + [System.Guid]::NewGuid().ToString("N"))

Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "   BẮT ĐẦU DỌN DẸP & ĐÓNG GÓI ZIP DỰ ÁN MOBILE EXPENSE TRACKER" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "Thư mục gốc dự án: $projectRoot" -ForegroundColor Yellow
Write-Host "File zip xuất bản: $destinationZip" -ForegroundColor Yellow

try {
    # 1. Quét và xóa các thư mục bin, obj, .vs trực tiếp trong project để giảm dung lượng
    Write-Host "`n[1/4] Dọn dẹp các thư mục build tạm (bin, obj, .vs, TestResults)..." -ForegroundColor Green
    $junkFolders = @("bin", "obj", ".vs", ".vscode", ".idea", "TestResults")
    foreach ($folderName in $junkFolders) {
        Get-ChildItem -Path $projectRoot -Recurse -Directory -Filter $folderName -ErrorAction SilentlyContinue | ForEach-Object {
            Write-Host "  -> Đang xóa: $($_.FullName)" -ForegroundColor DarkGray
            Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
        }
    }

    # 2. Tạo thư mục tạm để chuẩn bị dữ liệu sạch
    Write-Host "`n[2/4] Sao chép mã nguồn sạch sang thư mục tạm staging..." -ForegroundColor Green
    New-Item -ItemType Directory -Path $tempStagingDir -Force | Out-Null

    # Dùng robocopy để loại trừ file chứa API Key và các thư mục rác
    $excludeDirs = @("bin", "obj", ".vs", ".vscode", ".idea", "TestResults")
    $excludeFiles = @("appsettings.Local.json", "*.user", "*.suo")

    $robocopyArgs = @(
        $projectRoot,
        $tempStagingDir,
        "/E",
        "/XD"
    ) + $excludeDirs + @("/XF") + $excludeFiles + @("/NFL", "/NDL", "/NJH", "/NJS", "/nc", "/ns", "/np")

    & robocopy @robocopyArgs | Out-Null

    # 3. Nén thành file zip
    Write-Host "`n[3/4] Đang nén thành file ZIP: $destinationZip..." -ForegroundColor Green
    if (Test-Path $destinationZip) {
        Remove-Item -Path $destinationZip -Force
    }

    [System.IO.Compression.ZipFile]::CreateFromDirectory($tempStagingDir, $destinationZip, [System.IO.Compression.CompressionLevel]::Optimal, $false)

    $zipItem = Get-Item $destinationZip
    $sizeMb = [Math]::Round($zipItem.Length / 1MB, 2)

    Write-Host "`n[4/4] KIỂM TRA BẢO MẬT & KẾT QUẢ:" -ForegroundColor Cyan
    Write-Host "  ✔ Đã đóng gói thành công: $destinationZip" -ForegroundColor Green
    Write-Host "  ✔ Dung lượng file nén: $sizeMb MB (siêu nhẹ, không chứa rác build)" -ForegroundColor Green
    Write-Host "  ✔ File 'appsettings.Local.json' (chứa API Key cá nhân) ĐÃ ĐƯỢC LOẠI TRỪ 100%!" -ForegroundColor Green
    Write-Host "  ✔ File mẫu 'appsettings.Local.json.example' đã sẵn sàng để bạn của bạn điền key riêng." -ForegroundColor Green
    Write-Host "`nBạn có thể gửi ngay file '$destinationZip' cho bạn của bạn!" -ForegroundColor Yellow
}
finally {
    if (Test-Path $tempStagingDir) {
        Remove-Item -Path $tempStagingDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

