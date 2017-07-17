// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// TESTING SCRIPT FILE: output formats for numbers.
// Original filename: ScriptExtFormats

using System;
using System.Collections.Generic;
using System.IO;

using IG.Num;
using IG.Lib;

namespace IG.Script
{

    /// <summary>Partial class definition containing tests of CSV utilities.
    /// <para>Original class location: ...\iglib\igbase\interpreters\scriptloader\scripts\ScriptAppBase.cs 
    /// (locate it with "Go to definition" on class name).</para></summary>
    partial class ScriptAppBase
    {

        
        /// <summary>Adds commands to the internal interpreter.
        /// <para>This is the part of adding commands in the partial class.</para></summary>
        /// <param name="interpreter">Interpreter where commands are executed.</param>
        /// <param name="helpStrings">List containg help strings.</param>
        public void Script_AddCommands1(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {

        }


        #region Actions.DataStructures_Partial

        
        /// <summary>Initializes commands for form demo related utilities (embedded applications).</summary>
        protected virtual void InitAppDataStructuresPartial()
        {

            lock (Lock)
            {
                // Add data structures commands from this class:
                AddDataStructuresCommand(DataStructuresTestCsvApp, DataStructuresFunctionTestCsvApp, DataStructuresHelpTestCsvApp);

                AddDataStructuresCommand(DataStructuresTestCsvWriteDefinitionAndData, DataStructuresFunctionTestCsvWriteDefinitionAndData, DataStructuresHelpTestCsvWriteDefinitionAndData);
                AddDataStructuresCommand(DataStructuresTestCsvReadDefinitionAndData, DataStructuresFunctionTestCsvReadDefinitionAndData, DataStructuresHelpTestCsvReadDefinitionAndData);
            }
        }


        #region Actions.DataStructures.TestCsv

        public const string DataStructuresTestCsvApp = "TestCsv";

        protected const string DataStructuresHelpTestCsvApp = DataStructuresTestCsvApp + " : Runs the CSV simple demo application.";

        /// <summary>Executes embedded application - demo application for demonstration of work with CSVs.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string DataStructuresFunctionTestCsvApp(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Runnning the CSV demo..." + Environment.NewLine);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new DataStructures());

            Console.WriteLine(Environment.NewLine + "CSV demo finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.DataStructures.TestCsv


        #region Actions.DataStructures.TestCsvWriteDefinitionAndData

        public const string DataStructuresTestCsvWriteDefinitionAndData = "CsvWriteDefinitionAndData";

        protected const string DataStructuresHelpTestCsvWriteDefinitionAndData = DataStructuresTestCsvWriteDefinitionAndData +
@" <defFile> <dataFile> <outFile> <sameRow> <indentation> ... : Tests writing of sampled data definition and data itself to CSV.
  defFile: path to JSON file that contains definitions of input and output data elements
  dataFile: JSON file that contains the data
  outFile: path to the file where data in CSV is written
  sameRow: if true then key and data are in the same row
  indentation: number of empty cells before data begins
    Relative paths are relative to the .../workspaceprojects/00tests/data/sampleddata directory
    when the workspaceprojects directory is defined (define its path with  the WORKSPACE 
    system variable!).";

        /// <summary>Executes embedded application - test of writing sampled data definition and data in CSV format.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.
        /// <para>1st argument: path to the data definition file in JSON.</para>
        /// <para>2nd argument: path to the data file in JSON.</para>
        /// <para>3rd argument: path to the output file where data in CSV is written.</para>
        /// <para>4th argument: whether key and data are in the same row.</para>
        /// <para>5th argument: indentation - number of empty cells before data begins.</para>
        /// <para>All arguments are optional. Relative paths are considered relative to the .../workspaceprojects/00tests/data/sampleddata 
        /// directory if the workspaceprojects directory can be determined (define the WORKSPACE variable for this).</para></param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string DataStructuresFunctionTestCsvWriteDefinitionAndData(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Runnning test: writing data definition & sampled data in CSV ..." + Environment.NewLine);
            string projectsDir = UtilSystem.GetWorkspaceProjectsDirectoryPath();
            bool isProjectsdirDefined = false;
            string dataDefinitionPath = null;
            string dataPath = null;
            string csvPath = null;
            bool keyAndDataInSameRow = false;
            int indentation = 0;
            if (!string.IsNullOrEmpty(projectsDir))
            {
                projectsDir = Path.Combine(projectsDir, "00tests/data/sampleddata");
                if (!string.IsNullOrEmpty(projectsDir))
                {
                    UtilSystem.StandardizeDirectoryPath(ref projectsDir);
                    if (!Directory.Exists(projectsDir))
                    {
                        throw new InvalidDataException("The project directory does not exist: " + Environment.NewLine
                            + "  " + projectsDir);
                        //projectsDir = null;
                    }
                }
                if (!string.IsNullOrEmpty(projectsDir))
                    isProjectsdirDefined = true;
            }
            if (!isProjectsdirDefined)
            {
                Console.WriteLine(Environment.NewLine + "WARNING: " + Environment.NewLine
                    + "Workspace projects directory is not defined, rel. paths are cosidered relative " + Environment.NewLine
                    + "  to the current directory and must be specified." + Environment.NewLine
                    + "  To define the projects directory path, set the WORKSPACE system variable.");
            }
            else
            {
                Console.WriteLine("Relative paths are considered relative to the following directory: " + Environment.NewLine
                    + "  " + projectsDir);
                dataDefinitionPath = Path.Combine(projectsDir, "neuraldatadefinition.json");
                dataPath = Path.Combine(projectsDir, "neuraltrainingdata.json");
                csvPath = Path.Combine(projectsDir, "testdata/DefinitionsAndData.csv");
            }
            string path;
            if (numArgs >= 1)
            {
                path = args[0];
                if (!string.IsNullOrEmpty(path))
                {
                    dataDefinitionPath = path;
                    if (isProjectsdirDefined && !Path.IsPathRooted(path))
                        dataDefinitionPath = Path.Combine(projectsDir, path);
                }
            }
            if (numArgs >= 2)
            {
                path = args[1];
                if (!string.IsNullOrEmpty(path))
                {
                    dataPath = path;
                    if (isProjectsdirDefined && !Path.IsPathRooted(path))
                        dataPath = Path.Combine(projectsDir, path);
                }
            }
            if (numArgs >= 3)
            {
                path = args[2];
                if (!string.IsNullOrEmpty(path))
                {
                    csvPath = path;
                    if (isProjectsdirDefined && !Path.IsPathRooted(path))
                        csvPath = Path.Combine(projectsDir, path);
                }
            }
            if (numArgs >= 4)
            {
                if (!string.IsNullOrEmpty(args[3]))
                    keyAndDataInSameRow = Util.ParseBoolean(args[3]);
            }
            if (numArgs >= 5)
            {
                if (!string.IsNullOrEmpty(args[4]))
                    indentation = int.Parse(args[4]);
            }
            if (string.IsNullOrEmpty(dataDefinitionPath) || !File.Exists(dataDefinitionPath))
            {
                throw new InvalidOperationException("Data definition JSON file not defined or nonexistent. " + Environment.NewLine
                    + "  Path: \"" + dataDefinitionPath + "\"" + Environment.NewLine
                    + "  Try to define the WORKSPACE system variable containing the workspace directory path!");
            }
            if (string.IsNullOrEmpty(dataPath) || !File.Exists(dataPath))
            {
                throw new InvalidOperationException("Data JSON file not defined or nonexistent. " + Environment.NewLine
                    + "  Path: \"" + dataPath + "\"" + Environment.NewLine
                    + "  Try to define the WORKSPACE system variable containing the workspace directory path!");
            }
            if (string.IsNullOrEmpty(csvPath))
            {
                throw new InvalidOperationException("CSV result file (definition & data) not specified. " + Environment.NewLine
                    + "  Try to define the WORKSPACE system variable containing the workspace directory path!");
            }
            Console.WriteLine(Environment.NewLine
                + "File paths: "
                + "  Data definition: " + dataDefinitionPath + Environment.NewLine
                + "  Data           : " + dataPath + Environment.NewLine
                + "  Result         : " + csvPath + Environment.NewLine);
            InputOutputDataDefiniton dataDefinition = null;
            SampledDataSet sampledData = null;
            Console.Write("Loading definition data... ");
            InputOutputDataDefiniton.LoadJson(dataDefinitionPath, ref dataDefinition);
            Console.WriteLine("  ... done.");
            Console.Write("Loading sampled data... ");
            SampledDataSet.LoadJson(dataPath, ref sampledData);
            Console.WriteLine("  ... done.");
            if (dataDefinition == null)
                throw new ArgumentException("Data definitions could not be read correctly.");
            else
            {
                Console.WriteLine(
                      "Number of input parameters: " + dataDefinition.InputLength + Environment.NewLine
                    + "Number of joutput values:   " + dataDefinition.OutputLength + Environment.NewLine);
                //Console.WriteLine(Environment.NewLine + "Data definitions: " + Environment.NewLine
                //    + dataDefinition.ToString() + Environment.NewLine);
            }
            if (sampledData == null)
                throw new ArgumentException("Sampled data could not be read correctly.");
            Console.WriteLine(Environment.NewLine +
                "Saving data definitions & sampled data to a CSV file... ");
            StopWatch1 t = new StopWatch1();
            t.Start();
            SampledDataCsv csv = new SampledDataCsv();
            csv.OutputLevel = 5;
            csv.DataDefinition = dataDefinition;

            //Console.WriteLine(Environment.NewLine + "Data definitions on CSV: " + Environment.NewLine
            //    + csv.DataDefinition.ToString() + Environment.NewLine);

            csv.SampledData = sampledData;

            csv.KeyAndDataInSameRow = keyAndDataInSameRow;
            csv.Indentation = indentation;

            csv.SaveDefinitionAndData(csvPath);
            t.Stop();
            Console.WriteLine(Environment.NewLine
                + "   ... saving CSV done in " + t.TotalTime + " s.");
            Console.WriteLine(Environment.NewLine + "CSV definition & data writing demo finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.DataStructures.TestCsvWriteDefinitionAndData


        #region Actions.DataStructures.TestCsvReadDefinitionAndData

        public const string DataStructuresTestCsvReadDefinitionAndData = "CsvReadDefinitionAndData";

        protected const string DataStructuresHelpTestCsvReadDefinitionAndData = DataStructuresTestCsvReadDefinitionAndData +
@" <inputFile> <outputFile> <sameRow> <indentation> ... : Tests reading of sampled data definition and data itself from CSV.
  inputFile: path to CSV file that contains definitions and data to be read (default: neuraldefinitionanddata.csv)
  outputFile: path to the file where the read-in definitions and data will be written in CSV (default: testdata/ReadAndWritten.csv)
  sameRow: if true then key and data are in the same row when writing the data
  indentation: number of empty cells before data begins when writing the data
    Relative paths are relative to the .../workspaceprojects/00tests/data/sampleddata directory
    when the workspaceprojects directory is defined (define its path with  the WORKSPACE 
    system variable!).";

        /// <summary>Executes embedded application - test of writing sampled data definition and data in CSV format.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="appArgs">Arguments fo the embedded application's command.
        /// <para>1st argument: path to CSV file that contains definitions and data to be read.</para>
        /// <para>2nd argument: path to the file where the read-in definitions and data will be written (in CSV).</para>
        /// <para>3rd argument: if true then key and data are in the same row when writing the data.</para>
        /// <para>4th argument: indentation - number of empty cells before data begins when writing the data.</para>
        /// <para>All arguments are optional. Relative paths are considered relative to the .../workspaceprojects/00tests/data/sampleddata 
        /// directory if the workspaceprojects directory can be determined (define the WORKSPACE variable for this).</para></param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string DataStructuresFunctionTestCsvReadDefinitionAndData(string appName, string[] appArgs)
        {
            int numArgs = 0;
            if (appArgs != null)
                numArgs = appArgs.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Runnning test: writing data definition & sampled data in CSV ..." + Environment.NewLine);
            string projectsDir = UtilSystem.GetWorkspaceProjectsDirectoryPath();
            bool isProjectsdirDefined = false;
            string inputPath = null;
            string outputPath = null;
            bool keyAndDataInSameRow = false;
            int indentation = 0;
            if (!string.IsNullOrEmpty(projectsDir))
            {
                projectsDir = Path.Combine(projectsDir, "00tests/data/sampleddata");
                if (!string.IsNullOrEmpty(projectsDir))
                {
                    UtilSystem.StandardizeDirectoryPath(ref projectsDir);
                    if (!Directory.Exists(projectsDir))
                    {
                        throw new InvalidDataException("The project directory does not exist: " + Environment.NewLine
                            + "  " + projectsDir);
                        //projectsDir = null;
                    }
                }
                if (!string.IsNullOrEmpty(projectsDir))
                    isProjectsdirDefined = true;
            }
            if (!isProjectsdirDefined)
            {
                Console.WriteLine(Environment.NewLine + "WARNING: " + Environment.NewLine
                    + "Workspace projects directory is not defined, rel. paths are cosidered relative " + Environment.NewLine
                    + "  to the current directory and must be specified." + Environment.NewLine
                    + "  To define the projects directory path, set the WORKSPACE system variable.");
            }
            else
            {
                Console.WriteLine("Relative paths are considered relative to the following directory: " + Environment.NewLine
                    + "  " + projectsDir);
                inputPath = Path.Combine(projectsDir, "neuraldefinitionanddata.csv");
                outputPath = Path.Combine(projectsDir, "testdata/ReadAndWritten.csv");
            }
            string path;
            if (numArgs >= 1)
            {
                path = appArgs[0];
                if (!string.IsNullOrEmpty(path))
                {
                    inputPath = path;
                    if (isProjectsdirDefined && !Path.IsPathRooted(path))
                        inputPath = Path.Combine(projectsDir, path);
                }
            }
            if (numArgs >= 2)
            {
                path = appArgs[1];
                if (!string.IsNullOrEmpty(path))
                {
                    outputPath = path;
                    if (isProjectsdirDefined && !Path.IsPathRooted(path))
                        outputPath = Path.Combine(projectsDir, path);
                }
            }
            if (numArgs >= 3)
            {
                if (!string.IsNullOrEmpty(appArgs[2]))
                    keyAndDataInSameRow = Util.ParseBoolean(appArgs[2]);
            }
            if (numArgs >= 4)
            {
                if (!string.IsNullOrEmpty(appArgs[3]))
                    indentation = int.Parse(appArgs[3]);
            }
            if (string.IsNullOrEmpty(inputPath) || !File.Exists(inputPath))
            {
                throw new InvalidOperationException("Input definition & data CSV file not defined or nonexistent. " + Environment.NewLine
                    + "  Path: \"" + inputPath + "\"" + Environment.NewLine
                    + "  Try to define the WORKSPACE system variable containing the workspace directory path!");
            }
            if (string.IsNullOrEmpty(outputPath))
            {
                throw new InvalidOperationException("Output CSV file not defined. " + Environment.NewLine
                    + "  Try to define the WORKSPACE system variable containing the workspace directory path!");
            }
            Console.WriteLine(Environment.NewLine
                + "File paths: "
                + "  Input CSV file : " + inputPath + Environment.NewLine
                + "  Output CSV file: " + outputPath + Environment.NewLine);
            InputOutputDataDefiniton dataDefinition = null;
            SampledDataSet sampledData = null;
            Console.Write("Loading definition and data from CSV file... ");
            StopWatch1 t = new StopWatch1();
            t.Start();
            SampledDataCsv csv = new SampledDataCsv();
            csv.OutputLevel = 5;
            csv.LoadDefinitionAndData(inputPath);
            t.Stop();
            dataDefinition = csv.DataDefinition;
            sampledData = csv.SampledData;
            Console.WriteLine("  ... done.");
            if (dataDefinition == null)
                Console.WriteLine(Environment.NewLine + "ERROR: " + Environment.NewLine
                    + "Data definitions were not read correctly." + Environment.NewLine);
            else
            {
                Console.WriteLine(
                      "Number of input parameters: " + dataDefinition.InputLength + Environment.NewLine
                    + "Number of joutput values:   " + dataDefinition.OutputLength + Environment.NewLine);
            }
            if (sampledData == null)
                Console.WriteLine(Environment.NewLine + "ERROR: " + Environment.NewLine
                    + "Sampled data could not be read correctly." + Environment.NewLine);
            Console.WriteLine(Environment.NewLine +
                "Saving data definitions & sampled data to the control CSV file... ");
            t.Start();
            csv = new SampledDataCsv();
            csv.OutputLevel = 5;
            csv.DataDefinition = dataDefinition;
            csv.SampledData = sampledData;
            csv.KeyAndDataInSameRow = keyAndDataInSameRow;
            csv.Indentation = indentation;
            csv.SaveDefinitionAndData(outputPath);
            t.Stop();
            Console.WriteLine(Environment.NewLine
                + "   ... saving CSV done in " + t.Time + " s.");
            Console.WriteLine(Environment.NewLine
                + "File paths: "
                + "  Input CSV file : " + inputPath + Environment.NewLine
                + "  Output CSV file: " + outputPath + Environment.NewLine);
            Console.WriteLine(Environment.NewLine + "CSV definition & data reading demo finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.DataStructures.TestCsvReadDefinitionAndData



        #endregion Actions.DataStructures_Partial



    } // class ScriptAppBase



}
