
@echo off
rem This script sets the variable whose name is derived from its first argument to the
rem value specified by the second argument of the script.
rem AFTER setting the variable, if there are more arguments, the script treats the rest
rem of the arguments as command with parameters and executes it. This can propagate 
rem RECURSIVELY, therefore the following command would set both variables a and b:
rem   SetVar a 22 SetVar b 33

rem Examples:
rem This sets variable a to xx:
rem   SetVar a xx
rem This sets variable a to value "123 xyz" (without quotes):
rem   SetVar a "123 xyz"
rem The value must be in quotes in this case, otherwise 123 and xyz would be treated
rem as two arguments, value would be set to 123, and xyz would be treated as command 
rem to be called recursively.

rem The following would set variable cc to XXYY, then variable bb to "11 12", and finally 
rem variable aa to "123 xyz" (all values without quotes). This is because the remaining 
rem arguments after the second one are treated as another command that is called after setting
rem the variable, and this is don twoce recursively in this case:
rem   SetVar aa "123 xyz" SetVar bb "11 12" SetVar cc XXYY


echo Setting variable %~1 to value: %~2
call set  %~1=%~2

rem If command-line arguments were specified then take them as another command and run the command:
if "%~3" NEQ "" (
    rem echo.
    rem echo SetVar: Executing embedded after-commnd specified by arguments:
    rem echo   call "%~3" "%~4" "%~5" "%~6" "%~7" "%~8" "%~9"
    call "%~3" "%~4" "%~5" "%~6" "%~7" "%~8" "%~9"
)

ver > nul

