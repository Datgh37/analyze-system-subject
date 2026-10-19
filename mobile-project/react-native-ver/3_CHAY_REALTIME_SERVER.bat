@echo off
chcp 65001 >nul
title [REALTIME SERVER] Socket.IO Gateway Port 3001
echo =====================================================================
echo [NHOM 46 - DNTU] KHOI DONG REALTIME SOCKET.IO SERVER (PORT 3001)
echo =====================================================================
cd realtime-server
call npm start
pause
