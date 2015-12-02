
echo off
echo.
echo.

echo This will copy licene and other important doxuments to their intended places.

set dirorig=.
set diriglib=.\igbase
set dirigforms=.\igforms

copy /y %dirorig%\ReadMe_IGLib.html %diriglib%
copy /y %dirorig%\License_IGLib_Partner.html %diriglib%
copy /y %dirorig%\License_IGLib_Redistributable.html %diriglib%



copy /y %dirorig%\ReadMe_IGLib.html %dirigforms%
copy /y %dirorig%\License_IGLib_Partner.html %dirigforms%
copy /y %dirorig%\License_IGLib_Redistributable.html %dirigforms%




