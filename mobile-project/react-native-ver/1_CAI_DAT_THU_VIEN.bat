@echo off
chcp 65001 >nul
echo =====================================================================
echo [NHOM 46 - DNTU] CAI DAT THU VIEN CHO REALTIME SERVER VA REACT NATIVE
echo =====================================================================

echo.
echo [1/2] Dang cai dat thu vien cho Realtime Socket.IO Server...
cd realtime-server
call npm install
if %ERRORLEVEL% NEQ 0 (
    echo [!] Co loi xay ra khi cai dat thu vien realtime-server!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] Dang cai dat thu vien cho Frontend Mobile App (Expo React Native)...
cd ..\FE
call npm install
if %ERRORLEVEL% NEQ 0 (
    echo [!] Co loi xay ra khi cai dat thu vien FE!
    pause
    exit /b %ERRORLEVEL%
)

cd ..
echo.
echo =====================================================================
echo [THANH CONG] Da cai dat xong toan bo thu vien!
echo Tiep theo:
echo 1. Chay '2_KET_NOI_WAMP.bat' de tao lien ket voi WampServer.
echo 2. Chay '3_CHAY_REALTIME_SERVER.bat' de bat Socket.IO (Port 3001).
echo 3. Chay '4_CHAY_APP_MOBILE.bat' de bat ung dung Expo (Port 8081).
echo =====================================================================
pause
