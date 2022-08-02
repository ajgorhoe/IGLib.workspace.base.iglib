
@echo off

:: Cleans the repository at the specified location.
:: Usually used for reposiories that are embedded as full independent 
:: repositories rather than via submodules or some other Git mechanism.

:: Callinng this script has NO SIDE EFFECTS (the body is enclosed in
:: setlocal / endlocal block).

:: PARAMETERS of the script are obtained via environment variables:
::   ModuleDir: root directory of cloned Repository

:: One way to call the script is to set all environment variables
:: that define parameters of the update (see above), then call this
:: script to perform the update. A better way is to create a settings
:: script that sets all the required parameters, and call this script
:: via embedded calll mechanism simply by stating script path as parameter
:: when calling the script (the advantage is that scripts have no sde
:: effects (variables set or changed) that would propagate to the calling
:: environment). See below for explanation.

:: If any PARAMETERS are specified when calling the script, these are 
:: interpreted as EMBEDDED COMMAND with parameters, which is CALLED
:: BEFORE the body of the current script is executed, still WITHIN setlocal
:: / endlocal block.
:: This mechanism makes possible to define as parameter a script that sets
:: all the parameters as environment variables (the settings script), which
:: is called before the body of this script is executed. The advantage of
:: this approach is that settings for different repositories are packed into
:: the corresponding settings scripts and handling different repositories
:: is performed by calling the current script wih the appropriate settings
:: script as parameter. Annother advantage is that the calling environment
:: is not polluted with environment variables defining the parameters,
:: because this script takes care (by defining setlocal/endlocal block) that
:: variables set in the embedded call do not propagate to the callerr's 
:: environment.

:: As EXAMPLE, suppose we define the scrippt SettingsRepoIGLibCore, which
:: contains repository settings for the IGLibCore repository. Removing the
:: corresponding repository is done simply by calling:

::   CleanRepo.bat SettingsRepoIGLibCore.bat

:: It is advisable that the settings script is also created in such a way
:: that it can take an embedded script as parameter, and this script is 
:: run AFTER the environment variables (parameters for this script) are
:: set. In this way, some parameters can be simply overriden by recursively
:: nested commands, e.g.:
::   CleanRepo.bat SettingsRepoIGLibCore.bat SetVar ModuleDir .\MyDir
:: This would cause the same as command with a single parameter, except that
:: the module's cloned directory would be different, i.e. instead of the 
:: branch specified in SettingsRepoIGLibCore.bat.


setlocal

:: Reset the error level (by running an always successfull command):
ver > nul
:: Base directories:
set ScriptDirCleanRepo=%~dp0
set InitialDirCleanRepo=%CD%

rem Before execution, execute command composed of script parameters, when
rem any:
if "%~1" EQU "" goto AfterCommandCall
	:: If any command-line arguments were specified then assemble a 
	:: command-line from these arguments and execute it:

	:: Assemble command-line from the remaining arguments....
	set CommandLine90674="%~1"
	:loop
	shift
	if [%1]==[] goto afterloop
	set CommandLine90674=%CommandLine90674% "%~1"
	goto loop
	:afterloop

	:: Call the assembled command-line:
	echo.
	echo Calling command composed of arguments:
	echo   "%CommandLine90674%"
	echo.
	call %CommandLine90674%
:AfterCommandCall


echo.
echo ========
echo Cleaning (embedded) module repository located at:
echo   "%ModuleDir%"
echo.

:: Check that directory exists and it is a true Git repository:
:: Derived parameters:
set ModuleGitSubdir=%ModuleDir%\.git\refs
if not exist "%ModuleGitSubdir%" (
  echo.
  echo ====
  echo Module's Git subdirectory does NOT exist:
  echo   "%ModuleGitSubdir%"
  echo Cleaning of module directory will NOT be performed - not a Git repo.
  echo. 
  goto finalize
) else (
    rem 
)

echo.
echo ====
echo CLEANING Module directory (removing untracked files and dirs):
echo   "%ModuleDir%"
echo Executing:
echo   git clean -f -x -d

cd  "%ModuleDir%"
git clean -f -x -d



:finalize

cd %InitialDirCleanRepo%
ver > nul

endlocal

echo   ... done, "CleanRepo" script completed.

