
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
echo.
echo   ... updating IGLib modules completed.
echo.
echo Updating external modules...
echo.
call "%UpdateRepo%" "%~dp0\SettingsExternalMatnNetNumerics.bat"
call "%UpdateRepo%" "%~dp0\SettingsExternalZedGraph.bat"
echo.
echo   ... Updating external modules completeed.
echo.
REM echo Updating Git submodules...
REM git.exe submodule update --progress --init -- "%~dp0/../../../external/mathnet-numerics"
REM git.exe submodule update --progress --init -- "%~dp0/../../../external/ZedGraph"
REM echo   ... updating Git modules done.
REM echo.


endlocal

