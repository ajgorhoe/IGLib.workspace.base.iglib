
@echo off

rem Checks out or updates a repository to/at the specified location.
rem Usually used for reposiories that are embedded as full independent 
rem repositories rather than via submodules or some other Git mechanism.

rem Callinng this script has NO SIDE EFFECTS (the body is enclosed in setlocal
rem / endlocal block).

rem PARAMETERS of update are obtained via environment variables:
rem   ModuleDir: root directory of cloned Repository
rem   CheckoutBranch: branch that is checked out
rem   RepositoryAddress: attress of the main remote repository
rem   Remote: name of the remote that points to the above
rem   RepositoryAddressSecondary: OPTIONAL, address of Secondary
rem     remote Repository
rem   RemoteSecondary: name of the remote that points to the above
rem     (optional even if the above is defined)
rem   RepositoryAddressLocal: OPTIONAL address of another alternative
rem     remote repository, usually a local one
rem   RemoteLocal: name of the remote that points to the above
rem     (optional even if the above is defined)

rem One way to call the script is to set all environment variables
rem that define parameters of the update (see above), then call this
rem script to perform the update. AA better wat is to create a settings
rem script that sets all the required parameters, and call this script
rem via embedded calll mechanism simply by stating script path as parameter
rem when calling the script (the advantage is that scripts have no sde
rem effects (variables set or changed) that would propagate to the calling
rem environment). See below for explanation.

rem If any PARAMETERS are specified when calling the script, these are 
rem interpreted as EMBEDDED COMMAND with parameters, which is CALLED
rem BEFORE the body of the current script is executed, still WITHIN setlocal
rem / endlocal block.
rem This mechanism makes possible to define as parameter a script that sets
rem all the parameters as environment variables (the settings script), which
rem is called before the body of this script is executed. The advantage of
rem this approach is that settings for different repositories are packed into
rem the corresponding settings scripts and updating different repositories
rem is performed by calling the current script wih the appropriate settings
rem script as parameter. Annother advantage is that the calling environment
rem is not polluted with environment variables defining the parameters,
rem because this script takes care (by defining setlocal/endlocal block) that
rem variables set in the embedded call do not propagate to the callerr's 
rem environment.

rem As EXAMPLE, suppose we define the scrippt SettingsRepoIGLibCore, which
rem contains repository settings for the IGLibCore repository. Updating the
rem corresponding repositort is done simply by calling:

rem   UpdateRepo.bat SettingsRepoIGLibCore.bat

rem It is advisable that the settings script is also created in such a way
rem that it can take an embedded script as parameter, and this script is 
rem run AFTER the environment variables (parameters for this script) are
rem set. In this way, some parameters can be simply overriden by recursively
rem nested commands, e.g.:
rem   UpdateRepo.bat SettingsRepoIGLibCore.bat SetVar CheckoutBranch FeatureBranch1
rem This would cause the same as command with a single parameter, except that
rem the branch to be checked out would be different, i.e. FeatureBranch1
rem instead of the branch specified in SettingsRepoIGLibCore.bat (e.g. the
rem default master branch).


setlocal

rem Reset the error level (by running an always successfull command):
ver > nul
rem Base directories:
set ScriptDir=%~dp0
set InitialDir=%CD%

rem Skip the settings section:
rem Uncomment goto later! Also when testing is completed, this whole block will be removed.
goto afterSettings
rem Parameters for the update:
set ModuleDirRelative=..\..\modules\IGLibCore
set CheckoutBranch=master
set RepositoryAddress=https://github.com/ajgorhoe/IGLib.modules.IGLibCore.git
set RepositoryAddressSecondary=https://ajgorhoe@bitbucket.org/ajgorhoe/iglib.modules.iglibcore.git
set RepositoryAddressLocal=d:/backup_sync/bk_code/git/ig/misc/iglib_modules/IGLibCore/
set Remote=origin
set RemoteSecondary=originBitBucket
set RemoteLocal=local
set ModuleDir=%~dp0%ModuleDirRelative%
:afterSettings

rem If command-line arguments were specified then take them as another command and run the command:
if "%~1" NEQ "" (
    rem See: https://stackoverflow.com/questions/14731877/to-call-or-not-to-call-a-batch-file
    echo.
    echo Executing:
    echo   echo   call %~1 %~2 %~3 %~4 %~5 %~6 %~7 %~8 %~9
    call  "%~1" "%~2" "%~3" "%~4" "%~5" "%~6" "%~7" "%~8" "%~9" "%~10"
    echo.
)

call  %~dp0PrintRepoSettings.bat


Rem Basic checks if something is forgotten
if not defined Remote (set Remote=origin)
if "%Remote%" EQU "" (set Remote=origin)
if not defined RemoteSecondary (set RemoteSecondary=originsecondary)
if "%RemoteSecondary%" EQU "" (set RemoteSecondary=originsecondary)
if not defined RemoteLocal (set RemoteLocal=local)
if "%RemoteLocal%" EQU "" (set RemoteLocal=local)

rem Derived parameters:
set ModuleContainingDir=%ScriptDir%
set ModuleDir=%ModuleContainingDir%%ModuleDirRelative%
set ModuleGitSubdir=%ModuleDir%\.git\refs
echo Subdirectory identifying module correctness:
echo   "%ModuleGitSubdir%"
echo.

