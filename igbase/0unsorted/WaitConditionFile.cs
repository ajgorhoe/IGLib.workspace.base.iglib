// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

using SPath = System.IO.Path;

namespace IG.Lib
{


    /// <summary>Interface for classes that provide waiting for specific file events 
    /// (such as ceration or deletion of a specific file or directory).</summary>
    /// $A Igor Jun10;
    public interface IWaitFileEvent : IWaitCondition, ILockable
    {
        /// <summary>Path of the file or directory on which the particular event is waited for.
        /// Can be specified as relative path, but is internally stored as fully qualified path.</summary>
        string Path { get; set; }
    }


    #region WaitFileSystemEvents 


    /// <summary>Launches (immediately upon creation) a thread that performs pulsing on its object.
    /// PulseAll is perfomed on its Lock object every time the object is pulsed from another thread.
    /// This object acts as kind of proxy for pulsing and provides the object used for locking and pulsing.</summary>
    public class ThreadPulser
    {
        void PulseThread()
        {
            ThreadObject.Start();
        }


        #region TestTracking

        private bool _printNotes = false;

        /// <summary>If true then various events and actions will be notified by console output
        /// (for testing purposes only!).</summary>
        public bool PrintNotes { get { return _printNotes; } set { _printNotes = value; } }

        #endregion TestTracking


        private Thread _thread;

        public Thread ThreadObject
        {
            get
            {
                if (_thread == null)
                    _thread = new Thread(ThreadMethod);
                return _thread;
            }
            protected set { _thread = value; }
        }

        //private object _pulsingLock = new object();

        ///// <summary>Object that is pulsed by the thread of this object.</summary>
        //public object PulsingLock { get { return _pulsingLock; } }

        private object _triggerLock = new object();

        /// <summary>Object used to trigger pulsing and for related locking.
        /// Whenever Monitor.Pulse() is performed on this object, the main thread of the current
        /// object will perform Monitor.Pulse(PulsingLock).</summary>
        public object TriggerLock { get { return _triggerLock; } }

        /// <summary>Performs evantual preparation of data before pulsing is performed.</summary>
        public virtual void PrepareForPulsing()
        {
        }

        /// <summary>Triggers pulsing performed by this object's thread.</summary>
        public void TriggerPulsing()
        {
            lock (TriggerLock)
            {
                if (PrintNotes)
                    Console.WriteLine("Just before pulsing is triggered...");
                Monitor.PulseAll(TriggerLock);
            }
        }

        protected virtual void ThreadMethod()
        {
            lock (TriggerLock)
            {
                while (true)
                {
                    Monitor.Wait(TriggerLock);
                    try
                    {
                        // TODO: consider whether locking is necessary at this stahe!
                        // It is performed here because PrepareForPulsing() might be a lenghty
                        // operation, so locking prevents losing triggering pulses.
                        lock (TriggerLock)
                        {
                            if (PrintNotes)
                                Console.WriteLine("Just before pulsing by the pulsing thread.");
                            PrepareForPulsing();
                            Monitor.PulseAll(TriggerLock);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (PrintNotes)
                            Console.WriteLine("Exception in pulsing thread: " + ex.ToString());
                    }
                }
            }
        }

    }  // class ThreadPulser




    /// <summary>Base class for classes that provide blocking until a file or directory is created/removed.</summary>
    /// $A Igor Jun10;
    /// TODO: implement non-latence vaiting (via file events)!
    public abstract class WaitFileEventBase : WaitConditionBase, IWaitFileEvent, ILockable
    {

        protected WaitFileEventBase(string fileOrDirectoryPath) 
        {
            this.Path = fileOrDirectoryPath;
            
            EventWatcher.EnableRaisingEvents = false;
            EventWatcher.NotifyFilter = NotifyFiltersAll;
            EventWatcher.Path = SPath.GetDirectoryName(this.Path);
            EventWatcher.IncludeSubdirectories = false;

            Pulser.PrintNotes = PrintNotes;

            InitWaitFileEventBase(fileOrDirectoryPath);
        }

        /// <summary>Initializes the object appropriately.
        /// Overrride this method in derived classes!</summary>
        /// <param name="fileOrDirectoryPath"></param>
        protected virtual void InitWaitFileEventBase(string fileOrDirectoryPath)
        {
            // Add event handlers:
            EventWatcher.Changed += new FileSystemEventHandler(OnChanged);
            EventWatcher.Created += new FileSystemEventHandler(OnCreated);
            EventWatcher.Deleted += new FileSystemEventHandler(OnDeleted);
            EventWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            EventWatcher.Error += new ErrorEventHandler(OnError);
        }


        #region TestTracking

        private bool _printNotes = false;

        /// <summary>If true then various events and actions will be notified by console output
        /// (for testing purposes only!).</summary>
        public bool PrintNotes { 
            get { return _printNotes; }
            set { _printNotes = value; Pulser.PrintNotes = value; }
        }

        #endregion TestTracking



        #region EventHandlers

        /// <summary>NotifyFilter enumeration that allows all kinds of events to be fired.</summary>
        public const System.IO.NotifyFilters NotifyFiltersAll =
            NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
            NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
            NotifyFilters.Security | NotifyFilters.Size;

        FileSystemWatcher _watcher = new FileSystemWatcher();

        /// <summary>Component that responds to file system events.</summary>
        protected FileSystemWatcher EventWatcher
        {
            get { return _watcher; }
        }

        protected bool _eventOccured = false;

        /// <summary>This flag is set by event handlers, and can be used by waiting procedures to check if
        /// the current event has been triggered by the EventWatcher installed on the other object
        /// (since registration of events of all other EventWatchers is done on the same thread).</summary>
        public virtual bool EventOccured
        {
            get { lock (InternalLock) { return _eventOccured; }  }
            set { lock (InternalLock) { _eventOccured = value; }  }
        }


        // Event handlers for file system events: 

        protected string EventToString(FileSystemEventArgs e)
        {
            if (e == null)
                return "NULL FileSystemEventArgs" + Environment.NewLine;
            string ret = "  FileSystemEventArgs: " + Environment.NewLine
                + "Type: " + e.ChangeType + Environment.NewLine
                + "Name: " + e.Name + Environment.NewLine
                + "FullPath: " + e.FullPath + Environment.NewLine;
            return ret;
        }

