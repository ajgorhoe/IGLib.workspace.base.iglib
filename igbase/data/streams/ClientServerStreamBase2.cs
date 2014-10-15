using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using System.Threading;


namespace IG.Lib
{


    /// <summary>Base class for named pipe servers and clients, contains common stuff for both.</summary>
    /// $A Igor xx Mar14;
    public abstract class ClientServerStreamBase2 : ClienServerStreamBase, ILockable
    {

        // TODO: implement destructors and Close()! - if possible, do it simply on the base class.



        #region Data.General

        public abstract string Name
        {
            get;
            set;
        }

        private static string _defaultPipeName = "IGLibServerPipe";

        /// <summary>Default pipe name.</summary>
        public static string DefaultPipeName
        {
            get { lock (LockGlobal) { return _defaultPipeName; } }
            set
            {
                lock (LockGlobal)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new NullReferenceException("Default pipe name can not be empty or null string.");
                    else
                        _defaultPipeName = value;
                }
            }
        }





        private static bool _defaultIsMultilineRequest = true;

        /// <summary>Default pipe name.</summary>
        public static bool DefaultIsMultilineRequest
        {
            get { { return _defaultIsMultilineRequest; } }
        }

        private static string _defaultRequestEnd = "RequestEnd";

        /// <summary>Default string (line) that ends any multiline request.</summary>
        public static string DefaultRequestEnd
        {
            get { { return _defaultRequestEnd; } }
        }

        private static bool _defaultIsMultilineResponse = true;

        /// <summary>Default pipe name.</summary>
        public static bool DefaultIsMultilineResponse
        {
            get { { return _defaultIsMultilineResponse; } }
        }

        private static string _defaultResponseEnd = "ResponseEnd";

        /// <summary>Default string (line) that ends any multiline response.</summary>
        public static string DefaultResponseEnd
        {
            get { { return _defaultResponseEnd; } }
        }


        private bool _isMultilineRequest = DefaultIsMultilineRequest;

        /// <summary>Whether or not multi line requests are allowed.</summary>
        public virtual bool IsMultilineRequest
        {
            get { return _isMultilineRequest; }
            protected set { _isMultilineRequest = value; }
        }

        /// <summary>String (line) that ends a request (only when multiline requests are allowed).</summary>
        private string _requestEnd = DefaultRequestEnd;

        public string RequestEnd
        {
            get { return _requestEnd; }
            protected set { _requestEnd = value; }
        }


        private bool _isMultilineResponse = DefaultIsMultilineResponse;

        /// <summary>Whether or not multi line responses are allowed.</summary>
        public virtual bool IsMultilineResponse
        {
            get { return _isMultilineResponse; }
            protected set { _isMultilineResponse = value; }
        }

        /// <summary>String (line) that ends a response (only when multiline responses are allowed).</summary>
        private string _responseEnd = DefaultResponseEnd;

        public string ResponseEnd
        {
            get { return _responseEnd; }
            protected set { _responseEnd = value; }
        }


        private static string _defaultErrorBegin = "$$ERROR__83753093759$$: ";

        /// <summary>Default string that begins an error report.</summary>
        public static string DefaultErrorBegin
        {
            get { { return _defaultErrorBegin; } }
        }

        /// <summary>String (line) that ends a response (only when multiline responses are allowed).</summary>
        private string _errorBegin = DefaultErrorBegin;

        public string ErrorBegin
        {
            get { return _errorBegin; }
            protected set { _errorBegin = value; }
        }

        /// <summary>Returns true if the specified response string represents an error response (exception), false if not.</summary>
        /// <param name="responseString">Response string that is inspected.</param>
        public virtual bool IsErrorResponse(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
                return false;
            return (responseString.IndexOf(ErrorBegin) == 0);
        }

        /// <summary>Returns error message that corresponds to the specified response string.
        /// <para>Exception is thrown if the response string does not represent an error response.</para></summary>
        /// <param name="responseString">Response string for which erro message is returned.</param>
        public virtual string GetErrorMessage(string responseString)
        {
            if (!IsErrorResponse(responseString))
                throw new ArgumentException("The specified string does not represent an error response: "
                    + Environment.NewLine + responseString);
            return responseString.Substring(ErrorBegin.Length);
        }



        #endregion Data.General


        #region Data.Streams


        /// <summary>Closes the pipe and streams that depend on it.</summary>
        public abstract void ClosePipe();

        /// <summary>Input stream writer of the server's named pipe.</summary>
        public abstract StreamReader InputStream { get; protected set; }

