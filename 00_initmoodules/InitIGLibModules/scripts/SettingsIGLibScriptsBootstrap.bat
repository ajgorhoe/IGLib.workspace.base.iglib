
@echo off

:: Sets parameters for cloning or updating the IGLibScripts repository
:: WITHIN the current directory as part of BOOTSTRAPPING.


:: Reset the error level (by running an always successfull command):
ver > nul

echo.
echo SETTINGS for BOOTSTRAPPING repository: IGLibScripts
echo.


:: Take general settings for IGLibScripts:
call "%~dp0\SettingsIGLibScripts.bat"

:: Modify parameters that are different for the bootstrapping repo:

set ModuleDirRelative=IGLibScripts
set ModuleDir=%~dp0\%ModuleDirRelative%
set CheckoutBranch=release/IGLib1.9.9_21_11_18_MainApplicationsWork
:: Alternative  CheckoutBranch: release/latestrelease
set RepositoryAddressLocal=

:: Derived variable:
set ModuleGitSubdir=%ModuleDir%\.git\refs

