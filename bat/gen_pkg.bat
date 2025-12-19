@echo off
set procName=7za.exe
%procName% > NUL 2>&1 &
if "%ERRORLEVEL%" NEQ "0" (
    echo 7za.exe not found. Please download it from https://7-zip.org/download.html and add it to your PATH environment variable.
    call gen_pkg.bat
    exit 0
)
set projectName=App
if EXIST "bin\dll\net462\MouseClickTool.dll" (
    del .\%projectName%\x86.GZ /Q > NUL 2>&1
    del .\%projectName%\x64.GZ /Q > NUL 2>&1
    cd .\bin\dll\net462\ 
    %procName% a -tgzip -mx9 -mtm- ..\..\..\%projectName%\x86.GZ MouseClickTool.dll > NUL 2>&1
    copy  ..\..\..\%projectName%\x86.GZ  ..\..\..\%projectName%\x64.GZ > NUL 2>&1
) else (
   exit 0
)