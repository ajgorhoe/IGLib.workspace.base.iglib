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


    /// <summary>Dynamically generates IRealFunction classes from user definitions.
    /// User can define in string form how function, its derivative, second derivative, integral, 
    /// or inverse function is calculated. Then this class can be used to compile these definitions
    /// in the wrapping script, which is then used to create the corresponding function objects.</summary>
    /// $A Igor Jun10;
    public class RealFunctionLoader: ILockable
    {

        #region construction

        /// <summary>Constructor.</summary>
        public RealFunctionLoader()
        { }

        #endregion construction


        #region ILockable

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ILockable


        #region Constants

        /// <summary>Default name of the class used in loadable scripts containing user definitions of real function class.</summary>
        public const string DefaultScriptClassName = "RealFunctionScript";

        /// <summary>Name of the function that initializes variables realated to dynamically loaded real function class.</summary>
        public const string FuncNameInitDynamic = "InitDynamic";

        /// <summary>Name of variable (internal in class defined in loadable script) that holds the name 
        /// of local variable (defined within functions in the dynamically loaded script) where 
        /// returned value is stored.</summary>
        public const string VarNameReturnedValueName = "_returnedValueName";

        /// <summary>Default name of the variable that holds returned value in script functions.</summary>
        public const string DefaultReturnedValueName = "ret";

        /// <summary>Name of variable (internal in class defined in loadable script) that holds the name 
        /// of function argument (defined within functions in the dynamically loaded script) through which 
        /// independent variable is passed.</summary>
        public const string VarNameFunctionArgumentName = "_functionArgumentName";

        /// <summary>Default name of the function argument in script functions.</summary>
        public const string DefaultFunctionArgumentName = "arg";

        /// <summary>Name of variable (internal in class defined in loadable script) that holds the name of 
        /// local variable (defined within functions in the dynamically loaded script) that holds the independent 
        /// variable in dunction definitions.</summary>
        public const string VarNameIndependentVariableName = "_independentVariableName";

        /// <summary>Default name of the independent variable in expression definitions used in scripts
        /// to define calculation of function values, derivatives, etc.</summary>
        public const string DefaultIndependentVariableName = "x";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds user 
        /// definition (as string) of function value.</summary>
        public const string VarNameValueDefinitionString = "_valueDefinitionString";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds user 
        /// definition (as string) of function derivative.</summary>
        public const string VarNameDerivativeDefinitionString = "_derivativeDefinitionString";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds user 
        /// definition (as string) of function second derivative.</summary>
        public const string VarNameSecondDerivativeDefinitionString = "_secondDerivativeDefinitionString";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds user 
        /// definition (as string) of function integral.</summary>
        public const string VarNameIntegralDefinitionString = "_integralDefinitionString";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds user 
        /// definition (as string) of function inverse.</summary>
        public const string VarNameInverseDefinitionString = "_inverseDefinitionString";

        /// <summary>Name of the function (internal in class defined in loadable script) that defines 
        /// calculation of function value.</summary>
        public const string FuncNameValueDefinition = "RefValue";

        /// <summary>Name of the function (internal in class defined in loadable script) that defines 
        /// calculation of function derivative.</summary>
        public const string FuncNameDerivativeDefinition = "RefDerivative";

        /// <summary>Name of the function (internal in class defined in loadable script) that defines 
        /// calculation of function second derivative.</summary>
        public const string FuncNameSecondDerivativeDefinition = "RefSecondDerivative";

        /// <summary>Name of the function (internal in class defined in loadable script) that defines 
        /// calculation of function integral.</summary>
        public const string FuncNameIntegralDefinition = "RefIntegral";

        /// <summary>Name of the function (internal in class defined in loadable script) that defines 
        /// calculation of function inverse.</summary>
        public const string FuncNameInverseDefinition = "RefInverse";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds a flag 
        /// telling whether calculation of function value is implemented.</summary>
        public const string VarNameValueDefined = "_valueDefined";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds a flag 
        /// telling whether calculation of function derivative is implemented.</summary>
        public const string VarNameDerivativeDefined = "_derivativeDefined";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds a flag 
        /// telling whether calculation of function second derivative is implemented.</summary>
        public const string VarNameSecondDerivativeDefined = "_secondDerivativeDefined";

        /// <summary>Name of the variable (internal in class defined in loadable script) that holds a flag 
        /// telling whether calculation of function integral is implemented.</summary>
        public const string VarNameIntegralDefined = "_integralDefined";


        /// <summary>Name of the variable (internal in class defined in loadable script) that holds a flag 
        /// telling whether calculation of function inverse is implemented.</summary>
        public const string VarNameInverseDefined = "_inverseDefined";

        #endregion Constants


        #region Data

        /// <summary>Whether calculation of function value is implemented.</summary>
        public bool ValueDefined
        { get { lock (Lock) { return !string.IsNullOrEmpty(ValueDefinitionString); } } }

        /// <summary>Whether calculation of function derivative is implemented.</summary>
        public bool DerivativeDefined
        { get { lock (Lock) { return !string.IsNullOrEmpty(DerivativeDefinitionString); } } }

        /// <summary>Whether calculation of function second derivative is implemented.</summary>
        public bool SecondDerivativeDefined
        { get { lock (Lock) { return !string.IsNullOrEmpty(SecondDerivativeDefinitionString); } } }

        /// <summary>Whether calculation of function indefinite integral is implemented.</summary>
        public bool IntegralDefined
        { get { lock (Lock) { return !string.IsNullOrEmpty(IntegralDefinitionString); } } }

        /// <summary>Whether calculation of inverse function is implemented.</summary>
        public bool InverseDefined
        { get { lock (Lock) { return !string.IsNullOrEmpty(InverseDefinitionString); } } }


        private string
            _returnedValueName = DefaultReturnedValueName,
            _functionArgumentName = DefaultFunctionArgumentName,
            _independentVariableName = DefaultIndependentVariableName,
            _valueDefinitionString,
            _derivativeDefinitionString,
            _secondDerivativeDefinitionString,
            _integralDefinitionString,
            _inverseDefinitionString,
            _code;


        /// <summary>Clears strings that define the function (i.e. strings that define expressions for
        /// function value, derivative, integral, inverse, etc.).</summary>
        public void InvalidateDefinitions()
        {
            ValueDefinitionString = null;
            DerivativeDefinitionString = null;
            SecondDerivativeDefinitionString = null;
            IntegralDefinitionString = null;
            InverseDefinitionString = null;
        }

        /// <summary>Sets names used in generated script code.</summary>
        /// <param name="returnedValueName">Name of the variabble that holds returned value in functions in generated script code.</param>
        /// <param name="functionArgumentName">Name of function argument in generated script code.</param>
        /// <param name="independentVariableName">Name of independent variable in generated script code.</param>
        public void SetNames(string returnedValueName, string functionArgumentName, string independentVariableName)
        {
            this.ReturnedValueName = returnedValueName;
            this.FunctionArgumentName = functionArgumentName;
            this.IndependentVariableName = independentVariableName;
        }

        /// <summary>Name of variable (within functions in the loadable scripts) that holds
        /// the returned value. Set to constant <see cref="DefaultReturnedValueName"/> by default.</summary>
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
                        if (value == FunctionArgumentName)
                            throw new ArgumentException("Variable that holds returned value can not have the same name as function argument. Name: "
                                + value);
                        if (value == IndependentVariableName)
                            throw new ArgumentException("Variable that holds returned value can not have the same name as independent variable. Name: "
                                + value);
                    }
                    _returnedValueName = value;
                    // Invalidate dependencied:
                    InvalidateDefinitions();
                }
            }
        }

        /// <summary>Name of function arguments (in functions in the loadable scripts) that holds
        /// the independent variable. Set to constant <see cref="DefaultFunctionArgumentName"/> by default.</summary>
        public string FunctionArgumentName
        {
            get
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(_functionArgumentName))
                        FunctionArgumentName = DefaultFunctionArgumentName;
                    return _functionArgumentName;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value == ReturnedValueName)
                            throw new ArgumentException("Function argument can not have the same name as variable that holds return value. Name: "
                                + value);
                        if (value == IndependentVariableName)
                            throw new ArgumentException("Function argument can not have the same name as independent variable. Name: "
                                + value);
                    }
                    _functionArgumentName = value;
                    // Invalidate dependencied:
                    InvalidateDefinitions();
                }
            }
        }

        /// <summary>Name of variable (within functions in the loadable scripts) that holds
        /// the independent variable. Set to constant <see cref="DefaultIndependentVariableName"/> by default.</summary>
        public string IndependentVariableName
        {
            get
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(_independentVariableName))
                        IndependentVariableName = DefaultIndependentVariableName;
                    return _independentVariableName;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value == ReturnedValueName)
                            throw new ArgumentException("Independent variable can not have the same name as variable that holds return value. Name: "
                                + value);
                        if (value == FunctionArgumentName)
                            throw new ArgumentException("Independent variable can not have the same name as function argument. Name: "
                                + value);
                    }
                    _independentVariableName = value;
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

        /// <summary>Expression that defines function derivative.</summary>
        public string DerivativeDefinitionString
        {
            get  { lock (Lock) { return _derivativeDefinitionString; } }
            set { lock (Lock) { _derivativeDefinitionString = value; Code = null; } }
        }

        /// <summary>Expression that defines function second derivative.</summary>
        public string SecondDerivativeDefinitionString
        {
            get { lock (Lock) { return _secondDerivativeDefinitionString; } }
            set { lock (Lock) { _secondDerivativeDefinitionString = value; Code = null; } }
        }

        /// <summary>Expression that defines function indefinite integral derivative.</summary>
        public string IntegralDefinitionString
        {
            get { lock (Lock) { return _integralDefinitionString; } }
            set { lock (Lock) { _integralDefinitionString = value; Code = null; } }
        }

        /// <summary>Expression that defines function inverse.</summary>
        public string InverseDefinitionString
        {
            get { lock (Lock) { return _inverseDefinitionString; } }
            set { lock (Lock) { _inverseDefinitionString = value; Code = null; } }
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
        /// <param name="filePath">Path to the file where script code is saved.</param>
        public void SaveCode(string filePath)
        {
            File.WriteAllText(filePath, Code);
        }


        /// <summary>Appends to the apecified string builder the specified level of indentation.</summary>
        /// <param name="sb">String builder to which indents are appended.</param>
        /// <param name="numIndente">Number of indents that are appended.</param>
        private void AppendIndents(StringBuilder sb, int numIndent)
        {
            for (int i = 0; i < numIndent; ++i)
                sb.Append("\t");
        }

        /// <summary>Appends to the apecified string builder the C# statements that sets the specified variable to the specified value.</summary>
        /// <param name="sb">String builder to which the statement is appended.</param>
        /// <param name="varName">Name of the variable that is set.</param>
        /// <param name="value">Value that is assigned to the variable.</param>
        /// <param name="numIndents">Number of indents that are written before code lines.</param>
        private void AppendSetVariable(StringBuilder sb, string varName, object value, int numIndents)
        {
            if (sb == null)
                throw new ArgumentNullException("String builder object is not specified (null reference).");
            if (string.IsNullOrEmpty(varName))
                throw new ArgumentException("Variable name not specified (null or empty string).");
            for (int i = 0; i < numIndents; ++i)
                sb.Append("\t");
            sb.Append(varName + " = ");
            if (value == null)
                sb.Append("null");
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
            }
            sb.AppendLine(";");
        }


        /// <summary>Appends to the apecified string builder the C# definition of a double function returning double.
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
                sb.AppendLine("protected override double " + functionName + "(double " + this.FunctionArgumentName + ")");
                AppendIndents(sb, numIndents);
                sb.AppendLine("{");
                AppendIndents(sb, numIndents + 1);
                sb.AppendLine("double " + IndependentVariableName + " = " + FunctionArgumentName + ";");
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
    public class ScritpRealFunctionExample : LoadableScriptRealFunctionBase, ILoadableScript
    {

        public override LoadableRealFunctionBase CreateRealFunction()
        {
            return new Container.Function();
        }

        public override LoadableRealFunctionBase CreateRealFunction(double Kx, double Sx)
        {
            return new Container.Function(Kx, Sx);
        }

        public override LoadableRealFunctionBase CreateRealFunction(double Kx, double Sx, double Ky, double Sy)
        {
            return new Container.Function(Kx, Sx, Ky, Sy);
        }


        /// <summary>Container class inherits from M in order to enable use of comfortable mathematical functions.</summary>
        public class Container : M
        {

            public class Function : LoadableRealFunctionBase
            {

                #region Construction
                public Function() : base() { }
                public Function(double Kx, double Sx) : base(Kx, Sx) { }
                public Function(double Kx, double Sx, double Ky, double Sy) : base(Kx, Sx, Ky, Sy) { }
                #endregion construction

                #region Generated

");

               
            int numIndents = 5;
            AppendIndents(sb,numIndents);
            sb.AppendLine("protected override void " + FuncNameInitDynamic + "() ");
            AppendIndents(sb,numIndents);
            sb.AppendLine("{");

            // Set auxiliary internal variables that are used in functions of the generated class:
             AppendSetVariable(sb, VarNameReturnedValueName, ReturnedValueName, numIndents);
            AppendSetVariable(sb, VarNameFunctionArgumentName, FunctionArgumentName, numIndents);
            AppendSetVariable(sb, VarNameIndependentVariableName, IndependentVariableName, numIndents);
            AppendSetVariable(sb, VarNameValueDefinitionString, ValueDefinitionString, numIndents);
            AppendSetVariable(sb, VarNameDerivativeDefinitionString, DerivativeDefinitionString, numIndents);
            AppendSetVariable(sb, VarNameSecondDerivativeDefinitionString, SecondDerivativeDefinitionString, numIndents);
            AppendSetVariable(sb, VarNameIntegralDefinitionString, IntegralDefinitionString, numIndents);
            AppendSetVariable(sb, VarNameInverseDefinitionString, InverseDefinitionString, numIndents);

            AppendSetVariable(sb, VarNameValueDefined, ValueDefined, numIndents);
            AppendSetVariable(sb, VarNameDerivativeDefined, DerivativeDefined, numIndents);
            AppendSetVariable(sb, VarNameSecondDerivativeDefined, SecondDerivativeDefined, numIndents);
            AppendSetVariable(sb, VarNameIntegralDefined, IntegralDefined, numIndents);
            AppendSetVariable(sb, VarNameInverseDefined, InverseDefined, numIndents);
            
            sb.AppendLine(@"
                }
");

            AppendFunctonDefinition(sb, FuncNameValueDefinition, ValueDefinitionString, numIndents);
            AppendFunctonDefinition(sb, FuncNameDerivativeDefinition, DerivativeDefinitionString, numIndents);
            AppendFunctonDefinition(sb, FuncNameSecondDerivativeDefinition, SecondDerivativeDefinitionString, numIndents);
            AppendFunctonDefinition(sb, FuncNameIntegralDefinition, IntegralDefinitionString, numIndents);
            AppendFunctonDefinition(sb, FuncNameInverseDefinition, InverseDefinitionString, numIndents);

            sb.AppendLine(@"

                #endregion Generated

            }  // class Container.Function

        }  // class Container

    }  // class ScritpRealFunction

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


        /// <summary>Returns an object of the dynamically compiled class that can create function objects.</summary>
        public LoadableScriptRealFunctionBase Creator
        {
            get
            {
                lock (Lock)
                {
                    if (!IsCompiled)
                        Compile();
                    return Loader.CreateLoadableObject(null, ScriptClassName) as LoadableScriptRealFunctionBase;
                }
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


        /// <summary>Creates and returns an instance of dynamically compiled real function object.</summary>
        public LoadableRealFunctionBase CreateRealFunction()
        { return Creator.CreateRealFunction(); }

        /// <summary>Creates and returns an instance of dynamically compiled real function object.</summary>
        /// $A Igor Sep11;
        public LoadableRealFunctionBase CreateRealFunction(double Kx, double Sx)
        { return Creator.CreateRealFunction(Kx, Sx); }

        /// <summary>Creates and returns an instance of dynamically compiled real function object.</summary>
        /// $A Igor Sep11;
        public LoadableRealFunctionBase CreateRealFunction(double Kx, double Sx, double Ky, double Sy)
        { return Creator.CreateRealFunction(Kx, Sx, Ky, Sy); }

        #endregion Operation


        #region Examples

        
        /// <summary>Example of use of the <see cref="RealFunctionLoader"/> class.
        /// Creates a function loader and uses it for dynamic definition of functions.</summary>
        /// $A Igor Jun10;
        public static void Example()
        {
            Example(null);
        }

        /// <summary>Example of use of the <see cref="RealFunctionLoader"/> class.
        /// Creates a function loader and uses it for dynamic definition of functions.</summary>
        /// <param name="scriptPath">Path where script that defines the function is saved.
        /// If null or empty string then script is not saved to a file.</param>
        /// $A Igor Jun10;
        public static void Example(string scriptPath)
        {
            Console.WriteLine();
            Console.WriteLine("Example of dynamic function definitions.");
            bool continueExample = true;
            int numIterations = 0;
            string valueDefinition = null, derivativeDefinition = null;
            RealFunctionLoader loader = new RealFunctionLoader(); // create a new function loader
            loader.IndependentVariableName = "x"; // name of independent variable used in expressions that define the function
            StopWatch t = new StopWatch();
            while (continueExample)
            {
                try
                {
                    Console.WriteLine();
                    loader.InvalidateDefinitions();
                    if (numIterations == 0)
                    {
                        valueDefinition = "x*x"; // expression for function value
                        derivativeDefinition = "2*x"; // expression for function derivative
                    }
                    else if (numIterations == 1)
                    {
                        valueDefinition = "pow(x,3)";
                        derivativeDefinition = "3*pow(x,2)";
                    }
                    else
                    {
                        Console.WriteLine("Input definition of function and (optionally) its derivative!");
                        Console.Write("f(" + loader.IndependentVariableName + ") = ");
                        valueDefinition = Console.ReadLine();
                        Console.Write("f'(" + loader.IndependentVariableName + ") = ");
                        derivativeDefinition = Console.ReadLine();
                    }
                    t.Start();
                    // Define the function that will be dynamically loaded:
                    loader.InvalidateDefinitions();
                    loader.ValueDefinitionString = valueDefinition;
                    loader.DerivativeDefinitionString = derivativeDefinition;
                    if (!string.IsNullOrEmpty(scriptPath))
                    {
                        File.WriteAllText(scriptPath, loader.Code);
                        Console.WriteLine("Script saved to " + scriptPath + ".");
                    }
                    // Create the corresponding function object:
                    IRealFunction func = loader.CreateRealFunction();
                    t.Stop();
                    Console.WriteLine("Function object generated in " + t.Time + " seconds.");
                    Console.WriteLine("Definition: ");
                    Console.WriteLine("f(" + loader.IndependentVariableName + ") = " + loader.ValueDefinitionString);
                    Console.WriteLine("f'(" + loader.IndependentVariableName + ") = " + loader.DerivativeDefinitionString);
                    // Use function object:
                    if (!func.ValueDefined)
                        Console.WriteLine("Function value is not defined!");
                    else
                    {
                        Console.WriteLine("Values: ");
                        for (int i = 0; i <= 5; ++i)
                        {
                            Console.Write("f(" + i + ") = " + func.Value(i) + ".  ");
                            if (func.DerivativeDefined)
                                Console.Write("f'(" + i + ") = " + func.Derivative(i) + ".  ");
                            Console.WriteLine();
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

    }  // class RealFunctionLoader

}

