
Solution folder Additional in the solution:

This folder contains additional projects that do not constitute the Reporter,
but are used for testing, demonstration, etc.


Speech:

Subfolder Speech contains speech extensions of the Reporter. This is isolated
in a separate subfolder because the speech extension uses the SpeechLib
COM library. Beside the fact that this library is not part of .NET,
referencing it in the project causes many warnings when rebuilding.

In order to get rid of these warnings, just unload al projects from the
"Speech" solution folder.