        /// <summary>Handles events triggered when a file or directory is deleted.</summary>
        protected virtual void OnDeleted(object source, FileSystemEventArgs e)
        {
            if (PrintNotes)
            {
                Console.WriteLine();
                Console.WriteLine("File DELETED: " + e.FullPath + " " + e.ChangeType);
                if (e != null)
                    Console.Write(EventToString(e));
            }
            EventOccured = true;
            TriggerConditionCheck();
        }

        /// <summary>Handles events triggered when a file or directory is created.</summary>
        protected virtual void OnCreated(object source, FileSystemEventArgs e)
        {
            if (PrintNotes)
            {
                Console.WriteLine();
                Console.WriteLine("File CREATED: " + e.FullPath + " " + e.ChangeType);
                if (e != null)
                    Console.Write(EventToString(e));
            }
            EventOccured = true;
            TriggerConditionCheck();
        }

         /// <summary>Handles events triggered when a file or directory is changed.</summary>
       protected virtual void OnChanged(object source, FileSystemEventArgs e)
        {
            if (PrintNotes)
            {
                Console.WriteLine();
                Console.WriteLine("File CHANGED: " + e.FullPath + " " + e.ChangeType);
                if (e != null)
                    Console.Write(EventToString(e));
            }
            EventOccured = true;
            TriggerConditionCheck();
        }

         /// <summary>Handles events triggered when a file or directory is renamed.</summary>
        protected virtual void OnRenamed(object source, RenamedEventArgs e)
        {
            if (PrintNotes)
            {
                Console.WriteLine("File RENAMED: {0} renamed to {1}", e.OldFullPath, e.FullPath);
                if (e != null)
                    Console.Write(EventToString(e));
            }
            EventOccured = true;
            TriggerConditionCheck();
        }

         /// <summary>Handles events triggered when an error occurs.</summary>
        protected void OnError(object source, ErrorEventArgs e)
        {
            if (PrintNotes)
            {
                Console.WriteLine("File watcher: Error event occurred: " + e.ToString());
                Console.WriteLine("Press <Enter> in order to finish catching file events!");
            }
            EventOccured = true;
            TriggerConditionCheck();
        }

        #endregion EventHandlers


        #region PulseSignaling

        private static ThreadPulser _pulser = new ThreadPulser();

        /// <summary>Object that is used for pulsing.</summary>
        protected static ThreadPulser Pulser { get { return _pulser; } }

        /// <summary>Locking object that is used for waiting trigging pulses that cause to check contition.
        /// Comes from Pulser.</summary>
        protected object TriggerLock { get { return Pulser.TriggerLock; } }

        /// <summary>Triggers condition check by pulsing the object on which Wait() successively waits.</summary>
        public void TriggerConditionCheck()
        {
            Pulser.TriggerPulsing();
        }


        #endregion PulseSignaling

        /// <summary>File or directory name.</summary>
        protected string _fileOrDirectoryPath;

        /// <summary>Get or set path of the file on which the particular event is waited for.</summary>
        public virtual string Path
        {
            get { return _fileOrDirectoryPath; }
            set { 
                _fileOrDirectoryPath = SPath.GetFullPath(value);
                if (EventWatcher!=null)
                    EventWatcher.Path = SPath.GetDirectoryName(value);
            }
        }


        /// <summary>If not overridden, this condition always returns false.</summary>
        public override bool Condition()
        {
            return false;
        }

        /// <summary>Wrapper around Condition() that enables control output to console.</summary>
        protected bool ConditionInternal()
        {
            bool ret = Condition();
            if (PrintNotes) 
                Console.WriteLine("Condition checked, value: " + ret.ToString());
            return ret;
        }

        /// <summary>If true, events are raised by file system watched, otherwise events are not raised.
        /// This flag can not be set when in the stage of waiting. Attpmpt of doing so has no effect
        /// (also does not throw an exception).</summary>
        public bool EnableRaisingEvents 
        {
            get { lock(TriggerLock) { return EventWatcher.EnableRaisingEvents;  } }
            set { lock(TriggerLock) { if (!IsWaiting) EventWatcher.EnableRaisingEvents = value;  } }
        }


        /// <summary>Cancels the current waiting for the condition (if one is going on)
        /// and unblocks the thread on which waiting was called (possibly with some latency).</summary>
        public override void CancelOne()
        {
            lock (TriggerLock)
            {
                CancelFlag = true;
                TriggerConditionCheck();
            }
        }

        /// <summary>Cancel the current waiting for the condition on all threads.</summary>
        public override void CancelAll()
        {
            while (IsWaiting)
            {
                CancelOne();
            }
        }



        /// <summary>Blocks execution of the current thread until the first event is fired by
        /// the filesystem watcher.
        /// Enables raising events on the filesystem watcher, if not enabled.
        /// WARNING:
        /// This method is NOT thread safe.</summary>
        public void WaitFirstEvent()
        {
            WaitEvents(1);
        }

        /// <summary>Blocks execution of the current thread until the specified number of  file system 
        /// events are registered by the filesystem watcher. If the specified number of events is less
        /// than 1 then blicking is done ndefinitely (or until CancelWaiting() is called on the object
        /// from another thread).
        /// Enables raising events on the filesystem watcher, if not enabled.
        /// WARNING: 
        /// This method is NOT thread safe.</summary>
        public virtual void WaitEvents(int numEvents)
        {
            int caughtEvents = 0;
            lock (TriggerLock)
            {
                try
                {
                    IsWaiting = true;
                    CancelFlag = false;
                    if (PrintNotes)
                        Console.WriteLine("Blocking begins (waiting for " + numEvents + " matching file events).");
                    EnableRaisingEvents = true;
                    EventOccured = false;
                    while ( (numEvents > 0 && caughtEvents < numEvents) 
                            && !CancelFlag)
                    {
                        Monitor.Wait(TriggerLock);
                        lock (TriggerLock)
                        {
                            if (EventOccured)  // count events only if registered by this object's EventWatcher
                            {
                                ++caughtEvents;
                                EventOccured = false;  // reset the flag
                            }
                            if (PrintNotes)
                                Console.WriteLine("Pulse caught in the waiting loop.");
                        }
                    }
                    if (PrintNotes)
                        Console.WriteLine("Specified number of events occurred, unblocked.");
                }
                catch (Exception) { }
                finally
                {
                    IsWaiting = false; if (CancelFlag) CancelFlag = false;
                }
            }
        }  // void WaitEvents(numEvents)

