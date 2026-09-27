@echo off
setlocal

rem =========================
rem Root
rem =========================
set "ROOT=%~dp0"

rem =========================
rem Files
rem =========================
set "CSPROJ=%ROOT%src\YuJanggi.Engine.csproj"
set "VERSION_FILE=%ROOT%src\Version.cs"
set "UPM_PACKAGE=%ROOT%upm\package.json"
set "PREPARE_UPM=%ROOT%scripts\Prepare-Upm.ps1"
rem Validate required files before changing any version.
for %%F in ("%CSPROJ%" "%VERSION_FILE%" "%UPM_PACKAGE%" "%PREPARE_UPM%") do (
    if not exist "%%~F" (
        echo Required file not found: %%~F
        pause
        exit /b 1
    )
)

rem =========================
rem Variables
rem =========================
set "CURRENT_VERSION="
set "VERSION="

rem =========================
rem Load Current Version
rem =========================
for /f "tokens=6" %%V in ('findstr /C:"public const string Current" "%VERSION_FILE%"') do (
    set "CURRENT_VERSION=%%V"
)

set "CURRENT_VERSION=%CURRENT_VERSION:"=%"
set "CURRENT_VERSION=%CURRENT_VERSION:;=%"

echo Current Version: %CURRENT_VERSION%

rem =========================
rem New Version Input
rem =========================
:INPUT_VERSION

set "VERSION="
set /p VERSION=New Version:

if "%VERSION%"=="" (
    echo Version is empty.
    echo.
    goto INPUT_VERSION
)

echo.
echo Version: %VERSION%

choice /C YN /M "Is this version correct?"

if errorlevel 2 (
    echo.
    goto INPUT_VERSION
)

echo.
echo Version confirmed: %VERSION%


rem =========================
rem Version.cs Update
rem =========================
echo.
echo Updating Version.cs...

powershell -NoProfile -Command "$path=$env:VERSION_FILE; $version=$env:VERSION; $q=[char]34; $found=$false; $lines=Get-Content -LiteralPath $path; $lines=$lines | ForEach-Object { if($_ -match '^\s*public\s+const\s+string\s+Current\s*='){ $found=$true; '        public const string Current = ' + $q + $version + $q + ';' } else { $_ } }; if(-not $found){ throw 'Version.Current not found.' }; Set-Content -LiteralPath $path -Value $lines -Encoding UTF8"

if errorlevel 1 (
    echo Failed to update Version.cs.
    pause
    exit /b 1
)

echo Version.cs updated: %VERSION%


rem =========================
rem Project version update
rem =========================
echo.
echo Updating csproj version...

powershell -NoProfile -Command "$path=$env:CSPROJ; $version=$env:VERSION; $text=[System.IO.File]::ReadAllText($path); if($text -notmatch '<Version>[^<]+</Version>'){ throw 'Version element not found.' }; $updated=[regex]::Replace($text, '<Version>[^<]+</Version>', '<Version>' + $version + '</Version>', 1); [System.IO.File]::WriteAllText($path,$updated)"

if errorlevel 1 (
    echo Failed to update csproj version.
    pause
    exit /b 1
)

echo csproj version updated: %VERSION%


rem =========================
rem package.json Update
rem =========================
echo.
echo Updating package.json version...

powershell -NoProfile -Command "$path=$env:UPM_PACKAGE; $version=$env:VERSION; $json=Get-Content -LiteralPath $path -Raw | ConvertFrom-Json; $json.version=$version; $json | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $path -Encoding UTF8"

if errorlevel 1 (
    echo Failed to update package.json.
    pause
    exit /b 1
)

echo package.json version updated: %VERSION%


rem =========================
rem Packing NuGet
rem =========================
echo.
echo Packing NuGet package...

dotnet pack "%CSPROJ%" -c Release -o "%ROOT%artifacts\nuget"

if errorlevel 1 (
    echo Failed to pack NuGet package.
    pause
    exit /b 1
)

echo NuGet package created.


rem =========================
rem UPM Package
rem =========================
echo.
echo Preparing UPM package...

powershell -NoProfile -ExecutionPolicy Bypass -File "%PREPARE_UPM%"

if errorlevel 1 (
    echo Failed to prepare UPM package.
    pause
    exit /b 1
)

echo UPM package prepared.


rem =========================
rem Git Commit
rem =========================
rem echo.
rem choice /C YN /M "Commit version %VERSION% changes?"
rem
rem if errorlevel 2 (
rem     echo Commit skipped.
rem     goto END
rem )
rem
rem git add .
rem
rem git commit -m "chore(release): core v%VERSION%"
rem
rem if errorlevel 1 (
rem     echo Failed to create git commit.
rem     pause
rem     exit /b 1
rem )
rem
rem echo Git commit created.


:END

echo.
echo Update completed
pause