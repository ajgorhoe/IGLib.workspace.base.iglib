
@echo off

rem Updates all the specific IGLib module. For testing purposes.

setlocal

rem Reset the error level (by running an always successfull command):
ver > nul

rem Call bootstrapping script to define basic directories and strings:
call "%~dp0\..\bootstrappingscripts\BootStrapScripting.bat"

set UpdateScriptContainer=..\..\..\UpdateModule_iglibexternal.bat
if exist "%UpdateScriptContainer%" (
  echo.
  echo IGLib container's update script exists:
  echo   %UpdateScriptContainer%
  echo This script will be called to update iglibexternal/
  echo.
  call "%UpdateScriptContainer%" %*
  goto finalize
)

rem For debugging & trobleshooting - output script locations:
echo.
echo Updating IGLib module...
echo   IGLibScripts: %IGLibScripts%
echo   UpdateRepo: %UpdateRepo%
echo.

echo    Updating module IGLibExternal ...
call "%UpdateRepo%" "%~dp0\SettingsIGLibExternal.bat"
echo.

echo.
echo   ... updating module completeed.
echo.

:finalize

endlocal
