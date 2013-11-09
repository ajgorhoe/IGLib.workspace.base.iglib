// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// PARALLEL JOB SERVER 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using IG.Lib;

namespace IG.Num
{


    /// <summary>Defines state of the parallel server.</summary>
    public enum ParallelServerState
    {
        Uninitialized = 0,  // not yet ready for operation
        Idle,               // ready to accept a new job
        Executing,          // currently executing a job
        Inactive            // not active (switched off)
    }


    /// <summary>Parallel job server. Waits for job requests and executes them in a parallel thread.</summary>
    /// <typeparam name="JobContainerType">Type of the container that holds data for the job (input and output).</typeparam>/// 
    /// <remarks>
    /// <para>The server executes jobs on a separate thread that is spawned by the server and is used 
    /// just for this purpose.</para>
    /// <para>The server object can operate in two modes. In the server mode, a server threas is started that 
    /// continuously waits for job requests and executes them as soon as they arrive.
    /// In single job state, a new thread is started for each job request and is terminated when the job is completed.</para>
    /// <para>Job request is sent by the </para>
    /// </remarks>
    /// $A Igor Aug08;
    public abstract class ParallelJobServerBase<JobContainerType>: ILockable, IIdentifiable
        where JobContainerType: ParallelJobContainerBase
    {

        public ParallelJobServerBase()
        { this._state = ParallelServerState.Idle; }

        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking

        #region OperationData


        /// <summary>Default output level for objects of this and derived types.
        /// <para>Just aliases the <see cref="ParallelJobContainerBase.DefaultOutputLevel"/>.</para></summary>
        public static int DefaultOutputLevel
        {
            get { return ParallelJobContainerBase.DefaultOutputLevel; }
            set { ParallelJobContainerBase.DefaultOutputLevel = value; }
        }

        /// <summary>Output level for objects of this class. 
        /// <para>Specifies how much output is printed to console during operation.</para></summary>
        private volatile int _outputLevel = ParallelJobContainerBase.DefaultOutputLevel;

        /// <summary>Output level the current object. 
        /// <para>Specifies how much output is printed to console during operation.</para></summary>
        public int OutputLevel
        {
            get { return _outputLevel; }
            set { lock (Lock) { _outputLevel = value; } }
        }

        /// <summary>Default value of test mode flag.
        /// <para>Just aliases the <see cref="ParallelJobContainerBase.DefaultOutputLevel"/>.</para></summary>
        public static bool DefaultIsTestMode
        {
            get { return ParallelJobContainerBase.DefaultIsTestMode; }
            set { ParallelJobContainerBase.DefaultIsTestMode = value; }
        }

        protected volatile bool _isTestMode = ParallelJobContainerBase.DefaultIsTestMode;

        /// <summary>Whether the current job data conntainer is in test mode.
        /// In this mode, delays specified by internal variables are automatically added in job execution.</summary>
        public bool IsTestMode
        {
            get { return _isTestMode; }
            set
            {
                lock (Lock)
                {
                    if (value == true && value != _isTestMode)
                    {
                        //OutputLevel = OutputLevelTestMode;
                    }
                    _isTestMode = value;
                }
            }
        }


        private static int _nextId = 0;

        /// <summary>Returns another ID that is unique for objects of the containing class its and derived classes.</summary>
        protected static int GetNextId()
        {
            lock (ParallelJobContainerBase.LockId)
            {
                ++_nextId;
                return _nextId;
            }
        }

        private int _id = GetNextId();

        /// <summary>Unique ID for objects of the currnet and derived classes.</summary>
        public int Id
        { get { return _id; } }

        private static int _defaultSleepTimeMs = 2;

        /// <summary>Default sleeping time, in milliseconds, used by parallel job data objects when waiting for 
        /// fulfillment of some condition in a loop that includes sleeping when condition is not met.</summary>
        public static int DefaultSleepTimeMs
        {
            get { return _defaultSleepTimeMs; }
            set { if (value <= 0) value = 1; _defaultSleepTimeMs = value; }
        }

        protected int _sleepTimeMs = DefaultSleepTimeMs;

        /// <summary>Sleeping time, in milliseconds, used by the current object when waiting for 
        /// fulfillment of some condition in a loop that includes sleeping when condition is not met.
        /// <para><see cref="DefaultSleepTimeMs"/> specifies the default value that is set when object is created.</para></summary>
        public int SleepTimeMs
        {
            get { lock (Lock) { return _sleepTimeMs; } }
            set { lock (Lock) { if (value <= 0) value = DefaultSleepTimeMs; _sleepTimeMs = value; } }
        }


        protected ParallelJobDispatcherBase<JobContainerType> _dispatcher;

        public ParallelJobDispatcherBase<JobContainerType> Dispatcher
        {
            get { lock (Lock) { return _dispatcher; } }
            protected internal set { lock (Lock) { _dispatcher = value; } }
        }

        protected ParallelServerState _state = ParallelServerState.Idle;

        /// <summary>Gets the state of the current job runner.</summary>
        public ParallelServerState State
        {
            get { lock (Lock) { return _state; } }
            protected set { _state = value; }
        }

        /// <summary>Gets a flag indicating whether the current job runner is idle.</summary>
        public bool IsIdle
        { get { lock (Lock) { return (_state == ParallelServerState.Idle); } } }

        public bool IsActive
        { get { lock (Lock) { 
            return (_state != ParallelServerState.Uninitialized && _state!=ParallelServerState.Inactive); 
        } } }

        protected JobContainerType _jobData;

        /// <summary>Data for the job that is executed by teh current parallel job runner.</summary>
        JobContainerType JobData
        {
            get { lock (Lock) { return _jobData; } }
            set { lock (Lock) { _jobData = value; } }
        }

        /// <summary>Notifies all interested objects that the job has started.</summary>
        public void NotifyJobStarted()
        {
            if (_outputLevel >= 4)
            {
                Console.WriteLine("        server " + Id + ": NotifyJobStarted()...");
            }
            ParallelJobDispatcherBase<JobContainerType> dispatcher = null;
            JobContainerType jobData = null;
            lock (Lock)
            {
                _state = ParallelServerState.Executing;
                dispatcher = _dispatcher;
                jobData = _jobData;
            }
            if (jobData == null)
            {
                Console.WriteLine(Environment.NewLine+ "WARNING: Job data is null when sending 'job started' notification, server ID = " + Id + Environment.NewLine);
            } else
            {
                if (_outputLevel >= 4)
                {
                    Console.WriteLine("        server " + Id + ": notifying container (job started)...");
                }
                jobData.NotifyJobStarted();
            }
            if (dispatcher == null)
            {
                Console.WriteLine(Environment.NewLine+ "WARNING: Dispatcher is null when sending 'job started' notification, server ID = " + Id + Environment.NewLine);
            } else
            {
                if (_outputLevel >= 4)
                {
                    Console.WriteLine("        server " + Id + ": notifying dispatcher (job started)...");
                }
                dispatcher.NotifyJobStarted(this, jobData);
            }
            if (_outputLevel >= 4)
            {
                Console.WriteLine("        server " + Id + ": NotifyJobStarted() completed.");
            }
        }


        /// <summary>Notifies all interested objects that the job has finished.</summary>
        public void NotifyJobFinished()
        {
            ParallelJobDispatcherBase<JobContainerType> dispatcher = null;
            JobContainerType jobData = null;
            try
            {
                if (_outputLevel >= 4)
                {
                    Console.WriteLine("        server " + Id + ": NotifyJobFinished()...");
                }
                lock (Lock)
                {
                    dispatcher = _dispatcher;
                    jobData = _jobData;
                }
                if (jobData == null)
                {
                    Console.WriteLine(Environment.NewLine+ "WARNING: Job data is null when sending 'job started' notification, server ID = " + Id + Environment.NewLine);
                } else
                {
                    if (_outputLevel >= 4)
                    {
                        Console.WriteLine("        server " + Id + ": notifying container (job finished)...");
                    }
                    jobData.NotifyJobFinished();
                }
                if (dispatcher == null)
                {
                    Console.WriteLine(Environment.NewLine+ "WARNING: Dispatcher is null when sending 'job started' notification, server ID = " + Id + Environment.NewLine);
                } else
                {
                    if (_outputLevel >= 4)
                    {
                        Console.WriteLine("        server " + Id + ": notifying dispatcher (job finished)...");
                    }
                    _dispatcher.NotifyJobFinished(this, jobData);
                }
            }
            finally
            {
                lock (Lock)
                {
                    _jobData = null;
                }
                NotifyServerIdle(dispatcher);
            }
            if (_outputLevel >= 4)
            {
                Console.WriteLine("        server " + Id + ": NotifyJobFinished() completed.");
            }
        }


        /// <summary>Notifies all interested parties that job has been aborted.</summary>
        public void NotifyJobAborted()
        {
            if (_outputLevel >= 4)
            {
                Console.WriteLine("        server " + Id + ": NotifyJobAborted()...");
            }
            ParallelJobDispatcherBase<JobContainerType> dispatcher = null;
            JobContainerType jobData = null;
            try
            {
                lock (Lock)
                {
                    dispatcher = _dispatcher;
                    jobData = _jobData;
                }
                if (jobData == null)
                {
                    Console.WriteLine(Environment.NewLine+ "WARNING: Job data is null when sending 'job started' notification, server ID = " + Id + Environment.NewLine);
                } else
                {
                    if (_outputLevel >= 4)
                    {
                        Console.WriteLine("        server " + Id + ": notifying container (job aborted)...");
                    }
                    jobData.NotifyJobAborted();
                }
                if (dispatcher == null)
                {
                    Console.WriteLine(Environment.NewLine+ "WARNING: Dispatcher is null when sending 'job started' notification, server ID = " + Id + Environment.NewLine);
                } else
                {
                    if (_outputLevel >= 4)
                    {
                        Console.WriteLine("        server " + Id + ": notifying dispatcher (job aborted)...");
                    }
                    _dispatcher.NotifyJobAborted(this, jobData);
                }
            }
            finally
            {
                lock(Lock)
                {
                    _jobData = null;
                }
                NotifyServerIdle(dispatcher);
            }
            if (_outputLevel >= 4)
            {
                Console.WriteLine("        server " + Id + ": NotifyJobAborted() completed.");
            }
        }

        /// <summary>Sets the <see cref="Idle"/> flag to true and notifies the containing dispatcher that
        /// the current server object has became idle.</summary>
        public void NotifyServerIdle(ParallelJobDispatcherBase<JobContainerType> dispatcher)
        {
            try
            {
                if (_outputLevel >= 4)
                {
                    Console.WriteLine("        server " + Id + ": NotifyServerIdle()...");
                }
                lock (Lock)
                {
                    if (_outputLevel >= 4)
                    {
                        Console.WriteLine("        server " + Id + ": NotifyServerIdle(), inside lock...");
                    }
                    _state = ParallelServerState.Idle;
                    dispatcher = _dispatcher;
                }
                if (dispatcher == null)
                {
                    Console.WriteLine(Environment.NewLine + "WARNING: Dispatcher is null when sending 'server idle' notification, server ID = " + Id + Environment.NewLine);
                }
                else
                {
                    dispatcher.NotifyServerIdle(this);
                }
            }
            catch
            {
                if (_outputLevel >= 4)
                {
                    Console.WriteLine("        server " + Id + ": Exception thrown in NotifyServerIdle().");
                }
            }
            finally
            {
                if (_outputLevel >= 4)
                {
                    Console.WriteLine("        server " + Id + ": NotifyServerIdle() completed.");
                }
            }
        }


        protected bool _isServer = false;

        /// <summary>Indicating whether the current job runner work as server.
        /// <par>If true then a server is started in a parallel thread that continuously for signals to start jobs.</par>
        /// <para>If false then a new thread is started each time a job is run, and is deleted when a job finishes.</para></summary>
        public bool IsServer
        {
            get { lock (Lock) { return _isServer; } }
            set { lock (Lock) { _isServer = true; } }
        }

        protected bool _commandStopServing = false;

        /// <summary>Flag indicating whether the server should be stopped.
        /// If set to true and server thread is runing, then the server thread stops when the currently
        /// run job completes (or stops immediately if there is no job running).</summary>
        public bool CommandStopServing
        {
            get { lock (Lock) { return _commandStopServing; } }
            protected set { lock (Lock) { _commandStopServing = value; } }
        }

        /// <summary>Sends to the server thread command that it has to stop.</summary>
        public void StopServerThread()
        {
            CommandStopServing = true;
        }

        /// <summary>Forces the working to stop, even if it is in the middle of eecution of a job,
        /// by aborting the thread. To let the working thread finish its current jobs and then stop, 
        /// call <see cref="StopServerThread"/></summary>
        public void KillServerThread()
        {
            Thread threadToAbort = null;
            lock (Lock)
            {
                if (_workingThread != null)
                {
                    threadToAbort = _workingThread;
                }
                _workingThread = null;
            }
            if (threadToAbort != null)
            {
                try
                {
                    threadToAbort.Abort();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    try
                    {
                        threadToAbort.Join();
                    }
                    catch (Exception) { }
                }
            }
        }


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

        
        protected Thread _workingThread;


        protected bool _isServerRunning = false;

        /// <summary>Indicates whether the server is currently running or not.</summary>
        public bool IsServerRunning
        {
            get { lock (Lock) { return _isServerRunning; } }
            //set { lock (Lock) { _isServerRunning = value; } }
        }

        private bool _doRunJobByServer = false;  // instructs server to tun a job

        public bool DoRunJobByServer
        {
            get { lock (Lock) { return _doRunJobByServer; } }
            protected set { lock (Lock) { _doRunJobByServer = value; } }
        }

        protected void NotifyServerAboutJob()
        {
            lock (Lock)
            {
                if (_doRunJobByServer)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("ERROR in synchronization of job runner: flag for running a job on server thread is already set.");
                    sb.AppendLine("  Job server ID: " + this.Id);
                    if (JobData != null)
                    {
                        sb.AppendLine("  Job data ID: " + JobData.Id + ", Job ID = " + JobData.DispatcherJobId);
                    }
                    throw new InvalidOperationException(sb.ToString());
                }
                _doRunJobByServer = true;
            }
        }

