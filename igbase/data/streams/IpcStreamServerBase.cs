// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using System.Threading;


namespace IG.Lib
{

/// <summary>Server that creates a named pipe, listens on its input stream, and sends responses
/// to the client.</summary>
/// $A Igor xx Mar14;
public abstract class IpcStreamServerBase : IpcStreamClientServerBase2, ILockable
{


    #region Data.Streams



    /// <summary>Waits until a client connects to the specified server pipe.</summary>
    protected abstract void WaitForConnection();


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
                        if (line == MsgRequestEnd)
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
                if (IsMultilineResponse)
                {
                    WriteMessage(OutputStream, MsgResponseBegin, null);
                } else 
                {
                    if (responseString != null)
                        if (responseString.Contains('\n'))
                        {
                            responseString = responseString.Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
                        }
                }
                OutputStream.WriteLine(responseString);
                if (OutputLevel >= 1)
                {
                    Console.WriteLine("... response sent: " + responseString);
                }
                if (IsMultilineResponse)
                {
                    WriteMessage(OutputStream, MsgResponseEnd, null);
                    if (OutputLevel >= 2)
                    {
                        Console.WriteLine("Multiline response end message sent: \"" + MsgResponseEnd + "\"");
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

        /// <summary>Sends the response (i.e., the <see cref="IpcStreamClientServerBase2.ResponseString"/>) back to the client.</summary>
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

            if (!IsConnected())
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
            CloseConnection(); //ServerPipe = null;
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


} // namespace IG.Lib
