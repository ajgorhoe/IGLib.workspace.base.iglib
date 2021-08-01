
@echo off

:: Prepares environment for execution of repository updating scripts.

echo.
echo BootStrapScripting.bat:
echo   bootstrapping essential scripts...
echo.
echo Obtaining bootstrapping script repository settins...

:: Define initially script locations for bootstrapping:
set BootstrapSettings=%~dp0\SettingsIGLibScriptsBootstrap.bat
set UpdateRepo=%~dp0\UpdateRepo.bat
set SetVar=%~dp0\SetVar.bat
set PrintRepoSettings=%~dp0\PrintRepoSettings.bat

if 1 NEQ 0 (
  echo.
  echo Initial script paths:
  echo BootstrapSettings: "%BootstrapSettings%"
  echo UpdateRepo: "%UpdateRepo%"
  echo SetVar: "%SetVar%"
  echo PrintRepoSettings: "%PrintRepoSettings%"
)

:: Obtain for IGLibScripting repository for bootstrapping:
:: call "%~dp0\SettingsIGLibScriptsBootstrap.bat"
call "%BootstrapSettings%"

:: Derived variable - already defined in settings::
:: set ModuleGitSubdir=%ModuleDir%\.git\refs

:: Print settings for bootstrapping IGLib repo:
call "%PrintRepoSettings%"

set BootstrapRepoUpdated=0
if exist "%ModuleGitSubdir%" goto :SkipUpdate
  :: The IGLibScripts for bootstrapping not yet properly cloned, 
  :: perform repo update (cloning):
  call "%UpdateRepo%" "%BootstrapSettings%"
  set BootstrapRepoUpdated=1
:SkipUpdate


if not exist "%ModuleGitSubdir%" goto ErrorScriptRepo
  rem Update script locations to point intto IGLibScripts
  rem by calling SetScriptReferences.bat in IGLibScripts:
  call "%ModuleDir%\SetScriptReferences.bat"
  call "%PrintScriptReferences%"
  goto AfterScriptReferencesUpdate
:ErrorScriptRepo
  echo.
  echo ERROR in BOOTSTRAPPING scripts:
  echo   IGLibScripts module could not be cloned.
  echo   Fallback to bootstrapping scripts; may not work properly.
  echo.

:AfterScriptReferencesUpdate


:: Skip the remaining block - shall be removed later.
goto finalize












:: Define location of IGLibScripts repo in the current directory:

set IntendedLocationIGLibScripts=%~dp0IGLibScripts
set IntendedLocationIGLibScriptsGitSubdir=%IntendedLocationIGLibScripts%\.git\refs


:: Set Initial location of scripts to current directory:
set IGLibScripts=%~dp0
:: Set innitial paths to the essential scripts, such that proper IGLibScripts directory can be restored:
set UpdateRepo=%IGLibScripts%\UpdateRepo.bat
set SetVar=%IGLibScripts%\SetVar.bat


if not exist "%IntendedLocationIGLibScriptsGitSubdir%" (
	rem The intended IGLibScripts location does not contain a valid
	rem repository, we need to clone it:
	echo.
	echo Bootstrapping: essential variables before pointing to a proper 
	echo IGLIbScripts location:
	echo   IntendedLocationIGLibScripts: %IntendedLocationIGLibScripts%
	echo   UpdateRepo: %UpdateRepo%
	echo   SetVar: %SetVar%
	echo.
	echo.
	echo The following directory indicating IGLibScripts existence does not exist, 
	echo need to clone the IGLibScripts repository into:
	echo   "%IntendedLocationIGLibScriptsGitSubdir%"
	call "%UpdateRepo%" "%~dp0\SettingsIGLibScriptsBootstrap.bat"
)

if exist "%IntendedLocationIGLibScriptsGitSubdir%" (
	rem IGLibScripts directory exists at intended location, update script 
	rem locations appropriately by executing the SetScriptReferences.bat 
	rem script in that directory.
	
	
	echo.
	echo Setting script references, calling:
	echo   "%IntendedLocationIGLibScripts%\SetScriptReferences.bat"
	call "%IntendedLocationIGLibScripts%\SetScriptReferences.bat"
	rem Also print the variables by using a script from IGLibScripts; we can
	rem now use the variable containing printing script path:
	
	echo.
	echo After calling SetScriptReferences.bat:
	echo PrintScriptReferences: %PrintScriptReferences%

	echo. 
	echo Printing script references, executing:
	echo   call "%PrintScriptReferences%" 
	echo.
	
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

:finalize

echo.
echo   ... bootstrappping essential scripts completed.
echo.
