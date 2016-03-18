// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

        /**********************************/
        /*                                */
        /*    COMMAND-LINE INTERPRETER    */
        /*                                */
        /**********************************/

namespace IG.Lib
{



    /// <summary>Holds execution data (command arguments ns result) for interpreter commands
    /// of the CommandLine type.</summary>
    public class CommandLineData: IInterpreterCommandData
    {

    }


    /// <summary>Represents a single command-line that can be executed.
    /// $A Igor Feb09;</summary>
    public abstract class CommandLine : InterpreterCommandBase,
            IInterpreterCommand, ILockable, IDisposable,
            IRegisterable<CommandLine>
    {

        /// <summary>Creates a new command that can be used in command-line interpreters.</summary>
        public CommandLine()
        {
        }

        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking

        #region IRegistrableImplementation
        // Comment: this includes IIdrntifiable implementation, for which _idProxy is used in addition.

        /// <summary>Static object that providees object register and generates IDs 
        /// for this class:</summary>
        private static ObjectRegister<CommandLine> _register =
            new ObjectRegister<CommandLine>(1 /* first ID */);

        /// <summary>Proxy object that implements the IIdentifiable interface for this object.</summary>
        private IdProxy _idPproxy =
            new IdProxy(_register);

        /// <summary>Returns unique Id (in the scope of a given type) of the current object.
        /// Method is defined as virtual so that derived class can have its own IDs by defining its
        /// own static IdentifiableGenerator object.</summary>
        public virtual int Id { get { return _idPproxy.Id; } }

        /// <summary>Gets object register where the current object is registered.</summary>
        public ObjectRegister<CommandLine> ObjectRegister
        { get { return _register; } }

        /// <summary>Registers the current object.
        /// Subsequent calls (after the first one) have no effect.</summary>
        public void Register()
        { _register.Register(this); }

        /// <summary>Returns true if the current object is registered, false if not.</summary>
        /// <returns></returns>
        public bool IsRegistered()
        { return _register.IsRegistered(this); }

        /// <summary>Unregisters the current object if it is currently registered. 
        /// Can be performed several times, in this case only the first call may have effect.</summary>
        public void Unregister()
        { _register.Unregister(this.Id); }

        #endregion IRegistrableImplementation


        SortedDictionary<CommandUseReference, string> _commandReferences =
                new SortedDictionary<CommandUseReference, string>();

        //List<CommandUseReference> _commandReferences = new List<CommandUseReference>(2);

        /// <summary>Adds a new command use reference for this command.
        /// This method should be called whenever the command in installed on some interpreter.
        /// An internal list is used to store all references to this command on various interpreters.
        /// Stored references are unique (i.e. pairs {InterpreterId, CommandName}).</summary>
        /// <param name="interpreterId">ID of the interpreter where command is installed.</param>
        /// <param name="commandName">Command name under which the command is installed on this interpreter.</param>
        /// <param name="description">Custom description of the specified reference, can be null.</param>
        public void AddCommandReference(int interpreterId, string commandName, string description)
        {
            if (string.IsNullOrEmpty(commandName))
                throw new ArgumentException("Command name to be added is not specified.");
            CommandUseReference key = new CommandUseReference(interpreterId, commandName);
            lock (Lock)
            {
                if (!_commandReferences.ContainsKey(key))
                {
                    _commandReferences.Add(key,
                        commandName + " " + interpreterId.ToString() + ": " + description);
                }
            }
        }

        /// <summary>Removes command uage reference.
        /// This method should be called whenever a command is uninstalled form interpreter.</summary>
        public void RemoveCommandReference(int interpreterId, string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
                throw new ArgumentException("Command name to be added is not specified.");
            CommandUseReference key = new CommandUseReference(interpreterId, commandName);
            lock (Lock)
            {
                if (_commandReferences.ContainsKey(key))
                {
                    _commandReferences.Remove(key);
                }
            }
        }

        /// <summary>Executes the current command.
        /// Warning: whenever the data on command object (i.e. this) or interpreter object is accessed,
        /// these objects must be locked! If such data is not accessed then locking is not necessary.</summary>
        /// <param name="interpreter">Interpreter that executed the command.</param>
        /// <param name="command">Command name under which the command is executed.
        /// A single command instance can be installed under several names on the interpreter.</param>
        /// <param name="arguments">Arguments of the command.</param>
        protected abstract void Execute(CommandLineInterpreter interpreter,
                    string command, string[] arguments);


        /// <summary>Temporary storage of command name for execution in a new thread.</summary>
        protected string _command;

        /// <summary>Temporary storage of command arguments for execution in a new thread.</summary>
        protected string[] _arguments;

        /// <summary>Flag used to signal that command data has been picked by the executing
        /// thread and so object access can be unlocked.</summary>
        protected bool _threadStarted;

        #region Finalization

        /// <summary>Clean up after the reference is not used any more.</summary>
        public abstract void Dispose();

        ~CommandLine()
        { }

        #endregion Finalization



    }  // class CommandLine


