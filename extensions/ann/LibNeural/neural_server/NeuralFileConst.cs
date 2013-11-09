
// CONSTANTS (such as standard file names) FOR NEURAL NETWORKS APPROXIMATOR'S FILE SERVER AND CLIENT

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;
using IG.Num;

using IG.Neural;

namespace IG.Neural
{


    /// <summary>Constants used in definition of neural networks approximation servers and clients
    /// working through file system.</summary>
    /// <remarks>Idea based on constants for optimization file analysis server/client from IOptLib.Net.</remarks>
    /// $A Igor Feb11;
    static public class NeuralFileConst
    {

        public const string DirPrefix = "NeuralApproximation_Folder";

        // GLOBAL FILE SYSTEM LOCKING:

        public const string LockFileMutex = "IG.Lib.NeuralApproximationFileManager.LockFileMutex";


        // OPERATION DATA:

        /// <summary>File where trained network is stored.</summary>
        public const string NeuralNetworkFilename = "neuralnetwork.json";
        /// <summary>File containing input and output data definitions for neural networks-based approximation.</summary>
        public const string NeuralDataDefinitionFilename = "neuraldatadefinition.json";

        /// <summary>File containing input and output data definition for simulators that are used for obtaining
        /// neural data definitions.</summary>
        public const string SimulationDataDefinitionFilename = "simdatadefinition.json";

        /// <summary>File containing input and output mapping definitions.</summary>
        /// $A tako78 Jul.21
        public const string MappingDefinitionFilename = "mappingdefinition.json";
        /// <summary>Training data.</summary>
        public const string NeuralTrainingDataFilename = "neuraltrainingdata.json";
        
        /// <summary>Verification data.</summary>
        public const string NeuralVerificationDataFilename = "neuralverificationdata.json";

        /// <summary>Name of the file where optimal training parameters are saved.</summary>
        public const string NeuralOptimalTrainingParametersFilename = "optimaltrainingparameters.json";

        /// <summary>Name of the file where sets of training parameters and convergence results are stored.</summary>
        public const string NeuralTrainingParametersFilename = "trainingparameters.json";

        /// <summary>Name of the file where sets of training limits are stored.</summary>
        ///  $A Tako78 Aug12;
        public const string NeuralTrainingLimitsFilename = "traininglimits.json";

        /// <summary>Name of the file where sets of training complete results and limits are stored.</summary>
        ///  $A Tako78 Aug12;
        public const string NeuralTrainingResultsFilename = "trainingresults.json";

        /// <summary>Name of the file where sets of training complete results and limits are stored in CSV file.</summary>
        ///  $A Tako78 Sept12;
        public const string NeuralTrainingResultsCSVFilename = "trainingresults.csv";

        // DATA EXCHANGE FILES:

        /// <summary>Default file name of neural networks approximation input file in standard IGLib format.</summary>
        public const string NeuralInputFilename = "neuralinput.json";
        /// <summary>Default file name of neural networks approximation input file in XML format.</summary>
        public const string NeuralInputXmlFilename = "neuralinput.xml";
        /// <summary>Default file name of neural networks approximation output file in standard IGLib format.</summary>
        public const string NeuralOutputFilename = "neuraloutput.json";
        /// <summary>Default file name of neural networks approximation output file in XML format.</summary>
        public const string NeuralOutputeXmlFilename = "neuraloutput.xml";
        /// <summary>Default file name of function input parameters file in standard IGLib format.</summary>
        /// $A tako78 Jul.21
        public const string FunctionInputFilename = "functioninput.json";
        /// <summary>Default file name of function output parameters file in standard IGLib format.</summary>
        /// $A tako78 Jul.21
        public const string FunctiOutputFilename = "functionoutput.json";


        // MESSAGE FILES:

        /// <summary>Default file name for neural network approximation busy flag.</summary>
        public const string MsgNeuralBusyFilename = "neuralbusy.msg";
        /// <summary>Default file name for neural network approximation input data ready flag.</summary>
        public const string MsgNeuralInputReadyFilename = "neuraldataready.msg";
        /// <summary>Default file name for neural network approximation results ready flag.</summary>
        public const string MsgNeuralOutputReadyFilename = "neuraloutputready.msg";
        /// <summary>Default file name for function input parameters data ready flag.</summary>
        /// $A tako78 Jul.21
        public const string MsgFunctionInputReadyFilename = "functiondataready.msg";
        /// <summary>Default file name for function output parameters results ready flag.</summary>
        /// $A tako78 Jul.21
        public const string MsgFunctionOutputReadyFilename = "functionoutputready.msg";

        ///// <summary>Default file name for optimization busy flag.</summary>
        //public const string MsgOptBusy = "optbusy.msg";
        ///// <summary>Default file name for optimization input data ready flag.</summary>
        //public const string MsgOptDataReady = "optdataready.msg";
        ///// <summary>Default file name for optimization resutlts ready flag.</summary>
        //public const string MsgOptResultsReady = "optresultsready.msg";

        //// INTERFACE WITH PROGRAM Inverse:
        ///// <summary>Default file name for optimization command file for program Inverse (Inverse interface).</summary>
        //public const string InvOptCommandFile = "optimization.cm";
        ///// <summary>Default file name for analysis command file for program Inverse (Inverse interface).</summary>
        //public const string InvAnCommandFile = "analysis.cm";

    }



}