        /// <summary>Waits for condition to be fulfilled.</summary>
        public override void Wait()
        {
            lock (TriggerLock)
            {
                try
                {
                    IsWaiting = true;
                    CancelFlag = false;
                    if (PrintNotes)
                        Console.WriteLine("Blocking begins (waiting for unblocking condition).");
                    EnableRaisingEvents = true;
                    while (!ConditionInternal() && !CancelFlag)
                    {
                        Monitor.Wait(TriggerLock);
                        lock (TriggerLock)
                        {
                            if (PrintNotes)
                                Console.WriteLine("Pulse catched in waiting loop.");
                        }
                    }
                    if (PrintNotes)
                        Console.WriteLine("Condition fulfilled, unblocked.");
                }
                catch (Exception) { }
                finally { 
                    IsWaiting = false; if (CancelFlag) CancelFlag = false; 
                }
            }
        } // void Wait()

        public override string ToString()
        {
            return "File or directory creation/removal waiting: " + Environment.NewLine  
                + "Currently waiting: " + IsWaiting + Environment.NewLine
                + "Cancel flag: " + CancelFlag + Environment.NewLine;
        }


        #region ExamplesAndTests


        // EXAMPLE OF WAITING FOR THE SPECIFIED NUMBER OF EVENTS:

        /// <summary>Waits (blocks execution) until a given number of the specified file system events are 
        /// registered. Basic information is printed for each event when it occurs.</summary>
        public static void ExampleWaitEvents(string fileOrDirectoryPath, int numEvents)
        {
            WaitFileEvent waiter = new WaitFileEvent(fileOrDirectoryPath);
            waiter.PrintNotes = true;

            FileSystemWatcher watcher = waiter.EventWatcher;
            watcher.Path = SPath.GetDirectoryName(waiter.Path);
            watcher.Filter = SPath.GetFileName(waiter.Path);
            watcher.NotifyFilter = NotifyFiltersAll;

            //// Add event handlers:
            //watcher.Changed += new FileSystemEventHandler(OnChangedEx);
            //watcher.Created += new FileSystemEventHandler(OnCreatedEx);
            //watcher.Deleted += new FileSystemEventHandler(OnDeletedEx);
            //watcher.Renamed += new RenamedEventHandler(OnRenamedEx);
            // watcher.Error += new ErrorEventHandler(OnErrorEx);

            // Begin watching:
            watcher.EnableRaisingEvents = true;

            Console.WriteLine();
            Console.WriteLine("Started processing file events.");
            Console.WriteLine("Observed path (intended): " + waiter.Path);
            Console.WriteLine("Watcher's path: " + watcher.Path);
            Console.WriteLine("Watcher's filter (file/dir): " + watcher.Filter);
            Console.WriteLine();
            //Console.WriteLine("Press <Enter> in order to finish catching file events!");
            //Console.WriteLine("Waiting for 5 seconds...");
            //Thread.Sleep(5000);
            //Console.ReadLine();

            Console.WriteLine("Waiting for first " + numEvents.ToString() + " matching file events...");
            Console.WriteLine();
            waiter.WaitEvents(numEvents);
            waiter.EnableRaisingEvents = false;
            Console.WriteLine();
            Console.WriteLine("Processing file events finished.");
            Console.WriteLine();
            watcher.EnableRaisingEvents = false;

        }


        // STATIC TEST FUNCTIONS:

        // Test of blocking until file is created or removed:

        /// <summary>Monitors the specified file and successively blocks until it is created and then 
        /// until it is removed. This procedure is repeated twice.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        public static void ExampleBlockCreateRemove(string filePath)
        {
            new Example().ExampleBlockCreateRemove(filePath);
        }

        /// <summary>Monitors the specified file and successively blocks until it is created and then 
        /// until it is removed.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
        public static void ExampleBlockCreateRemove(string filePath, int numSwitches)
        {
            new Example().ExampleBlockCreateRemove(filePath, numSwitches);
        }

        /// <summary>Monitors the specified file and successively blocks until it is created and then 
        /// until it is removed.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
        /// <param name="directory">If true then creation/removal of a directory is waiting.</param>
        public static void ExampleBlockCreateRemove(string filePath, int numSwitches, bool waitDirectory)
        {
            new Example().ExampleBlockCreateRemove(filePath, numSwitches, waitDirectory);
        }



        // SPEED TESTS (what event rates blocking functions can handle):

        /// <summary>Test of speed of reaction of file/directory creation and removal blocking waits.
        /// A specified number of alternate creations and removals are perfomed in a parallel
        /// thread, with specified delay between them. In the main thread, blocking waits are
        /// performed waiting for creation/removal in an infinite loop, and it is counted how
        /// many events are captured and how many are missed.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        public static void TestSpeedBlockCreateRemove(string filePath)
        {
            new Example().TestSpeedBlockCreateRemove(filePath);
        }


        /// <summary>Test of speed of reaction of file/directory creation and removal blocking waits.
        /// A specified number of alternate creations and removals are perfomed in a parallel
        /// thread, with specified delay between them. In the main thread, blocking waits are
        /// performed waiting for creation/removal in an infinite loop, and it is counted how
        /// many events are captured and how many are missed.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
        /// <param name="sleepMs">Number of milliseconds to sleep between examples.</param>
        public static void TestSpeedBlockCreateRemove(string filePath, int numSwitches, int sleepMs)
        {
            new Example().TestSpeedBlockCreateRemove(filePath, numSwitches, sleepMs);
        }


        /// <summary>Test of speed of reaction of file/directory creation and removal blocking waits.
        /// A specified number of alternate creations and removals are perfomed in a parallel
        /// thread, with specified delay between them. In the main thread, blocking waits are
        /// performed waiting for creation/removal in an infinite loop, and it is counted how
        /// many events are captured and how many are missed.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
        /// <param name="sleepMs">Number of milliseconds to sleep between examples.</param>
        /// <param name="directory">If true then creation/removal of a directory is waiting.</param>
        public static void TestSpeedBlockCreateRemove(string filePath, int numSwitches,
                int sleepMs, bool waitDirectory)
        {
            new Example().TestSpeedBlockCreateRemove(filePath, numSwitches,
                    sleepMs, waitDirectory);
        }


        /// <summary>Class containing examples for waiting creation or removal of files and directories.</summary>
        protected class Example : WaitFileEventLatenceBase.ExampleLatence
        {
            // file or directory creation/removal waiters used by examples:
            protected override IWaitFileEvent Creation
            {
                get
                {
                    if (_creation == null)
                    {
                        if (IsDirectory)
                            _creation = new WaitDirectoryCreationLatence(ExamplePath);
                        else
                            _creation = new WaitFileCreationLatence(ExamplePath);
                    }
                    return _creation;
                }
            }

