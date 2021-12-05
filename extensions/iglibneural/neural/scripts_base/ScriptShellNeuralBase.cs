// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;


#if NETFRAMEWORK
using AsyncResult = System.Runtime.Remoting.Messaging.AsyncResult;
#endif

using IG.Lib;
using IG.Num;
using IG.Neural;



namespace IG.Script
{


    /// <summary>Base class for loadable scripts that are used as custom applications 
    /// that inclued neural networks approximations and optimization.</summary>
    /// <remarks><para>This is an abstract class. Derived class <see cref="LoadableScriptShellNeural"/>
    /// serves the same purpose and is concrete (but some implementations just throw exceptions).</para></remarks>
    /// $A Igor xx Jul11 Dec12 Feb12;
    public abstract class LoadableScriptShellNeuralBase : LoadableScriptShellBase, 
        ILoadableScript, INeuralModel
    {


        // public string BaseDirectory = "train";

#region Initialization

        public LoadableScriptShellNeuralBase()
            : base()
        {  }


        /// <summary>Inializes the current script object.</summary>
        /// <param name="arguments">Initialization arguments.
        /// The first argument must be the working directory path.</param>
        protected override void InitializeThis(string[] arguments)
        {
            base.InitializeThis(arguments);  // this sets the working directory to the first argument
        }

#endregion Initialization
        


#region NeuralApproximatorDependencies

        // in this region we handle the code that depends on the kind of neural network approxomator.

        /// <summary>Loads the neural network approximator from the specified file.
        /// <para>In the derived classes, this method can be overridden in order to handle more types
        /// of neurel network approximators.</para></summary>
        /// <param name="path">File path.</param>
        /// <param name="network">Neual network into which loaded data is restored.</param>
        protected virtual void LoadJson(string path, ref INeuralApproximator network)
        {
            // NeuralApproximatorBaseExt.LoadJson(path, ref network);
            NeuralApproximatorBase.LoadJson(path, ref network);
        }

        /// <summary>Saves the specified trained network to a file.
        /// <para>This method hould be </para></summary>
        /// <param name="approximator">Trained neural network to be  saved.</param>
        /// <param name="trainedNetworkFilePath">Path of the file into which trained neural network is saved in JSON form.</param>
        protected virtual void SaveJson(INeuralApproximator approximator, string trainedNetworkFilePath)
        {
            // NeuralApproximatorBaseExt.SaveJson(approximator, trainedNetworkFilePath);
            NeuralApproximatorBase.SaveJson(approximator, trainedNetworkFilePath);
        }


        /// <summary>Creates and returns a new neural network approximator, with the basic properties pre-set to 
        /// default values, dependent on the type of the requires approximator.
        /// <para>This methods should be overridden in derived classes in order to allow use of other neural approximatiors.</para></summary>
        /// <param name="annType">Type of the neural network approximator:
        /// <para>  1 - NeuralApproximatorNeuron (only availeble in more specialized libraries for internal use).</para>
        /// <para>  2 - NeuralApproximatorAforge (basic neural network approximator in this library).</para></param>
        protected virtual INeuralApproximator CreateApproximator(int annType = 2  /* 2 - approximatorAforge */)
        {
            INeuralApproximator approximator = null;

            double lowerInputRange = -2.0;
            double upperInputRange = 2.0;
            double lowerOutputRange = 0.0;
            double upperOutputRange = 1.0;


            //if (annType == 1) // NeuronDotNet
            //{
            //    approximator = new NeuralApproximatorNeuron();
            //    lowerInputRange = -2.0;
            //    upperInputRange = 2.0;
            //    lowerOutputRange = 0.0;
            //    upperOutputRange = 1.0;
            //    approximator.SigmoidAlphaValue = 2;
            //}
            //else 
            if (annType == 2) // Aforge
            {
                approximator = new NeuralApproximatorAforge();
                lowerInputRange = -2.0;
                upperInputRange = 2.0;
                lowerOutputRange = -1.0;
                upperOutputRange = 1.0;
                approximator.SigmoidAlphaValue = 1.3;
            }

            // Change the targeted range of input and otput neuraons: 
            approximator.InputNeuronsRange.Reset();
            approximator.InputNeuronsRange.UpdateAll(lowerInputRange, upperInputRange);
            approximator.OutputNeuronsRange.Reset();
            approximator.OutputNeuronsRange.UpdateAll(lowerOutputRange, upperOutputRange);


            return approximator;
        }


#endregion NeuralApproximatorDependencies

        



#region DummyImplementations

        /// <summary>Dummy analysis, jsut throws the  exception.</summary>
        /// <param name="anRes"></param><see cref="NotImplementedException"/>
        public override void Analyse(Num.IAnalysisResults anRes)
        {
            throw new NotImplementedException("The analysis method is not implemeted by this class (" + this.GetType().Name + ").");
        }

#endregion DummyImplementations


#region Commands

        /// <summary>Prints internal command being executed and its actual arguments</summary>
        /// <param name="arguments">Script run arguments, the first one is command name.</param>
        protected virtual void PrintInternalCommandAndArguments(string[] arguments)
        {
            Console.WriteLine();
            if (arguments == null)
                Console.WriteLine("Array of arguments is NOT SPECIFIED (null reference)!");
            else if (arguments.Length < 1)
                Console.WriteLine("No arguments are specified (args. array has no elements).");
            else
            {
                Console.WriteLine("EXECUTING COMMAND: " + arguments[0]);
                Script_PrintArguments("Script command's arguments: ", arguments);
                Console.WriteLine();
            }
        }

        /// <summary>Runs a custom basic test.
        /// <para>In its current variants, it jus prints some data about important directories, etc.</para></summary>
        /// <param name="arguments">Command arguments, first one (index 0) is always command name.</param>
        /// <returns>null.</returns>
        public virtual string Test(string[] arguments)
        {
            PrintInternalCommandAndArguments(arguments);

            Console.WriteLine();
            Console.WriteLine("Working directory: " + Environment.NewLine + "  " +  WorkingDirectory);
            Console.WriteLine("Optimization directory: " + Environment.NewLine + "  " + OptimizationDirectory);
            string trainingDataPath = NeuralFM.NeuralTrainingDataPath;
            Console.WriteLine("Path of training data file: "
                + Environment.NewLine + "  " + trainingDataPath);
            return null;
        }


        /// <summary>Runs a custom test.</summary>
        /// <param name="arguments">Command arguments, first one (index 0) is always command name.</param>
        /// <returns>null.</returns>
        /// <remarks>This command is reserved for quick custom tests whose code will change frequently.
        /// In general, you should override the command code in the derived classes if you need this 
        /// functionality.</remarks>
        public virtual string Custom(string[] arguments)
        {
            PrintInternalCommandAndArguments(arguments);

            Console.WriteLine();
            Console.WriteLine("====");
            Console.WriteLine("CUSTOM TEST.");
            Console.WriteLine("Working directory: " + Environment.NewLine + "  " + WorkingDirectory);
            Console.WriteLine("Optimization directory: " + Environment.NewLine + "  " + OptimizationDirectory);
            Console.WriteLine();

            bool testArrayDto = true;  // test of saving arrays of DTOs to JSON - in particular for NeuralTrainingParameters:
            if (testArrayDto)
            {
                bool testTrainingParameters = true;
                bool testOneObject = false;

                string filePath = Path.Combine(OptimizationDirectory, "test.json");
                bool append = false;

                if (testTrainingParameters)
                {
                    if (testOneObject)
                    {
                        NeuralTrainingParameters param = new NeuralTrainingParameters();
                        param.LearningRate = 56789.0;
                        NeuralTrainingParametersDto dtoOriginal = new NeuralTrainingParametersDto();
                        dtoOriginal.CopyFrom(param);
                        ISerializer serializer = new SerializerJson();
                        serializer.Serialize<NeuralTrainingParametersDto>(dtoOriginal, filePath, append);
                    }
                    else
                    {
                        NeuralTrainingParameters[] trainingParameters
                            = new NeuralTrainingParameters[]{
                            new NeuralTrainingParameters() , new NeuralTrainingParameters()
                        };

                        trainingParameters[0].LearningRate = 0.12345;

                        ArrayDto<NeuralTrainingParameters, NeuralTrainingParameters, NeuralTrainingParametersDto> dtoOriginal =
                        new ArrayDto<NeuralTrainingParameters, NeuralTrainingParameters, NeuralTrainingParametersDto>();
                        dtoOriginal.CopyFrom(trainingParameters);
                        ISerializer serializer = new SerializerJson();
                        serializer.Serialize<ArrayDto<NeuralTrainingParameters, NeuralTrainingParameters, NeuralTrainingParametersDto>>(dtoOriginal, filePath, append);
                    }
                }
                else
                {
                    if (testOneObject)
                    {
                        Vector param = new Vector(new double[] { 11.1, 11.2, 11.3, 11.4, 11.5 });
                        VectorDto dtoOriginal = new VectorDto();
                        dtoOriginal.CopyFrom(param);
                        ISerializer serializer = new SerializerJson();
                        serializer.Serialize<VectorDto>(dtoOriginal, filePath, append);
                    }
                    else
                    {
                        Vector[] storedVectors = new Vector[]
                            {
                                new Vector(new double[]{1.1, 1.2}), 
                                new Vector(new double[]{2.1, 2.2, 2.3, 2.4})
                            };
                        ArrayDto<Vector, IVector, VectorDto> dtoOriginal =
                            new ArrayDto<Vector, IVector, VectorDto>();

                        dtoOriginal.CopyFrom(storedVectors);

                        ISerializer serializer = new SerializerJson();
                        serializer.Serialize<ArrayDto<Vector, IVector, VectorDto>>(dtoOriginal, filePath, append);
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("====");
            return null;
        }

        // PARALLEL SIMULATIONS 

        /// <summary>Runs multiple simulations in parallel threads and saves results in each parallel thread.</summary>
        /// <param name="arguments">Command arguments, first one (index 0) is always command name.</param>
        /// <returns>null.</returns>
        public virtual string RunParallelSimulations(string[] arguments)
        {
            Console.WriteLine("Command RunParallel...");
            Script_PrintArguments("Script command's arguments: ", arguments);
            Console.WriteLine();
            if (arguments.Length > 1)
            {
                ParSimNumThreads = int.Parse(arguments[1]);
                Console.WriteLine("Number of threads set to " + ParSimNumThreads + ".");
            }
            if (arguments.Length > 2)
            {
                ParSimNumRuns = int.Parse(arguments[2]);
                Console.WriteLine("Number of runs set to " + ParSimNumRuns + ".");
            }
            if (arguments.Length > 3)
            {
                ParSimProcessDelayMilliseconds = int.Parse(arguments[3]);
                Console.WriteLine("Delay in nmilliseconds: " + ParSimProcessDelayMilliseconds + ".");
            }
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("Preparing data for parallel simulations in " + ParSimNumThreads + "threads...");
            ParSimPrepareDirectories(ParSimNumThreads);
            Console.WriteLine("... preparation done.");
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("Beginning parallel simulations in " + ParSimNumThreads + "threads...");
            ParSimBegin(ParSimNumThreads);
            Console.WriteLine();
            Console.WriteLine("Parallel threads started, waiting for results...");
            Console.WriteLine();
            ParSimWaitAllCompletion();

            return null;
        }


        /// <summary>Collects results of simulations run in parallel threads, joins them into a single object and saves them.</summary>
        /// <param name="arguments">Command arguments.</param>
        /// <returns>null.</returns>
        public virtual string CollectParallelSimulationResults(string[] arguments)
        {
            Console.WriteLine("Command CollectParallel...");
            Script_PrintArguments("Script command's arguments: ", arguments);
            Console.WriteLine();
            if (arguments.Length > 1)
            {
                ParSimNumThreads = int.Parse(arguments[1]);
                Console.WriteLine("Number of threads set to " + ParSimNumThreads + ".");
            }
            if (arguments.Length > 2)
            {
                ParSimNumRuns = int.Parse(arguments[2]);
                Console.WriteLine("Number of runs set to " + ParSimNumRuns + ".");
            }
            Console.WriteLine();


            GatherParallelResults(ParSimNumThreads);

            return null;
        }




        /// <summary>Comamnd name for test.</summary>
        public const string ConstTest = "Test";
        public const string ConstHelpTest = "Test of functionality.";

        /// <summary>Comamnd name for test.</summary>
        public const string ConstCustom = "Custom";
        public const string ConstHelpCustom = "Custom command that can be quickly modified as needed, for performing.";

        /// <summary>Comamnd name for running parallel simulations.</summary>
        public const string ConstRunParallel = "RunParallel";
        public const string ConstHelpRunParallel = "Runs parallel simulations. Args: numThreads numRunsPerThread";

        /// <summary>Comamnd name for running parallel simulations.</summary>
        public const string ConstCollectParallel = "collectparallel";
        public const string ConstHelpCollectParallel = "Collects results of parallel simulations. Args: numThreads";


        /// <summary>Comamnd name for testing network response for centered  point from verification or training points for all Inputs/outputs.</summary>
        public const string ConstCreateDistortedModelData = "CreateDistortedModelData";
        public const string ConstHelpCreateDistortedModelData = ConstCreateDistortedModelData +
@" WorkingDirectory DistortedModelDirectory ChangeElementNames ChangeTitlesAndDescriptions DataDefinitionFile 
  - creates data definition and data files for a model that is obtained by distortion 
  of the specified original model.Parameters:
    WorkingDirectory - working directory where the original model is found.
    DistortedModelDirectory - directory where distorted model is stored.
    NumDataPoints - number of data points in the sampled data generated by the original model.
    ChangeElementNames - optional; whether data element names are changed.
    ChangeTitlesAndDescriptions - optional; whether element titles and descriptions are changed.
    DataDefinitionFile - optinal; data definition file for distorted model. If specified then new parameter 
        bounds and other data (names, titles, descriptions, etc.) are taken from here.
    DistortionFactor - optional, default 2.5 - factor by which bounds are multiplied.
    RandomFactor - ouptional, default 0.25 - mahnitude of additional random shiht of bounds 
        (relative to distorted interval length).
";

        /// <summary>Adds commands to the internal interpreter.</summary>
        /// <param name="interpreter">Interpreter where commands are executed.</param>
        /// <param name="helpStrings">List containg help strings.</param>
        public override void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            base.Script_AddCommands(interpreter, helpStrings);
            Script_AddCommand(interpreter, helpStrings, ConstTest, Test, ConstHelpTest);
            Script_AddCommand(interpreter, helpStrings, ConstCustom, Custom, ConstHelpCustom);
            Script_AddCommand(interpreter, helpStrings, ConstRunParallel, RunParallelSimulations, ConstHelpRunParallel);
            Script_AddCommand(interpreter, helpStrings, ConstCollectParallel, CollectParallelSimulationResults, ConstHelpCollectParallel);
            Script_AddCommand(interpreter, helpStrings, ConstCreateDistortedModelData, CreateDistortedModelData, ConstHelpCreateDistortedModelData);
            //Script_AddCommand(interpreter, helpStrings, , , );
        }


#endregion Commands


#region Tests

        /// <summary>Performs a test approximation at the specified vector of parameters and outputs and returns results.</summary>
        /// <param name="parameters">Parameters at which approximation output vector is calculated.</param>
        /// <returns>Vector of approximated output values.</returns>
        public virtual IVector TestApproximation(IVector parameters)
        {
            IVector outputValues = new Vector(NumNeuralOutputs, 0.0);
            StopWatch1 t = new StopWatch1();
            Console.WriteLine();
            Console.WriteLine("******");
            Console.WriteLine("Performing approximation...");
            Console.WriteLine("Input parameters: " + Environment.NewLine + "  " + parameters.ToString());
            t.Start();
            NeuralCalculate(parameters, ref outputValues);
            t.Stop();
            Console.WriteLine("Approximation results:");
            Console.WriteLine(outputValues.ToString());
            Console.WriteLine("Calculation time: " + t.Time + " s, " + t.CpuTime + " s CPU.");
            Console.WriteLine("**");
            return outputValues;
        }

        /// <summary>Calculates a table of specified number of approximations, with parameters running between
        /// two points that are chosen by the algorithm.</summary>
        /// <param name="numPoints">Number of points in which approximation results results are calculated.</param>
        /// <returns>Array of two arrays - of input vector and of calculated approximated outputs (in form of vectors).</returns>
        public virtual IVector[][] TestApproximationTable(int numPoints)
        {
            IVector parNeural1 = null, parNeural2 = null, param1 = null, param2 = null;
            GetNeuralInputVector(0.2, ref parNeural1);
            GetNeuralInputVector(0.8, ref parNeural2);
            return TestApproximationTable(parNeural1, parNeural2, numPoints);
        }

        /// <summary>Performs a table of approximatinos between two specified vectors of approximation input parameters, 
        /// and outputs results. It also returns the table of results (approximated output values in form of vectors).
        /// <para>Approximations are performed in specified number of equidistant points between the two specified vectors of input parameters.</para></summary>
        /// <param name="param1">The first vector of approximation input parameters.</param>
        /// <param name="param2">The second vector of approximation input parameters.</param>
        /// <param name="numPoints">Number of points where calculations (approximations) are perfomed, must be at least two.</param>
        /// <returns>Array of two arrays - of input vector and of calculated approximated outputs (in form of vectors).</returns>
        public virtual IVector[][] TestApproximationTable(IVector param1, IVector param2, int numPoints)
        {
            if (param1 == null)
                throw new ArgumentException("First vector of parameters not specified.");
            else if (param2 == null)
                throw new ArgumentException("Second vector of parameters not specified.");
            else if (numPoints < 2)
                throw new ArgumentException("Number of points of calculation should be at least two; now the number is " + numPoints);
            StopWatch1 t = new StopWatch1();
            Console.WriteLine();
            Console.WriteLine("******************** Table of approximation results:");
            Console.WriteLine("Calculating a table of " + numPoints + " approximation output vectors between two points...");
            Console.WriteLine("First vector:");
            Console.WriteLine("  " + param1);
            Console.WriteLine("Second vector:");
            Console.WriteLine("  " + param1);
            Console.WriteLine("Calculating...");
            Console.WriteLine();
            IVector[] outputs = new IVector[numPoints];
            IVector[] inputs = new IVector[numPoints];
            IVector step = new Vector(param1.Length);
            Vector.Subtract(param2, param1, step);
            Vector.Multiply(step, 1.0 / ((double)numPoints - 1.0), step);
            IVector current = new Vector(param1.Length);
            Vector.Copy(param1, current);
            t.Start();
            for (int i = 0; i < numPoints; ++i)
            {
                Console.WriteLine();
                Console.WriteLine("Performing analysis No. " + i + "...");
                Vector param = new Vector(current);
                outputs[i] = TestApproximation(param);
                inputs[i] = param;
                Vector.Add(current, step, current);
            }
            t.Stop();
            Console.WriteLine();
            Console.WriteLine("... calculation finished.");
            Console.WriteLine();
            Console.WriteLine("Interval factors: ");
            for (int i = 0; i < numPoints; ++i)
            {
                Console.WriteLine(i + ": t = " + ((double)i / ((double)numPoints - 1.0)));
            }
            Console.WriteLine("Approximation input parameters:");
            for (int i = 0; i < numPoints; ++i)
            {
                Console.WriteLine("  " + i + ": " + inputs[i]);
            }
            Console.WriteLine("Approximation output values:");
            for (int i = 0; i < numPoints; ++i)
            {
                Console.WriteLine("  " + i + ": " + outputs[i]);
            }
            Console.WriteLine("Calculation time:  " + t.Time + " s, CPU: " + t.CpuTime + " s.");
            Console.WriteLine("**** End of analysis table.");
            Console.WriteLine();
            return new IVector[][] { inputs, outputs };
        } // TestanalysisTable()


        /// <summary>Performs test analysis at the specified optimization parameters and outputs results.
        /// <para>Analysis results are returned, too.</para></summary>
        /// <param name="parameters">Optimization parameters at which the analysis is performed.</param>
        public virtual AnalysisResults TestAnalysis(IVector parameters)
        {
            StopWatch1 t = new StopWatch1();
            Console.WriteLine();
            Console.WriteLine("******");
            Console.WriteLine("Performing direct analysis...");
            Console.WriteLine("Optimization parameters: " + Environment.NewLine + "  " + parameters.ToString());
            AnalysisResults anres = new AnalysisResults(NumOptimizationParameters, NumOptimizationConstraints,
                false /* reqGradients */ );
            anres.NumParameters = NumOptimizationParameters;
            anres.NumConstraints = NumOptimizationConstraints;
            anres.ReqObjective = true;
            anres.Parameters = parameters;
            t.Start();
            Analysis.Analyse(anres);
            t.Stop();
            Console.WriteLine("Analysis results:");
            Console.WriteLine(anres.ToString());
            Console.WriteLine("Objective funcion: " + anres.Objective);
            if (anres.NumConstraints <= 0)
                Console.WriteLine("There are no constraints.");
            else
            {
                Console.WriteLine("Constraint functions (there are " + anres.NumConstraints + " constraints): ");
                for (int i = 0; i < anres.NumConstraints; ++i)
                {
                    Console.WriteLine("  " + i + anres.GetConstraint(i));
                }
            }
            Console.WriteLine("Analysis time: " + t.Time + " s, " + t.CpuTime + " s CPU.");
            Console.WriteLine("**");
            return anres;
        }

        /// <summary>Calculates a table of specified number of analyses, with parameters running between
        /// two points that are chosen by the algorithm.</summary>
        /// <param name="numPoints">Number of points in which analysis results are calculated.</param>
        /// <returns>Array of calculated analysis results.</returns>
        public virtual AnalysisResults[] TestAnalysisTable(int numPoints)
        {
            IVector parNeural1 = null, parNeural2 = null, param1 = null, param2 = null;
            GetNeuralInputVector(0.2, ref parNeural1);
            GetNeuralInputVector(0.8, ref parNeural2);
            TransfNeuralToOptimizationParameters(parNeural1, ref param1);
            TransfNeuralToOptimizationParameters(parNeural2, ref param2);
            return TestAnalysisTable(param1, param2, numPoints);
        }

        /// <summary>Performs a table of direct analyses between two specified vectors of optimization parameters, 
        /// and outputs results. It also returns the table of analysis results.
        /// <para>Analyses are performed in specified number of equidistant points between the two specified vectors of parameters.</para></summary>
        /// <param name="param1">The first vector of parameters.</param>
        /// <param name="param2">The second vector of parameters.</param>
        /// <param name="numPoints">Number of analyses perfomed, must be at least two.</param>
        /// <returns>Array of calculated analysis results.</returns>
        public virtual AnalysisResults[] TestAnalysisTable(IVector param1, IVector param2, int numPoints)
        {
            if (param1 == null)
                throw new ArgumentException("First vector of parameters not specified.");
            else if (param2 == null)
                throw new ArgumentException("Second vector of parameters not specified.");
            else if (numPoints < 2)
                throw new ArgumentException("Number of analyses should be at least two; now the number is " + numPoints);
            StopWatch1 t = new StopWatch1();
            Console.WriteLine();
            Console.WriteLine("******************** Analysis table:");
            Console.WriteLine("Calculating a table of " + numPoints + " analysis results between two points...");
            Console.WriteLine("First vector:");
            Console.WriteLine("  " + param1);
            Console.WriteLine("Second vector:");
            Console.WriteLine("  " + param1);
            Console.WriteLine("Calculating...");
            Console.WriteLine();
            AnalysisResults[] results = new AnalysisResults[numPoints];
            IVector step = new Vector(param1.Length);
            Vector.Subtract(param2, param1, step);
            Vector.Multiply(step, 1.0 / ((double)numPoints - 1.0), step);
            IVector current = new Vector(param1.Length);
            Vector.Copy(param1, current);
            t.Start();
            for (int i = 0; i < numPoints; ++i)
            {
                Console.WriteLine();
                Console.WriteLine("Performing analysis No. " + i + "...");
                Vector param = new Vector(current);
                results[i] = TestAnalysis(param);
                Vector.Add(current, step, current);
            }
            t.Stop();
            Console.WriteLine();
            Console.WriteLine("... calculation finished.");
            Console.WriteLine();
            Console.WriteLine("Interval factors: ");
            for (int i = 0; i < numPoints; ++i)
            {
                Console.WriteLine(i + ": t = " + ((double)i / ((double)numPoints - 1.0)));
            }
            Console.WriteLine("Parameters:");
            for (int i = 0; i < numPoints; ++i)
            {
                Console.WriteLine("  " + i + ": " + results[i].Parameters);
            }
            Console.WriteLine("Values of objective function:");
            for (int i = 0; i < numPoints; ++i)
            {
                Console.WriteLine("  " + i + ": " + results[i].Objective);
            }
            if (results[0].NumConstraints > 0 && results[0].CalculatedConstraints == true)
            {
                Console.WriteLine("Values of constraint functions: ");
                IVector constr = new Vector(results[0].NumConstraints);
                for (int i = 0; i < numPoints; ++i)
                {
                    for (int whichConstr = 0; whichConstr < constr.Length; ++whichConstr)
                        constr[i] = results[i].GetConstraint(whichConstr);
                    Console.WriteLine("  " + i + ": " + constr);
                }
            }
            Console.WriteLine("Calculation time:  " + t.Time + " s, CPU: " + t.CpuTime + " s.");
            Console.WriteLine("**** End of analysis table.");
            Console.WriteLine();
            return results;
        } // TestanalysisTable()

        public virtual void OptimizeSimplex()
        {

        }

#endregion Tests







#region ParameterMap

        /* This region defines how neural network input parameters are mapped to optimization parameters and vice versa.
         * Description of mapping:
         * Input parameter No. 10 is skipped from optimization parameters. This parameter means billet dimension and was 
         * fixed in training data.
         * */

        // TODO: MOVE THIS TO UPPER LEVEL!
        // Here there can be abstract methods or methods that throw exceptions!

        ///// <summary>Number of neural network input parameters.</summary>
        //public abstract int NumNeuralParameters
        //{
        //    get;
        //    protected set;
        //}

        ///// <summary>Number of neural network output values.</summary>
        //public abstract int NumNeuralOutputs
        //{
        //    get;
        //    protected set;
        //}



        /// <summary>Number of optimization parameters.</summary>
        public abstract int NumOptimizationParameters
        {
            get;
            protected set;
        }

        // int _numOptimizationConstraints = 0;

        /// <summary>Number of optimization constraints.</summary>
        public abstract int NumOptimizationConstraints
        {
            get;
            protected set;
        }



        /// <summary>Maps input parameters for neural network to optimization input parameters.</summary>
        /// <param name="neuralParameters">Vector of neural network's input parameters.</param>
        /// <param name="optimizationParameters">Vector where corresponding optimization parameters are written.
        /// Reallocated if necessary.</param>
        public virtual void TransfNeuralToOptimizationParameters(IVector neuralParameters, ref IVector optimizationParameters)
        {
            throw new NotImplementedException("Transformation ftom neural to optimization parameters is not implemented.");
        }


        /// <summary>Maps optimization input parameters to neural network input parameters.</summary>
        /// <param name="optimizationParameters">Vector where corresponding optimization parameters are written.</param>
        /// <param name="neuralParameters">Vector of neural network's input parameters.
        /// Reallocated if necessary.</param>
        public virtual void TransfOptimizationToNeuralParameters(IVector optimizationParameters, ref IVector neuralParameters)
        {
            throw new NotImplementedException("Transformation from optimization to neural parameters is not yet implemented.");
        }

#endregion ParameterMap


#region SimulationData

        private InputOutputDataDefiniton _simulationDataDefinition;

        /// <summary>Simulation data definition (input and output used by simulator).</summary>
        public virtual InputOutputDataDefiniton SimulationDataDefinition
        {
            get
            {
                if (_simulationDataDefinition == null)
                    lock (Lock)
                    {
                        if (_simulationDataDefinition == null)
                            InputOutputDataDefiniton.LoadJson(
                                Path.Combine(OptimizationDirectory, NeuralFileConst.SimulationDataDefinitionFilename),
                                ref _simulationDataDefinition);
                    }
                return _simulationDataDefinition;
            }
        }

        /// <summary>Number of simulation input parameters.</summary>
        public virtual int NumSimulationParameters { 
            get { return SimulationDataDefinition.InputLength; }
            protected set { throw new InvalidOperationException("Number of simulation inputs can not be set."); }
        }

        /// <summary>Number of simulation output values.</summary>
        public virtual int NumSimulationOutputs { 
            get { return SimulationDataDefinition.OutputLength; } 
             protected set { throw new InvalidOperationException("Number of simulation outputs can not be set."); }
       }


        protected IVector _simulationInputDefault;

        /// <summary>Gets the vector of default values of simulation parameters and stores it in the specified vector.</summary>
        /// <param name="result">Vector where result is stored.</param>
        /// <remarks><para>Elements of vector are obtained from data definition. If for some element default value is not defined
        /// then it is set to 0.</para></remarks>
        protected virtual void GetSimulationInputDefault(ref IVector result)
        {
            lock (Lock)
            {
                if (_simulationInputDefault == null)
                {
                    InputOutputDataDefiniton def = SimulationDataDefinition;
                    int dim = def.InputLength;
                    // Vector.Resize(ref result, NumSimulationParameters);
                    _simulationInputDefault = new Vector(dim);
                    for (int i = 0; i < dim; ++i)
                    {
                        InputElementDefinition element = def.GetInputElement(i);
                        _simulationInputDefault[i] = 0.0;
                        if (element.DefaultValueDefined)
                            _simulationInputDefault[i] = element.DefaultValue;
                    }
                }
                Vector.Copy(_simulationInputDefault, ref result);
            }
        }

        protected IVector _simulationInputMin;

        /// <summary>Gets the vector of lower bounds on simulation parameters and stores it in the specified vector.</summary>
        /// <param name="result">Vector where result is stored.</param>
        /// <remarks><para>Elements of vector are obtained from data definition. If for some element bound is not defined
        /// then it is set to <see cref="double.MinValue"/></para></remarks>
        protected virtual void GetSimulationInputMin(ref IVector result)
        {
            lock (Lock)
            {
                if (_simulationInputMin == null)
                {
                    InputOutputDataDefiniton def = SimulationDataDefinition;
                    int dim = def.InputLength;
                    // Vector.Resize(ref result, NumSimulationParameters);
                    _simulationInputMin = new Vector(dim);
                    for (int i = 0; i < dim; ++i)
                    {
                        InputOutputElementDefinition element = def.GetInputElement(i);
                        _simulationInputMin[i] = double.MinValue;
                        if (element.BoundsDefined)
                            _simulationInputMin[i] = element.MinimalValue;
                    }
                }
                Vector.Copy(_simulationInputMin, ref result);
            }
        }


        protected IVector _simulationInputMax;

        /// <summary>Gets the vector of upper bounds on simulation parameters and stores it in the specified vector.</summary>
        /// <param name="result">Vector where result is stored.</param>
        /// <remarks><para>Elements of vector are obtained from data definition. If for some element bound is not defined
        /// then it is set to <see cref="double.MaxValue"/></para></remarks>
        protected virtual void GetSimulationInputMax(ref IVector result)
        {
            lock (Lock)
            {
                if (_simulationInputMax == null)
                {
                    InputOutputDataDefiniton def = SimulationDataDefinition;
                    int dim = def.InputLength;
                    // Vector.Resize(ref result, NumSimulationParameters);
                    _simulationInputMax = new Vector(dim);
                    for (int i = 0; i < dim; ++i)
                    {
                        InputOutputElementDefinition element = def.GetInputElement(i);
                        _simulationInputMax[i] = double.MaxValue;
                        if (element.BoundsDefined)
                            _simulationInputMax[i] = element.MaximalValue;
                    }
                }
                Vector.Copy(_simulationInputMax, ref result);
            }
        }


        protected IBoundingBox _simulationInputBounds;

        /// <summary>Bounds on simulation input parameters.</summary>
        public virtual IBoundingBox SimulationInputBounds
        {
            get
            {
                if (_simulationInputBounds == null)
                {
                    lock (Lock)
                    {
                        if (_simulationInputBounds == null)
                        {
                            _simulationInputBounds = new BoundingBox(NumSimulationParameters);
                            IVector min = null, max = null;
                            GetSimulationInputMin(ref min);
                            GetSimulationInputMax(ref max);
                            _simulationInputBounds.Update(min, max);
                        }
                    }
                }
                return _simulationInputBounds;
            }
            protected set
            {
                _simulationInputBounds = value;
            }
        }


#endregion SimulationData


#region NeuralParallel

        protected string _parallelResultsFilename = "ParallelResults";

        public string ParallelResultsFilename = "ParallelResults";

        protected string _parallelResultsFileExtension = ".json";

        public string ParallelResultsFileExtension
        {
            get { return _parallelResultsFileExtension; }
            protected set { _parallelResultsFileExtension = value; }
        }

#endregion NeuralParallel



#region ParallelSimulations

        public delegate string ParallelRunDelegate(int threadIndex);

        /// <summary>Number of runs of simulator in each parallel thread.</summary>
        public int ParSimNumRuns = 2;

        /// <summary>Number of jobs that can be executed in parallel.</summary>
        public int ParSimNumThreads = 2;

        /// <summary>Specifies how often the results are saved.</summary>
        public int ParSimSavingFrequency = 4;

        protected List<IAsyncResult> ParallelResults;


        /// <summary>Prepares data for the specified number of parallel execution threads.</summary>
        /// <param name="numThreads">Number of parallel threads for which data is prepared.</param>
        protected virtual void ParSimPrepareDirectories(int numThreads)
        {
            for (int threadIndex = 0; threadIndex < ParSimNumThreads; ++threadIndex)
            {
                ParSimPreparelDirectory(threadIndex);
            }
        }


        /// <summary>Waits until all parallel jobs complete.
        /// <para>It is necessary to execute this in the main thread because ending of the main thread would kill all 
        /// parallel threads (jobs are executed in background threads via async. invoke mechanism).</para></summary>
        protected virtual void ParSimWaitAllCompletion()
        {
#if !NETFRAMEWORK
            throw new IG.Lib.FrameworkDependencyException("ParSimWaitAllCompletion(...): AsyncResult is not defined.");
#else

            int numThreads = 0;
            lock (Lock)
            {
                if (ParallelResults != null)
                    numThreads = ParallelResults.Count;
            }
            if (numThreads < 1)
                Console.WriteLine("No parallel jobs were run.");
            else
            {
                Console.WriteLine();
                Console.WriteLine("Waiting for completion of all parallel jobs...");
            }
            for (int i = 0; i < numThreads; ++i)
            {
                AsyncResult result = null;
                lock (Lock)
                {
                    if (ParallelResults.Count > i)
                        result = (AsyncResult)ParallelResults[i];
                }
                if (result.IsCompleted)
                {
                    Console.WriteLine("Job No. " + i + " has already completed.");
                }
                else
                {
                    Console.WriteLine("Waiting job No. " + i + "...");
                    ParallelRunDelegate caller = (ParallelRunDelegate)result.AsyncDelegate;  // retrieve the delegate
                    // Call EndInvoke to retrieve the results.
                    string jobResult = caller.EndInvoke(result);
                    Console.WriteLine("... finished, result = " + jobResult);
                }
            }
#endif
        }


        /// <summary>Launches parallel jobs.</summary>
        /// <param name="numThreads">Number of parallel threads for which data is prepared.</param>
        public virtual void ParSimBegin(int numThreads)
        {
            ParallelResults = new List<IAsyncResult>();
            for (int threadIndex = 0; threadIndex < ParSimNumThreads; ++threadIndex)
            {
                lock (Lock)
                {
                    ParallelRunDelegate runDelegate = new ParallelRunDelegate(ParSimRunJob);
                    IAsyncResult result = runDelegate.BeginInvoke(threadIndex, null, threadIndex);
                    ParallelResults.Add(result);
                }
            }
        }


        /// <summary>Reads results form all parallel threads, gathers them in a single training set, and saves
        /// them in the template optimiation directory.</summary>
        public virtual void GatherParallelResults(int numThreads)
        {
            Console.WriteLine();
            Console.WriteLine("Gathering results from " + numThreads + " parallel threads..");
            SampledDataSet results = new SampledDataSet();
            for (int i = 0; i < numThreads; ++i)
            {
                ParSimAddResults(i, results);
            }

            Console.WriteLine();
            Console.WriteLine("Complete number of training points: " + results.Length);
            string resultFile = GetParallelResultFilePath();
            // Remove null and duplicated elements:
            int numRepeated = 10;
            numRepeated = +results.GetNumNullElemets();
            Console.WriteLine("Number of duplicated elements: " + results.GetNumInputDuplicates());
            Console.WriteLine("Number of null elements: " + results.GetNumNullElemets());
            results.RemoveInputDuplicates();
            Console.WriteLine("Number of training points after cleaning: " + results.Length);

            Console.WriteLine();
            Console.WriteLine("Saving results to " + resultFile + "...");
            SampledDataSet.SaveJson(results, resultFile);
            Console.WriteLine("... Done.");
            Console.WriteLine();
        }

        /// <summary>Callback method for asynchronous runs.</summary>
        /// <param name="ar">Asynchronous results that are passed to the method.</param>
        protected virtual void ParSimAsyncCallback(IAsyncResult ar)
        {
#if !NETFRAMEWORK
            throw new IG.Lib.FrameworkDependencyException("AsyncWait(...): AsyncResult is not defined.");
#else

            lock (Lock)
            {
                int Id = -1;
                try
                {
                    AsyncResult result = (AsyncResult)ar;
                    Id = (int)ar.AsyncState;
                    ParallelRunDelegate caller = (ParallelRunDelegate)result.AsyncDelegate;  // retrieve the delegate
                    string returnValue = null;
                    // Call EndInvoke to retrieve the results.
                    returnValue = caller.EndInvoke(ar);
                    if (OutputLevel > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Parallel run completed, thread ID: " + Id);
                        Console.WriteLine("  result: " + returnValue);
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    if (OutputLevel > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Exception thrown in callback of parallel run. Thread ID: " + Id
                            + Environment.NewLine + "  Error message: " + ex.Message);
                        Console.WriteLine();
                    }
                }
                finally
                {
                }
            }
#endif
        }


        /// <summary>Returns directory extension for data directory used in the specified parallel calculation thread.</summary>
        /// <param name="threadIndex">Index of the parllel calculation thread and corresponding data directory.</param>
        protected virtual string ParSimGetDirectoryExtension(int threadIndex)
        {
            return "_" + string.Format("{0:000}", threadIndex);
        }

        /// <summary>Returns path to the directory containing optimization data for the specified parallel thread.</summary>
        /// <param name="threadIndex">Index of parallel calculation thread for which the directory is returned.</param>
        public virtual string ParSimGetOptimizationDirectoryPath(int threadIndex)
        {
            string templateDirectory = OptimizationDirectory;
            UtilSystem.StandardizeDirectoryPath(ref templateDirectory);
            string templateParentDirectory = Path.GetDirectoryName(templateDirectory); // Directory.GetParent(templateDirectory);
            //UtilFile.StandardizeDirectoryPath(ref templateParentDirectory);
            string temlateDirName = Path.GetFileName(templateDirectory);
            string ret = Path.Combine(templateParentDirectory, temlateDirName + ParSimGetDirectoryExtension(threadIndex));
            UtilSystem.StandardizeDirectoryPath(ref ret);
            Console.WriteLine();
            if (OutputLevel > 3)
            {
                Console.WriteLine("Construction of data directory path for parallel execution: ");
                Console.WriteLine("Template optimization directory: " + OptimizationDirectory);
                Console.WriteLine("Its parent directory: " + templateParentDirectory);
                Console.WriteLine("Template dir. name: " + temlateDirName);
                Console.WriteLine("Created parallel execution directory path: "
                    + Environment.NewLine + "  " + ret);
            }
            return ret;
        }


        /// <summary>Returns name of the mutex for locking data on disk that is used by the specified parallel thread.</summary>
        /// <param name="threadIndex">Index (internal) of thread for which mutex name is obtained.</param>
        public virtual string ParSimGetMutexName(int threadIndex)
        {
            return "parallel_thread" + ParSimGetDirectoryExtension(threadIndex) + "_"
                + ParSimGetOptimizationDirectoryPath(threadIndex).GetHashCode().ToString();
        }

        /// <summary>Returns path of the file where resuls of calculation in the specified parallel thread are stored.</summary>
        /// <param name="threadIndex">Index of the parallel thread.</param>
        public virtual string ParSimGetParallelResultsFilePath(int threadIndex)
        {
            string fileName = ParallelResultsFilename + ParSimGetDirectoryExtension(threadIndex) + ParallelResultsFileExtension;
            return Path.Combine(ParSimGetOptimizationDirectoryPath(threadIndex), fileName);
        }

        public string ParSimResultFilename = "GatheredNeuralTrainingData.json";

        /// <summary>Returns path of the file where resuls of calculation in the specified parallel thread are stored.</summary>
        /// <param name="threadIndex">Index of the parallel thread.</param>
        public virtual string GetParallelResultFilePath()
        {
            // Joined (gathered) results will be saved to a file in the parent directory of the optimization directory...
            return Path.Combine(Path.GetDirectoryName(OptimizationDirectory), ParSimResultFilename);
        }


        /// <summary>Prepares data for running calculations in the parallel thread with the specified index.</summary>
        /// <param name="threadIndex"></param>
        public virtual void ParSimPreparelDirectory(int threadIndex)
        {
            string sourcePath = OptimizationDirectory;
            string targetPath = ParSimGetOptimizationDirectoryPath(threadIndex);
            lock (Lock)
            {
                if (OutputLevel > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Preparing parallel execution data directory: ");
                    Console.WriteLine("Copying data from: ");
                    Console.WriteLine(" " + sourcePath);
                    Console.WriteLine("To: ");
                    Console.WriteLine("  " + targetPath);
                    Console.WriteLine();
                }
                try
                {
                    UtilSystem.CopyDirectory(sourcePath, targetPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR occurred when prepairing parallel directory for thread No. " + threadIndex);
                    Console.WriteLine("Directory: " + targetPath);
                    Console.WriteLine("Error: " + ex.Message);
                    Console.WriteLine();
                }
            } // lock
        }

        /// <summary>Reads results of the specified thread and adds them to the existing training set.</summary>
        /// <param name="threadIndex">Index of the thread.</param>
        /// <param name="results">Training results to which results are added.</param>
        public void ParSimAddResults(int threadIndex, SampledDataSet results)
        {
            string optimizationDirectory = ParSimGetOptimizationDirectoryPath(threadIndex);
            // Path to the file where results will be saved:
            string resultFilePath = ParSimGetParallelResultsFilePath(threadIndex);
            SampledDataSet threadResults = null;
            Mutex mut = new Mutex(false /* initiallyOwned */, ParSimGetMutexName(threadIndex));
            try
            {
                Console.Write("Getting results of thread " + threadIndex + ", waiting mutex...");
                mut.WaitOne(); Console.WriteLine(" ... obtained ...");
                SampledDataSet.LoadJson(resultFilePath, ref threadResults);
                if (threadResults == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("WARNING: No results were found for thread No. " + threadIndex);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR when reading results, thread No. " + threadIndex + ":");
                Console.WriteLine("  " + ex.Message);
                Console.WriteLine();
            }
            finally { mut.ReleaseMutex(); }
            if (threadResults != null)
            {
                int numAdded = 0;
                for (int i = 0; i < threadResults.Length; ++i)
                {
                    SampledDataElement currentResults = threadResults[i];
                    if (currentResults.OutputValues != null)
                    {
                        results.AddElement(currentResults);
                        ++numAdded;
                    }
                }
                Console.WriteLine("Thread No. " + threadIndex + ": total results " + threadResults.Length + ", usable: " + numAdded);
            }
        }

        protected object ParSimProcessDelayLock = new object();

        /// <summary>Minimal delay between two successive launches of simulator processes, in milliseconds.
        /// This delay is introduced in order to prevent unusual errors that occur when running two simulator
        /// processes at approcimately the same time (although they use completely separate data directory structures).</summary>
        protected int ParSimProcessDelayMilliseconds = 500;

        /// <summary>Runs the job in the specified parallel thread.</summary>
        /// <param name="threadIndex">Index of the parallel thread.</param>
        /// <returns>null.</returns>
        public string ParSimRunJob(int threadIndex)
        {
            // string optimizationDirectory = GetOptimizationDirectoryPath(threadIndex);
            string threadDirectory = ParSimGetOptimizationDirectoryPath(threadIndex);
            if (!Directory.Exists(threadDirectory))
                ParSimPreparelDirectory(threadIndex);
            // Path to the file where results will be saved:
            string resultFilePath = ParSimGetParallelResultsFilePath(threadIndex);
            IResponseEvaluatorVectorSimple simulator = ParSimGetSimulator(threadIndex);
            SampledDataSet resultsData = new SampledDataSet();
            string mutexName = ParSimGetMutexName(threadIndex);
            Mutex mut = new Mutex(false /* initiallyOwned */, mutexName);
            if (OutputLevel >= 0)
            {
                lock (Lock)
                {
                    Console.WriteLine();
                    Console.WriteLine("==============================================================");
                    Console.WriteLine("Running parallel simulations for thread No. " + threadIndex);
                    Console.WriteLine("Directory: " + threadDirectory);
                    Console.WriteLine("Result file: " + resultFilePath);
                    // Console.WriteLine("Sim. root: " + simulator.RootDirectory);
                    Console.WriteLine("Mutex name: " + mutexName);
                    Console.WriteLine("Delay in milliseconds: " + ParSimProcessDelayMilliseconds);
                    Console.WriteLine();

                }
            }
            StopWatch1 t = new StopWatch1();
            double totalTime = 0, averageTime = 0;
            for (int whichRun = 0; whichRun < ParSimNumRuns; ++whichRun)
            {
                bool saveResults = ((whichRun + 1) % ParSimSavingFrequency == 0 || whichRun == ParSimNumRuns - 1);
                Console.WriteLine();
                Console.WriteLine("** Thread " + threadIndex + ", run " + (whichRun+1) + "/" + ParSimNumRuns + (saveResults ? " (results will be saved)." : ".")
                    + Environment.NewLine + "  Sim. root: " + ParSimGetSimulationDirectoryPath(threadIndex) // simulator.RootDirectory
                    + Environment.NewLine);
                IVector neuralInput = null, simInput = null, neuralOutput = null, simoutput = null;
                //ParSimGetNextInput(ref simInput);  // can not be performed directly here,  since we need neural input param.!
                ParSimGetNextNeuralInput(ref neuralInput);
                TransfNeuralToSimulationInput(neuralInput, ref simInput);
                lock (ParSimProcessDelayLock)
                {
                    // REMARK:
                    // If we run individual simulations too close together then errors will occur!
                    // This might be due to improper handling of resources such as temporary files in simulation!
                    Thread.Sleep(ParSimProcessDelayMilliseconds);
                }
                t.Start();
                try
                {
                    // Calculate response by this thread's simulator with the higher level method that
                    // performs all the necessary additional stuff:
                    SimulatorCalculateResponse(simulator, simInput, ref simoutput);
                }
                catch { }
                t.Stop();
                totalTime += t.Time;
                averageTime = totalTime/(double)(whichRun+1);
                TransfSimulationToNeuralInput(simInput, ref neuralInput);
                TransfSimulationToNeuralOutput(simoutput, ref neuralOutput);
                Console.WriteLine(Environment.NewLine +
                    "** Thread " + threadIndex + ", run " + (whichRun+1) + "/" + ParSimNumRuns 
                    + " finished in " + t.Time + " s (average: " + averageTime + ")."
                    + Environment.NewLine);
                resultsData.AddElement(new SampledDataElement(neuralInput, neuralOutput));
                if (saveResults)
                {
                    try
                    {
                        mut.WaitOne();
                        // We save intermediate results: 
                        string safetyBackupPath = resultFilePath + ".bak";
                        lock (Lock)
                        {
                            try
                            {
                                File.Move(resultFilePath, safetyBackupPath);
                            }
                            catch (Exception) { }
                            try
                            {
                                SampledDataSet.SaveJson(resultsData, resultFilePath);
                                File.Delete(safetyBackupPath);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine();
                                Console.WriteLine("ERROR when writing results, thread " + threadIndex + ", run: " + whichRun + ":");
                                Console.WriteLine("  " + ex.Message);
                                Console.WriteLine();
                            }
                        }
                    }
                    finally { mut.ReleaseMutex(); }
                }
            }
            return null;
        }



        private IVector _neuralInput;

        /// <summary>Generates the next vector of simulation input parameters that will be used for 
        /// calculation of a new parallel simulation.</summary>
        /// <remarks>Vector of neural input parameters is generated first by <see cref="ParSimGetNextNeuralInput"/>,
        /// then neural parameters are tansformed to simulation parameters.</remarks>
        public virtual void ParSimGetNextInput(ref IVector simInput)
        {
            lock(Lock)
            {
                ParSimGetNextNeuralInput(ref _neuralInput);
                TransfNeuralToSimulationInput(_neuralInput, ref simInput);
            }
        }


#region ToOverride

        /// <summary>Generates the next vector of neural input parameters that will be used for 
        /// calculation of a new training element.</summary>
        public virtual void ParSimGetNextNeuralInput(ref IVector simInput)
        {
            throw new NotImplementedException("Getting next input for parallel simulations is not implenmented.");
        }

        /// <summary>Returns the simulation directory for the specified parallel calculation thread.</summary>
        /// <param name="threadIndex">Index of parallel calculation thread for which the directory is returned.</param>
        public virtual string ParSimGetSimulationDirectoryPath(int threadIndex)
        {
            throw new NotImplementedException("Method for getting simulation directory path should be implemented in the derived class.");
        }


        /// <summary>Creates simulator file manager for the simulator that will the specified parallel task.</summary>
        /// <param name="threadIndex">Index of parallel task and its corresponding data directory.</param>
        /// <remeremarks>Derived classes that implement parallel execution of simulators for generating training data
        /// eill either override this method, or mor conveniently the <see cref="CreateNewSimulator"/> method that is called from this method.</remeremarks>
        public virtual IResponseEvaluatorVectorSimple ParSimGetSimulator(int threadIndex)
        {
            IResponseEvaluatorVectorSimple simulator = null;
            CreateNewSimulator(ParSimGetSimulationDirectoryPath(threadIndex), ref simulator);
            return simulator;
        }

#endregion ToOverride

#endregion ParallelSimulations


#region Transformations

        /// <summary>Transforms the specified vector of simulation input parameters to the vector of neural 
        /// input parameters and stores the vector to the specified variable.</summary>
        /// <param name="original">Vector to be transformed.</param>
        /// <param name="result">Vector where result of transformation is stored.</param>
        public abstract void TransfSimulationToNeuralInput(IVector original, ref IVector result);
        

        /// <summary>Transforms the specified vector of neural input parameters to the vector of simulation 
        /// input parameters and stores the vector to the specified variable.</summary>
        /// <param name="original">Vector to be transformed.</param>
        /// <param name="result">Vector where result of transformation is stored.</param>
        public virtual void TransfNeuralToSimulationInput(IVector original, ref IVector result)
        {
            throw new NotImplementedException("Transformation from neural to simulation input is not defined.");
        }

        /// <summary>Transforms the specified vector of simulation output values (results) to the vector of neural 
        /// output values and stores the vector to the specified variable.</summary>
        /// <param name="original">Vector to be transformed.</param>
        /// <param name="result">Vector where result of transformation is stored.</param>
        public virtual void TransfSimulationToNeuralOutput(IVector original, ref IVector result)
        {
            throw new NotImplementedException("Transformation from simulation to neural output is not defined.");
        }

        /// <summary>Transforms the specified vector of neural output values to the vector of simulation 
        /// output values (results) and stores the vector to the specified variable.</summary>
        /// <param name="original">Vector to be transformed.</param>
        /// <param name="result">Vector where result of transformation is stored.</param>
        public virtual void TransfNeuralToSimulationOutput(IVector original, ref IVector result)
        {
            throw new NotImplementedException("Transformation from neural to simulation input is not defined.");
        }


#endregion Transformations


#region NeuralData

        NeuraApproximationFileManager _neuralFileManager;

        /// <summary>File manager that provides access to trained neural network and related data.</summary>
        public virtual NeuraApproximationFileManager NeuralFM
        {
            get
            {
                if (_neuralFileManager == null)
                {
                    if (!string.IsNullOrEmpty(this.OptimizationDirectory))
                        _neuralFileManager = new NeuraApproximationFileManager(this.OptimizationDirectory);
                }
                return _neuralFileManager;
            }
        }
        

        /// <summary>If different than null, this static property provides alternative location of the file
        /// which neural network approximator is resd from, in terms of path relative to optimization directory.</summary>
        public static string TrainedNetworkAlternativeLovation = null; 

        /// <summary>Trained neural network.</summary>
        public virtual INeuralApproximator TrainedNetwork
        {
            get
            {
                if (_trainedNetwork == null)
                {
                    string path;
                    if (TrainedNetworkAlternativeLovation != null)
                    {
                        path = Path.Combine(OptimizationDirectory, TrainedNetworkAlternativeLovation);
                        Console.WriteLine();
                        Console.WriteLine("Reading neural netwoork... " + Environment.NewLine + "  Location: " + path);
                        Console.WriteLine();
                    } else
                        path = NeuralFM.NeuralNetworkPath;
                    INeuralApproximator network = null;
                    // NeuralApproximatorBaseExt.LoadJson(path, ref network);
                    this.LoadJson(path, ref network);
                    TrainedNetwork = network;
                }
                return _trainedNetwork;
            }
            protected set
            {
                _trainedNetwork = value;
            }
        }

        protected List<IVector> _sensitivityResultsVerification;

        public virtual List<IVector> SensitivityVerificationResults
        {
            get
            {
                if (_sensitivityResultsVerification == null)
                    _sensitivityResultsVerification = new List<IVector>();
                return _sensitivityResultsVerification;
            }
            protected set
            {
                _sensitivityResultsVerification = value;
            }
        }

        protected List<IVector> _sensitivityResultsTraining;

        public virtual List<IVector> SensitivityTrainingResults
        {
            get
            {
                if (_sensitivityResultsTraining == null)
                    _sensitivityResultsTraining = new List<IVector>();
                return _sensitivityResultsTraining;
            }
            protected set
            {
                _sensitivityResultsTraining = value;
            }
        }

        protected INeuralApproximator _trainedNetwork;

        /// <summary>Trained neural network.</summary>
        public virtual INeuralApproximator XXX_TrainedNetwork
        {
            get
            {
                if (_trainedNetwork == null)
                {
                    // NeuralApproximatorBaseExt.LoadJson(NeuralFM.NeuralNetworkPath, ref _trainedNetwork);
                    this.LoadJson(NeuralFM.NeuralNetworkPath, ref _trainedNetwork);
                }
                return _trainedNetwork;
            }
            protected set
            {
                _trainedNetwork = value;
            }
        }

        InputOutputDataDefiniton _neuralDataDefinition;

        /// <summary>Neural data definition.</summary>
        public virtual InputOutputDataDefiniton NeuralDataDefinition
        {
            get
            {
                if (_neuralDataDefinition == null)
                    lock (Lock)
                    {
                        if (_neuralDataDefinition == null)
                            InputOutputDataDefiniton.LoadJson(NeuralFM.NeuralDataDefinitionPath, ref _neuralDataDefinition);
                    }
                return _neuralDataDefinition;
            }
            protected set
            {
                _neuralDataDefinition = value;
            }
        }

        protected SampledDataSet _trainingData;

        public virtual SampledDataSet TrainingData
        {
            get 
            {
                if (_trainingData == null)
                {
                    string path;
                    path = NeuralFM.NeuralTrainingDataPath;
                    SampledDataSet trainingData = null;
                    SampledDataSet.LoadJson(path, ref trainingData);
                    TrainingData = trainingData;
                }
                return _trainingData;
            }
            set { _trainingData = value; }
        }

        protected SampledDataSet _verificationData;

        public virtual SampledDataSet VerificationData
        {
            get 
            {
                if (_verificationData == null)
                {
                    string path;
                    path = NeuralFM.NeuralVerificationDataPath;
                    SampledDataSet verificationData = null;
                    SampledDataSet.LoadJson(path, ref verificationData);
                    VerificationData = verificationData;
                }
                return _verificationData;
            }
            set { _verificationData = value; }
        }

        /// <summary>Number of neural network input parameters.</summary>
        public virtual int NumNeuralParameters
        {
            get { return NeuralDataDefinition.InputLength; }
            protected set { throw new InvalidOperationException("Can not set number of neural parameters, obtained from data."); }
        }

        /// <summary>Number of neural network output values.</summary>
        public virtual int NumNeuralOutputs
        {
            get { return NeuralDataDefinition.OutputLength; }
            protected set { throw new InvalidOperationException("Can not set number of neural output values, obtained from data."); }
        }


        protected IVector _neuralInputDefault;

        /// <summary>Gets the vector of default values of neural parameters and stores it in the specified vector.</summary>
        /// <param name="result">Vector where result is stored.</param>
        /// <remarks><para>Elements of vector are obtained from data definition. If for some element default value is not defined
        /// then it is set to 0.</para></remarks>
        protected virtual void GetNeuralInputDefault(ref IVector result)
        {
            lock (Lock)
            {
                if (_neuralInputDefault == null)
                {
                    InputOutputDataDefiniton def = NeuralDataDefinition;
                    int dim = def.InputLength;
                    // Vector.Resize(ref result, NumNeuralParameters);
                    _neuralInputDefault = new Vector(dim);
                    for (int i = 0; i < dim; ++i)
                    {
                        InputElementDefinition element = def.GetInputElement(i);
                        _neuralInputDefault[i] = 0.0;
                        if (element.DefaultValueDefined)
                            _neuralInputDefault[i] = element.DefaultValue;
                    }
                }
                Vector.Copy(_neuralInputDefault, ref result);
            }
        }

        protected IVector _neuralInputMin;

        /// <summary>Gets the vector of lower bounds on neural parameters and stores it in the specified vector.</summary>
        /// <param name="result">Vector where result is stored.</param>
        /// <remarks><para>Elements of vector are obtained from data definition. If for some element bound is not defined
        /// then it is set to <see cref="double.MinValue"/></para></remarks>
        protected virtual void GetNeuralInputMin(ref IVector result)
        {
            lock (Lock)
            {
                if (_neuralInputMin == null)
                {
                    InputOutputDataDefiniton def = NeuralDataDefinition;
                    int dim = def.InputLength;
                    // Vector.Resize(ref result, NumNeuralParameters);
                    _neuralInputMin = new Vector(dim);
                    for (int i = 0; i < dim; ++i)
                    {
                        InputOutputElementDefinition element = def.GetInputElement(i);
                        _neuralInputMin[i] = double.MinValue;
                        if (element.BoundsDefined)
                            _neuralInputMin[i] = element.MinimalValue;
                    }
                }
                Vector.Copy(_neuralInputMin, ref result);
            }
        }


        protected IVector _neuralInputMax;

        /// <summary>Gets the vector of upper bounds on neural parameters and stores it in the specified vector.</summary>
        /// <param name="result">Vector where result is stored.</param>
        /// <remarks><para>Elements of vector are obtained from data definition. If for some element bound is not defined
        /// then it is set to <see cref="double.MaxValue"/></para></remarks>
        protected virtual void GetNeuralInputMax(ref IVector result)
        {
            lock (Lock)
            {
                if (_neuralInputMax == null)
                {
                    InputOutputDataDefiniton def = NeuralDataDefinition;
                    int dim = def.InputLength;
                    // Vector.Resize(ref result, NumNeuralParameters);
                    _neuralInputMax = new Vector(dim);
                    for (int i = 0; i < dim; ++i)
                    {
                        InputOutputElementDefinition element = def.GetInputElement(i);
                        _neuralInputMax[i] = double.MaxValue;
                        if (element.BoundsDefined)
                            _neuralInputMax[i] = element.MaximalValue;
                    }
                }
                Vector.Copy(_neuralInputMax, ref result);
            }
        }


        protected IBoundingBox _neuralInputBounds;

        /// <summary>Bounds on neural input parameters.</summary>
        public virtual IBoundingBox NeuralInputBounds
        {
            get
            {
                lock (Lock)
                {
                    if (_neuralInputBounds == null)
                    {
                        lock (Lock)
                        {
                            if (_neuralInputBounds == null)
                            {
                                _neuralInputBounds = new BoundingBox(NumNeuralParameters);
                                IVector min = null, max = null;
                                GetNeuralInputMin(ref min);
                                GetNeuralInputMax(ref max);
                                _neuralInputBounds.Update(min, max);
                            }
                        }
                    }
                    return _neuralInputBounds;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    _neuralInputBounds = value;
                }
            }
        }


        SampledDataSet _neuralTrainingData;

        /// <summary>Gets the training data.</summary>
        public virtual SampledDataSet NeuralTrainingData
        {
            get
            {
                if (_neuralTrainingData == null)
                    _neuralTrainingData = TrainedNetwork.TrainingData;
                return _neuralTrainingData;
            }
        }

        /// <summary>Gets number of all training points, including verification points.</summary>
        public virtual int NumAllTrainingPoints
        {
            get
            {
                return TrainedNetwork.NumAllTrainingPoints;
            }
        }


        // GENERAL UTILITIES FOR MANIPULATION OF NEURAL NETWORKS:

        /// <summary>Gets the training element with the specified index (chosen out of al training points, 
        /// including verificaion points).</summary>
        public virtual SampledDataElement GetTrainingElement(int i)
        {
            return NeuralTrainingData[i];
        }

        /// <summary>Whether the training point with the specified index is a verification points 
        /// and has not been used in training.</summary>
        public virtual bool IsVerificationPoint(int trainingPointIndex)
        {
            if (trainingPointIndex < 0 || trainingPointIndex >= NumAllTrainingPoints)
                throw new ArgumentException("Training point index (" + trainingPointIndex
                    + ") is out of range. Number of all training points: " + NumAllTrainingPoints + ".");
            if (TrainedNetwork.VerificationIndices != null)
                if (TrainedNetwork.VerificationIndices.Contains(trainingPointIndex))
                    return true;
            return false;
        }

        /// <summary>Returns a list of all neural training elements.
        /// <para>The returned list is created anew.</para></summary>
        /// <param name="includeVerificationPoints">Flag indicating whether verification points are also included in 
        /// the returned list.</param>
        public List<SampledDataElement> GetTrainingElements(bool includeVerificationPoints)
        {
            return GetTrainingElements(includeVerificationPoints, true /* includeNonVerificationPoints */);
        }

        /// <summary>Returns a list of all neural training elements.
        /// <para>The returned list is created anew.</para></summary>
        /// <param name="includeVerificationPoints">Flag indicating whether verification points are also included in 
        /// the returned list.</param>
        /// <param name="includeNonVerificationPoints">Flag indicating whether non-verification points are included in 
        /// the returned list.</param>
        public List<SampledDataElement> GetTrainingElements(bool includeVerificationPoints,
            bool includeNonVerificationPoints)
        {
            List<SampledDataElement> ret = new List<SampledDataElement>();
            int numPoints = NumAllTrainingPoints;
            for (int i = 0; i < NeuralTrainingData.Length; ++i)
            {
                if (IsVerificationPoint(i))
                {
                    if (includeVerificationPoints)
                        ret.Add(GetTrainingElement(i));
                }
                else
                {
                    if (includeNonVerificationPoints)
                        ret.Add(GetTrainingElement(i));
                }
            }
            return ret;
        }

        /// <summary>Returns a perturbance vector whose componenets represent magnitudes of perturbances 
        /// of neural input parameters with the specified relative ratio with corresponding parameters' scaling lengths.</summary>
        /// <param name="ratio">Ratio between perturbance component and scaling length of each parameter.
        /// <para>Scaling length is either the difference between maximal and minimal value, or 0.</para></param>
        /// <param name="perturbance">Vector where perturbance magnitudes are stored.</param>
        public virtual void GetNeuralInputPerturbance(double ratio, ref IVector perturbance)
        {
            bool allocate = false;
            if (perturbance == null)
                allocate = true;
            else if (perturbance.Length != TrainedNetwork.InputLength)
                allocate = true;
            if (allocate)
                perturbance = new Vector(TrainedNetwork.InputLength);
            for (int i = 0; i < perturbance.Length; ++i)
            {
                InputElementDefinition element = NeuralDataDefinition.GetInputElement(i);
                if (!element.BoundsDefined)
                    throw new InvalidDataException("Input element No. " + i + " does not have bounds defined.");
                double scalingLength = element.MaximalValue - element.MinimalValue;
                //if (scalingLength == 0)
                //    scalingLength = element.MaximalValue;
                //if (scalingLength == 0)
                //    scalingLength = 1.0;
                perturbance[i] = scalingLength * ratio;
            }
        }

        /// <summary>Returns a perturbance vector whose componenets represent magnitudes of perturbances 
        /// of neural output values with the specified relative ratio with corresponding values' scaling lengths.</summary>
        /// <param name="ratio">Ratio between perturbance component and scaling length of each value.
        /// <para>Scaling length is either the difference between maximal and minimal value, or 0.</para></param>
        /// <param name="perturbance">Vector where perturbance magnitudes are stored.</param>
        public virtual void GetNeuralOutputPerturbance(double ratio, ref IVector perturbance)
        {
            bool allocate = false;
            if (perturbance == null)
                allocate = true;
            else if (perturbance.Length != TrainedNetwork.OutputLength)
                allocate = true;
            if (allocate)
                perturbance = new Vector(TrainedNetwork.OutputLength);
            for (int i = 0; i < perturbance.Length; ++i)
            {
                OutputElementDefinition element = NeuralDataDefinition.GetOutputElement(i);
                if (!element.BoundsDefined)
                    throw new InvalidDataException("Output element No. " + i + " does not have bounds defined.");
                double scalingLength = element.MaximalValue - element.MinimalValue;
                //if (scalingLength == 0)
                //    scalingLength = element.MaximalValue;
                //if (scalingLength == 0)
                //    scalingLength = 1.0;
                perturbance[i] = scalingLength * ratio;
            }
        }

        /// <summary>Returns an input vector whose components are at specified relative distances (with respect to full range)
        /// from lower bounds on componens of input vectors.</summary>
        /// <param name="relativeComponents">Relative coordinate of all vector components of the returned vector, which run from
        /// 0 to 1 within the range of components (defined by minimal and maximal bounds).</para></param>
        /// <param name="inputVector">Object where generated vector is stored.</param>
        public virtual void GetNeuralInputVector(double relativeComponents, ref IVector inputVector)
        {
            bool allocate = false;
            if (inputVector == null)
                allocate = true;
            else if (inputVector.Length != TrainedNetwork.InputLength)
                allocate = true;
            if (allocate)
                inputVector = new Vector(TrainedNetwork.InputLength);
            for (int i = 0; i < inputVector.Length; ++i)
            {
                InputElementDefinition element = NeuralDataDefinition.GetInputElement(i);
                if (!element.BoundsDefined)
                    throw new InvalidDataException("Input element No. " + i + " does not have bounds defined.");
                double scalingLength = element.MaximalValue - element.MinimalValue;
                //if (scalingLength == 0)
                //    scalingLength = element.MaximalValue;
                //if (scalingLength == 0)
                //    scalingLength = 1.0;
                inputVector[i] = element.MinimalValue + scalingLength * relativeComponents;
            }
        }

        /// <summary>Returns an output vector whose components are at specified relative distances (with respect to full range)
        /// from lower bounds on componens of input vectors.</summary>
        /// <param name="relativeComponents">Relative coordinate of all vector components of the returned vector, which run from
        /// 0 to 1 within the range of components (defined by minimal and maximal bounds).</para></param>
        /// <param name="inputVector">Object where generated vector is stored.</param>
        public virtual void GetNeuralOutputVector(double relativeComponents, ref IVector outputVector)
        {
            bool allocate = false;
            if (outputVector == null)
                allocate = true;
            else if (outputVector.Length != NumNeuralOutputs)
                allocate = true;
            if (allocate)
                outputVector = new Vector(NumNeuralOutputs);
            for (int i = 0; i < outputVector.Length; ++i)
            {
                OutputElementDefinition element = NeuralDataDefinition.GetOutputElement(i);
                if (!element.BoundsDefined)
                    throw new InvalidDataException("Output element No. " + i + " does not have bounds defined.");
                double scalingLength = element.MaximalValue - element.MinimalValue;
                //if (scalingLength == 0)
                //    scalingLength = element.MaximalValue;
                //if (scalingLength == 0)
                //    scalingLength = 1.0;
                outputVector[i] = element.MinimalValue + scalingLength * relativeComponents;
            }
        }

        /// <summary>Returns an input vector whose components are at specified relative distances (with respect to full range)
        /// from lower bounds on componens of input vectors.</summary>
        /// <param name="relativeComponents">Relative coordinates of all vector components of the returned vector, which run from
        /// 0 to 1 within the range of components (defined by minimal and maximal bounds).</para></param>
        /// <param name="inputVector">Object where generated vector is stored.</param>
        public virtual void GetNeuralInputVector(IVector relativeComponents, ref IVector inputVector)
        {
            if (relativeComponents == null)
                throw new ArgumentException("The vector of relative components is not specified.");
            else if (relativeComponents.Length != NumNeuralParameters)
                throw new ArgumentException("The vector of relative components is not of correct dimension: "
                    + relativeComponents.Length + " instead of " + NumNeuralParameters + ".");
            bool allocate = false;
            if (inputVector == null)
                allocate = true;
            else if (inputVector.Length != NumNeuralParameters)
                allocate = true;
            if (allocate)
                inputVector = new Vector(NumNeuralParameters);
            for (int i = 0; i < inputVector.Length; ++i)
            {
                InputElementDefinition element = NeuralDataDefinition.GetInputElement(i);
                if (!element.BoundsDefined)
                    throw new InvalidDataException("Input element No. " + i + " does not have bounds defined.");
                double scalingLength = element.MaximalValue - element.MinimalValue;
                //if (scalingLength == 0)
                //    scalingLength = element.MaximalValue;
                //if (scalingLength == 0)
                //    scalingLength = 1.0;
                inputVector[i] = element.MinimalValue + scalingLength * relativeComponents[i];
            }
        }

        /// <summary>Returns an output vector whose components are at specified relative distances (with respect to full range)
        /// from lower bounds on componens of input vectors.</summary>
        /// <param name="relativeComponents">Relative coordinates of all vector components of the returned vector, which run from
        /// 0 to 1 within the range of components (defined by minimal and maximal bounds).</para></param>
        /// <param name="inputVector">Object where generated vector is stored.</param>
        public virtual void GetNeuralOutputVector(IVector relativeComponents, ref IVector outputVector)
        {
            if (relativeComponents == null)
                throw new ArgumentException("The vector of relative components is not specified.");
            else if (relativeComponents.Length != NumNeuralOutputs)
                throw new ArgumentException("The vector of relative components is not of correct dimension: "
                    + relativeComponents.Length + " instead of " + NumNeuralOutputs + ".");
            bool allocate = false;
            if (outputVector == null)
                allocate = true;
            else if (outputVector.Length != NumNeuralOutputs)
                allocate = true;
            if (allocate)
                outputVector = new Vector(NumNeuralOutputs);
            for (int i = 0; i < outputVector.Length; ++i)
            {
                OutputElementDefinition element = NeuralDataDefinition.GetOutputElement(i);
                if (!element.BoundsDefined)
                    throw new InvalidDataException("Output element No. " + i + " does not have bounds defined.");
                double scalingLength = element.MaximalValue - element.MinimalValue;
                //if (scalingLength == 0)
                //    scalingLength = element.MaximalValue;
                //if (scalingLength == 0)
                //    scalingLength = 1.0;
                outputVector[i] = element.MinimalValue + scalingLength * relativeComponents[i];
            }
        }

        /// <summary>Calculates relative components (running from 0 to 1 within bounds for specified vector component)
        /// for the specified vector of neural input parameters.</summary>
        /// <param name="inputVector">Vector of input parameters for neural network.</param>
        /// <param name="relativeComponents">Vector where transformed relative components of the specified vector are stored.</param>
        public virtual void GetNeuralInputRelative(IVector inputVector, ref IVector relativeComponents)
        {
            if (inputVector == null)
                throw new ArgumentException("The vector of neural input parameters is not specified.");
            else if (inputVector.Length != NumNeuralParameters)
                throw new ArgumentException("The vector of neural input parameters is not of correct dimension: "
                    + relativeComponents.Length + " instead of " + NumNeuralParameters + ".");
            bool allocate = false;
            if (relativeComponents == null)
                allocate = true;
            else if (relativeComponents.Length != NumNeuralParameters)
                allocate = true;
            if (allocate)
                relativeComponents = new Vector(NumNeuralParameters);
            for (int i = 0; i < inputVector.Length; ++i)
            {
                InputElementDefinition element = NeuralDataDefinition.GetInputElement(i);
                if (!element.BoundsDefined)
                    throw new InvalidDataException("Input element No. " + i + " does not have bounds defined.");
                double scalingLength = element.MaximalValue - element.MinimalValue;
                if (scalingLength == 0)
                    scalingLength = 1;
                relativeComponents[i] = (inputVector[i] - element.MinimalValue) / scalingLength;
            }
        }

        /// <summary>Calculates relative components (running from 0 to 1 within bounds for specified vector component)
        /// for the specified vector of neural output values.</summary>
        /// <param name="outputVector">Vector of output values in neural network approximator's output space.</param>
        /// <param name="relativeComponents">Vector where transformed relative components of the specified vector are stored.</param>
        public virtual void GetNeuralOutputRelative(IVector outputVector, ref IVector relativeComponents)
        {
            if (outputVector == null)
                throw new ArgumentException("The vector of neural output values is not specified.");
            else if (outputVector.Length != NumNeuralOutputs)
                throw new ArgumentException("The vector of neural output values is not of correct dimension: "
                    + relativeComponents.Length + " instead of " + NumNeuralOutputs + ".");
            bool allocate = false;
            if (relativeComponents == null)
                allocate = true;
            else if (relativeComponents.Length != NumNeuralOutputs)
                allocate = true;
            if (allocate)
                relativeComponents = new Vector(NumNeuralOutputs);
            for (int i = 0; i < outputVector.Length; ++i)
            {
                OutputElementDefinition element = NeuralDataDefinition.GetOutputElement(i);
                if (!element.BoundsDefined)
                    throw new InvalidDataException("Output element No. " + i + " does not have bounds defined.");
                double scalingLength = element.MaximalValue - element.MinimalValue;
                //if (scalingLength == 0)
                //    scalingLength = element.MaximalValue;
                //if (scalingLength == 0)
                //    scalingLength = 1.0;
                relativeComponents[i] = (outputVector[i] - element.MinimalValue) / scalingLength;
            }
        }


        // NumNeuralOutputs NumNeuralParameters


        // DISTANCE MEASURE AND UTILITIES:

        /// <summary>Returns measure of distance between two vectors in the space of input parameters.
        /// Euclidean norm scaled accorging to individual parameter ranges is returned.</summary>
        public virtual double InputDistance(IVector v1, IVector v2)
        {
            InputOutputDataDefiniton def = NeuralDataDefinition;
            if (v1 == null || v2 == null)
                throw new ArgumentException("One of the vectors to compute distance is null.");
            if (v1.Length != def.InputLength)
                throw new ArgumentException("Wrong dimension of the first vector (" + v1.Length + ", should be " + def.InputLength + ").");
            if (v2.Length != def.InputLength)
                throw new ArgumentException("Wrong dimension of the second vector (" + v2.Length + ", should be " + def.InputLength + ").");
            double retSquare = 0;
            for (int i = 0; i < v1.Length; ++i)
            {
                InputElementDefinition element = def.GetInputElement(i);
                if (!element.BoundsDefined)
                    throw new InvalidDataException("Input element No. " + i + " does not have bounds defined.");
                double scalingLength = element.MaximalValue - element.MinimalValue;
                if (scalingLength == 0)
                    scalingLength = element.MaximalValue;
                if (scalingLength == 0)
                    scalingLength = 1.0;
                double contribution = (v1[i] - v2[i]) / (scalingLength);
                retSquare += contribution * contribution;
            }
            return Math.Sqrt(retSquare);
        }

        /// <summary>Returns measure of distance between two vectors in the space of output parameters.
        /// Euclidean norm scaled accorging to individual parameter ranges is returned.</summary>
        public virtual double OutputDistance(IVector v1, IVector v2)
        {
            InputOutputDataDefiniton def = NeuralDataDefinition;
            if (v1 == null || v2 == null)
                throw new ArgumentException("One of the vectors to compute distance is null.");
            if (v1.Length != def.OutputLength)
                throw new ArgumentException("Wrong dimension of the first vector (" + v1.Length + ", should be " + def.OutputLength + ").");
            if (v2.Length != def.OutputLength)
                throw new ArgumentException("Wrong dimension of the second vector (" + v2.Length + ", should be " + def.OutputLength + ").");
            double retSquare = 0;
            for (int i = 0; i < v1.Length; ++i)
            {
                OutputElementDefinition element = def.GetOutputElement(i);
                if (!element.BoundsDefined)
                    throw new InvalidDataException("Output element No. " + i + " does not have bounds defined.");
                double scalingLength = element.MaximalValue - element.MinimalValue;
                if (scalingLength == 0)
                    scalingLength = element.MaximalValue;
                if (scalingLength == 0)
                    scalingLength = 1.0;
                double contribution = (v1[i] - v2[i]) / (scalingLength);
                retSquare += contribution * contribution;
            }
            return Math.Sqrt(retSquare);
        }

        /// <summary>Returns measure of distance between the specified neural input vector and vector of input
        /// parameters of the training point with the specified index.</summary>
        /// <param name="v1">First vector in space of input parameter for calculating distance.</param>
        /// <param name="trainingPointIndex">Index of training point for obtaining the second input vector.</param>
        public virtual double InputDistance(IVector v1, int trainingPointIndex)
        {
            if (trainingPointIndex < 0 || trainingPointIndex >= NumAllTrainingPoints)
                throw new ArgumentException("Training point index (" + trainingPointIndex
                    + ") is out of range. Number of all training points: " + NumAllTrainingPoints + ".");
            IVector v2 = NeuralTrainingData[trainingPointIndex].InputParameters;
            return InputDistance(v1, v2);
        }

        /// <summary>Returns measure of distance between the specified neural output values vector and vector of output
        /// values of the training point with the specified index.</summary>
        /// <param name="v1">First vector in space of output values for calculating distance.</param>
        /// <param name="trainingPointIndex">Index of training point for obtaining the second output vector.</param>
        public virtual double OutputDistance(IVector v1, int trainingPointIndex)
        {
            if (trainingPointIndex < 0 || trainingPointIndex >= NumAllTrainingPoints)
                throw new ArgumentException("Training point index (" + trainingPointIndex
                    + ") is out of range. Number of all training points: " + NumAllTrainingPoints + ".");
            IVector v2 = NeuralTrainingData[trainingPointIndex].OutputValues;
            return OutputDistance(v1, v2);
        }

        /// <summary>Returns index of training element with the shortest distance of its input parameters to the
        /// specified vector.</summary>
        /// <param name="v1">Vector to which the distance is observed.</param>
        /// <param name="countVerificationPoints">Whether verification points are included or not.</param>
        public virtual int GetClosestInputIndex(IVector v1, bool includeVerificationPoints)
        {
            int ret = -1;
            double distance = double.MaxValue;
            for (int i = 0; i < NumAllTrainingPoints; ++i)
            {
                if (includeVerificationPoints || !IsVerificationPoint(i))
                {
                    double currentDistance = InputDistance(v1, i);
                    if (currentDistance < distance)
                    {
                        ret = i;
                        distance = currentDistance;
                    }
                }
            }
            return ret;
        }

        /// <summary>Returns index of training element with the shortest distance of its output values to the
        /// specified vector of output  values.</summary>
        /// <param name="v1">Vector to which the distance is observed.</param>
        /// <param name="countVerificationPoints">Whether verification points are included or not.</param>
        public virtual int GetClosestOutputIndex(IVector v1, bool includeVerificationPoints)
        {
            int ret = -1;
            double distance = double.MaxValue;
            for (int i = 0; i < NumAllTrainingPoints; ++i)
            {
                if (includeVerificationPoints || !IsVerificationPoint(i))
                {
                    double currentDistance = OutputDistance(v1, i);
                    if (currentDistance < distance)
                    {
                        ret = i;
                        distance = currentDistance;
                    }
                }
            }
            return ret;
        }


        /* REMARKS ON CLOSEST POINST TEST:
         *   These tests take a specified number of points, then for all of these poinst a specified number N of
         * their closest training points are found (according to input distance formulation), these distances are 
         * written to console and statistics over all points checked is made. 
         *   Beside distances of first N closest points, vector differences may also be written (in this case 
         * statistics is also made on absolute values of elements of vector differences).
         * 
         * There are two basic types of these methods, one checks for closest traing points of an arbitrary  
         * vector in  input parameter space, and the second one checks for closest points of a specified set
         * of training points. The difference is that the second method also checks distances in output values
         * space, which enables to check whether closest training points in input parameters space aso have
         * similar output values. This can be used as first INDICATOR ON HOW GOOD THE TRAINING SET IS.
         * 
         * Instead of specifying which points in the input parameter space are checked, user can also specify
         * how many points should be taken and the points are then generated randomly (to be implemented!).
         * 
         */


        /// <summary>For each point in the specified array, the training points are sorted according to the 
        /// distance to this point, and data for the specified number of closest points are written.
        /// <para></para>
        /// <para>Result of this test can give some rough feeling about filling of space (but very rough 
        /// because anisotropy can not be detected in this way).</para></summary>
        /// <param name="numClosestPoints">Number of closest points that are written.</param>
        /// <param name="includeVerificationPoints">Whether verification points are included or not.</param>
        /// <param name="printByComponents">If true then results are also printed by components.</param>
        /// <param name="points">Points that are checked for closest trainnig data points.</param>
        public void TestClosestPoints(int numClosestPoints, bool includeVerificationPoints,
            bool printByComponents, params IVector[] points)
        {
            TestClosestPoints(numClosestPoints, includeVerificationPoints,
                printByComponents, false /* printIndividualPointsCom */ , points);
        }

        /// <summary>For each point in the specified array, the training points are sorted according to the 
        /// distance to this point, and data for the specified number of closest points are written.
        /// <para>For individual points, differences are not printed by components.</para>
        /// <para>Result of this test can give some rough feeling about filling of space (but very rough 
        /// because anisotropy can not be detected in this way).</para></summary>
        /// <param name="numClosestPoints">Number of closest points that are written.</param>
        /// <param name="includeVerificationPoints">Whether verification points are included or not.</param>
        /// <param name="printByComponents">If true then results are also printed by components.</param>
        /// <param name="printIndividualPointsComp">If true then individual components of differences are printed for
        /// individual points (otherwise, they are only printed in statistics).</param>
        /// <param name="points">Points that are checked for closest trainnig data points.</param>
        public void TestClosestPoints(int numClosestPoints, bool includeVerificationPoints,
            bool printByComponents, bool printIndividualPointsComp, params IVector[] points)
        {
            Console.WriteLine();
            Console.WriteLine("Checking closest trining data points for the specified set of points...");
            if (numClosestPoints < 1)
                throw new ArgumentException("Number of closest points to be written for each chosen points should be at least 1.");
            if (points == null)
                throw new ArgumentException("No points to be checked are specified.");
            int numPoints = points.Length;
            if (numPoints < 1)
                throw new ArgumentException("The number of specified points to be checked is less than 1.");
            SampledDataSet.ComparerInputDistance comparerInput = new SampledDataSet.ComparerInputDistance
                (points[0], InputDistance);
            List<SampledDataElement> trainingElements = GetTrainingElements(includeVerificationPoints);
            if (trainingElements.Count < numClosestPoints)
                numClosestPoints = trainingElements.Count;
            StopWatch1 t = new StopWatch1();
            IVector currentRelative = new Vector(NumNeuralParameters);       // current point in relative coordinates
            IVector closestRelative = new Vector(NumNeuralParameters);  // i-th closest point in relative coordinates
            IVector differenceRelative = new Vector(NumNeuralParameters);    // difference of the above

            IVector currentPoint = null;
            double[] minDistances = new double[numClosestPoints], maxDistances = new double[numClosestPoints],
                averageDistances = new double[numClosestPoints];

            IVector[] minAbsDif = null, maxAbsDif = null, averageAbsDif = null;
            if (printByComponents)
            {
                minAbsDif = new IVector[numClosestPoints];
                maxAbsDif = new IVector[numClosestPoints];
                averageAbsDif = new IVector[numClosestPoints];
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    minAbsDif[i] = new Vector(NumNeuralParameters);
                    maxAbsDif[i] = new Vector(NumNeuralParameters);
                    averageAbsDif[i] = new Vector(NumNeuralParameters);
                    for (int k = 0; k < NumNeuralParameters; ++k)
                    {
                        minAbsDif[i][k] = double.MaxValue;
                        maxAbsDif[i][k] = double.MinValue;
                        averageAbsDif[i][k] = 0;
                    }
                }
            }
            for (int i = 0; i < numClosestPoints; ++i)
            {
                minDistances[i] = double.MaxValue;
                maxDistances[i] = double.MinValue;
                averageDistances[i] = 0.0;
            }
            for (int iPoint = 0; iPoint < numPoints; ++iPoint)
            {
                currentPoint = points[iPoint];
                GetNeuralInputRelative(currentPoint, ref currentRelative);
                t.Start();
                // Sort training elemets according to their distance to the current point:
                comparerInput.ReferencePoint = currentPoint;
                trainingElements.Sort(comparerInput);
                t.Stop();
                Console.WriteLine();
                Console.WriteLine("Point No. " + iPoint + " (calculated in "
                    + t.Time.ToString("0.#####") + " s):");
                Console.WriteLine("Coordinates: ");
                Console.WriteLine("  " + currentPoint.ToString("#.####e0"));
                Console.WriteLine("  Relative coordinates: ");
                Console.WriteLine("  " + currentRelative.ToString("0.####"));
                if (printByComponents)
                {
                    if (printIndividualPointsComp)
                    {
                        Console.WriteLine();
                        Console.WriteLine("The first " + numClosestPoints + " closest points from the training set with ");
                        Console.WriteLine("differences (component-wise) expressed in relative coordinates:");
                    }
                    for (int i = 0; i < numClosestPoints; ++i)
                    {
                        SampledDataElement element = trainingElements[i];
                        double inputDistance = comparerInput.Distance(element, currentPoint);
                        GetNeuralInputRelative(element.InputParameters, ref closestRelative);
                        Vector.Subtract(closestRelative, currentRelative, ref differenceRelative);
                        if (printIndividualPointsComp)
                            Console.WriteLine("{0,4}.: {1}", i, differenceRelative.ToString("0.####"));
                        // Calculate statistics on indivitual components, for i-th closest point:
                        for (int k = 0; k < differenceRelative.Length; ++k)
                        {
                            double dif = Math.Abs(differenceRelative[k]);  // here we take absolute values of difference components
                            differenceRelative[k] = dif;
                            if (dif < minAbsDif[i][k])
                                minAbsDif[i][k] = dif;
                            if (dif > maxAbsDif[i][k])
                                maxAbsDif[i][k] = dif;
                            averageAbsDif[i][k] += dif / (double)numPoints;
                        }
                    }  // work over specified number of closest points to the current specified point
                }
                Console.WriteLine("The first " + numClosestPoints + " closest points from the training set with disances: ");
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    SampledDataElement element = trainingElements[i];
                    double inputDistance = comparerInput.Distance(element, currentPoint);
                    Console.WriteLine("{0,4}: d = {1,-10:0.0000e0}, el. index: {2,-4}", i, inputDistance, element.Index);
                    if (inputDistance < minDistances[i])
                        minDistances[i] = inputDistance;
                    if (inputDistance > maxDistances[i])
                        maxDistances[i] = inputDistance;
                    averageDistances[i] += inputDistance / (double)numPoints;
                }  // work over specified number of closest points to the current specified point
            }  // iterate over specified points
            if (printByComponents)
            {
                Console.WriteLine();
                Console.WriteLine("Statistics on closest points absolute differences in componets (relative coordinates):");
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    string formatString = "0.00e0";
                    Console.WriteLine("*{0} -th closest point: ", i);
                    Console.WriteLine("  Minimal absolute difference of relative components:");
                    Console.WriteLine("    " + minAbsDif[i].ToString(formatString));
                    Console.WriteLine("  Maximal absolute difference of relative components:");
                    Console.WriteLine("    " + maxAbsDif[i].ToString(formatString));
                    Console.WriteLine("  Average absolute difference of relative components:");
                    Console.WriteLine("    " + averageAbsDif[i].ToString(formatString));
                }  // work over specified number of closest points to the current specified point
            }
            Console.WriteLine();
            Console.WriteLine("Statistics on closest points distances: ");
            Console.WriteLine("{0,10} {1,-10:0.0000E0}   {2,10:0.0000E0}    {3,-10:0.0000E0}",
                "No. ", "min. d", "max. d", "average");
            for (int i = 0; i < numClosestPoints; ++i)
            {
                Console.WriteLine("{0,8}.: {1,-10:0.0000E0} - {2,10:0.0000E0};   {3,-10:0.0000E0}",
                    i, minDistances[i], maxDistances[i], averageDistances[i]);
            }
            Console.WriteLine();
            Console.WriteLine("Total time for all calculations ({0} test points, {1} closest points): {2:0.####} s",
                numPoints, numClosestPoints, t.TotalTime);
            Console.WriteLine();
        }


        /// <summary>For each point (training element) in the specified array, the training points are 
        /// sorted according to the distance to this point, and data for the specified number of closest points are written.
        /// <para>For individual points, differences are not printed by components.</para>
        /// <para>Result of this test can give some rough feeling about filling of space (but very rough 
        /// because anisotropy can not be detected in this way).</para></summary>
        /// <param name="numClosestPoints">Number of closest points that are written.</param>
        /// <param name="includeVerificationPoints">Whether verification points are included or not.</param>
        /// <param name="printByComponents">If true then results are also printed by components.</param>
        /// <param name="points">Points that are checked for closest trainnig data points.</param>
        public void TestClosestPoints(int numClosestPoints, bool includeVerificationPoints,
            bool printByComponents, params SampledDataElement[] points)
        {
            TestClosestPoints(numClosestPoints, includeVerificationPoints,
                printByComponents, false /* printIndividualPointsComp */, points);
        }

        /// <summary>For each point (training element) in the specified array, the training points are 
        /// sorted according to the distance to this point, and data for the specified number of closest points are written.
        /// <para></para>
        /// <para>Result of this test can give some rough feeling about filling of space (but very rough 
        /// because anisotropy can not be detected in this way).</para></summary>
        /// <param name="numClosestPoints">Number of closest points that are written.</param>
        /// <param name="includeVerificationPoints">Whether verification points are included or not.</param>
        /// <param name="printByComponents">If true then results are also printed by components.</param>
        /// <param name="printIndividualPoints">If true then individual components of differences are printed for
        /// <param name="points">Points that are checked for closest trainnig data points.</param>
        public void TestClosestPoints(int numClosestPoints, bool includeVerificationPoints,
            bool printByComponents, bool printIndividualPointsComp, params SampledDataElement[] points)
        {
            Console.WriteLine();
            Console.WriteLine("Checking closest trining data points for the specified set of training data points...");
            if (numClosestPoints < 1)
                throw new ArgumentException("Number of closest points to be written for each chosen points should be at least 1.");
            if (points == null)
                throw new ArgumentException("No points to be checked are specified.");
            int numPoints = points.Length;
            if (numPoints < 1)
                throw new ArgumentException("The number of specified points to be checked is less than 1.");
            SampledDataSet.ComparerInputDistance comparerInput = new SampledDataSet.ComparerInputDistance
                (points[0].InputParameters, InputDistance);
            SampledDataSet.ComparerOutputDistance comparerOutput = new SampledDataSet.ComparerOutputDistance
                (points[0].OutputValues, OutputDistance);
            List<SampledDataElement> trainingElements = GetTrainingElements(includeVerificationPoints);
            if (trainingElements.Count < numClosestPoints + 1)
                numClosestPoints = trainingElements.Count - 1;
            StopWatch1 t = new StopWatch1();
            IVector currentRelative = new Vector(NumNeuralParameters);       // current point in relative coordinates
            IVector closestRelative = new Vector(NumNeuralParameters);  // i-th closest point in relative coordinates
            IVector differenceRelative = new Vector(NumNeuralParameters);    // difference of the above
            // Vectors of output values in relative coordinates:
            IVector currentOutRelative = new Vector(NumNeuralParameters);       // current point in relative coordinates
            IVector closestOutRelative = new Vector(NumNeuralParameters);  // i-th closest point in relative coordinates
            IVector differenceOutRelative = new Vector(NumNeuralParameters);    // difference of the above

            IVector currentPoint = null;
            IVector currentOutput = null;
            double[] minDistances = new double[numClosestPoints], maxDistances = new double[numClosestPoints],
                averageDistances = new double[numClosestPoints];
            double[] minOutDistances = new double[numClosestPoints], maxOutDistances = new double[numClosestPoints],
                averageOutDistances = new double[numClosestPoints];
            IVector[] minAbsDif = null, maxAbsDif = null, averageAbsDif = null;
            IVector[] minAbsOutDif = null, maxAbsOutDif = null, averageAbsOutDif = null;
            for (int i = 0; i < numClosestPoints; ++i)
            {
                minDistances[i] = double.MaxValue;
                maxDistances[i] = double.MinValue;
                averageDistances[i] = 0.0;
                minOutDistances[i] = double.MaxValue;
                maxOutDistances[i] = double.MinValue;
                averageOutDistances[i] = 0.0;
            }
            if (printByComponents)
            {
                minAbsDif = new IVector[numClosestPoints];
                maxAbsDif = new IVector[numClosestPoints];
                averageAbsDif = new IVector[numClosestPoints];
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    minAbsDif[i] = new Vector(NumNeuralParameters);
                    maxAbsDif[i] = new Vector(NumNeuralParameters);
                    averageAbsDif[i] = new Vector(NumNeuralParameters);
                    for (int k = 0; k < NumNeuralParameters; ++k)
                    {
                        minAbsDif[i][k] = double.MaxValue;
                        maxAbsDif[i][k] = double.MinValue;
                        averageAbsDif[i][k] = 0;
                    }
                }
                minAbsOutDif = new IVector[numClosestPoints];
                maxAbsOutDif = new IVector[numClosestPoints];
                averageAbsOutDif = new IVector[numClosestPoints];
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    minAbsOutDif[i] = new Vector(NumNeuralOutputs);
                    maxAbsOutDif[i] = new Vector(NumNeuralOutputs);
                    averageAbsOutDif[i] = new Vector(NumNeuralOutputs);
                    for (int k = 0; k < NumNeuralOutputs; ++k)
                    {
                        minAbsOutDif[i][k] = double.MaxValue;
                        maxAbsOutDif[i][k] = double.MinValue;
                        averageAbsOutDif[i][k] = 0;
                    }
                }
            }
            for (int iPoint = 0; iPoint < numPoints; ++iPoint)
            {
                SampledDataElement currentElement = points[iPoint];
                currentPoint = currentElement.InputParameters;
                currentOutput = points[iPoint].OutputValues;
                GetNeuralInputRelative(currentPoint, ref currentRelative);
                GetNeuralOutputRelative(currentOutput, ref currentOutRelative);
                t.Start();
                // Sort training elemets according to their distance to the current point:
                comparerInput.ReferencePoint = currentPoint;
                trainingElements.Sort(comparerInput);
                t.Stop();
                Console.WriteLine();
                Console.WriteLine("Point No. " + iPoint + " (element index " + currentElement.Index
                    + ", calculated in " + t.Time.ToString("0.#####") + " s):");
                Console.WriteLine("Coordinates: ");
                Console.WriteLine("  " + currentPoint.ToString("#.####e0"));
                Console.WriteLine("  Relative coordinates: ");
                Console.WriteLine("  " + currentRelative.ToString("0.####"));
                if (printByComponents)
                {
                    if (printIndividualPointsComp)
                    {
                        Console.WriteLine();
                        Console.WriteLine("The first " + numClosestPoints + " closest points from the training set with ");
                        Console.WriteLine("differences (component-wise) expressed in relative coordinates:");
                    }
                    for (int i = 0; i < numClosestPoints; ++i)
                    {
                        SampledDataElement element;
                        if (trainingElements[0] == currentElement)
                            element = trainingElements[i + 1];  // the first element must be skipped because it is the same as reference element
                        else
                            element = trainingElements[i];
                        //double inputDistance = comparerInput.Distance(element, currentPoint);
                        GetNeuralInputRelative(element.InputParameters, ref closestRelative);
                        Vector.Subtract(closestRelative, currentRelative, ref differenceRelative);
                        // Vector of differences in relative coordinates for output vector:
                        GetNeuralOutputRelative(element.OutputValues, ref closestOutRelative);
                        Vector.Subtract(closestOutRelative, currentOutRelative, ref differenceOutRelative);
                        if (printIndividualPointsComp)
                        {
                            Console.WriteLine("{0,4}.: {1}", i, differenceRelative.ToString("0.####"));
                            Console.WriteLine("{0,10} {1}", "out:", differenceOutRelative.ToString("0.####"));
                        }
                        // Calculate statistics on indivitual components, for i-th closest point:
                        for (int k = 0; k < differenceRelative.Length; ++k)
                        {
                            double dif = Math.Abs(differenceRelative[k]);  // here we take absolute values of difference components
                            differenceRelative[k] = dif;
                            if (dif < minAbsDif[i][k])
                                minAbsDif[i][k] = dif;
                            if (dif > maxAbsDif[i][k])
                                maxAbsDif[i][k] = dif;
                            averageAbsDif[i][k] += dif / (double)numPoints;
                        }
                        // Calculate statistics on indivitual components, for i-th closest point, for OUTPUT:
                        for (int k = 0; k < differenceOutRelative.Length; ++k)
                        {
                            double dif = Math.Abs(differenceOutRelative[k]);
                            differenceOutRelative[k] = dif;
                            if (dif < minAbsOutDif[i][k])
                                minAbsOutDif[i][k] = dif;
                            if (dif > maxAbsOutDif[i][k])
                                maxAbsOutDif[i][k] = dif;
                            averageAbsOutDif[i][k] += dif / (double)numPoints;
                        }
                    }  // work over specified number of closest points to the current specified point
                }
                Console.WriteLine("The first " + numClosestPoints + " closest points from the training set with disances: ");
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    // NeuralTrainingElement element = trainingElements[i];
                    SampledDataElement element;
                    if (trainingElements[0] == currentElement)
                        element = trainingElements[i + 1];  // the first element must be skipped because it is the same as reference element
                    else
                        element = trainingElements[i];
                    double inputDistance = comparerInput.Distance(element, currentPoint);
                    double outputDistance = comparerOutput.Distance(element, currentOutput);
                    Console.WriteLine("{0,4}: d = {1,-10:0.0000e0}, out. d = {2,-8:0.0000e0}, el. index: {3,-4}",
                        i, inputDistance, outputDistance, element.Index);
                    if (inputDistance < minDistances[i])
                        minDistances[i] = inputDistance;
                    if (inputDistance > maxDistances[i])
                        maxDistances[i] = inputDistance;
                    averageDistances[i] += inputDistance / (double)numPoints;
                    if (outputDistance < minOutDistances[i])
                        minOutDistances[i] = outputDistance;
                    if (outputDistance > maxOutDistances[i])
                        maxOutDistances[i] = outputDistance;
                    averageOutDistances[i] += outputDistance / (double)numPoints;
                }  // work over specified number of closest points to the current specified point
            }  // iterate over specified points
            if (printByComponents)
            {
                Console.WriteLine();
                Console.WriteLine("Statistics on closest points absolute differences in componets (relative coordinates):");
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    string formatString = "0.00e0";
                    Console.WriteLine("*{0} -th closest point: ", i);
                    Console.WriteLine("  Minimal absolute difference of relative components:");
                    Console.WriteLine("    " + minAbsDif[i].ToString(formatString));
                    Console.WriteLine("  Maximal absolute difference of relative components:");
                    Console.WriteLine("    " + maxAbsDif[i].ToString(formatString));
                    Console.WriteLine("  Average absolute difference of relative components:");
                    Console.WriteLine("    " + averageAbsDif[i].ToString(formatString));
                    Console.WriteLine("  OUTPUT:");
                    Console.WriteLine("  Minimal absolute difference of relative components:");
                    Console.WriteLine("    " + minAbsOutDif[i].ToString(formatString));
                    Console.WriteLine("  Maximal absolute difference of relative components:");
                    Console.WriteLine("    " + maxAbsOutDif[i].ToString(formatString));
                    Console.WriteLine("  Average absolute difference of relative components:");
                    Console.WriteLine("    " + averageAbsOutDif[i].ToString(formatString));
                }  // work over specified number of closest points to the current specified point
            }
            Console.WriteLine();
            Console.WriteLine("Statistics on closest points distances: ");
            Console.WriteLine("{0,10} {1,-10:0.0000E0}   {2,10:0.0000E0}    {3,-10:0.0000E0}",
                "No. ", "min. d", "max. d", "average");
            for (int i = 0; i < numClosestPoints; ++i)
            {
                Console.WriteLine("{0,8}.: {1,-10:0.0000E0} - {2,10:0.0000E0};   {3,-10:0.0000E0}",
                    i, minDistances[i], maxDistances[i], averageDistances[i]);
            }
            Console.WriteLine("Statistics on closest points OUTPUT distances: ");
            Console.WriteLine("{0,10} {1,-10:0.0000E0}   {2,10:0.0000E0}    {3,-10:0.0000E0}",
                "No. ", "min. d", "max. d", "average");
            for (int i = 0; i < numClosestPoints; ++i)
            {
                Console.WriteLine("{0,8}.: {1,-10:0.0000E0} - {2,10:0.0000E0};   {3,-10:0.0000E0}",
                    i, minOutDistances[i], maxOutDistances[i], averageOutDistances[i]);
            }
            Console.WriteLine();
            Console.WriteLine("Total time for all calculations ({0} test points, {1} closest points): {2:0.####} s",
                numPoints, numClosestPoints, t.TotalTime);
            Console.WriteLine();
        }  // TestClosestPoints(...)



        /// <summary>Calculates approximation at specified input parameters.</summary>
        /// <param name="inputParameters">Input parameters.</param>
        public virtual void NeuralCalculate(params double[] inputParameters)
        {
            NeuralCalculate(new Vector(inputParameters));
        }

        /// <summary>Calculates approximation at specified input parameters.</summary>
        /// <param name="inputParameters">Input parameters.</param>
        public virtual void NeuralCalculate(IVector inputParameters)
        {
            if (inputParameters.Length != TrainedNetwork.InputLength)
                throw new ArgumentException("Wrong number of input parameters, " + inputParameters.Length + " instead of " + TrainedNetwork.InputLength + ".");
            IVector output = new Vector(TrainedNetwork.OutputLength);
            TrainedNetwork.CalculateOutput(inputParameters, ref output);
            Console.WriteLine("Calculated approximation: ");
            Console.WriteLine("Input parameters: " + Environment.NewLine + " " + inputParameters);
            Console.WriteLine("Calculated output: " + Environment.NewLine + " " + output);
        }

        /// <summary>Calculates approximation at specified input parameters.</summary>
        /// <param name="inputParameters">Input parameters.</param>
        public virtual void NeuralCalculate(IVector inputParameters, ref IVector outputValues)
        {
            TrainedNetwork.CalculateOutput(inputParameters, ref outputValues);
        }


        // SETS OF RANDOM POINTS:

        /// <summary>Creates and returns a list of training elements that are randomly chosen from the 
        /// current training set. Trainingpoints that are not verification points are always included
        /// in the set of points for selection.
        /// <para>The global random generator is used to select training elements.</para></summary>
        /// <param name="numElements">Number of elemnts that are randomly selected and put into the 
        /// returned list. If the number of all available elements is smaller than this number then
        /// exception is thrown.</param>
        /// <param name="includeVerificationPoints">A flag indicationg whether verification elements should
        /// also be included.</param>
        public virtual List<SampledDataElement> GetRandomTrainingElements(int numElements,
            bool includeVerificationPoints)
        {
            return GetRandomTrainingElements(Random, numElements,
                includeVerificationPoints);
        }

        /// <summary>Creates and returns a list of training elements that are randomly chosen from the 
        /// current training set.
        /// <para>The global random generator is used to select training elements.</para></summary>
        /// <param name="numElements">Number of elemnts that are randomly selected and put into the 
        /// returned list. If the number of all available elements is smaller than this number then
        /// exception is thrown.</param>
        /// <param name="includeVerificationPoints">A flag indicationg whether verification elements should
        /// also be included.</param>
        /// <param name="includeNonVerificationPoints">A flag indicating whether non-verification elements
        /// should be included in the list.</param>
        public virtual List<SampledDataElement> GetRandomTrainingElements(int numElements,
            bool includeVerificationPoints, bool includeNonVerificationPoints)
        {
            return GetRandomTrainingElements(Random, numElements,
                includeVerificationPoints, includeNonVerificationPoints);
        }

        /// <summary>Creates and returns a list of training elements that are randomly chosen from the 
        /// current training set. Trainingpoints that are not verification points are always included
        /// in the set of points for selection.</summary>
        /// <param name="rand">Random generator used for random selection of training elements.</param>
        /// <param name="numElements">Number of elemnts that are randomly selected and put into the 
        /// returned list. If the number of all available elements is smaller than this number then
        /// exception is thrown.</param>
        /// <param name="includeVerificationPoints">A flag indicationg whether verification elements should
        /// also be included.</param>
        public virtual List<SampledDataElement> GetRandomTrainingElements(IRandomGenerator rand, int numElements,
            bool includeVerificationPoints)
        {
            return GetRandomTrainingElements(rand, numElements,
                includeVerificationPoints, true /* includeNonVerificationPoints */ );
        }

        /// <summary>Creates and returns a list of training elements that are randomly chosen from the 
        /// current training set.</summary>
        /// <param name="rand">Random generator used for random selection of training elements.</param>
        /// <param name="numElements">Number of elemnts that are randomly selected and put into the 
        /// returned list. If the number of all available elements is smaller than this number then
        /// exception is thrown.</param>
        /// <param name="includeVerificationPoints">A flag indicationg whether verification elements should
        /// also be included.</param>
        /// <param name="includeNonVerificationPoints">A flag indicating whether non-verification elements
        /// should be included in the list.</param>
        public virtual List<SampledDataElement> GetRandomTrainingElements(IRandomGenerator rand, int numElements,
            bool includeVerificationPoints, bool includeNonVerificationPoints)
        {
            if (rand == null)
                throw new ArgumentException("Random generator for selection of training elements is not specified (null reference).");
            if (numElements <= 0)
            {
                if (numElements == 0)
                    return new List<SampledDataElement>();
                else
                    throw new ArgumentException("Number of randomly selected elements should be greater than or equal to 0 (required: "
                        + numElements + ").");
            }
            List<SampledDataElement> source = GetTrainingElements(includeVerificationPoints, includeNonVerificationPoints);
            if (numElements > source.Count)
                throw new ArgumentException("The specified number of random elements in the list of training elements " + numElements
                    + Environment.NewLine + " is greater than the number of all available elements (" + source.Count + ").");
            List<SampledDataElement> ret = new List<SampledDataElement>();
            for (int i = 0; i < numElements; ++i)
            {
                // select hext element:
                int whichElement = rand.Next(0, source.Count - 1);
                ret.Add(source[whichElement]);
                source.RemoveAt(whichElement);
            }
            return ret;
        }



        /// <summary>Creates and returns a random vector of neural input parameters whose elements (components) lie
        /// within the lower and upper bounds on parameters.
        /// <para>Global random generator is used to generate vecor components.</para></summary>
        public virtual IVector GetRandomNeuralInput()
        {
            return GetRandomNeuralInput(Random);
        }


        /// <summary>Generates a random vector of neural input parameters whose elements (components) lie
        /// within the lower and upper bounds on parameters, and stores it to the specified vector.</summary>
        /// <param name="rand">Random number generator used to generate the elements.</param>
        /// <param name="result">Vector object whre the generated random vector is stored; allocated if necessary.</param>
        public virtual void GetRandomNeuralInput(IRandomGenerator rand, ref IVector result)
        {
            NeuralInputBounds.GetRandomPoint(ref result, rand);
        }


        /// <summary>Creates and returns a random vector of neural input parameters whose elements (components) lie
        /// within the lower and upper bounds on parameters.</summary>
        /// <param name="rand">Random number generator used to generate the elements.</param>
        public virtual IVector GetRandomNeuralInput(IRandomGenerator rand)
        {
            IVector ret = null;
            GetRandomNeuralInput(rand, ref ret);
            return ret;
        }

        /// <summary>Generates a random vector of neural input parameters whose elements (components) lie
        /// within the lower and upper bounds on parameters.</summary>
        public virtual void GetRandomNeuralInput(ref IVector result)
        {
            GetRandomNeuralInput(Random, ref result);
        }




        /// <summary>Generates a random vector of neural input parameters whose elements (components) lie
        /// within the lower and upper bounds on parameters, and stores it to the specified vector.</summary>
        /// <param name="rand">Random number generator used to generate the elements.</param>
        /// <param name="result">Vector object whre the generated random vector is stored; allocated if necessary.</param>
        protected virtual void GetRandomNeuralInputFromDataDefinition(IRandomGenerator rand, ref IVector result)
        {
            if (rand == null)
                throw new ArgumentException("Random generator is not specified (null argument).");
            bool allocate = false;
            if (result == null)
                allocate = true;
            else if (result.Length != TrainedNetwork.InputLength)
                allocate = true;
            if (allocate)
                result = new Vector(TrainedNetwork.InputLength);
            lock (Lock)
            {
                for (int i = 0; i < result.Length; ++i)
                {
                    InputElementDefinition element = NeuralDataDefinition.GetInputElement(i);
                    if (!element.BoundsDefined)
                        throw new InvalidDataException("Input element No. " + i + " does not have bounds defined.");
                    double scalingLength = element.MaximalValue - element.MinimalValue;
                    result[i] = rand.NextDouble(element.MinimalValue, element.MaximalValue);
                }
            }
        }



        /// <summary>Creates and returns a random vector of neural output values whose elements (components) lie
        /// within the lower and upper bounds on values.
        /// <para>Global random generator is used to generate vecor components.</para></summary>
        public virtual IVector GetRandomNeuralOutput()
        {
            return GetRandomNeuralOutput(Random);
        }

        /// <summary>Creates and returns a random vector of neural output values whose elements (components) lie
        /// within the lower and upper bounds on values.</summary>
        /// <param name="rand">Random number generator used to generate the elements.</param>
        public virtual IVector GetRandomNeuralOutput(IRandomGenerator rand)
        {
            IVector ret = null;
            GetRandomNeuralOutput(rand, ref ret);
            return ret;
        }

        /// <summary>Generates a random vector of neural output values whose elements (components) lie
        /// within the lower and upper bounds on values.</summary>
        public virtual void GetRandomNeuralOutput(ref IVector result)
        {
            GetRandomNeuralOutput(Random, ref result);
        }

        /// <summary>Generates a random vector of neural output values whose elements (components) lie
        /// within the lower and upper bounds on neural output values, and stores it to the specified vector.</summary>
        /// <param name="rand">Random number generator used to generate the elements.</param>
        /// <param name="result">Vector object whre the generated random vector is stored; allocated if necessary.</param>
        public virtual void GetRandomNeuralOutput(IRandomGenerator rand, ref IVector result)
        {
            if (rand == null)
                throw new ArgumentException("Random generator is not specified (null argument).");
            bool allocate = false;
            if (result == null)
                allocate = true;
            else if (result.Length != TrainedNetwork.OutputLength)
                allocate = true;
            if (allocate)
                result = new Vector(TrainedNetwork.OutputLength);
            lock (Lock)
            {
                for (int i = 0; i < result.Length; ++i)
                {
                    OutputElementDefinition element = NeuralDataDefinition.GetOutputElement(i);
                    if (!element.BoundsDefined)
                        throw new InvalidDataException("Output element No. " + i + " does not have bounds defined.");
                    double scalingLength = element.MaximalValue - element.MinimalValue;
                    result[i] = rand.NextDouble(element.MinimalValue, element.MaximalValue);
                }
            }
        }

        IVector _someInput;

        /// <summary>Returns some vector of input parameters that is within the range.
        /// <para>This methods just reads and returns the current vector of input parameter from the directory 
        /// for the first time, and then returns the same vector every time.</para></summary>
        public virtual IVector SomeNeuralInput
        {
            get
            {
                if (_someInput == null)
                {
                    IVector v = new Vector(TrainedNetwork.InputLength);
                    NeuralFM.ReadNeuralInput(ref v);
                    _someInput = v;
                }
                return _someInput;
            }
        }

        /// <summary>Vector of input parameters read form the file.</summary>
        public virtual IVector NeuralInputFromFile
        {
            get
            {
                IVector v = new Vector(TrainedNetwork.InputLength);
                NeuralFM.ReadNeuralInput(ref v);
                return v;
            }
        }

        /// <summary> Prepares table of neurons in geometric sequence. </summary>
        /// <param name="minNeurons"> Minimum number of neurons. </param>
        /// <param name="maxNeurons"> Maximum number of neurons. </param>
        /// <param name="numNeurons"> Number of neurons in sequence. </param>
        /// <param name="neurpnsTable"> Table of neurons. </param>
        /// $A Tako78 Nov12;
        public virtual void PrepareNeuronsTable(int minNeurons, int maxNeurons, int numNeurons, ref int[] neurpnsTable)
        {
            double[] tmpNumNeurons = new double[numNeurons];
            neurpnsTable = new int[numNeurons];

            //Prepare table of neurons in hidden layers
            GridGenerator1d greedGenerator = new GridGenerator1d();
            greedGenerator.CoordinateFirst = minNeurons;
            greedGenerator.CoordinateLast = maxNeurons;
            greedGenerator.GrowthFactor = 2; ;
            greedGenerator.NumNodes = numNeurons;
            tmpNumNeurons = greedGenerator.GetNodeTable();

            for (int i = 0; i < tmpNumNeurons.Length; i++)
            {
                neurpnsTable[i] = (int)tmpNumNeurons[i];
            }
        }

