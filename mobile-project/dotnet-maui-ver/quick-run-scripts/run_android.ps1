# ==============================================================================
# EXPENSE TRACKER - ANDROID BUILD & RUN SCRIPT
# ==============================================================================
# Script này tự động kiểm tra ổ ảo X:, biên dịch dự án cho Android và hỗ trợ
# cài đặt lên thiết bị/máy ảo đang kết nối qua ADB.
# ==============================================================================

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " 🚀 BIEN DICH VA KHOI CHAY ANDROID (NET 10.0-ANDROID)     " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Tu dong tim thu muc goc du an (thu muc chua ExpenseTracker.slnx) va gan o dia ao X:
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
$targetPath = $projectRoot

$currentSubst = subst
$xMount = $currentSubst | Where-Object { $_ -like "X:\:*" }

if ($xMount) {
    if ($xMount -like "*$targetPath*") {
        Write-Host "[1/5] O dia ao X: da gan dung thu muc du an." -ForegroundColor Green
    } else {
        Write-Host "[1/5] O dia X: dang gan vao thu muc khac, dang gan lai..." -ForegroundColor Yellow
        subst X: /D
        subst X: $targetPath
        Write-Host "[1/5] Da gan lai thanh cong o dia ao X: tro vao: $targetPath" -ForegroundColor Green
    }
} else {
    Write-Host "[1/5] O dia X: chua gan. Dang tu dong gan o dia ao X:..." -ForegroundColor Yellow
    subst X: $targetPath
    Write-Host "[1/5] Da gan thanh cong o dia ao X: tro vao: $targetPath" -ForegroundColor Green
}

# 2. Kiem tra va Khoi dong Ha tang Backend (Docker PostgreSQL & ASP.NET Core WebApi)
Write-Host "`n[2/5] Kiem tra Ha tang Backend (CSDL PostgreSQL & WebApi)..." -ForegroundColor Yellow
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
} else {
    Write-Host "    [!] Docker Desktop chua bat hoac khong co san. Bo qua khoi dong container..." -ForegroundColor Gray
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
    } else {
        Write-Host "    [!] Backend WebApi dang tiep tuc khoi dong..." -ForegroundColor Yellow
    }
} else {
    Write-Host "    [V] Backend WebApi dang hoat dong tai http://localhost:5018." -ForegroundColor Green
}

# 3. Tu dong do tim cong cu Android SDK, ADB va May ao tren he thong
Write-Host "`n[3/5] Tu dong do tim Android SDK, ADB va May ao (AVD)..." -ForegroundColor Yellow
$driveRoots = Get-PSDrive -PSProvider FileSystem | Select-Object -ExpandProperty Root

# 2.1. Tu dong do tim cong cu adb.exe tren he thong
$candidateAdb = @(
    (Get-Command "adb" -ErrorAction SilentlyContinue).Source,
    "$env:ANDROID_HOME\platform-tools\adb.exe",
    "$env:ANDROID_SDK_ROOT\platform-tools\adb.exe",
    "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe",
    "C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe",
    "C:\Program Files\Android\android-sdk\platform-tools\adb.exe",
    "C:\Android\android-sdk\platform-tools\adb.exe",
    "C:\Android\Sdk\platform-tools\adb.exe"
)
foreach ($root in $driveRoots) {
    $candidateAdb += "$($root)Android\Sdk\platform-tools\adb.exe"
    $candidateAdb += "$($root)android-sdk\platform-tools\adb.exe"
    $candidateAdb += "$($root)AndroidStudioSDK\platform-tools\adb.exe"
    $foundDirs = Get-ChildItem -Path $root -Directory -Filter "*Android*" -ErrorAction SilentlyContinue
    foreach ($dir in $foundDirs) {
        $candidateAdb += Join-Path $dir.FullName "platform-tools\adb.exe"
        $candidateAdb += Join-Path $dir.FullName "AndroidStudioSDK\platform-tools\adb.exe"
    }
}
$adbPath = $candidateAdb | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1

