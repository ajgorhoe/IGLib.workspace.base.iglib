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
            InitCommandLineApplicationInterpreterBasic(caseSensitive);
            InitCommands();
        }

        /// <summary>Performs basic initializations of the interpreter.</summary>
        /// <param name="caseSensitive">Whether interpreter is case sensitive or not.</param>
        protected virtual void InitCommandLineApplicationInterpreterBasic(bool caseSensitive)
        {
            this.CaseSensitive = caseSensitive;
            this.MainThread = this.AddNewThread();
            this._globalThread = new CommandThread(this);
            this._globalAuxiliaryFrame = _globalThread.BaseFrame;
        }

        /// <summary>Installs basic commands on the interpreter.</summary>
        protected virtual void InitCommands()
        {

            this.AddCommandMt("Get", CmdGetVariable);
            this.AddCommandMt("Set", CmdSetVariable);
            this.AddCommandMt("SetRes", CmdSetVariableToCommandResult);
            this.AddCommandMt("SetResult", CmdSetVariableToCommandResult);
            this.AddCommandMt("Clear", CmdClearVariable);
            this.AddCommandMt("PrintVariable", CmdPrintVariable);

            // Information about interpretation:
            this.AddCommandMt("ThreadInfo", CmdThreadInfo);
            this.AddCommandMt("ThreadId", CmdThreadId);
            this.AddCommandMt("StackLevel", CmdStackLevel);
            this.AddCommandMt("SuppressInteractive", CmdSuppressInteractive);
            this.AddCommandMt("SuppressInteractiveFrame", CmdSuppressInteractiveFrame);

            this.AddCommandMt(Command_Block, CmdBlock);    // "Block"
            this.AddCommandMt(Command_EndBlock, CmdEndBlock);    // "EndBlock"
            this.AddCommandMt(Command_BeginCalc, CmdCalcJsBlock);    // "BeginCalc"
            this.AddCommandMt(Command_EndCalc, CmdEndCalcJsBlock);    // "EndCalc"
            this.AddCommandMt(Command_If, CmdIf);    // "CmdIf"
            this.AddCommandMt(Command_ElseIf, CmdElseIf);    // "CmdElseIf"
            this.AddCommandMt(Command_Else, CmdElse);    // "CmdElse"
            this.AddCommandMt(Command_EndIf, CmdEndIf);    // "CmdEndIf"

            this.AddCommandMt(Command_While, CmdWhile);  // "While"
            this.AddCommandMt(Command_EndWhile, CmdEndWhile);  // "EndWhile"

            //this.AddCommandMt("", Cmd);
            //this.AddCommandMt("", Cmd);
            //this.AddCommandMt("", Cmd);
            //this.AddCommandMt("", Cmd);
            //this.AddCommandMt("", Cmd);


            this.AddCommandMt("OutputLevel", CmdOutputLevel);
            this.AddCommandMt("Write", CmdWrite);
            this.AddCommandMt("WriteLn", CmdWriteLine);
            this.AddCommandMt("WriteLine", CmdWriteLine);
            this.AddCommandMt("Read", CmdRead);
            this.AddCommandMt("Run", CmdRunFile);
            this.AddCommandMt("Try", CmdTryRun);
            this.AddCommandMt("Repeat", CmdRunRepeat);
            this.AddCommandMt("RepeatVerbose", CmdRunRepeatVerbose);
            this.AddCommandMt("SetPriority", CmdSetPriority);
            this.AddCommandMt("Parallel", CmdRunParallel);
            this.AddCommandMt("Par", CmdRunParallel);
            this.AddCommandMt("ParallelRepeat", CmdRunParallelRepeat);
            this.AddCommandMt("ParRep", CmdRunParallelRepeat);
            this.AddCommandMt("ParallelPrint", CmdPrintParallelCommands);
            this.AddCommandMt("ParPrint", CmdPrintParallelCommands);
            this.AddCommandMt("Async", CmdRunAsync);
            this.AddCommandMt("RunAsync", CmdRunAsync);
            this.AddCommandMt("AsyncWait", CmdAsyncWaitResults);
            this.AddCommandMt("AsyncIsCompleted", CmdAsyncCompleted);
            this.AddCommandMt("Sleep", CmdSleepSeconds);
            this.AddCommandMt("ThrowExceptions", CmdThtrowExceptions);
            this.AddCommandMt("Interactive", CmdRunInteractive);
            this.AddCommandMt("Int", CmdRunInteractive);
            // this.AddCommandMt("RunBlock", CmdRunBlock);
            this.AddCommandMt("System", CmdRunSystem);
            this.AddCommandMt("Sys", CmdRunSystem);
            this.AddCommandMt("Cd", CmdCurrentDirectory);
            this.AddCommandMt("CurrentDirectory", CmdCurrentDirectory);

            this.AddCommandMt("Calc", CmdExpressionEvaluatorJavascript);

            this.AddCommandMt("Exit", CmdExit);
            this.AddCommandMt("?", CmdHelp);
            this.AddCommandMt("Help", CmdHelp);
            this.AddCommandMt("About", CmdAbout);
            this.AddCommandMt("ApplicationInfo", CmdApplicationInfo);
            this.AddCommandMt("AppInfo", CmdApplicationInfo);
            this.AddCommandMt("C", CmdComment);
            this.AddCommandMt("Comment", CmdComment);
            this.AddCommandMt("//", CmdComment);
            this.AddCommandMt("PrintCommands", CmdPrintCommands);

            this.AddCommandMt("PipeServer", CmdPipeServerCreate);
            this.AddCommandMt("PipeServersRemove", CmdPipeServersRemove);
            this.AddCommandMt("PipeServerInfo", CmdPipeServerInfo);

            this.AddCommandMt("PipeClient", CmdPipeClientCreate);
            this.AddCommandMt("PipeClientsRemove", CmdPipeClientsRemove);
            this.AddCommandMt("PipeClientInfo", CmdPipeClientInfo);
            this.AddCommandMt("PipeClientSend", CmdPipeClientGetServerResponse);


            this.AddCommandMt("Module", CmdLoadModule);
            this.AddCommandMt("LoadModule", CmdLoadModule);
            this.AddCommandMt("IsModuleLoaded", CmdIsModuleLoaded);
            this.AddCommandMt("Loaded", CmdIsModuleLoaded);

            this.AddCommandMt("RunInternal", CmdRunInternalScriptClass);
            this.AddCommandMt("Internal", CmdRunInternalScriptClass);
            this.AddCommandMt("RunScript", CmdRunScriptFile);
            this.AddCommandMt("LoadClass", CmdLoadScript);
            this.AddCommandMt("RunClass", CmdRunLoadedScript);
            this.AddCommandMt("WriteAssemblies", WriteLoadableScriptReferencedAssemblies);

            // Test commands and modules:
            this.AddCommandMt("TestProduct", CmdTestProduct);
            this.AddCommandMt("Test", CmdTest);
            this.AddCommandMt("TestSpeed", CmdTestSpeed);
            this.AddCommandMt("TestSpeedLong", CmdTestSpeedLong);
            this.AddCommandMt("TestQR", CmdTestQR);
            this.AddCommandMt("TestLU", CmdTestLU);

            this.AddModule("Test1", ModuleTest1);
            this.AddModule("Test2", ModuleTest2);
        }



        /// <summary>Default value of the flg indicating whether command names are case sensitive.</summary>
        public const bool DefaultCaseSensitive = false;

        protected bool _caseSensitive = false;

        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            protected set { _caseSensitive = value; }
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

        // protected SortedDictionary<string, ApplicationCommandDelegate> _commands = new SortedDictionary<string, ApplicationCommandDelegate>();

        protected SortedDictionary<string, ApplicationCommandDelegateMt >
            _commandsMt = new SortedDictionary<string, ApplicationCommandDelegateMt >();

        // CommandStackFrame _globalFrame = null;


        protected CommandThread _globalThread = null;

        protected CommandStackFrame _globalAuxiliaryFrame = null;

        /// <summary>Global frame where global variables are stored.</summary>
        public CommandStackFrame GlobalFrame
        {
            get { return _globalAuxiliaryFrame; }
        }

        private CommandThread _mainThread = null; //new CommandThread<CommandLineApplicationInterpreter>(this);

        /// <summary>Stack frames (containing variables) and other data for the main interpretation thread.</summary>
        public CommandThread MainThread
        {
            get { return _mainThread; }
            protected set { _mainThread = value; }
        }

        /// <summary>Main command thread of the interpreter (usually run in the same thread where interpreter was created).</summary>
        private List<CommandThread> _commandThreads = new List<CommandThread>();

        /// <summary>List of command threads that exist on the interpreter.</summary>
        public List<CommandThread> CommandThreads
        {
            get { return _commandThreads; }
            protected set { _commandThreads = value; }
        }

        public CommandThread AddNewThread()
        {
            CommandThread ret = new CommandThread(this);
            return ret;
        }

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

        protected char _variableStart = '$';

        protected char _expressionStart = '#';

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

        //public StopWatch1 _timer;

        ///// <summary>Gets the stopwatch used for measuring time of commands.
        ///// <para>This property always returns an initialized stopwatch.</para></summary>
        //public StopWatch1 Timer
        //{
        //    get { if (_timer == null) _timer = new StopWatch1(); return _timer; }
        //}

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





        /// <summary>Returns true if the specified string represents a variable reference, false otherwise.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="str">String that is checked.</param>
        protected virtual bool IsVariableReference(CommandThread cmdThread, string str)
        {
            if (str != null)
                if (str.Length > 1)
                    if (str[0] == _variableStart)
                        return true;
            return false;
        }

        /// <summary>Returns value of the referenced variable if the specified string represents a 
        /// variable reference (begins with the variableStart character, usually '$'), otherwise the 
        /// original sting is returned.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="str">String that is eventually substituted by variable value in the case that it 
        /// represents a variable reference.</param>
        protected virtual string SubstituteVariableOrExpressionReference(CommandThread cmdThread, string str)
        {
            if (str == null)
                return str;
            else if (str.Length < 2)
                return str;
            else
            {
                char firstChar = str[0];
                if (firstChar == _variableStart)
                    return this.GetVariableValue(cmdThread, str.Substring(1, str.Length - 1));
                else if (firstChar == _expressionStart)
                    return this.EvaluateJs(str.Substring(1, str.Length - 1));
                else
                    return str;
            }
        }


        // VARIABLES:


        /// <summary>Returns local variable with the specified name that is defined specified 
        /// number of frames below the top stack frame.
        /// <para>Null is returned if the variable does not exist on that frame.</para></summary>
        /// <param name="cmdThread">Thread where this method is executed.</param>
        /// <param name="framesBelowTop">Number of frames below the top-most stack frame where the 
        /// variable is defined.</param>
        /// <returns></returns>
        public InterpreterVariable GetLocalVariable(CommandThread cmdThread, string varName, int framesBelowTop)
        {

            int stackLevel = cmdThread.TopFrameIndex;

            throw new NotImplementedException();




            if (cmdThread.TopFrameIndex < framesBelowTop)
                throw new InvalidOperationException("The stack is less than " + framesBelowTop + " levels deep.");
            InterpreterVariable ret = null;
            int whichLevelBelow = 0;
            CommandStackFrame frame = cmdThread.TopFrame;
            while (whichLevelBelow <= framesBelowTop)
            {
                if (whichLevelBelow == framesBelowTop)
                    return frame.GetVariableDef(varName);
                else
                {
                    if (frame.StackLevel == 0)
                        throw new InvalidOperationException("Stack is not deep enough.");
                    else if (frame.BlockType == CodeBlockType.Callable)
                        throw new InvalidOperationException("Callable code block was encountered " + whichLevelBelow + " levels below the stack top " 
                            + Environment.NewLine + "  , before the specified stack level was reached.");
                    frame = (CommandStackFrame) frame.GetParentStackFrame();
                }

            }
            CommandStackFrame freme = cmdThread.TopFrame;
            return ret;
        }

        public InterpreterVariable GetFirstLocalVariable(CommandThread cmdThread, string value)
        {
            InterpreterVariable ret = null;
            
            return ret;
        }

        public InterpreterVariable GetFiestLocalOrGlobalVariable(CommandThread cmdThread, string value)
        {
            InterpreterVariable ret = null;
            
            return ret;
        }


        /// <summary>Returns the specified global variable, if such a variable exists.</summary>
        /// <param name="varName">Name of the global variable to be returned.</param>
        public virtual InterpreterVariable GetGlobalVariable(string varName)
        {
            lock (Lock)
            {
                if (GlobalFrame.IsVariableDefined(varName))
                    return GlobalFrame[varName];
                else
                    return null;
            }
        }


        /// <summary>Returns value of the specified global variable, if such a global variable exists.</summary>
        /// <param name="varName">Name of the global variable whose value is returned.</param>
        public virtual string GetGlobalVariableValue(string varName)
        {
            lock (Lock)
            {
                return GlobalFrame.GetVariableValue(varName);
            }
        }

        /// <summary>Returns value of the specified global variable, if such a global variable exists.</summary>
        /// <param name="varName">Name of the variable.</param>
        /// <value>Value that is assigned to the variable.</value>
        public virtual void SetGetGlobalVariableValue(string varName, string varValue)
        {
            lock (Lock)
            {
                GlobalFrame.SetVariableValue(varName, varValue);
            }
        }















        /// <summary>Returns the value of the specified variable of the current command line interpreter.
        /// null is returned if the specified variable does not exist.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="varName">Name of the variable.</param>
        public virtual string GetVariableValue(CommandThread cmdThread, string varName)
        {
            try
            {
                lock (Lock)
                {
                    return GlobalFrame[varName].StringValue;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Variable \"" + varName + "\" could not be obtained: " + Environment.NewLine
                    + "  " + ex.Message, ex);
            }
        }

        /// <summary>Sets the specified variable to the specified value.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="varName">Name of the variable to be set.</param>
        /// <param name="value">Value that is assigned to the variable.</param>
        /// <returns>New value of the variable.</returns>
        public virtual string SetVariableValue(CommandThread cmdThread, string varName, string value)
        {
            try
            {
                lock (this.Lock)
                {
                    GlobalFrame.SetVariableValue(varName, value);
                }
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Variable " + varName + " could not be set: " + Environment.NewLine
                    + "  " + ex.Message, ex);
            }
            return value;
            //lock (Lock)
            //{
            //    if (string.IsNullOrEmpty(varName))
            //        throw new ArgumentException("Interpreter variable to be set not specified.");
            //    _variables[varName] = value;
            //    return value;
            //}
        }

        /// <summary>Clears (removes) the specified variable.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="varName">Name of the variable to be cleared.</param>
        /// <returns>null.</returns>
        public virtual string ClearVariable(CommandThread cmdThread, string varName)
        {
            try
            {
                lock (Lock)
                {
                    GlobalFrame.RemoveVariable(varName);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Variable " + varName + " could not be cleared: " + Environment.NewLine
                    + "  " + ex.Message, ex);
            }
            return null;
            //lock (Lock)
            //{
            //    if (string.IsNullOrEmpty(varName))
            //        throw new ArgumentException("Interpreter variable to be removed not specified.");
            //    _variables.Remove(varName);
            //    return null;
            //}
        }


        /// <summary>Prints the specified variable.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="varName">Name of the variable to be cleared.</param>
        /// <returns>null.</returns>
        public virtual string PrintVariable(CommandThread cmdThread, string varName)
        {
            Console.WriteLine("  " + varName + " = " + GetVariableValue(cmdThread, varName));
            return null;
        }


        // Code blocks, branches and loops: entry and exit commands.

        public readonly string Command_Block = "Block";

        public readonly string Command_EndBlock = "EndBlock";

        public readonly string Command_BeginCalc = "BeginCalc";

        public readonly string Command_EndCalc = "EndCalc";

        // TODO: If / Else / EndIf block is problematic from the point of block entry/exit . Solve this problem!
        // PROPOSAL: This can be solved by different behavior of the CheckForBlockEnterOrExitCommand and the IsBlokExitQuietCommand
        // functions in case of the If / Else etc. blocks. 
        // For example:
        // In If block, only If and EndIf commands would cause the level to be changed; however, also the 
        // ElseIf and Else functions would return true when potential block exit is checked. - or something like this.


        public readonly string Command_If = "If";

        public readonly string Command_ElseIf = "ElseIf";

        public readonly string Command_Else = "Else";

        public readonly string Command_EndIf = "EndIf";

        public readonly string Command_While = "While";

        public readonly string Command_EndWhile = "EndWhile";


        /// <summary>Adds block enter/exit commands for the "Block" block to the specified stack frame.
        /// <para>This method should be executed by the appropriate block enter command for the specific
        /// kind of code blocks.</para></summary>
        /// <param name="frame">Stack frame on which the block entry/exit commands are added.</param>
        protected virtual void AddEnterExitCommands_Block(CommandStackFrame frame)
        {
            frame.AddBlockEnterCommands(Command_Block);
            frame.AddBlockExitCommands(Command_EndBlock);
        }

        /// <summary>Adds block enter/exit commands for the "BeginCalc" block to the specified stack frame.
        /// <para>This method should be executed by the appropriate block enter command for the specific
        /// kind of code blocks.</para></summary>
        /// <param name="frame">Stack frame on which the block entry/exit commands are added.</param>
        protected virtual void AddEnterExitCommands_BeginCalc(CommandStackFrame frame)
        {
            frame.AddBlockEnterCommands(Command_BeginCalc);
            frame.AddBlockExitCommands(Command_EndCalc);
        }

        /// <summary>Adds block enter/exit commands for the "If" block to the specified stack frame.
        /// <para>This method should be executed by the appropriate block enter command for the specific
        /// kind of code blocks.</para></summary>
        /// <param name="frame">Stack frame on which the block entry/exit commands are added.</param>
        protected virtual void AddEnterExitCommands_If(CommandStackFrame frame)
        {
            frame.AddBlockEnterCommands(Command_If);
            frame.AddBlockExitCommands(Command_EndIf);
            frame.AddBlockExitCommandsNoLevelEffect(Command_ElseIf, Command_Else);
        }

        /// <summary>Adds block enter/exit commands for the "ElseIf" block to the specified stack frame.
        /// <para>This method should be executed by the appropriate block enter command for the specific
        /// kind of code blocks.</para></summary>
        /// <param name="frame">Stack frame on which the block entry/exit commands are added.</param>
        protected virtual void AddEnterExitCommands_ElseIf(CommandStackFrame frame)
        {
            frame.AddBlockEnterCommands(Command_If);
            frame.AddBlockExitCommands(Command_EndIf);
            frame.AddBlockExitCommandsNoLevelEffect(Command_ElseIf, Command_Else);
        }

        /// <summary>Adds block enter/exit commands for the "ElseIf" block to the specified stack frame.
        /// <para>This method should be executed by the appropriate block enter command for the specific
        /// kind of code blocks.</para></summary>
        /// <param name="frame">Stack frame on which the block entry/exit commands are added.</param>
        protected virtual void AddEnterExitCommands_Else(CommandStackFrame frame)
        {
            frame.AddBlockEnterCommands(Command_If);
            frame.AddBlockExitCommands(Command_EndIf);
            // Remarks, ElseIf is added below just for any casse, although the Else block should never 
            // be terminated by the ElseIf block (while the opposite can happen):
            frame.AddBlockExitCommandsNoLevelEffect(Command_ElseIf, Command_Else);
        }

        /// <summary>Adds block enter/exit commands for the "While" block to the specified stack frame.
        /// <para>This method should be executed by the appropriate block enter command for the specific
        /// kind of code blocks.</para></summary>
        /// <param name="frame">Stack frame on which the block entry/exit commands are added.</param>
        protected virtual void AddEnterExitCommands_While(CommandStackFrame frame)
        {
            frame.AddBlockEnterCommands(Command_While);
            frame.AddBlockExitCommands(Command_EndWhile);
        }


        /// <summary>Enters a new code block. A new stack frame is added where code is executed, such that the 
        /// embedded code block has isolated local variables.
        /// <para>Must be paired with <see cref="ExitBlock"/>.</para></summary>
        /// <param name="cmdThread">Command thread on which code is exected.</param>
        /// <param name="executeCommands">Whether commands should be exected immediately as they are encountered.
        /// <para>If false then execution is deferred until end of block is reached.</para></param>
        /// <param name="saveCommands">Whether commands hould be saved. Mainly used for testing purposes, but if 
        /// <paramref name="executeCommands"/> this flag is switched on, so that commands are collected and can be executed
        /// when the end of the block is reached.</param>
        public virtual void EnterBlock(CommandThread cmdThread, 
            bool executeCommands = true, bool saveCommands = false)
        {
            cmdThread.WasBlockEnterCommand = true;
            CommandStackFrame parentFrame = cmdThread.TopFrame;
            parentFrame.ReturnedValue = null;
            CommandStackFrame frame = cmdThread.AddFrame(CodeBlockType.Block);
            frame.SuppressInteractive = parentFrame.SuppressInteractive;  // inherits from parent
            // Add block enter / exit commands:
            AddEnterExitCommands_Block(frame);
            frame.DoExecuteCommands = executeCommands;
            frame.DoSaveCommands = saveCommands;
            if (!parentFrame.DoExecuteCommands)
            {
                frame.DoExecuteCommands = false;
                if (executeCommands && OutputLevel >= 1)
                    Console.WriteLine(Environment.NewLine + "DoExecuteCommands: DoExecute commands overridden, set to false." + Environment.NewLine);
            }
            if (parentFrame.DoSaveCommands)
            {
                frame.DoSaveCommands = true;
                if (!saveCommands && OutputLevel >= 1)
                    Console.WriteLine(Environment.NewLine + "EnterBlock: DoSaveCommands commands overridden, set to false." + Environment.NewLine);
            }
            if (frame.LastCommandLine != null)
                frame.BlockCommanddLine = frame.LastCommandLine;
        }

        /// <summary>Exits the current code block. 
        /// <para>Must be paired with <see cref="EnterBlock"/>.</para></summary>
        /// <param name="cmdThread">Command execution thread where execution occurs.</param>
        public virtual string ExitBlock(CommandThread cmdThread)
        {
            cmdThread.WasBlockExitCommand = true;
            CommandStackFrame frame = cmdThread.TopFrame;
            if (!frame.DoExecuteCommands && frame.DoSaveCommands)
            {
                var parentFrame = frame.GetParentStackFrame();
                if (parentFrame == null)
                    throw new InvalidOperationException("The current frame does not have a parent (level: " + frame.StackLevel 
                        + ", type: " + frame.BlockType + ".");
                if (frame.DoSaveCommands)
                {
                    // Remove the last stored command (because this would be the command that exits this block,
                    // and we only want commands that purely belong to the block):
                    int lastCommand = frame.CommandLines.Count - 1;
                    if (lastCommand >= 0)
                        frame.CommandLines.RemoveAt(lastCommand);
                }
                if (parentFrame.DoExecuteCommands)
                {
                    // Delayed execution of commands at the end of the block:
                    frame.DoExecuteCommands = true;
                    frame.DoSaveCommands = false;
                    Run(cmdThread, frame.CommandLines);
                    frame.CommandLines.Clear();
                    frame.ConditionExpression = null;
                    frame.BlockCommanddLine = null;
                }
            }
            CommandStackFrame removedFrame = cmdThread.RemoveFrame();
            // check:
            if (!object.ReferenceEquals(frame, removedFrame))
                throw new InvalidOperationException("Stack frame inconsistency when exiting the block.");
            // Save returned value of the completed block to the parent frame (which is now the top frame):
            cmdThread.TopFrame.ReturnedValue = frame.ReturnedValue;
            return frame.ReturnedValue;
        }

        /// <summary>Enters a new JavaScript calculator's code block. A new stack frame is added, such that the 
        /// embedded code block collects command lines independently of the containing blocks, and lines can
        /// be later concatenated into a single string and sent to JavaScript evaluator when the block exits.
        /// <para>Must be paired with <see cref="ExitJsBlock"/>.</para>
        /// <para>Important: JavaScript code block is executed in the parent stack frame (i.e. the frame that was executiing before the
        /// JavaScript block entered).</para></summary>
        /// <param name="cmdThread">Command thread on which code is exected.</param>
        public virtual void EnterJsBlock(CommandThread cmdThread)
        {
            cmdThread.WasBlockEnterCommand = true;
            Console.WriteLine(Environment.NewLine + Environment.NewLine 
                + "Entering the JavaScript code block. " + Environment.NewLine);

            CommandStackFrame parentFrame = cmdThread.TopFrame;
            //parentFrame.ReturnedValue = null;
            CommandStackFrame frame = cmdThread.AddFrame(CodeBlockType.Block);
            frame.SuppressInteractive = parentFrame.SuppressInteractive;  // inherits from parent
            // Add block enter / exit commands:
            AddEnterExitCommands_BeginCalc(frame);

            // Commands within the block will not be executed, but mast be stored in order to evaluate them
            // after the block exits by the internal JavaScript evaluator:
            frame.DoExecuteCommands = false;
            frame.DoSaveCommands = true;
            // Store (for eventual inspection) the commandline that introduced the block:
            if (frame.LastCommandLine != null)
                frame.BlockCommanddLine = frame.LastCommandLine;
        }

        /// <summary>Exits the current JavaScript code block, and sends the JavaScrit code contained within the block 
        /// (consisting of concatenated command lines entered within the block) to the JavaScript evaluator for execution.
        /// <para>Must be paired with <see cref="EnterJsBlock"/>.</para>
        /// <para>JavaScript code is executed in the parent stack frame (i.e. the frame on which code block was called).</para></summary>
        /// <param name="cmdThread">Command execution thread where execution occurs.</param>
        public virtual string ExitJsBlock(CommandThread cmdThread)
        {
            cmdThread.WasBlockExitCommand = true;
            string ret = null;
            // Concatenate the collected commandlines within the code block for execution with the JavaScript evaluator:
            CommandStackFrame frame = cmdThread.TopFrame;
            if (frame.DoSaveCommands)
            {
                // Remove the last stored command (because this would be the command that exits this block,
                // and we only want commands that purely belong to the block):
                int lastCommand = frame.CommandLines.Count - 1;
                if (lastCommand >= 0)
                    frame.CommandLines.RemoveAt(lastCommand);
            }
            StringBuilder sb = new StringBuilder();
            foreach (string command in frame.CommandLines)
                sb.AppendLine(command);
            string codeBlock = sb.ToString();

            if (OutputLevel >= 3)
            {
                Console.WriteLine(Environment.NewLine + "End of the JavaScript code block. " + Environment.NewLine
                    + "Code to be executed:" + Environment.NewLine
                    + "=====================================================================" + Environment.NewLine
                    + codeBlock // + Environment.NewLine
                    + "---------------------------------------------------------------------" + Environment.NewLine
                    + Environment.NewLine);
            }
            // Exit the current stack frame such that code is exected in the containing frame:
            CommandStackFrame removedFrame = cmdThread.RemoveFrame();
            // Check stack frames consistency:
            if (!object.ReferenceEquals(frame, removedFrame))
                throw new InvalidOperationException("Stack frame inconsistency when exiting the block.");
            // Execute the code in the JavaScript evaluator:
            ret = EvaluateJs(codeBlock);
            // Save returned value of the completed block to the parent frame (which is now the top frame):
            // cmdThread.TopFrame.ReturnedValue = ret;
            return ret;
        }

        /// <summary>Enters the If block.</summary>
        /// <param name="cmdThread">Command thread where commands are executed.</param>
        /// <param name="condition">Value of the condition of the if branch.</param>
        public virtual void EnterIf(CommandThread cmdThread, bool condition)
        {
            cmdThread.WasBlockEnterCommand = true;
            CommandStackFrame parentFrame = cmdThread.TopFrame;
            parentFrame.ReturnedValue = null;
            CommandStackFrame frame = cmdThread.AddFrame(CodeBlockType.If);
            frame.SuppressInteractive = parentFrame.SuppressInteractive;  // inherits from parent
            // Add block enter / exit commands:
            AddEnterExitCommands_If(frame);

            // frame.DoExecuteCommands = executeCommands;
            // frame.DoSaveCommands = saveCommands;
            if (parentFrame.DoSaveCommands)
            {
                frame.DoSaveCommands = true;
            }
            if (!parentFrame.DoExecuteCommands)
            {
                frame.DoExecuteCommands = false;
            } else
            {
                if (condition)
                {
                    frame.DoExecuteCommands = true;
                    frame.WasBranchAlreadyExecuted = true;
                }
                else
                    frame.DoExecuteCommands = false;
            }
            if (frame.LastCommandLine != null)
                frame.BlockCommanddLine = frame.LastCommandLine;
        }

        /// <summary>Enters the ElseIf block.</summary>
        /// <param name="cmdThread">Command thread where commands are executed.</param>
        /// <param name="condition">Value of the condition of the branch.</param>
        public virtual void EnterElseIf(CommandThread cmdThread, bool condition)
        {
            cmdThread.WasBlockEnterCommand = true;
            cmdThread.WasBlockExitCommand = true;
            CommandStackFrame previousFrame = cmdThread.RemoveFrame();
            CommandStackFrame parentFrame = cmdThread.TopFrame;
            if (previousFrame.ReturnedValue != null)
                parentFrame.ReturnedValue = previousFrame.ReturnedValue;
            CommandStackFrame frame = cmdThread.AddFrame(CodeBlockType.If);
            frame.SuppressInteractive = parentFrame.SuppressInteractive;  // inherits from parent
            frame.ReturnedValue = previousFrame.ReturnedValue;
            // Add block enter / exit commands:
            AddEnterExitCommands_ElseIf(frame);
            frame.WasBranchAlreadyExecuted = previousFrame.WasBranchAlreadyExecuted;  // transfer this information to the current block
            if (parentFrame.DoSaveCommands)
            {
                frame.DoSaveCommands = true;
            }
            if (!parentFrame.DoExecuteCommands)
            {
                frame.DoExecuteCommands = false;
            } else
            {
                frame.DoExecuteCommands = false;
                if (!frame.WasBranchAlreadyExecuted)
                {
                    // Any of the related if branches has not been executed yet, check condition and execute if appropriate:
                    if (condition)
                    {
                        frame.DoExecuteCommands = true;
                        frame.WasBranchAlreadyExecuted = true;
                    }
                }
            }
            if (frame.LastCommandLine != null)
                frame.BlockCommanddLine = frame.LastCommandLine;
        }

        /// <summary>Enters the ElseIf block.</summary>
        /// <param name="cmdThread">Command thread where commands are executed.</param>
        public virtual void EnterElse(CommandThread cmdThread)
        {
            cmdThread.WasBlockEnterCommand = true;
            cmdThread.WasBlockExitCommand = true;
            CommandStackFrame previousFrame = cmdThread.RemoveFrame();
            CommandStackFrame parentFrame = cmdThread.TopFrame;
            if (previousFrame.ReturnedValue != null)
                parentFrame.ReturnedValue = previousFrame.ReturnedValue;
            CommandStackFrame frame = cmdThread.AddFrame(CodeBlockType.If);
            frame.SuppressInteractive = parentFrame.SuppressInteractive;  // inherits from parent
            frame.ReturnedValue = previousFrame.ReturnedValue;
            // Add block enter / exit commands:
            AddEnterExitCommands_Else(frame);
            frame.WasBranchAlreadyExecuted = previousFrame.WasBranchAlreadyExecuted;  // transfer this information to the current block
            if (parentFrame.DoSaveCommands)
            {
                frame.DoSaveCommands = true;
            }
            if (!parentFrame.DoExecuteCommands)
            {
                frame.DoExecuteCommands = false;  // unnecessary? 
            } else
            {
                frame.DoExecuteCommands = false;
                if (!frame.WasBranchAlreadyExecuted)
                {
                    // Any of the related if branches has not been executed yet, execute the else block:
                    frame.DoExecuteCommands = true;
                    frame.WasBranchAlreadyExecuted = true;
                }
            }
            if (frame.LastCommandLine != null)
                frame.BlockCommanddLine = frame.LastCommandLine;
        }

        /// <summary>Enters the EndIf block.</summary>
        /// <param name="cmdThread">Command thread where commands are executed.</param>
        public virtual string ExitIf(CommandThread cmdThread)
        {
            cmdThread.WasBlockExitCommand = true;
            CommandStackFrame previousFrame = cmdThread.RemoveFrame();
            CommandStackFrame parentFrame = cmdThread.TopFrame;
            if (previousFrame.ReturnedValue != null)
                parentFrame.ReturnedValue = previousFrame.ReturnedValue;
            //// check:
            //if (!object.ReferenceEquals(frame, previousFrame))
            //    throw new InvalidOperationException("Stack frame inconsistency when exiting the block.");
            // Return the value returned by the last command executed in If statement:
            return parentFrame.ReturnedValue;
        }


        /// <summary>Enters the While block.</summary>
        /// <param name="cmdThread">Command thread where commands are executed.</param>
        /// <param name="conditionString">Value of the condition of the while loop.</param>
        public virtual void EnterWhile(CommandThread cmdThread, string conditionString)
        {
            cmdThread.WasBlockEnterCommand = true;
            CommandStackFrame parentFrame = cmdThread.TopFrame;
            CommandStackFrame frame = cmdThread.AddFrame(CodeBlockType.While);
            frame.SuppressInteractive = parentFrame.SuppressInteractive;  // inherits from parent
            // Add block enter / exit commands:
            AddEnterExitCommands_While(frame);

            frame.ConditionExpression = conditionString;
            frame.DoExecuteCommands = false;
            frame.DoSaveCommands = true;
            frame.SuppressInteractive = true;
        }


        /// <summary>Exits the current JavaScript code block, and sends the JavaScrit code contained within the block 
        /// (consisting of concatenated command lines entered within the block) to the JavaScript evaluator for execution.
        /// <para>Must be paired with <see cref="EnterJsBlock"/>.</para>
        /// <para>JavaScript code is executed in the parent stack frame (i.e. the frame on which code block was called).</para></summary>
        /// <param name="cmdThread">Command execution thread where execution occurs.</param>
        public virtual string ExitWhile(CommandThread cmdThread)
        {
            cmdThread.WasBlockExitCommand = true;
            string ret = null;
            // Concatenate the collected commandlines within the code block for execution with the JavaScript evaluator:
            CommandStackFrame frame = cmdThread.TopFrame;
            bool storeDoExecute = frame.DoExecuteCommands;
            bool storeDoSave = frame.DoSaveCommands;
            // bool storeOppressInteractive = frame.OppressInteractive;
            if (storeDoSave)
            {
                // Remove the last stored command (because this would be the command that exits this block,
                // and we only want commands that purely belong to the block):
                int lastCommand = frame.CommandLines.Count - 1;
                if (lastCommand >= 0)
                    frame.CommandLines.RemoveAt(lastCommand);
            }
            // Execute stored commands in a loop where condition is checked in each iteration:
            frame.DoExecuteCommands = true;
            frame.DoSaveCommands = false;
            bool condition = false;
            string conditionString = frame.ConditionExpression;
            string evaluatedContidion = EvaluateJs(conditionString);
            bool parsed = Util.TryParseBoolean(evaluatedContidion, ref condition);
            // Store commands because stack frames may change within the loop:
            string[] commands = frame.CommandLines.ToArray();
            int numCommands = commands.Length;
            while (condition)
            {
                for (int i = 0; i < numCommands; ++i)
                {
                    ret = Run(cmdThread, commands[i]);
                }
                // Re-evaluate condition in order to enter a new loop iteration if this is appropriate:
                evaluatedContidion = EvaluateJs(conditionString);
                parsed = Util.TryParseBoolean(evaluatedContidion, ref condition);
            }

            //Console.WriteLine(Environment.NewLine + "End of the JavaScript code block. " + Environment.NewLine
            //    + "Code to be executed:" + Environment.NewLine
            //    + "=====================================================================" + Environment.NewLine
            //    + codeBlock // + Environment.NewLine
            //    + "---------------------------------------------------------------------" + Environment.NewLine
            //    + Environment.NewLine);
            
            // Restore saved flags:
            frame.DoExecuteCommands = storeDoExecute;
            frame.DoSaveCommands = storeDoSave;
            frame.SuppressInteractive = false;

            // Exit the current stack frame such that code is exected in the containing frame:
            CommandStackFrame removedFrame = cmdThread.RemoveFrame();
            // Check stack frames consistency:
            if (!object.ReferenceEquals(frame, removedFrame))
                throw new InvalidOperationException("Stack frame inconsistency when exiting the block.");
            // Save returned value of the completed block to the parent frame (which is now the top frame):
            cmdThread.TopFrame.ReturnedValue = ret;
            return ret;
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
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="inputFilePath">Path to the file containing commands.</param>
        /// <returns>Return value of the last command.</returns>
        public virtual string RunFile(CommandThread cmdThread, string filePath)
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
                        ret = Run(cmdThread, line);
                        //if (!string.IsNullOrEmpty(line))
                        //{
                        //    string[] commandLineSplit = GetArguments(line);
                        //    ret = Run(cmdThread, commandLineSplit);
                        //}
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        public virtual string RunInteractive(CommandThread cmdThread)
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
                CommandStackFrame frame = cmdThread.TopFrame;  // must stand here to capture the current stack frame every time
                if (string.IsNullOrEmpty(line) && !frame.SuppressInteractive && !cmdThread.SuppressInteractive)
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
                        ret = Run(cmdThread, line);
                        //string[] commandLineSplit = GetArguments(line);
                        //ret = this.Run(cmdThread, commandLineSplit);
                        if (cmdThread.WasCommandExecuted  && !cmdThread.WasBlockEnterCommand)
                        {
                            string retOutput = ret;
                            if (retOutput == null)
                                retOutput = "null";
                            else if (retOutput == "")
                                retOutput = "\"\"";
                            if (!frame.SuppressInteractive && !cmdThread.SuppressInteractive)
                            {
                                Console.WriteLine("  = " + retOutput);
                            }
                        }
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
                        //if (!string.IsNullOrEmpty(ReturnedString))
                        //{
                        //    Console.WriteLine("  = " + ReturnedString);
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

        protected ExpressionEvaluatorJs _evalutorJs;

        /// <summary>Expression evaluator used by the current </summary>
        public ExpressionEvaluatorJs EvaluatorJs
        {
            get
            {
                lock (Lock)
                {
                    if (_evalutorJs == null)
                    {
                        _evalutorJs = new ExpressionEvaluatorJs();
                    }
                    return _evalutorJs;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    _evalutorJs = value;
                }
            }
        }

        /// <summary>Runs interpreter's JavaScript expression evaluator interactively.</summary>
        public string EvaluateJsInteractive()
        {
            EvaluatorJs.CommandLine();
            return null;
        }

        /// <summary>Evaluates the specified block of code by the internal JavaScript evaluator.</summary>
        /// <param name="codeBlock">Block of JavaScript code that is evaluateed (executed).</param>
        /// <returns>The returned value of the evaluated code (or null if the code does not return anything).</returns>
        public string EvaluateJs(string codeBlock)
        { 
            return EvaluatorJs.Execute(codeBlock); 
        }

        /// <summary>Evaluates a JavaScript expression obtained by merging elements of the array parameter
        /// <paramref name="args"/>. These usually represent parts of the code block to evaluate, obtained through
        /// command arguments or as commandlines read (but not executed) by the commandline interpreter.</summary>
        /// <param name="args">An array of strings that are concatenated in order to form the evaluated expression.</param>
        public string EvaluateJs(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            if (args != null)
            {
                for (int i = 0; i < args.Length; ++i)
                    sb.Append(args[i] + " ");
            }
            return EvaluateJs(sb.ToString());
        }


        /// <summary>Returns an array of installed commands.</summary>
        /// <remarks>The returned array is created anew and command names are copied to it from a collection of keys
        /// of a sorted dictionary (type <see cref="SortedDictionary{T, T}"/>).</remarks>
        public string[] GetCommands()
        {
            lock (Lock)
            {
                SortedDictionary<string, ApplicationCommandDelegateMt>.KeyCollection keys = _commandsMt.Keys;
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



        /// <summary>Obtains and stores the delegate that is used for execution of the specified command, and
        /// retrns an iindicator ehether the command is installed on the interpreter or not.
        /// <para>Interpreter seinsitivity is handled by the method.</para></summary>
        /// <param name="commandName">Name of the command for which execution delegate is obtained.</param>
        /// <param name="appDelegate">Reference to the variable where this function stores the obtained command
        /// execution delegate.</param>
        /// <returns>True if the specified command is installed on the current interpreter, false if not.</returns>
        protected virtual bool GetCommandDelegate(string commandName, ref ApplicationCommandDelegateMt appDelegate)
        {
            lock (Lock)
            {
                bool ret;
                appDelegate = null;
                if (!_caseSensitive)
                    commandName = commandName.ToLower();
                ret = _commandsMt.ContainsKey(commandName);
                if (ret)
                    appDelegate = _commandsMt[commandName];
                return ret;
            }
        }


        /// <summary>Returns true if the specified command is installed on the interpreter, false if not.
        /// <para>Case sensitivity of the interpreter is treated appropriately.</para></summary>
        /// <param name="commandName">Name of the command that is checked.</param>
        /// <returns>True if the specified command is installed on the current interpreter, false if not.</returns>
        public bool ContainsCommand(string commandName)
        {
            ApplicationCommandDelegateMt appDelegate = null;
            return GetCommandDelegate(commandName, ref appDelegate);
        }


        
        /// <summary>Runs command several times where the first argument is number of repetitions, second argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.
        /// <para>The interpreter's output level is used.</para></summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunRepeat(CommandThread cmdThread, string[] args)
        {
            return RunRepeat(cmdThread, OutputLevel, args);
        }
        
        /// <summary>Runs command several times where the first argument is number of repetitions, second argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.
        /// <para>Output level 3 is used, such that all information is output to console.</para></summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunRepeatVerbose(CommandThread cmdThread, string[] args)
        {
            return RunRepeat(cmdThread, 3, args);
        }
        
        /// <summary>Runs command several times where the first argument is number of repetitions, second argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.
        /// <para>Output level 0 is used, such that no information is output to console.</para></summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunRepeatSilent(CommandThread cmdThread, string[] args)
        {
            return RunRepeat(cmdThread, 0, args);
        }

        /// <summary>Runs command several times where the first argument is number of repetitions, second argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.
        /// <para>Output level is defined by the first argument. Level 0 means no output, level 1 means that summary is written to 
        /// the console, and level e means that a note is printed before and afterr each repetition starts.</para></summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunRepeatSpecificOutputLevel(CommandThread cmdThread, string[] args)
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
            return RunRepeat(cmdThread, outputLevel, arguments.ToArray());
        }


        /// <summary>Runs command several times where the first argument is number of repetitions, second argument is command name.
        /// Extracts command name and runs the corresponding command delegate. Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="outputLevel">Level of output of the command.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunRepeat(CommandThread cmdThread, int outputLevel, string[] args)
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
                    StopWatch1 t = new StopWatch1();
                    int threadId = Thread.CurrentThread.GetHashCode();
                    for (int i = 1; i <= numRepetitions; ++i)
                    {
                        if (outputLevel >= 2)
                        {
                            Console.WriteLine(Environment.NewLine + "Repeatively running command "
                                + i + " / " + numRepetitions + " (thread " + threadId + ") ...");
                        }
                        t.Start();
                        ret = ret + " " + Run(cmdThread, cmdName, cmdArgs);
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
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="outputLevel">Level of output of the command.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunTryCatch(CommandThread cmdThread, int outputLevel, string[] args)
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
                        ret = Run(cmdThread, cmdName, cmdArgs);
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

        /// <summary>Runs a set of command by the current interpreter.</summary>
        /// <param name="cmdThread">Interpreter's thread where commands are executed.</param>
        /// <param name="commandLines">List of comands, in raw string form, to be executed.</param>
        public virtual void Run(CommandThread cmdThread, List<string> commandLines)
        {
            if (commandLines != null)
            {
                int num = commandLines.Count;
                for (int i = 0; i < num; ++i)
                    Run(cmdThread, commandLines[i]);
            }
        }

        
        ///// <summary>Runs the specified command with specified name, installed on the current application object, without any
        ///// modifications of the command arguments.</summary>
        ///// <param name="commandName">Command name.</param>
        ///// <param name="commandArguments">Command arguments.</param>
        ///// <remarks>This method should not be overriden, but the <see cref="Run(CommandThread, string, string[])"/> method can be, e.g. in order to 
        ///// perform some argument or command name transformations.</remarks>
        //public string RunWithoutModificationsMainThread(string commandName, params string[] commandArguments)
        //{
        //    return RunWithoutModifications(this.MainThread, commandName, commandArguments);
        //}


        /// <summary>Runs the specified command with specified name, installed on the current application object, without any
        /// modifications of the command arguments.</summary>
        /// <param name="cmdThread">Interpreter's command thread where command is executed.</param>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <remarks>This method should not be overriden, but the <see cref="Run(CommandThread, string, string[])"/> method can be, e.g. in order to 
        /// perform some argument or command name transformations.</remarks>
        public string RunWithoutModifications(CommandThread cmdThread, string commandName, params string[] commandArguments)
        {
            string ret = null;
            cmdThread.WasCommandExecuted = false;
            CommandStackFrame frame = cmdThread.TopFrame;
            bool doExecuteFurther = true;
            if (!frame.DoExecuteCommands)
            {
                // Execution is switched off. Control will still be passed to further evaluation function in the case
                // that commandline can represent one of the possible block enter or exit commands for the current
                // kind of the code block. Thi is in order to execute the appropriate block exit command, even
                // if the command execution is crrently switched off.
                doExecuteFurther = false;
                // Check if the command could eventually represent a block enter or block exit command (without side effects):
                if (frame.CheckForBlockEnterOrExitCommand(commandName, justCheck: true, isOnlyCommandName: true))
                {
                    // Yes, now check for real (with possible side effects on frame), but only if the command is actually
                    // installed on the interpreter (this also safeguards for mistakes that may occur due to case sensitivity
                    // of the interpreter):
                    if (this.ContainsCommand(commandName))
                    {
                        // Test condition before CheckForBlockEnterOrExitCommand, because that one will already
                        // reduce the level! We still want to have the same level as before.
                        bool isBlockExitQuiet = frame.IsBlockExitQuietCommand(commandName, isOnlyCommandName: true);
                        if (frame.CheckForBlockEnterOrExitCommand(commandName, justCheck: false, isOnlyCommandName: true))
                            if (isBlockExitQuiet)
                            {
                                // Command will be executed in spite of the fact that execution is switched off; This is because
                                // the command represents the exit block command for the bolck that actually switched off execution.
                                doExecuteFurther = true;
                            }
                    }
                }
            }
            if (doExecuteFurther)  
            {
                ApplicationCommandDelegateMt appDelegate = null;
                lock (Lock)
                {
                    //if (string.IsNullOrEmpty(commandName))
                    //    throw new ArgumentException("Command name is not specified.");
                    // Perform substitution of variables with their values in command name and argumnets:
                    commandName = SubstituteVariableOrExpressionReference(MainThread, commandName);
                    if (commandArguments != null)
                    {
                        for (int i = 0; i < commandArguments.Length; ++i)
                        {
                            commandArguments[i] = SubstituteVariableOrExpressionReference(cmdThread, commandArguments[i]);
                        }
                    }


                    //{
                    //    bool substituteArgs = false;
                    //    if (commandArguments != null)
                    //    {
                    //        for (int i = 0; i < commandArguments.Length; ++i)
                    //        {
                    //            if (IsVariableReference(MainThread, commandArguments[i]))
                    //            {
                    //                substituteArgs = true;
                    //                break;
                    //            }
                    //        }
                    //        if (substituteArgs)
                    //        {
                    //            string[] argsSubstituted = new string[commandArguments.Length];
                    //            for (int i = 0; i < commandArguments.Length; ++i)
                    //                argsSubstituted[i] = SubstituteVariableReference(MainThread, commandArguments[i]);
                    //            commandArguments = argsSubstituted;
                    //        }
                    //    }
                    //}

                    if (string.IsNullOrEmpty(commandName))
                    {
                        // Empty string will just be ignored.
                    }
                    else
                    {
                        bool containsCommand = GetCommandDelegate(commandName, ref appDelegate);
                        if (!containsCommand)
                            throw new ArgumentException("Interpreter does not contain the following command: \"" + commandName + "\".");
                        if (appDelegate == null)
                        {
                            throw new InvalidOperationException("Command not properly installed on the interpreter: \"" + commandName + "\".");
                        }
                        else
                        {
                            ret = appDelegate(cmdThread, commandName, commandArguments);
                            cmdThread.WasCommandExecuted = true;
                            frame.ReturnedValue = ret;
                        }
                    }
                }  // lock
            }
            return ret;
        }




        // TODO:
        // Check if you can remove the Run command below, and replace it by the called command, where 
        // simultaneously the called command is renamed to "Run" such that all the interfaces remaiin satisfied.

        /// <summary>Runs the specified command with specified name, installed on the current application object.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        public virtual string Run(CommandThread cmdThread, string commandName, params string[] commandArguments)
        {
            return RunWithoutModifications(cmdThread, commandName, commandArguments);
        }

        /// <summary>Runs command where the first argument is command name.
        /// Extracts command name and runs the corresponding command delegate.Before running it, arguments
        /// for the application delegate are extracted and then passed to the delegate.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public string Run(CommandThread cmdThread, string[] args)
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
                return RunWithoutModifications(cmdThread, cmdName, cmdArgs);
            }
        }

        protected string [] _emptyCommandLine = { null };


        protected string [] EmptyCommandLine { get { return _emptyCommandLine; } }

        /// <summary>Runs command that is specified as a single string, composed of command name and its argumens.
        /// Splits the command and runs it.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="commandLine">Commandline - string containing command and its arguments.</param>
        public string Run(CommandThread cmdThread, string commandLine)
        {
            string ret = null;
            cmdThread.WasCommandExecuted = false;
            CommandStackFrame frame = cmdThread.TopFrame;
            frame.LastCommandLine = commandLine;
            if (frame.DoSaveCommands)
                frame.CommandLines.Add(commandLine);
            bool doExecuteFurther = true;
            if (!frame.DoExecuteCommands)
            {
                // Execution is switched off. Control will still be passed to further evaluation function in the case
                // that commandline can represent one of the possible block enter or exit commands for the current
                // kind of the code block. Thi is in order to execute the appropriate block exit command, even
                // if the command execution is crrently switched off.
                doExecuteFurther = false;
                if (frame.CheckForBlockEnterOrExitCommand(commandLine, justCheck: true, isOnlyCommandName: false))
                    doExecuteFurther = true;
            }
            if (doExecuteFurther)  // don't bother if execution is switched off!
            {
                if (!string.IsNullOrEmpty(commandLine))
                {
                    string[] commandLineSplit;
                    try
                    {
                        commandLineSplit = GetArguments(commandLine);
                    }
                    catch { commandLineSplit = EmptyCommandLine; }
                    ret = Run(cmdThread, commandLineSplit);
                }
            }
            return ret;
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
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="args">Command arguments where the first argument is command name. The rest of the arguments
        /// are collected and passed to the command delegate.</param>
        public virtual string RunAsync(CommandThread cmdThread, 
            string[] args)
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
                    return RunAsync(cmdThread, cmdName, cmdArgs);
                }
            }
        }


        /// <summary>Runs the command with specified name (installed on the current interpreter object) asynchronously.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="commandName">Command name.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <returns>ID of asynchronous run used to query results and whether command has completed, or -1 if a call was not
        /// launched (actually, an exception would be thrown in this case).</returns>
        public virtual string RunAsync(CommandThread cmdThread, 
            string commandName, params string[] commandArguments)
        {
            // TODO: use normal Run() instead of directly the command delegate!
            // TODO: Copy stack frames to the execution environment


            int asyncId = -1;
            ApplicationCommandDelegateMt appDelegate = null;
            lock (Lock)
            {
                //if (string.IsNullOrEmpty(commandName))
                //    throw new ArgumentException("Command name is not specified.");
                // Perform substitution of variables and calc. expressions with their values in command name and argumnets:
                commandName = SubstituteVariableOrExpressionReference(cmdThread, commandName);
                for (int i = 0; i < commandArguments.Length; ++i)
                {
                    commandArguments[i] = SubstituteVariableOrExpressionReference(cmdThread, commandArguments[i]);
                }

                //{
                //    bool substituteArgs = false;
                //    for (int i = 0; i < commandArguments.Length; ++i)
                //    {
                //        if (IsVariableReference(cmdThread, commandArguments[i]))
                //        {
                //            substituteArgs = true;
                //            break;
                //        }
                //    }
                //    if (substituteArgs)
                //    {
                //        string[] argsSubstituted = new string[commandArguments.Length];
                //        for (int i = 0; i < commandArguments.Length; ++i)
                //            argsSubstituted[i] = SubstituteVariableOrExpressionReference(cmdThread, commandArguments[i]);
                //        commandArguments = argsSubstituted;
                //    }
                //}
                if (!_caseSensitive)
                    commandName = commandName.ToLower();
                if (!_commandsMt.ContainsKey(commandName))
                    throw new ArgumentException("Interpreter does not contain the following command: " + commandName + ".");
                appDelegate = _commandsMt[commandName];
                if (appDelegate == null)
                {
                    throw new InvalidOperationException("Can not find command named " + commandName + ".");
                }
                else
                {
                    // return appDelegate(this, commandName, commandArguments);
                    IAsyncResult result = appDelegate.BeginInvoke(cmdThread, commandName, commandArguments, AsyncRunCallback, asyncId);
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
                        ApplicationCommandDelegateMt caller = (ApplicationCommandDelegateMt)result.AsyncDelegate;  // retrieve the delegate
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
                    ApplicationCommandDelegateMt caller = (ApplicationCommandDelegateMt)result.AsyncDelegate;  // retrieve the delegate
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


        List<CommandAdapterSingleThreaded> _stAdapters = new List<CommandAdapterSingleThreaded>();

        /// <summary>Adds command with the specified name.</summary>
        /// <param name="commandName">Name of the commant.</param>
        /// <param name="commandDelegate">Delegate that will be used to execute the command.</param>
        /// <remarks>After cleaning is finished, this method can be removed (also from the interface) because ther will be 
        /// no command functions that still use the old signature (with interpreter rather than thread as the first argument).</remarks>
        [Obsolete("Use the command that uses different function prototype (ApplicationCommandDelegate) where the first argument is CommandThread rather than Interpreter.")]
        public virtual void AddCommand(string commandName, ApplicationCommandDelegate commandDelegate)
        {
            CommandAdapterSingleThreaded atapterObj = new CommandAdapterSingleThreaded(commandDelegate, null);
            _stAdapters.Add(atapterObj);
            AddCommandMt(commandName, atapterObj.MultiThreadedApplication);


            //lock (Lock)
            //{
            //    if (string.IsNullOrEmpty(commandName))
            //        throw new ArgumentException("Command name not specified.");
            //    if (!_caseSensitive)
            //        commandName = commandName.ToLower();
            //    if (WarnCommandReplacement) if (_commandsMt.ContainsKey(commandName))
            //        {
            //            Console.WriteLine();
            //            Console.WriteLine("WARNING: Interpreter command redefined: " + commandName + ".");
            //            Console.WriteLine();
            //        }
            //    _commandsMt[commandName] = commandDelegate;
            //    //if (_commandsMt.ContainsKey(commandName))
            //    //    _commandsMt.Remove(commandName);
            //    //_commandsMt.Add(commandName, commandDelegate);
            //}
        }

        
        /// <summary>Adds command with the specified name.</summary>
        /// <param name="commandName">Name of the commant.</param>
        /// <param name="commandDelegate">Delegate that will be used to execute the command.</param>
        /// <remarks>WARNING: This should be removed later. It is only here such that when the signature of 
        /// the old command functions is changed, the AddCommand can act on them, so no new errors need
        /// to be corrected. In the future, this shoud be removed, and AddCommandMt should be renamed to
        /// AddCommand, and the old obsolete AddCommand should be also removed.</remarks>
        /// [Obsolete "Converge both methods with the same signature to only one method - AddCommandMt should be used and renamed."]
        public virtual void AddCommand(string commandName, ApplicationCommandDelegateMt commandDelegate)
        {
            AddCommandMt(commandName, commandDelegate);
        }

        /// <summary>Adds command with the specified name.</summary>
        /// <param name="commandName">Name of the commant.</param>
        /// <param name="commandDelegate">Delegate that will be used to execute the command.</param>
        public virtual void AddCommandMt(string commandName, ApplicationCommandDelegateMt commandDelegate)
        {
            lock (Lock)
            {
                if (string.IsNullOrEmpty(commandName))
                    throw new ArgumentException("Command name not specified.");
                if (!_caseSensitive)
                    commandName = commandName.ToLower();
                if (WarnCommandReplacement) if (_commandsMt.ContainsKey(commandName))
                    {
                        Console.WriteLine();
                        Console.WriteLine("WARNING: Interpreter command redefined: " + commandName + ".");
                        Console.WriteLine();
                    }
                _commandsMt[commandName] = commandDelegate;
                //if (_commandsMt.ContainsKey(commandName))
                //    _commandsMt.Remove(commandName);
                //_commandsMt.Add(commandName, commandDelegate);
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
                if (_commandsMt.ContainsKey(commandName))
                    _commandsMt.Remove(commandName);
            }
        }


        /// <summary>Removes all commands from the current interpreter.</summary>
        public virtual void RemoveAllCommands()
        {
            lock (Lock)
            {
                _commandsMt.Clear();
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
                        AddCommandMt(ModuleTestCommandName(moduleName), CmdModuleTestCommand);

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
            this.AddCommandMt(newCommandName, CmdRunLoadedScript);
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
        /// Sets the specified interpreter variable to the specified value. <br/>
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
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <param name="setCommandResult">If true then in any case variable value is set to the result of 
        /// an interpreter command (ven if command has no arguments). If false, then the first argument
        /// after variable name is considered an interpreter command only if additional arguments follow, 
        /// otherwise it is considered as a literal value that is assigned to the variable.</param>
        /// <returns>Value of the variable after setting.</returns>
        protected virtual string CmdSetVariableBase(CommandThread cmdThread, 
            string cmdName, string[] args, bool setCommandResult)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        if (setCommandResult)
                        {
                            Console.WriteLine();
                            Console.WriteLine(executableName + " " + cmdName + " varName cmd <arg1 arg2 arg3...> : sets the variable varName to the " + Environment.NewLine
                            + "  result of the command cmd with arguments arg1, arg2, etc. Arguments are OPTIONAL.");
                            Console.WriteLine();
                            return null;
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine(executableName + " " + cmdName + " varName value : sets the variable varName to value.");
                            Console.WriteLine();
                            Console.WriteLine(executableName + " " + cmdName + " varName cmd arg1 <arg2 arg3...> : sets the variable varName to the " + Environment.NewLine
                            + "  result of the command cmd with arguments arg1, arg2, etc. There must be AT LEAST ONE ARGUMENT.");
                            Console.WriteLine();
                            return null;
                        }
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires at lest two arguments.");
            else if (args.Length < 2)
                throw new ArgumentException(cmdName + " : Requires at lest two arguments.");
            else
            {
                string varName = null, value = null;
                varName = args[0];
                if (args.Length == 2 && !setCommandResult)
                    value = args[1];
                else
                {  // args.Length>2
                    string cmd = args[1];
                    string[] argsCmd = new string[args.Length - 2];
                    for (int i = 2; i < args.Length; ++i)
                        argsCmd[i - 2] = args[i];
                    value = Run(cmdThread, cmd, argsCmd);
                }
                ret = SetVariableValue(cmdThread, varName, value);
            }
            return ret;
        }

        /// <summary>Command.
        /// Sets the specified interpreter variable to the specified value. <br/>
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
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Value of the variable after setting.</returns>
        protected virtual string CmdSetVariable(CommandThread cmdThread, 
            string cmdName, string[] args)
        {
            return CmdSetVariableBase(cmdThread, cmdName, args, false /* setCommandResult */);

        }

        /// <summary>Command.
        /// Sets the specified interpreter variable to the result of the command (with arguments) that follow variable name. <br/>
        /// Usage: <br/>
        /// 2. Set the variable to the return value of the specified command: <br/>
        ///   SetVar varName command [arg1 [arg2] ...] <br/>
        ///     varName: name of the variable to be set. <br/>
        ///     command: command whose return value is the value to be assigned to the variable. <br/>
        ///     arg1: the first argument to the command (if any). <br/>
        ///     arg2: the second argument to the command (if any). <br/>
        ///     etc. <br/>
        /// </summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Value of the variable after setting.</returns>
        protected virtual string CmdSetVariableToCommandResult(CommandThread cmdThread, 
            string cmdName, string[] args)
        {
            return CmdSetVariableBase(cmdThread, cmdName, args, true /* setCommandResult */);
        }

        /// <summary>Command.
        /// Gets the specified variable and returns its value (or null if the variable does not exist).
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Value of the variable.</returns>
        protected virtual string CmdGetVariable(CommandThread cmdThread,
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
                ret = GetVariableValue(cmdThread, varName);
            }
            return ret;
        }

        /// <summary>Command.
        /// Clears the specified variable.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdClearVariable(CommandThread cmdThread,
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
                ret = ClearVariable(cmdThread, varName);
            }
            return ret;
        }

        /// <summary>Command.
        /// Prints the specified variable.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdPrintVariable(CommandThread cmdThread,
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
                ret = PrintVariable(cmdThread, varName);
            }
            return ret;
        }


        // FLOW OF CONTROL:


        /// <summary>Command.
        /// Prints the specified variable.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdThreadInfo(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <threadInfo localVar globalVar> : prints the variable varName." + Environment.NewLine
                            + "  threatInfo: whether information about the running thread is printed (default true)." + Environment.NewLine
                            + "  localVar: whether information about local varables is printed (default true)." + Environment.NewLine
                            + "  globalVar: whether information about global variables is printed (default true).");
                        Console.WriteLine();
                        return null;
                    }
            bool threadInfo = true, localVar = true, globalVar = true;
            if (args != null)
            {
                bool parsed;
                if (args.Length >= 1)
                {
                    parsed = Util.TryParseBoolean(args[0], ref threadInfo);
                    if (!parsed)
                        throw new ArgumentException("Argument can not be converted to boolean: " + args[0]);
                    if (args.Length >= 2)
                    {
                        parsed = Util.TryParseBoolean(args[1], ref localVar);
                        if (!parsed)
                            throw new ArgumentException("Argument can not be converted to boolean: " + args[1]);
                        if (args.Length >= 3)
                        {
                            parsed = Util.TryParseBoolean(args[2], ref globalVar);
                            if (!parsed)
                                throw new ArgumentException("Argument can not be converted to boolean: " + args[2]);
                        }
                    }
                }
            }
            ret = cmdThread.TopFrame.ToString(includeThreadInfo: threadInfo, includeLocalVariables: localVar, 
                includeGlobalVariables: globalVar); 
            return ret;
        }


        /// <summary>Command.
        /// Returns the interpreter's current executing thread Id.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdThreadId(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + ": Returns the interpreter's executing thread Id." );
                        Console.WriteLine();
                        return null;
                    }
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs > 0)
                throw new ArgumentException(cmdName + ": should be called without arguments.");
            else
            {
                ret = cmdThread.Id.ToString();
            }
            return ret;
        }


        /// <summary>Command.
        /// Returns the current interpreter's stack level for the currently executed interpreter thread.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdStackLevel(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + ": Returns the interpreter's current stack level.");
                        Console.WriteLine();
                        return null;
                    }
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs > 0)
                throw new ArgumentException(cmdName + ": should be called without arguments.");
            else
            {
                ret = cmdThread.TopFrame.StackLevel.ToString();
            }
            return ret;
        }



        /// <summary>Command.
        /// Prints and returns or sets and returns value of the flag that specifies whether interactive helpers 
        /// and outputs are suppressed at the thread level.
        /// <para>Called without arguments just prints and returns the current flag value.</para>
        /// <para>With one argument, the flag is set to that argument.</para></summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdSuppressInteractive(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <doSuppress> : gets or sets the flag for suppressing " + Environment.NewLine
                            + "  interactive behavior (helpers and output)."
                            + "    doSuppress: new value of the flag. " + Environment.NewLine
                            + "      If not specified then the current flag value is printed and returned.");
                        Console.WriteLine();
                        return null;
                    }
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs > 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at most 1 (argument being new flag value).");
            else
            {
                string argFlag = null;
                if (numArgs >= 1)
                    argFlag = args[0];
                bool currentFlag = cmdThread.SuppressInteractive;
                if (string.IsNullOrEmpty(argFlag))
                {
                    // No arguments, just print and return the current flag value:
                    ret = currentFlag.ToString();
                    if (this.OutputLevel >= 1)
                        Console.WriteLine(Environment.NewLine + "Suppress interactive behavior on the current thread: " + currentFlag + "." + Environment.NewLine);
                }
                else
                {
                    bool newFlag = currentFlag;
                    bool parsed = Util.TryParseBoolean(argFlag, ref newFlag);
                    if (!parsed)
                        throw new ArgumentException("Argument does not represent a bolean value: " + argFlag);
                    else
                    {
                        ret = argFlag;
                        cmdThread.SuppressInteractive = newFlag;
                        if (this.OutputLevel >= 1)
                        {
                            if (newFlag)
                                Console.WriteLine(Environment.NewLine + "Interactive behavior will be suppressed for the current thread." + Environment.NewLine);
                            else
                                Console.WriteLine(Environment.NewLine + "Interactive behavior will NOT be suppressed for the current thread." + Environment.NewLine);
                        }
                    }
                }
            }
            return ret;
        }




        /// <summary>Command.
        /// Prints and returns or sets and returns value of the flag that specifies whether interactive helpers 
        /// and outputs are suppressed for the currentt stack frame (the effect of this setting vanished when
        /// the stack frame exits).
        /// <para>Called without arguments just prints and returns the current flag value.</para>
        /// <para>With one argument, the flag is set to that argument.</para></summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdSuppressInteractiveFrame(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <doSuppress> : gets or sets the flag for suppressing " + Environment.NewLine
                            + "  interactive behavior on the current stack frame (helpers and output)."
                            + "    doSuppress: new value of the flag. " + Environment.NewLine
                            + "      If not specified then the current flag value is printed and returned.");
                        Console.WriteLine();
                        return null;
                    }
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs > 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at most 1 (argument being new flag value).");
            else
            {
                string argFlag = null;
                if (numArgs >= 1)
                    argFlag = args[0];
                bool currentFlag = cmdThread.TopFrame.SuppressInteractive;
                if (string.IsNullOrEmpty(argFlag))
                {
                    // No arguments, just print and return the current flag value:
                    ret = currentFlag.ToString();
                    if (this.OutputLevel >= 1)
                        Console.WriteLine(Environment.NewLine + "Suppress interactive behavior on the current stack frame: " + currentFlag + "." + Environment.NewLine);
                }
                else
                {
                    bool newFlag = currentFlag;
                    bool parsed = Util.TryParseBoolean(argFlag, ref newFlag);
                    if (!parsed)
                        throw new ArgumentException("Argument does not represent a bolean value: " + argFlag);
                    else
                    {
                        ret = argFlag;
                        cmdThread.TopFrame.SuppressInteractive = newFlag;
                        if (this.OutputLevel >= 1)
                        {
                            if (newFlag)
                                Console.WriteLine(Environment.NewLine + "Interactive behavior will be suppressed for the current stack frame." + Environment.NewLine);
                            else
                                Console.WriteLine(Environment.NewLine + "Interactive behavior will NOT be suppressed for the current stack frame." + Environment.NewLine);
                        }
                    }
                }
            }
            return ret;
        }




        /// <summary>Command.
        /// Enters a new command block.
        /// Optional arguments specify whether commands in the block are readily executed, and whether they are saved
        /// (for later execution or inspection).</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdBlock(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <executeImmediately saveCmds>: Enters a new code block. " + Environment.NewLine
                            + "  executeImmediately: whether to immediately execute commands in the block (default true). " + Environment.NewLine
                            + "  saveCmds: whether to save commands in the block (default true if executeImmediately is false). ");
                        Console.WriteLine();
                        return null;
                    }
            bool executeCommands = true;
            bool saveCommands = true;
            if (args != null)
            {
                bool parsed;
                if (args.Length >= 1)
                {
                    parsed = Util.TryParseBoolean(args[0], ref executeCommands);
                    if (!parsed)
                        throw new ArgumentException("Argument can not be converted to boolean: " + args[0]);
                    if (args.Length >= 2)
                    {
                        parsed = Util.TryParseBoolean(args[1], ref saveCommands);
                        if (!parsed)
                            throw new ArgumentException("Argument can not be converted to boolean: " + args[1]);
                    }
                }
            }
            EnterBlock(cmdThread, executeCommands, saveCommands);
            return null;
        }

        /// <summary>Command.
        /// Exits the current command block. Exception is thrown if we are not currently within a plain code block.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdEndBlock(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + ": Exits code block. ");
                        Console.WriteLine();
                        return null;
                    }
            //bool executeCommands = true;
            //bool saveCommands = true;
            if (args != null)
            {
                bool parsed;
                if (args.Length >= 1)
                    throw new Exception("Command " + cmdName + " can not have arguments.");
            }
            ret = ExitBlock(cmdThread);
            return ret;
        }


        /// <summary>Command.
        /// Enters a new block of JavaScript expressions that are evaluated. All lines within the block are treated as part
        /// of JavaScript code block, which is evaluated by the interpreter's JavaScript evaluator when the end of the block
        /// is reached.
        /// <para>A new command stack frame is entered only when execution is active, such that lines of JavaScript code are 
        /// collected on a separate stack frame and do not in</para></summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdCalcJsBlock(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <executeCmds saveCmds>: Enters a new JS code block. " + Environment.NewLine
                            + "  All commands are appended to the block until block exit command is called, upon which the code is executed. " );
                        Console.WriteLine();
                        return null;
                    }
            //bool executeCommands = true;
            //bool saveCommands = true;
            //if (args != null)
            //{
            //    bool parsed;
            //    if (args.Length >= 1)
            //    {
            //        parsed = Util.TryParseBoolean(args[0], ref executeCommands);
            //        if (!parsed)
            //            throw new ArgumentException("Argument can not be converted to boolean: " + args[0]);
            //        if (args.Length >= 2)
            //        {
            //            parsed = Util.TryParseBoolean(args[1], ref saveCommands);
            //            if (!parsed)
            //                throw new ArgumentException("Argument can not be converted to boolean: " + args[1]);
            //        }
            //    }
            //}
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs > 0)
                throw new ArgumentException("Command " + cmdName + " can not have arguments.");
            EnterJsBlock(cmdThread);
            return ret;
        }

        /// <summary>Command.
        /// Exits the current JavaScript expression block. If execution is active then the block of commands is sent to the
        /// internal JavaScript evaluator for execution.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdEndCalcJsBlock(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + ": Ends a JavaScript code block and executes the code." );
                        Console.WriteLine();
                        return null;
                    }
            if (args != null)
            {
                if (args.Length >= 1)
                    throw new Exception("Command " + cmdName + " can not have arguments.");
            }
            ret = ExitJsBlock(cmdThread);
            return ret;
        }



        /// <summary>Command.
        /// Prints the specified variable.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdIf(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " condition : executes a branch if condition is true.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires condition.");
            else if (args.Length < 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at least 1 (condition not specified).");
            else
            {
                bool condition = false;
                string evaluatedExpression = EvaluateJs(args);
                bool parsed = Util.TryParseBoolean(evaluatedExpression, ref condition);
                if (!parsed)
                    throw new InvalidOperationException(cmdName + ": Condition did not evaluate to boolean. Value: "
                        + evaluatedExpression + Environment.NewLine
                        + "  Command: " + cmdThread.TopFrame.LastCommandLine);
                EnterIf(cmdThread, condition);
            }
            return ret;
        }

        /// <summary>Command.
        /// Prints the specified variable.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdElseIf(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " condition : executes a branch if condition is true and " + Environment.NewLine
                            + "  none of the previous if-else branches haven't been executed.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires condition.");
            else if (args.Length < 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at least 1 (condition not specified).");
            else
            {
                bool condition = false;
                string evaluatedExpression = EvaluateJs(args);
                bool parsed = Util.TryParseBoolean(evaluatedExpression, ref condition);
                if (!parsed)
                    throw new InvalidOperationException(cmdName + ": Condition did not evaluate to boolean. Value: "
                        + evaluatedExpression + Environment.NewLine
                        + "  Command: " + cmdThread.TopFrame.LastCommandLine);
                EnterElseIf(cmdThread, condition);
            }
            return ret;
        }


        /// <summary>Command.
        /// Prints the specified variable.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdElse(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + ": executes a branch if  " + Environment.NewLine
                            + "  none of the previous if-else branches haven't been executed.");
                        Console.WriteLine();
                        return null;
                    }
            if (args != null)
            {
                if (args.Length > 0)
                    throw new ArgumentException(cmdName + " : Else should have no arguments.");
            }
            EnterElse(cmdThread);
            return ret;
        }


        /// <summary>Command.
        /// Ends the If/ElseIf/Else block.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdEndIf(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + ": ends the if branch.");
                        Console.WriteLine();
                        return null;
                    }
            if (args != null)
            {
                if (args.Length > 0)
                    throw new ArgumentException(cmdName + " : Else should have no arguments.");
            }
            ret = ExitIf(cmdThread);
            return ret;
        }



        /// <summary>Command.
        /// Enters the while block. Arguments represent loop contioton that is evaluated at the begining of each iteration.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdWhile(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " condition : repeats the block as long as condition evaluates to true.");
                        Console.WriteLine();
                        return null;
                    }
            if (args == null)
                throw new ArgumentException(cmdName + " : Requires condition.");
            else if (args.Length < 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at least 1 (condition not specified).");
            else
            {
                int numArgs = 0;
                if (args != null)
                    numArgs = args.Length;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < numArgs; ++i)
                {
                    sb.AppendLine(args[i] + " ");
                }
                string conditionString = sb.ToString();
                EnterWhile(cmdThread, conditionString);
            }
            return ret;
        }


        /// <summary>Command.
        /// Ends (closes) the while loop.
        /// Variable name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdEndWhile(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + ": closes the While loop.");
                        Console.WriteLine();
                        return null;
                    }
            {
                ExitWhile(cmdThread);
            }
            return ret;
        }




        /// <summary>Command.
        /// Prints or sets the current output level of the interpreter.
        /// <para>This determines how much information is ouptut by the interpreter about its actions.</para>
        /// <para>Called without arguments just prints the current output level.</para>
        /// <para>With one argument, ooutput level is set to that argument.</para></summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdOutputLevel(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <newOutputLevel> : gets or sets the interpreter's output level." + Environment.NewLine
                            + "  outputLevel: new output level (verbosity) of interpreter. " + Environment.NewLine
                            + "    If not specified then current level is printed and returned." );
                        Console.WriteLine();
                        return null;
                    }
            // if (args == null)
            //    throw new ArgumentNullException(cmdName + " : Requires 1 argument (file name).");
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs > 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at most 1 (argument being new output level).");
            else
            {
                string argLevel = null;
                if (numArgs >= 1)
                    argLevel = args[0];
                int currentLevel = OutputLevel;
                if (string.IsNullOrEmpty(argLevel))
                {
                    // No arguments, just print and return the current output level:
                    ret = OutputLevel.ToString();
                    Console.WriteLine(Environment.NewLine + "Interpreter's output level: " + currentLevel + "." + Environment.NewLine);
                } else
                {
                    int level = 0;
                    bool parsed = Util.TryParse(argLevel, ref level);
                    if (!parsed)
                        throw new ArgumentException("Argument does not represent an integer output level: " + argLevel);
                    else
                    {
                        ret = argLevel;
                        OutputLevel = level;
                        if (level > currentLevel)
                            Console.WriteLine(Environment.NewLine + "Interpreter's output level increased to " + level + "." + Environment.NewLine);
                        else if (level < currentLevel)
                            Console.WriteLine(Environment.NewLine + "Interpreter's output decreased to " + level + "." + Environment.NewLine);
                        else
                            Console.WriteLine(Environment.NewLine + "Interpreter's output level unchanged (" + level + ").");
                    }
                }
            }
            return ret;
        }




        /// <summary>Command.
        /// Prints concatenated argument with spaces between them.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdWriteLine(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = CmdWrite(cmdThread, cmdName, args);
            ret += Environment.NewLine;
            Console.WriteLine();
            return ret;
        }

        /// <summary>Command.
        /// Prints concatenated argument with spaces between them.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdWrite(CommandThread cmdThread,
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
        /// Reads value of the specified variable from console.
        /// Variable name must be the first argument; Eventual other arguments are concatenated and written to console.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdRead(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " varName <arg1 arg2> ... : reads variable named varName " + Environment.NewLine
                        + "  from the standard input. Optional arguments arg1, arg2, etc. are printed first (e.g. to form a prompt)." );
                        Console.WriteLine();
                        return null;
                    }
            string varName = null;
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            //if (numArgs < 1)
            //    throw new ArgumentException("The " + cmdName + " command should have at least one argument (variable name).");
            varName = args[0];
            if (string.IsNullOrEmpty(varName))
                throw new ArgumentException(cmdName + ": Variable name to be read is not specified (should be the first argument).");
            if (numArgs > 1)
            {
                StringBuilder sb = new StringBuilder();
                {
                    for (int i = 1; i < numArgs; ++i)
                    {
                        sb.Append(args[i]);
                    }
                }
                string outputString = sb.ToString();
                Console.Write(outputString);
            }
            string varValue = Console.ReadLine();
            SetVariableValue(cmdThread, varName, varValue);
            return ret;
        }



        /// <summary>Command.
        /// Runs a file by running all its lines in the current interpreter.
        /// File name must be the only argument of the command.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdRunFile(CommandThread cmdThread,
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
                ret = RunFile(cmdThread, fileName);
            }
            return ret;
        }

        /// <summary>Command.
        /// Runs another command repetitively the specified number of times.
        /// First argument must be the number of times command is run, the second argument must
        /// be command to be run repetitively, and the rest of the arguments are passed to that 
        /// command as its arguments.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Concatenated results of all runs, separated by spaces.</returns>
        protected virtual string CmdRunRepeatVerbose(CommandThread cmdThread,
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
                ret = RunRepeatVerbose(cmdThread, args);
            }
            return ret;
        }

        /// <summary>Command.
        /// Runs another command repetitively the specified number of times.
        /// First argument must be the number of times command is run, the second argument must
        /// be command to be run repetitively, and the rest of the arguments are passed to that 
        /// command as its arguments.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Concatenated results of all runs, separated by spaces.</returns>
        protected virtual string CmdRunRepeat(CommandThread cmdThread,
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
                ret = RunRepeat(cmdThread, 1, args);
            }
            return ret;
        }

        /// <summary>Command.
        /// Runs another command in a try-catch block, such that if command throws an exception execution is not
        /// broken.
        /// The second argument must be command to be run, and the rest of the arguments are passed to that 
        /// command as its arguments.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Concatenated results of all runs, separated by spaces.</returns>
        protected virtual string CmdTryRun(CommandThread cmdThread,
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
                ret = RunTryCatch(cmdThread, OutputLevel, args);
            }
            return ret;
        }

        /// <summary>Command. 
        /// Sets the flag for rethrowing exceptions in the interaction mode.
        /// Optional boolean arguemnt, default is true.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdThtrowExceptions(CommandThread cmdThread,
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
        /// Reads commands one by one from console and executes them, until only Enter is pressed.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdRunInteractive(CommandThread cmdThread,
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
                    Run(cmdThread, command, commandArgs);
                    //throw new ArgumentNullException(cmdName + " : Requires no arguments.");
                }
            ret = RunInteractive(cmdThread);
            return ret;
        }


        //public static string ConstEndBlock = "EndBlock";
        
        //public static string ConstEndBlock1 = "/q";

        ///// <summary>Command. Runs interpreter commands in one block that must end with EndBlock.
        ///// Reads commands one by one from console and executes them, until only Enter is pressed..</summary>
        ///// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        ///// <param name="cmdName">Command name.</param>
        ///// <param name="args">Command arguments.</param>
        ///// <returns>Result of the last command that is run.</returns>
        //protected virtual string CmdRunBlock(CommandThread cmdThread,
        //    string cmdName, string[] args)
        //{
        //    string ret = null;
        //    if (args != null)
        //        if (args.Length > 0)
        //            if (args[0] == "?")
        //            {
        //                string executableName = UtilSystem.GetCurrentProcessExecutableName();
        //                Console.WriteLine();
        //                Console.WriteLine(executableName + " " + cmdName + " : runs a block of commands inserted by user.");
        //                Console.WriteLine(executableName + " " + cmdName + "  Block must end with \"" +  ConstEndBlock +  "\".");
        //                Console.WriteLine();
        //                return null;
        //            }
        //    if (args != null)
        //        if (args.Length > 0)
        //        {
        //            // If there were arguments specified, then we first execute teh command that was specified by 
        //            // these arguments: 
        //            string command = args[0];
        //            int numArgs = args.Length - 1;
        //            string[] commandArgs = null;
        //            if (numArgs >= 1)
        //            {
        //                commandArgs = new string[numArgs];
        //                for (int i = 0; i < numArgs; ++i)
        //                    commandArgs[i] = args[i + 1];
        //            }
        //            Run(cmdThread, command, commandArgs);
        //            //throw new ArgumentNullException(cmdName + " : Requires no arguments.");
        //        }

        //    Console.WriteLine(Environment.NewLine + Environment.NewLine 
        //        + "Insert a block of commands! " + Environment.NewLine
        //        + "Finish the block with a line containing just \"" +  ConstEndBlock +  "\" " + Environment.NewLine
        //        + "  or \"" + ConstEndBlock1 + "\". ");
        //    List<string> commands = new List<string>();
        //    bool stopDo = false;

        //    do
        //    {
        //        string line = Console.ReadLine();
        //        string[] commandLineSplit = GetArguments(line);
        //        if (commandLineSplit != null)
        //            if (commandLineSplit.Length > 0)
        //                if (commandLineSplit[0] == ConstEndBlock || commandLineSplit[0] == ConstEndBlock1)
        //                    stopDo = true;
        //        if (!stopDo)
        //            commands.Add(line);
        //    } while (!stopDo);

        //    int numCommands = commands.Count;
        //    for (int i = 0; i < numCommands; ++i)
        //    {
                                
        //        try
        //        {
        //            string[] commandLineSplit = GetArguments(commands[i]);
        //            if (commandLineSplit != null)
        //                if (commandLineSplit.Length > 0)
        //                    ret = this.Run(cmdThread, commandLineSplit);
        //            bool outputAllResults = false;
        //            if (outputAllResults)
        //            {
        //            string retOutput = ret;
        //            if (retOutput == null)
        //                retOutput = "null";
        //            else if (retOutput == "")
        //                retOutput = "\"\"";
        //            Console.WriteLine("  = " + retOutput);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine();
        //            Console.WriteLine("ERROR: " + ex.Message);
        //            Console.WriteLine();
        //            if (ThrowExceptions)
        //                throw;
        //        }
        //    }

        //    return ret;
        //}


        /// <summary>Command. 
        /// Runs the specified command-line by the operating system.
        /// The first argument is the command to be executed while the following arguments are 
        /// arguments to this command.
        /// If there are no arguments then user is requested to insert commands interactively.</summary>
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments this command.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdRunSystem(CommandThread cmdThread,
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
        /// Rturns or sets the current directory.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of the last command that is run.</returns>
        protected virtual string CmdCurrentDirectory(CommandThread cmdThread,
            string cmdName, string[] args)
        {
            string ret = null;
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = UtilSystem.GetCurrentProcessExecutableName();
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " <newDir> : gets or sets the current directory.");
                        Console.WriteLine("    newDir: new current directory. '..', './dir' etc. can be used.");
                        Console.WriteLine();
                        return null;
                    }
            // if (args == null)
            //    throw new ArgumentNullException(cmdName + " : Requires 1 argument (file name).");
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs > 1)
                throw new ArgumentException(cmdName + " : invalid number of arguments, should be at most 1 (argument being new current directory).");
            else
            {
                string newDirectory = null;
                if (numArgs >= 1)
                    newDirectory = args[0];
                if (!string.IsNullOrEmpty(newDirectory))
                {
                    Directory.SetCurrentDirectory(newDirectory);
                    ret = Directory.GetCurrentDirectory();
                    if (OutputLevel >= 1)
                        Console.WriteLine(Environment.NewLine + "New current directory: " + ret + Environment.NewLine);
                }
                else 
                {
                    ret = Directory.GetCurrentDirectory();
                    if (OutputLevel >= 1)
                        Console.WriteLine(Environment.NewLine + "Current directory: " + ret + Environment.NewLine);
                }
            }
            return ret;
        }



        /// <summary>Command. 
        /// Runs the built in expression evaluator.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments this command.</param>
        /// <returns></returns>
        protected virtual string CmdExpressionEvaluatorJavascript(CommandThread cmdThread,
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
                return EvaluateJsInteractive();
            }
            else if (args.Length < 1)
            {
                ret = EvaluateJsInteractive();
            }
            else
            {
                ret = EvaluateJs(args);
                // Console.WriteLine(Environment.NewLine + "  = " + ret);
            }
            return ret;
        }





        /// <summary>Interpreter command. Sets the priority of the current process.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>Null.</returns>
        protected virtual string CmdSetPriority(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>ID of the job container that contains all command data.</returns>
        protected virtual string CmdRunParallel(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>IDs of the job container that contains all command data separated by spaces.</returns>
        protected virtual string CmdRunParallelRepeat(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>Null.</returns>
        protected virtual string CmdPrintParallelCommands(CommandThread cmdThread,
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
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>ID of the asynchronous command for querrying completion and ending invocation and picking results.</returns>
        protected virtual string CmdRunAsync(CommandThread cmdThread,
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
            return RunAsync(cmdThread, args);
        }


        /// <summary>Command. 
        /// Wait until asynchronously invoked command with the specified ID (first argument, must represent an int) completes.
        /// <para>The first argument is the ID of asynchronous invocation whose results are waited.</para></summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>Results of the async. command execution whose completion is waited for.</returns>
        protected virtual string CmdAsyncWaitResults(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>Results of the async. command execution whose completion is waited for.</returns>
        protected virtual string CmdAsyncCompleted(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name of this command.</param>
        /// <param name="args">Command arguments of this command.</param>
        /// <returns>Returns the ID of the thread where sleep is performed.</returns>
        protected virtual string CmdSleepSeconds(CommandThread cmdThread,
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
        /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null.</returns>
        protected virtual string CmdLoadModule(CommandThread cmdThread,
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
                    ret = this.Run(cmdThread, commandName, commandArgs);
                }
            }
            return ret;
        }


        /// <summary>Executinon method for command that checks if module is loaded.
        /// Writes to condole whether module is loaded or not, and returns "1" if module
        /// is loaded and "0" if not.</summary>
        /// <param name="thread">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdIsModuleLoaded(CommandThread thread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdModuleTestCommand(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdTestFromTestModules(CommandThread cmdThread,
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
            /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
            /// <param name="pipeName">Name of the pipe.</param>
            /// <param name="requestEnd">Line that ends each request. If null or empty string then the requests are single line.</param>
            /// <param name="responseEnd">Line that ends each response. If null or empty string then the responses are single line.</param>
            /// <param name="errorBegin">String that begins an error response. If null or empty string then default string remains in use,
            /// i.e. <see cref="DefaultErrorBegin"/></param>
            /// <param name="startImmediately">If true then server is starrted immediately, otherwise this is postponed.</param>
            public InterpreterPipeServer(CommandThread cmdThread, string pipeName, 
                bool startImmediately, string requestEnd, string responseEnd, string errorBegin) :
                base(pipeName, requestEnd, responseEnd, errorBegin, false /* startImmediately */)
            { 
                Interpreter = cmdThread.Interpreter;
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
            /// <param name="cmdThread">Commandline interpreter thread in which command is executed.</param>
            /// <param name="request"></param>
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
                    ret = Interpreter.Run(Interpreter.MainThread, commandLine);
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
        protected virtual string CmdPipeServerCreate(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>A string containing the information on the removed servers (their interpreter's names and pipe names).</returns>
        protected virtual string CmdPipeServersRemove(CommandThread cmdThread,
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
        /// Optional command argument is server name. If not specified then information about all installed servers is printed and returned.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>A string containing the information on pipe servers.</returns>
        protected virtual string CmdPipeServerInfo(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Commandline arguments.</param>
        /// <returns>A string containing the information on pipe clients.</returns>
        protected virtual string CmdPipeClientCreate(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Commandline arguments.</param>
        /// <returns>A string containing the information on the removed clients (their interpreter's names and pipe names).</returns>
        protected virtual string CmdPipeClientsRemove(CommandThread cmdThread,
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
        /// Optional command argument is client name. If not specified then information about all installed clients is printed and returned.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Commandline arguments.</param>
        /// <returns>A string containing some basic data on the created pipe client.</returns>
        protected virtual string CmdPipeClientInfo(CommandThread cmdThread,
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
        /// Command argument is the (interpreter's) name of the pipe client followed by command and eventual arguments sent to the server.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Commandline arguments.</param>
        /// <returns>Server response.</returns>
        protected virtual string CmdPipeClientGetServerResponse(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Name of the command</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of command execution.</returns>
        protected virtual string CmdRunInternalScriptClass(CommandThread cmdThread, string cmdName, string[] args)
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Name of the command</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of command execution.</returns>
        protected virtual string CmdRunScriptFile(CommandThread cmdThread, string cmdName, string[] args)
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Name of the command</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>null string.</returns>
        protected virtual string CmdLoadScript(CommandThread cmdThread,
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
            this.AddCommandMt(newCommandName, CmdRunLoadedScript);
            return null;
        }  // CmdLoadScript()

        /// <summary>Interpreter command.
        /// Runs a command based on dynamically loaded loadable script class. Arguments passed to this
        /// command are directly passed on to the dynamically loaded script class installed on 
        /// <see cref="LoadableScriptInterpreter"/> under the same <paramref name="cmdName"/>.
        /// Typically, the command that is executed by the current method, has been previously installed
        /// by the <see cref="CmdLoadScript"/>(...) method.</summary>
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Name of the command, which must be the same as command name
        /// under which dynamically loaded class is installed on <see cref="LoadableScriptInterpreter"/>.</param>
        /// <param name="args">Command arguments. These arguments are directly passed to the 
        /// executable method on the corresponding class.</param>
        /// <returns>Result of command execution.</returns>
        protected virtual string CmdRunLoadedScript(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Name of the command.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Result of command execution, a list of all referenced assemblies.</returns>
        protected virtual string WriteLoadableScriptReferencedAssemblies(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdExit(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdHelp(CommandThread cmdThread,
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
            if (_commandsMt == null)
                sb.AppendLine("  No applications (null reference).");
            else if (_commandsMt.Count < 1)
                sb.AppendLine("  No applications. ");
            else
            {
                sb.Append(" ");
                foreach (KeyValuePair<string, ApplicationCommandDelegateMt> pair in _commandsMt)
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdAbout(CommandThread cmdThread, string cmdName, string[] args)
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdApplicationInfo(CommandThread cmdThread, string cmdName, string[] args)
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdComment(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdPrintCommands(CommandThread cmdThread,
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
            if (_commandsMt == null)
                Console.WriteLine("  No commands (null reference).");
            else if (_commandsMt.Count < 1)
                Console.WriteLine("  No commands. ");
            else
            {
                Console.Write(" ");
                foreach (KeyValuePair<string, ApplicationCommandDelegateMt> pair in _commandsMt)
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdTestProduct(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected virtual string CmdTest(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Speed factor - Ratio between reference computation time and time spent for the same thing in current environment.</returns>
        protected virtual string CmdTestSpeed(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Speed factor - Ratio between reference computation time and time spent for the same thing in current environment.</returns>
        protected virtual string CmdTestSpeedLong(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Total wallclock time spent for computation.</returns>
        protected virtual string CmdTestQR(CommandThread cmdThread,
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
        /// <param name="cmdThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Total wallclock time spent for computation.</returns>
        protected virtual string CmdTestLU(CommandThread cmdThread,
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
            interpreter.AddCommandMt("Test", CmdTestFromTestModules);
            interpreter.AddCommandMt("Test1", CmdTestFromTestModules);
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
            interpreter.AddCommandMt("Test", CmdTestFromTestModules);
            interpreter.AddCommandMt("Test2", CmdTestFromTestModules);
            Console.WriteLine("Module commands: ");
            Console.WriteLine("  " + ModuleTestCommandName(name) + " - automatically installed when a module is loaded.");
            Console.WriteLine("  Test -  test commands, replaces the old 'test' command");
            Console.WriteLine("  Test2 - test commands, the same command with different name");
            Console.WriteLine();
            Console.WriteLine("Module loaded: " + name + ".");
            Console.WriteLine();
        }


        #endregion Modules


    }  // class CommandLineApplicationInterpreter

}