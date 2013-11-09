// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// TESTING SCRIPT FILE: output formats for numbers.
// Original filename: ScriptExtFormats

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using IG.Crypto;

using IG.Num;
using IG.Lib;

using CC = IG.Crypto.UtilCrypto.ConstCrypto;

namespace IG.Script
{



    /// <summary>Base cls. for application scripts that can also be used on its own.</summary>
    /// <remarks>The cls.es derived from this cls. are usually used as internal scripts and provide a set of 
    /// embedded utiliy applications within an application that is based on an command-line interpreter.
    /// <para>Applications will typically extend this cls. by their own specific cls. that is used in its place
    /// for providing various embedded utility applications. The base library itself provides the <see cref="AppBase"/>
    /// cls. derived from this cls., which is used because its shorter name. See that cls. for an example how to
    /// prepare a derived cls. that will provide as set of embedded applications.</para>
    /// <para>In applications that have the command-line interpreter, embedded applications from this cls. can typically be
    /// run in the following way:</para>
    /// <para>  AppName Internal IG.Script.AppBase CommandName arg1 arg2 ...</para>
    /// <para>where AppName is the application name, IG.Script.AppBase is the full name of the script cls. that contains
    /// embedded applications, CommandName is name of the command thar launches embedded application, and arg1, arg2, etc.
    /// are command arguments for the embedded application.</para></remarks>
    /// <seealso cref="ScriptAppBase"/>
    /// $A Igor xx;
    public class AppBase : ScriptAppBase, ILoadableScript
    {

        public AppBase()
            : base()
        { }



        /// <summary>Name of the command for a group of custom applications.</summary>
        public const string ConstCustom = "Custom";
        public const string ConstHelpCustom = "Custom application group.";


        /// <summary>Adds commands to the internal interpreter.</summary>
        public override void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            base.Script_AddCommands(interpreter, helpStrings);
            Script_AddCommand(interpreter, helpStrings, ConstCustom, AppCustom, ConstHelpCustom);

        }



        #region Actions.FileUtilities_Inherited

        // This region contains an example of extending a group of embedded application commands.
        // Base command of the group has already been added to the script.

        /// <summary>Initializes commands for file system related utilities (embedded applications).</summary>
        protected override void InitAppFile()
        {
            lock (Lock)
            {
                if (_appFileCommandsInitialized)
                    return;
               base.InitAppFile();
               AddFileCommand(FileTestArguments, FileFunctionTestArguments, FileHelpTestArguments);
             }
        }

        #region Actions.FileUtilities.TestArguments

        public const string FileTestArguments = "PrintArguments";

        private const string FileHelpTestArguments = FileTestArguments + " arg1 arg2 ... : Prints out the arguments of the command.";

        /// <summary>Executes embedded application that just prints arguments passed to the application to a console.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FileFunctionTestArguments(string appName, string[] args)
        {
            int numargs = 0;
            if (args != null)
                numargs = args.Length;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("This is a test embedded application of the script " + this.GetType().FullName + ".");
            sb.AppendLine("  Application name: " + appName);
            sb.AppendLine("  Script command arguments:");
            if (numargs == 0)
                sb.AppendLine("    No arguments passed.");
            for (int i = 0; i < numargs; ++i)
            {
                sb.AppendLine("    \"" + args[i] + "\"");
            }
            sb.AppendLine();
            Console.Write(sb.ToString());
            return numargs.ToString();
        }

        #endregion Actions.FileUtilities.TestArguments

        #endregion Actions.FileUtilities_Inherited


        #region Actions.Custom

        // This region contains an example of adding a new group of embedded application commands.
        // The corresponding command for launching embedded applications is installed in the 
        // overridden Script_AddCommands() method.

        /// <summary>List of installed file command names.</summary>
        protected List<string> AppCustomNames = new List<string>();

        /// <summary>List of help strings corresponding to installed file commands.</summary>
        protected List<string> AppCustomHelpStrings = new List<string>();

        /// <summary>List of methods used to perform file commmands.</summary>
        protected List<CommandMethod> AppCustomMethods = new List<CommandMethod>();

        /// <summary>Adds a new file system - related embedded application's command (added as sub-command of the base command named <see cref="ConstFile"/>).</summary>
        /// <param name="appName">Application name.</param>
        /// <param name="appMethod">Method used to perform the application.</param>
        /// <param name="appHelp">Eventual help string for the application.</param>
        protected void AddCustomCommand(string appName, CommandMethod appMethod, string appHelp)
        {
            lock (Lock)
            {
                AppCustomNames.Add(appName.ToLower());
                AppCustomHelpStrings.Add(appHelp);
                AppCustomMethods.Add(appMethod);
            }
        }

        #region Actions.Custom.PrintArguments

        public const string CustomPrintArguments = "PrintArguments";

        protected const string CustomHelpPrintArguments = CustomPrintArguments + " arg1 arg2 ... : Prints out the arguments of the command.";

        /// <summary>Executes embedded application - writing to the console information about file events for the specified file or directory.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string CustomFunctionPrintArguments(string appName, string[] args)
        {
            return FileFunctionTestArguments(appName, args);
        }

        #endregion Actions.Custom.PrintArguments



        protected bool _appCustomCommandsInitialized = false;

        /// <summary>Initializes commands for file system related utilities (embedded applications).</summary>
        protected virtual void InitAppCustom()
        {

            lock (Lock)
            {
                if (_appCustomCommandsInitialized)
                    return;
                AddCustomCommand(CustomPrintArguments, CustomFunctionPrintArguments, CustomHelpPrintArguments);

                _appCustomCommandsInitialized = true;
            }
        }


