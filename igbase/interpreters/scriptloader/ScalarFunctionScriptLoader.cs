// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// DYNAMIC LOADER FOR USER DEFINED REAL FUNCTIONS.

using System;
using System.Collections;
using System.Text;
using System.IO;

using IG.Lib;
using IG.Num;


namespace IG.Lib
{


    /// <summary>Dynamically generates IScalarFunction classes from user definitions.
    /// User can define in string form how function, its derivative, second derivative, integral, 
    /// or inverse function is calculated. Then this class can be used to compile these definitions
    /// in the wrapping script, which is then used to create the corresponding function objects.</summary>
    /// $A Igor Jun10;
    public class ScalarFunctionLoader: ILockable
    {

        #region construction

        /// <summary>Constructor.</summary>
        public ScalarFunctionLoader()
        { }

        #endregion construction

        #region ILockable

        private object _mainLock = new object();

        /// <summary>Current object's central lock object.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ILockable


        #region Constants

        /// <summary>Default name of the class used in loadable scripts containing user definitions of scalar function class.</summary>
        public const string DefaultScriptClassName = "ScalarFunctionScript";

        /// <summary>Name of the function that initializes variables realated to dynamically loaded scalar function class.</summary>
        public const string FuncNameInitDynamic = "InitDynamic";

        /// <summary>Name of variable (internal in class defined in loadable script) that holds the number of parameters.</summary>
        public const string VarNameNumParameters = "_numParam";

        /// <summary>Name of variable (internal in class defined in loadable script) that holds the name 
        /// of local variable (defined within functions in the dynamically loaded script) where 
        /// returned value is stored.</summary>
        public const string VarNameReturnedValueName = "_returnedValueName";

        /// <summary>Default name of the variable that holds returned value in script functions.</summary>
        public const string DefaultReturnedValueName = "ret";

        /// <summary>Name of variable (internal in class defined in loadable script) that holds the names 
        /// of function argument (defined within functions in the dynamically loaded script) through which 
        /// independent variable is passed.</summary>
        public const string VarNameFunctionArgumentParametersName = "_functionArgumentParametersName";

        /// <summary>Default name of vector of parameters in function arguments in script functions.</summary>
        public const string DefaultFunctionArgumentParametersName = "parameters";

        public const string VarNameFunctionArgumentGradientName = "_functionArgumentGradientName";

        /// <summary>Default name of gradient vector in function arguments in script functions.</summary>
        public const string DefaultFunctionArgumentGradientName = "gradient";

        public const string VarNameFunctionArgumentHessianName = "_functionArgumentHessianName";

        /// <summary>Default name of hessian vector in function arguments in script functions.</summary>
        public const string DefaultFunctionArgumentHessianName = "hessian";

        /// <summary>Default base name of the independent variables (components of parameter vector) in expression 
        /// definitions used in scripts to define calculation of function values, derivatives, etc.</summary>
        public const string DefaultIndependentVariableBaseName = "x";

        public const string VarNameIndependentVariableNames = "_independentVariableNames";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds user 
        /// definition (as string) of function value.</summary>
        public const string VarNameValueDefinitionString = "_valueDefinitionString";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds user 
        /// definition (as string) of function derivative.</summary>
        public const string VarNameGradientDefinitionStrings = "_gradientDefinitionStrings";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds user 
        /// definition (as string) of function second derivative.</summary>
        public const string VarNameHessianDefinitionStrings = "_hessianDefinitionStrings";

        /// <summary>Name of the function (internal in class defined in loadable script) that defines 
        /// calculation of function value.</summary>
        public const string FuncNameValueDefinition = "ReferenceValue";

        /// <summary>Name of the function (internal in class defined in loadable script) that defines 
        /// calculation of function gradient.</summary>
        public const string FuncNameGradientDefinition = "ReferenceGradientPlain";

        /// <summary>Name of the function (internal in class defined in loadable script) that defines 
        /// calculation of function hessian.</summary>
        public const string FuncNameHessianDefinition = "ReferenceHessianPlain";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds a flag 
        /// telling whether calculation of function value is implemented.</summary>
        public const string VarNameValueDefined = "_valueDefined";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds a flag 
        /// telling whether calculation of function gradient is implemented.</summary>
        public const string VarNameGradientDefined = "_gradientDefined";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds a flag 
        /// telling whether calculation of function hessian is implemented.</summary>
        public const string VarNameHessianDefined = "_hessianDefined";

        #endregion Constants


        #region Data

        /// <summary>Whether calculation of function value is defined.</summary>
        public bool ValueDefined
        { get { lock (Lock) { return !string.IsNullOrEmpty(ValueDefinitionString); } } }

        /// <summary>Whether calculation of function gradient is defined.</summary>
        public bool GradientDefined
        { get { lock (Lock) { return GradientDefinitionStrings!=null; } } }

        /// <summary>Whether calculation of function Hessian is defined.</summary>
        public bool HessianDefined
        { get { lock (Lock) { return HessianDefinitionStrings!=null; } } }



        private string
            _returnedValueName = DefaultReturnedValueName,
            _functionArgumentParametersName = DefaultFunctionArgumentParametersName,
            _functionArgumentGradientName = DefaultFunctionArgumentGradientName,
            _functionArgumentHessianName = DefaultFunctionArgumentHessianName,
            _independentVariableBaseName = DefaultIndependentVariableBaseName,
            _valueDefinitionString,
            _code;

        private int _numParameters;

        private string[] _independentVariableNames = null;
        
        private string [] _gradientDefinitionStrings = null;
        private string[][] _hessianDefinitionStrings = null;


        /// <summary>Clears strings that define the function (i.e. strings that define expressions for
        /// function value, derivative, integral, inverse, etc.).
        /// Number of parameters is not reset.</summary>
        public void InvalidateDefinitions()
        {
            ValueDefinitionString = null;
            GradientDefinitionStrings = null;
            HessianDefinitionStrings = null;
        }


