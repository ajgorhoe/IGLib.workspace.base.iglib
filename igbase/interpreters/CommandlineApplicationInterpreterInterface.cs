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
    /// $A Igor xx Aug08 Dec09;
    public interface ICommandLineApplicationInterpreter
    {

        /// <summary>Name of the current interpreter.</summary>
        string Name
        { get; set; }

        /// <summary>Description of hte current interpreter.</summary>
        string Description
        { get; set; }
        
        /// <summary>Global frame where global variables are stored.</summary>
        CommandStackFrame GlobalFrame { get; }

        /// <summary>Main command thread of the interpreter (usually run in the same thread where interpreter was created).</summary>
        CommandThread MainThread { get; }

        /// <summary>List of command threads that exist on the interpreter.</summary>
        List<CommandThread> CommandThreads { get; }

        /// <summary>Whether the exit flag is set, usually causing interpreter to stop.</summary>
        bool Exit
        { get; }

        ///// <summary>Gets the stopwatch used for measuring time of commands.
        ///// <para>This property always returns an initialized stopwatch.</para></summary>
        //StopWatch1 Timer
        //{ get; }

        /// <summary>Level of output for some of the interpreter's functionality (e.g. asynchronous command execution).</summary>
        int OutputLevel
        { get; set; }

        /// <summary>Specifies whether a wrning should be launched whenever an installed command
        /// is being replaced.</summary>
        bool WarnCommandReplacement
        { get; set; }


        /// <summary>Returns the value of the specified variable of the current command line interpreter.
        /// null is returned if the specified variable does not exist.</summary>
        /// <param name="commandThread">Command thread that is being executed.</param>
        /// <param name="varName">Name of the variable.</param>
        string GetVariableValue(CommandThread commandThread, string varName);

        /// <summary>Sets the specified variable to the specified value.</summary>
        /// <param name="commandThread">Command thread that is being executed.</param>
        /// <param name="varName">Name of the variable to be set.</param>
        /// <param name="value">Value that is assigned to the variable.</param>
        /// <returns>New value of the variable (before the method was called).</returns>
        string SetVariableValue(CommandThread commandThread, string varName, string value);

        /// <summary>Clears (removes) the specified variable.</summary>
        /// <param name="commandThread">Command thread that is being executed.</param>
        /// <param name="varName">Name of the variable to be cleared.</param>
        /// <returns>null.</returns>
        string ClearVariable(CommandThread commandThread, string varName);

        /// <summary>Prints the specified variable.</summary>
        /// <param name="commandThread">Command thread that is being executed.</param>
        /// <param name="varName">Name of the variable to be cleared.</param>
        /// <returns>null.</returns>
        string PrintVariable(CommandThread commandThread, string varName);

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
        /// <param name="commandThread">Command thread that is being executed.</param>
        /// <param name="filePath">Path to the file containing commands.</param>
        /// <returns>Return value of the last command.</returns>
        string RunFile(CommandThread commandThread, string filePath);

        /// <summary>Reads commands one by one from the standard input and executes them.</summary>
        /// <param name="commandThread">Command thread that is being executed.</param>
        string RunInteractive(CommandThread commandThread);


        /// <summary>Returns true if the specified command is installed on the interpreter, false if not.
        /// <para>Case sensitivity of the interpreter is treated appropriately.</para></summary>
        /// <param name="commandName">Name of the command that is checked.</param>
        /// <returns>True if the specified command is installed on the current interpreter, false if not.</returns>
        bool ContainsCommand(string commandName);


        /// <summary>Runs the specified command with specified name, installed on the current application object, without any
        /// modifications of the command arguments.</summary>
        /// <param name="commandThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <remarks>This method should not be overriden, but the <see cref="Run(CommandThread, string, string[])"/> method can be, e.g. in order to 
        /// perform some argument or command name transformations.</remarks>
        string RunWithoutModifications(CommandThread commandThread, 
            string commandName, params string[] commandArguments);

        /// <summary>Runs the command with specified name, installed on the current application object.</summary>
        /// <param name="commandThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="commandName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        string Run(CommandThread commandThread, string commandName, string[] args);

        /// <summary>Runs command where the first argument is command name.
        /// Extracts application name and runs the corresponding application delegate.Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.</summary>
        /// <param name="commandThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        string Run(CommandThread commandThread, string[] args);

        /// <summary>Runs a command asynchronously where the first argument is command name.
        /// Extracts command name and runs the corresponding application delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.</summary>
        /// <param name="commandThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        string RunAsync(CommandThread commandThread, string[] args);


        /// <summary>Runs the command with specified name (installed on the current interpreter object) asynchronously.</summary>
        /// <param name="commandThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <returns>ID of asynchronous run used to query results and whether command has completed, or -1 if a call was not
        /// launched (actually, an exception would be thrown in this case).</returns>
        string RunAsync(CommandThread commandThread, string commandName, params string[] commandArguments);

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

        /// <summary>Adds command with the specified name.
        /// <para>Commands added in this way are suitable for single threaded invocation, unless they don't interact with interpreter.</para></summary>
        /// <param name="appName">Name of the commant.</param>
        /// <param name="appMain">Delegate that will be used to execute the command.</param>
        [Obsolete("Commands added in this way are suitable only for single threaded invocation.")]
        void AddCommand(string appName, /* CommandLineApplicationInterpreter.*/
            ApplicationCommandDelegate appMain);
        
        /// <summary>Adds command with the specified name.</summary>
        /// <param name="commandName">Name of the commant.</param>
        /// <param name="commandDelegate">Delegate that will be used to execute the command.</param>
        /// <remarks>WARNING: This should be removed later. It is only here such that when the signature of 
        /// the old command functions is changed, the AddCommand can act on them, so no new errors need
        /// to be corrected. In the future, this shoud be removed, and AddCommandMt should be renamed to
        /// AddCommand, and the old obsolete AddCommand should be also removed.</remarks>
        /// [Obsolete "Converge both methods with the same signature to only one method - AddCommandMt should be used and renamed."]
        void AddCommand(string commandName, ApplicationCommandDelegateMt commandDelegate);

        /// <summary>Adds command with the specified name.</summary>
        /// <param name="appName">Name of the commant.</param>
        /// <param name="appMain">Delegate that will be used to execute the command.</param>
        void AddCommandMt(string appName, /* CommandLineApplicationInterpreter.*/
            ApplicationCommandDelegateMt appMain);



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
        void AddModule(string moduleName, /* CommandLineApplicationInterpreter. */ ModuleDelegate moduleDelegate);

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

}