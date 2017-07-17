// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// 

using System;
using System.Collections.Generic;

using IG.Lib;
using IG.Crypto;
using IG.Forms;

namespace IG.Script
{


    /// <summary>Embedded script class for cryptographic utilities such as calculation and verification 
    /// of file hashes. Commands of this embedded script can be run from the main application.
    /// <para>Currently, the set of cryptographic utilities provided is limited to file verification, 
    /// tools, i.e. calculation and verification of file hashes (checksums).</para></summary>
    /// <remarks><para>This script class has multiple roles.</para>
    /// <para>It wraps the relevant embedded commands of base script classes such as <see cref="AppBase"/>
    /// such that these commands can be called in a more convenient way. For example, calling such commands
    /// in the current script does not require stating the script class name (which usually does not have any
    /// connection with the function) and then the containing command name and then the nested command name, 
    /// but only the command name is called.
    /// Wrapping of these commands does not involve reimplementation of the underlying methods, as existent
    /// methods on the existent scripts are used.</para>
    /// <para>This script also implements some commands implemeted in the <see cref="AppBase"/> class. In this
    /// way, we do not to link the extended project (that includes that class but also includes graphics and 
    /// other redundand stuff), but we only need to include the IGLibForms project. This is especially 
    /// important in the case of the small shell project that only includes the cryptographic utilities.</para>
    /// <para></para>
    /// <para>This script is uesd in two locations: in a small shell project devoted to cryptographic utilities,
    /// and in the main script class of the IGShell program where it serves as embedded application providng
    /// some cryptographic utilities (file checksums etc.).</para></remarks>
    /// $A Igor Jul10;
    public class AppEmbeddedCryptoIGShell : AppBase, 
        ILoadableScript, ILockable
    {
        
        #region Construction

        /// <summary>Prevents argument-less constructor (because this application script is meant to be 
        /// used only as embedded script).</summary>
        private AppEmbeddedCryptoIGShell()
        {  }
        
        /// <summary>Constructs a new embedded application script object.</summary>
        /// <param name="masterCommand">Name of the command that was used to call this embedded script.</param>
        public AppEmbeddedCryptoIGShell(string masterCommand)
            : base()
        { 
            this.EmbeddedCommandName = masterCommand;

        }


        #endregion Construction


        #region Commands

        /// <summary>Name of the command that runs a test of the cryptographic application, i.e.
        /// it checks that the application is functional.</summary>
        public const string ConstTestCrypto = "TestCrypto";
        public const string ConstHelpTestCrypto = "Tests the cryptogrphy embedded application.";

        // Reimplemented commands:

        /// <summary>Name of the command that launches a form for calculation of file or string hash functions.</summary>
        public const string ConstHashForm = "HashForm";
        public const string ConstHelpHashForm = 
@"Launches a form for calculation and verification of various hash functions (checksums)
  of files and text strings.";

        /// <summary>Name of the command that launches a form for calculation of hash functions for all
        /// files contained in the specified directory.</summary>
        public const string ConstHashDirForm = "HashDirForm";
        public const string ConstHelpHashDirForm =
@"Launches a form for calculation and verification of various hash functions (checksums)
  of all files contained in a given directory and all its subdirectories.";

        /// <summary>Name of the command that launches a form for calculation of file or string hash functions,
        /// or hash functions for all files contained in a directory.</summary>
        public const string ConstHashAllForm = "HashAllForm";
        public const string ConstHelpHashAllForm =
@"Launches a form for calculation and verification of various hash functions (checksums)
  of individual files, text strings, or all files contained in a directory.";

        // Wrapped commands:

        /// <summary>Name of the command that calculates various hash values for the specified file, 
        /// and eventually saves them to a file..</summary>
        public const string ConstGetFileHash = "GetFileHash";
        public const string ConstHelpGetFileHash = ConstGetFileHash + 
@" FilePath <WriteToFile>: Calculates various hash values for the specified file, and eventually saves them to a file.
  FilePath: path to the file whose hash values are calculated.
  WriteToFile (0/1 or on/off or true/false, default false): whether hash values are written to a file. The file
    will have the same name as the hashed file, with extension .chk added.";


        /// <summary>Name of the command for calculation/verification of file/string hashes.</summary>
        public const string ConstCheckSum = "CheckSum";
        public const string ConstHelpCheckSum = ConstCheckSum +
@" <-c> <-s string> <-h hash> <-t hashType> <-o outputFile> <inputFile1> <inputFile2> ...";


        #endregion Commands

        protected int _numCommandsRemoved = 0;

        /// <summary>Adds application commands to the application interpreter.</summary>
        /// <param name="interpreter">Interpreter where commands are executed.</param>
        /// <param name="helpStrings">List containg help strings.</param>
        public override void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {

            //// Perform the base script's AddCommands:
            //// Base script's commands are not added in this case!
            base.Script_AddCommands(interpreter, helpStrings);

            if (_numCommandsRemoved < 1)
            {
                ++_numCommandsRemoved;
                // Remove all pre-installed commands on the script first:
                Script_RemoveAllCommands();
            }

            // Test command of this embedded application / TestCrypto:
            Script_AddCommand(interpreter, helpStrings, ConstTestCrypto, AppTestCrypto, ConstHelpTestCrypto);
            
            // Reimplementation of applications from other scrits:
            Script_AddCommand(interpreter, helpStrings, ConstHashForm, AppHashForm, ConstHelpHashForm);
            Script_AddCommand(interpreter, helpStrings, ConstHashDirForm, AppHashDirForm, ConstHelpHashDirForm);
            Script_AddCommand(interpreter, helpStrings, ConstHashAllForm, AppHashAllForm, ConstHelpHashAllForm);

            // Wrapped commands:
            Script_AddCommand(interpreter, helpStrings, ConstGetFileHash, AppGetFileHash, ConstHelpGetFileHash);

            Script_AddCommand(interpreter, helpStrings, ConstCheckSum, AppCheckSum, ConstHelpCheckSum);


        }

        #region Applications.Reimplemented


        protected HashForm hashForm;

        /// <summary>Executes embedded application - launches a windows form for calculation and verification
        /// of various hash values of a file.</summary>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        protected virtual string AppHashForm(string[] args)
        {
            Console.WriteLine(Environment.NewLine + "Launching a form for calculation of hash values..." + Environment.NewLine);

            lock (Lock)
            {
                try
                {
                    UtilConsole.HideConsoleWindow();
                    if (hashForm == null)
                        hashForm = new HashForm();
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


        protected HashDirForm hashDirForm;

        /// <summary>Executes embedded application - launches a windows form for calculation and verification
        /// of various hash values of all files of a specified directory.</summary>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        protected virtual string AppHashDirForm(string[] args)
        {
            Console.WriteLine(Environment.NewLine + "Launching a form for calculation of hash values in a directory ..." + Environment.NewLine);

            lock (Lock)
            {
                try
                {
                    UtilConsole.HideConsoleWindow();
                    if (hashDirForm == null)
                        hashDirForm = new HashDirForm();
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


        protected HashAllForm hashAllForm;

        /// <summary>Executes embedded application - launches a windows form for calculation and verification
        /// of various hash values of a file, text, or all files in a directory.</summary>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        protected virtual string AppHashAllForm(string[] args)
        {
            Console.WriteLine(Environment.NewLine + "Launching a form for calculation of hash values of a file, text, or directory ..." + Environment.NewLine);

            lock (Lock)
            {
                try
                {
                    UtilConsole.HideConsoleWindow();
                    if (hashAllForm == null)
                        hashAllForm = new HashAllForm();
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

        #endregion Applications.Reimplemented


        #region Actions.WrappedCommands


        protected ScriptAppBase _wrappedAppBase;

        /// <summary>Shell application of type <see cref="ScriptAppBase"/> used by the current application 
        /// interpreter as source of some wrapped commands.
        /// <para>Initialized on demand (lazy evaluation).</para></summary>
        public ScriptAppBase WrappedAppBase
        {
            get
            {
                lock (Lock)
                {
                    if (_wrappedAppBase == null)
                    {
                        _wrappedAppBase = new AppEmbeddedCryptoIGShell(null);
                    }
                }
                return _wrappedAppBase;
            }
            protected set
            { lock (Lock) { _wrappedAppBase = value; } }
        }


        /// <summary>Executes embedded application - calculates various hashes of the specified file
        /// and eventually writes them to a file.</summary>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        protected virtual string AppGetFileHash(string[] args)
        {
            return WrappedScriptAppBase(WrappedAppBase /* wrapped commandline application */, 
                ScriptAppBase.ConstCrypto1 /* basic command name */, 
                ScriptAppBase.CryptoGetFileHash /* embedded command name */, args);
        }


        /// <summary>Executes embedded application - calculation/verification of file or string checksums.</summary>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        protected virtual string AppCheckSum(string[] args)
        {
            return WrappedScriptAppBase(WrappedAppBase /* wrapped commandline application */,
                ScriptAppBase.ConstCrypto1 /* basic command name */,
                ScriptAppBase.CryptoCheckSum /* embedded command name */, args);
        }


        /// <summary>Generic wrapper for commandline application's commands. Runs the specified wrapped 
        /// command installed on the specified commandline application.</summary>
        /// <param name="wrappedScriptApplication">Wrapped commandline application by which the command is run.</param>
        /// <param name="wrappedCommand">Wrapped command that is run by the wrapped application.</param>
        /// <param name="wrappedEmbeddedCommand">Embedded command, or null if there is no embedded command.</param>
        /// <param name="arguments">Array of command-line arguments.</param>
        public string WrappedScriptAppBase(ILoadableScript wrappedScriptApplication, 
            string wrappedCommand, string wrappedEmbeddedCommand, string[] arguments)
        {
            if (wrappedScriptApplication == null)
                throw new ArgumentException("Wrapped commandline application is not specified (null argument).");
            if (string.IsNullOrEmpty(wrappedCommand))
                throw new ArgumentException("Wrapped command to be executed is not specified.");
            if (arguments == null)
                throw new ArgumentException("Arguments not specified (null array).");
            if (arguments.Length < 1)
                throw new ArgumentException("There are no arguments (empty array). " + Environment.NewLine
                    + "At least command name should be specified, or '?' for help.");

            wrappedScriptApplication.EmbeddedCommandName = arguments[1];  // pass the 1st arg. as nested command name
            string returnedString = null;
            if (OutputLevel >= 5)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine);
                Console.WriteLine("=========================================================");
                Console.WriteLine("EMBEDDED APPLICATION - class " + wrappedScriptApplication.GetType().FullName + ".");
                Console.WriteLine("Executing command: " + wrappedScriptApplication.EmbeddedCommandName);
                Console.WriteLine();
            }

            // Extract commandline:
            // Transcribe arguments from argument array of the external command's:
            string[] args;
            if (string.IsNullOrEmpty(wrappedEmbeddedCommand))
            {
                // Case where command is not nested (no embedded command):
                args = new string[arguments.Length + 1];
                args[0] = wrappedCommand;
                for (int i = 0; i < arguments.Length; ++i)
                {
                    args[i + 1] = arguments[i];
                }
            } else
            {
                ///* Case where there is nested command: */
                //args = new string[arguments.Length + 2];
                //args[0] = wrappedCommand;
                //args[1] = wrappedEmbeddedCommand;
                //for (int i = 0; i < arguments.Length; ++i)
                //{
                //    args[i+2] = arguments[i];
                //}

                /* Case where there is nested command: */
                args = new string[arguments.Length + 1];
                args[0] = wrappedCommand;
                args[1] = wrappedEmbeddedCommand;
                for (int i = 0; i < arguments.Length - 1; ++i)
                {
                    args[i + 2] = arguments[i + 1];
                }

            }

            /// Run the command by the embedded scripting object:
            returnedString = wrappedScriptApplication.Run(args);

            if (OutputLevel >= 5)
            {
                Console.WriteLine();
                Console.WriteLine("Embedded script's application finished.");
                Console.WriteLine("-------------------------------------------------------------");
                Console.WriteLine(Environment.NewLine + Environment.NewLine);
            }

            return returnedString;
        }

        #endregion Applications.WrappedCommands



        #region Applications

        /// <summary>Tests that the current embedded aplication script is functional; prints commans's argument and other data.</summary>
        /// <param name="arguments">Array containing the base command name and its arguments.</param>
        public virtual string AppTestCrypto(string[] arguments)
        {
            string ret = null;
            if (arguments == null)
                throw new ArgumentException("Commandline arguments not specified (null argument).");
            if (arguments.Length < 1)
                Console.WriteLine("Number of arguments should be at least 2 (at least base command name & application name).");
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Running EMBEDDED application "
                + arguments[0] + "..."
                + Environment.NewLine +
                "=============================="
                + Environment.NewLine);
            Console.WriteLine("This is the command " + ConstTestCrypto + " from EMBEDDED application class " + this.GetType().FullName);
            Console.WriteLine("Command used to invoke the embedded application script: " + EmbeddedCommandName);
            Console.WriteLine();
            Script_PrintArguments("Application arguments: ", arguments);
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("Printing argumets without using internal method: ");
            if (arguments == null)
                Console.WriteLine("No arguments (null array)!");
            else if (arguments.Length < 1)
                Console.WriteLine("No arguments (empty array).");
            else
            {
                Console.WriteLine("Arguments:");
                for (int i = 0; i < arguments.Length; ++i)
                {
                    Console.WriteLine("  arg. " + i + ": " + arguments[i]);
                }
            }
            Console.WriteLine();

            //if (ReturnedString == null)
            //    ReturnedString = RunAppCustom(arguments);

            Console.WriteLine("==============================");
            Console.WriteLine("Custom application  finished.");
            Console.WriteLine();
            return ret;
        }  // AppPrintArguments



        #endregion Applications

    }


}