        /// <summary>Runs a file system related utility (embedded application) according to arguments.</summary>
        /// <param name="args">Arguments. 0-th argument is the base command name, 1st argument is the embedded application name, and teh rest
        /// arguments are arguments that are used by the embedded application.</param>
        protected virtual string RunAppCustom(string[] args)
        {
            InitAppCustom();
            if (args == null)
                throw new ArgumentException("No arguments. Embedded application name (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Test name (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Test name (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " ApplicationName arg1 arg2...");
                        Console.WriteLine(args[0] + " ApplicationName ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of embedded applications: ");
                        for (int i = 0; i < AppCustomNames.Count; ++i)
                            Console.WriteLine("  " + AppCustomNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] surfaceParams = new string[args.Length - 2];
            for (int i = 0; i < surfaceParams.Length; ++i)
                surfaceParams[i] = args[i + 2];
            int index = AppCustomNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Embedded application named " + testName + " is not found. Call with '?' for list of application names.");
            testName = AppCustomNames[index];
            string helpString = AppCustomHelpStrings[index];
            CommandMethod method = AppCustomMethods[index];
            if (surfaceParams.Length >= 1)
                if (surfaceParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(AppCustomHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(AppCustomHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(testName, surfaceParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
        }

        /// <summary>Runs one of the custom embedded applications.</summary>
        /// <param name="arguments">Array containing the base command name, application command name and arguments.</param>
        public virtual string AppCustom(string[] arguments)
        {
            string ret = null;
            if (arguments == null)
                throw new ArgumentException("Commandline arguments not specified (null argument).");
            if (arguments.Length < 2)
                Console.WriteLine("Number of arguments should be at least 2 (at least base command name & application name).");
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Running custom embedded application " 
                + arguments[1] + "..."
                + Environment.NewLine +
                "=============================="
                + Environment.NewLine);
            Console.WriteLine();
            Script_PrintArguments("Application arguments: ", arguments);
            Console.WriteLine();

            if (ret == null)
                ret = RunAppCustom(arguments);

            Console.WriteLine("==============================");
            Console.WriteLine("Custom application  finished.");
            Console.WriteLine();
            return ret;
        }  // AppCustom


        #endregion Actions.Custom

    } // cls. AppBase


    /// <summary>Base cls. for application scripts.</summary>
    /// <remarks>The cls.es derived from this cls. are usually used as internal scripts and provide a set of 
    /// embedded utiliy applications within an application that is based on an command-line interpreter.
    /// <para>Applications will typically extend this cls. by their own specific cls. that is used in its place
    /// for providing various embedded utility applications. The base library itself provides the <see cref="AppBase"/>
    /// cls. derived from this cls., which is used because its shorter name. See that cls. for an example how to
    /// prepare a derived cls. that will provide as set of embedded applications.</para>
    /// <para>In applications that have the command-line interpreter, embedded applications from this cls. can typically be
    /// run in the following way:</para>
    /// <para>  AppName Internal IG.Script.AppBase CommandName arg1 arg2 ...</para>
    /// <para>where AppName is the application name, IG.Script.AppBase is the full name of the script cls. that contains
    /// embedded applications, CommandName is name of the command thar launches embedded application, and arg1, arg2, etc.
    /// are command arguments for the embedded application. In some applications, cls. name may be different and may
    /// be a name of a derived cls. that provides extended functionality or just provides a more suitable name of the script.</para></remarks>
    /// $A Igor xx Sep12;
    public partial class ScriptAppBase : LoadableScriptSpecialFunctionBase, ILoadableScript
    {

        #region Standard_TestScripts

        public ScriptAppBase()
            : base()
        { }

        /// <summary>Initializes the current object.</summary>
        protected override void InitializeThis(string[] arguments)
        {
            Script_DefaultInitialize(arguments);
        }

        /// <summary>Runs action of the current object.</summary>
        /// <param name="arguments">Command-line arguments of the action.</param>
        protected override string RunThis(string[] arguments)
        {
            return Script_DefaultRun(arguments);
        }

        #endregion Standard_TestScripts


        #region Commands


        /// <summary>Name of the command that performs my custom test.</summary>
        public const string ConstMyTest = "MyTest";
        public const string ConstHelpMyTest = "Custom test function.";

        /// <summary>Name of the command for custom test.</summary>
        public const string ConstCustomApp = "CustomApp";
        public const string ConstHelpCustomApp = "Custom aplication.";

        /// <summary>Name of the command for the system-related embedded applications.</summary>
        public const string ConstSystem = "System";
        public const string ConstHelpSystem = "Various system-related embedded applications. Run with ? argument to see which applications are available.";

        /// <summary>Name of the command for the numerics-related embedded applications.</summary>
        public const string ConstNumerics = "Numerics";
        public const string ConstHelpNumerics = "Various numerics-related embedded applications. Run with ? argument to see which applications are available.";

        /// <summary>Name of the command for the file system-related embedded applications.</summary>
        public const string ConstFile = "File";
        public const string ConstHelpFile = "Various file system-related embedded applications. Run with ? argument to see which applications are available.";

        /// <summary>Name of the command for the cryptography-related embedded applications.</summary>
        public const string ConstCrypto = "Crypto";
        public const string ConstHelpCrypto = "Various cryptography-related embedded applications. Run with ? argument to see which applications are available.";

        /// <summary>Name of the command for the process-related embedded applications.</summary>
        public const string ConstProcess = "Process";
        public const string ConstHelpProcess = "Various process-related embedded applications. Run with ? argument to see which applications are available.";


        /// <summary>Name of the command for the data structures-related embedded applications.</summary>
        public const string ConstDataStructures = "DataStructures";
        public const string ConstHelpDataStructures =
@"Various data structures-related embedded demo applications. 
  Run with the '?' argument to see which applications are available.";
                    

        #endregion Commands
        

        /// <summary>Adds commands to the internal interpreter.</summary>
        /// <param name="interpreter">Interpreter where commands are executed.</param>
        /// <param name="helpStrings">List containg help strings.</param>
        public override void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            base.Script_AddCommands(interpreter, helpStrings);
            if (IsAddRunFileByScriptCommand)
            {
                Script_AddRunFileByScriptCommand(interpreter, helpStrings);
            }
            this.Script_AddCommands1(interpreter, helpStrings);
            

            Script_AddCommand(interpreter, helpStrings, ConstMyTest, AppMyTest, ConstHelpMyTest);
            Script_AddCommand(interpreter, helpStrings, ConstCustomApp, AppCustomApp, ConstHelpCustomApp);
            Script_AddCommand(interpreter, helpStrings, ConstSystem, AppSystem, ConstHelpSystem);
            Script_AddCommand(interpreter, helpStrings, ConstNumerics, AppNumerics, ConstHelpNumerics);
            Script_AddCommand(interpreter, helpStrings, ConstFile, AppFile, ConstHelpFile);
            Script_AddCommand(interpreter, helpStrings, ConstCrypto, AppCrypto, ConstHelpCrypto);
            Script_AddCommand(interpreter, helpStrings, ConstProcess, AppProcess, ConstHelpProcess);
            Script_AddCommand(interpreter, helpStrings, ConstDataStructures, AppDataStructures, ConstHelpDataStructures);
        }

        
        #region ScriptInterpreter
        
        /// <summary>Name of the command that runs (interprets) the specified command file by the script's interpreter.</summary>
        public const string ConstRunFile = "RunFileByScript";
        public const string ConstHelpRunFile = 
ConstRunFile + @" commandFile : Runs (interprets) the specified file by the script's interpreter.
  commandFile: path to the command file that is interpreted (usual extension: '.cmd').";

        /// <summary>Adds the Run comand to the script's interpreter and performs the necessary
        /// additional tasks.
        /// <para>The run command runs line by line the specified command file by the script's interpreter.</para></summary>
        [Obsolete("The normal interpreter's Run command can now be used instead of this.")]
        protected void Script_AddRunFileByScriptCommand(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            Script_AddCommand(interpreter, helpStrings, ConstRunFile, AppRunFileByScript, ConstHelpRunFile);
        }

        /// <summary>Removes the Run comand from the script's interpreter and performs the necessary
        /// accompanying tasks.</summary>
        [Obsolete("The normal interpreter's Run command can now be used instead of this.")]
        protected void Script_RemoveRunFileByScriptCommand(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            interpreter.RemoveCommand(ConstRunFile);

        }

        protected bool _isAddRunFileByScriptCommand = true;

        /// <summary>Whether or not the Run command is installed on the script's interpreter.</summary>
        [Obsolete("The normal interpreter's Run command can now be used instead of this.")]
        public virtual bool IsAddRunFileByScriptCommand
        {
            get { lock (Lock) { return _isAddRunFileByScriptCommand; } }
            set {
                lock (Lock)
                {
                    if (value != _isAddRunFileByScriptCommand)
                    {
                        _isAddRunFileByScriptCommand = value;
                        if (value == true)
                        {
                            try
                            {
                                Script_AddRunFileByScriptCommand(Script_Interpreter, Script_CommandHelpStrings);
                            }
                            catch { }
                        } else
                        {
                            try
                            {
                                Script_RemoveRunFileByScriptCommand(Script_Interpreter, Script_CommandHelpStrings);
                            }
                            catch { }
                        }
                    }
                }
            }
        }
        
        /// <summary>Execution method that Runs the specified command file by the script's interpreter.</summary>
        /// <param name="args">Command arguments. Command file path must be the only argument.</param>
        [Obsolete("The normal interpreter's Run command can now be used instead of this.")]
        protected virtual string AppRunFileByScript(string[] args)
        {
            string ret = null;
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs < 2)
                throw new ArgumentException("Command file to be run is not specified.");
            string commandFilePath = args[1];
            Console.WriteLine(Environment.NewLine 
                + "Running command file " + commandFilePath + " ..." + Environment.NewLine);
            ret = this.RunFileByScript(commandFilePath); // Script_Interpreter.RunFile(commandFilePath);
            Console.WriteLine(Environment.NewLine + "... running command file done." + Environment.NewLine);
            return ret;
        }


        /// <summary>Runs all commands that are written in a file.
        /// Each line of a file is interpreted as a single command, consisting of command name followed by arguments.</summary>
        /// <param name="filePath">Path to the file containing commands.</param>
        /// <returns>Return value of the last command.</returns>
        [Obsolete("The normal interpreter's Run command can now be used instead of this.")]
        public virtual string RunFileByScript(string filePath)
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
                    if (Script_Interpreter.Exit)
                        return ret;
                    ++lineNum;
                    try
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] commandLineSplit = Script_Interpreter.GetArguments(line);
                            ret = Script_Run(commandLineSplit);
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
        #endregion ScriptInterpreter

        #region Actions

        

        /// <summary>Test action.</summary>
        /// <param name="arguments">Array of command-line arguments.</param>
        public string AppMyTest(string[] arguments)
        {
            Console.WriteLine();
            Console.WriteLine("MY CUSTOM TEST.");
            Console.WriteLine("This script is alive.");
            Script_PrintArguments("Script arguments: ", arguments);
            Console.WriteLine("==== END of my custom test.");
            Console.WriteLine();
            return null;
        }

        /// <summary>Custom application.</summary>
        public virtual string AppCustomApp(string[] arguments)
        {
            string ret = null;
            Console.WriteLine();
            Console.WriteLine("CUSTOM APPLICATION run from the APPLICATION SCRIPT.");
            Console.WriteLine("==============================");
            Script_PrintArguments("Script arguments: ", arguments);
            Console.WriteLine();

            Console.WriteLine("==============================");
            Console.WriteLine("Custom application finished.");
            Console.WriteLine();
            return ret;
        }


        #region Actions.NumericUtilities


        /// <summary>List of installed numerics command names.</summary>
        protected List<string> AppNumericsNames = new List<string>();

        /// <summary>List of help strings corresponding to installed numerics-related commands.</summary>
        protected List<string> AppNumericsHelpStrings = new List<string>();

        /// <summary>List of methods used to perform numerics-related commmands.</summary>
        protected List<CommandMethod> AppNumericsMethods = new List<CommandMethod>();

        /// <summary>Adds a new numerics - related embedded application's command (added as sub-command of the base command named <see cref="ConstNumerics"/>).</summary>
        /// <param name="appName">Application name.</param>
        /// <param name="appMethod">Method used to perform the application.</param>
        /// <param name="appHelp">Eventual help string for the application.</param>
        protected void AddNumericsCommand(string appName, CommandMethod appMethod, string appHelp)
        {
            lock (Lock)
            {
                AppNumericsNames.Add(appName.ToLower());
                AppNumericsHelpStrings.Add(appHelp);
                AppNumericsMethods.Add(appMethod);
            }
        }


        #region Actions.NumericUtilities.ScriptScalarFunction

        public const string NumericsScriptScalarFunction = "ScriptScalarFunction";

        protected const string NumericsHelpScriptScalarFunction = NumericsScriptScalarFunction + " : Definition of scalar functions by expressions.";

        /// <summary>Executes embedded application - testing of definition of scalar function objects through expressions.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string NumericsFunctionScriptScalarFuncitons(string surfaceName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;

            Console.WriteLine(Environment.NewLine + "Test of scalar function objects defined through expressions..." 
                + Environment.NewLine);

            ScalarFunctionLoader.Example();

            Console.WriteLine(Environment.NewLine + "Test of scalar function objects finished."
                + Environment.NewLine);

            return null;
        }

        #endregion Actions.NumericUtilities.WaitFileCreation



        protected bool _appNumericsCommandsInitialized = false;

        /// <summary>Initializes commands for numerics related utilities (embedded applications).</summary>
        protected virtual void InitAppNumerics()
        {

            lock (Lock)
            {
                if (_appNumericsCommandsInitialized)
                    return;
                AddNumericsCommand(NumericsScriptScalarFunction, NumericsFunctionScriptScalarFuncitons, NumericsHelpScriptScalarFunction);
                
                _appFileCommandsInitialized = true;
            }
        }


