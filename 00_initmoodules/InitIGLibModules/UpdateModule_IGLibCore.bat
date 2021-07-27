
@echo off
rem This script is for simple update of an embedded repository.

setlocal

rem Reset the error level (by running an always successfull command):
ver > nul
rem Base directories:
set ScriptDir=%~dp0
set InitialDir=%CD%

rem Parameters for the update:
set ModuleDirRelative=..\..\modules\IGLibCore
set CheckoutBranch=master
set RepositoryAddress=https://github.com/ajgorhoe/IGLib.modules.IGLibCore.git
set RepositoryAddressSecondary=https://ajgorhoe@bitbucket.org/ajgorhoe/iglib.modules.iglibcore.git
set RepositoryAddressLocal=d:/backup_sync/bk_code/git/ig/misc/iglib_modules/IGLibCore/
set Remote=origin
set RemoteSecondary=originBitBucket
set RemoteLocal=local


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
set ModuleGitSubdir=%ModuleDir%\.git\refs\remotes

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
echo Update / clone of embedded repository:
echo   %RepositoryAddress%
echo   to directory: %ModuleDirRelative%
echo   "%ModuleDir%"
echo   branch: %CheckoutBranch%
echo   remote: %Remote%
echo.

rem Clone the reepo if one does not exist (remove its directory before):
if not exist "%ModuleGitSubdir%" (
  if exist "%ModuleDir%" (
    rem Remove eventually existing directory beforehand:
    echo.
    echo Removing the current directory - invalid repo...
    rd /s /q %"ModuleDir"%
    echo.
  )
  echo.
  echo Cloning Git repository...
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
rem Checkout the remote branch (case when not yet checked out):
git checkout -b "%CheckoutBranch%" "remotes/%Remote%/%CheckoutBranch%" --



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

