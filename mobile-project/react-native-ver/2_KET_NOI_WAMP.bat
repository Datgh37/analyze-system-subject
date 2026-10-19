@echo off
chcp 65001 >nul
echo =====================================================================
echo [NHOM 46 - DNTU] LIEN KET THU MUC BACKEND BE VOI WAMP SERVER WWW
echo =====================================================================
echo Dang chay script tao Junction vao thu muc www cua WampServer...
powershell -ExecutionPolicy Bypass -File .\BE\setup_wamp_junction.ps1
pause
