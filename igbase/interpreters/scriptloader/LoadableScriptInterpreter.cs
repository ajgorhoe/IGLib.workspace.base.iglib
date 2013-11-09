// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Lib
{


    /// <summary>Interface for interpreters that can install commands from loadable scripts and run them.</summary>
    /// $A Igor Oct09;
    public interface ILoadableScriptInterpreter: ILockable
    {

    }

    /// <summary>Interpreter that can install commands from dynamically loaded (compiled) scripts and run them.
    /// Script loader object of a type <typeparamref name="ScriptLoaderBase"/> is accessed through
    /// a property that can be overridden in derived classes, such that a different script loader is used.
    /// This is importand because different libraries will be required for compilation in different contexts.
    /// Script loader property creates a new script loader on first access.</summary>
    /// $A Igor Oct09;
    public class LoadableScriptInterpreterBase: ILoadableScriptInterpreter
        // where ScriptLoaderType : ScriptLoaderBase, new()
    {

        #region Construction

        public LoadableScriptInterpreterBase()
        {  }

        #endregion Construction


        #region ILockable

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ILockable

        #region Operation


        protected ScriptLoaderBase _scriptLoader;

        // TODO: replace ScriptLoaderBase with interface when available!

        /// <summary>Script loader that is used to load loadable script classes that will be used for
        /// execution of commands.</summary>
        public virtual ScriptLoaderBase ScriptLoader
        {
            get
            {
                lock (Lock)
                {
                    if (_scriptLoader == null)
                        _scriptLoader = new ScriptLoaderIGLib();
                    return _scriptLoader;
                }
            }
            set
            {
                lock (Lock)
                {
                    _scriptLoader = value;
                }
            }
        }


        SortedList<string, ILoadableScript> _commands = new SortedList<string, ILoadableScript>();

        /// <summary>Sorted list that contains commands as key-value pairs where the key is command name
        /// and the corresponding value is a loadable script object of type <typeparamref name="ILoadableScript"/> 
        /// that can be executd.</summary>
        public SortedList<string, ILoadableScript> Commands
        { get { return _commands; } }

        public ILoadableScript this[string commandName]
        {
            get
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(commandName))
                        throw new ArgumentException("Command name not specified.");
                    if (Commands.ContainsKey(commandName))
                        return Commands[commandName];
                    else
                        return null;
                }
            }
            set
            {
                lock (Lock)
                {
                    Commands[commandName] = value;
                }
            }
        }


        /// <summary>Runs command with the specified name that is loaded on the current object.</summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandArguments">Command arguments.</param>
        /// <returns>Result of the command.</returns>
        public string RunCommand(string commandName, string[] commandArguments)
        {
            ILoadableScript commandObject = Commands[commandName];
            if (commandObject == null)
                throw new ArgumentException("Unknown command based on loadable scripts: \"" + commandName + "\"");
            return commandObject.Run(commandArguments);
        }

        /// <summary>Adds a new command whose execution is perfomed by an instance of a class that is
        /// dynamically compiled from the specified script code.</summary>
        /// <param name="commandName">Interpreter's name of the command; added command is installed 
        /// under this name on the current interpreter.</param>
        /// <param name="code">Script code containing definition of the loadable class that contains executable code of the command.</param>
        /// <param name="className">Name of the class containing loadable script code.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object 
        /// that will perform execution of the added command.</param>
        public void AddCommandFromCode(string commandName, string code, string className, string[] initializationArguments)
        {
            this[commandName] = ScriptLoader.CreateObjectFromCode(code, className, initializationArguments);
        }

        /// <summary>Adds a new command whose execution is perfomed by an instance of a class that is
        /// dynamically compiled from the specified script code. Name of the class is extracted from the script code.</summary>
        /// <param name="commandName">Interpreter's name of the command; added command is installed 
        /// under this name on the current interpreter.</param>
        /// <param name="code">Script code containing definition of the loadable class that contains executable code of the command.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object 
        /// that will perform execution of the added command.</param>
        public void AddCommandFromCode(string commandName, string code, string[] initializationArguments)
        {
            this[commandName] = ScriptLoader.CreateObjectFromCode(code, initializationArguments);
        }

        /// <summary>Adds a new command whose execution is perfomed by an instance of a class that is
        /// dynamically compiled from the script code contained in the specified file.</summary>
        /// <param name="commandName">Interpreter's name of the command; added command is installed 
        /// under this name on the current interpreter.</param>
        /// <param name="filePath">Path to the file containing script code containing definition of the 
        /// loadable class that contains executable code of the command.</param>
        /// <param name="className">Name of the class containing loadable script code.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object 
        /// that will perform execution of the added command.</param>
        public void AddCommandFromFile(string commandName, string filePath, string className, string[] initializationArguments)
        {
            this[commandName] = ScriptLoader.CreateObjectFromFile(filePath, className, initializationArguments);
        }


        /// <summary>Adds a new command whose execution is perfomed by an instance of a class that is
        /// dynamically compiled from the script code contained in the specified file. Name of the class 
        /// is extracted from the script code.</summary>
        /// <param name="commandName">Interpreter's name of the command; added command is installed 
        /// under this name on the current interpreter.</param>
        /// <param name="filePath">Path to the file containing script code containing definition of the 
        /// loadable class that contains executable code of the command.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object 
        /// that will perform execution of the added command.</param>
        public void AddCommandFromFile(string commandName, string filePath, string[] initializationArguments)
        {
            this[commandName] = ScriptLoader.CreateObjectFromFile(filePath, initializationArguments);
        }

        #endregion Operation



    } // class LoadableScriptInterpreter

    

}
