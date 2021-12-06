
@echo off

rem Updates IGLib external dependencies in workspace/base/iglibeternal/
rem Used for testing and troubleshooting purposes.

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

echo    Updating external libraries container...
call "%~dp0\scripts\UpdateIGLibExternal.bat"

echo    Updating external ActiVizDotNet ...
call "%~dp0\..\..\iglibexternal\IGLibExternal\UpdateActiVizDotNet.bat" %*

echo    Updating external AForgeDotNet ...
call "%~dp0\..\..\iglibexternal\IGLibExternal\UpdateAForgeDotNet.bat" %*

rem echo    Updating external AccordDotNet ...
rem call "%~dp0\..\..\iglibexternal\IGLibExternal\UpdateAccordDotNet.bat" %*

echo.
echo   ... updating external dependencies of IGLib completeed.
echo.

endlocal
