// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/


// SIMPLE COMMAND LINE APPLICATION INTERPRETER

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

using MatrixMathNetNumerics = MathNet.Numerics.LinearAlgebra.Double.DenseMatrix;
using QRDecompositionMathNetNumerics = MathNet.Numerics.LinearAlgebra.Double.Factorization.DenseQR;
using LUDecompositionMathNetNumerics = MathNet.Numerics.LinearAlgebra.Double.Factorization.DenseLU;

using IG.Lib;
using IG.Num;

using AsyncResult = System.Runtime.Remoting.Messaging.AsyncResult;

namespace IG.Lib
{

    /// <summary>Interface for simple command-line applicatino interpreters.</summary>
    public interface ICommandLineApplicationInterpreter
    {

        /// <summary>Name of the current interpreter.</summary>
        string Name
        { get; set; }

        /// <summary>Description of hte current interpreter.</summary>
        string Description
        { get; set; }

        /// <summary>Whether the exit flag is set, usually causing interpreter to stop.</summary>
        bool Exit
        { get; }

        /// <summary>Gets the stopwatch used for measuring time of commands.
        /// <para>This property always returns an initialized stopwatch.</para></summary>
        StopWatch Timer
        { get; }

        /// <summary>Level of output for some of the interpreter's functionality (e.g. asynchronous command execution).</summary>
        int OutputLevel
        { get; set; }

        /// <summary>Specifies whether a wrning should be launched whenever an installed command
        /// is being replaced.</summary>
        bool WarnCommandReplacement
        { get; set; }

        /// <summary>Returns the value of the specified variable of the current command line interpreter.
        /// null is returned if the specified variable does not exist.</summary>
        /// <param name="varName">Name of the variable.</param>
        string GetVariable(string varName);

        /// <summary>Sets the specified variable to the specified value.</summary>
        /// <param name="varName">Name of the variable to be set.</param>
        /// <param name="value">Value that is assigned to the variable.</param>
        /// <returns>New value of the variable (before the method was called).</returns>
        string SetVariable(string varName, string value);

        /// <summary>Clears (removes) the specified variable.</summary>
        /// <param name="varName">Name of the variable to be cleared.</param>
        /// <returns>null.</returns>
        string ClearVariable(string varName);

        /// <summary>Prints the specified variable.</summary>
        /// <param name="varName">Name of the variable to be cleared.</param>
        /// <returns>null.</returns>
        string PrintVariable(string varName);

        /// <summary>Parses a command line and extracts arguments from it.
        /// Arguments can be separated according to usual rules for command-line arguments:
        /// spaces are separators, there can be arbitraty number of spaces, and if we want an
        /// argument to contain spaces, we must enclose it in double quotes.
        /// Command line can also contain the command name followed by arguments. In this case it is treated in the same way, and
        /// command can be obtained simply as the first string in the returned array.</summary>
        /// <param name="commandLine">Command line that is split to individual arguments.
        /// Command line can also contain a command, which is treated equally.</param>
        /// <returns>An array of arguments.</returns>
        string[] GetArguments(string commandLine);

        /// <summary>Runs all commands that are written in a file.
        /// Each line of a file is interpreted as a single command, consisting of command name followed by arguments.</summary>
        /// <param name="filePath">Path to the file containing commands.</param>
        /// <returns>Return value of the last command.</returns>
        string RunFile(string filePath);

        /// <summary>Reads commands one by one from the standard input and executes them.</summary>
        string RunInteractive();

        /// <summary>Returns true if the interpreter contains a command with specified name, false otherwise.</summary>
        /// <param name="commandName">Name of the command whose existence is queried.</param>
        bool ContainsCommand(string commandName);

        /// <summary>Runs the specified command with specified name, installed on the current application object, without any
        /// modifications of the command arguments.</summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <remarks>This method should not be overriden, but the <see cref="Run(string, string[])"/> method can be, e.g. in order to 
        /// perform some argument or command name transformations.</remarks>
        string RunWithoutModifications(string commandName, params string[] commandArguments);

        /// <summary>Runs the command with specified name, installed on the current application object.</summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        string Run(string commandName, string[] args);

        /// <summary>Runs command where the first argument is command name.
        /// Extracts application name and runs the corresponding application delegate.Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.</summary>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        string Run(string[] args);

        /// <summary>Runs a command asynchronously where the first argument is command name.
        /// Extracts command name and runs the corresponding application delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.</summary>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        string RunAsync(string[] args);

        /// <summary>Runs the command with specified name (installed on the current interpreter object) asynchronously.</summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <returns>ID of asynchronous run used to query results and whether command has completed, or -1 if a call was not
        /// launched (actually, an exception would be thrown in this case).</returns>
        string RunAsync(string commandName, params string[] commandArguments);

        /// <summary>Returns true if the asynchronous command execution identified by id has completed, and false otherwise.</summary>
        /// <param name="id">ID of the asynchronous command execution that is querried.</param>
        /// <returns></returns>
        bool AsyncIsCompleted(int id);

        /// <summary>Waits until all asynchronously commands that have been eventually executed by the current 
        /// interpreter, complete.
        /// <para>It is sometimes necessary to call this method if any asynchronous command invocations were made because such 
        /// commands are executed in background threads, which are automatically broken when all foreground threads complete.</para></summary>
        void AsyncWaitAll();

        /// <summary>Waits for the specified asynchronous command (specified by command ID) to complete.</summary>
        /// <param name="callId">ID of the asynchronous command execution.</param>
        /// <returns>Results of the command if the command has not completed before, null otherwise.</returns>
        string AsyncWait(int callId);

        /// <summary>Adds command with the specified name.</summary>
        /// <param name="appName">Name of the commant.</param>
        /// <param name="appMain">Delegate that will be used to execute the command.</param>
        void AddCommand(string appName, CommandLineApplicationInterpreter.ApplicationCommandDelegate appMain);

        /// <summary>Removes the command with the specified name.</summary>
        /// <param name="appName">Name of the commad.</param>
        void RemoveCommand(string appName);

        /// <summary>Removes all commands from the current interpreter.</summary>
        void RemoveAllCommands();

        #region Modules

        /// <summary>Adds a new module to the interpreter. This adds an initialization function (via a delegate)
        /// which is executed when module module initialization is performed.</summary>
        /// <param name="moduleName">Name of the module. When used, module names are case sensitive if commands are case sensitive.</param>
        /// <param name="moduleDelegate">Method that performs module initialization.</param>
        void AddModule(string moduleName, CommandLineApplicationInterpreter.ModuleDelegate moduleDelegate);

        /// <summary>Loads and initializes the specified module.</summary>
        /// <param name="moduleName">Name of the module. It is case sensitive if commands are case sensitive.</param>
        void LoadModule(string moduleName);

        /// <summary>Returns true if the specified module has been loaded on the interpreter,
        /// false if not.</summary>
        /// <param name="moduleName">Name of the module.</param>
        bool IsModuleLoaded(string moduleName);

        #endregion Modules

        #region LoadableScripts

        /// <summary>Interprater based on dynamically loadable scripts.
        /// This enables installation and running of commands that are based on C# code that is 
        /// dynamically compiled. 
        /// Ihe object is created on first get access if it has not been assigned before.
        /// This property can be overridden in derived classes such that getter creates
        /// a dynamically loadable script - based interpreter of another kind. This is important 
        /// because different script loaders (in particuar with different dynamic libraries referenced)
        /// will be used in different contexts. Another possibility is that a custom object is 
        /// assigned to this property, usually in the initialization stage of the current interpreter.</summary>
        /// <exception cref="ArgumentNullException">When set to null reference.</exception>
        /// $A Igor Aug11;
        LoadableScriptInterpreterBase LoadableScriptInterpreter
        { get; set; }

        /// <summary>Dynamically loads (temporarily, just for execution of the current commad) a class 
        /// form the script contained in the specified file and executes its executable method.
        /// The file must contain the script that is dynamically loaded and executed, in form of 
        /// definition of the appropriate class of type <see cref="ILoadableScript"/>. 
        /// The dynamically loadable script class is loaded from the file and instantiated by the
        /// <see cref="LoadableScriptInterpreter"/> interpreter that is based on loadable scripts.</summary>
        /// <param name="scriptFilePath">Path to the file containing loadable script must be the first argument to the method.</param>
        /// <param name="initAndRunArgs">Initialization arguments for the object that will be instantiated
        /// in order to execute the script.</param>
        /// <returns>Result of execution returned by the executable method of the dynamically loaded script object.</returns>
        string RunScriptFile(string scriptFilePath, string[] initAndRunArgs);

        /// <summary>Dynamically loads (compiles and instantiates) a loadable script class contained in the specified file, 
        /// and installs a new command on <see cref="LoadableScriptInterpreter"/> and on the current interpreter, 
        /// based on the dynamically created instance of the loaded (dynamically compiled) class.</summary>
        /// <param name="newCommandName">Name of the newly installed command.</param>
        /// <param name="scriptFilePath">Name of the file containing the script code that defines a loadable script class.</param>
        /// <param name="initArgs">Arguments to the initialization method of the loaded object.
        /// The initialization method will be called before the first call to the executable method of the class,
        /// which takes care of execution of the newly installed command.</param>
        void LoadScript(string newCommandName, string scriptFilePath, string[] initArgs);

        /// <summary>Executes the specified command that has been dynamically loaded form a script.</summary>
        /// <param name="commandName">Name under which the command is installed on the current intepreter
        /// and on interpreder based on dynamically loaded scripts (<see cref="LoadableScriptInterpreter"/>).</param>
        /// <param name="arguments">Arguments to the command.</param>
        /// <returns>Results of command execution.</returns>
        string RunLoadedScript(string commandName, string[] arguments);

        /// <summary>Returns an array of assemblies that are currently referenced by the script loader
        /// that takes care of loading the dynamic scripts.</summary>
        /// <returns></returns>
        string[] GetLoadableScriptReferencedAssemblies();

        #endregion LoadableScripts

    }  // interface ICommandLineApplication


    /// <summary>Carries command execution data, results, and other data such as identification number, etc.
    /// <para>Used as job container for parallel execution of interpreter commands.</para></summary>
    /// <remarks>
    /// <para>Objects of this type contain all data necessary for execution of the specified command by the specified
    /// interpreter. Interpreter to execute the command, command name and arguments, and results of the command
    /// are all stored on the object. </para>
    /// </remarks>
    public class CommandLineJobContainer :
        ParallelJobContainerGen<CommandLineJobContainer, CommandLineJobContainer>,
            IIdentifiable, ILockable
    {

        /// <summary>Prevent calling argument-less constructor.</summary>
        private CommandLineJobContainer()
            : base()
        { }

        /// <summary>Argument-less constructor that is called by all other constructors in order to
        /// properly initialize the data. It sets the input for parallel job container's execution data 
        /// simply to the current object (the object being constructed).</summary>
        /// <param name="interpreter">Interpreter that will execute the command.</param>
        protected CommandLineJobContainer(ICommandLineApplicationInterpreter interpreter)
            : base(Run)
        {
            if (interpreter == null)
                throw new ArgumentException("Interpreter to execute the command is not specified.");
            // Set the interpreter to execute the command:
            this.Interpreter = interpreter;
            this.EvaluationDelegate = Run;
            // Prepare the input for the delegate call:
            this.Input = this;
            this.StartTime = this.CompletionTime = DateTime.Now;
        }

        /// <summary>Constructs a new interpreter command data.</summary>
        /// <param name="interpreter">Interpreter that will execute the command.</param>
        /// <param name="commandName">Name of the command to be executed.</param>
        /// <param name="commandArguments">Arguments to the command.</param>
        public CommandLineJobContainer(ICommandLineApplicationInterpreter interpreter,
            string commandName, string[] commandArguments)
            : this(interpreter)
        {
            if (string.IsNullOrEmpty(commandName))
                throw new ArgumentException("Command name is not specified (null or empty string).");
            this.CommandName = commandName;
            this.CommandArguments = commandArguments;
        }


        /// <summary>Constructs a new interpreter command data.</summary>
        /// <param name="interpreter">Interpreter.</param>
        /// <param name="commandAndArguments">Array containing the command to be executed (first element)
        /// and its eventual arguments.
        /// <para>Must not be null and the first element must not be null.</para></param>
        public CommandLineJobContainer(ICommandLineApplicationInterpreter interpreter,
            string[] commandAndArguments)
            : this(interpreter)
        {
            if (commandAndArguments == null)
                throw new ArgumentNullException("Commandline is not specified (null reference);");
            else if (commandAndArguments.Length < 1)
                throw new ArgumentNullException("Command name is not specified.");
            else
            {
                string[] cmdArgs = new string[commandAndArguments.Length - 1];
                string cmdName = commandAndArguments[0];
                if (string.IsNullOrEmpty(cmdName))
                    throw new ArgumentException("Command name not specifies (null or empty string - 1st array element).");
                for (int i = 1; i < commandAndArguments.Length; ++i)
                {
                    cmdArgs[i - 1] = commandAndArguments[i];
                }
                this.CommandName = cmdName;
                this.CommandArguments = cmdArgs;
            }
        }


        #region IIdentifiable

        private static object _lockIdCommandLine;

        /// <summary>Lock used for acquiring IDs.</summary>
        public static object LockIdCommandLine
        {
            get
            {
                if (_lockIdCommandLine == null)
                {
                    lock (Util.LockGlobal)
                    {
                        if (_lockIdCommandLine == null)
                            _lockIdCommandLine = new object();

                    }
                }
                return _lockIdCommandLine;
            }
        }

        private static int _nextIdCommandLine = 0;

        /// <summary>Returns another ID that is unique for objects of the containing class 
        /// its and derived classes.</summary>
        protected static int GetNextIdCommandLine()
        {
            lock (LockIdCommandLine)
            {
                ++_nextIdCommandLine;
                return _nextIdCommandLine;
            }
        }

        private int _id = GetNextIdCommandLine();

        /// <summary>Unique ID for objects of the currnet and derived classes.</summary>
        public override int Id
        { get { return _id; } }

        #endregion IIdentifiable


        #region Data

        /// <summary>Command-line interpreter that executes the command.</summary>
        public ICommandLineApplicationInterpreter Interpreter;

        /// <summary>name of the command to be executed.</summary>
        public string CommandName;

        /// <summary>Arguments to the command.</summary>
        public string[] CommandArguments;

        /// <summary>Stores the result of hte command when completed.</summary>
        public string CommandResult;

        /// <summary>Time when execution of the command has started.</summary>
        public DateTime StartTime;

        /// <summary>Time when the execution of the command has completed.</summary>
        public DateTime CompletionTime;

        /// <summary>Total execution time of the command, without the overhead generated
        /// by scheduling the command for parallel execution.</summary>
        public double ExecutionTime;

        #endregion Data

        #region Operation

        /// <summary>Executes the command that is represented by the current command data, and
        /// stores the results.</summary>
        protected virtual void RunCommand()
        {
            this.StartTime = DateTime.Now;
            this.CommandResult = Interpreter.Run(CommandName, CommandArguments);
            this.CompletionTime = DateTime.Now;
            this.ExecutionTime = (CompletionTime - StartTime).TotalSeconds;
        }

        /// <summary>Executes the command that is represented by the current command data,
        /// and stores the results.
        /// <para>This method just calls the argument-less <see cref="RunCommand"/> method and is used for execution delegate
        /// for the parallel job container.</para></summary>
        /// <param name="input">Input data. Does not have any effect because data is taken from the current object.</param>
        /// <returns>Resuts. Actually it just returns the current object, since results are stored in this object
        /// by the argument-less <see cref="RunCommand"/> function.</returns>
        protected static CommandLineJobContainer Run(CommandLineJobContainer input)
        {

            //if (input == null)
            //    input = this;
            input.RunCommand();
            return input;
        }

        #endregion Operation

        #region Auxiliary


        /// <summary>Returns the string that represents the command line, where command is followed
        /// by eventual arguments separated by spaces.</summary>
        public string CommandLine()
        {
            lock (Lock)
            {
                return UtilStr.GetCommandLine(CommandName, CommandArguments);
            }
        }

        /// <summary>Returns string representation of the current command container object.</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Parallel command, job ID = " + Id + " (" + State.ToString() + "): ");
            if (State >= ParallelJobState.Executing)
            {
                sb.AppendLine("  Started at " + StartTime);
                if (State >= ParallelJobState.ResultsReady)
                    sb.AppendLine("  Finished at " + CompletionTime + ". Duration: "
                        + ExecutionTime + " s.");
                //else
                //    sb.AppendLine(".");
            }

            sb.Append("  " + CommandLine());
            sb.AppendLine();
            if (Result != null && State >= ParallelJobState.ResultsReady)
                sb.AppendLine("    = '" + Result.CommandResult + "'");
            return sb.ToString();
        }

