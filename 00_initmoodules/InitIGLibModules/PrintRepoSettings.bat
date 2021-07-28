
@echo off
setlocal

rem Prints values of environment variables that are used for updating repositories,
rem e.g. by the script UpdateREpo.bat .
rem Execution of the script does not have any siide effect such as setting on changing
rem variables in the calling environment (executiion block is enclosed in setloca /
rem endlocal calls).

rem BEFORE printing the values, this script treats eventual parameters as another
rem command with parameters and executes that command. This adds the possibility
rem to run the setting script before printing variable values. This is done in a single
rem command line, and combination of scripts does not have side effects.

rem Reset the error level (by running an always successfull command):
ver > nul
rem Base directories:
set ScriptDir=%~dp0
set InitialDir=%CD%

rem If command-line arguments were specified then interpret them as another command and run the command:
if "%~1" NEQ "" (
    echo.
    echo PrintRepoSettings: Executing recursve commnd specified by arguments:
    echo   call "%~1" "%~2" "%~3" "%~4" "%~5" "%~6" "%~7" "%~8" "%~9"
    echo.
    call "%~1" "%~2" "%~3" "%~4" "%~5" "%~6" "%~7" "%~8" "%~9"
)

echo.
echo Parameters for repository update:
echo.
echo ModuleDirRelative: "%ModuleDirRelative%"
echo ModuleDir:         "%ModuleDir%"
echo CheckoutBranch:    "%CheckoutBranch%"
echo Remote:            "%Remote%"
echo RemoteSecondary:   "%RemoteSecondary%"
echo RemoteLocal:       "%RemoteLocal%"
echo.
echo RepositoryAddress: "%RepositoryAddress%"
echo RepositoryAddressSecondary: "%RepositoryAddressSecondary%"
echo RepositoryAddressLocal: "%RepositoryAddressLocal%"
echo.

:finalize
ver > nul

endlocal

