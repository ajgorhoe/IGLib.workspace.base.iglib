// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// PARALLEL JOB DISPATCHER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using IG.Lib;

namespace IG.Num
{



    /// <summary>Parallel job dispatcher. Accepts job requests and dispatches jobs to parallel job servers
    /// when available and redy to run a job.</summary>
    /// <typeparam name="JobContainerType">Type of the container that holds data for the job (input and output).</typeparam>
    /// <remarks>
    /// <para>This dispatcher object contains a list of parallel server objects (<see cref="ParallelJobServerBase"/> and 
    /// its subclasses), and sends requested jobs to these objects.</para>
    /// <para>If a job is requested and no servers are in idle state, the job is enqueued and waits for execution
    /// until a server becomes available.</para>
    /// 
    /// <para>Notes for developers:</para>
    /// <para>Dispatcher should be steer  synchronization with servers.</para>
    /// <para>In order to avoid deadlocks, server may not call any dispatcher's code its own lock!
    /// All dispatcher's code must be called out out of code blocks locked on server's lock.</para>
    /// 
    /// <para>History note:</para>
    /// <para>This implementation has been transfered in June 2012 from the IOptLib.Net library and adapted to 
    /// current needs in IGLib.  Some features were omitted and some implementatinon details changed. Implementations 
    /// will be unified in the future, therefore the class may be subject to larger changes. What is currently exposed
    /// should remain more or less the same, so users of the class should not be affected.</para>
    /// </remarks>
    /// $A Igor Aug08 Jun12;
    public class ParallelJobDispatcherBase<JobContainerType>: ParallelJobDispatcherBase
        where JobContainerType : ParallelJobContainerBase
    {

        #region Initialization 

        public ParallelJobDispatcherBase()
            : base()
        {
            StartServer();
        }

        #endregion Initialization

        #region Data

        /// <summary>Gets the number of enqueued jobs.</summary>
        public int NumEnqueuedJobs
        {
            get { lock (_jobQueue) { return _jobQueue.Count; } }
        }

        /// <summary>List of job servers contained by the current dispatcher.</summary>
        protected readonly List<ParallelJobServerBase<JobContainerType>> _jobServers =  
            new List<ParallelJobServerBase<JobContainerType>>();

        /// <summary>List of idle job servers dispatched by teh current dispatcher.</summary>
        protected readonly List<ParallelJobServerBase<JobContainerType>> _idleJobServers = 
            new List<ParallelJobServerBase<JobContainerType>>();


        /// <summary>Adds the specified server to the current dispatcher.</summary>
        /// <param name="server">Server to be added.</param>
        public void AddServer(ParallelJobServerBase<JobContainerType> server)
        {
            if (server == null)
                throw new ArgumentException("Parallel job server to be added is not specified (null reference).");
            if (_outputLevel >= 1)
            {
                Console.WriteLine("Dispatcher " + Id + ": adding server " + server.Id + "...");
            }
            server.Dispatcher = this;
            lock (Lock)
            {
                _jobServers.Add(server);
            }
            if (server.IsIdle)
            {
                AddIdleServer(server);
            }
            if (_outputLevel >= 1)
            {
                Console.WriteLine("  dispatcher " + Id + ": server " + server.Id + " added.");
            }
        }

        /// <summary>Removes the specified server to the current dispatcher.</summary>
        /// <param name="server">Server to be removed.</param>
        public void RemoveServer(ParallelJobServerBase<JobContainerType> server)
        {
            if (server == null)
                throw new ArgumentException("Parallel job server to be removed is not specified (null reference).");
            if (_outputLevel >= 1)
            {
                Console.WriteLine("Dispatcher " + Id + ": removing server " + server.Id + "...");
            }
            lock (server.Lock)
            {
                if (server.Dispatcher == this)
                    server.Dispatcher = null;
            }
            lock (Lock)
            {
                if (_jobServers.Contains(server))
                {
                    _jobServers.Remove(server);
                }
            }
            RemoveIdleServer(server);
            if (_outputLevel >= 1)
            {
                Console.WriteLine("  dispatcher " + Id + ": server " + server.Id + " removed.");
            }
        }

        /// <summary>Adds the specified parallel job server to the idle list.</summary>
        /// <param name="server">Server to be added to the list.</param>
        protected void AddIdleServer(ParallelJobServerBase<JobContainerType> server)
        {
            if (server == null)
                throw new ArgumentException("Parallel job server to be removed from idle servers list is not specified (null reference).");
            lock (ServersLock)
            {
                IncrementNumIdleJobServers();
                _idleJobServers.Add(server);
            }

        }

        /// <summary>Removes the specified parallel job server from the idle list.
        /// <para>If the specified server is not on the list then nothing happens.</para></summary>
        /// <param name="server">Server to be removed from the list.</param>
        protected void RemoveIdleServer(ParallelJobServerBase<JobContainerType> server)
        {
            if (server == null)
                throw new ArgumentException("Parallel job server to be added to idle servers list is not specified (null reference).");
            lock (ServersLock)
            {
                if (_idleJobServers.Contains(server))
                {
                    DecrementNumIdleJobServers();
                    _idleJobServers.Remove(server);
                }
            }
        }

        /// <summary>Sets sleeping time in milliseconds on all servers assigned to the current parallel job dispatcher.</summary>
        /// <param name="sleepTimeMs">The sleeping time in milliseconds to be set on servers.</param>
        public void SetServersSleepTimeMs(int sleepTimeMs)
        {
            ParallelJobServerBase<JobContainerType>[] servers = null;
            lock(Lock)
            {
                servers = _jobServers.ToArray();
            }
            int numServers = servers.Length;
            for (int i = 0; i < numServers; ++i)
            {
                ParallelJobServerBase<JobContainerType> server = servers[i];
                if (server == null)
                {
                    Console.WriteLine(Environment.NewLine + "WARNING: server No. " + i + " is null; dispatcher ID: " + Id + Environment.NewLine);
                } else
                {
                    server.SleepTimeMs = sleepTimeMs;
                }
            }
        }

        /// <summary>Sets output level on all servers assigned to the current parallel job dispatcher.</summary>
        /// <param name="outputLevel">The output level to be set on servers.</param>
        public void SetServersOutputLevel(int outputLevel)
        {
            ParallelJobServerBase<JobContainerType>[] servers = null;
            lock(Lock)
            {
                servers = _jobServers.ToArray();
            }
            int numServers = servers.Length;
            for (int i = 0; i < numServers; ++i)
            {
                ParallelJobServerBase<JobContainerType> server = servers[i];
                if (server == null)
                {
                    Console.WriteLine(Environment.NewLine + "WARNING: server No. " + i + " is null; dispatcher ID: " + Id + Environment.NewLine);
                } else
                {
                    server.OutputLevel = outputLevel;
                }
            }
        }

        /// <summary>Sets the server flag on all servers assigned to the current parallel job dispatcher.</summary>
        /// <param name="isServer">The server flag value to be set on servers.
        /// <para>Specified whether the server acts as server (true) or runs every job in a new thread.</para></param>
        public void SetServersIsServer(bool isServer)
        {
            ParallelJobServerBase<JobContainerType>[] servers = null;
            lock(Lock)
            {
                servers = _jobServers.ToArray();
            }
            int numServers = servers.Length;
            for (int i = 0; i < numServers; ++i)
            {
                ParallelJobServerBase<JobContainerType> server = servers[i];
                if (server == null)
                {
                    Console.WriteLine(Environment.NewLine + "WARNING: setting server flag: server No. " + i + " is null; dispatcher ID: " + Id + Environment.NewLine);
                } else
                {
                    server.IsServer = isServer;
                }
            }
        }

        /// <summary>number of job cervers that are currently asigned to hte dispatcher.</summary>
        public int NumJobServers
        { get { lock (Lock) { return _jobServers.Count; } } }

        /// <summary>Gets number of active job servers assigned to the dispatcher.
        /// <para>These are servers capable of serving jobs, but which may be busy at the moment.</para></summary>
        public int NumActiveJobServers
        {
            get
            {
                lock (Lock)
                {
                    int num = 0;
                    foreach (ParallelJobServerBase<JobContainerType> server in _jobServers)
                    {
                        if (server != null)
                            if (server.IsActive)
                            {
                                ++num;
                            }
                    }
                    return num;
                }
            }
        }

        /// <summary>Gets number of currently executing jobs.
        /// <para>The number is obtained by counting assigned job servers that are in the state of job execution.</para></summary>
        public int NumExecutingJobs
        {
            get
            {
                lock (Lock)
                {
                    int num = 0;
                    foreach (ParallelJobServerBase<JobContainerType> server in _jobServers)
                    {
                        if (server != null)
                            if (server.State == ParallelServerState.Executing)
                            {
                                ++num;
                            }
                    }
                    return num;
                }
            }
        }

        /// <summary>Queue of jobs that could not be immediately served, scheduled for later execution.</summary>
        protected Queue<JobContainerType> _jobQueue = new Queue<JobContainerType>();


        #endregion Data
        
        #region Operation


        /// <summary>Returns the first idle server (last on the list of idle servers) and removes it
        /// from the idle servers list, or returns null if there are no idle servers.</summary>
        /// <returns>The first idle job server available.</returns>
        protected ParallelJobServerBase<JobContainerType> GetFirstIdleServer()
        {
            if (_outputLevel >= 5)
            {
                Console.WriteLine("      Dispatcher " + Id + ": getting first idle server...");
            }
            ParallelJobServerBase<JobContainerType> ret = null;
            bool reArrangeIdleServers = false;
            lock (ServersLock)
            {
                if (NumIdleJobServers != _idleJobServers.Count)
                {
                    reArrangeIdleServers = true;
                    // Inconsistency in number of idle servers and number of servers that 
                    // are on the list of idle servers, we need to correct this:
                    Console.WriteLine(
                        Environment.NewLine
                        + "Inconsistency between declared number of idle servers and number of servers on the list of idle servers."
                        + Environment.NewLine + "  job dispatcher ID: " + this.Id
                        + Environment.NewLine + "  declared number of idle servers: " + NumIdleJobServers
                        + Environment.NewLine + "  number of servers on the idle list: " + _idleJobServers.Count
                        + Environment.NewLine
                    );
                    _idleJobServers.Clear();
                    ResetNumIdleJobServers();
                }
            }
            if (reArrangeIdleServers)
            {
                if (_outputLevel >= 5)
                {
                    Console.WriteLine("      Dispatcher " + Id + ": getting first idle server - rearranging idle list");
                }
                lock (Lock)
                {
                    for (int i = 0; i < _jobServers.Count; ++i)
                    {
                        ParallelJobServerBase<JobContainerType> server = _jobServers[i];
                        if (server!=null)
                            if (server.IsIdle)
                            {
                                AddIdleServer(server);
                            }
                    }
                }
            }
            if (_outputLevel >= 5)
            {
                Console.WriteLine("      Dispatcher " + Id + ": getting first idle server - getting the server");
            }
            lock(ServersLock)
            {
                while (_idleJobServers.Count > 0 && ret == null)
                {
                    ret = _idleJobServers[_idleJobServers.Count - 1];
                    _idleJobServers.RemoveAt(_idleJobServers.Count - 1);
                    DecrementNumIdleJobServers();
                    if (!ret.IsIdle)
                        ret = null;
                }
                if (_outputLevel >= 5)
                {
                    if (ret!=null)
                      Console.WriteLine("      Dispatcher " + Id + ": idle server obtained, Id = " + ret.Id);
                    else
                      Console.WriteLine("      Dispatcher " + Id + ": No idle server found.");
                }
                return ret;
            }
        }

        /// <summary>Adds the specified job container to the execution queue.</summary>
        /// <param name="jobData">Job container of the job that is enqueued.</param>
        protected void EnqueueJob(JobContainerType jobData)
        {
            lock (_jobQueue)
            {
                _jobQueue.Enqueue(jobData);
            }
        }

        /// <summary>Removes the last job from the execution queue and returns it, or returns 
        /// null if there are no jobs on the queue.</summary>
        /// <param name="jobData">Job container of the job that is removed from the queue, or null if there are no jobs in the queue.</param>
        protected JobContainerType DequeueJob()
        {
            lock(_jobQueue)
            {
                if (_jobQueue.Count>0)
                    return _jobQueue.Dequeue();
                else
                    return null;
            }
        }

        /// <summary>Enqueues job for execution, and returns a flag indicating whether a job has been started immediately.</summary>
        /// <param name="jobData">Job data.</param>
        public bool SendJob(JobContainerType jobData)
        {
            bool startedImmediately;
            SendJob(jobData, out startedImmediately);
            return startedImmediately;
        }

        /// <summary>Enqueues job for execution.</summary>
        /// <param name="jobData">Job data.</param>
        /// <param name="startedImmediately">Output flag, set to true if the job has been started 
        /// by some server immediately, false othwrwise.</param>
        public void SendJob(JobContainerType jobData, out bool startedImmediately)
        {
            if (jobData == null)
                throw new ArgumentException("Job data not specified (null argument).");
            if (_outputLevel >= 1)
            {
                Console.WriteLine("Dispatcher " + Id + ": sending job, ID = " + jobData.Id + ", disp. ID: " + jobData.DispatcherJobId);
            }
            lock (Lock)
            {
                jobData.AssignToDispatcher(this);  // this will update job state to Enqued
                IncrementNumSentJobs();
                startedImmediately = false;
                //if (NumIdleJobServers > 0)
                //{
                //    ParallelJobServerBase<JobContainerType> server = GetFirstIdleServer();
                //    if (server != null)
                //    {
                //        if (server.IsIdle)
                //        {
                //            if (_outputLevel >= 1)
                //            {
                //                Console.WriteLine("Dispatcher " + Id + ": sending job immediately, "
                //                    + Environment.NewLine + "  job ID = " + jobData.Id 
                //                    + Environment.NewLine + "  disp. job ID: " + jobData.DispatcherJobId
                //                    + Environment.NewLine + "  server ID: " + server.Id);
                //            }
                //            server.StartJob(jobData);
                //            startedImmediately = true;
                //        }
                //    }
                //}
                if (!startedImmediately)
                {
                    if (_outputLevel >= 1)
                    {
                        Console.WriteLine("Dispatcher " + Id + ": job enqueued. "
                            + Environment.NewLine + "  Job ID: " + jobData.Id
                            + Environment.NewLine + "  disp. job ID: " + jobData.DispatcherJobId);
                    }
                    if (!_isServerRunning)
                    {
                        StartServer();
                    }
                    EnqueueJob(jobData);
                }
            }
        }
        

        /// <summary>Waits for job completion.</summary>
        public void WaitAllJobsCompleted()
        {
            WaitAllJobsCompleted(0);
        }

        /// <summary>Wait until all jobs that weer sent to the current dispatcher object complete,
        /// or timeout occurs (timeout specified in seconds), and returns a flag indicating whether 
        /// the jobs have actually completed (i.e. stop was not due to timeout).</summary>
        /// <param name="timeoutInSeconds">Timeot in seconds. Less or equal to 0 means no timeout 
        /// (waiting for condition fulfilled indefinitely).</param>
        /// <returns>A flag indicating whether the job was actually completed when the function returned.
        /// If false is returned then timeout occurred.</returns>
        public bool WaitAllJobsCompleted(double timeoutInSeconds)
        {
            if (_outputLevel >= 1)
            {
                Console.WriteLine("Waiting job completion, timeout = " + timeoutInSeconds + " s...");
            }
            if (NumUncompletedJobs <= 0)
            {
                if (NumUncompletedJobs < 0)
                    throw new InvalidOperationException("Job dispatcher No. " + Id + ": number of executing jobs is less than 0.");
                return true;
            } else
            {
                bool completed = false, timeoutOccurred = false;
                int sleepMs = SleepTimeMs, timeoutMs = (int)Math.Round(1e3 * timeoutInSeconds);
                if (timeoutMs == 0)
                {
                    if (timeoutInSeconds > 0)  // prevent setting to 0 due to round-off
                        timeoutMs = 1;
                }
                int totalSleepTime = 0;
                do
                {
                    Thread.Sleep(sleepMs);
                    totalSleepTime += sleepMs;
                    completed = (NumUncompletedJobs <= 0);  // check whether all jobs have completed
                    if (!completed)
                    {
                        if (timeoutMs > 0 && totalSleepTime >= timeoutMs)
                            timeoutOccurred = true;
                    } else
                    {
                        if (NumUncompletedJobs < 0)
                            throw new InvalidOperationException("Job dispatcher No. " + Id + ": number of executing jobs is less than 0.");
                    }
                } while (!completed && !timeoutOccurred);
                if (OutputLevel >= 1)
                {
                    Console.WriteLine("Waiting job completion finished, job ID = " + Id + ", completed = " + completed);
                }
                return completed;
            }
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
        /// <remarks>
        /// <para>If new jobs are sent after the server has been stopped, the server is restarted automatically.</para>
        /// </remarks>
        public void StopServer()
        {
            CommandStopServing = true;
        }

        /// <summary>Waits until all jobs are completed, and then stops the server.</summary>
        /// <remarks>
        /// <para>If new jobs are sent after the server has been stopped, the server is restarted automatically.</para>
        /// </remarks>
        public bool StopServerWhenAllJobsDone()
        {
            return StopServerWhenAllJobsDone(0);
        }

        /// <summary>Waits until all jobs are completed or timeout occurs, and then stops the server.
        /// Returns a flag indicating whether all jobs are actually completed before server was ordered to stop.</summary>
        /// <param name="timeoutInSeconds">Timeot in seconds. Less or equal to 0 means no timeout defined
        /// (waiting for condition fulfilled indefinitely).</param>
        /// <remarks>
        /// <para>If new jobs are sent after the server has been stopped, the server is restarted automatically.</para>
        /// </remarks>
        public bool StopServerWhenAllJobsDone(double timeoutInSeconds)
        {
            bool ret = false;
            ret = WaitAllJobsCompleted(timeoutInSeconds);
            CommandStopServing = true;
            return ret;
        }

        /// <summary>Forces the working thread to stop. 
        /// To let the working thread finish its current jobs and then stop, call <see cref="StopServer"/></summary>
        public void KillDispatcherThread() 
        {
            Thread threadToAbort = null;
            lock (Lock)
            {
                threadToAbort = _workingThread;
                _workingThread = null;
            }
            if (threadToAbort != null)
            {
                try
                {
                    threadToAbort.Abort();
                }
                catch  {  }
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


        /// <summary>Forces all the server threads to stop, even if in the middle of eecution of a job. 
        /// To let the working thread finish its current jobs and then stop, call <see cref="StopServer"/></summary>
        public void KillServerThreads()
        {
            ParallelJobServerBase<JobContainerType>[] servers = null;
            lock (Lock)
            {
                servers = _jobServers.ToArray();
            }
            int numServers = servers.Length;
            for (int i = 0; i < numServers; ++i)
            {
                ParallelJobServerBase<JobContainerType> server = servers[i];
                server.KillServerThread();
            }
        }


        /// <summary>Forces all the server threads to stop, even if in the middle of eecution of a job. 
        /// To let the working thread finish its current jobs and then stop, call <see cref="StopServer"/></summary>
        public void KillThreads()
        {
                KillDispatcherThread();
                KillServerThreads();
        }


        /// <summary>Updates thread priority (property <see cref="ThreadPriority"/>) to the current global 
        /// thread priority (the <see cref="UtilSystem.ThreadPriority"/> property).</summary>
        protected virtual void UpdateThreadPriorityFromSystem()
        {
            ThreadPriority = UtilSystem.ThreadPriority;
        }

        /// <summary>Whether the "event" handler for system priprity changes has already been registered.</summary>
        protected bool _systemPriorityUpdatesRegistered = false;

        /// <summary>Registers the <see cref="UpdateThreadPriorityFromSystem"/> method as "event handler"
        /// for system priority changes. After registration, this method will be called every time the value 
        /// of the <see cref="UtilSystem.ThreadPriority"/> property changes.</summary>
        public void RegisterSystemPriorityUpdating()
        {
            bool doRegister = false;
            lock (Lock)
            {
                doRegister = !_systemPriorityUpdatesRegistered;
            }
            if (doRegister)
            {
                UtilSystem.AddOnThreadPriorityChange(UpdateThreadPriorityFromSystem);
                lock (Lock)
                {
                    _systemPriorityUpdatesRegistered = true;
                }
            }
        }


        /// <summary>Unregisters the <see cref="UpdateThreadPriorityFromSystem"/> method as "event handler"
        /// for system priority changes.</summary>
        /// <seealso cref="RegisterSystemPriorityUpdating"/>
        public void UnregisterSystemPriorityUpdating()
        {
            try
            {
                UtilSystem.RemoveOnThreadPriorityChange(UpdateThreadPriorityFromSystem);
                lock (Lock)
                {
                    _systemPriorityUpdatesRegistered = false;
                }
            }
            catch { }
        }


        protected ThreadPriority _threadPriority = UtilSystem.ThreadPriority;

        /// <summary>Priority of the dispatcher and contained server threads.
        /// <para>Setting priority changes priority of the dispatcher thread and all server threads.</para>
        /// <para>When threads are created, they will adopt the priority.</para></summary>
        public ThreadPriority ThreadPriority
        {
            get { lock (Lock) { return _threadPriority; } }
            set
            {
                bool changed = false;
                ParallelJobServerBase<JobContainerType>[] servers = null;
                lock (Lock)
                {
                    if (value != _threadPriority)
                    {
                        changed = true;
                        _threadPriority = value;
                        if (_workingThread != null)
                            _workingThread.Priority = value;
                        servers = _jobServers.ToArray();
                    }
                }
                if (changed)
                {
                    if (servers != null)
                    {
                        for (int i = 0; i < servers.Length; ++i)
                        {
                            ParallelJobServerBase<JobContainerType> server = servers[i];
                            if (server != null)
                                server.ThreadPriority = value;
                        }
                    }
                }
            }
        }

        protected Thread _workingThread;

        protected bool _isServerRunning = false;


        /// <summary>Method executed in the queue server thread. Excecutes eventual enqueued jobs 
        /// as job servers become idle.</summary>
        protected void Serve()
        {
            try
            {
                if (_outputLevel >= 1)
                {
                    Console.WriteLine("Dispatcher " + Id + ": server started.");
                }
                _isServerRunning = true;
                bool doServe = true;
                while (doServe)
                {
                    try
                    {
                        JobContainerType jobData = null;
                        ParallelJobServerBase<JobContainerType> idleServer = null;
                        do
                        {
                            // Try to pick & run the next enqueued job:
                            jobData = null;
                            lock (_jobQueue)
                            {
                                if (_jobQueue.Count > 0)
                                {
                                    idleServer = GetFirstIdleServer();
                                    if (idleServer != null)
                                    {
                                        jobData = _jobQueue.Dequeue();
                                    }
                                }
                            }
                            if (jobData != null)
                            {
                                if (_outputLevel >= 1)
                                {
                                    Console.WriteLine("Dispatcher " + Id + ": job dequeued and sent to server... "
                                        + Environment.NewLine + "  job ID = " + jobData.Id
                                        + Environment.NewLine + ", disp. job ID: " + jobData.DispatcherJobId
                                        + Environment.NewLine + ", server ID: " + idleServer.Id);
                                }
                                idleServer.StartJob(jobData);
                            }
                            if (CommandStopServing)
                            {
                                doServe = false;
                                CommandStopServing = false;
                            }
                        } while (doServe && jobData != null);  // && NumIdleJobServers > 0);
                        if (CommandStopServing)
                        {
                            doServe = false;
                            CommandStopServing = false;
                        }
                        if (doServe)
                            Thread.Sleep(SleepTimeMs);
                    }
                    catch {
                        if (_outputLevel >= 1)
                        {
                            Console.WriteLine(Environment.NewLine +
                                "WARNING: exception thrown in server thread! Dispatcher ID: " + Id + Environment.NewLine);
                        }
                    }
                }
            }
            catch
            {
                // Remark: Thread abortion is already handled in TunJobInThread.
                if (_outputLevel >= 1)
                {
                    Console.WriteLine("Dispatcher " + Id + ": WARNING: queue server stopped due to exception.");
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
                    Console.WriteLine("Dispatcher " + Id + ": queue server stopped.");
                }
            }
        }

        /// <summary>Starts the queue server.</summary>
        public void StartServer()
        {
            if (_outputLevel >= 1)
            {
                Console.WriteLine("Dispatcher " + Id + ": queue server starting...");
            }
            lock (Lock)
            {
                _isServerRunning = true;
                if (_workingThread != null)
                    if (!_workingThread.IsAlive)
                        _workingThread = null;
                if (_workingThread == null)
                {
                    _workingThread = new Thread(Serve);
                    _workingThread.IsBackground = false;   // application will not exit until this thread finishes
                    _workingThread.Priority = this.ThreadPriority;
                }
                _workingThread.Start();
            }
        }


        /// <summary>Notifies the current dispatched that the specified job has started on the specified server.</summary>
        /// <param name="server">Server to which the job is assigned.</param>
        /// <param name="job">The job in question.</param>
        /// <remarks>
        /// <para>This functionality is currently not implemented and is prepared in advanced e.g. for the ability of pulsing.</para>
        /// </remarks>
        public void NotifyJobStarted(ParallelJobServerBase<JobContainerType> server, JobContainerType job)
        {
            // Console.WriteLine("**** NotifyJobStarted() called.");
            if (_outputLevel >= 3)
                Console.WriteLine("Dispatcher " + Id + ": NotifyJobStarted()...");
            IncrementNumStartedJobs();
            if (_outputLevel >= 3)
                Console.WriteLine("Dispatcher " + Id + ": NotifyJobStarted() completed.");
        }

        /// <summary>Notifies the current dispatched that the specified job has started on the specified server.</summary>
        /// <param name="server">Server to which the job is assigned.</param>
        /// <param name="job">The job in question.</param>
        /// <remarks>
        /// <para>This functionality is currently not implemented and is prepared in advanced e.g. for the ability of pulsing.</para>
        /// </remarks>
        public void NotifyJobFinished(ParallelJobServerBase<JobContainerType> server, JobContainerType job)
        {
            if (_outputLevel >= 3)
                Console.WriteLine("Dispatcher " + Id + ": NotifyJobFinished()...");
            IncrementNumFinishedJobs();
            if (_outputLevel >= 3)
                Console.WriteLine("Dispatcher " + Id + ": NotifyJobFinished() completed.");
        }

        /// <summary>Notifies the current dispatched that the specified job has started on the specified server.</summary>
        /// <param name="server">Server to which the job is assigned.</param>
        /// <param name="job">The job in question.</param>
        /// <remarks>
        /// <para>This functionality is currently not implemented and is prepared in advanced e.g. for the ability of pulsing.</para>
        /// </remarks>
        public void NotifyJobAborted(ParallelJobServerBase<JobContainerType> server, JobContainerType job)
        {
            if (_outputLevel >= 3)
                Console.WriteLine("Dispatcher " + Id + ": NotifyJobAborted()...");
            IncrementNumAbortedJobs();
            if (_outputLevel >= 3)
                Console.WriteLine("Dispatcher " + Id + ": NotifyJobAborted() completed.");
        }


        /// <summary>Notifies the current parallel job dispatcher that the specified server has become idle.
        /// <para>This increases the number of idle servers and puts the server to the list of idle serves.</para>
        /// <para>When an idle server is requested, it is still checked on the server whether it is actually idle or not.</para></summary>
        /// <param name="server">Server that has become idle.</param>
        public void NotifyServerIdle(ParallelJobServerBase<JobContainerType> server)
        {
            if (server == null)
                throw new ArgumentException("Server that became idle is not specified (null reference).");
            if (_outputLevel >= 3)
            {
                Console.WriteLine("Dispatcher " + Id + ": NotifyServerIdle(), server " + server.Id + " ...");
            }
            lock (ServersLock)
            {
                IncrementNumIdleJobServers();
                _idleJobServers.Add(server);
            }

            //JobContainerType job = null;
            //lock (Lock)
            //{
            //    if (_jobQueue.Count > 0)
            //    {
            //        job = _jobQueue.Dequeue();
            //    }
            //    if (job == null)
            //    {
            //        IncrementNumIdleJobServers();
            //        _idleJobServers.Add(server);
            //    }
            //}
            //if (_outputLevel >= 3)
            //{
            //    if (job != null)
            //    {
            //        Console.WriteLine("Dispatcher " + Id + ": NotifyServerIdle(), server " + server.Id + " ."
            //            + Environment.NewLine + "  job will be run immediately, Id = " + job.Id);
            //    } else
            //    {
            //        Console.WriteLine("Dispatcher " + Id + ": NotifyServerIdle(), server " + server.Id + " .");
            //    }
            //}
            //// TODO: check whether the job can be started within lock without causing deadlocks!
            //if (job != null)
            //{
            //    server.StartJob(job);
            //}

        }


        #endregion Operation

        #region Misc

        /// <summary>Returns a string representation of the current job dispatcher, which contains relevent data
        /// about the server state.</summary>
        public override string ToString()
        {

            lock (Lock)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Parallel job dispatcher , ID = " + Id + ":");
                sb.AppendLine("  output level: " + OutputLevel);
                sb.AppendLine("  number of servers: " + NumIdleJobServers);
                sb.AppendLine("  servers active:  " + NumActiveJobServers + ", idle: " + NumIdleJobServers);
                sb.AppendLine("  jobs dispatched: ");
                sb.AppendLine("    total sent:    " + NumSentJobs);
                sb.AppendLine("    total started: " + NumStartedJobs);
                sb.AppendLine("    finished:      " + NumFinishedJobs);
                sb.AppendLine("    aborted:       " + NumAbortedJobs);
                sb.AppendLine("    uncompleted:   " + NumUncompletedJobs);
                sb.AppendLine("    enqueued:      " + NumEnqueuedJobs);
                sb.AppendLine("    executing:     " + NumExecutingJobs);
                if (_isServerRunning)
                    Console.WriteLine("  server is running");
                else
                    Console.WriteLine("  server is currently NOT running");
                if (_workingThread == null)
                    Console.WriteLine("  working thread not defined (null reference)");
                else
                {
                    string aliveStr = _workingThread.IsAlive ? "alive" : "not alive";
                    string backgroudStr = _workingThread.IsBackground ? "background" : "foreground";
                    Console.WriteLine("  working thread allocated, " + aliveStr + ", " + backgroudStr);
                }
                if (NumJobServers == 0)
                    sb.AppendLine("  NO SERVERS assigned");
                else
                {
                    sb.AppendLine("  assigned servers (" + NumJobServers + ", " + NumIdleJobServers + " idle): ");
                    sb.Append("    IDs: ");
                    for (int i=0; i<_jobServers.Count; ++i)
                    {
                        ParallelJobServerBase<JobContainerType> server = _jobServers[i];
                        if (server == null)
                            sb.Append("null");
                        else
                            sb.Append(server.Id);
                        if (i < _jobServers.Count - 1)
                            sb.Append(", ");
                    }
                    sb.AppendLine();
                    for (int i = 0; i < _jobServers.Count; ++i)
                    {
                        ParallelJobServerBase<JobContainerType> server = _jobServers[i];
                        sb.AppendLine("Assigned server No. " + i + ": ");
                        if (server == null)
                            sb.AppendLine("  Server is not specified (null reference)!");
                        sb.Append(server.ToString());
                    }
                }
                return sb.ToString();
            }
        }

        #endregion Misc

    }  // class ParallelJobDispatcherBase<JobContainerType>


    

    /// <summary>Base class for parallel job dispatchers. Accepts job requests and dispatches jobs to parallel job servers
    /// when available and redy to run a job.</summary>
    /// <typeparam name="DataContainerType">Type of the container that holds data for the job (input and output).</typeparam>
    /// <remarks>
    /// <para>A non-generic class is defined in order to be used in the job data container class (<see cref="ParallelJobContainerBase"/>).</para>
    /// </remarks>
    /// $A Igor Aug08;
    public abstract class ParallelJobDispatcherBase: ILockable, IIdentifiable
    {

        #region ThreadLocking

        private readonly object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        /// <summary>Lock that is usef for locking code that can be run from servers.
        /// <para>List of idle servers is locked by this lock, as well as various data that is accessed on
        /// job start, abort or finished events on servers, such as number of started jobs.</para></summary>
        protected readonly  object ServersLock = new object();

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
        protected volatile int _outputLevel = ParallelJobContainerBase.DefaultOutputLevel;

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

        protected bool _isTestMode = ParallelJobContainerBase.DefaultIsTestMode;

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


        #region IIdentifiable

        private static int _nextId = 0;

        /// <summary>Returns another ID that is unique for objects of the containing class 
        /// its and derived classes.</summary>
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

        #endregion IIdentifiable


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


        private int _nextJobId = 0;

        /// <summary>Generates and returns a new Job Id that is unique in the scope of the current dispatcher.</summary>
        protected internal int GetNextJobId()
        {
            lock (Lock)
            {
                ++_nextJobId;
                return _nextJobId;
            }
        }


        private int _numIdleJobServers = 0;
        
        /// <summary>Gets the number of idle job servers that are currently available on the dispatcher.</summary>
        public int NumIdleJobServers
        {
            get { lock (ServersLock) { return _numIdleJobServers; } }
        }

        /// <summary>Resets number of idle job servers to 0.</summary>
        protected void ResetNumIdleJobServers()
        {
            lock (ServersLock)
            {
                _numIdleJobServers = 0;
            }
        }

        /// <summary>Increments by one the number of idle job servers that are currently available on the dispatcher.</summary>
        protected internal void IncrementNumIdleJobServers()
        {
            lock (ServersLock)
            {
                ++_numIdleJobServers;
            }
        }

        /// <summary>Decrements by one the number of idle job servers that are currently available on the dispatcher.</summary>
        protected internal void DecrementNumIdleJobServers()
        {
            lock(ServersLock)
            {
                --_numIdleJobServers;
            }
        }

        
        protected int _numSentJobs = 0;

        /// <summary>Gets the number of sent jobs (all jobs sent to the current dispatcher for execution).</summary>
        public int NumSentJobs
        {
            get { lock (Lock) { return _numSentJobs; } }
        }

        /// <summary>Increments by one the number of sent jobs (all jobs sent to the current dispatcher for execution).</summary>
        public void IncrementNumSentJobs()
        {
            lock (Lock)
            {
                ++_numSentJobs;
            }
        }
        
        private int _numStartedJobs = 0;

        /// <summary>Gets the number of jobs started by the dispatcher up to this point.</summary>
        public int NumStartedJobs
        {
            get { lock (ServersLock) { return _numStartedJobs; } }
        }

        /// <summary>Increments by one the number of  started by the dispatcher up to this point.</summary>
        public void IncrementNumStartedJobs()
        {
            // Console.WriteLine("    **** IncrementNumStartedJobs() called. ");
            lock (ServersLock)
            {
                ++_numStartedJobs;
            }
        }

        private int _numFinishedJobs = 0;

        /// <summary>Gets the number of finished jobs (of those handled by the current dispatcher) up to this point.</summary>
        public int NumFinishedJobs
        {
            get { lock (ServersLock) { return _numFinishedJobs; } }
        }

        /// <summary>Increments by one the number of finished jobs (of those handled by the current dispatcher) up to this point.</summary>
        public void IncrementNumFinishedJobs()
        {
            lock (ServersLock)
            {
                ++_numFinishedJobs;
            }
        }

        private int _numAbortedJobs = 0;

        /// <summary>Gets the number of aborted jobs (of those handled by the current dispatcher) up to this point.</summary>
        public int NumAbortedJobs
        {
            get { lock (ServersLock) { return _numAbortedJobs; } }
        }

        /// <summary>Increments by one the number of aborted jobs (of those handled by the current dispatcher) up to this point.</summary>
        public void IncrementNumAbortedJobs()
        {
            lock (ServersLock) 
            {
                ++_numAbortedJobs;
            }
        }

        /// <summary>Gets the number of idle job runners that are currently available on the dispatcher.</summary>
        public int NumUncompletedJobs
        {
            get { lock (Lock) { return NumSentJobs - NumFinishedJobs - NumAbortedJobs; } }
        }


        #endregion OperationData


        #region Operation




        #endregion Operation



    }  // class ParallelJobDispatcherBase
}