#endregion NeuralData


#region NeuralTraining

        /// <summary>Train the artificial neural network.</summary>
        /// <param name="annType">1 - NeuronDotNet, 2 - Aforge.</param>
        /// <param name="NumNeurons">Number of neirons in 1st hidden layer.</param>
        /// <param name="MaxEpochs">Max epochs.</param>
        /// <param name="EpochsInBundle">Epochs in bundle.</param>
        /// <param name="LearnignRate">Learning rate.</param>
        /// <param name="Momentum">Momentum.</param>
        /// <param name="InputSafetyFactor">Input safety factor.</param>
        /// <param name="OutputSafetyFactor">Output safety factor.</param>
        /// <param name="PercentVerificationPoints">Percentage for verification points.</param>
        public void TrainANN(int annType, int NumNeurons, int MaxEpochs, int EpochsInBundle, double LearnignRate,
            double Momentum, double InputSafetyFactor, double OutputSafetyFactor, double PercentVerificationPoints)
        {
            string trainingDataFile = NeuralFM.NeuralTrainingDataPath;
            SampledDataSet trainingData = null;
            SampledDataSet.LoadJson(trainingDataFile, ref trainingData);

            trainingData.RemoveInputDuplicates();

            // Specify reasonable number of samples and verification points:
            int numTrainingElements = trainingData.Length;

            int numVerificationPoints = (int)Math.Round((double)numTrainingElements * PercentVerificationPoints);
            // numTrainingElements += numVerificationPoints;

            // Create training data by randomly sampling a specific quadratic response:
            SampledDataSet sarze = trainingData;

            // Speciy which samples will be used for verification of approximation:
            IndexList verificationIndices = IndexList.CreateRandom(numVerificationPoints, 0 /* lowerbound */, numTrainingElements - 1);

            INeuralApproximator approximator = null;
            // TODO: Train network on the training data!

            //double lowerInputRange = -2.0;
            //double upperInputRange = 2.0;
            //double lowerOutputRange = 0.0;
            //double upperOutputRange = 1.0;

            approximator = CreateApproximator(annType);

            //if (annType == 1) // NeuronDotNet
            //{
            //    approximator = new NeuralApproximatorNeuron();
            //    lowerInputRange = -2.0;
            //    upperInputRange = 2.0;
            //    lowerOutputRange = 0.0;
            //    upperOutputRange = 1.0;
            //    approximator.SigmoidAlphaValue = 2;
            //}
            //else if (annType == 2) // Aforge
            //{
            //    approximator = new NeuralApproximatorAforge();
            //    lowerInputRange = -2.0;
            //    upperInputRange = 2.0;
            //    lowerOutputRange = -1.0;
            //    upperOutputRange = 1.0;
            //    approximator.SigmoidAlphaValue = 1.3;
            //}

            approximator.OutputLevel = 2;
            approximator.InputLength = trainingData.InputLength;
            approximator.OutputLength = trainingData.OutputLength;

            // Set prepared data:
            approximator.TrainingData = sarze;
            approximator.VerificationIndices = verificationIndices;

            // Set network layout:
            approximator.MultipleNetworks = true;
            approximator.SetHiddenLayers(NumNeurons);

            // Set training parameters:
            approximator.MaxEpochs = MaxEpochs;
            approximator.EpochsInBundle = EpochsInBundle;
            approximator.ToleranceRms = new Vector(trainingData.OutputLength, 0.000001);

            // Specify learning parameters:
            approximator.LearningRate = LearnignRate;
            //neuralNeuron.SigmoidAlphaValue = 2;
            approximator.Momentum = Momentum;

            // Specify parameters defining the bounds for data applied to input and output neurons:
            approximator.InputBoundsSafetyFactor = InputSafetyFactor;
            approximator.OutputBoundsSafetyFactor = OutputSafetyFactor;

            // Test output (for debugging purposes):
            Console.WriteLine();
            Console.WriteLine("Neural network data: ");
            Console.WriteLine(approximator.ToString());
            Console.WriteLine("Insert <Enter> in order to continue: ");

            // Perform training: 
            //StopWatch1 watch = new StopWatch1();
            //watch.Start();
            approximator.TrainNetwork();
            //watch.Stop();

            // Calculate outputs and exact values for first 5 points in the training set:
            Console.WriteLine("A couple of calculated outputs from the training set (including verification points):");
            for (int i = 0; i < 5; ++i)
            {
                IVector input = sarze.GetInputParameters(i);
                IVector exactOutput = sarze.GetOutputValues(i);
                IVector calculatedOutput = new Vector(trainingData.OutputLength);
                approximator.CalculateOutput(input, ref calculatedOutput);
                Console.WriteLine();
                Console.WriteLine("Point No. " + i + "of the training set, is verification point: " + verificationIndices.Contains(i));
                Console.WriteLine("  Input parameters No. " + i + ":");
                Console.WriteLine("  " + input.ToString());
                Console.WriteLine("  Exact output: ");
                Console.WriteLine("  " + exactOutput.ToString());
                Console.WriteLine("  Approximated output: ");
                Console.WriteLine("  " + calculatedOutput.ToString());
                IVector dif = null;
                Vector.Subtract(calculatedOutput, exactOutput, ref dif);
                Console.WriteLine("Norm of the difference between sampled and calculated output: " + dif.Norm);
            }
            Console.WriteLine("");
            //Console.WriteLine("Training time: " + watch.Time);

            // Save the file on standard location (trainednetwork.json):
            string trainedNetworkFile = NeuralFM.NeuralNetworkPath;  // path to file where the network wil be stored...
            //NeuralApproximatorBaseExt.SaveJson(approximator, trainedNetworkFile);
            this.SaveJson(approximator, trainedNetworkFile);
        }