    /// <summary>Base class for all command line interpreters.</summary>
    public class CommandLineInterpreter : InterpreterBase<CommandLine, CommandLineData>,
            IInterpreter<CommandLine, CommandLineData>, ILockable, IDisposable,
            IRegisterable<CommandLineInterpreter>
    {



        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        //private object _internalLock = new object();

        ///// <summary>Used internally for locking access to internal fields.</summary>
        //protected object InternalLock { get { return _internalLock; } }

        //private object waitlock = new object();

        ///// <summary>Must be used only for locking waiting the Waiting() block (since it is potentially time consuming).</summary>
        //protected object WaitLock { get { return waitlock; } }

        #endregion ThreadLocking

        #region IRegistrableImplementation
        // Comment: this includes IIdrntifiable implementation, for which _idProxy is used in addition.

        /// <summary>Static object that providees object register and generates IDs 
        /// for this class:</summary>
        private static ObjectRegister<CommandLineInterpreter> _register =
            new ObjectRegister<CommandLineInterpreter>(1 /* first ID */);

        /// <summary>Proxy object that implements the IIdentifiable interface for this object.</summary>
        private IdProxy _idPproxy =
            new IdProxy(_register);

        /// <summary>Returns unique Id (in the scope of a given type) of the current object.
        /// Method is defined as virtual so that derived class can have its own IDs by defining its
        /// own static IdentifiableGenerator object.</summary>
        public virtual int Id { get { return _idPproxy.Id; } }

        /// <summary>Gets object register where the current object is registered.</summary>
        public ObjectRegister<CommandLineInterpreter> ObjectRegister
        { get { return _register; } }

        /// <summary>Registers the current object.
        /// Subsequent calls (after the first one) have no effect.</summary>
        public void Register()
        { _register.Register(this); }

        /// <summary>Returns true if the current object is registered, false if not.</summary>
        /// <returns></returns>
        public bool IsRegistered()
        { return _register.IsRegistered(this); }

        /// <summary>Unregisters the current object if it is currently registered. 
        /// Can be performed several times, in this case only the first call may have effect.</summary>
        public void Unregister()
        { _register.Unregister(this.Id); }

        #endregion IRegistrableImplementation

        #region InterpreterGeneral

        SortedDictionary<string, CommandLine> _commands = new SortedDictionary<string,CommandLine>();

        protected SortedDictionary<string, CommandLine> Commands
        { get { lock (Lock) { return _commands; } } }


        /// <summary>Adds a new command to the interpreter.
        /// If the command with the specified name already exists then the command is replaced.</summary>
        /// <param name="commandName">Command name through which interpreter will be able to invoke the command.</param>
        /// <param name="command">Command to be added to the interpreter.</param>
        /// <param name="referenceDescription">Description of command reference (data structure that includes
        /// the command, interpreter where command is installed, and name under which command is registered 
        /// on the interpretere). These data are used in order to know when command is not referenced any more,
        /// so its finalization block can be performed.</param>
        public void AddCommand(string commandName, CommandLine command, string referenceDescription)
        {
            lock (Lock)
            {
                if (!Commands.ContainsKey(commandName))
                {
                    RemoveCommand(commandName);
                }
                Commands.Add(commandName, command);
                command.AddCommandReference(this.Id, commandName, referenceDescription);
            }
        }


        /// <summary>Adds a new command to the interpreter.
        /// If the command with the specified name already exists then the command is replaced.</summary>
        /// <param name="commandName">Command name through which interpreter will be able to invoke the command.</param>
        /// <param name="command">Command to be added to the interpreter.</param>
        public override void AddCommand(string commandName, CommandLine command)
        {
            AddCommand(commandName, command, null);
        }

        /// <summary>Removes the specified command reference from the interpreter.</summary>
        /// <param name="commandName">Name that identifies the command on this interpreter.
        /// Note that any command can be registered under different names, but each command name
        /// references only one command.</param>
        public void RemoveCommand(string commandName)
        {
            lock(Lock)
            {
                if (string.IsNullOrEmpty(commandName))
                    throw new ArgumentException("Name of the command to remove not specified (null or empty string).");
                if (Commands.ContainsKey(commandName))
                {
                    CommandLine command = Commands[commandName];
                    Commands.Remove(commandName);
                    if (command != null)
                        command.RemoveCommandReference(this.Id, commandName);

                }
            }
        }

        /// <summary>Renames the specified command.</summary>
        /// <param name="oldCommandName"></param>
        /// <param name="newCommandName"></param>
        public void RenameCommand(string oldCommandName, string newCommandName)
        {
            lock (Lock)
            {
                if (string.IsNullOrEmpty(oldCommandName))
                    throw new ArgumentException("Name of the command to be removed not specified (null or empty string).");
                if (string.IsNullOrEmpty(newCommandName))
                    throw new ArgumentException("New name of command not specified (null or empty string).");
            }

        }

        /// <summary>Returns the command that is registered under the specified name, or null
        /// if there is no such command.</summary>
        /// <param name="commandName">Command name. Must not be null or empty string!</param>
        /// <returns>Command corresponding to the specified command name.</returns>
        protected CommandLine GetCommand(string commandName)
        {
            lock (Lock)
            {
                if (string.IsNullOrEmpty(commandName))
                    throw new ArgumentException("Command name not specified (null or empty string).");
                if (Commands.ContainsKey(commandName))
                    return Commands[commandName];
                else
                    return null;
            }
        }
        
        
        #endregion InterpreterGeneral




        #region Specific

        private char _separator = ',';

        public char Separator
        {
            get { lock (Lock) { return _separator; } }
            protected set { lock (Lock) { _separator = value; } }
        }

        #endregion Specific





        #region Finalization

        /// <summary>Clean up after the reference is not used any more.</summary>
        public virtual void Dispose()
        {

        }

        ~CommandLineInterpreter()
        { }

        #endregion Finalization

    } // class CommandLineInterpreter

}



