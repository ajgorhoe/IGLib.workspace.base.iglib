
:: Define initial script locations in the current directory:

set IntendedLocationIGLibScripts=%~dp0IGLibScripts
set IntendedLocationIGLibScriptsGitSubdir=%IntendedLocationIGLibScripts%\.git\refs
 
:: Set Initial location of scripts to current directory:
set IGLibScripts=%~dp0
:: Set innitial paths to the essential scripts, such that proper IGLibScripts directory can be restored:
set UpdateRepo=%IGLibScripts%\UpdateRepo.bat
set SetVar=%IGLibScripts%\SetVar.bat

echo.
echo Bootstrapping: essential variables before pointing to a properr IGLIbScripts location:
echo   IntendedLocationIGLibScripts: %IntendedLocationIGLibScripts%
echo   UpdateRepo: %UpdateRepo%
echo   SetVar: %SetVar%
echo.

if not exist "%IntendedLocationIGLibScriptsGitSubdir%" (
	echo.
	echo The following directory indikator of IGLibScripts existence does not exist, 
	echo need to clone the repository:
	echo   "%IntendedLocationIGLibScriptsGitSubdir%"
	"%UpdateRepo%"  "%~dp0SettingsIGLibScripts.bat" "%SetVar%" ModuleDir "%IntendedLocationIGLibScripts%"
)

if exist "%IntendedLocationIGLibScriptsGitSubdir%" (
	:: IGLibScripts directory exists at intended location, set script pointers appropriately:
	set IGLibScripts=%IntendedLocationIGLibScripts%
	set UpdateRepo=%IGLibScripts%\UpdateRepo.bat
	set SetVar=%IGLibScripts%\SetVar.bat
)

echo.
echo Bootstrapping: Final locations of relevant scripts:
echo   UpdateRepo: %UpdateRepo%
echo   SetVar: %SetVar%
echo.




