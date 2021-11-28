
rem This scripts updates the bootstrapping scripts in the bootstrapping 
rem directory by copying the most recent versions of the scripts from the 
rem current repository. 
rem The working copy of the current repository must be directly contained
rem in the bootstrapping directory and the appropriate branch or commit
rem (normally, the master branch) must be checked out.
rem After running this script, do not forget to commit changes in the 
rem bootstrapping directory.

@echo off

setlocal

set ScriptDir=%~dp0

rem Reset the error level:
ver > nul
echo.
echo ======================================== %~n0%~x0:

echo.
echo IGLibScripts repo locaition:
echo   "%ScriptDir%\..\"
echo Copying files from this repository to the containing bootstrapping 
echo directory ...

rem Currently we don't copy .gitignore; this can be changed by uncommenting
rem the line below: 
rem copy "%ScriptDir%\.gitignore" "%ScriptDir%\..\.." 
copy "%ScriptDir%\0readme_bootstrappingscripts.txt" "%ScriptDir%\..\.." 

copy "%ScriptDir%\BootStrapScripting.bat" "%ScriptDir%\..\.." 
copy "%ScriptDir%\BootStrapUpdate.bat" "%ScriptDir%\..\.." 
copy "%ScriptDir%\SettingsIGLibScriptsBootstrap.bat" "%ScriptDir%\..\.." 

copy "%ScriptDir%\..\PrintRepoSettings.bat" "%ScriptDir%\..\.." 
copy "%ScriptDir%\..\SetVar.bat" "%ScriptDir%\..\.." 
copy "%ScriptDir%\..\UpdateRepo.bat" "%ScriptDir%\..\.." 

echo   ... copying done.
echo.
echo ======== End: %~n0%~x0
echo.

endlocal