#endregion NeuralTraining


#region NeuralModelDistortion
        // This region provides tools for creatino of distorted models. These morels are
        // used as replacement for industrial models for shippment with demonstration 
        // code in cases where true models may not be used since they contain confidential
        // data.



        /// <summary>Creates data for distorted model and stores the data in the specified directory.
        /// <para>Data of the current ANN model (read on demand from the standard locations of the model directory)
        /// is used as original data. The method chnages boundaries of input and output elements (eventually with the 
        /// boundaries provided in a data definition file), creates and writes new data definition, then randomly
        /// samples the specified number of points by the original model, transforms input parameters and output
        /// values of hte transformed points in order to fit the new prescribed boundaries, and stores the new
        /// sampled data in the specified directory.</para>
        /// <para>Ifthe directory where distorted model data is stored already exists, then the user is prompted 
        /// whether to overwrite its contents or not.</para></summary>
        /// <param name="arguments">Command arguments, first one (index 0) is always command name.
        /// <para>Arguments are:</para>
        /// <para>  WorkingDirectory - working directory where the original model is found.</para>
        /// <para>  DistortedModelDirectory - directory where distorted model is stored. Specified as absolute path or relative to the current directory.</para>
        /// <para>  NumDataPoints - number of data points in the sampled data generated by the original model.</para>
        /// <para>  ChangeElementNames - optional, default false; whether data element names are changed.</para>
        /// <para>  ChangeTitlesAndDescriptions - optional, default true; whether element titles and descriptions are changed.</para>
        /// <para>  DataDefinitionFile - optional; data definition file for distorted model. If specified then new parameter 
        /// bounds and other data (names, titles, descriptions, etc.) are taken from here, otherwise data is generated as
        /// necessary.</para>
        /// <para>DistortionFactor - optional, default 2.5 - factor by which bounds are multiplied.</para>
        /// <para>RandomFactor - ouptional, default 0.25 - mahnitude of additional random shiht of bounds 
        /// (relative to distorted interval length).</para> </param>
        /// <returns> null. </returns>
        public virtual string CreateDistortedModelData(string[] arguments)
        {
            string distortedDirectoryPath = null;
            int numDatapoints = 10000;
            bool changeElementNames = false;
            bool changeTitlesAndDescriptions = true;
            string dataDefinitionFilePath = null;
            double distortionFactor = 2.5;
            double randomFactor = 0.25;

            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Creating sampled data and definitions for a distorted model..."
                + Environment.NewLine + Environment.NewLine);

            // Interpret arguments:
            if (arguments.Length > 1)
            {
                try
                {
                    distortedDirectoryPath = arguments[1];
                    Console.WriteLine("Argument DistortedDirectoryPath: " + distortedDirectoryPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured when parsing argument 'DistortedDirectoryPath': "
                        + Environment.NewLine + ex.Message);
                }
            }
            else
            {
                throw new ArgumentException("Path of the distorted model directory is not specified.");
            }
            if (arguments.Length > 2)
            {
                try
                {
                    numDatapoints = int.Parse(arguments[2]);
                    Console.WriteLine("Argument numDatapoints: " + numDatapoints.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured when parsing boolean argument 'numDatapoints': "
                        + Environment.NewLine + ex.Message);
                }
            }
            if (arguments.Length > 3)
            {
                try
                {
                    Util.TryParseBoolean(arguments[3], ref changeElementNames);
                    Console.WriteLine("Argument ChangeElementNames: " + changeElementNames.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured when parsing boolean argument 'ChangeElementNames': "
                        + Environment.NewLine + ex.Message);
                }
            }
            if (arguments.Length > 4)
            {
                try
                {
                    Util.TryParseBoolean(arguments[4], ref changeTitlesAndDescriptions);
                    Console.WriteLine("Argument ChangeTitlesAndDescriptions: " + changeTitlesAndDescriptions.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured when parsing boolean argument 'ChangeElementNames': "
                        + Environment.NewLine + ex.Message);
                }
            }
            if (arguments.Length > 5)
            {
                try
                {
                    dataDefinitionFilePath = arguments[5];
                    Console.WriteLine("Argument DataDefinitionFilePath: " + dataDefinitionFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured when parsing argument 'DataDefinitionFilePath': "
                        + Environment.NewLine + ex.Message);
                }
            }

            if (arguments.Length > 6)
            {
                try
                {
                    double.TryParse(arguments[6], out  distortionFactor);
                    Console.WriteLine("Argument distortionFactor: " + distortionFactor.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured when parsing boolean argument 'distortionFactor': "
                        + Environment.NewLine + ex.Message);
                }
            }
            if (arguments.Length > 7)
            {
                try
                {
                    double.TryParse(arguments[7], out  randomFactor);
                    Console.WriteLine("Argument randomFactor: " + randomFactor.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured when parsing boolean argument 'randomFactor': "
                        + Environment.NewLine + ex.Message);
                }
            }
            Console.WriteLine(Environment.NewLine
                + "Arguments of the function:     " + Environment.NewLine
                + "  distortedDirectoryPath:      " + distortedDirectoryPath + Environment.NewLine
                + "  changeElementNames:          " + changeElementNames + Environment.NewLine
                + "  changeTitlesAndDescriptions: " + changeTitlesAndDescriptions + Environment.NewLine
                + "  dataDefinitionFilePath:      " + dataDefinitionFilePath + Environment.NewLine
                + "  distortionFactor:            " + distortionFactor + Environment.NewLine
                + "  randomFactor:                " + randomFactor + Environment.NewLine);
            Console.WriteLine("Original data definition path: " + Environment.NewLine
                + "  " + NeuralFM.NeuralDataDefinitionPath);

            string targetDataDefinitionPath = Path.Combine(distortedDirectoryPath, NeuralFileConst.NeuralDataDefinitionFilename);
            string targetTrainingDataPath = Path.Combine(distortedDirectoryPath, NeuralFileConst.NeuralTrainingDataFilename);
            string targetVerificationDataPath = Path.Combine(distortedDirectoryPath, NeuralFileConst.NeuralVerificationDataFilename);
            Console.WriteLine(Environment.NewLine
                + "Data definition file: " + Environment.NewLine + "  " + targetDataDefinitionPath + Environment.NewLine
                + "Data file: " + Environment.NewLine + "  " + targetTrainingDataPath + Environment.NewLine);
            bool performCreation = true;
            if (File.Exists(targetDataDefinitionPath) || File.Exists(targetTrainingDataPath)
                || File.Exists(targetVerificationDataPath))
            {
                if (performCreation && File.Exists(targetDataDefinitionPath))
                {
                    performCreation = false;
                    Console.Write(Environment.NewLine + Environment.NewLine
                        + "WARNING: " + Environment.NewLine
                        + "Target data definition file already exists! " + Environment.NewLine
                        + "  File path: " + targetDataDefinitionPath + Environment.NewLine + Environment.NewLine
                        + "Would you like to overwrite the file (0/1)? ");
                    UtilConsole.Read(ref performCreation);
                    Console.WriteLine(Environment.NewLine);
                }
                if (performCreation && File.Exists(targetTrainingDataPath))
                {
                    performCreation = false;
                    Console.Write(Environment.NewLine + Environment.NewLine
                        + "WARNING: " + Environment.NewLine
                        + "Target data file already exists! " + Environment.NewLine
                        + "  File path: " + targetTrainingDataPath + Environment.NewLine + Environment.NewLine
                        + "Would you like to overwrite the file (0/1)? ");
                    UtilConsole.Read(ref performCreation);
                    Console.WriteLine(Environment.NewLine);
                }
                if (performCreation && File.Exists(targetVerificationDataPath))
                {
                    performCreation = false;
                    Console.Write(Environment.NewLine + Environment.NewLine
                        + "WARNING: " + Environment.NewLine
                        + "Target data file already exists! " + Environment.NewLine
                        + "  File path: " + targetVerificationDataPath + Environment.NewLine + Environment.NewLine
                        + "Would you like to overwrite the file (0/1)? ");
                    UtilConsole.Read(ref performCreation);
                    Console.WriteLine(Environment.NewLine);
                }
                if (!performCreation)
                {
                    Console.WriteLine(Environment.NewLine + Environment.NewLine
                        + "Creation of distorted model data and definitions ABORTED. " + Environment.NewLine);
                    return null;
                }
            }
            if (!Directory.Exists(distortedDirectoryPath))
                Directory.CreateDirectory(distortedDirectoryPath);
            if (!Directory.Exists(distortedDirectoryPath))
                throw new InvalidDataException("The distorted model directory can not be created. " + Environment.NewLine
                    + "  Directory path: " + distortedDirectoryPath);
            if (!performCreation)
                return null;
            // Create distorted data definition: 
            Console.WriteLine(Environment.NewLine + "Obtaining original data definition... " + Environment.NewLine
                + "  Data definition path: " + NeuralFM.NeuralDataDefinitionPath);
            InputOutputDataDefiniton originalDefinition = NeuralDataDefinition;
            Console.WriteLine("  ... obtaining original data definition done.");
            InputOutputDataDefiniton targetDefinition = null;
            if (!string.IsNullOrEmpty(dataDefinitionFilePath))
            {
                if (!File.Exists(dataDefinitionFilePath))
                {
                    throw new InvalidDataException("Target data definition file does not exist. " + Environment.NewLine
                        + "  File path: " + dataDefinitionFilePath);
                }
                else
                {
                    Console.WriteLine(Environment.NewLine + "Obtaining distorted data definition... " + Environment.NewLine
                        + "  Data definition path: " + dataDefinitionFilePath);
                    InputOutputDataDefiniton.LoadJson(dataDefinitionFilePath, ref targetDefinition);
                    Console.WriteLine("  ... obtaining distorted data definition done.");
                }
            }
            bool calculateDistortedData = false;
            if (targetDefinition == null)
            {
                calculateDistortedData = true;
                Console.WriteLine(Environment.NewLine + "Creating distorted data definition... ");
                // Distorted data definitions are not read form a file, create them here
                // by distortion of original definitions:
                InputOutputDataDefiniton.Copy(originalDefinition, ref targetDefinition);
                // Perform distortion: 
                for (int i = 0; i < targetDefinition.InputLength; ++i)
                {
                    InputElementDefinition def = targetDefinition.GetInputElement(i);
                    double min = 0, max = 0;
                    double minOrig = def.MinimalValue;
                    double maxOrig = def.MaximalValue;
                    // Calculate distorted bounds:
                    def.GetDistortedBounds(distortionFactor, randomFactor, ref min, ref max);
                    def.MinimalValue = min;
                    def.MaximalValue = max;
                    if (changeElementNames)
                        def.Name = InputOutputElementDefinition.GetDefaultInputElementName(i);
                    if (changeTitlesAndDescriptions)
                    {
                        def.Title = InputOutputElementDefinition.GetDefaultInputElementTitle(i);
                        def.Description = InputOutputElementDefinition.GetDefaultInputElementDescription(i);
                    }
                    targetDefinition.SetInputElement(i, def);
                    Console.WriteLine("Bounds for input " + i + ": [" + minOrig + ", " + maxOrig +
                        "] -> [" + min + ", " + max + "]");
                }
                for (int i = 0; i < targetDefinition.OutputLength; ++i)
                {
                    OutputElementDefinition def = targetDefinition.GetOutputElement(i);
                    double min = 0, max = 0;
                    double minOrig = def.MinimalValue;
                    double maxOrig = def.MaximalValue;
                    // Calculate distorted bounds:
                    def.GetDistortedBounds(distortionFactor, randomFactor, ref min, ref max);
                    def.MinimalValue = min;
                    def.MaximalValue = max;
                    if (changeElementNames)
                        def.Name = InputOutputElementDefinition.GetDefaultInputElementName(i);
                    if (changeTitlesAndDescriptions)
                    {
                        def.Title = InputOutputElementDefinition.GetDefaultInputElementTitle(i);
                        def.Description = InputOutputElementDefinition.GetDefaultInputElementDescription(i);
                    }
                    targetDefinition.SetOutputElement(i, def);
                    Console.WriteLine("Bounds for output " + i + ": [" + minOrig + ", " + maxOrig +
                        "] -> [" + min + ", " + max + "]");
                }
                Console.WriteLine(Environment.NewLine + "  ...creation of distorted data definition done.");
            }

            if (targetDefinition == null)
                throw new InvalidOperationException("Could not create the distorted data definition.");

            // Get the original and target bounding boxes:
            IBoundingBox originalInputBounds = null;
            IBoundingBox originalOutputBounds = null;
            IBoundingBox targetInputBounds = null;
            IBoundingBox targetOutputBounds = null;
            originalDefinition.GetInputBounds(ref originalInputBounds);
            originalDefinition.GetOutputBounds(ref originalOutputBounds);
            targetDefinition.GetInputBounds(ref targetInputBounds);
            targetDefinition.GetOutputBounds(ref targetOutputBounds);
            // Transform some other relevant quantities in the target definition data:
            if (calculateDistortedData)
            {
                calculateDistortedData = true;
                Console.WriteLine(Environment.NewLine + "Transforming other data w.r. original and new bounds... ");
                // Perform distortion of input element parameters: 
                for (int i = 0; i < targetDefinition.InputLength; ++i)
                {
                    InputElementDefinition defOrig = originalDefinition.GetInputElement(i);
                    InputElementDefinition def = targetDefinition.GetInputElement(i);
                    double minOrig = defOrig.MinimalValue;
                    double maxOrig = defOrig.MaximalValue;
                    double min = def.MinimalValue;
                    double max = def.MaximalValue;

                    double scalingLengthOrig = defOrig.ScalingLength;
                    double scalingLength = scalingLengthOrig*(max-min)/(maxOrig-minOrig);
                    def.ScalingLength = scalingLength;

                    double discretizationStepOrig = defOrig.DiscretizationStep;
                    double discretizationStep = discretizationStepOrig * (max - min) / (maxOrig - minOrig);
                    def.DiscretizationStep = discretizationStep;

                    int numSamplingPointsOrig = defOrig.NumSamplingPoints;
                    int numSamplingPoints = numSamplingPointsOrig;
                    def.NumSamplingPoints = numSamplingPoints;


                    def.NumSamplingPoints = defOrig.NumSamplingPoints;
 
                    double targetValueOrig = defOrig.TargetValue;
                    double targetValue = BoundingBox.Map(originalInputBounds, targetInputBounds,
                        i, targetValueOrig);
                    def.TargetValue = targetValue;

                    double defaultValueOrig = defOrig.DefaultValue;
                    double defaultValue = BoundingBox.Map(originalInputBounds, targetInputBounds,
                        i, defaultValueOrig);
                    def.DefaultValue = defaultValue;
                    
                    targetDefinition.SetInputElement(i, def);
                    Console.WriteLine("Bounds for input " + i + ": [" + minOrig + ", " + maxOrig +
                        "] -> [" + min + ", " + max + "]" + Environment.NewLine
                        + "  Scaling length:       " + scalingLengthOrig + " -> " + scalingLength + Environment.NewLine
                        + "  Discretization step:  " + discretizationStepOrig + " -> " + discretizationStep + Environment.NewLine
                        + "  Num. sampling points: " + numSamplingPointsOrig + " -> " + numSamplingPoints + Environment.NewLine
                        + "  Target value:         " + targetValueOrig + " -> " + targetValue + Environment.NewLine
                        + "  Default value:        " + defaultValueOrig + " -> " + defaultValue + Environment.NewLine
                        );
                }
                // Perform distortion of output element parameters: 
                for (int i = 0; i < targetDefinition.OutputLength; ++i)
                {
                    OutputElementDefinition defOrig = originalDefinition.GetOutputElement(i);
                    OutputElementDefinition def = targetDefinition.GetOutputElement(i);
                    double minOrig = defOrig.MinimalValue;
                    double maxOrig = defOrig.MaximalValue;
                    double min = def.MinimalValue;
                    double max = def.MaximalValue;


                    double scalingLengthOrig = defOrig.ScalingLength;
                    double scalingLength = scalingLengthOrig * (max - min) / (maxOrig - minOrig);
                    def.ScalingLength = scalingLength;

                    double targetValueOrig = defOrig.TargetValue;
                    double targetValue = BoundingBox.Map(originalOutputBounds, targetOutputBounds,
                        i, targetValueOrig);
                    def.TargetValue = targetValue;

                    targetDefinition.SetOutputElement(i, def);
                    Console.WriteLine("Bounds for input " + i + ": [" + minOrig + ", " + maxOrig +
                        "] -> [" + min + ", " + max + "]" + Environment.NewLine
                        + "  Scaling length:      " + scalingLengthOrig + " -> " + scalingLength + Environment.NewLine
                        + "  Target value:        " + targetValueOrig + " -> " + targetValue + Environment.NewLine
                        );
                }
                Console.WriteLine(Environment.NewLine + "  ...ctransformation of other data done.");
            }

            InputOutputDataDefiniton.SaveJson(targetDefinition, targetDataDefinitionPath);
            // Generate data:
            Console.WriteLine(Environment.NewLine + "Training and verification data for distorted data model, " + numDatapoints + " points... ");
            // SampledDataSet originalData = NeuralTrainingData;

            Console.WriteLine("Obtaining original ANN approximation model... ");
            INeuralApproximator model = this.TrainedNetwork;
            Console.WriteLine("  ... original model obtained.");
            if (originalInputBounds == null)
                throw new InvalidOperationException("Could not obtain original input bounds.");
            if (originalOutputBounds == null)
                throw new InvalidOperationException("Could not obtain original output bounds.");
            if (targetInputBounds == null)
                throw new InvalidOperationException("Could not obtain distorted input bounds.");
            if (targetOutputBounds == null)
                throw new InvalidOperationException("Could not obtain distorted output bounds.");
            if (model == null)
                throw new InvalidOperationException("Could not obtain the original ANN approximaiton model.");
            // Generate training data:
            Console.WriteLine(Environment.NewLine + "  Generating training  data, " + numDatapoints + " points... ");
            SampledDataSet targetTrainingData = new SampledDataSet(targetDefinition.InputLength, targetDefinition.OutputLength);
            for (int i = 0; i < numDatapoints; ++i)
            {
                IVector input = null, output = null, targetInput = null, targetOutput = null;
                // Create random input parameters for the original model and calculate the corresponding model output:
                originalInputBounds.GetRandomPoint(ref input);
                model.CalculateOutput(input, ref output);
                // Transform input and output vector to distorted spaces:
                BoundingBox.Map(originalInputBounds, targetInputBounds, input, ref targetInput);
                BoundingBox.Map(originalOutputBounds, targetOutputBounds, output, ref targetOutput);
                targetTrainingData.Add(new SampledDataElement(targetInput, targetOutput));
            }
            Console.WriteLine("    ... generation of training data done.");
            // Write training data to disk:
            Console.WriteLine(Environment.NewLine + "  Saving training data for distroted model... ");
            SampledDataSet.SaveJson(targetTrainingData, targetTrainingDataPath);
            Console.WriteLine("    ... saving training data done.");

            Console.WriteLine(Environment.NewLine + "  Generating verification  data, " + numDatapoints + " points... ");
            SampledDataSet targetVerificationData = new SampledDataSet(targetDefinition.InputLength, targetDefinition.OutputLength);
            for (int i = 0; i < numDatapoints; ++i)
            {
                IVector input = null, output = null, targetInput = null, targetOutput = null;
                // Create random input parameters for the original model and calculate the corresponding model output:
                originalInputBounds.GetRandomPoint(ref input);
                model.CalculateOutput(input, ref output);
                // Transform input and output vector to distorted spaces:
                BoundingBox.Map(originalInputBounds, targetInputBounds, input, ref targetInput);
                BoundingBox.Map(originalOutputBounds, targetOutputBounds, output, ref targetOutput);
                targetVerificationData.Add(new SampledDataElement(targetInput, targetOutput));
            }
            Console.WriteLine("    ... generation of verification data done.");
            // Write verification data to disk:
            Console.WriteLine(Environment.NewLine + "  Saving verification data for distroted model... ");
            SampledDataSet.SaveJson(targetVerificationData, targetVerificationDataPath);
            Console.WriteLine("    ... saving verification data done.");

            Console.WriteLine("  ... generation of training and verification data for distorted data model done.");
            Console.WriteLine(Environment.NewLine
                + "Creation of sampled data and definitions for a distorted model finished."
                + Environment.NewLine + Environment.NewLine);

            return null;
        }


