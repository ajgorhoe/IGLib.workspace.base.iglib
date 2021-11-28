
This directory contains additional scripts that should reside in local 
directories that perform bootstrapping (usually named bootstrappingscripts/).

This is the reference source for these scripts. the botstrapping directory
contains scripts from this directory and some scripts from ../ (and possibly
other directories) of this repository.

The current repository will usually be directly contained in the
bootstrapping directory (usually .../bootstrappingscripts/ or something 
similar). The script 
  UpdateBootstrappingScripts.bat
Copies all the scripts that are needed in the bootstrapping directory to 
that directory (which is two levels lower than this directory) from the
current working copy of this directory. 

Since this repository is a reference source for all the scripts needed for
bootstrapping, this is the normmal workflow for 
UPDATING the BOOTSTRAPPING SCRIPTS:
1. Make sure that working copy of this repository is cloned inside the 
    bootstrapping directory. If not then execute the following command in the
	bootstrapping directory:
	  git clone https://github.com/ajgorhoe/IGLib.modules.IGLibScripts.git IGLibScripts
2. Make sure that the appropriate branch / tag of this repository is checked
   out (normally, this should be the master branch).
3. Perform git pull within this repository, to update to the latest version.
4. Run the UpdateBootstrappingScripts.bat contained in the current directory.
5. Commit and push changes in the bootstrapping directory. It is always
   recommended to review the changes before doing that.

When updating scripts from this directory, one may want to actually update 
the sccripts and test them in the bootstrapping directory and then copying
them into this repository. If this is ths choice, one should perform an update
before starting to work on the scripts, to make sure that the updated versions
(i.e., the latest versions contained in this repository) are used as starting
point.

When a repository includes a bootstrapping/fallback/ directory that contains
a copy of the IGLibScripts repository, files from fallback_optional can 
be copied to that directory, to provide a standard fallback script. When a 
fallback copy of the IGLibScrips directory is located somewhere else, 
files from fallback_optional/ cannot be just automatically copied and the
fallback script (fallback/FallbackBootStrapScripting.bat) must be customized
in order to point to the correct location of the fallback copy of the 
IGLibScripts repository.
