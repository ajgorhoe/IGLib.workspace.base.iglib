
# IGLib Container Repository

This repository contains basic portions of the Investigative Generic Library (IGLib) and some related software (libraries and applications). Most of the IGLib is .NET software written in C#. Some of it still depends on the .NET Framework because of the dependencies, but the intention is to port most of the .NET libraries to latest versions of .NET (e.g. .NET 8).

The original repository for this container is located at

> *https://github.com/ajgorhoe/IGLib.workspace.base.iglib.git*

## How to use this repository on MS Windows

Majority of IGLib software is cross/platform. However, there are some additional tools available for the Windows OS that makes it easier to work with code.

The best way to work with IGLib code is to use the IGLib container repository, which contains a number of Windows batch scripts that take care of properly cloning and updating the workspace directory structures from the several repositories.

IGLib container repository can be cloned from this address:

> *htps://github.com/ajgorhoe/iglibcontainer.git*

After cloning this repository on a local Windows machine, just run one of the batch scripts in order to check out and update other repositories necessary to view, build and run the code.

In the root directory of the cloned repository, run the following script:

> *PrepareIglLib.bat*

After the script finishes, you will find basic IGLib libraries in:

> *workspace/base/iglib*

In order to build the code, you can then open one of the Visual Studio solution files found in:

> *workspace/base/igsolutions*

Authors of the software use Visual Studio to build the code by using the mentioned solution files. For building on other platforms, we use Visual Studio Code or command-line tools from .NET SDKs. People also recommend JetBrain's Rider. We have used MonoDevelop for cross-platform development in the past, but the project is not actively developed any more and the software features may gradually become outdated.

## How to use the repositories on other systems

Many of the IGLib libraries and applications are cross-platform. On Windows, Visual Studio is the recommended IDE for building the software. In other systems, you can use a cross-platform IDE like Visual Studio Code, or you can build by command-line utilities included in .NET SDKs. There are also other IDEs, but some may not support all the features necessary to build the complete IGLib code. 

I have used MonoDevelop to build the code on Linux, but it seems now that MonoDevelop is not actively developed any more in favor of some other IDEs such as Visual Studio Code (its repository is archived on GitHub).

## Other


Please refer to the licenses contained in specific repositories for conditions of use. Also note that software depends on a number of external libraries with their own licenses.


### 3.1 Solution files in this repository

*IGLib.sln* can be used to build base IGLib libraries. However, this solution file may be outdated.

*ShellDevAll.sln* is a solution file that contains a broader range of projects, including extended libraries not included in this repository, additional external libraries, and a number of IGLib-based applications. This solution is usually more up-to-date than the *IGLib.sln*, but it contains projects to which you may not have access. Many projects in this solution are outside the current repository, and can be cloned easily by using the container repository as mentioned above. However, a number of repositories that contain projects included in this solution are not publicly available. Just ignore the projects that cannot be loaded.

Solution files are included in this repository for convenience, especially for quick view of the project structure without needing to clone the other relevant repositories at the prescribed relative paths. For development, one should use solution files contained in *../igsolutions/*.




# Additional Information (from the previous HTML Readme)

  IGLib.NET (Investigative Generic Library) is a set of utility libraries that are particularly suited for development of technical applications (see also the [web page](http://www2.arnes.si/%7Eljc3m2/igor/iglib/) and [code documentation](http://www2.arnes.si/%7Eljc3m2/igor/software/codedoc/generated/iglib/html/classes.html)).

  The system has been designed and developed by [Igor Grešovnik](http://www2.arnes.si/%7Eljc3m2/igor/index.html), who set up its foundations in 2006 and is leading its development. In longer term, the library is intended (at least in part) for distribution as free open source under a BSD-like license. However, the library will not be open for public immediately because the author wants to achieve a certain level of maturity first and stabilize library development within a smaller group of dedicated developers. Before this happens, groups and individuals can join development or usage of the library by individual agreement with the principal author. Up to now, a small number of companies have been using the library for developing applications. The author is open for collaboration but would like to retain a good control over development for some time. He believes that such position will be beneficial for future users and developers.  

    Since 2011, the library has been used by the _[Laboratory for Multiphase Processes](http://www.ung.si/en/research/multiphase-processes/)_ of the _[University of Nova Gorica](http://www.ung.si/en/)_, and by the _[Laboratory for Advanced Materials Systems](http://www.cobik.si/laboratoriji/laboratorij-za-sisteme-z-naprednimi-materiali?lang=eng)_ of the _[Centre of Excellence for Biosensors, Instrumentation and Process Control](http://www.cobik.si/index?lang=eng)_, where it is used as base library for development of applications in the field of  neural networks and optimization. These groups use the code under a customized license agreement that allows free use of the binaries in order to create derived products.

  IGLib contains some basic utilities like those for parsing strings, generic I/O utilities, a couple of utilities for building GUI, a numerical library, 2D and 3D graphics modules, a parallel computing module, application framework featuring a layered interpreter system, and other components. It aims at providing a well designed base library for developnent of complex numerical and other technical applications. Parts of IGLib have also been used in other areas such as a system for managing a histological laboratory or large scale invoicing support system.

  Historically, the initial motivation for development of the library arose from the needs to have a good base library for development of complex optimization software, and development was first concentrated around re-implementation of parts of [IOptLib](http://www2.arnes.si/%7Eljc3m2/igor/ioptlib/). However, the library was planned in a broader sense since the very beginning of its existence. For more information, check the library home page at

  [http://www2.arnes.si/~ljc3m2/igor/iglib/](http://www2.arnes.si/%7Eljc3m2/igor/iglib/),  

or check [code documentation](http://www2.arnes.si/%7Eljc3m2/igor/software/codedoc/generated/iglib/index.html) at  

  [http://www2.arnes.si/~ljc3m2/igor/software/codedoc/generated/iglib/html/index.html](http://www2.arnes.si/%7Eljc3m2/igor/software/codedoc/generated/iglib/html/index.html).

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
