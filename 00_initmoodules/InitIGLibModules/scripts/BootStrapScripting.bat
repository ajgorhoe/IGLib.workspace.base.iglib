
@echo off

:: Define initial script locations in the current directory:


set IntendedLocationIGLibScripts=%~dp0IGLibScripts
set IntendedLocationIGLibScriptsGitSubdir=%IntendedLocationIGLibScripts%\.git\refs


:: Set Initial location of scripts to current directory:
set IGLibScripts=%~dp0
:: Set innitial paths to the essential scripts, such that proper IGLibScripts directory can be restored:
set UpdateRepo=%IGLibScripts%\UpdateRepo.bat
set SetVar=%IGLibScripts%\SetVar.bat


if not exist "%IntendedLocationIGLibScriptsGitSubdir%" (
	echo.
	echo Bootstrapping: essential variables before pointing to a properr IGLIbScripts location:
	echo   IntendedLocationIGLibScripts: %IntendedLocationIGLibScripts%
	echo   UpdateRepo: %UpdateRepo%
	echo   SetVar: %SetVar%
	echo.
	echo.
	echo The following directory indicating IGLibScripts existence does not exist, 
	echo need to clone the repository:
	echo   "%IntendedLocationIGLibScriptsGitSubdir%"
	"%UpdateRepo%"  "%~dp0SettingsIGLibScripts.bat" "%SetVar%" ModuleDir "%IntendedLocationIGLibScripts%"
)

if exist "%IntendedLocationIGLibScriptsGitSubdir%" (
	rem IGLibScripts directory exists at intended location, update script 
	rem locations appropriately by executing the SetScriptReferences.bat 
	rem script in that directory.
	
	call "%IntendedLocationIGLibScripts%\SetScriptReferences.bat"

	rem echo.
	rem echo CONTROL OUTPUT:
	rem echo.
	
	rem Also print the variables by using a script from IGLibScripts; we can
	rem now use the variable containing printing script path:
	
	echo PrintScriptReferences: "%PrintScriptReferences%"
	
	call "%PrintScriptReferences%" 
) else (
	rem IGLibScripts directory does not exist, print the final script 
	rem paths that will remain at bootstrapping locations:
	echo.
	echo Bootstrapping: Final locations of relevant scripts - bootstrapping 
	echo setup, as the IGLibScripts directory is not cloned:
	echo   IGLibScripts: %IGLibScripts%
	echo   UpdateRepo: %UpdateRepo%
	echo   SetVar: %SetVar%
	echo.
)