        protected void Serve()
        {
            try
            {
                if (_outputLevel >= 1)
                {
                    Console.WriteLine("Server " + Id + ": server thread started, sleep time = " + SleepTimeMs + "ms");
                } 
                _isServerRunning = true;
                bool doServe = true;
                while (doServe)
                {
                    bool runJob = false;
                    lock (Lock)
                    {
                        runJob = DoRunJobByServer;
                    }
                    if (runJob)
                    {
                        if (_outputLevel >= 1)
                        {
                            if (_jobData != null)
                            {
                                Console.WriteLine("  Server " + Id + ": job requested, running job " + _jobData.Id + ", dispatcher ID = "
                                    + _jobData.DispatcherJobId);
                            } else
                            {
                                Console.WriteLine("  Server " + Id + ": job requested, null container.");
                            }
                        }
                        DoRunJobByServer = false;
                        RunJobInThread();
                    }
                    if (CommandStopServing)
                    {
                        doServe = false;
                        CommandStopServing = false;
                    }
                    Thread.Sleep(SleepTimeMs);
                }
            }
            catch
            {
                // Remark: Thread abortion is already handled in TunJobInThread.
                if (_outputLevel >= 1)
                {
                    Console.WriteLine("Server " + Id + ": serving thread aborted (exception thrown).");
                }
            }
            finally
            {
                lock (Lock)
                {
                    _isServerRunning = false;
                    _workingThread = null;
                }
                if (_outputLevel >= 1)
                {
                    Console.WriteLine("Server " + Id + ": serving thread finished.");
                }
            }
        }

