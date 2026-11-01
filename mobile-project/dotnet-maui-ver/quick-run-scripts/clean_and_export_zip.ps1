$ErrorActionPreference = "Stop"
$projectRoot = Split-Path -Parent $PSScriptRoot
$destinationZip = Join-Path (Split-Path -Parent $projectRoot) "ExpenseTracker_MobileProject_Clean.zip"
$tempStagingDir = Join-Path $env:TEMP ("ExpenseTracker_Staging_" + [System.Guid]::NewGuid().ToString("N"))

Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "   DANG DONG GOI ZIP DU AN CHO FRONTEND (EXPENSE TRACKER)" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "Project Root: $projectRoot" -ForegroundColor Yellow
Write-Host "Output Zip:   $destinationZip" -ForegroundColor Yellow

try {
    # 1. Don dep cac thu muc build tam bin, obj, .vs
    Write-Host "`n[1/4] Dang don dep thu muc ruyet build (bin, obj, .vs)..." -ForegroundColor Green
    $junkFolders = @("bin", "obj", ".vs", ".vscode", ".idea", "TestResults")
    foreach ($folderName in $junkFolders) {
        Get-ChildItem -Path $projectRoot -Recurse -Directory -Filter $folderName -ErrorAction SilentlyContinue | ForEach-Object {
            Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
        }
    }

    # 2. Tao staging dir
    Write-Host "[2/4] Sao chep ma nguon sach sang thu muc tam..." -ForegroundColor Green
    New-Item -ItemType Directory -Path $tempStagingDir -Force | Out-Null

    $excludeDirs = @("bin", "obj", ".vs", ".vscode", ".idea", "TestResults")
    $excludeFiles = @("appsettings.Local.json", "*.user", "*.suo")

    $robocopyArgs = @(
        $projectRoot,
        $tempStagingDir,
        "/E",
        "/XD"
    ) + $excludeDirs + @("/XF") + $excludeFiles + @("/NFL", "/NDL", "/NJH", "/NJS", "/nc", "/ns", "/np")

    & robocopy @robocopyArgs | Out-Null

    # 3. Nen thanh file zip bang Compress-Archive
    Write-Host "[3/4] Dang nen du lieu thanh file ZIP (Compress-Archive)..." -ForegroundColor Green
    if (Test-Path $destinationZip) {
        Remove-Item -Path $destinationZip -Force
    }

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory($tempStagingDir, $destinationZip, [System.IO.Compression.CompressionLevel]::Optimal, $false)

    $zipItem = Get-Item $destinationZip
    $sizeMb = [Math]::Round($zipItem.Length / 1MB, 2)

    Write-Host "`n[4/4] HOAN TAT DONG GOI THANH CONG!" -ForegroundColor Cyan
    Write-Host "  -> File Zip: $destinationZip" -ForegroundColor Green
    Write-Host "  -> Dung luong: $sizeMb MB" -ForegroundColor Green
    Write-Host "  -> Bao mat: File appsettings.Local.json (chua API key) DA DUOC LOAI BO 100%." -ForegroundColor Green
    Write-Host "  -> File mau appsettings.Local.json.example da san sang cho nguoi nhan." -ForegroundColor Green
}
finally {
    if (Test-Path $tempStagingDir) {
        Remove-Item -Path $tempStagingDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}