            protected override IWaitFileEvent Removal
            {
                get
                {
                    if (_removal == null)
                    {
                        if (IsDirectory)
                            _removal = new WaitDirectoryRemovalByProxy(ExamplePath);
                        else
                            _removal = new WaitFileRemovalByProxy(ExamplePath);
                    }
                    return _removal;
                }
            }

        }

        #endregion ExamplesAndTests


    }  // class WaitFileEventBase



    /// <summary>Concrete class derived from WaitFileEventBase.
    /// It does not have a meaningful unblocking condition (it alwys evaluates to true),
    /// therefore the class can use WaitEvents() function but not Wait().</summary>
    public class WaitFileEvent: WaitFileEventBase
    {

        public WaitFileEvent(string fileOrDirectoryPath) : base(fileOrDirectoryPath)
        { }

    }  // class WaitFileEvent



    /// <summary>Blocking execution of the current thread until the specified file begins 
    /// to exist.</summary>
    public class WaitFileCreation : WaitFileEvent
    {

        public WaitFileCreation(string filePath)
            : base(filePath)
        { }

        /// <summary>Performs class specific initialization.</summary>
        protected override void InitWaitFileEventBase(string fileOrDirectoryPath)
        {
            EventWatcher.EnableRaisingEvents = false;
            EventWatcher.NotifyFilter = NotifyFiltersAll;
            EventWatcher.Path = SPath.GetDirectoryName(this.Path);
            EventWatcher.Filter = SPath.GetFileName(this.Path);
            EventWatcher.IncludeSubdirectories = false;
            // Add event handlers:
            EventWatcher.Created += new FileSystemEventHandler(OnCreated);
            EventWatcher.Error += new ErrorEventHandler(OnError);
        }

        /// <summary>Condition that unblocks Wait() when it becomes true.
        /// The condition is true if the observed file exists.</summary>
        /// <returns></returns>
        public override bool Condition()
        {
            return (File.Exists(this.Path));
        }

        /// <summary>Waits for condition to be fulfilled.</summary>
        public override void Wait()
        {
            if (PrintNotes)
            {
                Console.WriteLine();
                Console.WriteLine("Blocking until the following file begins to exist: ");
                Console.WriteLine("  " + Path);
                Console.WriteLine("Waiting...");
            }
            base.Wait();
        }

    } // class WaitFileCreation

    /// <summary>Blocking execution of the current thread until the specified file ceases 
    /// to exist.</summary>
    public class WaitFileRemoval : WaitFileEvent
    {

        public WaitFileRemoval(string filePath)
            : base(filePath)
        { }

        /// <summary>Performs class specific initialization.</summary>
        protected override void InitWaitFileEventBase(string fileOrDirectoryPath)
        {
            EventWatcher.EnableRaisingEvents = false;
            EventWatcher.NotifyFilter = NotifyFiltersAll;
            EventWatcher.Path = SPath.GetDirectoryName(this.Path);
            EventWatcher.Filter = SPath.GetFileName(this.Path);
            EventWatcher.IncludeSubdirectories = false;
            // Add event handlers:
            EventWatcher.Deleted += new FileSystemEventHandler(OnDeleted);
            EventWatcher.Error += new ErrorEventHandler(OnError);
        }

        /// <summary>Condition that unblocks Wait() when it becomes true.
        /// The condition is true if the observed file does not exist.</summary>
        /// <returns></returns>
        public override bool Condition()
        {
            return (! File.Exists(this.Path));
        }

        /// <summary>Waits for condition to be fulfilled.</summary>
        public override void Wait()
        {
            if (PrintNotes)
            {
                Console.WriteLine();
                Console.WriteLine("Blocking until the following file ceases to exist: ");
                Console.WriteLine("  " + Path);
                Console.WriteLine("Waiting...");
            }
            base.Wait();
        }

    } // class WaitFileRemoval

    /// <summary>Blocking execution of the current thread until the specified directory begins 
    /// to exist.</summary>
    public class WaitDirectoryCreation : WaitFileEvent
    {

        public WaitDirectoryCreation(string filePath)
            : base(filePath)
        { }

        /// <summary>Performs class specific initialization.</summary>
        protected override void InitWaitFileEventBase(string fileOrDirectoryPath)
        {
            EventWatcher.EnableRaisingEvents = false;
            EventWatcher.NotifyFilter = NotifyFiltersAll;
            EventWatcher.Path = SPath.GetDirectoryName(this.Path);
            EventWatcher.Filter = SPath.GetFileName(this.Path);
            EventWatcher.IncludeSubdirectories = false;
            // Add event handlers:
            EventWatcher.Created += new FileSystemEventHandler(OnCreated);
            EventWatcher.Error += new ErrorEventHandler(OnError);
        }

        /// <summary>Condition that unblocks Wait() when it becomes true.
        /// The condition is true if the specified directory exists.</summary>
        /// <returns></returns>
        public override bool Condition()
        {
            return (Directory.Exists(this.Path));
        }

        /// <summary>Waits for condition to be fulfilled.</summary>
        public override void Wait()
        {
            if (PrintNotes)
            {
                Console.WriteLine();
                Console.WriteLine("Blocking until the following directory begins to exist: ");
                Console.WriteLine("  " + Path);
                Console.WriteLine("Waiting...");
            }
            base.Wait();
        }

    } // class WaitDirectoryCreation

    /// <summary>Blocking execution of the current thread until the specified directory ceases 
    /// to exist.</summary>
    public class WaitDirectoryRemoval : WaitFileEvent
    {

        public WaitDirectoryRemoval(string filePath)
            : base(filePath)
        { }

        /// <summary>Performs class specific initialization.</summary>
        protected override void InitWaitFileEventBase(string fileOrDirectoryPath)
        {
            EventWatcher.EnableRaisingEvents = false;
            EventWatcher.NotifyFilter = NotifyFiltersAll;
            EventWatcher.Path = SPath.GetDirectoryName(this.Path);
            EventWatcher.Filter = SPath.GetFileName(this.Path);
            EventWatcher.IncludeSubdirectories = false;
            // Add event handlers:
            EventWatcher.Deleted += new FileSystemEventHandler(OnDeleted);
            EventWatcher.Error += new ErrorEventHandler(OnError);
        }

        /// <summary>Condition that unblocks Wait() when it becomes true.
        /// The condition is true if the specified directory does not exist.</summary>
        /// <returns></returns>
        public override bool Condition()
        {
            return (! Directory.Exists(this.Path));
        }

