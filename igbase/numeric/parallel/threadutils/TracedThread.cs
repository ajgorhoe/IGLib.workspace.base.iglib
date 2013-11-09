using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Threading;


namespace IG.Lib
{

    /// <summary>Tracked thread object that wraps a single created thread and enables that all 
    /// active wrapped threads are are tracked (i.e. a list of all active tracked threads
    /// can be obtained at any time).
    /// <para>Starting and manipulatin of the wrapped thread is tone through the <see cref="TrackedThread.Thread"/> property.</para>
    /// <para>The <see cref="TrackedThread.IsParameterizedStart"/> property can be used if the wrapped thread was
    /// created with the parameterized thread start delegate.</para>
    /// <para>The static <see cref="TrackedThread.Threads"/> property returns an array of all <see cref="TrackedThread"/> 
    /// objects that have ever been created and wrap active threads (threads that complete are removed from the list).</para></summary>
    /// $A Igor xx;
    [Obsolete("This class is useful but is currently not used anywhere but shoud be kept in the code. Remove this attribute when use of the class is made.")]
    public class TrackedThread
    {

        #region Construction

        /// <summary>Creates a new tracked thread with the specified parameterized thread start delegate.</summary>
        /// <param name="start">The <see cref="ParameterizedThreadStart"/> delegate that is called on thread start.</param>
        public TrackedThread(ParameterizedThreadStart start)
        {
            this._startParameterized = start;
            this._thread = new Thread(this.StartThreadParameterized);
            lock (_lockThreadList)
            {
                _threadList.Add(this);
            }
        }

        /// <summary>Creates a new tracked thread with the specified thread start delegate.</summary>
        /// <param name="start">The <see cref="ThreadStart"/> delegate that is called on thread start.</param>
        public TrackedThread(ThreadStart start)
        {
            this._start = start;
            this._thread = new Thread(this.StartThread);
            lock (_lockThreadList)
            {
                _threadList.Add(this);
            }
        }

        /// <summary>Creates a new tracked thread with the specified parameterized thread start delegate
        /// and the specified maximal stack size.</summary>
        /// <param name="start">The <see cref="ParameterizedThreadStart"/> delegate that is called on thread start.</param>
        /// <param name="maxStackSize">Maximal stack size of the thread.</param>
        public TrackedThread(ParameterizedThreadStart start, int maxStackSize)
        {
            this._startParameterized = start;
            this._thread = new Thread(this.StartThreadParameterized, maxStackSize);
            lock (_lockThreadList)
            {
                _threadList.Add(this);
            }
        }

        /// <summary>Creates a new tracked thread with the specified thread start delegate
        /// and the specified maximal stack size.</summary>
        /// <param name="start">The <see cref="ThreadStart"/> delegate that is called on thread start.</param>
        /// <param name="maxStackSize">Maximal stack size of the thread.</param>
        public TrackedThread(ThreadStart start, int maxStackSize)
        {
            this._start = start;
            this._thread = new Thread(this.StartThread, maxStackSize);
            lock (_lockThreadList)
            {
                _threadList.Add(this);
            }
        }

        #endregion Construction


        #region Data

        protected readonly Thread _thread;

        /// <summary>Gets the thread that is uses in this object.</summary>
        public Thread Thread
        {
            get
            {
                return this._thread;
            }
        }

        /// <summary>Parameterized start delegate, executed on the thread when it starts.</summary>
        protected readonly ParameterizedThreadStart _startParameterized;

        /// <summary>Start delegate (nonparameterized), executed on the thread when it starts.</summary>
        protected readonly ThreadStart _start;

        /// <summary>Gets the flag indicating whether the current traced thread uses 
        /// a parameterized thread start.</summary>
        public bool IsParameterizedStart
        {
            get
            {
                return (_startParameterized != null);
            }
        }

        /// <summary>The parameterized start method of the current tracked thread.
        /// Calls the delegate that has been passed at creation.</summary>
        /// <param name="obj">Object that is passed as argument to the delegate that is called on thread start.</param>
        protected void StartThreadParameterized(object obj)
        {
            try
            {
                this._startParameterized(obj);
            }
            catch(Exception)
            {
                // If the exception was thrown, rethrow:
                throw;
            }
            finally
            {
                lock (_lockThreadList)
                {
                    // After completion, remove the thread from the list of traced threads:
                    _threadList.Remove(this);
                }
            }
        }

        /// <summary>The parameterized start method of the current tracked thread.
        /// Calls the delegate that has been passed at creation.</summary>
        protected void StartThread()
        {
            try
            {
                this._start();
            }
            catch(Exception)
            {
                // If the exception was thrown, rethrow:
                throw;
            }
            finally
            {
                lock (_lockThreadList)
                {
                    // After completion, remove the thread from the list of traced threads:
                    _threadList.Remove(this);
                }
            }
        }

        /// <summary>Object for locking the list of active traced threads.</summary>
        protected static readonly object _lockThreadList = new object();

        /// <summary>The list of active traced threads.</summary>
        protected static readonly List<TrackedThread> _threadList = new List<TrackedThread>();

        /// <summary>Returns the array of all active tracked threads of type <see cref="TrackedThread"/>.
        /// <para>Use the <see cref="TrackedThread.Thread"/> property on the elements of the returned array
        /// in order to manipulate individual tracked threads.</para></summary>
        public static TrackedThread[] Threads
        {
            get 
            {
                lock (_lockThreadList)
                {
                    return _threadList.ToArray();
                }
            }
        }


        ///// <summary>Gets the collection of all active traced threads.</summary>
        //public static IEnumerable<TrackedThread> ThreadList
        //{
        //    get
        //    {
        //        lock (_lockThreadList)
        //        {
        //            return new ReadOnlyCollection<TrackedThread>(_threadList);
        //        }
        //    }
        //}

        /// <summary>Gets the current number of active traced threads.</summary>
        public static int Count
        {
            get
            {
                lock (_lockThreadList)
                {
                    return _threadList.Count;
                }
            }
        }


        #endregion Data


        #region Tests


        /// <summary>Tests and demonstrates the <see cref="TrackedThread"/> class.</summary>
        public static void Test()
        {
            TrackedThread thread1 = new TrackedThread(DoNothingForFiveSeconds);
            TrackedThread thread2 = new TrackedThread(DoNothingForTenSeconds);
            TrackedThread thread3 = new TrackedThread(DoNothingForSomeTime);

            thread1.Thread.Start();
            thread2.Thread.Start();
            thread3.Thread.Start(15);
            while (TrackedThread.Count > 0)
            {
                Console.WriteLine(TrackedThread.Count);
            }

            Console.ReadLine();
        }

        private static void DoNothingForFiveSeconds()
        {
            Thread.Sleep(5000);
        }

        private static void DoNothingForTenSeconds()
        {
            Thread.Sleep(10000);
        }

        private static void DoNothingForSomeTime(object seconds)
        {
            Thread.Sleep(1000 * (int)seconds);
        }

        #endregion Tests

    }  // class TrackedThread

}