        /// <summary>Output stream reader of the server's named pipe.</summary>
        public abstract StreamWriter OutputStream { get; protected set; }

        /// <summary>Closes the inpt stream.</summary>
        public abstract void CloseInput();

        /// <summary>Closes the outut stream.</summary>
        public abstract void CloseOutput();

        #endregion Data.Streams


        #region Data.Operation

        protected internal bool _isError = false;

        public bool IsError
        {
            get { lock (Lock) { return _isError; } }
            protected set { _isError = true; }
        }

        protected internal string _requestString = null;

        /// <summary>The last request string that was read from the pipe.</summary>
        public string RequestString
        {
            get { lock (_lock) { return _requestString; } }
            protected set { lock (Lock) { _requestString = value; } }
        }

        protected internal string _responseString = null;

        /// <summary>The last answer string that was written to the pipe.</summary>
        public string ResponseString
        {
            get { lock (_lock) { return _responseString; } }
            protected set { lock (Lock) { _responseString = value; } }
        }


        private static string _defaultStopRequest = "stop";

        /// <summary>Default stop request string - request string that will stop the server.</summary>
        public static string DefaultStopRequest
        {
            get { lock (LockGlobal) { return _defaultStopRequest; } }
            set
            {
                lock (LockGlobal)
                {
                    //if (string.IsNullOrEmpty(value))
                    //    throw new NullReferenceException("Default stop request can not be empty or null string.");
                    //else
                    _defaultStopRequest = value;
                }
            }
        }


        private string _stopRequest = DefaultStopRequest;

        /// <summary>Request that causes the server stop listening and closing the pipe.</summary>
        public string StopRequest
        {
            get { lock (Lock) { return _stopRequest; } }
            set { _stopRequest = value; }
        }


        private static string _defaultGenericResponse = "IGLib_PipeServer_GenericResponse";

        /// <summary>Default generic response (sent in absence of any other method to generate the response).</summary>
        public static string DefaultGenericResponse
        {
            get { lock (LockGlobal) { return _defaultGenericResponse; } }
            set
            {
                lock (LockGlobal)
                {
                    if (value != _defaultGenericResponse)
                    {
                        if (string.IsNullOrEmpty(value))
                            throw new NullReferenceException("Default generic response of pipe server can not be empty or null string.");
                        else
                        {
                            if (DefaultOutputLevel >= 1)
                                Console.WriteLine(Environment.NewLine + "Warning: default generic response of pipe servers changed: "
                                    + Environment.NewLine + "  from " + _defaultGenericResponse + " to " + value + ".");
                            _defaultGenericResponse = value;
                        }
                    }
                }
            }
        }

        private string _genericResponse = DefaultGenericResponse;

        /// <summary>Generic response that is sent back to the client in abscence of any
        /// method generating responses to specific requests.</summary>
        public string GenericResponse
        {
            get { lock (_lock) { return _genericResponse; } }
            protected set
            {
                lock (_lock)
                {
                    if (value != _genericResponse)
                    {
                        if (string.IsNullOrEmpty(value))
                            throw new NullReferenceException("Pipe server's generic response can not be an empty or null string.");
                        _genericResponse = value;
                    }
                }
            }
        }


        private static string _defaultStoppedResponse = "IGLib_PipeServer_StoppedResponse";

        /// <summary>Default stopped response (sent after the srver has sttopped on request).</summary>
        public static string DefaultStoppedResponse
        {
            get { lock (LockGlobal) { return _defaultStoppedResponse; } }
            set
            {
                lock (LockGlobal)
                {
                    if (value != _defaultStoppedResponse)
                    {
                        if (string.IsNullOrEmpty(value))
                            throw new NullReferenceException("Default generic response of pipe server can not be empty or null string.");
                        else
                        {
                            if (DefaultOutputLevel >= 1)
                                Console.WriteLine(Environment.NewLine + "Warning: default stopped response of pipe servers changed: "
                                    + Environment.NewLine + "  from " + _defaultStoppedResponse + " to " + value + ".");
                            _defaultStoppedResponse = value;
                        }
                    }
                }
            }
        }

        private string _stoppedResponse = DefaultStoppedResponse;

