
@echo off

:: Prepares environment for execution of repository updating scripts
:: and other scripts from the IGLibScripts repository.
::
:: Parameters:
::   %1: Optional. 0 - update of IGLibScripts repository is NOT forced
::       (not performed if the directory exists and is a valid Git repo)
::       1 or other nonzero number: update is forced.  

echo.
echo BootStrapScripting.bat:
echo   bootstrapping essential scripts...
echo.



rem Set script paths from fallback directory:
call "%~dp0\fallback\SetScriptReferences.bat"

rem Reset the error level (by running an always successfull command):
ver > nul

setlocal
  
  set ModuleDirRelative=IGLibScripts
  set CheckoutBranch=master
  set RepositoryAddress=https://github.com/ajgorhoe/IGLib.modules.IGLibScripts.git
  set RepositoryAddressSecondary=https://ajgorhoe@bitbucket.org/ajgorhoe/iglib.modules.iglibscripts.git
  set RepositoryAddressLocal=d:\backup_sync\bk_code\git\ig\misc\iglib_modules\IGLibScripts
  set Remote=origin
  set RemoteSecondary=originSecondary
  set RemoteLocal=local
  rem
  set ModuleDir=%~dp0\%ModuleDirRelative%
  set ModuleGitSubdir=%ModuleDir%\.git\refs
  
  echo #### After IGLib settings under setlocal 
  
  
  set ForceUpdate=0
  if "%1" NEQ "" (
    if "%1" NEQ "0" (
	  set ForceUpdate=1
	)
  )
  if not exist "%ModuleGitSubdir%" (
    rem There is no IGLibScripts module repo at expected location, 
	rem try to clone the repo:
    git clone "%RepositoryAddress%" "%ModuleDir%"
	
	echo #### After FIRST direct clone
	
	if not exist "%ModuleGitSubdir%" (
	  rem We still don't have the proper IGLibScript repo, try alternative
	  rem location:
	  git clone "%RepositoryAddressSecondary%" "%ModuleDir%"
	  
	  echo #### After SECOND direct clone
	  
	)
	if exist "%ModuleDir%" (
	  call "%ModuleDir%\SetScriptReferences.bat"
	)
	rem After pure clone, try to call the update script (which will now
	rem skip cloning) - to switch to the right branch.
	call "%UpdateRepo%"
	
	echo #### After UpdateRepo - end of Repo not exists
	
  ) else (
    rem We already have IGLibScripts module's repo at expected location.
	rem If specified, we update the repo according to parameters:
    if "%ForceUpdate%" NEQ "0" (
	  
	  echo #### ForceUpdate: beginning
	  
	  call "%ModuleDir%\SetScriptReferences.bat"
	  
	  echo #### ForceUpdate, between SetScriptReferences and UpdateRepo
	  
	  call "%UpdateRepo%"
	  
	  echo #### ForceUpdate: end
	  
	)
  )
  
  if "%PrintDebugInfo%" EQU "1" (
    call %PrintRepoSettings%
  )

endlocal

echo #### After endlocal


rem If IGLibScripts directory exists, make sure that script paths are 
rem updated to point to that directory: 
if exist "%~dp0\IGLibScripts" (
  call "%~dp0\IGLibScripts\SetScriptReferences.bat"
  
  echo #### After final SetScriptReferences
  
)

if "%PrintDebugInfo%" EQU "1" (
  call %PrintScriptReferences%
)


:finalize

echo.
echo   ... bootstrappping essential scripts completed.
echo.
