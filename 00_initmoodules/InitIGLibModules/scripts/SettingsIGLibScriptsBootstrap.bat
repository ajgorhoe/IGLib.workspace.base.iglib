
@echo off

:: Sets parameters for cloning or updating the IGLibScripts repository
:: WITHIN the current directory as part of BOOTSTRAPPING.


:: Reset the error level (by running an always successfull command):
ver > nul

echo.
echo Defining env. variables: SETTINGS for BOOTSTRAPPING repository: IGLibCore
echo.


:: Take general settings for IGLibScripts:
call "%~dp0\SettingsIGLibScripts.bat"

:: Modify parameters that are different for the bootstrapping repo:

set ModuleDirRelative=IGLibScripts
set ModuleDir=%~dp0\%ModuleDirRelative%
set CheckoutBranch=master
:: Alternative  CheckoutBranch: release/latestrelease
set RepositoryAddressLocal=

:: Derived variable:
set ModuleGitSubdir=%ModuleDir%\.git\refs

