// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


public delegate bool ConditionDelegateBase();


namespace IG.Lib
{
    
    /// <summary>Interface for classes that implement blocking until a specified condition is met.</summary>
    /// $A Igor Jun10;
    public interface IWaitCondition : ILockable
    {
        /// <summary>Returns true if unblocking condition is satisfied, and false otherwise.</summary>
        bool Condition();

        /// <summary>Returns true if waiting for unblocking condition is currently performed,
        /// and false otherwise.
        /// Setting should only be done within the waiting function.</summary>
        bool IsWaiting { get; }

        /// <summary>Cancels the current waiting for the condition (if one is going on) on one thread
        /// and unblocks the thread on which waiting was called (possibly with some latency).</summary>
        void CancelOne();

        /// <summary>Cancel the current waiting for the condition on all threads.</summary>
        void CancelAll();

        /// <summary>Blocks until the specified condition gets satisfied. See class description for details.</summary>
        /// <remarks>This method will normally not be overridden, except with intention to change 
        /// the condition check time plan. When overriding, use the original method as template.</remarks>
        void Wait();
    }


    /// <summary>Base class for objects that perform waiting until a condition is fulfilled.</summary>
    public abstract class WaitConditionBase : IWaitCondition, ILockable
    {

        #region ThreadLocking

