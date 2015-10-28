// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using Microsoft.JScript;

        /************************************************/
        /*                                              */
        /*    DYNAMIC COMPILATION-BASED INTERPRETERS    */
        /*                                              */
        /************************************************/


namespace IG.Lib
{



    /// <summary>Base class for expression evaluators that utilize the functionality 
    /// of CodeDomProvider class.</summary>
    public abstract class ExpressionEvaluatorCompiled : ILockable, IIdentifiable
    {

        /// <summary>Initializes ID and calls the main initialization method (which will
        /// typically be overridden in every subclass).</summary>
        public ExpressionEvaluatorCompiled()
        {
            lock (_idLock)
            {
                // Obtain unique evaluator's ID:
                ++ _lastId;
                if (_id < 0)
                    _id = _lastId;
            }
            InitExpressionEvaluator();
        }


        #region IIDentifiable

        private static int _lastId = 0;
        private static object _idLock = new object();
        private int _id = -1;

        /// <summary>Returns object's Id that is unique within a given type.</summary>
        public int Id { get { return _id; } }

        #endregion IIDentidiable

        protected abstract void InitExpressionEvaluator();


        public static int DefaultOutputLevel = 0;

        /// <summary>Default level of output for some of the interpreters' functionality (e.g. asynchronous command execution).</summary>
        protected int _outputLevel = DefaultOutputLevel;

        /// <summary>Level of output for some of the interpreter's functionality (e.g. asynchronous command execution).</summary>
        public int OutputLevel
        {
            get { return _outputLevel; }
            set { _outputLevel = value; }
        }


        #region ILockableImplementation

        private object _lock = new object();

        /// <summary>Used for locking internal fields.</summary>
        public object Lock { get { return _lock; } }

        #endregion ILockableImplementation

        #region OperationData

        protected string _language = "unknown";
        protected string _packageName = "Evaluator";
        protected string _className = "EvaluatorClass";
        protected string _evaluationFunctionName = "Eval";

        /// <summary>Programming language in use.</summary>
        public virtual string Language
        {
            get { return _language; }
            protected set { _language = value; }
        }

        /// <summary>Name of the JavaScript package in which evaluated code is executed.</summary>
        public virtual string PackageName
        {
            get { return _packageName; }
            protected set { _packageName = value; }
        }

        /// <summary>Name of the JavaScript class that is compiled to execute the evaluated code.</summary>
        public virtual string ClassName
        {
            get { return _className; }
            protected set { _className = value; }
        }

        /// <summary>Name of the function used for evaluation of expressions.</summary>
        public virtual string EvaluationFunctionName
        {
            get { return _evaluationFunctionName; }
            protected set { _evaluationFunctionName = value; }
        }


        #region DefinitionsAndCode

        private string _baseDefinitions = "";
        private string _userDefinitions = "";
        private string _newUserDefinitions = "";
        private string _completeCode = "";

        /// <summary>A set of pre-defined definitions that can be used in the evaluated code.</summary>
        public virtual string BaseDefinitions
        {
            get { return _baseDefinitions; }
            protected set { _baseDefinitions = value; }
        }

        /// <summary>All valid user code inserted up to this point.
        /// Only code that compiled and executed without errors is taken into account.</summary>
        public string CompleteCode
        {
            get { return _completeCode; }
            protected set { _completeCode = value; }
        }


        /// <summary>A set of definitions inserted by users.</summary>
        public virtual string UserDefinitions
        {
            get { return _userDefinitions; }
            protected set { _userDefinitions = value; }
        }

        /// <summary>New user definitions that are added temporarily for testing.</summary>
        protected virtual string NewUserDefinitions
        {
            get { return _newUserDefinitions; }
            set { _newUserDefinitions = value; }
        }

