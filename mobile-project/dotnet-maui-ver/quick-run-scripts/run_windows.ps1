# ==============================================================================
# EXPENSE TRACKER - WINDOWS QUICK LAUNCHER & UNLOCK SCRIPT
# ==============================================================================
# Script này tự động giải phóng File Lock của Windows App SDK, hủy đăng ký Appx cũ,
# biên dịch sạch và khởi chạy ứng dụng .NET MAUI trên Windows Desktop.
# ==============================================================================

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " 🚀 KHOI CHAY EXPENSE TRACKER (.NET MAUI 10 WINDOWS APP)  " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Tu dong tim thu muc goc du an (thu muc chua ExpenseTracker.slnx)
$currentDir = $PSScriptRoot
$projectRoot = $null
while ($currentDir) {
    if (Test-Path (Join-Path $currentDir "ExpenseTracker.slnx")) {
        $projectRoot = $currentDir
        break
    }
    $parent = Split-Path $currentDir -Parent
    if ($parent -eq $currentDir) { break }
    $currentDir = $parent
}
if (-not $projectRoot) {
    $projectRoot = Split-Path $PSScriptRoot -Parent
}
Push-Location $projectRoot

# 2. Kiem tra Ha tang Backend (Docker PostgreSQL & WebApi)
Write-Host "`n[1/5] Kiem tra Ha tang Backend (CSDL PostgreSQL & WebApi)..." -ForegroundColor Yellow
$dockerRunning = $false
try {
    $dockerCheck = docker version --format '{{.Server.Version}}' 2>$null
    if ($dockerCheck) { $dockerRunning = $true }
} catch {}

if ($dockerRunning) {
    $pgContainer = docker ps --filter "name=expensetracker-postgres" --format "{{.Status}}" 2>$null
    if (-not $pgContainer) {
        Write-Host "    Container PostgreSQL chua bat. Dang tu dong chay 'docker compose up -d'..." -ForegroundColor Yellow
        docker compose -f "$projectRoot\backend\docker-compose.yml" up -d 2>$null | Out-Null
        Start-Sleep -Seconds 2
    }
    Write-Host "    [V] CSDL PostgreSQL tren Docker da san sang." -ForegroundColor Green
}

# Kiem tra Backend WebApi (Port 5018)
$backendRunning = $false
try {
    $tcpTest = Test-NetConnection -ComputerName "localhost" -Port 5018 -WarningAction SilentlyContinue
    if ($tcpTest.TcpTestSucceeded) { $backendRunning = $true }
} catch {}

if (-not $backendRunning) {
    Write-Host "    Backend WebApi (Port 5018) chua chay. Dang tu dong khoi dong Backend trong cua so moi..." -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-NoExit -Command Write-Host '=== EXPENSE TRACKER BACKEND WEBAPI (PORT 5018) ===' -ForegroundColor Cyan; cd '$projectRoot\backend\ExpenseTracker.WebApi'; dotnet run"
    Write-Host "    Dang cho Backend WebApi khoi dong..." -ForegroundColor Yellow
    $retry = 0
    while (-not $backendRunning -and $retry -lt 15) {
        Start-Sleep -Seconds 2
        try {
            $tcpTest = Test-NetConnection -ComputerName "localhost" -Port 5018 -WarningAction SilentlyContinue
            if ($tcpTest.TcpTestSucceeded) { $backendRunning = $true }
        } catch {}
        $retry++
    }
    if ($backendRunning) {
        Write-Host "    [V] Backend WebApi da ket noi thanh cong tai http://localhost:5018!" -ForegroundColor Green
    }
} else {
    Write-Host "    [V] Backend WebApi dang hoat dong tai http://localhost:5018." -ForegroundColor Green
}

# 3. Tat tien trinh cu neu dang chay de tranh khoa tap tin (File Lock)
Write-Host "`n[2/5] Kiem tra va dung tien trinh cu..." -ForegroundColor Yellow
Stop-Process -Name "ExpenseTracker.Maui" -Force -ErrorAction SilentlyContinue

# 4. Huy dang ky goi Appx cu de giai phong handle cua Windows AppModel tren thu muc bin\Debug
Write-Host "[3/5] Giai phong File Lock (Huy dang ky goi cu)..." -ForegroundColor Yellow
Get-AppxPackage *expensetracker* | Remove-AppxPackage -ErrorAction SilentlyContinue

# 5. Bien dich du an cho nen tang Windows
Write-Host "`n[4/5] Dang bien dich ExpenseTracker.Maui (net10.0-windows10.0.19041.0)..." -ForegroundColor Yellow
dotnet build "$projectRoot\frontend\ExpenseTracker.Maui\ExpenseTracker.Maui.csproj" -f net10.0-windows10.0.19041.0

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n[X] Bien dich that bai! Vui long kiem tra loi phia tren." -ForegroundColor Red
    Pop-Location
    exit 1
}

# 6. Dang ky goi va khoi chay ung dung
Write-Host "`n[5/5] Dang ky goi ung dung vao Windows va khoi chay..." -ForegroundColor Green

# Tim kiem tap tin AppxManifest.xml
$defaultManifest = "$projectRoot\frontend\ExpenseTracker.Maui\bin\Debug\net10.0-windows10.0.19041.0\win-x64\AppxManifest.xml"
$manifestPath = $null

if (Test-Path -LiteralPath $defaultManifest) {
    $manifestPath = $defaultManifest
} else {
    $found = Get-ChildItem -Path "$projectRoot\frontend\ExpenseTracker.Maui\bin\Debug" -Filter "AppxManifest.xml" -Recurse -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName -First 1
    if ($found -and (Test-Path -LiteralPath $found)) {
        $manifestPath = $found
    }
}

if ($manifestPath) {
    try {
        Add-AppxPackage -Register $manifestPath -ErrorAction Stop
    } catch {
        Write-Host "    [!] Canh bao dang ky Appx: $($_.Exception.Message)" -ForegroundColor Yellow
    }

    # Tu dong lay PackageFamilyName dong tu he thong (tranh hardcode PublisherId hash)
    $installedPkg = Get-AppxPackage *expensetracker* | Select-Object -First 1
    if ($installedPkg) {
        $aumid = "$($installedPkg.PackageFamilyName)!App"
        Start-Process "explorer.exe" -ArgumentList "shell:AppsFolder\$aumid"
        Write-Host "`n[V] UNG DUNG DA KHOI CHAY THANH CONG TREN MAN HINH!" -ForegroundColor Green
        Write-Host "    (Goi ung dung: $aumid)" -ForegroundColor Gray
    } else {
        # Fallback: Chay truc tiep file exe neu khong lay duoc package identity
        $exePath = Join-Path (Split-Path $manifestPath -Parent) "ExpenseTracker.Maui.exe"
        if (Test-Path -LiteralPath $exePath) {
            Start-Process $exePath
            Write-Host "`n[V] Da khoi chay truc tiep tap tin ExpenseTracker.Maui.exe!" -ForegroundColor Green
        } else {
            Write-Host "`n[X] Khong the khoi chay ung dung. Vui long kiem tra Start Menu." -ForegroundColor Red
        }
    }
} else {
    Write-Host "`n[X] Khong tim thay AppxManifest.xml trong thu muc bin\Debug." -ForegroundColor Red
    Write-Host "    Vui long kiem tra qua trinh bien dich o buoc [4/5]." -ForegroundColor Yellow
    Pop-Location
    exit 1
}

Pop-Location
Write-Host "`nNhan phim Enter de dong cua so nay..." -ForegroundColor Gray
Read-Host

