
:: This scripts updates the bootstrapping scripts in the current 
:: directory by copying the most recent versions of the scripts from the 
:: IGLibScripts repository. 
::
:: The IGLibScripts repository is first cloned and updated by calling
:: BootstrapScripting.bat. Then, the files to be updated are copied from
:: the repository clone.
::
:: WARNING: Do not forget to commit changes after running this script.
:: 
:: Remark: You can perform this operation MANUALLY. Just clone the 
:: IGLibScripts repository to any suitable location (or update an existing
:: clone), then copy contents of directory to fallback/ subdirectory, and
:: direct content of the 0bootstrappingscripts/ subdirectory to the current
:: directory. If there are any issues with the current script, use this
:: approach as workaround.


@echo off

setlocal

set ScriptDir=%~dp0

echo.
echo Updating the current bootstrapping directory with the updated scripts 
echo and other files from the IGLibScripts repository...
echo.

rem Make sure that IGLibScripts is cloned here and updated (argument 1
rem forces updating, even if IGLibScripts has already been cloned):
call "%ScriptDir%\BootstrapScripting.bat" 1

rem If the fallback/ subdirectory does not exist, create it:
mkdir "%ScriptDir%\fallback" 

rem Copy scripts from updated IGLibScripts to fallback/ directory:
copy "%IGlibScripts%\*.bat" "%ScriptDir%\fallback" /y

rem Copy scripts, text files, and .gitignore
copy "%IGlibScripts%\0bootstrappingscripts\*.bat" "%ScriptDir%" /y
copy "%IGlibScripts%\0bootstrappingscripts\*.txt" "%ScriptDir%" /y
copy "%IGlibScripts%\0bootstrappingscripts\.gitignore" "%ScriptDir%" /y

echo   ... copying done.
echo.
echo ======== End: %~n0%~x0
echo.

endlocal

