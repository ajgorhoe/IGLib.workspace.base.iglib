@echo off

:: Performs bootstrapping by calling BootStrapScripting.bat
:: and in addition, it forces updating IGLibScripts.

:: BootStrapScripting.bat itself will skip updating IGLibScripts directory
:: if it is already cloned, and the curent script makes sure that update is
:: performed, terefore it can be run whenever one would like to pull the
:: eventual repository updates.


:: Call bootstrapping script to define basic directories and strings:
call "%~dp0BootStrapScripting.bat" 1


