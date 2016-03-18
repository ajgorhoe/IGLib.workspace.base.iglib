// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// CLASSES FOR DATA TRANSFER OBJECTS (DTO) THAT FACILITATE SERIALIZATION OF VECTOR OBJECTS.

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using IG.Lib;
using System.Collections.Generic;

namespace IG.Num
{


    /// <summary>Base class for building vector functions from scripts. This is currently an interrmediate
    /// class between the DTO and a vector function, and will probably be gradually replaced.</summary>
    /// <para>Beside acting as a kind of data transfer object, this class provides a variety of manipulations that are necessary
    /// when defining vector functions from scripts (i.e., from user definitions that are compiiled the JIT compiler).</para>
    /// <typeparam name="VectorFunctionType">Type parameter specifying the specific vector function type for which concrete DTO
    /// is designed.</typeparam>
    /// $A Igor Feb16;
    public abstract class VectorFunctionScriptControllerBase<VectorFunctionType, ScalarFunctionControllerType, ScalarFunctionType>
        where VectorFunctionType : class, IVectorFunction
        where ScalarFunctionControllerType : ScalarFunctionScriptControllerBase<ScalarFunctionType>, new()
        where ScalarFunctionType: class, IScalarFunction
    {


        #region Construction

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public VectorFunctionScriptControllerBase()
            : base()
        { }

        /// <summary>Constructor, prepares the current DTO for storing a scalar function of the specified dimension.</summary>
        /// <param name="numParameters">Number of parameters of the represented vector function (dimension of codomain).</param>
        /// <param name="numValues">Number of returned values of the represented vector function (dimension of function domain).</param>
        public VectorFunctionScriptControllerBase(int numParameters, int numValues)
            : this()
        {
            this.NumParameters = numParameters;
            this.NumValues = numValues;
        }

        #endregion Construction

        
        #region Data


        protected string _name;

        /// <summary>Scalar function name.</summary>
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _description;

        /// <summary>Scalar function description.</summary>
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }



        private int _numParameters = 1;

        /// <summary>Number of parameters of the vector function, i.e., 
        /// dimension of thefunction domain.</summary>
        public int NumParameters
        {
            get { return _numParameters; }
            set {
                if (value != _numParameters)
                {
                    _numParameters = value;
                    InvalidateNumParameters();
                    // throw new NotImplementedException("NumParameters not yet implemented.");
                }
            }
        }

        private int _numValues = 1;

        /// <summary>Number of elements (scalar function components) of the vector function, i.e., 
        /// dimension of thefunction codomain.</summary>
        public int NumValues
        {
            get { return _numValues; }
            set {
                _numValues = value;
                InvalidateNumValues();
                // throw new NotImplementedException("NumValues not yet implemented.");
            }
        }


        /// <summary>Creates and returns a new scalar function controller.</summary>
        public abstract ScalarFunctionControllerType CreateScalarrFunctionController();

        /// <summary>Adds a new scalar function controller to the list of controllers.</summary>
        /// <param name="functionController"></param>
        protected void AddScalarFunctionController(ScalarFunctionControllerType functionController)
        {
            Elements.Add(functionController);
        }

        //protected void ResizeElements(bool allocateIfNull)
        //{
        //    // TODO: implement!
        //    throw new NotImplementedException();
        //}


        private List<ScalarFunctionControllerType> _elements = new List<ScalarFunctionControllerType>();


        /// <summary>List of vector function elements, i.e. scalar functions that calculate individual elements of the return value.</summary>
        protected List<ScalarFunctionControllerType> Elements
        {
            get { return _elements; }
            //private set {
            //    _elements = value;
            //}
        }


        /// <summary>Returns an array of scalar function controllers (of type <see cref="ScalarFunctionControllerType"/>) that represent
        /// elements of the vector function.</summary>
        /// <returns></returns>
        public ScalarFunctionControllerType[] GetElements()
        {
            return Elements.ToArray();
        }

        /// <summary>Gets or sets scalar function controller identified by the specified index.</summary>
        /// <param name="which">Function index.</param>
        public ScalarFunctionControllerType this[int which]
        {
            get {
                if (which < 0 || which >= NumValues)
                    throw new IndexOutOfRangeException("Function index out of range, should be between 0 and " + (NumValues - 1).ToString() + ".");
                if (Elements.Count <= which)
                {
                    return null;
                }
                
                    return Elements[which]; }
            set {
                if (which < 0 || which >= NumValues)
                    throw new IndexOutOfRangeException("Function index out of range, should be between 0 and " + (NumValues - 1).ToString() + ".");
                while (Elements.Count <= which)
                    Elements.Add(null);
                if (!object.ReferenceEquals(value, Elements[which]))
                {
                    while (Elements.Count <= which)
                        AddScalarFunctionController(null);
                    Elements[which] = value;
                    if (value.Dimension != NumParameters && value.Dimension > 0)
                    {
                        NumParameters = value.Dimension;
                    }
                }
            }
        }