        /// <summary>Waits for condition to be fulfilled.</summary>
        public override void Wait()
        {
            if (PrintNotes)
            {
                Console.WriteLine();
                Console.WriteLine("Blocking until the following directory ceases to exist: ");
                Console.WriteLine("  " + Path);
                Console.WriteLine("Waiting...");
            }
            base.Wait();
        }

    } // class WaitDirectoryRemoval




    #endregion WaitFileSystemEvents




    #region WaitFilesystemEventsByProxy



    /// <summary>Base class for classes that provide blocking until a file or directory is created/removed.
    /// This clas uses a proxy class for performing its basic operation.</summary>
    /// $A Igor Jun10;
    /// TODO: implement non-latence vaiting (via file events)!
    public abstract class WaitFileEventBaseByProxyLatence : WaitConditionBase, IWaitFileEvent, ILockable
    {
        protected WaitFileEventBaseByProxyLatence() {  }

        /// <summary>Proxy object that actually performs operations for derived classes of this class.</summary>
        protected WaitFileEventLatenceBase _waiterLatence;
        // protected IWaitFileEvent _waiterLatence;


        /// <summary>Path of the file or directory on which the particular event is waited for.
        /// Can be specified as relative path, but is internally stored as fully qualified path.</summary>
        public virtual string Path { get { return _waiterLatence.Path; }  set { _waiterLatence.Path = value; } }

        /// <summary>Returns true if unblocking condition is satisfied, and false otherwise.</summary>
        public override bool Condition()
        {
            return _waiterLatence.Condition();
        }

        /// <summary>Returns true if waiting for unblocking condition is currently performed,
        /// and false otherwise.
        /// Setting should only be done within the waiting function.</summary>
        public override bool IsWaiting
        {
            get { { return _waiterLatence.IsWaiting; } }
        }

        /// <summary>If this flag is set then the current waiting (if one is going on) will be cancelled.</summary>
        protected override bool CancelFlag
        {
            get { lock (InternalLock) { return _waiterLatence.CancelFlagInternal; } }
            set { lock (InternalLock) { _waiterLatence.CancelFlagInternal = value; } }
        }

        /// <summary>Cancels the current waiting for the condition (if one is going on)
        /// and unblocks the thread on which waiting was called (possibly with some latency).</summary>
        public override void CancelOne()
        {
            _waiterLatence.CancelOne();
        }

        /// <summary>Blocks until the specified condition gets satisfied. See class description for details.</summary>
        /// <remarks>This method will normally not be overridden, except with intention to change 
        /// the condition check time plan. When overriding, use the original method as template.</remarks>
        public override void Wait()
        {
            _waiterLatence.Wait();
        }

        public override string ToString()
        {
            return "File or directory creation/removal waiting: " + Environment.NewLine + _waiterLatence.ToString();
        }

    }


    /// <summary>Base class for classes that provide blocking until a file or directory is created/removed.
    /// This clas uses a proxy class for performing its basic operation.</summary>
    /// $A Igor Jun10;
    /// TODO: implement non-latence vaiting (via file events)!
    public abstract class WaitFileEventBaseByProxy : WaitFileEventBaseByProxyLatence, IWaitFileEvent, ILockable
    {
        protected WaitFileEventBaseByProxy(): base() {  }
    }

    /// <summary>Implements blocking until the specified file is created (becomes existent).
    /// File is specified in constructor or by setting the FilePath property.
    /// Waiting is performed by calling the Wait() function.</summary>
    /// $A Igor Jun10;
    /// TODO: implement non-Latence vaiting (via file events)!
    public class WaitFileCreationByProxy : WaitFileEventBaseByProxy
    {

        public WaitFileCreationByProxy(string filePath)
        {
            _waiterLatence = new WaitFileCreationLatence(filePath);
        }

    }



    /// <summary>Implements blocking until the specified file is deleted (becomes nonexistent).
    /// File is specified in constructor or by setting the FilePath property.
    /// Waiting is performed by calling the Wait() function.</summary>
    /// $A Igor Jun10;
    /// TODO: implement non-Latence vaiting (via file events)!
    public class WaitFileRemovalByProxy : WaitFileEventBaseByProxy
    {
        public WaitFileRemovalByProxy(string filePath)
        {
            _waiterLatence = new WaitFileRemovalLatence(filePath);
        }
    }


    /// <summary>Implements blocking until the specified directory is created (becomes existent).
    /// Directory is specified in constructor or by setting the DirectoryPath property.
    /// Waiting is performed by calling the Wait() function.</summary>
    /// $A Igor Jun10;
    /// TODO: implement non-latence vaiting (via file events)!
    public class WaitDirectoryCreationByProxy : WaitFileEventBaseByProxy
    {

        public WaitDirectoryCreationByProxy(string directoryPath)
        {
            _waiterLatence = new WaitDirectoryCreationLatence(directoryPath);
        }

    }

    /// <summary>Implements blocking until the specified directory is deleted (becomes nonexistent).
    /// Directory is specified in constructor or by setting the DirectoryPath property.
    /// Waiting is performed by calling the Wait() function.</summary>
    /// $A Igor Jun10;
    /// TODO: implement non-latence vaiting (via file events)!
    public class WaitDirectoryRemovalByProxy : WaitFileEventBaseByProxy
    {
        public WaitDirectoryRemovalByProxy(string directoryPath)
        {
            _waiterLatence = new WaitDirectoryRemovalLatence(directoryPath);
        }
    }

    #endregion WaitFilesystemEventsByProxy


    #region LatenceWaiting



    /// <summary>Base class for classes that impelement methods that block until
    /// a file or directory is created or deleted.</summary>
    /// $A Igor Jun10;
    public abstract class WaitFileEventLatenceBase : WaitCondition, IWaitFileEvent, IWaitCondition
    {

        private WaitFileEventLatenceBase() { }

        protected WaitFileEventLatenceBase(string fileOrdirectoryPath) : base(1 /* minSleepMs */, 200 /* maxSleepMs */,
                0.01 /* maxRelativeLatency */, true /* sleepFirts */ )
        {
            this._fileOrDirectoryPath = fileOrdirectoryPath;
        }

        /// <summary>File or directory name.</summary>
        protected string _fileOrDirectoryPath;

        /// <summary>Get or set path of the file on which the particular event is waited for.</summary>
        public string Path
        {
            get { return _fileOrDirectoryPath; }
            set { _fileOrDirectoryPath = SPath.GetFullPath(value); }
        }

