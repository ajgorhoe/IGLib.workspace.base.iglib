
@echo off

rem Updates specific external modulles.
rem This script is currently used for testing and we will see where this goes.

setlocal

rem Reset the error level (by running an always successfull command):
ver > nul

rem Call bootstrapping script to define basic directories and strings:
call "%~dp0\BootStrapScripting.bat"

echo.
echo Updating external modules...
echo.
echo    Updating module MatnNetNumerics ...
call "%UpdateRepo%" "%~dp0\SettingsExternalMatnNetNumerics.bat"
echo.
echo   ... Updating external modules completeed.
echo.


endlocal

