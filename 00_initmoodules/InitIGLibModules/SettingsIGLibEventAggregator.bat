
@echo off

rem Sets parameters for cloning or updating the IGLibEventAggregator repository.

rem Parameters are set as environment variables with agreed names.
rem These settings are used by scripts such as:
rem   UpdateRepo.bat,  PrintRepoSettings.bat, etc.

rem Runnng the script has side effect (variables are set in the calling 
rem context). In order to eliminate side effects, call the script inside
rem a setlocal/endlocal block.

rem If the script is called with command-line PARAMETERS, these are interpreted
rem as EMBEDDED COMMAND with eventual parameters, which is CALLED AFTER the
rem parameters for repository updates are set. In lthis way one can e.g. achieve
rem overriding certain parameters by calling SetVar (with the corresponding
rem parameters) and so overriding the values set at the beginning of this script.

rem see also documentation comments in UpdateRepo.bat.


rem Reset the error level (by running an always successfull command):
ver > nul

echo.
echo Defining env. variables: SETTINGS for updating repository: IGLibCore
echo.

set ModuleDirRelative=..\..\modules\IGLibEventAggregator
set CheckoutBranch=master
set RepositoryAddress=https://github.com/ajgorhoe/IGLib.modules.IGLibEventAggregator.git
set RepositoryAddressSecondary=https://ajgorhoe@bitbucket.org/ajgorhoe/iglib.modules.iglibeventaggregator.git
set RepositoryAddressLocal=d:/backup_sync/bk_code/git/ig/misc/iglib_misc/EventAggregator/
set Remote=origin
set RemoteSecondary=originBitBucket
set RemoteLocal=local

set ModuleDir=%~dp0%ModuleDirRelative%

rem If command-line arguments were specified then interpret them as another command and run the command:
if "%~1" NEQ "" (
    echo.
    echo Settings script: Executing recursve commnd specified by arguments:
    echo   cmd /c "%~1" "%~2" "%~3" "%~4" "%~5" "%~6" "%~7" "%~8" "%~9"
    echo.
    cmd /c "%~1" "%~2" "%~3" "%~4" "%~5" "%~6" "%~7" "%~8" "%~9"
)

