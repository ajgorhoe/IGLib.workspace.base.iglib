// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// PARALLEL JOB DATA CONTAINER (input + results)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Job states.</summary>
    /// $A Igor Mar08;
    public enum ParallelJobState
    {
        Uninitialized = 0,
        Initialized,
        Unemployed,
        DataReady,
        EnQueued,
        Executing,
        ResultsReady,
        Aborted,
        ResultsProcessed
    }

    /// <summary>Callback delegate that can be assigned to job container for execution at 
    /// various notification events (such as job started, etc.).</summary>
    /// <param name="jobContainer">Job container that executed the callback.</param>
    public delegate void ParallelJobCallback(ParallelJobContainerBase jobContainer);

    /// <summary>Contains input data and results of a parallel job to be executed, oropertied indicating 
    /// the state of the job, and methods for interaction with job performer and dispatcher.</summary>
    /// <remarks>
    /// <para>This class interacts with parallel job dispatcher that sends a job to the specific server, 
    /// and with the parallel server that executes the job.</para>
    /// </remarks>
    /// $A Igor Aug08;
    public  abstract class ParallelJobContainerBase : ILockable, IIdentifiable
    {

        /// <summary>Constructs a new parallel data container.</summary>
        public ParallelJobContainerBase()
        {
            this._state = ParallelJobState.Initialized;
        }

        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking

        #region OperationData

        protected ParallelJobState _state = ParallelJobState.Uninitialized;

        /// <summary>State of the parallel job whose data is contained in the current object.</summary>
        public ParallelJobState State
        {
            get { lock (Lock) { return _state; } }
            protected internal set { lock (Lock) { _state = value; } }
        }

        /// <summary>Flag indicating whether a job has finished.
        /// <para>Flag is undefined (exception thrown) if state is earlier than <see cref="ParallelJobState.DataReady"/>
        /// or it equals to or is later than <see cref="ParallelJobState.ResultsProcessed"/>.</para></summary>
        public bool IsJobCompleted
        {
            get
            {
                lock (Lock)
                {
                    if (_state == ParallelJobState.ResultsReady)
                        return true;
                    if (_state < ParallelJobState.DataReady || _state >= ParallelJobState.ResultsProcessed)
                        throw new Exception("Job is not in one of the pre- or post- execution states, 'completed' has no sense.");
                    return false;
                }
            }
        }
        
        
        private static object _lockId;

        /// <summary>Lock used for acquiring IDs.</summary>
        public static object LockId
        {
            get
            {
                if (_lockId == null)
                {
                    lock (Util.LockGlobal)
                    {
                        if (_lockId == null)
                            _lockId = new object();

                    }
                }
                return _lockId;
            }
        }

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
        public virtual int Id
        { get { return _id; } }

        protected int _dispatcherJobId = -1;

        /// <summary>Dispatcher's Id of the job contained in the current job data container.
        /// <para>Id is normally assigned by the dispatcher.</para></summary>
        public int DispatcherJobId
        {
            get { return _dispatcherJobId; }
            protected set { _dispatcherJobId = value; }
        }

        protected int _clientJobId = -1;

        /// <summary>Client's Id of the job contained in the current job data container.
        /// <para>Assigned (or not) by the client.</para></summary>
        public int ClientJobId
        {
            get { return _clientJobId; }
            set { _clientJobId = value; }
        }

        protected object _clientData;

        /// <summary>Client's data for the job contained in the current job data container.
        /// <para>Assigned (or not) by the client who creates and sends this job.</para></summary>
        /// <remarks><para>This can be used to help the client determine how to treat results when finished.</para></remarks>
        public object ClientData
        {
            get { return _clientData; }
            protected set { _clientData = value; }
        }


        private static int _defaultSleepTimeMs = 5;

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


        protected ParallelJobDispatcherBase _dispatcher;

        #endregion OperationData


        #region Settings


        /// <summary>Default output level for objects of this and derived types.
        /// <para>Also used for classes derived from <see cref="ParallelJobServerBase{JT}"/> and <see cref="ParallelJobDispatcherBase"/></para></summary>
        public static volatile int DefaultOutputLevel = -1;

        /// <summary>Output level for objects of this class. 
        /// <para>Specifies how much output is printed to console during operation.</para></summary>
        protected int _outputLevel = DefaultOutputLevel;

        /// <summary>Output level for the current object. 
        /// <para>Specifies how much output is printed to console during operation.</para></summary>
        public int OutputLevel
        {
            get { return _outputLevel; }
            set { lock (Lock) { _outputLevel = value; } }
        }

        // private static bool _defaultIsTestMode = false;

        /// <summary>Default value of the testmode flag.
        /// <para>Also used for classes derived from <see cref="ParallelJobServerBase{CT}"/> and <see cref="ParallelJobDispatcherBase"/></para></summary>
        public static volatile bool DefaultIsTestMode = false;

        protected bool _isTestMode = DefaultIsTestMode;

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

        //public static volatile int _defaultOutputLevelTestMode = -1;

        ///// <summary>Default output level in testing mode.
        ///// <para>On first get access, this mode will be 2 greater than the current default output level
        ///// (static property <see cref="DefaultOutputLevel"/>). This happens when any instance 
        ///// property <see cref="OutputLevelTestMode"/> is first accessed, but if the property has
        ///// been set before, this does not apply.</para></summary>
        //public static int DefaultOutputLevelTestMode
        //{
        //    get
        //    {
        //        if (_defaultOutputLevelTestMode < 0)
        //            return DefaultOutputLevel + 2;
        //        // _defaultOutputLevelTestMode = DefaultOutputLevel + 2;
        //        return _defaultOutputLevelTestMode;
        //    }
        //    set { _defaultOutputLevelTestMode = value; }
        //}

        //protected volatile int _outputLevelTestmode = -1;  // when getter is first accessed, this will change to DefaultOutputLevelTestMode

        ///// <summary>Output level that is used in testing mode.
        ///// <para>When getter is first accessed, the property is set to the value of <see cref="DefaultOutputLevelTestMode"/> unless
        ///// it has been set before.</para></summary>
        //public int OutputLevelTestMode
        //{
        //    get { return _outputLevelTestmode; }
        //    set { _outputLevelTestmode = value; if (IsTestMode) OutputLevel = value; }
        //}

        protected volatile int _numTestDelays = 1;

        /// <summary>Number of time intervals into which the total delay interval (property <see cref="TestDelayInSeconds"/>) is divided.</summary>
        public int NumTestDelays
        {
            get { return _numTestDelays; }
            set { _numTestDelays = value; }
        }

        protected double _testDelayInSeconds = 1.0;

        /// <summary>Delay time in the testing mode (in seconds), which is caused by calling sleep for
        /// the <see cref="Thread.Sleep(int)"/> method.</summary>
        public double TestDelayInSeconds
        {
            get { return _testDelayInSeconds; }
            set { _testDelayInSeconds = value; }
        }

        protected double _testDelayRelativeError = 0.0;

        /// <summary>Interval of random errors added to the test delay time, relative to unnoisy delay time.</summary>
        public double TestDelayRelativeError
        {
            get { return _testDelayRelativeError; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1)
                    value = 1;
                _testDelayRelativeError = value;
            }
        }

        /// <summary>Single test delay interval in milliseconds.</summary>
        protected int TestDelaySingleMs
        {
            get
            {
                double inSeconds = _testDelayInSeconds;
                if (_testDelayRelativeError > 0)
                {
                    inSeconds *= (1.0 + (0.5 - RandomGenerator.Global.NextDouble()) * _testDelayInSeconds);
                }
                int num = _numTestDelays;
                int ret = (int)Math.Round(1e3 * inSeconds / (double)num);
                if (inSeconds > 0 && ret == 0)
                    ret = 1;
                return ret;
            }
        }


        #endregion Settings


        #region Events

        protected ParallelJobCallback _onStarted = null;

        /// <summary>Delegate that is executed on started event.
        /// <para>When executing, job container is locked.</para></summary>
        public ParallelJobCallback OnStarted
        {
            get { lock (Lock) { return _onStarted; } }
            set { lock (Lock) { _onStarted = value; } }
        }

        /// <summary>Called when 'started' notification is triggered (within the method <see cref="NotifyJobStarted"/>).</summary>
        protected virtual void RunOnStarted()
        {
            if (_onStarted != null)
                _onStarted(this);
        }

        protected ParallelJobCallback _onFinished = null;

        /// <summary>Delegate that is executed on started event.
        /// <para>When executing, job container is locked.</para></summary>
        public ParallelJobCallback OnFinished
        {
            get { lock (Lock) { return _onFinished; } }
            set { lock (Lock) { _onFinished = value; } }
        }

        /// <summary>Called when 'finished' notification is triggered (within the method <see cref="NotifyJobFinished"/>).</summary>
        protected virtual void RunOnFinished()
        {
            if (_onFinished != null)
                _onFinished(this);
        }

        protected ParallelJobCallback _onAborted = null;

        /// <summary>Delegate that is executed on started event.
        /// <para>When executing, job container is locked.</para></summary>
        public ParallelJobCallback OnAborted
        {
            get { lock (Lock) { return _onAborted; } }
            set { lock (Lock) { _onAborted = value; } }
        }

        /// <summary>Called when 'aborted' notification is triggered (within the method <see cref="NotifyJobAborted"/>).</summary>
        protected virtual void RunOnAborted()
        {
            if (_onAborted != null)
                _onAborted(this);
        }

        /// <summary>Notifies this job data container that its job has started.</summary>
        public void NotifyJobStarted()
        {
            if (_outputLevel >= 3)
            {
                Console.WriteLine("Job started. ID:" + Id + ", disp. ID: " + DispatcherJobId + ", cl. ID: " + ClientJobId);
            }
            lock (Lock)
            {
                _state = ParallelJobState.Executing;
                RunOnStarted();
            }
            if (_outputLevel >= 6)
            {
                Console.WriteLine("Job started notification completed. ID:" + Id);
            }
        }

        /// <summary>Notifies this job data container that its job has finished.</summary>
        public void NotifyJobFinished()
        {
            if (_outputLevel >= 3)
            {
                Console.WriteLine("Job finished. ID:" + Id + ", disp. ID: " + DispatcherJobId + ", cl. ID: " + ClientJobId);
            }
            lock (Lock)
            {
                _state = ParallelJobState.ResultsReady;
                RunOnFinished();
            }
            if (_outputLevel >= 6)
            {
                Console.WriteLine("Job finished notification completed. ID:" + Id);
            }
        }

        /// <summary>Notifies this job data container that its job has been aborted.</summary>
        public void NotifyJobAborted()
        {
            if (_outputLevel >= 3)
            {
                Console.WriteLine("Job aborted. ID:" + Id + ", disp. ID: " + DispatcherJobId + ", cl. ID: " + ClientJobId);
            }
            lock (Lock)
            {
                _state = ParallelJobState.Aborted;
                RunOnAborted();
            }
            if (_outputLevel >= 6)
            {
                Console.WriteLine("Job aborted notification completed. ID:" + Id);
            }
        }

        #endregion Events


        #region Operation


        /// <summary>Assigns the current job to the specified dispatcher.
        /// <para>This method is called in dispatcher's SendJob() method.</para></summary>
        /// <param name="dispatcher">Dispatcher to which the current job is assigned.</param>
        protected internal void AssignToDispatcher(ParallelJobDispatcherBase dispatcher)
        {
            lock (Lock)
            {
                _dispatcher = dispatcher;
                DispatcherJobId = dispatcher.GetNextJobId();
                this.State = ParallelJobState.EnQueued;
            }
        }

        /// <summary>Waits for job completion.</summary>
        public void WaitJobCompletion()
        {
            WaitJobCompletion(0);
        }

        /// <summary>Wait until the current job whose data is contained in this object completes,
        /// or timeout occurs (timeout specified in seconds), and returns a flag indicating whether 
        /// the job has actually completed (i.e. stop was not due to timeout).</summary>
        /// <param name="timeoutInSeconds">Timeot in seconds.</param>
        /// <returns>A flag indicating whether the job was actually completed when the function returned.
        /// If false is returned then timeout occurred.</returns>
        public bool WaitJobCompletion(double timeoutInSeconds)
        {
            if (_outputLevel >= 1)
            {
                Console.WriteLine("Waiting job completion, timeout = " + timeoutInSeconds + " s..."
                    + Environment.NewLine + "  job   ID: " + Id 
                    + Environment.NewLine + "  disp. ID: " + DispatcherJobId
                    + Environment.NewLine + "  cl.   ID: " + ClientJobId);
            }
            if (IsJobCompleted)
                return true;
            else
            {
                bool completed = false, timeoutOccurred = false;
                int sleepMs = SleepTimeMs, timeoutMs = (int)Math.Round(1e3 * timeoutInSeconds);
                if (timeoutMs==0)
                {
                    if (timeoutInSeconds>0)  // prevent setting to 0 due to round-off
                        timeoutMs = 1;
                }
                int totalSleepTime = 0;
                do 
                {
                    Thread.Sleep(sleepMs);
                    totalSleepTime += sleepMs;
                    completed = IsJobCompleted;  // check whether the job has completed
                    if (!completed)
                    {
                        if (timeoutMs > 0 && totalSleepTime >= timeoutMs)
                            timeoutOccurred = true;
                    }
                } while (!completed && !timeoutOccurred);
                if (OutputLevel >= 1)
                {
                    Console.WriteLine("Waiting job completion finished, job ID = " + Id + ", completed = " + completed);
                }
                return completed;
            }
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
                sb.AppendLine("Parallel job container , ID = " + Id + ":");
                sb.AppendLine("  dispatcher's job ID: " + DispatcherJobId);
                sb.AppendLine("  client's job ID:     " + ClientJobId);
                sb.AppendLine("  output level: " + OutputLevel);
                sb.AppendLine("  sleeping time in ms: " + SleepTimeMs);
                sb.AppendLine("  job state: " + State);
                sb.AppendLine("  is completed: " + IsJobCompleted);
                if (ClientData != null)
                    sb.AppendLine("  client data assigned");
                else
                    sb.AppendLine("  client data NOT assigned");
                sb.AppendLine("  event handlers: ");
                sb.AppendLine("    on started: " + OnStarted==null ? "not assigned" : "assigned");
                sb.AppendLine("    on started: " + OnAborted == null ? "not assigned" : "assigned");
                sb.AppendLine("    on started: " + OnFinished == null ? "not assigned" : "assigned");
                return sb.ToString();
            }
        }


        #endregion Misc

        #region Testing


        /// <summary>Test of parallel job execution by calculating the specified real function of one variable in
        /// a number of points. Calculatin is performed in parallel threads by using job dispatcher with parallel 
        /// servers.</summary>
        /// <param name="numPoints">Number of equidistand points in which function is evaluated.</param>
        /// <param name="numServers">Number of servers to be used.</param>
        /// <param name="maxEnqueued">Maximal allowed number of jobs enquied.</param>
        /// <param name="delayTimeSeconds">Delay tie iin seconds.</param>
        /// <param name="delayTimeRelativeError"></param>
        /// <param name="clientOutputLevel">Level of output on dispatching activity within the method.</param>
        /// <returns>Array of calculated results.</returns>
        /// <param name="sleepTimeMs">Sleeping time for server threads.</param>
        public static void TestPerformance(int numPoints,
            int numServers, int maxEnqueued, double delayTimeSeconds, double delayTimeRelativeError, int sleepTimeMs,
            int clientOutputLevel)
        {
            if (numPoints <= 0)
                throw new ArgumentException("Number of points must be greater than 1!");
            Console.WriteLine("Testing parallel dispatcher by evaluating a sine function on the interval [0, 2*Pi]...");
            double from = 0;
            double to = 2.0 * Math.PI;
            SimpleFunctionDelegate<double, double> evaluationFunction = Math.Sin;
            TestPerformance(from, to, numPoints, evaluationFunction, numServers, 
                maxEnqueued, delayTimeSeconds, delayTimeRelativeError, sleepTimeMs, clientOutputLevel);
        }

        /// <summary>Test of parallel job execution by calculating the specified real function of one variable in
        /// a number of points. Calculatin is performed in parallel threads by using job dispatcher with parallel 
        /// servers.</summary>
        /// <param name="from">Lower bount of the interval on which function is evaluated.</param>
        /// <param name="to">Upper bound of teh interval on which function is evaluated.</param>
        /// <param name="numPoints">Number of equidistand points in which function is evaluated.</param>
        /// <param name="evaluationFunction">Delegate that is used to evaluate functions.</param>
        /// <param name="numServers">Number of servers to be used.</param>
        /// <param name="maxEnqueued">Maximal allowed number of jobs enquied.</param>
        /// <param name="delayTimeSeconds">Delay tie iin seconds.</param>
        /// <param name="delayTimeRelativeError"></param>
        /// <param name="clientOutputLevel">Level of output on dispatching activity within the method.</param>
        /// <returns>Array of calculated results.</returns>
        /// <param name="sleepTimeMs">Sleeping time for server threads.</param>
        public static void TestPerformance(double from, double to, int numPoints, SimpleFunctionDelegate<double, double> evaluationFunction,
            int numServers, int maxEnqueued, double delayTimeSeconds, double delayTimeRelativeError, int sleepTimeMs,
            int clientOutputLevel)
        {
            Console.WriteLine();
            Console.WriteLine("Testing parallel dispatcher by evaluating a real function of one variable...");
            Console.WriteLine("  lower bound: " + from);
            Console.WriteLine("  upper bound: " + to);
            Console.WriteLine("  number of evaluation points: "   + numPoints);
            if (numPoints<=0)
                throw new ArgumentException("Number of points must be greater than 1!");
            double[] inputs = new double[numPoints];
            double h = (to-from)/((double) numPoints-1.0);
            for (int i=0; i<numPoints; ++i){
              inputs[i] = from+i*h;
            }
            double [] results = ParallelJobContainerGen<double, double>.TestPerformance(inputs, evaluationFunction, numServers, maxEnqueued,
                delayTimeSeconds, delayTimeRelativeError, sleepTimeMs, clientOutputLevel);
            Console.WriteLine();
            Console.WriteLine("Testing correctness of resuts calculated on parallel servers...");
            Console.WriteLine("  number of inputs: " + numPoints);
            Console.WriteLine("  number of results: " + results.Length);
            int numWrong = 0;
            int maxReported = 10;
            int numReported = 0;
            for (int i = 0; i < numPoints; ++i)
            {
                if (results[i] != evaluationFunction(inputs[i]))
                {
                    ++numWrong;
                    ++numReported;
                    if (numReported <= maxReported)
                    {
                        Console.WriteLine("Result No. " + (i+1) + " is Wrong.");
                    }
                }
            }
            if (numWrong == 0)
                Console.WriteLine("... test finished, ALL results are CORRECT.");
            else
                Console.WriteLine("ERROR: " + numWrong + " result(s) are WRONG!");
        }
        #endregion Testing

    }

}