        /// <summary>All definitions (pre-defined and user-defined) that can be used in the evaluated code.</summary>
        public virtual string Definitions
        {
            get
            {
                return BaseDefinitions + Environment.NewLine + UserDefinitions
                    + Environment.NewLine + NewUserDefinitions + Environment.NewLine;
            }
        }


        /// <summary>Container for interpreted code.</summary>
        /// <remarks>When overridden in subclasses, this property will be dynamic (i.e. each get
        /// accessor will generate the value anew), and the value will depend on current values
        /// of the package, class and function name as well as additonal definitions input by user.</remarks>
        protected abstract string ScriptBase { get; }

        #endregion DefinitionsAndCode

        #endregion OperationData


        #region BasicOperations


        /// <summary>Compiles the base script where evaluation is plugged in,
        /// and loads the generated assembly and necessary objects.</summary>
        /// <returns>Eventual compilation results.</returns>
        protected virtual string CompileBase()
        {
            return CompileBase(null);
        }

        private object _evaluator = null;

        private Type _evaluatorType = null;

        /// <summary>Compiles the base script where evaluation is plugged in,
        /// and loads the generated assembly and necessary objects.</summary>
        /// <param name="inputDefinitions">New definitions that are added to the base script
        /// and will not yet be part of permanent definitions (but will become part of them
        /// if compilation is successful).</param>
        protected virtual string CompileBase(string inputDefinitions)
        {
            string ret = null;
            //// Deprecated use example:
            //ICodeCompiler compiler;
            //compiler = new JScriptCodeProvider().CreateCompiler();
            CodeDomProvider compiler = CodeDomProvider.CreateProvider(Language);
            CompilerInfo langCompilerInfo = CodeDomProvider.GetCompilerInfo(Language);
            CompilerParameters parameters = langCompilerInfo.CreateDefaultCompilerParameters();
            parameters.GenerateInMemory = true;  // don't write compiled units to disk
            // Compile the base script and load the generated assembly:
            CompilerResults results;
            NewUserDefinitions = inputDefinitions;
            results = compiler.CompileAssemblyFromSource(parameters, ScriptBase);
            NewUserDefinitions = null;
            Assembly assembly = results.CompiledAssembly;
            // Instantiate the class from the generated assembly that is used for code execution:
            _evaluatorType = assembly.GetType(PackageName + "." + ClassName);
            _evaluator = Activator.CreateInstance(_evaluatorType);
            return ret;
        }

        /// <summary>Evaluates JavaScript code and returns result as object.</summary>
        /// <param name="code">JavaScript code to be evaluated.</param>
        /// <returns>Object that is result of evaluaton of code.</returns>
        public virtual object EvalToObject(string code)
        {
            return _evaluatorType.InvokeMember(
                        EvaluationFunctionName,
                        BindingFlags.InvokeMethod,
                        null,
                        _evaluator,
                        new object[] { code }
                     );
        }

        /// <summary>Evaluates (interprets) JavaScript code and returns integer result of evaluation.
        /// Code must be such that result of evaluation can be interpreted as integer.</summary>
        /// <param name="code">Code that is evaluated.</param>
        /// <returns></returns>
        public virtual int EvalToInteger(string code)
        {
            string s = EvalToString(code);
            return int.Parse(s.ToString());
        }

        /// <summary>Evaluates (interprets) JavaScript code and returns double result of evaluation.
        /// Code must be such that result of evaluation can be interpreted as double.</summary>
        /// <param name="code">Code that is evaluated.</param>
        /// <returns></returns>
        public virtual double EvalToDouble(string code)
        {
            string s = EvalToString(code);
            return double.Parse(s);
        }

        /// <summary>Evaluates (interprets) JavaScript code and returns string result of evaluation.
        /// Code must be such that result of evaluation can be interpreted as string.</summary>
        /// <param name="code">Code that is evaluated.</param>
        /// <returns></returns>
        public virtual string EvalToString(string code)
        {
            object o = EvalToObject(code);
            return o.ToString();
        }


