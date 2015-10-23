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
//namespace IG.Test
{





}



//namespace IG.Test
////namespace IG.Lib
//{



//    /// <summary>Stack frame for a block of command-line interpreter commands.</summary>
//    /// <remarks>
//    /// <para>Usually properties, index operators and methods are not thread safe. Variants whose names end with "Locked"
//    /// implement locking on the <see cref="Lock"/> property and are thus thread safe. It is not likely that
//    /// locking is needed because the object will typically be used within a single thread.</para>
//    /// </remarks>
//    /// $A Igor Sep15;
//    public class CommandStackFrame : CommandStackFrame<ICommandLineApplicationInterpreter>
//    {

//        public CommandStackFrame(CodeBlockType blockType, CommandThread<ICommandLineApplicationInterpreter> thread) :
//            base(blockType, thread)
//        {

//        }
//    }


//    /// <summary>Contains stack frames for a single command thread of a command-line interreter.</summary>
//    /// <remarks>
//    /// <para>Usually properties, index operators and methods are not thread safe. Variants whose names end with "Locked"
//    /// implement locking on the <see cref="Lock"/> property and are thus thread safe. It is not likely that
//    /// locking is needed because the object will typically be used within a single thread.</para>
//    /// </remarks>
//    /// $A Igor Sep15;
//    public class CommandThread : CommandThread<ICommandLineApplicationInterpreter>
//    {
//        public CommandThread(ICommandLineApplicationInterpreter interpreter) : base(interpreter) { }
//    }


//    /// <summary>Stack frame for a block of command-line interpreter commands.</summary>
//    /// <remarks>
//    /// <para>Usually properties, index operators and methods are not thread safe. Variants whose names end with "Locked"
//    /// implement locking on the <see cref="Lock"/> property and are thus thread safe. It is not likely that
//    /// locking is needed because the object will typically be used within a single thread.</para>
//    /// </remarks>
//    /// $A Igor Sep15;
//    public class CommandStackFrame<InterpreterType> // : ILockable
//        where InterpreterType : class, ICommandLineApplicationInterpreter
//    {

//        /// <summary>Prevent default constructor of being called.</summary>
//        private CommandStackFrame()
//            : this(CodeBlockType.None, null)
//        { }

//        /// <summary>Constructor, sets the block type.</summary>
//        public CommandStackFrame(CodeBlockType blockType, CommandThread<InterpreterType> thread)
//        {
//            this.BlockType = _blockType;
//            this.InterpreterThread = thread;
//        }

//        #region ILockable

//        private object _mainLock = new object();

//        /// <summary>This object's central lock object to be used by other object.
//        /// Do not use this object for locking in class' methods, for this you should use 
//        /// InternalLock.</summary>
//        public object Lock { get { return _mainLock; } }

//        #endregion ILockable

//        /// <summary>Interrpreter that handles interpretation of commands that act on the current stack frame.</summary>
//        public InterpreterType Interpreter
//        {
//            get
//            {
//                return this.InterpreterThread.Interpreter;
//            }
//        }

//        private CommandThread<InterpreterType> _interpreterThread = null;

//        /// <summary>Interpretation thread that contains the current stack frame.</summary>
//        public CommandThread<InterpreterType> InterpreterThread
//        { get { return this._interpreterThread; } protected set { this._interpreterThread = value; } }

//        private CodeBlockType _blockType = CodeBlockType.None;

//        /// <summary>Type of the code block represented by the current stack frame.</summary>
//        public CodeBlockType BlockType
//        {
//            get { return _blockType; }
//            protected set { _blockType = value; }
//        }

//        private bool _doExecute = true;

//        /// <summary>Whether commands are executed in the current code block.</summary>
//        public bool DoExecute { get { return _doExecute; } set { _doExecute = value; } }

//        private int _loopCount = 0;

//        /// <summary>Whether commands are executed in the current code block.</summary>
//        public int LoopCount { get { return _loopCount; } set { _loopCount = value; } }

//        private string _conditionExpression = null;

//        public string ConditionExptession
//        {
//            get { return _conditionExpression; }
//            set { _conditionExpression = value; }
//        }

//        private bool _doSave = false;

//        /// <summary>Whether commands are saved to the command store in the current code block.</summary>
//        public bool DoSaveCommands { get { return _doSave; } set { _doSave = value; } }

//        private List<string> _commandLines = null;

//        public List<string> CommandLines
//        {
//            get
//            {
//                if (_commandLines == null)
//                {
//                    lock (Lock)
//                    {
//                        if (_commandLines == null)
//                            _commandLines = new List<string>();
//                    }
//                }
//                return _commandLines;
//            }
//        }


//        protected SortedDictionary<string, string> _variables = new SortedDictionary<string, string>();