        /// <summary>Sets names used in generated script code.</summary>
        /// <param name="returnedValueName">Name of the variabble that holds returned value in functions in generated script code.</param>
        /// <param name="FunctionArgumentParametersName">Name of function argument in generated script code.</param>
        /// <param name="independentVariableNames">Names of independent variable in generated script code.</param>
        public void SetNames(string returnedValueName, string functionArgumentName, string[] independentVariableNames)
        {
            this.ReturnedValueName = returnedValueName;
            this.FunctionArgumentParametersName = functionArgumentName;
            this.IndependentVariableNames = independentVariableNames;
        }

        /// <summary>Name of variable (within functions in the loadable scripts) that temporarily holds
        /// the returned value(s). Set to constant <see cref="DefaultReturnedValueName"/> by default.</summary>
        public string ReturnedValueName
        {
            get
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(_returnedValueName))
                        ReturnedValueName = DefaultReturnedValueName;
                    return _returnedValueName;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value == FunctionArgumentParametersName)
                            throw new ArgumentException("Variable that holds returned value can not have the same name as parameters function argument. Name: "
                                + value);
                        if (IndependentVariableNames != null)
                        {
                            for (int i = 0; i < IndependentVariableNames.Length; ++i)
                            {
                                if (value == IndependentVariableNames[i])
                                {
                                    throw new ArgumentException("Variable that holds returned value can not have the same name as independent variable No. "
                                        + i + ". Name: + " + value + ".");
                                }
                            }
                        }
                    }
                    _returnedValueName = value;
                    // Invalidate dependencied:
                    InvalidateDefinitions();
                }
            }
        }

        /// <summary>Name of parameters vector in function arguments (in functions in the loadable scripts). 
        /// Set to constant <see cref="DefaultFunctionArgumentParametersName"/> by default.</summary>
        public string FunctionArgumentParametersName
        {
            get
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(_functionArgumentParametersName))
                        FunctionArgumentParametersName = DefaultFunctionArgumentParametersName;
                    return _functionArgumentParametersName;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value == ReturnedValueName)
                            throw new ArgumentException("Parameters function argument can not have the same name as variable that holds return value. Name: "
                                + value + ".");
                        if (IndependentVariableNames != null)
                        {
                            for (int i = 0; i < IndependentVariableNames.Length; ++i)
                            {
                                if (value == IndependentVariableNames[i])
                                {
                                    throw new ArgumentException("Parameters function argument can not have the same name as independent variable No. "
                                        + i + ". Name: + " + value + ".");
                                }
                            }
                        }
                    }
                    _functionArgumentParametersName = value;
                    // Invalidate dependencied:
                    InvalidateDefinitions();
                }
            }
        }

        /// <summary>Name of gradient vector in function arguments (in functions in the loadable scripts). 
        /// Set to constant <see cref="DefaultFunctionArgumentGradientName"/> by default.</summary>
        public string FunctionArgumentGradientName
        {
            get
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(_functionArgumentGradientName))
                        FunctionArgumentGradientName = DefaultFunctionArgumentGradientName;
                    return _functionArgumentGradientName;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value == ReturnedValueName)
                            throw new ArgumentException("Gradient function argument can not have the same name as variable that holds return value. Name: "
                                + value + ".");
                        if (IndependentVariableNames != null)
                        {
                            for (int i = 0; i < IndependentVariableNames.Length; ++i)
                            {
                                if (value == IndependentVariableNames[i])
                                {
                                    throw new ArgumentException("Gradient function argument can not have the same name as independent variable No. "
                                        + i + ". Name: + " + value + ".");
                                }
                            }
                        }
                    }
                    _functionArgumentGradientName = value;
                    // Invalidate dependencied:
                    InvalidateDefinitions();
                }
            }
        }


        /// <summary>Name of Hessian matrix in function arguments (in functions in the loadable scripts). 
        /// Set to constant <see cref="DefaultFunctionArgumentHessianName"/> by default.</summary>
        public string FunctionArgumentHessianName
        {
            get
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(_functionArgumentHessianName))
                        FunctionArgumentHessianName = DefaultFunctionArgumentHessianName;
                    return _functionArgumentHessianName;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value == ReturnedValueName)
                            throw new ArgumentException("Hessian function argument can not have the same name as variable that holds return value. Name: "
                                + value + ".");
                        if (IndependentVariableNames != null)
                        {
                            for (int i = 0; i < IndependentVariableNames.Length; ++i)
                            {
                                if (value == IndependentVariableNames[i])
                                {
                                    throw new ArgumentException("Hessian function argument can not have the same name as independent variable No. "
                                        + i + ". Name: + " + value + ".");
                                }
                            }
                        }
                    }
                    _functionArgumentHessianName = value;
                    // Invalidate dependencied:
                    InvalidateDefinitions();
                }
            }
        }

        /// <summary>Number of parameters of the scalar function that will be defined by a generated script
        /// - dimension of space in which the scalar function is defined.</summary>
        public int NumParameters
        {
            get
            {
                lock (Lock)
                {
                    if (_numParameters <= 0)
                    {
                        if (_independentVariableNames != null)
                            _numParameters = _independentVariableNames.Length;
                    }
                    return _numParameters;
                }
            }
            set
            {
                lock (Lock)
                {
                    _numParameters = value;
                    if (value > 0 && _independentVariableNames != null)
                    {
                        if (_independentVariableNames.Length != value)
                            IndependentVariableNames = null;
                    }
                    
                }
            }
        }

        /// <summary>Base name from which names of individual components of independent variables 
        /// (names of parameters used within fucntion definitions in scripts) are derived in the
        /// case that these names are not defined.</summary>
        public string IndependentVariableBaseName
        {
            get
            {
                lock (Lock)
                {
                    return _independentVariableBaseName;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("Independent variable base name not specified (null or empty string).");
                    _independentVariableBaseName = value;
                }
            }
        }

        /// <summary>Returns a table of default names of variables (within functions in the loadable scripts) that hold
        /// the independent variables (components of vector of parameters), for the specified number of parameters.</summary>
        /// <param name="numParam">Number of parameters.</param>
        public string[] GetDefaultIndependentVariableNames(int numParam)
        {
            if (numParam <= 0)
                return null;
            if (string.IsNullOrEmpty(DefaultIndependentVariableBaseName))
            {
                throw new InvalidOperationException("Can not generate default independent variable names: base name not defined (null or empty string).");
            }
            string[] ret = new string[numParam];
            for (int i=0; i<numParam; ++i)
                ret[i] = DefaultIndependentVariableBaseName + i.ToString();
            return ret;
        }

        /// <summary>Names of variables (within functions in the loadable scripts) that hold
        /// the independent variables (components of vector of parameters).</summary>
        public string[] IndependentVariableNames
        {
            get
            {
                lock (Lock)
                {
                    if (_independentVariableNames==null && _numParameters>0)
                        IndependentVariableNames = GetDefaultIndependentVariableNames(_numParameters);
                    return _independentVariableNames;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (value != null)
                    {
                        if (_numParameters != value.Length)
                            _numParameters = 0;
                        if (value.Length > 0)
                        {
                            for (int i = 0; i < value.Length; ++i)
                            {
                                if (value[i] != null)
                                {
                                    if (value[i] == ReturnedValueName)
                                        throw new ArgumentException("Independent variable No. " + i + " can not have the same name as variable that holds return value. Name: "
                                            + value[i]);
                                    if (value[i] == FunctionArgumentParametersName)
                                        throw new ArgumentException("Independent variable No. " + i + " can not have the same name as function argument. Name: "
                                            + value[i]);
                                }
                            }
                        }
                    }
                    _independentVariableNames = value;
                    // Invalidate dependencied:
                    InvalidateDefinitions();
                }
            }
        }

        /// <summary>Expression that defines function value.</summary>
        public string ValueDefinitionString
        {
            get  { lock (Lock) { return _valueDefinitionString; } }
            set { lock (Lock) { _valueDefinitionString = value; Code = null; } }
        }

        /// <summary>Expressions that defines function gradient components.</summary>
        public string[] GradientDefinitionStrings
        {
            get  { lock (Lock) { return _gradientDefinitionStrings; } }
            set { lock (Lock) { _gradientDefinitionStrings = value; Code = null; } }
        }

        /// <summary>Expressions that define function hessian components.</summary>
        public string[][] HessianDefinitionStrings
        {
            get { lock (Lock) { return _hessianDefinitionStrings; } }
            set { lock (Lock) { _hessianDefinitionStrings = value; Code = null; } }
        }


        /// <summary>Generated script code.</summary>
        public string Code
        {
            get { 
                lock (Lock) { 
                if (_code == null) { _code = GetCode(); }
                    return _code;
                }
            }
            protected set
            {
                lock (Lock) 
                { 
                    _code = value; 
                    IsCompiled = false; 
                }
            }
        }

        /// <summary>Saves the generated script code to the specified file.
        /// File is overwritten if it already exists.</summary>
        /// <param name="inputFilePath">Path to the file where script code is saved.</param>
        public void SaveCode(string filePath)
        {
            File.WriteAllText(filePath, Code);
        }


        /// <summary>Appends to the apecified string builder the specified level of indentation.</summary>
        /// <param name="sb">String builder to which indents are appended.</param>
        /// <param name="numIndente">Number of indents that are appended.</param>
        protected void AppendIndents(StringBuilder sb, int numIndent)
        {
            for (int i = 0; i < numIndent; ++i)
                sb.Append("\t");
        }

        /// <summary>Appends beginning of comment.</summary>
        /// <param name="sb">String buider that is used to assemble the compiled script.</param>
        protected void AppendBeginComment(StringBuilder sb)
        {
            sb.Append(" /* ");
        }

        /// <summary>Appends end of comment.</summary>
        /// <param name="sb">String buider that is used to assemble the compiled script.</param>
        protected void AppendEndcomment(StringBuilder sb)
        {
            sb.Append(" */");
        }

        /// <summary>Appends comment that denotes the componenent index.</summary>
        /// <param name="sb">String buider that is used to assemble the compiled script.</param>
        /// <param name="coponentBaseName">Component's base name used in the comment denoting indices.</param>
        /// <param name="index2">Second component index.</param>
        protected void AppendIndexComment(StringBuilder sb, string coponentBaseName, int index)
        {
            AppendBeginComment(sb);
            sb.Append(coponentBaseName + "_" + index.ToString());
            AppendEndcomment(sb);
        }

        /// <summary>Appends comment that denotes the componenent index.</summary>
        /// <param name="sb">String buider that is used to assemble the compiled script.</param>
        /// <param name="coponentBaseName">Component's base name used in the comment denoting indices.</param>
        /// <param name="index1">First component index.</param>
        /// <param name="index2">Second component index.</param>
        protected void AppendIndexComment(StringBuilder sb, string coponentBaseName, int index1, int index2)
        {
            AppendBeginComment(sb);
            sb.Append(coponentBaseName + "_" + index1.ToString() + "_" + index2.ToString());
            AppendEndcomment(sb);
        }

        /// <summary>Appends to the apecified string builder the C# statements that sets the specified variable to the specified value.</summary>
        /// <param name="sb">String builder to which the statement is appended.</param>
        /// <param name="varName">Name of the variable that is set.</param>
        /// <param name="value">Value that is assigned to the variable.</param>
        /// <param name="numIndents">Number of indents that are written before code lines.</param>
        protected void AppendSetVariable(StringBuilder sb, string varName, object value, int numIndents)
        {
            if (sb == null)
                throw new ArgumentNullException("String builder object is not specified (null reference).");
            if (string.IsNullOrEmpty(varName))
                throw new ArgumentException("Variable name not specified (null or empty string).");
            for (int i = 0; i < numIndents; ++i)
                sb.Append("\t");
            sb.Append(varName + " = ");
            if (value == null)
                sb.AppendLine("null;");
            else
            {
                if (value is string)
                    sb.Append("\"");
                if (value is bool)
                    sb.Append(value.ToString().ToLower());
                else
                    sb.Append(value.ToString());
                if (value is string)
                    sb.Append("\"");
                sb.AppendLine(";");
            }
        }

        /// <summary>Appends to the apecified string builder the C# statements that sets the specified variable to the specified value.</summary>
        /// <param name="sb">String builder to which the statement is appended.</param>
        /// <param name="varName">Name of the variable that is set.</param>
        /// <param name="value">Value that is assigned to the variable.</param>
        /// <param name="numIndents">Number of indents that are written before code lines.</param>
        protected void AppendSetVariable(StringBuilder sb, string varName, string [] values, string coponentBaseName, int numIndents)
        {
            if (sb == null)
                throw new ArgumentNullException("String builder object is not specified (null reference).");
            if (string.IsNullOrEmpty(varName))
                throw new ArgumentException("Variable name not specified (null or empty string).");
            AppendIndents(sb, numIndents);
            sb.Append(varName + " = ");
            if (values == null)
                sb.AppendLine("null;");
            else
            {
                sb.AppendLine("new string[] {");
                for (int i = 0; i < values.Length; ++i)
                {
                    AppendIndents(sb, numIndents + 1);
                    sb.Append("\"" + values[i] + "\"");
                    if (!string.IsNullOrEmpty(coponentBaseName))
                        AppendIndexComment(sb, coponentBaseName, i);
                    if (i < values.Length - 1)
                        sb.Append(", ");
                    sb.AppendLine();
                }
                AppendIndents(sb, numIndents);
                sb.AppendLine("}; ");
            }
        }

        /// <summary>Appends to the apecified string builder the C# statements that sets the specified variable to the specified value.</summary>
        /// <param name="sb">String builder to which the statement is appended.</param>
        /// <param name="varName">Name of the variable that is set.</param>
        /// <param name="value">Value that is assigned to the variable.</param>
        /// <param name="numIndents">Number of indents that are written before code lines.</param>
        protected void AppendSetVariable(StringBuilder sb, string varName, string [][] values, string componentBaseName, int numIndents)
        {
            if (sb == null)
                throw new ArgumentNullException("String builder object is not specified (null reference).");
            if (string.IsNullOrEmpty(varName))
                throw new ArgumentException("Variable name not specified (null or empty string).");
            AppendIndents(sb, numIndents);
            sb.Append(varName + " = ");
            if (values == null)
                sb.AppendLine("null;");
            else
            {
                sb.AppendLine("new string[][] {");
                for (int i = 0; i < values.Length; ++i)
                {
                    AppendIndents(sb, numIndents + 1);
                    if (values[i] == null)
                        sb.Append("null");
                    else
                    {
                        sb.Append("new string [] {");
                        if (!string.IsNullOrEmpty(componentBaseName))
                            AppendIndexComment(sb, componentBaseName, i);
                        sb.AppendLine();
                        for (int j = 0; j < values[i].Length; ++j)
                        {
                            AppendIndents(sb, numIndents + 2);
                            if (values[i][j] == null)
                                sb.Append("null");
                            else
                                sb.Append("\"" + values[i][j] + "\"");
                            if (!string.IsNullOrEmpty(componentBaseName))
                                AppendIndexComment(sb, componentBaseName, i, j);
                            if (j < values[i].Length - 1)
                                sb.Append(", ");
                            sb.AppendLine();
                        }
                        AppendIndents(sb, numIndents + 1);
                        sb.Append("}");
                    }
                    if (i < values.Length - 1)
                        sb.Append(", ");
                    sb.AppendLine();
                }
                AppendIndents(sb, numIndents);
                sb.AppendLine("}; ");
            }
        }


        /// <summary>Appends to the apecified string builder the C# definition of a function of parameters returning double.
        /// Function is of form 'protected override double (double arg)'.</summary>
        /// <param name="sb">String builder to which the code (function definition) is appended.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="definitionString">Expression that specifies how the returned value is calculated.</param>
        /// <param name="numIndents">Number of indents that are prepended before code lines.</param>
        private void AppendFunctonDefinition(StringBuilder sb, string functionName, string definitionString, int numIndents)
        {
            if (!string.IsNullOrEmpty(definitionString))
            {
                AppendIndents(sb, numIndents);
                sb.AppendLine("public override double " + functionName + "(IVector " + this.FunctionArgumentParametersName + ")");
                AppendIndents(sb, numIndents);
                sb.AppendLine("{");
                for (int i = 0; i < NumParameters; ++i)
                {
                    AppendIndents(sb, numIndents + 1);
                    sb.AppendLine("double " + IndependentVariableNames[i] + " = " + FunctionArgumentParametersName + "[" + i + "]" + ";");
                }
                AppendIndents(sb, numIndents + 1);
                // sb.AppendLine("xdouble " + ReturnedValueName + ";");
                sb.AppendLine("double " + ReturnedValueName + ";");
                AppendIndents(sb, numIndents + 1);
                sb.AppendLine(ReturnedValueName + " = zero + " + definitionString + ";");
                AppendIndents(sb, numIndents + 1);
                sb.AppendLine("return (double) " + ReturnedValueName + ";");
                AppendIndents(sb, numIndents);
                sb.AppendLine("}" + Environment.NewLine);
            }
        }

        /// <summary>Appends to the apecified string builder the C# definition of a function of parameters returning a vector
        /// throufh <see cref="IVector"/> argument.
        /// Function is of form 'protected override double (IVector param, IVector result)'.</summary>
        /// <param name="sb">String builder to which the code (function definition) is appended.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="definitionStrings">Expressions that specify how components of the returned vector are calculated.</param>
        /// <param name="returnedVectorName">Name of the formal vector argument that contains returned values.</param>
        /// <param name="numIndents">Number of indents that are prepended before code lines.</param>
        private void AppendFunctonDefinition(StringBuilder sb, string functionName, string[] definitionStrings, 
            string returnedVectorName, int numIndents)
        {
            if (definitionStrings!=null)
            {
                AppendIndents(sb, numIndents);
                sb.AppendLine("public override void " + functionName + "(IVector " + this.FunctionArgumentParametersName
                    + ", IVector " + returnedVectorName + ")");
                AppendIndents(sb, numIndents);
                sb.AppendLine("{");
                for (int i = 0; i < NumParameters; ++i)
                {
                    AppendIndents(sb, numIndents + 1);
                    sb.AppendLine("double " + IndependentVariableNames[i] + " = " + FunctionArgumentParametersName + "[" + i + "]" + ";");
                }
                AppendIndents(sb, numIndents + 1);
                // sb.AppendLine("xdouble " + ReturnedValueName + ";");
                sb.AppendLine("double " + ReturnedValueName + ";");
                for (int i = 0; i < NumParameters; ++i)
                {
                    AppendIndents(sb, numIndents + 1);
                    sb.AppendLine(ReturnedValueName + " = zero + " + definitionStrings[i] + ";");
                    AppendIndents(sb, numIndents + 1);
                    sb.AppendLine(returnedVectorName + "[" + i + "] = (double) " + ReturnedValueName + ";");
                }
                AppendIndents(sb, numIndents);
                sb.AppendLine("}" + Environment.NewLine);
            }
        }

        /// <summary>Returns the specific definition string out of a 2D jagged array of definitions, or null
        /// if the specified definition is not contained in the array of definitions.</summary>
        /// <param name="definitions">Definitions arranged in a 2D jagged array.</param>
        /// <param name="ind1">First index.</param>
        /// <param name="ind2">Second index.</param>
        public string getDefinitionString(string[][] definitions, int ind1, int ind2)
        {
            string ret = null;
            if (definitions != null)
                if (definitions.Length > ind1)
                    if (definitions[ind1] != null)
                        if (definitions[ind1].Length > ind2)
                            ret = definitions[ind1][ind2];
            return ret;
        }

        /// <summary>Appends to the apecified string builder the C# definition of a function of parameters returning a vector
        /// throufh <see cref="IVector"/> argument.
        /// Function is of form 'protected override double (IVector param, IVector result)'.</summary>
        /// <param name="sb">String builder to which the code (function definition) is appended.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="definitionStrings">Expressions that specify how components of the returned vector are calculated.</param>
        /// <param name="returnedVectorName">Name of the formal vector argument that contains returned values.</param>
        /// <param name="numIndents">Number of indents that are prepended before code lines.</param>
        private void AppendFunctonDefinition(StringBuilder sb, string functionName, string[][] definitionStrings, 
            string returnedMatrixName, int numIndents)
        {
            if (definitionStrings != null)
            {
                AppendIndents(sb, numIndents);
                sb.AppendLine("public override void " + functionName + "(IVector " + this.FunctionArgumentParametersName +
                    ", IMatrix " + returnedMatrixName + ")");
                AppendIndents(sb, numIndents);
                sb.AppendLine("{");
                for (int i = 0; i < NumParameters; ++i)
                {
                    AppendIndents(sb, numIndents + 1);
                    sb.AppendLine("double " + IndependentVariableNames[i] + " = " + FunctionArgumentParametersName + "[" + i + "]" + ";");
                }
                AppendIndents(sb, numIndents + 1);
                // sb.AppendLine("xdouble " + ReturnedValueName + ";");
                sb.AppendLine("double " + ReturnedValueName + ";");
                for (int i = 0; i < NumParameters; ++i)
                {
                    for (int j = 0; j < NumParameters; ++j)
                    {
                        string defStr = null;
                        defStr = getDefinitionString(definitionStrings, i, j);
                        if (string.IsNullOrEmpty(defStr))
                            defStr = getDefinitionString(definitionStrings, j, i);
                        if (string.IsNullOrEmpty(defStr))
                            throw new InvalidDataException("Function's Hessian component [" + i + ", " + j + "] is not defined.");
                        AppendIndents(sb, numIndents + 1);
                        sb.AppendLine(ReturnedValueName + " = zero + " + defStr + ";");
                        AppendIndents(sb, numIndents + 1);
                        sb.AppendLine(returnedMatrixName + "[" + i + ", " + j + "] = (double) " + ReturnedValueName + ";");
                    }
                }
                AppendIndents(sb, numIndents);
                sb.AppendLine("}" + Environment.NewLine);
            }
        }

        /// <summary>Returns a string representation of the list of independent variables, 
        /// separated by commas but not embedded in any braces.</summary>
        public string GetParametersPlainListString()
        {
            return UtilStr.GetParametersStringPlain(IndependentVariableNames);
            //StringBuilder sb = new StringBuilder();
            //if (IndependentVariableNames != null)
            //{
            //    for (int i = 0; i < IndependentVariableNames.Length; ++i)
            //    {
            //        sb.Append(IndependentVariableNames[i]);
            //        if (i < IndependentVariableNames.Length - 1)
            //            sb.Append(", ");
            //    }
            //}
            //return sb.ToString();
        }

        /// <summary>Generates and returns script code for dynamically loadable function definition.</summary>
        protected virtual string GetCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"

