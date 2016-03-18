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



    /// <summary>Base class for building scalar functions from scripts. This is currently an interrmediate
    /// class between the DTO and scalar function, and will probably be gradually replaced.</summary>
    /// <para>Beside acting as a kind of data transfer object, this class provides a variety of manipulations that are necessary
    /// when defining scalar functions from scripts (i.e., from symbolic user definitions dhat are compiled by the JIT compiler).</para>
    /// <typeparam name="ScalarFunctionType">Type parameter specifying the specific scalar function type for which concrete DTO
    /// is designed.</typeparam>
    /// $A Igor Feb16;
    public abstract class ScalarFunctionScriptControllerBase <ScalarFunctionType> // : SerializationDtoBase<ScalarFunctionType, IScalarFunction>
        where ScalarFunctionType : class, IScalarFunction
    {

        #region Construction

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public ScalarFunctionScriptControllerBase()
            : base()
        { }

        /// <summary>Constructor, prepares the current DTO for storing a scalar function of the specified dimension.</summary>
        /// <param name="spaceDimension">Number of parameters of the represented (script-constructed) scalar function.</param>
        public ScalarFunctionScriptControllerBase(int spaceDimension)
            : this()
        {
            this.Dimension = spaceDimension;
        }

        #endregion Construction
        

        #region Data

        private int _dimension;

        /// <summary>Dimension of the parameter space.</summary>
        public virtual int Dimension
        {
            get { return _dimension; }
            set
            {
                if (value != _dimension)
                {
                    this._dimension = value;
                    InvalidateDimension();
                }
            }
        }

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

        /// <summary>Specifies whether value is defined for the funciton represented by the current DTO.</summary>
        public virtual bool IsValueDefined
        {
            get { return (!string.IsNullOrEmpty(ValueDefinitonString)); }
        }

        /// <summary>Specifies whether gradient is defined for the funciton represented by the current DTO.</summary>
        public virtual bool IsGradientDefined
        {
            get { return (GradientDefinitionStrings != null && GradientDefinitionStrings.Length > 0); }
        }

        private string _valueString;

        /// <summary>Definition of function value.</summary>
        public virtual string ValueDefinitonString
        {
            get { return _valueString; }
            set {
                if (value != _valueString)
                {
                    InvalidateValueDefinition();
                    _valueString = value;
                }
            }
        }

        

        /// <summary>Converts an array of parameter names to a delimited single string that contains these names, and
        /// returns this string.</summary>
        /// <param name="parameterNames">array of parameter names.</param>
        public virtual string ConvertParameterNamesToString(params string[] parameterNames)
        {
            return CreateParameterNamesString(parameterNames, ParameterNamesSeparator, false /* insertNewlines */, true /* insertSpaces */);
        }

        /// <summary>Converts a single string containing parameter names in delimited form to an array of individual parameter 
        /// names, and returns this array.</summary>
        /// <param name="parameterNamesString"></param>
        public virtual string[] ConvertParameterNamesToArray(string parameterNamesString)
        {
            string[] ret = null;
            CreateParameterNamesArray(parameterNamesString, ParameterNamesSeparator, ref ret, true /* trimSpaces */);
            return ret;
        }

        /// <summary>Converts an array of function definition strings to a delimited single string that contains these defintions, and
        /// returns this string.</summary>
        /// <param name="functionDefinitions">array of function definitions.</param>
        public virtual string ConvertFunctionDefinitionsToString(params string[] functionDefinitions)
        {
            return CreateFunctionDefinitionsString(functionDefinitions, FunctionsSeparator, true /* insertNewlines */, false /* insertSpaces */);
        }

        /// <summary>Converts a single string containing function definitions in delimited form to an array of individual 
        /// function definition strings, and returns this array.</summary>
        /// <param name="functionDefinitionsString"></param>
        public virtual string[] ConvertFunctionDefinitionsToArray(string functionDefinitionsString)
        {
            string[] ret = null;
            CreateFunctionDefinitionsArray(functionDefinitionsString, FunctionsSeparator, ref ret, true /* trimSpaces */);
            return ret;
        }



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
            get {
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
                return _parameterNames; }
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
                        Dimension = value.Length;
                }
            }
        }
        


        /// <summary>Returns a string that contains delimited definitions of all gradient components.</summary>
        public string GetGradientDefinitionSingleString()
        {
            if (string.IsNullOrEmpty(_gradientsSingleString) && ParameterNames != null && ParameterNames.Length > 0)
            {
                _gradientsSingleString = ConvertFunctionDefinitionsToString(_gradientDefinitionStrings);
            }
            return _gradientsSingleString;
        }



        /// <summary>Sets definitions of components of function gradients by specifying a string that contains
        /// these definitions in delimited form (without any enclosing brackets).</summary>
        /// <param name="definitionsString">String that contains delimited definitions of elements of function gradient.</param>
        public void SetGradientDefinitionSingleString(string definitionsString)
        {
            if (definitionsString != this._gradientsSingleString)
            {
                InvalidateGradientDefinition();
                this.GradientDefinitionStrings = null;  // to cause invalidation
                this._gradientsSingleString = definitionsString;
            }
        }



        protected string _gradientsSingleString;

        private string[] _gradientDefinitionStrings;


        /// <summary>Gradient definition strings (separately for each gradient component).</summary>
        public virtual string[] GradientDefinitionStrings
        {
            get
            {
                bool undefined = true;
                if (_gradientDefinitionStrings != null)
                    if (_gradientDefinitionStrings.Length > 0)
                        undefined = false;
                if (undefined && !string.IsNullOrEmpty(_gradientsSingleString))
                {
                    // Lazy evaluation: if gradient definitions were set through a single delimited string then evaluation is 
                    // deferred until actually needed.
                    _gradientDefinitionStrings = ConvertFunctionDefinitionsToArray(_gradientsSingleString);
                }
                return _gradientDefinitionStrings;
            }
            set
            {
                string[] currentValue = GradientDefinitionStrings;  // to trigger eventual lazy evaluation
                if (!UtilStr.StringArraysEqual(value, currentValue))
                {
                    // Gradient component definition(s) changed, update dependencies:
                    InvalidateGradientDefinition();
                    _gradientsSingleString = null;
                    _gradientDefinitionStrings = value;
                    if (value != null)
                        Dimension = value.Length;
                }
            }
        }

        /// <summary>Sets a definition of the specified gradient components.</summary>
        /// <param name="whichComponent">Specifies which gradient component's definition is set.</param>
        /// <param name="gradientComponentDefinition">String containing definition of the specified gradient component.</param>
        public virtual void SetGradientComponentDefiniton(int whichComponent, string gradientComponentDefinition)
        {
            int dim = this.Dimension;
            if (whichComponent < 0 || whichComponent >= dim)
                throw new ArgumentException("Gradient component index (" + whichComponent + 
                    ") out of range, should be between 0 and " + (dim-1).ToString() + ".");
            string[] defStrings = GradientDefinitionStrings;
            bool allocate = true;
            bool invalidate = false;
            if (defStrings != null)
                if (defStrings.Length == dim)
                    allocate = false;
            if (allocate)
            {
                defStrings = new string[dim];
                GradientDefinitionStrings = defStrings;
                invalidate = true;
            }
            if (defStrings[whichComponent] != gradientComponentDefinition)
            {
                invalidate = true;
                defStrings[whichComponent] = gradientComponentDefinition;
            }
            if (invalidate)
            {
                _gradientsSingleString = null;
                InvalidateGradientDefinition();
            }
        }
        


        /// <summary>Invalidates dimension of function domain. All dependent data is invalidated.</summary>
        public virtual void InvalidateDimension()
        {
            InvalidateParameterNames();
            if (FunctionLoader != null)
                    FunctionLoader.InvalidateDefinitions();
        }

        /// <summary>Invalidates names of parameters. This also means that function value definition and function gradient 
        /// definitions are invalidated, if present.</summary>
        public virtual void InvalidateParameterNames()
        {
            InvalidateValueDefinition();
            InvalidateGradientDefinition();
            IsLoaderConsistent = false;
            if (FunctionLoader != null)
                FunctionLoader.InvalidateDefinitions();
        }


        /// <summary>Invalidates definition of function value.</summary>
        public virtual void InvalidateValueDefinition()
        {
            _valueString = null;
            IsLoaderConsistent = false;
            if (FunctionLoader != null)
                FunctionLoader.InvalidateDefinitions();
        }

        /// <summary>Invalidates definition of function gradient.</summary>
        public virtual void InvalidateGradientDefinition()
        {
            _gradientsSingleString = null;
            _gradientDefinitionStrings = null;
            IsLoaderConsistent = false;
            if (FunctionLoader != null)
                FunctionLoader.InvalidateDefinitions();
        }


        
        #endregion Data


        #region StaticUtilities

        // These utilities can be moved somewhere else, e.g. to the scriptiiing scalar function base class, or to scripting function creator...


        private static string _defaultParameterNamesSeparator = ",";

        /// <summary>Default parameter names separator, separates parametr names when stated in a single string.</summary>
        public static string DefaultParameterNamesSeparator
        {
            get { return _defaultParameterNamesSeparator; }
            protected set
            {
                if (value != _defaultParameterNamesSeparator)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException("Default parameter names separator not specified correctly (null or empty string).");
                    }
                    _defaultParameterNamesSeparator = value;
                }
            }
        }


        private static string _defaultFunctionsSeparator = ";";

        /// <summary>Default functions separator, separates string definitions of functions when stated in a single string.</summary>
        protected static string DefaultFunctionsSeparator
        {
            get { return _defaultFunctionsSeparator; }
            private set
            {
                if (value != _defaultFunctionsSeparator)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException("Default functions separator not specified correctly (null or empty string).");
                    }
                    _defaultFunctionsSeparator = value;
                }
            }
        }


        /// <summary>Parses a string that contains delimited parameter names, extract an array of individual parameter 
        /// names from it, and stores names to the specified array (which is reallocated if necessary).</summary>
        /// <param name="namesString">String that contains delimited parameterr names.</param>
        /// <param name="separator">String separators that is used to delimit parameter names.</param>
        /// <param name="namesArray">Reference to the array where extracted parameterr names are stored.</param>
        /// <param name="trimSpaces">If true then spaces and newlines are trimmed from parameter names. default is true.</param>
        public static void CreateParameterNamesArray(string namesString, string separator, ref string[] namesArray,
            bool trimSpaces = true)
        {
            CreateParameterNamesArray(namesString, new string[] { separator }, ref namesArray,
                trimSpaces, false /* allowMultipleSeparators */);
        }


        /// <summary>Parses a string that contains delimited parameter names, extract an array of individual parameter 
        /// names from it, and stores names to the specified array (which is reallocated if necessary).</summary>
        /// <param name="namesString">String that contains delimited parameterr names.</param>
        /// <param name="separators">A list of string separators that are used to delimit parameter names. Normally there
        /// will be onnly one separator (dependent on the <paramref name="allowMultipleSeparators"/>) parameter.</param>
        /// <param name="namesArray">Reference to the array where extracted parameterr names are stored.</param>
        /// <param name="trimSpaces">If true then spaces and newlines are trimmed from parameter names. default is true.</param>
        /// <param name="allowMultipleSeparators">If true then multiple separators are allowed. Default is false.</param>
        public static void CreateParameterNamesArray(string namesString, string[] separators, ref string[] namesArray,
            bool trimSpaces = true, bool allowMultipleSeparators = false)
        {
            int num = 0;
            if (string.IsNullOrEmpty(namesString))
            {
                if (namesArray != null)
                    if (namesArray.Length > 0)
                        namesArray = null;
            } else
            {
                string[] names = namesString.Split(separators, StringSplitOptions.None);
                for (int i = 0; i < names.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(names[i]))
                        names[i] = names[i].Trim();
                    if (!string.IsNullOrEmpty(names[i]))
                    {
                        names[num] = names[i];
                        ++num;
                    }
                }
                if (num < 1)
                {
                    if (namesArray != null)
                        if (namesArray.Length > 0)
                            namesArray = null;
                } else
                {
                    // There is at least one parameter name.
                    if (namesArray != null)
                    {
                        if (namesArray.Length != num)
                        {
                            namesArray = null;
                        }
                    }
                    if (namesArray == null)
                    {
                        namesArray = new string[num];
                    }
                    for (int i = 0; i < num; ++i)
                    {
                        namesArray[i] = names[i];
                    }
                }
            }
        }


        /// <summary>Parses a string that contains delimited function definitions, extract an array of individual function 
        /// definitions from it, and stores individual string definitions to the specified array (which is reallocated if necessary).</summary>
        /// <param name="namesString">String that contains delimited function definitions.</param>
        /// <param name="separator">String separators that is used to delimit function definitions.</param>
        /// <param name="namesArray">Reference to the array where extracted function definition strings are stored.</param>
        /// <param name="trimSpaces">If true then spaces and newlines are trimmed from function definition strings. Default is true.</param>
        public static void CreateFunctionDefinitionsArray(string namesString, string separator, ref string[] namesArray,
            bool trimSpaces = true)
        {
            CreateFunctionDefinitionsArray(namesString, new string[] { separator }, ref namesArray,
                trimSpaces, false /* allowMultipleSeparators */);
        }


        /// <summary>Parses a string that contains delimited parameter names, extract an array of individual parameter 
        /// names from it, and stores names to the specified array (which is reallocated if necessary).</summary>
        /// <remarks>Task is currently delegated to <see cref="CreateFunctionDefinitionsArray(string, string[], ref string[], bool, bool)"/>.</remarks>
        /// <param name="namesString">String that contains delimited parameterr names.</param>
        /// <param name="separators">A list of string separators that are used to delimit parameter names. Normally there
        /// will be onnly one separator (dependent on the <paramref name="allowMultipleSeparators"/>) parameter.</param>
        /// <param name="namesArray">Reference to the array where extracted parameterr names are stored.</param>
        /// <param name="trimSpaces">If true then spaces and newlines are trimmed from parameter names. default is true.</param>
        /// <param name="allowMultipleSeparators">If true then multiple separators are allowed. Default is false.</param>
        public static void CreateFunctionDefinitionsArray(string namesString, string[] separators, ref string[] namesArray,
            bool trimSpaces = true, bool allowMultipleSeparators = false)
        {
            CreateParameterNamesArray(namesString, separators, ref namesArray,
                trimSpaces, allowMultipleSeparators);
        }


        /// <summary>Constructs a delimited string cotaining parameter names, from the specified array of parameter names.</summary>
        /// <param name="namesArray">Array of parameter names.</param>
        /// <param name="separator">Separator used to delimit parameter names.</param>
        /// <param name="insertNewLines">If true then newlines are also inserted between between individual parameter names (after separators).</param>
        /// <param name="insertSpaces">If true then spaces are also inserted between individual parameter names (after separators).</param>
        /// <returns>A single delimited string that contains the specified parameter names, without any enclosing backets.</returns>
        public static string CreateParameterNamesString(string[] namesArray, string separator, bool insertNewLines = false, bool insertSpaces = true)
        {
            if (namesArray == null)
                return null;
            if (namesArray.Length < 1)
                return null;
            StringBuilder sb = new StringBuilder();
            int num = namesArray.Length;
            for (int i = 0; i < num; ++i)
            {
                sb.Append(namesArray[i]);
                if (i < num - 1)
                {
                    sb.Append(separator);
                    if (insertSpaces)
                        sb.Append(" ");
                    if (insertNewLines)
                        sb.AppendLine();
                }
            }
            return sb.ToString();
        }


        /// <summary>Construct a delimited string cotaining function definitons, from the specified array of individual string function definitions.</summary>
        /// <remarks>Work is delegated to <see cref="CreateParameterNamesString(string [], string, bool, bool)"/>.</remarks>
        /// <param name="definitionsArray">Array of parameter names.</param>
        /// <param name="separator">Separator used to delimit function definitions.</param>
        /// <param name="insertNewLines">If true then newlines are also inserted between between individual function definitions (after separators).</param>
        /// <param name="insertSpaces">If true then spaces are also inserted between individual function definitions (after separators).</param>
        /// <returns>A single delimited string that contains the specified function definitions, without any enclosing backets.</returns>
        public static string CreateFunctionDefinitionsString(string[] definitionsArray, string separator, bool insertNewLines = false, bool insertSpaces = true)
        {
            return CreateParameterNamesString(definitionsArray, separator, insertNewLines, insertSpaces);
        }




        private string _parameterNamesSeparator = DefaultParameterNamesSeparator;

        /// <summary>Separator string that is used to separate parameter names when stated in a single string.
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

        private string _functionsSeparator = DefaultFunctionsSeparator;

        /// <summary>Separator string that is used to separate function definitions when stated in a single state.</summary>
        public string FunctionsSeparator
        {
            get { return _functionsSeparator; }
            private set
            {
                if (value != _functionsSeparator)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException("Parameter names separator not specified correctly (null or empty string).");
                    }
                    _functionsSeparator = value;
                }
            }
        }


        /// <summary>Generates and returns an array of a specified number of variable or parameter names, composed of a base name and
        /// numerical string corresponding to the consecutive number of the variable.</summary>
        /// <param name="dimension">Number of variables.</param>
        /// <param name="baseName">Base for the parameter name.</param>
        /// <param name="startIndex">At which numbers parameterr count starts (default 0).</param>
        /// <param name="numPlaces">Number of places reserved for parameter's consecutive number (i.e. 2 in the sequence
        /// "x01", "x02", "x03", "x04" ...).</param>
        public static string[] GenerateParameterOrVariableNames(int dimension, string baseName = "x", int startIndex = 0, int numPlaces = 2)
        {
            if (string.IsNullOrEmpty(baseName))
                throw new ArgumentException("Parameters' base name is not defined (null or empty string).");
            if (dimension < 0)
                throw new ArgumentException("Dimension should be greater or equal than 0. Specified: " + dimension);
            if (startIndex < 0)
                throw new ArgumentException("Starting index for parameter names must be greater ot equal to zero.");
            string[] parameterNames = new string[dimension];
            string placesString = new string('0', numPlaces);
            for (int i = 0; i < dimension; ++i)
            {
                parameterNames[i] = GenerateFuctonParameterName(i, baseName, startIndex, numPlaces);
            }
            return parameterNames;
        }


        /// <summary>Generates and returns a pre-defined name of the specified variable or function parameter , composed of the specified
        ///  base name and numerical string corresponding to the consecutive number of the variable.</summary>
        /// <param name="whichParameter">Index of the parameter or consecutive number of tje variable for which name is generated.</param>
        /// <param name="baseName">Base for the  name.</param>
        /// <param name="startIndex">At which numbers parameterr count starts (default 0).</param>
        /// <param name="numPlaces">Number of places reserved for parameter's consecutive number (i.e. 2 in the sequence
        /// "x01", "x02", "x03", "x04" ...).</param>
        public static string GenerateFuctonParameterName(int whichParameter, string baseName = "x", int startIndex = 0, int numPlaces = 2)
        {
            string parameterName = null;
            if (string.IsNullOrEmpty(baseName))
                throw new ArgumentException("Parameters' base name is not defined (null or empty string).");
            if (startIndex < 0)
                throw new ArgumentException("Starting index for parameter names must be greater ot equal to zero.");
            if (numPlaces <= 1)
                parameterName = baseName + (startIndex + whichParameter).ToString();
            else
            {
                string placesString = new string('0', numPlaces);
                parameterName = baseName + (startIndex + whichParameter).ToString(placesString);
            }
            return parameterName;
        }


        #endregion StaticUtilities
        

        #region FunctionLoader


        private bool _isLoaderConsistent = false;

        /// <summary>A flag indicateing whether the loader is currently consistent with the function definition contained 
        /// in the current DTO.</summary>
        protected bool IsLoaderConsistent
        {
            get { return _isLoaderConsistent; }
            set { _isLoaderConsistent = value; }
        }

        /// <summary>Returns a flag telling whether the loader is currently consistent with function definition
        /// contained in the DTO.</summary>
        public bool GetIsLoaderConsistent()
        { return IsLoaderConsistent; }

        private ScalarFunctionLoader _functionLoader = null;


        /// <summary>Copies function deffinition data from the current function DTO to the internal function loader.</summary>
        protected void CopyDataToFunctionLoader()
        {
            CopyDataToFunctionLoader(FunctionLoader);
            IsLoaderConsistent = true;
        }

        /// <summary>Copies function deffinition data from the current function DTO to the specified function loader.</summary>
        /// <param name="loader">Function loader where data is xopied to.</param>
        protected void CopyDataToFunctionLoader(ScalarFunctionLoader loader)
        {
            if (loader == null)
                throw new ArgumentException("Functon loader to copy data to is not specified (null reference).");
            loader.IndependentVariableNames = ParameterNames;
            loader.ValueDefinitionString = ValueDefinitonString;
            if (this.IsGradientDefined)
            {
                loader.GradientDefinitionStrings = GradientDefinitionStrings;
            }
            else
            {
                loader.GradientDefinitionStrings = null;
            }
        }

        /// <summary>Object that is responsible for creation of scalar function objects that correspond to the 
        /// definitions found on the current scalar function DTO.</summary>
        protected virtual ScalarFunctionLoader FunctionLoader
        {
            get
            {
                if (_functionLoader == null)
                {
                    _functionLoader = new ScalarFunctionLoader();
                    CopyDataToFunctionLoader();
                }
                return _functionLoader;
            }
            set
            {
                if (!object.ReferenceEquals(value, _functionLoader))
                {
                    _functionLoader = value;
                    IsLoaderConsistent = false;
                }
            }
        }

        /// <summary>Returns the loader that can create a scalar function according to the definition from the current
        /// DTO.</summary>
        public virtual ScalarFunctionLoader GetFunctionLoader()
        {
            return FunctionLoader;
        }

        /// <summary>Sets the loader that can create a scalar function according to the definition from the current
        /// DTO.</summary>
        /// <param name="loader">Function loader to be set.</param>
        public virtual void SetFunctionLoader(ScalarFunctionLoader loader)
        {
            FunctionLoader = loader;
        }


        /// <summary>Returns a scalar function that is created from the funciton definitio on the
        /// current function DTO.</summary>
        public LoadableScalarFunctionBase GetFunction()
        { return this.Function; }


        /// <summary>Scalar function (of type <see cref="IScalarFunction"/>) created on basis of definition in this DTO.</summary>
        protected LoadableScalarFunctionBase Function
        {
            get
            {
                if (!IsLoaderConsistent)
                {
                    DataToFunctionLoader();
                }
                return FunctionLoader.Function;
            }
        }

        /// <summary>Copies data of the form to function loader.</summary>
        public void DataToFunctionLoader()
        {
            FunctionLoader.IndependentVariableNames = ParameterNames;
            FunctionLoader.ValueDefinitionString = ValueDefinitonString;
            if (IsGradientDefined)
            {
                FunctionLoader.GradientDefinitionStrings = GradientDefinitionStrings;
            }
            else
            {
                FunctionLoader.GradientDefinitionStrings = null;
            }
            IsLoaderConsistent = true;
            //FunctionLoader.
        }


        #endregion FunctionLoader


        


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
            for (int i = 0; i < Dimension; ++i)
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
            for (int i = 0; i < 1; ++i)
            {
                OutputElementDefinition outputElement = new OutputElementDefinition(i, null, null);
                outputElement.Name = outputElement.Title = Name;
                outputElement.Description = "Scalar function \"" + Name + "\".";
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
                        if (! (outputBounds.IsMinDefined(i) || outputBounds.IsMinDefined(i)) )
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


        /// <summary>Returns a string representation of the specified <see cref="ScalarFunctionScriptController"/> object.</summary>
        /// <param name="sc">Vector whose string representation is returned.</param>
        public static string ToString(ScalarFunctionScriptControllerBase<ScalarFunctionType> sc)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Function name: " + sc.Name);
            sb.AppendLine("Function description: \"" + sc.Description + "\"");
            sb.AppendLine("Space dimension: " + sc.Dimension);
            sb.AppendLine("Parameters: " + sc.GetParametersString());
            sb.AppendLine("Function definition: " + sc.ValueDefinitonString);
            if (sc.IsGradientDefined)
            {
                sb.AppendLine("Function gradient is defined. Gradients: ");
                sb.AppendLine(sc.GetGradientDefinitionSingleString());
            }
            else
            {
                sb.AppendLine("Gradients are NOT defined.");
                if (!string.IsNullOrEmpty(sc.GetGradientDefinitionSingleString()))
                    sb.AppendLine("  Gradient definition string is still defined: \"" + sc.GetGradientDefinitionSingleString() + "\"");
            }
            return sb.ToString();
        }



        /// <summary>Saves (serializes) the specified script-based scalar function controller to the specified JSON file.
        /// If the file already exists, contents either overwrites the file or is appended at the end, 
        /// dependent on the value of the append flag.
        /// <para>File is overwritten if it already exists.</para>
        /// <para>The data definition is also written to the file apart to function data, and can be later restored to a 
        /// separate object if necessary.</para></summary>
        /// <param name="functionController">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file in which object is saved.</param>
        /// <param name="dataDefinition">Optional object containing the definition of functions's input/output data. If specified,
        /// this object is added on the DTO annd is saved to the file together with pure function definitiion.</param>
        public static void SaveJson(ScalarFunctionScriptController functionController, string filePath,
            InputOutputDataDefiniton dataDefinition)
        {
            SaveJson(functionController, filePath, false /* append */, dataDefinition);
        }

        /// <summary>Saves (serializes) the specified script-based scalar function controller to the specified JSON file.
        /// If the file already exists, contents either overwrites the file or is appended at the end, 
        /// dependent on the value of the append flag.
        /// <para>In addition to function data, the data definition is also stored to the output file when specified.</para></summary>
        /// <param name="functionController">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file in which object is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        /// <param name="dataDefinition">Optional object containing the definition of functions's input/output data. If specified,
        /// this object is added on the DTO annd is saved to the file together with pure function definitiion.</param>
        public static void SaveJson(ScalarFunctionScriptController functionController, string filePath, bool append = false,
            InputOutputDataDefiniton dataDefinition = null)
        {
            ScalarFunctionScriptDto dtoOriginal = new ScalarFunctionScriptDto();
            dtoOriginal.CopyFrom(functionController);
            if (dataDefinition != null)
            {
                InputOutputDataDefinitonDto dataDefDto = new InputOutputDataDefinitonDto();
                dataDefDto.CopyFrom(dataDefinition);
                dtoOriginal.ZDataDefinition = dataDefDto;
            }
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<ScalarFunctionScriptDto>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) a script-based scalar function controller from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which object is restored.</param>
        /// <param name="controllerRestored">Object that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref ScalarFunctionScriptController controllerRestored)
        {
            ScalarFunctionScriptDto dto = null;
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
        public static void LoadJson(string filePath, ref ScalarFunctionScriptController controllerRestored, ref ScalarFunctionScriptDto dto)
        {
            ISerializer serializer = new SerializerJson();
            ScalarFunctionScriptDto dtoRestored = serializer.DeserializeFile<ScalarFunctionScriptDto>(filePath);
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
        /// in the file (which is possible due to the ability of <see cref="ScalarFunctionScriptDto"/> to incorporate a data definition
        /// object, in addition to pure function data).</param>
        public static void LoadJson(string filePath, ref ScalarFunctionScriptController controllerRestored, ref InputOutputDataDefiniton dataDef)
        {
            ScalarFunctionScriptDto dto = null;
            InputOutputDataDefinitonDto dataDto = null;
            LoadJson(filePath, ref controllerRestored, ref dto);
            if (dto!= null)
                dataDto = dto.ZDataDefinition;
            if (dataDto != null)
                dataDto.CopyTo(ref dataDef);
            else
                dataDef = null;
        }


        /// <summary>Saves the specified vector to a CSV file.
        /// It the specified file already exists then it is overwritten.
        /// Constant <see cref="UtilCsv.DefaultCsvSeparator"/> is used as separator.</summary>
        /// <param name="vec">Vector to be stored to a file.</param>
        /// <param name="filePath">Path of the file to which vector is stored.</param>
        public static void SaveCsv(IVector vec, string filePath)
        {
            SaveCsv(vec, filePath, UtilStr.DefaultCsvSeparator /* separator */, false /* append */);
        }

        /// <summary>Saves the specified vector to a CSV file.
        /// It the specified file already exists then it is overwritten.</summary>
        /// <param name="vec">Vector to be stored to a file.</param>
        /// <param name="filePath">Path of the file to which vector is stored.</param>
        /// <param name="separator">Separator used in the CSV file.</param>
        public static void SaveCsv(IVector vec, string filePath, string separator)
        {
            SaveCsv(vec, filePath, separator, false /* append */);
        }

        /// <summary>Saves the specified vector to a CSV file.
        /// Constant <see cref="UtilCsv.DefaultCsvSeparator"/> is used as separator in CSV.</summary>
        /// <param name="vec">Vector to be stored to a file.</param>
        /// <param name="filePath">Path of the file to which vector is stored.</param>
        /// <param name="append">Specifies whether the data is appended at the end of the file
        /// in the case that the ifle already exists.</param>
        public static void SaveCsv(IVector vec, string filePath, bool append)
        {
            SaveCsv(vec, filePath, UtilStr.DefaultCsvSeparator, append);
        }

        /// <summary>Saves the specified vector to a CSV file.</summary>
        /// <param name="vec">Vector to be stored to a file.</param>
        /// <param name="filePath">Path of the file to which vector is stored.</param>
        /// <param name="separator">Separator used in the CSV file.</param>
        /// <param name="append">Specifies whether the data is appended at the end of the file
        /// in the case that the ifle already exists.</param>
        public static void SaveCsv(IVector vec, string filePath, string separator, bool append)
        {
            throw new NotImplementedException("Saving user defined scalar function state to CSV is not yet supported.");

            //int dimension = 0;
            //if (vec != null)
            //    dimension = vec.Length;
            //if (dimension < 0)
            //    dimension = 0;
            //string[][] values = new string[1][];
            //values[0] = new string[dimension];
            //for (int i = 0; i < dimension; ++i)
            //    values[0][i] = vec[i].ToString();
            //UtilStr.SaveCsv(filePath, values, separator, append);
        }

        /// <summary>Reads a vector from a CSV file.
        /// Constant <see cref="UtilCsv.DefaultCsvSeparator"/> is used as separator in CSV file.
        /// If there are no components then a null vector is returned by this method (no exceptions thrown).
        /// If there are more than one rows in the CSV file then vector is read from the first row.</summary>
        /// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        /// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        public static void LoadCsv(string filePath, ref IVector vecRestored)
        {
            LoadCsv(filePath, UtilStr.DefaultCsvSeparator, ref vecRestored);
        }

        /// <summary>Reads a vector written in CSV format from a file.
        /// If there are no components then a null vector is returned by this method (no exceptions thrown).
        /// If there are more than one rows in the CSV file then vector is read from the first row.</summary>
        /// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        /// <param name="separator">Separator that is used to separate values in a row in the CSV file.</param>
        /// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        public static void LoadCsv(string filePath, string separator, ref IVector vecRestored)
        {
            throw new NotImplementedException("Reading user defined scalar function state from CSV is not yet supported.");

            //string[][] values = null;
            //values = UtilStr.LoadCsv(filePath, separator);
            //if (values == null)
            //    vecRestored = null;
            //else if (values.Length == 0)
            //    vecRestored = null;
            //else
            //{
            //    int dimension = values[0].Length;
            //    Vector.Resize(ref vecRestored, dimension);
            //    for (int i = 0; i < dimension; ++i)
            //    {
            //        double comp = 0;
            //        bool readCorrectly = double.TryParse(values[0][i], out comp);
            //        if (readCorrectly)
            //            vecRestored[i] = comp;
            //        else
            //        {
            //            throw new FormatException("Vector coponenet No. " + i + " in a CSV file is not a number. "
            //                + Environment.NewLine + "  Component representation in the file: " + values[0][i]);
            //        }
            //    }
            //}

        }


        /// <summary>Reads a vector from the specified row of a CSV file.
        /// Constant <see cref="UtilCsv.DefaultCsvSeparator"/> is used as CSV separator.
        /// If the specified row does not exisist in the file then exception is thrown.</summary>
        /// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        /// <param name="rowNum">Number of the row from which the vector is read.</param>
        /// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        public static void LoadCsv(string filePath, int rowNum, ref IVector vecRestored)
        {
            LoadCsv(filePath, rowNum, UtilStr.DefaultCsvSeparator /* separator */, ref vecRestored);
        }

        /// <summary>Reads a vector from the specified row of a CSV file.
        /// If the specified row does not exisist in the file then exception is thrown.</summary>
        /// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        /// <param name="rowNum">Number of the row from which the vector is read.</param>
        /// <param name="separator">Separator that is used to separate values in a row in the CSV file.</param>
        /// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        public static void LoadCsv(string filePath, int rowNum, string separator, ref IVector vecRestored)
        {
            throw new NotImplementedException("Reading user defined scalar function state from CSV is not yet supported.");

            //string[][] values = null;
            //values = UtilStr.LoadCsv(filePath, separator);
            //if (values == null)
            //    throw new FormatException("CSV file has no rows, can not read vector from row " + rowNum + ".");
            //else if (values.Length < rowNum + 1)
            //    throw new FormatException("CSV file has only " + values.Length + " rows, can not read vector from row " + rowNum + ".");
            //else
            //{
            //    int dimension = values[rowNum].Length;
            //    Vector.Resize(ref vecRestored, dimension);
            //    for (int i = 0; i < dimension; ++i)
            //    {
            //        int comp = 0;
            //        bool readCorrectly = int.TryParse(values[rowNum][i], out comp);
            //        if (readCorrectly)
            //            vecRestored[i] = comp;
            //        else
            //        {
            //            throw new FormatException("Vector coponenet No. " + i + ", element (" + rowNum +
            //                "," + i + ") of a CSV file, is not a number. "
            //                + Environment.NewLine + "  Component representation in the file: " + values[rowNum][i]);
            //        }
            //    }
            //}
        }


        #endregion StaticInputOutput




    }  // abstract class ScalarFunctionDtoBase<ScalarFunctionType>


    /// <summary>Class for building scalar functions from scripts, used by GUI elements.</summary>
    /// $A Igor Feb16;
    public class ScalarFunctionScriptController : ScalarFunctionScriptControllerBase<IScalarFunction>
    {

        #region Construction

        /// <summary>Creates a DTO for storing state of a scalar function object of any type</summary>
        public ScalarFunctionScriptController()
            : base()
        { }

        /// <summary>Creates a DTO for storing a scalar function object of any vector type, with specified dimension.</summary>
        /// <param name="length">Vector dimension.</param>
        public ScalarFunctionScriptController(int length)
            : base(length)
        { }

        #endregion Construction


        ///// <summary>Creates and returns a new scalar function.</summary>
        ///// <param name="length">Vector dimension.</param>
        //public override IScalarFunction CreateScalarFunction(int length)
        //{
        //throw new NotImplementedException();
        //    // return new IScalarFunction(length);
        //}


    } // class ScalarFunctionScripController






}