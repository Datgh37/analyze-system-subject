@echo off
chcp 65001 >nul
title [MOBILE APP] Expo React Native Bundler Port 8081
echo =====================================================================
echo [NHOM 46 - DNTU] KHOI DONG UNG DUNG EXPO REACT NATIVE MOBILE APP
echo =====================================================================
echo.
echo Cach xem giao dien ung dung:
echo  1. Nhap phim 'w' de mo truc tiep tren Trinh duyet Web (Chrome/Edge).
echo  2. Hoac mo ung dung 'Expo Go' tren dien thoai de quet ma QR.
echo.
echo =====================================================================
cd FE
call npx expo start
pause
