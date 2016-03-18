using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using System.Threading;


using IG.Lib;

namespace IG.Lib.Copy
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



    }  // class NamedPipeServerClientBase


    /// <summary>Server that creates a named pipe, listens on its input stream, and sends responses
    /// to the client.</summary>
    /// $A Igor xx Mar14;
    public abstract class ServerStreamBase : ClientServerStreamBase2, ILockable
    {


        #region Data.Streams



        /// <summary>Waits until a client connects to the specified server pipe.</summary>
        protected abstract void WaitForConnection();


        /// <summary>Closes connection of the server pype.</summary>
        protected abstract void NullifyServerPipeLine();

        /// <summary>Returns true if server pipe is connected, .</summary>
        public abstract bool IsServerPipelineConnected();

        /// <summary>Closes the inpt stream.</summary>
        public override void CloseInput()
        {
            InputStream = null;
        }

        /// <summary>Closes the outut stream.</summary>
        public override void CloseOutput()
        {
            OutputStream = null;
        }


        #endregion Data.Streams


        #region Data.Operaton

        protected internal bool _isResponseSent = false;

        /// <summary>Auxiliary flag telling whether response to a request has already been sent to the client.
        /// Used for synchronization of diffeeent parts of the response generation process,
        /// which enables e.g special handling of Exceptions.</summary>
        public bool IsResponseSent
        {
            get { lock (Lock) { return _isResponseSent; } }
            protected set { _isResponseSent = value; }
        }


        private bool _stopServe = false;

        /// <summary>Whether the pipe should be closed.</summary>
        public bool StopServe
        { get { lock (_lock) { return _stopServe; } } protected set { lock (_lock) { _stopServe = value; } } }


        private bool _isServerRunning = false;

        /// <summary>Flag telling whether the server is currently running.</summary>
        public bool IsServerRunning
        {
            get { lock (Lock) { return _isServerRunning; } }
            protected set { lock (Lock) { _isServerRunning = true; } }
        }


        public abstract void SendDummyRequest();


        public virtual void StopServer()
        {
            lock (Lock)
            {
                StopServe = true;
                SendDummyRequest();
            }
        }

        #endregion Data.Operaton


        #region Operation.ResponseDefinition


        /// <summary>The deefault method that returns response to the specified request.
        /// <para>Just returns a string that tells which was the request string.</para></summary>
        /// <param name="request"></param>
        protected static string DefaultResponseMethod(string request)
        {
            return "Request: \" " + request + "\"";
        }

        private ResponseDelegate _responseMethod = DefaultResponseMethod;

        /// <summary>Delegate that calculates response to given request.</summary>
        public virtual ResponseDelegate ResponseMethod
        {
            get { lock (Lock) { return _responseMethod; } }
            set { lock (Lock) { _responseMethod = value; } }
        }


        /// <summary>Returns response string for given request string.
        /// <para>This method will generally be overridden in derived classes.</para></summary>
        /// <param name="request">The request string.</param>
        /// <returns>Response to the request string.</returns>
        public virtual string GetResponse(string request)
        {
            if (ResponseMethod != null)
            {
                return ResponseMethod(request);
            }
            else
            {
                // return GenericResponse;
                throw new InvalidOperationException("Method of response calculation is not defined.");
            }
        }

        /// <summary>Returns error message corresponding to the specified exception.</summary>
        /// <param name="ex"></param>
        protected virtual string GetErrorMessage(Exception ex)
        {
            if (ex == null)
                return "ERROR. Cause unknown.";
            else
                return "ERROR - " + ex.GetType().Name + ": " + ex.Message;
        }

        #endregion Operation.ResponseDefinition


        #region Operation

        /// <summary>Reads the next request from the pipe.</summary>
        protected virtual string ReadRequest()
        {
            lock (Lock)
            {
                try
                {
                    RequestString = null;
                    string clientRequestString = null;
                    if (IsMultilineRequest)
                    {
                        if (OutputLevel >= 1)
                        {
                            Console.WriteLine(Environment.NewLine + "Waiting for a multiline request...");
                        }
                        StringBuilderInternal.Clear();
                        bool stop = false;
                        do
                        {
                            if (OutputLevel >= 2)
                            {
                                Console.WriteLine("  Waiting for next line of request...");
                            }
                            string line = InputStream.ReadLine();
                            if (OutputLevel >= 2)
                            {
                                Console.WriteLine("... line read: \"" + line + "\"");
                            }
                            if (line == RequestEnd)
                                stop = true;
                            else
                                StringBuilderInternal.AppendLine(line);
                        } while (!stop);
                        clientRequestString = StringBuilderInternal.ToString();
                        clientRequestString = clientRequestString.TrimEnd('\n', '\r');
                        if (OutputLevel >= 1)
                        {
                            Console.WriteLine("... multiline request read: \"" + clientRequestString + "\"");
                        }

                    }
                    else
                    {
                        if (OutputLevel >= 1)
                        {
                            Console.WriteLine(Environment.NewLine + "Waiting for a single lne request...");
                        }
                        clientRequestString = InputStream.ReadLine();
                        {
                            Console.WriteLine(Environment.NewLine + "... single line request read: \"" + clientRequestString + "\"");
                        }
                    }
                    RequestString = clientRequestString;
                    LastRequestString = clientRequestString;
                    IsResponseSent = false;
                    ResponseString = null;
                    IsError = false;
                    LastException = null;
                    LastErrorMessage = null;
                }
                catch (Exception ex)
                {
                    LastRequestString = RequestString;
                    IsResponseSent = false;
                    ResponseString = null;
                    IsError = true;
                    LastException = ex;
                    LastErrorMessage = ex.Message;
                }
                return RequestString;
            }
        }

        /// <summary>Sends the specified response string back to the server.</summary>
        /// <param name="responseString"></param>
        protected virtual void SendResponse(string responseString)
        {
            lock (Lock)
            {
                try
                {
                    if (OutputLevel >= 1)
                    {
                        Console.WriteLine("Sending response to client...");
                    }
                    OutputStream.WriteLine(responseString);
                    if (OutputLevel >= 1)
                    {
                        Console.WriteLine("... response sent: " + responseString);
                    }
                    if (IsMultilineResponse)
                    {
                        OutputStream.WriteLine(ResponseEnd);
                        if (OutputLevel >= 2)
                        {
                            Console.WriteLine("Multiline response end message sent: \"" + ResponseEnd + "\"");
                        }
                    }
                    OutputStream.Flush();
                    if (OutputLevel >= 1)
                    {
                        Console.WriteLine();
                    }
                    IsResponseSent = true;
                    LastResponseString = responseString;
                }
                catch (Exception ex)
                {
                    if (OutputLevel >= 1)
                        Console.WriteLine(Environment.NewLine + "ERROR when sending named pipe server response: "
                             + Environment.NewLine + "  " + ex.Message);
                }
            }
        }

        /// <summary>Sends the response (i.e., the <see cref="ClientServerStreamBase2.ResponseString"/>) back to the client.</summary>
        protected virtual void SendResponse()
        {
            SendResponse(this.ResponseString);
        }

        /// <summary>Reads a single request from the client and sends back the response.</summary>
        protected virtual void RespondToRequest()
        {
            lock (Lock)
            {
                // ReadRequest();
                if (OutputLevel >= 2)
                {
                    Console.WriteLine(Environment.NewLine +
                        "Request: \"" + RequestString + "\"");
                }

                if (!string.IsNullOrEmpty(RequestString))
                {
                    // Verify special requests with pre-defined meaning, such as stop server request:
                    if (StopRequest == RequestString)
                    {
                        StopServe = true;
                        SendResponse(StoppedResponse);
                        if (OutputLevel >= 1)
                        {
                            Console.WriteLine(Environment.NewLine + "Stop request sent, server is stopping." + Environment.NewLine
                                + "  Request sting: " + RequestString);
                        }
                    }
                }
                if (!_isResponseSent)
                {
                    // Calculate a normal response and send it back to the client:
                    ResponseString = GetResponse(RequestString);
                    if (OutputLevel >= 2)
                    {
                        Console.WriteLine("Response: \"" + ResponseString + "\"");
                    }
                    SendResponse();
                }

            }
        }



        /// <summary>Enters the serving loop.</summary>
        /// <remarks>Within the loop, only <see cref="ReadRequest"/>() and <see cref="RespondToRequest"/>()
        /// are executed. The latter must handle things like stopping requests.</remarks>
        protected virtual void ServeInCurrentThread()
        {
            bool doStart = false;
            lock (Lock)
            {
                if (!IsServerRunning)
                {
                    IsServerRunning = true;
                    doStart = true;
                }
            }
            if (doStart)
            {
                // Open pipe server stream if not yet opened:
                // NamedPipeServerStream stream = ServerPipe;


                //NamedPipeServerStream pipeStream = ServerPipe;
                //if (!pipeStream.IsConnected)
                //    WaitForConnection(pipeStream);

                if (!IsServerPipelineConnected())
                    WaitForConnection();

                StopServe = false;
                while (!StopServe)
                {

                    lock (_lock)
                    {
                        try
                        {
                            ReadRequest();
                        }
                        catch (Exception ex)
                        {
                            if (OutputLevel >= 1)
                                Console.WriteLine(Environment.NewLine + Environment.NewLine + "ERROR in named pipe server when reading request: "
                                    + Environment.NewLine + "  " + ex.Message);
                        }
                        try
                        {
                            RespondToRequest();
                        }
                        catch (Exception ex)
                        {
                            IsError = true;
                            LastException = ex;
                            LastErrorMessage = GetErrorMessage(ex);
                            if (!_isResponseSent)
                                SendResponse(ErrorBegin + "Exception " + ex.GetType() + ", reason: " + ex.Message);
                        }
                    }
                }
                // After the server stops listneing, reset its state:
                lock (Lock)
                {
                    ClearData();
                    IsServerRunning = false;
                }
            }
        }



        private Thread _workingThread = null;

        protected ThreadPriority _threadPriority = UtilSystem.ThreadPriority;

        /// <summary>Priority of the server thread.
        /// <para>Setting priority changes priority of the server thread if it exists.</para></summary>
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
                        if (_workingThread != null)
                            _workingThread.Priority = value;
                    }
                }
            }
        }

        /// <summary>Launches a named pipe server in a new thread.</summary>
        public void ThreadServe()
        {
            lock (Lock)
            {
                if (!IsServerRunning)
                {
                    if (_workingThread != null)
                    {
                        AbortWorkingThread();
                    }
                    _workingThread = new Thread(ServeInCurrentThread);
                    _workingThread.Name = "Streamed server: \"" + Name + "\".";
                    _workingThread.IsBackground = true;
                    _workingThread.Priority = this.ThreadPriority;
                    _workingThread.Start();

                }
            }
        }

        /// <summary>Aborts the working thread.</summary>
        /// <param name="timeoutSeconds"></param>
        public void AbortWorkingThread(double timeoutSeconds = 0)
        {
            try
            {
                if (_workingThread != null)
                {
                    _workingThread.Abort();
                    if (timeoutSeconds > 0)
                    {
                        int timeoutMilliseconds = (int)(timeoutSeconds * 1000.0);
                        if (timeoutMilliseconds <= 0)
                            timeoutMilliseconds = 1;
                        _workingThread.Join(timeoutMilliseconds);
                    }
                    else
                        _workingThread.Join();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                _workingThread = null;
                NullifyServerPipeLine(); //ServerPipe = null;
            }
        }



        /// <summary>Clears all the data related to servig requests (i.e. request and response strings, error flags, exceptions, etc.).</summary>
        public override void ClearData()
        {
            lock (Lock)
            {
                IsResponseSent = false;
                IsError = false;
                RequestString = null;
                ResponseString = null;
                LastRequestString = null;
                LastResponseString = null;
                LastException = null;
                LastErrorMessage = null;
            }
        }

        #endregion Operation


    }  // abstract class ServerStreamBase



    /// <summary>Client to the pipe server (classes derived from <see cref="ClientServerStreamBase2"/>).</summary>
    /// $A Igor xx Mar14;
    public abstract class ClientStreamBase : ClientServerStreamBase2, ILockable
    {


        #region Data.Streams

        /// <summary>Closes the inpt stream.</summary>
        public override void CloseInput()
        {
            InputStream = null;
        }

        /// <summary>Closes the outut stream.</summary>
        public override void CloseOutput()
        {
            OutputStream = null;
        }

        #endregion Data.Streams


        #region Data.Operation


        protected internal bool _isResponseReceived = false;

        /// <summary>Auxiliary flag telling whether response to a request has already been received from the server.
        /// Used for synchronization of diffeeent parts of the request sending process,
        /// which enables e.g special handling of Exceptions.</summary>
        public bool IsResponseReceived
        {
            get { lock (Lock) { return _isResponseReceived; } }
            protected set { _isResponseReceived = value; }
        }


        #endregion Data.Operation


        #region  Operation


        /// <summary>Connects with the server.
        /// <para>If timeout is not specifies then it tries to connect indefinitely.</para></summary>
        /// <param name="timeOutSeconds">Timeout in secconds for establishig connection.</param>
        public abstract void Connect(double timeOutSeconds = 0);

        /// <summary>Returns error message corresponding to the specified exception.</summary>
        /// <param name="ex"></param>
        protected virtual string GetErrorMessage(Exception ex)
        {
            if (ex == null)
                return "ERROR. Cause unknown.";
            else
                return "Client ERROR " + ex.GetType().Name + ": " + ex.Message;
        }


        /// <summary>Send specified request to server through a named pipe.</summary>
        /// <param name="requestString">Request string.</param>
        protected virtual void SendRequest(string requestString)
        {
            lock (Lock)
            {
                try
                {
                    if (OutputLevel >= 2)
                    {
                        Console.WriteLine(Environment.NewLine + "Sending request: \"" + requestString + "\"...");
                    }
                    RequestString = requestString;
                    OutputStream.WriteLine(requestString);
                    if (IsMultilineRequest)
                    {
                        OutputStream.WriteLine(RequestEnd);
                        if (OutputLevel >= 2)
                        {
                            Console.WriteLine("Multiline request end message sent: \"" + RequestEnd + "\"");
                        }
                    }
                    OutputStream.Flush();
                    if (OutputLevel >= 2)
                    {
                        Console.WriteLine("... request sent. ");
                    }
                    ResponseString = null;
                    IsResponseReceived = false;
                    IsError = false;
                    LastErrorMessage = null;
                    LastException = null;
                }
                catch (Exception ex)
                {
                    IsError = true;
                    LastException = ex;
                    LastErrorMessage = GetErrorMessage(ex);
                }
            }
        }


        /// <summary>Sends the current request string (the <see cref="ClientServerStreamBase2.RequestString"/> property) to the 
        /// server through a named pipe.</summary>
        protected virtual void SentRequest()
        {
            SendRequest(this.RequestString);
        }


        /// <summary>Reads response from the server and stores it.</summary>
        protected virtual string ReadResponse()
        {
            lock (Lock)
            {
                if (OutputLevel >= 1)
                    Console.WriteLine("Waiting  for server response...");

                try
                {
                    string serverResponseString = null;
                    if (IsMultilineResponse)
                    {
                        StringBuilderInternal.Clear();
                        bool stop = false;
                        do
                        {
                            if (OutputLevel >= 2)
                                Console.WriteLine("  Waiting for next line of response...");
                            string line = InputStream.ReadLine();
                            if (OutputLevel >= 2)
                                Console.WriteLine("  ... next line received: \"" + line + "\"");
                            if (line == ResponseEnd)
                                stop = true;
                            else
                                StringBuilderInternal.AppendLine(line);
                        } while (!stop);
                        serverResponseString = StringBuilderInternal.ToString();
                        serverResponseString = serverResponseString.TrimEnd('\n', '\r');
                        if (OutputLevel >= 1)
                            Console.WriteLine("Received multiline response: \"" + serverResponseString + "\"");
                    }
                    else
                    {
                        if (OutputLevel >= 1)
                        {
                            Console.WriteLine("Waiting for single line response...");
                        }
                        serverResponseString = InputStream.ReadLine();
                        if (OutputLevel >= 1)
                        {
                            Console.WriteLine("... Single line response received: \"" + serverResponseString + "\"");
                        }
                    }
                    if (IsErrorResponse(serverResponseString))
                    {
                        // Error occurred on the server:
                        IsResponseReceived = true;
                        IsError = true;
                        LastException = null;
                        LastErrorMessage = GetErrorMessage(serverResponseString);
                        ResponseString = null;
                        if (OutputLevel >= 1)
                        {
                            Console.WriteLine("Error response obtained, message: \"" + LastErrorMessage + "\"" + Environment.NewLine);
                        }
                    }
                    else
                    {
                        IsResponseReceived = true;
                        IsError = false;
                        LastException = null;
                        LastErrorMessage = null;
                        ResponseString = serverResponseString;
                        if (OutputLevel >= 1)
                        {
                            Console.WriteLine("Response from server: \"" + serverResponseString + "\"");
                        }
                    }
                    LastRequestString = _requestString;
                    LastResponseString = _responseString;
                    if (IsError)
                    {
                        if (OutputLevel >= 1)
                        {
                            Console.WriteLine(Environment.NewLine + "Error occurred: "
                                + Environment.NewLine + "  " + _lastErrorMessage + Environment.NewLine);
                        }
                    }
                    else
                    {
                        if (OutputLevel >= 2)
                        {
                            Console.WriteLine(Environment.NewLine + "  Response: \"" + _responseString + "\"." + Environment.NewLine);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ResponseString = null;
                    LastRequestString = _requestString;
                    LastRequestString = null;
                    IsError = true;
                    LastException = ex;
                    LastErrorMessage = GetErrorMessage(ex);
                }
                return ResponseString;
            }
        }


        /// <summary>Sends a request to the server and returns its response.
        /// <para>Synchronous.</para></summary>
        /// <param name="requestString">Request string that is sent to the server.</param>
        /// <returns>Server's response, which can also be the error string in the case that serving the request has thrown exception.</returns>
        public string GetServerResponse(string requestString)
        {
            lock (Lock)
            {
                SendRequest(requestString);
                string ret = ReadResponse();
                return ret;
            }
        }


        /// <summary>Clears all the data related to servig requests (i.e. request and response strings, error flags, exceptions, etc.).</summary>
        public override void ClearData()
        {
            lock (Lock)
            {
                // IsResponseSent = false;
                IsError = false;
                RequestString = null;
                ResponseString = null;
                LastRequestString = null;
                LastResponseString = null;
                LastException = null;
                LastErrorMessage = null;
            }
        }

        #endregion Operation


    }  // abstract class ClientStreamBase


} // namespace IG.Lib
