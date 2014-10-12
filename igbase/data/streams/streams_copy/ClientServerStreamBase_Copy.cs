// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// Base classes for client/server based on streamed communication 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Pipes;
using System.Threading;

#if false
using IG.Lib;
using IG.Lib.Copy;
using ClienServerStreamBase = IG.Lib.Copy.ClienServerStreamBase;
using ClientServerStreamBase2 = IG.Lib.Copy.ClientServerStreamBase2;
using ServerStreamBase = IG.Lib.Copy.ServerStreamBase;
using ClientStreamBase = IG.Lib.Copy.ClientStreamBase;
using NamedPipeServerBase = IG.Lib.Copy.NamedPipeServerBase;
using NamedPipeClientBase = IG.Lib.Copy.NamedPipeClientBase;
#endif

using IG.Lib;

namespace IG.Lib.Copy
{

    /// <summary>Base class for client and server classes with stream-based communication.</summary>
    /// $A Igor xx Aug14;
    public abstract class ClienServerStreamBase : ILockable
    {


        /// <summary>Provides an answer string to the specified request string.</summary>
        /// <param name="request">Request string.</param>
        /// <returns>Answer to the request.</returns>
        public delegate string ResponseDelegate(string request);


        #region ThreadLocking

        protected object _lock = new object();

        /// <summary>Objectt for locking the current object.</summary>
        public object Lock { get { return _lock; } }


        private static object _lockGlobal = null;

        /// <summary>Static lock object used by all instances of this class (and possibly by other classes).</summary>
        public static object LockGlobal
        {
            get
            {
                if (_lockGlobal == null)
                {
                    lock (Util.LockGlobal)
                    {
                        if (_lockGlobal == null)
                            _lockGlobal = new object();
                    }
                }
                return _lockGlobal;
            }
        }


        #endregion ThreadLocking 


        #region Messages

        public const int MinimalMessagePreffixLength = 3;

        private static string _defaultMessagePrefix = "IGLibMessage";

