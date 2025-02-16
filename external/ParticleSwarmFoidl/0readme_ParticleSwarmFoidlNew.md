
# New project from Foidl's Particle Swarm Code

Upgrade to .NET 8 could not be done easily for the project.

Therefore, a new Windows Forms project was created from scratch (**[ParticleSwarmFoidlNew.csproj](./src/ParticleSwarmFoidlNew/00ParticleSwarmFoidlNew.csproj)**) and the old code was added into it.

The directory **[FilesFromProjectCreation/](./src/ParticleSwarmFoidlNew/FilesFromProjectCreation/)** contains the original files from the time project was created. These were used to make thee minimal WinForms project dual-target both .NET Framework 4.8 and .NET 8.

## Links

* Original project:
  * [Particle swarm optimization for function optimization](https://www.codeproject.com/Articles/42258/Particle-swarm-optimization-for-function-optimizat) on CodeProject
* Projects in source repository:
  * [0readme_ParticleSwarmFoidlNew.md](./0readme_ParticleSwarmFoidlNew.md)

## Old Files Kept

The **[src/Backup_ParticleSwarmFoidl_Original/](./src/Backup_ParticleSwarmFoidl_Original/)** directory contains files form the original project downloaded from CodeProject (see the links).

Directory **[src/ParticleSwarmFoidl/](./src/ParticleSwarmFoidl/)** contains the old project **[00ParticleSwarmFoidl.csproj](./src/ParticleSwarmFoidl/00ParticleSwarmFoidl.csproj)** with code that was a slightly corrected original project, just in the way that project begun to work (the original project code downloaded long time ago from CodeProject did not work).

## Upgrading from .NET 4.8 to .NET 8

The upgraded project is **[00ParticleSwarmFoidlNew.csproj](./src/ParticleSwarmFoidlNew/00ParticleSwarmFoidlNew.csproj)**.

Upgrading to SDK-style project went smoothly. However, just adding the .NET 8 build target do the existing .NET Framework 4.8 target did not work straight away. As said, the approach taken was to create an empty .NET 8 project and to add source files from the old project to the new project, hoping that I can make this work. Of course, in the template-based new project, the **Main()** method was **renamed** because we cannot have two entry methods. Also, all **files form the newly created template project** (except the project file) were moved to a separate directory ([FilesFromProjectCreation/](./src/ParticleSwarmFoidlNew/FilesFromProjectCreation/)) such that they are clearly separated form the files that came from the old particle swarm project.

## Modified and Improved Project

The **[00ParticleSwarmFoidlModified.csproj](./src/ParticleSwarmFoidlModified/00ParticleSwarmFoidlModified.csproj)** project started as a copy of [00ParticleSwarmFoidlNew.csproj](./src/ParticleSwarmFoidlNew/00ParticleSwarmFoidlNew.csproj), intended to be modified and improved. This is the place for **all future modifications**.
