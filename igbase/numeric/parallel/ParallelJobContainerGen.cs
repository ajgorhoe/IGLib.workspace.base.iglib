// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// PARALLEL JOB DATA CONTAINERS (input + results) / GENERAL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using IG.Lib;






namespace IG.Num
{


    /// <summary>A generic delegate that can contain simple functions that take one input argument 
    /// of the specified type, and return result of another type.</summary>
    /// <typeparam name="InputType">Type of the function input argument.</typeparam>
    /// <typeparam name="ResultType">Type of function result (returned by the function).</typeparam>
    /// <param name="input">Input argument of the function.</param>
    /// <returns>Result of calculation performed by the function.</returns>
    public delegate ResultType SimpleFunctionDelegate<InputType, ResultType> (InputType input);

    /// <summary>Callback delegate that can be assigned to job container for execution at 
    /// various notification events (such as job started, etc.). Used in simple events.</summary>
    /// <typeparam name="InputType">Type of parallel jobs input data.</typeparam>
    /// <typeparam name="ResultType">Type of parallel jobs results data.</typeparam>
    /// <param name="jobContainer">Job container that executed the callback.</param>
    public delegate void ParallelJobCallbackGen<InputType, ResultType>(ParallelJobContainerGen<InputType, ResultType> jobContainer);
    


