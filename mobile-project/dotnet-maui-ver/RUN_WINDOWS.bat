@echo off
chcp 65001 >nul
title Khởi Chạy Ứng Dụng Expense Tracker - Windows Desktop
echo ==========================================================
echo  🚀 KHOI CHAY 1-CLICK: WINDOWS DESKTOP (.NET MAUI 10)
echo ==========================================================
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0quick-run-scripts\run_windows.ps1"