        /// <summary>This method should be overridden in derived classes.</summary>
        public override bool Condition()
        {
            lock (InternalLock)
            {
                throw new InvalidOperationException("Condition evaluation method is not implemented in this class."
                    + Environment.NewLine + "This is probably an error in class implementation.");
            }
        }

        /// <summary>Must not be used!</summary>
        protected sealed override bool ConditionFunction()
        {
            throw new InvalidOperationException("Condition evaluation function should not be called in this class.");
        }

        /// <summary>Setter of this property must not be used!</summary>
        public sealed override ConditionDelegateBase ConditionDelegate
        {
            protected get { return null; }
            set
            {
                throw new InvalidOperationException("Condition delegate can not be set in this class.");
            }
        }

        /// <summary>protected internal acces to CancelFlag property.
        /// Enables access to CancelFlag on proxy classes.</summary>
        protected internal bool CancelFlagInternal  
        {
            get { lock (InternalLock) { return base.CancelFlag; } }
            set { lock (InternalLock) { base.CancelFlag = value; } }
        }


        #region ExamplesAndTests


        // STATIC TEST FUNCTIONS:

        // Test of blocking until file is created or removed:

        /// <summary>Monitors the specified file and successively blocks until it is created and then 
        /// until it is removed. This procedure is repeated twice.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        public static void ExampleBlockCreateRemoveLatence(string filePath)
        {
            new ExampleLatence().ExampleBlockCreateRemove(filePath);
        }

        /// <summary>Monitors the specified file and successively blocks until it is created and then 
        /// until it is removed.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
        public static void ExampleBlockCreateRemoveLatence(string filePath, int numSwitches)
        {
            new ExampleLatence().ExampleBlockCreateRemove(filePath, numSwitches);
        }

        /// <summary>Monitors the specified file and successively blocks until it is created and then 
        /// until it is removed.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
        /// <param name="directory">If true then creation/removal of a directory is waiting.</param>
        public static void ExampleBlockCreateRemoveLatence(string filePath, int numSwitches, bool waitDirectory)
        {
            new ExampleLatence().ExampleBlockCreateRemove(filePath, numSwitches, waitDirectory);
        }



        // SPEED TESTS (what event rates blocking functions can handle):

        /// <summary>Test of speed of reaction of file/directory creation and removal blocking waits.
        /// A specified number of alternate creations and removals are perfomed in a parallel
        /// thread, with specified delay between them. In the main thread, blocking waits are
        /// performed waiting for creation/removal in an infinite loop, and it is counted how
        /// many events are captured and how many are missed.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        public static void TestSpeedBlockCreateRemoveLatence(string filePath)
        {
            new ExampleLatence().TestSpeedBlockCreateRemove(filePath);
        }


        /// <summary>Test of speed of reaction of file/directory creation and removal blocking waits.
        /// A specified number of alternate creations and removals are perfomed in a parallel
        /// thread, with specified delay between them. In the main thread, blocking waits are
        /// performed waiting for creation/removal in an infinite loop, and it is counted how
        /// many events are captured and how many are missed.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
        /// <param name="sleepMs">Number of milliseconds to sleep between examples.</param>
        public static void TestSpeedBlockCreateRemoveLatence(string filePath, int numSwitches, int sleepMs)
        {
            new ExampleLatence().TestSpeedBlockCreateRemove(filePath, numSwitches, sleepMs);
        }


        /// <summary>Test of speed of reaction of file/directory creation and removal blocking waits.
        /// A specified number of alternate creations and removals are perfomed in a parallel
        /// thread, with specified delay between them. In the main thread, blocking waits are
        /// performed waiting for creation/removal in an infinite loop, and it is counted how
        /// many events are captured and how many are missed.</summary>
        /// <param name="filePath">File whose creation and removal is monitored.</param>
        /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
        /// <param name="sleepMs">Number of milliseconds to sleep between examples.</param>
        /// <param name="directory">If true then creation/removal of a directory is waiting.</param>
        public static void TestSpeedBlockCreateRemoveLatence(string filePath, int numSwitches,
                int sleepMs, bool waitDirectory)
        {
            new ExampleLatence().TestSpeedBlockCreateRemove(filePath, numSwitches,
                    sleepMs, waitDirectory);
        }


        /// <summary>Class containing examples for</summary>
        public class ExampleLatence
        {

            // Variables used in the examples to transfer data between threads:

            private object lockExample = new object();

            protected IWaitFileEvent _creation = null, _removal = null;

            // file or directory creation/removal waiters used by examples:
            protected virtual IWaitFileEvent Creation { 
                get  {
                    if (_creation == null)
                    {
                        if (IsDirectory)
                            _creation = new WaitDirectoryCreationLatence(ExamplePath);
                        else
                            _creation = new WaitFileCreationLatence(ExamplePath);
                    }
                    return _creation;
                } 
            }

            protected virtual IWaitFileEvent Removal { 
                get  {
                    if (_removal == null)
                    {
                        if (IsDirectory)
                            _removal = new WaitDirectoryRemovalLatence(ExamplePath);
                        else
                            _removal = new WaitFileRemovalLatence(ExamplePath);
                    }
                    return _removal;
                } 
            }

 
            // file that is monitored by the examples:
            protected string ExamplePath;

            // Whether we are working with a directory or a file:
            protected bool IsDirectory;

            // signals main thread that parallel thread has stopped
            protected bool TthreadStopped;

            // Number of successive creations and removals of the file:
            protected int NumIterations;

            // Sleeping time between successive removals and creations of the monitored file / directory:
            protected int SleepTimeMs;

            protected double TestTime;


            // Test of blocking until file is created or removed:

            /// <summary>Monitors the specified file and successively blocks until it is created and then 
            /// until it is removed. This procedure is repeated twice.</summary>
            /// <param name="filePath">File whose creation and removal is monitored.</param>
            public void ExampleBlockCreateRemove(string filePath)
            {
                lock (lockExample)
                {
                    ExampleBlockCreateRemove(filePath, 2);
                }
            }

            /// <summary>Monitors the specified file and successively blocks until it is created and then 
            /// until it is removed.</summary>
            /// <param name="filePath">File whose creation and removal is monitored.</param>
            /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
            public void ExampleBlockCreateRemove(string filePath, int numSwitches)
            {
                lock (lockExample)
                {
                    ExampleBlockCreateRemove(filePath, numSwitches, false /* waitDirectory */);
                }
            }