#endregion NeuralModelDistortion


#region NeuralTesting

        /// <summary>Test of distances of a specified number of training points with respect to the training point with 
        /// specified index, in the input parameters space as well as in the output parameter space.</summary>
        /// <param name="referencePointIndex">Index of the point for which distances are calculated.</param>
        /// <param name="maxNumPoints">Maximal number of points taken in the test.</param>
        public virtual void TestDistances(int referencePointIndex, int maxNumPoints)
        {
            IBoundingBox distBounds = new BoundingBox2d();
            Console.WriteLine("");
            Console.WriteLine("Test of distance calculation...");
            Console.WriteLine("Distances between different training points and training point No. " + referencePointIndex + ":");
            IVector
                referenceInputVector = GetTrainingElement(referencePointIndex).InputParameters,
                referenceOutputVector = GetTrainingElement(referencePointIndex).OutputValues;
            for (int i = 0; i < NumAllTrainingPoints && i < maxNumPoints; ++i)
            {
                double inputDist = InputDistance(referenceInputVector, GetTrainingElement(i).InputParameters);
                double outputDist = OutputDistance(referenceOutputVector, GetTrainingElement(i).OutputValues);
                distBounds.Update(0, inputDist);
                distBounds.Update(1, outputDist);
                Console.WriteLine("  Distance between points " + referencePointIndex + " and " + i
                    + "\n                          - input: " + inputDist
                    + "\n                         - output: " + outputDist);
            }
            Console.WriteLine();
            Console.WriteLine("Maximal input parameters distance: " + distBounds.GetMax(0));
            Console.WriteLine("Maximal output parameters distance: " + distBounds.GetMax(1));
            Console.WriteLine("Distance test done");
            Console.WriteLine();
        }


        /// <summary>Performs test of speed of calculation of neural network.</summary>
        /// <param name="numEvaluations">Number of times the approximation is calculated.</param>
        public virtual void TestNeuralSpeed(int numEvaluations)
        {
            IVector input = new Vector(SomeNeuralInput);
            IVector output = new Vector(TrainedNetwork.OutputLength);
            Console.WriteLine();
            Console.WriteLine("Testing speed of neural network-based approximation...");
            Console.WriteLine("Vector of input parameters: \n  " + input);
            Timer.Stop(); Timer.Start();
            for (int i = 0; i < numEvaluations; ++i)
                TrainedNetwork.CalculateOutput(input, ref output);
            Timer.Stop();
            Console.WriteLine(numEvaluations + " evaluations performed in " + Timer.Time + " s (" +
                (Timer.Time / (double)numEvaluations) + " per evaluation).");
            Console.WriteLine();
        }


        /// <summary>This example demonstrates how to extract data necessary for definition of
        /// optimization problems.</summary>
        public virtual void PrintNeuralData()
        {
            NeuraApproximationFileManager fm = NeuralFM;
            // Obtain data from the data definition file:
            string dataDefinitionFile = fm.NeuralDataDefinitionPath;
            InputOutputDataDefiniton def = null;
            InputOutputDataDefiniton.LoadJson(dataDefinitionFile, ref def);
            // Go through definition data for output values and read various related parameters:
            // Remark: field names are the same as in the JSON file

            Console.WriteLine();
            Console.WriteLine("Description of approximated data:");

            // Go through definition data for input parameters and read various related parameters:
            Console.WriteLine("NN approximation - input data:");
            for (int i = 0; i < def.InputLength; ++i)
            {
                Console.WriteLine("Output element No. " + i + ":");
                InputElementDefinition outputDef = def.GetInputElement(i);
                if (!outputDef.BoundsDefined)
                    throw new InvalidDataException("Bounds are not defined for output element No. " + i + ".");
                string name = outputDef.Name;
                string description = outputDef.Description;
                double minimalValue = outputDef.MinimalValue;
                double maximalValue = outputDef.MaximalValue;
                double intervalLength = outputDef.MaximalValue - outputDef.MinimalValue;
                double targetValue = outputDef.TargetValue;
                // Here you can do whatever needed with the data obtained from the data definition file...
                Console.WriteLine("     name:" + name);
                Console.WriteLine("     description:" + description);
                Console.WriteLine("     minimalValue:" + minimalValue);
                Console.WriteLine("     maximalValue:" + maximalValue);
                Console.WriteLine("     intervalLength:" + intervalLength);
                Console.WriteLine("     targetValue:" + targetValue);
            }

            Console.WriteLine();
            Console.WriteLine("NN approximation - output data:");
            for (int i = 0; i < def.OutputLength; ++i)
            {
                Console.WriteLine("Output element No. " + i + ":");
                OutputElementDefinition outputDef = def.GetOutputElement(i);
                if (!outputDef.BoundsDefined)
                    throw new InvalidDataException("Bounds are not defined for output element No. " + i + ".");
                string name = outputDef.Name;
                string description = outputDef.Description;
                double minimalValue = outputDef.MinimalValue;
                double maximalValue = outputDef.MaximalValue;
                double intervalLength = outputDef.MaximalValue - outputDef.MinimalValue;
                double targetValue = outputDef.TargetValue;
                // Here you can do whatever needed with the data obtained from the data definition file...
                Console.WriteLine("     name:" + name);
                Console.WriteLine("     description:" + description);
                Console.WriteLine("     minimalValue:" + minimalValue);
                Console.WriteLine("     maximalValue:" + maximalValue);
                Console.WriteLine("     intervalLength:" + intervalLength);
                Console.WriteLine("     targetValue:" + targetValue);
            }
            Console.WriteLine();
            Console.WriteLine();
        }

