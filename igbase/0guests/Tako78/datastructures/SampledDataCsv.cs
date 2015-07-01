// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Base class for CSV (comma separated files) representation, importer and exporter of sampled data and 
    /// data definitions.</summary>
    /// $A Igo Oct08 Jul13;
    public class SampledDataCsv: StringTable
    {

        #region Construction

        /// <summary>Construct a new CSV representation of sampled data and data definitions.</summary>
        public SampledDataCsv()
        {
            IsReadOnly = false;
            IsAutoExtend = true;
        }

        /// <summary>Construct a new CSV representation of sampled data and data definitions,
        /// with numbers of input parameters and output values specified.</summary>
        /// <param name="inputLength">Number of input parameters.</param>
        /// <param name="outputLength">Number of output values.</param>
        public SampledDataCsv(int inputLength, int outputLength)
            : this()
        {
            this.InputLength = inputLength;
            this.OutputLength = outputLength;
        }


        #endregion Construction 


        #region Keys.Default

        private static bool _defaultIsKeysCaseSensitive = false;

        /// <summary>Default value for the flag indicating whether keys should be treated as case sensitive.</summary>
        public static bool DefaultIsKeysCaseSensitive
        {
            get { return _defaultIsKeysCaseSensitive; }
            set { _defaultIsKeysCaseSensitive = value; }
        }

        private static string _defaultKeyData = "Data";

        /// <summary>Default key for introduction of sampled data.</summary>
        public static string DefaultKeyData
        {
            get { return _defaultKeyData; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for data must be a legal variable name.");
                _defaultKeyData = value;
            }
        }


        private static string _defaultKeyElementTypeInput = "Input";

        /// <summary>Default element type keyword for input element.</summary>
        public static string DefaultKeyElementTypeInput
        {
            get { return _defaultKeyElementTypeInput; }
            protected set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("A default keyword for input element type must not be null or empty string.");
                _defaultKeyElementTypeInput = value;
            }
        }

        private static string _defaultKeyElementTypeOutput = "Output";

        /// <summary>Default element type keyword for output element.</summary>
        public static string DefaultKeyElementTypeOutput
        {
            get { return _defaultKeyElementTypeOutput; }
            protected set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("A default keyword for output element type must not be null or empty string.");
                _defaultKeyElementTypeOutput = value;
            }
        }

        private static string _defaultKeyComment = "Comment";

        /// <summary>Default key for comment in the current line.</summary>
        public static string DefaultKeyComment
        {
            get { return _defaultKeyComment; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for comment must be a legal variable name.");
                _defaultKeyComment = value;
            }
        }

        private static string _defaultKeyNumInputParameters = "NumInputs";

        /// <summary>Default key for introduction of number of input parameters.</summary>
        public static string DefaultKeyNumInputParameters
        {
            get { return _defaultKeyNumInputParameters; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for number of inputs must be a legal variable name.");
                _defaultKeyNumInputParameters = value;
            }
        }

        private static string _defaultKeyNumOutputParameters = "NumOutputs";

        /// <summary>Default key for introduction of number of output parameters.</summary>
        public static string DefaultKeyNumOutputValues
        {
            get { return _defaultKeyNumOutputParameters; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for number of outputs must be a legal variable name.");
                _defaultKeyNumOutputParameters = value;
            }
        }

        private static string _defaultKeyNames = "Names";

        /// <summary>Default key for introduction of names of input and output data elements.</summary>
        public static string DefaultKeyNames
        {
            get { return _defaultKeyNames; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for names must be a legal variable name.");
                _defaultKeyNames = value;
            }
        }


        private static string _defaultKeyTitles = "Titles";

        /// <summary>Default key for introduction of titles of input and output data elements.</summary>
        public static string DefaultKeyTitles
        {
            get { return _defaultKeyTitles; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for titles must be a legal variable name.");
                _defaultKeyTitles = value;
            }
        }


        private static string _defaultKeyDescriptions = "Descriptions";

        /// <summary>Default key for introduction of descriptions of input and output data elements.</summary>
        public static string DefaultKeyDescriptions
        {
            get { return _defaultKeyDescriptions; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for descriptions must be a legal variable name.");
                _defaultKeyDescriptions = value;
            }
        }


        private static string _defaultKeyElementTypes = "ElementTypes";

        /// <summary>Default key for introduction of element types of input and output data elements
        /// (e.g. input/output).</summary>
        public static string DefaultKeyElementTypes
        {
            get { return _defaultKeyElementTypes; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for element types must be a legal variable name.");
                _defaultKeyElementTypes = value;
            }
        }


        private static string _defaultKeyElementIndices = "ElementIndices";

        /// <summary>Default key for introduction of element indices of input and output data elements.</summary>
        public static string DefaultKeyElementIndices
        {
            get { return _defaultKeyElementIndices; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for element indices must be a legal variable name.");
                _defaultKeyElementIndices = value;
            }
        }


        private static string _defaultKeyMinimalValues = "MinimalValues";

        /// <summary>Default key for introduction of minimal values of input and output data elements.</summary>
        public static string DefaultKeyMinimalValues
        {
            get { return _defaultKeyMinimalValues; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for minimal values must be a legal variable name.");
                _defaultKeyMinimalValues = value;
            }
        }


        private static string _defaultKeyMaximalValues = "MaximalValues";

        /// <summary>Default key for introduction of maximal values of input and output data elements.</summary>
        public static string DefaultKeyMaximalValues
        {
            get { return _defaultKeyMaximalValues; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for maximal values must be a legal variable name.");
                _defaultKeyMaximalValues = value;
            }
        }


        private static string _defaultKeyDefaultValues = "DefaultValues";

        /// <summary>Default key for introduction of default values of input data elements.</summary>
        public static string DefaultKeyDefaultValues
        {
            get { return _defaultKeyDefaultValues; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for default values must be a legal variable name.");
                _defaultKeyDefaultValues = value;
            }
        }

        private static string _defaultKeySclaingLengths = "ScalingLengths";

        /// <summary>Default key for introduction of scaling lengths of input and output data elements.</summary>
        public static string DefaultKeyScalingLengths
        {
            get { return _defaultKeySclaingLengths; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for scaling lengths must be a legal variable name.");
                _defaultKeySclaingLengths = value;
            }
        }

        private static string _defaultKeyDiscretizationSteps = "DiscretizationSteps";

        /// <summary>Default key for introduction of discretization steps of input and output data elements.</summary>
        public static string DefaultKeyDiscretizationSteps
        {
            get { return _defaultKeyDiscretizationSteps; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for discretization steps must be a legal variable name.");
                _defaultKeyDiscretizationSteps = value;
            }
        }

        private static string _defaultKeyTargetValues = "TargetValues";

        /// <summary>Default key for introduction of target values of input and output data elements.</summary>
        public static string DefaultKeyTargetValues
        {
            get { return _defaultKeyTargetValues; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for target values must be a legal variable name.");
                _defaultKeyTargetValues = value;
            }
        }


        private static string _defaultKeyOptimizationIndices = "OptimizationIndices";

        /// <summary>Default key for introduction of optimization indices of input and output data elements.</summary>
        public static string DefaultKeyOptimizationIndices
        {
            get { return _defaultKeyOptimizationIndices; }
            protected set
            {
                if (!UtilStr.IsVariableName(value))
                    throw new ArgumentException("A default keyword for optimization indices must be a legal variable name.");
                _defaultKeyOptimizationIndices = value;
            }
        }

        

        #endregion Keys.Default


        #region Keys

        protected bool _isKeysCaseSensitive = DefaultIsKeysCaseSensitive;

        /// <summary>Whether keywords are case sensitive.</summary>
        public bool IsKeysCaseSensitive
        {
            get { lock (Lock) { return _isKeysCaseSensitive; } }
            set { lock(Lock) { _isKeysCaseSensitive = value; } }
        }

        /// <summary>Returns true if the specified string represents the specified keyword.
        /// <para>Always use this method to verify if the string represents a specific key, 
        /// sice it takes into account relevant internal parameters such as case sensitivity.</para></summary>
        /// <param name="str">String that is checked against the specified keyword.</param>
        /// <param name="keyString">Keyword ageinst which the string is checked.</param>
        public bool IsKey(string str, string keyString)
        {
            if (string.IsNullOrEmpty(keyString))
                throw new ArgumentException("Keyword for comparison is not specified (null or empty string).");
            if (string.IsNullOrEmpty(str))
                return false;
            if (IsKeysCaseSensitive)
                return (str == keyString);
            else
                return (str.ToLower() == keyString.ToLower());
        }


        /// <summary>Returns a value indicating whether the specified string is a keyword for a field that has a single 
        /// value (e.g. number of parameters or number of output values).</summary>
        /// <param name="str">String that is queried.</param>
        public bool IsSingleValueKey(string str)
        {
            return (IsKeyNumInputParameters(str) || IsKeyNumOutputValues(str));
        }


        /// <summary>Returns a value indicating whether the specified string is a keyword indicating a specific
        /// value (e.g. <see cref="KeyElementTypeInput"/> or <see cref="KeyElementTypeOutput"/>).</summary>
        /// <param name="str">String that is queried.</param>
        public bool IsElementTypeKey(string str)
        {
            return (IsKeyElementTypeInput(str) || IsKeyElementTypeOutput(str));
        }

        /// <summary>Returns a value indicating whether the specified string is a keyword for a field that is
        /// a part of the data definition.</summary>
        /// <param name="str">String that is queried.</param>
        public bool IsDefinitionKey(string str)
        {
            return (
                   IsKeyComment(str)
                || IsKeyNumInputParameters(str)
                || IsKeyNumOutputValues(str)
                || IsKeyNames(str)
                || IsKeyTitles(str)
                || IsKeyDescriptions(str)
                || IsKeyElementTypes(str)
                || IsKeyElementIndices(str)
                || IsKeyMinimalValues(str)
                || IsKeyMaximalValues(str)
                || IsKeyScalingLengths(str)
                || IsKeyDefaultValues(str)
                || IsKeyDiscretizationSteps(str)
                || IsKeyTargetValues(str)
                || IsKeyOptimizationIndices(str)
                );
        }

        
        /// <summary>Returns a value indicating whether the specified string is a keyword for a data field.</summary>
        /// <param name="str">String that is queried.</param>
        public bool IsDataKey(string str)
        {
            return (
                IsKeyData(str)
                );
        }

        /// <summary>Returns a value indicating whether the specified string is a keyword for some field, 
        /// either a part of data definition or a keyword that introduces data.</summary>
        /// <param name="str">String that is queried.</param>
        public bool IsDefinitionOrDataKey(string str)
        {
            return (IsDefinitionKey(str) || IsDataKey(str));
        }

        protected string _keyData = DefaultKeyData;

        /// <summary>Keyword that introduces the sampled data.</summary>
        public string KeyData
        {
            get { lock (Lock) { return _keyData; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for data must be a valid variable name.");
                    _keyData = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing the sampled data.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyData(string str)
        {
            return IsKey(str, KeyData);
        }

        protected string _keyElementTypeInput = DefaultKeyElementTypeInput;

        /// <summary>A keyword string for input element type in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyElementTypeInput
        {
            get { lock (Lock) { return _keyElementTypeInput; } }
            protected set
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("A keyword for input element type must not be null or empty string.");
                    _keyElementTypeInput = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword that marks the input data type.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyElementTypeInput(string str)
        {
            return IsKey(str, KeyElementTypeInput);
        }

        protected string _keyElementTypeOutput = DefaultKeyElementTypeOutput;

        /// <summary>A keyword string for output element type in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyElementTypeOutput
        {
            get { lock (Lock) { return _keyElementTypeOutput; } }
            protected set
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("A keyword for output element type must not be null or empty string.");
                    _keyElementTypeOutput = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword that marks the output data type.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyElementTypeOutput(string str)
        {
            return IsKey(str, KeyElementTypeOutput);
        }


        protected string _keyComment = DefaultKeyComment;

        /// <summary>A keyword string that introduces a commet in the containing line.</summary>
        public string KeyComment
        {
            get { lock (Lock) { return _keyComment; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for introducing comment must be a legal variable name.");
                    _keyComment = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing a comment in the line that contains it.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyComment(string str)
        {
            return IsKey(str, KeyComment);
        }


        protected string _keyNumInputParameters = DefaultKeyNumInputParameters;

        /// <summary>A keyword string that introduces the number of input parameters in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyNumInputParameters
        {
            get { lock (Lock) { return _keyNumInputParameters; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for number of input paremeters must be a legal variable name.");
                    _keyNumInputParameters = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing the number of input parameters.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyNumInputParameters(string str)
        {
            return IsKey(str, KeyNumInputParameters);
        }

        protected string _keyNumOutputValues = DefaultKeyNumOutputValues;

        /// <summary>A keyword string that introduces the number of output parameters in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyNumOutputValues
        {
            get { lock (Lock) { return _keyNumOutputValues; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for number of output parameters must be a legal variable name.");
                    _keyNumOutputValues = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing the number of output parameters.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyNumOutputValues(string str)
        {
            return IsKey(str, KeyNumOutputValues);
        }

        protected string _keyNames = DefaultKeyNames;

        /// <summary>A keyword string that introduces variable names in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyNames
        {
            get { lock (Lock) { return _keyNames; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element names must be a legal variable name.");
                    _keyNames = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable names.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyNames(string str)
        {
            return IsKey(str, KeyNames);
        }

        protected string _keyTitles = DefaultKeyTitles;

        /// <summary>A keyword string that introduces variable titles in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyTitles
        {
            get { lock (Lock) { return _keyTitles; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element titles must be a legal variable name.");
                    _keyTitles = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable titles.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyTitles(string str)
        {
            return IsKey(str, KeyTitles);
        }

        protected string _keyDescriptions = DefaultKeyDescriptions;

        /// <summary>A keyword string that introduces variable descriptions in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyDescriptions
        {
            get { lock (Lock) { return _keyDescriptions; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element descriptions must be a legal variable name.");
                    _keyDescriptions = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable descriptions.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyDescriptions(string str)
        {
            return IsKey(str, KeyDescriptions);
        }

        protected string _keyElementTypes = DefaultKeyElementTypes;

        /// <summary>A keyword string that introduces variable element types in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyElementTypes
        {
            get { lock (Lock) { return _keyElementTypes; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element types must be a legal variable name.");
                    _keyElementTypes = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable element types (e.g. input/output).</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyElementTypes(string str)
        {
            return IsKey(str, KeyElementTypes);
        }

        protected string _keyElementIndices = DefaultKeyElementIndices;

        /// <summary>A keyword string that introduces variable element indices in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyElementIndices
        {
            get { lock (Lock) { return _keyElementIndices; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element names must be a legal variable element indices name.");
                    _keyElementIndices = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable element indices names.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyElementIndices(string str)
        {
            return IsKey(str, KeyElementIndices);
        }

        protected string _keyMinimalValues = DefaultKeyMinimalValues;

        /// <summary>A keyword string that introduces variable minimal values in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyMinimalValues
        {
            get { lock (Lock) { return _keyMinimalValues; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element minimal values must be a legal variable name.");
                    _keyMinimalValues = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable minimal values.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyMinimalValues(string str)
        {
            return IsKey(str, KeyMinimalValues);
        }

        protected string _keyMaximalValues = DefaultKeyMaximalValues;

        /// <summary>A keyword string that introduces variable maximal values in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyMaximalValues
        {
            get { lock (Lock) { return _keyMaximalValues; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element macimal values must be a legal variable name.");
                    _keyMaximalValues = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable maximal values.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyMaximalValues(string str)
        {
            return IsKey(str, KeyMaximalValues);
        }

        protected string _keyScalingLengths = DefaultKeyScalingLengths;

        /// <summary>A keyword string that introduces variable scaling lengths in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyScalingLengths
        {
            get { lock (Lock) { return _keyScalingLengths; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element scaling lengths must be a legal variable name.");
                    _keyScalingLengths = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable scaling lengths.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyScalingLengths(string str)
        {
            return IsKey(str, KeyScalingLengths);
        }
        protected string _keyDefaultValues = DefaultKeyDefaultValues;

        /// <summary>A keyword string that introduces variable default values in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyDefaultValues
        {
            get { lock (Lock) { return _keyDefaultValues; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element default values must be a legal variable name.");
                    _keyDefaultValues = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable default values.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyDefaultValues(string str)
        {
            return IsKey(str, KeyDefaultValues);
        }

        protected string _keyDiscretizationSteps = DefaultKeyDiscretizationSteps;

        /// <summary>A keyword string that introduces variable discretization steps in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyDiscretizationSteps
        {
            get { lock (Lock) { return _keyDiscretizationSteps; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element discretization steps must be a legal variable name.");
                    _keyDiscretizationSteps = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable discretization steps.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyDiscretizationSteps(string str)
        {
            return IsKey(str, KeyDiscretizationSteps);
        }

        protected string _keyTargetValues = DefaultKeyTargetValues;

        /// <summary>A keyword string that introduces variable target values in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyTargetValues
        {
            get { lock (Lock) { return _keyTargetValues; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element target values must be a legal variable name.");
                    _keyTargetValues = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable target values.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyTargetValues(string str)
        {
            return IsKey(str, KeyTargetValues);
        }

        protected string _keyOptimizationIndices = DefaultKeyOptimizationIndices;

        /// <summary>A keyword string that introduces variable optimization indices in the CSV file containing sampled data 
        /// and / or data definitions.</summary>
        public string KeyOptimizationIndices
        {
            get { lock (Lock) { return _keyOptimizationIndices; } }
            protected set
            {
                lock (Lock)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("A keyword for element optimization indices must be a legal variable name.");
                    _keyOptimizationIndices = value;
                }
            }
        }

        /// <summary>Whether the specified string is a keyword introducing variable optimization indices.</summary>
        /// <param name="str">String that is checked.</param>
        public bool IsKeyOptimizationIndices(string str)
        {
            return IsKey(str, KeyOptimizationIndices);
        }


        #endregion Keys


        #region Data

        InputOutputDataDefiniton _dataDefinition;

        public InputOutputDataDefiniton DataDefinition
        {
            get {
                lock (Lock)
                {
                    if (_dataDefinition == null)
                    {
                        if (_inputLength >= 0 && _outputLength >= 0)
                        {
                            _dataDefinition = new InputOutputDataDefiniton();
                            for (int i = 0; i < _inputLength; ++i)
                                _dataDefinition.AddInputElement(new InputElementDefinition(null));
                            for (int i = 0; i < _outputLength; ++i)
                                _dataDefinition.AddOutputElement(new OutputElementDefinition(null));
                        }
                    }
                    return _dataDefinition;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (value != _dataDefinition)
                    {
                        _dataDefinition = value;
                        // invalidate dependencies
                        if (value != null)
                        {
                            InputLength = value.InputLength;
                            OutputLength = value.OutputLength;
                        }
                    }
                }
            }
        }

        SampledDataSet _sampledData;

        public SampledDataSet SampledData
        {
            get
            {
                lock (Lock)
                {
                    if (_sampledData == null)
                    {
                        if (_inputLength >= 0 && _outputLength >= 0)
                        {
                            _sampledData = new SampledDataSet(_inputLength, _outputLength);
                        }
                    }
                    return _sampledData;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (value != SampledData)
                    {
                        _sampledData = value;
                        // invalidate dependencies
                        if (value != null)
                            if (value.InputLength > 0 || value.OutputLength > 0)
                            {
                                InputLength = value.InputLength;
                                OutputLength = value.OutputLength;
                            }
                    }

                }
            }
        }


        int _inputLength = -1;

        /// <summary>Number of input elements.</summary>
        int InputLength
        {
            get
            {
                lock (Lock)
                {
                    if (_inputLength < 0)
                    {
                        // Try to figure out number of input elements from other data:
                        if (_dataDefinition != null)
                        {
                            if (_dataDefinition.InputLength >= 0)
                                _inputLength = _dataDefinition.InputLength;
                        }
                        if (_inputLength < 0 && _sampledData != null)
                        {
                            if (_sampledData.InputLength >= 0)
                                _inputLength = _sampledData.InputLength;
                        }
                    }
                    return _inputLength;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (value != _inputLength)
                    {
                        _inputLength = value;
                        // Invalidate dependencies: 
                        if (_dataDefinition != null)
                        {
                            if (_dataDefinition.InputLength!=value)
                                _dataDefinition = null;
                        }
                        if (_sampledData != null)
                        {
                            if (_sampledData.InputLength!=value)
                                _sampledData = null;
                        }
                    }
                }
            }
        }


        int _outputLength = -1;

        /// <summary>Number of output values in the data.</summary>
        int OutputLength
        {
            get
            {
                lock (Lock)
                {
                    if (_outputLength < 0)
                    {
                        // Try to figure out number of output elements from other data:
                        if (_dataDefinition != null)
                        {
                            if (_dataDefinition.OutputLength >= 0)
                                _outputLength = _dataDefinition.OutputLength;
                        }
                        if (_outputLength < 0 && _sampledData != null)
                        {
                            if (_sampledData.OutputLength >= 0)
                                _outputLength = _sampledData.OutputLength;
                        }
                    }
                    return _outputLength;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (value != _outputLength)
                    {
                        _outputLength = value;
                        // Invalidate dependencies: 
                        // Invalidate dependencies: 
                        if (_dataDefinition != null)
                        {
                            if (_dataDefinition.OutputLength != value)
                                _dataDefinition = null;
                        }
                        if (_sampledData != null)
                        {
                            if (_sampledData.OutputLength != value)
                                _sampledData = null;
                        }
                    }
                }
            }
        }

        #endregion Data


        #region OperationData

        protected int _currentRow = 0;

        public int CurrentRow
        {
            get { lock (Lock) { return _currentRow; }  }
            protected set
            { lock (Lock) { _currentRow = value; } }
        }

        protected int _currentColumn = 0;

        public int CurrentColumn
        {
            get { lock (Lock) { return _currentColumn; } }
            protected set
            { lock (Lock) { _currentColumn = value; } }
        }


        /// <summary>Returns the string indicating the current position in the form (row, column), e.g. "(23, 55)".
        /// <para>Rows and columns are counted form 1.</para></summary>
        /// <returns></returns>
        public string GetPositionString()
        {
            return GetPositionString(CurrentRow, CurrentColumn);
        }

        /// <summary>Returns the string indicating the specified position position in the form (row, column), e.g. "(23, 55)".
        /// <para>Rows and columns are counted form 1.</para></summary>
        /// <param name="rowNum">Row number (counting from 0).</param>
        /// <param name="columnNum">Column number (counting from 0).</param>
        public string GetPositionString(int rowNum, int columnNum)
        {
            return "(" + (rowNum + 1).ToString() + ", " + (columnNum + 1).ToString() + ")";
        }

        /// <summary>Used to define which columns in a data table correspond to which input or output elements.</summary>
        public class DataColumnDefinition
        {

            /// <summary>Number of column within the data table.</summary>
            public int Column = -1;

            /// <summary>Index of the data colum.
            /// <para>Corresponds to sequential index of the corresponding column in the data table, 
            /// in order of appearance in the data table.</para></summary>
            public int ColumnIndex = -1;

            /// <summary>Whether it is known if the element is input or output element.</summary>
            public bool IsInputOutputDefined = false;

            protected bool _isInput = true;

            /// <summary>Whether the column belongs to input elements.</summary>
            public bool IsInput
            {
                get { return _isInput; }
                set { _isInput = value; IsInputOutputDefined = true; }
            }


            /// <summary>Input or output element index within the data set (defines which input parameter
            /// or which output value is defined by the specific element).</summary>
            public int ElementIndex = -1;

            public bool IsDefined()
            {
                return (Column>=0 && ElementIndex>=0 && IsInputOutputDefined);
            }

            /// <summary>Returns string representation of the current column data definition.</summary>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Data column definition No. " + ColumnIndex);
                sb.AppendLine("  Column No.: " + Column);
                if (!IsInputOutputDefined)
                    sb.AppendLine("  Input / output type NOT defined.");
                else
                    sb.AppendLine("  Type: " + (IsInput?" input.":"output."));
                sb.AppendLine("  Element index: " + (ElementIndex<0?" Not defined.":ElementIndex.ToString()));
                sb.AppendLine("  Column data defined: " + IsDefined());
                return sb.ToString();
            }
            
            /// <summary>Returns string representation of a list of data column definitions.</summary>
            /// <param name="list">List of data column definitions whose string representation is returned.</param>
            public static string ToStringList(List<DataColumnDefinition> list)
            {
                return ToStringList(list, null /* introString */);
            }

            /// <summary>Returns string representation of a list of data column definitions.</summary>
            /// <param name="list">List of data column definitions whose string representation is returned.</param>
            /// <param name="introString">Introducory string, which is written in fromt of other data (with 
            /// additional newline after it). If null or empty string then it is skipped.</param>
            public static string ToStringList(List<DataColumnDefinition> list, string introString)
            {
                StringBuilder sb = new StringBuilder();
                if (string.IsNullOrEmpty(introString))
                    sb.AppendLine("List of data columns: ");
                else
                    sb.AppendLine(introString);
                if (list == null)
                    sb.AppendLine("  List of data column definitions is null.");
                else if (list.Count == 0)
                    sb.AppendLine("  List of data column definitions is EMPTY.");
                else
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        DataColumnDefinition def = list[i];
                        if (def == null)
                            sb.AppendLine("  No. " + i + ": null");
                        else
                            sb.AppendLine("  No. " + i + ": " + def.ToString());
                    }
                }
                return sb.ToString();
            }

        }  // class DataColumnDefinition 

        protected List<DataColumnDefinition> _dataColumnDefinitions = new List<DataColumnDefinition>();

        /// <summary>Contains definitions of data columns within the data table.
        /// <para>Definitions follow in the same order as the corresponding columns, but there may
        /// be gaps between them.</para></summary>
        protected List<DataColumnDefinition> DataColumnDefinitions
        { get { lock (Lock) { return _dataColumnDefinitions; } } }

        protected List<DataColumnDefinition> _inputColumnDefinitions = new List<DataColumnDefinition>();

        /// <summary>Contains definitions for input data columns, sorted by element indices.</summary>
        protected List<DataColumnDefinition> InputColumnDefinitions
        {
            get
            {
                lock (Lock)
                {
                    return _inputColumnDefinitions;
                }
            }
        }

        private List<DataColumnDefinition> _outputColumnDefinitions = new List<DataColumnDefinition>();

        /// <summary>Contains definitions for output data columns, sorted by element indices.</summary>
        protected List<DataColumnDefinition> OutputColumnDefinitions
        {
            get
            {
                lock (Lock)
                {
                    return _outputColumnDefinitions;
                }
            }
        }


        /// <summary>Clears data column definitions.</summary>
        protected void ClearDataColumnDefinitions()
        {
            lock (Lock)
            {
                IsDataColumnDefinitionsDefined = false;
                IsDataColumnDefinitionsUpdated = false;
                DataColumnDefinitions.Clear();
                InputColumnDefinitions.Clear();
                OutputColumnDefinitions.Clear();
            }
        }

        /// <summary>Returns a flag defining whether all data columns in the data table are defined.</summary>
        public bool CheckIfDataColumnsDefined()
        {
            lock (Lock)
            {
                int numColumnDefinitions = DataColumnDefinitions.Count;
                if (numColumnDefinitions == 0)
                    return false;
                for (int i=0; i < numColumnDefinitions; ++i)
                {
                    if (DataColumnDefinitions[i] == null)
                        return false;
                    else if (!DataColumnDefinitions[i].IsDefined())
                        return false;
                }
                return true;
            }
        }

        /// <summary>Whether a full list of data columns is defined.</summary>
        protected bool IsDataColumnDefinitionsDefined = false;


        private bool _isDataColumnDefinitionsUpdated = false;

        /// <summary>Whether complete information about data column definitions is up to date,
        /// including auciliary lists with sorted input and output definitions.</summary>
        public bool IsDataColumnDefinitionsUpdated
        {
            get { lock(Lock) { return _isDataColumnDefinitionsUpdated; } }
            protected set { lock(Lock) { _isDataColumnDefinitionsUpdated = value; } }
        }


        /// <summary>Updates index lists for data column definitions for inputs (<see cref="InputColumnDefinitions"/>)
        /// and outputs (<see cref="OutputColumnDefinitions"/>), and sets the <see cref="IsDataColumnDefinitionsUpdated"/>
        /// flag to true if successful.</summary>
        public void UpdateDataColumnDefinitions()
        {
            lock (Lock)
            {
                if (!IsDataColumnDefinitionsUpdated || !IsDataColumnDefinitionsDefined)
                {
                    InputColumnDefinitions.Clear();
                    OutputColumnDefinitions.Clear();
                    if (CheckIfDataColumnsDefined())
                    {
                        int numDataColumns = DataColumnDefinitions.Count;
                        for (int i=0; i<numDataColumns; ++i)
                        {
                            DataColumnDefinition def = DataColumnDefinitions[i];
                            if (def == null)
                                throw new InvalidDataException("Data column definition No. " + i + " is null.");
                            if (!def.IsDefined())
                                throw new InvalidDataException("Data column No. " + i + " is not fully defined. " + Environment.NewLine
                                    + "  column: " + def.Column + ", element index: " + def.ElementIndex
                                    + ", input/output: " + (def.IsInputOutputDefined?(def.IsInput?"Input":"Output"):"Undefined"));
                            else
                            {
                                if (def.IsInput)
                                    InputColumnDefinitions.Add(def);
                                else
                                    OutputColumnDefinitions.Add(def);
                            }
                        }
                        InputColumnDefinitions.Sort(
                            (el1, el2) => {
                                if (el1!=null && el2!=null)
                                {
                                    if (el1.ElementIndex < el2.ElementIndex)
                                        return -1;
                                    else if (el1.ElementIndex > el2.ElementIndex)
                                        return 1;
                                    else
                                        return 0;
                                }
                                return 0;
                            }
                        );
                        OutputColumnDefinitions.Sort(
                            (el1, el2) =>
                            {
                                if (el1 != null && el2 != null)
                                {
                                    if (el1.ElementIndex < el2.ElementIndex)
                                        return -1;
                                    else if (el1.ElementIndex > el2.ElementIndex)
                                        return 1;
                                    else
                                        return 0;
                                }
                                return 0;
                            }
                        );
                        // Update data definiton:
                        for (int i = 0; i < InputColumnDefinitions.Count; ++i)
                        {
                            DataColumnDefinition column = InputColumnDefinitions[i];
                            if (column == null)
                                throw new InvalidDataException("Column definition for input element " + i + " is not specified (null reference).");
                            else
                            {
                                InputElementDefinition element = GetInputElementDefinition(i);
                                if (!column.IsInputOutputDefined)
                                    throw new InvalidDataException("Column " + column.Column + ": input/output status unknown.");
                                if (!column.IsInput)
                                    throw new InvalidDataException("Column " + column.Column + " for input element is not specified as input.");
                                if (column.ElementIndex >= 0)
                                {
                                    element.ElementIndex = column.ElementIndex;
                                    element.ElementIndexSpecified = true;
                                }
                            }
                        }
                        for (int i = 0; i < OutputColumnDefinitions.Count; ++i)
                        {
                            DataColumnDefinition column = OutputColumnDefinitions[i];
                            if (column == null)
                                throw new InvalidDataException("Column definition for output element " + i + " is not specified (null reference).");
                            else
                            {
                                OutputElementDefinition element = GetOutputElementDefinition(i);
                                if (!column.IsInputOutputDefined)
                                    throw new InvalidDataException("Column " + column.Column + ": input/output status unknown.");
                                if (column.IsInput)
                                    throw new InvalidDataException("Column " + column.Column + " for output element is not specified as output.");
                                if (column.ElementIndex >= 0)
                                {
                                    element.ElementIndex = column.ElementIndex;
                                    element.ElementIndexSpecified = true;
                                }
                            }
                        }
                        IsDataColumnDefinitionsDefined = true;
                        IsDataColumnDefinitionsUpdated = true;
                    }
                }
            }
        }



        #endregion OperationData


        #region Operation.Write


        protected bool _keyAndDataInSameRow = false;

        /// <summary>Whether keys and data are in the same row in data definition section wnen writing to CSV format.</summary>
        public bool KeyAndDataInSameRow
        {
            get { lock (Lock) { return _keyAndDataInSameRow; } }
            set { lock (Lock) { 
                _keyAndDataInSameRow = value;
                if (value == true && _indentation < 1)
                    _indentation = 1;
            } }
        }
        
        protected int _indentation = 0;

        /// <summary>Offset - specifies in which column data columns begin when writing tyo CSV format.</summary>
        public int Indentation
        {
            get { lock (Lock) { return _indentation; } }
            set
            {
                lock (Lock)
                {
                    _indentation = value;
                    if (_keyAndDataInSameRow)
                    {
                        if (_indentation < 1)
                            _indentation = 0;
                    }
                    else
                    {
                        if (_indentation < 0)
                            _indentation = 0;
                    }
                }
            }
        }

        public bool _throwExceptionsOnDataErrors = false;

        public bool ThrowExceptionsOnDataErrors
        {
            get { lock (Lock) { return _throwExceptionsOnDataErrors; } }
            set { lock (Lock) { _throwExceptionsOnDataErrors = value; } }
        }

        /// <summary>Stores definition data to the data table.</summary>
        /// <param name="clearFirst">Whether the data table is cleared before the operation begins.</param>
        protected void StoreDefinition(bool clearFirst) 
        {
            if (Indentation < 0)
                Indentation = 0;
            if (KeyAndDataInSameRow && Indentation < 1)
                Indentation = 1;
            lock(Lock)
            {
                if (clearFirst)
                {
                    Clear();
                    CurrentRow = 0;
                    CurrentColumn = 0;
                }
                if (DataDefinition == null)
                    throw new InvalidOperationException("Data definition is not specified  (null reference), can not be stored.");
                // Store number of input and output parameters:
                this[CurrentRow, 0] = KeyNumInputParameters;
                if (!KeyAndDataInSameRow)
                    ++CurrentRow; 
                this[CurrentRow, Indentation + 0] = DataDefinition.InputLength.ToString();
                ++CurrentRow;
                CurrentColumn = 0;
                this[CurrentRow, 0] = KeyNumOutputValues;
                if (!KeyAndDataInSameRow)
                    ++CurrentRow;
                this[CurrentRow, Indentation + 0] = DataDefinition.OutputLength.ToString();
                ++CurrentRow;
                CurrentColumn = 0;
                // Store input/output flags:
                this[CurrentRow, 0] = KeyElementTypes;
                if (!KeyAndDataInSameRow)
                    ++CurrentRow;
                CurrentColumn = Indentation + 0;
                for (int i = 0; i < DataDefinition.InputLength; ++i)
                {
                    this[CurrentRow, CurrentColumn] = KeyElementTypeInput;
                    ++ CurrentColumn;
                }
                for (int i=0; i<DataDefinition.OutputLength; ++i)
                {
                    this[CurrentRow, CurrentColumn] = KeyElementTypeOutput;
                    ++ CurrentColumn;
                }
                ++CurrentRow;
                CurrentColumn = 0;
                // Store element indices:
                this[CurrentRow, 0] = KeyElementIndices;
                if (!KeyAndDataInSameRow)
                    ++CurrentRow;
                CurrentColumn = Indentation + 0;
                for (int i = 0; i < DataDefinition.InputLength; ++i)
                {
                    this[CurrentRow, CurrentColumn] = i.ToString();
                    ++ CurrentColumn;
                }
                for (int i=0; i<DataDefinition.OutputLength; ++i)
                {
                    this[CurrentRow, CurrentColumn] = i.ToString();
                    ++ CurrentColumn;
                }
                ++CurrentRow;
                CurrentColumn = 0;
                // Store element names:
                this[CurrentRow, 0] = KeyNames;
                if (!KeyAndDataInSameRow)
                    ++CurrentRow;
                CurrentColumn = Indentation + 0;
                for (int i = 0; i < DataDefinition.InputLength; ++i)
                {
                    InputElementDefinition def = DataDefinition.GetInputElement(i);
                    if (def != null)
                    {
                        if (!string.IsNullOrEmpty(def.Name))
                            this[CurrentRow, CurrentColumn] = def.Name;
                        else
                        {
                            if (ThrowExceptionsOnDataErrors)
                                throw new InvalidDataException("Input element No. " + i + " does not have name specified.");
                        }
                    }
                    ++ CurrentColumn;
                }
                for (int i=0; i<DataDefinition.OutputLength; ++i)
                {
                    OutputElementDefinition def = DataDefinition.GetOutputElement(i);
                    if (def != null)
                    {
                        if (!string.IsNullOrEmpty(def.Name))
                            this[CurrentRow, CurrentColumn] = def.Name;
                        else
                        {
                            if (ThrowExceptionsOnDataErrors)
                                throw new InvalidDataException("Input element No. " + i + " does not have name specified.");
                        }
                    }
                    ++ CurrentColumn;
                }
                ++CurrentRow;
                CurrentColumn = 0;
                if (DataDefinition.IsAnyTitleDefined())
                {
                    // Store element titles:
                    this[CurrentRow, 0] = KeyTitles;
                    if (!KeyAndDataInSameRow)
                        ++CurrentRow;
                    CurrentColumn = Indentation + 0;
                    for (int i = 0; i < DataDefinition.InputLength; ++i)
                    {
                        InputElementDefinition def = DataDefinition.GetInputElement(i);
                        if (def != null)
                        {
                            if (!string.IsNullOrEmpty(def.Title))
                                this[CurrentRow, CurrentColumn] = def.Title;
                        }
                        ++CurrentColumn;
                    }
                    for (int i = 0; i < DataDefinition.OutputLength; ++i)
                    {
                        OutputElementDefinition def = DataDefinition.GetOutputElement(i);
                        if (def != null)
                        {
                            if (!string.IsNullOrEmpty(def.Title))
                                this[CurrentRow, CurrentColumn] = def.Title;
                        }
                        ++CurrentColumn;
                    }
                    ++CurrentRow;
                    CurrentColumn = 0;
                }
                if (DataDefinition.IsAnyDescriptionDefined())
                {
                    // Store element descriptions:
                    this[CurrentRow, 0] = KeyDescriptions;
                    if (!KeyAndDataInSameRow)
                        ++CurrentRow;
                    CurrentColumn = Indentation + 0;
                    for (int i = 0; i < DataDefinition.InputLength; ++i)
                    {
                        InputElementDefinition def = DataDefinition.GetInputElement(i);
                        if (def != null)
                        {
                            if (!string.IsNullOrEmpty(def.Description))
                                this[CurrentRow, CurrentColumn] = def.Description;
                        }
                        ++CurrentColumn;
                    }
                    for (int i = 0; i < DataDefinition.OutputLength; ++i)
                    {
                        OutputElementDefinition def = DataDefinition.GetOutputElement(i);
                        if (def != null)
                        {
                            if (!string.IsNullOrEmpty(def.Description))
                                this[CurrentRow, CurrentColumn] = def.Description;
                        }
                        ++CurrentColumn;
                    }
                    ++CurrentRow;
                    CurrentColumn = 0;
                }
                if (DataDefinition.IsAnyBoundDefined())
                {
                    // Store element minimal values:
                    this[CurrentRow, 0] = KeyMinimalValues;
                    if (!KeyAndDataInSameRow)
                        ++CurrentRow;
                    CurrentColumn = Indentation + 0;
                    for (int i = 0; i < DataDefinition.InputLength; ++i)
                    {
                        InputElementDefinition def = DataDefinition.GetInputElement(i);
                        if (def != null)
                        {
                            if (def.BoundsDefined)
                                this[CurrentRow, CurrentColumn] = def.MinimalValue.ToString();
                        }
                        ++CurrentColumn;
                    }
                    for (int i = 0; i < DataDefinition.OutputLength; ++i)
                    {
                        OutputElementDefinition def = DataDefinition.GetOutputElement(i);
                        if (def != null)
                        {
                            if (def.BoundsDefined)
                                this[CurrentRow, CurrentColumn] = def.MinimalValue.ToString();
                        }
                        ++CurrentColumn;
                    }
                    ++CurrentRow;
                    CurrentColumn = 0;
                    // Store element maximal values:
                    this[CurrentRow, 0] = KeyMaximalValues;
                    if (!KeyAndDataInSameRow)
                        ++CurrentRow;
                    CurrentColumn = Indentation + 0;
                    for (int i = 0; i < DataDefinition.InputLength; ++i)
                    {
                        InputElementDefinition def = DataDefinition.GetInputElement(i);
                        if (def != null)
                        {
                            if (def.BoundsDefined)
                                this[CurrentRow, CurrentColumn] = def.MaximalValue.ToString();
                        }
                        ++CurrentColumn;
                    }
                    for (int i = 0; i < DataDefinition.OutputLength; ++i)
                    {
                        OutputElementDefinition def = DataDefinition.GetOutputElement(i);
                        if (def != null)
                        {
                            if (def.BoundsDefined)
                                this[CurrentRow, CurrentColumn] = def.MaximalValue.ToString();
                        }
                        ++CurrentColumn;
                    }
                    ++CurrentRow;
                    CurrentColumn = 0;
                }
                if (DataDefinition.IsAnyScalingLengthDefined())
                {
                    // Store element scaling lengths:
                    this[CurrentRow, 0] = KeyScalingLengths;
                    if (!KeyAndDataInSameRow)
                        ++CurrentRow;
                    CurrentColumn = Indentation + 0;
                    for (int i = 0; i < DataDefinition.InputLength; ++i)
                    {
                        InputElementDefinition def = DataDefinition.GetInputElement(i);
                        if (def != null)
                        {
                            if (def.ScalingLengthDefined)
                                this[CurrentRow, CurrentColumn] = def.ScalingLength.ToString();
                        }
                        ++CurrentColumn;
                    }
                    for (int i = 0; i < DataDefinition.OutputLength; ++i)
                    {
                        OutputElementDefinition def = DataDefinition.GetOutputElement(i);
                        if (def != null)
                        {
                            if (def.ScalingLengthDefined)
                                this[CurrentRow, CurrentColumn] = def.ScalingLength.ToString();
                        }
                        ++CurrentColumn;
                    }
                    ++CurrentRow;
                    CurrentColumn = 0;
                }
                if (DataDefinition.IsAnyDefaultValueDefined())
                {
                    // Store element default values:
                    this[CurrentRow, 0] = KeyDefaultValues;
                    if (!KeyAndDataInSameRow)
                        ++CurrentRow;
                    CurrentColumn = Indentation + 0;
                    for (int i = 0; i < DataDefinition.InputLength; ++i)
                    {
                        InputElementDefinition def = DataDefinition.GetInputElement(i);
                        if (def != null)
                        {
                            if (def.DefaultValueDefined)
                                this[CurrentRow, CurrentColumn] = def.DefaultValue.ToString();
                        }
                        ++CurrentColumn;
                    }
                    for (int i = 0; i < DataDefinition.OutputLength; ++i)
                    {
                        //OutputElementDefinition def = DataDefinition.GetOutputElement(i);
                        //if (def != null)
                        //{
                        //    if (def.DefaultValueDefined)
                        //        this[CurrentRow, CurrentColumn] = def.DefaultValue.ToString();
                        //}
                        ++CurrentColumn;
                    }
                    ++CurrentRow;
                    CurrentColumn = 0;
                }
                if (DataDefinition.IsAnyDiscretizationStepDefined())
                {
                    // Store element discretization steps:
                    this[CurrentRow, 0] = KeyDiscretizationSteps;
                    if (!KeyAndDataInSameRow)
                        ++CurrentRow;
                    CurrentColumn = Indentation + 0;
                    for (int i = 0; i < DataDefinition.InputLength; ++i)
                    {
                        InputElementDefinition def = DataDefinition.GetInputElement(i);
                        if (def != null)
                        {
                            if (def.DiscretizationStep>0)
                                this[CurrentRow, CurrentColumn] = def.DiscretizationStep.ToString();
                        }
                        ++CurrentColumn;
                    }
                    for (int i = 0; i < DataDefinition.OutputLength; ++i)
                    {
                        //OutputElementDefinition def = DataDefinition.GetOutputElement(i);
                        //if (def != null)
                        //{
                        //    if (def.DiscretizationStep>0)
                        //        this[CurrentRow, CurrentColumn] = def.DiscretizationStep.ToString();
                        //}
                        ++CurrentColumn;
                    }
                    ++CurrentRow;
                    CurrentColumn = 0;
                }
                if (DataDefinition.IsAnyTargetValueDefined())
                {
                    // Store element target values:
                    this[CurrentRow, 0] = KeyTargetValues;
                    if (!KeyAndDataInSameRow)
                        ++CurrentRow;
                    CurrentColumn = Indentation + 0;
                    for (int i = 0; i < DataDefinition.InputLength; ++i)
                    {
                        InputElementDefinition def = DataDefinition.GetInputElement(i);
                        if (def != null)
                        {
                            if (def.TargetValueDefined)
                                this[CurrentRow, CurrentColumn] = def.TargetValue.ToString();
                        }
                        ++CurrentColumn;
                    }
                    for (int i = 0; i < DataDefinition.OutputLength; ++i)
                    {
                        OutputElementDefinition def = DataDefinition.GetOutputElement(i);
                        if (def != null)
                        {
                            if (def.TargetValueDefined)
                                this[CurrentRow, CurrentColumn] = def.TargetValue.ToString();
                        }
                        ++CurrentColumn;
                    }
                    ++CurrentRow;
                    CurrentColumn = 0;
                }
                if (DataDefinition.IsAnyOptimizationIndexSpecified())
                {
                    // Store element optimization indices:
                    this[CurrentRow, 0] = KeyOptimizationIndices;
                    if (!KeyAndDataInSameRow)
                        ++CurrentRow;
                    CurrentColumn = Indentation + 0;
                    for (int i = 0; i < DataDefinition.InputLength; ++i)
                    {
                        InputElementDefinition def = DataDefinition.GetInputElement(i);
                        if (def != null)
                        {
                            if (def.OptimizationIndexSpecified)
                                this[CurrentRow, CurrentColumn] = def.OptimizationIndex.ToString();
                        }
                        ++CurrentColumn;
                    }
                    for (int i = 0; i < DataDefinition.OutputLength; ++i)
                    {
                        //OutputElementDefinition def = DataDefinition.GetOutputElement(i);
                        //if (def != null)
                        //{
                        //    if (def.OptimizationIndexSpecified)
                        //        this[CurrentRow, CurrentColumn] = def.OptimizationIndex.ToString();
                        //}
                        ++CurrentColumn;
                    }
                    ++CurrentRow;
                    CurrentColumn = 0;
                }
            }
        }


        /// <summary>Stores the samples data to the data table.</summary>
        /// <param name="clearFirst">Whether the data table is cleared before the operation begins.</param>
        protected void StoreData(bool clearFirst)
        {
            if (clearFirst)
            {
                Clear();
                CurrentRow = 0;
                CurrentColumn = 0;
            }
            this[CurrentRow, CurrentColumn] = KeyData;
            ++ CurrentRow;
            for (int which = 0; which < SampledData.Length; ++which)
            {
                SampledDataElement data = SampledData[which];
                IVector input = data.InputParameters;
                IVector output = data.OutputValues;
                if (ThrowExceptionsOnDataErrors)
                {
                    if (input == null)
                    {
                        if (InputLength>0)
                            throw new InvalidDataException("Data set No. " + which + ": input parameters not defined.");
                    } else if (input.Length!=InputLength)
                    {
                        throw new InvalidDataException("Data set No. " + which + ": input parameters have wrong dimension, " + input.Length + " instead of " + InputLength + ".");
                    }
                    if (output == null)
                    {
                        if (OutputLength>0)
                            throw new InvalidDataException("Data set No. " + which + ": output values not defined.");
                    } else if (output.Length!=OutputLength)
                    {
                        throw new InvalidDataException("Data set No. " + which + ": output values have wrong dimension, " + output.Length + " instead of " + OutputLength + ".");
                    }
                }
                CurrentColumn = Indentation + 0;
                for (int i = 0; i < InputLength; ++i)
                {
                    if (input!=null)
                        if (i<input.Length)
                            this[CurrentRow, CurrentColumn] = input[i].ToString();
                    ++ CurrentColumn;
                }
                for (int i=0; i<OutputLength; ++i)
                {
                    if (output!=null)
                        if (i<output.Length)
                            this[CurrentRow, CurrentColumn] = output[i].ToString();
                    ++ CurrentColumn;
                }
                ++CurrentRow;
            }
        }

        /// <summary>Stores data definitions AND sampled data to the data table.</summary>
        /// <param name="clearFirst">Whether the data table is cleared before the operation begins.</param>
        protected void StoreDefinitionAndData(bool clearFirst)
        {
            if (clearFirst)
            {
                Clear();
                CurrentRow = 0;
                CurrentColumn = 0;
            }
            StoreDefinition(false /* clearFirst */ );
            StoreData(false /* clearFirst */ );
        }

        /// <summary>Stores data definitions to the data table.
        /// <para>The table is cleared first.</para></summary>
        public void StoreDefinition()
        {
            StoreDefinition(true /* clearFirst */ );
        }

        /// <summary>Stores sampled data to the data table.
        /// <para>The table is cleared first.</para></summary>
        public void StoreData()
        {
            StoreData(true /* clearFirst */ );
        }


        /// <summary>Stores data definitions AND sampled data to the data table.
        /// <para>The table is cleared first.</para></summary>
        public void StoreDefinitionAndData()
        {
            StoreDefinitionAndData(true /* clearFirst */);
        }

        /// <summary>Saves data definitions to the specified file.
        /// <para>The file is overridden if it already exists.</para></summary>
        /// <param name="inputFilePath">Path to the file where data definitions are stored.</param>
        public void SaveDefinition(string filePath)
        {
            lock (Lock)
            {
                StoreDefinition();
                SaveCsv(filePath, false /* append */);
            }
        }


        /// <summary>Saves sampled data to the specified file.
        /// <para>The file is overridden if it already exists.</para></summary>
        /// <param name="inputFilePath">Path to the file where data is stored.</param>
        public void SaveData(string filePath)
        {
            lock (Lock)
            {
                StoreData();
                SaveCsv(filePath, false /* append */);
            }
        }


        /// <summary>Saves data definitions AND sampled data to the specified file.
        /// <para>The file is overridden if it already exists.</para></summary>
        /// <param name="inputFilePath">Path to the file where data is stored.</param>
        public void SaveDefinitionAndData(string filePath)
        {
            lock (Lock)
            {
                StoreDefinitionAndData();
                SaveCsv(filePath, false /* append */);
            }
        }


        #endregion Operation.Write

        
        #region Operation.Read.Aux

        /// <summary>Gets the specified input element definition from <see cref="DataDefinition"/>. If data definition
        /// is not defined then it is created, and if it has less input elements than the required element index
        /// plus one, then new element definitions are created and added to it.
        /// <para>Method does not check whether element index is out of range with respect to <see cref="InputLength"/>.</para></summary>
        /// <param name="whichElement">Specifies which input element definition is returned.</param>
        protected InputElementDefinition GetInputElementDefinition(int whichElement)
        {
            lock (Lock)
            {
                if (DataDefinition == null)
                    DataDefinition = new InputOutputDataDefiniton();
                while (DataDefinition.InputLength < whichElement + 1)
                    DataDefinition.AddInputElement(new InputElementDefinition(null));
                return DataDefinition.GetInputElement(whichElement);
            }
        }

        /// <summary>Gets the specified output element definition from <see cref="DataDefinition"/>. If data definition
        /// is not defined then it is created, and if it has less output elements than the required element index
        /// plus one, then new element definitions are created and added to it.
        /// <para>Method does not check whether element index is out of range with respect to <see cref="OutputLength"/>.</para></summary>
        /// <param name="whichElement">Specifies which output element definition is returned.</param>
        protected OutputElementDefinition GetOutputElementDefinition(int whichElement)
        {
            lock (Lock)
            {
                if (DataDefinition == null)
                    DataDefinition = new InputOutputDataDefiniton();
                while (DataDefinition.OutputLength < whichElement + 1)
                    DataDefinition.AddOutputElement(new OutputElementDefinition(null));
                return DataDefinition.GetOutputElement(whichElement);
            }
        }

        /// <summary>Returns the specified data column definition by its sequential number on the list
        /// of columns <see cref="DataColumnDefinitions"/>.</summary>
        /// <param name="dataColumnIndex">Sequential number of the column data definition on the list of
        /// definitions. Definitions follow in the same order as the corresponding columns, but there may
        /// be gaps between them.</param>
        protected DataColumnDefinition GetDataColumnDefinition(int dataColumnIndex)
        {
            while (DataColumnDefinitions.Count <= dataColumnIndex)
                DataColumnDefinitions.Add(new DataColumnDefinition());
            return DataColumnDefinitions[dataColumnIndex];
        }

        /// <summary>Reads a single integer value. Current line and column must be set to its key.</summary>
        /// <remarks>After successful execution, current cell is set just after the value position.</remarks>
        protected bool ReadSingleInt(ref int value)
        {
            int row = CurrentRow;
            int column = CurrentColumn + 1;
            column = FirstNonemptyColumn(row,column);
            if (column >= 0)
            {
                CurrentColumn = column+1;
                if (IsInt(row, column))
                {
                    value = GetInt(row, column);
                    return true;
                }
                else
                {
                    throw new InvalidDataException("Invalid data at " + GetPositionString(row, column) + ": should be an integer."
                        + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                }
            } else
            {
                // Check the next line:
                ++row; column = 0;
                column = FirstNonemptyColumn(row, column);
                if (column >= 0)
                {
                    if (IsInt(row, column))
                    {
                        CurrentRow = row;
                        CurrentColumn = column + 1;
                        value = GetInt(row, column);
                        return true;
                    } else
                    {
                        throw new InvalidDataException("Invalid data at " + GetPositionString(row, column) + ": should be an integer."
                            + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                    }
                }
            }
            return false;
        }

        /// <summary>Finds the first data cell for data inroduced by a key, provided that
        /// the key that introduces the data is located at (<paramref name="keyRow"/>, <paramref name="keyColumn"/>).</summary>
        /// <param name="keyRow">Row number of the cell containing the key that introduces the data.</param>
        /// <param name="keyColumn">Column number of the cell containing the key that introduces the data.</param>
        /// <param name="row">Variable where row number of the cell where data begins is stored.</param>
        /// <param name="column">Variable where column number of the cell where data begins is stored.</param>
        /// <exception cref="">When the first data cell could not be located.</exception>
        protected void FindFirstDataCell(int keyRow, int keyColumn, out int row, out int column)
        {
            lock (Lock)
            {
                row = keyRow;
                column = keyColumn + 1;
                column = FirstNonemptyColumn(row, column);
                if (column < 0)
                {
                    ++row;
                    column = FirstNonemptyColumn(row, 0);
                }
                if (column < 0)
                {
                    throw new InvalidDataException("At " + GetPositionString(keyRow, keyColumn) + ": data corresponding to the key not found."
                        + Environment.NewLine + "  Key: \"" + this[keyRow, keyColumn]);
                }
            }
        }

        /// <summary>Finds the first data cell for data inroduced by a key, provided that
        /// the key that introduces the data is located at (<see cref="CurrentRow"/>, <see cref="CurrentColumn"/>).</summary>
        /// <param name="row">Variable where row number of the cell where data begins is stored.</param>
        /// <param name="column">Variable where column number of the cell where data begins is stored.</param>
        /// <exception cref="">When the first data cell could not be located.</exception>
        protected void FindFirstDataCell(out int row, out int column)
        {
            FindFirstDataCell(CurrentRow, CurrentColumn, out row, out column);
        }

        /// <summary>Extracts information about data columns from the data.</summary>
        /// <param name="keyRow">Row of the cell that contains the key that introduces the data from
        /// which information is extracted.</param>
        /// <param name="keyColumn">Column of the cell that contains the key that introduces the data from
        /// which information is extracted.</param>
        /// <param name="isDouble">Whether data elements are real numbers (e.g. of type double).</param>
        /// <param name="isInt">Whether data elements are integer numbers.</param>
        /// <remarks><para>For tis method in order to work, data must be defined for all input and output
        /// elements, and no additional cells must be nonempty.</para>
        /// <para>This method is called when one encounters data whose reading would already require 
        /// infomration about data columns, but this information is not yet available from the definition
        /// data already read and must be inferred from the data itself. Conditions are that data
        /// cellls are in natural order (first input and then output elements in the correct order), that
        /// there are no non-empty cells in the data row from the beginning of data that do not belong to
        /// data, that all data columns (input and output) are defined and that input and output dimensions
        /// are known.</para></remarks>
        protected void GetDataColumnDefinitionsFromData(int keyRow, int keyColumn, bool isDouble, bool isInt)
        {
            lock (Lock)
            {
                if (!IsDataColumnDefinitionsDefined)
                {
                    if (InputLength < 0)
                        throw new InvalidOperationException("Can not infer data column information from data since input dimension is unknown.");
                    if (InputLength < 0)
                        throw new InvalidOperationException("Can not infer data column information from data since output dimension is unknown.");
                    int row;
                    int column;
                    FindFirstDataCell(out row, out column);
                    if (column >= 0)
                    {
                        CurrentRow = row;
                        CurrentColumn = column + 1;
                        int whichDataColumn = 0;
                        int whichInput = 0;
                        int whichOutput = 0;
                        while (column >= 0)
                        {
                            if (whichDataColumn >= InputLength + OutputLength)
                                throw new InvalidDataException("Invalid data at " + GetPositionString(row, column) + ", data cell No. " + whichDataColumn + ": "
                                    + Environment.NewLine + "  There are more data columns than input and output dimensions together."
                                            + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                            bool isInput = (whichDataColumn < InputLength);
                            bool isOutput = !isInput;
                            // string cellValue = this[row, column];
                            if (isInt && !IsInt(row, column))
                                throw new InvalidDataException("Invalid data at " + GetPositionString(row, column) + ": should be an integer."
                                            + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                            if (isDouble && !IsDouble(row, column))
                                throw new InvalidDataException("Invalid data at " + GetPositionString(row, column) + ": should be a number."
                                            + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                            DataColumnDefinition def = GetDataColumnDefinition(whichDataColumn);
                            def.Column = column;
                            def.ColumnIndex = whichDataColumn;
                            if (isInput)
                            {
                                def.IsInput = true;
                                //if (def.ElementIndex < 0)
                                //{
                                def.ElementIndex = whichInput;
                                //}
                            }
                            else if (isOutput)
                            {
                                def.IsInput = false;
                                //if (def.ElementIndex < 0)
                                //{
                                def.ElementIndex = whichOutput;
                                //}
                            }

                            if (isInput)
                                ++whichInput;
                            else
                                ++whichOutput;
                            ++whichDataColumn;
                            column = FirstNonemptyColumn(row, column + 1);
                        }
                    }
                }
                if (CheckIfDataColumnsDefined())
                {
                    UpdateDataColumnDefinitions();
                } else
                {
                    throw new InvalidDataException("Data column definitions could not be inferred from data at " + GetPositionString() + ".");
                }
            }
        }

        /// <summary>Extracts information about data columns from the data.</summary>
        /// <param name="keyRow">Row of the cell that contains the key that introduces the data from
        /// which information is extracted.</param>
        /// <param name="keyColumn">Column of the cell that contains the key that introduces the data from
        /// which information is extracted.</param>
        /// <param name="isDouble">Whether data elements are real numbers (e.g. of type double).</param>
        /// <param name="isInt">Whether data elements are integer numbers.</param>
        /// <remarks><para>For tis method in order to work, data must be defined for all input and output
        /// elements, and no additional cells must be nonempty.</para>
        /// <para>This method is called when one encounters data whose reading would already require 
        /// infomration about data columns, but this information is not yet available from the definition
        /// data already read and must be inferred from the data itself. Conditions are that data
        /// cellls are in natural order (first input and then output elements in the correct order), that
        /// there are no non-empty cells in the data row from the beginning of data that do not belong to
        /// data, that all data columns (input and output) are defined and that input and output dimensions
        /// are known.</para></remarks>
        protected void GetDataColumnDefinitionsFromData(bool isDouble, bool isInt)
        {
            GetDataColumnDefinitionsFromData(CurrentRow /* keyRow */, CurrentColumn /* keyColumn */, isDouble, isInt);
        }

        /// <summary>Reads types of elements from the CSV-like string table, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementTypes()
        {
            lock (Lock)
            {
                int row; // = CurrentRow;
                int column; // = CurrentColumn + 1;
                FindFirstDataCell(out row, out column);
                if (column >= 0) if (!IsElementTypeKey(this[row, column]))
                    throw new InvalidDataException("Invalid element type at " + GetPositionString(row, column) + ": not a valid type."
                            + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                if (column >= 0)
                {
                    CurrentRow = row;
                    CurrentColumn = column + 1;
                    int whichDataColumn = 0;
                    int whichInput = 0;
                    int whichOutput = 0;
                    while (column >= 0)
                    {
                        string cellValue = this[row, column];
                        bool isInput = IsKeyElementTypeInput(cellValue);
                        bool isOutput = IsKeyElementTypeOutput(cellValue);
                        if (isInput || isOutput)
                        {
                            if (OutputLevel >= 2)
                            {
                                Console.WriteLine("Element type of column No. " + whichDataColumn + ": " + (isInput?"Input":"Output"));
                            }
                            DataColumnDefinition def = GetDataColumnDefinition(whichDataColumn);
                            def.Column = column;
                            def.ColumnIndex = whichDataColumn;
                            if (isInput)
                            {
                                if (isOutput)
                                    throw new InvalidOperationException("Invalid data at " + GetPositionString(row, column) + ": can not be input and output at the same time."
                                        + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                                if (def.IsInputOutputDefined && !def.IsInput)
                                    throw new InvalidDataException("Invalid data at " + GetPositionString(row, column) + ": column previously defined as output, now as input."
                                        + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                                def.IsInput = true;
                                if (def.ElementIndex < 0)
                                {
                                    def.ElementIndex = whichInput;
                                }
                                ++whichInput;
                            } else if (isOutput)
                            {
                                def.IsInput = false;
                                if (def.IsInputOutputDefined && def.IsInput)
                                    throw new InvalidDataException("Invalid data at " + GetPositionString(row, column) + ": column previously defined as input, now as output."
                                        + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                                def.IsInput = false;
                                if (def.ElementIndex < 0)
                                {
                                    def.ElementIndex = whichOutput;
                                }
                                ++whichOutput;
                            }
                            ++whichDataColumn;
                        }
                        else
                        {
                            throw new InvalidDataException("Invalid element type " + GetPositionString(row, column) + ": neither \"" + KeyElementTypeInput + "\" nor  \"" + KeyElementTypeOutput + "\"."
                                + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                        }
                        column = FirstNonemptyColumn(row, column + 1);
                    }
                    if (InputLength < 0)
                        InputLength = whichInput;
                    else
                    {
                        if (whichInput > 0 && whichInput != InputLength)
                            throw new InvalidDataException("Number of stated input elements, " 
                                + whichInput + ", is different than the current input length, " + InputLength);
                    }
                    if (OutputLength < 0)
                        OutputLength = whichOutput;
                    else
                    {
                        if (whichOutput > 0 && whichOutput != OutputLength)
                            throw new InvalidDataException("Number of stated output elements, " + whichOutput 
                                + ", is different than the current output length, " + OutputLength);
                    }
                }
                if (!IsDataColumnDefinitionsDefined)
                {
                    if (CheckIfDataColumnsDefined())
                    {
                        UpdateDataColumnDefinitions();
                    }
                }
                if (OutputLevel >= 2)
                {
                    Console.WriteLine(Environment.NewLine
                        + DataColumnDefinition.ToStringList(DataColumnDefinitions, 
                        "Column data after reading data column input/output types: ") + Environment.NewLine);
                }
                // return false;
            }
        }  // ReadElementTypes()

        /// <summary>Reads types of elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementIndices()
        {
            lock (Lock)
            {
                int row; // = CurrentRow;
                int column; // = CurrentColumn + 1;
                FindFirstDataCell(out row, out column);
                if (column<0)
                    throw new InvalidDataException("Invalid data at " + GetPositionString(row, column) + ": there are no element indices."
                            + Environment.NewLine + "  Cell value: \"" + this[CurrentRow, CurrentColumn]);
                if (column >= 0) if (!IsInt(row, column))
                    throw new InvalidDataException("Invalid element index at " + GetPositionString(row, column) + ": should be an integer."
                            + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                if (column >= 0)
                {
                    CurrentRow = row;
                    CurrentColumn = column + 1;
                    int whichDataColumn = 0;
                    int whichInput = 0;
                    int whichOutput = 0;
                    while (column >= 0)
                    {
                        string cellValue = this[row, column];
                        //bool isInput = IsKeyElementTypeInput(cellValue);
                        //bool isOutput = IsKeyElementTypeOutput(cellValue);
                        bool isInputOutputDefined = true;
                        if (IsInt(row, column))
                        {
                            int val = GetInt(row, column);
                            if (OutputLevel >= 2)
                            {
                                Console.WriteLine("Element index No. " + whichDataColumn + ": " + val);
                            }
                            DataColumnDefinition def = GetDataColumnDefinition(whichDataColumn);
                            // If at least one column does not have input or output status defined, then we will assume
                            // that types are not defined yet and we will define them according to sequential numbers
                            // of columns:
                            if (!def.IsInputOutputDefined)
                                isInputOutputDefined = false;
                            bool isInput;
                            if (!isInputOutputDefined)
                            {
                                if (whichDataColumn < InputLength)
                                    isInput = true;
                                else
                                    isInput = false;
                            } else
                            {
                                isInput = def.IsInput;
                            }
                            def.IsInput = isInput;
                            def.ElementIndex = val;
                            ++whichDataColumn;
                            if (isInput)
                                ++whichInput;
                            else
                                ++whichOutput;
                        }
                        else
                        {
                            throw new InvalidDataException("Invalid element index at " + GetPositionString(row, column) + ": not an integer."
                                + Environment.NewLine + "  Cell value: \"" + this[row, column]);
                        }
                        column = FirstNonemptyColumn(row, column + 1);
                    }
                    if (InputLength < 0)
                        InputLength = whichInput;
                    else
                    {
                        if (whichInput > 0 && whichInput != InputLength)
                            throw new InvalidDataException("Number of stated input elements, " 
                                + whichInput + ", is different than the current input length, " + InputLength);
                    }
                    if (OutputLength < 0)
                        OutputLength = whichOutput;
                    else
                    {
                        if (whichOutput > 0 && whichOutput != OutputLength)
                            throw new InvalidDataException("Number of stated output elements, " + whichOutput 
                                + ", is different than the current output length, " + OutputLength);
                    }
                }
                if (!IsDataColumnDefinitionsDefined)
                {
                    if (CheckIfDataColumnsDefined())
                    {
                        UpdateDataColumnDefinitions();
                    }
                }
                if (OutputLevel >= 2)
                {
                    Console.WriteLine(Environment.NewLine
                        + DataColumnDefinition.ToStringList(DataColumnDefinitions, 
                        "Column data after reading data column input/output types: ") + Environment.NewLine);
                }
            }
        }  // ReadElementIndices()

        /// <summary>Reads string values from the CSV-like string table, from the current position on (inclusively),
        /// and stores them into the provided array.
        /// <para>Array is sorted in such a way that values corresponding to input elements are included
        /// first, followed by values corresponding to output elements, both sorted by element indices.</para>
        /// <para>After reading, current cell position is set to the first column after the last cell in the data.</para>
        /// <para>If element positions are not yet known then they are established (if possible, otherwise exception is thrown).</para></summary>
        /// <para>Exception is thrown if values are not at expected positions.</para>
        /// <param name="dataArray">Array where string values read from data columns are stored.</param>
        protected void ReadStringData(ref string[] dataArray)
        {
            lock(Lock)
            {
                // IsDataColumnDefinitionsDefined = false;
                if (!IsDataColumnDefinitionsDefined)
                    GetDataColumnDefinitionsFromData(false /* isDouble */, false /* isInt */);
                if (!IsDataColumnDefinitionsDefined)
                    throw new InvalidDataException("Could not obtain data column definitions form data at " + GetPositionString() + "."
                                + Environment.NewLine + "  Cell value: \"" + this[CurrentRow, CurrentColumn]);
                int row, column;
                FindFirstDataCell(out row, out column);  // just to find in which row data is listed; uses current position
                if (column < 0)
                    throw new InvalidDataException("Can not find first data cell for the key " + this[CurrentRow, CurrentColumn]
                        + " at " + GetPositionString() + ".");
                if (InputColumnDefinitions.Count != InputLength)
                    throw new InvalidDataException("Number of input column definitions " + InputColumnDefinitions.Count +
                        " is different than input dimension " + InputLength);
                if (OutputColumnDefinitions.Count != OutputLength)
                    throw new InvalidDataException("Number of output column definitions " + OutputColumnDefinitions.Count +
                        " is different than output dimension " + OutputLength);
                int numDataColumns = InputLength + OutputLength;
                if (numDataColumns != DataColumnDefinitions.Count)
                    throw new InvalidDataException("Sum of input and output dimensions " + (numDataColumns)
                        + " does not correspond to the number of data columns " + DataColumnDefinitions.Count + ".");
                bool resize = false;
                if (dataArray == null)
                    resize = true;
                else if (dataArray.Length != numDataColumns)
                    resize = true;
                if (resize)
                    dataArray = new string[numDataColumns];
                // Clear the array:
                for (int i = 0; i < numDataColumns; ++i)
                    dataArray[i] = null;
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    DataColumnDefinition def = InputColumnDefinitions[inputElementIndex];
                    if (def == null)
                        throw new InvalidDataException("Data column definition for input No. " + inputElementIndex
                            + " is not defined (null reference).");
                    if (def.ElementIndex != inputElementIndex)
                        throw new InvalidDataException("Data column definition for input No. " + inputElementIndex
                            + " has wrong element index: " + def.ElementIndex + ".");
                    dataArray[inputElementIndex] = this[row, def.Column];
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    DataColumnDefinition def = OutputColumnDefinitions[outputElementIndex];
                    if (def == null)
                        throw new InvalidDataException("Data column definition for output No. " + outputElementIndex
                            + " is not defined (null reference).");
                    if (def.ElementIndex != outputElementIndex)
                        throw new InvalidDataException("Data column definition for output No. " + outputElementIndex
                            + " has wrong element index: " + def.ElementIndex + ".");
                    dataArray[InputLength + outputElementIndex] = this[row, def.Column];
                }
                CurrentRow = row;
                CurrentColumn = DataColumnDefinitions[numDataColumns - 1].Column + 1;
            }
        }  // ReadStringData

        /// <summary>Reads double values from the CSV-like string table, from the current position on (inclusively),
        /// and stores them into the provided array.
        /// <para>Array is sorted in such a way that values corresponding to input elements are included
        /// first, followed by values corresponding to output elements, both sorted by element indices.</para>
        /// <para><see cref="double.NaN"/> is stored to positions for which data cells were empty.</para>
        /// <para>After reading, current cell position is set to the first column after the last cell in the data.</para>
        /// <para>If element positions are not yet known then they are established (if possible, otherwise exception is thrown).</para></summary>
        /// <para>Exception is thrown if values are not at expected positions.</para>
        /// <param name="dataArray">Array where string values read from data columns are stored.</param>
        protected void ReadDoubleData(ref double[] dataArray)
        {
            lock (Lock)
            {
                if (!IsDataColumnDefinitionsDefined)
                    GetDataColumnDefinitionsFromData(true /* isDouble */, false /* isInt */);
                if (!IsDataColumnDefinitionsDefined)
                    throw new InvalidDataException("Could not obtain data column definitions form data at " + GetPositionString() + "."
                                + Environment.NewLine + "  Cell value: \"" + this[CurrentRow, CurrentColumn]);
                int row, column;
                FindFirstDataCell(out row, out column);  // just to find in which row data is listed; uses current position
                if (column < 0)
                    throw new InvalidDataException("Can not find first data cell for the key " + this[CurrentRow, CurrentColumn]
                        + " at " + GetPositionString() + ".");
                if (InputColumnDefinitions.Count != InputLength)
                    throw new InvalidDataException("Number of input column definitions " + InputColumnDefinitions.Count +
                        " is different than input dimension " + InputLength);
                if (OutputColumnDefinitions.Count != OutputLength)
                    throw new InvalidDataException("Number of output column definitions " + OutputColumnDefinitions.Count +
                        " is different than output dimension " + OutputLength);
                int numDataColumns = InputLength + OutputLength;
                if (numDataColumns != DataColumnDefinitions.Count)
                    throw new InvalidDataException("Sum of input and output dimensions " + (numDataColumns)
                        + " does not correspond to the number of data columns " + DataColumnDefinitions.Count + ".");
                bool resize = false;
                if (dataArray == null)
                    resize = true;
                else if (dataArray.Length != numDataColumns)
                    resize = true;
                if (resize)
                    dataArray = new double[numDataColumns];
                // Clear the array:
                for (int i = 0; i < numDataColumns; ++i)
                    dataArray[i] = double.NaN;
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    DataColumnDefinition def = InputColumnDefinitions[inputElementIndex];
                    if (def == null)
                        throw new InvalidDataException("Data column definition for input No. " + inputElementIndex
                            + " is not defined (null reference).");
                    if (def.ElementIndex != inputElementIndex)
                        throw new InvalidDataException("Data column definition for input No. " + inputElementIndex
                            + " has wrong element index: " + def.ElementIndex + ".");
                    if (IsNotNullOrEmpty(row, def.Column))
                        dataArray[inputElementIndex] = GetDouble(row, def.Column);
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    DataColumnDefinition def = OutputColumnDefinitions[outputElementIndex];
                    if (def == null)
                        throw new InvalidDataException("Data column definition for output No. " + outputElementIndex
                            + " is not defined (null reference).");
                    if (def.ElementIndex != outputElementIndex)
                        throw new InvalidDataException("Data column definition for output No. " + outputElementIndex
                            + " has wrong element index: " + def.ElementIndex + ".");
                    if (IsNotNullOrEmpty(row, def.Column))
                        dataArray[InputLength + outputElementIndex] = GetDouble(row, def.Column);
                }
                CurrentRow = row;
                CurrentColumn = DataColumnDefinitions[numDataColumns - 1].Column + 1;
            }
        }  // ReadDoubleData

        /// <summary>Reads integer values from the CSV-like string table, from the current position on (inclusively),
        /// and stores them into the provided array.
        /// <para>Array is sorted in such a way that values corresponding to input elements are included
        /// first, followed by values corresponding to output elements, both sorted by element indices.</para>
        /// <para><see cref="int.MinValue"/> is stored to positions for which data cells were empty.</para>
        /// <para>After reading, current cell position is set to the first column after the last cell in the data.</para>
        /// <para>If element positions are not yet known then they are established (if possible, otherwise exception is thrown).</para></summary>
        /// <para>Exception is thrown if values are not at expected positions.</para>
        /// <param name="dataArray">Array where string values read from data columns are stored.</param>
        protected void ReadIntData(ref int[] dataArray)
        {
            lock (Lock)
            {
                if (!IsDataColumnDefinitionsDefined)
                    GetDataColumnDefinitionsFromData(false /* isDouble */, true /* isInt */);
                if (!IsDataColumnDefinitionsDefined)
                    throw new InvalidDataException("Could not obtain data column definitions form data at " + GetPositionString() + "."
                                + Environment.NewLine + "  Cell value: \"" + this[CurrentRow, CurrentColumn]);
                int row, column;
                FindFirstDataCell(out row, out column);  // just to find in which row data is listed; uses current position
                if (column < 0)
                    throw new InvalidDataException("Can not find first data cell for the key " + this[CurrentRow, CurrentColumn]
                        + " at " + GetPositionString() + ".");
                if (InputColumnDefinitions.Count != InputLength)
                    throw new InvalidDataException("Number of input column definitions " + InputColumnDefinitions.Count +
                        " is different than input dimension " + InputLength);
                if (OutputColumnDefinitions.Count != OutputLength)
                    throw new InvalidDataException("Number of output column definitions " + OutputColumnDefinitions.Count +
                        " is different than output dimension " + OutputLength);
                int numDataColumns = InputLength + OutputLength;
                if (numDataColumns != DataColumnDefinitions.Count)
                    throw new InvalidDataException("Sum of input and output dimensions " + (numDataColumns)
                        + " does not correspond to the number of data columns " + DataColumnDefinitions.Count + ".");
                bool resize = false;
                if (dataArray == null)
                    resize = true;
                else if (dataArray.Length != numDataColumns)
                    resize = true;
                if (resize)
                    dataArray = new int[numDataColumns];
                // Clear the array:
                for (int i = 0; i < numDataColumns; ++i)
                    dataArray[i] = int.MinValue;
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    DataColumnDefinition def = InputColumnDefinitions[inputElementIndex];
                    if (def == null)
                        throw new InvalidDataException("Data column definition for input No. " + inputElementIndex
                            + " is not defined (null reference).");
                    if (def.ElementIndex != inputElementIndex)
                        throw new InvalidDataException("Data column definition for input No. " + inputElementIndex
                            + " has wrong element index: " + def.ElementIndex + ".");
                    if (IsNotNullOrEmpty(row, def.Column))
                        dataArray[inputElementIndex] = GetInt(row, def.Column);
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    DataColumnDefinition def = OutputColumnDefinitions[outputElementIndex];
                    if (def == null)
                        throw new InvalidDataException("Data column definition for output No. " + outputElementIndex
                            + " is not defined (null reference).");
                    if (def.ElementIndex != outputElementIndex)
                        throw new InvalidDataException("Data column definition for output No. " + outputElementIndex
                            + " has wrong element index: " + def.ElementIndex + ".");
                    if (IsNotNullOrEmpty(row, def.Column))
                        dataArray[InputLength + outputElementIndex] = GetInt(row, def.Column);
                }
                CurrentRow = row;
                CurrentColumn = DataColumnDefinitions[numDataColumns - 1].Column + 1;
            }
        }  // ReadIntData


        /// <summary>Auxiliary array for temporary storage of strings.</summary>
        protected string[] AuxStringArray = null;

        /// <summary>Auxiliary array for temporary storage of double numbers.</summary>
        protected double[] AuxDoubleArray = null;

        /// <summary>Auxiliary array for temporary storage of integer numbers.</summary>
        protected int[] AuxIntArray = null;

        /// <summary>Reads names of input and output data elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementNames()
        {
            lock (Lock)
            {
                ReadStringData(ref AuxStringArray);
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    InputElementDefinition def = GetInputElementDefinition(inputElementIndex);
                    def.Name = AuxStringArray[inputElementIndex];
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    OutputElementDefinition def = GetOutputElementDefinition(outputElementIndex);
                    def.Name = AuxStringArray[InputLength + outputElementIndex];
                }
            }
        }

        /// <summary>Reads titles of input and output data elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementTitles()
        {
            lock (Lock)
            {
                ReadStringData(ref AuxStringArray);
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    InputElementDefinition def = GetInputElementDefinition(inputElementIndex);
                    def.Title = AuxStringArray[inputElementIndex];
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    OutputElementDefinition def = GetOutputElementDefinition(outputElementIndex);
                    def.Title = AuxStringArray[InputLength + outputElementIndex];
                }
            }
        }

        /// <summary>Reads descriptions of input and output data elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementDescriptions()
        {
            lock (Lock)
            {
                ReadStringData(ref AuxStringArray);
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    InputElementDefinition def = GetInputElementDefinition(inputElementIndex);
                    def.Description = AuxStringArray[inputElementIndex];
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    OutputElementDefinition def = GetOutputElementDefinition(outputElementIndex);
                    def.Description = AuxStringArray[InputLength + outputElementIndex];
                }
            }
        }  

        /// <summary>Reads minimal values of input and output data elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementMinimalValues()
        {
            lock (Lock)
            {
                ReadDoubleData(ref AuxDoubleArray);
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    InputElementDefinition def = GetInputElementDefinition(inputElementIndex);
                    def.MinimalValue = AuxDoubleArray[inputElementIndex];
                    if (!double.IsNaN(AuxDoubleArray[inputElementIndex]))
                    {
                        def.BoundsDefined = true;
                    } else
                    {
                        def.BoundsDefined = false;
                    }
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    OutputElementDefinition def = GetOutputElementDefinition(outputElementIndex);
                    def.MinimalValue = AuxDoubleArray[InputLength + outputElementIndex];
                    if (!double.IsNaN(AuxDoubleArray[InputLength + outputElementIndex]))
                    {
                        def.BoundsDefined = true;
                    } else
                    {
                        def.BoundsDefined = false;
                    }
                }
            }
        }  // ReadElementMinimalValues()

        /// <summary>Reads maximal values of input and output data elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementMaximalValues()
        {
            lock (Lock)
            {
                ReadDoubleData(ref AuxDoubleArray);
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    InputElementDefinition def = GetInputElementDefinition(inputElementIndex);
                    def.MaximalValue = AuxDoubleArray[inputElementIndex];
                    if (!double.IsNaN(AuxDoubleArray[inputElementIndex]))
                    {
                        def.BoundsDefined = true;
                    } else {
                        def.BoundsDefined = false;
                    }
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    OutputElementDefinition def = GetOutputElementDefinition(outputElementIndex);
                    def.MaximalValue = AuxDoubleArray[InputLength + outputElementIndex];
                    if (!double.IsNaN(AuxDoubleArray[InputLength + outputElementIndex]))
                    {
                        def.BoundsDefined = true;
                    } else 
                    {
                        def.BoundsDefined = false;
                    }
                }
            }
        }  // ReadElementMaximalValues()

        /// <summary>Reads scaling lengths of input and output data elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementScalingLengths()
        {
            lock (Lock)
            {
                ReadDoubleData(ref AuxDoubleArray);
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    InputElementDefinition def = GetInputElementDefinition(inputElementIndex);
                    def.ScalingLength = AuxDoubleArray[inputElementIndex];
                    if (!double.IsNaN(AuxDoubleArray[inputElementIndex]))
                    {
                        def.ScalingLengthDefined = true;
                    } else 
                    {
                        def.ScalingLengthDefined = false;
                    }
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    OutputElementDefinition def = GetOutputElementDefinition(outputElementIndex);
                    def.ScalingLength = AuxDoubleArray[InputLength + outputElementIndex];
                    if (double.IsNaN(AuxDoubleArray[InputLength + outputElementIndex]))
                    {
                        def.ScalingLengthDefined = true;
                    } else 
                    {
                        def.ScalingLengthDefined = false;
                    }
                }
            }
        }  // ReadElementScalingLengths()

        /// <summary>Reads default values of input and output data elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementDefaultValues()
        {
            lock (Lock)
            {
                ReadDoubleData(ref AuxDoubleArray);
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    InputElementDefinition def = GetInputElementDefinition(inputElementIndex);
                    def.DefaultValue = AuxDoubleArray[inputElementIndex];
                    if (!double.IsNaN(AuxDoubleArray[inputElementIndex]))
                    {
                        def.DefaultValueDefined = true;
                    } else 
                    {
                        def.DefaultValueDefined = false;
                    }
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    //if (AuxDoubleArray[InputLength + outputElementIndex] != double.NaN)
                    if (!double.IsNaN(AuxDoubleArray[InputLength + outputElementIndex])) 
                    {
                        throw new InvalidDataException("Invalid data at data column No. " + (InputLength + outputElementIndex) +
                            ": default value can not be defined for output elements.");
                    }
                }
            }
        }  // ReadElementDefaultValues()


        /// <summary>Reads discretization steps of input data elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementDiscretizationSteps()
        {
            lock (Lock)
            {
                ReadDoubleData(ref AuxDoubleArray);
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    InputElementDefinition def = GetInputElementDefinition(inputElementIndex);
                    if (!double.IsNaN(AuxDoubleArray[inputElementIndex]))
                    {
                        def.DiscretizationStep = AuxDoubleArray[inputElementIndex];
                    } else
                        def.DiscretizationStep = 0.0;
                }
                // Output elements do not hava DiscretizationStep step property.
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    //OutputElementDefinition def = GetOutputElementDefinition(outputElementIndex);
                    //def.DiscretizationStep = AuxDoubleArray[InputLength + outputElementIndex];
                    double val = AuxDoubleArray[InputLength + outputElementIndex];
                    if (!double.IsNaN(val) && val != 0.0)
                    {
                        throw new InvalidDataException("Invalid data at data column No. " + (InputLength + outputElementIndex) + 
                            ": Discretization step can not be defined for output elements.");
                        //def.DiscretizationStep = true;
                    }
                }
            }
        }  // ReadElementDiscretizationSteps()

        /// <summary>Reads target values of input and output data elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementTargetValues()
        {
            lock (Lock)
            {
                ReadDoubleData(ref AuxDoubleArray);
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    InputElementDefinition def = GetInputElementDefinition(inputElementIndex);
                    def.TargetValue = AuxDoubleArray[inputElementIndex];
                    if (! double.IsNaN(AuxDoubleArray[inputElementIndex]))
                    {
                        def.TargetValueDefined = true;
                    } else
                    {
                        def.TargetValueDefined = false;
                    }
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    OutputElementDefinition def = GetOutputElementDefinition(outputElementIndex);
                    def.TargetValue = AuxDoubleArray[InputLength + outputElementIndex];
                    if (!double.IsNaN(AuxDoubleArray[InputLength + outputElementIndex]))
                    {
                        def.TargetValueDefined = true;
                    } else
                    {
                        def.TargetValueDefined = false;
                    }
                }
            }
        }  // ReadElementTargetValues()

        /// <summary>Reads optimization indices of input and output data elements from the CSV, from the current position on (inclusively).</summary>
        /// <remarks>After successful execution, the current cell is set just after the last read position.</remarks>
        protected void ReadElementOptimizationIndices()
        {
            lock (Lock)
            {
                ReadIntData(ref AuxIntArray);
                for (int inputElementIndex = 0; inputElementIndex < InputLength; ++inputElementIndex)
                {
                    InputElementDefinition def = GetInputElementDefinition(inputElementIndex);
                    def.OptimizationIndex = AuxIntArray[inputElementIndex];
                    if (AuxIntArray[inputElementIndex] != int.MinValue)
                    {
                        def.OptimizationIndexSpecified = true;
                    } else
                    {
                        def.OptimizationIndexSpecified = false;
                    }
                }
                for (int outputElementIndex = 0; outputElementIndex < OutputLength; ++outputElementIndex)
                {
                    OutputElementDefinition def = GetOutputElementDefinition(outputElementIndex);
                    // def.OptimizationIndex = AuxIntArray[InputLength + outputElementIndex];
                    // Optimization indices can not be defined for output values:
                    if (AuxIntArray[InputLength + outputElementIndex] != int.MinValue)
                    {
                        throw new InvalidDataException("Invalid data at data column No. " + (InputLength + outputElementIndex) +
                            ": Optimization index can not be defined for output elements.");
                        //def.DiscretizationStep = true;
                    } 
                }
            }
        }  // ReadElementOptimizationIndices()



        // READ DATA:

        ///// <summary>Finds the first data cell for data inroduced by a key, provided that
        ///// the key that introduces the data is located at (<paramref name="keyRow"/>, <paramref name="keyColumn"/>).</summary>
        ///// <param name="keyRow">Row number of the cell containing the key that introduces the data.</param>
        ///// <param name="keyColumn">Column number of the cell containing the key that introduces the data.</param>
        ///// <param name="row">Variable where row number of the cell where data begins is stored.</param>
        ///// <param name="column">Variable where column number of the cell where data begins is stored.</param>
        ///// <exception cref="">When the first data cell could not be located.</exception>
        //protected void FindSampledData(int keyRow, int keyColumn, out int row, out int column)
        //{
        //    lock (Lock)
        //    {
        //        row = keyRow;
        //        column = keyColumn + 1;
        //        column = FirstNonemptyColumn(row, column);
        //        if (column < 0)
        //        {
        //            ++row;
        //            column = FirstNonemptyColumn(row, 0);
        //        }
        //        if (column < 0)
        //        {
        //            throw new InvalidDataException("At " + GetPositionString(keyRow, keyColumn) + ": data corresponding to the key not found."
        //                + Environment.NewLine + "  Key: \"" + this[keyRow, keyColumn]);
        //        }
        //    }
        //}

        ///// <summary>Finds the first cell containing the sampled data ftom the position 
        ///// (<see cref="CurrentRow"/>, <see cref="CurrentColumn"/>) on.</summary>
        ///// <param name="row">Variable where row number of the cell where data begins is stored.</param>
        ///// <param name="column">Variable where column number of the cell where data begins is stored.</param>
        ///// <exception cref="">When the first data cell could not be located.</exception>
        //protected void FindSampledData(out int row, out int column)
        //{
        //    FindSampledData(CurrentRow, CurrentColumn, out row, out column);
        //}
 


        #endregion Operation.Read.Aux


        #region Operation.Read

        
        /// <summary>Restores data definition from the data table.
        /// <para>Data definition is assigned to the <see cref="DataDefinition"/> property.</para></summary>
        /// <param name="resetPosition">Whether position is reset before the restoration begins.</param>
        public void RestoreDefinition(bool resetPosition)
        {
            if (resetPosition)
            {
                CurrentRow = 0;
                CurrentColumn = 0;
                InputLength = -1;
                OutputLength = -1;
            }
            ClearDataColumnDefinitions();
            DataDefinition = new InputOutputDataDefiniton();
            do
            {
                int column = FirstNonemptyColumn(CurrentRow, CurrentColumn);
                if (column >= 0)
                {
                    CurrentColumn = column;
                    if (IsKeyComment(this[CurrentRow, CurrentColumn]))
                    {
                        // Do nothing, just skip the current line.
                    }
                    else if (IsKeyNumInputParameters(this[CurrentRow, CurrentColumn]))
                    {
                        int intVal = 0;
                        bool wasRead = ReadSingleInt(ref intVal);
                        if (wasRead)
                        {
                            InputLength = intVal;
                            if (OutputLevel >= 1)
                                Console.WriteLine("Number of inputs after reading: " + InputLength);
                        }
                    } else if (IsKeyNumOutputValues(this[CurrentRow, CurrentColumn]))
                    {
                        int intVal = 0;
                        bool wasRead = ReadSingleInt(ref intVal);
                        if (wasRead)
                            OutputLength = intVal;
                            if (OutputLevel >= 1)
                                Console.WriteLine("Number of outputs after reading: " + InputLength);
                    } else if (IsKeyElementTypes(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementTypes();
                    } else if (IsKeyElementIndices(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementIndices();
                    }
                    else if (IsKeyNames(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementNames();
                    }
                    else if (IsKeyTitles(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementTitles();
                    }
                    else if (IsKeyDescriptions(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementDescriptions();
                    }
                    else if (IsKeyMinimalValues(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementMinimalValues();
                    }
                    else if (IsKeyMaximalValues(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementMaximalValues();
                    }
                    else if (IsKeyScalingLengths(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementScalingLengths();
                    }
                    else if (IsKeyDefaultValues(this[CurrentRow, CurrentColumn]))
                    {
                        if (OutputLevel >= 2)
                        {
                            Console.WriteLine(Environment.NewLine + "Current data definition before reading default values: " + Environment.NewLine
                                + DataDefinition.ToString() + Environment.NewLine );
                        }
                        ReadElementDefaultValues();
                    }
                    else if (IsKeyDiscretizationSteps(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementDiscretizationSteps();
                    }
                    else if (IsKeyTargetValues(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementTargetValues();
                    }
                    else if (IsKeyOptimizationIndices(this[CurrentRow, CurrentColumn]))
                    {
                        ReadElementOptimizationIndices();
                    }
                    else if (IsKeyData(this[CurrentRow, CurrentColumn]))
                    {
                        if (OutputLevel >= 2)
                            Console.WriteLine(Environment.NewLine + "Data key encountered at " + GetPositionString() + "." + Environment.NewLine);
                        // Simply stop processing:
                        break;
                    }
                    else
                    {
                        // Simply report unknown key and skip to the next line.
                        if (IsDefinitionOrDataKey(this[CurrentRow, CurrentColumn]))
                            Console.WriteLine("Unknown key encountered at " + GetPositionString() + ": \"" + this[CurrentRow, CurrentColumn] + "\".");
                        else
                            Console.WriteLine("Unexpected cell value at " + GetPositionString() + ": \"" + this[CurrentRow, CurrentColumn] + "\".");
                    }
                }
                ++CurrentRow;
                CurrentColumn = 0;
            } while (CurrentRow < NumRows);
        }  // RestoreDefinition()



        /// <summary>Restores data definition from the data table.
        /// Position is reset before the operation begins.
        /// <para>Data definition is assigned to the <see cref="DataDefinition"/> property.</para></summary>
        /// <param name="resetPosition">Whether position is reset before the restoration begins.</param>
        public void RestoreDefinition()
        {
            RestoreDefinition(true /* resetPosition */);
        }


        /// <summary>Restores sampled data from the data table, starting from the current position, i.e. (<see cref="CurrentRow"/>, <see cref="CurrentColumn"/>).
        /// <para>Data is assigned to the <see cref="SampledData"/> property.</para></summary>
        /// <param name="resetPosition">Whether position is reset before the restoration begins.</param>
        public void RestoreData(bool resetPosition /* resetPosition */)
        {
            lock (Lock)
            {
                if (OutputLevel >= 1)
                {
                    Console.WriteLine(Environment.NewLine + "Reading data..." );
                }
                if (resetPosition)
                {
                    CurrentRow = 0;
                    CurrentColumn = 0;
                }
                SampledDataSet sd = new SampledDataSet();
                if (InputLength >= 0)
                    sd.InputLength = InputLength;
                if (OutputLength >= 0)
                    sd.OutputLength = OutputLength;
                SampledData = sd;
                int row = CurrentRow;
                int column = CurrentColumn;
                bool dataKeyFound = false;  // whether the key introducing sampled data has been found
                bool dataFound = false;  // whether the first cell of the sampled data has been found
                bool stopReading = false;
                int whichDataLine = 0;
                do
                {
                    column = FirstNonemptyColumn(row, column);
                    if (column >= 0)
                    {
                        CurrentRow = row;
                        CurrentColumn = column;
                        string cellValue = this[row, column];
                        if (IsKeyComment(cellValue))
                        {
                            if (OutputLevel >= 1)
                            {
                                Console.WriteLine("Comment at row " + row);
                            }
                            // Comment: do nothing, just skip to the next row
                            ++row;
                            column = 0;
                        } if (IsDataKey(cellValue))
                        {
                            if (OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine + "Data key found at positon " 
                                    + GetPositionString() + ". " + Environment.NewLine);
                            }
                            // Next nonempty cell is the data key. Mark beginning of data:
                            dataKeyFound = true;
                            FindFirstDataCell(out row, out column);
                            if (row >= 0 && column >= 0)
                            {
                                // We have also found beginning of data (which may be in the same or the next row as the data key):
                                if (IsDouble(row, column))
                                    dataFound = true;
                            } else
                            {
                                // We will read the data from the next line:
                                row = CurrentRow + 1;
                                column = 0;
                            }
                        }
                        else if (IsDefinitionKey(cellValue))
                        {
                            if (OutputLevel >= 1)
                            {
                                Console.WriteLine("Definition key at positon "
                                    + GetPositionString() + ":  " + cellValue);
                            }
                            // Definition key has been found. If we already found the beginning of data then
                            // this will stop reading, otherwise we just skip the current data line:
                            if (dataKeyFound || dataFound)
                            {
                                // Stop reading the data and eventually enable other operations to proceed from here:
                                stopReading = true;
                            }
                            else
                            {
                                FindFirstDataCell(out row, out column);
                                if (column < 0)
                                    throw new InvalidDataException("Can not find first data cell for the key " + this[CurrentRow, CurrentColumn]
                                        + " at " + GetPositionString() + ".");
                                // Skip the current line:
                                ++row;
                                column = 0;
                            }
                        }
                        else if (IsDouble(row, column))
                        {
                            if (!dataFound)
                            {
                                dataFound = true;
                                // Infer information about data columns if not yet available
                                if (!IsDataColumnDefinitionsDefined)
                                    GetDataColumnDefinitionsFromData(false /* isDouble */, false /* isInt */);
                                if (!IsDataColumnDefinitionsDefined)
                                    throw new InvalidDataException("Could not obtain data column definitions form data at " + GetPositionString() + "."
                                                + Environment.NewLine + "  Cell value: \"" + this[CurrentRow, CurrentColumn]);
                            }
                            ReadDoubleData(ref AuxDoubleArray);
                            if (AuxDoubleArray.Length != (InputLength + OutputLength))
                            {
                                throw new InvalidDataException("Invalid data row at " + GetPositionString() + Environment.NewLine
                                    + "  Number of cells different than total number of inputs and outputs " + (InputLength + OutputLength) + ".");
                            }
                            IVector inputParameters = new Vector(InputLength);
                            IVector outputValues = new Vector(OutputLength);
                            for (int i = 0; i < InputLength; ++i)
                                inputParameters[i] = AuxDoubleArray[i];
                            for (int i = 0; i < OutputLength; ++i)
                                outputValues[i] = AuxDoubleArray[InputLength + i];
                            SampledData.Add(new SampledDataElement(inputParameters, outputValues));
                            if (OutputLevel >= 2)
                            {
                                Console.WriteLine(Environment.NewLine + "Data line No. " + whichDataLine + ": " + Environment.NewLine
                                    + "  Input:  " + inputParameters.ToString() + Environment.NewLine
                                    + "  Output: " + outputValues.ToString());
                            }
                            ++whichDataLine;
                            ++row;
                            column = 0;
                        } else
                        {
                            // The current cell does not contain any known information. 
                            // If this was before the data begun, throw exception, otherwise just stop reading.
                            if (dataKeyFound || dataFound)
                            {
                                if (OutputLevel >= 1)
                                {
                                    Console.WriteLine("Definition key at positon "
                                        + GetPositionString() + ":  " + cellValue);
                                }
                                // Stop reading the data and eventually enable other operations to proceed from here:
                                stopReading = true;
                            } else
                            {
                                throw new InvalidDataException("Unexpected kind of data at " + GetPositionString()
                                        + "  Cell value: " + this[CurrentRow, CurrentColumn] + ".");
                            }
                        }
                    }
                } while (row < NumRows && column >= 0 && !stopReading);
            }
            if (OutputLevel >= 1)
            {
                Console.WriteLine(Environment.NewLine + "... reading data finished." + Environment.NewLine);
            }

        }


        /// <summary>Restores sampled data definition from the data table.
        /// Position is reset before the operation begins.
        /// <para>Data is assigned to the <see cref="SampledData"/> property.</para></summary>
        /// <param name="resetPosition">Whether position is reset before the restoration begins.</param>
        public void RestoreData()
        {
            RestoreData(true /* resetPosition */);
        }


        /// <summary>Restores data definition AND sampled data definition from the data table.
        /// <para>Data definition is assigned to the <see cref="DataDefinition"/> property.</para>
        /// <para>Data is assigned to the <see cref="SampledData"/> property.</para></summary>
        /// <param name="resetPosition">Whether position is reset before the restoration begins.</param>
        public void RestoreDefinitionAndData(bool resetPosition)
        {
            if (resetPosition)
            {
                CurrentRow = 0;
                CurrentColumn = 0;
            }
            RestoreDefinition(false /* resetPosition */);
            RestoreData(false /* resetPosition */);
        }


        /// <summary>Restores data definition AND sampled data definition from the data table.
        /// Position is reset before the operation begins.
        /// <para>Data definition is assigned to the <see cref="DataDefinition"/> property.</para>
        /// <para>Data is assigned to the <see cref="SampledData"/> property.</para></summary>
        public void RestoreDefinitionAndData()
        {
            RestoreDefinitionAndData(true /* resetPosition */);
        }


        /// <summary>Loads data definition form the specified CSV file.
        /// <para>Data definition is assigned to the <see cref="DataDefinition"/> property.</para></summary>
        /// <param name="inputFilePath">Path to the file where data definition is read from.</param>
        public void LoadDefinition(string filePath)
        {
            this.LoadCsv(filePath);
            this.RestoreDefinition(true);
        }


        /// <summary>Loads sampled data form the specified CSV file.
        /// <para>Data is assigned to the <see cref="SampledData"/> property.</para></summary>
        /// <param name="inputFilePath">Path to the file where data definition is read from.</param>
        public void LoadData(string filePath)
        {
            this.LoadCsv(filePath);
            this.RestoreDefinition(true);
        }


        /// <summary>Loads data definition AND sampled data form the specified CSV file.
        /// <para>Data definition is assigned to the <see cref="DataDefinition"/> property.</para></summary>
        /// <para>Data is assigned to the <see cref="SampledData"/> property.</para></summary>
        /// <param name="inputFilePath">Path to the file where data definition is read from.</param>
        public void LoadDefinitionAndData(string filePath)
        {
            this.LoadCsv(filePath);
            this.RestoreDefinitionAndData(true);
        }

        #endregion Operation.Read


    }  // class SampledDataCsv


}
