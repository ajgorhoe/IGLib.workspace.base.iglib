
This directory contains copies of some scripts from the IGLibScripts
repository, which are used to botstrap the scripts necessary for updating
module repositories.

From time to time, these scripts will be updated. If there are breaking
changes in the original copies from IGLibScripts then other scripts 
involved in bootstrapping should be modified accordingly, and updating
modules should be tested.

After bootstrapping is performed, this directory will contain a clone of
the IGLibScripts repository. Files from this cloned repository may or may not
be added and committed to the containinng repository, dependent on the 
policy of that specific repository. 
A possible motivation for committing the files to containing repository is
to provide a fallback in case that the remote IGLibScripts repository is 
inaccessible and cannot be cloned.

