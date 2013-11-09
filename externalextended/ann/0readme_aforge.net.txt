
Some modifications were made to this solution after inclusion in this
directory. Instructions below tell how these modifications are made in the
case that a new version of the library is downloaded and utilized.

Projects in the library make references to the generated dlls. Therefore,
for each project used, an additional project is created as a copy of
original, whose name is prepended by AForge (e.g. AForge.Core.csproj).
These projects will then be used in internal solutions. These projects
must be modified in such a way that references to dlls are replaced
by references to the new projects (with the "AForge." prefix)! In this way
it is also easy to distinguish projects that belong to the AForge library
within our internal solutions!


	** Aforge.Net (neural networks library)

For easier inclusion in solutions, project files located in subdirectories of
./aforge.net/Sources are copied under different names. Copies are in the same
directories as originals, and have the "Aforge." prefix in their names.



