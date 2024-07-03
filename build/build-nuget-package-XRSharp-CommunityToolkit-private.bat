@echo off

IF NOT EXIST "nuspec/XRSharp.CommunityToolkit.nuspec" (
echo Wrong working directory. Please navigate to the folder that contains the BAT file before executing it.
PAUSE
EXIT
)

rem Define the escape character for colored text
for /F %%a in ('"prompt $E$S & echo on & for %%b in (1) do rem"') do set "ESC=%%a"

rem Define the "%PackageVersion%" variable:
set /p PackageVersion="%ESC%[92mXRSharp.CommunityToolkit version:%ESC%[0m 0.1.7-private-"

set PackageVersion="0.1.7-private-%PackageVersion%"

set XRSharpVersion="0.1.7-preview-2024-07-03-083945-ec96f694"

call "build-nuget-package-XRSharp-CommunityToolkit-cicd.bat" %PackageVersion% %XRSharpVersion%

explorer "output"

pause
