// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// TESTING SCRIPT FILE: various examples.

using System;
//using System.Collections;
using System.Collections.Generic;
//using System.IO;
//using System.Windows.Forms;

//using IG.Num;
using IG.Lib;

//using IG.Plot2d;
// using IG.Gr3d;
//using IG.Ferdo;

using IG.Script;


namespace IG.Script
{


    /// <summary>Internal script interpreter for running embedded applications.</summary>
    /// <remarks>
    /// <para>In the applications that have the command-line interpreter, embedded applications from this class can typically be
    /// run in the following way:</para>
    /// <para>  AppName Internal IG.Script.AppShellExt CommandName arg1 arg2 ...</para>
    /// <para>where AppName is the application name, IG.Script.AppBase is the full name of the script class that contains
    /// embedded applications, CommandName is name of the command thar launches embedded application, and arg1, arg2, etc.
    /// are command arguments for the embedded application.</para></remarks>
    /// <seealso cref="ScriptAppBase"/>
    /// $A Igor Jan13;
    public class ScriptAppIgBaseExt : AppBase, 
        ILoadableScript
    {

        public ScriptAppIgBaseExt()
            : base()
        { }



        #region Commands.Shell

        // Embedded shell application:

        /// <summary>Name of the command that performs my custom test.</summary>
        public const string ConstShell = "Shell";
        public const string ConstHelpShell =
@"Command shell that enables running pre-installed commands.";

        #endregion Commands.Shell

        #region Commands.Embedded

        /// <summary>Name of the command that invokes embedded cryptographic commands.</summary>
        public const string ConstEmbeddedCrypto = "Crypto";
        public const string ConstHelpEmbeddedCrypto = ConstCrypto1 +
@" command <arg1> <arg2> ... : Executes cryptography-related embedded commands.";


//        // From guest developers:
//        /// <summary>Name of the command that performs embedded commands contributed by Ferdo.</summary>
//        public const string ConstEmbeddedFerdo = "Ferdo";
//        public const string ConstHelpEmbeddedFerdo = ConstEmbeddedFerdo +
//@" command <arg1> <arg2> ... : Executes Ferdo's commands.";



        #endregion Commands.Embedded


        protected int _numExecutedRemove = 0;

        /// <summary>Adds application commands to the application interpreter.</summary>
        /// <param name="interpreter">Interpreter where commands are executed.</param>
        /// <param name="helpStrings">List containg help strings.</param>
        public override void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            base.Script_AddCommands(interpreter, helpStrings);

            if (_numExecutedRemove < 1)
            {
                ++_numExecutedRemove;
                base.Script_RemoveCommand("crypto");
                base.Script_RemoveCommand("Crypto");
            }

            //// TODO: uncomment or delete!
            //// Embedded Shell Application:
            //Script_AddCommand(interpreter, helpStrings, ConstShell, AppShell, ConstHelpShell);

            // Embedded command-line applications:
            Script_AddCommand(interpreter, helpStrings, ConstEmbeddedCrypto, AppEmbeddedCryptoIGS, ConstHelpEmbeddedCrypto);

            // Install lower case version of command to override existing command form the AppBase class:
            // Script_AddCommand(interpreter, helpStrings, ConstEmbeddedCrypto.ToLower(), AppEmbeddedCryptoIGS, ConstHelpEmbeddedCrypto);
            
            //// From guest developers:
            //Script_AddCommand(interpreter, helpStrings, ConstEmbeddedFerdo, AppEmbeddedFerdo, ConstHelpEmbeddedFerdo);
            
        }




        #region Actions.Shell

        //// TODO: UNCOMMENT OR DELETE!

        ///// <summary>Shell application. Enables running pre-installed commands.</summary>
        ///// <param name="arguments">Array of command-line arguments.</param>
        //public string AppShell(string[] arguments)
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("Launching shell interpreter...");
        //    Console.WriteLine("  " + ConstShell + " Internal - runs commands interactively/");
        //    Console.WriteLine("  " + ConstShell + " Internal ? - runs commands interactively, lists installed commands");
        //    Console.WriteLine();
        //    Script_PrintArguments("Application arguments: ", arguments);
        //    Console.WriteLine("");
        //    int numShellArguments = 0;
        //    if (arguments != null)
        //    {
        //        if (arguments.Length > 0)
        //            numShellArguments = arguments.Length - 1;
        //    }
        //    string[] shellArguments = null;
        //    if (numShellArguments > 0)
        //        shellArguments = new string[numShellArguments];
        //    for (int i = 1; i < arguments.Length; ++i)
        //    {
        //        shellArguments[i - 1] = arguments[i];
        //    }
        //    new ShellIGLib().Main(shellArguments);
        //    return null;
        //}

        #endregion Actions.Shell


        #region Actions.EmbeddedScripts

        protected AppEmbeddedCryptoIGShell _embeddedCrypto;