        public void StartServer()
        {
            if (_outputLevel >= 1)
            {
                Console.WriteLine("Server " + Id + " starting...");
            }
            bool waitThread = false;
            Thread thread = null;
            lock (Lock)
            {
                if (_workingThread != null)
                {
                    if (_workingThread.IsAlive)
                    {
                        waitThread = true;
                        thread = _workingThread;
                    }
                    else
                        _workingThread = null;
                }
            }
            if (waitThread)
            {
                if (_outputLevel >= 1)
                {
                    Console.WriteLine("Working thread still alive in server " + Id + ", joining...");
                }
                thread.Join();
                if (_outputLevel >= 1)
                {
                    Console.WriteLine("... working thread joined in server " + Id + ".");
                }
            }
            lock (Lock)
            {
                _isServerRunning = true;
                _workingThread = new Thread(Serve);
                _workingThread.IsBackground = true;
                _workingThread.Priority = this.ThreadPriority;
                _workingThread.Start();
            }
            if (_outputLevel >= 1)
            {
                Console.WriteLine("... server " + Id + " started.");
            }
        }

        /// <summary>Starts job by the server.</summary>
        public void StartJobByServer()
        {
            lock (Lock)
            {
                if (!_isServerRunning)
                    StartServer();
                NotifyServerAboutJob();
            }
        }

