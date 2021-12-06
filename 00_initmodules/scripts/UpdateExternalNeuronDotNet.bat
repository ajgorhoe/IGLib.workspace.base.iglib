
@echo off

rem Updates all the specific IGLib module. For testing purposes.

setlocal




echo    Updating module ExternalNeuronDotNet ...
call "%~dp0\..\..\..\iglibexternal\IGLibExternal\UpdateNeuronDotNet.bat" %*
echo.

echo.
echo   ... updating module completeed.
echo.

endlocal
