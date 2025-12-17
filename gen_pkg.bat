@echo off


REM ==========================
REM 该批处理脚本用于将编译后的 DLL 压缩为 GZip 格式文件（x86.GZ 和 x64.GZ），
REM 采用系统自带的 PowerShell 来完成压缩，避免依赖外部的 7za.exe。
REM 使用前请确保已构建项目，DLL 位于 bin\dll\net462\MouseClickTool.dll。
REM ==========================

setlocal enabledelayedexpansion

REM Configuration
set "PROJECT_NAME=App"
set "TARGET_FRAMEWORK=net462"
set "DLL_SOURCE=bin\dll\%TARGET_FRAMEWORK%\MouseClickTool.dll"

echo.
echo ==============================
echo MouseClickTool Packaging Tool
echo ==============================
echo.
echo Project: %PROJECT_NAME%
echo Target Framework: %TARGET_FRAMEWORK%
echo.

REM 验证 DLL 是否存在
if not exist "%DLL_SOURCE%" (
    echo [ERROR] DLL not found at: %DLL_SOURCE%
    echo [INFO] Please build the project first
    echo.
    exit /b 1
)

echo [OK] DLL found: %DLL_SOURCE%

REM 验证项目目录是否存在
if not exist ".\%PROJECT_NAME%\" (
    echo [ERROR] Project directory not found: %PROJECT_NAME%
    exit /b 1
)

echo [OK] Project directory found
echo.

REM 使用 PowerShell 进行压缩（无需外部依赖），下面调用 PowerShell 执行压缩逻辑
REM PowerShell 脚本会读取 DLL、使用 .NET GZipStream 压缩为 x86.GZ，
REM 然后拷贝为 x64.GZ。脚本包括错误处理并返回适当的错误代码。
echo [INFO] Attempting to package DLL...
echo.

REM Use PowerShell for robust GZip compression (no external dependencies)
powershell -NoProfile -Command ^
    "$ErrorActionPreference = 'Stop'; ^
    $dllPath = '%DLL_SOURCE%'; ^
    $x86Gz = '.\%PROJECT_NAME%\x86.GZ'; ^
    $x64Gz = '.\%PROJECT_NAME%\x64.GZ'; ^
    try { ^
        if (Test-Path $x86Gz) { Remove-Item $x86Gz -Force }; ^
        if (Test-Path $x64Gz) { Remove-Item $x64Gz -Force }; ^
        $dllContent = [System.IO.File]::ReadAllBytes($dllPath); ^
        $gzStream = [System.IO.File]::Create($x86Gz); ^
        $gzipStream = [System.IO.Compression.GZipStream]::new($gzStream, [System.IO.Compression.CompressionMode]::Compress); ^
        $gzipStream.Write($dllContent, 0, $dllContent.Length); ^
        $gzipStream.Dispose(); ^
        $gzStream.Dispose(); ^
        [System.IO.File]::Copy($x86Gz, $x64Gz, $true); ^
        Write-Host '[OK] Packaging complete'; ^
    } catch { ^
        Write-Host '[ERROR]' $_.Exception.Message; ^
        exit 1; ^
    }"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Packaging failed!
    echo.
    echo Troubleshooting:
    echo - Ensure the project has been built
    echo - Verify output path: bin\dll\%TARGET_FRAMEWORK%\
    echo - Check write permissions to: %PROJECT_NAME%\
    echo.
    exit /b 1
)

echo.
echo [OK] x86.GZ created successfully
if exist ".\%PROJECT_NAME%\x86.GZ" (
    for %%f in (".\%PROJECT_NAME%\x86.GZ") do echo     Size: %%~zf bytes
)

echo [OK] x64.GZ created successfully
if exist ".\%PROJECT_NAME%\x64.GZ" (
    for %%f in (".\%PROJECT_NAME%\x64.GZ") do echo     Size: %%~zf bytes
)

echo.
echo ==============================
echo Packaging completed!
echo Output: .\%PROJECT_NAME%\
echo ==============================
echo.

exit /b 0