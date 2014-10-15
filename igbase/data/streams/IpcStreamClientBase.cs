using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Pipes;
using System.Threading;


namespace IG.Lib
{




    /// <summary>Client to the pipe server (classes derived from <see cref="IpcStreamClientServerBase2"/>).</summary>
    /// $A Igor xx Mar14;
    public abstract class IpcStreamClientBase : IpcStreamClientServerBase2, ILockable
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


        /// <summary>Sends the current request string (the <see cref="RequestString"/> property) to the 
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
