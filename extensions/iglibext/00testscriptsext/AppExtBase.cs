// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// TESTING SCRIPT FILE: various examples.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using IG.Num;
using IG.Lib;
using IG.Forms;

using IG.Plot2d;
using IG.Gr3d;

namespace IG.Script
{


    /// <summary>Internal script for running embedded applications.</summary>
    /// <remarks>
    /// <para>In the applications that have the command-line interpreter, embedded applications from this class can typically be
    /// run in the following way:</para>
    /// <para>  AppName Internal IG.Script.AppExtBase CommandName arg1 arg2 ...</para>
    /// <para>where AppName is the application name, IG.Script.AppBase is the full name of the script class that contains
    /// embedded applications, CommandName is name of the command thar launches embedded application, and arg1, arg2, etc.
    /// are command arguments for the embedded application.</para></remarks>
    /// <seealso cref="ScriptAppBase"/>
    /// $A Igor xx Feb03;
    public class AppExtBase : AppBase, ILoadableScript
    {

        public AppExtBase()
            : base()
        { }


        #region Commands


        /// <summary>Name of the command for the file system-related embedded applications.</summary>
        public const string ConstFormDemo = "FormDemo";
        public const string ConstHelpFormDemo = 
@"Various windows forms-related demonstrational embedded applications. 
  Run with the '?' argument to see which applications are available.";


        #endregion Commands

        
        /// <summary>Adds commands to the internal interpreter.</summary>
        /// <param name="interpreter">Interpreter where commands are executed.</param>
        /// <param name="helpStrings">List containg help strings.</param>
        public override void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            base.Script_AddCommands(interpreter, helpStrings);
            Script_AddCommand(interpreter, helpStrings, ConstFormDemo, AppFormDemo, ConstHelpFormDemo);
        }


        #region Actions


        #region Actions.FormDemos

        /// <summary>List of installed form demo command names.</summary>
        protected List<string> AppFormDemoNames = new List<string>();

        /// <summary>List of help strings corresponding to installed form demo commands.</summary>
        protected List<string> AppFormDemoHelpStrings = new List<string>();

        /// <summary>List of methods used to perform form demo commmands.</summary>
        protected List<CommandMethod> AppFormDemoMethods = new List<CommandMethod>();

        /// <summary>Adds a new form demonstration - related embedded application's command (added as 
        /// a sub-command of the base command named <see cref="ConstFormDemo"/>).</summary>
        /// <param name="appName">Application name.</param>
        /// <param name="appMethod">Method used to perform the application.</param>
        /// <param name="appHelp">Eventual help string for the application.</param>
        protected void AddFormDemoCommand(string appName, CommandMethod appMethod, string appHelp)
        {
            lock (Lock)
            {
                AppFormDemoNames.Add(appName.ToLower());
                AppFormDemoHelpStrings.Add(appHelp);
                AppFormDemoMethods.Add(appMethod);
            }
        }


        #region Actions.FormDemos.FadingMessage

        public const string FormDemoFadingMessage = "FadingMessage";

        protected const string FormDemoHelpFadingMessage = FileLogEvents + " : Runs the fading message demo.";

        /// <summary>Executes embedded application - demonstration of fading messages.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FormDemoFunctionFadingMessage(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Runnning the fading message demo..." + Environment.NewLine);

            Console.WriteLine("Running 1st example ...");
            FadingMessage.Example();

            Console.WriteLine("Running 2nd example...");
            FadingMessage.Example2();

