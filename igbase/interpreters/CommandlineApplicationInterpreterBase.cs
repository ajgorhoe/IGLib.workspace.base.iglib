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


    /// <summary>Carries command execution data, results, and other data such as identification number, etc.
    /// <para>Used as job container for parallel execution of interpreter commands.</para></summary>
    /// <remarks>
    /// <para>Objects of this type contain all data necessary for execution of the specified command by the specified
    /// interpreter. Interpreter to execute the command, command name and arguments, and results of the command
    /// are all stored on the object. </para>
    /// </remarks>
    /// $A Igor Jan09;
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
        /// <param name="commandThread">Commandline interpreter thread in which command is executed.</param>
        protected virtual void RunCommand()
        {
            this.StartTime = DateTime.Now;
            this.CommandResult = Interpreter.Run(Interpreter.MainThread, CommandName, CommandArguments);
            this.CompletionTime = DateTime.Now;
            this.ExecutionTime = (CompletionTime - StartTime).TotalSeconds;
        }

        /// <summary>Executes the command that is represented by the current command data,
        /// and stores the results.
        /// <para>This method just calls the argument-less <see cref="RunCommand"/> method and is used for execution delegate
        /// for the parallel job container.</para></summary>
        /// <param name="commandThread">Commandline interpreter thread in which command is executed.</param>
        /// <param name="input">Input data. Does not have any effect because data is taken from the current object.</param>
        /// <returns>Resuts. Actually it just returns the current object, since results are stored in this object
        /// by the argument-less <see cref="RunCommand"/> function.</returns>
        protected static CommandLineJobContainer Run( 
            CommandLineJobContainer input)
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


    /// <summary>Adapter class that wraps a single threaded command delegate and provides a multithreaded function.</summary>
    /// $A Igor Aug08;
    [Obsolete("Single threaded command delegates are deprecated and will be removed in the future.")]
    public class CommandAdapterSingleThreaded
    {

        private CommandAdapterSingleThreaded() {  }

        /// <summary>Constructor.</summary>
        /// <param name="singleThreadedApplication">The delegate to handle commands (wrong type because it is single threaded).</param>
        /// <param name="interpreter">Interpreter that will be used to interpret the new command.
        /// <para>This argument is optional, because in most cases the interpreter will be obtained from the commad thread, which is 
        /// passed to the adapted command.</para></param>
        public CommandAdapterSingleThreaded(ApplicationCommandDelegate singleThreadedApplication,
            ICommandLineApplicationInterpreter interpreter = null)
        {
            this.SingleThreadedApplication = singleThreadedApplication;
            this.Interpreter = interpreter;
        }

        /// <summary>The original single threaded command delegate that we want to install to execute commands.</summary>
        public ApplicationCommandDelegate SingleThreadedApplication { get; protected set; }

        public ICommandLineApplicationInterpreter Interpreter
        { get; protected set; }

        /// <summary>Converted method that can be added to a collection of multithreaded commands, and calls the original
        /// single threaded command.</summary>
        /// <param name="commandThread"></param>
        /// <param name="commandName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string MultiThreadedApplication(CommandThread commandThread,
            string commandName, string[] args)
        {
            ICommandLineApplicationInterpreter interp = null;
            if (commandThread != null)
                interp = commandThread.Interpreter;
            if (interp == null && this.Interpreter != null)
                interp = this.Interpreter;
            return SingleThreadedApplication(this.Interpreter, commandName, args);
        }
    }  // class CommandAdapterSingleThreaded



    /// <summary>Delegate for commands that are installed on interpreter.
    /// <para>Only suitable for single threaded command execution.</para></summary>
    /// <param name="interpreter">Interpreter on which commad is run. 
    /// Enables access to interpreter internal data from command body.</param>
    /// <param name="commandName">Command name.</param>
    /// <param name="args">Command arguments.</param>
    /// <returns>Command return data.</returns>
    /// $A Igor Aug08;
    [Obsolete("Only suitable for single threaded command execution.")]
    public delegate string ApplicationCommandDelegate(ICommandLineApplicationInterpreter interpreter, string commandName, string[] args);


    /// <summary>Delegate for commands that are installed on interpreter.</summary>
    /// <param name="commandThread">Interpreter on which commad is run. 
    /// Enables access to interpreter internal data from command body.</param>
    /// <param name="commandName">Command name.</param>
    /// <param name="args">Command arguments.</param>
    /// <returns>Command return data.</returns>
    /// $A Igor xx Sep15;
    public delegate string ApplicationCommandDelegateMtGeneric<InterpreterType>(CommandThread<InterpreterType> commandThread, 
        string commandName, string[] args)
        where InterpreterType: class, ICommandLineApplicationInterpreter;

    /// <summary>Delegate for commands that are installed on interpreter.</summary>
    /// <param name="commandThread">Interpreter on which commad is run. 
    /// Enables access to interpreter internal data from command's body.</param>
    /// <param name="commandName">Command name.</param>
    /// <param name="args">Command arguments.</param>
    /// <returns>Command return data.</returns>
    /// $A Igor xx Sep15;
    public delegate string ApplicationCommandDelegateMt(CommandThread commandThread, 
        string commandName, string[] args);


    /// <summary>Delegate for installing a module on the interpreter.</summary>
    /// <param name="modulename">Name of the module.</param>
    /// <param name="interpreter">Interperter where module is installed.</param>
    /// $A Igor Mar09;
    public delegate void ModuleDelegate(string modulename, ICommandLineApplicationInterpreter interpreter);



    /// <summary>Flags that specify interpreter variable type and behavior.
    /// <para>Flag-like values.</para></summary>
    /// $A Igor Oct15;
    [Flags]
    public enum VariableFlags : int
    {
        /// <summary>Helper flag, meaning that no other type flag is set.</summary>
        None = 0,
        /// <summary>Indicates that variable is defined and active, i.e., it is valid and can be used.</summary>
        Valid = 1,
        /// <summary>Variable contains string value.
        /// <para>Commandline interpreter variables are string variables by default, but may be extended to contain
        /// other object types.</para></summary>
        StringVar = 2,
        /// <summary>Variable references another variable.</summary>
        ReferenceVar = 4,
        /// <summary>Default flags - for most ordinary value (= <see cref="VariableFlags.StringVar"/> and <see cref="VariableFlags.Valid"/>)</summary>
        Default = Valid & StringVar
    }




    /// <summary>Represents type of the interpretation block for which a stack frame exists on command thread.
    /// <para>Flag-like values.</para></summary>
    /// $A Igor Sep15;
    [Flags]
    public enum CodeBlockType : int
    {
        /// <summary>Helper type, meaning that no other type flag is set.</summary>
        None = 0,
        /// <summary>Base block of the thread. <para>Currently not used, but might be in the future.</para></summary>
        Base = 1,
        /// <summary>Plain code block (entered via "Block" command).</summary>
        Block = 2,
        /// <summary>Branching block (entered via If, ElseIf or Else comands).</summary>
        If = 4,
        /// <summary>While block (entered via While command).</summary>
        While = 8,
        /// <summary>Function block (entered via Call command).</summary>
        Function = 32,
        /// <summary>Any callable block, such as function. <para>Code for such a block is stored in a separate store (e.g.
        /// when Function command is executed that stores function definition) and it is executed from there. These blocks
        /// do not see local variables of the calling blocks (lower level stack frames).</para></summary>
        Callable = Function | Function
    }


    /// <summary>Base class for interpreter variables.</summary>
    /// $A Igor Oct15;
    public class InterpreterVariable
    {

        private InterpreterVariable() {  }


        /// <summary>Constructs a new interpreter variable.</summary>
        /// <param name="variableName">Variable name. Must be specified.</param>
        /// <param name="variableValue">String value of the variable. Can be null.</param>
        /// <param name="flags">Flags that define type and behavior of the variable. Default is <see cref="VariableFlags.Default"/></param>
        public InterpreterVariable(string variableName, int stackLevel = StackLevelDefault, string variableValue = null, VariableFlags flags = VariableFlags.Default)
        {
            this.Name = variableName;
            this.StringValue = variableValue;
            this.Flags = flags;
        }

        private string _variableName;

        /// <summary>Variable name.
        /// <para>Normally, this will coincide with the name through which the variable is referenced in the interpreter.</para></summary>
        public string Name
        { get { return _variableName; } protected set { _variableName = value; } }

        private int _stackLevel = -1;

        /// <summary>Specifies the stack level of the local variable and whether the variable is local or global.
        /// <para>Greater or equal to 0 means that the variable is a local variable defined on the stack frame of 
        /// the specified level.</para>
        /// <para>-1 means that the variable is global.</para>
        /// <para>Less than -1 meand that the variable is neither local nor global (usually, this indicates an error).</para></summary>
        public int StackLevel
        { get { return _stackLevel; } protected set { _stackLevel = value; } }

        /// <summary>Stack level used for global variables.</summary>
        public const int StackLevelGlobal = -1;

        /// <summary>Stack level used for variables that are neither local or global (or for which stack level is unknown).</summary>
        public const int StackLevelUndefined = -2;

        /// <summary>Default stack level - <see cref="StackLevelUndefined"/></summary>
        public const int StackLevelDefault = StackLevelUndefined;
        
        /// <summary>Gets a flag indicating whether the current variable is a global variable.</summary>
        bool IsGlobal { get { return _stackLevel == -1; } }

        /// <summary>Gets a flag indicating whether the current variable is a global variable.</summary>
        bool IsLocal { get { return _stackLevel >= 0; } }

        private string _stringValue;

        /// <summary>String value of the variable.</summary>
        /// <remarks>
        /// <para>Variables of the commandline interpreter (<see cref="ICommandLineApplicationInterpreter"/>) normally have string values.</para>
        /// </remarks>
        public string StringValue
        { 
            get 
            { 
                if (IsReference)
                {
                    if (ReferencedVariable == null)
                        throw new InvalidOperationException("Getting variable value: Referenced variable is not defined.");
                    return ReferencedVariable.StringValue;
                }
                return _stringValue;
            } 
            set 
            {
                if (IsReference)
                {
                    if (ReferencedVariable == null)
                        throw new InvalidOperationException("Setting variable value: Referenced variable is not defined.");
                    ReferencedVariable.StringValue = value;
                } else
                {
                    _stringValue = value;
                }
            } 
        }

        private VariableFlags _flags = VariableFlags.Default;

        /// <summary>Flags that define type and behavior of the variable.</summary>
        public VariableFlags Flags
        { get { return _flags; } protected set { _flags = value; } }

        /// <summary>Indicates whether the variable is valid and can be used (i.e., is defined).
        /// <para>Can be get and set.</para>
        /// <para>When a variable is removed, its valid flag will be set to false. In this way, variables that
        /// evantually reference this variable will know that their reference is invalid.</para></summary>
        public bool IsValid
        { 
            get { return (Flags & VariableFlags.Valid) != 0; } 
            set { if (value) { Flags |= VariableFlags.Valid; } else { Flags &= ~VariableFlags.Valid; } } 
        }

        /// <summary>Indicates whether the variable is a reference to another variable.</summary>
        public bool IsReference
        {
            get { return (Flags & VariableFlags.ReferenceVar) != 0; }
            protected set { if (value) { Flags |= VariableFlags.ReferenceVar; } else { Flags &= ~VariableFlags.ReferenceVar; } } 
        }

        private InterpreterVariable _referencedVar;

        /// <summary>Reference to the variable that is referenced by the current variable.</summary>
        public InterpreterVariable ReferencedVariable
        { 
            get { return _referencedVar; } 
            protected set { 
                _referencedVar = value;
                if (value != null)
                    IsReference = true;
            } 
        }

        /// <summary>Sets the the variable reference, which defines the variable that the current variable references.
        /// <para>Exception is thrown if the current variable is not a reference variable.</para></summary>
        /// <param name="referencedVariable">Variable that will be referenced by the current variable.
        /// <para>Normally, this is required information for reference variables, but can be set undefined.</para></param>
        /// <param name="referencedVariableName">Name of the referenced variable. This is auxiliary information and is
        /// usually provided only when the name <paramref name="referencedVariable"/> is not specified (i.e., it is a null reference).</param>
        /// <param name="referencedVariableStackLevel">The stack level of the referenced variable.</param>
        public void SetReferencedVariable(InterpreterVariable referencedVariable, string referencedVariableName = null,
            int referencedVariableStackLevel = StackLevelDefault)
        {
            if (!IsReference)
                throw new InvalidOperationException("The current variable is not a reference to another variable.");
            // this.IsReference = true;
            this.ReferencedVariable = referencedVariable;
            if (referencedVariable != null)
            {
                // The variable reference is specified, no need to set the auxiliary data. Just check consistency of arguments.
                if (referencedVariableName != null && referencedVariableName != ReferencedVariable.Name)
                    throw new ArgumentException("The specified reference variable name \"" + referencedVariableName +
                        "\" does not match its actual name (\"" + ReferencedVariable.Name + "\").");
                if (referencedVariableStackLevel != StackLevelDefault && referencedVariableStackLevel != ReferencedVariable.StackLevel)
                    throw new ArgumentException("The specified referenced variable stack level " + referencedVariableStackLevel +
                        " does not match its actual stack level (" + ReferencedVariable.StackLevel + ").");
            } else
            {
                this.ReferencedVariableName = referencedVariableName;
                this.ReferencedVariableStackLevel = referencedVariableStackLevel;
            }
        }

        private int _referencedVariableStackLevel = StackLevelDefault;

        /// <summary>Gets the stack level of the referenced variable.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the current variable is not a reference to another variable.</exception>
        public int ReferencedVariableStackLevel
        { 
            get {
                if (!IsReference)
                    throw new InvalidOperationException("The current variable is not a reference to another variable.");
                if (ReferencedVariable != null)
                    return ReferencedVariable.StackLevel;
                else
                    return _referencedVariableStackLevel; 
            } 
            protected set {
                if (!IsReference)
                    throw new InvalidOperationException("The current variable is not a reference to another variable."); 
                _referencedVariableStackLevel = value;
            }
        }

        private string _referencedVariableName = null;

        /// <summary>Gets the referenced variable name.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the current variable is not a reference to another variable.</exception>
        public string ReferencedVariableName
        {
            get
            {
                if (!IsReference)
                    throw new InvalidOperationException("The current variable is not a reference to another variable.");
                if (ReferencedVariable != null)
                    return ReferencedVariable.Name;
                else
                    return _referencedVariableName;
            }
            protected set
            {
                if (!IsReference)
                    throw new InvalidOperationException("The current variable is not a reference to another variable."); 
                _referencedVariableName = value;
            }
        }

        /// <summary>Gets a flag indicating whether the current variable references a local variable.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the current variable is not a reference to another variable.</exception>
        public bool IsReferencedVariableLocal
        /// <exception cref="InvalidOperationException">Thrown when the current variable is not a reference to another variable.</exception>
        {
            get
            {
                if (!IsReference)
                    throw new InvalidOperationException("The current variable is not a reference to another variable."); 
                return ReferencedVariableStackLevel >= 0;
            }
        }

        /// <summary>Gets a flag indicating whether the current variable references a global variable.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the current variable is not a reference to another variable.</exception>
        public bool IsReferencedVariableGlobal
        {
            get
            {
                if (!IsReference)
                    throw new InvalidOperationException("The current variable is not a reference to another variable.");
                return ReferencedVariableStackLevel == StackLevelGlobal;
            }
        }

        /// <summary>Gets the number of stack levels for which the referenced variable is defined below the current variable.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the current variable is not a reference to another variable,
        /// or the referenced variable is not a local variable, or the current variable is not a local variable..</exception>
        public int ReferencedVariableLevelsBelow
        { 
            get {
            if (!IsReference)
                throw new InvalidOperationException("The current variable is not a reference to another variable.");
            if (!IsReferencedVariableLocal)
                throw new InvalidOperationException("Referenced variable is not a local variable.");
            else if (!IsLocal)
                throw new InvalidOperationException("The reference variable is not local.");
            else
                return this.StackLevel - this.ReferencedVariableStackLevel;
            }
        }

        /// <summary>Returns a string that describes the current interpreter variable.</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Interpreter variable \"" + Name + "\":");
            if (IsValid)
                sb.AppendLine("  Valid: " + IsValid);
            else
                sb.AppendLine("  NOT VALID.");
            if (IsLocal)
            {
                sb.AppendLine("  Local variable (stack level: " + StackLevel + ").");
            }
            else if (IsGlobal)
                sb.AppendLine("  Global variable.");
            else
                sb.AppendLine("  Local or global: NOT DEFINED.");
            if (StringValue == null)
                sb.AppendLine("  String value: undefined (null).");
            else
                sb.AppendLine("  String value: \"" + StringValue + "\".");
            sb.Append("  Flags:");
            if (Flags == VariableFlags.None)
                sb.AppendLine(Environment.NewLine + "    No flags are specified.");
            else
            {
                if ((Flags & VariableFlags.Valid) != 0)
                    sb.Append(" " + VariableFlags.Valid.ToString());
                if ((Flags & VariableFlags.StringVar) != 0)
                    sb.Append(" " + VariableFlags.StringVar.ToString());
                if ((Flags & VariableFlags.ReferenceVar) != 0)
                    sb.Append(" " + VariableFlags.ReferenceVar.ToString());
            }
            if (IsReference)
            {
                sb.Append("  Variable is a reference to another variable.");
                if (ReferencedVariable != null)
                    sb.Append("    Referenced variable is defined explicitly.");
                else
                    sb.Append("    Referenced variable is NOT defined explicitly.");
                if (IsReferencedVariableLocal)
                    sb.AppendLine("    References a local variable with stack level " + ReferencedVariableStackLevel 
                        + Environment.NewLine + "      (" + ReferencedVariableLevelsBelow + " levels below the current variable's stack level).");
                else if (IsReferencedVariableGlobal)
                    sb.AppendLine("    References a global variable.");
                else
                    sb.AppendLine("    Referenced variable: local or global NOT DEFINED.");
            }
            return sb.ToString();
        }

    }  // class InterpreterVariable





    /// <summary>Stack frame for a block of command-line interpreter commands.</summary>
    /// <remarks>
    /// <para>Usually properties, index operators and methods are not thread safe. Variants whose names end with "Locked"
    /// implement locking on the <see cref="Lock"/> property and are thus thread safe. It is not likely that
    /// locking is needed because the object will typically be used within a single thread.</para>
    /// </remarks>
    /// $A Igor Sep15;
    public class CommandStackFrame :
        CommandStackFrame<ICommandLineApplicationInterpreter, CommandThread>
    {

        /// <summary>Constructor.</summary>
        /// <param name="blockType">Type of code block corresponding to the current stack frame.</param>
        /// <param name="thread">Command thread on which the current stack frame exists.</param>
        /// <param name="stackLevel">Level of the current stack frame.</param>
        public CommandStackFrame(CodeBlockType blockType, CommandThread thread, int stackLevel) :
            base(blockType, thread, stackLevel) { }


        /// <summary>Returns the sibling stack frame at the certain level.</summary>
        /// <param name="stackLevel">Stack level for which the sibling frame is returned.</param>
        /// <returns></returns>
        public override CommandStackFrame<ICommandLineApplicationInterpreter, CommandThread> GetThreadStackFrame(int stackLevel)
        {
            return this.InterpreterThread[stackLevel];
        }

        /// <summary>Returns the previous (parent, or one lower level) stack frame of the current stack frame.</summary>
        public override CommandStackFrame<ICommandLineApplicationInterpreter, CommandThread> GetParentStackFrame()
        {
            int stackLevel = this.StackLevel - 1;
            if (stackLevel < 0)
                return null;
            else
                return this.InterpreterThread[stackLevel];
        }

        /// <summary>Returns the commandline interpreter of the crrent stack frame.</summary>
        public override ICommandLineApplicationInterpreter Interpreter { get { return this.InterpreterThread.Interpreter; } }

    }  // class CommandStackFrame


    /// <summary>Contains stack frames and other command thread data for a single command thread of a 
    /// command-line interreter.</summary>
    /// <remarks>
    /// <para>Usually properties, index operators and methods are not thread safe. Variants whose names end with "Locked"
    /// implement locking on the <see cref="Lock"/> property and are thus thread safe. It is not likely that
    /// locking is needed because the object will typically be used within a single thread.</para>
    /// </remarks>
    /// $A Igor Sep15;
    public class CommandThread :
        CommandThread<ICommandLineApplicationInterpreter, CommandStackFrame, CommandThread>
    {
        public CommandThread(ICommandLineApplicationInterpreter interpreter) : base(interpreter) { }

        /// <summary>Creates a new stack frame.</summary>
        /// <param name="type">Type of code block for which stack frame is created.</param>
        /// <param name="stackLevel">Level of the stack frame.</param>
        /// $A Igor Sep15;
        protected override CommandStackFrame CreateFrame(CodeBlockType type, int stackLevel)
        {
            return new CommandStackFrame(type, this, stackLevel);
        }
    }  //class CommandThread


    // Remarks:
    // The two classes below are heneric varians of CommandStackFrame and CommandThread where type of 
    // the commandline interpreter is still free.
    // If this class was used to inherit in nongeneric classes then in these classes, types of the
    // embedded stackframe and thread variables would be generic (always stated with type parameter
    // for interpreter type), which is why the nongeneric types will inherit from more generic types, 
    // from which these two types also inherit, directly. This makes possible that all types used within
    // the classes will be nongeneric.


    /// <summary>Stack frame for a block of command-line interpreter commands.</summary>
    /// <remarks>
    /// <para>Usually properties, index operators and methods are not thread safe. Variants whose names end with "Locked"
    /// implement locking on the <see cref="Lock"/> property and are thus thread safe. It is not likely that
    /// locking is needed because the object will typically be used within a single thread.</para>
    /// </remarks>
    /// <typeparam name="InterpreterType">Type of the interpreter for which this class provides stack frames.</typeparam>
    /// $A Igor Sep15;
    public class CommandStackFrame<InterpreterType> :
        CommandStackFrame<InterpreterType, CommandThread<InterpreterType>>
        where InterpreterType : class, ICommandLineApplicationInterpreter
    {

        /// <summary>Constructor.</summary>
        /// <param name="blockType">Type of the command block represented by the current stack frame.</param>
        /// <param name="thread">Command thread data that contains the current stack frame.</param>
        /// <param name="stackLevel">Stack level of the current frame.</param>
        public CommandStackFrame(CodeBlockType blockType, CommandThread<InterpreterType> thread, int stackLevel) :
            base(blockType, thread, stackLevel) {  }

        /// <summary>Returns the sibling stack frame at the certain level.</summary>
        /// <param name="stackLevel">Stack level for which the sibling frame is returned.</param>
        /// <returns></returns>
        public override CommandStackFrame<InterpreterType, CommandThread<InterpreterType>> GetThreadStackFrame(int stackLevel)
        {
            return this.InterpreterThread[stackLevel];
        }

        /// <summary>Returns the previous (parent, or one lower level) stack frame of the current stack frame.</summary>
        public override CommandStackFrame<InterpreterType, CommandThread<InterpreterType>> GetParentStackFrame()
        {
            int stackLevel = this.StackLevel - 1;
            if (stackLevel < 0)
                return null;
            else
                return this.InterpreterThread[stackLevel];
        }

        /// <summary>Returns the commandline interpreter of the crrent stack frame.</summary>
        public override InterpreterType Interpreter { get { return this.InterpreterThread.Interpreter;  } }

    }  // class CommandStackFrame<InterpreterType>


    /// <summary>Contains stack frames and other command thread data for a single command thread of a 
    /// command-line interreter.</summary>
    /// <remarks>
    /// <para>Usually properties, index operators and methods are not thread safe. Variants whose names end with "Locked"
    /// implement locking on the <see cref="Lock"/> property and are thus thread safe. It is not likely that
    /// locking is needed because the object will typically be used within a single thread.</para>
    /// </remarks>
    /// <typeparam name="InterpreterType">Type of the interpreter for which this class provides command thread data.</typeparam>
    /// $A Igor Sep15;
    public class CommandThread<InterpreterType> :
        CommandThread<InterpreterType, CommandStackFrame<InterpreterType>, CommandThread<InterpreterType>>
        where InterpreterType : class, ICommandLineApplicationInterpreter
    {
        public CommandThread(InterpreterType interpreter) : base(interpreter) { }

        /// <summary>Creates a new stack frame.</summary>
        /// <param name="type">Type of code block for which stack frame is created.</param>
        /// <param name="stackLevel">Level of the stack frame.</param>
        protected override CommandStackFrame<InterpreterType> CreateFrame(CodeBlockType type, int stackLevel)
        {
            return new CommandStackFrame<InterpreterType>(type, this, stackLevel);
        }

    }  // class CommandThread<InterpreterType>




    /// <summary>Stack frame for a block of command-line interpreter commands.</summary>
    /// <remarks>
    /// <para>Usually properties, index operators and methods are not thread safe. Variants whose names end with "Locked"
    /// implement locking on the <see cref="Lock"/> property and are thus thread safe. It is not likely that
    /// locking is needed because the object will typically be used within a single thread.</para>
    /// </remarks>
    /// <typeparam name="InterpreterType">Type of the interpreter for which this class provides a stack frame.</typeparam>
    /// <typeparam name="ThreadType">Type of the thread object on which the stack frames are installed.</typeparam>
    /// $A Igor Sep15;
    public abstract class CommandStackFrame<InterpreterType, ThreadType> : CommandStackFrameBase // : ILockable
        where InterpreterType : class, ICommandLineApplicationInterpreter
        where ThreadType : CommandThreadBase
    {

        /// <summary>Prevent default constructor of being called.</summary>
        private CommandStackFrame()
            : this(CodeBlockType.None, null, 0)
        { }

        /// <summary>Constructor, sets the block type.</summary>
        /// <param name="blockType">Type of the code block represented by the current stack frame.</param>
        /// <param name="thread">Command thread on which this frame is installed.</param>
        /// <param name="stackLevel">Stack level of the current stack frame (growing from 0 upwards).</param>
        public CommandStackFrame(CodeBlockType blockType, ThreadType thread, int stackLevel)
        {
            this.BlockType = blockType;
            this.InterpreterThread = thread;
            this.StackLevel = stackLevel;
        }

        #region Data.GenericTypesBypass

        /// <summary>Returns the commans-line interpreter to which the current command thread belongs.
        /// <para>WARNING: This method is intended for use in the base classes; use more specific (type 
        /// dependent) methods in derived classes.</para></summary>
        public override ICommandLineApplicationInterpreter GetInterpreterBase()
        { return Interpreter; }

        /// <summary>Returns the stack frame of the specified level for the current thread.
        /// <para>WARNING: This method is intended for use in the base classes; use more specific (type 
        /// dependent) methods in derived classes.</para></summary>
        /// <param name="level">Level of the stack frame to be returned.
        /// <para>Command thread (if not replicated for another one) begins executon at stack level 0, then each 
        /// command block or function call increments the stack level by creating a new stack frame.</para></param>
        public override CommandThreadBase GetThreadBase()
        { return InterpreterThread; }

        #endregion Data.GenericTypesBypass


        private ThreadType _interpreterThread = null;

        /// <summary>Interpretation thread that contains the current stack frame.</summary>
        public ThreadType InterpreterThread
        { get { return this._interpreterThread; } protected set { this._interpreterThread = value; } }

        /// <summary>Returns the sibling stack frame of the specified level.</summary>
        /// <param name="stackLevel">Level of the sibling stack frame.</param>
        public abstract CommandStackFrame<InterpreterType, ThreadType> GetThreadStackFrame(int stackLevel);

        /// <summary>Returns the previous (parent, or one lower level) stack frame of the current stack frame.</summary>
        public abstract CommandStackFrame<InterpreterType, ThreadType> GetParentStackFrame();

        /// <summary>Returns the commandline interpreter of the crrent stack frame.</summary>
        public abstract InterpreterType Interpreter { get; }


    }  // abstract class CommandStackFrame<InterpreterType, ThreadType>


    /// <summary>Base class for classes of type CommandStackFrame{InterpreterType, ThreadType}see cref=""/>.
    /// Contains everyting that does not depend on specific type of generic parameters.</summary>
    public abstract class CommandStackFrameBase
    {

        public CommandStackFrameBase() {  }


        #region Data.GenericTypesBypass

        /// <summary>Returns the commans-line interpreter to which the current command thread belongs.
        /// <para>WARNING: This method is intended for use in the base classes; use more specific (type 
        /// dependent) methods in derived classes.</para></summary>
        public abstract ICommandLineApplicationInterpreter GetInterpreterBase();

        /// <summary>Returns the stack frame of the specified level for the current thread.
        /// <para>WARNING: This method is intended for use in the base classes; use more specific (type 
        /// dependent) methods in derived classes.</para></summary>
        /// <para>Command thread (if not replicated for another one) begins executon at stack level 0, then each 
        /// command block or function call increments the stack level by creating a new stack frame.</para></param>
        public abstract CommandThreadBase GetThreadBase();

        #endregion Data.GenericTypesBypass


        #region ILockable

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ILockable


        private int _stackLevel = 0;

        public int StackLevel
        { get { return _stackLevel; } protected set { _stackLevel = value; } }

        #region  BlockEnterAndExit

        // This part controls blodk entries and exits even if the comands are not executed.

        private List<string> _blockEnterCommands = null;

        private List<string> _blockExitCommands = null;

        private List<string> _blockExitCommandsNoLevelffect = null;  // for handling If/Else... blocks

        /// <summary>A list of commands that can enter the type of the current code block.</summary>
        protected List<string> BlockEnterCommands
        {
            get
            {
                if (_blockEnterCommands == null)
                    _blockEnterCommands = new List<string>();
                return _blockEnterCommands;
            }
        }

        /// <summary>A list of commands that can exit the type of the current code block.</summary>
        protected List<string> BlockExitCommands
        {
            get
            {
                if (_blockExitCommands == null)
                    _blockExitCommands = new List<string>();
                return _blockExitCommands;
            }
        }

        /// <summary>A list of commands that can exit the type of the current code block, but willl nnot have effect on
        /// the quet wxit/entry level.</summary>
        /// <remarks>Example is If/Else/Endif blocks. Both Else and Endif can exit the If block, but in quiet mode,
        /// only Endif should decrease the quiet entry/exit level (becauee only If increases it, and If always have the
        /// cottesponding EndIf).</remarks>
        protected List<string> BlockExitCommandsNoLevelEffect
        {
            get
            {
                if (_blockExitCommandsNoLevelffect == null)
                    _blockExitCommandsNoLevelffect = new List<string>();
                return _blockExitCommandsNoLevelffect;
            }
        }

        /// <summary>Adds the specified strings to the list of commands that can enter the current kind of the code block.
        /// <para>Warnig: Kind of the code block is not defined by the <see cref="CodeBlockType"/> in this context,
        /// because each type listed in the enumeration can refer to several customly defined subtypes of code blocks.</para></summary>
        /// <remarks>This list is used as list of commands to be monitored when in the non-executable mode (where commands are not
        /// actually executed) in order to know when the command is hit that will end the block, and therefore exit the 
        /// block that is in non-executable more.</remarks>
        /// <param name="commands">Commands that are added to the list of commands that can enter the current kind of block.</param>
        public void AddBlockEnterCommands(params string[] commands)
        {
            if (commands != null)
                foreach (string cmd in commands)
                    BlockEnterCommands.Add(cmd);
        }

        /// <summary>Adds the specified strings to the list of commands that can exit the current kind of the code block.
        /// <para>Warnig: Kind of the code block is not defined by the <see cref="CodeBlockType"/> in this context,
        /// because each type listed in the enumeration can refer to several customly defined subtypes of code blocks.</para></summary>
        /// <remarks>This list is used as list of commands to be monitored when in the non-executable mode (where commands are not
        /// actually executed) in order to know when the command is hit that will end the block, and therefore exit the 
        /// block that is in non-executable more.</remarks>
        /// <param name="commands">Commands that are added to the list of commands that can enter the current kind of block.</param>
        public void AddBlockExitCommands(params string[] commands)
        {
            if (commands != null)
                foreach (string cmd in commands)
                    BlockExitCommands.Add(cmd);
        }

        /// <summary>Adds the specified strings to the list of commands that can exit the current kind of the code block, but don't
        /// affect the quiet entry/exit level.
        /// <para>Warnig: Kind of the code block is not defined by the <see cref="CodeBlockType"/> in this context,
        /// because each type listed in the enumeration can refer to several customly defined subtypes of code blocks.</para></summary>
        /// <remarks>This list is used as list of commands to be monitored when in the non-executable mode (where commands are not
        /// actually executed) in order to know when the command is hit that will end the block, and therefore exit the 
        /// block that is in non-executable more.
        /// 
        /// Example is If/Else/Endif blocks. Both Else and Endif can exit the If block, but in quiet mode,
        /// only Endif should decrease the quiet entry/exit level (becauee only If increases it, and If always have the
        /// cottesponding EndIf).</remarks>
        /// <param name="commands">Commands that are added to the list of commands that can enter the current kind of block.</param>
        public void AddBlockExitCommandsNoLevelEffect(params string[] commands)
        {
            if (commands != null)
                foreach (string cmd in commands)
                    BlockExitCommandsNoLevelEffect.Add(cmd);
        }

        /// <summary>Returns the array of commands that can enter the current kind of code block.</summary>
        public string[] GetBlockEnterCommands()
        { return BlockEnterCommands.ToArray(); }

        /// <summary>Returns the array of commands that can exit the current kind of code block.</summary>
        public string[] GetBlockExitCommands()
        { return BlockExitCommands.ToArray(); }

        /// <summary>Returns the array of commands that can exit the current kind of code block but have no level effect.</summary>
        public string[] GetBlockExitCommandsNoLevelEffect()
        { return BlockExitCommandsNoLevelEffect.ToArray(); }

        /// <summary>Returns true if the specified commandline can eventually represent one of the commands contained in the
        /// specified list of commands.
        /// <para>The conclusion returned is not definitive because it does not take into account whether the interpreter is
        /// case sensitive or not. It does check, however, that the command name is the first non-whitespace substring appearing in
        /// the command, and that it is separate by whitespace characters from eventual subsequeent parts of the string.</para></summary>
        /// <param name="commandLine">Command lne that is check for whetherr it can represent one of the commands listed.</param>
        /// <param name="commands">A list of command names that are checked.</param>
        /// <param name="isOnlyCommandName">If true then <paramref name="commandLine"/> represents only the command name (stripped 
        /// off eventual arguments and whitespace).</param>
        protected bool IsEventualListedCommand(string commandLine, List<string> commands, bool isOnlyCommandName = false)
        {
            bool canBeListedCommand = false;
            int numCommands = 0;
            if (commands != null)
                numCommands = commands.Count;
            for (int whichCommand = 0; whichCommand < numCommands && !canBeListedCommand; ++whichCommand)
            {
                string cmd = commands[whichCommand];
                if (isOnlyCommandName)
                {
                    if (string.Compare(commandLine, cmd, StringComparison.OrdinalIgnoreCase) == 0)
                        canBeListedCommand = true;
                }
                else
                {
                    int firstIndex = commandLine.IndexOf(cmd, StringComparison.OrdinalIgnoreCase);
                    if (firstIndex >= 0)
                    {
                        // commandLine could represent one of the commands. Further check wheter command name is the frst substring
                        // (after ignoring whitecharacters):
                        string[] cutCommandline = UtilStr.GetArgumentsArray(commandLine);
                        if (cutCommandline != null)
                        {
                            if (cutCommandline.Length > 0)
                                if (string.Compare(cutCommandline[0], cmd, StringComparison.OrdinalIgnoreCase) == 0)
                                    canBeListedCommand = true;
                        }
                    }
                }
            }
            return canBeListedCommand;
        }

        /// <summary>Returns true if the specified commandline can eventually represent one of the commands contained in the
        /// list of possible block enter commands for the current kind of code block.
        /// <para>The conclusion returned is not definitive because it does not take into account whether the interpreter is
        /// case sensitive or not. It does check, however, that the command name is the first non-whitespace substring appearing in
        /// the command, and that it is separate by whitespace characters from eventual subsequeent parts of the string.</para></summary>
        /// <param name="commandLine">Command lne that is check for whetherr it can represent one of the commands listed.</param>
        /// <param name="isOnlyCommandName">If true then <paramref name="commandLine"/> represents only the command name (stripped 
        /// off eventual arguments and whitespace).</param>
        public bool IsEventualBlockEnterCommand(string commandLine, bool isOnlyCommandName = false)
        {
            return IsEventualListedCommand(commandLine, BlockEnterCommands, isOnlyCommandName);
        }

        /// <summary>Returns true if the specified commandline can eventually represent one of the commands contained in the
        /// list of possible block exit commands for the current kind of code block.
        /// <para>The conclusion returned is not definitive because it does not take into account whether the interpreter is
        /// case sensitive or not. It does check, however, that the command name is the first non-whitespace substring appearing in
        /// the command, and that it is separate by whitespace characters from eventual subsequeent parts of the string.</para></summary>
        /// <param name="commandLine">Command lne that is check for whetherr it can represent one of the commands listed.</param>
        /// <param name="isOnlyCommandName">If true then <paramref name="commandLine"/> represents only the command name (stripped 
        /// off eventual arguments and whitespace).</param>
        public bool IsEventualBlockExitCommand(string commandLine, bool isOnlyCommandName = false)
        {
            return IsEventualListedCommand(commandLine, BlockExitCommands, isOnlyCommandName);
        }

        /// <summary>Returns true if the specified commandline can eventually represent one of the commands contained in the
        /// list of possible block exit commands that do not affect quiet entry/exit level, for the current kind of code block.
        /// <para>The conclusion returned is not definitive because it does not take into account whether the interpreter is
        /// case sensitive or not. It does check, however, that the command name is the first non-whitespace substring appearing in
        /// the command, and that it is separate by whitespace characters from eventual subsequeent parts of the string.</para></summary>
        /// <param name="commandLine">Command lne that is check for whetherr it can represent one of the commands listed.</param>
        /// <param name="isOnlyCommandName">If true then <paramref name="commandLine"/> represents only the command name (stripped 
        /// off eventual arguments and whitespace).</param>
        public bool IsEventualBlockExitCommandNoLevelEffect(string commandLine, bool isOnlyCommandName = false)
        {
            return IsEventualListedCommand(commandLine, BlockExitCommandsNoLevelEffect, isOnlyCommandName);
        }


        protected int _quietBlockLevel = 0;

        /// <summary>Indicates the quiet entry/exit level of the current kind of block when commands are not executed.
        /// <para>The level is obtained by counting the block entry and block exit commands encountered (which were,
        /// nonetheless, not executed, because command execution is switched off)</para></summary>
        public int QuietBlockEntryLevel { get { return _quietBlockLevel; } protected set { _quietBlockLevel = value; } }


        /// <summary>Inspects the specified commandline and checks whether it can represent a block enter or a block exit
        /// command for the currennt code block.</summary>
        /// <param name="commandLine">Commandline to be checked.</param>
        /// <param name="justCheck">If true then the function only performs the check, but does not increment or
        /// decrement the level of encountered current block entries/exits. Default is false.</param>
        /// <param name="isOnlyCommandName">If true then <param>commandLine</param> is treated as command name, i.e.
        /// it should not contain additional arguments and / or whitespaces.</param>
        /// <returns>True if the specfied commandline can eventually represent the current non-executing code block 
        /// entry or exit command.</returns>
        public bool CheckForBlockEnterOrExitCommand(string commandLine, bool justCheck = false, bool isOnlyCommandName = false)
        {
            bool isEnterOrExitCommand = false;
            if (DoExecuteCommands)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine
                    + "WARNING: CheckForEnterOrExitCommand called when commands are actually executed. " + Environment.NewLine
                    + "  The call is IGNORED." + Environment.NewLine + Environment.NewLine);
            }
            else
            {
                if (IsEventualBlockEnterCommand(commandLine, isOnlyCommandName))
                {
                    isEnterOrExitCommand = true;
                    if (!justCheck)
                        ++QuietBlockEntryLevel;
                }
                else if (IsEventualBlockExitCommand(commandLine, isOnlyCommandName))
                {
                    isEnterOrExitCommand = true;
                    if (!justCheck)
                        --QuietBlockEntryLevel;
                }
                else if (IsEventualBlockExitCommandNoLevelEffect(commandLine, isOnlyCommandName))
                {
                    isEnterOrExitCommand = true;
                    //if (!justCheck)
                    //    --QuietBlockEntryLevel;
                }
            }
            return isEnterOrExitCommand;
        }

        /// <summary>Returns true if the specified command should be executed as block exit command, in spite of the 
        /// fact that commannd execution is currently switched off on the current code block.
        /// <para>This makes possible to hit the block exit command for code blocks that have been entered, but command execution
        /// was then switched off (e.g. in order to defer command evaluation after all commands are known).</para>
        /// <para>This method only perfomrs checks and does not increase or decrease the quied block entry/exit level.</para>
        /// <para>If command execution is switched on then the method returnes false (in order to save time for actual checks).</para></summary>
        /// <param name="commandLine">Commadline that is checked.</param>
        /// <param name="isOnlyCommandName">If true then <paramref name="commandLine"/> is treated as just the command name,
        /// i.e. it must not contain eventual whitespaces or parameters.</param>
        public bool IsBlockExitQuietCommand(string commandLine, bool isOnlyCommandName = false)
        {
            return (!DoExecuteCommands && QuietBlockEntryLevel <= 0 &&
                (IsEventualBlockExitCommand(commandLine, isOnlyCommandName) ||
                 IsEventualBlockExitCommandNoLevelEffect(commandLine, isOnlyCommandName)));
        }


        #endregion BlockEnterAndExit


        #region Data.Execution 

        // Data that controls execution of the iinterpreter, and are bound to stack frames.
        
        private CodeBlockType _blockType = CodeBlockType.None;

        /// <summary>Type of the code block represented by the current stack frame.</summary>
        public CodeBlockType BlockType
        {
            get { return _blockType; }
            protected set { _blockType = value; }
        }

        private bool _doExecute = true;

        /// <summary>Whether commands are executed in the current code block.</summary>
        public bool DoExecuteCommands { get { return _doExecute; } set { _doExecute = value; } }

        private bool _isBranchExecuted = false;

        /// <summary>Whether a branch has already been executed.</summary>
        public bool WasBranchAlreadyExecuted { get { return _isBranchExecuted; } set { _isBranchExecuted = value; } }

        private int _numExitLevels = 0;

        public int NumExitLevels { get { return _numExitLevels; } set { _numExitLevels = value; } }

        private int _loopCount = 0;

        /// <summary>Whether commands are executed in the current code block.</summary>
        public int LoopCount { get { return _loopCount; } set { _loopCount = value; } }

        private bool _suppressInteractive = false;

        /// <summary>Indicates that the interactive mode is temporarily represset.</summary>
        public bool SuppressInteractive
        {
            get { return _suppressInteractive; }
            set { _suppressInteractive = value; }
        }

        private string _conditionExpression = null;

        /// <summary>For loopig and branching blocks, this property contains the condition expression of the block,
        /// which was a part of the block command (such as While, If, or ElseIf).</summary>
        public string ConditionExpression
        {
            get { return _conditionExpression; }
            set { _conditionExpression = value; }
        }

        private string _blockCommandLine = null;

        /// <summary>Commandline that started the current block. It may have been saved when the current block of 
        /// code was entered, in order to be used within the block (e.g. for inspection and debugging) or when
        /// the block exits. Typical use is in blocks with deferred evaluation, such as in While block, where
        /// commandlines within the block are stored first, and then repeatedly evaluated until the condition
        /// from the While block entering command is fulfilled.</summary>
        public string BlockCommanddLine { get { return _blockCommandLine; } set { _blockCommandLine = value; } }

        #endregion Data.Execution


        #region Data.Operation

        // Data that affects operation of the interpreter (data that interpreter and its commands woulld want
        // to save or exchange on stack frames).

        private bool _doSave = false;

        /// <summary>Whether commands are saved to the command store in the current code block.</summary>
        public bool DoSaveCommands { get { return _doSave; } set { _doSave = value; } }

        private List<string> _commandLines = null;

        public List<string> CommandLines
        {
            get
            {
                if (_commandLines == null)
                {
                    lock (Lock)
                    {
                        if (_commandLines == null)
                            _commandLines = new List<string>();
                    }
                }
                return _commandLines;
            }
        }

        private string _lastCommandLine = null;

        public string LastCommandLine { get { return _lastCommandLine; } set { _lastCommandLine = value; } }

        /// <summary>Local variables.</summary>
        protected SortedDictionary<string, InterpreterVariable> _variables = new SortedDictionary<string, InterpreterVariable>();

        private string _returnedValue = null;

        /// <summary>Value returned from the last executed command.</summary>
        public string ReturnedValue
        { get { return _returnedValue; } set { _returnedValue = value; } }

        //private string _returnedBlockValue = null;

        ///// <summary>Value returned from a code block.</summary>
        //public string ReturnedBlockValue
        //{ get { return _returnedBlockValue; } set { _returnedBlockValue = value; } }

        protected List<string> _auxVarNames = null;

        /// <summary>Gets names of all variables that are defined on the current stack frame.
        /// <para>Operation is locked.</para></summary>
        public string[] VariableNames
        {
            get
            {
                lock (Lock)
                {
                    if (_auxVarNames == null)
                        _auxVarNames = new List<string>();
                    _auxVarNames.Clear();
                    var keys = _variables.Keys;
                    foreach (string key in keys)
                    {
                        _auxVarNames.Add(key);
                    }
                    return _auxVarNames.ToArray();
                }
            }
        }

        /// <summary>Gets names of all variables that are defined on the current stack frame.
        /// <para>Operation is locked.</para></summary>
        public string[] VariableValueStrings
        {
            get
            {
                lock (Lock)
                {
                    if (_auxVarNames == null)
                        _auxVarNames = new List<string>();
                    _auxVarNames.Clear();
                    var keys = _variables.Keys;
                    foreach (string key in keys)
                    {
                        try
                        {
                            _auxVarNames.Add(key + "=" + this[key].StringValue);
                        }
                        catch { _auxVarNames.Add(key + "=" + "ERROR:Undefined!!!"); }
                    }
                    return _auxVarNames.ToArray();
                }
            }
        }

        /// <summary>Gets or sets value of the variable with the specified name.</summary>
        /// <param name="variableName">Name of the variable to be set or retrieved.</param>
        public InterpreterVariable this[string variableName]
        {
            get { if (_variables.ContainsKey(variableName)) return _variables[variableName]; else return null; }
            set { _variables[variableName] = value; }
        }

        /// <summary>Returns true if the specified variable is defined, false if it is not.</summary>
        /// <param name="varName">Name of the variable that is queried.</param>
        public bool IsVariableDefined(string varName)
        {
            return _variables.ContainsKey(varName);
        }

        /// <summary>Returns true if the specified variable is defined, false if it is not.
        /// <para>Thread safe variant, lock is aquired on <see cref="Lock"/>.</para></summary>
        /// <param name="varName">Name of the variable that is queried.</param>
        public bool IsVariableDefinedLocked(string varName)
        {
            lock (Lock)
            {
                return _variables.ContainsKey(varName);
            }
        }

        /// <summary>Returns variable object (definition) for the variable with specified name.
        /// <para>null is returned if the variable does not exist.</para></summary>
        /// <param name="varName">Name of the variable.</param>
        public InterpreterVariable GetVariableDef(string varName)
        {
            return this[varName];
        }

        /// <summary>Returns value of the specified variable.</summary>
        /// <param name="varName">Name of the variable.</param>
        public string GetVariableValue(string varName)
        {
            return this[varName].StringValue;
        }

        /// <summary>Gets the variable within lock on the <see cref="Lock"/> property and returns it.
        /// <para>Thread safe variant, lock is aquired on <see cref="Lock"/>.</para></summary>
        /// <param name="varName">Name of the variable whose value is to be reurned.</param>
        public string GetVariableValueLocked(string varName)
        {
            lock (Lock)
            {
                return this[varName].StringValue;
            }
        }


        /// <summary>Sets value of the specified variable.</summary>
        /// <param name="varName">Name of the variable to be set.</param>
        /// <param name="varValue">Value to be assigned to the specified variable.</param>
        /// <param name="flags">Variable flags (optional).</param>
        public void SetVariableValue(string varName, string varValue, VariableFlags flags = VariableFlags.Default)
        {
            if (IsVariableDefined(varName))
            {
                InterpreterVariable intVar = this[varName];
                if (flags != VariableFlags.Default && intVar.Flags != flags)
                    throw new Exception("Variable " + varName + " exists and has different flags than specified by the set method." + Environment.NewLine
                        + "  variable's flags: " + intVar.Flags.ToString() + ", specified: " + flags);
                intVar.StringValue = varValue;
            }
            else
                this[varName] = new InterpreterVariable(varName, StackLevel, varValue, flags);
        }

        /// <summary>Sets the variable within lock on the property.
        /// <para>Thread safe variant, lock is aquired on <see cref="Lock"/>.</para></summary>
        /// <param name="varName">Name of the variable whose value is to be set.</param>
        /// <param name="varValue">Value to be assigned to the variable.</param>
        /// <param name="flags">Variable flags (optional).</param>
        public void SetVariableValueLocked(string varName, string varValue, VariableFlags flags = VariableFlags.Default)
        {
            lock (Lock)
            {
                if (IsVariableDefined(varName))
                {
                    InterpreterVariable intVar = this[varName];
                    if (flags != VariableFlags.Default && intVar.Flags != flags)
                        throw new Exception("Variable " + varName + " exists and has different flags than specified by the set method." + Environment.NewLine
                            + "  variable's flags: " + intVar.Flags.ToString() + ", specified: " + flags);
                    intVar.StringValue = varValue;
                }
                else
                    this[varName] = new InterpreterVariable(varName, StackLevel, varValue, flags);
            }
        }


        /// <summary>Removes the specified variable.</summary>
        /// <param name="varName">Name of the variable to be removed.</param>
        public void RemoveVariable(string varName)
        {
            _variables.Remove(varName);
        }

        /// <summary>Removes the specified variable within lock on the property.
        /// <para>Thread safe variant, lock is aquired on <see cref="Lock"/>.</para></summary>
        /// <param name="varName">Name of the variable whose value is to be set.</param>
        public void RemoveVariableLocked(string varName)
        {
            lock (Lock)
            {
                _variables.Remove(varName);
            }
        }


        #endregion Data.Operaton



        public string ToStringVariableNames()
        {
            return Util.ToString<String>(VariableNames, false);
        }

        public string ToStringVariableValues()
        {
            return Util.ToString<String>(VariableValueStrings, false);
        }

        /// <summary>Returns a string containing information about the current command thread.</summary>
        public virtual string ToString(bool includeThreadInfo = true, bool includeLocalVariables = true,
            bool includeGlobalVariables = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Command stack frame: ");
            sb.AppendLine("  Stack level: " + this.StackLevel);
            sb.AppendLine("  Block type: " + BlockType.ToString());
            sb.AppendLine("  DoExecute: " + DoExecuteCommands);
            sb.AppendLine("  DoSaveCommands: " + DoSaveCommands);
            sb.AppendLine("  Number of saved commands: " + CommandLines.Count);
            sb.AppendLine("  LoopCount: " + LoopCount);
            sb.AppendLine("  ConditionExpression: " + ConditionExpression);
            sb.AppendLine("  LoopCount: " + LoopCount);
            sb.AppendLine("  Last commandline: " + LastCommandLine);
            sb.AppendLine("  Block commandline: " + BlockCommanddLine);
            sb.AppendLine("  Condition expression: " + ConditionExpression);
            if (CommandLines == null)
                sb.AppendLine("  Saved commandlines: null.");
            else
            {
                int num = CommandLines.Count;
                if (num < 1)
                    sb.AppendLine("  Saved commmandlines: empty.");
                else
                {
                    sb.AppendLine("  Saved commandlines: ");
                    for (int i = 0; i < num; ++i)
                        sb.AppendLine("    " + i + ": " + CommandLines[i]);
                }
            }

            try
            {
                if (includeThreadInfo)
                {
                    sb.AppendLine(Environment.NewLine + GetInterpreterBase().ToString());
                }
            }
            catch { }

            try
            {
                if (includeLocalVariables)
                {
                    sb.AppendLine("  Stack frame variables: " + Environment.NewLine
                        + "    " + ToStringVariableNames() + Environment.NewLine
                        + "  Variables with values: " + Environment.NewLine
                        + "    " + ToStringVariableValues());
                }
                var thread = GetInterpreterBase();
                int level = StackLevel - 1;
                if (level < 0)
                    sb.AppendLine("  This is the lowest stack frame.");
                else
                {
                    while (level >= 0)
                    {
                        try
                        {
                            // CommandStackFrame<InterpreterType, ThreadType> lowerFrame = GetSiblingStackFrame(level);

                            CommandStackFrameBase lowerFrame = GetThreadBase().GetStackFrameBase(level);

                            sb.AppendLine("  Stack frame variables for stack level " + level + ": " + Environment.NewLine
                                + "    " + lowerFrame.ToStringVariableNames() + Environment.NewLine
                                + "  Variables with values: " + Environment.NewLine
                                + "    " + lowerFrame.ToStringVariableValues());
                        }
                        catch { sb.AppendLine("  Could not obtain variable information for stack level " + level + "."); }
                        --level;
                    }
                }
            }
            catch { }

            if (includeGlobalVariables)
            {
                try
                {
                    ICommandLineApplicationInterpreter interpreter = this.GetInterpreterBase();

                    var globalFrame = interpreter.GlobalFrame;

                    sb.AppendLine("  Global interpreter variables: " + Environment.NewLine
                        + "    " + globalFrame.ToStringVariableNames() + Environment.NewLine
                        + "  Variables with values: " + Environment.NewLine
                        + "    " + globalFrame.ToStringVariableValues());
                }
                catch (Exception ex)
                {
                    sb.AppendLine("  Error when accessing interpreter global variables: " +
                        Environment.NewLine + "  " + ex.Message);
                }
            }


            return sb.ToString();
        }


        /// <summary>Returns string representation of the current object.</summary>
        public override string ToString()
        {
            return ToString(includeThreadInfo: true, includeLocalVariables: true,
                includeGlobalVariables: true);
        }


    }  // class CommandStackFrameBase



    /// <summary>Contains stack frames and other command thread data for a single command thread of a 
    /// command-line interreter.</summary>
    /// <remarks>
    /// <para>Usually properties, index operators and methods are not thread safe. Variants whose names end with "Locked"
    /// implement locking on the <see cref="Lock"/> property and are thus thread safe. It is not likely that
    /// locking is needed because the object will typically be used within a single thread.</para>
    /// </remarks>
    /// <typeparam name="InterpreterType">Type of the interpreter for which this class provides command thread data.</typeparam>
    /// <typeparam name="FrameType">Type of stack frames that is combined with this type.</typeparam>
    /// <typeparam name="ThreadType">Type of command thread data - represents this very type, to make it easier to use
    /// the frame type (which, in turn, has a type parameter for thread type).</typeparam>
    /// $A Igor Sep15;
    public abstract class CommandThread<InterpreterType, FrameType, ThreadType> : CommandThreadBase,  IIdentifiable, ILockable
        where InterpreterType : class, ICommandLineApplicationInterpreter
        where FrameType : CommandStackFrame<InterpreterType, ThreadType>
        where ThreadType : CommandThread<InterpreterType, FrameType, ThreadType>
    {

        private CommandThread()
            : this(null)
        { }

        public CommandThread(InterpreterType interpreter)
        {
            this.Interpreter = interpreter;
            this.BaseFrame = this.AddFrame(CodeBlockType.Block);
        }


        #region Data.GenericTypesBypass

        /// <summary>Returns the commans-line interpreter to which the current command thread belongs.
        /// <para>WARNING: This method is intended for use in the base classes; use more specific (type 
        /// dependent) methods in derived classes.</para></summary>
        public override ICommandLineApplicationInterpreter GetInterpreterBase()
        { return Interpreter; }

        /// <summary>Returns the stack frame of the specified level for the current thread.
        /// <para>WARNING: This method is intended for use in the base classes; use more specific (type 
        /// dependent) methods in derived classes.</para></summary>
        /// <param name="level">Level of the stack frame to be returned.
        /// <para>Command thread (if not replicated for another one) begins executon at stack level 0, then each 
        /// command block or function call increments the stack level by creating a new stack frame.</para></param>
        public override CommandStackFrameBase GetStackFrameBase(int level)
        { return this[level]; }


        #endregion Data.GenericTypesBypass


        #region Data.Basic

        private InterpreterType _interpreter;

        /// <summary>Returns interpreter that handles command execution on the current command thread.</summary>
        public InterpreterType Interpreter
        {
            get
            {
                if (_interpreter == null) throw new InvalidOperationException("Interpreater is not set on CommandThread.");
                return _interpreter;
            }
            protected set { _interpreter = value; }
        }

        private List<FrameType> _stackFrames = new List<FrameType>();

        /// <summary>A list of stack frames existent in the current thread.</summary>
        protected List<FrameType> StackFrames
        { get { return _stackFrames; } }

        private FrameType _baseFrame = null;

        /// <summary>Base stack frame, created when the thread is initialized and exists until thread is exited.</summary>
        public FrameType BaseFrame
        { get { return _baseFrame; } protected set { _baseFrame = value; } }

        private FrameType _topFrame = null;

        /// <summary>Base stack frame, created when the thread is initialized and exists until thread is exited.</summary>
        public FrameType TopFrame
        { get { return _topFrame; } protected set { _topFrame = value; } }

        public FrameType this[int which]
        {
            get { return StackFrames[which]; }
        }


        protected abstract FrameType CreateFrame(CodeBlockType type, int stackLevel);

        /// <summary>Adds a new stack frame.</summary>
        /// <param name="blockType">Type of the code block represented by the stack frame.</param>
        /// <returns>Stack frame that has been added.</returns>
        public FrameType AddFrame(CodeBlockType blockType)
        {
            FrameType ret = CreateFrame(blockType, TopFrameIndex+1); // new FrameType(blockType, this);
            StackFrames.Add(ret);
            TopFrame = ret;
            ++TopFrameIndex;
            SaveNumStoredParameters();
            return ret;
        }



        /// <summary>Removes the last stack frame.</summary>
        /// <returns>The stack frame that has been removed, so it can be used for inspection.</returns>
        public FrameType RemoveFrame()
        {
            FrameType ret = null;
            int which = StackFrames.Count - 1;
            if (which == 0)
                throw new InvalidOperationException("Can not remove the base stack frame.");
            if (which < 0)
                throw new InvalidOperationException("The current command thread does not contain any stack frames.");
            ret = StackFrames[which];
            StackFrames.RemoveAt(which);
            --which;
            TopFrame = StackFrames[which];
            --TopFrameIndex;
            RestorePreviousNumStoredParameters(ret);
            return ret;
        }


        #endregion Data.Basic


    }  // abstract class CommandThread<InterpreterType, FrameType, ThreadType>





    /// <summary>Base class for classes of type <see cref="CommandThread{InterpreterType, FrameType, ThreadType}"/>.
    /// Contains everything that is not dependent on generic frame and other parameters.</summary>
    public abstract class CommandThreadBase: IIdentifiable, ILockable
    {

        public CommandThreadBase() { }

        #region Data.GenericTypesBypass

        /// <summary>Returns the commans-line interpreter to which the current command thread belongs.
        /// <para>WARNING: This method is intended for use in the base classes; use more specific (type 
        /// dependent) methods in derived classes.</para></summary>
        public abstract ICommandLineApplicationInterpreter GetInterpreterBase();

        /// <summary>Returns the stack frame of the specified level for the current thread.
        /// <para>WARNING: This method is intended for use in the base classes; use more specific (type 
        /// dependent) methods in derived classes.</para></summary>
        /// <param name="level">Level of the stack frame to be returned.
        /// <para>Command thread (if not replicated for another one) begins executon at stack level 0, then each 
        /// command block or function call increments the stack level by creating a new stack frame.</para></param>
        public abstract CommandStackFrameBase GetStackFrameBase(int level);


        #endregion Data.GenericTypesBypass



        #region Data.Basic


        private int _topFrameIndex = -1;

        public int TopFrameIndex
        {
            get { return _topFrameIndex; }
            protected set { _topFrameIndex = value; }
        }


        #endregion Data.Basic


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking


        #region IIdentifiable

        private static object _lockIdThread;

        /// <summary>Lock used for acquiring IDs.</summary>
        public static object LockIdThread
        {
            get
            {
                if (_lockIdThread == null)
                {
                    lock (Util.LockGlobal)
                    {
                        if (_lockIdThread == null)
                            _lockIdThread = new object();

                    }
                }
                return _lockIdThread;
            }
        }

        private static int _nextIdThread = 0;

        /// <summary>Returns another ID that is unique for objects of the containing class 
        /// its and derived classes.</summary>
        protected static int GetNextIdThread()
        {
            lock (LockIdThread)
            {
                ++_nextIdThread;
                return _nextIdThread;
            }
        }

        private int _id = GetNextIdThread();

        /// <summary>Unique ID for objects of the currnet and derived classes.</summary>
        public virtual int Id
        { get { return _id; } }

        #endregion IIdentifiable



        #region Data.Operational

        private bool _wasCommandExecuted = false;

        /// <summary>Auxiliary flag that indicates whether the last command launched on the current thread has actually been executed.
        /// <para>This flag is used for communication between the calling environment and the method that takes care of
        /// final invocation of commands.</para></summary>
        public bool WasCommandExecuted
        {
            get { return _wasCommandExecuted; }
            set
            {
                _wasCommandExecuted = value;
                if (value == false)
                    WasBlockEnterCommand = false;
                WasBlockExitCommand = false;
            }
        }

        private bool _wasBlockEnterCommand = false;

        /// <summary>Auxiliary flag that indicates whether the last executed command was a block enter command.
        /// <para>This flag is cleared when <see cref="WasCommandExecuted"/> is cleared, and is set by 
        /// block entering commands themselves.</para></summary>
        public bool WasBlockEnterCommand
        {
            get { return _wasBlockEnterCommand; }
            set { _wasBlockEnterCommand = value; }
        }

        private bool _wasBlockExitCommand = false;

        /// <summary>Auxiliary flag that indicates whether the last executed command was a block exit command.
        /// <para>This flag is cleared when <see cref="WasCommandExecuted"/> is cleared, and is set by 
        /// block exiting commands themselves.</para></summary>
        public bool WasBlockExitCommand
        {
            get { return _wasBlockExitCommand; }
            set { _wasBlockExitCommand = value; }
        }

        private List<string> _storedPrompts = null;

        /// <summary>Auxiliary list where calling environment can store prompts used e.g. in interactive mode</summary>
        /// <remarks>Currently NOT USED!</remarks>
        private List<String> StoredPrompts
        {
            get
            {
                if (_storedPrompts == null)
                    _storedPrompts = new List<string>();
                return StoredPrompts;
            }
        }

        /// <summary>Stores a string (prompt) to the end of the list of stored prompts.</summary>
        /// <remarks>Currently NOT USED!</remarks>
        /// <param name="prompt">Prompt string to be stored.</param>
        public void StorePrompt(string prompt)
        {
            StoredPrompts.Add(prompt);
        }

        /// <summary>Restores a string (prompt) from the end of the list of stored prompts.</summary>
        /// <remarks>Currently NOT USED!</remarks>
        public string RestorePrompt()
        {
            string ret = null;
            int which = StoredPrompts.Count - 1;
            if (which >= 0)
            {
                ret = StoredPrompts[which];
                StoredPrompts.RemoveAt(which);
            }
            return ret;
        }


        #endregion Data.Operational


        #region Data.StoredParameters

        // This defines a stack of parameters that commands can send ot each other.
        // For example, command for beginning of a code block can store its parameters 
        // for the command that ends the block and does something with the code.
        // For example see BeginRepeatBlock(...) and ExitRepeatBlock(...) in CommandLineApplicationInterpreter.

        private List<object> _parametersStore = null;

        /// <summary>Stored objects.</summary>
        private List<object> ParameterStore
        {
            get
            {
                if (_parametersStore == null)
                {
                    lock (Lock)
                    {
                        if (_parametersStore == null)
                            _parametersStore = new List<object>();
                    }
                }
                return _parametersStore;
            }
        }

        /// <summary>Stores a new parameter at the end of the parameter store.
        /// <para>WARNING: Users must make sure that <see cref="PushParameter"/> and <see cref="PopParameter"/> are
        /// properrly called in pairs, otherwise parameter store will ger corrupted and unusable for otherr users.</para></summary>
        /// <param name="param">Parameter that is added to parameter store.</param>
        public void PushParameter(object param)
        {
            ParameterStore.Add(param);
        }

        /// <summary>Removae and returns the last object on the parameter store.
        /// <para>WARNING: Users must make sure that <see cref="PushParameter"/> and <see cref="PopParameter"/> are
        /// properrly called in pairs, otherwise parameter store will ger corrupted and unusable for otherr users.</para>
        /// <para>This method must be called on the same top-level stack frame on which the <see cref="PushParameter"/> for
        /// the corresponding stored parameter was called.</para></summary>
        public object PopParameter()
        {
            int numObjects = ParameterStore.Count;
            if (numObjects <= _lastNumStoredParameters)
                throw new InvalidOperationException("Can not pop another parameter on the current top-level stack frame: " + Environment.NewLine
                    + "  Number of stored parameters is already less or equal to the number before the" + Environment.NewLine 
                    + "  current stack frame was entered. " + Environment.NewLine
                    + "  Some methods must have used stored parameters inappropriately (e.g. popped " + Environment.NewLine 
                    + "  the same parameter twice). " + Environment.NewLine
                    + "    Thread ID: " + Id + Environment.NewLine
                    + "    Top stack frame level: " + TopFrameIndex + Environment.NewLine
                    + "    Number of stored parameters (before executon of pop()): " + numObjects + Environment.NewLine
                    + "    Number of stored parameters before the current frame: " + _lastNumStoredParameters + Environment.NewLine
                    + "    Frame index noted internally: " + _indexOfLastNumStoredParameters+ Environment.NewLine
                    + "    Number of stored parameters before the current frame, obtained another way: " 
                    + _numParamsBeforeFrame[_numParamsBeforeFrame.Count - 1]);
            if (numObjects < 1)
                throw new InvalidOperationException("Thread's parameter store does not have any elements.");
            object ret = ParameterStore[numObjects - 1];
            ParameterStore.RemoveAt(numObjects - 1);
            return ret;
        }



        /// <summary>Returns the stored parameter with the specified index from the top.
        /// <para>This method allows to access parameters that are owned by parent frames (because the <see cref="PopParameter"/>
        /// method can not access parameters that were not added by the current frame).</para></summary>
        /// <param name="whichPlaceFromTop">Specifies which stored parameter, in terms of the place from the 
        /// top of the parameter stack downwards, should be obtained. Index 0 (default) specifies the last (top-most) parameter.</param>
        public object GetParameterFromTop(int whichPlaceFromTop = 0)
        {
            int numObjects = ParameterStore.Count;
            if (numObjects < 1 + whichPlaceFromTop)
                throw new InvalidOperationException("Thread's parameter store does not have enough elements.");
            return ParameterStore[numObjects - 1 - whichPlaceFromTop];
        }

        public int NumStoredParameters { get { return ParameterStore.Count; } }

        private List<int> _numParamsBeforeFrame = new List<int>();

        private int _lastNumStoredParameters = 0;

        private int _indexOfLastNumStoredParameters = -1;

        /// <summary>Saves the current number of stored parameters on the command thread.
        /// <para>This must be called when a new stack frame is added.</para>
        /// <para>If called at other places, it must be consistently called in pair with <see cref="RestorePreviousNumStoredParameters"/>.</para></summary>
        protected virtual void SaveNumStoredParameters()
        {
            _lastNumStoredParameters = NumStoredParameters;
            ++_indexOfLastNumStoredParameters;
            _numParamsBeforeFrame.Add(_lastNumStoredParameters);
        }

        /// <summary>Saves the current number of stored parameters on the command thread.
        /// <para>This must normally be called when a stack frame is removed.</para>
        /// <para>If called at other places, it must be consistently called in pair with <see cref="SaveNumStoredParameters"/>.</para></summary>
        protected virtual void RestorePreviousNumStoredParameters(CommandStackFrameBase frame)
        {
            int count = _numParamsBeforeFrame.Count;
            int savedNumParams = _lastNumStoredParameters;  // how many parameters were stored on the thread before the removed frame was added
            --_indexOfLastNumStoredParameters;
            if (_indexOfLastNumStoredParameters < 0)
                throw new InvalidOperationException("Internal error in keeping track of number of stored parameters: index" + Environment.NewLine
                    + "  of last stored information is less than 0. " + Environment.NewLine 
                    + "  Saves and restores of number of stored parameters were not performed coherently.");
            _lastNumStoredParameters = _numParamsBeforeFrame[_indexOfLastNumStoredParameters];  // information obtained fromprevious save
            _numParamsBeforeFrame.RemoveAt(count - 1);  // remove the information
            if (NumStoredParameters != savedNumParams)
            {
                // Number of stored parameters is not equal than it was before the frame was added:
                throw new InvalidOperationException("Number of command thresd's stored parameters after removal of the current stack frame ("
                    + NumStoredParameters + ") " + Environment.NewLine + "  is not equal to number of parameters before additon of this stack frame ("
                    + savedNumParams + "). " + Environment.NewLine
                    + "  Some methods must have used stored parameters inappropriately (e.g. did not clean its parameters). " + Environment.NewLine
                    + "    Thread ID: " + Id + Environment.NewLine
                    + "    Stack level of the corrupted frame: " + frame.StackLevel);
            }
        }

        #endregion Data.StoredParameters


        #region Data.Auxiliary


        private bool _threadSuppressInteractive = false;

        /// <summary>Indicates that the interactive mode is temporarily represset.</summary>
        public bool SuppressInteractive
        {
            get { return _threadSuppressInteractive; }
            set { _threadSuppressInteractive = value; }
        }

        public static int DefaultOutputLevel
        {
            get { return CommandLineApplicationInterpreter.DefaultOutputLevel; }
            set { CommandLineApplicationInterpreter.DefaultOutputLevel = value; }
        } // = 1;

        private bool _isOutputLevelSet = false;

        public bool IsOutputLevelSet
        {
            get { return _isOutputLevelSet; }
            protected set { _isOutputLevelSet = value; }
        }

        /// <summary>Default level of output for some of the interpreters' functionality (e.g. asynchronous command execution).</summary>
        protected int _outputLevel = DefaultOutputLevel;

        /// <summary>Level of output for some of the interpreter's functionality (e.g. asynchronous command execution).</summary>
        public virtual int OutputLevel
        {
            get
            {
                if (IsOutputLevelSet)
                    return _outputLevel;
                else
                    return GetInterpreterBase().OutputLevel;
            }
            set { _outputLevel = value; }
        }

        public StopWatch1 _timer;

        /// <summary>Gets the stopwatch used for measuring time of commands.
        /// <para>This property always returns an initialized stopwatch.</para></summary>
        public StopWatch1 Timer
        {
            get { if (_timer == null) _timer = new StopWatch1(); return _timer; }
        }

        #endregion Data.Auxiliary


        /// <summary>Returns a string containing information about the current command thread.</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Command interpretation thread: ");
            try
            {
                sb.AppendLine("  Interpreter: " + GetInterpreterBase().Name);
            }
            catch { }
            try
            {
                if (object.ReferenceEquals(this, GetInterpreterBase().MainThread))
                    sb.AppendLine("  This is the MAIN THREAD.");
                else
                    sb.AppendLine("  Not the main thread.");
            }
            catch { }
            try
            {
                ICommandLineApplicationInterpreter interp = this.GetInterpreterBase();
                int threadIndex = -1;
                int numThreads = interp.NumCommandThreads;
                for (int i = 0; i < numThreads; ++i)
                {
                    if (object.ReferenceEquals(this, interp.GetCommmandThread(i)))
                    {
                        threadIndex = i;
                        break;
                    }
                }
                if (threadIndex < 0)
                    sb.AppendLine("  Thread is not contained on interpreter thread collection (which has " + numThreads + " threads).");
                else
                    sb.AppendLine("  Interpreter's thread index: " + threadIndex + " (number of threads: " + numThreads + "(.");

            }
            catch { }


            sb.AppendLine("  Stack depth: " + TopFrameIndex);


            return sb.ToString();
        }


    }  // class CommandThreadBase






}