        /// <summary>Repairs the specified command and returns the repaired command string.
        /// <para>Reparations serve for easier insertion of commands and for addition of syntactic cookies.</para></summary>
        /// <param name="command">Command to be repaired.</param>
        public string GetRepairedCommand(string command)
        {
            string ret = command;
            RepairCommand(ref ret);
            return ret;
        }

        /// <summary>Repairs the specified command and returns the repaired command string.
        /// <para>Reparations serve for easier insertion of commands and for addition of syntactic cookies.</para></summary>
        /// <param name="command">Command to be repaired.</param>
        public virtual void RepairCommand(ref string command)
        {  }

        /// <summary>Executes the specified code and returns the result.
        /// Throws exception if errors occur when interpreting code.
        /// After execution, the code is appended to the complete code that has been executed up to this point.</summary>
        /// <param name="code">Code that is exected by the JavaScript interpreter.</param>
        /// <returns>Result of code execution as string.</returns>
        public virtual string Execute(string inputCode)
        {
            if (string.IsNullOrEmpty(inputCode))
                return null;
            string result = "";
            string codeToExecute = CompleteCode + GetRepairedCommand(inputCode) /* + ";" */ + Environment.NewLine;
            // Complete input, evaluate it, print results, and reset the code:

            if (this.OutputLevel >= 1)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine + "Code to execute: " + Environment.NewLine
                    + "============" + Environment.NewLine + codeToExecute + Environment.NewLine 
                    + "------------" + Environment.NewLine + Environment.NewLine);
            }

            result = EvalToString(codeToExecute);
            // Execution OK, append the inserted code to the complete code:
            CompleteCode = codeToExecute;
            return result;
        }

        /// <summary>Recompiles the basic script where evaluatin is plugged in, with added definitions.
        /// Throws exceptions if errors occur when compiling code.
        /// After execution, user definitions are added to the base script if compilatin is successful,
        /// so that they can be used in subsequent evaluations of code.</summary>
        /// <param name="inputDefinitions">User definitions that are compiled and added to the code.</param>
        /// <returns></returns>
        public virtual string Compile(string inputDefinitions)
        {
            string ret = null;
            ret = CompileBase(inputDefinitions);
            // Compilatin successful (no exception thrown), add user definitions to the base script:
            this.UserDefinitions += inputDefinitions;
            return ret;
        }

        /// <summary>Resets the code evaluator (clears variable definitions, etc.).</summary>
        public void Reset()
        {
            CompleteCode = null;
            UserDefinitions = null;
            Compile(null);
        }

        #endregion BasicOperations

        #region CommandLine

        protected string
            _inputMark = "Calc> ",
            _definitionsMark = "Def> ",
            _multilineMark = "Calc ml> ",
            _resultMark = "      = ",

            _helpCommand = "?",
            _printDefinitionsCommand = "/pd",
            _printDefinitionsCommand1 = "/printdefinitions",
            _printCodeCommand = "/pc",
            _printCodeCommand1 = "/printcode",
            _evaluationCommand = "/e",
            _evaluationCommand1 = "/evaluate",
            _definitionsCommand = "/d",
            _definitionsCommand1 = "/def",
            _multilineCommand = "/m",
            _multilineCommand1 = "/multiline",

            _saveCodeCommand = "/sc",
            _appendCodeCommand = "/ac",
            _saveDefinitionsComand = "/sd",
            _appendDefinitionsComand = "/ad",
            _resetCommand = "/reset",
            _quitCommand = "/exit",
            _quitCommand1 = "/q",
            _quitCommand2 = "/quit";

        protected char
            _multiLineCharacter = '\\',
            _commandCharacter = '/';


        protected string _commandLineHead = @"

Command-line Calculator:

Insert expressions or commands! 
/m for multiline mode, /d for definitions, /e to evaluate, /q to exit!
End line with '\\' to input multiline code, ? for help!

