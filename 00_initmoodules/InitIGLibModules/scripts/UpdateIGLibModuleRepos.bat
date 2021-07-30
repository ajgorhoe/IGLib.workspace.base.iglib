
@echo off

rem Updates all IGLib modulles.

setlocal

rem Reset the error level (by running an always successfull command):
ver > nul

rem Call bootstrapping script to define basic directories and strings:
call "%~dp0BootStrapScripting.bat"

echo.
echo Updating IGLib modules...
echo   IGLibScripts: %IGLibScripts%
echo   UpdateScript: %UpdateScript%
echo.

call "%UpdateScript%" "%~dp0SettingsIGLibScripts.bat"
call "%UpdateScript%" "%~dp0SettingsIGLibCore.bat"
call "%UpdateScript%" "%~dp0SettingsIGLibEventAggregator.bat"

endlocal