        /// <summary>Starts a single job in a new therad taht is created for this purpose.</summary>
        public void StartSingleJob()
        {
            //Console.WriteLine("**** Starting single job - before lock, server "
            //    + Id + ", job " + (_jobData==null?"null":_jobData.Id.ToString()) );
            bool waitThread = false;
            Thread thread = null;
            if (_outputLevel >= 1)
            {
                Console.WriteLine("Server " + Id + ": starting  single job...");
            }
            lock (Lock)
            {
                //Console.WriteLine("  **** Starting single job - in lock for thread data,  server "
                //    + Id + ", job " + (_jobData==null?"null":_jobData.Id.ToString()) );
                if (OutputLevel >= 1)
                {
                    if (_jobData!=null)
                        Console.WriteLine("  server " + Id + ": single job, Id = " + _jobData.Id + ", disp. Id = " + JobData.DispatcherJobId);
                    else
                        Console.WriteLine("  server " + Id + ": No job data!");
                }
                if (_workingThread != null)
                {
                    if (_workingThread.IsAlive)
                        waitThread = true;
                    thread = _workingThread;
                }
            } 
            //Console.WriteLine("  **** Starting single job - outside lock for thread data,  server "
            //    + Id + ", job " + (_jobData == null ? "null" : _jobData.Id.ToString()) );
            if (waitThread)
            {
                Console.WriteLine(Environment.NewLine + "WARNING: working thread is active when trying to start a new single job."
                    + Environment.NewLine + "  job server ID: " + this.Id + "."
                    +Environment.NewLine );
                thread.Join();
                //Console.WriteLine("  **** Starting single job - inside Join(),  server "
                //    + Id + ", job " + (_jobData == null ? "null" : _jobData.Id.ToString()) );

            }
            lock (Lock)
            {
                //Console.WriteLine("  **** Starting single job - inside lock for thread start,  server "
                //    + Id + ", job " + (_jobData == null ? "null" : _jobData.Id.ToString()) );
                _workingThread = new Thread(RunSingleJobInThread);
                _workingThread.IsBackground = true;
                _workingThread.Priority = this.ThreadPriority;
                _workingThread.Start();
                if (OutputLevel >= 1)
                {
                    Console.WriteLine("  server " + Id + ": starting single job finished.");
                }
            }
            //Console.WriteLine("  **** Starting single job - finished starting single job,  server "
            //    + Id + ", job " + (_jobData == null ? "null" : _jobData.Id.ToString()) );
        }


