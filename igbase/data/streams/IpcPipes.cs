// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Pipes;
using System.Threading;

using System.Diagnostics;


namespace IG.Lib
{

    /* See:
     * 
     * https://social.msdn.microsoft.com/Forums/vstudio/en-US/46c91711-755f-48fa-a4a8-92956082218f/howto-launch-process-and-write-to-its-stdin
     * 
     * http://stackoverflow.com/questions/850802/piping-in-a-file-on-the-command-line-using-system-diagnostics-process
     * http://msdn.microsoft.com/en-us/library/system.io.textreader.peek%28v=vs.110%29.aspx
     * 
     * http://stackoverflow.com/questions/3873878/capturing-console-stream-input
     */


    // TODO: ADAPT AND REPAIR THESE CLASSES! (copied from NamedPipes) - gothrough all methods!



    /// <summary>Server that creates a named pipe, listens on its input stream, and sends responses
    /// to the client.</summary>
    /// $A Igor xx Mar14;
    public class PipeServerBase : IpcStreamServerBase, ILockable
    {

        /// <summary>Prevent default  constructor.</summary>
        private PipeServerBase()
            : base()
        { }


        /// <summary>Constructs a new pip server.</summary>
        /// <param name="pipeName">Name of the pipe used for client-server communication.</param>
        /// <param name="startImmediately">If true then server is started immediately after created.</param>
        public PipeServerBase(string pipeName, bool startImmediately = true)
            : this()
        {
            this.PipeName = pipeName;
            if (startImmediately)
                ThreadServe();
        }

        /// <summary>Constructs a new named pipe server with the specified pipe name and other paramters.</summary>
        /// <param name="pipeName">Name of the pipe.</param>
        /// <param name="requestEnd">Line that ends each request. If null or empty string then the requests are single line.</param>
        /// <param name="responseEnd">Line that ends each response. If null or empty string then the responses are single line.</param>
        /// <param name="errorBegin">String that begins an error response. If null or empty string then default string remains in use,
        /// i.e. <see cref="DefaultErrorBegin"/></param>
        /// <param name="startImmediately">If true then server is started immediately after created.</param>
        public PipeServerBase(string pipeName, string requestEnd, string responseEnd, string errorBegin,
            bool startImmediately = true) :
            this(pipeName, false /* startImmediately */)
        {
            if (string.IsNullOrEmpty(requestEnd))
                this.IsMultilineRequest = false;
            else
            {
                this.IsMultilineRequest = true;
                this.MsgRequestEnd = requestEnd;
            }
            if (string.IsNullOrEmpty(responseEnd))
                this.IsMultilineResponse = false;
            else
            {
                this.IsMultilineResponse = true;
                this.MsgResponseEnd = responseEnd;
            }
            if (!string.IsNullOrEmpty(errorBegin))
                this.ErrorBegin = errorBegin;
            if (startImmediately)
                ThreadServe();
        }


        #region Data.General

        /// <summary>Server name. The same as pipe name.</summary>
        public override string Name
        {
            get { return PipeName; }
            set { PipeName = value; }
        }

        private string _pipeName = DefaultPipeName;

        public string PipeName
        {
            get { lock (_lock) { return _pipeName; } }
            set
            {
                lock (_lock)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new NullReferenceException("Pipe name can not be an empty or null string.");
                    if (value != _pipeName)
                    {
                        _pipeName = value;
                        ClosePipe();
                    }
                }
            }
        }

        #endregion Data.General


        #region Data.Streams

        private NamedPipeServerStream _serverPipe = null;

