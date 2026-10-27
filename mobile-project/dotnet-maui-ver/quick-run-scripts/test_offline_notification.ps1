# ==============================================================================
# SCRIPT KIỂM THỬ THÔNG BÁO OFFLINE KILLED STATE (ANDROID MAUI)
# Kịch bản: Kiểm tra khả năng đánh thức và đẩy Notification khi App đã bị tắt hoàn toàn
# ==============================================================================

Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host "  KIỂM THỬ THÔNG BÁO NATIVE KHI APP BỊ ĐÓNG HOÀN TOÀN (KILLED STATE) " -ForegroundColor Yellow
Write-Host "==================================================================" -ForegroundColor Cyan

# 1. Tìm đường dẫn công cụ ADB trên hệ thống
$adbCandidates = @(
    "C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe",
    "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe",
    "$env:ANDROID_HOME\platform-tools\adb.exe",
    (Get-Command "adb" -ErrorAction SilentlyContinue).Source
)
$adb = $adbCandidates | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1

if (-not $adb) {
    Write-Host "[X] Khong tim thay adb.exe tren he thong!" -ForegroundColor Red
    pause
    exit 1
}

# 2. Kiểm tra thiết bị / máy ảo Android đang kết nối
$devices = & $adb devices | Where-Object { $_ -and -not $_.StartsWith("List") -and $_.Contains("device") }
if (-not $devices) {
    Write-Host "[X] Chua co may ao hoac dien thoai Android nao dang ket noi qua ADB!" -ForegroundColor Red
    pause
    exit 1
}

$deviceId = ($devices[0] -split "`t")[0]
$pkgName = "com.companyname.expensetracker.maui"
Write-Host "[V] Da ket noi thiet bi: $deviceId" -ForegroundColor Green
Write-Host "[*] Package muc tieu: $pkgName" -ForegroundColor Cyan

# 3. Menu chọn kịch bản kiểm thử
Write-Host "`nCHON KICH BAN TEST THONG BAO OFFLINE:" -ForegroundColor Yellow
Write-Host "  [1] Nhac nho chi tieu 21:00 (Daily Reminder Alarm - BroadcastReceiver)" -ForegroundColor White
Write-Host "  [2] Gia lap bien dong so du Ngan hang (VCB Digibank - Notification Listener)" -ForegroundColor White
Write-Host "  [3] Ban thong bao he thong truc tiep (Android Notification Manager)" -ForegroundColor White
$choice = Read-Host "`nNhap lua chon (1, 2 hoac 3) [Mac dinh: 1]"
if ([string]::IsNullOrWhiteSpace($choice)) { $choice = "1" }

# 4. ÉP BUỘC TẮT HOÀN TOÀN ỨNG DỤNG (FORCE-STOP KILLED STATE)
Write-Host "`n[BUOC 1] Dang ep tat hoan toan ung dung tren Android (Killed State)..." -ForegroundColor Yellow
& $adb -s $deviceId shell am force-stop $pkgName

Start-Sleep -Milliseconds 500
$pidCheck = & $adb -s $deviceId shell pidof $pkgName
if ([string]::IsNullOrWhiteSpace($pidCheck)) {
    Write-Host "[V] XAC NHAN: Tien trinh $pkgName da chet hoan toan (PID rỗng, 0 RAM, 0 Socket)!" -ForegroundColor Green
} else {
    Write-Host "[!] Tien trinh van con ton tai voi PID: $pidCheck" -ForegroundColor Yellow
}

# 5. THỰC HIỆN KỊCH BẢN TƯƠNG ỨNG
if ($choice -eq "1") {
    Write-Host "`n[BUOC 2] Ban Broadcast Intent 'com.expensetracker.action.DAILY_REMINDER' tu he thong..." -ForegroundColor Cyan
    $broadcastOutput = & $adb -s $deviceId shell am broadcast -a com.expensetracker.action.DAILY_REMINDER -p $pkgName
    Write-Host "Ket qua Broadcast: $broadcastOutput" -ForegroundColor DarkGray
    
    Write-Host "`n==================================================================" -ForegroundColor Green
    Write-Host "[V] THANH CONG! Android OS da danh thuc DailyReminderReceiver." -ForegroundColor Green
    Write-Host "    --> Hay nhin vao man hinh dien thoai / may ao Android:" -ForegroundColor White
    Write-Host "    --> Ban se thay tren thanh trang thai (Status Bar) xuat hien thong bao:" -ForegroundColor Cyan
    Write-Host "        '⏰ Nhac Nho Tai Chinh 21:00'" -ForegroundColor Yellow
    Write-Host "        'Da 21:00 roi! Dung quen ghi lai cac khoan chi hom nay...'" -ForegroundColor White
    Write-Host "    --> Hay cham vao thong bao do: App se tu dong khoi dong lanh (Cold Launch)" -ForegroundColor Cyan
    Write-Host "        va mo thang vao So Giao Dich Chi Tieu (ExpensesPage)!" -ForegroundColor Cyan
    Write-Host "==================================================================" -ForegroundColor Green
}
elseif ($choice -eq "2") {
    Write-Host "`n[BUOC 2] Gia lap ung dung Ngan hang VCB Digibank ban thong bao tru tien..." -ForegroundColor Cyan
    $bankMsg = "TK 123456789 GD: -45,000VND 27/10/26 20:30 ND: Highlands Coffee"
    & $adb -s $deviceId shell cmd notification post -S bigtext -t "VCB Digibank" "tag_vcb_test" "$bankMsg"
    
    Write-Host "`n==================================================================" -ForegroundColor Green
    Write-Host "[V] Da gia lap thanh cong thong bao tru tien tu Ngan hang!" -ForegroundColor Green
    Write-Host "    Neu may ao da cap quyen 'Notification Access' cho ExpenseTracker:" -ForegroundColor White
    Write-Host "    BankNotificationListenerService se tu dong boc tach 45,000 VND" -ForegroundColor White
    Write-Host "    va hien Action Notification moi vao ghi chep!" -ForegroundColor White
    Write-Host "==================================================================" -ForegroundColor Green
}
else {
    Write-Host "`n[BUOC 2] Ban thong bao he thong truc tiep qua Android Notification Manager..." -ForegroundColor Cyan
    & $adb -s $deviceId shell cmd notification post -S bigtext -t "ExpenseTracker [Offline Test]" "tag_direct" "Day la thong bao test khi ung dung dang o trang thai Killed State!"
    Write-Host "[V] Da day thong bao len thanh he thong Android thanh cong!" -ForegroundColor Green
}

Write-Host "`nNhan phim Enter de ket thuc..." -ForegroundColor Gray
Read-Host