        public static string DefaultMessagePrefix
        {
            get { lock(LockGlobal) { return _defaultMessagePrefix; } }
            set {
                lock(LockGlobal)
                {
                    if (value != _defaultMessagePrefix)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            throw new ArgumentException("New default value for message prefix is not specified (null or empty string).");
                        } else if (value.Length < MinimalMessagePreffixLength)
                        {
                            throw new ArgumentException("Length of default value for message prefix is too short, should be at least "
                                + MinimalMessagePreffixLength + " haracters long.");
                        } else
                        {
                            _defaultMessagePrefix = value;
                        }
                    }
                }
            }
        }

        private static char _defaultMessageSeparator = '_';

        public static char DefaultMessageSeparator
        {
            get { lock (LockGlobal) { return _defaultMessageSeparator; } }
            set
            {
                lock (LockGlobal)
                {
                    if (value != _defaultMessageSeparator)
                    {
                        if (char.IsSeparator(value) || char.IsWhiteSpace(value))
                            throw new ArgumentException("Invalid default message separator '" + value + "': may not be separator or whitespace.");
                        else if (value == DefaultMessageFalseSeparator)
                            throw new ArgumentException("Invalid default message separator '" + value + "': can not be the same as false separator.");
                        else _defaultMessageSeparator = value;
                    }
                }
            }
        }

        private static char _defaultMessageFalseSeparator = '.';

        public static char DefaultMessageFalseSeparator
        {
            get { lock (LockGlobal) { return _defaultMessageFalseSeparator; } }
            set
            {
                lock (LockGlobal)
                {
                    if (value != _defaultMessageFalseSeparator)
                    {
                        if (char.IsSeparator(value) || char.IsWhiteSpace(value))
                            throw new ArgumentException("Invalid default false message separator '" + value + "': may not be separator or whitespace.");
                        else if (value == DefaultMessageSeparator)
                            throw new ArgumentException("Invalid default false message separator '" + value + "': can not be the same as separator.");
                        else _defaultMessageFalseSeparator = value;
                    }
                }
            }
        }
        
        
        private string _messagePrefix = DefaultMessagePrefix;


        public string MessagePrefix
        {
            get { lock (Lock) { return _messagePrefix; } }
            protected set
            {
                lock (Lock)
                {
                    if (value != MessagePrefix)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            throw new ArgumentException("New message prefix is not specified (null or empty string).");
                        }
                        else if (value.Length < MinimalMessagePreffixLength)
                        {
                            throw new ArgumentException("Length of message prefix is too short, should be at least "
                                + MinimalMessagePreffixLength + " characters long.");
                        } else
                        {
                            // Invalidate combined message prefix and separator:
                            MessagePrefixWithSeparator = null;
                            _messagePrefix = value;
                        }

                    }
                }
            }
        }

        /// <summary>Changes message prefix for the current object.
        /// <para>This setter method is provided in order to keep the <see cref="MessagePrefix"/> setter protected.</para></summary>
        /// <param name="messagePrefix">New message prefix. Must be at least <see cref="MinimalMessagePreffixLength"/> characters long.</param>
        public void SetMessagePreix(string messagePrefix)
        {
            MessagePrefix = messagePrefix;
        }

        private char _messageSeparator = DefaultMessageSeparator;

        public char MessageSeparator
        {
            get { lock (Lock) { return _messageSeparator; } }
            protected set {
                lock (Lock)
                {
                    if (value != _messageSeparator)
                    {
                        if (char.IsSeparator(value) || char.IsWhiteSpace(value))
                            throw new ArgumentException("Invalid message separator '" + value + "': may not be separator or whitespace.");
                        else if (value == MessageFalseSeparator)
                            throw new ArgumentException("Invalid message separator '" + value + "': may not be the same as false separator.");
                        else
                        {
                            // Invalidate combined message prefix and separator:
                            MessagePrefixWithSeparator = null;
                            _messageSeparator = value;
                        }
                    }
                }
            }
        }

        /// <summary>Changes message separator for the current object.
        /// <para>This setter method is provided in order to keep the <see cref="MessageSeparator"/> setter protected.</para></summary>
        /// <param name="messagePrefix">New message separator must not be a separator or a white space character.</param>
        public void SetMessageSeparator(char messageSeparator)
        {
            MessageSeparator = messageSeparator;
        }





        private char _messageFalseSeparator = DefaultMessageFalseSeparator;

        public char MessageFalseSeparator
        {
            get { lock (Lock) { return _messageFalseSeparator; } }
            protected set
            {
                lock (Lock)
                {
                    if (value != _messageFalseSeparator)
                    {
                        if (char.IsSeparator(value) || char.IsWhiteSpace(value))
                            throw new ArgumentException("Invalid message false separator '" + value + "': may not be separator or whitespace.");
                        else if (value == MessageSeparator)
                            throw new ArgumentException("Invalid message false separator '" + value + "': may not be the same as separator.");
                        else 
                        {
                            _messageFalseSeparator = value;
                        }
                    }
                }
            }
        }

        /// <summary>Changes message false separator for the current object.
        /// <para>This setter method is provided in order to keep the <see cref="MessageFalseSeparator"/> setter protected.</para></summary>
        /// <param name="messagePrefix">New message false separator must not be a separator or a white space character.</param>
        public void SetMessageFalseSeparator(char messageFalseSeparator)
        {
            MessageFalseSeparator = messageFalseSeparator;
        }





        private string _messagePrefixWithSeparator = null;

        /// <summary>Gets the mesage prefix with separator. If some string is a message, everything that follows this string
        /// until the first separator is a message name.
        /// <para>A protected getter is defined that is only allowed to set the property to null (invalidate it). If a
        /// non-null value is set then an exception is thrown.</para></summary>
        public string MessagePrefixWithSeparator
        {
            get
            {
                lock (Lock)
                {
                    if (_messagePrefixWithSeparator == null)
                    {
                        _messagePrefixWithSeparator = MessagePrefix + MessageSeparator;
                    }
                    return _messagePrefixWithSeparator;
                }
            }
            protected set
            {
                if (value != null)
                    throw new InvalidOperationException("Message separator with prefix can only be set to null.");
                else
                    _messagePrefixWithSeparator = value;
            }
        }


        /// <summary>Creates a built-in message (possibly with arguments) that is to be interpreted directly by the receiver 
        /// (stream client or server) and is not executed via ordinary path.
        /// <para>Messages commands are composed of message prefix, separator (jointly <see cref="MessagePrefixWithSeparator"/>), 
        /// and message name. The first two are fixed parts whire the latter varies and defines the message.</para></summary>
        /// <param name="messageName">Name of the message (a kind of command name).</param>
        /// <param name="messageArguments">Optional arguments of the message.</param>
        /// <returns></returns>
        public string CreateMessage(string messageName, string[] messageArguments)
        {
            StringBuilderInternal.Clear();
            StringBuilderInternal.Append(MessagePrefixWithSeparator);
            StringBuilderInternal.Append(messageName);
            if (messageArguments != null)
            {
                int num = messageArguments.Length;
                for (int i = 0; i < num; ++i)
                {
                    StringBuilderInternal.Append(" ");
                    StringBuilderInternal.Append(messageArguments[i]);
                }
            }
            return StringBuilderInternal.ToString();
        }

        /// <summary>Generates request and response string in such a way that it can not be mixed up with a message.
        /// <para>If the original string begins with the message prefix theen a false separator is inserted after the part that
        /// is the same as message prefix. In this way the string can be distinguished form a message and can be correctly decoded
        /// on the other side of the communication pipeline (simply by removng the false separator).</para></summary>
        /// <param name="originalResponseOrRequestString">Original response string that is sent to the other side.</param>
        /// <returns>The created request string that can be distinguished form a command.</returns>
        public string createResponseOrRequestString(string originalResponseOrRequestString)
        {
            if (originalResponseOrRequestString == null)
                return null;
            else if (!originalResponseOrRequestString.StartsWith(MessagePrefix))
                return originalResponseOrRequestString;
            else
            {
                return MessagePrefix + MessageFalseSeparator + originalResponseOrRequestString.Substring(MessagePrefix.Length);
            }
        }

        /// <summary>Returns the (eventually decoded) request or response string corresponding to the stirng that is read form the 
        /// communication pipeline, and also parameters that specify whether the request string represents a message or not. Eventual
        /// command or message parameters are also returned.</summary>
        /// <param name="responseOrRequestString">Original response or request string that is to be decoded.</param>
        /// <param name="isMessage">Output flag telling whether the string is a message or not.</param>
        /// <param name="messageOrCommandName">Name of the message or command extracted from the string.</param>
        /// <param name="messageArguments">Message or command arguments.</param>
        public void GetRequestOrResponse(ref string responseOrRequestString, out bool isMessage, 
            out string messageOrCommandName, out string [] messageArguments)
        {
            isMessage = false;
            messageOrCommandName = null;
            messageArguments = null;
            if (responseOrRequestString != null)
            {
                if (responseOrRequestString.StartsWith(MessagePrefix))
                {
                    if (responseOrRequestString.StartsWith(MessagePrefixWithSeparator))
                    {
                        // String is actually a message:
                        isMessage = true;
                        responseOrRequestString = responseOrRequestString.Substring(MessagePrefixWithSeparator.Length);
                        string[] parts = responseOrRequestString.Split(new char[] {' ', '\t', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                        messageOrCommandName = parts[0];
                        int numParts = parts.Length;
                        if (numParts > 1)
                        {
                            messageArguments = new string[numParts - 1];
                            for (int i = 1; i < numParts; ++i)
                                messageArguments[i-1] = parts[i];
                        }
                    } else
                    {
                        // String begins with message prfix, but it is not a message (because a separator is not following):
                        // Actual request/response string is obtained by excluding the inserted additional character after the MessagePrefix:
                        responseOrRequestString = MessagePrefix + responseOrRequestString.Substring(MessagePrefix.Length + 1);
                    }
                }
            }
        }

        #endregion Messages




        #region Data.General


        private StringBuilder _sb = new StringBuilder();

        protected StringBuilder StringBuilderInternal
        {
            get { return _sb; }
        }


        private static int _defatultOutputLevel = 1;



        /// <summary>Default level of output for this kind of class.</summary>
        public static int DefaultOutputLevel
        {
            get { lock (LockGlobal) return _defatultOutputLevel; }
            set { lock (LockGlobal) { _defatultOutputLevel = value; } }
        }

        private int _otputLevel = DefaultOutputLevel;

        /// <summary>Level of output generated by operatins.</summary>
        public virtual int OutputLevel
        {
            get { lock (Lock) { return _otputLevel; } }
            set { lock (Lock) { _otputLevel = value; } }
        }




        #endregion Data.General


    }  // class ClienServerStreamBase


} // namespace IG.Lib
