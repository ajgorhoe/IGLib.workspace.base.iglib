// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Lib;

namespace IG.Num
{

    /// <summary>File manager for interfacing (for optimization purposes) the simulation code 
    /// of Gregor Kosec.</summary>
    /// $A Igor Aug11; 
    public abstract class SimKosecFileManagerBase : IResponseEvaluatorVectorSimple, ILockable
    {

        #region Construction

        /// <summary>Constructs a new file manager for interfacing simulator of Gregor Kosec.</summary>
        /// <param name="dataDirectory">Data directory for simulation code.
        /// This directory usually contains the simulator as well as the input files, and output is written
        /// to the specified locations within the directory.</param>
        protected SimKosecFileManagerBase(string dataDirectory)
        {
            this.DataDirectory = dataDirectory;
            InstallInputFields();
            InstallInputMappings();
        }

        
        /// <summary>Initializes internal variables. Called at the beginning of all constructors.
        /// <para>To be overridden in derived classes!</para></summary>
        protected abstract void Init();


        /// <summary>Installs data about input fields that can be queried and set in the input file.</summary>
        protected virtual void InstallInputFields()
        {
            AddInputFieldDefinitions(
                new InputFieldDefinition(1, "NumX0", "Stevilo tock v X smeri na zacetni porazdelitvi tock"),
                new InputFieldDefinition(2, "NumY0", "Stevilo tock v Y smeri na zacetni porazdelitvi tock "),
                new InputFieldDefinition(3, "dt", "Time step"),
                new InputFieldDefinition(4, "dl", "relaxation parameter"),
                new InputFieldDefinition(5, "g", "gravity coefficient"),
                new InputFieldDefinition(7, "PVI_ACCURACY", "Zahtevana natancnost divergence hitrosti"),
                //new InputFieldDefinition(8, "time"),
                new InputFieldDefinition(8, "time")
                /*
                new InputFieldDefinition(0, ""),
                new InputFieldDefinition(0, ""),
                new InputFieldDefinition(0, ""),
                new InputFieldDefinition(0, ""),
                new InputFieldDefinition(0, ""),
                new InputFieldDefinition(0, "")
                 * */
            );
        }

        /// <summary>Installs definitions of default values of input fields that are automatically set
        /// before running the simulation, and eventually definitios for mappings between (optimization) input parameters
        /// and input fields in input file. 
        /// <para>The latter are eventually used when for each input (optimization) parameter
        /// there exists a field that corresponds to that parameter. Many times this is not true because a single optimization
        /// parameters can affect a whole set of input fields.</para>
        /// <para>When running simulator by calling <see cref="CalculateVectorResponse"/>, input is prepared in 
        /// the following order: First default parameters are set, then automatic mappings are performed (if any are defined)
        /// and finally the manual mappings are performed by calling <see cref="UpdateInputParametersManual"/>, thus
        /// manually defined parameter mapping overrides all others when defined.</para></summary>
        protected abstract void InstallInputMappings();

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

        #region RedirectOutput

        protected bool _redirectOutput = false;

        /// <summary>Gets or sets the plag indicating whether standard output will be 
        /// redirected or not.</summary>
        /// <remarks>If this flag is set to true but path to the file (property <see cref="RedirectedOutputPath"/>) where
        /// output is redirected is not specified yet then this path will be generated form path to input file (just the
        /// ".out" extension is added).</remarks>
        public bool RedirectOutput
        {
            get { return _redirectOutput; }
            set { _redirectOutput = value; if (value == false) _redirectedOutputPath = null; }
        }

        protected string _redirectedOutputPath = null;

        /// <summary>Gets or sets path to the file where standard output will be redirected. 
        /// Setting to null causes that output will not be redirected.</summary>
        public string RedirectedOutputPath
        {
            get 
            {
                if (_redirectedOutputPath == null && _redirectOutput)
                {
                    _redirectedOutputPath = InputPath + ".out";
                }
                return _redirectedOutputPath; 
            }
            set
            {
                _redirectedOutputPath = value;
                if (string.IsNullOrEmpty(_redirectedOutputPath))
                    _redirectOutput = false;
                else
                    _redirectOutput = true;
            }
        }

        /// <summary>Gets or sets name of the file where output is redirected.
        /// <para>Setting to null or empty string means that output will not be redirected.</para>
        /// <para>Setting to non-empty string means that output will be redirected to the file
        /// named as set by this property, and residing in data directory (property <see cref="DataDirectory"/>).</para>
        /// <para>Getter just returns the current state of matters (null if output is not redirected, file name otherwise).</para></summary>
        public string RedirectedOutputFileName
        {
            get
            {
                if (RedirectedOutputPath == null)
                    return null;
                else
                    return Path.GetFileName(RedirectedOutputPath);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    RedirectedOutputPath = null;
                else
                    RedirectedOutputPath = Path.Combine(DataDirectory, value);
            }
        }

        #endregion RedirectOutput

        private int _outputLevel = Util.OutputLevel;

        /// <summary>Level of information that is output to the console by some methods.</summary>
        public int OutputLevel
        {
            get { return _outputLevel; }
            set { _outputLevel = value; }
        }


        #region ProblemData

        private int _numInputParameters;

        /// <summary>Number of input optimization parameters for the simulator.</summary>
        public virtual int NumInputParameters
        {
            get { lock (Lock) {
                if (_numInputParameters <= 0)
                    throw new NotImplementedException("Number of input parameters is not specified tor this simulator.");
                return _numInputParameters; } }
            set { lock (Lock) { _numInputParameters = value; } }
        }
        
        private int _numOutputValues;

        /// <summary>Number of optimization output values produced by the simulator.</summary>
        public virtual int NumOutputValues
        {
            get { lock (Lock) {
                if (_numOutputValues <= 0)
                    throw new NotImplementedException("Number of output vlaues is not specified for this simulator.");
                return _numOutputValues; } }
            set { lock (Lock) { _numOutputValues = value; } }
        }

        #endregion ProblemData


        #region OperationData


        #region DataDirectory

        protected string _directory = null;

        /// <summary>Directory for data and messages exchange through files.</summary>
        public string DataDirectory
        {
            get
            {
                lock (Lock)
                {
                    if (String.IsNullOrEmpty(_directory))
                        throw new InvalidDataException("Directory of the GK direct analysis is not specified.");
                    return _directory;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (String.IsNullOrEmpty(value))
                        throw new ArgumentException("Directory of the GK direct analysis is not specified (null or empty string).");
                    string dir = Path.GetFullPath(value);
                    if (!Directory.Exists(dir))
                    {
                        try
                        {
                            Directory.CreateDirectory(dir);
                        }
                        catch { }
                        if (!Directory.Exists(_directory))
                            throw new ArgumentException("The specified GK direct analysis directory does not exist. "
                                + Environment.NewLine + "  Directory: " + value
                                + Environment.NewLine + "  Full path: " + dir);
                    }
                    _directory = dir;
                }

            }
        }

        /// <summary>Returns full path of the file or directory with the specified relative path
        /// within the data and messages exchange directory.</summary>
        /// <param name="relativePath">Relative path (with respect to data and messages exchange directory)
        /// </param>
        public string GetPath(string relativePath)
        {
            lock (Lock)
            {
                return Path.Combine(DataDirectory, relativePath);
            }
        }


        #endregion DataDirectory


        #region Constants