        /// <summary>Clears the list of scalar functions.</summary>
        protected void Clear()
        {
            Elements.Clear();
        }


        /// <summary>Synchronizes data on the current vector function controller in such a way that it is consistent.</summary>
        /// <param name="syncParameterNames">If true then parameterr names are also synchroniized.</param>
        /// <param name="syncFunctionNames">If true then function names are also synchronized.</param>
        public void SynchronizeData(bool syncParameterNames = false, bool syncFunctionNames = true)
        {
            if (Elements.Count >= NumValues)
                Elements.RemoveRange(NumValues, Elements.Count - NumValues);
            // Update parameter names and function names, if necessary:
            bool updateParameterNames = false;
            if (ParameterNames == null)
                updateParameterNames = true;
            else if (ParameterNames.Length != NumParameters)
                updateParameterNames = true;
            if (updateParameterNames)
            {
                string[] previousNames = ParameterNames;
                string[] names = ScalarFunctionScriptController.GenerateParameterOrVariableNames(NumParameters, "x");
                if (previousNames != null)
                {
                    for (int i = 0; i < NumParameters; ++i)
                        if (!string.IsNullOrEmpty(previousNames[i]))
                            names[i] = previousNames[i];
                }
                this.ParameterNames = names;
            }
            bool updateFunctionNames = false;
            if (FunctionNames == null)
                updateFunctionNames = true;
            else if (FunctionNames.Length != NumValues)
                updateFunctionNames = true;
            if (updateFunctionNames)
            {
                string[] previousNames = FunctionNames;
                string[] names = ScalarFunctionScriptController.GenerateParameterOrVariableNames(NumParameters, "f");
                if (previousNames != null)
                {
                    for (int i = 0; i < NumValues; ++i)
                        if (!string.IsNullOrEmpty(previousNames[i]))
                            names[i] = previousNames[i];
                }
                this.FunctionNames = names;
            }
            // Synchronize state of scalar function controllers:
            while (Elements.Count < NumValues)
            {
                AddScalarFunctionController(null);
            }
            for (int whichScalarFuction = 0; whichScalarFuction < NumValues; ++whichScalarFuction)
            {
                ScalarFunctionControllerType scalarController = this[whichScalarFuction];
                if (scalarController == null)
                {
                    this[whichScalarFuction] = scalarController = new ScalarFunctionControllerType();
                    scalarController.Dimension = this.NumParameters;
                    scalarController.ParameterNames = this.ParameterNames;
                    if (this.FunctionNames != null)
                    {
                        if (this.FunctionNames.Length > whichScalarFuction)
                            scalarController.Name = this.FunctionNames[whichScalarFuction];
                    }
                } else
                {
                    scalarController.Dimension = this.NumParameters;
                    if (syncParameterNames)
                        scalarController.ParameterNames = this.ParameterNames;
                    if (syncFunctionNames)
                    {
                        if (this.FunctionNames.Length > whichScalarFuction)
                        {
                            scalarController.Name = this.FunctionNames[whichScalarFuction];
                        }
                    }
                }
            }
        }




        #region Data.NamesAuxiliary

        private string _parameterNamesSeparator = ScalarFunctionScriptController.DefaultParameterNamesSeparator;

