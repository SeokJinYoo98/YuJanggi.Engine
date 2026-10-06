@echo off
setlocal EnableExtensions
set "TARGET="
set "VERSION="
set /p "TARGET=Target: "
set /p "VERSION=Version: "
if not defined TARGET (
    echo [ERROR] Target is required.
    goto FAILED
)
if not defined VERSION (
    echo [ERROR] Version is required.
    goto FAILED
)
powershell -NoProfile -NonInteractive -ExecutionPolicy Bypass -File "%~dp0SetVersion.ps1" -Target "%TARGET%" -Version "%VERSION%"
if errorlevel 1 (
    echo [ERROR] SetVersion failed.
    goto FAILED
)
call "%~dp0BuildPackage.bat" "%TARGET%"
if errorlevel 1 (
    echo [ERROR] BuildPackage failed.
    goto FAILED
)
echo Package completed.
echo Target: %TARGET%
echo Version: %VERSION%
pause
exit /b 0
:FAILED
pause
exit /b 1
