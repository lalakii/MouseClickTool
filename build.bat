@echo off
set MSBuild="C:\Program Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\MSBuild.exe"
if not exist %MSBuild% (
    echo Missing MSBuild.exe
    pause
)
%MSBuild% -noLogo MouseClickTool.sln /p:Configuration="Windows" /p:Platform="x86" > NUL 2>&1 &
%MSBuild% -noLogo MouseClickTool.sln /p:Configuration="Windows" /p:Platform="x64" > NUL 2>&1 &
explorer obj\