
@echo off

:: Sets parameters for cloning or updating the IGLibScripts repository
:: WITHIN the current directory as part of BOOTSTRAPPING.


:: Reset the error level (by running an always successfull command):
ver > nul

echo.
echo Defining env. variables: SETTINGS for BOOTSTRAPPING repository: IGLibCore
echo.


REM :: Just make sure that variable IntendedLocationIGLibScripts is defined,
REM :: otherwise call BootStrapScripting.bat but also prevent infinite recursion:
REM if not defined IntendedLocationIGLibScripts (
  REM rem Call BootStrapScripting.bat to define the variable 
  REM rem IntendedLocationIGLibScripts containing location of IGLibScripts:
  REM call "%~dp0\BootStrapScripting.bat"
  REM if not defined IntendedLocationIGLibScripts (
    REM rem something went wrong, just exit:
    REM echo.
    REM echo FATAL ERROR: bootstrapping - location of IGLibScripts not defined.
	REM echo.
	REM set IntendedLocationIGLibScripts=%~dp0\WRONG_REPO_LOCATION_IGLibScripts
	REM exit -1
  REM )
REM )

:: Take general settings for IGLibScripts:
call "%~dp0\SettingsIGLibScripts.bat"

:: Modify parameters that are different for the bootstrapping repo:

set ModuleDirRelative=IGLibScripts
set ModuleDir=%~dp0\%ModuleDirRelative%
set CheckoutBranch=master
rem set CheckoutBranch=release/latestrelease
set RepositoryAddressLocal=

:: Derived variable:
set ModuleGitSubdir=%ModuleDir%\.git\refs


if "%~1" EQU "" goto AfterCommandCall
	:: If any command-line arguments were specified then assemble a 
	:: command-line from these arguments and execute it:

	:: Assemble command-line from the remaining arguments....
	set CommandLine6945="%~1"
	:loop
	shift
	if [%1]==[] goto afterloop
	set CommandLine6945=%CommandLine6945% "%~1"
	goto loop
	:afterloop

	:: Call the assembled command-line:
	call %CommandLine6945%
:AfterCommandCall