//        /// <summary>Gets or sets value of the variable with the specified name.</summary>
//        /// <param name="variableName">Name of the variable to be set or retrieved.</param>
//        public string this[string variableName]
//        {
//            get { return _variables[variableName]; }
//            set { _variables[variableName] = value; }
//        }

//        /// <summary>Returns true if the specified variable is defined, false if it is not.</summary>
//        /// <param name="varName">Name of the variable that is queried.</param>
//        public bool IsVariableDefined(string varName)
//        {
//            return _variables.ContainsKey(varName);
//        }

//        /// <summary>Returns true if the specified variable is defined, false if it is not.
//        /// <para>Thread safe variant, lock is aquired on <see cref="Lock"/>.</para></summary>
//        /// <param name="varName">Name of the variable that is queried.</param>
//        public bool IsVariableDefinedLocked(string varName)
//        {
//            lock (Lock)
//            {
//                return _variables.ContainsKey(varName);
//            }
//        }

//        /// <summary>Returns value of the specified variable.</summary>
//        /// <param name="varName">Name of the variable.</param>
//        public string GetVariable(string varName)
//        {
//            return this[varName];
//        }

//        /// <summary>Gets the variable within lock on the <see cref="Lock"/> property and returns it.
//        /// <para>Thread safe variant, lock is aquired on <see cref="Lock"/>.</para></summary>
//        /// <param name="varName">Name of the variable whose value is to be reurned.</param>
//        public string GetVariableLocked(string varName)
//        {
//            lock (Lock)
//            {
//                return this[varName];
//            }
//        }

//        /// <summary>Sets value of the specified variable.</summary>
//        /// <param name="varName">Name of the variable to be set.</param>
//        /// <param name="varValue">Value to be assigned to the specified variable.</param>
//        public void SetVariable(string varName, string varValue)
//        {
//            this[varName] = varValue;
//        }

//        /// <summary>Sets the variable within lock on the property.
//        /// <para>Thread safe variant, lock is aquired on <see cref="Lock"/>.</para></summary>
//        /// <param name="varName">Name of the variable whose value is to be set.</param>
//        /// <param name="varValue">Value to be assigned to the variable.</param>
//        public void SetVariableLocked(string varName, string varValue)
//        {
//            lock (Lock)
//            {
//                this[varName] = varValue;
//            }
//        }


//        /// <summary>Removes the specified variable.</summary>
//        /// <param name="varName">Name of the variable to be removed.</param>
//        public void RemoveVariable(string varName)
//        {
//            _variables.Remove(varName);
//        }

//        /// <summary>Removes the specified variable within lock on the property.
//        /// <para>Thread safe variant, lock is aquired on <see cref="Lock"/>.</para></summary>
//        /// <param name="varName">Name of the variable whose value is to be set.</param>
//        public void RemoveVariableLocked(string varName)
//        {
//            lock (Lock)
//            {
//                _variables.Remove(varName);
//            }
//        }


//    }  // class CommandStackFrame



//    /// <summary>Contains stack frames for a single command thread of a command-line interreter.</summary>
//    /// <remarks>
//    /// <para>Usually properties, index operators and methods are not thread safe. Variants whose names end with "Locked"
//    /// implement locking on the <see cref="Lock"/> property and are thus thread safe. It is not likely that
//    /// locking is needed because the object will typically be used within a single thread.</para>
//    /// </remarks>
//    /// $A Igor Sep15;
//    public class CommandThread<InterpreterType> : ILockable
//        where InterpreterType : class, ICommandLineApplicationInterpreter
//    {

//        private CommandThread()
//            : this(null)
//        { }

//        public CommandThread(InterpreterType interpreter)
//        {
//            this.Interpreter = interpreter;
//            this.BaseFrame = this.AddFrame(CodeBlockType.Block);
//        }


//        #region ThreadLocking

//        private object _mainLock = new object();

//        /// <summary>This object's central lock object to be used by other object.
//        /// Do not use this object for locking in class' methods, for this you should use 
//        /// InternalLock.</summary>
//        public object Lock { get { return _mainLock; } }

//        #endregion ThreadLocking


//        #region Data.Basic

//        private InterpreterType _interpreter;

//        /// <summary>Returns interpreter that handles command execution on the current command thread.</summary>
//        public InterpreterType Interpreter
//        {
//            get
//            {
//                if (_interpreter == null) throw new InvalidOperationException("Interpreater is not set on CommandThread.");
//                return _interpreter;
//            }
//            protected set { _interpreter = value; }
//        }

//        private List<CommandStackFrame<InterpreterType>> _stackFrames = new List<CommandStackFrame<InterpreterType>>();