        /// <summary>Separator string that is used to separate parameter names or function names when stated in a single string.
        /// <para>Usually used for function parameters, but also for other kinds of parameters.</para></summary>
        public string ParameterNamesSeparator
        {
            get { return _parameterNamesSeparator; }
            protected set
            {
                if (value != _parameterNamesSeparator)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException("Parameter names separator not specified correctly (null or empty string).");
                    }
                    _parameterNamesSeparator = value;
                }
            }
        }


        /// <summary>Converts an array of parameter names or function names to a delimited single string that contains these names, and
        /// returns this string.</summary>
        /// <param name="parameterNames">array of parameter names.</param>
        public virtual string ConvertParameterNamesToString(params string[] parameterNames)
        {
            return ScalarFunctionScriptController.CreateParameterNamesString(parameterNames, ParameterNamesSeparator, false /* insertNewlines */, true /* insertSpaces */);
        }

        /// <summary>Converts a single string containing parameter names or function names in delimited form to an array of individual parameter 
        /// names or function names, respectively, and returns this array.</summary>
        /// <param name="parameterNamesString"></param>
        public virtual string[] ConvertParameterNamesToArray(string parameterNamesString)
        {
            string[] ret = null;
            ScalarFunctionScriptController.CreateParameterNamesArray(parameterNamesString, ParameterNamesSeparator, ref ret, true /* trimSpaces */);
            return ret;
        }

        #endregion Data.NamesAuxiliary


        private bool _isNamesSynchronized = true;

        public bool IsNamesSynchronized
        {
            get { return _isNamesSynchronized; }
            set { _isNamesSynchronized = value; }
        }

        // Parameter names:

        /// <summary>Returns function parameter names in form of delimited string that contains all 
        /// function parameter names.</summary>
        public virtual string GetParametersString()
        {
            if (string.IsNullOrEmpty(_parametersString) && ParameterNames != null && ParameterNames.Length > 0)
            {
                _parametersString = ConvertParameterNamesToString(_parameterNames);
            }
            return _parametersString;
        }

        /// <summary>Sets function parameter names through a string containing delimided parameters names.</summary>
        /// <param name="parametersString">String containing parameterr names.</param>
        public virtual void SetParametersString(string parametersString)
        {
            if (parametersString != this._parametersString)
            {
                InvalidateParameterNames();
                this.ParameterNames = null;  // to cause invalidation
                this._parametersString = parametersString;
            }
        }


        protected string _parametersString;

        private string[] _parameterNames;

        /// <summary>Names of function parameters.</summary>
        public virtual string[] ParameterNames
        {
            get
            {
                bool undefined = true;
                if (_parameterNames != null)
                    if (_parameterNames.Length > 0)
                        undefined = false;
                if (undefined && !string.IsNullOrEmpty(_parametersString))
                {
                    // Lazy evaluation: if parameter names were set through a single delimited string then evaluation is 
                    // deferred until actually needed.
                    _parameterNames = ConvertParameterNamesToArray(_parametersString);
                }
                return _parameterNames;
            }
            set
            {
                string[] currentValue = ParameterNames;  // to trigger eventual lazy evaluation
                if (!UtilStr.StringArraysEqual(value, currentValue))
                {
                    // Parameter name(s) changed, update dependencies:
                    InvalidateParameterNames();
                    _parametersString = null;
                    _parameterNames = value;
                    if (value != null)
                        NumParameters = value.Length;
                }
            }
        }



        // Function names:

        /// <summary>Returns function function names in form of delimited string that contains all 
        /// function function names.</summary>
        public virtual string GetFunctionsString()
        {
            if (string.IsNullOrEmpty(_functionsString) && FunctionNames != null && FunctionNames.Length > 0)
            {
                _functionsString = ConvertParameterNamesToString(_functionNames);
            }
            return _functionsString;
        }

        /// <summary>Sets function names through a string containing delimided functions names.</summary>
        /// <param name="functionsString">String containing function names.</param>
        public virtual void SetFunctionsString(string functionsString)
        {
            if (functionsString != this._functionsString)
            {
                InvalidateFunctionNames();
                this.FunctionNames = null;  // to cause invalidation
                this._functionsString = functionsString;
            }
        }


        protected string _functionsString;

        private string[] _functionNames;

        /// <summary>Names of function functions.</summary>
        public virtual string[] FunctionNames
        {
            get
            {
                bool undefined = true;
                if (_functionNames != null)
                    if (_functionNames.Length > 0)
                        undefined = false;
                if (undefined && !string.IsNullOrEmpty(_functionsString))
                {
                    // Lazy evaluation: if function names were set through a single delimited string then evaluation is 
                    // deferred until actually needed.
                    _functionNames = ConvertParameterNamesToArray(_functionsString);
                }
                return _functionNames;
            }
            set
            {
                string[] currentValue = FunctionNames;  // to trigger eventual lazy evaluation
                if (!UtilStr.StringArraysEqual(value, currentValue))
                {
                    // Function name(s) changed, update dependencies:
                    InvalidateFunctionNames();
                    _functionsString = null;
                    _functionNames = value;
                    if (value != null)
                        NumValues = value.Length;
                }
            }
        }




        /// <summary>Invalidates dimension of function domain. All dependent data is invalidated.</summary>
        public virtual void InvalidateNumParameters()
        {
            InvalidateParameterNames();
            //if (FunctionLoader != null)
            //    FunctionLoader.InvalidateDefinitions();
        }

        /// <summary>Invalidates names of parameters. This also means that function value definition and function gradient 
        /// definitions are invalidated, if present.</summary>
        public virtual void InvalidateParameterNames()
        {

            // InvalidateValueDefinition();
            // InvalidateGradientDefinition();
            //IsLoaderConsistent = false;
            //if (FunctionLoader != null)
            //    FunctionLoader.InvalidateDefinitions();
        }



        /// <summary>Invalidates dimension of function domain. All dependent data is invalidated.</summary>
        public virtual void InvalidateNumValues()
        {
            InvalidateFunctionNames();
            //if (FunctionLoader != null)
            //    FunctionLoader.InvalidateDefinitions();
        }

        /// <summary>Invalidates names of parameters. This also means that function value definition and function gradient 
        /// definitions are invalidated, if present.</summary>
        public virtual void InvalidateFunctionNames()
        {
            // InvalidateValueDefinition();
            // InvalidateGradientDefinition();
            //IsLoaderConsistent = false;
            //if (FunctionLoader != null)
            //    FunctionLoader.InvalidateDefinitions();
        }

        

        #endregion Data



        #region Utilities


        protected static double _defaultLowerBound = 0.0;

        protected static double _defaultUpperBound = 1.0;


        /// <summary>Creates and returns a data definition object for input parametera and output values of the current function DTO.
        /// <para>Bounds and default values are eventually added to definitions, dependent on parameters.</para>
        /// <para>Definition contains function name and parameter names, and composes meaningful descriptions.</para></summary>
        /// <param name="setBoundsAndDefaults">Whether output bounds are added to input and output element definitions. Default is false.
        /// <para>If true and bouds are not specified then default bounds (currently 0/1) are specified.</para>
        /// <para>If bounds are specified then default values for function parameters are also set to the mean of the corresponding 
        /// minimal and maximal value.</para></param>
        /// <param name="inputBounds">Input bounds that are added to definitions in the case that <paramref name="setBoundsAndDefaults"/>
        /// is true.
        /// <para>If it is null then bounds are replaced by default values.</para>
        /// <para>If it is specified but some bounds are not defined then corresponding bounds are also not specified.</para></param>
        /// <param name="outputBounds">Output bounds that are added to definitions in the case that <paramref name="setBoundsAndDefaults"/>
        /// is true.
        /// <para>If it is null then bounds are replaced by default values.</para>
        /// <para>If it is specified but some bounds are not defined then corresponding bounds are also not specified.</para></param>
        public virtual InputOutputDataDefiniton GetDataDefinitionObject(bool setBoundsAndDefaults = false, IBoundingBox inputBounds = null, IBoundingBox outputBounds = null)
        {
            InputOutputDataDefiniton ret = new InputOutputDataDefiniton();
            // string digitFormatString = new string('0', numNameDigits);
            for (int i = 0; i < NumParameters; ++i)
            {
                InputElementDefinition inputElement = new InputElementDefinition(i, null, null);
                inputElement.Name = inputElement.Title = ParameterNames[i];
                inputElement.Description = "Function parameter " + i + "(" + ParameterNames[i] + ").";
                if (setBoundsAndDefaults)
                {
                    inputElement.MinimalValue = _defaultLowerBound;
                    inputElement.MaximalValue = _defaultUpperBound;
                    inputElement.DefaultValue = _defaultLowerBound;
                    inputElement.BoundsDefined = true;
                    inputElement.DefaultValueDefined = true;
                    if (inputBounds != null)
                    {
                        if (inputBounds.IsMinDefined(i))
                            inputElement.MinimalValue = inputBounds.GetMin(i);
                        else
                            inputElement.MinimalValue = double.MinValue;
                        if (inputBounds.IsMaxDefined(i))
                            inputElement.MaximalValue = inputBounds.GetMax(i);
                        else
                            inputElement.MaximalValue = double.MaxValue;
                        if (!(inputBounds.IsMinDefined(i) || inputBounds.IsMinDefined(i)))
                            inputElement.BoundsDefined = false;
                        if (inputElement.MinimalValue > inputElement.MaximalValue)
                        {
                            // swap lower and upper bound if necessary:
                            double min = inputElement.MinimalValue;
                            inputElement.MinimalValue = inputElement.MaximalValue;
                            inputElement.MaximalValue = min;
                        }
                        if (inputBounds.IsMinDefined(i) && inputBounds.IsMinDefined(i))
                            inputElement.DefaultValue = 0.5 * (inputElement.MinimalValue + inputElement.MaximalValue);
                    }
                }
                ret.AddInputElement(inputElement, true /* assign element index */);
            }
            for (int i = 0; i < NumValues; ++i)
            {
                OutputElementDefinition outputElement = new OutputElementDefinition(i, null, null);
                outputElement.Name = outputElement.Title = FunctionNames[i];
                outputElement.Description = "Function component " + i + "(" + FunctionNames[i] + ").";
                if (setBoundsAndDefaults)
                {
                    outputElement.MinimalValue = _defaultLowerBound;
                    outputElement.MaximalValue = _defaultUpperBound;
                    outputElement.BoundsDefined = true;
                    if (outputBounds != null)
                    {

                        if (outputBounds.IsMinDefined(i))
                            outputElement.MinimalValue = outputBounds.GetMin(i);
                        else
                            outputElement.MinimalValue = double.MinValue;
                        if (outputBounds.IsMaxDefined(i))
                            outputElement.MaximalValue = outputBounds.GetMax(i);
                        else
                            outputElement.MaximalValue = double.MaxValue;
                        if (!(outputBounds.IsMinDefined(i) || outputBounds.IsMinDefined(i)))
                            outputElement.BoundsDefined = false;
                        if (outputElement.MinimalValue > outputElement.MaximalValue)
                        {
                            // swap bounds if necessary: 
                            double min = outputElement.MinimalValue;
                            outputElement.MinimalValue = outputElement.MaximalValue;
                            outputElement.MaximalValue = min;
                        }

                    }
                }
                ret.AddOutputElement(outputElement, true /* assign element index */);
            }
            return ret;
        }


        #endregion Utilities


        /// <summary>Returns human readable string representation of the current object.</summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(this);
        }


        #region StaticInputOutput


        /// <summary>Returns a string representation of the specified <see cref="VectorFunctionScripController"/> object.</summary>
        /// <param name="sc">Vector whose string representation is returned.</param>
        public static string ToString(VectorFunctionScriptControllerBase<VectorFunctionType, ScalarFunctionControllerType, ScalarFunctionType> sc)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Function name: " + sc.Name);
            sb.AppendLine("Function description: \"" + sc.Description + "\"");
            sb.AppendLine("Number of parameters: " + sc.NumParameters);
            sb.AppendLine("Parameters: " + sc.GetParametersString());
            sb.AppendLine("Number of function elements: " + sc.NumValues);
            sb.AppendLine("Function names: " + sc.GetFunctionsString());
            sb.AppendLine("Function definitions:");
            if (sc.Elements == null)
                sb.AppendLine("  Function elements are not specified (null list reference).");
            else if (sc.Elements.Count < 0)
                sb.AppendLine("  No functions are defined (list of function elemennts is empty).");
            else
            {
                for (int whichFunction = 0; whichFunction < sc.Elements.Count; ++whichFunction)
                {
                    ScalarFunctionControllerType scalarController = sc[whichFunction];
                    sb.AppendLine();
                    if (scalarController == null)
                        sb.AppendLine("    Function No. " + whichFunction + " is not specified (null reference).");
                    else
                        sb.AppendLine("  Scalar function element No. " + whichFunction + ": " + Environment.NewLine
                            + whichFunction.ToString());
                }
            }
            return sb.ToString();
        }
        

        /// <summary>Saves (serializes) the specified script-based vector function controller to the specified JSON file.
        /// If the file already exists, contents either overwrites the file or is appended at the end, 
        /// dependent on the value of the append flag.
        /// <para>File is overwritten if it already exists.</para>
        /// <para>The data definition is also written to the file apart to function data, and can be later restored to a 
        /// separate object if necessary.</para></summary>
        /// <param name="functionController">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file in which object is saved.</param>
        /// <param name="dataDefinition">Optional object containing the definition of functions's input/output data. If specified,
        /// this object is added on the DTO annd is saved to the file together with pure function definitiion.</param>
        public static void SaveJson(VectorFunctionScriptController functionController, string filePath,
            InputOutputDataDefiniton dataDefinition)
        {
            SaveJson(functionController, filePath, false /* append */, dataDefinition);
        }



        /// <summary>Saves (serializes) the specified script-based vector function controller to the specified JSON file.
        /// If the file already exists, contents either overwrites the file or is appended at the end, 
        /// dependent on the value of the append flag.
        /// <para>In addition to function data, the data definition is also stored to the output file when specified.</para></summary>
        /// <param name="functionController">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file in which object is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        /// <param name="dataDefinition">Optional object containing the definition of functions's input/output data. If specified,
        /// this object is added on the DTO annd is saved to the file together with pure function definitiion.</param>
        public static void SaveJson(VectorFunctionScriptController functionController, string filePath, bool append = false,
            InputOutputDataDefiniton dataDefinition = null)
        {
            VectorFunctionScriptDto dtoOriginal = new VectorFunctionScriptDto();
            dtoOriginal.CopyFrom(functionController);
            if (dataDefinition != null)
            {
                InputOutputDataDefinitonDto dataDefDto = new InputOutputDataDefinitonDto();
                dataDefDto.CopyFrom(dataDefinition);
                dtoOriginal.ZDataDefinition = dataDefDto;
            }
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<VectorFunctionScriptDto>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) a script-based scalar function controller from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which object is restored.</param>
        /// <param name="controllerRestored">Object that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref VectorFunctionScriptController controllerRestored)
        {
            VectorFunctionScriptDto dto = null;
            LoadJson(filePath, ref controllerRestored, ref dto);
        }

        /// <summary>Restores (deserializes) a script-based scalar function controller from the specified file in JSON format.
        /// <para>An intermediate object in the process, the DTO, is also stored, because this kind of DTO also contains
        /// some additional data that is not copied to <paramref name="controllerRestored"/>, such as function's input/output
        /// data definition.</para></summary>
        /// <param name="filePath">File from which object is restored.</param>
        /// <param name="controllerRestored">Object that is restored by deserialization.</param>
        /// <param name="dto">Variable into which the intermediate data transfer object (DTO) is stored in the process.
        /// The DTO can in this case include some additional data that is not copied to <paramref name="controllerRestored"/> but
        /// may be used in the context where the method is called - for example the function's input/output data definitions.</param>
        public static void LoadJson(string filePath, ref VectorFunctionScriptController controllerRestored, ref VectorFunctionScriptDto dto)
        {
            ISerializer serializer = new SerializerJson();
            VectorFunctionScriptDto dtoRestored = serializer.DeserializeFile<VectorFunctionScriptDto>(filePath);
            dto = dtoRestored;
            dtoRestored.CopyTo(ref controllerRestored);
        }

        /// <summary>Restores (deserializes) a script-based scalar function controller from the specified file in JSON format.
        /// <para>An intermediate object in the process, the DTO, is also stored, because this kind of DTO also contains
        /// some additional data that is not copied to <paramref name="controllerRestored"/>, such as function's input/output
        /// data definition.</para></summary>
        /// <param name="filePath">File from which object is restored.</param>
        /// <param name="controllerRestored">Object that is restored by deserialization.</param>
        /// <param name="dataDef">Variable into which the input/output data definition is restored when it is also written
        /// in the file (which is possible due to the ability of <see cref="VectorFunctionScriptDto"/> to incorporate a data definition
        /// object, in addition to pure function data).</param>
        public static void LoadJson(string filePath, ref VectorFunctionScriptController controllerRestored, ref InputOutputDataDefiniton dataDef)
        {
            VectorFunctionScriptDto dto = null;
            InputOutputDataDefinitonDto dataDto = null;
            LoadJson(filePath, ref controllerRestored, ref dto);
            if (dto != null)
                dataDto = dto.ZDataDefinition;
            if (dataDto != null)
                dataDto.CopyTo(ref dataDef);
            else
                dataDef = null;
        }


        ///// <summary>Saves the specified vector to a CSV file.
        ///// It the specified file already exists then it is overwritten.
        ///// Constant <see cref="UtilCsv.DefaultCsvSeparator"/> is used as separator.</summary>
        ///// <param name="vec">Vector to be stored to a file.</param>
        ///// <param name="filePath">Path of the file to which vector is stored.</param>
        //public static void SaveCsv(IVector vec, string filePath)
        //{
        //    SaveCsv(vec, filePath, UtilStr.DefaultCsvSeparator /* separator */, false /* append */);
        //}

        ///// <summary>Saves the specified vector to a CSV file.
        ///// It the specified file already exists then it is overwritten.</summary>
        ///// <param name="vec">Vector to be stored to a file.</param>
        ///// <param name="filePath">Path of the file to which vector is stored.</param>
        ///// <param name="separator">Separator used in the CSV file.</param>
        //public static void SaveCsv(IVector vec, string filePath, string separator)
        //{
        //    SaveCsv(vec, filePath, separator, false /* append */);
        //}

        ///// <summary>Saves the specified vector to a CSV file.
        ///// Constant <see cref="UtilCsv.DefaultCsvSeparator"/> is used as separator in CSV.</summary>
        ///// <param name="vec">Vector to be stored to a file.</param>
        ///// <param name="filePath">Path of the file to which vector is stored.</param>
        ///// <param name="append">Specifies whether the data is appended at the end of the file
        ///// in the case that the ifle already exists.</param>
        //public static void SaveCsv(IVector vec, string filePath, bool append)
        //{
        //    SaveCsv(vec, filePath, UtilStr.DefaultCsvSeparator, append);
        //}

        ///// <summary>Saves the specified vector to a CSV file.</summary>
        ///// <param name="vec">Vector to be stored to a file.</param>
        ///// <param name="filePath">Path of the file to which vector is stored.</param>
        ///// <param name="separator">Separator used in the CSV file.</param>
        ///// <param name="append">Specifies whether the data is appended at the end of the file
        ///// in the case that the ifle already exists.</param>
        //public static void SaveCsv(IVector vec, string filePath, string separator, bool append)
        //{
        //    throw new NotImplementedException("Saving user defined scalar function state to CSV is not yet supported.");

        //    //int dimension = 0;
        //    //if (vec != null)
        //    //    dimension = vec.Length;
        //    //if (dimension < 0)
        //    //    dimension = 0;
        //    //string[][] values = new string[1][];
        //    //values[0] = new string[dimension];
        //    //for (int i = 0; i < dimension; ++i)
        //    //    values[0][i] = vec[i].ToString();
        //    //UtilStr.SaveCsv(filePath, values, separator, append);
        //}


        ///// <summary>Reads a vector from a CSV file.
        ///// Constant <see cref="UtilCsv.DefaultCsvSeparator"/> is used as separator in CSV file.
        ///// If there are no components then a null vector is returned by this method (no exceptions thrown).
        ///// If there are more than one rows in the CSV file then vector is read from the first row.</summary>
        ///// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        ///// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        //public static void LoadCsv(string filePath, ref IVector vecRestored)
        //{
        //    LoadCsv(filePath, UtilStr.DefaultCsvSeparator, ref vecRestored);
        //}

        ///// <summary>Reads a vector written in CSV format from a file.
        ///// If there are no components then a null vector is returned by this method (no exceptions thrown).
        ///// If there are more than one rows in the CSV file then vector is read from the first row.</summary>
        ///// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        ///// <param name="separator">Separator that is used to separate values in a row in the CSV file.</param>
        ///// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        //public static void LoadCsv(string filePath, string separator, ref IVector vecRestored)
        //{
        //    throw new NotImplementedException("Reading user defined scalar function state from CSV is not yet supported.");

        //    //string[][] values = null;
        //    //values = UtilStr.LoadCsv(filePath, separator);
        //    //if (values == null)
        //    //    vecRestored = null;
        //    //else if (values.Length == 0)
        //    //    vecRestored = null;
        //    //else
        //    //{
        //    //    int dimension = values[0].Length;
        //    //    Vector.Resize(ref vecRestored, dimension);
        //    //    for (int i = 0; i < dimension; ++i)
        //    //    {
        //    //        double comp = 0;
        //    //        bool readCorrectly = double.TryParse(values[0][i], out comp);
        //    //        if (readCorrectly)
        //    //            vecRestored[i] = comp;
        //    //        else
        //    //        {
        //    //            throw new FormatException("Vector coponenet No. " + i + " in a CSV file is not a number. "
        //    //                + Environment.NewLine + "  Component representation in the file: " + values[0][i]);
        //    //        }
        //    //    }
        //    //}
        //}


        ///// <summary>Reads a vector from the specified row of a CSV file.
        ///// Constant <see cref="UtilCsv.DefaultCsvSeparator"/> is used as CSV separator.
        ///// If the specified row does not exisist in the file then exception is thrown.</summary>
        ///// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        ///// <param name="rowNum">Number of the row from which the vector is read.</param>
        ///// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        //public static void LoadCsv(string filePath, int rowNum, ref IVector vecRestored)
        //{
        //    LoadCsv(filePath, rowNum, UtilStr.DefaultCsvSeparator /* separator */, ref vecRestored);
        //}

        ///// <summary>Reads a vector from the specified row of a CSV file.
        ///// If the specified row does not exisist in the file then exception is thrown.</summary>
        ///// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        ///// <param name="rowNum">Number of the row from which the vector is read.</param>
        ///// <param name="separator">Separator that is used to separate values in a row in the CSV file.</param>
        ///// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        //public static void LoadCsv(string filePath, int rowNum, string separator, ref IVector vecRestored)
        //{
        //    throw new NotImplementedException("Reading user defined scalar function state from CSV is not yet supported.");

        //    //string[][] values = null;
        //    //values = UtilStr.LoadCsv(filePath, separator);
        //    //if (values == null)
        //    //    throw new FormatException("CSV file has no rows, can not read vector from row " + rowNum + ".");
        //    //else if (values.Length < rowNum + 1)
        //    //    throw new FormatException("CSV file has only " + values.Length + " rows, can not read vector from row " + rowNum + ".");
        //    //else
        //    //{
        //    //    int dimension = values[rowNum].Length;
        //    //    Vector.Resize(ref vecRestored, dimension);
        //    //    for (int i = 0; i < dimension; ++i)
        //    //    {
        //    //        int comp = 0;
        //    //        bool readCorrectly = int.TryParse(values[rowNum][i], out comp);
        //    //        if (readCorrectly)
        //    //            vecRestored[i] = comp;
        //    //        else
        //    //        {
        //    //            throw new FormatException("Vector coponenet No. " + i + ", element (" + rowNum +
        //    //                "," + i + ") of a CSV file, is not a number. "
        //    //                + Environment.NewLine + "  Component representation in the file: " + values[rowNum][i]);
        //    //        }
        //    //    }
        //    //}
        //}


        // ScalarFunction ScalarFunction ScalarFunction

        #endregion StaticInputOutput


        #region FunctionLoader


        /// <summary>Returns a vector function that is created from the funciton definition on the
        /// current vector function script controller.</summary>
        public IVectorFunction GetFunction()
        { return this.Function; }


        /// <summary>Scalar function (of type <see cref="IVectorFunction"/>) created on basis of definition in this controller.</summary>
        protected IVectorFunction Function
        {
            get
            {
                List<IScalarFunction> elementFunctions = new List<IScalarFunction>();
                for (int whichFunction = 0; whichFunction < NumValues; ++whichFunction)
                {
                    IScalarFunction scalarFunction = null;
                    ScalarFunctionControllerType scalarFuncController = this[whichFunction];
                    if (scalarFuncController == null)
                        throw new InvalidOperationException("Could not create a vector function from script controller: " + Environment.NewLine
                            + "  scalar function script controller No. " + whichFunction +  " is not specified (null reference).");
                    scalarFunction = scalarFuncController.GetFunction();
                    if (scalarFunction == null)
                        throw new InvalidOperationException("Could not create a vector function from script controller: " + Environment.NewLine
                            + "  scalar function script controller No. " + whichFunction +  " failed to generate a function.");
                    elementFunctions.Add(scalarFunction);
                }
                return new VectorFunctionFromScalar(elementFunctions.ToArray());
            }
        }

        #endregion FunctionLoader


    }  // class VectorFunctionScriptController<VectorFunctionType>



    /// <summary>Class for building vector functions from scripts, used by GUI elements.</summary>
    /// $A Igor Feb16;
    public class VectorFunctionScriptController: VectorFunctionScriptControllerBase<IVectorFunction, ScalarFunctionScriptController, IScalarFunction>
    {

        #region Construction

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public VectorFunctionScriptController()
            : base()
        { }

        /// <summary>Constructor, prepares the current object for storing a vector function definition of the specified dimensions.</summary>
        /// <param name="numParameters">Number of parameters of the represented vector function (dimension of codomain).</param>
        /// <param name="numValues">Number of returned values of the represented vector function (dimension of function domain).</param>
        public VectorFunctionScriptController(int numParameters, int numValues)
            : base(numParameters, numValues)
        {
        }

        #endregion Construction


        /// <summary>Creates and returns a new scalar function controller.</summary>
        public override ScalarFunctionScriptController CreateScalarrFunctionController()
        {
            return new ScalarFunctionScriptController();
        }


    } // class ScalarFunctionScriptDto





}