        /// <summary>Runs the job.</summary>
        /// <param name="jobData">Data container for the job to be run.</param>
        protected abstract void RunJobDefined(JobContainerType jobData);


        /// <summary>Methods that runs the job on the serving thread, for the case when a new thread is
        /// allocated for each job (single job per thread).</summary>
        protected void RunSingleJobInThread()
        {
            try
            {
                RunJobInThread();
            }
            finally
            {
                lock (Lock)
                {
                    _workingThread = null;
                }
            }
        }

        /// <summary>Methods that runs the job on the serving thread, for the case when server thread executes
        /// multiple jobs (server mode).</summary>
        protected void RunJobInThread()
        {
            try
            {
                if (_outputLevel >= 2)
                {
                    Console.WriteLine("    server " + Id + ": notifying job started...");
                }
                NotifyJobStarted();
                if (_outputLevel >= 2)
                {
                    Console.WriteLine("    server " + Id + ": runniing the job...");
                }
                RunJobDefined(_jobData);
                if (_outputLevel >= 2)
                {
                    Console.WriteLine("    server " + Id + ": job finished.");
                }
                NotifyJobFinished();
                
            }
            catch
            {
                if (_state == ParallelServerState.Executing)
                {
                    lock (Lock)
                    {
                        if (_outputLevel >= 2)
                        {
                            Console.WriteLine("    server " + Id + ": notifying job ABORTED...");
                        }
                        // Exception was thrown in the middle of job execution, notify about abortion
                        NotifyJobAborted();
                        if (_outputLevel >= 2)
                        {
                            Console.WriteLine("    server " + Id + ": job aborted.");
                        }
                    }
                }
            }
        }

