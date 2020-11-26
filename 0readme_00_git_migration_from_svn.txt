
This is a temporary directory that contains IGLib git repositories converted
from the original SVN repositories.

Conversion were done using git svn clone:
https://git-scm.com/docs/git-svn
https://www.atlassian.com/git/tutorials/migrating-convert
https://git-scm.com/docs/git-svn


Typical commands used:

git svn clone --stdlayout https://localhost:8443/svn/ig_base/iglib  iglib  --authors-file=auth.txt

Above c. is for projects with standard SVN structure trunk - tags = branches.
Address is address of the project that contains trunk, branches and tags 
subdirectories, and does not need to be root of the repository (repositories 
can contain several projects in SVN and are usually sub-directories of a
repository root directory, contaiing standard subdirectories trung, tags and
branches).

If the structure is not standard, you can specify in mrore detail where to
find standard directories (or that they do not exist) by the following options:
  --trunk=<trunk_subdir>
  -tags=<tags_subdir>
  --branches=<branches_subdir> 

The --authors-file=auth.txt causes problems and reports the following error:
  Author: VisualSVN Server not defined in auth.txt file

Here is something on a similar problem (but it was not helpful):
https://stackoverflow.com/questions/11037166/author-not-defined-when-importing-svn-repository-into-git


======== List of actual conversion commands:

iglib: IGLib.workspace.base.iglib
https://gitlab.com/ajgorhoe/iglib.workspace.base.iglib.git
git svn clone --stdlayout https://localhost:8443/svn/ig_base/iglib  iglib 


unittests:
git svn clone --stdlayout https://localhost:8443/svn/ig_base/unittests  unittests 

igsolutions:
git svn clone --stdlayout https://localhost:8443/svn/ig_base_testdevelop/igsolutions  igsolutions 

shelldev:
git svn clone --stdlayout https://localhost:8443/svn/ig_base/shelldev  shelldev 

igapp:
git svn clone --stdlayout https://localhost:8443/svn/ig_base/shelldev https://localhost:8443/svn/ig_base_testdevelop/igapp  igapp 


data:
git svn clone --stdlayout https://localhost:8443/svn/ig_base/data  data 

igtest:
https://localhost:8443/svn/ig_base_testdevelop/igtest
git svn clone --stdlayout  https://localhost:8443/svn/ig_base_testdevelop/igtest  igtest 

igsandbox:
git svn clone --stdlayout  https://localhost:8443/svn/ig_base_testdevelop/igsandbox  igsandbox 


======== Remarks

After migration, create the following tag: special/20_11_migration_from_svn
with Message: This git repo was created by migration from IGLib's SVN repository.

---- Configure existing local git repository to sync with empty GitLab repo:

See for examples:
https://gitlab.com/ajgorhoe/iglib.workspace.base.iglib

Essential commands:

git config --global user.name "Igor Grešovnik"
git config --global user.email "gresovnik@gmail.com"

git remote rename origin old-origin
git remote add origin https://gitlab.com/ajgorhoe/iglib.workspace.base.iglib.git
git push -u origin --all
git push -u origin --tags
