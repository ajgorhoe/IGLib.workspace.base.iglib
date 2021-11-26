# IGLib Container Repository

This repository contains basic portions of the Investigative Generic Library (IGLib) and some related software (libraries and applications). Most of the IGLib is .NET software written in C#. Some of it still depends on the .NET Framework because of the dependencies, but the intention is to port most of the .NET libraries to latest versions of .NET (e.g. .NET 5).


The original repository for this container is located at

> *https://github.com/ajgorhoe/IGLib.workspace.base.iglib.git*


## 1. How to use this repository on MS Windows

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

Authors of the software use Visual Studio to build the code by using the mentioned solution files. For building on other platforms, we use Visual Studio Code or command-line tools from .NET SDKs. People also recomment JetBrain's Rider. We have used MonoDevelop for cross-platform development in the past, but the project is not actively developed any more and the software features may gradually become outdated.

## 2. How to use the repositories on other systems

Many of the IGLib libraries and applications are cross-platform. On Windows, Visual Studio is the recommended IDE for building the software. In other systems, you can use a cross-platform IDE like Visual Studio Code, or you can build by command-line utilities included in .NET SDKs. Theree are also other IDEs, but some may not support all the features necessary to build the complete IGLib code. 

I have used MonoDevelop to build the code on Linux, but it seems now that MonoDevelop is not actively developed any more in favor of some other IDEs such as Visual Studio Code (its repository is archived on GitHub). 

## 3. Other


Please refer to the licences contained in specific repositories for conditions of use. Also note that software depends on a number of external libraries with their own licenses.


### 3.1 Solution files inthis repository

*IGLib.sln* can be used to build base IGLib libraries. However, this solution file may be outdated.

*ShellDevAll.sln* is a solution file that contains a broader range of projects, including extended libraries not included in this repository, additional external libraries, and a number of IGLib-based applications. This solution is usually more up-to-date than the *IGLib.sln*, but it conteins projects to which you may not have access. Many projects in this soltion are outside the current repository, and can be cloned easily by using the container repository as mentioned above. However, a number of repositories that contain projects included in this solution are not publically available. Just ignore the projects that cannot be loaded.

Solution files are included in this repository for convenience, especially for quick view of the project structure without needing to clone the other relevant repositories at the prescribed relative paths. For development, one should use solution files contained in *../igsolutions/*.
