

Notes on MathNet.Numerics:

This library replaced Math.Net library. It was created by merging dnAnalytics
and Math.NET Iridium. Gradually, Math.Net will be replaced by this library and
will be removed form the repository. Currently, there are still many 
dependencies on older Mat.Net code, therefore the library should not be 
removed from the repository before beginning of 2013!


CHANGES to be applied to MathNet.Numerics when included in IGLib:
  - in the directory .../MathNetNumerics/src/Examples/, copy Examples.csproj
to NumericsExamples.csproj.
  - in .../MathNetNumerics/src/, copy MathNet.Numerics.sln to 
MathNet.NumericsReduced.sln. In this solution, keep only the Numerics and the
Example projects.

Changes in ParticleSwarmFoidl:
  - License agreement and links to original software locations were copied
into the project directory.
  - A project file 00ParticleSwarmFoidl.csproj was added in order to obtain
a class library project (original was Windows Forms application).
  - The file Program.cs was copied to ProgramParticleSwarmModified.cs and
modified. This file is contained in the modified project and is such that 
you can switch between version for library and application through definition
of the ISCLASSLIBRARY conditional compilation symbol.