using System;

using IG.Lib;
using IG.Num;


namespace IG.Script
{

    /// <summary>Example script for definition of a new real function class.</summary>
    public class ScritpScalarFunctionExample : LoadableScriptScalarFunctionBase, ILoadableScript
    {

        /// <summary>Creates  and returns a new scalar function object where functions are defined by compiled user defined strings.</summary>
        public override LoadableScalarFunctionBase CreateScalarFunction()
        {
            return new Container.Function();
        }

        /// <summary>Creates  and returns a new scalar function object where functions are defined by compiled user defined strings,
        /// and with affine transformation of parameters.</summary>
        /// <param name=""transf"">Affine transformation used to transform function parameters from the reference to the actual frame.</param>
        public override LoadableScalarFunctionBase CreateScalarFunction(IAffineTransformation transf)
        {
            return new Container.Function(transf);
        }

        /// <summary>Container class inherits from M in order to enable use of comfortable mathematical functions.</summary>
        public class Container : M
        {

            public class Function : LoadableScalarFunctionBase
            {

                #region Construction
                public Function() : base() { }
                public Function(IAffineTransformation transf): base(transf) { }
                #endregion construction

                #region Generated


");


            int numIndents = 5;
            AppendIndents(sb, numIndents);
            sb.AppendLine("protected override void " + FuncNameInitDynamic + "() ");
            AppendIndents(sb, numIndents);
            sb.AppendLine("{");

