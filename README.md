# IGLib Container Repository

This repository contains basic parts of the Investigative Generic Library (IGLib) and related software.


The original repository for this container is located at

> *https://github.com/ajgorhoe/IGLib.workspace.base.iglib.git*


## 1. How to use this repository on MS Windows:

Majority of IGLib software is cross/platform. However, there are some additional tools available for the Windows OS that makes it easier to work with code.

The best way to work with IGLib code is to use the IGLib container repository, which contains a number of Windows batch scripts that take care of properly cloning and updating the workspace directory structures from the several repositories.

IGLib container repository can be cloned from this address:

> *htps://github.com/ajgorhoe/iglibcontainer.git*


After cloning this repository on a local Windows machine, just run one of the batch scripts in order to check out and update other repositories necessary to view, build and run the code.

In the root directory of the cloned repository, run the following script:

> *PrepareIglLib.bat*

After the script finishes, you will find basic IGLib libraries in:

> *workspace/base/iglib*

In order to build the code, you can load one of the Visual Studio solution files found in:

> *workspace/base/igsolutions*


## 2. Other:


Please refer to the licences contained in specific repositories for conditions of use. Also note that software depends on a number of external libraries with their own licenses.

