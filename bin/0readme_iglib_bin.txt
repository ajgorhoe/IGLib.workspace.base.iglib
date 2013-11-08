
This directory contains binary files (such as library and dll files) that
are used by some extensions of the IGLib ot applications based on it.

RULES:
  All dlls and other libraries that libraries and applications link to, should
be copied directly to the iglib/bin/ directory.
  Shell script may be created to copy the appropriate libraries for specific 
platforms.
  If there are libraries for different platforms available, these libraries are
typically put in the appropriate sub-directory of iglib/bin/lib as originals.
Different versions for different platforms can be located in corresponding
platform subdirectories, and these versions mey be copied by dedicated shell
script to the iglib/bin/ directory where they should be liked to.



Wolfram Mathematica:
Wolfram.NETLink.dll - MathLink dll, routines for communicating with 
  Mathematica
ml64i3.dll - Native MathLink dll. Seems this is uesd by the previous one,
  since code reported error stating that this dll is missing, but saving 
  the dll in a directory included in Path does not solve the problem.


iglib/bin/lib/64/windows/ :
Contains MS Windows, MS Office and other Microsoft or Microsoft OS-related
libraries. These are proprietary libiraries.




