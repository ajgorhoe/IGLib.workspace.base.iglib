
@echo off

rem Updates all the specific IGLib module.

setlocal

rem Reset the error level (by running an always successfull command):
ver > nul

rem Call bootstrapping script to define basic directories and strings:
call "%~dp0\bootstrappingscripts\BootStrapScripting.bat"

rem For debugging & trobleshooting - output script locations:
echo.
echo Updating IGLib module...
echo   IGLibScripts: %IGLibScripts%
echo   UpdateRepo: %UpdateRepo%
echo.

echo    Updating module IGLibCore ...
call "%UpdateRepo%" "%~dp0\scripts\SettingsIGLibCore.bat"
echo.

echo.
echo   ... updating module completeed.
echo.

endlocal