        /// <summary>Embedded cryptographic shell application used by the current application 
        /// interpreter as proxy application for execution of cryptography-related commands.
        /// <para>Initialized on demand (lazy evaluation).</para></summary>
        public AppEmbeddedCryptoIGShell EmbeddedCrypto
        {
            get
            {
                lock (Lock)
                {
                    if (_embeddedCrypto == null)
                    {
                        _embeddedCrypto = new AppEmbeddedCryptoIGShell(null);
                    }
                }
                return _embeddedCrypto;
            }
            protected set
            {  lock (Lock) { _embeddedCrypto = value; }  }
        }

        /// <summary>Shell application. Enables running pre-installed commands.</summary>
        /// <param name="arguments">Array of command-line arguments.</param>
        public virtual string AppEmbeddedCryptoIGS(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentException("Arguments not specified (null array).");
            if (arguments.Length < 2)
                throw new ArgumentException("There are no arguments (empty array). " + Environment.NewLine
                    + "At least command name should be specified, or '?' for help.");
            
            AppEmbeddedCryptoIGShell embeddedScriptApplication = EmbeddedCrypto;  // allocated as necessary (property getter)
            embeddedScriptApplication.EmbeddedCommandName = arguments[1];  // pass the 1st arg. as nested command name
            string returnedString = null;
            if (OutputLevel >= 5)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine);
                Console.WriteLine("=========================================================");
                Console.WriteLine("EMBEDDED APPLICATION - class " + embeddedScriptApplication.GetType().FullName + ".");
                Console.WriteLine("Executing command: " + embeddedScriptApplication.EmbeddedCommandName);
                Console.WriteLine();
            }

            // Extract commandline:
            // Transcribe arguments from argument array of the external command's:
            string[] args = new string[arguments.Length - 1];
            for (int i = 1; i < arguments.Length; ++i)
            {
                args[i - 1] = arguments[i];
            }

            /// Run the command by the embedded scripting object:
            returnedString = embeddedScriptApplication.Run(args);

            if (OutputLevel >= 5)
            {
                Console.WriteLine();
                Console.WriteLine("Embedded script's application finished.");
                Console.WriteLine("-------------------------------------------------------------");
                Console.WriteLine(Environment.NewLine + Environment.NewLine);
            }

            return returnedString;
        }



        //#region AppFerdo

        //// Guest's embedded application (Ferdo):


        //protected AppFerdo _embeddedFerdo;

        ///// <summary>Embedded cryptographic shell application used by the current application 
        ///// interpreter as proxy application for execution of cryptography-related commands.
        ///// <para>Initialized on demand (lazy evaluation).</para></summary>
        //public AppFerdo EmbeddedFerdo
        //{
        //    get
        //    {
        //        lock (Lock)
        //        {
        //            if (_embeddedFerdo == null)
        //            {
        //                _embeddedFerdo = new AppFerdo(null);
        //            }
        //        }
        //        return _embeddedFerdo;
        //    }
        //    protected set
        //    { lock (Lock) { _embeddedFerdo = value; } }
        //}

        ///// <summary>Shell application. Enables running pre-installed commands.</summary>
        ///// <param name="arguments">Array of command-line arguments.</param>
        //public string AppEmbeddedFerdo(string[] arguments)
        //{
        //    if (arguments == null)
        //        throw new ArgumentException("Arguments not specified (null array).");
        //    if (arguments.Length < 2)
        //        throw new ArgumentException("There are no arguments (empty array). " + Environment.NewLine
        //            + "At least command name should be specified, or '?' for help.");

        //    AppFerdo embeddedScriptApplication = EmbeddedFerdo;  // allocated as necessary (property getter)
        //    embeddedScriptApplication.EmbeddedCommandName = arguments[1];  // pass the 1st arg. as nested command name
        //    string returnedString = null;
        //    if (OutputLevel >= 5)
        //    {
        //        Console.WriteLine(Environment.NewLine + Environment.NewLine);
        //        Console.WriteLine("=========================================================");
        //        Console.WriteLine("EMBEDDED APPLICATION - class " + embeddedScriptApplication.GetType().FullName + ".");
        //        Console.WriteLine("Executing command: " + embeddedScriptApplication.EmbeddedCommandName);
        //        Console.WriteLine();
        //    }

        //    // Extract commandline:
        //    // Transcribe arguments from argument array of the external command's:
        //    string[] args = new string[arguments.Length - 1];
        //    for (int i = 1; i < arguments.Length; ++i)
        //    {
        //        args[i - 1] = arguments[i];
        //    }

        //    /// Run the command by the embedded scripting object:
        //    returnedString = embeddedScriptApplication.Run(args);

        //    if (OutputLevel >= 5)
        //    {
        //        Console.WriteLine();
        //        Console.WriteLine("Embedded script's application finished.");
        //        Console.WriteLine("-------------------------------------------------------------");
        //        Console.WriteLine(Environment.NewLine + Environment.NewLine);
        //    }

        //    return returnedString;
        //}

        //#endregion AppFerdo




        #endregion Actions.EmbeddedScripts



    }  // AppIGShell

}