";

        protected string _commandLineStopNote = @"

Command-line calculator stopped.

";

        protected string _helpCommandLineHeading = @"
Command-line Expression Evaluator Help:
Insert valid code or special commands one line after another!
Quit with '\q' .";

        protected string _helpCommandLine = @"
Special commands must be inserted without leading spaces.
End lines with '\\' in order to insert multiline code.
List of commands:
  ? : prints this help 
  /e or /evaluate : insertion and evaluation of code
  /m or /multiline : insertion and evaluation of multiple lines
  /d or /def : insertion of definitions (multiline)
  /pc or /printcode: prints the current code contained in the buffer
  /pd or /printdefinitions: prints definitions (user and predefined)
  /sc : saves the evaluated code to a file (file path prompted)
  /ac : appends the evaluated code to a file (file path prompted)
  /sd : saves all added definitions to a file (file path prompted)
  /ad : appends all added definitons to a file (file path prompted)
  /reset : resets the evaluator
  /q or /quit or /exit : exits command-line evaluator

Syntactic specifics:
Automatic variable declaration does not work. 
    All variables must be declared with 'var'.
Invalid code is ignored (with error report launched). 
    This enables you to re-enter corrected code.
The last valid result will be returned.
The last expression does not need to (but it can) end with semicolon.
";


        /// <summary>Returns textual help for JavaScript command-line interpreter.</summary>
        /// <returns></returns>
        public virtual string HelpCommandLine
        {
            get { return _helpCommandLineHeading + _helpCommandLine; }
        }

        /// <summary>Prints help for the command-line JavaScript interpreter to the standard output.</summary>
        public virtual void PrintHelpCommandLine()
        { Console.WriteLine(HelpCommandLine); }

        /// <summary>Prints all definitions (preinstalled and user defined).</summary>
        public virtual void PrintDefinitions()
        {
            Console.WriteLine("All definitions (pre-installed & user defined): ");
            Console.WriteLine(Definitions);
        }

        /// <summary>Prints the complete code that has been input up to now to the console.</summary>
        public virtual void PrintCompleteCode()
        {
            Console.WriteLine("Complete code inserted up to this point: ");
            Console.WriteLine(CompleteCode);
        }

        /// <summary>Prints the complete code inserter up to this moment.</summary>
        /// <param name="inputFilePath"></param>
        /// <param name="append"></param>
        public virtual void SaveCompleteCode(string filePath, bool append)
        {
            using (TextWriter tw = new StreamWriter(filePath, append))
            {
                tw.WriteLine(CompleteCode);
            }
        }

        /// <summary>Saves or appends the complete code evaluated up to now to a file
        /// specified by the user.
        /// User is promped for the file to which code is saved.</summary>
        /// <param name="append"></param>
        public virtual void UserSaveCompleteCode(bool append)
        {
            string filePath;
            Console.WriteLine();
            if (append)
                Console.WriteLine("Valid code inserted up to this point will be appended to a file.");
            else
                Console.WriteLine("Valid code inserted up to this point will be saved to a file.");
            Console.Write("Insert file path (empty string to cancel saving): ");
            filePath = Console.ReadLine();
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Empty path inserted, saving cancelled.");
            }
            else
            {
                SaveCompleteCode(filePath, append);
            }
            Console.WriteLine();
        }

        /// <summary>Saves or appends to the specified file all valid user definitions added 
        /// to the evaluator up to this point.</summary>
        /// <param name="inputFilePath">Path to the file that definitions saved to.</param>
        /// <param name="append">If true then definitions are appended to the file (old content is preserved).</param>
        public virtual void SaveUserDefinitions(string filePath, bool append)
        {
            using (TextWriter tw = new StreamWriter(filePath, append))
            {
                tw.WriteLine(UserDefinitions);
            }
        }

        /// <summary>Saves or appends the all valid user definitions added to the evaluator
        /// up to this point.
        /// User is promped for the file to which definitions are saved.</summary>
        /// <param name="append">If true then definitions are appended to the file (old content is preserved).</param>
        public virtual void UserSaveUserDefinitions(bool append)
        {
            string filePath;
            Console.WriteLine();
            if (append)
                Console.WriteLine("Valid definitons added up to this point will be appended to a file.");
            else
                Console.WriteLine("Valid definitons added up to this point will be saved to a file.");
            Console.Write("Insert file path (empty string to cancel saving): ");
            filePath = Console.ReadLine();
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Empty path inserted, saving cancelled.");
            }
            else
            {
                SaveUserDefinitions(filePath, append);
            }
            Console.WriteLine();
        }



        #region InteractiveOperations

        /// <summary>Executes the code, prints results and reports eventual errors.
        /// Also appends the code to the complete valid code that has been interpreted up to now.</summary>
        /// <param name="command">Command to be executed.</param>
        /// <returns></returns>
        protected virtual string ExecuteUser(string command)
        {
            string result = "";
            try
            {
                //Console.WriteLine("Code to be evaluated: ");
                //Console.WriteLine(code);
                result = Execute(command);
                if (string.IsNullOrEmpty(result))
                    Console.WriteLine();
                else
                {
                    Console.WriteLine(_resultMark + result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR occurred: ");
                // Console.WriteLine("Error description: " + ex.Message);
                if (ex.InnerException != null)
                    if (ex.InnerException.Message != null)
                        Console.WriteLine(ex.InnerException.Message);
                Console.WriteLine();
            }
            return result;
        }


        /// <summary>Compiles the specified new definitions.
        /// If compilation is successful, the definitions are added to existing definitinos
        /// and user is notified through console. Otherwise, error is reported on console.</summary>
        /// <param name="inputDefinitions">User definitions to be added and compiled.</param>
        /// <returns>Eventual results of compilation.</returns>
        protected virtual string CompileUser(string inputDefinitions)
        {
            string ret = null;
            try
            {
                Compile(inputDefinitions);
                Console.WriteLine("New definitions successfully added.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR occurred during compilation: ");
                if (ex.Message != null)
                    Console.WriteLine("  Details: " + ex.Message);
                if (ex.InnerException != null) if (ex.InnerException.Message != null)
                        Console.WriteLine("  Cause: " + ex.InnerException.Message);
            }
            return ret;
        }

        /// <summary>Compiles definitions, if any specified, and executes code, if any specified.
        /// Eventual results and errors are reported in console.
        /// After execution, code and definitions are set to null.</summary>
        /// <param name="inpDefinitions">User definitions that must be added to existing definitions.
        /// If specified then the evaluation code is re-compiled.</param>
        /// <param name="inpCode">User code that is evaluated.</param>
        /// <returns>Eventual results of evaluation.</returns>
        protected virtual string ExecuteUser(ref string inpDefinitions, ref string inpCode)
        {
            string ret = "";
            if (!string.IsNullOrEmpty(inpDefinitions))
            {
                CompileUser(inpDefinitions);
                inpDefinitions = "";
            }
            if (!string.IsNullOrEmpty(inpCode))
            {
                ret = ExecuteUser(inpCode);
                inpCode = null;
            }
            return ret;
        }

        #endregion InteractiveOperations

        /// <summary>Command-line utility where user can successively enter JavaScrit expressions or 
        /// general portions of code, and evaluates them.
        /// Displays prompt to instruct the user, and result of operations.</summary>
        public virtual void CommandLine()
        {
            Console.Write(_commandLineHead);
            string line, trimmed;
            string inputCode = "";
            string inputDefinitions = "";
            bool evaluationMode = true;
            bool multilineMode = false;
            bool definitionMode = false;
            bool stopCommandLine = false;
            while (!stopCommandLine)
            {
                try
                {
                    if (multilineMode)
                        Console.Write(_multilineMark);
                    else if (definitionMode)
                        Console.Write(_definitionsMark);
                    else
                        Console.Write(_inputMark);
                    line = Console.ReadLine();
                    trimmed = line.TrimEnd();
                    if (string.IsNullOrEmpty(trimmed))
                    {
                        // empty string inserted, just ignore
                    }
                    else if (trimmed == _quitCommand || trimmed == _quitCommand1 || trimmed == _quitCommand2)
                    {
                        stopCommandLine = true;
                    }
                    else if (trimmed == _helpCommand)
                    {
                        PrintHelpCommandLine();
                    }
                    else if (trimmed == _printDefinitionsCommand || trimmed == _printDefinitionsCommand1)
                    {
                        PrintDefinitions();
                    }
                    else if (trimmed == _printCodeCommand || trimmed == _printCodeCommand1)
                    {
                        PrintCompleteCode();
                    }
                    else if (trimmed == _saveCodeCommand || trimmed == _appendCodeCommand)
                    {
                        bool append = false;
                        if (trimmed == _appendCodeCommand)
                            append = true;
                        UserSaveCompleteCode(append);
                    }
                    else if (trimmed == _resetCommand)
                    {
                        Reset();
                    }
                    else if (trimmed == _evaluationCommand || trimmed == _evaluationCommand1
                          || trimmed == _multilineCommand || trimmed == _multilineCommand1
                          || trimmed == _definitionsCommand || trimmed == _definitionsCommand1)
                    {
                        // Perform all pending actions:
                        ExecuteUser(ref inputDefinitions, ref inputCode);
                        evaluationMode = multilineMode = definitionMode = false;
                        if (trimmed == _evaluationCommand || trimmed == _evaluationCommand1)
                            evaluationMode = true;
                        else if (trimmed == _multilineCommand || trimmed == _multilineCommand1)
                        {
                            Console.WriteLine("Multi-line mode. Input commands, {0} or {1} for evaluation.",
                                _evaluationCommand, _multilineCommand);
                            multilineMode = true;
                        }
                        else if (trimmed == _definitionsCommand || trimmed == _definitionsCommand1)
                        {
                            Console.WriteLine("Definitions mode. Input definitions, {0} or {1} to compile.",
                                _evaluationCommand, _definitionsCommand);
                            definitionMode = true;
                        }
                    }
                    else if (trimmed[0] == _commandCharacter)
                    {
                        // input line begins with command character, but command was not recognized,
                        // report this and ignore the input:
                        Console.WriteLine("  Unknown command: " + trimmed);
                    }
                    else if (trimmed[trimmed.Length - 1] == _multiLineCharacter
                  || multilineMode || definitionMode)
                    {
                        // Line ended with '\\' or in multiline mode, add input to concatenated lines read 
                        // since the last complete input:
                        if (definitionMode)
                            inputDefinitions += Environment.NewLine + trimmed.TrimEnd(new char[] { _multiLineCharacter });
                        else  // evaluation or multiline mode
                            inputCode += Environment.NewLine + trimmed.TrimEnd(new char[] { _multiLineCharacter });
                    }
                    else
                    {
                        // Last line of input (code or definitions), append and execute:
                        if (definitionMode)
                            inputDefinitions += Environment.NewLine + trimmed.TrimEnd(new char[] { _multiLineCharacter });
                        else  // evaluation or multiline mode
                            inputCode += Environment.NewLine + trimmed.TrimEnd(new char[] { _multiLineCharacter });
                        ExecuteUser(ref inputDefinitions, ref inputCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error occurred in housekeeping code.");
                    if (!string.IsNullOrEmpty(ex.Message))
                        Console.WriteLine("Details: " + ex.Message);
                    Console.WriteLine();
                }
            }
            Console.Write(_commandLineStopNote);
        }


        #endregion CommandLine







    }  // class ExpressionEvaluatorCompiled

}



