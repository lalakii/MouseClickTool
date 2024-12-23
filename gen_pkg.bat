@echo off
7za > NUL 2>&1 &
if "%ERRORLEVEL%" NEQ "0" (
    echo Missing: 7za.exe, please download: https://7-zip.org/
    exit -1
)
cd GzExe && del x86.gz x64.gz /Q
cd ..\obj\
7za a -tgzip -mx9 -mtm- ..\GzExe\x64.gz MouseClickTool_x64.dll > NUL 2>&1 &
7za a -tgzip -mx9 -mtm- ..\GzExe\x86.gz MouseClickTool_x86.dll > NUL 2>&1 &