        /// <summary>Runs a numerics related utility (embedded application) according to arguments.</summary>
        /// <param name="args">Arguments. 0-th argument is the base command name, 1st argument is the embedded application name, and teh rest
        /// arguments are arguments that are used by the embedded application.</param>
        protected virtual string RunAppNumerics(string[] args)
        {
            InitAppNumerics();
            if (args == null)
                throw new ArgumentException("No arguments. Embedded application name (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Test name (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Test name (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " ApplicationName arg1 arg2...");
                        Console.WriteLine(args[0] + " ApplicationName ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of embedded applications: ");
                        for (int i = 0; i < AppNumericsNames.Count; ++i)
                            Console.WriteLine("  " + AppNumericsNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] surfaceParams = new string[args.Length - 2];
            for (int i = 0; i < surfaceParams.Length; ++i)
                surfaceParams[i] = args[i + 2];
            int index = AppNumericsNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Embedded application named " + testName + " is not found. Call with '?' for list of application names.");
            testName = AppNumericsNames[index];
            string helpString = AppNumericsHelpStrings[index];
            CommandMethod method = AppNumericsMethods[index];
            if (surfaceParams.Length >= 1)
                if (surfaceParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(AppNumericsHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(AppNumericsHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(testName, surfaceParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
        }

        /// <summary>Runs one of the numerics - related embedded applications.</summary>
        /// <param name="arguments">Array containing the base command name, application command name and arguments.</param>
        public virtual string AppNumerics(string[] arguments)
        {
            string ret = null;
            if (arguments == null)
                throw new ArgumentException("Commandline arguments not specified (null argument).");
            if (arguments.Length < 2)
                Console.WriteLine("Number of arguments should be at least 2 (at least base command name & application name).");
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Running numerics - related embedded application..."
                + Environment.NewLine +
                "=============================="
                + Environment.NewLine);
            //Console.WriteLine();
            //Script_PrintArguments("Application arguments: ", arguments);
            //Console.WriteLine();

            if (ret == null)
                ret = RunAppNumerics(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("Numerics - related application  finished.");
            Console.WriteLine();
            return ret;
        }  // AppNumerics

        // File Fle File File

        #endregion Actions.NumericUtilities




        #region Actions.FileUtilities

        /// <summary>List of installed file command names.</summary>
        protected List<string> AppFileNames = new List<string>();

        /// <summary>List of help strings corresponding to installed file commands.</summary>
        protected List<string> AppFileHelpStrings = new List<string>();

        /// <summary>List of methods used to perform file commmands.</summary>
        protected List<CommandMethod> AppFileMethods = new List<CommandMethod>();

        /// <summary>Adds a new file system - related embedded application's command (added as sub-command of the base command named <see cref="ConstFile"/>).</summary>
        /// <param name="appName">Application name.</param>
        /// <param name="appMethod">Method used to perform the application.</param>
        /// <param name="appHelp">Eventual help string for the application.</param>
        protected void AddFileCommand(string appName, CommandMethod appMethod, string appHelp)
        {
            lock (Lock)
            {
                AppFileNames.Add(appName.ToLower());
                AppFileHelpStrings.Add(appHelp);
                AppFileMethods.Add(appMethod);
            }
        }


        #region Actions.FileUtilities.LogFileEvents

        public const string FileLogEvents = "LogEvents";

        protected const string FileHelpLogEvents = FileLogEvents + " FileOrDirectory MaxEvents : Logs file events for the specified file or directory.";

        /// <summary>Executes embedded application - writing to the console information about file events for the specified file or directory.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FileFunctionLogEvents(string surfaceName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs < 1)
                throw new ArgumentException("File or Directory path on which file events are printed is not specified.");
            string testFileOrDirectory = null;
            int numEvents = (int) 1.0e9;   // unlimited
            if (numArgs >= 1)
            {
                testFileOrDirectory = args[0];
                if (string.IsNullOrEmpty(testFileOrDirectory))
                    throw new ArgumentException("File or Directory path on which file events are printed is not specified.");
            }
            if (numArgs >= 2)
            {
                if (!string.IsNullOrEmpty(args[1]))
                    numEvents = int.Parse(args[1]);
            }
            Console.WriteLine();
            Console.WriteLine("Waiting file events and printing info...");
            Console.WriteLine("Number of events waited for: " + numEvents + ".");
            // WAITING FOR A SPECIFIC NUMBER OF FILE EVENTS TO OCCUR: 
            // Waiting until a specified number fo file events to occur, print information about events:
            WaitFileEvent.ExampleWaitEvents(testFileOrDirectory, numEvents );
            Console.WriteLine();

            return null;
        }

        #endregion Actions.FileUtilities.LogFileEvents



        #region Actions.FileUtilities.WaitFileCreation

        public const string FileWaitCreation = "WaitCreation";

        protected const string FileHelpWaitCreation = NumericsScriptScalarFunction + " FilePath <ReturnIfExists> : Waits until file is created.";

        /// <summary>Executes embedded application - waiting for creation of the specified file.
        /// <para>Application blocks until the specified file is created.</para>
        /// <para>The first argument must be path to the file whose creation is waited for.</para>
        /// <para>Optional second argument (boolean) specifies whether function automatically unblocks if the file 
        /// already exists.</para></summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FileFunctionWaitCreation(string surfaceName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs < 1)
                throw new ArgumentException("Path of the file whose creation is waited for is not specified.");
            string filePath = null;
            bool returnIfExists  = true;   // return automatically if the file exists
            if (numArgs >= 1)
            {
                filePath = args[0];
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentException("Path of the file whose creation is waited for is not specified.");
            }
            if (numArgs >= 2)
            {
                if (!string.IsNullOrEmpty(args[1]))
                {
                    returnIfExists = Util.ParseBoolean(args[1]);
                }
            }
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Waiting file creation...");
            Console.WriteLine("  file path: " + filePath);
            if (returnIfExists)
                Console.WriteLine("  Automatic return if the file exists.");
            else
                Console.WriteLine("  No automatic return.");
            WaitFileCreation waiter = new WaitFileCreation(filePath);
            if (!returnIfExists || !File.Exists(filePath))
            {
                waiter.Wait();
                Console.WriteLine("... file created.");
            } else
            {
                Console.WriteLine("... finished, file already existed." + Environment.NewLine+Environment.NewLine);
            }
            return null;
        }

        #endregion Actions.FileUtilities.WaitFileCreation
                
        protected bool _appFileCommandsInitialized = false;

        /// <summary>Initializes commands for file system related utilities (embedded applications).</summary>
        protected virtual void InitAppFile()
        {

            lock (Lock)
            {
                if (_appFileCommandsInitialized)
                    return;
                AddFileCommand(FileLogEvents, FileFunctionLogEvents, FileHelpLogEvents);
                AddFileCommand(NumericsScriptScalarFunction, NumericsFunctionScriptScalarFuncitons, NumericsHelpScriptScalarFunction);

                _appFileCommandsInitialized = true;
            }
        }


        /// <summary>Runs a file system related utility (embedded application) according to arguments.</summary>
        /// <param name="args">Arguments. 0-th argument is the base command name, 1st argument is the embedded application name, and teh rest
        /// arguments are arguments that are used by the embedded application.</param>
        protected virtual string RunAppFile(string[] args)
        {
            InitAppFile();
            if (args == null)
                throw new ArgumentException("No arguments. Embedded application name (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Test name (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Test name (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " ApplicationName arg1 arg2...");
                        Console.WriteLine(args[0] + " ApplicationName ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of embedded applications: ");
                        for (int i = 0; i < AppFileNames.Count; ++i)
                            Console.WriteLine("  " + AppFileNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] surfaceParams = new string[args.Length - 2];
            for (int i = 0; i < surfaceParams.Length; ++i)
                surfaceParams[i] = args[i + 2];
            int index = AppFileNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Embedded application named " + testName + " is not found. Call with '?' for list of application names.");
            testName = AppFileNames[index];
            string helpString = AppFileHelpStrings[index];
            CommandMethod method = AppFileMethods[index];
            if (surfaceParams.Length >= 1)
                if (surfaceParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(AppFileHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(AppFileHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(testName, surfaceParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
        }

        /// <summary>Runs one of the file system - related embedded applications.</summary>
        /// <param name="arguments">Array containing the base command name, application command name and arguments.</param>
        public virtual string AppFile(string[] arguments)
        {
            string ret = null;
            if (arguments == null)
                throw new ArgumentException("Commandline arguments not specified (null argument).");
            if (arguments.Length < 2)
                Console.WriteLine("Number of arguments should be at least 2 (at least base command name & application name).");
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Running file system - related embedded application..."
                + Environment.NewLine +
                "=============================="
                + Environment.NewLine);
            //Console.WriteLine();
            //Script_PrintArguments("Application arguments: ", arguments);
            //Console.WriteLine();

            if (ret == null)
                ret = RunAppFile(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("File system - related application  finished.");
            Console.WriteLine();
            return ret;
        }  // AppFile

        
        #endregion Actions.FileUtilities




        #region Actions.CryptoUtilities

        /// <summary>List of installed cryptographic command names.</summary>
        protected List<string> AppCryptoNames = new List<string>();

        /// <summary>List of help strings corresponding to installed cryptographic commands.</summary>
        protected List<string> AppCryptoHelpStrings = new List<string>();

        /// <summary>List of methods used to perform cryptographic commmands.</summary>
        protected List<CommandMethod> AppCryptoMethods = new List<CommandMethod>();

        /// <summary>Adds a new cryptography - related embedded application's command (added as sub-command of the base command named <see cref="ConstCrypto"/>).</summary>
        /// <param name="appName">Application name.</param>
        /// <param name="appMethod">Method used to perform the application.</param>
        /// <param name="appHelp">Eventual help string for the application.</param>
        protected void AddCryptoCommand(string appName, CommandMethod appMethod, string appHelp)
        {
            lock (Lock)
            {
                AppCryptoNames.Add(appName.ToLower());
                AppCryptoHelpStrings.Add(appHelp);
                AppCryptoMethods.Add(appMethod);
            }
        }




        #region Actions.CryptoUtilities.GetFileHash


        public const string CryptoGetFileHash = "GetFileHash";

        protected const string CryptoHelpGetFileHash = CryptoGetFileHash + 
@" FilePath <WriteToFile>: Calculates various hash values for the specified file, and eventually saves them to a file.
  FilePath: path to the file whose hash values are calculated.
  WriteToFile (0/1 or on/off or true/false, default false): whether hash values are written to a file. The file
    will have the same name as the hashed file, with extension .chk added.";




        /// <summary>Executes embedded application - calculation of various hash values of a file.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        protected virtual string CryptoFunctionGetFileHash(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs < 1)
                throw new ArgumentException("Path to the file whose hash values are calculated is not specified.");
            string filePath = null;
            bool writeToFile = false;
            string hashFileName = null;  // this will be determined from the file name unless specified by the 3rd argument
            if (numArgs >= 1)
            {
                filePath = args[0];
                if (numArgs >= 2)
                {

                    if (!string.IsNullOrEmpty(args[1]))
                        writeToFile = UtilStr.ToBoolean(args[1]);
                    if(numArgs>=3)
                    {
                        if (!string.IsNullOrEmpty(args[2]))
                            hashFileName = args[2];
                    }
                }
            }
            if (!File.Exists(filePath))
            {
                Console.WriteLine(Environment.NewLine 
                    + "File whose hash values should be calculated does not exist!" + Environment.NewLine
                    + "  Path: " +filePath);
            } else
            {
                FileInfo info = new FileInfo(filePath);
                long fileLength = new FileInfo(filePath).Length;
                string directoryPath = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);
                Console.WriteLine(Environment.NewLine
                    + "Calculationg hash values for the file " + fileName + "..." + Environment.NewLine
                    + "  File length: " + fileLength + Environment.NewLine
                    + "  Complete path: " + filePath);
                Console.WriteLine("Calculating MD5 hash... ");
                string hashMD5 = UtilCrypto.GetFileHashMd5(filePath);
                Console.WriteLine("  = " + hashMD5 + Environment.NewLine);
                Console.WriteLine("Calculating SHA1 hash... ");
                string hashSHA1 = UtilCrypto.GetFileHashSha1(filePath);
                Console.WriteLine("  = " + hashSHA1 + Environment.NewLine);
                Console.WriteLine("Calculating SHA256 hash... ");
                string hashSHA256 = UtilCrypto.GetFileHashSha256(filePath);
                Console.WriteLine("  = " + hashSHA256 + Environment.NewLine);
                Console.WriteLine("Calculating SHA512 hash... ");
                string hashSHA512 = UtilCrypto.GetFileHashSha512(filePath);
                Console.WriteLine("  = " + hashSHA512 + Environment.NewLine);
                //Console.WriteLine(Environment.NewLine
                //  + "Hash values: " + Environment.NewLine
                //  + "  MD5:    " + hashMD5 + Environment.NewLine
                //  + "  SHA1:   " + hashSHA1 + Environment.NewLine
                //  + "  SHA256: " + hashSHA256 + Environment.NewLine
                //  + "  SHA512: " + hashSHA512 + Environment.NewLine);
                string hashFilePath = null;
                if (writeToFile)
                {
                    if (hashFileName == null)
                    {
                        hashFilePath = Path.Combine(directoryPath, fileName + UtilCrypto.ConstCrypto.HashFileExtension);
                    } else
                    {
                        hashFilePath = Path.Combine(directoryPath, hashFileName);
                    }
                    if (File.Exists(hashFilePath))
                    {
                        writeToFile = false;
                        Console.Write(Environment.NewLine + "File to which hash values should be saved already exists." + Environment.NewLine
                          + "  Path: " + hashFilePath + Environment.NewLine + Environment.NewLine
                          + "Do you want to overwrite the file (0/1)? ");
                        UtilConsole.Read(ref writeToFile);
                    }
                    if (writeToFile)
                    {
                        using (TextWriter writer = new StreamWriter(hashFilePath))
                        {
                            Console.WriteLine(Environment.NewLine + "Writing hash values to a file..." + Environment.NewLine
                                + "  path: " + hashFilePath);
                            writer.WriteLine(Environment.NewLine
                                + "File:   " + fileName + Environment.NewLine
                                + "Length: " + fileLength + Environment.NewLine + Environment.NewLine
                                + "Hash values: " + Environment.NewLine
                                + "  MD5:    " + Environment.NewLine + hashMD5 + Environment.NewLine
                                + "  SHA1:   " + Environment.NewLine + hashSHA1 + Environment.NewLine
                                + "  SHA256: " + Environment.NewLine + hashSHA256 + Environment.NewLine
                                + "  SHA512: " + Environment.NewLine + hashSHA512 + Environment.NewLine + "  " + Environment.NewLine);
                            Console.WriteLine("... writing done." + Environment.NewLine);
                        }
                    }
                }
            }
            return null;
        }


        #endregion Actions.CryptoUtilities.GetFileHash




        #region Actions.CryptoUtilities.CheckSum


        public const string CryptoCheckSum = "CheckSum";

        protected const string CryptoHelpCheckSum = CryptoCheckSum +
@" <-c> <-s string> <-h hash> <-t hashType> <-o outputFile> <inputFile1> <inputFile2> ...: 
  Calculates or verifies various types of hash values for files or strings. Calculated file hashes
  can be saved to a file.
    -t hashType: specifies hash type (MD5, SHA-1, SHA-256, SHA-512)
    -c: verification rather than calculation of hashes.
    -s: hash is calculated or verified for the specified string rather than file(s).
    -h hash: hash value to be verified.
    -o outputFile: output file where calculated hashes are written.
    - inputFile1 inputFile2 ...: input files, either files whose hashes are calculated, or files
      containing hash values to be verified (in the case of -c option)";



        public virtual void ReportError(string errorString)
        {
            Console.WriteLine(Environment.NewLine + "ERROR: " + errorString + Environment.NewLine);
        }

        /// <summary>Executes embedded application - calculation AND verification of various hash values of a file.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        protected virtual string CryptoFunctionCheckSum(string appName, string[] args)
        {
            
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs < 1)
                throw new ArgumentException("There should be at least 1 argument (file whose hash value is calculated).");
            HashType hashType = CC.DefaultHashType;
            string outputPath = null;
            string hashValue = null;
            string hashedString = null;
            List<string> inputFilePaths = new List<string>();
            int numInputFiles = 0;
            bool isChecked = false;  // hash is verified, not calculated
            bool isStringHash = false;  // hash is calculated for the specified string
            string ret = null;

            // Parsing of arguments: 
            if (OutputLevel>=5)
            {
                Console.WriteLine();
            }
            for (int whichArg = 0; whichArg < numArgs; ++whichArg)
            {
                string arg = args[whichArg];
                string argLowercase = arg.ToLower();
                if (OutputLevel >= 5)
                {
                    if (whichArg == 0)
                        Console.WriteLine("Argument No. " + whichArg + " (command name): " + arg);
                    else
                        Console.WriteLine("Argument No. " + whichArg + ": " + arg);
                }
                if (whichArg == 0)
                    ;  //  arg. No. 0 skipped because it represents command name.
                // Any argument:
                // Arguments for modified behavior:
                else if (argLowercase == CC.ArgCheck || argLowercase == CC.ArgCheck1)
                {
                    isChecked = true;
                } else if (argLowercase == CC.ArgHashType || argLowercase == CC.ArgHashType1)
                {
                    if (whichArg + 1 >= numArgs)
                        ReportError("Argument \"" + arg + "\" should be followed by hash type.");
                    else
                    {
                        ++whichArg;
                        HashType type = UtilCrypto.GetHashType(args[whichArg]);
                        if (type==HashType.None)
                        {
                            ReportError("Unrecognized hash type: " + args[whichArg] + ".");
                        } else
                            hashType = UtilCrypto.GetHashType(args[whichArg]);
                    }
                } else if (argLowercase == CC.ArgOutputFile || argLowercase == CC.ArgOutputFile1)
                {
                    if (whichArg + 1 >= numArgs)
                        ReportError("Argument \"" + arg + "\" should be followed by output file path.");
                    else
                    {
                        ++whichArg;
                        outputPath = args[whichArg];
                    }
                } else if (argLowercase == CC.ArgHashValue || argLowercase == CC.ArgHashValue1)
                {
                    if (whichArg + 1 >= numArgs)
                        ReportError("Argument \"" + arg + "\" should be followed by hash value.");
                    else
                    {
                        ++whichArg;
                        hashValue = args[whichArg];
                    }
                }
                else if (argLowercase == CC.ArgString || argLowercase == CC.ArgString1)
                {
                    if (whichArg + 1 >= numArgs)
                        ReportError("Argument \"" + arg + "\" should be followed by hashed string.");
                    else
                    {
                        ++whichArg;
                        hashedString = args[whichArg];
                        isStringHash = true;
                    }
                }
                else
                {
                    // Input file arguments:
                    if (!string.IsNullOrEmpty(arg))
                        inputFilePaths.Add(arg);
                }
            }
            numInputFiles = inputFilePaths.Count;
            if (isStringHash)
            {
                // Hash values for STRINGS:
                if (numInputFiles > 0)
                    ReportError("Hash files specified while checksum is calculated for a string.");
                if (isChecked)
                {
                    // String hash verification:
                    if (numInputFiles > 0)
                        ReportError("Redundant specification of hashed files, will not be used.");
                    if (outputPath != null)
                        ReportError("Redundant output file specification, not used.");
                    bool checkPassed = false;
                    if (string.IsNullOrEmpty(hashValue))
                    {
                        ReportError("Hash value to be verified for a string is not specified.");
                    } else
                    {
                        checkPassed = UtilCrypto.CheckStringHash(hashedString, hashValue, hashType);
                    }
                    ret = checkPassed.ToString();
                    if (checkPassed)
                    {
                        Console.WriteLine("String " + hashType.ToString() + ": OK.");
                    } else
                    {
                        Console.WriteLine("String " + hashType.ToString() + ": NOT PASSED!");
                    }
                } else 
                {
                    // String hash calculation:
                    if (hashValue != null)
                        ReportError("Redundant hash value, not used.");
                    if (outputPath != null)
                        ReportError("Writing string hash value to a file is not supported, output file not used.");
                    ret = UtilCrypto.GetStringHash(hashedString, hashType);
                    Console.WriteLine(hashType.ToString() + ": " + ret);
                }
            } else
            {
                if (isChecked)
                {
                    // File hash verification:
                    if (outputPath != null)
                        ReportError("Redundant output file specification, not used.");
                    bool checkPassed = false;
                    if (hashValue != null)
                    {
                        // Verify a single file whose hash is specified by command line:
                        if (numInputFiles < 1)
                        {
                            ReportError("No files for verification specified, there should be one file.");
                            checkPassed = false;
                            ret = checkPassed.ToString();
                        } else
                        {
                            if (numInputFiles > 1)
                                ReportError("More than one file specified for verification, should be one. All will be ckecked.");
                            bool passedThis = true;
                            foreach (string filePath in inputFilePaths)
                            {
                                try
                                {
                                    if (!File.Exists(filePath))
                                    {
                                        passedThis = false;
                                        ReportError("File does not exist: " + filePath);
                                    } else
                                    if (UtilCrypto.CheckFileHash(filePath, hashValue, hashType))
                                    {
                                        Console.WriteLine("File " + hashType.ToString() + ": OK.");
                                    }
                                    else
                                    {
                                        passedThis = false;
                                        Console.WriteLine("File " + hashType.ToString() + ": NOT PASSED.");
                                    }
                                }
                                catch(Exception ex)
                                {
                                    passedThis = false;
                                    ReportError("Exception thrown: " + ex.Message);
                                }
                            }
                            checkPassed = passedThis;
                            ret = checkPassed.ToString();
                        }
                    }
                    else
                    {
                        // Verification of mutiple file hashes from a file:
                        bool passedThis = true;
                        if (outputPath != null)
                            ReportError("Redundant output file specification, not used.");
                        if (numInputFiles < 1)
                        {
                            ReportError("No files containing hash information are specified.");
                            passedThis = false;
                        } else
                        {
                            int numCheckedAll = 0;
                            int numPassed = 0;
                            int numNotPassed = 0;
                            for (int whichInputFile = 0; whichInputFile<numInputFiles; ++ whichInputFile)
                            {
                                string inputFilePath = inputFilePaths[whichInputFile];
                                // Read hash value / file pairs:
                                List<string[]> hashList = null;
                                UtilCrypto.ParseHashFile(inputFilePath, ref hashList);
                                Console.WriteLine("From input file " + Path.GetFileName(inputFilePath) + ":");
                                int numChecked = 0;
                                if (hashList != null)
                                    numChecked = hashList.Count;
                                //if (numChecked == 0)
                                //{
                                //    Console.WriteLine("  No entries.");
                                //} else
                                {
                                    foreach (string[] pair in hashList)
                                    {
                                        string hash = pair[0];
                                        string checkedFile = pair[1];
                                        if (string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(checkedFile))
                                        {
                                            if (string.IsNullOrEmpty(hash))
                                                ReportError("Hash not specified.");
                                            if (string.IsNullOrEmpty(checkedFile))
                                                ReportError("FilePath not specified.");
                                        }
                                        else
                                        {
                                            ++numCheckedAll;
                                            if (!File.Exists(checkedFile))
                                            {
                                                passedThis = false;
                                                ReportError("File does not exist: " + checkedFile);
                                                ++numNotPassed;
                                            } else
                                                if (UtilCrypto.CheckFileHash(checkedFile, hash, hashType))
                                                {
                                                    Console.WriteLine(hashType.ToString() + " OK: " + checkedFile);
                                                    ++numPassed;
                                                }
                                                else
                                                {
                                                    passedThis = false;
                                                    Console.WriteLine(hashType.ToString() + " NOT PASSED: " + checkedFile);
                                                    ++numNotPassed;
                                                }
                                        }
                                    }
                                }
                            }
                            if (numCheckedAll < 1)
                            {
                                passedThis = false;
                                Console.WriteLine("No files to check.");
                            } else
                            {
                                if (numPassed < 1)
                                    Console.WriteLine("No files passed.");
                                else if (numNotPassed < 1)
                                    Console.WriteLine("All files OK.");
                                else
                                {
                                    Console.WriteLine(numPassed.ToString() + " files passed, " + numNotPassed.ToString() + " NOT PASSED.");
                                }
                            }
                            
                        }
                        checkPassed = passedThis;
                        ret = checkPassed.ToString();
                    }
                    ret = checkPassed.ToString();
                }
                else
                {
                    ret = null;
                    // Calculation of one or more file hashes:

                    if (numInputFiles < 1)
                    {
                        ReportError("No files to calculate hash values were specified. Nothing to be done.");
                    } else
                    {
                        List <string[]> hashes = new List<string[]>();
                        foreach (string filePath in inputFilePaths)
                        {
                            if (!File.Exists(filePath))
                            {
                                ReportError("File does not exist: " + filePath);
                            } else
                            {
                                string fileHash = null;
                                try
                                {
                                    fileHash = UtilCrypto.GetFileHash(filePath, hashType);
                                    hashes.Add(new string[] { fileHash, filePath });
                                    Console.WriteLine(hashType.ToString() + ": " + fileHash + " " + filePath);
                                }
                                catch (Exception ex)
                                {
                                    ReportError("Exception thrown: " + ex.Message);
                                }
                            }
                        }
                        if (hashes.Count < 0)
                        {
                            ReportError("No file hashes could be calculated correctly.");
                        }
                        else
                        {
                            if (outputPath != null)
                            {
                                // Write file hashes to the output file:
                                bool doWrite = false;
                                if (!File.Exists(outputPath))
                                {
                                    doWrite = true;
                                }
                                else
                                {
                                    Console.Write(Environment.NewLine
                                        + "Output file already exists: " + outputPath + Environment.NewLine + Environment.NewLine
                                        + "Do you want to overwrite the file (0/1)? ");
                                    UtilConsole.Read(ref doWrite);
                                    if (doWrite)
                                        Console.WriteLine(Environment.NewLine + "File will be overwritten." + Environment.NewLine);
                                    else
                                        Console.WriteLine(Environment.NewLine + "Writing of hashes to a file skipped." + Environment.NewLine);
                                }
                                if (doWrite)
                                {
                                    try
                                    {
                                        using (StreamWriter hashWriter = new StreamWriter(outputPath, false /* append */, Encoding.UTF8))  // File.CreateText(outputPath))
                                        {
                                            foreach (string[] pair in hashes)
                                            {
                                                hashWriter.WriteLine(pair[0] + " " + pair[1]);
                                            }
                                        }
                                        Console.WriteLine("Hashes written to file: " + outputPath);
                                    }
                                    catch (Exception ex)
                                    {
                                        ReportError("Exception thrown: " + ex.Message);
                                    }
                                }
                            }
                        }
                    }




                }
            }

            return ret;
        }


        #endregion Actions.CryptoUtilities.CheckSum






        protected bool _appCryptoCommandsInitialized = false;

        /// <summary>Initializes commands for cryptography related utilities (embedded applications).</summary>
        protected virtual void InitAppCrypto()
        {

            lock (Lock)
            {
                if (_appCryptoCommandsInitialized)
                    return;
                AddCryptoCommand(CryptoGetFileHash, CryptoFunctionGetFileHash, CryptoHelpGetFileHash);
                AddCryptoCommand(CryptoCheckSum, CryptoFunctionCheckSum, CryptoHelpCheckSum);

                _appCryptoCommandsInitialized = true;
            }
        }


        /// <summary>Runs a cryptography related utility (embedded application) according to arguments.</summary>
        /// <param name="args">Arguments. 0-th argument is the base command name, 1st argument is the embedded application name, and teh rest
        /// arguments are arguments that are used by the embedded application.</param>
        protected virtual string RunAppCrypto(string[] args)
        {
            InitAppCrypto();
            if (args == null)
                throw new ArgumentException("No arguments. Embedded application name (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Test name (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Test name (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " ApplicationName arg1 arg2...");
                        Console.WriteLine(args[0] + " ApplicationName ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of embedded applications: ");
                        for (int i = 0; i < AppCryptoNames.Count; ++i)
                            Console.WriteLine("  " + AppCryptoNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] surfaceParams = new string[args.Length - 2];
            for (int i = 0; i < surfaceParams.Length; ++i)
                surfaceParams[i] = args[i + 2];
            int index = AppCryptoNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Embedded application named " + testName + " is not found. Call with '?' for list of application names.");
            testName = AppCryptoNames[index];
            string helpString = AppCryptoHelpStrings[index];
            CommandMethod method = AppCryptoMethods[index];
            if (surfaceParams.Length >= 1)
                if (surfaceParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(AppCryptoHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(AppCryptoHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(testName, surfaceParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
        }

        /// <summary>Runs one of the cryptography - related embedded applications.</summary>
        /// <param name="arguments">Array containing the base command name, application command name and arguments.</param>
        public virtual string AppCrypto(string[] arguments)
        {
            string ret = null;
            if (arguments == null)
                throw new ArgumentException("Commandline arguments not specified (null argument).");
            if (arguments.Length < 2)
                Console.WriteLine("Number of arguments should be at least 2 (at least base command name & application name).");
            if (OutputLevel >= 3)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine + "Running cryptography - related embedded application..."
                    + Environment.NewLine +
                    "=============================="
                    + Environment.NewLine);
                if (OutputLevel >= 5)
                {
                    Console.WriteLine();
                    Script_PrintArguments("Application arguments: ", arguments);
                    Console.WriteLine();
                }
            }
            if (ret == null)
                ret = RunAppCrypto(arguments);

            if (OutputLevel >= 3)
            {
                Console.WriteLine("==============================");
                Console.WriteLine("Cryptography - related application  finished.");
                Console.WriteLine();
            }
            return ret;
        }  // AppCrypto


        #endregion Actions.CryptoUtilities



        #region Action.SystemUtilities

        /// <summary>List of installed system related command names.</summary>
        protected List<string> AppSystemNames = new List<string>();

        /// <summary>List of help strings corresponding to installed system related commands.</summary>
        protected List<string> AppSystemHelpStrings = new List<string>();

        /// <summary>List of methods used to perform system related commmands.</summary>
        protected List<CommandMethod> AppSystemMethods = new List<CommandMethod>();

        /// <summary>Adds a new system - related embedded application's command (added as sub-command of the base command named <see cref="ConstSystem"/>).</summary>
        /// <param name="appName">Application name.</param>
        /// <param name="appMethod">Method used to perform the application.</param>
        /// <param name="appHelp">Eventual help string for the application.</param>
        protected void AddSystemCommand(string appName, CommandMethod appMethod, string appHelp)
        {
            lock (Lock)
            {
                AppSystemNames.Add(appName.ToLower());
                AppSystemHelpStrings.Add(appHelp);
                AppSystemMethods.Add(appMethod);
            }
        }


        #region Actions.SystemUtilities.RuntimeVersion

        public const string SystemRuntimeVersion = "RuntimeVersion";

        protected const string SystemHelpRuntimeVersion = SystemRuntimeVersion + " : Prints version of the runtime environment that application runs on.";

        /// <summary>Executes embedded application - writing to the console and returning version of the runtime that application runs on.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string SystemFunctionRuntimeVersion(string surfaceName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            string ret = UtilSystem.GetRuntimeVersionString();
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Version of the runtime where application is executed: " + ret + Environment.NewLine + Environment.NewLine);

            return ret;
        }

        #endregion Actions.SystemUtilities.RuntimeVersion


        #region Actions.SystemUtilities.ComputerName

        public const string SystemComputerName = "ComputerName";

        protected const string SystemHelpComputerName = SystemComputerName + " : Prints and returns the current computer name.";

        /// <summary>Executes embedded application - writing to the console and returning the current computer name.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string SystemFunctionComputerName(string surfaceName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            string ret = UtilSystem.GetComputerName();
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Name of the current computer: " + ret + Environment.NewLine + Environment.NewLine);

            return ret;
        }

        #endregion Actions.SystemUtilities.ComputerName


        #region Actions.SystemUtilities.DomainName

        public const string SystemDomainName = "DomainName";

        protected const string SystemHelpDomainName = SystemDomainName + " : Prints and returns the current domain name.";

        /// <summary>Executes embedded application - writing to the console and returning the current domain name.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string SystemFunctionDomainName(string surfaceName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            string ret = UtilSystem.GetDomainName();
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Name of the domain: " + ret + Environment.NewLine + Environment.NewLine);

            return ret;
        }

        #endregion Actions.SystemUtilities.DomainName


        #region Actions.SystemUtilities.IpAddress

        public const string SystemIpAddress = "IpAddress";

        protected const string SystemHelpIpAddress = SystemIpAddress + " : Prints and returns IP address of the current computer.";

        /// <summary>Executes embedded application - writing to the console and returning the current IP address.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string SystemFunctionIpAddress(string surfaceName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            string ret = UtilSystem.GetIpAddressLocal();
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "IP address of the current computer: " + ret + Environment.NewLine + Environment.NewLine);

            return ret;
        }

        #endregion Actions.SystemUtilities.IpAddress


        #region Actions.SystemUtilities.UserName

        public const string SystemUserName = "UserName";

        protected const string SystemHelpUserName = SystemUserName + " : Prints and returns the current user name.";

        /// <summary>Executes embedded application - writing to the console and returning the current user name.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string SystemFunctionUserName(string surfaceName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            string ret = UtilSystem.UserName;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Name of the current user: " + ret + Environment.NewLine + Environment.NewLine);

            return ret;
        }

        #endregion Actions.SystemUtilities.UserName


        #region Actions.SystemUtilities.SystemInfo

        public const string SystemSystemInfo = "Info";

        protected const string SystemHelpSystemInfo = SystemSystemInfo + " : Prints and returns basic system info.";

        /// <summary>Executes embedded application - writing to the console and returning the system info.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string SystemFunctionSystemInfo(string surfaceName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            string ret = UtilSystem.GetSystemInfoString();
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "System info: " + Environment.NewLine + ret + Environment.NewLine + Environment.NewLine);

            return ret;
        }

