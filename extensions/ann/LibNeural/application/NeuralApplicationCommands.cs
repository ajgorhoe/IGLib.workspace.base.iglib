
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Lib;
using IG.Num;


namespace IG.Neural
{

    /// <summary>Commands for application's interpreter for neural networks.</summary>
    /// $A Igor Apr11;
    public class NeuralAllpicationCommands
    {


        /// <summary>Installs Tadejcommands for neural networks on the applications's command interpreter.</summary>
        public static void InstallCommands(ICommandLineApplicationInterpreter app)
        {
            if (app == null)
                throw new ArgumentNullException("Application command-line interpreter not specified (null reference).");
            app.AddCommand("ExampleNeural", CmdExampleNeural);

            app.AddCommand("TestJson", CmdTestJson);
            app.AddCommand("TestFormat", CmdTestFormat);
            app.AddCommand("SupplementNeuralDataDefinition", CmdSupplementNeuralDataDefinition);
            app.AddCommand("TestSerializationNeural", cmdTestSerializationNeural);
            app.AddCommand("TestSerializationNeuralCSV", cmdTestSerializationNeuralCSV);

            app.AddCommand("NeuralCalculateApproximationPlain", CmdNeuralCalculateApproximationPlain);
            app.AddCommand("NeuralCalculateApproximationServer", CmdNeuralCalculateApproximationServer);
            app.AddCommand("NeuralCalculateApproximationClient", CmdNeuralCalculateApproximationClient);
            app.AddCommand("NeuralClearMessages", CmdNeuralClearMessages);

            app.AddCommand("NeuralCalculateMappingApproximationServer", CmdNeuralCalculateMappingApproximationServer);
            app.AddCommand("NeuralCalculateMappingApproximationClient", CmdNeuralCalculateMappingApproximationClient);
        
        }


        #region NeuralGeneral


        /// <summary>Tests serialization and deserialization of various data used in approximation by
        /// neural networks.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// Tako78 7.Jul
        public static string cmdTestSerializationNeuralCSV(ICommandLineApplicationInterpreter interpreter, 
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine("Usage: ");
                        Console.WriteLine(executableName + " " + cmdName + " directoryPath ");
                        Console.WriteLine("  filePath: path to the file written in the specified format.");
                        Console.WriteLine("  numRows: number of matrix rows.");
                        Console.WriteLine("  numColumns: number of matrix columns.");
                        Console.WriteLine("  numElements: number of input/output elements.");
                        Console.WriteLine("A random matrix of specified dimension will be written to the specified file in CSV");
                        Console.WriteLine("format, then read from that file and written in another file.");
                        Console.WriteLine();
                        return null;
                    }

            string directory = "";
            bool printData = false;
            int inputLenght = 6;
            int outputLenght = 3;
            int numElements = 4;
            if (args.Length == 0)
                throw new ArgumentException("No argumets defined.");

            if (args.Length > 0)
                directory = args[0];
            if (args.Length > 1)
                printData = Convert.ToBoolean(args[1]);
            if (args.Length > 2)
                inputLenght = Convert.ToInt32(args[2]);
            if (args.Length > 3)
                outputLenght = Convert.ToInt32(args[3]);
            if (args.Length > 4)
                numElements = Convert.ToInt32(args[4]);

            string dirPath = Path.GetFullPath(directory);
            string filePath;

            filePath = Path.Combine(directory, "NeuralTrainingSet.csv");

            SampledDataSet trainingSet = SampledDataSet.CreateExampleSimple(inputLenght, outputLenght, numElements);
            InputOutputDataDefiniton dataDefinition = new InputOutputDataDefiniton();
            SampledDataSetDto trainingSetDto = new SampledDataSetDto();
            SampledDataSet trainingSetNew =new SampledDataSet();
            trainingSetDto.CopyFrom(trainingSet);
            SampledDataSet.SaveSampledDataCSV(filePath, trainingSet, false, false, false, null);
            SampledDataSet.LoadSampledDataCSV(filePath, inputLenght, outputLenght, false, false, false, ref trainingSetNew, ref dataDefinition);
            string filePathRestored;
            filePathRestored = Path.Combine(directory, "NeuralTrainingSet_restored.csv");
            SampledDataSet.SaveSampledDataCSV(filePathRestored, trainingSetNew, false, false, false, null);
            return null;
        }


