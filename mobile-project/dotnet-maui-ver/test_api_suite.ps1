<#
.SYNOPSIS
    Script Kiem Thu Tu Dong Toan Bo API va Kenh Bat Loi Dinh San (API Master Test Runner).
.DESCRIPTION
    Script thuc hien kiem thu tu dong toan dien qua toan bo cac Controllers cua Backend ExpenseTracker.
    Bao gom:
    1. Kiem tra ket noi CSDL PostgreSQL (Fail-fast: ngat chuong trinh ngay neu khong co CSDL).
    2. Happy Paths: Kiem thu toan bo cac API endpoints cua he thong.
    3. Sad Paths / Negative Cases: Kiem thu bat loi dinh san (400 Bad Request, 401 Unauthorized, 404 Not Found).
    4. Concurrency & ACID: Chuyen tien giua cac vi an toan tuyet doi.
    5. Don dep va Khoi phuc CSDL ve trang thai Demo Story chuan o cuoi chuong trinh.
    6. Dung man hinh cho nguoi dung nhan phim Enter moi duoc dong, tranh tu dong tat console.
    (Luu y: Da loai bo cac cuoc goi Gemini AI de bao ve han muc API Key cua nguoi dung).
.PARAMETER BaseUrl
    Dia chi goc cua Backend API (Mac dinh: http://localhost:5018).
.EXAMPLE
    .\test_api_suite.ps1
    .\test_api_suite.ps1 -BaseUrl "http://localhost:5018"
#>

[CmdletBinding()]
param (
    [string]$BaseUrl = "http://localhost:5018"
)

# Thiet lap bang ma UTF-8 cho console
try {
    [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
    $OutputEncoding = [System.Text.Encoding]::UTF8
} catch {}

# Bien dem thong ke
$script:totalTests = 0
$script:passedTests = 0
$script:failedTests = 0
$script:jwtToken = ""

# Cac bien luu ID thuc the de test xuyen suot
$script:walletVcbId = ""
$script:walletCashId = ""
$script:categoryId = ""
$script:autoRuleId = ""
$script:incomeTxId = ""
$script:expenseTxId = ""
$script:recurringIncomeId = ""
$script:budgetId = ""
$script:savingsGoalId = ""
$script:notificationId = ""
$script:coolingOffId = ""
$script:actionInboxId = ""

function Write-Header {
    param ([string]$title)
    Write-Host ""
    Write-Host "================================================================================" -ForegroundColor Cyan
    Write-Host " $title" -ForegroundColor Cyan
    Write-Host "================================================================================" -ForegroundColor Cyan
}

function Write-Section {
    param ([string]$name)
    Write-Host ""
    Write-Host "--------------------------------------------------------------------------------" -ForegroundColor DarkGray
    Write-Host " >> $name" -ForegroundColor Yellow
    Write-Host "--------------------------------------------------------------------------------" -ForegroundColor DarkGray
}

function Invoke-Api {
    param (
        [string]$Method,
        [string]$Endpoint,
        [object]$Body = $null,
        [bool]$UseAuth = $true
    )

    $url = "$BaseUrl$Endpoint"
    $headers = @{
        "Accept" = "application/json"
    }

    if ($UseAuth -and -not [string]::IsNullOrEmpty($script:jwtToken)) {
        $headers["Authorization"] = "Bearer $script:jwtToken"
    }

    $jsonBody = $null
    if ($Body -ne $null) {
        $headers["Content-Type"] = "application/json"
        if ($Body -is [string]) {
            $jsonBody = $Body
        } else {
            $jsonBody = $Body | ConvertTo-Json -Depth 10 -Compress
        }
    }

    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $statusCode = 0
    $responseBody = $null
    $rawResponse = ""

    try {
        $params = @{
            Uri = $url
            Method = $Method
            Headers = $headers
            TimeoutSec = 30
            UseBasicParsing = $true
            ErrorAction = "Stop"
        }
        if ($jsonBody -ne $null) {
            $params["Body"] = $jsonBody
        }

        $res = Invoke-WebRequest @params
        $sw.Stop()
        $statusCode = [int]$res.StatusCode
        $rawResponse = $res.Content
        if (-not [string]::IsNullOrWhiteSpace($rawResponse)) {
            try { $responseBody = $rawResponse | ConvertFrom-Json } catch { $responseBody = $rawResponse }
        }
    }
    catch {
        $sw.Stop()
        if ($_.Exception -ne $null -and $_.Exception.Response -ne $null) {
            try {
                $statusCode = [int]$_.Exception.Response.StatusCode
            } catch {
                $statusCode = 500
            }
            try {
                $stream = $_.Exception.Response.GetResponseStream()
                if ($stream -ne $null) {
                    $reader = New-Object System.IO.StreamReader($stream, [System.Text.Encoding]::UTF8)
                    $rawResponse = $reader.ReadToEnd()
                    try { $responseBody = $rawResponse | ConvertFrom-Json } catch { $responseBody = $rawResponse }
                }
            } catch {}
        } else {
            $statusCode = 500
            $rawResponse = if ($_.Exception) { $_.Exception.Message } else { "Loi ket noi hoac timeout" }
        }
    }

    return [PSCustomObject]@{
        StatusCode = $statusCode
        Body = $responseBody
        Raw = $rawResponse
        ElapsedMs = $sw.ElapsedMilliseconds
        Url = $url
        Method = $Method
    }
}

function Assert-Result {
    param (
        [string]$TestName,
        [object]$Response,
        [int[]]$ExpectedStatus,
        [string]$ExpectedDetailSubstr = ""
    )

    $script:totalTests++
    $isStatusMatch = $ExpectedStatus -contains $Response.StatusCode
    $isDetailMatch = $true

    if (-not [string]::IsNullOrEmpty($ExpectedDetailSubstr)) {
        $rawStr = "$($Response.Raw)"
        if (-not $rawStr.Contains($ExpectedDetailSubstr)) {
            $isDetailMatch = $false
        }
    }

    if ($isStatusMatch -and $isDetailMatch) {
        $script:passedTests++
        Write-Host " [PASS] " -ForegroundColor Green -NoNewline
        Write-Host "$TestName " -ForegroundColor White -NoNewline
        Write-Host "($($Response.StatusCode) | $($Response.ElapsedMs)ms)" -ForegroundColor DarkGray
        return $true
    } else {
        $script:failedTests++
        Write-Host " [FAIL] " -ForegroundColor Red -NoNewline
        Write-Host "$TestName " -ForegroundColor White
        Write-Host "        Mong doi Status: $($ExpectedStatus -join ', ') | Thuc te: $($Response.StatusCode)" -ForegroundColor Yellow
        if (-not [string]::IsNullOrEmpty($ExpectedDetailSubstr) -and -not $isDetailMatch) {
            Write-Host "        Mong doi chuoi: '$ExpectedDetailSubstr' | Khong tim thay trong Body" -ForegroundColor Yellow
        }
        if (-not [string]::IsNullOrWhiteSpace($Response.Raw)) {
            $snippet = $Response.Raw
            if ($snippet.Length -gt 180) { $snippet = $snippet.Substring(0, 180) + "..." }
            Write-Host "        Phan hoi: $snippet" -ForegroundColor DarkGray
        }
        return $false
    }
}

# ==============================================================================
# BAT DAU CHU TRINH KIEM THU
# ==============================================================================

Write-Header "EXPENSE TRACKER - BO KIEM THU TU DONG TOAN DIEN TOAN BO API"

# Tu dong do tim cong Backend neu dia chi mac dinh khong phan hoi
$candidateUrls = @($BaseUrl, "http://localhost:5018", "http://localhost:5000", "https://localhost:7295") | Select-Object -Unique
$detectedUrl = $null

foreach ($cand in $candidateUrls) {
    try {
        $probe = Invoke-WebRequest -Uri "$cand/api/system/database-status" -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
        if ($probe.StatusCode -eq 200 -or $probe.StatusCode -eq 503) {
            $detectedUrl = $cand
            break
        }
    } catch {
        if ($_.Exception.Response -ne $null) {
            $detectedUrl = $cand
            break
        }
    }
}

if ($detectedUrl -ne $null) {
    $BaseUrl = $detectedUrl
    Write-Host " [INFO] Da ket noi thanh cong den Backend tai: $BaseUrl" -ForegroundColor Cyan
} else {
    Write-Host " [WARN] Khong the phan hoi tu bat ky cong Backend nao trong danh sach: $($candidateUrls -join ', ')" -ForegroundColor Yellow
}

# ------------------------------------------------------------------------------
# 0. KIEM TRA SUC KHOE DATABASE VA CO CHE NGAT NGAY (FAIL-FAST)
# ------------------------------------------------------------------------------
Write-Section "0. KIEM TRA SUC KHOE DATABASE VA CO CHE NGAT NGAY (FAIL-FAST)"

$resStatus = Invoke-Api -Method "GET" -Endpoint "/api/system/database-status" -UseAuth $false

if ($resStatus.StatusCode -ne 200 -or $null -eq $resStatus.Body -or $resStatus.Body.databaseConnected -ne $true) {
    Write-Host ""
    Write-Host " ==============================================================================" -ForegroundColor Red
    Write-Host " [NGAT TIEN TRINH - DATABASE OFFLINE]" -ForegroundColor Red
    Write-Host " Khong the ket noi den PostgreSQL Database hoac WebApi khong hoat dong tai $BaseUrl!" -ForegroundColor Red
    Write-Host " Chi tiet loi: $($resStatus.Raw)" -ForegroundColor DarkGray
    Write-Host ""
    Write-Host " HUONG DAN KHAC PHUC:" -ForegroundColor Yellow
    Write-Host "   1. Hay bat Docker Desktop tren Windows." -ForegroundColor Yellow
    Write-Host "   2. Mo terminal va chay lenh: docker compose up -d" -ForegroundColor Yellow
    Write-Host "   3. Khoi dong Backend: dotnet run --project ExpenseTracker.WebApi" -ForegroundColor Yellow
    Write-Host " ==============================================================================" -ForegroundColor Red
    Write-Host ""
    Read-Host -Prompt " Nhan phim Enter de thoat chuong trinh..."
    exit 1
}

Assert-Result -TestName "0.1 Kiem tra ket noi CSDL PostgreSQL & Tinh trang WebApi" -Response $resStatus -ExpectedStatus @(200)

Write-Host " [OK] Co so du lieu PostgreSQL Online! Users: $($resStatus.Body.statistics.users), Wallets: $($resStatus.Body.statistics.wallets)" -ForegroundColor Green

# Khoi tao Admin mac dinh neu chua co
$resSeedAdmin = Invoke-Api -Method "POST" -Endpoint "/api/system/seed-admin" -UseAuth $false
Assert-Result -TestName "0.2 Khoi tao tai khoan Admin mac dinh he thong" -Response $resSeedAdmin -ExpectedStatus @(200)

# ------------------------------------------------------------------------------
# 1. PHAN HE XAC THUC (AUTH) VA QUAN LY NGUOI DUNG (USERS)
# ------------------------------------------------------------------------------
Write-Section "1. PHAN HE 1: XAC THUC (AUTH) VA HO SO NGUOI DUNG (USERS)"

$testEmail = "suite_tester_$(Get-Random)@expensetracker.local"
$testPass = "Password123!"
$newPass = "NewPassword123!"

# 1.1 POST /api/auth/register
$regBody = @{
    fullName = "Master Suite Tester"
    email = $testEmail
    password = $testPass
}
$resReg = Invoke-Api -Method "POST" -Endpoint "/api/auth/register" -Body $regBody -UseAuth $false
Assert-Result -TestName "1.1 POST /api/auth/register (Dang ky tai khoan moi)" -Response $resReg -ExpectedStatus @(200, 201)

# 1.2 POST /api/auth/login
$loginBody = @{
    email = $testEmail
    password = $testPass
}
$resLogin = Invoke-Api -Method "POST" -Endpoint "/api/auth/login" -Body $loginBody -UseAuth $false
Assert-Result -TestName "1.2 POST /api/auth/login (Dang nhap lay JWT token)" -Response $resLogin -ExpectedStatus @(200)

$refreshToken = ""
if ($resLogin.StatusCode -eq 200 -and $resLogin.Body.accessToken) {
    $script:jwtToken = $resLogin.Body.accessToken
    $refreshToken = $resLogin.Body.refreshToken
}

# 1.3 POST /api/auth/refresh-token
if (-not [string]::IsNullOrEmpty($refreshToken)) {
    $refreshBody = @{
        accessToken = $script:jwtToken
        refreshToken = $refreshToken
    }
    $resRefresh = Invoke-Api -Method "POST" -Endpoint "/api/auth/refresh-token" -Body $refreshBody -UseAuth $false
    Assert-Result -TestName "1.3 POST /api/auth/refresh-token (Lam moi Token cap phep)" -Response $resRefresh -ExpectedStatus @(200)
    if ($resRefresh.StatusCode -eq 200 -and $resRefresh.Body.accessToken) {
        $script:jwtToken = $resRefresh.Body.accessToken
    }
}

# 1.4 GET /api/users/profile
$resProfile = Invoke-Api -Method "GET" -Endpoint "/api/users/profile"
Assert-Result -TestName "1.4 GET /api/users/profile (Xem ho so ca nhan)" -Response $resProfile -ExpectedStatus @(200)

# 1.5 PUT /api/users/profile
$updateProfileBody = @{
    fullName = "Master Suite Tester (Updated)"
    baseCurrency = "VND"
}
$resUpdateProfile = Invoke-Api -Method "PUT" -Endpoint "/api/users/profile" -Body $updateProfileBody
Assert-Result -TestName "1.5 PUT /api/users/profile (Cap nhat ho so ca nhan)" -Response $resUpdateProfile -ExpectedStatus @(200)

# 1.6 PUT /api/users/change-password
$changePassBody = @{
    currentPassword = $testPass
    newPassword = $newPass
}
$resChangePass = Invoke-Api -Method "PUT" -Endpoint "/api/users/change-password" -Body $changePassBody
Assert-Result -TestName "1.6 PUT /api/users/change-password (Doi mat khau dang nhap)" -Response $resChangePass -ExpectedStatus @(200, 204)

# 1.7 POST /api/auth/logout
$resLogout = Invoke-Api -Method "POST" -Endpoint "/api/auth/logout"
Assert-Result -TestName "1.7 POST /api/auth/logout (Dang xuat he thong)" -Response $resLogout -ExpectedStatus @(200, 204)

# 1.8 POST /api/auth/login voi mat khau moi
$reLoginBody = @{
    email = $testEmail
    password = $newPass
}
$resReLogin = Invoke-Api -Method "POST" -Endpoint "/api/auth/login" -Body $reLoginBody -UseAuth $false
Assert-Result -TestName "1.8 POST /api/auth/login (Xac nhan dang nhap bang mat khau moi)" -Response $resReLogin -ExpectedStatus @(200)
if ($resReLogin.StatusCode -eq 200 -and $resReLogin.Body.accessToken) {
    $script:jwtToken = $resReLogin.Body.accessToken
}

# ------------------------------------------------------------------------------
# 2. PHAN HE QUAN LY DA VI CA NHAN (MULTI-WALLET ACID)
# ------------------------------------------------------------------------------
Write-Section "2. PHAN HE 2: QUAN LY DA VI CA NHAN (MULTI-WALLET ACID)"

# 2.1 POST /api/wallets (Tao vi VCB)
$vcbBody = @{
    name = "Tai khoan Vietcombank Digibank"
    walletType = 1
    currency = "VND"
    initialBalance = 20000000
    accountNumber = "9988776655"
    bankName = "Vietcombank"
    colorHex = "#0066FF"
    iconKey = "building-columns"
}
$resVcb = Invoke-Api -Method "POST" -Endpoint "/api/wallets" -Body $vcbBody
Assert-Result -TestName "2.1 POST /api/wallets (Tao vi Ngan hang VCB 20.000.000 đ)" -Response $resVcb -ExpectedStatus @(200, 201)
if ($resVcb.Body -and $resVcb.Body.id) { $script:walletVcbId = $resVcb.Body.id }

# 2.2 POST /api/wallets (Tao vi Tien mat)
$cashBody = @{
    name = "Vi Tien Mat Hang Ngay"
    walletType = 0
    currency = "VND"
    initialBalance = 5000000
    colorHex = "#10B981"
    iconKey = "cash"
}
$resCash = Invoke-Api -Method "POST" -Endpoint "/api/wallets" -Body $cashBody
Assert-Result -TestName "2.2 POST /api/wallets (Tao vi Tien mat 5.000.000 đ)" -Response $resCash -ExpectedStatus @(200, 201)
if ($resCash.Body -and $resCash.Body.id) { $script:walletCashId = $resCash.Body.id }

# 2.3 GET /api/wallets
$resListWallets = Invoke-Api -Method "GET" -Endpoint "/api/wallets"
Assert-Result -TestName "2.3 GET /api/wallets (Xem danh sach tat ca cac vi)" -Response $resListWallets -ExpectedStatus @(200)

# 2.4 GET /api/wallets/{id}
$resGetWallet = Invoke-Api -Method "GET" -Endpoint "/api/wallets/$script:walletVcbId"
Assert-Result -TestName "2.4 GET /api/wallets/{id} (Xem chi tiet vi theo ID)" -Response $resGetWallet -ExpectedStatus @(200)

# 2.5 PUT /api/wallets/{id}
$updateWalletBody = @{
    name = "Tai khoan VCB Premium"
    bankName = "Vietcombank"
    accountNumber = "9988776655"
    colorHex = "#1E40AF"
    iconKey = "building-columns"
}
$resUpdateWallet = Invoke-Api -Method "PUT" -Endpoint "/api/wallets/$script:walletVcbId" -Body $updateWalletBody
Assert-Result -TestName "2.5 PUT /api/wallets/{id} (Cap nhat thong tin vi)" -Response $resUpdateWallet -ExpectedStatus @(200)

# 2.6 PATCH /api/wallets/{id}
$patchWalletBody = @{
    colorHex = "#2563EB"
    iconKey = "credit-card"
}
$resPatchWallet = Invoke-Api -Method "PATCH" -Endpoint "/api/wallets/$script:walletVcbId" -Body $patchWalletBody
Assert-Result -TestName "2.6 PATCH /api/wallets/{id} (Sua doi mot phan vi RFC 7396)" -Response $resPatchWallet -ExpectedStatus @(200)

# 2.7 POST /api/wallets/transfer (ACID transfer)
$transferBody = @{
    sourceWalletId = $script:walletVcbId
    destinationWalletId = $script:walletCashId
    amount = 2000000
    transferDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    note = "Rut tien ATM bo vi tieu vat"
}
$resTransfer = Invoke-Api -Method "POST" -Endpoint "/api/wallets/transfer" -Body $transferBody
Assert-Result -TestName "2.7 POST /api/wallets/transfer (Chuyen tien giua 2 vi ACID)" -Response $resTransfer -ExpectedStatus @(200)

# 2.8 POST /api/wallets/{id}/adjust (Can chinh so du)
$adjustBody = @{
    newBalance = 6950000
    reason = "Mua do vat quen ghi chep"
    adjustmentDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
}
$resAdjust = Invoke-Api -Method "POST" -Endpoint "/api/wallets/$script:walletCashId/adjust" -Body $adjustBody
Assert-Result -TestName "2.8 POST /api/wallets/{id}/adjust (Can chinh so du thuc te)" -Response $resAdjust -ExpectedStatus @(200)

# 2.9 GET /api/wallets/{id}/adjustments (Lich su can chinh)
$resAdjHistory = Invoke-Api -Method "GET" -Endpoint "/api/wallets/$script:walletCashId/adjustments"
Assert-Result -TestName "2.9 GET /api/wallets/{id}/adjustments (Xem lich su kiem ke so du)" -Response $resAdjHistory -ExpectedStatus @(200)

# ------------------------------------------------------------------------------
# 3. PHAN HE DANH MUC & QUY TAC TU DONG (CATEGORIES & AUTO RULES)
# ------------------------------------------------------------------------------
Write-Section "3. PHAN HE 3: DANH MUC & QUY TAC TU DONG (CATEGORIES & AUTO RULES)"

# 3.1 GET /api/categories
$resCats = Invoke-Api -Method "GET" -Endpoint "/api/categories"
Assert-Result -TestName "3.1 GET /api/categories (Xem cay danh muc thu chi)" -Response $resCats -ExpectedStatus @(200)

# Lay danh muc chi tieu co san
$foodCategoryId = $null
if ($resCats.StatusCode -eq 200 -and $resCats.Body) {
    foreach ($cat in $resCats.Body) {
        if ($cat.transactionType -eq 2 -or $cat.transactionType -eq "Expense") {
            $foodCategoryId = $cat.id
            break
        }
    }
}

# 3.2 POST /api/categories (Tao danh muc tuy bien moi, TransactionType = 2: Expense)
$createCatBody = @{
    name = "Thiet Bi Cong Nghe"
    transactionType = 2
    iconKey = "laptop"
    colorHex = "#8B5CF6"
    displayOrder = 10
}
$resCreateCat = Invoke-Api -Method "POST" -Endpoint "/api/categories" -Body $createCatBody
Assert-Result -TestName "3.2 POST /api/categories (Tao danh muc tuy bien moi)" -Response $resCreateCat -ExpectedStatus @(200, 201)
if ($resCreateCat.Body -and $resCreateCat.Body.id) { $script:categoryId = $resCreateCat.Body.id }

# Target expense category
$ruleTargetCat = if ($script:categoryId) { $script:categoryId } else { $foodCategoryId }

# 3.3 GET /api/categories/{id}
if (-not [string]::IsNullOrEmpty($script:categoryId)) {
    $resGetCat = Invoke-Api -Method "GET" -Endpoint "/api/categories/$script:categoryId"
    Assert-Result -TestName "3.3 GET /api/categories/{id} (Xem chi tiet danh muc theo ID)" -Response $resGetCat -ExpectedStatus @(200)

    # 3.4 PUT /api/categories/{id}
    $updateCatBody = @{
        name = "Thiet Bi & Do Dien Tu"
        iconKey = "laptop"
        colorHex = "#7C3AED"
        displayOrder = 11
    }
    $resUpdateCat = Invoke-Api -Method "PUT" -Endpoint "/api/categories/$script:categoryId" -Body $updateCatBody
    Assert-Result -TestName "3.4 PUT /api/categories/{id} (Cap nhat toan phan danh muc)" -Response $resUpdateCat -ExpectedStatus @(200)

    # 3.5 PATCH /api/categories/{id}
    $patchCatBody = @{
        name = "Cong Nghe & Do Dien Tu"
        colorHex = "#6D28D9"
    }
    $resPatchCat = Invoke-Api -Method "PATCH" -Endpoint "/api/categories/$script:categoryId" -Body $patchCatBody
    Assert-Result -TestName "3.5 PATCH /api/categories/{id} (Sua doi mot phan danh muc)" -Response $resPatchCat -ExpectedStatus @(200)
}

# 3.6 POST /api/auto-rules (Tao quy tac tu dong phan loai)
$createRuleBody = @{
    keywordPattern = "Highlands Coffee"
    targetCategoryId = $ruleTargetCat
    priorityOrder = 1
    isRegex = $false
    isCaseSensitive = $false
}
$resCreateRule = Invoke-Api -Method "POST" -Endpoint "/api/auto-rules" -Body $createRuleBody
Assert-Result -TestName "3.6 POST /api/auto-rules (Tao quy tac tu dong gan tu khoa)" -Response $resCreateRule -ExpectedStatus @(200, 201)
if ($resCreateRule.Body -and $resCreateRule.Body.id) { $script:autoRuleId = $resCreateRule.Body.id }

# 3.7 GET /api/auto-rules
$resListRules = Invoke-Api -Method "GET" -Endpoint "/api/auto-rules"
Assert-Result -TestName "3.7 GET /api/auto-rules (Xem danh sach quy tac tu dong)" -Response $resListRules -ExpectedStatus @(200)

# 3.8 GET /api/auto-rules/{id}
if (-not [string]::IsNullOrEmpty($script:autoRuleId)) {
    $resGetRule = Invoke-Api -Method "GET" -Endpoint "/api/auto-rules/$script:autoRuleId"
    Assert-Result -TestName "3.8 GET /api/auto-rules/{id} (Xem chi tiet quy tac theo ID)" -Response $resGetRule -ExpectedStatus @(200)

    # 3.9 PUT /api/auto-rules/{id}
    $updateRuleBody = @{
        keywordPattern = "Highlands Coffee"
        targetCategoryId = $ruleTargetCat
        priorityOrder = 5
        isRegex = $false
        isCaseSensitive = $false
    }
    $resUpdateRule = Invoke-Api -Method "PUT" -Endpoint "/api/auto-rules/$script:autoRuleId" -Body $updateRuleBody
    Assert-Result -TestName "3.9 PUT /api/auto-rules/{id} (Cap nhat do uu tien quy tac)" -Response $resUpdateRule -ExpectedStatus @(200)
}

# 3.10 GET /api/auto-rules/match?text=...
$resMatch = Invoke-Api -Method "GET" -Endpoint "/api/auto-rules/match?text=Highlands%20Coffee%20Vincom"
Assert-Result -TestName "3.10 GET /api/auto-rules/match (Kiem tra khop tu khoa tu dong)" -Response $resMatch -ExpectedStatus @(200)

# ------------------------------------------------------------------------------
# 4. PHAN HE GIAO DICH THU - CHI (TRANSACTIONS & RECURRING)
# ------------------------------------------------------------------------------
Write-Section "4. PHAN HE 4: GIAO DICH THU - CHI (TRANSACTIONS & RECURRING)"

# Lay 1 danh muc thu (Income Category: TransactionType = 1)
$incomeCatId = $null
if ($resCats.StatusCode -eq 200 -and $resCats.Body) {
    foreach ($cat in $resCats.Body) {
        if ($cat.transactionType -eq 1 -or $cat.transactionType -eq "Income") {
            $incomeCatId = $cat.id
            break
        }
    }
}
if (-not $incomeCatId) { $incomeCatId = $ruleTargetCat }

# 4.1 POST /api/transactions/incomes (Ghi nhan thu nhap luong)
$incomeBody = @{
    walletId = $script:walletVcbId
    categoryId = $incomeCatId
    amount = 25000000
    transactionDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    note = "Nhan luong thang 10 cong ty"
    isWindfall = $false
    isRecurring = $true
}
$resIncome = Invoke-Api -Method "POST" -Endpoint "/api/transactions/incomes" -Body $incomeBody
Assert-Result -TestName "4.1 POST /api/transactions/incomes (Ghi nhan thu nhap 25.000.000 đ)" -Response $resIncome -ExpectedStatus @(200, 201)
if ($resIncome.Body -and $resIncome.Body.id) { $script:incomeTxId = $resIncome.Body.id }

# 4.2 GET /api/transactions/incomes/monthly-total
$curMonth = (Get-Date).Month
$curYear = (Get-Date).Year
$resIncomeTotal = Invoke-Api -Method "GET" -Endpoint "/api/transactions/incomes/monthly-total?month=$curMonth&year=$curYear"
Assert-Result -TestName "4.2 GET /api/transactions/incomes/monthly-total (Tong thu nhap trong thang)" -Response $resIncomeTotal -ExpectedStatus @(200)

# 4.3 POST /api/transactions/expenses (Ghi nhan chi tieu)
$expenseBody = @{
    walletId = $script:walletCashId
    categoryId = $ruleTargetCat
    amount = 120000
    transactionDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    note = "Highlands Coffee hop nhom du an"
    isEssential = $false
    isRecurring = $false
}
$resExpense = Invoke-Api -Method "POST" -Endpoint "/api/transactions/expenses" -Body $expenseBody
Assert-Result -TestName "4.3 POST /api/transactions/expenses (Ghi nhan chi tieu ca phe 120.000 đ)" -Response $resExpense -ExpectedStatus @(200, 201)
if ($resExpense.Body -and $resExpense.Body.id) { $script:expenseTxId = $resExpense.Body.id }

# 4.4 GET /api/transactions/expenses/monthly-total
$resExpenseTotal = Invoke-Api -Method "GET" -Endpoint "/api/transactions/expenses/monthly-total?month=$curMonth&year=$curYear"
Assert-Result -TestName "4.4 GET /api/transactions/expenses/monthly-total (Tong chi tieu trong thang)" -Response $resExpenseTotal -ExpectedStatus @(200)

# 4.5 GET /api/transactions (Loc danh sach giao dich)
$resFilterTx = Invoke-Api -Method "GET" -Endpoint "/api/transactions?page=1&pageSize=20"
Assert-Result -TestName "4.5 GET /api/transactions (Xem lich su giao dich co phan trang)" -Response $resFilterTx -ExpectedStatus @(200)

# 4.6 GET /api/transactions/{id}
if (-not [string]::IsNullOrEmpty($script:expenseTxId)) {
    $resGetTx = Invoke-Api -Method "GET" -Endpoint "/api/transactions/$script:expenseTxId"
    Assert-Result -TestName "4.6 GET /api/transactions/{id} (Xem chi tiet giao dich theo ID)" -Response $resGetTx -ExpectedStatus @(200)

    # 4.7 GET /api/transactions/expenses/{id}/details
    $resExpDetails = Invoke-Api -Method "GET" -Endpoint "/api/transactions/expenses/$script:expenseTxId/details"
    Assert-Result -TestName "4.7 GET /api/transactions/expenses/{id}/details (Xem chi tiet chi tieu va hoa don)" -Response $resExpDetails -ExpectedStatus @(200)

    # 4.8 PUT /api/transactions/expenses/{id}
    $updateExpBody = @{
        walletId = $script:walletCashId
        categoryId = $ruleTargetCat
        amount = 135000
        transactionDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        note = "Highlands Coffee hop nhom du an (them banh)"
        isEssential = $false
        isRecurring = $false
    }
    $resUpdateExp = Invoke-Api -Method "PUT" -Endpoint "/api/transactions/expenses/$script:expenseTxId" -Body $updateExpBody
    Assert-Result -TestName "4.8 PUT /api/transactions/expenses/{id} (Cap nhat giao dich chi tieu)" -Response $resUpdateExp -ExpectedStatus @(200)
}

# 4.9 PUT /api/transactions/incomes/{id}
if (-not [string]::IsNullOrEmpty($script:incomeTxId)) {
    $updateIncBody = @{
        walletId = $script:walletVcbId
        categoryId = $incomeCatId
        amount = 26000000
        transactionDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        note = "Nhan luong thang 10 cong ty (kem thuong)"
        isWindfall = $false
        isRecurring = $true
    }
    $resUpdateInc = Invoke-Api -Method "PUT" -Endpoint "/api/transactions/incomes/$script:incomeTxId" -Body $updateIncBody
    Assert-Result -TestName "4.9 PUT /api/transactions/incomes/{id} (Cap nhat giao dich thu nhap)" -Response $resUpdateInc -ExpectedStatus @(200)
}

# 4.10 POST /api/recurring-incomes (Tao lich thu nhap dinh ky)
$recurringBody = @{
    name = "Thu nhap Freelance Design"
    expectedAmount = 5000000
    walletId = $script:walletVcbId
    categoryId = $incomeCatId
    frequency = 3
    dayOfMonth = 15
    autoRecord = $true
}
$resRecurring = Invoke-Api -Method "POST" -Endpoint "/api/recurring-incomes" -Body $recurringBody
Assert-Result -TestName "4.10 POST /api/recurring-incomes (Tao lich thu nhap dinh ky hang thang)" -Response $resRecurring -ExpectedStatus @(200, 201)
if ($resRecurring.Body -and $resRecurring.Body.id) { $script:recurringIncomeId = $resRecurring.Body.id }

# 4.11 GET /api/recurring-incomes
$resListRec = Invoke-Api -Method "GET" -Endpoint "/api/recurring-incomes"
Assert-Result -TestName "4.11 GET /api/recurring-incomes (Xem danh sach thu nhap dinh ky)" -Response $resListRec -ExpectedStatus @(200)

# 4.12 GET /api/recurring-incomes/{id}
if (-not [string]::IsNullOrEmpty($script:recurringIncomeId)) {
    $resGetRec = Invoke-Api -Method "GET" -Endpoint "/api/recurring-incomes/$script:recurringIncomeId"
    Assert-Result -TestName "4.12 GET /api/recurring-incomes/{id} (Xem chi tiet thu nhap dinh ky)" -Response $resGetRec -ExpectedStatus @(200)

    # 4.13 PUT /api/recurring-incomes/{id}
    $updateRecBody = @{
        name = "Thu nhap Freelance Design & Tech"
        expectedAmount = 6000000
        walletId = $script:walletVcbId
        categoryId = $incomeCatId
        frequency = 3
        dayOfMonth = 15
        autoRecord = $true
    }
    $resUpdateRec = Invoke-Api -Method "PUT" -Endpoint "/api/recurring-incomes/$script:recurringIncomeId" -Body $updateRecBody
    Assert-Result -TestName "4.13 PUT /api/recurring-incomes/{id} (Cap nhat thong tin thu nhap dinh ky)" -Response $resUpdateRec -ExpectedStatus @(200)

    # 4.14 PATCH /api/recurring-incomes/{id}/toggle
    $resToggleRec = Invoke-Api -Method "PATCH" -Endpoint "/api/recurring-incomes/$script:recurringIncomeId/toggle"
    Assert-Result -TestName "4.14 PATCH /api/recurring-incomes/{id}/toggle (Bat/Tat trang thai thu nhap)" -Response $resToggleRec -ExpectedStatus @(200)
}

# 4.15 POST /api/recurring-incomes/process-due
$resProcessDue = Invoke-Api -Method "POST" -Endpoint "/api/recurring-incomes/process-due" -Body @{ asOfDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }
Assert-Result -TestName "4.15 POST /api/recurring-incomes/process-due (Quet va ghi nhan thu nhap toi han)" -Response $resProcessDue -ExpectedStatus @(200)

# ------------------------------------------------------------------------------
# 5. PHAN HE HOACH DINH NGAN SACH & HAN MUC DSA (BUDGETS & DSA)
# ------------------------------------------------------------------------------
Write-Section "5. PHAN HE 5: HOACH DINH NGAN SACH & HAN MUC DSA (BUDGETS & DSA)"

$startDate = (Get-Date -Day 1).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
$endDate = (Get-Date -Day 1).AddMonths(1).AddDays(-1).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

# 5.1 POST /api/budgets (Tao ngan sach 50/30/20)
$budgetBody = @{
    name = "Ngan Sach Thang $curMonth/$curYear"
    periodType = 1
    startDate = $startDate
    endDate = $endDate
    totalAmount = 15000000
    method = 1
    allocations = @(
        @{
            categoryId = $ruleTargetCat
            allocatedAmount = 5000000
            alertThresholdPercent = 80
        }
    )
}
$resBudget = Invoke-Api -Method "POST" -Endpoint "/api/budgets" -Body $budgetBody
Assert-Result -TestName "5.1 POST /api/budgets (Tao ngan sach thang 15.000.000 đ theo 50/30/20)" -Response $resBudget -ExpectedStatus @(200, 201)
if ($resBudget.Body -and $resBudget.Body.id) { $script:budgetId = $resBudget.Body.id }

# 5.2 GET /api/budgets
$resListBudgets = Invoke-Api -Method "GET" -Endpoint "/api/budgets"
Assert-Result -TestName "5.2 GET /api/budgets (Xem danh sach tat ca ngan sach)" -Response $resListBudgets -ExpectedStatus @(200)

# 5.3 GET /api/budgets/{id}
if (-not [string]::IsNullOrEmpty($script:budgetId)) {
    $resGetBudget = Invoke-Api -Method "GET" -Endpoint "/api/budgets/$script:budgetId"
    Assert-Result -TestName "5.3 GET /api/budgets/{id} (Xem chi tiet ngan sach theo ID)" -Response $resGetBudget -ExpectedStatus @(200)

    # 5.4 PUT /api/budgets/{id}
    $updateBudgetBody = @{
        name = "Ngan Sach Thang $curMonth/$curYear (Dieu chinh)"
        totalAmount = 16000000
    }
    $resUpdateBudget = Invoke-Api -Method "PUT" -Endpoint "/api/budgets/$script:budgetId" -Body $updateBudgetBody
    Assert-Result -TestName "5.4 PUT /api/budgets/{id} (Cap nhat tong han muc ngan sach)" -Response $resUpdateBudget -ExpectedStatus @(200)

    # 5.5 POST /api/budgets/{id}/rebalance
    $rebalanceBody = @{
        rebalances = @(
            @{
                categoryId = $ruleTargetCat
                newAllocatedAmount = 5500000
            }
        )
    }
    $resRebalance = Invoke-Api -Method "POST" -Endpoint "/api/budgets/$script:budgetId/rebalance" -Body $rebalanceBody
    Assert-Result -TestName "5.5 POST /api/budgets/{id}/rebalance (Dieu chuyen han muc dong)" -Response $resRebalance -ExpectedStatus @(200)
}

# 5.6 GET /api/budgets/active
$resActiveBudget = Invoke-Api -Method "GET" -Endpoint "/api/budgets/active"
Assert-Result -TestName "5.6 GET /api/budgets/active (Lay ngan sach dang hieu luc hien tai)" -Response $resActiveBudget -ExpectedStatus @(200)

# 5.7 GET /api/budgets/active/dsa
$resDsa = Invoke-Api -Method "GET" -Endpoint "/api/budgets/active/dsa"
Assert-Result -TestName "5.7 GET /api/budgets/active/dsa (Han muc chi tieu moi ngay DSA)" -Response $resDsa -ExpectedStatus @(200)

# 5.8 GET /api/budgets/active/velocity
$resVelocity = Invoke-Api -Method "GET" -Endpoint "/api/budgets/active/velocity"
Assert-Result -TestName "5.8 GET /api/budgets/active/velocity (Toc do tieu tien Spending Velocity)" -Response $resVelocity -ExpectedStatus @(200)

# 5.9 GET /api/budgets/active/progress
$resProgress = Invoke-Api -Method "GET" -Endpoint "/api/budgets/active/progress"
Assert-Result -TestName "5.9 GET /api/budgets/active/progress (Tien do thuc hien ngan sach)" -Response $resProgress -ExpectedStatus @(200)

# ------------------------------------------------------------------------------
# 6. PHAN HE QUY TICH LUY & 6 CHIEC HU (SAVINGS GOALS & JARS)
# ------------------------------------------------------------------------------
Write-Section "6. PHAN HE 6: QUY TICH LUY & 6 CHIEC HU (SAVINGS GOALS & JARS)"

# 6.1 POST /api/savings-goals (Tao quy khan cap 30 trieu)
$goalBody = @{
    name = "Quy Du Phong Khan Cap 6 Thang"
    targetAmount = 30000000
    startDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    targetDate = (Get-Date).AddMonths(12).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    type = 1
    priority = 1
    initialAmount = 0
    description = "Quy du phong phong ngua bien co suc khoe hoac that nghiep"
    iconKey = "shield-halved"
    colorHex = "#10B981"
}
$resGoal = Invoke-Api -Method "POST" -Endpoint "/api/savings-goals" -Body $goalBody
Assert-Result -TestName "6.1 POST /api/savings-goals (Tao muc tieu tich luy 30.000.000 đ)" -Response $resGoal -ExpectedStatus @(200, 201)
if ($resGoal.Body -and $resGoal.Body.id) { $script:savingsGoalId = $resGoal.Body.id }

# 6.2 GET /api/savings-goals
$resListGoals = Invoke-Api -Method "GET" -Endpoint "/api/savings-goals"
Assert-Result -TestName "6.2 GET /api/savings-goals (Xem danh sach tat ca muc tieu)" -Response $resListGoals -ExpectedStatus @(200)

# 6.3 GET /api/savings-goals/{id}
if (-not [string]::IsNullOrEmpty($script:savingsGoalId)) {
    $resGetGoal = Invoke-Api -Method "GET" -Endpoint "/api/savings-goals/$script:savingsGoalId"
    Assert-Result -TestName "6.3 GET /api/savings-goals/{id} (Xem chi tiet muc tieu tich luy)" -Response $resGetGoal -ExpectedStatus @(200)

    # 6.4 PUT /api/savings-goals/{id}
    $updateGoalBody = @{
        name = "Quy Du Phong Khan Cap Toan Dien"
        description = "Quy du phong 6 thang song an toan"
        targetAmount = 35000000
        targetDate = (Get-Date).AddMonths(12).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        priority = 1
        iconKey = "shield-halved"
        colorHex = "#059669"
    }
    $resUpdateGoal = Invoke-Api -Method "PUT" -Endpoint "/api/savings-goals/$script:savingsGoalId" -Body $updateGoalBody
    Assert-Result -TestName "6.4 PUT /api/savings-goals/{id} (Cap nhat muc tieu tich luy)" -Response $resUpdateGoal -ExpectedStatus @(200)

    # 6.5 PATCH /api/savings-goals/{id}
    $patchGoalBody = @{
        colorHex = "#047857"
        description = "Quy du phong khan cap tuyet doi"
    }
    $resPatchGoal = Invoke-Api -Method "PATCH" -Endpoint "/api/savings-goals/$script:savingsGoalId" -Body $patchGoalBody
    Assert-Result -TestName "6.5 PATCH /api/savings-goals/{id} (Sua doi mot phan muc tieu RFC 7396)" -Response $resPatchGoal -ExpectedStatus @(200)

    # 6.6 POST /api/savings-goals/{id}/deposit (Gui tien vao quy)
    $depositBody = @{
        walletId = $script:walletVcbId
        amount = 2000000
        note = "Trich luong thang 10 vao quy du phong"
    }
    $resDeposit = Invoke-Api -Method "POST" -Endpoint "/api/savings-goals/$script:savingsGoalId/deposit" -Body $depositBody
    Assert-Result -TestName "6.6 POST /api/savings-goals/{id}/deposit (Nop 2.000.000 đ vao quy)" -Response $resDeposit -ExpectedStatus @(200)

    # 6.7 POST /api/savings-goals/{id}/withdraw (Rut tien khoi quy kem Nudge)
    $withdrawBody = @{
        targetWalletId = $script:walletCashId
        amount = 500000
        reason = "Chi phi mua thuoc dot xuat"
    }
    $resWithdraw = Invoke-Api -Method "POST" -Endpoint "/api/savings-goals/$script:savingsGoalId/withdraw" -Body $withdrawBody
    Assert-Result -TestName "6.7 POST /api/savings-goals/{id}/withdraw (Rut 500.000 đ khoi quy kem ly do)" -Response $resWithdraw -ExpectedStatus @(200)

    # 6.8 GET /api/savings-goals/{id}/contributions
    $resContribs = Invoke-Api -Method "GET" -Endpoint "/api/savings-goals/$script:savingsGoalId/contributions"
    Assert-Result -TestName "6.8 GET /api/savings-goals/{id}/contributions (Xem lich su nop/rut)" -Response $resContribs -ExpectedStatus @(200)

    # 6.9 GET /api/savings-goals/{id}/feasibility
    $resFeas = Invoke-Api -Method "GET" -Endpoint "/api/savings-goals/$script:savingsGoalId/feasibility"
    Assert-Result -TestName "6.9 GET /api/savings-goals/{id}/feasibility (Danh gia tinh kha thi muc tieu)" -Response $resFeas -ExpectedStatus @(200)

    # 6.10 POST /api/savings-goals/{id}/auto-deposit
    $autoDepBody = @{
        isEnabled = $true
        amount = 1500000
        dayOfMonth = 10
        walletId = $script:walletVcbId
    }
    $resAutoDep = Invoke-Api -Method "POST" -Endpoint "/api/savings-goals/$script:savingsGoalId/auto-deposit" -Body $autoDepBody
    Assert-Result -TestName "6.10 POST /api/savings-goals/{id}/auto-deposit (Cau hinh tu dong trich nop)" -Response $resAutoDep -ExpectedStatus @(200)

    # 6.11 PATCH /api/savings-goals/{id}/status
    $patchStatusBody = @{
        status = 1
    }
    $resPatchStatus = Invoke-Api -Method "PATCH" -Endpoint "/api/savings-goals/$script:savingsGoalId/status" -Body $patchStatusBody
    Assert-Result -TestName "6.11 PATCH /api/savings-goals/{id}/status (Cap nhat trang thai muc tieu)" -Response $resPatchStatus -ExpectedStatus @(200)
}

# 6.12 GET /api/savings-goals/jars-summary (Bao cao 6 chiec hu)
$resJars = Invoke-Api -Method "GET" -Endpoint "/api/savings-goals/jars-summary"
Assert-Result -TestName "6.12 GET /api/savings-goals/jars-summary (Bao cao tong hop Mo hinh 6 Chiec Hu)" -Response $resJars -ExpectedStatus @(200)

# ------------------------------------------------------------------------------
# 7. PHAN HE REALTIME, PUSH NOTIFICATIONS & TRANG THAI DOC
# ------------------------------------------------------------------------------
Write-Section "7. PHAN HE 7: REALTIME, PUSH NOTIFICATIONS & TRANG THAI DOC"

# 7.1 POST /api/v1/notifications/push-token
$tokenBody = @{
    deviceToken = "fcm_test_token_$(Get-Random)"
    platform = "Android"
    deviceId = "android_device_emulator"
}
$resPushToken = Invoke-Api -Method "POST" -Endpoint "/api/v1/notifications/push-token" -Body $tokenBody
Assert-Result -TestName "7.1 POST /api/v1/notifications/push-token (Dang ky FCM Push Token)" -Response $resPushToken -ExpectedStatus @(200)

# 7.2 POST /api/v1/notifications/test-push
$testPushBody = @{
    title = "Canh bao ngan sach"
    message = "Ban da tieu het 85% han muc An uong thang nay!"
    type = "BudgetAlert"
    targetRoute = "BudgetPage"
    targetId = "$script:budgetId"
}
$resTestPush = Invoke-Api -Method "POST" -Endpoint "/api/v1/notifications/test-push" -Body $testPushBody
Assert-Result -TestName "7.2 POST /api/v1/notifications/test-push (Ban thong bao thu nghiem)" -Response $resTestPush -ExpectedStatus @(200)
if ($resTestPush.Body -and $resTestPush.Body.id) { $script:notificationId = $resTestPush.Body.id }

# 7.3 GET /api/v1/notifications
$resListNotifs = Invoke-Api -Method "GET" -Endpoint "/api/v1/notifications?take=20"
Assert-Result -TestName "7.3 GET /api/v1/notifications (Xem danh sach thong bao in-app)" -Response $resListNotifs -ExpectedStatus @(200)

# 7.4 PATCH /api/v1/notifications/{id}/read
if (-not [string]::IsNullOrEmpty($script:notificationId)) {
    $resReadNotif = Invoke-Api -Method "PATCH" -Endpoint "/api/v1/notifications/$script:notificationId/read"
    Assert-Result -TestName "7.4 PATCH /api/v1/notifications/{id}/read (Danh dau da doc 1 thong bao)" -Response $resReadNotif -ExpectedStatus @(200)
}

# 7.5 PATCH /api/v1/notifications/read-all
$resReadAllNotifs = Invoke-Api -Method "PATCH" -Endpoint "/api/v1/notifications/read-all"
Assert-Result -TestName "7.5 PATCH /api/v1/notifications/read-all (Danh dau da doc tat ca thong bao)" -Response $resReadAllNotifs -ExpectedStatus @(200)

# ------------------------------------------------------------------------------
# 8. PHAN HE TAM LY HANH VI NUDGE & COOLING-OFF 24H (BEHAVIORAL ECONOMICS)
# ------------------------------------------------------------------------------
Write-Section "8. PHAN HE 8: TAM LY HANH VI NUDGE & COOLING-OFF 24H"

# 8.1 GET /api/nudges/active
$resActiveNudges = Invoke-Api -Method "GET" -Endpoint "/api/nudges/active"
Assert-Result -TestName "8.1 GET /api/nudges/active (Lay danh sach Cu Hich Nudge dang kich hoat)" -Response $resActiveNudges -ExpectedStatus @(200)

# 8.2 GET /api/nudges/rebalance-proposal
$resRebProposal = Invoke-Api -Method "GET" -Endpoint "/api/nudges/rebalance-proposal"
Assert-Result -TestName "8.2 GET /api/nudges/rebalance-proposal (De xuat tai can bang ngan sach tu dong)" -Response $resRebProposal -ExpectedStatus @(200, 400)

# 8.3 POST /api/cooling-off/evaluate (Tien danh gia mon do dat tien)
$evalCoolBody = @{
    walletId = $script:walletVcbId
    categoryId = $ruleTargetCat
    amount = 3500000
    description = "Tai nghe Bluetooth Sony chong on"
}
$resEvalCool = Invoke-Api -Method "POST" -Endpoint "/api/cooling-off/evaluate" -Body $evalCoolBody
Assert-Result -TestName "8.3 POST /api/cooling-off/evaluate (Tien danh gia do tre 24h & gio cong tuong duong)" -Response $resEvalCool -ExpectedStatus @(200)

# 8.4 POST /api/cooling-off (Khoa tam thoi 24h)
$createCoolBody = @{
    walletId = $script:walletVcbId
    categoryId = $ruleTargetCat
    amount = 3500000
    description = "Tai nghe Bluetooth Sony chong on"
    cooldownHours = 24
}
$resCreateCool = Invoke-Api -Method "POST" -Endpoint "/api/cooling-off" -Body $createCoolBody
Assert-Result -TestName "8.4 POST /api/cooling-off (Khoi tao giao dich cho duyet 24h)" -Response $resCreateCool -ExpectedStatus @(200)
if ($resCreateCool.Body -and $resCreateCool.Body.id) { $script:coolingOffId = $resCreateCool.Body.id }

# 8.5 GET /api/cooling-off/pending
$resPendingCool = Invoke-Api -Method "GET" -Endpoint "/api/cooling-off/pending"
Assert-Result -TestName "8.5 GET /api/cooling-off/pending (Xem danh sach mon do dang cho het 24h)" -Response $resPendingCool -ExpectedStatus @(200)

# 8.6 POST /api/cooling-off/{id}/cancel (Huy mua de chien thang cam do chi tieu)
if (-not [string]::IsNullOrEmpty($script:coolingOffId)) {
    $resCancelCool = Invoke-Api -Method "POST" -Endpoint "/api/cooling-off/$script:coolingOffId/cancel"
    Assert-Result -TestName "8.6 POST /api/cooling-off/{id}/cancel (Huy giao dich - Chien thang cam do mua sam)" -Response $resCancelCool -ExpectedStatus @(200)
}

# ------------------------------------------------------------------------------
# 9. PHAN HE TAC TU TU HANH & HO HOM JEV (AUTONOMOUS ACTIONS)
# ------------------------------------------------------------------------------
Write-Section "9. PHAN HE 9: TAC TU TU HANH & HO HOM JEV (AUTONOMOUS ACTIONS)"

# 9.1 POST /api/actions/from-notification (Bat bien dong so du bien thanh hanh dong)
$bankNotifBody = @{
    packageName = "com.acb.mobile"
    title = "ACB ONE - Bien dong so du"
    message = "TK 123xxx GD -185,000VND tai HIGHLANDS COFFEE vao luc 14:30"
    detectedAmount = 185000
    notificationTime = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
}
$resActionFromNotif = Invoke-Api -Method "POST" -Endpoint "/api/actions/from-notification" -Body $bankNotifBody
Assert-Result -TestName "9.1 POST /api/actions/from-notification (Phan tich bien dong so du thanh Hanh dong Jev)" -Response $resActionFromNotif -ExpectedStatus @(200)
if ($resActionFromNotif.Body -and $resActionFromNotif.Body.actionId) { $script:actionInboxId = $resActionFromNotif.Body.actionId }

# 9.2 GET /api/actions/inbox
$resActionInbox = Invoke-Api -Method "GET" -Endpoint "/api/actions/inbox"
Assert-Result -TestName "9.2 GET /api/actions/inbox (Xem hop thu Hanh dong dang cho duyet)" -Response $resActionInbox -ExpectedStatus @(200)

# 9.3 POST /api/actions/quick-commit (1-cham chap nhan hanh dong vao vi VCB dang co so du)
if (-not [string]::IsNullOrEmpty($script:actionInboxId)) {
    $quickCommitBody = @{
        actionId = $script:actionInboxId
        amount = 185000
        walletName = "VCB"
        categoryName = "Ăn uống"
        note = "Highlands Coffee (Tu dong Jev)"
    }
    $resQuickCommit = Invoke-Api -Method "POST" -Endpoint "/api/actions/quick-commit" -Body $quickCommitBody
    Assert-Result -TestName "9.3 POST /api/actions/quick-commit (1-Cham xac nhan ghi so thanh cong)" -Response $resQuickCommit -ExpectedStatus @(200)
}

# ------------------------------------------------------------------------------
# 10. PHAN HE TIEN ICH THONG MINH & NHAP DU LIEU LAI (SMART UTILITIES & DATA IMPORT)
# ------------------------------------------------------------------------------
Write-Section "10. PHAN HE 10: TIEN ICH THONG MINH & NHAP DU LIEU LAI (NON-AI)"

# 10.1 GET /api/smart/subscriptions (Quet bay thue bao ngam)
$resSubs = Invoke-Api -Method "GET" -Endpoint "/api/smart/subscriptions"
Assert-Result -TestName "10.1 GET /api/smart/subscriptions (Quet phat hien Bay thue bao ngam Netflix, Spotify...)" -Response $resSubs -ExpectedStatus @(200)

# 10.2 POST /api/smart/vietqr (Tao ma VietQR chuan NAPAS 247)
$vietQrBody = @{
    bankBin = "970436"
    accountNumber = "9988776655"
    accountName = "NGUYEN VAN A"
    amount = 500000
    note = "Thanh toan tien nha thang 10"
}
$resVietQr = Invoke-Api -Method "POST" -Endpoint "/api/smart/vietqr" -Body $vietQrBody
Assert-Result -TestName "10.2 POST /api/smart/vietqr (Tao chuoi EMVCo & anh VietQR NAPAS 247)" -Response $resVietQr -ExpectedStatus @(200)

# 10.3 POST /api/smart/voice-parse (Boc tach cau thoai tieng Viet khong dung AI)
$voiceBody = @{
    transcript = "An pho bo 45 nghin tien mat"
}
$resVoice = Invoke-Api -Method "POST" -Endpoint "/api/smart/voice-parse" -Body $voiceBody
Assert-Result -TestName "10.3 POST /api/smart/voice-parse (Boc tach cau thoai chi tieu bang Luat & Slang tieng Viet)" -Response $resVoice -ExpectedStatus @(200)

# 10.4 POST /api/data-import/preview-text (Xem truoc nhap lieu tu ghi chu phi cau truc)
$previewTextBody = @{
    text = "2026-10-25: Mua cafe 45000; 2026-10-25: Do xang 50000; 2026-10-25: Tien an trua 65000"
}
$resPreviewText = Invoke-Api -Method "POST" -Endpoint "/api/data-import/preview-text" -Body $previewTextBody
Assert-Result -TestName "10.4 POST /api/data-import/preview-text (Tien xem truoc bóc tách ghi chú thu chi)" -Response $resPreviewText -ExpectedStatus @(200)

# 10.5 POST /api/data-import/commit-bulk (Luu hang loat vao so cai)
$bulkCommitBody = @{
    walletId = $script:walletCashId
    selectedRows = @(
        @{
            rowIndex = 1
            transactionDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
            amount = 45000
            transactionType = "Expense"
            categoryName = "Ăn uống"
            note = "Mua cafe"
            isValid = $true
        },
        @{
            rowIndex = 2
            transactionDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
            amount = 50000
            transactionType = "Expense"
            categoryName = "Đi lại"
            note = "Do xang"
            isValid = $true
        }
    )
}
$resCommitBulk = Invoke-Api -Method "POST" -Endpoint "/api/data-import/commit-bulk" -Body $bulkCommitBody
Assert-Result -TestName "10.5 POST /api/data-import/commit-bulk (Ghi so hang loat giao dich tu import)" -Response $resCommitBulk -ExpectedStatus @(200)

# ------------------------------------------------------------------------------
# 11. PHAN HE DU BAO TAI CHINH & WHAT-IF SANDBOX (FORECASTING)
# ------------------------------------------------------------------------------
Write-Section "11. PHAN HE 11: DU BAO TAI CHINH & WHAT-IF SANDBOX (FORECASTING)"

# 11.1 GET /api/forecasting/expenditure
$resForeExp = Invoke-Api -Method "GET" -Endpoint "/api/forecasting/expenditure"
Assert-Result -TestName "11.1 GET /api/forecasting/expenditure (Du bao xu huong chi tieu chuoi thoi gian)" -Response $resForeExp -ExpectedStatus @(200)

# 11.2 GET /api/forecasting/liquidity-depletion
$resLiquidity = Invoke-Api -Method "GET" -Endpoint "/api/forecasting/liquidity-depletion"
Assert-Result -TestName "11.2 GET /api/forecasting/liquidity-depletion (Du bao ngay can kiet thanh khoan vs Ky luong)" -Response $resLiquidity -ExpectedStatus @(200)

# 11.3 GET /api/forecasting/category-risks
$resCatRisks = Invoke-Api -Method "GET" -Endpoint "/api/forecasting/category-risks"
Assert-Result -TestName "11.3 GET /api/forecasting/category-risks (Radar canh bao tham hut danh muc thiet yeu)" -Response $resCatRisks -ExpectedStatus @(200)

# 11.4 POST /api/forecasting/what-if (Mo phong gia dinh What-If 0ms)
$whatIfBody = @{
    salaryChangeAmount = 3000000
    plannedExtraExpenses = @(
        @{
            categoryName = "Du lịch"
            amount = 5000000
            expectedDate = (Get-Date).AddDays(15).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        }
    )
    cutbackPercentage = 10
}
$resWhatIf = Invoke-Api -Method "POST" -Endpoint "/api/forecasting/what-if" -Body $whatIfBody
Assert-Result -TestName "11.4 POST /api/forecasting/what-if (Mo phong tinh huong gia dinh In-Memory 0ms)" -Response $resWhatIf -ExpectedStatus @(200)

# ------------------------------------------------------------------------------
# 12. PHAN HE BAO CAO PHAN TICH DA CHIEU (ANALYTICS & EXPORT)
# ------------------------------------------------------------------------------
Write-Section "12. PHAN HE 12: BAO CAO PHAN TICH DA CHIEU (ANALYTICS & EXPORT)"

$fromDt = (Get-Date).AddMonths(-3).ToString("yyyy-MM-dd")
$toDt = (Get-Date).AddMonths(1).ToString("yyyy-MM-dd")

# 12.1 GET /api/analytics/reports/income-expense
$resIncExpRep = Invoke-Api -Method "GET" -Endpoint "/api/analytics/reports/income-expense?from=$fromDt&to=$toDt&groupBy=month"
Assert-Result -TestName "12.1 GET /api/analytics/reports/income-expense (So sanh Thu - Chi MoM/YoY)" -Response $resIncExpRep -ExpectedStatus @(200)

# 12.2 GET /api/analytics/reports/net-worth
$resNetWorth = Invoke-Api -Method "GET" -Endpoint "/api/analytics/reports/net-worth"
Assert-Result -TestName "12.2 GET /api/analytics/reports/net-worth (Bao cao Gia tri tai san rong Net Worth)" -Response $resNetWorth -ExpectedStatus @(200)

# 12.3 POST /api/analytics/reports/net-worth/snapshot
$snapBody = @{
    notes = "Chot so du quy 4/2026"
}
$resSnap = Invoke-Api -Method "POST" -Endpoint "/api/analytics/reports/net-worth/snapshot" -Body $snapBody
Assert-Result -TestName "12.3 POST /api/analytics/reports/net-worth/snapshot (Chup anh Snapshot Net Worth)" -Response $resSnap -ExpectedStatus @(200)

# 12.4 GET /api/analytics/reports/cashflow
$resCashflow = Invoke-Api -Method "GET" -Endpoint "/api/analytics/reports/cashflow?from=$fromDt&to=$toDt"
Assert-Result -TestName "12.4 GET /api/analytics/reports/cashflow (Bieu do Dong tien thuan Waterfall Analysis)" -Response $resCashflow -ExpectedStatus @(200)

# 12.5 GET /api/analytics/reports/expense-structure
$resStructure = Invoke-Api -Method "GET" -Endpoint "/api/analytics/reports/expense-structure?from=$fromDt&to=$toDt"
Assert-Result -TestName "12.5 GET /api/analytics/reports/expense-structure (Co cau chi tieu 50/30/20 & Kakeibo)" -Response $resStructure -ExpectedStatus @(200)

# 12.6 GET /api/analytics/reports/outliers
$resOutliers = Invoke-Api -Method "GET" -Endpoint "/api/analytics/reports/outliers?days=30&threshold=2.0"
Assert-Result -TestName "12.6 GET /api/analytics/reports/outliers (Phat hien bat thuong chi tieu Z-Score Outlier)" -Response $resOutliers -ExpectedStatus @(200)

# 12.7 GET /api/analytics/reports/export-excel (Xuat bao cao Excel thuan khong AI)
$resExportExcel = Invoke-Api -Method "GET" -Endpoint "/api/analytics/reports/export-excel?year=$curYear&month=$curMonth&smart=false"
Assert-Result -TestName "12.7 GET /api/analytics/reports/export-excel (Xuat file bang tinh Excel giao dich)" -Response $resExportExcel -ExpectedStatus @(200)

# ------------------------------------------------------------------------------
# 13. PHAN HE XOA TAI NGUYEN (CLEANUP DELETE ENDPOINTS)
# ------------------------------------------------------------------------------
Write-Section "13. PHAN HE 13: KIEM THU CAC ENDPOINTS XOA (DELETE OPERATIONS)"

# 13.1 DELETE /api/auto-rules/{id}
if (-not [string]::IsNullOrEmpty($script:autoRuleId)) {
    $resDelRule = Invoke-Api -Method "DELETE" -Endpoint "/api/auto-rules/$script:autoRuleId"
    Assert-Result -TestName "13.1 DELETE /api/auto-rules/{id} (Xoa quy tac tu dong)" -Response $resDelRule -ExpectedStatus @(200)
}

# 13.2 DELETE /api/recurring-incomes/{id}
if (-not [string]::IsNullOrEmpty($script:recurringIncomeId)) {
    $resDelRec = Invoke-Api -Method "DELETE" -Endpoint "/api/recurring-incomes/$script:recurringIncomeId"
    Assert-Result -TestName "13.2 DELETE /api/recurring-incomes/{id} (Xoa thu nhap dinh ky)" -Response $resDelRec -ExpectedStatus @(200)
}

# 13.3 DELETE /api/savings-goals/{id} (Tao muc tieu 0 dong de test xoa an toan theo business rule)
$tempGoalBody = @{
    name = "Muc Tieu Tam De Xoa"
    targetAmount = 5000000
    startDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    targetDate = (Get-Date).AddMonths(6).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    type = 1
    priority = 2
    initialAmount = 0
    description = "Muc tieu tam de test xoa"
    iconKey = "trash"
    colorHex = "#94A3B8"
}
$resTempGoal = Invoke-Api -Method "POST" -Endpoint "/api/savings-goals" -Body $tempGoalBody
if ($resTempGoal.Body -and $resTempGoal.Body.id) {
    $tempGoalId = $resTempGoal.Body.id
    $resDelGoal = Invoke-Api -Method "DELETE" -Endpoint "/api/savings-goals/$tempGoalId"
    Assert-Result -TestName "13.3 DELETE /api/savings-goals/{id} (Xoa muc tieu tich luy co so du = 0)" -Response $resDelGoal -ExpectedStatus @(200, 204)
}

# 13.4 DELETE /api/budgets/{id}
if (-not [string]::IsNullOrEmpty($script:budgetId)) {
    $resCloseBudget = Invoke-Api -Method "DELETE" -Endpoint "/api/budgets/$script:budgetId"
    Assert-Result -TestName "13.4 DELETE /api/budgets/{id} (Dong ngan sach)" -Response $resCloseBudget -ExpectedStatus @(200)
}

# 13.5 DELETE /api/categories/{id} (Tao danh muc chua phat sinh giao dich de test xoa an toan)
$tempCatBody = @{
    name = "Danh Muc Tam De Xoa"
    transactionType = 2
    iconKey = "tag"
    colorHex = "#94A3B8"
    displayOrder = 99
}
$resTempCat = Invoke-Api -Method "POST" -Endpoint "/api/categories" -Body $tempCatBody
if ($resTempCat.Body -and $resTempCat.Body.id) {
    $tempCatId = $resTempCat.Body.id
    $resDelCat = Invoke-Api -Method "DELETE" -Endpoint "/api/categories/$tempCatId"
    Assert-Result -TestName "13.5 DELETE /api/categories/{id} (Xoa danh muc tuy bien khong co giao dich)" -Response $resDelCat -ExpectedStatus @(200)
}

# 13.6 DELETE /api/transactions/expenses/{id}
if (-not [string]::IsNullOrEmpty($script:expenseTxId)) {
    $resDelExp = Invoke-Api -Method "DELETE" -Endpoint "/api/transactions/expenses/$script:expenseTxId"
    Assert-Result -TestName "13.6 DELETE /api/transactions/expenses/{id} (Xoa giao dich chi tieu)" -Response $resDelExp -ExpectedStatus @(200)
}

# 13.7 DELETE /api/transactions/incomes/{id}
if (-not [string]::IsNullOrEmpty($script:incomeTxId)) {
    $resDelInc = Invoke-Api -Method "DELETE" -Endpoint "/api/transactions/incomes/$script:incomeTxId"
    Assert-Result -TestName "13.7 DELETE /api/transactions/incomes/{id} (Xoa giao dich thu nhap)" -Response $resDelInc -ExpectedStatus @(200)
}

# 13.8 DELETE /api/wallets/{id} (Tao vi 0 dong de test quy tac chi duoc xoa vi co so du = 0)
$tempWalletBody = @{
    name = "Vi Tam Dung De Xoa"
    walletType = 0
    currency = "VND"
    initialBalance = 0
    colorHex = "#94A3B8"
    iconKey = "wallet"
}
$resTempWallet = Invoke-Api -Method "POST" -Endpoint "/api/wallets" -Body $tempWalletBody
if ($resTempWallet.Body -and $resTempWallet.Body.id) {
    $tempWalletId = $resTempWallet.Body.id
    $resDelWallet = Invoke-Api -Method "DELETE" -Endpoint "/api/wallets/$tempWalletId"
    Assert-Result -TestName "13.8 DELETE /api/wallets/{id} (Xoa mem tai khoan vi co so du = 0)" -Response $resDelWallet -ExpectedStatus @(200)
}

# ------------------------------------------------------------------------------
# 14. PHAN HE KIEM TRA BAT LOI DINH SAN (SAD PATHS & NEGATIVE CASES)
# ------------------------------------------------------------------------------
Write-Section "14. PHAN HE 14: KIEM TRA BAT LOI DINH SAN (SAD PATHS - 400, 401, 404)"

# 14.1 401 Unauthorized khi khong co JWT Token
$resNoToken = Invoke-Api -Method "GET" -Endpoint "/api/wallets" -UseAuth $false
Assert-Result -TestName "14.1 [401] Tu choi truy cap khi thieu JWT Bearer Token" -Response $resNoToken -ExpectedStatus @(401)

# 14.2 401 Unauthorized khi sai mat khau
$wrongPassBody = @{
    email = $testEmail
    password = "WrongPassword999!"
}
$resWrongPass = Invoke-Api -Method "POST" -Endpoint "/api/auth/login" -Body $wrongPassBody -UseAuth $false
Assert-Result -TestName "14.2 [401] Dang nhap that bai khi nhap sai mat khau" -Response $resWrongPass -ExpectedStatus @(401)

# 14.3 400 Bad Request khi email sai dinh dang
$badEmailBody = @{
    fullName = "Bad Email User"
    email = "invalid-email-format"
    password = "Password123!"
}
$resBadEmail = Invoke-Api -Method "POST" -Endpoint "/api/auth/register" -Body $badEmailBody -UseAuth $false
Assert-Result -TestName "14.3 [400] Dang ky that bai khi dia chi email sai cu phap" -Response $resBadEmail -ExpectedStatus @(400)

# 14.4 400 Bad Request khi tao vi ten rong
$emptyWalletBody = @{
    name = "   "
    walletType = 0
    currency = "VND"
    initialBalance = 1000000
}
$resEmptyWallet = Invoke-Api -Method "POST" -Endpoint "/api/wallets" -Body $emptyWalletBody
Assert-Result -TestName "14.4 [400] Tu choi tao vi khi ten tai khoan de trong" -Response $resEmptyWallet -ExpectedStatus @(400)

# 14.5 400 Bad Request khi chuyen tien vuot qua so du vi nguon
if (-not [string]::IsNullOrEmpty($script:walletVcbId)) {
    $overTransferBody = @{
        sourceWalletId = $script:walletVcbId
        destinationWalletId = $script:walletVcbId
        amount = 9999999999
        transferDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        note = "Chuyen tien vuot qua so du hien co"
    }
    $resOverTransfer = Invoke-Api -Method "POST" -Endpoint "/api/wallets/transfer" -Body $overTransferBody
    Assert-Result -TestName "14.5 [400] Tu choi chuyen tien khi so tien vuot qua so du vi" -Response $resOverTransfer -ExpectedStatus @(400)
}

# 14.6 400 Bad Request khi ghi nhan chi tieu so tien am
$negExpenseBody = @{
    walletId = $script:walletVcbId
    categoryId = $ruleTargetCat
    amount = -50000
    transactionDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    note = "Chi tieu so am khong hop le"
}
$resNegExp = Invoke-Api -Method "POST" -Endpoint "/api/transactions/expenses" -Body $negExpenseBody
Assert-Result -TestName "14.6 [400] Tu choi ghi nhan chi tieu khi so tien nho hon hoac bang 0" -Response $resNegExp -ExpectedStatus @(400)

# 14.7 400 Bad Request khi tao ngan sach voi EndDate truoc StartDate
$badBudgetBody = @{
    name = "Ngan sach ngay sai lech"
    periodType = 1
    startDate = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    endDate = (Get-Date).AddDays(-5).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    totalAmount = 10000000
    method = 1
}
$resBadBudget = Invoke-Api -Method "POST" -Endpoint "/api/budgets" -Body $badBudgetBody
Assert-Result -TestName "14.7 [400] Tu choi tao ngan sach khi ngay ket thuc truoc ngay bat dau" -Response $resBadBudget -ExpectedStatus @(400)

# 14.8 404 Not Found khi truy van tai nguyen khong ton tai
$fakeGuid = [System.Guid]::NewGuid().ToString()
$resNotFound = Invoke-Api -Method "GET" -Endpoint "/api/wallets/$fakeGuid"
Assert-Result -TestName "14.8 [404] Tra ve Not Found khi truy van ID khong ton tai trong he thong" -Response $resNotFound -ExpectedStatus @(404)

# ------------------------------------------------------------------------------
# 15. PHAN HE DON DEP & RESET CSDL VE TRANG THAI CHUAN (DEMO STORY SEEDING)
# ------------------------------------------------------------------------------
Write-Section "15. PHAN HE 15: DON DEP DU LIEU VA RESET CSDL VE TRANG THAI CHUAN"

# 15.1 POST /api/system/reset-database
$resResetDb = Invoke-Api -Method "POST" -Endpoint "/api/system/reset-database" -UseAuth $false
Assert-Result -TestName "15.1 POST /api/system/reset-database (Don sach du lieu rac sau khi test)" -Response $resResetDb -ExpectedStatus @(200)

# 15.2 POST /api/system/seed-demo-story
$resSeedDemo = Invoke-Api -Method "POST" -Endpoint "/api/system/seed-demo-story" -UseAuth $false
Assert-Result -TestName "15.2 POST /api/system/seed-demo-story (Nap lai kich ban Demo Story hoan chinh)" -Response $resSeedDemo -ExpectedStatus @(200)

# ==============================================================================
# TONG KET BAO CAO NGHIEM THU & CHO LENH NGUOI DUNG
# ==============================================================================

Write-Header "TONG KET KET QUA BO KIEM THU TU DONG MASTER TEST SUITE"
$passRate = 0
if ($script:totalTests -gt 0) {
    $passRate = [math]::Round(($script:passedTests / $script:totalTests) * 100, 2)
}

Write-Host " Tong so bai kiem tra da chay : " -NoNewline
Write-Host "$script:totalTests" -ForegroundColor White
Write-Host " So bai kiem tra DAT (PASS)   : " -NoNewline
Write-Host "$script:passedTests" -ForegroundColor Green
Write-Host " So bai kiem tra LOI (FAIL)   : " -NoNewline
if ($script:failedTests -gt 0) {
    Write-Host "$script:failedTests" -ForegroundColor Red
} else {
    Write-Host "0" -ForegroundColor Green
}
Write-Host " Ty le thanh cong             : " -NoNewline
if ($passRate -eq 100) {
    Write-Host "$passRate% (XUAT SAC - HOAN HAO)" -ForegroundColor Green
} else {
    Write-Host "$passRate%" -ForegroundColor Yellow
}
Write-Host "================================================================================" -ForegroundColor Cyan

if ($script:failedTests -eq 0) {
    Write-Host " KET LUAN: Toan bo cac API trong he thong Backend da duoc kiem thu thanh cong 100%!" -ForegroundColor Green
    Write-Host " Co so du lieu da duoc Reset va Nap san sang cho Demo Story (demo@expensetracker.vn / admin@expensetracker.com)." -ForegroundColor Cyan
} else {
    Write-Host " KET LUAN: Co $script:failedTests bai kiem thu khong dat yeu cau. Vui long kiem tra chi tiet phan hoi o tren." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "--------------------------------------------------------------------------------" -ForegroundColor DarkGray
# Dung man hinh cho nguoi dung nhan Enter moi duoc dong, khong de tien trinh tu tat
Read-Host -Prompt " [HOAN TAT] Nhan phim Enter de dong cua so kiem thu..."