        /// <summary>Stopped response that is sent back to the client after the server stops on its
        /// request.</summary>
        public string StoppedResponse
        {
            get { lock (_lock) { return _stoppedResponse; } }
            protected set
            {
                lock (_lock)
                {
                    if (value != _stoppedResponse)
                    {
                        if (string.IsNullOrEmpty(value))
                            throw new NullReferenceException("Pipe server's stopped response can not be an empty or null string.");
                        _stoppedResponse = value;
                    }
                }
            }
        }




        //private static string _defaultErrorResponse = "IGLib_PipeServer_ErrorResponse";

        ///// <summary>Default error response (sent as response when exception is thrown in the process of 
        ///// response generation).</summary>
        //public static string DefaultErrorResponse
        //{
        //    get { lock (LockGlobal) { return _defaultErrorResponse; } }
        //    protected set
        //    {
        //        lock (LockGlobal)
        //        {
        //            if (value != _defaultErrorResponse)
        //            {
        //                if (string.IsNullOrEmpty(value))
        //                    throw new NullReferenceException("Pipe servers' default error response can not be empty or null string.");
        //                else
        //                {
        //                    if (DefaultOutputLevel > 0)
        //                        Console.WriteLine(Environment.NewLine + "Default error response of pipe servers has changed: "
        //                            + Environment.NewLine + "  from " + _defaultErrorResponse + " to " + value + ".");
        //                    _defaultErrorResponse = value;
        //                }
        //            }
        //        }
        //    }
        //}


        //private string _errorResponse = DefaultErrorResponse;

        ///// <summary>Response that is sent to the client in case of exception.</summary>
        //public string ErrorResponse
        //{
        //    get { lock (_lock) { return _errorResponse; } }
        //    protected set
        //    {
        //        lock (_lock)
        //        {
        //            if (value != _errorResponse)
        //            {
        //                if (string.IsNullOrEmpty(value))
        //                    throw new NullReferenceException("Pipe server's error response can not be an empty or null string.");
        //                _errorResponse = value;
        //            }
        //        }
        //    }
        //}



        /// <summary>Clears all the data related to servig requests (i.e. request and response strings, error flags, exceptions, etc.).</summary>
        public abstract void ClearData();


        #endregion Data.Operation


        #region Data.SavedState


        protected internal Exception _lastException = null;

        /// <summary>Returns the last exception thrown when serving request.</summary>
        public Exception LastException
        {
            get { lock (Lock) { return _lastException; } }
            protected set { lock (Lock) { _lastException = value; } }
        }


        protected internal string _lastErrorMessage = null;

        /// <summary>Returns the last error message.</summary>
        public string LastErrorMessage
        {
            get { lock (Lock) { return _lastErrorMessage; } }
            protected set { lock (Lock) { _lastErrorMessage = value; } }
        }


        protected internal string _lastRequestString = null;

        /// <summary>Returns the last request string.</summary>
        public string LastRequestString
        {
            get { lock (Lock) { return _lastRequestString; } }
            protected set { lock (Lock) { _lastRequestString = value; } }
        }


        protected internal string _lastResponseString = null;

        /// <summary>Returns the last response string.</summary>
        public string LastResponseString
        {
            get { lock (Lock) { return _lastResponseString; } }
            protected set { lock (Lock) { _lastResponseString = value; } }
        }

        #endregion Data.SavedState


        #region Operation


        //public override string ToString()
        //{
        //    StringBuilderInternal.Clear();
        //    StringBuilderInternal.AppendLine("Named pipe server or client.");
        //    StringBuilderInternal.AppendLine("Server/client name: \"" + Name + "\".");
        //    StringBuilderInternal.AppendLine("Multiline requests: " + IsMultilineRequest + ".");
        //    StringBuilderInternal.AppendLine("Multiline responses: " + IsMultilineResponse + ".");
        //    StringBuilderInternal.AppendLine("End of request mark: \"" + StopRequest + "\".");
        //    StringBuilderInternal.AppendLine("End of response mark: \"" + StopRequest + "\".");
        //    StringBuilderInternal.AppendLine("End of response: \"" + GenericResponse + "\".");
        //    StringBuilderInternal.AppendLine("Server stopped response: \"" + StoppedResponse + "\".");
        //    StringBuilderInternal.AppendLine("Last request string: \"" + LastRequestString + "\".");
        //    StringBuilderInternal.AppendLine("Last response string: \"" + LastResponseString + "\".");
        //    StringBuilderInternal.AppendLine("Last error message string: \"" + LastErrorMessage + "\".");
        //    return StringBuilderInternal.ToString();
        //}

        #endregion Operation



    }  // class ClientServerStreamBase2




} // namespace IG.Lib

