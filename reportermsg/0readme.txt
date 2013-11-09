
This is developers' readme file for the ReporterMsg module.
Please refer to License_Reporter.html for licence information 
and to ReadMe_Reporter.html for general information.

    * TO DO:
  Decide the structure of ReportInfo functions! Maybe there should be variants
with an Exception argument to get information about file location. This would
make the following kind of calls possible:
    Rep.ReportInfo("My location","My message", new Exception (null));

  Update DefaultReport_TextLogger() and FormatLogMsgDefault() in suuch a way
that file location (file name & line) is output. 

  Check the Reporter.Finalize(). Try to make sure that the finalization
part is performed before the TextWriters are closed (now this isn't the case).

  Consider whether the txt some additional operations should be provided on
text loggers and textwriters. For example, RemoveAll? What about Remove without
arguments, should this remove all TextWriters or only the default one? 
Currently the way to remove the default one is by (...).SetTextLogger(""),
which is not comfortable for users.

  Go through all source codes and carefully check/update locks! 