        /// <summary>Tests serialization and deserialization of various data used in approximation by
        /// neural networks.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// Tako78 7.Jul
        public static string cmdTestSerializationNeural(ICommandLineApplicationInterpreter interpreter, 
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine("Usage: ");
                        Console.WriteLine(executableName + " " + cmdName + " directoryPath ");
                        Console.WriteLine("  filePath: path to the file written in the specified format.");
                        Console.WriteLine("  numRows: number of matrix rows.");
                        Console.WriteLine("  numColumns: number of matrix columns.");
                        Console.WriteLine("  numElements: number of input/output elements.");
                        Console.WriteLine("A random matrix of specified dimension will be written to the specified file in JSON");
                        Console.WriteLine("format, then read from that file and written in another file.");
                        Console.WriteLine();
                        return null;
                    }

            string directory = "";
            bool printData = false;
            int inputLenght = 6;
            int outputLenght = 3;
            int numElements = 4;
            double prepareDataTime = 0.0;
            double writeReadWriteTime = 0.0;
            double totalTime = 0.0;
            if (args.Length == 0)
                throw new ArgumentException("No argumets defined.");

            if (args.Length > 0)
                directory = args[0];
            if (args.Length > 1)
                printData = Convert.ToBoolean(args[1]);
            if (args.Length > 2)
                inputLenght = Convert.ToInt32(args[2]);
            if (args.Length > 3)
                outputLenght = Convert.ToInt32(args[3]);
            if (args.Length > 4)
                numElements = Convert.ToInt32(args[4]);
 

            string dirPath = Path.GetFullPath(directory);
            string filePath;
            StopWatch t = new StopWatch();

            t.Start();
            filePath = Path.Combine(directory, "NeuralTrainingSet.json");
            SampledDataSet trainingSet = SampledDataSet.CreateExampleSimple(inputLenght, outputLenght, numElements);
            SampledDataSetDto trainingSetDto = new SampledDataSetDto();
            trainingSetDto.CopyFrom(trainingSet);
            t.Stop();
            prepareDataTime = t.Time;
            
            if (printData)
            {
                Console.WriteLine("Neural network training set to be serialized: ");
                Console.WriteLine(trainingSetDto.ToString());
            }
            t.Start();
            SerializerBase.TestSerializationDto<SampledDataSetDto, SampledDataSet, SampledDataSet>(
                new SerializerJson(true),
                trainingSet,
                filePath,
                true /* firstStep */, true /* secondStep */);
            t.Stop();
            if (printData)
            {
                Console.WriteLine("Time for preparing training data: ");
                Console.WriteLine(prepareDataTime);
                writeReadWriteTime = t.Time;
                Console.WriteLine("Time for reading, writing and reading once again: ");
                Console.WriteLine(writeReadWriteTime);
                totalTime = t.TotalTime;
                Console.WriteLine("Total time: ");
                Console.WriteLine(totalTime);
            }
            
