@echo off
chcp 65001 >nul
title Dọn Dẹp Toàn Diện Cache & Khởi Chạy Android (.NET MAUI 10)
echo ==========================================================
echo  🧹 DỌN DẸP SẠCH SẼ CACHE (BIN/OBJ) & REBUILD ANDROID
echo ==========================================================
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0quick-run-scripts\run_android.ps1" -Clean
pause

