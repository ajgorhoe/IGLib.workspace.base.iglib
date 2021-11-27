
@echo off

rem Updates all IGLib modules.

setlocal

rem Reset the error level (by running an always successfull command):
ver > nul

rem Call bootstrapping script to define basic directories and strings:
call "%~dp0\bootstrappingscripts\BootStrapScripting.bat"

rem also make sure that iglibexternal/ is checked out / updated:
if not exist "%~dp0\..\..\iglibexternal\IGLibExternal\UpdateNeuronDotNet.bat" (
  call "%~dp0\..\..\UpdateModule_iglibexternal.bat"
)

echo.
echo Updating IGLib modules...
echo   IGLibScripts: %IGLibScripts%
echo   UpdateRepo: %UpdateRepo%
echo.
echo    Updating module IGLibScripts ...
call "%UpdateRepo%" "%~dp0\scripts\SettingsIGLibScripts.bat"
echo    Updating module IGLibCore ...
call "%UpdateRepo%" "%~dp0\scripts\SettingsIGLibCore.bat"
echo    Updating module IGLibEventAggregator ...
call "%UpdateRepo%" "%~dp0\scripts\SettingsIGLibEventAggregator.bat"
echo.
echo   ... updating IGLib modules completed.


echo.
echo Updating external modules...
echo.
echo    Updating module MatnNetNumerics ...
call "%UpdateRepo%" "%~dp0\scripts\SettingsExternalMathNetNumerics.bat"
echo    Updating module ZedGraph ...
call "%UpdateRepo%" "%~dp0\scripts\SettingsExternalZedGraph.bat"


rem Lines below are new. When solutions are changed, remove the 
rem corresponding lines above for local updates!
echo    Updating external MatnNetNumerics ...
call "%~dp0\..\..\iglibexternal\IGLibExternal\UpdateMatnNetNumerics.bat" %*
echo    Updating external ZedGraph ...
call "%~dp0\..\..\iglibexternal\IGLibExternal\UpdateZedGraph.bat" %*

echo    Updating external NeuronDotNet ...
call "%~dp0\..\..\iglibexternal\IGLibExternal\UpdateNeuronDotNet.bat" %*
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

ver > nul

