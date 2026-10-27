@echo off
chcp 65001 >nul
title Kiểm Thử Toàn Diện API Backend - Expense Tracker
echo ==========================================================
echo  🧪 KIEM THU TU DONG TOAN BO HE THONG REST API
echo ==========================================================
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0quick-run-scripts\test_api_suite.ps1"
