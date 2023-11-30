@echo off
setlocal

rem Get the current date and time:
for /F "tokens=2" %%i in ('date /t') do set currentdate=%%i
set currenttime=%time%

rem Format date and time to a filename-friendly format:
set "currentdatetime=%date:~-4,4%-%date:~-10,2%-%date:~-7,2%-%time:~0,2%-%time:~3,2%-%time:~6,2%"
set "currentdatetime=%currentdatetime: =0%"

rem If argument 1 is not given, use default value for PackageVersion:
set "PackageVersion=%~1"
if not defined PackageVersion set "PackageVersion=0.1.4-private-%currentdatetime%"

rem If argument 2 is not given, use default value for XRSharpVersion:
set "XRSharpVersion=%~2"
if not defined XRSharpVersion set "XRSharpVersion=0.1.4-preview-2023-11-29-105504-f5d71aac"

if not exist "nuspec/XRSharp.CommunityToolkit.nuspec" (
    echo Wrong working directory. Please navigate to the folder that contains the BAT file before executing it.
    exit /b 1
)

rem Define the escape character for colored text
for /F %%a in ('"prompt $E$S & echo on & for %%b in (1) do rem"') do set "ESC=%%a"

rem Create a Version.txt file with the date:
md temp
@echo XRSharp.CommunityToolkit %PackageVersion% (%currentdate% %currenttime%)> temp/Version.txt

echo.
echo %ESC%[95mBuilding %ESC%[0mRuntime %ESC%[95mproject%ESC%[0m
echo.
msbuild ../src/XRSharp.CommunityToolkit.sln -p:Configuration=Release -clp:ErrorsOnly -restore

if %ERRORLEVEL% neq 0 (
    echo Can not build the XRSharp. Exiting script.
    exit /b 1
)

echo. 
echo %ESC%[95mPacking %ESC%[0mXRSharp %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack nuspec\XRSharp.CommunityToolkit.nuspec -OutputDirectory "output" -Properties "PackageId=XRSharp.CommunityToolkit;PackageVersion=%PackageVersion%;Configuration=Release;XRSharpVersion=%XRSharpVersion%;RepositoryUrl=https://github.com/XRSharp/XRSharp.CommunityToolkit"

if %ERRORLEVEL% neq 0 (
    echo Can not pack the XRSharp nuget package. Exiting script.
    exit /b 1
)