        #endregion Auxiliary

    }  // class CommandExecutionData



    /// <summary>Simple command-line application interpreters, holds a set of commands that can be executed by name.
    /// Each of these command can take an arbitrary number of string arguments.
    /// Interpreter has its internal variables, which are strings. Each variable has a name and a value.
    /// If any arguments (and even command) start with the '$' character then then it is treated as reference to a variable
    /// and is substituted with the value of that variable (whose name follows the '$' character) before it is used.</summary>
    /// <remarks>This is a case of a very primitive interpreter, which can be used only for finding and executing
    /// commands by name and passing an array of strig arguments to them.</remarks>
    /// $A Igor Nov08;
    public class CommandLineApplicationInterpreter : ICommandLineApplicationInterpreter, ILockable
    {

        /// <summary>Delegate for commands that are installed on interpreter.</summary>
        /// <param name="interpreter">Interpreter on which commad is run. 
        /// Enables access to interpreter internal data from command body.</param>
        /// <param name="commandName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Command return data.</returns>
        public delegate string ApplicationCommandDelegate(ICommandLineApplicationInterpreter interpreter, string commandName, string[] args);

        /// <summary>Delegate for installing a module on the interpreter.</summary>
        /// <param name="modulename">Name of the module.</param>
        /// <param name="interpreter">Interperter where module is installed.</param>
        public delegate void ModuleDelegate(string modulename, ICommandLineApplicationInterpreter interpreter);


        /// <summary>Default value of the flg indicating whether command names are case sensitive.</summary>
        public const bool DefaultCaseSensitive = false;

        protected bool _caseSensitive = false;

        /// <summary>Creates a new MessyApplication object initialized with some basisc applications.
        /// <para>The flag indicating whether interpreter is case sensitive or not is set to <see cref="DefaultCaseSensitive"/></para></summary>
        public CommandLineApplicationInterpreter()
            : this(DefaultCaseSensitive  /* casesensitive */)
        {
        }

        /// <summary>Construct a new commandline interpreter object initialized with some basisc commands.</summary>
        /// <param name="caseSensitive">Flag that specifies whether command names are case sensitive.</param>
        public CommandLineApplicationInterpreter(bool caseSensitive)
        {
            this._caseSensitive = caseSensitive;
            this.AddCommand("Get", CmdGetVariable);
            this.AddCommand("Set", CmdSetVariable);
            this.AddCommand("Clear", CmdClearVariable);
            this.AddCommand("PrintVariable", CmdPrintVariable);
            this.AddCommand("Write", CmdWrite);
            this.AddCommand("WriteLn", CmdWriteLine);
            this.AddCommand("WriteLine", CmdWriteLine);
            this.AddCommand("Run", CmdRunFile);
            this.AddCommand("Try", CmdTryRun);
            this.AddCommand("Repeat", CmdRunRepeat);
            this.AddCommand("RepeatVerbose", CmdRunRepeatVerbose);
            this.AddCommand("SetPriority", CmdSetPriority);
            this.AddCommand("Parallel", CmdRunParallel);
            this.AddCommand("Par", CmdRunParallel);
            this.AddCommand("ParallelRepeat", CmdRunParallelRepeat);
            this.AddCommand("ParRep", CmdRunParallelRepeat);
            this.AddCommand("ParallelPrint", CmdPrintParallelCommands);
            this.AddCommand("ParPrint", CmdPrintParallelCommands);
            this.AddCommand("Async", CmdRunAsync);
            this.AddCommand("RunAsync", CmdRunAsync);
            this.AddCommand("AsyncWait", CmdAsyncWaitResults);
            this.AddCommand("AsyncIsCompleted", CmdAsyncCompleted);
            this.AddCommand("Sleep", CmdSleepSeconds);
            this.AddCommand("ThrowExceptions", CmdThtrowExceptions);
            this.AddCommand("Interactive", CmdRunInteractive);
            this.AddCommand("Int", CmdRunInteractive);
            this.AddCommand("System", CmdRunSystem);
            this.AddCommand("Sys", CmdRunSystem);

            this.AddCommand("Calc", CmdExpressionEvaluatorInteractive);

            this.AddCommand("Exit", CmdExit);
            this.AddCommand("?", CmdHelp);
            this.AddCommand("Help", CmdHelp);
            this.AddCommand("About", CmdAbout);
            this.AddCommand("ApplicationInfo", CmdApplicationInfo);
            this.AddCommand("AppInfo", CmdApplicationInfo);
            this.AddCommand("C", CmdComment);
            this.AddCommand("Comment", CmdComment);
            this.AddCommand("//", CmdComment);
            this.AddCommand("PrintCommands", CmdPrintCommands);

            this.AddCommand("PipeServer", CmdPipeServerCreate);
            this.AddCommand("PipeServersRemove", CmdPipeServersRemove);
            this.AddCommand("PipeServerInfo", CmdPipeServerInfo);

            this.AddCommand("PipeClient", CmdPipeClientCreate);
            this.AddCommand("PipeClientsRemove", CmdPipeClientsRemove);
            this.AddCommand("PipeClientInfo", CmdPipeClientInfo);
            this.AddCommand("PipeClientSend", CmdPipeClientGetServerResponse);


            this.AddCommand("Module", CmdLoadModule);
            this.AddCommand("LoadModule", CmdLoadModule);
            this.AddCommand("IsModuleLoaded", CmdIsModuleLoaded);
            this.AddCommand("Loaded", CmdIsModuleLoaded);

            this.AddCommand("RunInternal", CmdRunInternalScriptClass);
            this.AddCommand("Internal", CmdRunInternalScriptClass);
            this.AddCommand("RunScript", CmdRunScriptFile);
            this.AddCommand("LoadClass", CmdLoadScript);
            this.AddCommand("RunClass", CmdRunLoadedScript);
            this.AddCommand("WriteAssemblies", WriteLoadableScriptReferencedAssemblies);

            // Test commands and modules:
            this.AddCommand("TestProduct", CmdTestProduct);
            this.AddCommand("Test", CmdTest);
            this.AddCommand("TestSpeed", CmdTestSpeed);
            this.AddCommand("TestSpeedLong", CmdTestSpeedLong);
            this.AddCommand("TestQR", CmdTestQR);
            this.AddCommand("TestLU", CmdTestLU);

            this.AddModule("Test1", ModuleTest1);
            this.AddModule("Test2", ModuleTest2);

        }


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking


        #region Data

        protected static List<CommandLineApplicationInterpreter> _interpreters = new List<CommandLineApplicationInterpreter>();

        protected SortedDictionary<string, ApplicationCommandDelegate> _commands = new SortedDictionary<string, ApplicationCommandDelegate>();

        protected SortedDictionary<string, string> _variables = new SortedDictionary<string, string>();

        protected SortedDictionary<string, ModuleDelegate> _modules = new SortedDictionary<string, ModuleDelegate>();

        protected List<String> _loadedModules = new List<string>();

        /// <summary>Default interpreter name.</summary>
        public const string DefaultName = "ApplicationIntepreter";

        public const string AutoGlobalName = "AutomaticGlobalInterpreter";

        public const string AutoGlobalDescription = "Automatically created global command-line interpreter.";


        protected string _name = DefaultName;

        protected string _sescription = "Application's command-line interpreter.";

        protected string _description;

        protected char variableStart = '$';

        protected bool _exit = false;

        /// <summary>Whether the exit flag is set, usually causing interpreter to stop.</summary>
        public bool Exit
        {
            get { lock (Lock) { return _exit; } }
            protected set { lock (Lock) { _exit = value; } }
        }

        public static List<CommandLineApplicationInterpreter> Interpreters
        {
            get { return _interpreters; }
        }

        /// <summary>Global command-line interpreter.
        /// <para>This returns the first interpreter created in the application, or a new interpreter if no 
        /// interpreters have been created yet.</para></summary>
        public static CommandLineApplicationInterpreter Global
        {
            get
            {
                lock (Interpreters)
                {
                    if (Interpreters.Count > 0)
                    {
                        CommandLineApplicationInterpreter interp = new CommandLineApplicationInterpreter();
                        interp.Name = AutoGlobalName;
                        interp.Description = AutoGlobalDescription;
                    }
                    return Interpreters[0];
                }
            }
        }


        /// <summary>Name of the current interpreter.</summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>Description of hte current interpreter.</summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public StopWatch _timer;

        /// <summary>Gets the stopwatch used for measuring time of commands.
        /// <para>This property always returns an initialized stopwatch.</para></summary>
        public StopWatch Timer
        {
            get { if (_timer == null) _timer = new StopWatch(); return _timer; }
        }

        public static int DefaultOutputLevel = 1;

        /// <summary>Default level of output for some of the interpreters' functionality (e.g. asynchronous command execution).</summary>
        protected int _outputLevel = DefaultOutputLevel;

        /// <summary>Level of output for some of the interpreter's functionality (e.g. asynchronous command execution).</summary>
        public int OutputLevel
        {
            get { return _outputLevel; }
            set { _outputLevel = value; }
        }

        public static bool DefaultWarnCommandReplacement = true;

        private bool _warnCommandReplacement = DefaultWarnCommandReplacement;

        /// <summary>Specifies whether a wrning should be launched whenever an installed command
        /// is being replaced.</summary>
        public bool WarnCommandReplacement
        { get { return _warnCommandReplacement; } set { _warnCommandReplacement = value; } }

        #endregion Data


        #region Operation


        /// <summary>Returns the value of the specified variable of the current command line interpreter.
        /// null is returned if the specified variable does not exist.</summary>
        /// <param name="varName">Name of the variable.</param>
        public virtual string GetVariable(string varName)
        {
            lock (Lock)
            {
                if (string.IsNullOrEmpty(varName))
                    throw new ArgumentException("Interpreter variable name not specified.");
                if (!_variables.ContainsKey(varName))
                    throw new ArgumentException("Interpreter variable not defined: " + varName + ".");
                return _variables[varName];
            }
        }


        /// <summary>Returns true if the specified string represents a variable reference, false otherwise.</summary>
        /// <param name="str">String that is checked.</param>
        protected virtual bool IsVariableReference(string str)
        {
            if (str != null)
                if (str.Length > 1)
                    if (str[0] == variableStart)
                        return true;
            return false;
        }

        /// <summary>Returns value of the referenced variable if the specified string represents a 
        /// variable reference (begins with the variableStart character, usually '$'), otherwise the 
        /// original sting is returned.</summary>
        /// <param name="str">String that is eventually substituted by variable value in the case that it 
        /// represents a variable reference.</param>
        protected virtual string SubstituteVariableReference(string str)
        {
            if (str == null)
                return str;
            else if (str.Length < 2)
                return str;
            else if (str[0] != variableStart)
                return str;
            else return GetVariable(str.Substring(1, str.Length - 1));
        }

        /// <summary>Sets the specified variable to the specified value.</summary>
        /// <param name="varName">Name of the variable to be set.</param>
        /// <param name="value">Value that is assigned to the variable.</param>
        /// <returns>New value of the variable.</returns>
        public virtual string SetVariable(string varName, string value)
        {
            lock (Lock)
            {
                if (string.IsNullOrEmpty(varName))
                    throw new ArgumentException("Interpreter variable to be set not specified.");
                _variables[varName] = value;
                return value;
            }
        }

        /// <summary>Clears (removes) the specified variable.</summary>
        /// <param name="varName">Name of the variable to be cleared.</param>
        /// <returns>null.</returns>
        public virtual string ClearVariable(string varName)
        {
            lock (Lock)
            {
                if (string.IsNullOrEmpty(varName))
                    throw new ArgumentException("Interpreter variable to be removed not specified.");
                _variables.Remove(varName);
                return null;
            }
        }


        /// <summary>Prints the specified variable.</summary>
        /// <param name="varName">Name of the variable to be cleared.</param>
        /// <returns>null.</returns>
        public virtual string PrintVariable(string varName)
        {
            Console.WriteLine("  " + varName + " = " + GetVariable(varName));
            return null;
        }


        /// <summary>Parses a command line and extracts arguments from it.
        /// Arguments can be separated according to usual rules for command-line arguments:
        /// spaces are separators, there can be arbitraty number of spaces, and if we want an
        /// argument to contain spaces, we must enclose it in double quotes.
        /// Command line can also contain the command name followed by arguments. In this case it is treated in the same way, and
        /// command can be obtained simply as the first string in the returned array.</summary>
        /// <param name="commandLine">Command line that is split to individual arguments.
        /// Command line can also contain a command, which is treated equally.</param>
        /// <returns>An array of arguments.</returns>
        public virtual string[] GetArguments(string commandLine)
        {
            return UtilStr.GetArgumentsArray(commandLine);
        }

        /// <summary>Runs all commands that are written in a file.
        /// Each line of a file is interpreted as a single command, consisting of command name followed by arguments.</summary>
        /// <param name="filePath">Path to the file containing commands.</param>
        /// <returns>Return value of the last command.</returns>
        public virtual string RunFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Path of the file to be run by a command-line interpreter is not specified.");
            else if (!File.Exists(filePath))
                throw new ArgumentException("File to be run by a command-line interpreter does not exist. File path: "
                    + Environment.NewLine + "  " + filePath);
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                string ret = null;
                int lineNum = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (Exit)
                        return ret;
                    ++lineNum;
                    try
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] commandLineSplit = GetArguments(line);
                            ret = Run(commandLineSplit);
                        }
                    }
                    catch (Exception ex)
                    {
                        Exception exRethrown = new InvalidOperationException("Error in line " + lineNum + ", interpreted file: "
                            + Environment.NewLine + "  " + filePath + Environment.NewLine
                            + "  Details: " + ex.Message, ex /* innerException */);
                        throw (exRethrown);
                    }
                }
                return ret;
            }
        }

        private bool _throwExceptions = false;

        /// <summary>Flag specifying whether exceptions are thrown in interactive mode or not.
        /// <para>If false then only error messages are written to console, but exceptions are not
        /// rethrown.</para></summary>
        /// <remarks><para>In some modes of operation, exceptions thrown in commands executed by
        /// the interpreter are cought, user is notified about the exception (usually by writing 
        /// the error message to the console), but exceptions are not re-thrown. Because of this
        /// behavior, much information about exceptions is lost (e.g. the stack trace).</para>
        /// </remarks>
        public virtual bool ThrowExceptions
        {
            get { return _throwExceptions; }
            set { _throwExceptions = value; }
        }

        /// <summary>Reads commands one by one from the standard input and executes them.</summary>
        public virtual string RunInteractive()
        {
            Console.WriteLine();
            Console.WriteLine("Interactive interpreter started.");
            Console.WriteLine("Insert commands line by line. Press Enter to exit.");
            Console.WriteLine();
            bool doExit = false;
            string line;
            string ret = null;
            while (!doExit)
            {
                if (Exit)
                {
                    Console.WriteLine("Exiting interpreter...");
                    Console.WriteLine("Bye.");
                    return ret;
                }
                Console.Write("Cmd>");
                line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    try
                    {
                        Console.WriteLine();
                        Console.Write("Exit the interactive interpreter (0/1)? ");
                        UtilConsole.Read(ref doExit);
                        if (doExit)
                        {
                            Console.WriteLine("Exiting interactive mode...");
                            if (!AsyncIsAllCompleted())
                            {
                                bool waitAsync = true;
                                Console.WriteLine();
                                Console.Write("Wait for asynchronous jobs to complete (0/1)? ");
                                UtilConsole.Read(ref waitAsync);
                                if (waitAsync)
                                {
                                    AsyncWaitAll();
                                }
                            }
                            Console.WriteLine("Bye." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: " + ex.Message);
                        //if (ThrowExceptions)
                        //    throw;
                    }
                }
                else
                {
                    try
                    {
                        string[] commandLineSplit = GetArguments(line);
                        ret = this.Run(commandLineSplit);
                        string retOutput = ret;
                        if (retOutput == null)
                            retOutput = "null";
                        else if (retOutput == "")
                            retOutput = "\"\"";
                        Console.WriteLine("  = " + retOutput);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ERROR: " + ex.Message);
                        Console.WriteLine();
                        if (ThrowExceptions)
                            throw;
                    }
                }
            }
            return ret;
        }


        /// <summary>Executes the specified system commmand and blocks until the execution completes.</summary>
        /// <param name="args">Array of strings where the first element is command to be executed, and the subsequent
        /// elements are command-line arguments.</param>
        public static void ExecuteSystemCommand(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException("Command and arguments are not specified (null table).");
            if (args.Length < 1)
                throw new ArgumentNullException("Command and arguments are not specified (table has no elements).");
            string command = args[0];
            string[] commandArgs = new string[args.Length - 1];
            for (int i = 1; i < args.Length; ++i)
                commandArgs[i - 1] = args[i];
            ExecuteSystemCommand(command, commandArgs);
        }

        /// <summary>Executes system command with arguments.</summary>
        /// <param name="command">Command string, usually a path to executable or other type of command.</param>
        /// <param name="args">Arguments to system command.</param>
        public static void ExecuteSystemCommand(string command, params string[] args)
        {
            string workingDirectory = null;
            bool asynchronous = false;
            bool useShell = false;
            bool createNoWindow = false;
            string redirectedOutputPath = null;
            bool redirectStandardOutput = false;
            UtilSystem.ExecuteSystemCommand(workingDirectory, asynchronous, useShell,
                createNoWindow, redirectedOutputPath, redirectStandardOutput,
                command, args);
            // UtilSystem.ExecuteSystemCommand(command, args);
        }

        /// <summary>Reads commands with their arguments ont by one from the console and 
        /// executes them as system commands.</summary>
        protected virtual void ExecuteSystemCommandsInteractive()
        {
            Console.WriteLine();
            Console.WriteLine("Runing system commands.");
            Console.WriteLine("Insert commands line by line. Press Enter to exit.");
            Console.WriteLine();
            bool doExit = false;
            string line;
            while (!doExit)
            {
                if (Exit)
                {
                    Console.WriteLine("Exiting system command-line interpreter...");
                    Console.WriteLine("Bye.");
                    return;
                }
                Console.Write("System>");
                line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    try
                    {
                        Console.WriteLine();
                        Console.Write("Exit the system command-line interpreter (0/1)? ");
                        UtilConsole.Read(ref doExit);
                        if (doExit)
                        {
                            Console.WriteLine("Exiting system command-line interpreter...");
                            Console.WriteLine("Bye." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: " + ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        string[] commandLineSplit = GetArguments(line);
                        ExecuteSystemCommand(commandLineSplit);
                        //if (!string.IsNullOrEmpty(ret))
                        //{
                        //    Console.WriteLine("  = " + ret);
                        //}
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ERROR: " + ex.Message);
                        Console.WriteLine();
                    }
                }
            }
        }

        // Expression evaluator:

        protected ExpressionEvaluatorJs _expressionEvaluator;

        /// <summary>Expression evaluator used by the current </summary>
        public ExpressionEvaluatorJs ExpressionEvaluator
        {
            get
            {
                lock (Lock)
                {
                    if (_expressionEvaluator == null)
                    {
                        _expressionEvaluator = new ExpressionEvaluatorJs();
                    }
                    return _expressionEvaluator;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    _expressionEvaluator = value;
                }
            }
        }

        /// <summary>Runs interpreter's expression evaluator interactively.</summary>
        public string ExpressionEvaluatorInteractive()
        {
            ExpressionEvaluator.CommandLine();
            return null;
        } 

        public string ExpressionEvaluatorEvaluate(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            if (args != null)
            {
                for (int i = 0; i < args.Length; ++i)
                    sb.Append(args[i] + " ");
            }
            return ExpressionEvaluator.Execute(sb.ToString());
        }


        /// <summary>Returns an array of installed commands.</summary>
        /// <remarks>The returned array is created anew and command names are copied to it from a collection of keys
        /// of a sorted dictionary (type <see cref="SortedDictionary{T, T}"/>).</remarks>
        public string[] GetCommands()
        {
            lock (Lock)
            {
                SortedDictionary<string, ApplicationCommandDelegate>.KeyCollection keys = _commands.Keys;
                int numKeys = keys.Count;
                string[] ret = new string[numKeys];
                int ind = 0;
                foreach (string currentKey in keys)
                {
                    ret[ind] = currentKey;
                    ++ind;
                }
                return ret;
            }
        }


        /// <summary>Returns true if the interpreter contains a command with specified name, false otherwise.</summary>
        /// <param name="commandName">Name of the command whose existence is queried.</param>
        public virtual bool ContainsCommand(string commandName)
        {
            lock (Lock)
            {
                if (!_caseSensitive)
                    commandName = commandName.ToLower();
                return _commands.ContainsKey(commandName);
            }
        }
        
        /// <summary>Runs command several times where the first argument is number of repetitions, second argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.
        /// <para>The interpreter's output level is used.</para></summary>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunRepeat(string[] args)
        {
            return RunRepeat(OutputLevel, args);
        }
        
        /// <summary>Runs command several times where the first argument is number of repetitions, second argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.
        /// <para>Output level 3 is used, such that all information is output to console.</para></summary>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunRepeatVerbose(string[] args)
        {
            return RunRepeat(3, args);
        }
        
        /// <summary>Runs command several times where the first argument is number of repetitions, second argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.
        /// <para>Output level 0 is used, such that no information is output to console.</para></summary>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunRepeatSilent(string[] args)
        {
            return RunRepeat(0, args);
        }

        /// <summary>Runs command several times where the first argument is number of repetitions, second argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.
        /// <para>Output level is defined by the first argument. Level 0 means no output, level 1 means that summary is written to 
        /// the console, and level e means that a note is printed before and afterr each repetition starts.</para></summary>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunRepeatSpecificOutputLevel(string[] args)
        {
            int whichArgLevel = 1;
            int outputLevel = int.Parse(args[whichArgLevel]);
            List<string> arguments = new List<string>();
            int numArgsOriginal = args.Length;
            for (int i = 0; i < numArgsOriginal; ++i)
            {
                if (i != whichArgLevel)
                    arguments.Add(args[i]);
            }
            return RunRepeat(outputLevel, arguments.ToArray());
        }


        /// <summary>Runs command several times where the first argument is number of repetitions, second argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.</summary>
        /// <param name="outputLevel">Level of output of the command.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunRepeat(int outputLevel, string[] args)
        {
            lock (Lock)
            {
                if (args == null)
                    throw new ArgumentNullException("Commandline is not specified (null reference);");
                else if (args.Length < 2)
                    throw new ArgumentNullException("Number of repetitions and command name are not specified.");
                else
                {
                    string[] cmdArgs = new string[args.Length - 2];
                    int numRepetitions = int.Parse(args[0]);
                    string cmdName = args[1];
                    for (int i = 2; i < args.Length; ++i)
                    {
                        cmdArgs[i - 2] = args[i];
                    }
                    string ret = "";
                    StopWatch t = new StopWatch();
                    int threadId = Thread.CurrentThread.GetHashCode();
                    for (int i = 1; i <= numRepetitions; ++i)
                    {
                        if (outputLevel >= 2)
                        {
                            Console.WriteLine(Environment.NewLine + "Repeatively running command "
                                + i + " / " + numRepetitions + " (thread " + threadId + ") ...");
                        }
                        t.Start();
                        ret = ret + " " + Run(cmdName, cmdArgs);
                        t.Stop();
                        if (outputLevel >= 2)
                        {
                            Console.WriteLine(Environment.NewLine + "... repetition "
                                + i + "/" + numRepetitions + " (thread " + threadId + ") fininshed in " + t.Time + " s.");
                        }
                    }
                    double numPerSecond = (double)numRepetitions / t.TotalTime;
                    double numPerSecondCPU = (double) numRepetitions / (t.TotalCpuTime + 1.0e-20);
                    ret = numPerSecond.ToString();
                    if (outputLevel >= 1)
                    {
                        Console.WriteLine(Environment.NewLine +  numRepetitions + " repetitions of command " + cmdName
                            + " finished in " + t.TotalTime + " s (CPU: " + t.TotalCpuTime + " s)."
                            + Environment.NewLine + "Number of executions per second: " + numPerSecond + " (CPU: " + numPerSecondCPU + ").");

                    }
                    return ret;
                }
            }
        }

        /// <summary>Runs command in a try-catch block, where first argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.</summary>
        /// <param name="outputLevel">Level of output of the command.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunTryCatch(int outputLevel, string[] args)
        {
            lock (Lock)
            {
                if (args == null)
                    throw new ArgumentNullException("Commandline is not specified (null reference);");
                else if (args.Length < 1)
                    throw new ArgumentNullException("Command is not not specified.");
                else
                {
                    string[] cmdArgs = new string[args.Length - 1];
                    string cmdName = args[0];
                    for (int i = 1; i < args.Length; ++i)
                    {
                        cmdArgs[i - 1] = args[i];
                    }
                    string ret = "";
                    try
                    {
                        if (outputLevel >= 2)
                        {
                            Console.WriteLine(Environment.NewLine + "Runninng command " + cmdName  + " in a try-catch block ...");
                        }
                        ret = Run(cmdName, cmdArgs);
                        if (outputLevel >= 2)
                        {
                            Console.WriteLine(Environment.NewLine + "... command run successfully." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        ret = "ERROR: " + ex.Message;
                        if (outputLevel >= 1)
                            Console.WriteLine(Environment.NewLine + "ERROR: " + ex.Message + Environment.NewLine);
                    }
                    return ret;
                }
            }
        }


        /// <summary>Runs the specified command with specified name, installed on the current application object, without any
        /// modifications of the command arguments.</summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <remarks>This method should not be overriden, but the <see cref="Run(string, string[])"/> method can be, e.g. in order to 
        /// perform some argument or command name transformations.</remarks>
        public string RunWithoutModifications(string commandName, params string[] commandArguments)
        {
            ApplicationCommandDelegate appDelegate = null;
            lock (Lock)
            {
                if (string.IsNullOrEmpty(commandName))
                    throw new ArgumentException("Command name is not specified.");
                // Perform substitution of variables with their values in command name and argumnets:
                commandName = SubstituteVariableReference(commandName);
                bool substituteArgs = false;
                if (commandArguments != null)
                {
                    for (int i = 0; i < commandArguments.Length; ++i)
                    {
                        if (IsVariableReference(commandArguments[i]))
                        {
                            substituteArgs = true;
                            break;
                        }
                    }
                    if (substituteArgs)
                    {
                        string[] argsSubstituted = new string[commandArguments.Length];
                        for (int i = 0; i < commandArguments.Length; ++i)
                            argsSubstituted[i] = SubstituteVariableReference(commandArguments[i]);
                        commandArguments = argsSubstituted;
                    }
                }
                if (!_caseSensitive)
                    commandName = commandName.ToLower();
                if (!_commands.ContainsKey(commandName))
                    throw new ArgumentException("Interpreter does not contain the following command: \"" + commandName + "\".");
                appDelegate = _commands[commandName];
            }  // lock
            if (appDelegate == null)
            {
                throw new InvalidOperationException("Can not find command named " + commandName + ".");
            }
            else
            {
                return appDelegate(this, commandName, commandArguments);
            }
        }


        /// <summary>Runs the specified command with specified name, installed on the current application object.</summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        public virtual string Run(string commandName, params string[] commandArguments)
        {
            return RunWithoutModifications(commandName, commandArguments);
        }

        /// <summary>Runs command where the first argument is command name.
        /// Extracts command name and runs the corresponding command delegate.Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.</summary>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public string Run(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException("Commandline is not specified (null reference);");
            else if (args.Length < 1)
                throw new ArgumentNullException("Command name is not specified.");
            else
            {
                string[] cmdArgs = new string[args.Length - 1];
                string cmdName = args[0];
                for (int i = 1; i < args.Length; ++i)
                {
                    cmdArgs[i - 1] = args[i];
                }
                return Run(cmdName, cmdArgs);
            }
        }

        #region ParallelExecution


        protected List<CommandLineJobContainer> _parallelCommands;

        /// <summary>List where parallel commands are stored.</summary>
        public List<CommandLineJobContainer> ParallelCommands
        {
            get
            {
                lock (Lock)
                {
                    if (_parallelCommands == null)
                        _parallelCommands = new List<CommandLineJobContainer>();
                    return _parallelCommands;
                }
            }
        }


        /// <summary>Adds a new parallel command data to the list of commands executed by in parallel.</summary>
        /// <param name="commandData">Command data object that is added to the list of barallelly executed commands.</param>
        public void AddParallelCommand(CommandLineJobContainer commandData)
        {
            lock (Lock)
            {
                ParallelCommands.Add(commandData);
            }
        }


        /// <summary>Returns the command data object of the parallel command that with the specified ID.</summary>
        /// <param name="id">Identification nuber of the job container that carries command data and executes it.</param>
        public CommandLineJobContainer GetParallelCommandData(int id)
        {
            lock (Lock)
            {
                foreach (CommandLineJobContainer data in ParallelCommands)
                {
                    if (data != null)
                        if (data.Id == id)
                            return data;
                }
            }
            return null;
        }


        /// <summary>Prints the commands that were scheduled for parallel execution, together with their
        /// current status, results and execution times (when available).</summary>
        /// <param name="printAll">Whether all commands are printed (included those already completed).
        /// If false then only completed commands are printed.</param>
        public void PrintParallelCommands(bool printAll)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("System for parallel execution of commands:");
            ParallelJobDispatcherGen<CommandLineJobContainer, CommandLineJobContainer> dispatcher = _parallelDispatcher;
            if (dispatcher == null)
                sb.AppendLine("Parallel dispatcher is not initialized.");
            else
            {
                sb.AppendLine("  Number of parallel servers: " + dispatcher.NumJobServers);
                sb.AppendLine("  Number of idle servers:     " + dispatcher.NumIdleJobServers);
                sb.AppendLine("  Commands: ");
                sb.AppendLine("    Total scheduled for execution: " + dispatcher.NumSentJobs);
                sb.AppendLine("    Finished: " + dispatcher.NumFinishedJobs);
                sb.AppendLine("    Executing: " + dispatcher.NumExecutingJobs);
                sb.AppendLine("    Enqueued: " + dispatcher.NumEnqueuedJobs);
            }
            sb.AppendLine();
            if (printAll)
                sb.AppendLine("List of all parallel commands:");
            else
                sb.AppendLine("List of unfinished parallel commands:");
            lock (Lock)
            {
                if (_parallelCommands == null)
                    sb.AppendLine("  The list of parallel commands is empty.");
                else if (_parallelCommands.Count < 0)
                    sb.AppendLine("  The list of parallel commands is empty.");
                else
                {
                    int numUnfinished = 0, numAll = 0, numNull = 0;
                    foreach (CommandLineJobContainer container in _parallelCommands)
                    {
                        if (container == null)
                        {
                            ++numNull;
                            if (printAll)
                                sb.AppendLine("null container.");
                        }
                        else
                        {
                            ++numAll;
                            if (container.State < ParallelJobState.ResultsReady)
                                ++numUnfinished;
                            if (container.State < ParallelJobState.ResultsReady || printAll)
                            {
                                sb.AppendLine();
                                sb.Append(container.ToString());
                            }
                        }
                    }
                    if (printAll && numAll < 1)
                        sb.AppendLine("    No commands in the list.");
                    else if (!printAll && numUnfinished < 1)
                        sb.AppendLine("    No unfinished commands.");
                    sb.AppendLine();
                    sb.AppendLine("Total number of parallel commands: " + numAll);
                    sb.AppendLine("             Number of unfinished: " + numUnfinished);
                    if (numNull > 0)
                        sb.AppendLine("                   Number of null: " + numNull);
                }
            }
            sb.AppendLine();
            Console.WriteLine(sb.ToString());
        }


        private ParallelJobDispatcherGen<CommandLineJobContainer, CommandLineJobContainer> _parallelDispatcher;

        /// <summary>Parallel job dispatcher that is responsible for parallel execution of commands.</summary>
        protected internal ParallelJobDispatcherGen<CommandLineJobContainer, CommandLineJobContainer>
            ParallelDispatcher
        {
            get
            {
                lock (Lock)
                {
                    if (_parallelDispatcher == null)
                    {
                        _parallelDispatcher = new ParallelJobDispatcherGen<CommandLineJobContainer, CommandLineJobContainer>();
                        _parallelDispatcher.ThreadPriority = this._threadPriority;
                    }
                }
                return _parallelDispatcher;
            }
        }


        #region ThreadPriority



        protected ThreadPriority _threadPriority = UtilSystem.ThreadPriority;

        /// <summary>Priority of the current interpreter main thread and threads for executing the
        /// parallel commands.
        /// <para>Setting priority changes priority of the threads.</para></summary>
        public ThreadPriority ThreadPriority
        {
            get { lock (Lock) { return _threadPriority; } }
            set
            {
                lock (Lock)
                {
                    if (value != _threadPriority)
                    {
                        _threadPriority = value;
                        Thread.CurrentThread.Priority = value;
                        if (_parallelDispatcher != null)
                            _parallelDispatcher.ThreadPriority = value;
                    }
                }
            }
        }


        /// <summary>Updates thread priority (property <see cref="ThreadPriority"/>) of the interpreter to the current global 
        /// thread priority (the <see cref="UtilSystem.ThreadPriority"/> property).</summary>
        public virtual void UpdateThreadPriorityFromSystem()
        {
            ThreadPriority = UtilSystem.ThreadPriority;
        }

        /// <summary>Whether the "event" handler for system priprity changes has already been registered.</summary>
        protected bool _systemPriorityUpdatesRegistered = false;

        /// <summary>Registers the <see cref="UpdateThreadPriorityFromSystem"/> method as "event handler"
        /// for system priority changes. After registration, this method will be called every time the value 
        /// of the <see cref="UtilSystem.ThreadPriority"/> property changes.</summary>
        public void RegisterSystemPriorityUpdating()
        {
            bool doRegister = false;
            lock (Lock)
            {
                doRegister = !_systemPriorityUpdatesRegistered;
            }
            if (doRegister)
            {
                UtilSystem.AddOnThreadPriorityChange(UpdateThreadPriorityFromSystem);
                lock (Lock)
                {
                    _systemPriorityUpdatesRegistered = true;
                }
            }
        }


        /// <summary>Unregisters the <see cref="UpdateThreadPriorityFromSystem"/> method as "event handler"
        /// for system priority changes.</summary>
        /// <seealso cref="RegisterSystemPriorityUpdating"/>
        public void UnregisterSystemPriorityUpdating()
        {
            try
            {
                UtilSystem.RemoveOnThreadPriorityChange(UpdateThreadPriorityFromSystem);
                lock (Lock)
                {
                    _systemPriorityUpdatesRegistered = false;
                }
            }
            catch { }
        }


        #endregion ThreadPriority


        protected int _maxNumParallelServers = 0;

        /// <summary>Maximal number of parallel servers allowed to be created for parallel command execution.
        /// <para>If less than 1 then creation of unlimited number of servers is allowed. As long as the number
        /// is not exceeded, new servers will be created whenever a new parallel jobs are created and no servers
        /// are idle, in order to start execution of new parallel jobs immediately.</para></summary>
        public int MaxNumParallelServers
        {
            get { return _maxNumParallelServers; }
            set
            {
                lock (Lock)
                {
                    _maxNumParallelServers = value;
                }
            }
        }


        /// <summary>Runs the specified command with arguments the specified number of times
        /// in parallel threads by using the interpreter's job dispatcher with corresponding parallel servers.</summary>
        /// <param name="numRepeat">Number of parallel executions of the command that are started.</param>
        /// <param name="command">Command that is run in each of the parallel threads.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <returns>An array of identification numbers of parallel jobs that take over execution of the command.
        /// Queries about jobs progress and results can be made by using this identification number.</returns>
        /// <remarks><para>Parallel commands are scheduled for execution by interpreter's dispathcer (the 
        /// <see cref="ParallelDispatcher"/> property), which delegates execution to its parallel servers.</para>
        /// <para>Before transferring them to the dispatcher, commands are wrapped into job container object
        /// of type <see cref="CommandLineJobContainer"/>. Job containers are stored in a list, such that execution 
        /// status and results can be accessed through contaier ID (which is returned by the method).</para>
        /// <para>New parallel servers are dynamically added to the dispatcher if there are no idle servers 
        /// to execute the parallel command immediately at the time it is called. However, the maximal number of 
        /// parallel servers can be limited by the <see cref="MaxNumParallelServers"/> property.</para>
        /// </remarks>
        public int[] RunParallelRepeat(int numRepeat, string command, string[] commandArguments)
        {
            if (numRepeat < 1)
                throw new ArgumentException("Number of repetitions can not be less than 1.");
            else if (string.IsNullOrEmpty(command))
                throw new ArgumentException("Command to be executed in parallel is not specified (null or empty string).");
            int[] ret = new int[numRepeat];

            Console.WriteLine("Executing command in " + numRepeat + " parallel threads...");

            ParallelJobDispatcherGen<CommandLineJobContainer, CommandLineJobContainer> dispatcher;
            lock (Lock)
            {
                dispatcher = ParallelDispatcher;
                if (dispatcher.NumIdleJobServers < numRepeat && (_maxNumParallelServers < 1
                    || dispatcher.NumJobServers < _maxNumParallelServers))
                {
                    // If the number of currently idle servers is smaller than the number of parallel jobs scheduled, 
                    // then create more servers if this is allowed, such that all jobs will start immediatrly.
                    lock (dispatcher.Lock)
                    {
                        while (dispatcher.NumIdleJobServers < numRepeat && (_maxNumParallelServers < 1
                            || dispatcher.NumJobServers < _maxNumParallelServers))
                        {
                            ParallelJobServerGen<CommandLineJobContainer, CommandLineJobContainer> server =
                                new ParallelJobServerGen<CommandLineJobContainer, CommandLineJobContainer>();
                            dispatcher.AddServer(server);
                        }
                    }
                }


                Console.WriteLine("Dispatcher acquired and servers prepared.");

            }
            for (int iJob = 0; iJob < numRepeat; ++iJob)
            {
                CommandLineJobContainer container = new CommandLineJobContainer(this, command, commandArguments);
                // container.Input = container;
                container.ClientJobId = container.Id;
                ret[iJob] = container.Id;
                // Set callback for job aborted event by anonymous function:
                container.OnAbortedGeneric = delegate(ParallelJobContainerGen<CommandLineJobContainer, CommandLineJobContainer>
                    jobContaier)
                {
                    Console.WriteLine(Environment.NewLine + "ERROR: Parallel interpreter command aborted. "
                        + Environment.NewLine + "  job ID: " + container.Id
                        + Environment.NewLine + "  commad: " + container.CommandName
                        + Environment.NewLine);
                    throw new InvalidOperationException("Parallel interpreter command aborted. "
                        + Environment.NewLine + "  job ID: " + container.Id
                        + Environment.NewLine + "  commad: " + container.CommandName);
                };
                // Set callback for job started event:
                container.OnStartedGeneric = delegate(ParallelJobContainerGen<CommandLineJobContainer, CommandLineJobContainer> jobContaier)
                {
                    if (OutputLevel >= 1)
                    {
                        Console.WriteLine("Parallel command started, ID = " + container.Id.ToString() + ".");
                    }
                };
                // Set callback for job finished event:
                container.OnFinishedGeneric = delegate(ParallelJobContainerGen<CommandLineJobContainer, CommandLineJobContainer> jobContaier)
                {
                    if (OutputLevel >= 1)
                    {
                        string str = "Parallel command finished, ID = " + container.Id.ToString() + ".";
                        if (OutputLevel >= 2)
                        {
                            str += Environment.NewLine + "  " + container.CommandName + "  ";
                            if (container.CommandArguments != null)
                            {
                                for (int i = 0; i < container.CommandArguments.Length; ++i)
                                    str += container.CommandArguments[i] + "  ";
                            }
                            str += Environment.NewLine + "    = " + container.Result;
                        }
                        Console.WriteLine(str);
                    }
                };

                Console.WriteLine("Sending command No. " + container.Id + "...");

                AddParallelCommand(container);

                dispatcher.SendJob(container);

                Console.WriteLine("    ... sending command No. " + container.Id + " done.");

            }
            return ret;
        }


        /// <summary>Runs she specified command with arguments once in a parallel thread by using 
        /// the interpreter's job dispatcher with corresponding parallel servers. </summary>
        /// <param name="command">Command that is run in each of the parallel threads.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <returns>Identification number of the parallel job that take over execution of the command.
        /// Queries about job progress and results can be made by using this identification number.</returns>
        /// <seealso cref="RunParallelRepeat(string, string[])"/>
        public int RunParallel(string command, string[] commandArguments)
        {
            return RunParallelRepeat(1, command, commandArguments)[0];
        }


        #endregion ParallelExecution


        #region AsynchronousExecution

        /// <summary>List of asynchronous results objects from individual asynchronous executions.</summary>
        private List<IAsyncResult> _asyncCommandResults;

        /// <summary>List of <see cref="IAsyncResult"/> objects that were returned by asynchronous command invocations.
        /// <para>Lazy evaluation, created when getter is invoked first time.</para></summary>
        protected List<IAsyncResult> AsyncCommandResults
        {
            get
            {
                lock (Lock)
                {
                    if (_asyncCommandResults == null)
                        _asyncCommandResults = new List<IAsyncResult>();
                    return _asyncCommandResults;
                }
            }
        }


        /// <summary>Runs a command asynchronously where the first argument is command name.
        /// Extracts command name and runs the corresponding application delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.</summary>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunAsync(string[] args)
        {
            lock (Lock)
            {
                if (args == null)
                    throw new ArgumentNullException("Commandline is not specified (null reference);");
                else if (args.Length < 1)
                    throw new ArgumentNullException("Command name is not specified.");
                else
                {
                    string[] cmdArgs = new string[args.Length - 1];
                    string cmdName = args[0];
                    if (string.IsNullOrEmpty(cmdName))
                        throw new ArgumentException("Command name not specifies (null or empty string - 1st array element).");
                    for (int i = 1; i < args.Length; ++i)
                    {
                        cmdArgs[i - 1] = args[i];
                    }
                    return RunAsync(cmdName, cmdArgs);
                }
            }
        }


        /// <summary>Runs the command with specified name (installed on the current interpreter object) asynchronously.</summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <returns>ID of asynchronous run used to query results and whether command has completed, or -1 if a call was not
        /// launched (actually, an exception would be thrown in this case).</returns>
        public virtual string RunAsync(string commandName, params string[] commandArguments)
        {
            int asyncId = -1;
            ApplicationCommandDelegate appDelegate = null;
            lock (Lock)
            {
                if (string.IsNullOrEmpty(commandName))
                    throw new ArgumentException("Command name is not specified.");
                // Perform substitution of variables with their values in command name and argumnets:
                commandName = SubstituteVariableReference(commandName);
                bool substituteArgs = false;
                for (int i = 0; i < commandArguments.Length; ++i)
                {
                    if (IsVariableReference(commandArguments[i]))
                    {
                        substituteArgs = true;
                        break;
                    }
                }
                if (substituteArgs)
                {
                    string[] argsSubstituted = new string[commandArguments.Length];
                    for (int i = 0; i < commandArguments.Length; ++i)
                        argsSubstituted[i] = SubstituteVariableReference(commandArguments[i]);
                    commandArguments = argsSubstituted;
                }
                if (!_caseSensitive)
                    commandName = commandName.ToLower();
                if (!_commands.ContainsKey(commandName))
                    throw new ArgumentException("Interpreter does not contain the following command: " + commandName + ".");
                appDelegate = _commands[commandName];
                if (appDelegate == null)
                {
                    throw new InvalidOperationException("Can not find command named " + commandName + ".");
                }
                else
                {
                    // return appDelegate(this, commandName, commandArguments);
                    IAsyncResult result = appDelegate.BeginInvoke(this, commandName, commandArguments, AsyncRunCallback, asyncId);
                    AsyncCommandResults.Add(result);
                    asyncId = AsyncCommandResults.Count - 1;
                    return asyncId.ToString();
                }
            }  // lock
        }

        private bool _asyncEndInvokeInCallback = true;

        /// <summary>Flag indicating whether Endinvoke must be called in the callback of asynchronous command calls.</summary>
        /// <remarks>This flag is unset temporarily in methods where endinvoke is called explicitly.</remarks>
        protected bool AsyncEndInvokeInCallback
        {
            get { return _asyncEndInvokeInCallback; }
            set { _asyncEndInvokeInCallback = value; }
        }

        /// <summary>Returns true if the asynchronous command execution identified by id has completed, and false otherwise.</summary>
        /// <param name="id">ID of the asynchronous command execution that is querried.</param>
        /// <returns></returns>
        public bool AsyncIsCompleted(int id)
        {
            if (id < 0)
                throw new ArgumentException("Asynchronous command execution ID can not be less than 0.");
            lock (Lock)
            {
                if (AsyncCommandResults.Count <= id)
                    throw new IndexOutOfRangeException("Asynchronous command execution ID " + id + " is greater than the largest ID.");
                return AsyncCommandResults[id].IsCompleted;
            }
        }

        /// <summary>Returns true if all asynchronous command executions have completed, and false otherwise.</summary>
        public bool AsyncIsAllCompleted()
        {
            lock (Lock)
            {
                bool ret = true;
                int numAsync = 0;
                numAsync = AsyncCommandResults.Count;
                for (int i = 0; i < numAsync; ++i)
                    ret = ret && AsyncIsCompleted(i);
                return ret;
            }
        }

        /// <summary>Waits until all asynchronously commands that have been eventually executed by the current 
        /// interpreter, complete.
        /// <para>It is sometimes necessary to call this method if any asynchronous command invocations were made because such 
        /// commands are executed in background threads, which are automatically broken when all foreground threads complete.</para></summary>
        public void AsyncWaitAll()
        {
            int numAsync = 0;
            lock (Lock)
            {
                numAsync = AsyncCommandResults.Count;
            }
            for (int i = 0; i < numAsync; ++i)
                AsyncWait(i);
        }

        /// <summary>Waits for the specified asynchronous command (specified by command ID) to complete.</summary>
        /// <param name="callId">ID of the asynchronous command execution.</param>
        /// <returns>Results of the command if the command has not completed before, null otherwise.</returns>
        public string AsyncWait(int callId)
        {
            int Id = -1;
            string commandResult = null;
            lock (Lock) // lock in order to prevent modification of the AsyncEndInvokeInCallback flag in other threads
            {
                bool flagvalue = AsyncEndInvokeInCallback;
                try
                {
                    if (callId < 0 || callId >= AsyncCommandResults.Count)
                        throw new IndexOutOfRangeException("Asyncronous call ID " + callId + " is out of range.");
                    AsyncEndInvokeInCallback = false;  // because EndInvoke is called here
                    if (OutputLevel > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Waiting for async. command call to complete, command ID = " + callId + "...");
                    }
                    AsyncResult result = (AsyncResult)AsyncCommandResults[callId];
                    Id = (int)result.AsyncState;
                    if (result.IsCompleted)
                    {
                        if (OutputLevel > 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Asynchronous command already completed before, command ID: " + callId);
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        ApplicationCommandDelegate caller = (ApplicationCommandDelegate)result.AsyncDelegate;  // retrieve the delegate
                        // Call EndInvoke to retrieve the results.
                        commandResult = caller.EndInvoke(result);
                        if (OutputLevel > 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Asynchronous command completed, command ID: " + callId);
                            Console.WriteLine("  command result: " + commandResult);
                            Console.WriteLine();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (OutputLevel > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Exception thrown in callback of asynchronous command call. Call ID: " + callId
                            + Environment.NewLine + "  Error message: " + ex.Message);
                        Console.WriteLine();
                    }
                }
                finally
                {
                    AsyncEndInvokeInCallback = flagvalue;
                }
            }
            return commandResult;
        }

        /// <summary>Callback method for asynchronous command executions.</summary>
        /// <param name="ar">Asynchronous results that are passed to the method.</param>
        protected void AsyncRunCallback(IAsyncResult ar)
        {
            lock (Lock)
            {
                int Id = -1;
                try
                {
                    AsyncResult result = (AsyncResult)ar;
                    Id = (int)ar.AsyncState;
                    ApplicationCommandDelegate caller = (ApplicationCommandDelegate)result.AsyncDelegate;  // retrieve the delegate
                    string returnValue = null;
                    // Call EndInvoke to retrieve the results.
                    if (AsyncEndInvokeInCallback)
                    {
                        returnValue = caller.EndInvoke(ar);
                    }
                    if (OutputLevel > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Asynchronous command completed, command ID: " + Id);
                        if (AsyncEndInvokeInCallback)
                            Console.WriteLine("  command result: " + returnValue);
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    if (OutputLevel > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Exception thrown in callback of asynchronous command call. Call ID: " + Id
                            + Environment.NewLine + "  Error message: " + ex.Message);
                        Console.WriteLine();
                    }
                }
                finally
                {
                }
            }
        }

        #endregion AsynchronousExecution


        /// <summary>Adds command with the specified name.</summary>
        /// <param name="commandName">Name of the commant.</param>
        /// <param name="commandDelegate">Delegate that will be used to execute the command.</param>
        public virtual void AddCommand(string commandName, ApplicationCommandDelegate commandDelegate)
        {
            lock (Lock)
            {
                if (string.IsNullOrEmpty(commandName))
                    throw new ArgumentException("Command name not specified.");
                if (!_caseSensitive)
                    commandName = commandName.ToLower();
                if (WarnCommandReplacement) if (_commands.ContainsKey(commandName))
                    {
                        Console.WriteLine();
                        Console.WriteLine("WARNING: Interpreter command redefined: " + commandName + ".");
                        Console.WriteLine();
                    }
                _commands[commandName] = commandDelegate;
                //if (_commands.ContainsKey(commandName))
                //    _commands.Remove(commandName);
                //_commands.Add(commandName, commandDelegate);
            }
        }


        /// <summary>Removes the command with the specified name.</summary>
        /// <param name="commandName">Name of the commad.</param>
        public virtual void RemoveCommand(string commandName)
        {
            lock (Lock)
            {
                if (string.IsNullOrEmpty(commandName))
                    throw new ArgumentException("Application name not specified.");
                if (!_caseSensitive)
                    commandName = commandName.ToLower();
                if (_commands.ContainsKey(commandName))
                    _commands.Remove(commandName);
            }
        }


        /// <summary>Removes all commands from the current interpreter.</summary>
        public virtual void RemoveAllCommands()
        {
            lock (Lock)
            {
                _commands.Clear();
            }
        }


        #region Modules

        /// <summary>Adds a new module to the interpreter. This adds an initialization function (via a delegate)
        /// which is executed when module module initialization is performed.</summary>
        /// <param name="moduleName">Name of the module. When used, module names are case sensitive if commands are case sensitive.</param>
        /// <param name="moduleDelegate">Method that performs module initialization.</param>
        public virtual void AddModule(string moduleName, ModuleDelegate moduleDelegate)
        {
            lock (Lock)
            {
                if (string.IsNullOrEmpty(moduleName))
                    throw new ArgumentException("Module name not specified.");
                if (!_caseSensitive)
                    moduleName = moduleName.ToLower();
                _modules[moduleName] = moduleDelegate;
            }
        }

        /// <summary>Loads and initializes the specified module.</summary>
        /// <param name="moduleName">Name of the module. It is case sensitive if commands are case sensitive.</param>
        public virtual void LoadModule(string moduleName)
        {
            lock (Lock)
            {
                if (string.IsNullOrEmpty(moduleName))
                    return;
                if (!_caseSensitive)
                    moduleName = moduleName.ToLower();
                if (!_modules.ContainsKey(moduleName))
                    throw new ArgumentException("The followind module can not be found: " + moduleName + ".");
                else
                {
                    ModuleDelegate initMethod = _modules[moduleName];
                    if (initMethod == null)
                        throw new ArgumentException("The following module does not have initialization method defined: " + moduleName + ".");
                    else
                    {
                        initMethod(moduleName, this);
                        _loadedModules.Add(moduleName);  // mark module as loaded
                        // Add a new command that can be used for verification if module is installed.
                        AddCommand(ModuleTestCommandName(moduleName), CmdModuleTestCommand);

                    }
                }
            }
        }

        protected string _ModuleTestCommandPrefix = "TestModule_";

        /// <summary>Returns the standard name for the command that gets installed 
        /// when a module is loaded.</summary>
        /// <param name="modulename">Name of the module that is loaded.</param>
        /// <remarks>Whenever a module is loaded, </remarks>
        public virtual string ModuleTestCommandName(string modulename)
        {
            return (_ModuleTestCommandPrefix + modulename);
        }

        /// <summary>Returns true if the specified module has been loaded on the interpreter,
        /// false if not.</summary>
        /// <param name="moduleName">Name of the module.</param>
        public virtual bool IsModuleLoaded(string moduleName)
        {
            if (!_caseSensitive)
                moduleName = moduleName.ToLower();
            return _loadedModules.Contains(moduleName);
        }

        #endregion Modules

        #region ScriptLoading

        // TODO: 
        // replace class with interface when interface is defined!

        LoadableScriptInterpreterBase _loadableScriptInterpreter;

        /// <summary>Interpreter based on dynamically loadable scripts.
        /// This enables installation and running of commands that are based on C# code that is 
        /// dynamically compiled. 
        /// Ihe object is created on first get access if it has not been assigned before.
        /// This property can be overridden in derived classes such that getter creates
        /// a dynamically loadable script - based interpreter of another kind. This is important 
        /// because different script loaders (in particuar with different dynamic libraries referenced)
        /// will be used in different contexts. Another possibility is that a custom object is 
        /// assigned to this property, usually in the initialization stage of the current interpreter.</summary>
        /// <exception cref="ArgumentNullException">When set to null reference.</exception>
        /// $A Igor Aug11;
        public virtual LoadableScriptInterpreterBase LoadableScriptInterpreter
        {
            get
            {
                lock (Lock)
                {
                    if (_loadableScriptInterpreter == null)
                        _loadableScriptInterpreter = new LoadableScriptInterpreterBase();
                    return _loadableScriptInterpreter;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (value == null)
                        throw new ArgumentNullException("Loadable script-based interpreter to be set is not specified (null reference).");
                }
            }
        }

        /// <summary>Gets the script loader that is used by the current command-line
        /// interpreter for dynamically loading commands from scripts.</summary>
        public ScriptLoaderBase ScriptLoader
        {
            get { return LoadableScriptInterpreter.ScriptLoader; }
        }

        /// <summary>Dynamically loads (temporarily, just for execution of the current commad) a class 
        /// form the script contained in the specified file and executes its executable method.
        /// The file must contain the script that is dynamically loaded and executed, in form of 
        /// definition of the appropriate class of type <see cref="ILoadableScript"/>. 
        /// The dynamically loadable script class is loaded from the file and instantiated by the
        /// <see cref="LoadableScriptInterpreter"/> interpreter that is based on loadable scripts.</summary>
        /// <param name="scriptFilePath">Path to the file containing loadable script must be the first argument to the method.</param>
        /// <param name="initAndRunArgs">Initialization arguments for the object that will be instantiated
        /// in order to execute the script.</param>
        /// <returns>Result of execution returned by the executable method of the dynamically loaded script object.</returns>
        public virtual string RunScriptFile(string scriptFilePath, string[] initAndRunArgs)
        {
            return LoadableScriptInterpreter.ScriptLoader.RunFile(scriptFilePath, initAndRunArgs);
        }

        /// <summary>Dynamically loads (compiles and instantiates) a loadable script class contained in the specified file, 
        /// and installs a new command on <see cref="LoadableScriptInterpreter"/> and on the current interpreter, 
        /// based on the dynamically created instance of the loaded (dynamically compiled) class.</summary>
        /// <param name="newCommandName">Name of the newly installed command.</param>
        /// <param name="scriptFilePath">Name of the file containing the script code that defines a loadable script class.</param>
        /// <param name="initArgs">Arguments to the initialization method of the loaded object.
        /// The initialization method will be called before the first call to the executable method of the class,
        /// which takes care of execution of the newly installed command.</param>
        public void LoadScript(string newCommandName, string scriptFilePath, string[] initArgs)
        {
            // Add new dynamically loaded command on the LoadableScriptInterpreter, loaded form script file:
            LoadableScriptInterpreter.AddCommandFromFile(newCommandName, scriptFilePath, initArgs);
            // Add new command with the same name on the current interpreter, such that command will execute
            // teh dynamically loaded command on LoadableScriptInterpreter when executed:
            this.AddCommand(newCommandName, CmdRunLoadedScript);
        }

        /// <summary>Executes the specified command that has been dynamically loaded form a script.</summary>
        /// <param name="commandName">Name under which the command is installed on the current intepreter
        /// and on interpreder based on dynamically loaded scripts (<see cref="LoadableScriptInterpreter"/>).</param>
        /// <param name="arguments">Arguments to the command.</param>
        /// <returns>Results of command execution.</returns>
        public virtual string RunLoadedScript(string commandName, string[] arguments)
        {
            return LoadableScriptInterpreter.RunCommand(commandName, arguments);
        }

        /// <summary>Returns an array of assemblies that are currently referenced by the script loader
        /// that takes care of loading the dynamic scripts.</summary>
        /// <returns></returns>
        public string[] GetLoadableScriptReferencedAssemblies()
        {
            return LoadableScriptInterpreter.ScriptLoader.GetReferencedAssemblies();
        }

        #endregion ScriptLoading

        #endregion Operation


        #region StaticMethods


        #endregion StaticMethods


        #region CommandMethods

        /// <summary>Command.
        /// Sets the specified varuable to the specified value. <br/>
        /// Usage: <br/>
        /// 1. Set the variable to the specified value:
        ///   SetVar varName value <br/>
        ///     varName: name of the variable to be set. <br/>
        ///     value: value that is assigned to the variable. <br/>
        /// 2. Set the variable to the return value of the specified command: <br/>
        ///   SetVar varName command arg1 arg2 ... <br/>
        ///     varName: name of the variable to be set. <br/>
        ///     command: command whose return value is the value to be assigned to the variable. <br/>
        ///     arg1: the first argument to the command (if any). <br/>
        ///     arg2: the second argument to the command (if any). <br/>
        ///     etc. <br/>
        /// </summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Value of the variable after setting.</returns>
        protected virtual string CmdSetVariable(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " varName value : sets the variable varName to value.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires at lest two arguments.");
            else if (args.Length < 2)
                throw new ArgumentException(cmdName + " : Requires at lest two arguments.");
            else
            {
                string varName = null, value = null;
                varName = args[0];
                if (args.Length == 2)
                    value = args[1];
                else
                {  // args.Length>2
                    string cmd = args[1];
                    string[] argsCmd = new string[args.Length - 2];
                    for (int i = 2; i < args.Length; ++i)
                        argsCmd[i - 2] = args[i];
                    value = Run(cmd, argsCmd);
                }
                ret = SetVariable(varName, value);
            }
            return ret;
        }

        /// <summary>Command.
        /// Gets the specified varuable and returns its value (or null if the variable does not exist).
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Value of the variable.</returns>
        protected virtual string CmdGetVariable(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " varName : returns the value of variable varName.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires 1 argument.");
            else if (args.Length != 1)
                throw new ArgumentNullException(cmdName + " : no arguments (null reference); variable name should be specified.");
            else if (args.Length != 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be 1 (argument being variable name).");
            else
            {
                string varName = args[0];
                ret = GetVariable(varName);
            }
            return ret;
        }

        /// <summary>Command.
        /// Clears the specified varuable.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdClearVariable(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " varName : clears (removes) the variable varName.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires 1 argument.");
            else if (args.Length != 1)
                throw new ArgumentNullException(cmdName + " : no arguments (null reference); variable name should be specified.");
            else if (args.Length != 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be 1 (argument being variable name).");
            else
            {
                string varName = args[0];
                ret = ClearVariable(varName);
            }
            return ret;
        }

        /// <summary>Command.
        /// Prints the specified varuable.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdPrintVariable(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " varName : prints the variable varName.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires 1 argument.");
            else if (args.Length != 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be 1 (argument being variable name).");
            else
            {
                string varName = args[0];
                ret = PrintVariable(varName);
            }
            return ret;
        }

        /// <summary>Command.
        /// Prints concatenated argument with spaces between them.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdWriteLine(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = CmdWrite(interpreter, cmdName, args);
            ret += Environment.NewLine;
            Console.WriteLine();
            return ret;
        }

        /// <summary>Command.
        /// Prints concatenated argument with spaces between them.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdWrite(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " arg1 arg2 ... : prints the arguments arg1, etc.");
                        Console.WriteLine();
                        return null;
                    }
            StringBuilder sb = new StringBuilder();
            if (args == null)
                sb.Append("");
            else if (args.Length < 1)
                sb.Append("");
            else
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    sb.Append(args[i]);
                }
            }
            ret = sb.ToString();
            Console.Write(ret);
            return ret;
        }


        /// <summary>Command.
        /// Runs a file by running all its lines in the current interpreter.
        /// File name must be the only argument of the command.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdRunFile(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " filePath : runs the file located at filePath.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentNullException(cmdName + " : Requires 1 argument (file name).");
            else if (args.Length != 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be 1 (argument being file name).");
            else
            {
                string fileName = args[0];
                ret = RunFile(fileName);
            }
            return ret;
        }

        /// <summary>Command.
        /// Runs another command repetitively the specified number of times.
        /// First argument must be the number of times command is run, the second argument must
        /// be command to be run repetitively, and the rest of the arguments are passed to that 
        /// command as its arguments.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Concatenated results of all runs, separated by spaces.</returns>
        protected virtual string CmdRunRepeatVerbose(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " numRep cmd arg1 arg2 ... : ");
                        Console.WriteLine("    runs the command specified number of times, verbose output.");
                        Console.WriteLine("  numRep: number of repetitions of command.");
                        Console.WriteLine("  cmd:    command to be run several times in a row.");
                        Console.WriteLine("  arg1, arg2, ... : Eventual arguments of the command.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentNullException(cmdName + " : Requires 1 argument (file name).");
            else if (args.Length < 2)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at least 2 "
                    + Environment.NewLine + "  and include number of repetitions and command name.");
            else
            {
                ret = RunRepeatVerbose(args);
            }
            return ret;
        }

        /// <summary>Command.
        /// Runs another command repetitively the specified number of times.
        /// First argument must be the number of times command is run, the second argument must
        /// be command to be run repetitively, and the rest of the arguments are passed to that 
        /// command as its arguments.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Concatenated results of all runs, separated by spaces.</returns>
        protected virtual string CmdRunRepeat(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " numRep cmd arg1 arg2 ... : ");
                        Console.WriteLine("    runs the command specified number of times.");
                        Console.WriteLine("  numRep: number of repetitions of command.");
                        Console.WriteLine("  cmd:    command to be run several times in a row.");
                        Console.WriteLine("  arg1, arg2, ... : Eventual arguments of the command.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentNullException(cmdName + " : Requires 2 argument (number of repetitions and command name).");
            else if (args.Length < 2)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at least 2 "
                    + Environment.NewLine + "  and include number of repetitions and command name.");
            else
            {
                ret = RunRepeat(1, args);
            }
            return ret;
        }

        /// <summary>Command.
        /// Runs another command in a try-catch block, such that if command throws an exception execution is not
        /// broken.
        /// The second argument must be command to be run, and the rest of the arguments are passed to that 
        /// command as its arguments.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Concatenated results of all runs, separated by spaces.</returns>
        protected virtual string CmdTryRun(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " cmd arg1 arg2 ... : ");
                        Console.WriteLine("    runs the command in a try-catch block such that it can not break execution.");
                        Console.WriteLine("      Error message is returned if exception is thrown, otherwise command's result");
                        Console.WriteLine("      is returned.");
                        Console.WriteLine("  cmd:    command to be run several times in a row.");
                        Console.WriteLine("  arg1, arg2, ... : Eventual arguments of the command.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentNullException(cmdName + " : Requires at least 1 argument (command name).");
            else if (args.Length < 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at least 1 "
                    + Environment.NewLine + "  and include command name.");
            else
            {
                ret = RunTryCatch(OutputLevel, args);
            }
            return ret;
        }

        /// <summary>Command. 
        /// Sets the flag for rethrowing exceptions in the interaction mode.
        /// Optional boolean arguemnt, default is true.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdThtrowExceptions(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {

            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine(Environment.NewLine
                            + executableName + " " + cmdName + " <doThrow> : " + Environment.NewLine
                            + "  sets the flag for rethrowing exceptions." + Environment.NewLine
                            + "  When this flag is set, exceptions thrown in interactively run commands " + Environment.NewLine
                            + "    are rethrown, typically resulting in breaking the application." + Environment.NewLine
                            + "  doThrow: optional argument defining value of the flag (default true)."
                            + Environment.NewLine);
                        Console.WriteLine(executableName + " " + cmdName + " command arg1 arg1 ... : runs commands interactively,"
                            + Environment.NewLine + "  running command with specified arguments before entering interactive mode.");
                        return null;
                    }
            bool flagValue = true;
            if (args != null)
                if (args.Length > 0)
                {
                    // If there were arguments specified, the first argument is boolean value of the flag:
                    if (args.Length > 1)
                        throw new ArgumentException(cmdName + ": At most one argument can be passed.");
                    string str = args[0];
                    if (!Util.TryParseBoolean(str, ref flagValue))
                        throw new ArgumentException(cmdName + ": Can not convert \"" + str + "\" to boolean. " + Environment.NewLine
                            + "  Use the following values: 0, 1, true, false, yes, no, y, n (case insensitive)!");
                }
            ThrowExceptions = flagValue;
            return flagValue.ToString();
        }


        /// <summary>Command. Runs interpreter commands interactively.
        /// Reads commands one by one from console and executes them, until only Enter is pressed..</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdRunInteractive(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : runs commands interactively.");
                        Console.WriteLine(executableName + " " + cmdName + " command arg1 arg1 ... : runs commands interactively,"
                            + Environment.NewLine + "  running command with specified arguments before entering interactive mode.");
                        Console.WriteLine();
                        return null;
                    }
            if (args != null)
                if (args.Length > 0)
                {
                    // If there were arguments specified, then we first execute teh command that was specified by 
                    // these arguments: 
                    string command = args[0];
                    int numArgs = args.Length - 1;
                    string[] commandArgs = null;
                    if (numArgs >= 1)
                    {
                        commandArgs = new string[numArgs];
                        for (int i = 0; i < numArgs; ++i)
                            commandArgs[i] = args[i + 1];
                    }
                    Run(command, commandArgs);
                    //throw new ArgumentNullException(cmdName + " : Requires no arguments.");
                }
            ret = RunInteractive();
            return ret;
        }


        /// <summary>Command. 
        /// Runs the specified command-line by the operating system.
        /// The first argument is the command to be executed while the following arguments are 
        /// arguments to this command.
        /// If there are no arguments then user is requested to insert commands interactively.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments this command.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdRunSystem(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " command arg1 arg2... : runs system command-line.");
                        Console.WriteLine("  command: command to be run (e.g. executable path)");
                        Console.WriteLine("  arg1, arg2, etc.: eventual arguments passed (optional).");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
            {
                ExecuteSystemCommandsInteractive();
                return null;
            }
            else if (args.Length < 1)
            {
                ExecuteSystemCommandsInteractive();
                return null;
            }
            else
            {
                ExecuteSystemCommand(args);
            }
            return ret;
        }


        /// <summary>Command. 
        /// Runs the built in expression evaluator.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments this command.</param>
        /// <returns></returns>
        protected virtual string CmdExpressionEvaluatorInteractive(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + ": Runs interactive expression evaluator.");
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " expression : Evaluates expression and returns result.");
                        Console.WriteLine("    If there are multiple arguments, these are concatenated to form expression.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
            {
                return ExpressionEvaluatorInteractive();
            }
            else if (args.Length < 1)
            {
                ret = ExpressionEvaluatorInteractive();
            }
            else
            {
                ret = ExpressionEvaluatorEvaluate(args);
                Console.WriteLine(Environment.NewLine + "  = " + ret);
            }
            return ret;
        }

        /// <summary>Interpreter command. Sets the priority of the current process.</summary>
        /// <param name="interpreter">Interpreter by which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>Null.</returns>
        protected virtual string CmdSetPriority(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <threadPriority> : sets (and prints & returns) thread priority.");
                        Console.WriteLine("  threadPriority: the new thread priority (optional) that is set.");
                        Console.WriteLine("    This affects the application-level thread priosity. current interpreter's priority, ");
                        Console.WriteLine("    and interpreter's parallel dospatcher's priority.");
                        Console.WriteLine("    If new priority is not specified then interpreter's priority is updated to the global value.");
                        Console.WriteLine();
                        return null;
                    }
            bool defined = false;
            ThreadPriority newPriority = ThreadPriority.Normal;
            if (args != null)
                if (args.Length > 0)
                {
                    defined = Util.TryParseThreadPriority(args[0], ref newPriority);
                    if (!defined)
                    {
                        Console.WriteLine(Environment.NewLine + "Error: the first argument does not represent a thread priority."
                            + Environment.NewLine);
                    }
                }
            StringBuilder sb = new StringBuilder();
            if (OutputLevel >= 0)
            {
                sb.AppendLine();
                if (defined)
                    sb.AppendLine(Environment.NewLine + "Settig thread priority to " + newPriority + "...");
                else
                    sb.AppendLine(Environment.NewLine + "Getting thread priority.");
                sb.AppendLine("  Current global thread priority: " + UtilSystem.ThreadPriority);
                sb.AppendLine("  Current interpreter's thread priority: " + this.ThreadPriority);
                if (ParallelDispatcher == null)
                    sb.AppendLine("  Interpreter's parallel job dispatcher is not defined yet.");
                else
                    sb.AppendLine("  Interpreter's parallel dispatcher's priority: " + ParallelDispatcher.ThreadPriority);
            }
            if (defined)
            {
                UtilSystem.ThreadPriority = newPriority;
                this.ThreadPriority = newPriority;
                if (_parallelDispatcher != null)
                    ParallelDispatcher.ThreadPriority = newPriority;
                if (OutputLevel >= 1)
                {
                    sb.AppendLine("After setting priority: ");
                    sb.AppendLine("  G thread priority: " + UtilSystem.ThreadPriority);
                    sb.AppendLine("  Interpreter's thread priority: " + this.ThreadPriority);
                    if (ParallelDispatcher == null)
                        sb.AppendLine("  Interpreter's parallel job dispatcher is not defined yet.");
                    else
                        sb.AppendLine("  Interpreter's parallel dispatcher's priority: " + ParallelDispatcher.ThreadPriority);
                }
            }
            Console.WriteLine(sb.ToString());
            return UtilSystem.ThreadPriority.ToString();
        }


        /// <summary>Interpreter command. 
        /// Runs the specified command-line in parallel thread.
        /// <para>The first argument is the command to be executed while the following arguments are 
        /// arguments to this command.</para></summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>ID of the job container that contains all command data.</returns>
        protected virtual string CmdRunParallel(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " command arg1 arg2... : runs comman in a parallel thread.");
                        Console.WriteLine("  command: command to be run ");
                        Console.WriteLine("  arg1, arg2, etc.: eventual arguments passed (optional).");
                        Console.WriteLine("Returns an ID that can be used to query the status and results ");
                        Console.WriteLine("  of the command or to wait its completion.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException("Arguments (command name and eventually its arguments) not specified.");
            if (args.Length < 1)
                throw new ArgumentException("Arguments (command name and eventually its arguments) not specified.");
            string commandName = args[0];
            string[] commandArgumets = new string[args.Length - 1];
            for (int i = 1; i < args.Length; ++i)
            {
                commandArgumets[i - 1] = args[i];
            }
            return RunParallel(commandName, commandArgumets).ToString();
        }

        /// <summary>Interpreter command. 
        /// Runs the specified command-line several times in the specified number of parallel threads.
        /// <para>The first argument is the number of parallel executions of the same command, the second
        /// argument is command to be executed while the following arguments are 
        /// arguments to this command.</para></summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>IDs of the job container that contains all command data separated by spaces.</returns>
        protected virtual string CmdRunParallelRepeat(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            StringBuilder sbReturned = new StringBuilder();
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " command numRepeats arg1 arg2... : runs comman in several parallel threads.");
                        Console.WriteLine("  command: command to be run ");
                        Console.WriteLine("  numRepeats: number of parallel threads in which the command is run.");
                        Console.WriteLine("  arg1, arg2, etc.: eventual arguments passed (optional).");
                        Console.WriteLine("Returns IDs that can be used to query the status and results ");
                        Console.WriteLine("  of the command executions, or to wait their completions.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException("Arguments (command name and eventually its arguments) not specified.");
            if (args.Length < 2)
                throw new ArgumentException("Arguments (number of parallel executions, command name and eventually its arguments) not specified.");
            int numRepetitions = int.Parse(args[0]);
            string commandName = args[1];
            string[] commandArgumets = new string[args.Length - 2];
            for (int i = 2; i < args.Length; ++i)
            {
                commandArgumets[i - 2] = args[i];
            }
            int[] ids = RunParallelRepeat(numRepetitions, commandName, commandArgumets);
            if (ids != null)
            {
                for (int i = 0; i < ids.Length; ++i)
                {
                    if (i > 0)
                        sbReturned.Append(" ");
                    sbReturned.Append(ids[i].ToString());
                }
            }
            return sbReturned.ToString();
        }

        /// <summary>Interpreter command. 
        /// Prints data about commands executed in parallel threads.
        /// <para>The optional first argument is a flag (boolean, can be integer) that specifies whether
        /// the completed commands are also printed or not. Default is true.</para></summary>
        /// <param name="interpreter">Interpreter by which commad is run.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>Null.</returns>
        protected virtual string CmdPrintParallelCommands(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <printAll> : prints the parallel commands.");
                        Console.WriteLine("  printall: specifies whether all commands are printed, including those already completed.");
                        Console.WriteLine();
                        return null;
                    }
            bool printAll = true;
            if (args != null)
                if (args.Length > 0)
                {
                    bool defined = Util.TryParseBoolean(args[0], ref printAll);
                    if (!defined)
                    {
                        Console.WriteLine(Environment.NewLine + "Error: the first argument does not represent a boolean."
                            + Environment.NewLine);
                    }
                }
            PrintParallelCommands(printAll);
            return null;
        }


        /// <summary>Command. 
        /// Runs the specified command-line asynchronously.
        /// <para>The first argument is the command to be executed while the following arguments are 
        /// arguments to this command.</para></summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>ID of the asynchronous command for querrying completion and ending invocation and picking results.</returns>
        protected virtual string CmdRunAsync(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " command arg1 arg2... : runs comman asynchronously.");
                        Console.WriteLine("  command: command to be run (e.g. executable path)");
                        Console.WriteLine("  arg1, arg2, etc.: eventual arguments passed (optional).");
                        Console.WriteLine("Returns an ID that can be used to query whether async. command has completed, ");
                        Console.WriteLine("  to wait completion or to obtain results.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException("Arguments (command name and eventually its arguments) not specified.");
            if (args.Length < 1)
                throw new ArgumentException("Arguments (command name and eventually its arguments) not specified.");
            return RunAsync(args);
        }


        /// <summary>Command. 
        /// Wait until asynchronously invoked command with the specified ID (first argument, must represent an int) completes.
        /// <para>The first argument is the ID of asynchronous invocation whose results are waited.</para></summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>Results of the async. command execution whose completion is waited for.</returns>
        protected virtual string CmdAsyncWaitResults(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " asyncCommandID : waits for results of asynchronous command execution.");
                        Console.WriteLine("  asyncCommandID: ID of the asynchronous invocation whose completion is waited for.");
                        Console.WriteLine("Returns results of the asynchronous command.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException("ID of hte asynchronous command invocation is not specified.");
            if (args.Length < 1)
                throw new ArgumentException("ID of hte asynchronous command invocation is not specified.");
            int id;
            bool success = int.TryParse(args[0], out id);
            if (!success)
                throw new ArgumentException("Command argument does not represent an integer ID of asynchronous invocation.");
            return AsyncWait(id);
        }

        /// <summary>Command. 
        /// Returns a flag indicating whether the asynchroneous command invocation identified by the specified ID 
        /// (first argument, must represent an int) has completed.
        /// <para>The first argument is the ID of asynchronous invocation whose completion is waited for.</para></summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>Results of the async. command execution whose completion is waited for.</returns>
        protected virtual string CmdAsyncCompleted(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " asyncCommandID : checks whether asynchronous command execution has completed.");
                        Console.WriteLine("  asyncCommandID: ID of the asynchronous invocation whose completion is waited for.");
                        Console.WriteLine("Returns '1' if completed, '0' otherwise.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException("ID of hte asynchronous command invocation is not specified.");
            if (args.Length < 1)
                throw new ArgumentException("ID of hte asynchronous command invocation is not specified.");
            int id;
            bool success = int.TryParse(args[0], out id);
            if (!success)
                throw new ArgumentException("Command argument does not represent an integer ID of asynchronous invocation.");
            if (AsyncIsCompleted(id))
                return "1";
            else
                return "0";
        }


        /// <summary>Command. 
        /// Sleeps (suspends execution of the executing thread) for the specified number of seconds.
        /// <para>The first argument is the number of seconds (must be string representing double) to sleep.</para></summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>Returns the ID of the thread where sleep is performed.</returns>
        protected virtual string CmdSleepSeconds(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " numSeconds : suspends execution for specified time.");
                        Console.WriteLine("  numSeconds: number of seconds (not necessarily integer) to sleep.");
                        Console.WriteLine("Returns the ID of the thread where sleep is performed.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException("Number of seconds to sleep sleep is not specified.");
            if (args.Length < 1)
                throw new ArgumentException("Number of seconds to sleep sleep is not specified.");
            double secondsToSleep;
            bool success = double.TryParse(args[0], out secondsToSleep);
            if (!success)
                throw new ArgumentException("Command argument (time to sleep in seconds) does not represent an a number.");
            int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            if (OutputLevel > 0)
            {
                Console.WriteLine();
                Console.Write("Suspending execution for " + secondsToSleep + " seconds...");
            }
            System.Threading.Thread.Sleep((int)(secondsToSleep * 1000.0));
            if (OutputLevel > 0)
            {
                Console.Write(Environment.NewLine + "    ... sleeping {0:G3} s done.", secondsToSleep);
                Console.WriteLine();
            }
            return threadId.ToString();
        }


        #region ModuleCommands

        /// <summary>Command.
        /// Loads the specified module (whos name must be the first argument) and performs its initialization.
        /// If there are more than 1 arguments then the rest of the arguments specify a command and (if more 
        /// than 1) its arguments, and the specified command is also run.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdLoadModule(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " moduleName : loads the module named moduleName.");
                        Console.WriteLine(executableName + " " + cmdName + " moduleName cmd arg1 arg2 ... : loads the module named moduleName,"
                          + Environment.NewLine + " then runs command names cmd with arguments arg1, arg2, etc.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentNullException(cmdName + " : Requires at least one argument (module name).");
            else if (args.Length < 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at leas 1 (1st argument being module name).");
            else
            {
                string moduleName = args[0];
                this.LoadModule(moduleName);
                if (args.Length > 1)
                {
                    string commandName = args[1];
                    string[] commandArgs = new string[args.Length - 2];
                    for (int i = 2; i < args.Length; ++i)
                        commandArgs[i - 2] = args[i];
                    ret = this.Run(commandName, commandArgs);
                }
            }
            return ret;
        }


        /// <summary>Executinon method for command that checks if module is loaded.
        /// Writes to condole whether module is loaded or not, and returns "1" if module
        /// is loaded and "0" if not.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdIsModuleLoaded(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " modulename : checks whether the specified module is loaded.");
                        Console.WriteLine("  moduleName - name of the module that is checked.");
                        Console.WriteLine("  returns '1' if module has been loaded and '0' if not.");
                        Console.WriteLine();
                        return null;
                    }
            string ret = "0";
            if (args == null)
                throw new ArgumentNullException("Checking whether module has been installed: command has no arguments.");
            else if (args.Length != 1)
                throw new ArgumentNullException("Checking whether module has been installed: Invalid number of arguments, should be 1.");
            else
            {
                if (IsModuleLoaded(args[0]))
                {
                    ret = "1";
                    Console.WriteLine();
                    Console.WriteLine("Module " + args[0] + " has been loaded.");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Module is NOT LOADED: " + args[0] + ".");
                    Console.WriteLine();
                }
            }
            return ret;
        }

        /// <summary>Executinon method for test command that is installed when a module is installed.
        /// This is a command that enables to verify that a module with the specified name has been installed.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdModuleTestCommand(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : test command that gets installed with every module.");
                        Console.WriteLine();
                        if (_caseSensitive)
                            Console.WriteLine("Command name is module name prefixed by '" + _ModuleTestCommandPrefix + "'.");
                        else
                            Console.WriteLine("Command name is module name prefixed by '" + _ModuleTestCommandPrefix.ToLower() + "'.");
                        Console.WriteLine();
                        return null;
                    }
            Console.WriteLine();
            Console.WriteLine("This output indicates that the specific module has been loaded at some point.");
            Console.WriteLine();
            Console.WriteLine("Command name: " + cmdName);
            if (_caseSensitive)
                Console.WriteLine("Command name is module name prefixed by '" + _ModuleTestCommandPrefix + "'.");
            else
                Console.WriteLine("Command name is module name prefixed by '" + _ModuleTestCommandPrefix.ToLower() + "'.");
            Console.WriteLine("Warning:");
            Console.WriteLine("Even if the specific module has been loaded, its commands may have been ");
            Console.WriteLine("overridden by other modules.");
            Console.WriteLine();
            return "Module's automatic test command successful: " + cmdName + ".";
        }


        /// <summary>Executinon method for test command, which just prints its name and arguments.
        /// This is a replacement for usuel test command, which gets installed when one of the two
        /// basic test modules are installed.</summary>
        /// <param name="interpreter">Interpreter on which commad is run. 
        /// Enables access to interpreter internal data from command body.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdTestFromTestModules(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : new test command (for modules), just prints its arguments.");
                        Console.WriteLine();
                        return null;
                    }
            Console.WriteLine();
            Console.WriteLine("Test command launched (new test command, for testing modules).");
            Console.WriteLine("This output also indicates that a test module was loaded.");
            Console.WriteLine();
            Console.WriteLine("Command name: " + cmdName);
            Console.WriteLine("Command arguments (in double quotes): ");
            if (args == null)
                Console.WriteLine("  No arguments(null reference).");
            else if (args.Length == 0)
                Console.WriteLine("  No arguments.");
            else
                Console.Write("  ");
            {
                for (int i = 0; i < args.Length; ++i)
                    Console.Write("\"" + args[i] + "\" ");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            return "Return from the 'Test' command for testing modules.";
        }

        #endregion ModuleCommands




        #region NamedPipes



        /// <summary>Client to the interpreter pipe server (classes derived from <see cref="InterpreterPipeServer"/>).</summary>
        /// $A Igor xx Mar14;
        public class InterpreterPipeClient : NamedPipeClientBase
        {

            #region Construction

            /// <summary>Constructs a new named pipe client with the specified pipe name, default server address (<see cref="NamedPipeClientBase.DefaultServerAddress"/>)
            /// and default values for other paramters.</summary>
            /// <param name="pipeName">Name of the pipe. Must not be null or empty string.</param>
            public InterpreterPipeClient(string pipeName)
                : base(pipeName)
            { }

            /// <summary>Constructs a new named pipe client with the specified pipe name, server address (<see cref="NamedPipeClientBase.DefaultServerAddress"/>)
            /// and default values for other paramters.</summary>
            /// <param name="pipeName">Name of the pipe. Must not be null or empty string.</param>
            /// <param name="serverAddress">Address of the server where named pipe server is run. If null or empty string then the
            /// default server address is uesd (<see cref="NamedPipeClientBase.DefaultServerAddress"/>), referring to the current computer.</param>
            public InterpreterPipeClient(string pipeName, string serverAddress)
                : base(pipeName, serverAddress)
            { }

            /// <summary>Constructs a new named pipe client with the specified pipe name, server address (<see cref="DefaultServerAddress"/>)
            /// and other paramters.</summary>
            /// <param name="pipeName">Name of the pipe.</param>
            /// <param name="serverAddress">Address of the server where named pipe server is run. If null or empty string then the
            /// default server address is uesd (<see cref="DefaultServerAddress"/>), referring to the current computer.</param>
            /// <param name="requestEnd">Line that ends each request. If null or empty string then the requests are single line.</param>
            /// <param name="responseEnd">Line that ends each response. If null or empty string then the responses are single line.</param>
            /// <param name="errorBegin">String that begins an error response. If null or empty string then default string remains in use,
            /// i.e. <see cref="DefaultErrorBegin"/></param>
            public InterpreterPipeClient(string pipeName, string serverAddress, string requestEnd, string responseEnd, string errorBegin) :
                base(pipeName, serverAddress, requestEnd, responseEnd, errorBegin)
            { }

            #endregion Construction

            #region Data

            protected string _interpretersName = null;

            public string InterpretersName
            {
                get { return _interpretersName; }
                set { _interpretersName = value; }
            }

            #endregion Data

        }  // class InterpreterPipeClient


        /// <summary>Command-line interpreter's server that creates a named pipe, listens on its input stream
        /// for client requests, executes requests in the corresponding interpreter, and sends responses back 
        /// to the client.</summary>
        /// $A Igor xx Jun;
        public class InterpreterPipeServer : NamedPipeServerBase
        {

            /// <summary>Constructs a new pip server.</summary>
            /// <param name="interpreter">Interpreter used to serve requests.</param>
            /// <param name="pipeName">Name of the pipe used for client-server communication.</param>
            /// <param name="startImmediately">If true then server is starrted immediately, otherwise this is postponed.</param>
            public InterpreterPipeServer(ICommandLineApplicationInterpreter interpreter, string pipeName, 
                bool startImmediately = true)
                : base(pipeName, false /* startImmediately */)
            { 
                Interpreter = interpreter;
                if (startImmediately)
                    ThreadServe();
            }

            /// <summary>Constructs a new named pipe server with the specified pipe name and other paramters.</summary>
            /// <param name="pipeName">Name of the pipe.</param>
            /// <param name="requestEnd">Line that ends each request. If null or empty string then the requests are single line.</param>
            /// <param name="responseEnd">Line that ends each response. If null or empty string then the responses are single line.</param>
            /// <param name="errorBegin">String that begins an error response. If null or empty string then default string remains in use,
            /// i.e. <see cref="DefaultErrorBegin"/></param>
            /// <param name="startImmediately">If true then server is starrted immediately, otherwise this is postponed.</param>
            public InterpreterPipeServer(ICommandLineApplicationInterpreter interpreter, string pipeName, 
                bool startImmediately, string requestEnd, string responseEnd, string errorBegin) :
                base(pipeName, requestEnd, responseEnd, errorBegin, false /* startImmediately */)
            { 
                Interpreter = interpreter;
                if (startImmediately)
                    ThreadServe();
            }

            protected ICommandLineApplicationInterpreter _interpreter;

            public ICommandLineApplicationInterpreter Interpreter
            {
                get { return _interpreter; }
                protected set { _interpreter = value; }
            }


            /// <summary>Calculates and returns response </summary>
            /// <param name="request"></param>
            /// <returns></returns>
            public override string GetResponse(string request)
            {
                string ret = null;
                try
                {
                    string[] commandLine = Interpreter.GetArguments(request);
                    if (!Interpreter.ContainsCommand(commandLine[0]))
                    {
                        throw new ArgumentException("Pipe server's interpreter does not contain the command \"" + commandLine[0] + "\".");
                    }
                    ret = Interpreter.Run(commandLine);
                }
                catch (Exception)
                {
                    throw;
                }
                return ret;
            }

        }

        SortedList<string, IpcStreamServerBase> _pipeServers = null;

        /// <summary>Array of registered pipe servers, accessible through server name.</summary>
        SortedList<string, IpcStreamServerBase> PipeServers
        {
            get
            {
                if (_pipeServers == null)
                {
                    lock (Lock)
                    {
                        if (_pipeServers == null)
                            _pipeServers = new SortedList<string, IpcStreamServerBase>();
                    }
                }
                return _pipeServers;
            }
        }

        //SortedList<string, InterpreterPipeClient> _pipeClients = null;

        SortedList<string, IpcStreamClientBase> _pipeClients = null;

        ///// <summary>Array of registered pipe clients, accessible through client name.</summary>
        //SortedList<string, InterpreterPipeClient> PipeClients
        //{
        //    get
        //    {
        //        if (_pipeClients == null)
        //        {
        //            lock (Lock)
        //            {
        //                if (_pipeClients == null)
        //                    _pipeClients = new SortedList<string, InterpreterPipeClient>();
        //            }
        //        }
        //        return _pipeClients;
        //    }
        //}

        /// <summary>Array of registered pipe clients, accessible through client name.</summary>
        SortedList<string, IpcStreamClientBase> IpcClients
        {
            get
            {
                if (_pipeClients == null)
                {
                    lock (Lock)
                    {
                        if (_pipeClients == null)
                            _pipeClients = new SortedList<string, IpcStreamClientBase>();
                    }
                }
                return _pipeClients;
            }
        }


        #region NamedPipes.InterpreterCommandsServer


        /// <summary>Creates and registers a new interpreter's named pipe server.</summary>
        /// <param name="pipeName">Name of the pipe where the server listens.</param>
        /// <param name="serverName">name of the pipe server. If not specified then it is the  same as pipe name.</param>
        /// <param name="createCommand">Whether an interpreter command is created for accessig the server. Not functional at the moment.</param>
        /// <param name="outputLevel">Output level with which the server is started.</param>
        /// <returns>The created named pipe server.</returns>
        public InterpreterPipeServer CreatePipeServer(string pipeName, string serverName = null, bool createCommand = false,
            int outputLevel = 3)
        {
            if (string.IsNullOrEmpty(pipeName))
                throw new ArgumentException("Name of the named pipe is not specified (null or empty string).");
            if (serverName == null)
                serverName = pipeName;
            InterpreterPipeServer server = new InterpreterPipeServer(this, pipeName, false /* startImmediately */);
            server.OutputLevel = outputLevel;
            PipeServers.Add(serverName, server);
            server.ThreadServe();
            return server;
        }


        /// <summary>Removes the specified interpreter's named pipe servers. Servers are stopped and their pipes closed.
        /// Returns a string contaiing information about the removed servers.</summary>
        /// <param name="serverNames">Names of the servers to be removed.</param>
        /// <returns>A string containing basic information about the removed servers (i.e. their interpreter's names and pipe names).</returns>
        public string RemovePipeServers(params string[] serverNames)
        {
            StringBuilder sb = new StringBuilder();
            lock (Lock)
            {
                int numNames = 0;
                if (serverNames == null)
                    numNames = serverNames.Length;
                if (numNames == 0)
                {
                    IList<string> keys = PipeServers.Keys;
                    int numKeys = PipeServers.Keys.Count;
                    if (numKeys > 0)
                    {
                        serverNames = new string[numKeys];
                        numNames = numKeys;
                        for (int i = 0; i < numKeys; ++i)
                        {
                            serverNames[i] = keys[i];
                        }
                    }
                }
                for (int i = 0; i < numNames; ++i)
                {
                    string serverName = serverNames[i];
                    if (PipeServers.ContainsKey(serverName))
                    {
                        IpcStreamServerBase server = PipeServers[serverName];
                        PipeServers.Remove(serverName);
                        if (server != null)
                        {
                            server.ClosePipe();
                            server.StopServer();
                            sb.AppendLine(serverName + ":" + server.Name);
                        }
                    }
                }
            }
            return sb.ToString();
        }



        /// <summary>Returns a string containing informattion on the installed named pipe servers.</summary>
        /// <param name="serverName">Name of the pipe server (optional). If specified then information is returned only for the 
        /// server with this particular name (otherwise information is returned for all installed servers).</param>
        /// <returns></returns>
        public string PipeServerInfo(string serverName = null)
        {
            if (!string.IsNullOrEmpty(serverName))
            {
                string ret = null;
                if (PipeServers.ContainsKey(serverName))
                {
                    IpcStreamServerBase server = PipeServers[serverName];
                    ret = server.ToString();
                }
                return ret;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                if (PipeServers.Count < 1)
                    sb.AppendLine("There are no pipe servers installed on the interpreter.");
                else
                {
                    sb.AppendLine("Pipe servers installed on the interpreter: ");
                    IList<string> serverNames = PipeServers.Keys;
                    for (int i = 0; i < serverNames.Count; ++i)
                    {
                        string currentServerName = serverNames[i];
                        IpcStreamServerBase server = PipeServers[currentServerName];
                        sb.AppendLine(Environment.NewLine + "Named pipe server \"" + currentServerName + "\":"
                            + Environment.NewLine + server.ToString());
                    }
                }
                return sb.ToString();
            }
        }


        /// <summary>Command.
        /// Creates a new server that listens for interpreter commands on a named pipe, executes them, and writes result
        /// back to the named pipe.
        /// Command arguments are pipe name and server name (optional, if not specified then server name is the same as pipe name).</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>A string containing some basic data on the created pipe server.</returns>
        protected virtual string CmdPipeServerCreate(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " pipeName <serverName> <createCommand> <outputLevel>: creates a named pipe server." + Environment.NewLine
                            + "  pipeName: name of the named pipe on which the server listens for requests. " + Environment.NewLine
                            + "  serverName: name of the server (if omitted, it is set equal to the corresponding pipe name)." + Environment.NewLine
                            + "  createCommand: whether interpreter command for accessing the serrver is created. Not functional." + Environment.NewLine
                            + "  outputLevel: level of console output generated by server operation."); // + Environment.NewLine
                            // + "  createcommand: whether a command is created.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires at least 1 argument.");
            else if (args.Length < 1)
                throw new ArgumentException(cmdName + " : Invalid number of arguments, should be at least 1 (1st argument being pipe name).");
            else
            {
                string pipeName = args[0];
                string serverName = pipeName;
                bool createCommand = false;
                int outputLevel = 3;
                if (args.Length > 1)
                    serverName = args[1];
                if (args.Length > 2)
                    Util.TryParseBoolean(args[2], ref createCommand);
                if (args.Length > 3)
                    int.TryParse(args[3], out outputLevel);
                InterpreterPipeServer server = CreatePipeServer(pipeName, serverName, createCommand, outputLevel);
                if (this.OutputLevel >= 0)
                    Console.WriteLine(Environment.NewLine +"Pipe server started: " + serverName + Environment.NewLine);
                ret = "PipeServer " + serverName + ", PipeName = '" + pipeName + "'.";

            }
            return ret;
        }


        /// <summary>Command.
        /// Removes the spcified (or all, if names are not specified) named pipe servers.
        /// Command arguments are names of the pipe servers to be removed. If none is specified then all pipe servers 
        /// installed on the interperter are removed.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <returns>A string containing the information on the removed servers (their interpreter's names and pipe names).</returns>
        protected virtual string CmdPipeServersRemove(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <serverName1 serverName2 ...> : removes named pipe servers." + Environment.NewLine
                            + "  serverName1, ...: names of the named pipe servers to be removed. " + Environment.NewLine
                            + "    If no names are specified then all pipe servers are removed.");
                        Console.WriteLine();
                        return null;
                    }
            {
                ret = RemovePipeServers(args);
                Console.WriteLine("Removed the following named pipe servers: " + Environment.NewLine + ret);
            }
            return ret;
        }


        /// <summary>Command.
        /// Prints and returns inormation on the installed named pipe servers.
        /// Optional command argument is server name. If not specified then information about all installed servers is printed and returned.</param>
        /// <param name="cmdName">Command name.</param>
        /// <returns>A string containing the information on pipe servers.</returns>
        protected virtual string CmdPipeServerInfo(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <serverName>: Prints information about installed pipe servers." + Environment.NewLine
                            + "  serverName: name of the server for which information is printed. If omitted then info for all servers is printed.");
                        Console.WriteLine();
                        return null;
                    }
            {
                string serverName = null;
                if (args != null) if (args.Length >= 1)
                        serverName = args[0];
                ret = PipeServerInfo(serverName);
                Console.WriteLine(ret);
            }
            return ret;
        }


        #endregion NamedPipes.InterpreterCommandsServer



        #region NamedPipes.InterpreterCommandsClient


        /// <summary>Creates and registers a new interpreter's named pipe client.</summary>
        /// <param name="pipeName">Name of the pipe where the client listens.</param>
        /// <param name="clientName">name of the pipe client. If not specified then it is the  same as pipe name.</param>
        /// <param name="createCommand">Whether command for direct access is created or not. If true then an interpreter command with the
        /// same name as the name of the client is created, which can be directly used for sending requests by the created
        /// client (without specifying the command for sending a request first and then the server name and then the actual command).</param>
        /// <param name="outputLevel">Output level with which the client is started.</param>
        /// <returns>The created named pipe client.</returns>
        public InterpreterPipeClient CreatePipeClient(string pipeName, string clientName = null, bool createCommand = false,
            int outputLevel = 3)
        {
            if (string.IsNullOrEmpty(pipeName))
                throw new ArgumentException("Name of the named pipe is not specified (null or empty string).");
            if (clientName == null)
                clientName = pipeName;
            InterpreterPipeClient client = new InterpreterPipeClient(pipeName);
            client.OutputLevel = outputLevel;
            IpcClients.Add(clientName, client);
            client.Connect();
            return client;
        }

        /// <summary>Removes the specified interpreter's named pipe clients. Client's pipes are closed.
        /// Returns a string contaiing information about the removed clients.</summary>
        /// <param name="clientNames">Names of the clients to be removed.</param>
        /// <returns>A string containing basic information about the removed clients (i.e. their interpreter's names and pipe names).</returns>
        public string RemovePipeClients(params string[] clientNames)
        {
            StringBuilder sb = new StringBuilder();
            lock (Lock)
            {
                int numNames = 0;
                if (clientNames == null)
                    numNames = clientNames.Length;
                if (numNames == 0)
                {
                    IList<string> keys = IpcClients.Keys;
                    int numKeys = IpcClients.Keys.Count;
                    if (numKeys > 0)
                    {
                        clientNames = new string[numKeys];
                        numNames = numKeys;
                        for (int i = 0; i < numKeys; ++i)
                        {
                            clientNames[i] = keys[i];
                        }
                    }
                }
                for (int i = 0; i < numNames; ++i)
                {
                    string clientName = clientNames[i];
                    if (IpcClients.ContainsKey(clientName))
                    {
                        IpcStreamClientBase client = IpcClients[clientName];
                        IpcClients.Remove(clientName);
                        if (client != null)
                        {
                            client.ClosePipe();
                            sb.AppendLine(clientName + ":" + client.Name);
                        }
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>Returns a string containing informattion on the installed named pipe clients.</summary>
        /// <param name="clientName">Name of the pipe client. If specified then information is returned only for the 
        /// client with this particular name (otherwise information is returned for all installed clients).</param>
        /// <returns></returns>
        public string PipeClientInfo(string clientName = null)
        {
            if (!string.IsNullOrEmpty(clientName))
            {
                string ret = null;
                if (IpcClients.ContainsKey(clientName))
                {
                    IpcStreamClientBase client = IpcClients[clientName];
                    ret = client.ToString();
                }
                return ret;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                if (IpcClients.Count < 1)
                    sb.AppendLine("There are no pipe clients installed on the interpreter.");
                else
                {
                    sb.AppendLine("Pipe clients installed on the interpreter: ");
                    IList<string> clientNames = IpcClients.Keys;
                    for (int i = 0; i < clientNames.Count; ++i)
                    {
                        string currentClientName = clientNames[i];
                        IpcStreamClientBase client = IpcClients[currentClientName];
                        sb.AppendLine(Environment.NewLine + "IPC stream client \"" + currentClientName + "\":"
                            + Environment.NewLine + client.ToString());
                    }
                }
                return sb.ToString();
            }
        }



        /// <summary>Sends the specified command to the corresponding pipe serverr and reads and returns its response.</summary>
        /// <param name="clientName">Name of the pipe client that sends the request to the named pipe server and returns the response.</param>
        /// <param name="commandLineString">Comandline string that is sent to the server as request.</param>
        /// <returns>Response obtained from the named pipe server with which client is connected.</returns>
        public string PipeClientGetServerResponse(string clientName, string commandLineString)
        {
            string ret = null;
            if (string.IsNullOrEmpty(clientName))
                throw new ArgumentException("Pipe client name not specified (null or empty string).");
            if (!IpcClients.ContainsKey(clientName))
            {
                throw new ArgumentException("Pipe client does not exist: \"" + clientName + "\".");
            }
            else
            {
                IpcStreamClientBase client = IpcClients[clientName];
                // string commandLine = UtilStr.GetCommandLine(args);
                ret = client.GetServerResponse(commandLineString);
            }
            return ret;
        }


        /// <summary>Command.
        /// Creates a new client to the interpreter pipe server. The client can send command to the server listening on the
        /// specified named pipe, and recieves responses from the server and returns them.
        /// Command arguments are pipe name and client name (optional, if not specified then server name is the same as pipe name).</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <returns>A string containing the information on pipe clients.</returns>
        protected virtual string CmdPipeClientCreate(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " pipeName <clientName>: creates a named pipe client." + Environment.NewLine
                            + "  pipeName: name of the named pipe on which the client sends requests to the server. " + Environment.NewLine
                            + "  clientName: name of the client (if omitted, it is set equal to the corresponding pipe name)." + Environment.NewLine
                            + "  createCommand: whether interpreter command for sending commands by client is created." + Environment.NewLine
                            + "  outputLevel: level of console output generated by client operation.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires at least 1 argument.");
            else if (args.Length < 1)
                throw new ArgumentException(cmdName + " : Invalid number of arguments, should be at least 1 (1st argument being pipe name).");
            else
            {
                string pipeName = args[0];
                string clientName = pipeName;
                bool createCommand = false;
                int outputLevel = 3;
                if (args.Length > 1)
                    clientName = args[1];
                if (args.Length > 2)
                    Util.TryParseBoolean(args[2], ref createCommand);
                if (args.Length > 3)
                    int.TryParse(args[3], out outputLevel);
                InterpreterPipeClient client = CreatePipeClient(pipeName, clientName, createCommand, outputLevel);
                if (this.OutputLevel >= 1)
                    Console.WriteLine(Environment.NewLine + "Pipe client created: " + clientName + Environment.NewLine);
                ret = "PipeClient " + clientName + ", PipeName = '" + pipeName + "'.";
            }
            return ret;
        }

        /// <summary>Command.
        /// Removes the spcified (or all, if names are not specified) named pipe clients.
        /// Command arguments are names of the pipe clients to be removed. If none is specified then all pipe clients 
        /// installed on the interperter are removed.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <returns>A string containing the information on the removed clients (their interpreter's names and pipe names).</returns>
        protected virtual string CmdPipeClientsRemove(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <clientName1 clientName2 ...> : removes named pipe clients." + Environment.NewLine
                            + "  clientName1, ...: names of the named pipe clients to be removed. " + Environment.NewLine
                            + "    If no names are specified then all pipe clients are removed.");
                        Console.WriteLine();
                        return null;
                    }
            {
                ret = RemovePipeClients(args);
                Console.WriteLine("Removed the following named pipe clients: " + Environment.NewLine + ret);
            }
            return ret;
        }



        /// <summary>Command.
        /// Prints and returns inormation on the installed named pipe clients.
        /// Optional command argument is client name. If not specified then information about all installed clients is printed and returned.</param>
        /// <param name="cmdName">Command name.</param>
        /// <returns>A string containing some basic data on the created pipe client.</returns>
        protected virtual string CmdPipeClientInfo(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <clientName>: Prints information about installed pipe clients." + Environment.NewLine
                            + "  clientName: name of the client for which information is printed. If omitted then info for all clients is printed.");
                        Console.WriteLine();
                        return null;
                    }
            {
                string clientName = null;
                if (args != null) if (args.Length >= 1)
                        clientName = args[0];
                ret = PipeClientInfo(clientName);
                Console.WriteLine(ret);
            }
            return ret;
        }

        /// <summary>Command.
        /// Sends the specified command to the corresponding pipe serverr and reads and returns its response.
        /// Command argument is the (interpreter's) name of the pipe client followed by command and eventual arguments sent to the server.</param>
        /// <param name="cmdName">Command name.</param>
        /// <returns>Server response.</returns>
        protected virtual string CmdPipeClientGetServerResponse(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " clientName commandName <arg 1 arg2 ...>: " + Environment.NewLine 
                            + "  Seds a command to the named pipe server corresponding to the client, and returns server response." + Environment.NewLine
                            + "  clientName: name of the client that is used to send the request to the server." + Environment.NewLine
                            + "  commandName: name of the commands that is sent to the server for execution." + Environment.NewLine
                            + "  arrg1, arg2, ...: optional arguments to the command sent to the server.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires at least 1 argument (client name).");
            else if (args.Length < 1)
                throw new ArgumentException(cmdName + " : Requires at least 1 argument (client name).");
            else
            {
                string clientName = clientName = args[0];
                string commandLine = null;
                int numArgs = args.Length;
                if (numArgs > 1)
                {
                    string[] commandLineParts = new string[numArgs - 1];
                    for (int i = 0; i < numArgs - 1; ++i)
                    {
                        commandLineParts[i] = args[i + 1];
                    }
                    commandLine = UtilStr.GetCommandLine(commandLineParts);

                }
                ret = PipeClientGetServerResponse(clientName, commandLine);
            }
            return ret;
        }



        #endregion NamedPipes.InterpreterCommandsClient



        #endregion NamedPipes



        #region LoadableScriptCommands


        /// <summary>Interpreter command.
        /// Dynamically creates and runs an internal script object, i.e. an object of the class that is
        /// already compiled in the code.
        /// Interpreter command arguments:
        /// The first argument must be a full name of the script class whose object is run.
        /// The rest of the arguments (if any) are directly transferred to the executable method of the script
        /// and are also used as argument to script initialization method.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="commandName">Name of the command</param>
        /// <param name="commandArgumens">Command arguments.</param>
        /// <returns>Result of command execution.</returns>
        protected virtual string CmdRunInternalScriptClass(ICommandLineApplicationInterpreter interpreter, string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " classFullName arg1 arg2 ... :" + Environment.NewLine
                            + "  runs a command based on internal (compiled) scrip class.");
                        Console.WriteLine("  classFullName: full name of the script class (including namespace)"
                            + Environment.NewLine + "    that is executed.");
                        Console.WriteLine("The rest of the arguments (optional) will be passed to the executable method "
                            + Environment.NewLine + "  of the script.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + ": No arguments provided (null array). " + Environment.NewLine +
                    "  At least the script class full name should be specified.");
            if (args.Length == 0)
                throw new ArgumentException(cmdName + ": No arguments provided (empty array). " + Environment.NewLine +
                    "  At least the script class full name should be specified.");
            string scriptClassFullName = args[0];
            if (string.IsNullOrEmpty(scriptClassFullName))
                throw new ArgumentException(cmdName + ": scritp class full name is not specified (null or empty string).");
            string[] initAndRunArgs = new string[args.Length - 1];
            for (int i = 0; i < initAndRunArgs.Length; ++i)
                initAndRunArgs[i] = args[i + 1];
            // Instantiate the script object and run it:
            ILoadableScript script = null;
            script = ScriptLoaderBase.CreateScriptObject(scriptClassFullName);
            if (script == null)
                throw new InvalidOperationException("Could not find the following script class: " + scriptClassFullName);
            script.InitializationArguments = initAndRunArgs;
            return script.Run(initAndRunArgs);
        }  // CmdRunInternalScriptClass()


        /// <summary>Interpreter command.
        /// Dynamically loads (temporarily, just for execution of the current commad) a class form the 
        /// script contained in the specified file and executes its executable method.
        /// The file must contain the script that is dynamically loaded and executed, in form of definition of
        /// the appropriate class of type <typeparamref name="ILoadableScript"/>. 
        /// The dynamically loadable script class is loaded from the file and instantiated by the
        /// <see cref="LoadableScriptInterpreter"/> loadable script-based interpreter object.
        /// Interpreter command arguments:
        /// Path to the file containing loadable script must be the first argument to the method.
        /// The rest of the arguments (if any) are directly transferred to the executable method of the script.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="commandName">Name of the command</param>
        /// <param name="commandArgumens">Command arguments.</param>
        /// <returns>Result of command execution.</returns>
        protected virtual string CmdRunScriptFile(ICommandLineApplicationInterpreter interpreter, string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " scriptFile arg1 arg2 ... :" + Environment.NewLine
                            + "  runs a command based on dynamically compiled script loaded form a file.");
                        Console.WriteLine("  scriptFile: path to the file that contains dynamically loadable script "
                            + Environment.NewLine + "    that is executed.");
                        Console.WriteLine("The rest of the arguments (optional) will be passed to the executable method "
                            + Environment.NewLine + "  loaded from the script file.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + ": No arguments provided (null array). " + Environment.NewLine +
                    "  At least the script file path should be specified.");
            if (args.Length == 0)
                throw new ArgumentException(cmdName + ": No arguments provided (empty array). " + Environment.NewLine +
                    "  At least the script file path should be specified.");
            string scriptFilePath = args[0];
            if (string.IsNullOrEmpty(scriptFilePath))
                throw new ArgumentException(cmdName + ": scritp file is not specified (null or empty string).");
            string[] initAndRunArgs = new string[args.Length - 1];
            for (int i = 0; i < initAndRunArgs.Length; ++i)
                initAndRunArgs[i] = args[i + 1];
            // Run the script file:
            return RunScriptFile(scriptFilePath, initAndRunArgs);
        }  // CmdRunScriptFile()

        /// <summary>Interpreter command.
        /// Dynamically loads (compiles and instantiates) a loadable script class contained in the specified file, 
        /// and installs a new command on <see cref="LoadableScriptInterpreter"/> and on the current interpreter, 
        /// based on the dynamically allocated instance of the loaded (dynamically compiled) class.
        /// Required arguments to the interpreter command are: 
        ///   - name of the newly installed command
        ///   - name of the file containing the script code that defines a loadable script class.
        /// The rest of the arguments are passed to the dynamically generated instance of the 
        /// class that was dynamically compiled and loaded and stored (under the specified command name)
        /// on <see cref="LoadableScriptInterpreter"/>.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="commandName">Name of the command</param>
        /// <param name="commandArgumens">Command arguments.</param>
        /// <returns>null string.</returns>
        protected virtual string CmdLoadScript(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " cmdName scriptFile initArg1 initArg2 ... :" + Environment.NewLine
                            + "  instals a new command based on dynamically compiled script loaded form a file.");
                        Console.WriteLine("  cmdName : name of the newly installed command");
                        Console.WriteLine("  scriptFile: path to the file that contains dynamically loadable script "
                            + Environment.NewLine + "    defining execution of new command.");
                        Console.WriteLine("The rest of the arguments (optional) will be passed to initialization method of the class "
                            + Environment.NewLine + "  loaded from the script file.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + ": No arguments provided (null array). " + Environment.NewLine +
                    "  There should be at least command name and script file path.");
            if (args.Length == 0)
                throw new ArgumentException(cmdName + ": No arguments provided (empty array). " + Environment.NewLine +
                    "  There should be at least command name and script file path.");
            if (args.Length == 1)
                throw new ArgumentException(cmdName + ": Script file path is not specified.");
            string newCommandName = args[0];
            string scriptFilePath = args[1];
            if (string.IsNullOrEmpty(newCommandName))
                throw new ArgumentException(cmdName + ": command name is not specified (null or empty string).");
            if (string.IsNullOrEmpty(scriptFilePath))
                throw new ArgumentException(cmdName + ": dynamic scritp file is not specified (null or empty string).");
            string[] initArgs = new string[args.Length - 2];
            for (int i = 0; i < initArgs.Length; ++i)
                initArgs[i] = args[i + 2];
            // Add new dynamically loaded command on the LoadableScriptInterpreter, loaded form script file,
            // and add the command to the current interpreter.
            LoadScript(newCommandName, scriptFilePath, initArgs);
            // Add new command with the same name on the current interpreter, such that command will execute
            // teh dynamically loaded command on LoadableScriptInterpreter when executed:
            this.AddCommand(newCommandName, CmdRunLoadedScript);
            return null;
        }  // CmdLoadScript()

        /// <summary>Interpreter command.
        /// Runs a command based on dynamically loaded loadable script class. Arguments passed to this
        /// command are directly passed on to the dynamically loaded script class installed on 
        /// <see cref="LoadableScriptInterpreter"/> under the same <paramref name="cmdName"/>.
        /// Typically, the command that is executed by the current method, has been previously installed
        /// by the <see cref="CmdLoadScript"/>(...) method.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="commandName">Name of the command, which must be the same as command name
        /// under which dynamically loaded class is installed on <see cref="LoadableScriptInterpreter"/>.</param>
        /// <param name="commandArgumens">Command arguments. These arguments are directly passed to the 
        /// executable method on the corresponding class.</param>
        /// <returns>Result of command execution.</returns>
        protected virtual string CmdRunLoadedScript(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " arg1 arg2 ... : runs a dynamically loaded script.");
                        Console.WriteLine("  All arguments are directly passed to the executable method of the corresponding "
                            + Environment.NewLine + "  dynamically compiled and loaded object.");
                        Console.WriteLine();
                        Console.WriteLine("Trying to get help form the script...");
                        try
                        {
                            return LoadableScriptInterpreter.RunCommand(cmdName, args);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine();
                            Console.WriteLine("It seems that the script does not support help.");
                            Console.WriteLine();
                        }
                        return null;
                    }
            return RunLoadedScript(cmdName, args);
        }  // CmdLoadScript()

        /// <summary>Interpreter command.
        /// Writes to the console the assemblies that are currently referenced by compiler used 
        /// for dynamic loading of scripts. This information can be used for control if something
        /// goes wrong with dynamic script loading.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandArgumens">Command arguments.</param>
        /// <returns>Result of command execution, a list of all referenced assemblies.</returns>
        protected virtual string WriteLoadableScriptReferencedAssemblies(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : Writes a list of all assemblies referenced by .");
                        Console.WriteLine("  the dynamic script loader. ");
                        Console.WriteLine();
                    }
            string[] assemblies = GetLoadableScriptReferencedAssemblies();
            StringBuilder sb = new StringBuilder();
            Console.WriteLine();
            if (assemblies == null)
                Console.WriteLine("There are no referenced assemblies (null array).");
            else if (assemblies.Length == 0)
                Console.WriteLine("There are no referenced assemblies (empty array).");
            else
            {
                Console.WriteLine("List of assemblies referenced by the current script loader: ");
                foreach (string assembly in assemblies)
                {
                    Console.WriteLine("  " + assembly);
                    sb.AppendLine(assembly);
                }
            }
            Console.WriteLine();
            return sb.ToString();
        }  // CmdLoadScript()


        #endregion LoadableScriptCommands


        /// <summary>Execution method that exits the interpreter.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdExit(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string executableName = UtilSystem.GetCurrentProcessExecutableName();
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : Exits the interpreter.");
                        Console.WriteLine();
                        return null;
                    }
            Exit = true;
            return null;
        }


        /// <summary>Execution method for applications help.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdHelp(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            string executableName = UtilSystem.GetCurrentProcessExecutableName();
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("Help for the whole command interpreter follows below.");
                    }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("Run commands in the following way: ");
            sb.AppendLine("  " + executableName + " commandName arg1 arg2 arg3 ...");
            sb.AppendLine("where:");
            sb.AppendLine("  cmd: command name to start the host application ");
            sb.AppendLine("  commandName: application name ");
            sb.AppendLine("  arg1, arg2, arg3...: application arguments separated by spaces. ");
            sb.AppendLine();
            sb.AppendLine("Installed applications by names: ");
            if (_commands == null)
                sb.AppendLine("  No applications (null reference).");
            else if (_commands.Count < 1)
                sb.AppendLine("  No applications. ");
            else
            {
                sb.Append(" ");
                foreach (KeyValuePair<string, ApplicationCommandDelegate> pair in _commands)
                {
                    sb.AppendLine("  " + pair.Key);
                }
                sb.AppendLine();
            }
            sb.AppendLine();
            sb.AppendLine("Standard command for individual application's help: ");
            sb.AppendLine("  " + executableName + " commandName ? ");
            sb.AppendLine();
            // Console.WriteLine(sb.ToString());
            return sb.ToString();
        }

        /// <summary>Execution method that prints some information about the application.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdAbout(ICommandLineApplicationInterpreter interpreter, string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <level igLib versionLevel ass1, ass2...> : " + Environment.NewLine  
                            + "  prints some basic information about the current command interpreter.");
                        Console.WriteLine("    level: level of information included (1 - only basic information, default: 3)");
                        Console.WriteLine("    igLib: whether information on IGLib base library is also included.");
                        Console.WriteLine("    versionLevel: number of levels of version included.");
                        Console.WriteLine("    ass1, ass2...: simple names of eventual additional important assemblies whose names are included.");
                        Console.WriteLine();
                        return null;
                    }
            int infoLevel = 3;
            bool includeIglib = true;
            int versionLevel = 0;   // default
            bool parsed = false;
            List<Assembly> additionalAssemblies = null;
            Assembly[] additionalAssembliesArray = null;
            if (args != null && args.Length > 0)
            {
                if (!string.IsNullOrEmpty(args[0]))
                {
                    int res;
                    parsed = int.TryParse(args[0], out res);
                    if (parsed)
                        infoLevel = res;
                }
                if (args.Length > 1)
                {
                    if (!string.IsNullOrEmpty(args[1]))
                    {
                        bool res = true;
                        parsed = Util.TryParseBoolean(args[1], ref res);
                        if (parsed)
                            includeIglib = res;
                    }
                    if (args.Length > 2)
                    {
                        int res;
                        parsed = int.TryParse(args[2], out res);
                        if (parsed)
                            versionLevel = res;
                        if (args.Length > 3)
                        {
                            additionalAssemblies = new List<Assembly>();
                            for (int i = 3; i < args.Length; ++i)
                            {
                                string arg = args[i];
                                if (!string.IsNullOrEmpty(arg))
                                {
                                    Assembly assembly = UtilSystem.GetAssemblyByName(arg);
                                    if (assembly != null)
                                        additionalAssemblies.Add(assembly);
                                }
                            }
                            additionalAssembliesArray = additionalAssemblies.ToArray();
                        }
                    }
                }
            }
            string ret = Environment.NewLine + "Command line interpreter by Igor Grešovnik." + Environment.NewLine
                + UtilSystem.GetApplicationInfo(infoLevel, includeIglib, versionLevel, additionalAssembliesArray);
            return ret;
        }

        /// <summary>Execution method that prints some information about the current application.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdApplicationInfo(ICommandLineApplicationInterpreter interpreter, string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <level igLib versionLevel ass1, ass2...> : " + Environment.NewLine + "  prints some basic information about the current application.");
                        Console.WriteLine("    level: level of information included (1 - only basic information, default: 3)");
                        Console.WriteLine("    igLib: whether information on IGLib base library is also included.");
                        Console.WriteLine("    versionLevel: number of levels of version included.");
                        Console.WriteLine("    ass1, ass2...: simple names of eventual additional important assemblies whose names are included.");
                        Console.WriteLine();
                        return null;
                    }
            int infoLevel = 3;
            bool includeIglib = true;
            int versionLevel = 0;   // default
            bool parsed = false;
            List<Assembly> additionalAssemblies = null;
            Assembly[] additionalAssembliesArray = null;
            if (args != null && args.Length > 0)
            {
                if (!string.IsNullOrEmpty(args[0]))
                {
                    int res;
                    parsed = int.TryParse(args[0], out res);
                    if (parsed)
                        infoLevel = res;
                }
                if (args.Length > 1)
                {
                    if (!string.IsNullOrEmpty(args[1]))
                    {
                        bool res = true;
                        parsed = Util.TryParseBoolean(args[1], ref res);
                        if (parsed)
                            includeIglib = res;
                    }
                    if (args.Length > 2)
                    {
                        int res;
                        parsed = int.TryParse(args[2], out res);
                        if (parsed)
                            versionLevel = res;
                        if (args.Length > 3)
                        {
                            additionalAssemblies = new List<Assembly>();
                            for (int i = 3; i < args.Length; ++i)
                            {
                                string arg = args[i];
                                if (!string.IsNullOrEmpty(arg))
                                {
                                    Assembly assembly = UtilSystem.GetAssemblyByName(arg);
                                    if (assembly != null)
                                        additionalAssemblies.Add(assembly);
                                }
                            }
                            additionalAssembliesArray = additionalAssemblies.ToArray();
                        }
                    }
                }
            }
            string ret = Environment.NewLine + UtilSystem.GetApplicationInfo(infoLevel, includeIglib, versionLevel, additionalAssembliesArray);
            return ret;
        }

        /// <summary>Execution method that does nothing (for comments).</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdComment(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " whateverArgs : Comment. Ignores this line.");
                        Console.WriteLine();
                        return null;
                    }
            return null;
        }

        /// <summary>Execution method for command that prints names of all installed applications.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdPrintCommands(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : prints all commands installed.");
                        Console.WriteLine();
                        return null;
                    }
            Console.WriteLine();
            Console.WriteLine("Installed commands by names: ");
            if (_commands == null)
                Console.WriteLine("  No commands (null reference).");
            else if (_commands.Count < 1)
                Console.WriteLine("  No commands. ");
            else
            {
                Console.Write(" ");
                foreach (KeyValuePair<string, ApplicationCommandDelegate> pair in _commands)
                {
                    Console.WriteLine("  " + pair.Key);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
            return null;
        }

        /// <summary>Executinon method for test command, which just prints its name and arguments.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdTestProduct(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : test command, just multiplies its arguments.");
                        Console.WriteLine();
                        return null;
                    }
            //Console.WriteLine();
            //Console.WriteLine("Test command launched.");
            //Console.WriteLine("Command name: " + cmdName);
            //Console.WriteLine("Command arguments (in double quotes): ");
            if (args == null)
                return "0";
            else if (args.Length == 0)
                return "0";
            double ret = 1;
            for (int i = 0; i < args.Length; ++i)
            {
                double factor = double.Parse(args[i]);
                ret *= factor;
            }
            return ret.ToString();
        }

        /// <summary>Executinon method for test command, which just prints its name and arguments.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdTest(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : test command, just prints its arguments.");
                        Console.WriteLine();
                        return null;
                    }
            Console.WriteLine();
            Console.WriteLine("Test command launched.");
            Console.WriteLine("Command name: " + cmdName);
            Console.WriteLine("Command arguments (in double quotes): ");
            if (args == null)
                Console.WriteLine("  No arguments(null reference).");
            else if (args.Length == 0)
                Console.WriteLine("  No arguments.");
            else
                Console.Write("  ");
            {
                for (int i = 0; i < args.Length; ++i)
                    Console.Write("\"" + args[i] + "\" ");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            return "Return from the 'Test' command.";
        }


        /// <summary>Description of reference configuration.</summary>
        protected const string TestSpeedReferenceSystem = "Intel Core i7 2.8 GHz";

        /// <summary>Number of equations used to test speed of numerical computations.</summary>
        protected const int TestSpeedNumEq = 500;

        /// <summary>Reference execution time for speed test for numerical operations.</summary>
        protected const double TestSpeedReferenceTime = 0.640917;

        /// <summary>Number of equations used to test speed of numerical computations.</summary>
        protected const int TestSpeedLongNumEq = 1000;

        /// <summary>Reference execution time for speed test for numerical operations.</summary>
        protected const double TestSpeedLongReferenceTime = 63.9688;

        /// <summary>Executinon method for TestSpeed command, performs test of speed of numerical computations
        /// on LU decomposition, and outputs the result and comparison with reference results (usually achieved on Igor's computer).</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Speed factor - Ratio between reference computation time and time spent for the same thing in current environment.</returns>
        protected virtual string CmdTestSpeed(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " numRepeats : tests current speed of numerical computation.");
                        Console.WriteLine("  numRepeats: number of times the test is repeated (to calculate average time).");
                        Console.WriteLine();
                        return null;
                    }
            int numEquations = TestSpeedNumEq;
            int numRepeats = 1;
            if (args != null)
                if (args.Length > 0)
                    numRepeats = int.Parse(args[0]);
            Console.WriteLine();
            Console.WriteLine("Test of speed of numerical computation in current environment... ");
            double averageTime = 0.0;
            double totalTime = 0.0;
            int threadId = Thread.CurrentThread.GetHashCode();
            for (int i = 1; i <= numRepeats; ++i)
            {
                if (numRepeats > 1)
                {
                    Console.WriteLine();
                    Console.WriteLine("Repetition No. " + i + " / " + numRepeats + " (thread " + threadId + ") ...");
                }
                double currentTime = TestComputationalTimesLU(numEquations, 0);
                totalTime += currentTime;
                averageTime = totalTime / (double)i;
                if (numRepeats > 1)
                {
                    Console.WriteLine("... done (" + i + "/" + numRepeats + ", thread " + threadId + ")"
                        + Environment.NewLine + "        in " + currentTime + " s.");
                    Console.WriteLine("    Average time: " + averageTime + " s, factor: " + (TestSpeedReferenceTime / averageTime));
                }
            }
            Console.WriteLine();
            Console.WriteLine("Speed compared to: " + TestSpeedReferenceSystem);
            Console.WriteLine("Average time: " + averageTime + " s. ");
            Console.WriteLine("Speed factor: " + TestSpeedReferenceTime / averageTime);
            Console.WriteLine();
            return null;
        }

        /// <summary>Executinon method for TestSpeedLong command, performs a longer test of speed of numerical computations
        /// on QR decomposition, and outputs the result and comparison with reference results (usually achieved on Igor's computer).</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Speed factor - Ratio between reference computation time and time spent for the same thing in current environment.</returns>
        protected virtual string CmdTestSpeedLong(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " numRepeats : " + Environment.NewLine
                            + " tests current speed of numerical computation (longer test).");
                        Console.WriteLine("  numRepeats: number of times the test is repeated (to calculate average time).");
                        Console.WriteLine();
                        return null;
                    }
            int numEquations = TestSpeedLongNumEq;
            int numRepeats = 1;
            if (args != null)
                if (args.Length > 0)
                    numRepeats = int.Parse(args[0]);
            Console.WriteLine();
            Console.WriteLine("Test of speed of numerical computation in current environment (longer test)... ");
            double averageTime = 0.0;
            double totalTime = 0.0;
            int threadId = Thread.CurrentThread.GetHashCode();
            for (int i = 1; i <= numRepeats; ++i)
            {
                if (numRepeats > 1)
                {
                    Console.WriteLine();
                    Console.WriteLine("Repetition No. " + i + " / " + numRepeats + " (thread " + threadId + ") ...");
                }
                double currentTime = TestComputationalTimesQR(numEquations, 0);
                totalTime += currentTime;
                averageTime = totalTime / (double)i;
                if (numRepeats > 1)
                {
                    Console.WriteLine("... done (" + i + "/" + numRepeats + ", thread " + threadId + ")"
                        + Environment.NewLine + "        in " + currentTime + " s.");
                    Console.WriteLine("    Average time: " + averageTime + " s, factor: " + (TestSpeedLongReferenceTime / averageTime));
                }
            }
            Console.WriteLine();
            Console.WriteLine("Speed compared to: " + TestSpeedReferenceSystem);
            Console.WriteLine("Average time: " + averageTime + " s. ");
            Console.WriteLine("Speed factor: " + TestSpeedLongReferenceTime / averageTime);
            Console.WriteLine();
            return null;
        }  // CmdTestSpeedLong

        protected static int DefaultNumEquations = 1000;

        /// <summary>Executinon method for TestQR command, performs test of QR decomposition.
        /// <para>Command takes 1 argument that is dimension of the system of equations to be solved.
        /// If dimension is not stated then default value is taken.</para>
        /// <para>Optionally, command can take the second argument that represents number of repetitions of the 
        /// decomposition test. In this tame, command returns average total execution time for each test.</para></summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Total wallclock time spent for computation.</returns>
        protected virtual string CmdTestQR(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + "  dimension <numRep> : " + Environment.NewLine
                            + "  tests QR dcomposition and prints execution times.");
                        Console.WriteLine("  dimension: number of equations (optional; default is " + DefaultNumEquations + ")");
                        Console.WriteLine("  numRep: number of repetitions of test (optional; default is " + 1 + ")");
                        Console.WriteLine();
                        return null;
                    }
            if (DefaultNumEquations < 1)
                DefaultNumEquations = 1;
            int numRepeats = 1;
            int numEquations = DefaultNumEquations;
            if (args != null)
                if (args.Length > 0)
                {
                    numEquations = int.Parse(args[0]);
                    if (args.Length > 1)
                        numRepeats = int.Parse(args[1]);
                }
            double cumulativeTime = 0, averageTime = 0;
            int threadId = Thread.CurrentThread.GetHashCode();
            for (int i = 1; i <= numRepeats; ++i)
            {
                if (numRepeats > 1)
                {
                    Console.WriteLine(Environment.NewLine + "Test No. " + i + " / " + numRepeats
                        + " (thread " + threadId + ") ...");
                }
                double totalWallclockTime = TestComputationalTimesQR(numEquations, 3);
                cumulativeTime += totalWallclockTime;
                averageTime = cumulativeTime / (int)i;
                if (numRepeats > 1)
                {
                    Console.WriteLine("Test " + i + "/" + numRepeats
                        + " (thread " + threadId + "), average total time: " + averageTime);
                }
            }
            return averageTime.ToString();
        }


        /// <summary>Test of QR decomposition. Writes times necessary for all steps.</summary>
        /// <param name="numEq">Number of equations to be solved with decomposition.</param>        
        /// <param name="outLevel">Level of output.</param>
        /// <returns>Total time spent for all operations.</returns>
        protected virtual double TestComputationalTimesQR(int numEq, int outLevel)
        {
            return SpeedTestCpu.TestComputationalTimesQR(numEq, outLevel);
        }// TestComputationalTimesQR()


        /// <summary>Executinon method for TestLU command, performs test of LU decomposition.
        /// <para>Command takes 1 argument that is dimension of the system of equations to be solved.
        /// If dimension is not stated then default value is taken.</para>
        /// <para>Optionally, command can take the second argument that represents number of repetitions of the 
        /// decomposition test. In this tame, command returns average total execution time for each test.</para></summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Total wallclock time spent for computation.</returns>
        protected virtual string CmdTestLU(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + "  dimension <numRep> : " + Environment.NewLine
                            + "  tests LU dcomposition and prints execution times.");
                        Console.WriteLine("  dimension: number of equations (optional; default is " + DefaultNumEquations + ")");
                        Console.WriteLine("  numRep: number of repetitions of test (optional; default is " + 1 + ")");
                        Console.WriteLine();
                        return null;
                    }
            if (DefaultNumEquations < 1)
                DefaultNumEquations = 1;
            int numRepeats = 1;
            int numEquations = DefaultNumEquations;
            if (args != null)
                if (args.Length > 0)
                {
                    numEquations = int.Parse(args[0]);
                    if (args.Length > 1)
                        numRepeats = int.Parse(args[1]);
                }
            double cumulativeTime = 0, averageTime = 0;
            int threadId = Thread.CurrentThread.GetHashCode();
            for (int i = 1; i <= numRepeats; ++i)
            {
                if (numRepeats > 1)
                {
                    Console.WriteLine(Environment.NewLine + "Test No. " + i + " / " + numRepeats
                        + " (thread " + threadId + ") ...");
                }
                double totalWallclockTime = TestComputationalTimesLU(numEquations, 3);
                cumulativeTime += totalWallclockTime;
                averageTime = cumulativeTime / (int)i;
                if (numRepeats > 1)
                {
                    Console.WriteLine("Test " + i + "/" + numRepeats
                        + " (thread " + threadId + "), average total time: " + averageTime);
                }
            }
            return averageTime.ToString();
        }


        /// <summary>Test of LU decomposition.</summary>
        /// <param name="outLevel">Level of output.</param>
        /// <param name="numEq">Number of equations to be solved with decomposition.</param>
        protected virtual double TestComputationalTimesLU(int numEq, int outLevel)
        {
            return SpeedTestCpu.TestComputationalTimesLU(numEq, outLevel);
        }  // TestComputationalTimesLU()



        #endregion CommandMethods


        #region Modules

        /// <summary>Loads the first built-in test module (just for testing modules).</summary>
        /// <param name="name">Name under which the module is being loaded.</param>
        /// <param name="interpreter">Command line interpreter under on which the module is loaded.</param>
        void ModuleTest1(string name, ICommandLineApplicationInterpreter interpreter)
        {
            // Remark: remove the console output below if you don't want to inform the user about installation of module
            // and its commands!
            Console.WriteLine();
            Console.WriteLine("Module is being loaded: " + name);
            Console.WriteLine();
            Console.WriteLine("This is TestModule1 that is built in the basic interpreter.");
            Console.WriteLine();
            interpreter.AddCommand("Test", CmdTestFromTestModules);
            interpreter.AddCommand("Test1", CmdTestFromTestModules);
            Console.WriteLine("Module commands: ");
            Console.WriteLine("  " + ModuleTestCommandName(name) + " - automatically installed when a module is loaded.");
            Console.WriteLine("  Test -  test commands, replaces the old 'test' command");
            Console.WriteLine("  Test1 - test commands, the same command with different name");
            Console.WriteLine();
            Console.WriteLine("Module loaded: " + name + ".");
            Console.WriteLine();
        }

        /// <summary>Loads the second built-in test module (just for testing modules).</summary>
        /// <param name="name">Name under which the module is being loaded.</param>
        /// <param name="interpreter">Command line interpreter under on which the module is loaded.</param>
        void ModuleTest2(string name, ICommandLineApplicationInterpreter interpreter)
        {
            // Remark: remove the console output below if you don't want to inform the user about installation of module
            // and its commands!
            Console.WriteLine();
            Console.WriteLine("Module is being loaded: " + name);
            Console.WriteLine();
            Console.WriteLine("This is TestModule2 that is built in the basic interpreter.");
            Console.WriteLine();
            interpreter.AddCommand("Test", CmdTestFromTestModules);
            interpreter.AddCommand("Test2", CmdTestFromTestModules);
            Console.WriteLine("Module commands: ");
            Console.WriteLine("  " + ModuleTestCommandName(name) + " - automatically installed when a module is loaded.");
            Console.WriteLine("  Test -  test commands, replaces the old 'test' command");
            Console.WriteLine("  Test2 - test commands, the same command with different name");
            Console.WriteLine();
            Console.WriteLine("Module loaded: " + name + ".");
            Console.WriteLine();
        }


        #endregion Modules


    }  // class MessyApplications

}