            // Set auxiliary internal variables that are used in functions of the generated class:
            AppendSetVariable(sb, VarNameNumParameters, NumParameters, numIndents);
            AppendSetVariable(sb, VarNameIndependentVariableNames, IndependentVariableNames, "param", numIndents);
            AppendSetVariable(sb, VarNameReturnedValueName, ReturnedValueName, numIndents);
            AppendSetVariable(sb, VarNameFunctionArgumentParametersName, FunctionArgumentParametersName, numIndents);
            AppendSetVariable(sb, VarNameFunctionArgumentGradientName, FunctionArgumentGradientName, numIndents);
            AppendSetVariable(sb, VarNameFunctionArgumentHessianName, FunctionArgumentHessianName, numIndents);

            AppendSetVariable(sb, VarNameValueDefinitionString, ValueDefinitionString, numIndents);
            AppendSetVariable(sb, VarNameGradientDefinitionStrings, GradientDefinitionStrings, "grad", numIndents);
            AppendSetVariable(sb, VarNameHessianDefinitionStrings, HessianDefinitionStrings, "hessian", numIndents);

            AppendSetVariable(sb, VarNameValueDefined, ValueDefined, numIndents);
            AppendSetVariable(sb, VarNameGradientDefined, GradientDefined, numIndents);
            AppendSetVariable(sb, VarNameHessianDefined, HessianDefined, numIndents);
            sb.AppendLine(@"
                }
");