        /// <summary>Named pipe used for communication by the server.</summary>
        public NamedPipeServerStream ServerPipe
        {
            get
            {
                lock (Lock)
                {
                    if (_serverPipe == null)
                    {
                        if (true)  // $$ TEST
                            _serverPipe = new NamedPipeServerStream(PipeName);
                        else
                            _serverPipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut);
                        if (OutputLevel >= 1)
                            Console.WriteLine(Environment.NewLine + "Pipe server created, ({0}, name: '{1}')",
                                _serverPipe.GetHashCode(), PipeName);
                    }
                    // if (!_serverPipe.IsConnected)
                    //    WaitForConnection(_serverPipe);
                    //if (OutputLevel >= 1)
                    //{
                    //    Console.WriteLine("Named pipe server created (pipe name: '" + PipeName + "'), waiting for connection...");
                    //}
                    //_serverPipe.WaitForConnection();

                    //if (OutputLevel >= 2)
                    //{
                    //    Console.WriteLine("Pipe server connected (pipe name: '" + PipeName + "')."
                    //        + Environment.NewLine + "  CanRead:" + _serverPipe.CanRead
                    //        + Environment.NewLine + "  CanWrite:" + _serverPipe.CanWrite
                    //        + Environment.NewLine + "  IsAsync:" + _serverPipe.IsAsync
                    //        + Environment.NewLine + "  ReadTimeout:" + _serverPipe.ReadTimeout
                    //        + Environment.NewLine + "  ReadTimeout:" + _serverPipe.WriteTimeout
                    //        + Environment.NewLine + "  ReadTimeout:" + _serverPipe.TransmissionMode);
                    //} else if (OutputLevel >= 1)
                    //{
                    //    Console.WriteLine("Pipe server connected (pipe name: '" + PipeName + "')." + Environment.NewLine);
                    //}
                    return _serverPipe;
                }
            }
            protected set
            {
                lock (_lock)
                {
                    if (value != _serverPipe)
                    {
                        if (_serverPipe != null)
                        {
                            _serverPipe.Close();
                        }
                        InputStream = null;
                        OutputStream = null;
                        _serverPipe = value;
                    }
                }
            }
        }


        /// <summary>
        /// Closes the server pipe.
        /// </summary>
        protected override void CloseConnection()
        {

            InputStream = null;
            OutputStream = null;

            ServerPipe = null;
        }



        /// <summary>Returns true if server pipe is connected, .</summary>
        public override bool IsConnected()
        {
            if (InputStream != null && OutputStream != null)
                if (ServerPipe.IsConnected)
                    return true;
            return false;
        }


        /// <summary>Waits until a client connects to the specified server pipe.</summary>
        /// <param name="pipe">Pipe that waits for connection to be established.</param>
        protected override void WaitForConnection()
        {
            WaitForConnection(ServerPipe);
        }

        /// <summary>Waits until a client connects to the specified server pipe.</summary>
        /// <param name="pipe">Pipe that waits for connection to be established.</param>
        protected virtual void WaitForConnection(NamedPipeServerStream pipe)
        {
            lock (Lock)
            {
                if (pipe.IsConnected)
                {
                    if (OutputLevel >= 1)
                    {
                        Console.WriteLine(Environment.NewLine + "Attempt to wait for client pipe connection named '" + PipeName + "'."
                            + Environment.NewLine + "  Already connected.");
                    }
                    return;
                }
                if (OutputLevel >= 1)
                {
                    Console.WriteLine("Named pipe server created (pipe name: '" + PipeName + "'), waiting for connection...");
                }
                pipe.WaitForConnection();
                if (OutputLevel >= 2)
                {
                    Console.WriteLine("Pipe server connected (pipe name: '" + PipeName + "')."
                        + Environment.NewLine + "  CanRead:" + pipe.CanRead
                        + Environment.NewLine + "  CanWrite:" + pipe.CanWrite
                        + Environment.NewLine + "  IsAsync:" + pipe.IsAsync
                        + Environment.NewLine + "  CanTimeot:" + pipe.CanTimeout
                        // + Environment.NewLine + "  ReadTimeout:" + pipe.ReadTimeout
                        // + Environment.NewLine + "  ReadTimeout:" + pipe.WriteTimeout
                        + Environment.NewLine + "  ReadTimeout:" + pipe.TransmissionMode);
                    if (pipe.CanTimeout)
                    {
                        Console.WriteLine(Environment.NewLine + "  WriteTimeout:" + pipe.WriteTimeout
                            + Environment.NewLine + "  ReadTimeout:" + pipe.ReadTimeout + Environment.NewLine);
                    }
                    else
                        Console.WriteLine();

                }
                else if (OutputLevel >= 1)
                {
                    Console.WriteLine("Pipe server connected (pipe name: '" + PipeName + "')." + Environment.NewLine);
                }
            }
        }


        private StreamReader _inputStream = null;

        /// <summary>Input stream of the server.</summary>
        public override StreamReader InputStream
        {
            get
            {
                lock (Lock)
                {
                    if (_inputStream == null)
                    {
                        _inputStream = new StreamReader(ServerPipe);
                    }
                    return _inputStream;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (value != _inputStream)
                    {
                        if (_inputStream != null)
                        {
                            _inputStream.Close();
                        }
                        _inputStream = value;
                    }
                }
            }
        }


        private StreamWriter _outputStream = null;

        /// <summary>Output stream of the server's named pipe.</summary>
        public override StreamWriter OutputStream
        {
            get
            {
                lock (Lock)
                {
                    if (_outputStream == null)
                    {
                        _outputStream = new StreamWriter(ServerPipe);
                    }
                    return _outputStream;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (value != _outputStream)
                    {
                        if (_outputStream != null)
                        {
                            _outputStream.Close();
                        }
                        _outputStream = value;
                    }
                }
            }
        }

        /// <summary>Closes the Server's pipe and the associated streams.</summary>
        public override void ClosePipe()
        {
            ServerPipe = null;
        }


        /// <summary>Sends a dummy request in order for the serving function to stop blocking</summary>
        public override void SendDummyRequest()
        {
            // Send a dummy request in order for the serving function to stop blocking:
            using (NamedPipeClientStream clientStream = new NamedPipeClientStream(PipeName))
            {
                using (StreamWriter clientOutputWriter = new StreamWriter(clientStream))
                {
                    if (IsMultilineRequest)
                        clientOutputWriter.WriteLine();
                    else
                        clientOutputWriter.WriteLine(MsgRequestEnd);
                }
            }
        }

        #endregion Data.Streams


        #region Operation.Auxiliary


        /// <summary>Returns a stirng containing the server data.</summary>
        public override string ToString()
        {
            lock (Lock)
            {

                StringBuilderInternal.Clear();
                StringBuilderInternal.AppendLine("Named pipe server.");
                StringBuilderInternal.AppendLine("Pipe name: \"" + PipeName + "\".");
                StringBuilderInternal.AppendLine("Currently running: " + IsServerRunning);
                StringBuilderInternal.AppendLine("Multiline requests: " + IsMultilineRequest + ".");
                StringBuilderInternal.AppendLine("Multiline responses: " + IsMultilineResponse + ".");
                StringBuilderInternal.AppendLine("End of request mark: \"" + StopRequest + "\".");
                StringBuilderInternal.AppendLine("End of response mark: \"" + StopRequest + "\".");
                StringBuilderInternal.AppendLine("End of response: \"" + GenericResponse + "\".");
                StringBuilderInternal.AppendLine("Server stopped response: \"" + StoppedResponse + "\".");
                StringBuilderInternal.AppendLine("Last request string: \"" + LastRequestString + "\".");
                StringBuilderInternal.AppendLine("Last response string: \"" + LastResponseString + "\".");
                StringBuilderInternal.AppendLine("Last error message string: \"" + LastErrorMessage + "\".");
                return StringBuilderInternal.ToString();
            }
        }

        #endregion Operation.Auxiliary




    } // classs NamedPipeServerBase 





    /// <summary>Client to the pipe server (classes derived from <see cref="IpcStreamClientServerBase2"/>).</summary>
    /// $A Igor xx Mar14;
    public class PipeClientBase : IpcStreamClientBase, ILockable
    {

        private PipeClientBase()
            : base()
        { }

        /// <summary>Constructs a new named pipe client with the specified pipe name, default server address (<see cref="DefaultServerAddress"/>)
        /// and default values for other paramters.</summary>
        /// <param name="pipeName">Name of the pipe. Must not be null or empty string.</param>
        public PipeClientBase(string pipeName)
            : this(pipeName, null)
        { }

        /// <summary>Constructs a new named pipe client with the specified pipe name, server address (<see cref="DefaultServerAddress"/>)
        /// and default values for other paramters.</summary>
        /// <param name="pipeName">Name of the pipe. Must not be null or empty string.</param>
        /// <param name="serverAddress">Address of the server where named pipe server is run. If null or empty string then the
        /// default server address is uesd (<see cref="DefaultServerAddress"/>), referring to the current computer.</param>
        public PipeClientBase(string pipeName, string serverAddress)
            : this()
        {
            this.PipeName = pipeName;
            this.ServerAddress = serverAddress;
        }

        /// <summary>Constructs a new named pipe client with the specified pipe name, server address (<see cref="DefaultServerAddress"/>)
        /// and other paramters.</summary>
        /// <param name="pipeName">Name of the pipe.</param>
        /// <param name="serverAddress">Address of the server where named pipe server is run. If null or empty string then the
        /// default server address is uesd (<see cref="DefaultServerAddress"/>), referring to the current computer.</param>
        /// <param name="requestEnd">Line that ends each request. If null or empty string then the requests are single line.</param>
        /// <param name="responseEnd">Line that ends each response. If null or empty string then the responses are single line.</param>
        /// <param name="errorBegin">String that begins an error response. If null or empty string then default string remains in use,
        /// i.e. <see cref="DefaultErrorBegin"/></param>
        public PipeClientBase(string pipeName, string serverAddress, string requestEnd, string responseEnd, string errorBegin) :
            this(pipeName, serverAddress)
        {
            if (string.IsNullOrEmpty(requestEnd))
                this.IsMultilineRequest = false;
            else
            {
                this.IsMultilineRequest = true;
                this.MsgRequestEnd = requestEnd;
            }
            if (string.IsNullOrEmpty(responseEnd))
                this.IsMultilineResponse = false;
            else
            {
                this.IsMultilineResponse = true;
                this.MsgResponseEnd = responseEnd;
            }
            if (!string.IsNullOrEmpty(errorBegin))
                this.ErrorBegin = errorBegin;
        }



        // NEW:

        public static bool ExecuteSvnCommandWithFileInput(string command, string arguments, string filePath, out string result, out string errors)
        {


            bool retval = false;
            string output = string.Empty;
            string errorLines = string.Empty;
            Process svnCommand = null;
            var psi = new ProcessStartInfo(command);

            

            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            try
            {
                Process.Start(psi);
                psi.Arguments = arguments;
                svnCommand = Process.Start(psi);

                var file = new FileInfo(filePath);
                StreamReader reader = file.OpenText();
                string fileContents = reader.ReadToEnd();
                reader.Close();

                StreamWriter myWriter = svnCommand.StandardInput;
                StreamReader myOutput = svnCommand.StandardOutput;
                StreamReader myErrors = svnCommand.StandardError;

                myWriter.AutoFlush = true;
                myWriter.Write(fileContents);
                myWriter.Close();

                output = myOutput.ReadToEnd();
                errorLines = myErrors.ReadToEnd();

                // Check for errors
                if (errorLines.Trim().Length == 0)
                {
                    retval = true;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                errorLines += Environment.NewLine + msg;
            }
            finally
            {
                if (svnCommand != null)
                {
                    svnCommand.Close();
                }
            }

            result = output;
            errors = errorLines;

            return retval;
        }



        #region Data.General

        /// <summary>Client name. The same as pipe name.</summary>
        public override string Name
        {
            get { return PipeName; }
            set { PipeName = value; }
        }

        private string _pipeName = DefaultPipeName;

        public string PipeName
        {
            get { lock (_lock) { return _pipeName; } }
            set
            {
                lock (_lock)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new NullReferenceException("Pipe name can not be an empty or null string.");
                    if (value != _pipeName)
                    {
                        _pipeName = value;
                        ClosePipe();
                    }
                }
            }
        }


        private static string _defaultServerAddress = ".";

        /// <summary>Default server address.
        /// Setting to null sets it to "." (i.e. the local machine).</summary>
        public static string DefaultServerAddress
        {
            get { lock (LockGlobal) { return _defaultServerAddress; } }
            set
            {
                lock (LockGlobal)
                {
                    if (string.IsNullOrEmpty(value))
                        _defaultServerAddress = ".";
                    else
                        _defaultServerAddress = value;
                }
            }
        }


        private string _serverAddress = DefaultServerAddress;

        /// <summary>Server address.
        /// <para>Setting it to null sets it to <see cref="DefaultServerAddress"/>.</para></summary>
        public string ServerAddress
        {
            get { lock (_lock) { return _serverAddress; } }
            set
            {
                lock (_lock)
                {
                    if (string.IsNullOrEmpty(value))
                        value = DefaultServerAddress;
                    if (value != _serverAddress)
                    {
                        _serverAddress = value;
                        ClosePipe();
                    }
                }
            }
        }


        #endregion Data.General


        #region Data.Streams





        #endregion Data.Streams

        private NamedPipeClientStream _clientPipe = null;

        /// <summary>Named pipe used for communication by the server.</summary>
        public NamedPipeClientStream ClientPipe
        {
            get
            {
                lock (_lock)
                {
                    if (_clientPipe == null)
                    {
                        if (true) // $$ TEST
                            _clientPipe = new NamedPipeClientStream(PipeName);
                        else
                            _clientPipe = new NamedPipeClientStream(ServerAddress, PipeName, PipeDirection.InOut);

                        if (OutputLevel >= 1)
                        {
                            Console.WriteLine(Environment.NewLine + "Named pipe client created (pipe name: '" + PipeName + "').");
                        }
                        Connect(_clientPipe);
                        // Console.WriteLine(Environment.NewLine + "  ... pipe client '" + PipeName + "' connected." + Environment.NewLine);
                    }
                    return _clientPipe;
                }
            }
            protected set
            {
                lock (_lock)
                {
                    if (value != _clientPipe)
                    {
                        if (_clientPipe != null)
                        {
                            _clientPipe.Close();
                        }
                        InputStream = null;
                        OutputStream = null;
                        _clientPipe = value;
                    }
                }
            }
        }

        private StreamReader _inputStream = null;

        /// <summary>Input stream of the server's named pipe.</summary>
        public override StreamReader InputStream
        {
            get
            {
                lock (Lock)
                {
                    if (_inputStream == null)
                    {
                        if (!ClientPipe.IsConnected)
                            Connect(ClientPipe);
                        _inputStream = new StreamReader(ClientPipe);
                    }
                    return _inputStream;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (value != _inputStream)
                    {
                        if (_inputStream != null)
                        {
                            _inputStream.Close();
                        }
                        _inputStream = value;
                    }
                }
            }
        }


        private StreamWriter _outputStream = null;

        /// <summary>Output stream of the server's named pipe.</summary>
        public override StreamWriter OutputStream
        {
            get
            {
                lock (Lock)
                {
                    if (_outputStream == null)
                    {
                        if (!ClientPipe.IsConnected)
                            Connect(ClientPipe);
                        _outputStream = new StreamWriter(ClientPipe);
                    }
                    return _outputStream;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (value != _outputStream)
                    {
                        if (_outputStream != null)
                        {
                            _outputStream.Close();
                        }
                        _outputStream = value;
                    }
                }
            }
        }

        /// <summary>Closes the Server's pipe and the associated streams.</summary>
        public override void ClosePipe()
        {
            ClientPipe = null;
        }


        /// <summary>Connects with the server.
        /// <para>If timeout is not specifies then it tries to connect indefinitely.</para></summary>
        /// <param name="timeOutSeconds">Timeout in secconds for establishig connection.</param>
        public override void Connect(double timeOutSeconds = 0)
        {
            Connect(ClientPipe, timeOutSeconds);
        }


        /// <summary>Connects the specified pype with the server.
        /// <para>If timeout is not specifies then it tries to connect indefinitely.</para></summary>
        /// <param name="pipe">Pipe through which connection with the serverr is achieved.</param>
        /// <param name="timeOutSeconds">Timeout in secconds for establishig connection.</param>
        protected virtual void Connect(NamedPipeClientStream pipe, double timeOutSeconds = 0)
        {
            lock (Lock)
            {
                if (pipe.IsConnected)
                {
                    if (OutputLevel >= 1)
                    {
                        Console.WriteLine(Environment.NewLine + "Attempt to connect to pipe server '" + PipeName + "'."
                            + Environment.NewLine + "  already connected.");
                    }
                    return;
                }
                if (timeOutSeconds > 0 && pipe.CanTimeout)
                {
                    int milliSeconds = (int)(1000.0 * timeOutSeconds);
                    if (timeOutSeconds > 0 && milliSeconds <= 0)
                        milliSeconds = 1;
                    if (OutputLevel >= 1)
                    {
                        Console.WriteLine(Environment.NewLine + "Pipe client '" + PipeName + "': connecting with server, timeout: "
                            + milliSeconds + " ms ...");
                    }
                    pipe.Connect(milliSeconds);


                }
                else
                {
                    if (OutputLevel >= 1 && timeOutSeconds > 0 && !pipe.CanTimeout)
                    {
                        Console.WriteLine(Environment.NewLine + "WARNING: The client pipe does not support timeouts."
                            + Environment.NewLine + "  The specified timeout will have no effect." + Environment.NewLine);
                    }
                    if (OutputLevel >= 1)
                    {
                        Console.WriteLine(Environment.NewLine + "Pipe client '" + PipeName + "': connecting with server, no timeout... ");
                    }
                    if (false) // $$ TEST
                    {
                        Console.WriteLine("Testing mode, connecting to server with timeout..");
                        pipe.Connect(20000);
                        Console.WriteLine("  ... CONNECTED to server.");
                    }
                    else
                        pipe.Connect();
                }
                if (OutputLevel >= 2)
                {
                    Console.WriteLine(Environment.NewLine + "  Pipe client '" + PipeName + "' connected to server."
                        + Environment.NewLine + "  CanRead:" + ClientPipe.CanRead
                        + Environment.NewLine + "  CanWrite:" + ClientPipe.CanWrite
                        + Environment.NewLine + "  CanTimeout:" + ClientPipe.CanTimeout
                        // + Environment.NewLine + "  WriteTimeout:" + ClientPipe.WriteTimeout
                        // + Environment.NewLine + "  ReadTimeout:" + ClientPipe.ReadTimeout
                        + Environment.NewLine + "  IsAsync:" + ClientPipe.IsAsync
                        + Environment.NewLine + "  NumberOfServerInstances:" + ClientPipe.NumberOfServerInstances
                        + Environment.NewLine + "  CanSeek:" + ClientPipe.CanSeek
                        + Environment.NewLine + "  InBufferSize:" + ClientPipe.InBufferSize
                        + Environment.NewLine + "  OutBufferSize:" + ClientPipe.OutBufferSize);
                    if (ClientPipe.CanTimeout)
                    {
                        Console.WriteLine("  WriteTimeout:" + ClientPipe.WriteTimeout
                            + Environment.NewLine + "  ReadTimeout:" + ClientPipe.ReadTimeout + Environment.NewLine);
                    }
                    else
                        Console.WriteLine();
                }
                else if (OutputLevel >= 1)
                {
                    Console.WriteLine(Environment.NewLine + "  Pipe client '" + PipeName + "' connected to server.");
                }

            }
        }


        #region Operation.Auxiliary



        /// <summary>
        /// Closes the server pipe.
        /// </summary>
        protected override void CloseConnection()
        {

            InputStream = null;
            OutputStream = null;

            ClientPipe = null;
        }



        /// <summary>Returns true if server pipe is connected, .</summary>
        public override bool IsConnected()
        {
            if (InputStream != null && OutputStream != null)
                if (ClientPipe.IsConnected)
                    return true;
            return false;
        }


        /// <summary>Returns a stirng containing the server data.</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Named pipe client.");
            sb.AppendLine("Pipe name: \"" + PipeName + "\".");
            sb.AppendLine("Server address: \"" + ServerAddress + "\".");
            sb.AppendLine("Multiline requests: " + IsMultilineRequest + ".");
            sb.AppendLine("Multiline responses: " + IsMultilineResponse + ".");
            sb.AppendLine("End of request mark: \"" + StopRequest + "\".");
            sb.AppendLine("End of response mark: \"" + StopRequest + "\".");
            sb.AppendLine("End of response: \"" + GenericResponse + "\".");
            sb.AppendLine("Server stopped response: \"" + StoppedResponse + "\".");
            sb.AppendLine("Last request string: \"" + LastRequestString + "\".");
            sb.AppendLine("Last response string: \"" + LastResponseString + "\".");
            sb.AppendLine("Last error message string: \"" + LastErrorMessage + "\".");
            return sb.ToString();
        }

        #endregion Operation.Auxiliary


    } // class PipeClientBase



}  // namespace IG.Lib

