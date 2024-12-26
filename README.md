
# IGLib Framework

<img src="./IGLibIcon_256x256.png" alt="[IGLib]" align="right" width="48pt"
  style="float: right; max-width: 30%; width: 48pt; margin-left: 8pt;" />

This repository contains basic portions of the ***Investigative Generic Library** (**IGLib**)* and some related software (libraries and applications). Most of the IGLib is .NET software written in C#. Some of it still depends on the .NET Framework because of the dependencies, but the intention is to port most of the .NET libraries to latest long-term support versions of .NET (e.g. .NET 8).

The original repository for this library is located at

> *https://github.com/ajgorhoe/IGLib.workspace.base.iglib.git*

## How to Use this Repository on MS Windows

Majority of IGLib software is cross/platform. However, there are some additional tools available for the Windows OS that makes it easier to work with code.

The best way to work with IGLib code is to use the **[IGLib Container repository](https://github.com/ajgorhoe/iglibcontainer)**, which contains a number of Windows batch scripts that take care of properly cloning and updating the workspace directory structure from the several [constituent repositories](#repository-structure). IGLib container repository can be cloned from the following address:

> *https://github.com/ajgorhoe/iglibcontainer.git

After cloning this repository on a local Windows machine, just run one of the batch scripts in order to clone or update other repositories necessary to view, build and run the code.

In the root directory of the cloned IGLib Container repository, run the following script:

> *Prepare_IglLib.bat*

This will clone the necessary repositories to work with IGLib at the correct locations. The script attempts to clone many repositories that are private, and you should just skip them if you don't have the access to these repositories. Alternatively, you can run individual update scripts from the cloned container repository. You can start by running the following scripts:

> *workspace/base/UpdateModule_iglib.bat*
> *workspace/base/UpdateModule_igsolutions.bat*
> *workspace/base/UpdateModule_iglibexternal.bat*
> *workspace/base/iglibexternal/IGLibExternal/UpdateIGLibAll.bat*

After the script finishes, you will find basic IGLib libraries in the following directory:

> *workspace/base/iglib/*

In order to build the code, you can then open one of the Visual Studio solution files contained in the directory:

> *workspace/base/igsolutions/*

Authors of the software use Visual Studio to build the code by using the mentioned solution files. For building on other platforms, we use Visual Studio Code or command-line tools from .NET SDKs. People also recommend JetBrain's Rider. We have used MonoDevelop for cross-platform development in the past, but the project is not actively developed any more and the software features may gradually become outdated.

## How to use the repositories on other systems

Many of the IGLib libraries and applications are cross-platform. On Windows, Visual Studio is the recommended IDE for building the software. In other systems, you can use a cross-platform IDE like Visual Studio Code, or you can build by command-line utilities included in .NET SDKs. There are also other IDEs, but some may not support all the features necessary to build the complete IGLib code. 

I have used MonoDevelop to build the code on Linux, but it seems now that MonoDevelop is not actively developed any more in favor of some other IDEs such as Visual Studio Code (its repository is archived on GitHub).

## Other

Please refer to the licenses contained in specific repositories for conditions of use. Also note that software depends on a number of external libraries with their own licenses.

Solution files contained in this repository:

* *IGLib.sln* can be used to build base IGLib libraries. However, this solution file may be outdated.
* *ShellDevAll.sln* is a solution file that contains a broader range of projects, including extended libraries not included in this repository, additional external libraries, and a number of IGLib-based applications. This solution is usually more up-to-date than the *IGLib.sln*, but it contains projects to which you may not have access. Many projects in this solution are outside the current repository, and can be cloned easily by using the container repository as mentioned above. However, a number of repositories that contain projects included in this solution are not publicly available. Just ignore the projects that cannot be loaded.

Solution files are included in this repository for convenience, especially for quick view of the project structure without needing to clone the other relevant repositories at the prescribed relative paths. For development, one should use solution files contained in *../igsolutions/*.

# Additional Information (from the previous HTML Readme)

**IGLib.NET** (the Investigative Generic Library) is a set of utility libraries that are particularly suited for development of technical applications (see also the [web page](http://www2.arnes.si/%7Eljc3m2/igor/iglib/) and [code documentation](http://www2.arnes.si/%7Eljc3m2/igor/software/codedoc/generated/iglib/html/classes.html)).

  The system has been designed and developed by [Igor Grešovnik](http://www2.arnes.si/%7Eljc3m2/igor/index.html), who set up its foundations in 2006 and is leading its development. In longer term, the library is intended (at least in part) for distribution as free open source under a BSD-like license. However, the library will not be open for public immediately because the author wants to achieve a certain level of maturity first and stabilize library development within a smaller group of dedicated developers. Before this happens, groups and individuals can join development or usage of the library by individual agreement with the principal author. Up to now, a small number of companies have been using the library for developing applications. The author is open for collaboration but would like to retain a good control over development for some time. He believes that such position will be beneficial for future users and developers.  

    Since 2011, the library has been used by the *Laboratory for Multiphase Processes* of the *[University of Nova Gorica](http://www.ung.si/en/)*, and by the *[Laboratory for Advanced Materials Systems](http://www.cobik.si/laboratoriji/laboratorij-za-sisteme-z-naprednimi-materiali?lang=eng)* of the *[Centre of Excellence for Biosensors, Instrumentation and Process Control](http://www.cobik.si/index?lang=eng)*, where it is used as base library for development of applications in the field of  neural networks and optimization. These groups use the code under a customized license agreement that allows free use of the binaries in order to create derived products.

  IGLib contains some basic utilities like those for parsing strings, generic I/O utilities, a couple of utilities for building GUI, a numerical library, 2D and 3D graphics modules, a parallel computing module, application framework featuring a layered interpreter system, and other components. It aims at providing a well designed base library for development of complex numerical and other technical applications. Parts of IGLib have also been used in other areas such as a system for managing a histological laboratory or large scale invoicing support system.

  Historically, the initial motivation for development of the library arose from the needs to have a good base library for development of complex optimization software, and development was first concentrated around re-implementation of parts of [IOptLib](http://www2.arnes.si/%7Eljc3m2/igor/ioptlib/). However, the library was planned in a broader sense since the very beginning of its existence. For more information, check the library home page at

> [http://www2.arnes.si/~ljc3m2/igor/iglib/](http://www2.arnes.si/%7Eljc3m2/igor/iglib/),  

or check [code documentation](http://www2.arnes.si/%7Eljc3m2/igor/software/codedoc/generated/iglib/index.html) at  

> [http://www2.arnes.si/~ljc3m2/igor/software/codedoc/generated/iglib/html/index.html](http://www2.arnes.si/%7Eljc3m2/igor/software/codedoc/generated/iglib/html/index.html).

## Repository Structure

> **Important Remark**: the source of information in this section is [this wiki section](https://github.com/ajgorhoe/wiki.IGLib/blob/main/web/IGLibWebPages.md#iglib-legacy-repositories-on-github) (private access). This section will **probably not be updated often**, but if necessary, please *only copy updates in the direction source => this section*.

IGLib Framework projects need to be positioned in particular relative positions with respect to each other in order to build (this also applies to their external dependencies). The usual practice is to clone the IGLib repositories via the **[IGLib Container Repository](https://github.com/ajgorhoe/iglibcontainer)**.

Most of the IGLib.NET Framework repositories are contained in the *[workspace/base/ directory](https://github.com/ajgorhoe/iglibcontainer/tree/master/workspace/base)* of the *container repository*, and they can be cloned by using the update scripts in this directory, such as *[UpdateModule_iglib.bat](https://github.com/ajgorhoe/iglibcontainer/blob/master/workspace/base/UpdateModule_iglib.bat)* for the most basic IGLib library or *[UpdateModule_igsolutions.bat](https://github.com/ajgorhoe/iglibcontainer/blob/master/workspace/base/UpdateModule_igsolutions.bat)* for Visual Studio solutions used to build IGLib and the related applications. All repositories in the workspace/base/ directory of the *container repository* can be restored (cloned / updated) by running the *[UpdateDirectoryModules_Base.bat](https://github.com/ajgorhoe/iglibcontainer/blob/master/workspace/base/UpdateDirectoryModules_Base.bat)* script, or the *[UpdateDirectoryModules_BaseExtended.bat](https://github.com/ajgorhoe/iglibcontainer/blob/master/workspace/base/UpdateDirectoryModules_BaseExtended.bat)* script to include a wider set of repositories.

Some repositories related to IGLib Framework are also located in the *[workspace/ directory](https://github.com/ajgorhoe/iglibcontainer/tree/master/workspace)* of the container repository, such as *workspace/codedoc/* that contains scripts for generating IGLib code documentation.

The scripts that update and clone IGLib repositories first clone the IGLibScripts repository (if not cloned yet) and then use the scripts from this repository to clone or update other repositories from the *IGLib Container repository*. The corresponding GitHub repositories have names that indicate their standard positions within the [IGLib Container Repository](https://github.com/ajgorhoe/iglibcontainer). For example, the repository named **[IGLib.<i><u>workspace.base.iglib</u></i>](https://github.com/ajgorhoe/IGLib.workspace.base.iglib)** has its standard place in the *workspace/base/iglib* subdirectory of the cloned container's repository, and is cloned or updated by running the script *[workspace/base/iglib/UpdateModule_iglib.bat](https://github.com/ajgorhoe/iglibcontainer/blob/master/workspace/base/UpdateModule_iglib.bat)* (no arguments are necessary).

Below is the list of IGLib Framework-related repositories.

* Core IGLib repositories:
  * **[iglib](https://github.com/ajgorhoe/IGLib.workspace.base.iglib)** (full name: *IGLib.workspace.base.iglib*) - the base libraries of IGLib Framework
  * **[iglibexternal](https://github.com/ajgorhoe/IGLib.workspace.base.iglibexternal)** (full name: *IGLib.workspace.base.iglibexternal*) - contains scripts for cloning and updating the **external libraries used by IGLib** (the dependencies)
  * **[igsolutions](https://github.com/ajgorhoe/IGLib.workspace.base.igsolutions)** (full name: *IGLib.workspace.base.igsolutions*) - contains **Visual Studio solutions for building the IGLib** libraries and applications
  * **[shelldev](https://github.com/ajgorhoe/IGLib.workspace.base.shelldev)** (full name: *IGLib.workspace.base.shelldev*) - contains **higher level libraries of IGLib Framework**, several **IGLib-based applications**, and **experimental projects**
  * **[unittests](https://github.com/ajgorhoe/IGLib.workspace.base.unittests)** (full name: *IGLib.workspace.base.unittests*) - contains some of the unit tests for the IGLib framework's libraries and applications (the majority of these are archived somewhere, or may be lost during moves and server migrations)
  * **[igtest](https://github.com/ajgorhoe/IGLib.workspace.base.igtest)** (full name: *IGLib.workspace.base.igtest*) - contains some **testing projects for IGLib libraries and applications**. The majority of IGLib-related tests are probably archived somewhere, or maybe they were lost during moves and server migrations.
  * **[../../workspaceprojects](https://github.com/ajgorhoe/IGLib.workspaceprojects_container)** (full name: *IGLib.workspaceprojects_container*, location in the IGLib container repository (not consistent with name): workspaceprojects/) - a container repository for repositories containing high level tests that need extensive data.
  * **[../../workspaceprojects/00tests](https://github.com/ajgorhoe/IGLib.workspaceprojects.00tests)** (full name: *IGLib.workspaceprojects.00tests*) - contains various high level tests for IGLib and applications based on it, which are sometimes run by IGShell scripts and may require extensive test data.
  * **[../../workspaceprojects/00testsext](https://github.com/ajgorhoe/IGLib.workspaceprojects.00testsext)** (full name: *IGLib.workspaceprojects.00testsext*) - contains extended high level tests for IGLib and applications based on it, which are sometimes run by IGShell scripts and may require extensive test data.
  * **[]()** (full name: **) - 
* Auxiliary IGLib repositories:
  * **[../codedoc](https://github.com/ajgorhoe/IGLib.workspace.doc.codedoc)** (full name: *IGLib.workspace.doc.codedoc*, location in the container repository (not consistent with name): *workspace.codedoc/*) - contains scripts for **automatic generation of code documentation** for IGLib libraries, applications, and tests
  * **[../codedoc_resources](https://github.com/ajgorhoe/IGLib.workspace.codedoc_resources)** (full name: *IGLib.workspace.codedoc_resources*) - contains additional resources (such as binaries) for the *codedoc* repository mentioned above this entry
  * **[../codedoc_new](https://gitlab.com/ajgorhoe/iglib.workspace.codedoc_develop.git)** (full name: *IGLib.workspace.codedoc_new*, location in the container repository (not consistent with name): *workspace.codedoc_new/*) - contains **new developments** for scripts for **automatic generation of code documentation** for IGLib libraries, applications, and tests
  * **[data](https://github.com/ajgorhoe/IGLib.workspace.base.data)** (full name: *IGLib.workspace.base.data*) - contains some data for projects based on IGLib, e.g. some **neural network models**.
* Other repositories:
  * **[../applications](https://github.com/ajgorhoe/IGLib.workspace.applications)** (full name: *IGLib.workspace.applications*): contains binaries of some demo applications for IGLib
  * **[iglibapp](https://github.com/ajgorhoe/IGLib.workspace.base.iglibapp)** (full name: *IGLib.workspace.base.iglibapp*) - contains some **applications based on IGLib** in **binary form**, in particular the **IGLibShell**, **HashForm** and **HashShell**
Obsolete:
  * **[igapp](https://github.com/ajgorhoe/IGLib.workspace.base.igapp)** (full name: *IGLib.workspace.base.igapp*) - contains some old applications related to Inverse and IGlib. This repo is **mainly obsolete**
* Out of use:
  * **[bin](https://github.com/ajgorhoe/IGLib.workspace.base.bin)** (full name: *IGLib.workspace.base.bin*) - an **obsolete project** containing some stuff that I have provided to the *Josef Stefan institute*.
  * **[iglibapp_TODELETE](https://github.com/ajgorhoe/IGLib.workspace.base.iglibapp_TODELETE)** (full name: *IGLib.workspace.base.iglibapp_TODELETE*) - not used.

## External Software Libraries

  This library depends on a number of external free open source libraries. Authors of the code are grateful to all developers that invested their work to develop these libraries and who made them open and accessible to the public.

The following external libraries are used:  

* [Math.Net Numerics](http://numerics.mathdotnet.com/), an excellent scientific library written entirely in C#, development lead by Christoph Rüegg. Created by [merging](http://christoph.ruegg.name/blog/2009/8/3/dnanalytics-iridium-mathnet-numerics.html) two predecessor libraries, the [Math.Net Iridium](http://www.mathdotnet.com/Iridium.aspx) and the [dnAnalytics](http://dnanalytics.codeplex.com/).
* [Math.Net](http://www.mathdotnet.com/), a scientific library written entirely in C#. Iridium and Neodym libraries were used from this project before Iridium merged with dnAnalytics into Math.Net Numerics. Now the latter library is used.
* [ZedGraph](http://zedgraph.org/), a flexible charting library for .NET.
* [NPlot](http://netcontrols.org/nplot/wiki/index.php?n=Main.HomePage), an easy to use 2D plotting library.
* [Activiz](http://www.kitware.com/products/activiz.html), C# wrappers for the VTK 3D graphics library.
* [Aforge.Net](http://www.aforgenet.com/aforge/framework/), a framework library for development of neural network-based application.
* [Accord.Net](http://code.google.com/p/accord/), an extension of the Aforge.Net framework.
* [ParticleSwarm](http://www.codeproject.com/Articles/42258/Particle-swarm-optimization-for-function-optimizat) project of Guinther M. Foidl, an implementation of a particle swarm minimization algorithm.
* [ALGLIB](http://www.alglib.net/), a numerical analysis and data processing library. Small portions of an old version are used that were licensed under a more permissive license (now the library is under the GPL).

### Other External Works Used

  Beside libraries, there are other external works used by IGLib:

* [Silk Icons](http://www.famfamfam.com/lab/icons/silk/), a free icon library.
* Some sounds from the [FreeSound](https://freesound.org/) library.  
  
Please visit the web pages of these great libraries (just follow the links above) and consider whether you can support their development in some way. Check also the license agreements for these referenced libraries for precise conditions for using the libraries.

## Related Links

### Software Pages

[IGLib](https://ajgorhoe.github.io/IGLibFramework/iglib/)\-[related](https://ajgorhoe.github.io/IGLibFramework/):
* [Site map](https://ajgorhoe.github.io/IGLibFramework/): IGLib and related software   
* [**IGLib**, the Investigative Generic Library](https://ajgorhoe.github.io/IGLibFramework/iglib/)
  * [IGLib code documentation](https://ajgorhoe.github.io/IGLibFramework/software/codedoc/generated/iglib/): [index](https://ajgorhoe.github.io/IGLibFramework/software/codedoc/generated/iglib/html/), [class list](https://ajgorhoe.github.io/IGLibFramework/software/codedoc/generated/iglib/html/annotated.html), [class hierarchy](https://ajgorhoe.github.io/IGLibFramework/software/codedoc/generated/iglib/html/hierarchy.html) ([graphical](https://ajgorhoe.github.io/IGLibFramework/software/codedoc/generated/iglib/html/inherits.html)), [namespace list](https://ajgorhoe.github.io/IGLibFramework/software/codedoc/generated/iglib/html/namespaces.html), [file list](https://ajgorhoe.github.io/IGLibFramework/software/codedoc/generated/iglib/html/files.html)  
  * [Extended code documentation](https://ajgorhoe.github.io/IGLibFrameworkCodedoc/generated/16_04_igliball_1.7.2/): [class list](https://ajgorhoe.github.io/IGLibFrameworkCodedoc/generated/16_04_igliball_1.7.2/html/annotated.html), [class hierarchy](https://ajgorhoe.github.io/IGLibFrameworkCodedoc/generated/16_04_igliball_1.7.2/html/hierarchy.html) ([graphical](https://ajgorhoe.github.io/IGLibFrameworkCodedoc/generated/16_04_igliball_1.7.2/html/inherits.html)), [namespace list](https://ajgorhoe.github.io/IGLibFrameworkCodedoc/generated/16_04_igliball_1.7.2/html/namespaces.html), [file list](https://ajgorhoe.github.io/IGLibFrameworkCodedoc/generated/16_04_igliball_1.7.2/html/files.html) 
  * [documentation archive](https://www2.arnes.si/~fgreso/archive/software/codedoc/generated/archive.html)  
* [Related software](https://ajgorhoe.github.io/IGLibFramework/software/) :
  * [**NeurApp** - approximation with artificial neural networks](https://ajgorhoe.github.io/IGLibFramework/software/NeurApp/) (for education & exploration purposes)  
        
  * **[AnnApp](https://ajgorhoe.github.io/IGLibFramework/software/AnnApp/)** \- a software for training and using industrial neural network models
  * [IGShell](https://ajgorhoe.github.io/IGLibFramework/software/IGLibShellApp/) - a shell application based on IGLib. Includes a Visual Studio / Mono project and two [hosted applications](https://ajgorhoe.github.io/IGLibFramework/software/IGLibShellApp/HashForm.html) :
  * [HashForm](https://ajgorhoe.github.io/IGLibFramework/software/IGLibShellApp/HashForm.html#hashform) - a GUI-based user friendly application for file verification; Performs calculation and verification of various kinds of file or text hash values.
  * [HashShell](https://ajgorhoe.github.io/IGLibFramework/software/IGLibShellApp/HashForm.html#hashshell) - a command-line application for file verification and cryptographic operations
  * [IOptLib](https://ajgorhoe.github.io/IGLibFramework/ioptlib/) - a library for solving engineering optimization problems  
        

_[Optimization Shell Inverse](https://ajgorhoe.github.io/Inverse/sitemap.html)_ : (a legacy C/C++ software for solving inverse and optimization problems in engineering)  

* [*Inverse* site map](https://ajgorhoe.github.io/Inverse/sitemap.html)
* *[Inverse](https://ajgorhoe.github.io/Inverse/)* - index
* Documentation:
  * [Quick Introduction to Inverse (PDF)](https://ajgorhoe.github.io/Inverse/doc/other/invquick2.pdf) ; [HTML version](https://ajgorhoe.github.io/Inverse/doc/other/quick/)  
  * [Inverse Manuals](https://ajgorhoe.github.io/Inverse/doc/man/)
  * [Thesis on Inverse](https://ajgorhoe.github.io/Inverse/doc/phd/)

Other:

* [Sendigence](https://ajgorhoe.github.io/IGLibFramework/sendigence/) 
* [IGS](https://ajgorhoe.github.io/IGLibFramework/c3m/igs/igs.html) (graphical system)

### Documents and Other Pages

**[Documents](https://ajgorhoe.github.io/IGLibFramework/doc/)** (reports, manuals, courses, articles, etc.)  

* [Documents from COBIK](https://ajgorhoe.github.io/IGLibFramework/cobik/)  
* [Ph. D. thesis](https://ajgorhoe.github.io/IGLibFramework/doc/theses/phd/) on use of optimization, inverse methods, numerical simulation and the related software in various engineering fields  
* [Bibliography (Igor G.)](https://ajgorhoe.github.io/IGLibFramework/0docigor/bib/bibliography_gresovnik_cobiss.htm)  

### Repositories

* [Repositories base](https://www.github.com/ajgorhoe)
* [IGLib base repository](https://github.com/ajgorhoe/IGLib.workspace.base.iglib) (see also the [Repository Structure](https://github.com/ajgorhoe/IGLib.workspace.base.iglib#repository-structure) section in readme)  
* IGLib.NET modules:
* [IGLibScripts](https://github.com/ajgorhoe/IGLib.modules.IGLibScripts) repository - contains a variety of script to assist software development  

Overviews:

* [Site map (GitHub pages)](https://ajgorhoe.github.io/)   
* [IGLib Framework site map](https://ajgorhoe.github.io/IGLibFramework/) on GitHub  
* [Inverse site map](https://ajgorhoe.github.io/Inverse/) on GitHub  

#### Other Pages on GitHub

* [IGLib](https://ajgorhoe.github.io/IGLibFramework/iglib/)    
* [NeurApp](https://ajgorhoe.github.io/IGLibFramework/software/NeurApp/)
* [AnnApp](https://ajgorhoe.github.io/IGLibFramework/software/AnnApp/)
* [IGShell](https://ajgorhoe.github.io/IGLibFramework/software/IGLibShellApp/)
* [HashForm](https://ajgorhoe.github.io/IGLibFramework/software/IGLibShellApp/HashForm.html#hashform)   and [HashShell](https://ajgorhoe.github.io/IGLibFramework/software/IGLibShellApp/HashForm.html#hashshell)
* [IOptLib](https://ajgorhoe.github.io/IGLibFramework/ioptlib/)
* [Inverse](https://ajgorhoe.github.io/Inverse/)
