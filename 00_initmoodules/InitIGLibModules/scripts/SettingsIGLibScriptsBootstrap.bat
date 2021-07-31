
@echo off

:: Sets parameters for cloning or updating the IGLibScripts repository
:: WITHIN the current directory as part of BOOTSTRAPPING.


:: Reset the error level (by running an always successfull command):
ver > nul

echo.
echo Defining env. variables: SETTINGS for BOOTSTRAPPING repository: IGLibCore
echo.


:: Just make sure that variable IntendedLocationIGLibScripts is defined,
:: otherwise call BootStrapScripting.bat but also prevent infinite recursion:
if not defined IntendedLocationIGLibScripts (
  rem Call BootStrapScripting.bat to define the variable 
  rem IntendedLocationIGLibScripts containing location of IGLibScripts:
  call "%~dp0\BootStrapScripting.bat"
  if not defined IntendedLocationIGLibScripts (
    rem something went wrong, just exit:
    echo.
    echo FATAL ERROR: bootstrapping - location of IGLibScripts not defined.
	echo.
	set IntendedLocationIGLibScripts=%~dp0\WRONG_REPO_LOCATION_IGLibScripts
	exit -1
  )
)

:: Take general settings for IGLibCore:
call "%~dp0\SettingsIGLibScripts.bat"

:: Modify parameters that are different for the bootstrapping copy of repo:
set ModuleDir=%IntendedLocationIGLibScripts%
set CheckoutBranch=master
rem set CheckoutBranch=release/latestrelease
:: Undefine local repository address:
set RepositoryAddressLocal=


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

