

echo 
echo.
echo.

echo This will copy licene and other important doxuments to their intended places.

set dirorig=.
set diriglib=.\igbase
set dirigforms=.\igforms

set dirplot2d=.\extensions\igplot2d\plot2d
set dirplot3d=.\extensions\igplot3d\plot3d
set dirext=.\extensions\iglibext
set dirneural=.\extensions\iglibneural\neural
set dirneuralext=.\extensions\iglibneural\neural_ext

copy /y %dirorig%\ReadMe_IGLib.html %diriglib%
copy /y %dirorig%\License_IGLib_Partner.html %diriglib%
copy /y %dirorig%\License_IGLib_Redistributable.html %diriglib%


copy /y %dirorig%\ReadMe_IGLib.html %dirigforms%
copy /y %dirorig%\License_IGLib_Partner.html %dirigforms%
copy /y %dirorig%\License_IGLib_Redistributable.html %dirigforms%


copy /y %dirorig%\ReadMe_IGLib.html %dirplot2d%
copy /y %dirorig%\License_IGLib_Partner.html %dirplot2d%
copy /y %dirorig%\License_IGLib_Redistributable.html %dirplot2d%

copy /y %dirorig%\ReadMe_IGLib.html %dirplot3d%
copy /y %dirorig%\License_IGLib_Partner.html %dirplot3d%
copy /y %dirorig%\License_IGLib_Redistributable.html %dirplot3d%

copy /y %dirorig%\ReadMe_IGLib.html %dirext%
copy /y %dirorig%\License_IGLib_Partner.html %dirext%
copy /y %dirorig%\License_IGLib_Redistributable.html %dirext%

copy /y %dirorig%\ReadMe_IGLib.html %dirneural%
copy /y %dirorig%\License_IGLib_Partner.html %dirneural%
copy /y %dirorig%\License_IGLib_Redistributable.html %dirneural%


copy /y %dirorig%\ReadMe_IGLib.html %dirneural%
copy /y %dirorig%\License_IGLib_Partner.html %dirneural%
copy /y %dirorig%\License_IGLib_Redistributable.html %dirneural%

copy /y %dirorig%\ReadMe_IGLib.html %dirneuralext%
copy /y %dirorig%\License_IGLib_Partner.html %dirneuralext%
copy /y %dirorig%\License_IGLib_Redistributable.html %dirneuralext%



