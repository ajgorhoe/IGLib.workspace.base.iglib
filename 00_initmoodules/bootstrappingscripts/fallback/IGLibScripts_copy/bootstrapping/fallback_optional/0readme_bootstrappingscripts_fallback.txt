
This directory contains a fallback script for ../BootStrapScripting.bat, the
FallbackBootStrapScripting.bat.

This fallback script is executed when IGLibScripts repository cannot be 
accessed and IGLibScripts/ directory does not exist and cannot be cloned
from its original repository.

In this case, the fallback script attempts to locate a hardcopy of the
IGScripts repository (which may be located in the current repository,
usually in IGLibScripts_copy/ subdirectory of the current directory) or
at some other predefined location on disk, possibly at a known location
within the IGLib container directory.

The fallback script is usually custom for each repository because locations
of the fallback copy of IGLibScripts/ may vary from repository to repository.
However, some repositories have their own local copy of IGLibScripts, which
is by convention located in the IGLibScripts_copy/ subdirectory of the 
current directory. In such cases, the FallbackBootStrapScripting.bat is
standard and can be copied from the IGLibScripts repository, from its 
subdirectory bootstrapping/fallback_optional/

