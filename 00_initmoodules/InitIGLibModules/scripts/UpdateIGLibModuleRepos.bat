
@echo off

rem Updates all IGLib modulles.

setlocal

rem Reset the error level (by running an always successfull command):
ver > nul

rem Call bootstrapping script to define basic directories and strings:
call "%~dp0\BootStrapScripting.bat"

echo.
echo Updating IGLib modules...
echo   IGLibScripts: %IGLibScripts%
echo   UpdateScript: %UpdateScript%
echo.

rem call "%UpdateScript%" "%~dp0SettingsIGLibScripts.bat"
rem call "%UpdateScript%" "%~dp0SettingsIGLibCore.bat"
rem call "%UpdateScript%" "%~dp0SettingsIGLibEventAggregator.bat"

endlocal

