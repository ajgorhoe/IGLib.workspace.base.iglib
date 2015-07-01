
// ELEMENTARY OPERATIONS FOR NEURAL NETWORKS APPROXIMATOR'S FILE SERVER AND CLIENT

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;
using IG.Num;


namespace IG.Neural
{

    /// <summary>File manager for neural network approximation file client and server.</summary>
    /// <remarks>This class is modelled after the DragonFly optimization server.</remarks>
    /// $A Igor Apr11; 
    public class NeuraApproximationFileManager : ILockable
    {

        #region Construction 


        private NeuraApproximationFileManager() { }

        /// <summary>Nonstruct a new file manager for neural approximation file client/server 
        /// that operates in the specified directory.</summary>
        /// <param name="directoryPath">Operation directory for data & message exchange through files.</param>
        public NeuraApproximationFileManager(string directoryPath)
        {
            this.DataDirectory = directoryPath;
        }


        #endregion Construction


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        private object _internalLock = new object();

        /// <summary>Used internally for locking access to internal fields.</summary>
        protected object InternalLock { get { return _internalLock; } }

        //private object waitlock = new object();

        ///// <summary>Must be used only for locking waiting the Waiting() block (since it is potentially time consuming).</summary>
        //protected object WaitLock { get { return waitlock; } }

        #endregion ThreadLocking


        #region OperationData


        #region DataDirectory

        protected string _dataDirectory = null;

        /// <summary>Directory for data and messages exchange through files.</summary>
        public string DataDirectory
        {
            get
            {
                lock (Lock)
                {
                    return _dataDirectory;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (String.IsNullOrEmpty(value))
                        throw new ArgumentException("Directory of the neural network approximator file manager is not specified (null or empty string).");
                    string dir = Path.GetFullPath(value);
                    if (!Directory.Exists(dir))
                    {
                        try
                        {
                            Directory.CreateDirectory(dir);
                        }
                        catch { }
                        if (!Directory.Exists(_dataDirectory))
                            throw new ArgumentException("The specified neural network approximator file manager's directory does not exist. "
                                + Environment.NewLine + "  Directory: " + value
                                + Environment.NewLine + "  Full path: " + dir);
                    }
                    _dataDirectory = dir;
                }
            }
        }

        /// <summary>Returns full path of the file or directory with the specified relative path
        /// within the data directory of the current file server/client.</summary>
        /// <param name="fileOrDirectoryName">Relative path (with respect to data and messages exchange directory)</param>
        public string GetPath(string fileOrDirectoryName)
        {
            lock (Lock)
            {
                return Path.Combine(DataDirectory, fileOrDirectoryName);
            }
        }

        #endregion DataDirectory

        #region Constants

        // FILE SYSTEM LOCKING:

        private string _LockFileMutex = NeuralFileConst.LockFileMutex;

        
        // DATA EXCHANGE FILES:

        private string
 
            _neuralNetworkFilename = NeuralFileConst.NeuralNetworkFilename,
            _neuralNetworkPath = null,
            _neuralDataDefinitionFilename = NeuralFileConst.NeuralDataDefinitionFilename,
            _neuralDataDefinitionPath = null,
            _simulationDataDefinitionFilename = NeuralFileConst.SimulationDataDefinitionFilename,
            _simulationDataDefinitionPath = null,
            _neuralTrainingDataFilename = NeuralFileConst.NeuralTrainingDataFilename,
            _neuralTrainingDataPath = null,
            _neuralVerificationDataFilename = NeuralFileConst.NeuralVerificationDataFilename,
            _neuralVerificationDataPath = null,

            _neuralInputFilename = NeuralFileConst.NeuralInputFilename,
            _neuralInputPath = null,
            _neuralInputXmlFilename = NeuralFileConst.NeuralInputXmlFilename,
            _neuralInputXmlPath = null,
            _neuralOutputFilename = NeuralFileConst.NeuralOutputFilename,
            _neuralOutputPath = null,
            _neuralOutputXmlFilename = NeuralFileConst.NeuralOutputeXmlFilename,
            _neuralOutputXmlPath = null;


        // MESSAGE FILES:

        private string
            _msgNeuralBusyFilename = NeuralFileConst.MsgNeuralBusyFilename,
            _msgNeuralBusyPath = null,
            _msgNeuralInputReadyFilename = NeuralFileConst.MsgNeuralInputReadyFilename,
            _msgNeuralInputReadyPath = null,
            _msgNeuralOutputReadyFilename = NeuralFileConst.MsgNeuralOutputReadyFilename,
            _msgNeuralOutputReadyPath = null;