    /// <summary>General purpose parallel job container that contains methods for running the job in 
    /// on the same machine thread. Contains many auxiliary methods for testing and for adaptation of 
    /// parallel jobs concepts to different tasks. 
    /// <para>Contains input data and results of a parallel job to be executed, oropertied indicating 
    /// the state of the job, and methods for interaction with job performer and dispatcher.</para>
    /// </summary>
    /// <remarks>
    /// <para>This class interacts with parallel job dispatcher that sends a job to the specific server, 
    /// and with the parallel server that executes the job.</para>
    /// </remarks>
    /// $A Igor Aug08;
    public class ParallelJobContainerGen<InputType, ResultType> :
        ParallelJobContainerBase, ILockable, IIdentifiable
    {

        #region Construction

        /// <summary>Creates a new parallel job container.</summary>
        public ParallelJobContainerGen()
            : base() // base() sets state to Initialized.
        {  }

        /// <summary>Creates a new parallel job container with inptu data assigned.</summary>
        /// <param name="evaluationDelegate">Dellegate that performs calculation.</param>
        public ParallelJobContainerGen(SimpleFunctionDelegate<InputType, ResultType> evaluationDelegate)
            : this()
        {
            this.EvaluationDelegate = evaluationDelegate;
            this.State = ParallelJobState.DataReady;
        }


        /// <summary>Creates a new parallel job container with inptu data assigned.</summary>
        /// <param name="EvaluationDelegate">Dellegate that performs calculation.</param>
        /// <param name="inputData">Inpt data for the job.</param>
        public ParallelJobContainerGen(SimpleFunctionDelegate<InputType, ResultType> evaluationDelegate, 
            InputType inputData): this(evaluationDelegate)
        {
            this.Input = inputData;
        }

        /// <summary>Creates a new parallel job container with inptu data assigned.</summary>
        /// <param name="EvaluationDelegate">Dellegate that performs calculation.</param>
        /// <param name="inputData">Inpt data for the job.</param>
        public ParallelJobContainerGen(SimpleFunctionDelegate<InputType, ResultType> evaluationDelegate,
            InputType inputData, object clientData)
            : this(evaluationDelegate, inputData)
        {
            this.ClientData = clientData;
        }

        #endregion construction


        #region Data

        protected InputType _input;

        /// <summary>Input for the current job.</summary>
        public InputType Input
        {
            get { lock (Lock) { return _input; } }
            set { 
                lock (Lock) { 
                    _input = value;
                    if (value != null)
                        State = ParallelJobState.DataReady;
                }
            }
        }


        protected ResultType _result;

        /// <summary>Result of the current job.</summary>
        public ResultType Result
        {
            get { lock (Lock) { return _result; } }
            set { lock (Lock) { 
                _result = value;
                if (value != null)
                {
                    State = ParallelJobState.ResultsReady;
                }
            } }
        }

        #endregion data

        
        #region Events

        protected ParallelJobCallbackGen<InputType, ResultType> _onStartedGeneric = null;

        /// <summary>Delegate that is executed on started event.
        /// <para>When executing, job container is locked.</para></summary>
        public ParallelJobCallbackGen<InputType, ResultType> OnStartedGeneric
        {
            get { lock (Lock) { return _onStartedGeneric; } }
            set { lock (Lock) { _onStartedGeneric = value; } }
        }

        /// <summary>Called when 'started' notification is triggered (within the method <see cref="NotifyJobStarted"/>).</summary>
        protected override void RunOnStarted()
        {
            base.RunOnStarted();
            if (_onStartedGeneric != null)
                _onStartedGeneric(this);
        }

        protected ParallelJobCallbackGen<InputType, ResultType> _onFinishedGeneric = null;

        /// <summary>Delegate that is executed on started event.
        /// <para>When executing, job container is locked.</para></summary>
        public ParallelJobCallbackGen<InputType, ResultType> OnFinishedGeneric
        {
            get { lock (Lock) { return _onFinishedGeneric; } }
            set { lock (Lock) { _onFinishedGeneric = value; } }
        }

        /// <summary>Called when 'finished' notification is triggered (within the method <see cref="NotifyJobFinished"/>).</summary>
        protected override void RunOnFinished()
        {
            base.RunOnFinished();
            if (_onFinishedGeneric != null)
                _onFinishedGeneric(this);
        }

        protected ParallelJobCallbackGen<InputType, ResultType> _onAbortedGeneric = null;

        /// <summary>Delegate that is executed on started event.
        /// <para>When executing, job container is locked.</para></summary>
        public ParallelJobCallbackGen<InputType, ResultType> OnAbortedGeneric
        {
            get { lock (Lock) { return _onAbortedGeneric; } }
            set { lock (Lock) { _onAbortedGeneric = value; } }
        }

        /// <summary>Called when 'aborted' notification is triggered (within the method <see cref="NotifyJobAborted"/>).</summary>
        protected override void RunOnAborted()
        {
            base.RunOnAborted();
            if (_onAbortedGeneric != null)
                _onAbortedGeneric(this);
        }

        #endregion Events


        #region JobExecution

        protected SimpleFunctionDelegate<InputType, ResultType> _evaluationDelegate = null;

        /// <summary>Evaluation delegate that is by default used for calculation of results from input 
        /// data of the parallel job.</summary>
        /// <remarks>
        /// <para>This delegate can be specified by the user of the job container object in order to 
        /// specify means of how job is performed. The delegate is called on input data in order to
        /// calculate and provide job results via returned value.</para>
        /// <para>Normally, job is perfomed by the job server object that knows how to perfom the job.
        /// However, some job server objects will use this delegate in order toperform the job. The delegate
        /// should be accessed through calling the <see cref="CalculateResultsPlain"/> method.</para>
        /// </remarks>
        public SimpleFunctionDelegate<InputType, ResultType> EvaluationDelegate
        {
            protected get { lock (Lock) { return _evaluationDelegate; } }
            set { lock (Lock) { _evaluationDelegate = value; } }
        }

        /// <summary>Calculates and returns results from the specified input data.</summary>
        /// <param name="input">Input for calculation.</param>
        /// <returns>Results of calculation.</returns>
        /// <remarks>
        /// <para>Warning: This method should only be called if the  flag is true. Otherwise, the job container itself does
        /// not know how to execute the job, an this must be done by the appropriate parallel job server object 
        /// (base type <see cref="ParallelJobServerBase"/>).</para>
        /// <para>When overriding this method, it must be checked that consistency with the <see cref="IsJobDefined"/>
        /// flag is preserved (one may also need to override the flag).</para>
        /// <para>The parallel job can be executed on the current job container by the <see cref="RunJob"/> 
        /// method. That method in turn calls this method, and by overriding the method one can modify
        /// how job execution is performed.</para>
        /// <para>The basic variant defined in the <see cref="ParallelJobContainerGen"/> class just calls the 
        /// <see cref="EvaluationDelegate"/> delegate to perform the job, or throws exception if the delegate is 
        /// not assigned. This behavior can be chabged by overriding the method.</para>
        /// </remarks>
        protected internal virtual ResultType CalculateResultsPlain(InputType input)
        {
            SimpleFunctionDelegate<InputType, ResultType> dlg = EvaluationDelegate;
            if (dlg != null)
                return dlg(input);
            else
                throw new InvalidOperationException("Evaluation delegate is not specified (null reference).");
        }


        protected bool _doesNotContainJobDefinition = false;

        /// <summary>Indicates whether the curren job container can itself execute a job by calling
        /// either variant of the <see cref="RunJob"/> method.</summary>
        /// <remarks><para>This flag must be consistent with the <see cref="CalculateResultsPlain"/> and <see cref="RunJob"/> methods. If any of
        /// these method is overridden, consistency must be preserved, which may require overriding the property,
        /// too.</para></remarks>
        public virtual bool IsJobDefined
        {
            get { lock (Lock) { return !_doesNotContainJobDefinition && _evaluationDelegate != null; } }
            set { _doesNotContainJobDefinition = !value; }
        }

        /// <summary>Performs the job by the data contained in the current job container.
        /// <para>Warning: only works when the <see cref="IsJobDefined"/> flag is true.</para></summary>
        /// <remarks>
        /// <para>Warning: This method should only be called if the  flag is true. Otherwise, the job container itself does
        /// not know how to execute the job, an this must be done by the appropriate parallel job server object 
        /// (base type <see cref="ParallelJobServerBase"/>).</para>
        /// <para>When overriding this method, it must be checked that consistency with the <see cref="IsJobDefined"/>
        /// flag is preserved (one may also need to override the flag).</para>
        /// <para>Parallel jobs by data contained job container objects are normally run by the job
        /// server objects that know how to execute teh job. Im this type of job containers, it can be 
        /// defined by the job container how to run the job in a parallel thread, and this can be 
        /// done by calling this method. This method can also be used by job servers to execute the job.</para>
        /// <para>Calculation is performed by calling the <see cref="CalculateResultsPlain"/> method.
        /// In its original form, that method just calls the <see cref="EvaluationDelegate"/> delegate
        /// and throws exception if the delegate is not defined. The behavior can be changed either
        /// by overriding the <see cref="CalculateResultsPlain"/> method (recommendable) or by overriding
        /// this method (not recommended, but still left as choice).</para>
        /// </remarks>
        public virtual void RunJob()
        {
            if (OutputLevel>=1)
                Console.WriteLine("Running job by container's method, container No. " + Id + "...");
            InputType input = default(InputType);
            ResultType result = default(ResultType);
            lock (Lock)
            {
                input = this.Input;
                State = ParallelJobState.Executing;
            }
            if (IsTestMode)
            {
                int num = NumTestDelays;
                int delayMs = TestDelaySingleMs;
                if (num>0 && delayMs>0)
                {
                    for (int i = 0; i<num; ++i)
                    {
                        if (OutputLevel>=2)
                        {
                            if (num>1)
                            {
                                Console.WriteLine("Job No. " + Id + ", sleeping for " + ((double)delayMs/1.0e3) + "s, " 
                                    + (i+1) + "/" + num + "...");
                            } else
                            {
                                Console.WriteLine("Job No. " + Id + ", sleeping for " + ((double)delayMs/1.0e3) + "s...");
                            }
                        }
                        Thread.Sleep(delayMs);
                    }
                }
            }
            result = CalculateResultsPlain(input);
            lock(Lock)
            {
                Result = result;
                State = ParallelJobState.ResultsReady;
            }
            if (OutputLevel >= 1)
            {
                Console.WriteLine("... job (by container's method) finished, container No. " + this.Id + ".");
            }
        }


        /// <summary>Calculates and results of the job corresponding to the current job container, 
        /// based on the specified intput data.
        /// <para>Calculation is performed by setting input data and calling the <see cref="RunJob"/>() method.</para></summary>
        /// <param name="input">Input data for the job that is executed when calling this function.</param>
        /// <returns>Results of execution of the job.</returns>
        /// <remarks>
        /// <para>Warning: This method should only be called if the  flag is true. Otherwise, the job container itself does
        /// not know how to execute the job, an this must be done by the appropriate parallel job server object 
        /// (base type <see cref="ParallelJobServerBase"/>).</para>
        /// <para>This function locks the current job container object's lock. It is therefore thread safe, but
        /// may not be called in the context where locking the job container could cause delays or deadlocks.</para>
        /// </remarks>
        public ResultType RunJob(InputType input)
        {
            lock (Lock)
            {
                this.Input = input;
                State = ParallelJobState.DataReady;
                RunJob();
                return this.Result;
            }
        }

        #endregion JobExecution


        #region Static


        /// <summary>Creates and returns a parallel job dispatcher for job containers of the current typa, with
        /// the specified number of servers initialized and added on the dispatcher.</summary>
        /// <param name="numServers">Number of servers created and added to the created dispatcher.</param>
        public static ParallelJobDispatcherGen<InputType, ResultType> CreateDispatcher(int numServers)
        {
            return CreateDispatcher(numServers, -1);
        }

 
        /// <summary>Creates and returns a parallel job dispatcher for job containers of the current typa,  with
        /// the specified number of servers initialized and added on the dispatcher, and with specified 
        /// sleeping time (used for the dispetcher and added servers).</summary>
        /// <param name="numServers">Number of servers created and added to the created dispatcher.</param>
        /// <param name="sleepTimeMs">Sleeping time (in milliseconds) for dospatcher's server thread and for
        /// servers' threads.</param>
        public static ParallelJobDispatcherGen<InputType, ResultType> CreateDispatcher(int numServers, int sleepTimeMs)
        {
            ParallelJobDispatcherGen<InputType, ResultType> ret;
            ret = new ParallelJobDispatcherGen<InputType, ResultType>();
            if (sleepTimeMs > -1)
                ret.SleepTimeMs = sleepTimeMs;
            for (int i = 0; i < numServers; ++i)
            {
                ret.AddServer(CreateServer(sleepTimeMs));
            }
            return ret;
        }

        /// <summary>Creates and returns a new job server for the current job container type.</summary>
        public static ParallelJobServerGen<InputType, ResultType> CreateServer()
        {
            return CreateServer(-1);
        }

        /// <summary>Creates and returns a new job server for the current job container type.</summary>
        /// <param name="sleeptimeMs">Sleeping time for the server's serving thread, in milliseconds.</param>
        public static ParallelJobServerGen<InputType, ResultType> CreateServer(int sleeptimeMs)
        {
            ParallelJobServerGen<InputType, ResultType> ret;
            ret = new ParallelJobServerGen<InputType, ResultType>();
            if (sleeptimeMs > -1)
                ret.SleepTimeMs = sleeptimeMs;
            return ret;
        }

        /// <summary>Creates and returns a new job container of the current type.</summary>
        public static ParallelJobContainerGen<InputType, ResultType> CreateJobContainer()
        {
            return new ParallelJobContainerGen<InputType, ResultType>();
        }


        #endregion Static


        #region Tests

        
        /// <summary>Test of parallel job execution.</summary>
        /// <param name="inputs">Array of job inputs.</param>
        /// <param name="evaluationFunction">Delegate that is used to evaluate functions.</param>
        /// <param name="numServers">Number of servers to be used.</param>
        /// <param name="maxEnqueued">Maximal allowed number of jobs enquied.</param>
        /// <param name="delayTimeSeconds">Delay tie iin seconds.</param>
        /// <param name="delayTimeRelativeError"></param>
        /// <param name="sleepTimeMs">Sleep time of server threads, in milliseconds.</param>
        /// <param name="clientOutputLevel">Level of output on dispatching activity within the method.</param>
        /// <returns>Array of calculated results.</returns>
        public static ResultType[] TestPerformance(InputType[] inputs, SimpleFunctionDelegate<InputType, ResultType> evaluationFunction,
            int numServers, int maxEnqueued, double delayTimeSeconds, double delayTimeRelativeError, int sleepTimeMs,
            int clientOutputLevel)
        {
            int savedDefaultOutputLevel = DefaultOutputLevel;
            bool savedDefaultIsTestMode = DefaultIsTestMode;
            DefaultIsTestMode = true;
            if (DefaultOutputLevel < 0)
                DefaultOutputLevel = 4;
            List<ResultType> results = new List<ResultType>();
            try
            {
            if (inputs==null)
                throw new ArgumentException("Array of inputs is not specified (null array).");
            else if (inputs.Length<1)
                throw new ArgumentException("Array of inputs does not have any elements.");
            if (evaluationFunction == null)
                throw new ArgumentException("Evaluation function is not specified (null argument).");
            if (numServers<1)
                throw new ArgumentException("Number of job execution servers is less than 1.");
            Console.WriteLine();
            Console.WriteLine("Test of parallel job diapatcher.");
            Console.WriteLine("  number of inputs: " + inputs.Length);
            Console.WriteLine("  number of servers:" + numServers);
            Console.WriteLine("  max. enqueved jobs: " + maxEnqueued);
            Console.WriteLine("  average comp. time: " + delayTimeSeconds);
            Console.WriteLine("  relative error in comp. time: " + delayTimeRelativeError);
            Console.WriteLine("  server thread latency (sleeping time in ms): " + sleepTimeMs);
            Console.WriteLine();
            ParallelJobDispatcherGen<InputType, ResultType> dispatcher = CreateDispatcher(numServers, sleepTimeMs /* sleepTimeMs */);
            dispatcher.SetServersIsServer(true);
            Console.WriteLine();
            Console.WriteLine("Dispatcher and server data: " 
                + Environment.NewLine + dispatcher.ToString());
            Console.WriteLine();

            StopWatch1 watch = new StopWatch1();
            watch.Start();

            // Send jobs:
            for (int i = 0; i < inputs.Length; ++i)
            {
                ParallelJobContainerGen<InputType, ResultType> job = new ParallelJobContainerGen<InputType,ResultType>(evaluationFunction, inputs[i]);
                job.ClientJobId = i;
                // job.ClientData = results; // we do not need this, since callback delegate knows about the results array.
                // Code below will cause artificial delays, so that we can follow what's happening.
                job.IsTestMode = true;
                job.NumTestDelays = 1;
                job.TestDelayInSeconds = delayTimeSeconds;
                job.TestDelayRelativeError = delayTimeRelativeError;
                // Set callback for job aborted event, assign anonymous function:
                job.OnAbortedGeneric = delegate(ParallelJobContainerGen<InputType, ResultType> jobContaier)
                {
                    throw new InvalidOperationException(">>  ERROR: Job aborted. "
                        + Environment.NewLine + "  job ID: " + jobContaier.Id
                        + Environment.NewLine + "  job client ID: " + jobContaier.ClientJobId);
                };
                // Set callback for job finished event, assign anonymous function:
                job.OnFinishedGeneric = delegate(ParallelJobContainerGen<InputType, ResultType> jobContaier)
                {
                    // Find out which result is contained on the job container - through client index:
                    int which = jobContaier.ClientJobId;
                    if (clientOutputLevel>=2)
                        Console.WriteLine(">> Finished event triggered, client ID = " + jobContaier.ClientJobId);
                    // Resize result list, if necessary:
                    while (which >= results.Count)
                    {
                        results.Add(default(ResultType));
                    }
                    // Store the result:
                    results[which] = jobContaier.Result;
                };
                if (clientOutputLevel >= 2)
                    Console.WriteLine(Environment.NewLine 
                        + "  >> max. enqueued jobs: " + maxEnqueued + ", num. enqueued: " + dispatcher.NumEnqueuedJobs
                        + ", num. idle servers: " + dispatcher.NumIdleJobServers);
                while ((maxEnqueued>=1 && dispatcher.NumEnqueuedJobs >= maxEnqueued) || (maxEnqueued<1 && dispatcher.NumIdleJobServers<1))
                {
                    Thread.Sleep(10);
                }
                if (clientOutputLevel >= 2)
                    Console.WriteLine(""
                        + "    >> max. enqueued jobs: " + maxEnqueued + ", num. enqueued: " + dispatcher.NumEnqueuedJobs
                        + ", num. idle servers: " + dispatcher.NumIdleJobServers);
                int jobNumber = job.Id;
                if (clientOutputLevel >= 1)
                    Console.WriteLine(">> Sendinig job ID = {0} ... ", jobNumber);
                dispatcher.SendJob(job);
                if (clientOutputLevel >= 3)
                {
                    Console.WriteLine("      >> ...job ID = {0} sent. ", jobNumber);
                    Console.WriteLine(">> jobs dispatched: "
                        + Environment.NewLine + "    total sent:    " + dispatcher.NumSentJobs
                        + Environment.NewLine + "    total started: " + dispatcher.NumStartedJobs
                        + Environment.NewLine + "    finished:      " + dispatcher.NumFinishedJobs
                        + Environment.NewLine + "    aborted:       " + dispatcher.NumAbortedJobs
                        + Environment.NewLine + "    uncompleted:   " + dispatcher.NumUncompletedJobs
                        + Environment.NewLine + "    enqueued:      " + dispatcher.NumEnqueuedJobs
                        + Environment.NewLine + "    executing:     " + dispatcher.NumExecutingJobs);
                }
            }
            // Wait for execution of all jobs...
            if (clientOutputLevel >= 1)
                Console.WriteLine(Environment.NewLine + ">> Waiting job completion..." + Environment.NewLine);
            bool stopWaiting = (false);
            double waitTimeoutSeconds = 2.0;
            do
            {
                stopWaiting = (dispatcher.NumUncompletedJobs <= 0);
                if (clientOutputLevel >= 2)
                    Console.WriteLine(">> stopWaiting = " + stopWaiting + "; jobs dispatched: "
                        + Environment.NewLine + "    total sent:    " + dispatcher.NumSentJobs
                        + Environment.NewLine + "    total started: " + dispatcher.NumStartedJobs
                        + Environment.NewLine + "    finished:      " + dispatcher.NumFinishedJobs
                        + Environment.NewLine + "    aborted:       " + dispatcher.NumAbortedJobs
                        + Environment.NewLine + "    uncompleted:   " + dispatcher.NumUncompletedJobs
                        + Environment.NewLine + "    enqueued:      " + dispatcher.NumEnqueuedJobs
                        + Environment.NewLine + "    executing:     " + dispatcher.NumExecutingJobs);
                if (!stopWaiting)
                {
                    if (clientOutputLevel >= 1)
                        Console.WriteLine(Environment.NewLine + ">> Waiting completion of all jobs, timeout: " + waitTimeoutSeconds + " s");
                    bool isFinished = dispatcher.WaitAllJobsCompleted(waitTimeoutSeconds);
                    if (!isFinished)
                    {
                        if (clientOutputLevel >= 2)
                        {
                            Console.WriteLine(Environment.NewLine + ">> Not all jobs completed jet.");
                            if (dispatcher.NumUncompletedJobs > 0)
                                Console.WriteLine("  >> number of uncompleted jobs: " + dispatcher.NumUncompletedJobs);
                        }
                    }
                }
            } while (!stopWaiting);
            // IMPORTANT: Stop the dispatcher's server when all jobs done
            dispatcher.StopServerWhenAllJobsDone();
            if (clientOutputLevel >= 1)
                Console.WriteLine(Environment.NewLine + ">> All jobs scheduled by dispatcher are completed.");

            Console.WriteLine();
            Console.WriteLine("==========");
            Console.WriteLine("Test of parallel dispatcher finished in " + watch.Time + " s.");
            Console.WriteLine("  number of inputs: " + inputs.Length);
            Console.WriteLine("  number of servers:" + numServers);
            Console.WriteLine("  max. enqueved jobs: " + maxEnqueued);
            Console.WriteLine("  average comp. time: " + delayTimeSeconds);
            Console.WriteLine("  relative error in comp. time: " + delayTimeRelativeError);
            Console.WriteLine("  server thread latency (sleeping time in ms): " + sleepTimeMs);
            Console.WriteLine("==========");
            Console.WriteLine();
            }
            finally
            {
                DefaultOutputLevel = savedDefaultOutputLevel;
                DefaultIsTestMode = savedDefaultIsTestMode;
            }
            return results.ToArray();
        }



        #endregion Tests

        
        #region Misc
        
        /// <summary>Returns a string representation of the current job dispatcher, which contains relevent data
        /// about the server state.</summary>
        public override string ToString()
        {
            lock (Lock)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(base.ToString());
                sb.AppendLine("  generic container's event handlers: ");
                sb.AppendLine("    on started gen.: " + OnStartedGeneric == null ? "not assigned" : "assigned");
                sb.AppendLine("    on started gen.: " + OnAbortedGeneric == null ? "not assigned" : "assigned");
                sb.AppendLine("    on started gen.: " + OnFinishedGeneric == null ? "not assigned" : "assigned");
                sb.AppendLine("  input data: " + Input == null ? "not specified" : "specified" );
                sb.AppendLine("  results:    " + Result == null ? "not specified" : "specified" );
                return sb.ToString();
            }
        }