        #endregion Actions.SystemUtilities.SystemInfo


        #region Actions.SystemUtilities.MACAddress

        public const string SystemMACAddress = "MACAddress";

        protected const string SystemHelpMACAddress = SystemMACAddress + " : Prints and returns the current domain name.";

        /// <summary>Executes embedded application - writing to the console and returning the current domain name.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string SystemFunctionMACAddress(string surfaceName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            string ret = UtilSystem.GetMacAddressFastest();
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "MACaddress: " + ret + Environment.NewLine + Environment.NewLine);

            return ret;
        }

        #endregion Actions.SystemUtilities.MACAddress


        protected bool _appSystemCommandsInitialized = false;

        /// <summary>Initializes commands for file system related utilities (embedded applications).</summary>
        protected virtual void InitAppSystem()
        {

            lock (Lock)
            {
                if (_appSystemCommandsInitialized)
                    return;
                
                AddSystemCommand(SystemRuntimeVersion, SystemFunctionRuntimeVersion, SystemHelpRuntimeVersion);
                AddSystemCommand(SystemComputerName, SystemFunctionComputerName, SystemHelpComputerName);
                AddSystemCommand(SystemDomainName, SystemFunctionDomainName, SystemHelpDomainName);
                AddSystemCommand(SystemIpAddress, SystemFunctionIpAddress, SystemHelpIpAddress);
                AddSystemCommand(SystemUserName, SystemFunctionUserName, SystemHelpUserName);
                AddSystemCommand(SystemSystemInfo, SystemFunctionSystemInfo, SystemHelpSystemInfo);
                AddSystemCommand(SystemMACAddress, SystemFunctionMACAddress, SystemHelpMACAddress);

                _appSystemCommandsInitialized = true;
            }
        }


        // DomainName DomainName


        /// <summary>Runs a file system related utility (embedded application) according to arguments.</summary>
        /// <param name="args">Arguments. 0-th argument is the base command name, 1st argument is the embedded application name, and teh rest
        /// arguments are arguments that are used by the embedded application.</param>
        protected virtual string RunAppSystem(string[] args)
        {
            InitAppSystem();
            if (args == null)
                throw new ArgumentException("No arguments. Embedded application name (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Test name (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Test name (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " ApplicationName arg1 arg2...");
                        Console.WriteLine(args[0] + " ApplicationName ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of embedded applications: ");
                        for (int i = 0; i < AppSystemNames.Count; ++i)
                            Console.WriteLine("  " + AppSystemNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] surfaceParams = new string[args.Length - 2];
            for (int i = 0; i < surfaceParams.Length; ++i)
                surfaceParams[i] = args[i + 2];
            int index = AppSystemNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Embedded application named " + testName + " is not found. Call with '?' for list of application names.");
            testName = AppSystemNames[index];
            string helpString = AppSystemHelpStrings[index];
            CommandMethod method = AppSystemMethods[index];
            if (surfaceParams.Length >= 1)
                if (surfaceParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(AppSystemHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(AppSystemHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(testName, surfaceParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
        }

        /// <summary>Runs one of the file system - related embedded applications.</summary>
        /// <param name="arguments">Array containing the base command name, application command name and arguments.</param>
        public virtual string AppSystem(string[] arguments)
        {
            string ret = null;
            if (arguments == null)
                throw new ArgumentException("Commandline arguments not specified (null argument).");
            if (arguments.Length < 2)
                Console.WriteLine("Number of arguments should be at least 2 (at least base command name & application name).");
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Running file system - related embedded application..."
                + Environment.NewLine +
                "=============================="
                + Environment.NewLine);
            //Console.WriteLine();
            //Script_PrintArguments("Application arguments: ", arguments);
            //Console.WriteLine();

            if (ret == null)
                ret = RunAppSystem(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("System system - related application  finished.");
            Console.WriteLine();
            return ret;
        }  // AppSystem


        #endregion Actions.SystemUtilities



        #region Actions.ProcessUtilities

        /// <summary>List of installed process-related command names.</summary>
        protected List<string> AppProcessNames = new List<string>();

        /// <summary>List of help strings corresponding to installed process commands.</summary>
        protected List<string> AppProcessHelpStrings = new List<string>();

        /// <summary>List of methods used to perform process-related commmands.</summary>
        protected List<CommandMethod> AppProcessMethods = new List<CommandMethod>();

        /// <summary>Adds a new process - related embedded application's command (added as sub-command of the base command named <see cref="ConstProcess"/>).</summary>
        /// <param name="appName">Application name.</param>
        /// <param name="appMethod">Method used to perform the application.</param>
        /// <param name="appHelp">Eventual help string for the application.</param>
        protected void AddProcessCommand(string appName, CommandMethod appMethod, string appHelp)
        {
            lock (Lock)
            {
                AppProcessNames.Add(appName.ToLower());
                AppProcessHelpStrings.Add(appHelp);
                AppProcessMethods.Add(appMethod);
            }
        }


        #region Actions.ProcessUtilities.ListProcesses

        public const string ProcessListProcesses = "ListProcesses";

        protected const string ProcessHelpListProcesses = ProcessListProcesses + @" <ProcessName> <CaseSensitive> <FullName> <printDetails> : 
    Lists running processes with specified names.
    ProcessName: process name. If not specified then all running processes are listed.
    CaseSensitive: whether process name is case sensitive, default is false.
    FullName: whether full name must be specified (otherwise substring is enough), default is true.";

        /// <summary>Embedded application - lists all processes that satisfy the specified conditions.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of processes.</returns>
        protected virtual string ProcessFunctionListProcesses(string surfaceName, string[] args)
        {
            int numargs = 0;
            if (args != null)
                numargs = args.Length;
            string processName = null;
            bool caseSensitive = false;
            bool fullName = true;
            bool printDetails = true;
            if (numargs >= 1)
                processName = args[0];
            if (numargs >= 2)
            {
                caseSensitive = Util.ParseBoolean(args[1]);
            }
            if (numargs >= 3)
            {
                fullName = Util.ParseBoolean(args[2]);
            }
            if (numargs >= 4)
            {
                printDetails = Util.ParseBoolean(args[3]);
            }
            List<Process> processes = new List<Process>();
            if (string.IsNullOrEmpty(processName))
                UtilSystem.GetAllProcesses(ref processes);
            else
                UtilSystem.GetProcesses(processName, caseSensitive, fullName, ref processes);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("List of running processes");
            if (processName != null)
            {
                sb.Append(" named " + processName);
                if (caseSensitive)
                    sb.Append("  (case sensitive)");
                else
                    sb.Append("  (case insensitive)");
            }
            sb.AppendLine(":");
            if (processes.Count == 0)
                sb.AppendLine("  No matching processes are running.");
            for (int i = 0; i < processes.Count; ++i)
            {
                Process proc = processes[i];
                if (proc != null)
                {
                    sb.AppendLine(i.ToString() + ": " + proc.ProcessName + ", PID = " + proc.Id);
                    if (printDetails)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(proc.MainWindowTitle))
                                sb.AppendLine("  Window title:" + proc.MainWindowTitle);
                            sb.AppendLine("  Total CPU time:" + proc.TotalProcessorTime.TotalSeconds.ToString());
                            sb.AppendLine("  Total time: " + (DateTime.Now - proc.StartTime).TotalSeconds.ToString());
                            sb.AppendLine("  Num. threads:" + proc.Threads.Count);
                            sb.AppendLine("  Base priority:" + proc.BasePriority);
                            sb.AppendLine("  Priority class:" + proc.PriorityClass);
                        }
                        catch (Exception ex) { sb.AppendLine("  Error: " + ex.Message); } 
                   }
                }
            }
            sb.AppendLine();
            Console.WriteLine(sb.ToString());
            return processes.Count.ToString();
        }

        #endregion Actions.ProcessUtilities.ListProcesses


        #region Actions.ProcessUtilities.ListApplications

        public const string ProcessListApplications = "ListApplications";

        protected const string ProcessHelpListApplications = ProcessListApplications + @" <ProcessName> <CaseSensitive> <FullName> <printDetails> : 
    Lists running applications (processes having main window with title) with specified process names.
    ProcessName: process name. If not specified then all running applications are listed.
    CaseSensitive: whether process name is case sensitive, default is false.
    FullName: whether full name must be specified (otherwise substring is enough), default is true.";

        /// <summary>Embedded application. Lists all running applications sarisfyin specified conditions.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of processes.</returns>
        protected virtual string ProcessFunctionListApplications(string surfaceName, string[] args)
        {
            int numargs = 0;
            if (args != null)
                numargs = args.Length;
            string processName = null;
            bool caseSensitive = false;
            bool fullName = true;
            bool printDetails = true;
            if (numargs >= 1)
                processName = args[0];
            if (numargs >= 2)
            {
                caseSensitive = Util.ParseBoolean(args[1]);
            }
            if (numargs >= 3)
            {
                fullName = Util.ParseBoolean(args[2]);
            }
            if (numargs >= 4)
            {
                printDetails = Util.ParseBoolean(args[3]);
            }
            List<Process> processes = new List<Process>();
            if (string.IsNullOrEmpty(processName))
                UtilSystem.GetAllApplications(ref processes);
            else
                UtilSystem.GetApplications(processName, caseSensitive, fullName, ref processes);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("List of running applications");
            if (processName != null)
            {
                sb.Append(" with process name " + processName);
                if (caseSensitive)
                    sb.Append("  (case sensitive)");
                else
                    sb.Append("  (case insensitive)");
            }
            sb.AppendLine(":");
            if (processes.Count == 0)
                sb.AppendLine("  No matching applications are running.");
            for (int i = 0; i < processes.Count; ++i)
            {
                Process proc = processes[i];
                if (proc != null)
                {
                    sb.AppendLine(i.ToString() + ": " + proc.ProcessName + ", PID = " + proc.Id);
                    if (printDetails)
                    {
                        try
                        {
                        if (!string.IsNullOrEmpty(proc.MainWindowTitle))
                            sb.AppendLine("  Window title:" + proc.MainWindowTitle);
                        sb.AppendLine("  Total CPU time:" + proc.TotalProcessorTime.TotalSeconds.ToString());
                        sb.AppendLine("  Total time: " + (DateTime.Now - proc.StartTime).TotalSeconds.ToString());
                        sb.AppendLine("  Num. threads:" + proc.Threads.Count);
                        sb.AppendLine("  Base priority:" + proc.BasePriority);
                        sb.AppendLine("  Priority class:" + proc.PriorityClass);
                        }
                        catch (Exception ex) { sb.AppendLine("  Error: " + ex.Message); }
                    }
                }
            }
            sb.AppendLine();
            Console.WriteLine(sb.ToString());
            return processes.Count.ToString();
        }

        #endregion Actions.ProcessUtilities.ListApplications


        #region Actions.ProcessUtilities.ListApplicationsByWindow

        public const string ProcessListApplicationsByWindow = "ListApplicationsByWindow";

        protected const string ProcessHelpListApplicationsByWindow = ProcessListApplicationsByWindow + @" <WindowTitle> <CaseSensitive> <FullName> <printDetails> : 
    Lists running applications with specified names of main window title.
    WindowTitle: main window title of the application. If not specified then all running applications are listed.
    CaseSensitive: whether window title is case sensitive, default is false.
    FullName: whether full name must be specified (otherwise substring is enough), default is false.";

        /// <summary>Embedded application. Lists all running applications sarisfyin specified conditions.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of processes.</returns>
        protected virtual string ProcessFunctionListApplicationsByWindow(string surfaceName, string[] args)
        {
            int numargs = 0;
            if (args != null)
                numargs = args.Length;
            string windiwTitle = null;
            bool caseSensitive = false;
            bool fullName = true;
            bool printDetails = true;
            if (numargs >= 1)
                windiwTitle = args[0];
            if (numargs >= 2)
            {
                caseSensitive = Util.ParseBoolean(args[1]);
            }
            if (numargs >= 3)
            {
                fullName = Util.ParseBoolean(args[2]);
            }
            if (numargs >= 4)
            {
                printDetails = Util.ParseBoolean(args[3]);
            }
            List<Process> processes = new List<Process>();
            if (string.IsNullOrEmpty(windiwTitle))
                UtilSystem.GetAllApplications(ref processes);
            else
                UtilSystem.GetApplicationsByWindowTitle(windiwTitle, caseSensitive, fullName, ref processes);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("List of running applications ");
            if (windiwTitle != null)
            {
                sb.Append(" with main window title " + windiwTitle);
                if (caseSensitive)
                    sb.Append("  (case sensitive)");
                else
                    sb.Append("  (case insensitive)");
            }
            sb.AppendLine(":");
            if (processes.Count == 0)
                sb.AppendLine("  No matching applications are running.");
            for (int i = 0; i < processes.Count; ++i)
            {
                Process proc = processes[i];
                if (proc != null)
                {
                    sb.AppendLine(i.ToString() + ": " + proc.ProcessName + ", PID = " + proc.Id);
                    if (printDetails)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(proc.MainWindowTitle))
                                sb.AppendLine("  Window title:" + proc.MainWindowTitle);
                            sb.AppendLine("  Total CPU time:" + proc.TotalProcessorTime.TotalSeconds.ToString());
                            sb.AppendLine("  Total time: " + (DateTime.Now - proc.StartTime).TotalSeconds.ToString());
                            sb.AppendLine("  Num. threads:" + proc.Threads.Count);
                            sb.AppendLine("  Base priority:" + proc.BasePriority);
                            sb.AppendLine("  Priority class:" + proc.PriorityClass);
                        }
                        catch (Exception ex) { sb.AppendLine("  Error: " + ex.Message); }
                    }
                }
            }
            sb.AppendLine();
            Console.WriteLine(sb.ToString());
            return processes.Count.ToString();
        }

        #endregion Actions.ProcessUtilities.ListApplicationsByWindow













        #region Actions.ProcessUtilities.KillProcesses

        public const string ProcessKillProcesses = "KillProcesses";

        protected const string ProcessHelpKillProcesses = ProcessKillProcesses + @" <ProcessName> <CaseSensitive> <FullName> <printDetails> : 
    Kills running processes with specified names.
    ProcessName: process name. If not specified then all running processes are killed.
    CaseSensitive: whether process name is case sensitive, default is false.
    FullName: whether full name must be specified (otherwise substring is enough), default is true.";

        /// <summary>Embedded application - kills all processes that satisfy the specified conditions.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of processes.</returns>
        protected virtual string ProcessFunctionKillProcesses(string surfaceName, string[] args)
        {
            int numargs = 0;
            if (args != null)
                numargs = args.Length;
            string processName = null;
            bool caseSensitive = false;
            bool fullName = true;
            bool printDetails = true;
            if (numargs >= 1)
                processName = args[0];
            if (numargs >= 2)
            {
                caseSensitive = Util.ParseBoolean(args[1]);
            }
            if (numargs >= 3)
            {
                fullName = Util.ParseBoolean(args[2]);
            }
            if (numargs >= 4)
            {
                printDetails = Util.ParseBoolean(args[3]);
            }
            List<Process> processes = new List<Process>();
            UtilSystem.GetProcesses(processName, caseSensitive, fullName, ref processes);
            Console.WriteLine();
            if (processes.Count < 1)
                Console.WriteLine("No matching processes found to be killed.");
            else
            {
                Console.WriteLine("Killing the following processes: ");
                foreach (Process proc in processes)
                {
                    if (proc == null)
                        Console.WriteLine("Null process.");
                    else
                    {
                        Console.WriteLine("  Process " + proc.ProcessName + ", PID = " + proc.Id);
                        proc.Kill();
                    }
                }
            }
            Console.WriteLine();
            return processes.Count.ToString();
        }

        #endregion Actions.ProcessUtilities.KillProcesses


        #region Actions.ProcessUtilities.KillApplications

        public const string ProcessKillApplications = "KillApplications";

        protected const string ProcessHelpKillApplications = ProcessKillApplications + @" <ProcessName> <CaseSensitive> <FullName> <printDetails> : 
    Kills running applications (processes having main window with title) with specified process names.
    ProcessName: process name. If not specified then all running applications are killed.
    CaseSensitive: whether process name is case sensitive, default is false.
    FullName: whether full name must be specified (otherwise substring is enough), default is true.";

        /// <summary>Embedded application. Kills all running applications sarisfyin specified conditions.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of processes.</returns>
        protected virtual string ProcessFunctionKillApplications(string surfaceName, string[] args)
        {
            int numargs = 0;
            if (args != null)
                numargs = args.Length;
            string processName = null;
            bool caseSensitive = false;
            bool fullName = true;
            bool printDetails = true;
            if (numargs >= 1)
                processName = args[0];
            if (numargs >= 2)
            {
                caseSensitive = Util.ParseBoolean(args[1]);
            }
            if (numargs >= 3)
            {
                fullName = Util.ParseBoolean(args[2]);
            }
            if (numargs >= 4)
            {
                printDetails = Util.ParseBoolean(args[3]);
            }
            List<Process> processes = new List<Process>();
            UtilSystem.GetApplications(processName, caseSensitive, fullName, ref processes);
            Console.WriteLine();
            if (processes.Count < 1)
                Console.WriteLine("No matching processes found to be killed.");
            else
            {
                Console.WriteLine("Killing the following processes: ");
                foreach (Process proc in processes)
                {
                    if (proc == null)
                        Console.WriteLine("Null process."); 
                    else
                    {
                        Console.WriteLine("  Process " + proc.ProcessName + ", PID = " + proc.Id);
                        proc.Kill();
                    }
                }
            }
            Console.WriteLine();
            return processes.Count.ToString();
        }

        #endregion Actions.ProcessUtilities.KillApplications


        #region Actions.ProcessUtilities.KillApplicationsByWindow

        public const string ProcessKillApplicationsByWindow = "KillApplicationsByWindow";

        protected const string ProcessHelpKillApplicationsByWindow = ProcessKillApplicationsByWindow + @" <WindowTitle> <CaseSensitive> <FullName> <printDetails> : 
    Kills running applications with specified names of main window title.
    WindowTitle: main window title of the application. If not specified then all running applications are killed.
    CaseSensitive: whether window title is case sensitive, default is false.
    FullName: whether full name must be specified (otherwise substring is enough), default is false.";

        /// <summary>Embedded application. Kills all running applications sarisfyin specified conditions.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of processes.</returns>
        protected virtual string ProcessFunctionKillApplicationsByWindow(string surfaceName, string[] args)
        {
            int numargs = 0;
            if (args != null)
                numargs = args.Length;
            string windowTitle = null;
            bool caseSensitive = false;
            bool fullName = true;
            bool printDetails = true;
            if (numargs >= 1)
                windowTitle = args[0];
            if (numargs >= 2)
            {
                caseSensitive = Util.ParseBoolean(args[1]);
            }
            if (numargs >= 3)
            {
                fullName = Util.ParseBoolean(args[2]);
            }
            if (numargs >= 4)
            {
                printDetails = Util.ParseBoolean(args[3]);
            }
            List<Process> processes = new List<Process>();
            UtilSystem.GetApplicationsByWindowTitle(windowTitle, caseSensitive, fullName, ref processes);
            Console.WriteLine();
            if (processes.Count < 1)
                Console.WriteLine("No matching processes found to be killed.");
            else
            {
                Console.WriteLine("Killing the following processes: ");
                foreach (Process proc in processes)
                {
                    if (proc == null)
                        Console.WriteLine("Null process.");
                    else
                    {
                        Console.WriteLine("  Process " + proc.ProcessName + ", PID = " + proc.Id);
                        proc.Kill();
                    }
                }
            }
            Console.WriteLine();
            return processes.Count.ToString();
        }

        #endregion Actions.ProcessUtilities.KillApplicationsByWindow


        protected bool _appProcessCommandsInitialized = false;

        /// <summary>Initializes commands for process - related utilities (embedded applications).</summary>
        protected virtual void InitAppProcess()
        {

            lock (Lock)
            {
                if (_appProcessCommandsInitialized)
                    return;
                AddProcessCommand(ProcessListProcesses, ProcessFunctionListProcesses, ProcessHelpListProcesses);
                AddProcessCommand(ProcessListApplications, ProcessFunctionListApplications, ProcessHelpListApplications);
                AddProcessCommand(ProcessListApplicationsByWindow, ProcessFunctionListApplicationsByWindow, ProcessHelpListApplicationsByWindow);
                AddProcessCommand(ProcessKillProcesses, ProcessFunctionKillProcesses, ProcessHelpKillProcesses);
                AddProcessCommand(ProcessKillApplications, ProcessFunctionKillApplications, ProcessHelpKillApplications);
                AddProcessCommand(ProcessKillApplicationsByWindow, ProcessFunctionKillApplicationsByWindow, ProcessHelpKillApplicationsByWindow);

                _appProcessCommandsInitialized = true;
            }
        }


        /// <summary>Runs a process - related utility (embedded application) according to arguments.</summary>
        /// <param name="args">Arguments. 0-th argument is the base command name, 1st argument is the embedded application name, and teh rest
        /// arguments are arguments that are used by the embedded application.</param>
        protected virtual string RunAppProcess(string[] args)
        {
            InitAppProcess();
            if (args == null)
                throw new ArgumentException("No arguments. Embedded application name (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Test name (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Test name (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " ApplicationName arg1 arg2...");
                        Console.WriteLine(args[0] + " ApplicationName ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of embedded applications: ");
                        for (int i = 0; i < AppProcessNames.Count; ++i)
                            Console.WriteLine("  " + AppProcessNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] surfaceParams = new string[args.Length - 2];
            for (int i = 0; i < surfaceParams.Length; ++i)
                surfaceParams[i] = args[i + 2];
            int index = AppProcessNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Embedded application named " + testName + " is not found. Call with '?' for list of application names.");
            testName = AppProcessNames[index];
            string helpString = AppProcessHelpStrings[index];
            CommandMethod method = AppProcessMethods[index];
            if (surfaceParams.Length >= 1)
                if (surfaceParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(AppProcessHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(AppProcessHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(testName, surfaceParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
        }

        /// <summary>Runs one of the process - related embedded applications.</summary>
        /// <param name="arguments">Array containing the base command name, application command name and arguments.</param>
        public virtual string AppProcess(string[] arguments)
        {
            string ret = null;
            if (arguments == null)
                throw new ArgumentException("Commandline arguments not specified (null argument).");
            if (arguments.Length < 2)
                Console.WriteLine("Number of arguments should be at least 2 (at least base command name & application name).");
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Running process system - related embedded application..."
                + Environment.NewLine +
                "=============================="
                + Environment.NewLine);
            //Console.WriteLine();
            //Script_PrintArguments("Application arguments: ", arguments);
            //Console.WriteLine();

            if (ret == null)
                ret = RunAppProcess(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("Process system - related application  finished.");
            Console.WriteLine();
            return ret;
        }  // AppProcess


        #endregion Actions.ProcessUtilities.LogProcessEvents



        #region Actions.DataStructures


        /// <summary>List of installed data structure - related demo command names.</summary>
        protected List<string> AppDataStructuresNames = new List<string>();

        /// <summary>List of help strings corresponding to the installed data structure - related demo commands.</summary>
        protected List<string> AppDataStructuresHelpStrings = new List<string>();

        /// <summary>List of methods used to perform data structure - related demo commmands.</summary>
        protected List<CommandMethod> AppDataStructuresMethods = new List<CommandMethod>();

        /// <summary>Adds a new data structure- related embedded demo application's command (added as 
        /// a sub-command of the base command named <see cref="ConstDataStructures"/>).</summary>
        /// <param name="appName">Application name.</param>
        /// <param name="appMethod">Method used to perform the application.</param>
        /// <param name="appHelp">Eventual help string for the application.</param>
        protected void AddDataStructuresCommand(string appName, CommandMethod appMethod, string appHelp)
        {
            lock (Lock)
            {
                AppDataStructuresNames.Add(appName.ToLower());
                AppDataStructuresHelpStrings.Add(appHelp);
                AppDataStructuresMethods.Add(appMethod);
            }
        }




        protected bool _appDataStructuresCommandsInitialized = false;

        /// <summary>Initializes commands for form demo related utilities (embedded applications).</summary>
        protected virtual void InitAppDataStructures()
        {

            lock (Lock)
            {
                if (_appDataStructuresCommandsInitialized)
                    return;

                // Call the init method from the other part of partial class definition:
                InitAppDataStructuresPartial();

                //AddDataStructuresCommand(DataStructuresTestCsvApp, DataStructuresFunctionTestCsvApp, DataStructuresHelpTestCsvApp);
                //AddDataStructuresCommand(DataStructuresTestMultiDimArrayApp, DataStructuresFunctionTestMultiDimArrayApp, DataStructuresHelpTestMultiDimArrayApp);

                _appDataStructuresCommandsInitialized = true;
            }
        }


        /// <summary>Runs a data structures demo - related utility (embedded application) according to arguments.</summary>
        /// <param name="args">Arguments. 0-th argument is the base command name, 1st argument is the embedded application name, and the rest
        /// are arguments that are used by the embedded application.</param>
        protected virtual string RunAppDataStructures(string[] args)
        {
            InitAppDataStructures();
            if (args == null)
                throw new ArgumentException("No arguments. Embedded application name (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Test name (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Test name (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " ApplicationName arg1 arg2...");
                        Console.WriteLine(args[0] + " ApplicationName ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of embedded applications: ");
                        for (int i = 0; i < AppDataStructuresNames.Count; ++i)
                            Console.WriteLine("  " + AppDataStructuresNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] appParams = new string[args.Length - 2];
            for (int i = 0; i < appParams.Length; ++i)
                appParams[i] = args[i + 2];
            int index = AppDataStructuresNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Embedded application named " + testName + " is not found. Call with '?' for list of application names.");
            testName = AppDataStructuresNames[index];
            string helpString = AppDataStructuresHelpStrings[index];
            CommandMethod method = AppDataStructuresMethods[index];
            if (appParams.Length >= 1)
                if (appParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(AppDataStructuresHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(AppDataStructuresHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(testName, appParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
        }

        /// <summary>Runs one of the data structures demo - related embedded applications.</summary>
        /// <param name="arguments">Array containing the base command name, application command name and arguments.</param>
        public virtual string AppDataStructures(string[] arguments)
        {
            string ret = null;
            if (arguments == null)
                throw new ArgumentException("Commandline arguments not specified (null argument).");
            if (arguments.Length < 2)
                Console.WriteLine("Number of arguments should be at least 2 (at least base command name & application name).");
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Running form demo - related embedded application..."
                + Environment.NewLine +
                "=============================="
                + Environment.NewLine);
            //Console.WriteLine();
            //Script_PrintArguments("Application arguments: ", arguments);
            //Console.WriteLine();

            if (ret == null)
                ret = RunAppDataStructures(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("Form demo - related application  finished.");
            Console.WriteLine();
            return ret;
        }  // AppDataStructures



        #endregion Actions.DataStructures


        #endregion Actions


    }  // class ScriptAppBase  // : LoadableScriptBase


    /// <summary>Base cls. for various special function loadable scripts.</summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    /// $A Igor xx;
    public abstract class LoadableScriptSpecialFunctionBase : LoadableScriptBase, ILoadableScript
    {

        #region Standard_TestScripts

        public LoadableScriptSpecialFunctionBase()
            : base()
        { }

        /// <summary>Initializes the current object.</summary>
        protected override void InitializeThis(string[] arguments)
        {
            Script_DefaultInitialize(arguments);
        }

        /// <summary>Runs action of the current object.</summary>
        /// <param name="arguments">Command-line arguments of the action.</param>
        protected override string RunThis(string[] arguments)
        {
            return Script_DefaultRun(arguments);
        }

        #endregion Standard_TestScripts


        #region Commands


        /// <summary>Name of the command that performs my custom test.</summary>
        public const string ConstMyTest = "MyTest";
        public const string ConstHelpMyTest = "Custom test function.";

        /// <summary>Name of the command for custom test.</summary>
        public const string ConstCustomApp = "CustomApp";
        public const string ConstHelpCustomApp = "Custom aplication.";

        #endregion Commands



        /// <summary>Adds a new internal script command under specified name to the internal interpreter of the current 
        /// script object.</summary>
        /// <param name="interpreter">Interpreter on which the command is added.</param>
        /// <param name="commandName">Name of the command. <para>Must not be null or empty string.</para></param>
        /// <param name="command">Method that executes the command. <para>Must not be null.</para></param>
        /// <param name="helpString">Help string associated with command, optionsl (can be null).</param>
        public override void Script_AddCommand(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings,
            string commandName, Script_CommandDelegate command, string helpString)
        {
            base.Script_AddCommand(interpreter, helpStrings, commandName, command, helpString);
        }


        /// <summary>Adds commands to the internal interpreter.</summary>
        /// <param name="interpreter">Interpreter where commands are executed.</param>
        /// <param name="helpStrings">List containg help strings.</param>
        public override void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            base.Script_AddCommands(interpreter, helpStrings);

            Script_AddCommand(interpreter, helpStrings, ConstMyTest, AppMyTest, ConstHelpMyTest);
            Script_AddCommand(interpreter, helpStrings, ConstCustomApp, AppCustomApp, ConstHelpCustomApp);
        }

        #region Actions


        /// <summary>Test action.</summary>
        /// <param name="arguments">Array of command-line arguments.</param>
        public string AppMyTest(string[] arguments)
        {
            Console.WriteLine();
            Console.WriteLine("MY CUSTOM TEST.");
            Console.WriteLine("This script is alive.");
            Script_PrintArguments("Script arguments: ", arguments);
            Console.WriteLine("==== END of my custom test.");
            Console.WriteLine();
            return null;
        }

        /// <summary>Custom application.</summary>
        public virtual string AppCustomApp(string[] arguments)
        {
            string ret = null;
            Console.WriteLine();
            Console.WriteLine("CUSTOM APPLICATION run from the APPLICATION SCRIPT.");
            Console.WriteLine("==============================");
            Script_PrintArguments("Script arguments: ", arguments);
            Console.WriteLine();

            Console.WriteLine("==============================");
            Console.WriteLine("Custom application finished.");
            Console.WriteLine();
            return ret;
        }


        #endregion Actions


    }  // class LoadableScriptSpecialFunctionBase

}

