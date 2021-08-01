
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
echo   UpdateRepo: %UpdateRepo%
echo.

call "%UpdateRepo%" "%~dp0\SettingsIGLibScripts.bat"
call "%UpdateRepo%" "%~dp0\SettingsIGLibCore.bat"
call "%UpdateRepo%" "%~dp0\SettingsIGLibEventAggregator.bat"

endlocal