# 2.2. Tu dong do tim cong cu emulator.exe tren he thong
$candidateEmulator = @(
    (Get-Command "emulator" -ErrorAction SilentlyContinue).Source,
    "$env:ANDROID_HOME\emulator\emulator.exe",
    "$env:ANDROID_SDK_ROOT\emulator\emulator.exe",
    "$env:LOCALAPPDATA\Android\Sdk\emulator\emulator.exe",
    "C:\Program Files (x86)\Android\android-sdk\emulator\emulator.exe",
    "C:\Program Files\Android\emulator\emulator.exe",
    "C:\Android\android-sdk\emulator\emulator.exe",
    "C:\Android\Sdk\emulator\emulator.exe"
)
if ($adbPath) {
    $sdkDir = Split-Path (Split-Path $adbPath -Parent) -Parent
    $candidateEmulator += Join-Path $sdkDir "emulator\emulator.exe"
}
foreach ($root in $driveRoots) {
    $candidateEmulator += "$($root)Android\Sdk\emulator\emulator.exe"
    $candidateEmulator += "$($root)android-sdk\emulator\emulator.exe"
    $foundDirs = Get-ChildItem -Path $root -Directory -Filter "*Android*" -ErrorAction SilentlyContinue
    foreach ($dir in $foundDirs) {
        $candidateEmulator += Join-Path $dir.FullName "emulator\emulator.exe"
        $candidateEmulator += Join-Path $dir.FullName "AndroidStudioSDK\emulator\emulator.exe"
    }
}
$emulatorPath = $candidateEmulator | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1

# 2.3. Tu dong do tim thu muc luu tru may ao (AVD Home) tren moi may
$candidateAvdHome = @(
    $env:ANDROID_AVD_HOME,
    "$env:USERPROFILE\.android\avd",
    "$env:HOME\.android\avd",
    "$env:HOMEDRIVE$env:HOMEPATH\.android\avd"
)
foreach ($root in $driveRoots) {
    $candidateAvdHome += "$($root).android\avd"
    $foundDirs = Get-ChildItem -Path $root -Directory -Filter "*Android*" -ErrorAction SilentlyContinue
    foreach ($dir in $foundDirs) {
        $candidateAvdHome += Join-Path $dir.FullName ".android\avd"
        $candidateAvdHome += Join-Path $dir.FullName "avd"
    }
}
$detectedAvdHome = $candidateAvdHome | Where-Object { 
    $_ -and (Test-Path $_) -and (Get-ChildItem -Path $_ -Filter "*.ini" -ErrorAction SilentlyContinue) 
} | Select-Object -First 1

if ($detectedAvdHome) {
    $env:ANDROID_AVD_HOME = $detectedAvdHome
}
if ($emulatorPath) {
    $detectedSdkRoot = Split-Path (Split-Path $emulatorPath -Parent) -Parent
    $env:ANDROID_HOME = $detectedSdkRoot
    $env:ANDROID_SDK_ROOT = $detectedSdkRoot
    $env:PATH = (Split-Path $adbPath -Parent) + ";" + (Split-Path $emulatorPath -Parent) + ";" + $env:PATH
}

# 2.3. Tu dong do tim JDK 21 (yeu cau boi .NET Android SDK, tranh dung JDK 26)
$candidateJdkDirs = @(
    "C:\Program Files\Microsoft",
    "C:\Program Files\Eclipse Adoptium",
    "C:\Program Files\Java",
    "D:\Program Files\Java"
)
$detectedJdk = Get-ChildItem -Path $candidateJdkDirs -Filter "*21*" -Directory -ErrorAction SilentlyContinue | Select-Object -First 1
if ($detectedJdk) {
    $env:JAVA_HOME = $detectedJdk.FullName
    $env:PATH = (Join-Path $detectedJdk.FullName "bin") + ";" + $env:PATH
}