        private object mainlock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return mainlock; } }

        private object internallock = new object();

        /// <summary>Used internally for locking access to internal fields.</summary>
        protected object InternalLock { get { return internallock; } }

        private object waitlock = new object();

        /// <summary>Must be used only for locking waiting the Waiting() block (since it is potentially time consuming).</summary>
        protected object WaitLock { get { return waitlock; } }

        #endregion ThreadLocking

        /// <summary>Returns true if unblocking condition is satisfied, and false otherwise.</summary>
        public abstract bool Condition();


        private int _numWaiting = 0;

        /// <summary>True if waiting for unblocking condition is currently performed,
        /// and false otherwise.
        /// Setting should only be done within the waiting function.</summary>
        public virtual bool IsWaiting
        {
            get { return _numWaiting < 1; }
            protected set
            {
                if (value == true)
                    ++_numWaiting;
                else
                    --_numWaiting;
            }
        }

        private bool _canceFlag = false;

        /// <summary>If this flag is set then the current waiting (if one is going on) will be cancelled.</summary>
        protected virtual bool CancelFlag  // ineernal enables use as proxy class, such as in WaitFileEventBase0
        {
            get { lock (InternalLock) { return _canceFlag; } }
            set { lock (InternalLock) { _canceFlag = value; } }
        }

        /// <summary>Cancels the current waiting for the condition (if one is going on)
        /// and unblocks the thread on which waiting was called (possibly with some latency).</summary>
        public virtual void CancelOne()
        {
            CancelFlag = true;
        }

        /// <summary>Cancel the current waiting for the condition on all threads.</summary>
        public virtual void CancelAll()
        {
            while (IsWaiting)
            {
                CancelOne();
                Thread.Sleep(1);
            }
        }

        /// <summary>Blocks until the specified condition gets satisfied. See class description for details.</summary>
        /// <remarks>This method will normally not be overridden, except with intention to change 
        /// the condition check time plan. When overriding, use the original method as template.</remarks>
        public abstract void Wait();
    }  // abstract class WaitConditionBase

    /// <summary>Provides a framework for blocking execution until the specified condition becomes
    /// satisfied. Function Wait() does that. The function continuously check the 
    /// unblocking condition until it becomes satisfied, sleeping a certain amount of time between
    /// consecutive checks. Time plan of checks (i.e. the amount of sleepin time between them) can be 
    /// adjusted by public properties SleepFirst, MinSleepMs, MaxSleepMs, and MaxRelativeLatency.
    /// These properties can be adjusted while waiting.
    ///   Blocking condition is evaluated by the (public) function Condition() and can be adjusted 
    /// in one of the following ways:
    ///   - by setting the delegate ConditionDelegate.
    ///   - by overriding the protected ConditionFunction() in a derived class and setting ConditionDelegate to null.
    ///   - by overriding the Condition() function itself.
    /// </summary>
    /// $A Igor Jun10;
    public class WaitCondition : WaitConditionBase, IWaitCondition, ILockable
    {

        #region Construction

        /// <summary>Creates event waiter with properties initialized to default values.</summary>
        public WaitCondition()
        {
            InitWaitCondition();
        }

        /// <summary>Creates event waiter with properties initialized to specified values
        /// (or to default valuse for those parameters that are not specified).</summary>
        public WaitCondition(int minSleepMs, double maxRelativeLatency)
        {
            InitWaitCondition(minSleepMs, MaxSleepMs);
        }

        /// <summary>Creates event waiter with properties initialized to specified values
        /// (or to default valuse for those parameters that are not specified).</summary>
        public WaitCondition(int minSleepMs, int maxSleepMs,
                double maxRelativeLatency, bool sleepFirst)
        {
            InitWaitCondition(minSleepMs, maxSleepMs,
                maxRelativeLatency, sleepFirst);
        }



        /// <summary>Initializes object properties with default values.</summary>
        protected virtual void InitWaitCondition()
        {
                this.MinSleepMs = 1;
                this.MaxSleepMs = -1;
                this.MaxRelativeLatency = 0.05;
                this.SleepFirst = false;
        }

        /// <summary>Initializes blocking parameters to the specified values
        /// (or to default values for those parameters that are not specified).</summary>
        protected virtual void InitWaitCondition(int minSleepMs, double maxRelativeLatency)
        {
            // Set properties to default values, then assign the specified properties according to parameters:
            InitWaitCondition();  
            this.MinSleepMs = minSleepMs;
            this.MaxRelativeLatency = MaxRelativeLatency;
            
        }

        /// <summary>Initializes blocking parameters.</summary>
        protected virtual void InitWaitCondition(int minSleepMs, int maxSleepMs, 
                double maxRelativeLatency, bool sleepFirst)
        {
            // Set properties to default values, then assign the specified properties according to parameters:
            InitWaitCondition();  
            this.MinSleepMs = minSleepMs;
            this.MaxSleepMs = maxSleepMs;
            this.MaxRelativeLatency = MaxRelativeLatency;
            this.SleepFirst = sleepFirst;
        }

        #endregion Constructon



        #region UnblockingCondition

        /// <summary>Function that returns true if unblocking condition is satisfied, and false otherwise.
        /// If the condition delegate is set then the delegate is used to evaluate the condition,
        /// otherwise the protected method ConditionFunction() is used.
        /// The condition can therefore be adjusted in one of the following ways:
        ///   - by setting the ConditionDelegate field
        ///   - by overriding the ConditionFunction and leaving ConditionDelegate null.</summary>
        public override bool Condition()
        {
            lock (InternalLock)
            {
                if (ConditionDelegate != null)
                    return ConditionDelegate();
                else
                    return ConditionFunction();
            }
        }

        protected ConditionDelegateBase _conditionDelegate = null;

        /// <summary>Contains function that is called to evaluate the unblocking condition.
        /// If this delegate is set and Condition() is not overridden then the delegate is used
        /// to check whether the unblocking condition is satisfied.</summary>
        public virtual ConditionDelegateBase ConditionDelegate
        {
            protected get { return _conditionDelegate; }
            set { lock (InternalLock) { _conditionDelegate = value; } }
        }

        /// <summary>Evaluates blocking condition in the case that the condition delegate is not specified.</summary>
        /// <returns>True if the unblocking condition is satisfied (such that the thread will continue 
        /// with execution) after the condition is being evaluated.</returns>
        protected virtual bool ConditionFunction()
        {
            throw new NotImplementedException("Base class' condition function: Unblocking condition can not be evaluated. "
                + Environment.NewLine + "Neither condition delegate is specified nor condition function is overridden.");
        }

        #endregion UnblockingCondition


        #region TimePlan

        protected int 
            _minSleepMs = 1,
            _maxSleepMs = -1;

        double
            _maxRelativeLatency = 0.05;

        bool _sleepFirst = false;

        
        /// <summary>Minimal sleeping time, in milliseconds, between successive condition checks.
        /// If less than 0 then minimal sleeping time is not specified, so there may be no sleeping.</summary>
        public virtual int MinSleepMs { 
            get { lock (InternalLock) { return _minSleepMs; } }
            set { lock (InternalLock) { _minSleepMs = value; } } 
        }

        /// <summary>Maximal sleeping time, in milliseconds, between successive condition checks.
        /// If less than 0 then maximal sleeping time is not specified and sleeping interval is not bounded above.
        /// If set to 0 then no sleeping will be performed between successive checks (max. sleeping
        /// time overrides the minimal sleeping time).</summary>
        public virtual int MaxSleepMs 
        { 
            get { lock (InternalLock) { return _maxSleepMs; } }
            set { lock (InternalLock) { _maxSleepMs = value; } } 
        }

        /// <summary>Maximal relative latency of waiting procedure.
        /// Sleeping time chosen between two successive condition check willl be chosen smaller or equal
        /// to total elapsed waiting time multiplied by this number.
        /// If less than 0 then maximal relative latency is not specified. 
        /// If minimal sleeping time is specified then it overrides the sleeping time calculated according to this parameter.</summary>
        public virtual double MaxRelativeLatency { 
            get { lock (InternalLock) { return _maxRelativeLatency; } }
            set { lock (InternalLock) { _maxRelativeLatency = value; } } 
        }

        /// <summary>If true and if minimal sleeping time is larger than 0, then sleep for minimal sleeping
        /// time will be performed before the first check for unblocking condition.</summary>
        public virtual bool SleepFirst 
        {
            get { lock (InternalLock) { return _sleepFirst; } }
            set { lock (InternalLock) { _sleepFirst = value; } }
        }

        #endregion TimePlan


        #region Blocking

        protected StopWatch1 _timer = null;

        /// <summary>Timer that measures the total time elapsed when waiting for fulfillment of unblocking condition.
        /// Used to evaluate appropriate sleeping times that will not cause too much latency.</summary>
        protected StopWatch1 Timer
        {
            get {
                if (_timer==null)
                    _timer = new StopWatch1();
                return _timer;
            }
        }


        /// <summary>Blocks until the specified condition gets satisfied. See class description for details.</summary>
        /// <remarks>This method will normally not be overridden, except with intention to change 
        /// the condition check time plan. When overriding, use the original method as template.</remarks>
        public override void Wait()
        {
            bool stop = false;
            double totaltime = 0.0;
            int sleeptime = 0;
            bool doSleep = true;
            try
            {
                lock (InternalLock)
                {
                    IsWaiting = true;
                    CancelFlag = false;
                    Timer.Reset();
                    Timer.Start();
                    doSleep = (SleepFirst && MinSleepMs >= 0);
                }
                if (doSleep)
                    Thread.Sleep(MinSleepMs);
                while (!stop)
                {
                    if (Condition() || CancelFlag)
                    {
                        lock (InternalLock)
                        {
                            stop = true;
                            if (CancelFlag)
                                CancelFlag = false;
                        }
                    }
                    else
                    {
                        lock (InternalLock)
                        {
                            // Calculate the appropriate sleeping time and sleep before the next evaluation of 
                            // unblocking condition:
                            sleeptime = 0;
                            if (MaxRelativeLatency > 0.0)
                            {
                                totaltime = Timer.Time; // total waiting time up to now, in seconds
                                sleeptime = (int)Math.Floor(1000 * totaltime * MaxRelativeLatency);
                            }
                            if (MinSleepMs >= 0 && sleeptime < MinSleepMs)
                                sleeptime = MinSleepMs;
                            if (MaxSleepMs >= 0 && sleeptime > MaxSleepMs)
                                sleeptime = MaxSleepMs;
                        }
                        if (sleeptime > 0)
                            Thread.Sleep(sleeptime);
                    }
                }  // lock InternalLock
            }
            catch
            {
                throw;
            }
            finally
            {
                IsWaiting = false;
            }

        }  // Wait()

        #endregion Blocking

        #region misc

        public override string ToString()
        {
            string ret = "";
            ret += "Event Waiter: ";
            ret+= Environment.NewLine;
            if (IsWaiting)
                ret += "Currently waiting for condition to become fulfilled.";
            else
                ret += "Currently idle.";
            ret += Environment.NewLine;
            ret += "Properties: ";
            ret += Environment.NewLine;
            ret+="MinSleepMs: " + MinSleepMs;
            ret+=Environment.NewLine;
            ret+="MaxSleepMs: " + MaxSleepMs;
            ret+=Environment.NewLine;
            ret+="MaxRelativeLatency: " + MaxRelativeLatency;
            ret+=Environment.NewLine;
            ret+="SleepFirst: " + SleepFirst;
            ret+=Environment.NewLine;
            return ret;
        }

        #endregion misc


    }  // class WaitCondition

}