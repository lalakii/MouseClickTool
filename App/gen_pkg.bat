@echo off
set procName=7za.exe
%procName% > NUL 2>&1 &
if "%ERRORLEVEL%" NEQ "0" (
    echo Missing 7za.exe, download: https://7-zip.org/download.html
    exit -1
)
set projectName=App
cd ..\obj\ || exit -1
del ..\%projectName%\x64.GZ ..\%projectName%\x86.GZ /Q > NUL 2>&1
%procName% a -tgzip -mx9 -mtm- ..\%projectName%\x64.GZ MouseClickTool_x64.dll > NUL 2>&1
%procName% a -tgzip -mx9 -mtm- ..\%projectName%\x86.GZ MouseClickTool_x86.dll > NUL 2>&1