
@echo off

rem Updates all IGLib modules.

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
echo    Updating module IGLibScripts ...
call "%UpdateRepo%" "%~dp0\SettingsIGLibScripts.bat"
echo    Updating module IGLibCore ...
call "%UpdateRepo%" "%~dp0\SettingsIGLibCore.bat"
echo    Updating module IGLibEventAggregator ...
call "%UpdateRepo%" "%~dp0\SettingsIGLibEventAggregator.bat"
echo.
echo   ... updating IGLib modules completed.
echo.
echo Updating external modules...
echo.
echo    Updating module MatnNetNumerics ...
call "%UpdateRepo%" "%~dp0\SettingsExternalMathNetNumerics.bat"
echo    Updating module ZedGraph ...
call "%UpdateRepo%" "%~dp0\SettingsExternalZedGraph.bat"
echo.
echo   ... Updating external modules completeed.
echo.

REM The block below is commented because explicit clone / checkout is used
REM instead of Git submodules.
REM echo Updating Git submodules...
REM git.exe submodule update --progress --init -- "%~dp0/../../../external/mathnet-numerics"
REM git.exe submodule update --progress --init -- "%~dp0/../../../external/ZedGraph"
REM echo   ... updating Git modules done.
REM echo.


endlocal

