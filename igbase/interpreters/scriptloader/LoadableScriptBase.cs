// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Lib
{

   
    /// <summary>Command-line interpreters adapted for executing script commands.</summary>
    /// $A Igor Nov08;
    public class CommandLineApplicationScriptInterpreter : CommandLineApplicationInterpreter, 
        ICommandLineApplicationInterpreter, ILockable
    {

        /// <summary>Runs the specified command with specified name, installed on the current application object.</summary>
        /// <param name="commandThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <remarks>This command overrides the original Run commands in such a way that it treats script commands differently form 
        /// the original interpreter commands.
        /// <para>If a command is script command (i.e. contained on the <see cref="Script_CommandHelpStrings"/> list) then
        /// its arguments will contain the command name, otherwise they will not.</para></remarks>
        public override string Run(CommandThread commandThread, string commandName, params string[] commandArguments)
        {
            string[] args = commandArguments;
            if (Script_ContainsScriptCommand(commandName))
            {
                int numArgs = 0;
                if (commandArguments != null)
                    numArgs = commandArguments.Length;
                args = new string[numArgs + 1];
                args[0] = commandName;
                for (int i=0; i<numArgs; ++i)
                {
                    args[i+1] = commandArguments[i];
                }
            }
            return base.Run(commandThread, commandName, args);
        }

        private SortedList<string, string> _script_CommandHelpStrings;

        /// <summary>Contains help strings associated with script commands installed on the SCRIPT's interpreter.
        /// This is used to distinguish script commands form usual interpreter commands.</summary>
        public SortedList<string, string> Script_CommandHelpStrings
        {
            get
            {
                lock (Lock)
                {
                    if (_script_CommandHelpStrings == null)
                        _script_CommandHelpStrings = new SortedList<string, string>();
                    return _script_CommandHelpStrings;
                }
            }
            set
            {
                lock (Lock) { _script_CommandHelpStrings = value; }
            }
        }


        /// <summary>Returns true if the specified command is script command (i.e. its first argument is command-name and it is run
        /// through the <see cref="LoadableScriptBase.Script_CommandAdapter"/> object).</summary>
        /// <param name="commandName">Name of the command that is queried.</param>
        /// <remarks><para>A command is considered script command if it is contained on the <see cref="Script_CommandHelpStrings"/> list.</para></remarks>
        public virtual bool Script_ContainsScriptCommand(string commandName)
        {
            if (_script_CommandHelpStrings != null)
            {
                if (_script_CommandHelpStrings.ContainsKey(commandName))
                    return true;
            }
            return false;
        }

    }


    /// <summary>Interface for classes that can be dynamically loadeded from scripts and run, 
    /// which provides functionality of dynamic scripting. 
    /// It is recommendable to derive all such classes from the <see cref="LoadableScriptBase"/> base class.</summary>
    /// $A Igor Jul09 May10;
    public interface ILoadableScript
    {

        #region EmbeddedScriptSupport

        /// <summary>Command that was used to launch the current embedded application script.</summary>
        string EmbeddedCommandName
        { get; set; }

        #endregion EmbeddedScriptSupport

        /// <summary>Runs the executable method of the object.</summary>
        /// <param name="arguments">String arguments to the executable method.</param>
        /// <returns></returns>
        string Run(string[] arguments);

        /// <summary>Arguments used by the initialization method.</summary>
        string[] InitializationArguments
        { get; set; }

        /// <summary>Whether the object has been initialized or not.</summary>
        bool IsInitialized
        { get; }

        /// <summary>Initializes the current object.</summary>
        /// <param name="arguments">Arguments of initialization.</param>
        void Initialize(string[] arguments);

    }  // interface ILoadableScript

    /// <summary>Controllable loadable script, provides more control over loading and execution.</summary>
    /// $A Igor Jul09;
    public interface ILoadableScriptC : ILoadableScript
    {

        /// <summary>Either or not the script can be dynamically loaded.</summary>
        bool IsLoadable
        {
            get;
            set;
        }

        /// <summary>Either or not the script can be run (some scripts only support other tasks).</summary>
        bool IsRunnable
        {
            get;
            set;
        }

    }


    /// <summary>Base class for classes that can be dynamically loadeded from scripts and run, 
    /// which provides functionality of dynamic scripting. It is recommendable to derive all 
    /// such classes that implement the <see cref="ILoadableScript"/> interface from this base class.</summary>
    /// <remarks>Initialization of objects of this class can be performed by the <see cref="Initialize"/> method.
    /// If not performed explicitly, initialization is perform automatically at the first call to teh Run() method.</remarks>
    /// $A Igor Jul09 May10;
    public abstract class LoadableScriptBase : ILoadableScript, ILockable
    {

        /// <summary>Argument-less constructor. If argument-less constructor is called then initialization is not
        /// performed and will be performed later.</summary>
        public LoadableScriptBase()
        {  }

        ///// <summary>Constructs the class, calls the <see cref="Initialize"/>() method, which in turn 
        ///// calls the <see cref="InitializeThis"/>() method.</summary>
        //public LoadableScriptBase(string [] arguments) : this()
        //{
        //    InitializationArguments = arguments;
        //}


        #region EmbeddedScriptSupport

        protected string _embeddedCommandName = null;

        /// <summary>Command that was used to launch the current embedded application script.</summary>
        public string EmbeddedCommandName
        {
            get { return _embeddedCommandName; }
            set { _embeddedCommandName = value; }
        }

        #endregion EmbeddedScriptSupport



        /// <summary>Delegate for internal command methods.</summary>
        /// <param name="commandName">Name of the internal command.</param>
        /// <param name="args">Arguments to the command.</param>
        /// <returns></returns>
        protected delegate string CommandMethod(string commandName, string[] args);


        #region ILockable

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ILockable


        #region ILoadable_Interface


        /// <summary>Performs the action of this object.
        /// Override this in derived classes!</summary>
        /// <param name="arguments">Arguments through which different information can be passed.</param>
        /// <returns>String that represents result of the action.</returns>
        protected abstract string RunThis(string[] arguments);

        /// <summary>Performs all the necessary initializations of the object.
        /// Override this method in derived classes (if extra initialization is needed)
        /// and call the base class' method in it.</summary>
        protected abstract void InitializeThis(string[] arguments);


        /// <summary>Performs the action of this object.
        /// Override this in derived classes!</summary>
        /// <param name="arguments">Arguments through which different information can be passed.</param>
        /// <returns>String that represents result of the action.</returns>
        public string Run(string[] arguments)
        {
            lock (Lock)
            {
                if (!IsInitialized)
                    Initialize(InitializationArguments);  // initializes with initialization arguments.
            }
            return RunThis(arguments);
        }


        private string[] _initializationArguments;

        /// <summary>Arguments used by the initialization method.
        /// WARNING: arguments can only be set before initialization is performed.
        /// Initialization is performed either implicitly at the first call to the <see cref="Run"/> method 
        /// or explicitly by calling the <see cref="Initialize"/> method.</summary>
        public string[] InitializationArguments
        {
            get { return _initializationArguments; }
            set 
            {
                if (IsInitialized)
                {
                    throw new InvalidOperationException("Can not set initialization arguments because the object is already initialized.");
                }
                _initializationArguments = value;
            }
        }

        private bool _isInitialized = false;

       /// <summary>Whether the object has been initialized or not.</summary>
        public virtual bool IsInitialized
        {  
            get { return _isInitialized; }
            protected set { _isInitialized = value; }
        }


        /// <summary>Initializes the object. 
        /// If not called explicitly, this method is automatically called at the first call to the
        /// <see cref="Run"/> method.</summary>
        public void Initialize(string[] arguments)
        {
            lock (Lock)
            {
                if (!IsInitialized)
                {
                    InitializeThis(arguments);
                    InitializationArguments = arguments;
                    IsInitialized = true;
                }
            }
        }


        #endregion ILoadable_Interface

        #region ScriptInterpreter

        // This region defines script's internal interpreter where script individual commands can be loaded.
        // Script commands recieve the same command arguments as the Run() command.

        private static int _defaultOutputLevel = -1;

        ///// <summary>Default output level for loadable scripts (mainly affects methods related to internal interpreter).</summary>
        public static int DefaultOutputLevel
        {
            get {
                if (_defaultOutputLevel >= 0)
                    return _defaultOutputLevel;
                else
                    return Util.OutputLevel;
            }
            set { _defaultOutputLevel = value; }
        }


        protected int _outputLevel = DefaultOutputLevel; 

        /// <summary>Level of output to console produced by some operations of the current object.</summary>
        public int OutputLevel
        {
            get { return _outputLevel; }
            set { _outputLevel = value; }
        }


        /// <summary>Default initialization method for scripts.</summary>
        /// <param name="arguments">Initialization arguments.</param>
        /// <returns>null string.</returns>
        public virtual string Script_DefaultInitialize(string[] arguments)
        {
            if (OutputLevel >= 3)
            {
                Console.WriteLine();
                Console.WriteLine("******** SCRIPT INITIALIZATION... *********");
                Console.WriteLine("Class name: " + this.GetType().Name);
                if (OutputLevel >= 2)
                {
                    Script_PrintArguments("Initialization arguments: ", arguments);
                }
                Console.WriteLine("-------------------------------------------");
            }
            return null;
        }

        /// <summary>Default run method for the script. Can be used when only installed commands are run by hte script.</summary>
        /// <param name="arguments">Command arguments. The first argument must be name of the command that is run.</param>
        /// <returns>Return value of the script command that is run.</returns>
        public virtual string Script_DefaultRun(string[] arguments)
        {
            if (OutputLevel >= 3)
            {
                Console.WriteLine();
                Console.WriteLine("*********** SCRIPT EXECUTION... ***********");
                Console.WriteLine("Class name: " + this.GetType().Name);
                if (OutputLevel >= 2)
                {
                    Script_PrintArguments("Run arguments: ", arguments);
                }
                Console.WriteLine("-------------------------------------------");
            }
            string ret = null;
            // bool commandRun = false;
            try
            {
                ret = Script_Run(arguments);
                // commandRun = true;
            }
            catch (Exception ex)
            {
                if (OutputLevel >= 0)
                {
                    Console.WriteLine("ERROR occurred: " + Environment.NewLine + ex.Message);
                }
                Script_PrintCommandsHelp();
                throw;
            }
            finally
            {
            }
            if (OutputLevel >= 1)
            {
                Console.WriteLine("*********** EXECUTION FINISHED. ***********");
                Console.WriteLine();
            }
            return ret;
        }





        /// <summary>Creates and returns an interpreter that can be used as script's internal interpreter for
        /// running script's commands.
        /// <para>Commands installed on this interpreter recieve the same command arguments as the <see cref="Run"/> command.</para></summary>
        public virtual ICommandLineApplicationInterpreter Script_CreateInterpreterWithoutCommands()
        {
            CommandLineApplicationScriptInterpreter ret = new CommandLineApplicationScriptInterpreter();
            ret.Script_CommandHelpStrings = this.Script_CommandHelpStrings;
            ret.WarnCommandReplacement = false;
            return ret;
        }

        /// <summary>Creates and returns an interpreter that can be used as script's internal interpreter for
        /// running script's commands.
        /// <para>Script's internal commands are added by the <see cref="Script_AddCommands"/> method before 
        /// the created interpreter is returned.</para>
        /// <para>In order to create another interpreter, override the <see cref="Script_CreateInterpreterWithoutCommands"/> method. 
        /// In order to add another set of script internal commands, override the <see cref="Script_AddCommands"/> method.</para>
        /// <para>Commands installed on this interpreter recieve the same command arguments as the <see cref="Run"/> command.</para></summary>
        protected ICommandLineApplicationInterpreter Script_CreateInterpreter()
        {
            ICommandLineApplicationInterpreter returnedInterpreter = Script_CreateInterpreterWithoutCommands();
            Script_AddCommands(returnedInterpreter, Script_CommandHelpStrings);
            return returnedInterpreter;
        }

        /// <summary>Add wscript's internal commands to the specified interpreter.</summary>
        /// <param name="interpreter">Interpreter to which commands are added.</param>
        /// <param name="helpStrings">List where help strings for corresponding commands are stored (optional).</param>
        public virtual void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            if (interpreter == null)
                throw new ArgumentException("Interpreter to add script commands to is not specified (null reference).");
            Script_AddCommand(interpreter, helpStrings, ConstDefaultHelp, Script_CommandHelp, "Prints help." /* help string */);
            Script_AddCommand(interpreter, helpStrings, ConstHelpDefaultUniversal, Script_CommandHelp, "Prints help." /* help string */);
            Script_AddCommand(interpreter, helpStrings, ConstDefaultTestScrip, Script_CommandTestScript, "Test method to check that the script functions." /* help string */);
        }


        /// <summary>Default command name for help.</summary>
        public const string ConstDefaultHelp = "Help";

        /// <summary>Universal name of the help command.</summary>
        public const string ConstHelpDefaultUniversal = "?";

        /// <summary>Prints help.</summary>
        /// <param name="arguments">Arguments.</param>
        /// <returns>null.</returns>
        protected virtual string Script_CommandHelp(string[] arguments)
        {
            if (arguments!=null)
                if (arguments.Length >= 2)
                {
                    Console.WriteLine();
                    Console.WriteLine("Command " + arguments[1] + ": ");
                    Console.WriteLine("  " + Script_GetHelpString(arguments[1]));
                    Console.WriteLine();
                    Console.WriteLine("General information: ");
                }
            Script_PrintCommandsHelp();
            return null;
        }

        /// <summary>Default command name for test method.</summary>
        public const string ConstDefaultTestScrip = "TestScript";

        /// <summary>Prints help.</summary>
        /// <param name="arguments">Arguments.</param>
        /// <returns>null.</returns>
        protected virtual string Script_CommandTestScript(string[] arguments)
        {
            Console.WriteLine();
            Console.WriteLine("Testing script...");
            Console.WriteLine("This script is alive.");
            Console.WriteLine();
            Script_PrintCommandsHelp();
            Console.WriteLine();
            Script_PrintArguments("Run arguments: ", arguments);
            Console.WriteLine();
            return null;
        }

        protected ICommandLineApplicationInterpreter _script_interpreter;

        /// <summary>Script's internal interpreter that takes care for execution of installed internal commands.
        /// <para>This interpreter is created when first needed (lazy evaluation).</para></summary>
        /// <remarks>Although this property is market virtual, you will normally not need to override it.
        /// Instead, override the <see cref="Script_CreateInterpreter"/> method, whch should do all the initializations
        /// of the interpreter.</remarks>
        public virtual ICommandLineApplicationInterpreter Script_Interpreter
        {
            get
            {
                if (_script_interpreter == null)
                    _script_interpreter = Script_CreateInterpreter();
                return _script_interpreter;
            }
            protected set
            {
                _script_interpreter = value;
            }
        }

        /// <summary>Delegate for commands that are installed on script's internal interpreter (property <see cref="Script_Interpreter"/>).
        /// <para>These methods only take command arguments as parameters, and the first argument is
        /// usually name under which command is installed.</para></summary>
        /// <param name="args">Command arguments. The first argument is internal script's command name.</param>
        /// <returns>Command's return value.</returns>
        public delegate string Script_CommandDelegate(string[] args);

        /// <summary>Adapts that converts internal script commands (delegate of type <see cref="Script_CommandDelegate"/>) to interpreter commands.
        /// <para>This adapter enables definition of script internal commands in a simple form and installation of them
        /// on internal interpreter, which requires command delegate of type ....</para></summary>
        public class Script_CommandAdapter
        {
            private Script_CommandAdapter() { }  // prevent argument-less execution.

            public Script_CommandAdapter(LoadableScriptBase script, Script_CommandDelegate scriptCommand)
            {
                this.ScriptCommand = scriptCommand;
                this.Script = script;
            }


            protected LoadableScriptBase _script;

            /// <summary>Script object where the command is installed.</summary>
            public LoadableScriptBase Script
            {
                get { return _script; }
                protected set { _script = value; }
            }

            protected Script_CommandDelegate _scriptCommand;

            /// <summary>Script's internal command that is executed when interpreter command is called.</summary>
            public Script_CommandDelegate ScriptCommand
            {
                get { return _scriptCommand; }
                protected set {
                    if (value == null)
                        throw new ArgumentException("Script to interpreter command adaptor: script commad method is not specified.");
                    _scriptCommand = value; 
                }
            }

            /// <summary>Method that is used to execute interpreter command.
            /// <para>This method actually runs the script command enclosed in this adapter.</para></summary>
            /// <param name="interpreter">Dummy argument, only to match delegate signature.</param>
            /// <param name="commandName">Name of the command. This will be the same as the 0-th argument.</param>
            /// <param name="args">Command arguments. 0-th arguments will usually be command name.</param>
            public string InterpreterCommand(CommandThread interpreter, string commandName, string[] args)
            {
                string ret = _scriptCommand(args);
                if (args!=null)
                    if (args.Length==2)
                        if (args[1] == ConstHelpDefaultUniversal)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Script command " + commandName + ": ");
                            Console.WriteLine("  " + _script.Script_GetHelpString(commandName));
                            Console.WriteLine();
                        }
                return ret;
            }

        }  // class ScriptCommandAdapter
        
        private SortedList<string, string> _script_CommandHelpStrings;

        /// <summary>Contains help strings associated with script commands installed on interpreter.</summary>
        protected SortedList<string, string> Script_CommandHelpStrings
        {
            get
            {
                if (_script_CommandHelpStrings == null)
                    _script_CommandHelpStrings = new SortedList<string, string>();
                return _script_CommandHelpStrings;
            }
        }



        /// <summary>Adds a new internal script command under specified name to the internal interpreter of the current 
        /// script object.</summary>
        /// <param name="commandName">Name of the command. <para>Must not be null or empty string.</para></param>
        /// <param name="command">Method that executes the command. <para>Must not be null.</para></param>
        /// <param name="helpString">Help string associated with command, optionsl (can be null).</param>
        public void Script_AddCommand(string commandName,
            Script_CommandDelegate command, string helpString)
        {
            if (_script_interpreter == null)
                throw new InvalidOperationException("Called highlevel Script_AddCommand(...) while Script_Interpreter not yet initialized." + Environment.NewLine
                    + "Do not call the high level Script_AddCommand within any initialization method such as Script_AddCommands(...)." + Environment.NewLine
                    + "Call the low level Script_AddCommand(ICommandLineApplicationInterpreter interpreter, ...) instead." 
                    + Environment.NewLine);
            Script_AddCommand(Script_Interpreter, Script_CommandHelpStrings, commandName, command, helpString);
        }

        /// <summary>Adds a new internal script command under specified name to the internal interpreter of the current 
        /// script object.</summary>
        /// <param name="interpreter">Interpreter on which the command is added.</param>
        /// <param name="helpStrings">Dictionary containing help strings corresponding to interpreter commands.</param>
        /// <param name="commandName">Name of the command. <para>Must not be null or empty string.</para></param>
        /// <param name="command">Method that executes the command. <para>Must not be null.</para></param>
        /// <param name="helpString">Help string associated with command, optionsl (can be null).</param>
        public virtual void Script_AddCommand(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings,
            string commandName,  Script_CommandDelegate command, string helpString)
        {
            if (string.IsNullOrEmpty(commandName))
                throw new ArgumentException("Script command name is not specified.");
            if (command == null)
                throw new ArgumentException("Can not add command named " + commandName + ", execution method is not specified.");
            Script_CommandAdapter adapter = new Script_CommandAdapter(this, command);
            if (interpreter.ContainsCommand(commandName))
            {
                interpreter.RemoveCommand(commandName);
                if (OutputLevel >= 3)
                    Console.WriteLine(Environment.NewLine + "WARNING: Internal script command \"" + commandName +
                        "\" has been replaced. " + Environment.NewLine);
            }
            interpreter.AddCommand(commandName, adapter.InterpreterCommand);
            if (helpStrings != null)
            {
                if (helpStrings.ContainsKey(commandName))
                    helpStrings.Remove(commandName);
                helpStrings.Add(commandName, helpString);
            }
        }

        /// <summary>Returns help string for internal script command with specified name, or null if help string
        /// is not installed for such a command.</summary>
        /// <param name="scriptCommandName">Name of the csript command.</param>
        public string Script_GetHelpString(string scriptCommandName)
        {
            string ret = null;
            if (Script_CommandHelpStrings.ContainsKey(scriptCommandName))
                ret = Script_CommandHelpStrings[scriptCommandName];
            return ret;
        }

        /// <summary>Prits help for the installed internal commands of the script.</summary>
        public void Script_PrintCommandsHelp()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("This is a runnable script.");
            Console.WriteLine("Installed runnable commands: ");
            foreach (string key in Script_CommandHelpStrings.Keys)
            {
                Console.WriteLine(" " + key + ":");
                Console.WriteLine("   " + Script_CommandHelpStrings[key]);
            }
            Console.WriteLine();
        }

        /// <summary>Returns true if the internal script's interpreter contains a command with specified name, false otherwise.</summary>
        /// <param name="commandName">Name of the command whose existence is queried.</param>
        public virtual bool Script_ContainsCommand(string commandName)
        {
            return Script_Interpreter.ContainsCommand(commandName);
        }

        /// <summary>Returns true if the specified command is script command (i.e. its first argument is command-name and it is run
        /// through the <see cref="Script_CommandAdapter"/> object).</summary>
        /// <param name="commandName">Name of the command that is queried.</param>
        public virtual bool Script_ContainsScriptCommand(string commandName)
        {
            if (_script_CommandHelpStrings != null)
            {
                if (_script_CommandHelpStrings.ContainsKey(commandName))
                    return true;
            }
            return false;
        }



        /// <summary>Removes the specified internal script command from the internal interpreter of the current 
        /// scripting object.</summary>
        /// <param name="commandName">Name of the command. <para>Must not be null or empty string.</para></param>
        public virtual void Script_RemoveCommand(string commandName)
        {
            try
            {
                Script_Interpreter.RemoveCommand(commandName);
                Script_CommandHelpStrings.Remove(commandName);
            }
            catch (Exception ex)
            {
                if (OutputLevel >= 2)
                {
                    Console.WriteLine(Environment.NewLine
                        + "ERROR occurred when removing command " + commandName + ": " + Environment.NewLine
                        + ex.Message + Environment.NewLine);
                }
            }
        }


        /// <summary>Removes ALL internal script commands from the internal interpreter of the current 
        /// scripting object.</summary>
        public virtual void Script_RemoveAllCommands()
        {
            try
            {
                ICommandLineApplicationInterpreter interpreter = Script_Interpreter;
                interpreter.RemoveAllCommands();
            }
            catch (Exception ex)
            {
                if (OutputLevel >= 2)
                {
                    Console.WriteLine(Environment.NewLine
                        + "ERROR occurred when removing all commands: " + Environment.NewLine
                        + ex.Message + Environment.NewLine);
                }
            }
        }


        /// <summary>Runs internal script command.</summary>
        /// <param name="arguments">Arguments of the command. The first argument must be command name.</param>
        /// <returns>The return valu of executed command.</returns>
        /// <exception cref="ArgumentException">When argumens or command are not specified or command is not known.</exception>
        public string Script_Run(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentException("Arguments not specified (null array).");
            else if (arguments.Length < 1)
                throw new ArgumentException("Number of arguments less than 1. At least command name should be specified.");
            string commandName = arguments[0];
            if (string.IsNullOrEmpty(commandName))
                throw new ArgumentException("Command name (the 0-th argument) is not specified (null or empty string).");
            if (Script_ContainsCommand(commandName))
            {
                if (Script_ContainsScriptCommand(commandName))
                {
                    // We are running a script command, therefore it is run without modifications:
                    return Script_Interpreter.RunWithoutModifications(Script_Interpreter.MainThread, commandName, arguments);
                }
                else
                {
                    string[] args = null;
                    // We are running a normal interpreter command through Script_Run, we therefore have to take away the first
                    // argument that is the command name:
                    if (arguments != null)
                    {
                        int numArgs = arguments.Length - 1;
                        if (numArgs < 0)
                            numArgs = 0;
                        args = new string[numArgs];
                        for (int i = 0; i < numArgs; ++i)
                            args[i] = arguments[i + 1];
                    }
                    return Script_Interpreter.RunWithoutModifications(Script_Interpreter.MainThread, commandName, args);
                }

            }  else
                throw new InvalidOperationException("Script: unknown internal or interpreter command " + commandName + ".");
        }


        /// <summary>Runs internal script command.</summary>
        /// <param name="commandName">Name of the command</param>
        /// <param name="otherArguments">The remainind command arguments (without a name). 
        /// <para>Name is prepended to arguments before the script is run.</para></param>
        /// <exception cref="ArgumentException">When argumens or command are not specified or command is not known.</exception>
        public string Script_Run(string commandName, params string[] otherArguments)
        {
            int numArguments = 1;
            if (otherArguments != null)
                numArguments += otherArguments.Length;
            string[] arguments = new string[numArguments];
            arguments[0] = commandName;
            for (int i = 0; i < otherArguments.Length; ++i)
            {
                arguments[i + 1] = otherArguments[i];
            }
            return Script_Run(arguments);
        }


        #endregion ScriptInterpreter


        #region Auxiliary

        /// <summary>Prints the specified array of string arguments (usually passed as command-line 
        /// arguments).</summary>
        /// <param name="arguments">Arguments to be printed.</param>
        /// <param name="messageString">Message to be printed first (optional, can be null).</param>
        public virtual void Script_PrintArguments(string messageString, string[] arguments)
        {
            if (messageString != null)
                Console.WriteLine(messageString);
            else
                Console.WriteLine("Arguments: ");
            if (arguments == null)
                Console.WriteLine("  null");
            else if (arguments.Length == 0)
                Console.WriteLine("  No arguments (array has zero length).");
            else for (int i = 0; i < arguments.Length; ++i)
                    Console.WriteLine("  No. " + i + ": " + arguments[i]);
            //Console.WriteLine("Script class: " + this.GetType().FullName);
        }

        #endregion Auxiliary


        #region StoredScriptSettings

        /// <summary>In methods of this class you will find all the settings that apply to this script.</summary>
        /// <remarks>Before custom application script is archived, settings should be moved </remarks>
        /// $A Igor Feb12;
        protected class StoredScriptSettings : ApplicationCommandlineBase
        {

            protected override CommandLineApplicationInterpreter CreateInterpreter() { throw new NotImplementedException("Creation of commandline interpreter is not implemnted in this class."); }

            public override void TestMain(string[] args) { throw new NotImplementedException(""); }

            public void TestMain_Basic(string[] args)
            {
                // Store script settings in this method!
            }
        }

        #endregion StoredScriptSettings


    }  // class LoadableScriptBase


}