            /// <summary>Monitors the specified file and successively blocks until it is created and then 
            /// until it is removed.</summary>
            /// <param name="filePath">File whose creation and removal is monitored.</param>
            /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
            /// <param name="directory">If true then creation/removal of a directory is waiting.</param>
            public void ExampleBlockCreateRemove(string filePath, int numSwitches, bool waitDirectory)
            {
                lock (lockExample)
                {
                    this.ExamplePath = filePath;
                    this.IsDirectory = true;
                    this.NumIterations = numSwitches;

                    Console.WriteLine();
                    if (waitDirectory)
                    {
                        Console.WriteLine("Test of blocking until a directory is created or deleted (waits creation first):");
                        Console.WriteLine("Monitored directory: " + filePath);
                        Console.WriteLine("Number of blocking iterations (ceration/removal): " + numSwitches);
                        Console.WriteLine();
                        Console.WriteLine("Continuously create and remove the directory until the teest completes!");
                    }
                    else
                    {
                        Console.WriteLine("Test of blocking until a file is created or deleted (waits creation first):");
                        Console.WriteLine("Monitored file: " + filePath);
                        Console.WriteLine("Number of blocking iterations (ceration/removal): " + numSwitches);
                        Console.WriteLine();
                        Console.WriteLine("Continuously create and remove the file until the teest completes!");
                    }
                    Console.WriteLine();

                    for (int i = 1; i <= numSwitches; ++i)
                    {
                        Console.WriteLine("Iteration " + i + "/" + numSwitches + ":");
                        Console.Write("  Waiting for creation... ");
                        Creation.Wait();
                        Console.WriteLine(" .. created.");
                        Console.Write("  Waiting removal... ");
                        Removal.Wait();
                        Console.WriteLine(" ... removed.");
                    }
                    Console.WriteLine();
                    Console.WriteLine("End of the blocking test.");
                    Console.WriteLine();
                }
            }

            // SPEED TESTS (what event rates blocking functions can handle):

            /// <summary>Lock for file creation and removal operations.</summary>
            protected object fileOperationLock = new object();


            /// <summary>Alternately creates and removes the specified file or directory in its own thread.</summary>
            private void AlternateCreateRemoveExample()
            {
                TestTime = 0;
                StopWatch t = new StopWatch();
                t.Reset();
                t.Start();
                for (int i = 1; i <= NumIterations; ++i)
                {
                    if (SleepTimeMs > 0)
                        Thread.Sleep(SleepTimeMs);
                    lock (fileOperationLock)
                    {
                        if (IsDirectory)
                            Directory.CreateDirectory(ExamplePath);
                        else
                            using (TextWriter fw = File.CreateText(ExamplePath))
                            {
                                fw.WriteLine("Test file." + Environment.NewLine);
                            }
                    }
                    if (SleepTimeMs>0)
                        Thread.Sleep(SleepTimeMs);
                    lock (fileOperationLock)
                    {
                        if (IsDirectory)
                            Directory.Delete(ExamplePath, true /* recursive */ );
                        else
                            File.Delete(ExamplePath);
                    }
                }
                t.Stop();
                TestTime = t.Time;  // store execution time of the loop
                // Sleep for the final time to give the last blocker time to catch up, then instruct
                // main thread to exit the loop:
                if (SleepTimeMs > 0)
                    Thread.Sleep(SleepTimeMs);
                this.TthreadStopped = true;
                Thread.Sleep(600);
                // Make iteration in the main thread exid and cancel eventual blocking of the
                // underlaying file waiters:
                Creation.CancelOne();
                Removal.CancelOne();
            }


            /// <summary>Test of speed of reaction of file/directory creation and removal blocking waits.
            /// A specified number of alternate creations and removals are perfomed in a parallel
            /// thread, with specified delay between them. In the main thread, blocking waits are
            /// performed waiting for creation/removal in an infinite loop, and it is counted how
            /// many events are captured and how many are missed.</summary>
            /// <param name="filePath">File whose creation and removal is monitored.</param>
            public void TestSpeedBlockCreateRemove(string filePath)
            {
                TestSpeedBlockCreateRemove(filePath, 100 /* numSwitches */, 10 /* SleepMs */);
            }

            /// <summary>Test of speed of reaction of file/directory creation and removal blocking waits.
            /// A specified number of alternate creations and removals are perfomed in a parallel
            /// thread, with specified delay between them. In the main thread, blocking waits are
            /// performed waiting for creation/removal in an infinite loop, and it is counted how
            /// many events are captured and how many are missed.</summary>
            /// <param name="filePath">File whose creation and removal is monitored.</param>
            /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
            /// <param name="sleepMs">Number of milliseconds to sleep between examples.</param>
            public void TestSpeedBlockCreateRemove(string filePath, int numSwitches, int sleepMs)
            {
                TestSpeedBlockCreateRemove(filePath, numSwitches, sleepMs, false /* waitDirectory */);
            }