            return null;
        }


        /// <summary>Command.
        /// Tests speed of I/O in JSON format. A random matrix of specific size is randomly generated, read and written.
        /// Argumnets:
        ///   filePath: file path
        ///   numRows: number of rows of the matrix
        ///   numColumnt: number of columns of the matrix
        /// </summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Format of the file if test was performed, null otherwise.</returns>
        public static string CmdTestJson(ICommandLineApplicationInterpreter interpreter, string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine("Usage: ");
                        Console.WriteLine(executableName + " " + cmdName + " filePath numRows numColumns");
                        Console.WriteLine("  filePath: path to the file written in the specified format.");
                        Console.WriteLine("  numRows: number of matrix rows.");
                        Console.WriteLine("  numColumns: number of matrix columns.");
                        Console.WriteLine("A random matrix of specified dimension will be written to the specified file in JSON");
                        Console.WriteLine("format, then read from that file, and the times elapsed will be written.");
                        Console.WriteLine();
                        return null;
                    }
            Console.WriteLine();
            if (args.Length < 3)
            {
                Console.WriteLine("ERROR: ");
                Console.WriteLine("At least three arguments should be specified, the file path and matrix dimensions.");
                Console.WriteLine("Try " + cmdName + " ? for help.");
                return null;
            }
            string filePath = args[0];
            int numRows = int.Parse(args[1]);
            int numColumns = int.Parse(args[2]);
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("ERROR: file path not specified.");
                return null;
            }
            Console.WriteLine("Writing a random matrix of dimensions " + numRows + "x" + numColumns + " to JSON file");
            Console.WriteLine("and reading it back from the file...");
            StopWatch t = new StopWatch();
            t.Start();
            Matrix testMat = new Matrix(numRows, numColumns);
            testMat.SetRandom();
            t.Stop();
            double tGen = t.Time;
            Console.WriteLine("... matrix generated");
            t.Start();
            MatrixDtoBase testMatDto = new MatrixDtoBase();
            testMatDto.CopyFromObject(testMat);
            t.Stop();
            double tTranscript = t.Time;
            Console.WriteLine("... transcripted to DTO");
            t.Start();
            Matrix.SaveJson(testMat, filePath);
            t.Stop();
            double tSave = t.Time;
            Console.WriteLine("... saved to a file");
            t.Start();
            IMatrix testMatI = null;
            Matrix.LoadJson(filePath, ref testMatI);
            t.Stop();
            double tLoad = t.Time;
            Console.WriteLine("... read from a file");
            Console.WriteLine("Test done.");
            Console.WriteLine();
            Console.WriteLine("Generation time:    " + tGen + " s.");
            Console.WriteLine("Transcription time: " + tTranscript + " s.");
            Console.WriteLine("Saving time:        " + tSave + " s.");
            Console.WriteLine("Loading time:       " + tLoad + " s.");
            Console.WriteLine("Total time: " + t.TotalTime + " s");
            Console.WriteLine();
            return t.TotalTime.ToString();
        }


        // TEST OF DATA FORMATS:


        /// <summary>Command.
        /// Tests file formats. Attempsts to import the specified file in the specified format, then to export it under new name.
        /// Argumnets:
        ///   format: file format specification. Not case sensitive.
        ///       NeuralDataDefinition
        ///   filePath: file path
        /// </summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Format of the file if test was performed, null otherwise.</returns>
        public static string CmdTestFormat(ICommandLineApplicationInterpreter interpreter, string cmdName, string[] args)
        {
            const string formatTrainingSet = "NeuralTrainingSet";
            const string formatDataDefinition = "NeuralDataDefiniton";
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine("Usage: ");
                        Console.WriteLine(executableName + " " + cmdName + " format filePath");
                        Console.WriteLine("filePath: path to the file written in the specified format.");
                        Console.WriteLine("Attempt will be made to import the file and export it under modified name in the same directory.");
                        Console.WriteLine("In this way you can compare the contents of the original and imported/exported file.");
                        Console.WriteLine("If the contents do not match then there are errors in file format or import/export procedures.");
                        Console.WriteLine();
                        Console.WriteLine("Available formats (case does not matter): ");
                        Console.WriteLine("    " + formatTrainingSet + " - training data for neural networks.");
                        Console.WriteLine("    " + formatDataDefinition + " - definitions of neural network input and output parameters.");
                        return null;
                    }
            Console.WriteLine();
            if (args.Length < 2)
            {
                Console.WriteLine("ERROR: ");
                Console.WriteLine("At least two arguments should be specified, the format and the file path.");
                Console.WriteLine("Try " + cmdName + " ? for help.");
                return null;
            }
            string format = args[0];
            string filePath = args[1];
            bool knownFormat = false;
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("ERROR: file path not specified.");
                return null;
            }
            else if (!File.Exists(filePath))
            {
                Console.WriteLine("ERROR: file does not exist: ");
                Console.WriteLine("  " + filePath);
                return null;
            }
            else if (string.IsNullOrEmpty(format))
            {
                Console.WriteLine("ERROR: file format is not specified.");
                return null;
            }
            Console.WriteLine();
            Console.WriteLine("Trying to import a file and export it again...");
            Console.WriteLine();
            format = format.ToLower();
            if (format == formatTrainingSet.ToLower())
            {
                knownFormat = true;
                SerializerBase.TestSerializationDto<SampledDataSetDto, SampledDataSet, SampledDataSet>(
                new SerializerJson(true),
                null /* trainingSet */,
                filePath,
                false /* firstStep */, true /* secondStep */);
            }
            else if (format == formatDataDefinition.ToLower())
            {
                knownFormat = true;
                SerializerBase.TestSerializationDto<InputOutputDataDefinitonDto, InputOutputDataDefiniton, InputOutputDataDefiniton>(
                new SerializerJson(true),
                null /* trainingSet */,
                filePath,
                false /* firstStep */, true /* secondStep */);
            }
            if (knownFormat)
                Console.WriteLine(Environment.NewLine + "... Done.");
            else
            {
                Console.WriteLine("ERROR: Unknown file format: " + format);
                return null;
            }
            Console.WriteLine();
            return format;
        }


        // AUTOMATICALLY SUPPLEMENT NEURAL DATA DEFINITIONS FROM TRAINING DATA:


        /// <summary>Command method that supplements neural input/output data definitions from training data
        /// (some values are inferred from data ranges in training data).</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        public static string CmdSupplementNeuralDataDefinition(ICommandLineApplicationInterpreter interpreter, 
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " defFile trainingFile updatedDefFile : " + Environment.NewLine
                            + "supplements neural input/output data definitions from defFile with information obtained from training file trainingFile " + Environment.NewLine
                            + "and stores updated definitions to updatedDefFile. ");
                        Console.WriteLine(executableName + " " + cmdName + " defFile trainingFile : " + Environment.NewLine
                            + "supplements neural input/output data definitions from defFile with information obtained from training file trainingFile " + Environment.NewLine
                            + "and stores updated definitions back to defFile. ");
                        Console.WriteLine("WARNING: ");
                        Console.WriteLine("Data for which '...Defined' flag exists is not re-calculated by this command.");
                        Console.WriteLine("In order to overwrite such data, set the corrresponding flag in data definition file to false.");
                        Console.WriteLine();
                        return null;
                    }
            if (args.Length < 2)
                throw new ArgumentException("There should be at least 2 arguments.");
            else
            {
                string defFile = args[0];
                string trainingFile = args[1];
                string updatedFile = null;
                if (args.Length >= 3)
                    updatedFile = args[2];
                else
                    updatedFile = defFile;
                //Timer.Stop(); Timer.Start();
                InputOutputDataDefiniton.SupplementDataDefinition(defFile, trainingFile, updatedFile);
                //Timer.Stop();
            }
            return null;
        }


        #endregion NeuralGeneral


        #region ApproximationServer


        /// <summary>Command.
        /// Clears all messages for neural network approximation file client/server.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Null.</returns>
        public static string CmdNeuralClearMessages(ICommandLineApplicationInterpreter interpreter, string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " workingDirectory : Clears all messages for neural network approximation server.");
                        Console.WriteLine("  workingDirectory :  directory where data exchange and message files will reside.");
                        Console.WriteLine();
                        return null;
                    }
            Console.WriteLine();
            Console.WriteLine("Performing server-side calculation of neural network approximation...");
            Console.WriteLine("Input/output at standard locations.");
            string workingDirectory = null;
            if (args != null)
                if (args.Length > 0)
                    workingDirectory = args[0];
            if (string.IsNullOrEmpty(workingDirectory))
                throw new ArgumentNullException(cmdName + ": Working directory not specified.");
            ApproximationFileServerNeural approxServer = new ApproximationFileServerNeural(workingDirectory);
            Console.WriteLine();
            Console.WriteLine("Removing all messages from the neural approximation client/server directory...");
            approxServer.ClearMessages();
            Console.WriteLine("... removing messages done.");
            Console.WriteLine();
            return null;
        }


        /// <summary>Command.
        /// Calculates approximation according to input parameters written in a file, and outputs resulting
        /// approximated values to a file. This command loads a neural network from the specified file,
        /// reads input parameters from another file, and writes output parameters to the third file.
        /// The first argument is the neural network file, the second is file with input parameters, and the third is 
        /// file where approximated output values willl be written.</summary>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Null.</returns>



        /// <summary>Interpreter command.
        /// Calculates approximation according to input parameters written in a file, and outputs resulting
        /// approximated values to a file. This command loads a neural network from the specified file,
        /// reads input parameters from another file, and writes the calculated output values to the third file.
        /// The first argument is the neural network file, the second is file with input parameters, and the third is 
        /// file where approximated output values willl be written.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Null.</returns>
        public static string CmdNeuralCalculateApproximationPlain(ICommandLineApplicationInterpreter interpreter, 
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " neuralFile inputFile outputFile : Calculates approximated output values at given parameters and writes them to a file.");
                        Console.WriteLine("  neuralFile :  File containing the trained neural network.");
                        Console.WriteLine("  inputFile :   File in JSON where input parameters are obtained.");
                        Console.WriteLine("  outputFile :  Optional. File in JSON where output parameters are written.");
                        Console.WriteLine();
                        return null;
                    }
            string neuralFile = null;
            string inputFile = null;
            string outputFile = null;
            if (args != null)
            {
                if (args.Length > 0)
                    neuralFile = args[0];
                if (args.Length > 1)
                    inputFile = args[1];
                if (args.Length > 2)
                    outputFile = args[2];
            }
            if (neuralFile == null)
                throw new ArgumentNullException(cmdName + ": Working directory not specified.");
            if (inputFile == null)
                throw new ArgumentNullException(cmdName + ": Input file not specified.");
            Console.WriteLine();
            Console.WriteLine("Calculating approximated output values with the specified neural network...");
            INeuralApproximator approx = null;
            NeuralApproximatorBase.LoadJson(neuralFile, ref approx);
            Console.WriteLine("Neural network read from " + neuralFile + ".");
            IVector inputParameters = null, outputValues = null;
            Vector.LoadJson(inputFile, ref inputParameters);
            approx.CalculateOutput(inputParameters, ref outputValues);
            Console.WriteLine("... calculation done.");
            Console.WriteLine("Input parameters: " + Environment.NewLine + "  " + inputParameters);
            Console.WriteLine("Output values: " + Environment.NewLine + "  " + outputValues);
            if (!string.IsNullOrEmpty(outputFile))
            {
                Vector.SaveJson(outputValues, outputFile);
                Console.WriteLine("Output values written to " + outputFile + ".");
            }
            Console.WriteLine();
            //ApproximationFileServerNeural approxServer = new ApproximationFileServerNeural(workingDirectory);
            //// Read input parameters from a file:
            //approxServer.ClientTestCalculateApproximation(inputFile, outputFile);
            return null;
        }


        /// <summary>Command.
        /// Calculates approximation according to input parameters written in a file, and outputs resulting
        /// approximated values to a file.
        /// The only argument is the client/server working directory.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Null.</returns>
        public static string CmdNeuralCalculateApproximationServer(ICommandLineApplicationInterpreter interpreter, 
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " workingDirectory inFile outFile : Calculates approximated output values at given parameters (read from file) and writes them to a file.");
                        Console.WriteLine("  workingDirectory :  directory where data exchange and message files will reside.");
                        //Console.WriteLine("  inFile :  file containing input parameters.");
                        //Console.WriteLine("  outFile :  file where output parameters will be written.");
                        Console.WriteLine();
                        return null;
                    }
            Console.WriteLine();
            Console.WriteLine("Performing server-side calculation of neural network approximation...");
            Console.WriteLine("Input/output at standard locations.");
            string workingDirectory = null;
            if (args!=null)
                if (args.Length>0)
                    workingDirectory = args[0];
            if (string.IsNullOrEmpty(workingDirectory))
                throw new ArgumentNullException(cmdName + ": Working directory not specified.");
            ApproximationFileServerNeural approxServer = new ApproximationFileServerNeural(workingDirectory);
            // Because the calling application will probably not do this, we need to set the input ready flag:
            approxServer.NeuralFileManager.SetNeuralBusy();
            approxServer.NeuralFileManager.SetNeuralInputReady();
            // Now run teh calculation of the approximated values:
            approxServer.ServerCalculateApproximation();
            // Finally, clean some flags:
            approxServer.NeuralFileManager.ClearNeuralBusy();
            approxServer.NeuralFileManager.ClearNeuralOutputReady();
            Console.WriteLine("... server side calculation of neural network approximation done.");
            Console.WriteLine();
            return null;
        }


        /// <summary>Command.
        /// Sends request to a server to calculate an approximation.
        /// The first argument is the client/server working directory, the second is file with input parameters, and the third is 
        /// file where approximated output values willl be written</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Null.</returns>
        public static string CmdNeuralCalculateApproximationClient(ICommandLineApplicationInterpreter interpreter, 
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " workingDirectory optimizationinput output : Mapps and calculatesand approximated output values at given parameters (read from file) and writes them to a file.");
                        Console.WriteLine("  workingDirectory :  directory where data exchange and message files will reside.");
                        Console.WriteLine("  InputFile :  File in JSON where reduced input parameters are obtained.");
                        Console.WriteLine("  OutputFile :  Optional. File in JSON where reduced output parameters are written.");
                        Console.WriteLine();
                        return null;
                    }
            string workingDirectory = null;
            string inputFile = null;
            string outputFile = null;
            if (args != null)
            {
                if (args.Length > 0)
                    workingDirectory = args[0];
                if (args.Length > 1)
                    inputFile = args[1];
                if (args.Length > 2)
                    outputFile = args[2];
            }
            if (workingDirectory == null)
                throw new ArgumentNullException(cmdName + ": Working directory not specified.");
            if (inputFile == null)
                throw new ArgumentNullException(cmdName + ": Functioninput file not specified.");
            ApproximationFileServerNeural approxServer = new ApproximationFileServerNeural(workingDirectory);
            // Read input parameters from a file:
            approxServer.ClientTestCalculateApproximation(inputFile, outputFile);
            return null;
        }


        /// <summary>Command.
        /// Calculates mapping + approximation according to reduced input parameters written in a file, and outputs resulting
        /// approximated values to a file.
        /// The only argument is the client/server working directory.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Null.</returns>
        /// $A tako78 Jul.21
        public static string CmdNeuralCalculateMappingApproximationServer(ICommandLineApplicationInterpreter interpreter, 
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " workingDirectory optimizationinput output : Mapps and calculatesand approximated output values at given parameters (read from file) and writes them to a file.");
                        Console.WriteLine("  workingDirectory :  directory where data exchange and message files will reside.");
                        //Console.WriteLine("  inFile :  file containing input parameters.");
                        //Console.WriteLine("  outFile :  file where output parameters will be written.");
                        Console.WriteLine();
                        return null;
                    }
            Console.WriteLine();
            Console.WriteLine("Performing server-side mapping and calculation of neural network approximation...");
            Console.WriteLine("Input/output at standard locations.");
            string workingDirectory = null;
            if (args != null)
                if (args.Length > 0)
                    workingDirectory = args[0];
            if (string.IsNullOrEmpty(workingDirectory))
                throw new ArgumentNullException(cmdName + ": Working directory not specified.");
            MappingApproximationFileServerNeural mappApproxServer = new MappingApproximationFileServerNeural(workingDirectory);
            // Because the calling application will probably not do this, we need to set the input ready flag:
            mappApproxServer.NeuralFileManager.SetNeuralBusy();
            mappApproxServer.MappingFileManager.SetFunctionInputReady();
            mappApproxServer.NeuralFileManager.SetNeuralInputReady();
            // Now run teh calculation of the approximated values:
            mappApproxServer.ServerCalculateMappingApproximation();
            // Finally, clean some flags:
            mappApproxServer.NeuralFileManager.ClearNeuralBusy();
            mappApproxServer.MappingFileManager.ClearFunctionOutputReady();
            mappApproxServer.NeuralFileManager.ClearNeuralOutputReady();
            Console.WriteLine("... server side calculation of neural network approximation done.");
            Console.WriteLine("... data mapping done.");
            Console.WriteLine();

            return null;
        }


        /// <summary>Command.
        /// Sends request to a server to calculate mapping + approximation.
        /// The first argument is the client/server working directory, the second is file with reduced input parameters, and the third is 
        /// file where approximated reduced output values willl be written</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Null.</returns>
        /// $A tako78 Jul.21
        public static string CmdNeuralCalculateMappingApproximationClient(ICommandLineApplicationInterpreter interpreter, 
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " workingDirectory optimizationinput output : Mapps and calculatesand approximated output values at given parameters (read from file) and writes them to a file.");
                        Console.WriteLine("  workingDirectory :  directory where data exchange and message files will reside.");
                        Console.WriteLine("  functionInputFile :  File in JSON where reduced input parameters are obtained.");
                        Console.WriteLine("  functionOutputFile :  Optional. File in JSON where reduced output parameters are written.");
                        Console.WriteLine();
                        return null;
                    }
            string workingDirectory = null;
            string functionInputFile = null;
            string functionOutputFile = null;
            if (args != null)
            {
                if (args.Length > 0)
                    workingDirectory = args[0];
                if (args.Length > 1)
                    functionInputFile = args[1];
                if (args.Length > 2)
                    functionOutputFile = args[2];
            }
            if (workingDirectory == null)
                throw new ArgumentNullException(cmdName + ": Working directory not specified.");
            if (functionInputFile == null)
                throw new ArgumentNullException(cmdName + ": Functioninput file not specified.");
            MappingApproximationFileServerNeural mappApproxServer = new MappingApproximationFileServerNeural(workingDirectory);
            // Read input parameters from a file:
            mappApproxServer.ClientTestCalculateMappingApproximation(functionInputFile, functionOutputFile);

            return null;
        }


        #endregion ApproximationServer


        #region AnalysisServer


        #endregion AnalysisServer


        /// <summary>Command.
        /// Example command for Tadej.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Format of the file if test was performed, null otherwise.</returns>
        public static string CmdExampleNeural(ICommandLineApplicationInterpreter interpreter, 
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : Does nothing.");
                        Console.WriteLine();
                        return null;
                    }
            Console.WriteLine();
            Console.WriteLine("This is a test comand (neural application). It does nothing.");
            Console.WriteLine();
            return null;
        }

    }

}