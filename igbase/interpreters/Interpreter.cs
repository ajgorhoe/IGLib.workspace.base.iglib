// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/****************************************************/
/*                                                  */
/*    INTERPRETERS (interfaces and base classes)    */
/*                                                  */
/****************************************************/

namespace IG.Lib
{


    public interface IInterpreterCommandData
    {
    }

    public interface IInterpreterCommand
    {
    }


    public interface IInterpreter<TCommand, TCommandData>
    {

        void AddCommand(string commandName, TCommand command);



    }


    public abstract class InterpreterCommandBase : IInterpreterCommandData
    {

    }

    

    public abstract class InterpreterCommandDataBase : IInterpreterCommand
    {

    }


    public abstract class InterpreterBase<TCommand, TCommandData> : IInterpreter<TCommand, TCommandData>
    {
        public abstract void AddCommand(string commandName, TCommand command);

    }



    /// <summary>A reference of command usage, contains ID of the interpreter where a command is
    /// registered, and command name under which command is registered on that interpreter.
    /// Objects of this class are immutable.</summary>
    public class CommandUseReference : IComparable<CommandUseReference>
    {
        private CommandUseReference() { }

        /// <summary>Creates a new command reference.</summary>
        /// <param name="interpreterId"></param>
        /// <param name="commandName"></param>
        public CommandUseReference(int interpreterId, string commandName)
        {
            this._interpreterId = interpreterId;
            this._commandName = commandName;
        }

        private int _interpreterId;

        private string _commandName;

        /// <summary>Gets ID of the interpreter where the command is registered.</summary>
        public int InterpreterId
        {
            get { return _interpreterId; }
        }

        /// <summary>Gets the command name under which command is registered in a specific interpreter.</summary>
        public string CommandName
        {
            get { return _commandName; }
        }


        #region Comparison

        int IComparable<CommandUseReference>.CompareTo(CommandUseReference other)
        {
            if (other == null)
                return 1;
            else if (this.InterpreterId < other.InterpreterId)
                return -1;
            else if (this.InterpreterId > other.InterpreterId)
                return 1;
            else if (this.CommandName == null)
            {
                if (other.CommandName == null)
                    return 0;
                else
                    return -1;
            }
            else if (other.CommandName == null)
                return 1;
            else return
                this.CommandName.CompareTo(other);

        } // class CommandUseReference

        #endregion Comparison
    }


}