        #endregion Constants

        // OPERATION DATA:

        /// <summary>File path of the file where complete (usually trained) neural network is stored.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string NeuralNetworkPath
        {
            get
            {
                lock (Lock)
                {
                    if (_neuralNetworkPath == null)
                        _neuralNetworkPath = Path.Combine(DataDirectory, _neuralNetworkFilename);
                    return _neuralNetworkPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _neuralNetworkFilename = Path.GetFileName(value);
                    }
                    _neuralNetworkPath = null;
                }
            }
        }


        /// <summary>File path of the input and output data definition file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string NeuralDataDefinitionPath
        {
            get
            {
                lock (Lock)
                {
                    if (_neuralDataDefinitionPath == null)
                        _neuralDataDefinitionPath = Path.Combine(DataDirectory, _neuralDataDefinitionFilename);
                    return _neuralDataDefinitionPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _neuralDataDefinitionFilename = Path.GetFileName(value);
                    }
                    _neuralDataDefinitionPath = null;
                }
            }
        }

        /// <summary>File path of the input and output data definition file for simulator.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string SimulationDataDefinitionPath
        {
            get
            {
                lock (Lock)
                {
                    if (_simulationDataDefinitionPath == null)
                        _simulationDataDefinitionPath = Path.Combine(DataDirectory, _simulationDataDefinitionFilename);
                    return _simulationDataDefinitionPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _simulationDataDefinitionFilename = Path.GetFileName(value);
                    }
                    _simulationDataDefinitionPath = null;
                }
            }
        }

        /// <summary>File path of the verification data file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string NeuralVerificationDataPath
        {
            get
            {
                lock (Lock)
                {
                    if (_neuralVerificationDataPath == null)
                        _neuralVerificationDataPath = Path.Combine(DataDirectory, _neuralVerificationDataFilename);
                    return _neuralVerificationDataPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _neuralVerificationDataFilename = Path.GetFileName(value);
                    }
                    _neuralVerificationDataPath = null;
                }
            }
        }

        /// <summary>File path of the training data file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string NeuralTrainingDataPath
        {
            get
            {
                lock (Lock)
                {
                    if (_neuralTrainingDataPath == null)
                        _neuralTrainingDataPath = Path.Combine(DataDirectory, _neuralTrainingDataFilename);
                    return _neuralTrainingDataPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _neuralTrainingDataFilename = Path.GetFileName(value);
                    }
                    _neuralTrainingDataPath = null;
                }
            }
        }


        // DATA EXCHANGE FILES:



        /// <summary>File path of the file with input parameters.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string NeuralInputPath
        {
            get
            {
                lock (Lock)
                {
                    if (_neuralInputPath == null)
                        _neuralInputPath = Path.Combine(DataDirectory, _neuralInputFilename);
                    return _neuralInputPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _neuralInputFilename = Path.GetFileName(value);
                    }
                    _neuralInputPath = null;
                }
            }
        }


        /// <summary>File path of the input parameters file in XML format.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string NeuralInputXmlPath
        {
            get
            {
                lock (Lock)
                {
                    if (_neuralInputXmlPath == null)
                        _neuralInputXmlPath = Path.Combine(DataDirectory, _neuralInputXmlFilename);
                    return _neuralInputXmlPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _neuralInputXmlFilename = Path.GetFileName(value);
                    }
                    _neuralInputXmlPath = null;
                }
            }
        }


        /// <summary>File path of the file for storing approximated output values.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string NeuralOutputPath
        {
            get
            {
                lock (Lock)
                {
                    if (_neuralOutputPath == null)
                        _neuralOutputPath = Path.Combine(DataDirectory, _neuralOutputFilename);
                    return _neuralOutputPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _neuralOutputFilename = Path.GetFileName(value);
                    }
                    _neuralOutputPath = null;
                }
            }
        }


        /// <summary>File path of the file for storing approximated output values in XML format.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string NeuralOutputeXmlPath
        {
            get
            {
                lock (Lock)
                {
                    if (_neuralOutputXmlPath == null)
                        _neuralOutputXmlPath = Path.Combine(DataDirectory, _neuralOutputXmlFilename);
                    return _neuralOutputXmlPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _neuralOutputXmlFilename = Path.GetFileName(value);
                    }
                    _neuralOutputXmlPath = null;
                }
            }
        }



        // MESSAGE FILES:



        /// <summary>File path of the message file indicating that approximator is busy by performing a job.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string MsgNeuralBusyPath
        {
            get
            {
                lock (Lock)
                {
                    if (_msgNeuralBusyPath == null)
                        _msgNeuralBusyPath = Path.Combine(DataDirectory, _msgNeuralBusyFilename);
                    return _msgNeuralBusyPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _msgNeuralBusyFilename = Path.GetFileName(value);
                    }
                    _msgNeuralBusyPath = null;
                }
            }
        }


        /// <summary>File path of the message file indicating taht input data is ready to be processed.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string MsgNeuralInputReadyPath
        {
            get
            {
                lock (Lock)
                {
                    if (_msgNeuralInputReadyPath == null)
                        _msgNeuralInputReadyPath = Path.Combine(DataDirectory, _msgNeuralInputReadyFilename);
                    return _msgNeuralInputReadyPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _msgNeuralInputReadyFilename = Path.GetFileName(value);
                    }
                    _msgNeuralInputReadyPath = null;
                }
            }
        }


        /// <summary>File path of the message file indicating that the approximated output is ready to be read.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string MsgNeuralOutputReadyPath
        {
            get
            {
                lock (Lock)
                {
                    if (_msgNeuralOutputReadyPath == null)
                        _msgNeuralOutputReadyPath = Path.Combine(DataDirectory, _msgNeuralOutputReadyFilename);
                    return _msgNeuralOutputReadyPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _msgNeuralOutputReadyFilename = Path.GetFileName(value);
                    }
                    _msgNeuralOutputReadyPath = null;
                }
            }
        }


        #endregion OperationData


        #region Messages


        /// <summary>Sets the falg that indicates that the approximation server is busy.</summary>
        public void SetNeuralBusy()
        {
            lock (Lock)
            {
                string path = MsgNeuralBusyPath;
                using (TextWriter sw = new StreamWriter(path))
                {  }
                // Remark:
                // Creation of file with File.Create() is not good because the file is locked in
                // this case and can not be deleted later.
                //if (!File.Exists(path))
                //    File.Create(path);
            }
        }

        /// <summary>Clears the falg that indicates that the approximation server is busy.</summary>
        public void ClearNeuralBusy()
        {
            lock (Lock)
            {
                string path = MsgNeuralBusyPath;
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        /// <summary>Returns value of the falg that indicates whether the approximation server is busy.</summary>
        public bool IsNeuralBusy()
        {
            lock(Lock)
            {
                string path = MsgNeuralBusyPath;
                if (File.Exists(path))
                    return true;
                else
                    return false;
            }
        }


        /// <summary>Sets the falg that indicates that the approximation input data is ready.</summary>
        public void SetNeuralInputReady()
        {
            lock (Lock)
            {
                string path = MsgNeuralInputReadyPath;
                using (TextWriter sw = new StreamWriter(path))
                { }
                // Remark:
                // Creation of file with File.Create() is not good because the file is locked in
                // this case and can not be deleted later.
            }
        }

        /// <summary>Clears the falg that indicates that the approximation input data is ready.</summary>
        public void ClearNeuralInputReady()
        {
            lock (Lock)
            {
                string path = MsgNeuralInputReadyPath;
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        /// <summary>Returns value of the falg that indicates whether the approximation input data is ready.</summary>
        public bool IsNeuralInputReady()
        {
            lock(Lock)
            {
                string path = MsgNeuralInputReadyPath;
                if (File.Exists(path))
                    return true;
                else
                    return false;
            }
        }


        /// <summary>Sets the falg that indicates that the approximation output data is ready.</summary>
        public void SetNeuralOutputReady()
        {
            lock (Lock)
            {
                string path = MsgNeuralOutputReadyPath;
                using (TextWriter sw = new StreamWriter(path))
                { }
                // Remark:
                // Creation of file with File.Create() is not good because the file is locked in
                // this case and can not be deleted later.
            }
        }

        /// <summary>Clears the falg that indicates that the approximation output data is ready.</summary>
        public void ClearNeuralOutputReady()
        {
            lock (Lock)
            {
                string path = MsgNeuralOutputReadyPath;
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        /// <summary>Returns value of the falg that indicates whether the approximation output data is ready.</summary>
        public bool IsNeuralOutputReady()
        {
            lock (Lock)
            {
                string path = MsgNeuralOutputReadyPath;
                if (File.Exists(path))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>Clears all messages for neural approximation client and server.</summary>
        public void ClearMessages()
        {
            ClearNeuralInputReady();
            ClearNeuralOutputReady();
            ClearNeuralBusy();
        }


        #endregion Messages


        #region OperationBasic

        /// <summary>Reads neural approximator input parameters form the file at standard location.</summary>
        /// <param name="inputParameters">Object where parameters are stored.</param>
        public virtual void ReadNeuralInput(ref IVector inputParameters)
        {
            Vector.LoadJson(NeuralInputPath, ref inputParameters);
        }

        /// <summary>Writes neural approximator input parameters to standard location.</summary>
        /// <param name="inputParameters">Parameters to be written.</param>
        public virtual void WriteNeuralInput(IVector inputParameters)
        {
            Vector.SaveJson(inputParameters,NeuralInputPath);
        }

        /// <summary>Reads neural approximator output values from the file at standard location.</summary>
        /// <param name="outputValues">Object where the read outut values are stored.</param>
        public virtual void ReadNeuralOutput(ref IVector outputValues)
        {
            Vector.LoadJson(NeuralOutputPath, ref outputValues);
        }
        
        /// <summary>Writes neural approximator outut values to the file at standard location.</summary>
        /// <param name="outputValues">Output values that are written.</param>
        public virtual void WriteNeuralOutput(IVector outputValues)
        {
            Vector.SaveJson(outputValues,NeuralOutputPath);
        }

        /// <summary>Loads the trained neural network approximator from the file at standard location.</summary>
        /// <param name="trainedNetwork">Object where the loaded neural network approximator is stored.</param>
        public virtual void LoadNeuralNetwork(ref INeuralApproximator trainedNetwork)
        {
            NeuralApproximatorBase.LoadJson(NeuralNetworkPath, ref trainedNetwork);
        }

        #endregion OperationBasic


        #region OperationClient


        /// <summary>Client writes input parameters for calculation of neural network based approximated values.
        /// Messages are set nad clears appropriately.</summary>
        /// <param name="inputParameters">Parameters to be written.</param>
        public virtual void ClientWriteNeuralInput(IVector inputParameters)
        {
            SetNeuralBusy();
            ClearNeuralInputReady();
            WriteNeuralInput(inputParameters);
            SetNeuralInputReady();
        }

        /// <summary>Client reads the results of neural network-based approximation (output values).
        /// Messages are set and clears appropriately.</summary>
        /// <param name="outputValues"></param>
        public virtual void ClientReadNeuralOutput(ref IVector outputValues)
        {
            ReadNeuralOutput(ref outputValues);
            ClearNeuralOutputReady();
        }

        /// <summary>Performs request to the server for calculation of neural network based approximated values.</summary>
        public virtual void ClientSendRequestCalculateApproximation()
        {
            ServerCalculateApproximation();
        }

        /// <summary>Calculates approximation by using the neural network approximation server.</summary>
        /// <param name="inputParameters">Intput parameters for which approximation is calculated.</param>
        /// <param name="outputValues">Vector where approximation output values are stored.</param>
        public virtual void ClientCalculateApproximation(IVector inputParameters, ref IVector outputValues)
        {
            ClientWriteNeuralInput(inputParameters);
            ClientSendRequestCalculateApproximation();
            ClientReadNeuralOutput(ref outputValues);
        }

        /// <summary>Performs client-side test calculation of neural network based approximation where
        /// input parameters are read from a specified JSON file, and calculated output values are written 
        /// to the specified file.</summary>
        /// <param name="inputFilePath">Path to the JSON file where input parameters are read from.
        /// The file pointed at must exist.</param>
        /// <param name="deletedFilePath">Path of a file where the calculated approximated values are written to.
        /// It can be null or empty string, in this case parameters are not written to a file (but they are still 
        /// output on console).</param>
        public virtual void ClientTestCalculateApproximation(string inputFilePath, string outputFilePath)
        {
            if (!File.Exists(inputFilePath))
                throw new ArgumentException("File with input parameters for neural network approximation does not exist: "
                    + Environment.NewLine + "  " + inputFilePath + ".");
            Console.WriteLine();
            Console.WriteLine("Performing test approximation by neural network...");
            IVector inputParameters = null, outputValues = null;
            Vector.LoadJson(inputFilePath, ref inputParameters);
            Console.WriteLine("Input parameters read from " + inputFilePath + ".");
            ClientCalculateApproximation(inputParameters, ref outputValues);
            Console.WriteLine();
            Console.WriteLine("Neural network approximation results:");
            Console.WriteLine("Input parameters: " + Environment.NewLine + "  " + inputParameters);
            Console.WriteLine("Approximated values: " + Environment.NewLine + "  " + outputValues);
            Console.WriteLine();
            if (!string.IsNullOrEmpty(outputFilePath))
            {
                Vector.SaveJson(outputValues, outputFilePath);
                Console.WriteLine("Output parameters saved to " + outputFilePath + ".");
            }
            Console.WriteLine("... approximation done.");
            Console.WriteLine();
        }


        #endregion OperationClient


        #region OperationServer


        /// <summary>Performs neural network-based approximation at prescribed input parameters and saves results.
        /// Messages are set and cleared appropriately.
        /// This method read input parameters from standard location, loads trained neural network, calculates
        /// approximated outpur values and stores them to the standard location.</summary>
        public virtual void ServerCalculateApproximation()
        {
            if (!IsNeuralInputReady())
            {
                //Console.WriteLine();
                //Console.WriteLine("WARNING: Neural approximation input data is not ready.");
                //Console.WriteLine();
                throw new InvalidOperationException("Neural input ready flag is not set.");
            }
            if (!IsNeuralBusy())
                SetNeuralBusy();
            if (IsNeuralOutputReady())
                ClearNeuralOutputReady();
            ClearNeuralInputReady();
            IVector inputParameters = null;
            ReadNeuralInput(ref inputParameters);
            INeuralApproximator approximator = null;
            LoadNeuralNetwork(ref approximator);
            IVector outputValues = null;
            // Calculate approximation:
            approximator.CalculateOutput(inputParameters, ref outputValues);
            WriteNeuralOutput(outputValues);
            SetNeuralOutputReady();
            // ClearNeuralBusy();  // this should be done by client!!
        }


        #endregion OperationServer


    }  // class NeuralFileManager


    /// <summary>File manager for mapping file client and server.</summary>
    /// $A tako78 Jul11; Igor Jul11;
    public class MappingApproximationFileManager : NeuraApproximationFileManager
    {

        #region Construction 


        private MappingApproximationFileManager() 
            : base(null) { }

        /// <summary>Constructs a new file manager for mapping approximation file client/server 
        /// that operates in the specified directory.</summary>
        /// <param name="directoryPath">Operation directory for data & message exchange through files.</param>
        /// $A Igor Jul, tako78 Jul
        public MappingApproximationFileManager(string directoryPath)
            : base(directoryPath)
        {
            InputOutputDataDefiniton definitionData = null;
            MappingDefinition mappingData = null;

            if (File.Exists(NeuralDataDefinitionPath))
                LoadDataDefinition(ref definitionData);
            if (File.Exists(MappingDefinitionPath))
                LoadMappingDefinition(ref mappingData);

            DataMapperSimple mapper = new DataMapperSimple(mappingData, definitionData);
            MapperDefinition = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="mapper"></param>
        /// $A Igor Jul, tako78 Jul
        public MappingApproximationFileManager(string directoryPath, DataMapperSimple mapper)
            : base(directoryPath)
        {
            MapperDefinition = mapper;
        }


        #endregion Construction

      
        #region OperationData


        #region Constants


        // DATA EXCHANGE FILES:

        private string

        _mappingDefinitionFilename = NeuralFileConst.MappingDefinitionFilename,
        _mappingDefinitionPath = null,

        _functionInputFilename = NeuralFileConst.FunctionInputFilename,
        _functionInputPath = null,
        _functionOutputFilename = NeuralFileConst.FunctiOutputFilename,
        _functionOutputPath = null;

        // MESSAGE FILES:

        private string

            _msgFunctionInputReadyFilename = NeuralFileConst.MsgFunctionInputReadyFilename,
            _msgFunctionInputReadyPath = null,
            _msgFunctionOutputReadyFilename = NeuralFileConst.MsgFunctionOutputReadyFilename,
            _msgFunctionOutputReadyPath = null;


        #endregion


        // OPERATION DATA:

        static DataMapperIdentity _aux = null;
        private IDataMapper _mapperDefinition = null;


        /// <summary>Initialize DataMapperIdentity object for identity copying.</summary>
        /// <summary>Initializing is performed only once.</summary>
        /// $A Igor Jul, tako78 Jul
        static DataMapperIdentity Aux
        {
            get
            {
                if (_aux == null)
                    _aux = new DataMapperIdentity();
                return _aux;
            }
        }


        /// <summary>Initialize DataMapperIdentity object for identity copying if mapper is null.
        /// Setter takes DataMapperSimple object.</summary>
        /// $A Igor Jul, tako78 Jul
        public IDataMapper MapperDefinition
        {
            get
            {
                if (_mapperDefinition == null)
                {
                    return Aux;
                }
                else
                    return _mapperDefinition;
            }
            set
            {
                _mapperDefinition = value;
            }

        }


        /// <summary>File path of the input and output mapping definition file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        /// $A tako78 Jul.21
        public string MappingDefinitionPath
        {
            get
            {
                lock (Lock)
                {
                    if (_mappingDefinitionPath == null)
                        _mappingDefinitionPath = Path.Combine(DataDirectory, _mappingDefinitionFilename);
                    return _mappingDefinitionPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _mappingDefinitionFilename = Path.GetFileName(value);
                    }
                    _mappingDefinitionPath = null;
                }
            }
        }

        // DATA EXCHANGE FILES:

        /// <summary>File path of the file with reduced input parameters.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        /// $A tako78 Jul.21
        public string FunctionInputPath
        {
            get
            {
                lock (Lock)
                {
                    if (_functionInputPath == null)
                        _functionInputPath = Path.Combine(DataDirectory, _functionInputFilename);
                    return _functionInputPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _functionInputFilename = Path.GetFileName(value);
                    }
                    _functionInputPath = null;
                }
            }
        }

        /// <summary>File path of the file for storing reduced approximated output values.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        /// $A tako78 Jul.21
        public string FunctionOutputPath
        {
            get
            {
                lock (Lock)
                {
                    if (_functionOutputPath == null)
                        _functionOutputPath = Path.Combine(DataDirectory, _functionOutputFilename);
                    return _functionOutputPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _functionOutputFilename = Path.GetFileName(value);
                    }
                    _functionOutputPath = null;
                }
            }
        }

        // MESSAGE FILES:

        /// <summary>File path of the message file indicating that reduced input data is ready to be processed.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        /// $A tako78 Jul.21
        public string MsgFunctionInputReadyPath
        {
            get
            {
                lock (Lock)
                {
                    if (_msgFunctionInputReadyPath == null)
                        _msgFunctionInputReadyPath = Path.Combine(DataDirectory, _msgFunctionInputReadyFilename);
                    return _msgFunctionInputReadyPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _msgFunctionInputReadyFilename = Path.GetFileName(value);
                    }
                    _msgFunctionInputReadyPath = null;
                }
            }
        }

        /// <summary>File path of the message file indicating that the reduced approximated output is ready to be read.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        /// $A tako78 Jul.21
        public string MsgFunctionOutputReadyPath
        {
            get
            {
                lock (Lock)
                {
                    if (_msgFunctionOutputReadyPath == null)
                        _msgFunctionOutputReadyPath = Path.Combine(DataDirectory, _msgFunctionOutputReadyFilename);
                    return _msgFunctionOutputReadyPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _msgFunctionOutputReadyFilename = Path.GetFileName(value);
                    }
                    _msgFunctionOutputReadyPath = null;
                }
            }
        }


        #endregion


        #region Messages


        /// <summary>Sets the falg that indicates that the reduced approximation input data is ready.</summary>
        /// $A tako78 Jul.21
        public void SetFunctionInputReady()
        {
            lock (Lock)
            {
                string path = MsgFunctionInputReadyPath;
                using (TextWriter sw = new StreamWriter(path))
                { }
                // Remark:
                // Creation of file with File.Create() is not good because the file is locked in
                // this case and can not be deleted later.
            }
        }

        /// <summary>Clears the falg that indicates that the reduced approximation input data is ready.</summary>
        /// $A tako78 Jul.21
        public void ClearFunctionInputReady()
        {
            lock (Lock)
            {
                string path = MsgFunctionInputReadyPath;
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        /// <summary>Returns value of the falg that indicates whether the reduced approximation input data is ready.</summary>
        /// $A tako78 Jul.21
        public bool IsFunctionInputReady()
        {
            lock (Lock)
            {
                string path = MsgFunctionInputReadyPath;
                if (File.Exists(path))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>Sets the falg that indicates that the reduced approximation output data is ready.</summary>
        /// $A tako78 Jul.21
        public void SetFunctionOutputReady()
        {
            lock (Lock)
            {
                string path = MsgFunctionOutputReadyPath;
                using (TextWriter sw = new StreamWriter(path))
                { }
                // Remark:
                // Creation of file with File.Create() is not good because the file is locked in
                // this case and can not be deleted later.
            }
        }

        /// <summary>Clears the falg that indicates that the reduced approximation output data is ready.</summary>
        /// $A tako78 Jul.21
        public void ClearFunctionOutputReady()
        {
            lock (Lock)
            {
                string path = MsgFunctionOutputReadyPath;
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        /// <summary>Returns value of the falg that indicates whether the reduced approximation output data is ready.</summary>
        /// $A tako78 Jul.21
        public bool IsFunctionOutputReady()
        {
            lock (Lock)
            {
                string path = MsgFunctionOutputReadyPath;
                if (File.Exists(path))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>Clears all messages for neural approximation client and server.</summary>
        /// $A tako78 Jul.21
        public void ClearMessages()
        {
            ClearFunctionInputReady();
            ClearFunctionOutputReady();
        }


        #endregion


        #region OperationBasic


        /// <summary>Reads function reduced input parameters form the file at standard location.</summary>
        /// <param name="inputParameters">Object where parameters are stored.</param>
        /// $A tako78 Jul.21
        public virtual void ReadFunctionInput(ref IVector inputParameters)
        {
            Vector.LoadJson(FunctionInputPath, ref inputParameters);
        }

        /// <summary>Writes function reduced input parameters to standard location.</summary>
        /// <param name="inputParameters">Parameters to be written.</param>
        /// $A tako78 Jul.21
        public virtual void WriteFunctionInput(IVector inputParameters)
        {
            Vector.SaveJson(inputParameters, FunctionInputPath);
        }

        /// <summary>Reads Function reduced output values from the file at standard location.</summary>
        /// <param name="outputValues">Object where the read outut values are stored.</param>
        /// $A tako78 Jul.21
        public virtual void ReadFunctionOutput(ref IVector outputValues)
        {
            Vector.LoadJson(FunctionOutputPath, ref outputValues);
        }

        /// <summary>Writes function reduced outut values to the file at standard location.</summary>
        /// <param name="outputValues">Output values that are written.</param>
        /// $A tako78 Jul.21
        public virtual void WriteFunctionOutput(IVector outputValues)
        {
            Vector.SaveJson(outputValues, FunctionOutputPath);
        }

        /// <summary>Loads the definition data from the file at standard location.</summary>
        /// <param name="trainedNetwork">Object where the loaded neural network approximator is stored.</param>
        /// $A tako78 Jul.21
        public virtual void LoadDataDefinition(ref InputOutputDataDefiniton definitionData)
        {
            InputOutputDataDefiniton.LoadJson(NeuralDataDefinitionPath, ref definitionData);
        }

        /// <summary>Loads the mapping definition data from the file at standard location.</summary>
        /// <param name="trainedNetwork">Object where the loaded neural network approximator is stored.</param>
        /// $A tako78 Jul.21
        public virtual void LoadMappingDefinition(ref MappingDefinition mappingDefinition)
        {
            MappingDefinition.LoadJson(MappingDefinitionPath, ref mappingDefinition);
        }


        #endregion


        #region OperationClient


        /// <summary>Performs client-side test calculation of neural network based approximation where
        /// input parameters are read from a specified function JSON file with reduced input parameters, 
        /// copies to specified JSON file with total inputs and calculated output values are written 
        /// to the specified file.</summary>
        /// <param name="inputFilePath">Path to the JSON file where input parameters are read from.
        /// The file pointed at must exist.</param>
        /// <param name="deletedFilePath">Path of a file where the calculated approximated values are written to.
        /// It can be null or empty string, in this case parameters are not written to a file (but they are still 
        /// output on console).</param>
        /// $A tako78 Jul;
        public virtual void ClientTestCalculateMappingApproximation(string functionInputFilePath, string functionOutputFilePath)
        {
            if (!File.Exists(functionInputFilePath))
                throw new ArgumentException("Function file with reduced input parameters for neural network approximation does not exist: "
                    + Environment.NewLine + "  " + functionInputFilePath + ".");
            IVector functionInputParameters = null;
            IVector functionOutputParameters = null;
            IVector originalInputParameters = null;
            IVector originalOutputParameters = null;
            // Read functioninput file.
            Vector.LoadJson(functionInputFilePath, ref functionInputParameters);
            Console.WriteLine();
            Console.WriteLine("Function Input reduced parameters read from " + functionInputFilePath + ".");
            // Load definition data.
            InputOutputDataDefiniton definitionData = null;
            LoadDataDefinition(ref definitionData);
            Console.WriteLine("Definition data are loaded from " + NeuralDataDefinitionPath + ".");
            // Load mapping definition data.
            MappingDefinition mappingDefinition = null;
            LoadMappingDefinition(ref mappingDefinition);
            Console.WriteLine("Mapping definition data are loaded from " + MappingDefinitionPath + ".");
            // Perform input data mapping.
            if (MapperDefinition == null)
            {
                Aux.MapInput(functionInputParameters, ref originalInputParameters);
            }
            else
                MapperDefinition.MapInput(functionInputParameters, ref originalInputParameters);
            Console.WriteLine("Input data mapping done.");
            // Save neuralinput file for approximation.
            WriteNeuralInput(originalInputParameters);
            Console.WriteLine("Neural Input parameters saved in " + NeuralInputPath + ".");
            // Perform client aproximation with neural network.
            string originalInputFilePath = NeuralInputPath;
            string originalOutputFilePath = NeuralOutputPath;
            ClientTestCalculateApproximation(originalInputFilePath, originalOutputFilePath);
            // Load neuraloutput file.
            ReadNeuralOutput(ref originalOutputParameters);
            Console.WriteLine("Neural Output values read from " + NeuralOutputPath + ".");
            // Perform output data mapping.
            if (MapperDefinition == null)
            {
                Aux.MapOutput(originalOutputParameters, ref functionOutputParameters);
            }
            else
                MapperDefinition.MapOutput(originalOutputParameters, ref functionOutputParameters);
            Console.WriteLine("Output data mapping done.");
            // Save functionoutput file.
            WriteFunctionOutput(functionOutputParameters);
            Console.WriteLine("Function Output reduced parameters saved in " + FunctionOutputPath + ".");
        }


        #endregion


        #region OperationServer


        /// <summary>Performs neural network-based approximation at prescribed reduced input parameters and saves results.
        /// Messages are set and cleared appropriately.
        /// This method read reduced input parameters from standard location, mapps input parameters, loads trained neural network, calculates
        /// approximated outpur values, mapps output values, and stores them to the standard location.</summary>
        /// $A tako78 Jul.21
        public virtual void ServerCalculateMappingApproximation()
        {
            if (!IsFunctionInputReady())
            {
                //Console.WriteLine();
                //Console.WriteLine("WARNING: Function input data is not ready.");
                //Console.WriteLine();
                throw new InvalidOperationException("Function input ready flag is not set.");
            }
            if (IsFunctionOutputReady())
                ClearFunctionOutputReady();
            ClearFunctionInputReady();
            
            IVector functionInputParameters = null;
            IVector functionOutputParameters = null;
            IVector originalInputParameters = null;
            IVector originalOutputParameters = null;
            
            // Read functioninput file.
            ReadFunctionInput(ref functionInputParameters);
            // Load definition data.
            InputOutputDataDefiniton definitionData = null;
            LoadDataDefinition(ref definitionData);
            // Load mapping definition data.
            MappingDefinition mappingDefinition = null;
            LoadMappingDefinition(ref mappingDefinition);
            // Perform input data mapping.
            if (MapperDefinition == null)
            {
                Aux.MapInput(functionInputParameters, ref originalInputParameters);
            }
            else
                MapperDefinition.MapInput(functionInputParameters, ref originalInputParameters);            
            // Save neuralinput file for approximation.
            WriteNeuralInput(originalInputParameters);
            // Perform server aproximation with neural network.
            ServerCalculateApproximation();
            // Load neuraloutput file.
            ReadNeuralOutput(ref originalOutputParameters);
            // Perform output data mapping.
            if (MapperDefinition == null)
            {
                Aux.MapOutput(originalOutputParameters, ref functionOutputParameters);
            }
            else
                MapperDefinition.MapOutput(originalOutputParameters, ref functionOutputParameters);
            // Save functionoutput file.
            WriteFunctionOutput(functionOutputParameters);
            
            SetFunctionOutputReady();
        }


        #endregion
    } // class MappingApproximationFileManager
} 