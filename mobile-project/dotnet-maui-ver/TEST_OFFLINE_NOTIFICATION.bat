@echo off
chcp 65001 >nul
title Kiểm Thử Thông Báo Offline Killed State (ExpenseTracker MAUI)
cd /d "%~dp0"

powershell -NoProfile -ExecutionPolicy Bypass -File "quick-run-scripts\test_offline_notification.ps1"

pause