//        /// <summary>A list of stack frames existent in the current thread.</summary>
//        protected List<CommandStackFrame<InterpreterType>> StackFrames
//        { get { return _stackFrames; } }

//        private CommandStackFrame<InterpreterType> _baseFrame = null;

//        /// <summary>Base stack frame, created when the thread is initialized and exists until thread is exited.</summary>
//        public CommandStackFrame<InterpreterType> BaseFrame
//        { get { return _baseFrame; } protected set { _baseFrame = value; } }

//        private CommandStackFrame<InterpreterType> _topFrame = null;

//        /// <summary>Base stack frame, created when the thread is initialized and exists until thread is exited.</summary>
//        public CommandStackFrame<InterpreterType> TopFrame
//        { get { return _topFrame; } protected set { _topFrame = value; } }

//        private int _topFrameIndex = -1;

//        public int TopFrameIndex
//        {
//            get { return _topFrameIndex; }
//            protected set { _topFrameIndex = value; }
//        }

//        public CommandStackFrame<InterpreterType> this[int which]
//        {
//            get { return StackFrames[which]; }
//        }

//        /// <summary>Adds a new stack frame.</summary>
//        /// <param name="blockType">Type of the code block represented by the stack frame.</param>
//        /// <returns>Stack frame that has been added.</returns>
//        public CommandStackFrame<InterpreterType> AddFrame(CodeBlockType blockType)
//        {
//            CommandStackFrame<InterpreterType> ret = new CommandStackFrame<InterpreterType>(blockType, this);
//            StackFrames.Add(ret);
//            TopFrame = ret;
//            ++TopFrameIndex;
//            return ret;
//        }


//        /// <summary>Removes the last stack frame.</summary>
//        /// <returns>The stack frame that has been removed, so it can be used for inspection.</returns>
//        public CommandStackFrame<InterpreterType> RemoveFrame()
//        {
//            CommandStackFrame<InterpreterType> ret = null;
//            int which = StackFrames.Count - 1;
//            if (which == 0)
//                throw new InvalidOperationException("Can not remove the base stack frame.");
//            if (which < 0)
//                throw new InvalidOperationException("The current command thread does not contain any stack frames.");
//            ret = StackFrames[which];
//            StackFrames.RemoveAt(which);
//            --which;
//            TopFrame = StackFrames[which];
//            --TopFrameIndex;
//            return ret;
//        }


//        //public string this[string varName]
//        //{
//        //    get
//        //    {
//        //        string ret = null;
//        //        if (TopFrame.IsVariableDefined(varName))
//        //            ret = TopFrame[varName];
//        //        if (ret == null && BaseFrame.IsVariableDefined(varName))
//        //            ret = BaseFrame[varName];
//        //        if (ret == null)
//        //            throw new ArgumentException("Neither local nor global variable is defined named " + varName + ".");
//        //        return ret;

//        //    }
//        //    set
//        //    {
//        //        BaseFrame[varName] = value;
//        //    }
//        //}

//        ///// <summary>Returns true if the specified interpreter variable is defined (either local or global), false if not.</summary>
//        ///// <param name="varName">Variable whose existence is queried.</param>
//        ////public bool IsVariableDefined(string varName)
//        //{
//        //    return TopFrame.IsVariableDefined(varName) || BaseFrame.IsVariableDefined(varName);
//        //}

//        #endregion Data.Basic


//        #region Datta.Auxiliary


//        public static int DefaultOutputLevel
//        {
//            get { return CommandLineApplicationInterpreter.DefaultOutputLevel; }
//            set { CommandLineApplicationInterpreter.DefaultOutputLevel = value; }
//        } // = 1;

//        private bool _isOutputLevelSet = false;

//        public bool IsOutputLevelSet
//        {
//            get { return _isOutputLevelSet; }
//            protected set { _isOutputLevelSet = value; }
//        }

//        /// <summary>Default level of output for some of the interpreters' functionality (e.g. asynchronous command execution).</summary>
//        protected int _outputLevel = DefaultOutputLevel;

//        /// <summary>Level of output for some of the interpreter's functionality (e.g. asynchronous command execution).</summary>
//        public int OutputLevel
//        {
//            get
//            {
//                if (IsOutputLevelSet)
//                    return _outputLevel;
//                else
//                    return Interpreter.OutputLevel;
//            }
//            set { _outputLevel = value; }
//        }

//        public StopWatch1 _timer;

//        /// <summary>Gets the stopwatch used for measuring time of commands.
//        /// <para>This property always returns an initialized stopwatch.</para></summary>
//        public StopWatch1 Timer
//        {
//            get { if (_timer == null) _timer = new StopWatch1(); return _timer; }
//        }




//        #endregion Data.Auxiliary


//    }  // class CommandThread


//}