            /// <summary>Test of speed of reaction of file/directory creation and removal blocking waits.
            /// A specified number of alternate creations and removals are perfomed in a parallel
            /// thread, with specified delay between them. In the main thread, blocking waits are
            /// performed waiting for creation/removal in an infinite loop, and it is counted how
            /// many events are captured and how many are missed.</summary>
            /// <param name="filePath">File whose creation and removal is monitored.</param>
            /// <param name="numSwitches">Number of iterations (creation/removal waits).</param>
            /// <param name="sleepMs">Number of milliseconds to sleep between examples.</param>
            /// <param name="directory">If true then creation/removal of a directory is waiting.</param>
            public void TestSpeedBlockCreateRemove(string filePath, int numSwitches, 
                    int sleepMs, bool waitDirectory)
            {
                lock (lockExample)
                {
                    this.ExamplePath = filePath;
                    this.IsDirectory = waitDirectory;
                    this.NumIterations = numSwitches;
                    this.SleepTimeMs = sleepMs;
                    Console.WriteLine();
                    if (waitDirectory)
                    {
                        Console.WriteLine("Speed test of blocking until a directory is created or deleted (waits creation first):");
                        Console.WriteLine("Monitored directory: " + filePath);
                        Console.WriteLine("Number of iterations (ceration/removal): " + numSwitches);
                        Console.WriteLine();
                        Console.WriteLine("Directory will be alternately created /removed in a parallel thread.");
                        Console.WriteLine("Sleeping interval between individual operations: "
                            + SleepTimeMs + " ms");
                    }
                    else
                    {
                        Console.WriteLine("Speed test of blocking until a file is created or deleted (waits creation first):");
                        Console.WriteLine("Monitored file: " + filePath);
                        Console.WriteLine("Number of iterations (ceration/removal): " + numSwitches);
                        Console.WriteLine();
                        Console.WriteLine("Directory will be alternately created /removed in a parallel thread.");
                        Console.WriteLine("Sleeping interval between individual operations: "
                            + SleepTimeMs + " ms");
                    }
                    Console.WriteLine();

                    if (waitDirectory)
                    {
                        try
                        {
                            File.Delete(filePath);
                            Directory.Delete(filePath);
                        }
                        catch { }
                    } else
                    {
                        try
                        {
                            Directory.Delete(filePath);
                            File.Delete(filePath);
                        }
                        catch { }
                    }
                    this.TthreadStopped = false;
                    Thread fileThread = new Thread(this.AlternateCreateRemoveExample);
                    if (waitDirectory)
                        Console.WriteLine("Starting the thread that alternately creates and deletes the directory...");
                    else
                        Console.WriteLine("Starting the thread that alternately creates and deletes the file...");
                    fileThread.Start();

                    int numCatchedEvents = 0;
                    while (! this.TthreadStopped)
                    {
                        //Console.WriteLine("Iteration " + numCatchedEvents + "/" + numSwitches + ":");
                        //Console.Write("  Waiting for creation... ");
                        lock (fileOperationLock) {  } // just acquire the lock for synchronization with creating thread
                        Creation.Wait();
                        lock (fileOperationLock) { } // just acquire the lock for synchronization with creating thread
                        
                        //Console.WriteLine(" .. created.");
                        //Console.Write("  Waiting removal... ");
                        
                        lock (fileOperationLock) { } // just acquire the lock for synchronization with creating thread
                        Removal.Wait();
                        lock (fileOperationLock) { } // just acquire the lock for synchronization with creating thread
                        
                        //Console.WriteLine(" ... removed.");
                        
                        ++numCatchedEvents;
                    }
                    Console.WriteLine();
                    Console.WriteLine("End of the blocking speed test.");
                    Console.WriteLine("Results:");
                    Console.WriteLine("Number of creations and removals performed: " + this.NumIterations);
                    Console.WriteLine("Registered pairs of events: " + numCatchedEvents + ", missed: "
                        + (numSwitches - numCatchedEvents) + ".");
                    Console.WriteLine("Relative RATIO of registration: " +
                        ((double) (numCatchedEvents)) / (double) numSwitches);
                    Console.WriteLine("Duration of the test: " + TestTime + " s.");
                    Console.WriteLine("Event rate (taking into account creations and removals): "
                        + ((double) (2*numSwitches)/TestTime) + "/s.");
                    Console.WriteLine();
                }
            }

        }  // class ExampleLatence  - nested class


        #endregion ExamplesAndTests


    } // class WaitFileEventLatenceBase


    /// <summary>Implements blocking until the specified file is created (becomes existent).
    /// File is specified in constructor or by setting the FilePath property.
    /// Waiting is performed by calling the Wait() function.
    /// It is implemented by successively checking whether unblocking condition is fulfilled
    /// and by sleeping between checks. Therefore it has some latency (i.e. unblocking is not
    /// always performed immediatley the condition is met) and spends some CPU time and system resources.</summary>
    /// $A Igor Jun10;
    public class WaitFileCreationLatence : WaitFileEventLatenceBase
    {
        public WaitFileCreationLatence(string filePath) : base(filePath)
        {  }

        /// <summary>Returns true if the monitored file exists, and false if not.</summary>
        public override bool Condition()
        {
            if (String.IsNullOrEmpty(this.Path))
                throw new InvalidDataException("Monitiored file path is not specified properly (null or empty string).");
            return File.Exists(this.Path);
        }

    }

    /// <summary>Implements blocking until the specified file is deleted (becomes nonexistent).
    /// File is specified in constructor or by setting the FilePath property.
    /// Waiting is performed by calling the Wait() function.
    /// It is implemented by successively checking whether unblocking condition is fulfilled
    /// and by sleeping between checks. Therefore it has some latency (i.e. unblocking is not
    /// always performed immediatley the condition is met) and spends some CPU time and system resources.</summary>
    /// $A Igor Jun10;
    public class WaitFileRemovalLatence : WaitFileEventLatenceBase
    {
        public WaitFileRemovalLatence(string filePath)
            : base(filePath)
        { }

        /// <summary>Returns false if the monitored file exists, and true if not.</summary>
        public override bool Condition()
        {
            if (String.IsNullOrEmpty(this.Path))
                throw new InvalidDataException("Monitiored file path is not specified properly (null or empty string).");
            return !File.Exists(this.Path);
        }
    }


    /// <summary>Implements blocking until the specified directory is created (becomes existent).
    /// Directory is specified in constructor or by setting the DirectoryPath property.
    /// Waiting is performed by calling the Wait() function.
    /// It is implemented by successively checking whether unblocking condition is fulfilled
    /// and by sleeping between checks. Therefore it has some latency (i.e. unblocking is not
    /// always performed immediatley the condition is met) and spends some CPU time and system resources.</summary>
    /// $A Igor Jun10;
    public class WaitDirectoryCreationLatence : WaitFileEventLatenceBase
    {
        public WaitDirectoryCreationLatence(string directoryPath)
            : base(directoryPath)
        { }

        /// <summary>Returns true if the monitored directory exists, and false if not.</summary>
        public override bool Condition()
        {
            if (String.IsNullOrEmpty(this.Path))
                throw new InvalidDataException("Monitored directory path is not specified properly (null or empty string).");
            return Directory.Exists(this.Path);
        }

    }

    /// <summary>Implements blocking until the specified directory is deleted (becomes nonexistent).
    /// Directory is specified in constructor or by setting the DirectoryPath property.
    /// Waiting is performed by calling the Wait() function.
    /// It is implemented by successively checking whether unblocking condition is fulfilled
    /// and by sleeping between checks. Therefore it has some latency (i.e. unblocking is not
    /// always performed immediatley the condition is met) and spends some CPU time and system resources.</summary>
    /// $A Igor Jun10;
    public class WaitDirectoryRemovalLatence : WaitFileEventLatenceBase
    {
        public WaitDirectoryRemovalLatence(string directoryPath)
            : base(directoryPath)
        { }

        /// <summary>Returns false if the monitored directory exists, and true if not.</summary>
        public override bool Condition()
        {
            if (String.IsNullOrEmpty(this.Path))
                throw new InvalidDataException("Monitored directory path is not specified properly (null or empty string).");
            return ! Directory.Exists(this.Path);
        }
    }

    #endregion LatenceceWaiting


} 