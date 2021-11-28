
@echo off

:: Prepares environment for execution of repository updating scripts.

echo.
echo BootStrapScripting.bat:
echo   bootstrapping essential scripts...
echo.
echo Obtaining bootstrapping script repository settins...

rem Check conditions for needing to bootstrap / skip bootstrapping...
rem Bootstrapping is performed conservatively: if bootstrappng script
rem locations are already defined (via environment pre-agreed variables) 
rem and scripts actually exist at these locations then bootstrapping
rem is skipped. This saves time when bootstrappiing is called in a script 
rem that was called by another script that already performed bootstrapping.
rem Call BootstrapUpdate to override this and clone/update the IGLibScripts
rem repository unconditionally.
if not defined IGLibScripts goto ContinueBootStrapScripting
if not exist "%IGLibScripts%" goto ContinueBootStrapScripting
if not defined SetScriptReferences goto ContinueBootStrapScripting
if not exist "%SetScriptReferences%" goto ContinueBootStrapScripting
rem no need to continue:
echo.
echo Scripting is already bootsttrapped, skipping the remainder 
echo   off BootStrapScripting.
echo.
goto AfterScriptReferencesUpdate
:ContinueBootStrapScripting

:: Define initially script locations for bootstrapping:
set BootstrapSettings=%~dp0\SettingsIGLibScriptsBootstrap.bat
set UpdateRepo=%~dp0\UpdateRepo.bat
set SetVar=%~dp0\SetVar.bat
set PrintRepoSettings=%~dp0\PrintRepoSettings.bat

REM if 1 NEQ 0 (
  REM echo.
  REM echo Initial script paths:
  REM echo BootstrapSettings: "%BootstrapSettings%"
  REM echo UpdateRepo: "%UpdateRepo%"
  REM echo SetVar: "%SetVar%"
  REM echo PrintRepoSettings: "%PrintRepoSettings%"
REM )

:: Obtain for IGLibScripting repository for bootstrapping:
:: call "%~dp0\SettingsIGLibScriptsBootstrap.bat"
call "%BootstrapSettings%"

:: Derived variable - already defined in settings::
set ModuleGitSubdir=%ModuleDir%\.git\refs

REM :: Print settings for bootstrapping IGLib repo:
REM call "%PrintRepoSettings%"

set BootstrapRepoUpdated=0
if exist "%ModuleGitSubdir%" goto :SkipUpdate
  :: The IGLibScripts for bootstrapping not yet properly cloned, 
  :: perform repo update (cloning):
  call "%UpdateRepo%" "%BootstrapSettings%"
  set BootstrapRepoUpdated=1
:SkipUpdate


if not exist "%ModuleGitSubdir%" goto Fallback
  rem Update script locations to point intto IGLibScripts
  rem by calling SetScriptReferences.bat in IGLibScripts:
  call "%ModuleDir%\SetScriptReferences.bat"
  rem call "%PrintScriptReferences%"
  goto AfterScriptReferencesUpdate

:Fallback
if not exist "%~dp0\fallback\FallbackBootStrapScripting.bat" goto ErrorScriptRepo

rem Call the falback bootstrapping script:
call "%~dp0\fallback\FallbackBootStrapScripting.bat"
goto AfterScriptReferencesUpdate

:ErrorScriptRepo
  echo.
  echo ERROR in BOOTSTRAPPING scripts:
  echo   IGLibScripts module could not be cloned.
  echo   Fallback to bootstrapping scripts; may not work properly.
  echo.

:AfterScriptReferencesUpdate



:finalize

echo.
echo   ... bootstrappping essential scripts completed.
echo.