        #endregion Misc


    }  // ParallelJobContainerGen



    /// <summary>Parallel job server for job containers that inherit from <see cref="ParallelJobContainerGen"/>.</summary>
    /// <typeparam name="InputType">Type of inpout data for jobs.</typeparam>
    /// <typeparam name="ResultType">Type of output data for jobs.</typeparam>
    /// <typeparam name="JobContainerType">Type of the job data container used by this parallel server class.
    /// It must be be of type <see cref="ParallelJobContainer<InputType, ResultType>"/>, or must derive form this type.</typeparam>
    public class ParallelJobServerGen<InputType, ResultType, JobContainerType> : ParallelJobServerBase<JobContainerType>,
        ILockable, IIdentifiable
        where JobContainerType : ParallelJobContainerGen<InputType, ResultType>
    {
        public ParallelJobServerGen()
            : base()
        { }

        /// <summary>Runs the job whose data is contained in the specified job data container.</summary>
        /// <param name="jobData">Data container for the job to be run.</param>
        /// <remarks>This method runs the job by runniing the <see cref="ParallelJobContainer<InputType, ResultType>.RunJob"/>
        /// method defined on the job container (argument <paramref name="jobData"/>).</remarks>
        protected override void RunJobDefined(JobContainerType jobData)
        {
            if (jobData == null)
                throw new ArgumentException("Job data is not specified (null argument)."
                    + Environment.NewLine + "  Parallel job server ID: " + this.Id);
            if (!jobData.IsJobDefined)
                throw new ArgumentException("The job container does not contained definition of how job is performed."
                    + Environment.NewLine + "  Server ID: " + this.Id
                    + Environment.NewLine + "  Job ID: " + jobData.Id);
            jobData.RunJob();
        }
    
    }  // class ParallelJobServerGen<InputType, ResultType, JobContainerType>


    /// <summary>Parallel job server for job containers that inherit from <see cref="ParallelJobContainerGen"/>.</summary>
    /// <typeparam name="InputType">Type of inpout data for jobs.</typeparam>
    /// <typeparam name="ResultType">Type of output data for jobs.</typeparam>
    public class ParallelJobServerGen<InputType, ResultType> :
        ParallelJobServerGen<InputType, ResultType, ParallelJobContainerGen<InputType, ResultType>>,
        ILockable, IIdentifiable
    {

        public ParallelJobServerGen()
            : base()
        { }

    } // class ParallelJobServerGen<InputType, ResultType>



    /// <summary>Parallel job dispatcher for job containers that inherit from <see cref="ParallelJobContainerGen"/>.</summary>
    /// <typeparam name="InputType">Type of inpout data for jobs.</typeparam>
    /// <typeparam name="ResultType">Type of output data for jobs.</typeparam>
    /// <typeparam name="JobContainerType">Type of the job data container used by this parallel server class.
    /// It must be be of type <see cref="ParallelJobContainer<InputType, ResultType>"/>, or must derive form this type.</typeparam>
    public class ParallelJobDispatcherGen<InputType, ResultType, JobContainerType> : ParallelJobDispatcherBase<JobContainerType>,
        ILockable, IIdentifiable
        where JobContainerType : ParallelJobContainerGen<InputType, ResultType>
    {
        public ParallelJobDispatcherGen()
            : base()
        { }

    }  // class ParallelJobDispatcherGen<InputType, ResultType, JobContainerType>


    /// <summary>Parallel job jerver for job containers that inherit from <see cref="ParallelJobContainerGen"/>.</summary>
    /// <typeparam name="InputType">Type of inpout data for jobs.</typeparam>
    /// <typeparam name="ResultType">Type of output data for jobs.</typeparam>
    public class ParallelJobDispatcherGen<InputType, ResultType> :
        ParallelJobDispatcherGen<InputType, ResultType, ParallelJobContainerGen<InputType, ResultType>>,
        ILockable, IIdentifiable
    {

        public ParallelJobDispatcherGen()
            : base()
        { }

    } // class ParallelJobDispatcherGen<InputType, ResultType>



}