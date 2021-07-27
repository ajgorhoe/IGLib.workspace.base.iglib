
@echo off
rem This script is for simple update of an embedded repository.

setlocal

rem Reset the error level (by running an always successfull command):
ver > nul
rem Base directories:
set ScriptDir=%~dp0
set InitialDir=%CD%

rem Skip the settings section:
goto afterSettings
rem Parameters for the update:
set ModuleDirRelative=..\..\modules\IGLibCore
set CheckoutBranch=master
set RepositoryAddress=https://github.com/ajgorhoe/IGLib.modules.IGLibCore.git
set RepositoryAddressSecondary=https://ajgorhoe@bitbucket.org/ajgorhoe/iglib.modules.iglibcore.git
set RepositoryAddressLocal=d:/backup_sync/bk_code/git/ig/misc/iglib_modules/IGLibCore/
set Remote=origin
set RemoteSecondary=originBitBucket
set RemoteLocal=local
:afterSettings

rem If command-line arguments were specified then take them as another command and run the command:
if "%~1" NEQ "" (
    echo.
    echo Executing recursve commnd specified by arguments:
    echo   %~1 %~2 %~3 %~4 %~5 %~6 %~7 %~8 %~9
    call "%~1" "%~2" "%~3" "%~4" "%~5" "%~6" "%~7" "%~8" "%~9"
)

echo.
echo Parameters from module update from a repository 
echo   (set by the recursive command specified by arguments):
echo.
echo "ModuleDirRelative": "%ModuleDirRelative%"
echo "ModuleDir":         "%ModuleDir%"
echo "CheckoutBranch":    "%CheckoutBranch%"
echo "Remote":            "%Remote%"
echo "RemoteSecondary":   "%RemoteSecondary%"
echo "RemoteLocal":       "%RemoteLocal%"
echo.
echo "RepositoryAddress": "%RepositoryAddress%"
echo "RepositoryAddressSecondary": "%RepositoryAddressSecondary%"
echo "RepositoryAddressLocal": "%RepositoryAddressLocal%"
echo.


:finalize

cd %InitialDir%
ver > nul

endlocal

