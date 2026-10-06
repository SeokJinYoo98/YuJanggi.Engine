@echo off
setlocal EnableExtensions
set "TARGET=%~1"
if not defined TARGET (
    echo [ERROR] Target is required.
    exit /b 1
)
if not "%~2"=="" (
    echo [ERROR] BuildPackage accepts only Target.
    exit /b 1
)
for %%I in ("%~dp0..") do set "ROOT=%%~fI"
set "CSPROJ=%ROOT%\src\YuJanggi.%TARGET%.csproj"
set "NUGET_OUTPUT=%ROOT%\artifacts\nuget"
set "UPM_OUTPUT=%ROOT%\artifacts\upm"
if not exist "%CSPROJ%" (
    echo [ERROR] Project file not found: "%CSPROJ%"
    exit /b 1
)
if not exist "%~dp0Prepare-Upm.ps1" (
    echo [ERROR] Prepare-Upm.ps1 not found.
    exit /b 1
)
dotnet pack "%CSPROJ%" --configuration Release --output "%NUGET_OUTPUT%"
if errorlevel 1 exit /b 1
powershell -NoProfile -NonInteractive -ExecutionPolicy Bypass -File "%~dp0Prepare-Upm.ps1" -Target "%TARGET%"
if errorlevel 1 exit /b 1
if not exist "%UPM_OUTPUT%" mkdir "%UPM_OUTPUT%"
if errorlevel 1 exit /b 1
pushd "%ROOT%"
if errorlevel 1 exit /b 1
call npm pack ./upm --pack-destination ./artifacts/upm
if errorlevel 1 (
    popd
    exit /b 1
)
popd
echo Package build completed: %TARGET%
exit /b 0
