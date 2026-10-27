@echo off
chcp 65001 >nul
title Khởi Chạy Ứng Dụng Expense Tracker - Android
echo ==========================================================
echo  🚀 KHOI CHAY 1-CLICK: ANDROID (.NET MAUI 10)
echo ==========================================================
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0quick-run-scripts\run_android.ps1" %*
pause