        public void RunJobSynchronous(JobContainerType jobData)
        {
            StartJob(jobData);
            jobData.WaitJobCompletion();
        }


        /// <summary>Starts the current job in the way specified by internal flags.</summary>
        /// <remarks>If the <see cref="IsServer"/> flag is true then job is started by notifying the continuously
        /// running server thread to run the job. Server thread is created and started if necessary.
        /// If the flag is false then a new thread is started to run the job and the thread exits
        /// when the job is run.</remarks>
        public void StartJob(JobContainerType jobData)
        {
            //Console.WriteLine("  **** Inside startJob(),  server "
            //        + Id + ", job " + (jobData == null ? "null" : jobData.Id.ToString()) );
            lock (Lock)
            {
                this._jobData = jobData;
            }
            if (_isServer)
            {
                //Console.WriteLine("  **** Inside startJob(), sending to server, server "
                //    + Id + ", job " + (_jobData == null ? "null" : _jobData.Id.ToString()) );
                StartJobByServer();
            } else
            {
                //Console.WriteLine("  **** Inside startJob(), in thread, server "
                //    + Id + ", job " + (_jobData == null ? "null" : _jobData.Id.ToString()) );

                StartSingleJob();
            }
            
            //NotifyJobStarted();
        }

        //public void WaitJobCompletion()
        //{

        //    NotifyJobFinished();
        //}


        #endregion OperationData

        #region Misc

        /// <summary>Returns a string representation of the current job server, which contains relevent data
        /// about the server state.</summary>
        public override string ToString()
        {
            lock (Lock)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Parallel job server , ID = " + Id + ":");
                sb.AppendLine("  output level: " + OutputLevel);
                sb.AppendLine("  state: " + State);
                if (IsIdle)
                    sb.AppendLine("  server is idle");
                else
                    sb.AppendLine("  server is NOT idle");
                if (IsServer)
                    Console.WriteLine("  operates in SERVER MODE");
                else
                    Console.WriteLine("  operated in SINGLE JOB PER THREAD mode");
                Console.WriteLine("  server running flag: " + IsServerRunning);
                if (_workingThread == null)
                    Console.WriteLine("  working thread not defined (null reference)");
                else
                {
                    string aliveStr = _workingThread.IsAlive ? "alive" : "not alive";
                    string backgroudStr = _workingThread.IsBackground ? "background" : "foreground";
                    Console.WriteLine("  working thread allocated, " + aliveStr + ", " + backgroudStr);
                }
                if (Dispatcher == null)
                    sb.AppendLine("  not assigned to any dispatcher");
                else
                    sb.AppendLine("  assigned to dispatcher, ID = " + Dispatcher.Id);
                if (JobData == null)
                    sb.AppendLine("  job not defined");
                else
                {
                    sb.AppendLine("  job assigned, ID = " + JobData.Id + ", dispatcher job ID: " + JobData.DispatcherJobId);
                    sb.AppendLine("    job dispatcher ID: " + JobData.DispatcherJobId);
                    sb.AppendLine("    job client ID: " + JobData.ClientJobId);
                    sb.AppendLine("    job state: " + JobData.State);
                }
                sb.AppendLine("  sleep times in ms: " + SleepTimeMs);
                sb.AppendLine("  command to stop serving: " + _commandStopServing);
                sb.AppendLine("  command for running a job: " + _doRunJobByServer);
                return sb.ToString();
            }
        }

        #endregion Misc

    }


}
