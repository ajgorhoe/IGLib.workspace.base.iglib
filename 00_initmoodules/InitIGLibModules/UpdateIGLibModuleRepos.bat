
@echo off

rem Updates all IGLib modulles.

setlocal

rem Reset the error level (by running an always successfull command):
ver > nul

set ScriptDir=%~dp0
set InitialDir=%CD%
set UpdateScript=%ScriptDir%UpdateRepo.bat

echo.
echo Updating IGLib modules...
echo   ScriptDir: %ScriptDir%
echo   UpdateScript: %UpdateScript%
echo.

call "%UpdateScript%" "%ScriptDir%SettingsIGLibScripts.bat"
call "%UpdateScript%" "%ScriptDir%SettingsIGLibCore.bat"
call "%UpdateScript%" "%ScriptDir%SettingsIGLibEventAggregator.bat"

endlocal