            Console.WriteLine(Environment.NewLine + "Fading message demo finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.FormDemos.FadingMessage


        #region Actions.FormDemos.BrowserSimple

        public const string FormDemoBrowserSimple = "Browser";

        protected const string FormDemoHelpBrowserSimple = FileLogEvents + " : Runs the fading message demo.";

        /// <summary>Executes embedded application - demonstration of fading messages.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FormDemoFunctionBrowserSimple(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Running the simple web browser demo..." + Environment.NewLine);

            BrowserSimpleWindow browser = new BrowserSimpleWindow();
            try
            {
                Application.Run(browser);
            }
            catch
            {
                browser.ShowDialog();
            }

            Console.WriteLine(Environment.NewLine + "Fading message demo finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.FormDemos.BrowserSimple


        #region Actions.FormDemos.WindowPositioning

        public const string FormDemoWindowPositioning = "WindowPositioning";

        protected const string FormDemoHelpWindowPositioning = FormDemoWindowPositioning + " : Runs the window positioning test.";

        /// <summary>Executes embedded application - window positioning test.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FormDemoFunctionWindowPositioning(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Running the window positioning test..." + Environment.NewLine);

            WindowPositionerForm testForm = new WindowPositionerForm();
            try
            {
                Application.Run(testForm);
            }
            catch
            {

            }

            Console.WriteLine(Environment.NewLine + "Window positioning test finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        // BrowserSimple BrowseerSimple

        #endregion Actions.FormDemos.WindowPositioning



        #region Actions.FormDemos.MessageBoxes

        public const string FormDemoMessageBoxLauncher = "MessageBoxLauncher";

        protected const string FormDemoHelpMessageBoxLauncher = FormDemoMessageBoxLauncher + " : Runs the window positioning test.";

        /// <summary>Executes embedded application - messaxe box launcher demo.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FormDemoFunctionMessageBoxLauncher(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Running the message box launcher demonstration..." + Environment.NewLine);

            MessageBoxLauncher testForm = new MessageBoxLauncher();
            try
            {
                Application.Run(testForm);
            }
            catch
            {

            }

            Console.WriteLine(Environment.NewLine + "Maeesge box launcher demo finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.FormDemos.MessageBoxes


        // BrowserSimple FadingMessage FadingMessage 

        protected bool _appFormDemoCommandsInitialized = false;

        /// <summary>Initializes commands for form demo related utilities (embedded applications).</summary>
        protected virtual void InitAppFormDemo()
        {

            lock (Lock)
            {
                if (_appFormDemoCommandsInitialized)
                    return;
                AddFormDemoCommand(FormDemoFadingMessage, FormDemoFunctionFadingMessage, FormDemoHelpFadingMessage);
                AddFormDemoCommand(FormDemoBrowserSimple, FormDemoFunctionBrowserSimple, FormDemoHelpBrowserSimple);
                AddFormDemoCommand(FormDemoWindowPositioning, FormDemoFunctionWindowPositioning, FormDemoHelpWindowPositioning);
                AddFormDemoCommand(FormDemoMessageBoxLauncher, FormDemoFunctionMessageBoxLauncher, FormDemoHelpMessageBoxLauncher);
                
                _appFormDemoCommandsInitialized = true;
            }
        }


        /// <summary>Runs a form demo - related utility (embedded application) according to arguments.</summary>
        /// <param name="args">Arguments. 0-th argument is the base command name, 1st argument is the embedded application name, and the rest
        /// are arguments that are used by the embedded application.</param>
        protected virtual string RunAppFormDemo(string[] args)
        {
            InitAppFormDemo();
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
                        for (int i = 0; i < AppFormDemoNames.Count; ++i)
                            Console.WriteLine("  " + AppFormDemoNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] appParams = new string[args.Length - 2];
            for (int i = 0; i < appParams.Length; ++i)
                appParams[i] = args[i + 2];
            int index = AppFormDemoNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Embedded application named " + testName + " is not found. Call with '?' for list of application names.");
            testName = AppFormDemoNames[index];
            string helpString = AppFormDemoHelpStrings[index];
            CommandMethod method = AppFormDemoMethods[index];
            if (appParams.Length >= 1)
                if (appParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(AppFormDemoHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(AppFormDemoHelpStrings[index]);
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

        /// <summary>Runs one of the form demo - related embedded applications.</summary>
        /// <param name="arguments">Array containing the base command name, application command name and arguments.</param>
        public virtual string AppFormDemo(string[] arguments)
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
                ret = RunAppFormDemo(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("Form demo - related application  finished.");
            Console.WriteLine();
            return ret;
        }  // AppFormDemo


        #endregion Actions.FormDemos


        #region Actions.CryptoUtilities_Inherited


        public const string CryptoHashForm = "HashForm";

        protected const string CryptoHelpHashForm = CryptoHashForm +
@": Launches a GUI window for calculation of hash values for files and text.";

        protected HashGeneratorForm hashForm;

        /// <summary>Executes embedded application - launches a windows form for calculation of 
        /// various hash values of a file.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        protected virtual string CryptoFunctionHashForm(string appName, string[] args)
        {
            Console.WriteLine(Environment.NewLine + "Launching a form for calculation of hash values..." + Environment.NewLine);
            
            lock (Lock)
            {
                try
                {
                    UtilConsole.HideConsoleWindow();
                    if (hashForm == null)
                        hashForm = new HashGeneratorForm();
                    hashForm.ShowDialog();
                }
                finally
                {
                    UtilConsole.ShowConsoleWindow();
                }
            }

            Console.WriteLine("Form closed.");
            return null;
        }

        /// <summary>Initializes commands for cryptography related utilities (embedded applications).
        /// <para>Here the method from the base class is overridden in order to add some additional utilities.</para></summary>
        protected override void InitAppCrypto()
        {

            lock (Lock)
            {
                if (_appCryptoCommandsInitialized)
                    return;
                AddCryptoCommand(CryptoHashForm, CryptoFunctionHashForm, CryptoHelpHashForm);

                
                base.InitAppCrypto();
                _appCryptoCommandsInitialized = true;
            }
        }

        #endregion Actions.CryptoUtilities_Inherited

        #endregion Actions


    }  // class AppExtBase

}