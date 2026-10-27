# ==============================================================================
# SCRIPT 1-CLICK CLOUDFLARE TUNNEL (EXPENSE TRACKER BACKEND HTTPS GATEWAY)
# Phục vụ chạy đường hầm HTTPS an toàn trỏ thẳng vào Backend Web API (cổng 5018)
# ==============================================================================

[CmdletBinding()]
param (
    [Parameter(Mandatory = $false)]
    [string]$TunnelToken = "eyJhIjoiMzUwYzY1NWQzODAwNjc2NzJlNDcxZDVhZjM4NmEyN2EiLCJ0IjoiN2VhZTFhYTItOTQ5YS00M2RmLWIzYjYtNTg4ZmU5OTI5MjU5IiwicyI6IlpEWmlObUZsTXpFdFpXTm1aaTAwWVdWaExXSmtNREV0WVdZMVlUazBOV1JtTVRGaSJ9",

    [Parameter(Mandatory = $false)]
    [string]$PublicDomain = "https://expense-api.hobbyshop-ecommerce.me",

    [Parameter(Mandatory = $false)]
    [int]$BackendPort = 5018
)

$ErrorActionPreference = "Stop"

Write-Host "`n====================================================================" -ForegroundColor Cyan
Write-Host "   CLOUDFLARE TUNNEL HTTPS GATEWAY - EXPENSE TRACKER BACKEND" -ForegroundColor Cyan
Write-Host "====================================================================`n" -ForegroundColor Cyan

# 1. Tìm thư mục gốc dự án (Upward Tree Traversal tìm ExpenseTracker.slnx)
$currentDir = $PSScriptRoot
$rootFound = $false
while ($currentDir) {
    if (Test-Path (Join-Path $currentDir "ExpenseTracker.slnx")) {
        $rootFound = $true
        break
    }
    $parent = Split-Path -Parent $currentDir
    if ($parent -eq $currentDir) { break }
    $currentDir = $parent
}

if (-not $rootFound) {
    $currentDir = (Get-Item $PSScriptRoot).Parent.FullName
}

$toolsDir = Join-Path $PSScriptRoot "tools"
if (-not (Test-Path $toolsDir)) {
    New-Item -ItemType Directory -Path $toolsDir -Force | Out-Null
}

$cloudflaredExe = Join-Path $toolsDir "cloudflared.exe"

# 2. Kiểm tra hoặc tải portable cloudflared.exe nếu chưa có
if (-not (Test-Path $cloudflaredExe)) {
    # Kiểm tra xem hệ thống đã có lệnh cloudflared trong PATH chưa
    $sysCloudflared = Get-Command "cloudflared" -ErrorAction SilentlyContinue
    if ($sysCloudflared) {
        $cloudflaredExe = $sysCloudflared.Source
        Write-Host " [+] Tim thay cloudflared tren he thong: $cloudflaredExe" -ForegroundColor Green
    }
    else {
        Write-Host " [!] Chua co cloudflared.exe. Dang tu dong tai ban portable chinh thuc tu Cloudflare..." -ForegroundColor Yellow
        $downloadUrl = "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-windows-amd64.exe"
        try {
            [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12 -bor [Net.SecurityProtocolType]::Tls13
            Invoke-WebRequest -Uri $downloadUrl -OutFile $cloudflaredExe -UseBasicParsing
            Write-Host " [+] Da tai thanh cong cloudflared.exe ve: $cloudflaredExe" -ForegroundColor Green
        }
        catch {
            Write-Host " [-] Khong the tu dong tai cloudflared: $($_.Exception.Message)" -ForegroundColor Red
            Write-Host " [!] Vui long tai thu cong tu $downloadUrl va dat vao $toolsDir" -ForegroundColor Yellow
            return
        }
    }
}

# 3. Kiểm tra xem Backend Web API (cổng 5018) đã chạy chưa
$backendUrl = "http://localhost:$BackendPort"
Write-Host " [*] Kiem tra Backend Web API tai $backendUrl..." -ForegroundColor Cyan

$backendAlive = $false
try {
    $resp = Invoke-WebRequest -Uri "$backendUrl/api/health" -TimeoutSec 2 -UseBasicParsing -ErrorAction SilentlyContinue
    if ($resp.StatusCode -eq 200) {
        $backendAlive = $true
    }
}
catch {
    $backendAlive = $false
}

if (-not $backendAlive) {
    Write-Host " [!] Backend Web API chua chay tren cong $BackendPort. Dang tu dong khoi dong..." -ForegroundColor Yellow
    
    # Kiểm tra Docker
    $dockerCmd = Get-Command "docker" -ErrorAction SilentlyContinue
    if ($dockerCmd) {
        Write-Host " [*] Kiem tra PostgreSQL Docker..." -ForegroundColor Cyan
        & docker compose -f (Join-Path $currentDir "backend\docker-compose.yml") up -d 2>$null
    }

    # Bật Backend trong tiến trình ngầm
    $backendProject = Join-Path $currentDir "backend\ExpenseTracker.WebApi\ExpenseTracker.WebApi.csproj"
    Start-Process "dotnet" -ArgumentList "run --project `"$backendProject`" --urls $backendUrl" -WindowStyle Minimized
    
    Write-Host " [*] Dang cho Backend khoi dong (10s)..." -ForegroundColor Cyan
    Start-Sleep -Seconds 10
}

Write-Host " [+] Backend Web API san sang tai: $backendUrl" -ForegroundColor Green

# 4. Khởi chạy Cloudflare Tunnel
Write-Host "`n ====================================================================" -ForegroundColor Green
Write-Host "   DANG KHOI CHAY DUONG HAM CLOUDFLARE HTTPS..." -ForegroundColor Green
Write-Host " ====================================================================`n" -ForegroundColor Green

if (-not [string]::IsNullOrWhiteSpace($TunnelToken)) {
    Write-Host " [*] Chay Named Tunnel voi Token tai khoan Cloudflare..." -ForegroundColor Cyan
    Write-Host " [*] PUBLIC HTTPS DOMAIN: $PublicDomain" -ForegroundColor Magenta
    try {
        Set-Clipboard $PublicDomain -ErrorAction SilentlyContinue
        Write-Host " [+] Da tu dong sao chep $PublicDomain vao Clipboard!`n" -ForegroundColor Green
    } catch {}
    & $cloudflaredExe tunnel run --token $TunnelToken
}
else {
    Write-Host " [*] Chay Quick Tunnel truc tiep cho cong $BackendPort..." -ForegroundColor Cyan
    Write-Host " [!] LUU Y: Cloudflare se xuat ra mot duong dan HTTPS (vi du: https://xxxx.trycloudflare.com)" -ForegroundColor Yellow
    Write-Host " [!] Hay sao chep duong link do va dan vao muc 'Cai dat May chu' tren App dien thoai!`n" -ForegroundColor Yellow

    & $cloudflaredExe tunnel --url $backendUrl
}
