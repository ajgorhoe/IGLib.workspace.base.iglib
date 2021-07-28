
@echo off
rem This script is for simple update of an embedded repository.

rem Reset the error level (by running an always successfull command):
ver > nul
rem Base directories:
rem set ScriptDir=%~dp0
rem set InitialDir=%CD%

echo.
echo Defining env. variables: SETTINGS for updating repository: IGLibCore
echo.

rem Parameters for the update:
set ModuleDirRelative=..\..\modules\IGLibCore
set CheckoutBranch=master
set RepositoryAddress=https://github.com/ajgorhoe/IGLib.modules.IGLibCore.git
set RepositoryAddressSecondary=https://ajgorhoe@bitbucket.org/ajgorhoe/iglib.modules.iglibcore.git
set RepositoryAddressLocal=d:/backup_sync/bk_code/git/ig/misc/iglib_modules/IGLibCore/
set Remote=origin
set RemoteSecondary=originBitBucket
set RemoteLocal=local

set ModuleDir=%~dp0%ModuleDirRelative%



rem If command-line arguments were specified then interpret them as another command and run the command:
if "%~1" NEQ "" (
    echo.
    echo Settings script: Executing recursve commnd specified by arguments:
    echo   call "%~1" "%~2" "%~3" "%~4" "%~5" "%~6" "%~7" "%~8" "%~9"
    echo.
    call "%~1" "%~2" "%~3" "%~4" "%~5" "%~6" "%~7" "%~8" "%~9"
)