            AppendFunctonDefinition(sb, FuncNameValueDefinition, ValueDefinitionString, numIndents);

            AppendFunctonDefinition(sb, FuncNameGradientDefinition, GradientDefinitionStrings, 
                FunctionArgumentGradientName, numIndents);

            AppendFunctonDefinition(sb, FuncNameHessianDefinition, HessianDefinitionStrings, 
                FunctionArgumentHessianName, numIndents);

            sb.AppendLine(@"

                #endregion Generated

            }  // class Container.Function

        }  // class Container

    }  // class ScritpScalarFunction

}

");
            return sb.ToString();
        }

        ScriptLoaderBase _loader;

        /// <summary>Script loader used to load and instantiate real function class generated from script.</summary>
        /// $A Igor Jun10 Aug10;
        public ScriptLoaderBase Loader
        {
            get
            {
                lock (Lock)
                {
                    if (_loader == null)
                    {
                        Loader = new ScriptLoaderIGLib();
                        _loader.ClassName = DefaultScriptClassName;
                    }
                    return _loader;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    _loader = value;
                }
            }
        }

        private bool _iscompiled = false;

        /// <summary>Whether the current function definition has been compiled or not.</summary>
        public bool IsCompiled
        {
            get { lock (Lock) { return _iscompiled; } }
            protected set
            {
                lock (Lock)
                {
                    _iscompiled = value;
                    if (value == false)
                        ScriptClassName = null;
                }
            }
        }


        private string _scriptClassName;

        /// <summary>Name of the script class that containe definition of the compiled real function class.</summary>
        public string ScriptClassName
        {
            get { lock (Lock) { return _scriptClassName; } }
            protected set { _scriptClassName = value; }
        }


        LoadableScriptScalarFunctionBase _creator;

        /// <summary>Returns an object of the dynamically compiled class that can create function objects.</summary>
        public LoadableScriptScalarFunctionBase Creator
        {
            get
            {
                lock (Lock)
                {
                    if (!IsCompiled || _creator == null)
                    {
                        _function = null;
                        Compile();
                        _creator = Loader.CreateLoadableObject(null, ScriptClassName) as LoadableScriptScalarFunctionBase;
                    }
                    return _creator;  // Loader.CreateLoadableObject(null, ScriptClassName) as LoadableScriptScalarFunctionBase;
                }
            }
        }

        private LoadableScalarFunctionBase _function;

        /// <summary>Returns an instance of a scalar function created by the current loader.
        /// <para>Loader provides a property through which a scalar function created on demand can be accessed. If compiled scripts the invalid
        /// (e.g. when some contents are changed) the function will be created anew at next access, so it is always consistent with the loader data.</para></summary>
        /// <remarks>As alternative, the scalar function can be created by the <see cref="CreateScalarFunction"/> call, but this call creates a neew 
        /// object every time. This property creates a single function that can be used by anoone using this property.</remarks>
        public LoadableScalarFunctionBase Function
        {
            get
            {
                if (!IsCompiled)
                {
                    // We need to compile (or recompile) the source code first:
                    _function = null;
                }
                if (_function == null)
                {
                    _function = Creator.CreateScalarFunction();
                }
                return _function;
            }
            protected set
            {
                _function = value;
            }
        }

        #endregion Data


        #region Operation


        /// <summary>Compiles the code that contains dynamically loadable definition of a real function of one variable.</summary>
        /// <returns></returns>
        public string Compile()
        {
            lock (Lock)
            {
                if (!IsCompiled)
                {
                    Loader.Code = this.Code;
                    ScriptClassName = Loader.Compile();
                }
                return ScriptClassName;
            }
        }

        /// <summary>Creates and returns an instance of dynamically compiled scalar function object.</summary>
        public LoadableScalarFunctionBase CreateScalarFunction()
        { return Creator.CreateScalarFunction(); }

        /// <summary>Creates and returns an instance of dynamically compiled scalar function object,
        /// where the scalar funciton is obtained from user defined expressions and affine trensformation of
        /// vector parameters.</summary>
        /// $A Igor Sep11;
        public LoadableScalarFunctionBase CreateScalarFunction(IAffineTransformation transf)
        { return Creator.CreateScalarFunction(transf); }

        #endregion Operation


        #region Examples


        /// <summary>Example of use of the <see cref="ScalarFunctionLoader"/> class.
        /// Creates a function loader and uses it for dynamic definition of functions.</summary>
        /// $A Igor Jun10;
        public static void Example()
        {
            Example(null);
        }

        /// <summary>Example of use of the <see cref="ScalarFunctionLoader"/> class.
        /// Creates a function loader and uses it for dynamic definition of functions.</summary>
        /// <param name="scriptPath">Path where script that defines the function is saved.
        /// If null or empty string then script is not saved to a file.</param>
        /// $A Igor Jun10;
        public static void Example(string scriptPath)
        {
            Console.WriteLine();
            Console.WriteLine("Example of dynamic scalar function definitions.");
            bool continueExample = true;
            int numIterations = 0;
            ScalarFunctionLoader loader = new ScalarFunctionLoader(); // create a new function loader
            loader.NumParameters = 2;
            loader.IndependentVariableNames = new string[] { "x", "y" }; // names of independent variables used in expressions that define the function
            StopWatch1 t = new StopWatch1();
            bool defineGradient = true;
            bool defineHessian = false;
            bool redefineParameterNames = false;
            bool insertParameters = false;
            while (continueExample)
            {
                try
                {
                    Console.WriteLine();
                    loader.InvalidateDefinitions();
                    if (numIterations == 0)
                    {
                        loader.NumParameters = 2;
                        loader.IndependentVariableNames = new string[] { "x", "y" };
                        loader.ValueDefinitionString = "x*x + y"; // expression for function value
                        loader.GradientDefinitionStrings = new string[] { "2*x", "1" };
                        loader.HessianDefinitionStrings = new string[][] {
                            new string[] {
                                "2",
                                "0.0000"
                            },
                            new string[]
                            {
                                null,
                                "0.0"
                            }
                        };
                    } else if (numIterations == 1)
                    {
                        // The Rosenbrock function:
                        loader.NumParameters = 2;
                        loader.IndependentVariableNames = new string[] { "x", "y" };
                        loader.ValueDefinitionString = "0; " + "double term1 = 1 - x; double term2 = y - x*x; "
                            + loader.ReturnedValueName + " = " + "term1*term1 + 100 * term2 * term2; "; // expression for function value
                        loader.GradientDefinitionStrings = new string[] { 
                            "-2*(1-x)-400*x*( y - x*x)", 
                            "200*( y - x*x)" 
                        };
                        loader.HessianDefinitionStrings = null;
                    } else
                    {
                        Console.WriteLine("Input definition of scalar function and (optionally) its gradient!");
                        Console.WriteLine("Insert '?' to get help for entering values!");
                        // Read number of parameters
                        int numParameters = loader.NumParameters;
                        Console.Write(Environment.NewLine + "Number of parameters: ");
                        UtilConsole.Read(ref numParameters);
                        if (numParameters != loader.NumParameters)
                        {
                            loader.InvalidateDefinitions();
                            loader.NumParameters = numParameters;
                        }
                        // Read names of parameters:
                        Console.WriteLine(Environment.NewLine + "Current parameter names: {" + loader.GetParametersPlainListString() + "}.");
                        Console.Write(Environment.NewLine + "Redefine parameter names (0/1)? ");
                        UtilConsole.Read(ref redefineParameterNames);
                        if (redefineParameterNames)
                        {
                            string[] parameterNames = new string[loader.NumParameters];
                            for (int i=0; i< parameterNames.Length; ++i)
                            {
                                string parName = null;
                                if (loader.IndependentVariableNames!=null)
                                    if (loader.IndependentVariableNames.Length>i)
                                        parName = loader.IndependentVariableNames[i];
                                Console.Write("Name of parameter No. " + i + ": ");
                                UtilConsole.Read(ref parName);
                                parameterNames[i] = parName;
                            }
                            loader.IndependentVariableNames = parameterNames;
                        }
                        Console.WriteLine();
                        // Read definition of function value:
                        string valueDefinitionString = loader.ValueDefinitionString;
                        Console.Write("f(" + loader.GetParametersPlainListString() + ") = " 
                            + Environment.NewLine + "  ");
                        UtilConsole.Read(ref valueDefinitionString);
                        loader.ValueDefinitionString = valueDefinitionString;

                        Console.Write(Environment.NewLine + "Define gradient (0/1)? ");
                        UtilConsole.Read(ref defineGradient);
                        if (!defineGradient)
                            loader.GradientDefinitionStrings = null;
                        else
                        {
                            string[] gradientDefinitionStrings = new string[loader.NumParameters];
                            for (int i = 0; i < numParameters; ++i)
                            {
                                string defStr = null;
                                if (loader.GradientDefinitionStrings != null)
                                    if (loader.GradientDefinitionStrings.Length > i)
                                    defStr = loader.GradientDefinitionStrings[i];
                                Console.Write("grad f(" + loader.GetParametersPlainListString() + ")[" + i + "] = "
                                    + Environment.NewLine + "  ");
                                UtilConsole.Read(ref defStr);
                                gradientDefinitionStrings[i] = defStr;
                            }
                            loader.GradientDefinitionStrings = gradientDefinitionStrings;
                        }

                        Console.Write(Environment.NewLine + "Define Hessian (0/1)? ");
                        UtilConsole.Read(ref defineHessian);
                        if (!defineHessian)
                            loader.HessianDefinitionStrings = null;
                        else
                        {
                            throw new NotImplementedException("Definition of Hesian components is not yet implemented.");
                        }

                    }
                    t.Start();
                    // Define the function that will be dynamically loaded:
                    if (!string.IsNullOrEmpty(scriptPath))
                    {
                        File.WriteAllText(scriptPath, loader.Code);
                        Console.WriteLine("Script saved to " + scriptPath + ".");
                    }
                    // Create the corresponding function object:
                    LoadableScalarFunctionBase func = loader.CreateScalarFunction();
                    t.Stop();
                    Console.WriteLine("Function object generated in " + t.Time + " seconds.");
                    Console.WriteLine("Definition: ");
                    Console.WriteLine("f(" + loader.GetParametersPlainListString() + ") = " 
                        + Environment.NewLine + "  " + loader.ValueDefinitionString);
                    if (loader.GradientDefined)
                    {
                        Console.WriteLine("Definitions of gradient components: ");
                        if (loader.GradientDefinitionStrings!=null)
                            for (int i = 0; i < loader.GradientDefinitionStrings.Length; ++i)
                            {
                                Console.WriteLine("grad f(" + loader.GetParametersPlainListString() + ")[" + i + "] = "
                                    + Environment.NewLine + "  " + loader.GradientDefinitionStrings[i]);
                            }
                        Console.WriteLine();
                    }
                    if (loader.HessianDefined)
                    {
                        Console.WriteLine("Definitions of Hessian components: ");
                        if (loader.HessianDefinitionStrings!=null)
                            for (int i = 0; i < loader.HessianDefinitionStrings.Length; ++i)
                            {
                                for (int j = 0; j < loader.HessianDefinitionStrings[i].Length; ++j)
                                {
                                    Console.WriteLine("Hessian f(" + loader.GetParametersPlainListString() + ")[" + i + ", " + j + "] = "
                                        + Environment.NewLine + "  " + loader.HessianDefinitionStrings[i][j]);
                                }
                            }
                        Console.WriteLine();
                    }
                    
                    if (!func.ValueDefined)
                        Console.WriteLine("Function value is not defined!");
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Test Values: ");
                        for (int i = 0; i <= 4; ++i)
                        {
                            IVector param = new Vector(func.NumParameters);
                            for (int j = 0; j < func.NumParameters; ++j)
                                param[j] = (double)i;
                            Console.WriteLine(loader.FunctionArgumentParametersName + ": " + param.ToString());
                            Console.WriteLine("  f(" + loader.FunctionArgumentParametersName + ") = " + func.Value(param) + ".  ");
                            if (func.GradientDefined)
                            {
                                IVector grad = new Vector(func.NumParameters);
                                func.Gradient(param, ref grad);
                                Console.Write("  grad f(" + loader.FunctionArgumentParametersName + ") = " 
                                    + "  " +  grad + ".  ");
                            }
                            Console.WriteLine();
                        }

                        Console.WriteLine();
                        Console.Write("Insert parameters manually (0/1)? ");
                        UtilConsole.Read(ref insertParameters);
                        bool insertanotherParameters = true;
                        IVector parameters = new Vector(func.NumParameters);
                        while (insertParameters && insertanotherParameters)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Insert parameters! ");
                            for (int i = 0; i < func.NumParameters; ++i)
                            {
                                double parameterComponent = parameters[i];
                                Console.Write("  " + loader.IndependentVariableNames[i] + " = ");
                                UtilConsole.Read(ref parameterComponent);
                                parameters[i] = parameterComponent;
                            }
                            Console.WriteLine();
                            Console.WriteLine(loader.FunctionArgumentParametersName + " = " + parameters.ToString());

                            if (func.ValueDefined)
                                Console.WriteLine("  f(" + loader.FunctionArgumentParametersName + ") = " + func.Value(parameters) + ".  ");
                            if (func.GradientDefined)
                            {
                                IVector grad = new Vector(func.NumParameters);
                                func.Gradient(parameters, ref grad);
                                Console.WriteLine("  grad f(" + loader.FunctionArgumentParametersName + ") = "
                                    + "  " + grad + ".  ");
                            }
                            if (func.HessianDefined)
                            {
                                IMatrix hessian = new Matrix(func.NumParameters, func.NumParameters);
                                func.Hessian(parameters, ref hessian);
                                Console.WriteLine("  Hessian f(" + loader.FunctionArgumentParametersName + ") = "
                                    + Environment.NewLine + "  " + hessian.ToStringNewlines() + ".  ");
                            }
                            Console.WriteLine();

                            Console.Write(Environment.NewLine + "Insert another set of parameters (0/1)? ");
                            UtilConsole.Read(ref insertanotherParameters);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
                ++numIterations;
                Console.WriteLine();
                Console.Write("Continue example (0/1)? ");
                UtilConsole.Read(ref continueExample);
            }
            Console.WriteLine();
        }

        #endregion Examples

    }  // class ScalarFunctionLoader

}