# 4. Kiem tra va Tu dong Khoi dong May ao Android
if ($adbPath) {
    $devices = & $adbPath devices | Where-Object { $_ -and -not $_.StartsWith("List") -and $_.Contains("device") }

    # Neu chua co thiet bi nao bat, thu tu dong khoi dong May ao (Emulator)
    if (-not $devices -and $emulatorPath) {
        $rawAvdList = & $emulatorPath -list-avds 2>$null
        $avdList = @($rawAvdList | Where-Object { $_ -and -not $_.StartsWith("=") -and -not $_.Contains(" ") } | ForEach-Object { $_.Trim() })
        if ($avdList.Count -gt 0) {
            $targetAvd = $avdList[0]
            Write-Host "`n[4/5] Phat hien May ao Android: '$targetAvd'. Dang tu dong bat len..." -ForegroundColor Yellow
            Start-Process $emulatorPath -ArgumentList "-avd $targetAvd"
            
            Write-Host "    Dang cho may ao ket noi ADB..." -ForegroundColor Yellow
            & $adbPath wait-for-device
            
            Write-Host "    Dang cho he dieu hanh Android boot xong (sys.boot_completed)..." -ForegroundColor Yellow
            $booted = ""
            $retryCount = 0
            while ($booted -ne "1" -and $retryCount -lt 90) {
                Start-Sleep -Seconds 2
                $booted = (& $adbPath shell getprop sys.boot_completed 2>$null)
                if ($booted) { $booted = $booted.Trim() }
                $retryCount++
            }
            if ($booted -eq "1") {
                Write-Host "[V] May ao Android '$targetAvd' da khoi dong xong!" -ForegroundColor Green
            } else {
                Write-Host "[!] May ao Android '$targetAvd' dang tiep tuc boot..." -ForegroundColor Yellow
            }
            $devices = & $adbPath devices | Where-Object { $_ -and -not $_.StartsWith("List") -and $_.Contains("device") }
        }
    }

    # 5. Bien dich, Deploy va Khoi chay len thiet bi
    if ($devices) {
        Write-Host "`n[5/5] Phat hien thiet bi Android dang ket noi: $devices" -ForegroundColor Green
        Write-Host "    Dang dong goi APK va cai dat len thiet bi..." -ForegroundColor Yellow
        
        # Giai phong cac tien trinh build cu neu co tranh khoa tep (File Lock)
        dotnet build-server shutdown 2>$null | Out-Null
        
        dotnet build X:\frontend\ExpenseTracker.Maui\ExpenseTracker.Maui.csproj -f net10.0-android -t:Run -nr:false
        
        if ($LASTEXITCODE -eq 0) {
            # Dam bao Activity mo len tren man hinh
            & $adbPath shell monkey -p com.companyname.expensetracker.maui -c android.intent.category.LAUNCHER 1 2>$null | Out-Null
            Write-Host "`n[V] UNG DUNG EXPENSE TRACKER DA KHOI CHAY THANH CONG TREN MAN HINH ANDROID!" -ForegroundColor Green
        } else {
            Write-Host "`n[X] Deploy Android that bai! Vui long kiem tra log phia tren." -ForegroundColor Red
        }
    } else {
        Write-Host "`n[!] CHUA CO MAY AO HOAC DIEN THOAI ANDROID NAO DANG KET NOI." -ForegroundColor Yellow
        Write-Host "----------------------------------------------------------" -ForegroundColor DarkGray
        Write-Host "BAN CO THE CHON 1 TRONG 2 CACH SAU DE CHAY:" -ForegroundColor Cyan
        Write-Host "  - CACH 1 (Nhanh & Tien nhat): Cam dien thoai Android that qua cap USB, bat USB Debugging." -ForegroundColor White
        Write-Host "  - CACH 2: Mo Visual Studio tai 'X:\ExpenseTracker.slnx'," -ForegroundColor White
        Write-Host "             vao menu: Tools > Android > Android Device Manager de bat/tao May ao," -ForegroundColor White
        Write-Host "             sau do nhan F5 de chay truc tiep." -ForegroundColor White
        Write-Host "----------------------------------------------------------" -ForegroundColor DarkGray
    }
} else {
    Write-Host "`n[X] Khong tim thay cong cu adb.exe tren he thong!" -ForegroundColor Red
}

Write-Host "`nNhan phim Enter de dong cua so nay..." -ForegroundColor Gray
Read-Host



