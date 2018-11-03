
Json.NET

Original distribution available at:
https://www.newtonsoft.com/json
Downloaded from here (under 11.0.2 / Source code (.zip)):
https://github.com/JamesNK/Newtonsoft.Json/releases

WARNINGS and NOTES:
Project references had to be changed: target framework .NET Standard 2.0 was removed due to error reported.
This was done like this:
  * In Vilsual studio, the library project "Newtonsoft.Json" was unloaded (right-click, "Unload project"). 
  * Then in Solution exporer, right-click on project, "Edit ...csproj".
  * In <TargetFrameworks> element, comment "netstandard2.0;"
  * In Solution Explorer, "Load project" again. 

