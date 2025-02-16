

In the original Foidl's project, some modifications have been made, although all the original 
files were kept as they were.

A new project was created that compiles to a class library rather than application. However, a
copy of the file containning the main program was created and included in a new project. The 
Main() method was renamed so that it is not considered as entry poiint and does not conflict 
with any other entry point. Changed name is under conditional compilation, which can be 
switched by defining/undefining the ISCLASSLIBRARY connditinal compilation symbol. Therefore
the library project can be quickli changed to application project and back when necessary for
individual testing. 

Project web site:
http://www.codeproject.com/Articles/42258/Particle-swarm-optimization-for-function-optimizat

