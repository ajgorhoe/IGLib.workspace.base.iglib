
This directory contains scripts used to bootstrap utility scripts from the
IGLibScripts repository.

In order to use scripts form IGLibScripts repo:
1. Copy content of this directory to a location where bootstrapping scripts
  can be accessed. ommit the directory.
2. Before using any scripts, just call the BootstrapScripting.bat. This will
  clone the IGLibScripts directory inside the bootstrapping directory, 
  eventually update it (updating is forced when the script is called wit
  the first argument equal to 1) and define environment variables that 
  contain path to the IGLibScripts directory and some of the contained 
  scripts.
  Remark: Script paths are stored in environment variables by the 
  SetScriptReferences.bat script of the IGLibScripts repository.
3. Call scripts via environment variables. For example, path of the 
  repository cloning/updating script is stored in the %UpdateRepo.bat%
  environment variable, path of the script for removing a repository is 
  stored in the %% variable, etc.

After bootstrapping is performed, this directory will contain a clone of
the IGLibScripts repository.

The fallback subdirectory contains some scripts from IGLibScripts repo, 
such that they can be used when the repository cannot be accessed (e.g., 
because there is no internet connection).

From time to time, the bootstrapping directory should be updated. You can do
this by calling the _UpdateBootstrappingScripts.bat. 
Do not forget to COMMIT and PUSH after calling that script.
_UpdateBootstrappingScripts can also be called in the bootstrapping directory
that is part of the IGLibScripts directory. In that case, this will 
recursively clone another copy of IGLibScripts repository into the 
bootstrapping directory. It can be removed later but this is not necessary.


