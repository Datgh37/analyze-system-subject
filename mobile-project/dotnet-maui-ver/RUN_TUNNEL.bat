@echo off
chcp 65001 >nul
title Khởi Chạy Cloudflare Tunnel - Expense Tracker HTTPS Gateway
echo ==========================================================
echo  🚀 KHOI CHAY 1-CLICK: CLOUDFLARE TUNNEL (HTTPS GATEWAY)
echo ==========================================================
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0quick-run-scripts\run_tunnel.ps1"
pause
