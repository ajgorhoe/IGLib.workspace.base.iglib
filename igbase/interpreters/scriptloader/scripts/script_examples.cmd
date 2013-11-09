
C This script contains some examples of running commands form built in scripts.

C C Get information about the installed commands:
C Internal IG.Script.ScriptAppBase ?



C ==============================================
C DATA STRUCTURES related embedded applications:

C Internal IG.Script.AppBase DataStructures ?

C C Run the CSV demo:
C Internal IG.Script.AppBase DataStructures TestCsv

C C Writing data definitions and data in CSV:
C C Internal IG.Script.AppBase DataStructures CsvWriteDefinitionAndData 
C C     <definitionFile> <dataFile> <csvResultFile> <sameRow> <indentation>
C Internal IG.Script.AppBase DataStructures CsvWriteDefinitionAndData "" "" "" false 2

C C Reading data definitions and data in CSV:
C C Internal IG.Script.AppBase DataStructures CsvReadDefinitionAndData 
C C     <inputCsvFile> <outputCsvFile> <sameRow> <indentation>
C Internal IG.Script.AppBase DataStructures CsvReadDefinitionAndData "" "" false 0



C ==============================================
C NUMERICS related embedded applications:


C ==============================================
C BASIC NUMERICS:

C C Scalar function objects defined through string expressions:
C Internal IG.Script.AppBase Numerics ScriptScalarFunction



C ==============================================