#endregion NeuralTesting


#region StoredScriptSettings

        /// <summary>In methods of this class you will find all the settings that apply to this script.</summary>
        /// <remarks>Before custom application script is archived, settings should be moved </remarks>
        /// $A Igor Feb12;
        protected new class StoredScriptSettings : ApplicationCommandlineBase
        {

            protected override CommandLineApplicationInterpreter CreateInterpreter() { throw new NotImplementedException("Creation of commandlinbe interpreter is not implemented in this class."); }
            public override void TestMain(string[] args) { throw new NotImplementedException(""); }

            public void TestMain_Basic(string[] args)
            {
                // Store script settings in this method!
            }
        }

#endregion StoredScriptSettings


#region Common

        public bool saveGraphs = false;

        /// <summary> Saves sensitivity test in csv file. </summary>
        /// $A Tako78 Jul13;
        public void SaveSensitivityCSV()
        {
            int outputlength = TrainedNetwork.OutputLength;
            
            if (SensitivityTrainingResults != null)
            {
                Console.WriteLine("Saving sensitivity test for training data...");

                StringTable csv = new StringTable();
                csv.AddRow();
                for (int i = 0; i < outputlength; i++)
                {
                    string outputName = NeuralDataDefinition.GetOutputElement(i).Name;
                    csv[0, i + 1] = outputName;
                }
                // Write data lines
                for (int i = 0; i < SensitivityTrainingResults.Count; i++)
                {
                    IVector sensitivityValue = new Vector(outputlength);
                    sensitivityValue = SensitivityTrainingResults[i];
                    string inputName = NeuralDataDefinition.GetInputElement(i).Name;
                    csv[i + 1, 0] = inputName;
                    
                    for (int j = 0; j < sensitivityValue.Length; j++)
                    {
                        csv[i + 1, j + 1] = sensitivityValue[j].ToString();
                    }
                }
                csv.SaveCsv(Path.Combine(Path.GetDirectoryName(OptimizationDirectory) + "\\opt\\" + "sensitivitytrainingdata_test.csv"));
                Console.WriteLine("Sensitivity test for training data saved!");
            }

            if (SensitivityVerificationResults != null)
            {
                Console.WriteLine("Saving sensitivity test for verification data...");

                StringTable csv = new StringTable();
                csv.AddRow();
                for (int i = 0; i < outputlength; i++)
                {
                    string outputName = NeuralDataDefinition.GetOutputElement(i).Name;
                    csv[0, i + 1] = outputName;
                }
                // Write data lines
                for (int i = 0; i < SensitivityVerificationResults.Count; i++)
                {
                    IVector sensitivityValue = new Vector(outputlength);
                    sensitivityValue = SensitivityVerificationResults[i];
                    string inputName = NeuralDataDefinition.GetInputElement(i).Name;
                    csv[i + 1, 0] = inputName;

                    for (int j = 0; j < sensitivityValue.Length; j++)
                    {
                        csv[i + 1, j + 1] = sensitivityValue[j].ToString();
                    }
                }
                csv.SaveCsv(Path.Combine(Path.GetDirectoryName(OptimizationDirectory) + "\\opt\\" + "sensitivityverificationdata_test.csv"));
                Console.WriteLine("Sensitivity test for verification data saved!");
            }
                
            

            //if (SensitivityTrainingResults != null)
            //{
            //    TextWriter textwr = new StreamWriter(Path.Combine(Path.GetDirectoryName(OptimizationDirectory) + "\\opt\\" + "sensitivitytrainingdata.csv"));
            //    using (textwr)
            //    {
            //        Console.WriteLine("Saving sensitivity test for training data...");
            //        // Write first line
            //        for (int i = 0; i < outputlength; i++)
            //        {
            //            if (i == 0)
            //                textwr.Write(" ;");
            //            string outputName = NeuralDataDefinition.GetOutputElement(i).Name;
            //            textwr.Write(outputName);
            //            if (i < outputlength - 1)
            //                textwr.Write(";");
            //        }
            //        textwr.WriteLine();

            //        // Write data lines
            //        for (int i = 0; i < SensitivityTrainingResults.Count; i++)
            //        {
            //            IVector sensitivityValue = new Vector(outputlength);
            //            sensitivityValue = SensitivityTrainingResults[i];
            //            string inputName = NeuralDataDefinition.GetInputElement(i).Name;
            //            textwr.Write(inputName);
            //            textwr.Write(" ;");

            //            for (int j = 0; j < sensitivityValue.Length; j++)
            //            {
            //                textwr.Write(sensitivityValue[j].ToString());
            //                if (j < sensitivityValue.Length - 1)
            //                    textwr.Write(";");
            //            }
            //            textwr.WriteLine();
            //        }
            //    }
            //    textwr.Close();
            //    Console.WriteLine("Sensitivity test for training data saved!");
            //}
            //if (SensitivityVerificationResults != null)
            //{
            //    TextWriter textwr = new StreamWriter(Path.Combine(Path.GetDirectoryName(OptimizationDirectory) + "\\opt\\" + "sensitivityverificationdata.csv"));
            //    using (textwr)
            //    {
            //        Console.WriteLine("Saving sensitivity test for verification data...");
            //        // Write first line
            //        for (int i = 0; i < outputlength; i++)
            //        {
            //            if (i == 0)
            //                textwr.Write(" ;");
            //            string outputName = NeuralDataDefinition.GetOutputElement(i).Name;
            //            textwr.Write(outputName);
            //            if (i < outputlength - 1)
            //                textwr.Write(";");
            //        }
            //        textwr.WriteLine();

            //        // Write data lines
            //        for (int i = 0; i < SensitivityVerificationResults.Count; i++)
            //        {
            //            IVector sensitivityValue = new Vector(outputlength);
            //            sensitivityValue = SensitivityVerificationResults[i];
            //            string inputName = NeuralDataDefinition.GetInputElement(i).Name;
            //            textwr.Write(inputName);
            //            textwr.Write(" ;");

            //            for (int j = 0; j < sensitivityValue.Length; j++)
            //            {
            //                textwr.Write(sensitivityValue[j].ToString());
            //                if (j < sensitivityValue.Length - 1)
            //                    textwr.Write(";");
            //            }
            //            textwr.WriteLine();
            //        }
            //    }
            //    textwr.Close();
            //    Console.WriteLine("Sensitivity test for verification data saved!");
            //}
            
        }



#endregion

    }  // abstract class LoadableScriptShellNeural


}