        protected string
            _csvSeparator = SimKosecConst.CsvSeparatorDefault,
            _templateInputFilename = SimKosecConst.TemplateInputFileNameDefault,
            _templateInputPath,
            _inputFilename = SimKosecConst.InputFileNameDefault,
            _inputPath,
            _executableFilename = SimKosecConst.ExecutableFilenameDefault,
            _executablePath,
            _outputBaseDirname = SimKosecConst.OutputDirectortyBaseDefault,
            _outputBaseDirpath,
            _outputDirname,
            _outputDirpath,

            _optOutputCsvFilename = SimKosecConst.OptimizationOutputFileName,
            _optOutputCsvPath,



            // TODO: delete the last variable!
            xxxfjafjkasjflajfla = "";



        #endregion Constants

        /// <summary>Separator used to separate values in CSV files.</summary>
        public string CsvSeparator
        {
            get { lock (Lock) { return _csvSeparator; } }
            set
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("CSV separator to be assigned is not defined (null or empty string).");
                    _csvSeparator = value;
                }
            }
        }

        /// <summary>File path of the template analysis input file. This file is transcribed, with appropriate
        /// modification according to the optimization parameters, to the actual simulation input file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public virtual string TemplateInputPath
        {
            get
            {
                lock (Lock)
                {
                    if (_templateInputPath == null)
                    {
                        if (DataDirectory == null)
                            throw new InvalidDataException("Path of the root simulator directory not specified (null reference).");
                        _templateInputPath = Path.Combine(DataDirectory, _templateInputFilename);
                    }
                    return _templateInputPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _templateInputFilename = Path.GetFileName(value);
                    }
                    _templateInputPath = null;  // invalidate the path.
                    // Invalidate dependencies:
                    InputFileContents = null;
                }
            }
        }


        /// <summary>File path of the analysis input file. This file contains all the input data for
        /// the direct analysis.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public virtual string InputPath
        {
            get
            {
                lock (Lock)
                {
                    if (_inputPath == null)
                    {
                        if (DataDirectory == null)
                            throw new InvalidDataException("Path of the root simulator directory not specified (null reference).");
                        _inputPath = Path.Combine(DataDirectory, _inputFilename);
                    }
                    return _inputPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _inputFilename = Path.GetFileName(value);
                    }
                    _inputPath = null;  // invalidate the path.
                }
            }
        }

        /// <summary>File name of the executable that performs the direct analysis.</summary>
        public virtual string ExecutableFilename
        {
            get { lock (Lock) { return _executableFilename; } }
            set
            {
                lock (Lock)
                {
                    _executableFilename = value;
                    // Invalidate dependencies:
                    ExecutablePath = null;
                }
            }
        }


        /// <summary>File path of the analysis exeutable. 
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public virtual string ExecutablePath
        {
            get
            {
                lock (Lock)
                {
                    if (_executablePath == null)
                    {
                        if (DataDirectory == null)
                            throw new InvalidDataException("Path of the root simulator directory not specified (null reference).");
                        _executablePath = Path.Combine(DataDirectory, ExecutableFilename);
                    }
                    return _executablePath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        ExecutableFilename = Path.GetFileName(value);
                    }
                    _executablePath = null;  // invalidate the path.
                }
            }
        }


        // input input

        /// <summary>Path of the base directory where output files are located.
        /// Setter takes only pure directory name, without path information.
        /// If set to null then directory path is set to null and will be recalculated when getter is called.</summary>
        public virtual string OutputBaseDirpath
        {
            get
            {
                lock (Lock)
                {
                    if (_outputBaseDirpath == null)
                    {
                        if (string.IsNullOrEmpty(DataDirectory))
                            throw new InvalidDataException("Path of the root simulator directory not specified (null reference).");
                        _outputBaseDirpath = Path.Combine(DataDirectory,  // RootDirectory contains absolute path
                            _outputBaseDirname);
                    }
                    return _outputBaseDirpath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        char lastCharacter = value[value.Length - 1];
                        if (lastCharacter != Path.DirectorySeparatorChar && lastCharacter != Path.AltDirectorySeparatorChar)
                        {
                            // The last character must be directory separator, otherways directory name is not extracted
                            // correctly:
                            value = value + Path.DirectorySeparatorChar;
                            //throw new ArgumentException("Invalid directory name or path when setting execution directory."
                            //    + Environment.NewLine + "  path should end with directory separator."
                            //    + Environment.NewLine + "  provided path: " + value);
                        }
                        // We have to extract pure directory name:
                        _outputBaseDirname = Path.GetFileName(Path.GetDirectoryName(value));
                    }
                    _outputBaseDirpath = null;
                    // Invalidate dependencies:
                    OutputDirpath = null;
                }
            }
        }


        /// <summary>Path of the directory where output files are located.
        /// Setter takes only pure directory name, without path information.
        /// If set to null then directory path is set to null and will be recalculated when getter is called.</summary>
        public virtual string OutputDirpath
        {
            get
            {
                lock (Lock)
                {
                    if (_outputDirpath == null)
                    {
                        if (string.IsNullOrEmpty(OutputBaseDirpath))
                            throw new InvalidDataException("Path of output base directory not specified (null reference).");
                        _outputDirpath = Path.Combine(OutputBaseDirpath,  // RootDirectory contains absolute path
                            _inputFilename /* _outputDirname */ );
                    }
                    return _outputDirpath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        char lastCharacter = value[value.Length - 1];
                        if (lastCharacter != Path.DirectorySeparatorChar && lastCharacter != Path.AltDirectorySeparatorChar)
                        {
                            // The last character must be directory separator, otherways directory name is not extracted
                            // correctly:
                            value = value + Path.DirectorySeparatorChar;
                            //throw new ArgumentException("Invalid directory name or path when setting execution directory."
                            //    + Environment.NewLine + "  path should end with directory separator."
                            //    + Environment.NewLine + "  provided path: " + value);
                        }
                        // We have to extract pure directory name:
                        _outputBaseDirname = Path.GetFileName(Path.GetDirectoryName(value));
                    }
                    _outputDirpath = null;
                    // Invalidate dependencies:
                    OptOutputCsvPath = null;
                }
            }
        }



        /// <summary>File path of the analysis output file in CSV format. This file contains the pre-agreed
        /// set of output values obtained by post-processing of simulator results.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string OptOutputCsvPath
        {
            get
            {
                lock (Lock)
                {
                    if (_optOutputCsvPath == null)
                    {
                        if (OutputDirpath == null)
                            throw new InvalidDataException("Path of the simuator output directory not specified (null reference).");
                        _optOutputCsvPath = Path.Combine(OutputDirpath, _optOutputCsvFilename);
                    }
                    return _optOutputCsvPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _optOutputCsvFilename = Path.GetFileName(value);
                    }
                    _optOutputCsvPath = null;  // invalidate the path.
                }
            }
        }


        private string _inputFileContents;

        private string[][] _inputFileLines;

        /// <summary>The current input file contents.
        /// By default this is obtained form the template input file.
        /// If the property is set to null at some point then it will be set to contents of the 
        /// template input file at the first get access.</summary>
        public virtual string InputFileContents
        {
            get
            {
                lock (Lock)
                {
                    if (_inputFileContents == null)
                    {
                        if (TemplateInputPath == null)
                            throw new InvalidDataException("Template simulation input file path is not specified (null string).");
                        else if (!File.Exists(TemplateInputPath))
                            throw new InvalidDataException("Template simulation input file does not exist. "
                                + Environment.NewLine + "  Path: " + TemplateInputPath + ".");
                        using (TextReader tr = new StreamReader(TemplateInputPath))
                        {
                            _inputFileContents = tr.ReadToEnd();
                        }
                    }
                    return _inputFileContents;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    _inputFileContents = value;
                    // Invalidate dependencies:
                    InputFileLines = null;
                }
            }
        }

        /// <summary>The current input represented as an array of text lines.</summary>
        public virtual string[][] InputFileLines
        {
            get
            {
                lock (Lock)
                {
                    if (_inputFileLines == null)
                    {
                        if (InputFileContents != null)
                        {
                            List<string[]> lines = new List<string[]>();
                            using (StringReader sr = new StringReader(InputFileContents))
                            {
                                string line;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    // first, each line entry will contain only one string containing the whole line
                                    // (no splitting performed):
                                    lines.Add(new string[] { line });
                                }
                            }
                            InputFileLines = lines.ToArray();
                        }
                    }
                    return _inputFileLines;
                }
            }
            set
            {
                lock (Lock)
                {
                    _inputFileLines = value;
                }
            }
        }


        /// <summary>Saves contents of the simulation input file.
        /// Usually the contents is generated by taking the contents of the template  input 
        /// file and making some modification in it in order to reflect the current values
        /// of optimization parameters.</summary>
        public virtual void SaveInput()
        {
            lock (Lock)
            {
                if (InputPath == null)
                    throw new InvalidDataException("Input file path is not specified (null reference).");
                using (TextWriter tw = new StreamWriter(InputPath))
                {
                    tw.Write(InputFileContents);
                }
            }
        }

        #endregion OperationData


        #region AutomaticInput

        // Tools for automatically adjusting input fields acdcording to definitions stored on designated lists

        private List<InputFieldDefinition> _optimizationParameters = new List<InputFieldDefinition>();

        private List<InputFieldDefinition> _defaultInputValues = new List<InputFieldDefinition>();



        /// <summary>Finds and returns the specified input field definition on the specified list.</summary>
        /// <param name="list">List that is searched for the specified definition.</param>
        /// <param name="id">ID of the definiton. If less or equal to 0 then definition is searched only 
        /// by field name.</param>
        /// <param name="name">Field name that it searched for. If <paramref name="id"/> is greater than 0
        /// then the definition that is found must have matching names and IDs, otherwise exception is thrown.</param>
        /// <returns>The definition form the list that matched the specified field ID and/or field name, 
        /// or null if such a definitio is not found on the list.</returns>
        protected virtual InputFieldDefinition GetDefinition(List<InputFieldDefinition> list, int id, string name)
        {
            if (id < 1 && string.IsNullOrEmpty(name))
                throw new ArgumentException("Neither field name nor field ID are specified.");
            foreach (InputFieldDefinition def in list)
            {
                if (def != null)
                {
                    if (id > 0)
                    {
                        if (id == def.Id)
                        {
                            if (!string.IsNullOrEmpty(name))
                            {
                                if (name != def.Name)
                                    throw new ArgumentException("Names of searched and stored deinitions do not match for ID = " + id + ". "
                                        + Environment.NewLine + "  searched: " + name + ", stored: " + def.Name + ".");
                            }
                            return def;
                        }
                    }
                    else
                    {
                        // only name specified
                        if (!string.IsNullOrEmpty(name))
                        {
                            if (name == def.Name)
                                return def;
                        }
                    }
                }
            }
            return null;
        }


        /// <summary>List containing default values for input parameters, which will be set before 
        /// the simulation input file is prepared, which makes possible to play with the definition of the
        /// template direct problem.</summary>
        protected List<InputFieldDefinition> DefaultInputValues
        {
            get { 
                lock (Lock) 
                { 
                    return _defaultInputValues; 
                } 
            }
        }

        /// <summary>Adds a set of default input values to the list <see cref="DefaultInputValues"/>
        /// of default values. These values will be set when input file is prepared.</summary>
        /// <param name="definitions"></param>
        protected void AddDefaultInputValues(params InputFieldDefinition[] definitions)
        {
            lock (Lock)
            {
                if (definitions != null)
                {
                    for (int i = 0; i < definitions.Length; ++i)
                    {
                        InputFieldDefinition def = definitions[i];
                        if (def == null)
                            throw new ArgumentException("Definition of optimization parameter (No. " + i + " in the array to be added) not specified (null reference).");

                        InputFieldDefinition current = GetDefinition(DefaultInputValues, def.Id, def.Name);
                        if (current != null)
                        {
                            // There is already a definition of this field, we will perform replacement:
                            DefaultInputValues.Remove(current);
                        }
                        DefaultInputValues.Add(def);
                    }
                }
            }
        }

        /// <summary>Finds and returns the specified deefault input value definition. <seealso cref="GetDefinition"/></summary>
        /// <param name="list">List that is searched for the specified definition.</param>
        /// <param name="id">ID of the definiton. If less or equal to 0 then definition is searched only 
        /// by field name.</param>
        /// <param name="name">Field name that it searched for. If <paramref name="id"/> is greater than 0
        /// then the definition that is found must have matching names and IDs, otherwise exception is thrown.</param>
        /// <returns>The definition form the list that matched the specified field ID and/or field name, 
        /// or null if such a definitio is not found on the list.</returns>
        protected InputFieldDefinition GetDefaultInputValue(int id, string name)
        {
            return GetDefinition(DefaultInputValues, id, name);
        }

        /// <summary>Updates the input field values for which default values are defined on
        /// the list <see cref="DefaultInputValues"/>.</summary>
        public void UpdateDefaultInputValues()
        {
            lock (Lock)
            {
                int numDefined = DefaultInputValues.Count;
                for (int i = 0; i < numDefined; ++i)
                {
                    InputFieldDefinition def = DefaultInputValues[i];
                    if (def != null)
                    {
                        if (def.DefaultValueDefined)
                            SetInputFieldValue(def, def.DefaultValue);
                    }
                }
            }
        }

        /// <summary>List of definitions of direct mappings od input (optimization) 
        /// parameters to input fields.
        /// Contains, for each input parameter, the definition of the input field that is represented
        /// by this parameter.
        /// This list can contain null references, which are usually for parameters that can not 
        /// be directly mapped to scalar input fields defined in the simulation input file.</summary>
        protected List<InputFieldDefinition> OptimizationParameterDefinitions
        {
            get { lock (Lock) { return _optimizationParameters; } }
        }

        /// <summary>Adds a set of optimization parameter definitions to the list.
        /// Each definitios specifies which parameter in the simulation input file corresponds to a given 
        /// optimization parameter.
        /// It is alloved that some definitions are null. The corresponding optimization parameters will not
        /// be mapped automatically to input fields by functions such as <see cref="UpdateOptimizationParametersDefined"/>,
        /// but tehir mapping must be provided specially in definitions of (usually overridden) functions such as 
        /// <see cref="UpdateInputParametersManual"/>.</summary>
        /// <param name="definitions">Definitions to be added. If any of them is null then an
        /// <see cref="ArgumentException"/> is thrown.</param>
        protected void AddOptimizationParameterDefinitions(params InputFieldDefinition[] definitions)
        {
            lock (Lock)
            {
                if (definitions != null)
                {
                    for (int i = 0; i < definitions.Length; ++i)
                    {
                        InputFieldDefinition def = definitions[i];
                        //if (def == null)
                        //    throw new ArgumentException("Definition of optimization parameter (No. " + i + " in the array to be added) not specified (null reference).");
                        OptimizationParameterDefinitions.Add(def);
                    }
                }
            }
        }

        /// <summary>Finds and returns the specified optimization parameter definition. <seealso cref="GetDefinition"/></summary>
        /// <param name="list">List that is searched for the specified definition.</param>
        /// <param name="id">ID of the definiton. If less or equal to 0 then definition is searched only 
        /// by field name.</param>
        /// <param name="name">Field name that it searched for. If <paramref name="id"/> is greater than 0
        /// then the definition that is found must have matching names and IDs, otherwise exception is thrown.</param>
        /// <returns>The definition form the list that matched the specified field ID and/or field name, 
        /// or null if such a definitio is not found on the list.</returns>
        protected InputFieldDefinition GetOptimizationParameterDefinition(int id, string name)
        {
            return GetDefinition(OptimizationParameterDefinitions, id, name);
        }


        /// <summary>Returns value of the specified input (optimization) parameter obtained from 
        /// the current contents of simulation input, according to the definition of direct 
        /// mapping between this parameter and scalar input field value.
        /// If mapping is not defined for the component specified then null is returned.
        /// Returned value is obtained by <see cref="GetInputFieldValue"/>.</summary>
        /// <param name="which">Specifies which optimization parameter is returned.</param>
        /// <returns>Value of the specified input (optimization) parameter if direct mapping 
        /// between that parameter and a scalar simulation input field is defined, or null if such 
        /// mapping is not defined.</returns>
        public string GetOptimizationParametersDefinedFromInput(int which)
        {
            int numDef = OptimizationParameterDefinitions.Count;
            if (which < 0)
                throw new ArgumentException("Input parameter number less than 0.");
            if (which >= numDef)
                return null;
            // throw new ArgumentException("Input parameter index " + which " is larger than number of definitions of direct parameter mappings.");
            InputFieldDefinition def = OptimizationParameterDefinitions[which];
            if (def == null)
                return null;
            else
                return GetInputFieldValue(def);
        }


        /// <summary>Gets current values of input (optimization) parameters obtained from 
        /// the current contents of simulation input, according to the definitions of direct 
        /// mappings between input parameters and scalar input field values.
        /// Values are written to components of the provided vector. Vector is not resized, 
        /// therefore it must be of correct dimension. 
        /// Those components for which direct mapping between a specific input parameter and
        /// scalar input field in simulation input is not defined, are not obtained and stored
        /// in vector components (i.e., eventual previous values are preserved). 
        /// Returned value is obtained by <see cref="GetInputFieldValue"/>.</summary>
        /// <param name="param">Vector of parameters that are provided.</param>
        public void GetOptimizationParametersDefinedFromInput(IVector param)
        {
            if (param != null)
            {
                for (int i = 0; i < param.Length; ++i)
                {
                    string val = GetOptimizationParametersDefinedFromInput(i);
                    if (val != null)
                        param[i] = double.Parse(val);
                }
            }
        }

        /// <summary>Updates simulation inputs according to the values of input (optimization) parameters for
        /// those parameters for which direct mapping to input fields are defined.
        /// <para>Throws exception if the number of definitions is greater than 0 but also greater than 
        /// dimension of the specified parameter vector (i.e. all definitions must be used, if any defined).</para></summary>
        /// <param name="parameters">Vector of values of input (optimization) parameters to be 
        /// mapped to simulation input.</param>
        /// <exception cref="ArgumentException">If number of definitions of parameters installed is greater than 0
        /// but the specified vector of parameters is null or of dimension smaller than the number of definitions.</exception>
        public virtual void UpdateOptimizationParametersDefined(IVector parameters)
        {
            lock (Lock)
            {
                int numDefined = OptimizationParameterDefinitions.Count;
                if (numDefined > 0)
                {
                    if (parameters == null)
                        throw new ArgumentNullException("Vector of input parameters is not defined (null reference).");
                    if (parameters.Length < numDefined)
                        throw new ArgumentException("Number of provided input parameters (" + parameters.Length +
                            ") is smaller than the number of defined direct mappings to input fields (" + numDefined + ").");
                    
                    for (int i = 0; i < numDefined && i<parameters.Length; ++i)
                    {
                        InputFieldDefinition def = OptimizationParameterDefinitions[i];
                        if (def != null)
                            SetInputFieldValue(def, parameters[i].ToString());
                    }
                }
            }
        }

        /// <summary>Updates simulation input with default values and with values of optimization parameters
        /// for which direct mappings to input fields are defined (the latter only when vector of parameters is
        /// specified, i.e. not null).</summary>
        /// <param name="parameters">Vector of input (optimization) parameters.</param>
        public void UpdateInputDefined(IVector parameters)
        {
            UpdateDefaultInputValues();
            if (parameters != null)
                UpdateOptimizationParametersDefined(parameters);
        }

        #endregion AutomaticInput


        #region InputFieldDefinitions

        private SortedDictionary<string, InputFieldDefinition> _inputFields = new SortedDictionary<string, InputFieldDefinition>();


        /// <summary>Gets the sorted dictionary that contains input field definitions.
        /// This should be used only exceptionally; use the higher level method for dealing with input field definitions.</summary>
        protected SortedDictionary<string, InputFieldDefinition> InputFieldDefinitions
        {
            get { return _inputFields; }
        }

        /// <summary>Returns an input field definition that corresponds to the specified field ID.
        /// Exception is thrown if the field with this ID is not yet contained in input field definitions.</summary>
        /// <param name="fieldId">Input field ID. Should be greater than 0.</param>
        public virtual InputFieldDefinition GetInputFieldDefinition(int fieldId)
        {
            return GetInputFieldDefinition(fieldId, null, null);
        }


        /// <summary>Returns an input field definition that corresponds to the specified field name.
        /// Exception is thrown if definition with this name does not yet exist.</summary>
        /// <param name="fieldName">Input field name.</param>
        public virtual InputFieldDefinition GetInputFieldDefinition(string fieldName)
        {
            return GetInputFieldDefinition(-1, fieldName, null);
        }


        /// <summary>Returns an input field definition that corresponds to the specified field ID
        /// and field name.
        /// If a matching definition is already included then that definition is returned,
        /// if not then definition is created anew. Exception is thrown if neither of this can be done.</summary>
        /// <param name="fieldId">Input field ID. Less or equal to 0 means undefined.</param>
        /// <param name="fieldName">Input field name.</param>
        public virtual InputFieldDefinition GetInputFieldDefinition(int fieldId, string fieldName)
        {
            return GetInputFieldDefinition(fieldId, fieldName, null);
        }

        /// <summary>Returns an input field definition that corresponds to the specified field ID
        /// and field name.
        /// If a matching definition is already included then that definition is returned,
        /// if not then definition is created anew. Exception is thrown if neither of this can be done.</summary>
        /// <param name="fieldId">Input field ID. Less or equal to 0 means undefined.</param>
        /// <param name="fieldName">Input field name.</param>
        /// <param name="fieldDescription">Input field description.</param>
        public virtual InputFieldDefinition GetInputFieldDefinition(int fieldId, string fieldName, string fieldDescription)
        {
            InputFieldDefinition def = null;
            lock (Lock)
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    if (fieldId <= 0)
                        throw new ArgumentException("Could not get an input field definition: neither field name nor Id is specified.");
                    foreach (InputFieldDefinition current in InputFieldDefinitions.Values)
                    {
                        if (current.Id == fieldId)
                        {
                            def = current;
                            // Update field description if specified by argument but not specified on already contained definition:
                            if (!string.IsNullOrEmpty(fieldDescription) && string.IsNullOrEmpty(current.Description))
                                current.Description = fieldDescription;
                            break;
                        }
                    }
                    if (def == null)
                        throw new ArgumentException("Could not get a field definition: field with ID " + fieldId + " is not defined.");
                }
                else
                {
                    if (InputFieldDefinitions.ContainsKey(fieldName))
                    {
                        def = InputFieldDefinitions[fieldName];
                        if (fieldId > 0 && fieldId != def.Id)
                            throw new ArgumentException("Could not get an input field definition: Field ID " + fieldId
                                + " does not match that of the stored field definition (" + def.Id + ").");
                        // Update field description on the containing definition if currently not defined:
                        if (!string.IsNullOrEmpty(fieldDescription) && string.IsNullOrEmpty(def.Description))
                            def.Description = fieldDescription;
                    }
                    else
                    {
                        if (fieldId <= 0)
                            throw new ArgumentException("Can not get an input field definition: ID not specified and definition with name "
                                + def.Name + " does not yet exist.");
                        def = new InputFieldDefinition(fieldId, fieldName, fieldDescription);
                        // Field definition not yet contained in definitions, add it:
                        AddInputFieldDefinition(def);
                    }
                }
            }
            if (def == null)
                throw new ArgumentException("Can not get input field definition, reason unknown. Field ID: "
                    + def.Id + ", Name: " + def.Name + ".");
            return def;
        }

        /// <summary>Adds a new input field definition. Name of the definition must be unique (not already added).</summary>
        /// <param name="def">Input field definition to be added.</param>
        public virtual void AddInputFieldDefinition(InputFieldDefinition def)
        {
            lock (Lock)
            {
                if (def == null)
                    throw new ArgumentNullException("Input field definition to be added is not specified (null reference).");
                if (InputFieldDefinitions.ContainsKey(def.Name))
                {
                    // Definition with given field name already exists, check that it 
                    InputFieldDefinition containedDefinition = InputFieldDefinitions[def.Name];
                    if (containedDefinition.Id != def.Id)
                        throw new ArgumentException("Adding definition: Input field with name " + def.Name
                            + " already included, but with a different ID (" + containedDefinition.Id + " instead of "
                            + def.Id + ").");
                    InputFieldDefinitions[def.Name] = def; // update - use this definition from now on
                } else
                    InputFieldDefinitions.Add(def.Name, def);
            }
        }

        /// <summary>Adds a set of input field definitions by calling <seealso cref="AddInputFieldDefinition"/>.</summary>
        /// <param name="definitions">Definitions to be added (can be null).</param>
        public virtual void AddInputFieldDefinitions(params InputFieldDefinition[] definitions)
        {
            lock (Lock)
            {
                if (definitions != null)
                {
                    for (int i = 0; i < definitions.Length; ++i)
                        AddInputFieldDefinition(definitions[i]);
                }
            }
        }


        /// <summary>Removes the specified input field definition from the stored collection of definitions.</summary>
        /// <param name="fieldId">Field ID of the definiiton to be removed. Must be greater than 0.</param>
        public void ClearInputFieldDefinition(int fieldId)
        {
            ClearInputFieldDefinition(fieldId, null);
        }

        /// <summary>Removes the specified input field definition from the stored collection of definitions.</summary>
        /// <param name="fieldName">Field name of the definition to be removed.</param>
        public void ClearinputfieldDefinition(string fieldName)
        {
            ClearInputFieldDefinition(0, fieldName);
        }


        /// <summary>Removes the specified input field definition from the stored collection of definitions.</summary>
        /// <param name="fieldId">Field ID. This is used to find the input field definition 
        /// if field name is not specified.</param>
        /// <param name="fieldName">Field name. If not specified the field ID is used to find
        /// the definition to be removed, otherwise field ID is just used to check if the 
        /// field name is consistent with ID.</param>
        public void ClearInputFieldDefinition(int fieldId, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                if (fieldId <= 0)
                    throw new ArgumentException("Removing input field definition: neither field name nor ID is defined.");
                foreach (InputFieldDefinition def in InputFieldDefinitions.Values)
                {
                    if (def.Id == fieldId)
                    {
                        InputFieldDefinitions.Remove(def.Name);
                        break;
                    }
                }
            }
            else
            {
                if (InputFieldDefinitions.ContainsKey(fieldName))
                {
                    if (fieldId > 0)
                    {
                        InputFieldDefinition def = InputFieldDefinitions[fieldName];
                        if (def.Id != fieldId)
                        {
                            throw new ArgumentException("Input field ID does not match the ID of the installed field named " + fieldName
                                + ": " + fieldId + " instead of " + def.Id + ".");
                        }
                    }
                    InputFieldDefinitions.Remove(fieldName);
                }
            }
        }

        /// <summary>Clears all input fielddefinitions.</summary>
        public void ClearinputFieldDefinitions()
        {
            lock (Lock)
            {
                InputFieldDefinitions.Clear();
            }
        }

        public virtual void RemoveInputFieldDefinitions()
        {
        }


        #endregion InputFieldDefinitions


        #region InputManipulation

        /// <summary>Sets the sepcified input field to the specified value.
        /// This is done by appending a corresponding text to the contents of the input file 
        /// (at this stage, the file is not modified yet).</summary>
        /// <param name="fieldName">Name of the field to be set.</param>
        /// <param name="fieldValue">Value to be assigned to the specified field.</param>
        public void SetInputFieldValue(string fieldName, string fieldValue)
        {
            SetInputFieldValue(0, fieldName, fieldValue);
        }

        /// <summary>Sets the sepcified input field to the specified value.
        /// This is done by appending a corresponding text to the contents of the input file 
        /// (at this stage, the file is not modified yet).</summary>
        /// <param name="fieldId">Id of the field to be set.</param>
        /// <param name="fieldValue">Value to be assigned to the specified field.</param>
        public void SetInputFieldValue(int fieldId, string fieldValue)
        {
            SetInputFieldValue(fieldId, null, fieldValue);
        }

        /// <summary>Sets the sepcified input field to the specified value.
        /// This is done by appending a corresponding text to the contents of the input file 
        /// (at this stage, the file is not modified yet).</summary>
        /// <param name="fieldId">Id of the field to be set.</param>
        /// <param name="fieldName">Name of the field to be set.</param>
        /// <param name="fieldValue">Value to be assigned to the specified field.</param>
        public void SetInputFieldValue(int fieldId, string fieldName, string fieldValue)
        {
            lock (Lock)
            {
                InputFieldDefinition def = GetInputFieldDefinition(fieldId, fieldName);
                if (def == null)
                    throw new ArgumentException("Could not get the input field definition, ID: " + fieldId + ", name: " + fieldName);
                else
                    SetInputFieldValue(def, fieldValue);
            }
        }

        /// <summary>Sets the sepcified input field to the specified value.
        /// This is done by appending a corresponding text to the contents of 
        /// the input file (at this stage, the file is not modified yet).</summary>
        /// <param name="def">Definition of the input field to be set.</param>
        /// <param name="fieldValue">Value to be assigned to the field.</param>
        public void SetInputFieldValue(InputFieldDefinition def, string fieldValue)
        {
            lock (Lock)
            {
                if (def == null)
                    throw new ArgumentException("Input field definition not specified (null reference).");
                InputFileContents = InputFileContents +
                    "  " + def.Id + "  " + def.Name + "  " + fieldValue + " " + Environment.NewLine;
            }
        }


        /// <summary>Adds comment to the input file.</summary>
        /// <param name="commentString">Comment text.</param>
        public void AddInputComment(string commentString)
        {
            InputFileContents = InputFileContents + Environment.NewLine + SimKosecConst.CommentLineString + commentString + Environment.NewLine;
        }



        /// <summary>Returns value of the sepcified input field.</summary>
        /// <param name="fieldName">Name of the field to be set.</param>
        public string GetInputFieldValue(string fieldName)
        {
            return GetInputFieldValue(0, fieldName);
        }

        /// <summary>Returns value of the sepcified input field.</summary>
        /// <param name="fieldId">Id of the field to be set.</param>
        public string GetInputFieldValue(int fieldId)
        {
            return GetInputFieldValue(fieldId, null);
        }

        /// <summary>Returns value of the sepcified input field.</summary>
        /// <param name="fieldId">Id of the field to be set.</param>
        /// <param name="fieldName">Name of the field to be set.</param>
        public string GetInputFieldValue(int fieldId, string fieldName)
        {
            lock (Lock)
            {
                InputFieldDefinition def = GetInputFieldDefinition(fieldId, fieldName);
                if (def == null)
                    throw new ArgumentException("Could not get the input field definition, ID: " + fieldId + ", name: " + fieldName);
                return GetInputFieldValue(def);
            }
        }


        /// <summary>Returns the value of the specified input field, which is obtained from
        /// the contents of teh input file.</summary>
        /// <param name="def">Definition data for the input field whose value is to be returned.</param>
        /// <remarks>Current contents of the input file are stored in the internal structure (lines of text
        /// split in such a way that ID, field name and value can be readily obtained). When input field values
        /// are added, this data is modified such that changes are immediately taken into account. Algorithm
        /// for this is rather naive (complete input is invalidated upon each change), therefore it is 
        /// recommended to first querry all the necessary parameters and then change anything,
        /// or to perform all the changes first and then query input field values.</remarks>
        public string GetInputFieldValue(InputFieldDefinition def)
        {
            lock (Lock)
            {
                string ret = null;
                if (def == null)
                    throw new ArgumentException("Input field definition not specified (null reference).");
                string name = def.Name;
                int id = def.Id;
                char[] delimiters = new char[] { ' ', '\n', '\r', '\t' };
                for (int i = InputFileLines.Length - 1; i >= 0; --i)
                {
                    if (InputFileLines[i] == null)
                        continue;
                    if (InputFileLines[i].Length == 0)
                        continue;
                    if (InputFileLines[i].Length == 1)
                    {
                        if (InputFileLines[i][0].Length <= 3)
                            continue;
                        else
                            // The current line of text has not been split yet, do it:
                            InputFileLines[i] = InputFileLines[i][0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    }
                    string[] parts = InputFileLines[i];
                    if (parts.Length < 3)
                        continue;
                    int readId = 0;
                    string readName = null;
                    string readValue = null;
                    if (!int.TryParse(parts[0], out readId))
                        continue;
                    readName = parts[1];
                    readValue = parts[2];
                    if (readId == def.Id)
                    {
                        if (def.Name != null)
                        {
                            if (readName != def.Name)
                                throw new ArgumentException("Getting input field value: the specified field name does not match the name in input file. "
                                    + Environment.NewLine + "  Field ID: " + readId + ", specified field name: " + def.Name
                                    + ", read name: " + readName);
                        }
                        // We have found the last line where field ID is the same as the specified ID, and we return the value 
                        // of that field form the (parsed) contents of the input file: 
                        ret = readValue;
                        break;
                    }
                }
                return ret;
            }
        }

        #endregion InputManipulation

        #region OperationBasic


        /// <summary>Gets the current values of input parameters form the input file.
        /// This method must be overridden in derived concrete classes.</summary>
        /// <param name="inputParameters">Vector object where current values of input parameters are stored.</param>
        protected abstract void GetInputParametersManual(ref IVector inputParameters);

        protected IBoundingBox _inputParameterBounds;

        /// <summary>Bounds on input parameters.</summary>
        public virtual IBoundingBox InputParameterBounds
        {
            get
            {
                lock (Lock)
                {
                    if (_inputParameterBounds == null)
                        throw new NotImplementedException("Bounds on input parameters are not defined for this simulator.");
                    return _inputParameterBounds;
                }
            }
            protected set
            {
                _inputParameterBounds = value;
            }
        }
        
        /// <summary>Repairs simulation parameters, if necessary, in such a way that values are consistent with
        /// simuation data (e.g. spacing of nodes).</summary>
        /// <param name="parameters">Vector of parameters to be repaired. Repaired values are stored in the same vector.</param>
        /// <returns>true if parameters were corrected, false otherwise.</returns>
        public abstract bool RepairInputParameters(IVector parameters);

        /// <summary>Prepares current values of input parameters in the simulation input that
        /// will be written to the simulation input file.
        /// This method must be overridden in derived concrete classes.
        /// Only mappings form input parameters to simulation input that are manually defined must 
        /// be performed by this function, since automatic mappings are already included in functions
        /// such as <see cref="WriteInputParameters"/>.</summary>
        /// <param name="inputParameters">Vector of input (optimization) parameter values to be set.</param>
        protected abstract void UpdateInputParametersManual(IVector inputParameters);


        /// <summary>Reads optimization input parameters form the file at a standard location.</summary>
        /// <param name="inputParameters">Reference to an object where input parameters 
        /// are stored.
        /// If number of inpput parameters is defined then the provided vector is resized if necessary.
        /// Otherwise, the vector provided must be of correct dimension.</param>
        public virtual void ReadInputParameters(ref IVector inputParameters)
        {
            lock (Lock)
            {
                if (NumInputParameters > 0)
                {
                    if (inputParameters == null)
                        Vector.Resize(ref inputParameters, NumInputParameters);
                    if (inputParameters.Length != NumInputParameters)
                        Vector.Resize(ref inputParameters, NumInputParameters);
                }
                GetOptimizationParametersDefinedFromInput(inputParameters);  // automatic extraction of certain input parameters
                GetInputParametersManual(ref inputParameters);  // manual extraction of certain input parameters
                if (inputParameters == null)
                    throw new InvalidDataException("The read-in vector of input parameters is null.");
                if (inputParameters.Length < 1)
                    throw new InvalidDataException("The read-in vector of input parameters has dimension less than 1.");
                if (NumInputParameters != inputParameters.Length)
                {
                    if (NumInputParameters <= 0)
                        NumInputParameters = inputParameters.Length;
                    else
                        throw new ArgumentException("Invalid number of read-in input parameters: " + inputParameters.Length + ", should be " + this.NumInputParameters + ".");
                }
            }
        }


        /// <summary>Writes optimization input parameters to the standard location.</summary>
        /// <param name="outputValues">Parameters to be written.</param>
        public virtual void WriteInputParameters(IVector inputParameters)
        {
            lock (Lock)
            {
                if (inputParameters == null)
                    throw new ArgumentNullException("Vector of input parameters not specified (null reference).");
                if (inputParameters.Length < 1)
                    throw new ArgumentException("Invalid vector of input parameters: dimension is 0.");
                if (NumInputParameters != inputParameters.Length)
                {
                    if (NumInputParameters <= 0)
                        NumInputParameters = inputParameters.Length;
                    else
                        throw new ArgumentException("Invalid number of input parameters to be written: " + inputParameters.Length + ", should be " + this.NumInputParameters + ".");
                }
                AddInputComment("Input parameters (automatic definitions):");
                UpdateInputDefined(inputParameters);  // automatic updates of default input and simulation input according to input parameters
                AddInputComment("Input parameters (manual definitions):");
                UpdateInputParametersManual(inputParameters);  // manual updates of simulation input according to input parameters.
                SaveInput();
                if (OutputLevel>=7)
                    Console.WriteLine(Environment.NewLine + "Input file contents: " + Environment.NewLine + InputFileContents + Environment.NewLine);
            }
        }

        /// <summary>Deletes the output files. 
        /// <para>It is recommendable to delete simulation output before running simulation.
        /// In this way, one would know if the simulation program has been broken because the
        /// output file would not exist or would be corrupted (otherwise, one could simply read 
        /// the output file genenerated in some previout run, without noticing that something
        /// is wrong).</summary>
        public virtual void DeleteOutputFiles()
        {
            lock (Lock)
            {
                if (File.Exists(OptOutputCsvPath))
                    File.Delete(OptOutputCsvPath);
            }
        }

        /// <summary>Reads optimization output values form the file at a standard location.</summary>
        /// <param name="outputValues">Reference to an object where output parameters 
        /// will be stored.</param>
        public virtual void ReadOutputValues(ref IVector outputValues)
        {
            lock (Lock)
            {
                if (!File.Exists(OptOutputCsvPath))
                {
                    outputValues = null;
                    return;
                }
                try
                {
                    Vector.LoadCsv(OptOutputCsvPath, CsvSeparator, ref outputValues);
                }
                catch
                {
                    // Seems that output file is corrupted, set output values to null:
                    outputValues = null;
                    return;
                }
                if (outputValues == null)
                    throw new InvalidDataException("The read-in vector of output values is null.");
                if (outputValues.Length < 1)
                    throw new InvalidDataException("The read-in vector of output values has dimension less than 1.");
                if (NumOutputValues != outputValues.Length)
                {
                    if (NumOutputValues <= 0)
                        NumOutputValues = outputValues.Length;
                    else
                        throw new ArgumentException("Invalid number of read-in output values: " + outputValues.Length + ", should be " + this.NumOutputValues + ".");
                }
            }
        }

        /// <summary>Writes optimization ouptut values to the standard location.</summary>
        /// <param name="outputValues">Values to be written.</param>
        public virtual void WriteOutputValues(IVector outputValues)
        {
            lock (Lock)
            {
                if (outputValues == null)
                    throw new ArgumentNullException("Vector of output values to be written is not specified (null reference).");
                if (outputValues.Length < 1)
                    throw new ArgumentException("Invalid vector of output values: dimension is 0.");
                if (NumOutputValues != outputValues.Length)
                {
                    if (NumOutputValues <= 0)
                        NumOutputValues = outputValues.Length;
                    else
                        throw new ArgumentException("Invalid number of output values to be written: " + outputValues.Length + ", should be " + this.NumOutputValues + ".");
                }
                Vector.SaveCsv(outputValues, this.OptOutputCsvPath, CsvSeparator);
            }
        }




        private string[] _oneCommandArgument = new string[1];

        /// <summary>Used to run the simulatior in the default case where there is
        /// only one command-line argument.</summary>
        protected string[] OneCommandArgument
        { get { return _oneCommandArgument; } }

        /// <summary>Runs the simulaor interfaced by the current object.
        /// This method only runs the simulator, it does not prepare any input.</summary>
        public void RunSimulator()
        {
            lock (Lock)
            {
                string inputFileArg; 
                // WARNING: 
                // It is not known completely how the 
                // inputFileArg = Path.GetFileName(InputPath);
                inputFileArg = UtilSystem.GetRelativePath(Path.GetDirectoryName(ExecutablePath), InputPath);
                OneCommandArgument[0] = inputFileArg;
                string[] commandlineArguments = OneCommandArgument;
                RunSimulator(commandlineArguments);
            }
        }

        /// <summary>Runns the simulator with the specified command-line arguments.
        /// Warning: this method should be used for testing only because the command-line 
        /// arguments that should be passed to the simulator are specified by the interrface 
        /// rules and can not be specified arbitrarily. Otherwise, run the overloaded method
        /// with no arguments.
        /// This method only runs the simulator, it does not prepare any input.</summary>
        /// <param name="commandlineArguments">Command-line arguments that are passed to the simulator program.</param>
        public void RunSimulator(string[] commandlineArguments)
        {
            if (string.IsNullOrEmpty(ExecutablePath))
                throw new InvalidDataException("Executable path is not specified.");
            if (!File.Exists(ExecutablePath))
                throw new InvalidDataException("Invalid executable path, executable does not exist. "
                    + Environment.NewLine + "  Executable path: " + ExecutablePath + ".");

            
            //CommandLineApplicationInterpreter.ExecuteSystemCommand(ExecutablePath, commandlineArguments);
            string workingDirectory = Path.GetDirectoryName(InputPath);
            string currentDirectory = Directory.GetCurrentDirectory();

            //Directory.SetCurrentDirectory(workingDirectory);

            if (OutputLevel > 2)
            {
                Console.WriteLine();
                Console.WriteLine("Executing simulator... ");
                Console.WriteLine("Process directory will be set to: " + Environment.NewLine + "  " + workingDirectory);
                Console.WriteLine("Current directory: " + Environment.NewLine + "  " + Directory.GetCurrentDirectory());
                Console.WriteLine("Executable: " + ExecutablePath);
                Console.WriteLine("Commandline arguments: ");
                if (commandlineArguments == null)
                    Console.WriteLine("null");
                else
                {
                    if (commandlineArguments.Length == 0)
                        Console.WriteLine("  Empty array.");
                    for (int i = 0; i < commandlineArguments.Length; ++i)
                        Console.WriteLine(i + ": " + commandlineArguments[i]);
                }
            }

            UtilSystem.ExecuteSystemCommand(workingDirectory, false /* asynchronous */, 
                false /* useShell */, false /* createNoWindow */,
                RedirectedOutputPath /* redirectedOutputPath */, false /* redirectStandardOutput */,
                ExecutablePath, commandlineArguments);
            if (OutputLevel > 2)
            {
                Console.WriteLine();
                Console.WriteLine("Execution finished.");
            }

            // Directory.SetCurrentDirectory(currentDirectory);
        }

        /// <summary>Calculates simulator's response for the specified input parameters.</summary>
        /// <param name="inputParameters">Input parameters for which response is calculated.</param>
        /// <param name="outputValues">Vector object where the calculated response is stored after calculation.</param>
        public void CalculateVectorResponse(IVector inputParameters, ref IVector outputValues)
        {
            lock (Lock)
            {
                if (OutputLevel > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Calculating vector response by " + this.GetType().Name + "...");
                    Console.WriteLine("  Input parameters: " + inputParameters == null ? "null" : inputParameters.ToStringMath());
                }
                if (inputParameters == null)
                    throw new ArgumentNullException("Input parameters not specified (null or empty string).");
                if (NumInputParameters == 0)
                    NumInputParameters = inputParameters.Length;
                else if (NumInputParameters > 0)
                {
                    if (inputParameters.Length != NumInputParameters)
                        throw new ArgumentException("Dimension of input parameters (" + inputParameters.Length +
                            ") does not match number of input parameters for the current case (" + NumInputParameters + ").");
                }
                StopWatch t = new StopWatch();
                double tWriteParam = 0, tExecute = 0, tReadResponse = 0, tAll = 0;
                t.Start();
                DeleteOutputFiles();
                WriteInputParameters(inputParameters);
                t.Stop(); tWriteParam = t.Time; t.Start();
                RunSimulator();
                t.Stop(); tExecute = t.Time; t.Start();
                ReadOutputValues(ref outputValues);
                t.Stop(); tReadResponse = t.Time; tAll = t.TotalTime;
                if (outputValues == null)
                    throw new InvalidDataException("Output values are not specified after execution of the simulator (null reference). "
                        + Environment.NewLine + "  Output path: " + OptOutputCsvPath);
                if (NumOutputValues == 0)
                    NumOutputValues = outputValues.Length;
                else if (NumOutputValues > 0)
                    if (outputValues.Length != NumOutputValues)
                        throw new ArgumentException("Dimension of vector of output values (" + outputValues.Length +
                            ") does not match number of output vlues for the current case (" + NumOutputValues + ").");
                if (OutputLevel > 0)
                {
                    Console.WriteLine("... calculation of response finished in " + t.TotalTime + " s.");
                    Console.WriteLine("Durations: Writing input: " + tWriteParam + " s, running simulation: " + tExecute + " s, "
                        + Environment.NewLine + "  reading output: " + tReadResponse + " s, total: " + tAll + "s.");
                    Console.WriteLine("Input file: " + InputPath);
                    Console.WriteLine("Output file: " + OptOutputCsvPath);
                    Console.WriteLine("Calculated Response:");
                    Console.WriteLine("  " + outputValues);
                }
            }
        }  // CalculateResponse(IVector, ref IVector)



        #endregion OperationBasic


        /// <summary>Writes data about the current object to console.
        /// Normally, <see cref="toString"/>() will be used for that, but sometimed this
        /// method is used because it is more suitable for debugging.</summary>
        public void WriteToConsole()
        {
            Console.WriteLine("======");
            Console.WriteLine("Interface with the meshless simulation code of Gregor Kosec.");
            Console.WriteLine("  Root directory:  " + DataDirectory);
            Console.WriteLine("  Output base dir: " + OutputBaseDirpath);
            Console.WriteLine("  Output dir:      " + OutputDirpath);
            Console.WriteLine("------");
            Console.WriteLine("  Template file:   " + TemplateInputPath);
            Console.WriteLine("  Input file:      " + InputPath);
            if (InputFileContents == null)
                Console.WriteLine("  Input file contents are NULL!");
            else
                Console.WriteLine("  Length of input: " + InputFileContents.Length);
            Console.WriteLine("  Opt. output:     " + OptOutputCsvPath);
            Console.WriteLine("------");
            Console.WriteLine("  Input field definitions: ");
            if (InputFieldDefinitions.Count == 0)
                Console.WriteLine("There are no input file definitions installed.");
            else
            {
                Console.WriteLine("Input file definitions installed: ");
                foreach (InputFieldDefinition def in InputFieldDefinitions.Values)
                {
                    if (def == null)
                        Console.WriteLine("  null!");
                    else
                        Console.WriteLine("  " + def.Id + ":  " + def.Name);
                }
            }
            Console.WriteLine("------");
            Console.WriteLine();
        }

        /// <summary>Returns string representation of the current object.</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("======");
            sb.AppendLine("Interface with the meshless simulation code of Gregor Kosec.");
            sb.AppendLine("  Root directory:  " + DataDirectory);
            sb.AppendLine("  Output base dir: " + OutputBaseDirpath);
            sb.AppendLine("  Output dir:      " + OutputDirpath);
            sb.AppendLine("------");
            sb.AppendLine("  Template file:   " + TemplateInputPath);
            sb.AppendLine("  Input file:      " + InputPath);
            if (InputFileContents == null)
                sb.AppendLine("  Input file contents are NULL!");
            else
                sb.AppendLine("  Length of input: " + InputFileContents.Length);
            sb.AppendLine("  Opt. output:     " + OptOutputCsvPath);
            sb.AppendLine("------");
            sb.AppendLine("  Input field definitions: ");
            if (InputFieldDefinitions.Count == 0)
                sb.AppendLine("There are no input file definitions installed.");
            else
            {
                sb.AppendLine("Input file definitions installed: ");
                // Sort definitions by ID:
                SortedList<int, InputFieldDefinition> fields = new SortedList<int, InputFieldDefinition>();
                foreach (InputFieldDefinition def in InputFieldDefinitions.Values.ToArray())
                {
                    fields.Add(def.Id, def);
                }
                foreach (InputFieldDefinition def in fields.Values)
                {
                    if (def == null)
                        sb.AppendLine("  null!");
                    else
                        sb.AppendLine("  " + def.Id + ":  " + def.Name);
                }
            }
            sb.AppendLine("------");
            sb.AppendLine();
            return sb.ToString();
        }


        #region AuxiliaryClass

        /// <summary>Contains data about a field that is recoginzed in the input file,
        /// such as field identification number, name, and description.
        /// This class is immutable except for Description, and it is thread safe
        /// in the scope of intended use.</summary>
        public class InputFieldDefinition
        {

            /// <summary>Constructs new input field definition.</summary>
            /// <param name="id">Unique ID of the input field (this really matters in the input file).</param>
            /// <param name="name">Agreed mnemonic name of the input field (case sensitive).
            /// If null or empty string then some default string will be assigned.</param>
            public InputFieldDefinition(int id, string name)
                : this(id, name, null, null)
            { }

            /// <summary>Constructs new input field definition.</summary>
            /// <param name="id">Unique ID of the input field (this really matters in the input file).</param>
            /// <param name="name">Agreed mnemonic name of the input field (case sensitive).
            /// If null or empty string then some default string will be assigned.</param>
            /// <param name="description">Optional description of the input field.</param>
            public InputFieldDefinition(int id, string name, string description) :
                this(id, name, description, null)
            { }

            /// <summary>Constructs new input field definition.</summary>
            /// <param name="id">Unique ID of the input field (this really matters in the input file).</param>
            /// <param name="name">Agreed mnemonic name of the input field (case sensitive).
            /// If null or empty string then some default string will be assigned.</param>
            /// <param name="description">Optional description of the input field.</param>
            /// <param name="defaultValue">Default value of the field.</param>
            public InputFieldDefinition(int id, string name, string description, string defaultValue)
            {
                this.Id = id;
                this.Name = name;
                this.Description = description;
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    this.DefaultValue = defaultValue;
                }
            }

            private int _id;
            private string _name;
            public const string DefaultName = "UndefVarName";
            private string _description;
            private string _defaultvalue;

            /// <summary>Unique ID of the input field (this really matters in the input file).</summary>
            public int Id
            {
                get { return _id; }
                protected set
                {
                    if (value <= 0)
                        throw new ArgumentException("Input field ID can not be less or equal to zero. Assigned value: " + value + ".");
                    _id = value;
                }
            }

            /// <summary>Agreed mnemonic name of the input field (case sensitive).
            /// If set to null or empty string then some default string is assigned.</summary>
            public string Name
            {
                get { return _name; }
                protected set
                {
                    if (string.IsNullOrEmpty(value))
                        value = DefaultName;
                    _name = value;
                }
            }

            /// <summary>Optional description of the input field.</summary>
            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }

            /// <summary>Whether the default value is defined or not.</summary>
            public bool DefaultValueDefined
            {
                get { return (!string.IsNullOrEmpty(DefaultValue)); }
            }

            /// <summary>Default value for the curretn input field.</summary>
            public string DefaultValue
            {
                get { return _defaultvalue; }
                set { _defaultvalue = value; }
            }

        }  // class InputFieldDefinition

        #endregion AuxiliaryClass


    }  // class SimKosecFileManager

}