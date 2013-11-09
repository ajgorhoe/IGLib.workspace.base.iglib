using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IG.Lib
{

    /// <summary>Base class for main application classes containing main method of application,
    /// based on command line interpreter.</summary>
    /// <remarks><para>Dhis class represent the base for standard scheme for IGLib-based applications that include
    /// command-line interpreter and eventually a number of tests whose selection is hard-coded and which are
    /// performed when application command-line arguments are not specified.</para></remarks>
    /// $A Igor xx Nov09;
    public abstract class ApplicationCommandlineBase
    {

        /// <summary>Constructor.</summary>
        public ApplicationCommandlineBase()
        {
            AddDefaultAssemblies();
            TestInterpreter = CreateInterpreter();
        }

        #region Data


        // Choices of different kinds of scripts:

        /// <summary>Selecting constant fot TEST script.
        /// <para>Test scripts typically include several example methods, which can be 
        /// selected by the first command-line argument.</para></summary>
        protected const string ConstScriptTests = "ScriptTests";

        /// <summary>Selecting constant fot CUSTOM script.</summary>
        protected const string ConstScriptCustomApp = "ScriptCustomApp";

        /// <summary>Selecting constant for interactive run.</summary>
        protected const string ConstInteractive = "Interactive";

        /// <summary>Interpreter command for running in interactive mode.</summary>
        protected string ConstRunInteractive = "Interactive";


        /// <summary>Interpreter command for running dynamic scripts.</summary>
        protected string ConstRunScript = "RunScript";

        /// <summary>Location of the script to be loaded.</summary>
        protected string ScriptLocation = null;

        /// <summary>Main selector for script that is run.</summary>
        protected string ScriptGroupChoice;

        protected string _scriptChoice;

        /// <summary>Name of the selected script type that is run.
        /// <para>If <see cref="ScriptType"/> is defined then this property is defined in accordance with it.</para></summary>
        protected string ScriptChoice
        {
            get {
                if (_scriptChoice == null)
                {
                    if (ScriptType != null)
                        _scriptChoice = ScriptType.FullName;
                }
                return _scriptChoice; 
            }
            set {
                _scriptChoice = value;
                if (_scriptChoice != null)
                {
                    if (_scriptType != null && _scriptChoice != _scriptType.Name)
                    {
                        _scriptType = null;
                    }
                    if (_scriptToRun != null && _scriptChoice != _scriptToRun.GetType().Name)
                    {
                        _scriptToRun = null;
                    }
                }
            }
        }

        protected Type _scriptType;

        /// <summary>Selected type of the script to be run.</summary>
        protected Type ScriptType
        {
            get { return _scriptType; }
            set { 
                _scriptType = value;
                if (value != null)
                {
                    _scriptChoice = value.Name;
                    if (_scriptToRun != null)
                    {
                        if (_scriptToRun.GetType() != _scriptType)
                            _scriptToRun = null;
                    }
                }
            }
        }

        /// <summary>Selector for script action.</summary>
        protected string ScriptAction;

        /// <summary>Script arguments used when script is run directly by creating a script object.</summary>
        protected string[] DirectArguments = null;

        /// <summary>Interpreter arguments used when script is run through interpreter.</summary>
        protected string[] InterpreterArguments = null;

        /// <summary>Default active directory. The current directory will be set to this directory.</summary>
        protected string DefaultActiveDir;

        /// <summary>Optimization directory that contains data and message files of optimization server.</summary>
        public string OptDir;

        protected ILoadableScript _scriptToRun;

        /// <summary>Script to be run.</summary>
        protected ILoadableScript ScriptToRun
        {
            get 
            { 
                if (_scriptToRun == null && _scriptType!=null)
                {
                    _scriptToRun = ScriptLoaderBase.CreateScriptObject(_scriptType.FullName);
                }
                return _scriptToRun; 
            }
            set { 
                _scriptToRun = value;
                if (_scriptToRun != null)
                {
                    ScriptType = _scriptToRun.GetType();
                }
            }
        }

        /// <summary>Whether script is loaded and run through interpreter (alternative is direct construction of script class).</summary>
        protected bool RunThroughInterpreter = false;

        protected string _userNameLowerCase = null;

        /// <summary>Sets name of the current user. This method is provided to enable 
        /// testing code under another user name. Setting to null anihilates effect of previous calls.
        /// <para>After call to this method, user name can be set to null in order to retrieve the true
        /// user logged in for subsequent operations.</para>
        /// <para>Warning: you should use this only exceptionally, e.g. for testing, and only in testing or
        /// demo sections of code.</para></summary>
        /// <param name="username">Name of the user to be set. Null annihilates previous calls and causes
        /// that system provided user name is returned by subsequent queries.</param>
        protected virtual void SetUserName(string userName)
        {
            UtilSystem.SetUsername(userName);
        }

        /// <summary>Gets name of the current user with all letters converted to lower case 
        /// (in order to avoid ambiguities).</summary>
        public virtual string UserNameLowerCase
        {
            get 
            {
                return UtilSystem.UserNameLowerCase;
            }
        }

        /// <summary>Returns true if the current user logged on the computer is Igor, or false otherwise.</summary>
        public virtual bool IsUserIgor
        {
            get { return UtilSystem.IsUserIgor; }
        }

        /// <summary>Returns true if the current user logged on the computer is Tadej, or false otherwise.</summary>
        public virtual bool IsUserTadej
        {
            get { return UtilSystem.IsUserTadej; }
        }


        /// <summary>Application interpreter for running test scripts.</summary>
        protected CommandLineApplicationInterpreter TestInterpreter;

        #endregion Data

        #region Operation


        //protected virtual CommandLineApplicationInterpreter CreateInterpreterDefault()
        //{
        //    return new CommandLineApplicationInterpreter();
        //}

        /// <summary>Creates and returns application's command-line interpreter.</summary>
        protected abstract CommandLineApplicationInterpreter CreateInterpreter();

        /// <summary>Adds assemblies to be automatically referenced by loaded scripts.</summary>
        public virtual void AddDefaultAssemblies()
        {
            ScriptLoaderBase.AddDefaultAssemblies();
        }


        /// <summary>Returns the number of script run method's arguments (i.e. arguments that are stored in <see cref="DirectArguments"/> 
        /// and <see cref="InterpreterArguments"/>). If there are different numbers of </summary>
        protected int GetScriptNumArguments()
        {
            int ret = 0;
            if (DirectArguments != null)
                ret = DirectArguments.Length;
            if (InterpreterArguments != null)
            {
                int numArg = InterpreterArguments.Length - 2;  // first 2 arguments here are for script load/run command + for script location
                if (numArg > ret)
                    ret = numArg;
            }
            return ret;
        }

        /// <summary>Sets the number of script run method's arguments to the specified number. Reallocates argument arrays if necessary.
        /// <para>Number of arguments includes script run command and command selector for script's internal interpreter, i.e. this number
        /// is 2 greater than the number of pure arguments.</para>
        /// <para>For arguments for running script through interpreter, actual number of arguments is 2 greater because
        /// it includes command for script loading/running + script location.</para></summary>
        /// <param name="numArguments"></param>
        protected virtual void SetScriptNumArguments(int numArguments)
        {
            // Allocate argument arrays if necessary:
            if (DirectArguments == null)
                DirectArguments = new string[numArguments + 0];
            if (InterpreterArguments == null)
                InterpreterArguments = new string[numArguments + 2];
            // Resize argument arrays if necessary: 
            if (DirectArguments.Length != numArguments)
            {
                string[] args = new string[numArguments + 0];
                // transcribe previous arguments as applicable:
                for (int i = 0; i < DirectArguments.Length && i<args.Length; ++i)
                    args[i] = DirectArguments[i];
                DirectArguments = args;
            }
            if (InterpreterArguments.Length != numArguments + 2)  // 2 for script run command + script location
            {
                string[] args = new string[numArguments + 2];
                // transcribe previous arguments as applicable:
                for (int i = 0; i < InterpreterArguments.Length && i<args.Length; ++i)
                    args[i] = InterpreterArguments[i];
                InterpreterArguments = args;
            }
        }

        /// <summary>Sets the specified script argument; updates array of arguments for running script directly
        /// as well as arguments for running script through interpreter.</summary>
        /// <param name="whichArgument">Index of argument in the array of arguments that is actually passed to the 
        /// script (in the case of running through interpreter, this means without interpreter command for running
        /// dynamic scripts and without script location). The 0-th argument is typically (but not always) reserved
        /// for action selection.</param>
        /// <param name="argumentValue">Value of the specified argument to be set.</param>
        /// $A Igor Nov09 Dec11;
        protected virtual void SetScriptArgument(int whichArgument, string argumentValue)
        {
            if (whichArgument < 0)
                throw new ArgumentException("Argument index can not be less than 0.");
            // Calculate number of arguments as the greater of the minimal number of arguments necessary for including
            // the specified argument index, and current number of arguments:
            int numArguments = whichArgument + 1;
            if (GetScriptNumArguments() > numArguments)
                numArguments = GetScriptNumArguments();
            // Allocate argument arrays if necessary:
            SetScriptNumArguments(numArguments);
            // Set the specified arguments:
            DirectArguments[whichArgument] = argumentValue;
            InterpreterArguments[whichArgument + 2] = argumentValue;
        }

        /// <summary>Gets the specified string arguments, as it is currently set.</summary>
        /// <param name="whichArgument">Index of the obtained argument in the array of arguments to be passed to the 
        /// script (in the case of running through interpreter, this means without interpreter command for running
        /// dynamic scripts and without script location). The 0-th argument is typically (but not always) reserved
        /// for action selection.</param>
        protected virtual string GetScriptArgument(int whichArgument)
        {
            return DirectArguments[whichArgument];
        }

        /// <summary>Runs tests from scripts according to hard-coded settings.
        /// <para>Standard form of test applications for functionality based on IGLib.</para></summary>
        /// <param name="args">Command-line argumets passed when the application is run.</param>
        /// <remarks><para>Standard scheme for IGLib-based test applications has been adopted in December 2011.</para></remarks>
        /// $A Igor xx Nov09 Dec11;
        public abstract void TestMain(string[] args);

        /// <summary>Default main method for the current application.</summary>
        /// <param name="args">Command-line argumets passed when the application is run.</param>
        /// <remarks><para>Standard scheme for IGLib-based test applications has been adopted in December 2011.</para></remarks>
        /// $A Igor xx Nov09 Dec11;
        public virtual void AplicationMain(string[] args)
        {
            // Start the application's command line interpreter:
            if (args != null)
                if (args.Length > 0)
                {
                    try
                    {
                        TestInterpreter.Run(args);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ERROR in executing the command " + args[0] + ": ");
                        Console.WriteLine("  " + ex.Message);
                        Console.WriteLine();
                        Console.WriteLine();
                        throw;
                    }
                    finally
                    {
                    }
                    return;
                }

            // We did not specify interpreter command to run (eventually with arguments), run the default:
            Console.WriteLine("Launching application, hard-coded arguments ...");
            TestInterpreter.WarnCommandReplacement = true;  // warn me if any command has been replaced.
            TestMain(args);
        }  // MainDefault

        #endregion Operation

    }  // class CommandlineInterpreterApplicationBase




    /// <summary>Class encapsulating a command-line shell. Used as embedded shell application that can be 
    /// installed on command-line interpreters and runnable scripts.</summary>
    /// $A Igor Jan01;
    public class ShellApplication<InterpreterType> : ApplicationCommandlineBase
        where InterpreterType : CommandLineApplicationInterpreter, new()
    {

        public ShellApplication()
            : base()
        {
            DefaultActiveDir = @"./";
            OptDir = DefaultActiveDir;
        }



        /// <summary>Entry point of the application.</summary>
        /// <param name="args">Application arguments.</param>
        public void Main(string[] args)
        {
            //new ProgramTestIgor().AplicationMain(args);
            this.AplicationMain(args);
        } // Main(string[])


        #region Operation

        /// <summary>Creates and returns application's command-line interpreter.</summary>
        protected override CommandLineApplicationInterpreter CreateInterpreter()
        {
            // return new AppTestOpt(false);
            CommandLineApplicationInterpreter ret = new InterpreterType();
            ret.RegisterSystemPriorityUpdating();  // thread priority of the interpreter will be updated when global thread priority changes.
            return ret;
        }

        /// <summary>Adds assemblies to be automatically referenced by loaded scripts.</summary>
        public override void AddDefaultAssemblies()
        {
            base.AddDefaultAssemblies();
            ScriptLoaderBase.AddDefaultAssemblies(
                "IGLib.dll",
                "IGLibReporterMsg.dll",
                "MathNet.Numerics.dll"
                );

            //"iglibopt.dll",
            //"iglibshellext.dll",
            //"iglibneuralext.dll",
            //"System.Windows.Forms.dll",
            //"System.Drawing.dll",
            //"igplot2d.dll",
            //"igplot3d.dll",
            //"Kitware.VTK.dll",
            //"ZedGraph.dll"

        }


        /// <summary>Default main method for the current shell application.</summary>
        /// <param name="args">Command-line argumets passed when the application is run.</param>
        /// $A Igor xx Nov09 Dec11;
        public override void AplicationMain(string[] args)
        {
            Util.OutputLevel = 0;
            base.AplicationMain(args);
            Util.OutputLevel = 0;
            if (TestInterpreter != null)
            {
                if (!TestInterpreter.AsyncIsAllCompleted())
                {
                    Console.WriteLine();
                    Console.WriteLine("Waiting for asynchronous jobs to complete...");
                    TestInterpreter.AsyncWaitAll();
                    Console.WriteLine();
                    Console.WriteLine("... all asynchronous jobs have completed.");
                    Console.WriteLine();
                }
            }
        }


        /// <summary>Runs the shell interpreter.</param>
        /// <remarks><para>Standard scheme for IGLib-based test applications.</para></remarks>
        /// $A Igor Dec12;
        public override void TestMain(string[] args)
        {
            try
            {
                DefaultActiveDir = @"../../testdata/scripts";
                OptDir = DefaultActiveDir;
                RunThroughInterpreter = false;
                LoadableScriptBase.DefaultOutputLevel = 2;
                //// Run interpreter interactively:
                //TestInterpreter.Run(new string[] { ConstRunInteractive });
                // Run interpreter interactively:
                TestInterpreter.Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine();
                Console.WriteLine("In: Testmain.");
                Console.WriteLine("Script group executed: " + ScriptGroupChoice);
                Console.WriteLine("Script: " + ScriptChoice);
                Console.WriteLine("Action: " + ScriptAction);
                throw;
            }

        }  // TestMain(string[])

        #endregion Operation


    }  // class ShellApplication<InterpreterType>


}