rem Defaults for eventually missing information:
set IsDefinedCheckoutBranch=0
if defined CheckoutBranch (
  if "%CheckoutBranch%" NEQ "" (
    set IsDefinedCheckoutBranch=1
  )
)
if %IsDefinedCheckoutBranch% EQU 0 (
  echo.
  echo CheckoutBranch set to default - master
  set CheckoutBranch=master
)
set IsDefinedRemote=0
if defined Remote (
  if "%Remote%" NEQ "" (
    set IsDefinedRemote=1
  )
)
if %IsDefinedRemote% EQU 0 (
  echo.
  echo Remote set to default - origin
  set Remote=origin
)

set IsDefinedRepositoryAddressSecondary=0
if defined RepositoryAddressSecondary (
  if "%RepositoryAddressSecondary%" NEQ "" (
    set IsDefinedRepositoryAddressSecondary=1
  )
)
if %IsDefinedRepositoryAddressSecondary% NEQ 0 (
    echo.
    echo Secondary repository address: %RepositoryAddressSecondary%
    echo.
) else (
    echo.
    echo Secondary repository address is not defined.
)
set IsDefinedRepositoryAddressLocal=0
if defined RepositoryAddressLocal (
  if "%RepositoryAddressLocal%" NEQ "" (
    set IsDefinedRepositoryAddressLocal=1
  )
)
if %IsDefinedRepositoryAddressLocal% NEQ 0 (
    echo.
    echo Local repository address: %RepositoryAddressLocal%
    echo.
) else (
    echo.
    echo Local repository address is not defined.
)

set IsDefinedRepositoryAddressSecondary=0
if defined RepositoryAddressSecondary (
  if "%RepositoryAddressSecondary%" NEQ "" (
    set IsDefinedRepositoryAddressSecondary=1
  )
)
if %IsDefinedRepositoryAddressSecondary% NEQ 0 (
    echo.
    echo Secondary repository address: %RepositoryAddressSecondary%
    echo.
) else (
    echo.
    echo Secondary repository address is not defined.
)

set IsClonedAlready=0
if exist "%ModuleGitSubdir%" (
    set IsClonedAlready=1
)

echo.
echo.
echo Updating / cloning of (embedded) repository:
echo   %RepositoryAddress%
echo   to directory: %ModuleDirRelative%
echo   "%ModuleDir%"
echo   branch: %CheckoutBranch%
echo   remote: %Remote%
echo.

rem Clone the repo if one does not exist (remove its directory before):
if not exist "%ModuleGitSubdir%" (
  if exist "%ModuleDir%" (
    rem Remove eventually existing directory beforehand:
    echo.
    echo Removing the current directory - invalid repo...
    echo Executing:
    echo   rd /s /q %"ModuleDir"%
    rd /s /q %"ModuleDir"%
    echo.
  )
  echo.
  echo Cloning Git repository...
  echo Calling: 
  echo   git clone "%RepositoryAddress%" "%ModuleDir%"
  git clone "%RepositoryAddress%" "%ModuleDir%"
  echo   ... done.
  echo.
  ver > nul
) else (
    echo.
    echo Cloning skipped, repository already cloned.
    echo.
)

if not exist "%ModuleGitSubdir%" (
  echo.
  echo ERROR Could not clone the repository.
  echo.
  goto finalize
) else (
    echo.
    echo Repository exists.
    echo.
)

cd "%ModuleDir%"

echo.
if %IsClonedAlready% EQU 0 (
    rem Set the first remote if named differently than origin:
    if "%Remote%" EQU "origin" (
        echo Remote %Remote% not set, since remote "origin" is already set by default.
    ) else (
        echo setting remote %Remote% ...
        rem git remote remove %Remote% 
        rem ver > nul
        git remote add %Remote% "%RepositoryAddress%"
        ver > nul
    )
    rem Set the two alternative remotes when specified:
    if %IsDefinedRepositoryAddressSecondary% EQU 0 (
        echo Remote %RemoteSecondary% is not specified, not set.
    ) else (
        echo setting remote %RemoteSecondary% to   %RepositoryAddressSecondary% ...
        git remote add %RemoteSecondary% "%RepositoryAddressSecondary%"
        echo.
    )
    if %IsDefinedRepositoryAddressLocal% EQU 0 (
        echo Remote %RemoteLocal% is not specified, not set.
    ) else (
        echo setting remote %RemoteLocal% to   %RepositoryAddressLocal% ...
        git remote add %RemoteLocal% "%RepositoryAddressLocal%"
        echo.
    )
) else (
    echo Remotes are not set because directtory has already been cloned before.
    echo If remotes are not set correcly in the cloned repository, remove it first and run the script again.
    echo In this case, make sure that any changes to reposiory have been committed and pushed.
)
echo.

REM echo.
REM echo Fetching from %Remote%...
REM git fetch %Remote%

echo.
echo Try to check out remote branch...
rem Checkout the remote branch (in case not yet checked out):
git checkout -b "%CheckoutBranch%" "remotes/%Remote%/%CheckoutBranch%" --
ver > nul


echo.
rem Switch to checkout branch  (in case not yet checked out):
echo Switching to branch "%CheckoutBranch%"...
git switch "%CheckoutBranch%"

echo.
echo Pulling changes from %Remote%...
git pull %Remote%


:finalize

echo.
echo Update script completed for %ModuleDirRelative%/.
echo.
echo.
echo.

cd %InitialDir%
ver > nul

endlocal

