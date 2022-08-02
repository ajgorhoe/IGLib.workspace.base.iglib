

rem This script restores or updates IGLib dependencies from their remote
rem repositories.
rem The script delegates the task to the script contained in the 
rem InitIGLibModules project, which is also used to update the dependencies
rem via build.

echo.
echo Restoring / updating internal and external dependencies of IGLib...
echo Executing:
echo   call "%~dp0\00_initmodules\UpdateIGLibModuleRepos.bat"
call "%~dp0\00_initmodules\UpdateIGLibModuleRepos.bat"
echo.
echo  ... IGLib dependencies were updated.
echo.


