 // Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using IG.Lib;
using System.Security.Principal;
using System.Security.AccessControl;

namespace IG.Num
{

    /// <summary>Performs elementary operations for optimization and analysis servers and 
    /// clients that exchange data and messages through files.
    /// Each such server can serve a single request at a time (single thread of execution).
    /// Client-server pair (or pairs, when both analysis and optimization are performed in
    /// this way) has (or have) a single directory for exchanging data and messages. 
    /// If there is a need for analyses running in parallel, each thread must have its own
    /// directory and its own client-server pair.</summary>
    /// <remarks>WARNING:
    /// This module is taken from Dragonfly opt. server and adapted for purpose of some
    /// projects. If necessary to further develop, synchronize (& possibly merge) with
    /// Dragonfly, otherwise there will be problems with consistent development of both branches.</remarks>
    /// $A Igor jul08 Mar11;
    public class OptFileManager: ILockable
    {


        /// <summary>Creates a new optimization file server manager.</summary>
        /// <param name="directoryPath">Path to the working directory for the current manager.
        /// This is the directory where all data and commmunication (data transfer & messaging) 
        /// files are located.</param>
        public OptFileManager(string directoryPath)
        { DataDirectory = directoryPath; }


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        //private object _internalLock = new object();

        ///// <summary>Used internally for locking access to internal fields.</summary>
        //protected object Lock { get { return _internalLock; } }

        //private object waitlock = new object();

        ///// <summary>Must be used only for locking waiting the Waiting() block (since it is potentially time consuming).</summary>
        //protected object WaitLock { get { return waitlock; } }

        #endregion ThreadLocking


        #region OperationData


        #region DataDirectory

        protected string _directory = null;

        /// <summary>Directory for data and messages exchange through files.</summary>
        public string DataDirectory
        {
            get
            {
                lock(Lock)
                {
                if (String.IsNullOrEmpty(_directory))
                    throw new InvalidDataException("Directory of the optimization/analysis file server is not specified.");
                return _directory;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (String.IsNullOrEmpty(value))
                        throw new ArgumentException("Directory of the optimization/analysis file server is not specified (null or empty string).");
                    string dir = Path.GetFullPath(value);
                    if (!Directory.Exists(dir))
                    {
                        try
                        {
                            Directory.CreateDirectory(dir);
                        }
                        catch { }
                        if (!Directory.Exists(_directory))
                            throw new ArgumentException("The specified optimization/analysis file server directory does not exist. "
                                + Environment.NewLine + "  Directory: " + value
                                + Environment.NewLine + "  Full path: " + dir);
                    }
                    _directory = dir;
                }

            }
        }

        /// <summary>Returns full path of the file or directory with the specified relative path
        /// within the data and messages exchange directory.</summary>
        /// <param name="relativePath">Relative path (with respect to data and messages exchange directory)
        /// </param>
        /// <returns></returns>
        public string GetPath(string relativePath)
        {
            lock (Lock)
            {
                return Path.Combine(DataDirectory, relativePath) ;
            }
        }

        #endregion DataDirectory


        #region Constants


        // CONSTANTS: 

        // DATA EXCHANGE FILES
        protected  string
            _anInMathFilename = OptFileConst.AnInMathFileName,
            _anInMathPath = null,
            _anInJsonFilename = OptFileConst.AnInJsonFilename,
            _anInJsonPath = null,
            _anInXmlFilename = OptFileConst.AnInXmlFileName,
            _anInXmlPath = null,
            _anOutMathFilename = OptFileConst.AnOutMathFilename,
            _anOutMathPath = null,
            _anOutJsonFilename = OptFileConst.AnOutJsonFilename,
            _anOutJsonPath = null,
            _anOutXmlFilename = OptFileConst.AnOutXmlFilename,
            _anOutXmlPath = null;

        // MESSAGE FILES:
        protected string
            _msgAnBusyFileName = OptFileConst.MsgAnBusyFilename,
            _msgAnBusyPath = null,
            _msgAnInputReadyFileName = OptFileConst.MsgAnInputReadyFilename,
            _msgAnInputReadyPath = null,
            _msgAnResultsReadyFileName = OptFileConst.MsgAnResultsReadyFilename,
            _msgAnResultsReadyPath = null,

            _msgOptBusyFilename = OptFileConst.MsgOptBusyFilename,
            _msgOptBusyPath = null,
            _msgOptInputReadyFilename = OptFileConst.MsgOptDataReadyFilename,
            _msgOptInputReadyPath = null,
            _msgOptResultsReadyFilename = OptFileConst.MsgOptResultsReadyFilename,
            _msgOptResultsReadyPath = null;

        // INTERFACE WITH PROGRAM Inverse:
        protected string
            _invOptCommandFilename = OptFileConst.InvOptCommandFilename,
            _invOptCommandPath = null,
            _invAnCommandFilename = OptFileConst.InvAnCommandFilename,
            _invAnCommandPath = null;

        #endregion Constants


        // FILE SYSTEM LOCKING:

        //protected string _LockFileMutexName = OptFileConst.LockFileMutexName;

        ///// <summary>Name of the mutex for locking file systam.</summary>
        //public string LockFileMutexName
        //{
        //    get { lock (Lock) { return _LockFileMutexName; } }
        //    protected set { lock (Lock) { _LockFileMutexName = value; } }
        //}


        protected string _lockFileMutexName = OptFileConst.LockFileMutexName;

        /// <summary>Name of the mutex for system-wide locking of files.</summary>
        public string LockFileMutexName
        {
            get { lock (Lock) {
                if (string.IsNullOrEmpty(_lockFileMutexName))
                    throw new InvalidDataException("File locking mutex name is not specified (null or empty string).");
                return _lockFileMutexName; } }
            protected set { _lockFileMutexName = value; }
        }

        protected Mutex _lockFileMutex;

        /// <summary>Mutex for system-wide exclusive locks for file system operations
        /// related to the current class.</summary>
        protected Mutex LockFileMutex
        {
            get {
                if (_lockFileMutex == null)
                {
                    lock (Lock)
                    {
                        if (_lockFileMutex == null)
                        {
                            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                            MutexSecurity mutexsecurity = new MutexSecurity();
                            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.FullControl, AccessControlType.Allow));
                            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.ChangePermissions, AccessControlType.Deny));
                            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.Delete, AccessControlType.Deny));
                            bool createdNew;
                            // _lockFileMutex = new Mutex(false, LockFileMutexName);
                            _lockFileMutex = new Mutex(false, LockFileMutexName,
                                out createdNew, mutexsecurity);
                        }
                    }
                }
                return _lockFileMutex;
            }
        }


        /// <summary>Check whether the filesystem locking mutex (property <see cref="LockFileMutex"/>) has been abandoned, 
        /// and returns true if it has been (otherwise, false is returned).
        /// <para>After the call, mutex is no longer in abandoned state (WaitOne() will not throw an exception)
        /// if it has been before the call.</para>
        /// <para>Call does not block.</para></summary>
        /// <returns>true if mutex has been abandoned, false otherwise.</returns>
        public bool LockFileMutexCheckAbandoned()
        {
            return Util.MutexCheckAbandoned(LockFileMutex);
        }
        


        // DATA EXCHANGE FILES:


        /// <summary>File path of the analysis input file in standard IGLib format.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string AnInMathPath {
            get
            {
                lock (Lock)
                {
                    if (_anInMathPath == null)
                        _anInMathPath = Path.Combine(DataDirectory, _anInMathFilename);
                    return _anInMathPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _anInMathFilename = Path.GetFileName(value);
                    }
                    _anInMathPath = null;  // invalidate the path.
                }
            }
        }

        /// <summary>File path of the analysis input file in Json format.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string AnInJsonPath
        {
            get
            {
                lock (Lock)
                {
                    if (_anInJsonPath == null)
                        _anInJsonPath = Path.Combine(DataDirectory, _anInJsonFilename);
                    return _anInJsonPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _anInJsonFilename = Path.GetFileName(value);
                    }
                    _anInJsonPath = null;
                }
            }
        }



        /// <summary>File path of the analysis input file in XML format.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string AnInXmlPath {
            get { 
                lock (Lock) 
                {
                    if (_anInXmlPath==null)
                        _anInXmlPath = Path.Combine(DataDirectory, _anInXmlFilename);
                    return _anInXmlPath; 
                } 
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _anInXmlFilename = Path.GetFileName(value);
                    }
                    _anInXmlPath = null;
                }
            }
        }

        /// <summary>File path of the analysis output file in standard IGLib format.
        /// Setter takes only pure file name, without path information.
        /// If set to null then fle path is set to null and will be recalculated when getter is called.</summary>
        public string AnOutMathPath {
            get
            {
                lock (Lock)
                {
                    if (_anOutMathPath == null)
                        _anOutMathPath = Path.Combine(DataDirectory, _anOutMathFilename);
                    return _anOutMathPath;
                }
            }
            protected set
            {
                lock (Lock) 
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _anOutMathFilename = Path.GetFileName(value);
                    }
                    _anOutMathPath = null;
                }
            }
        }


        /// <summary>File path of the analysis output file in JSON format.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string AnOutJsonPath
        {
            get
            {
                lock (Lock)
                {
                    if (_anOutJsonPath == null)
                        _anOutJsonPath = Path.Combine(DataDirectory, _anOutJsonFilename);
                    return _anOutJsonPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _anOutJsonFilename = Path.GetFileName(value);
                    }
                    _anOutJsonPath = null;
                }
            }
        }



        /// <summary>File path of the analysis output file in XML format.
        /// Setter takes only pure file name, without path information.
        /// If set to null then file path is set to null and will be recalculated when getter is called.</summary>
        public string AnOutXmlPath {
            get
            {
                lock (Lock)
                {
                    if (_anOutXmlPath == null)
                        _anOutXmlPath = Path.Combine(DataDirectory, _anOutXmlFilename);
                    return _anOutXmlPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _anOutXmlFilename = Path.GetFileName(value);
                    }
                    _anOutXmlPath = null;
                }
            }
        }



        // MESSAGE FILES:

        /// <summary>File path of the analysis busy flag file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then path is set to null and will be recalculated when getter is called.</summary>
        public string MsgAnBusyPath {
            get
            {
                lock (Lock)
                {
                    if (_msgAnBusyPath == null)
                        _msgAnBusyPath = Path.Combine(DataDirectory, _msgAnBusyFileName);
                    return _msgAnBusyPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _msgAnBusyFileName = Path.GetFileName(value);
                    }
                    _msgAnBusyPath = null;
                }
            }
        }

        /// <summary>File path of the analysis input data ready flag file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then path is set to null and will be recalculated when getter is called.</summary>
        public string MsgAnInputReadyPath {
            get
            {
                lock (Lock)
                {
                    if (_msgAnInputReadyPath == null)
                        _msgAnInputReadyPath = Path.Combine(DataDirectory, _msgAnInputReadyFileName);
                    return _msgAnInputReadyPath;
                }
            }
            protected set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _msgAnInputReadyFileName = Path.GetFileName(value);
                }
                _msgAnInputReadyPath = null;
            }
        }

        /// <summary>File path of the analysis results ready flag file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then path is set to null and will be recalculated when getter is called.</summary>
        public string MsgAnResultsReadyPath {
            get
            {
                lock (Lock)
                {
                    if (_msgAnResultsReadyPath == null)
                        _msgAnResultsReadyPath = Path.Combine(DataDirectory, _msgAnResultsReadyFileName);
                    return _msgAnResultsReadyPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _msgAnResultsReadyFileName = Path.GetFileName(value);
                    }
                    _msgAnResultsReadyPath = null;
                }
            }
        }

        /// <summary>File path of the optimization busy flag file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then path is set to null and will be recalculated when getter is called.</summary>
        public string MsgOptBusyPath {
            get
            {
                lock (Lock)
                {
                    if (_msgOptBusyPath == null)
                        _msgOptBusyPath = Path.Combine(DataDirectory, _msgOptBusyFilename);
                    return _msgOptBusyPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _msgOptBusyFilename = Path.GetFileName(value);
                    }
                    _msgOptBusyPath = null;
                }
            }
        }

        /// <summary>File path of the optimization input data ready flag file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then path is set to null and will be recalculated when getter is called.</summary>
        public string MsgOptInputReadyPath {
            get
            {
                lock (Lock)
                {
                    if (_msgOptInputReadyPath == null)
                        _msgOptInputReadyPath = Path.Combine(DataDirectory, _msgOptInputReadyFilename);
                    return _msgOptInputReadyPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _msgOptInputReadyFilename = Path.GetFileName(value);
                    }
                    _msgOptInputReadyPath = null;
                }
            }
        }

        /// <summary>File path of the optimization resutlts ready flag file.
        /// Setter takes only pure file name, without path information.
        /// If set to null then path is set to null and will be recalculated when getter is called.</summary>
        public string MsgOptResultsReadyPath {
            get
            {
                lock (Lock)
                {
                    if (_msgOptResultsReadyPath == null)
                        _msgOptResultsReadyPath = Path.Combine(DataDirectory, _msgOptResultsReadyFilename);
                    return _msgOptResultsReadyPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _msgOptResultsReadyFilename = Path.GetFileName(value);
                    }
                    _msgOptResultsReadyPath = null;
                }
            }
        }


        // INTERFACE WITH PROGRAM Inverse:

        /// <summary>File path of the optimization command file for program Inverse (Inverse interface).
        /// Setter takes only pure file name, without path information.
        /// If set to null then path is set to null and will be recalculated when getter is called.</summary>
        public string InvOptCommandPath {
            get
            {
                lock (Lock)
                {
                    if (_invOptCommandPath == null)
                        _invOptCommandPath = Path.Combine(DataDirectory, _invOptCommandFilename);
                    return _invOptCommandPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _invOptCommandFilename = Path.GetFileName(value);
                    }
                    _invOptCommandPath = null;
                }
            }
        }

        /// <summary>File path of the analysis command file for program Inverse (Inverse interface).
        /// Setter takes only pure file name, without path information.
        /// If set to null then path is set to null and will be recalculated when getter is called.</summary>
        public string InvAnCommandFilePath {
            get
            {
                lock (Lock)
                {
                    if (_invAnCommandPath == null)
                        _invAnCommandPath = Path.Combine(DataDirectory, _invAnCommandFilename);
                    return _invAnCommandPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _invAnCommandFilename = Path.GetFileName(value);
                    }
                    _invAnCommandPath = null;
                }
            }
        }

        #endregion OperationData


        #region SerializationSupport

        private ISerializer _serializerJson;

        //private AnalysisRequestDTO _anInputDTO;
        //private AnalysisResultsDTO _anOutputDTO;

        private bool _anFormatMath = true;
        private bool _anFormatJson = true;
        private bool _anFormatXml = false;

        /// <summary>Serializer for JSON format.</summary>
        protected ISerializer SerializerJson
        { 
            get 
            {
                if (_serializerJson == null)
                    _serializerJson = new SerializerJson();
                return _serializerJson;
            } 
        }


        /// <summary>Gets or sets the flag indicating whether analysis input and output is written Math format.</summary>
        public bool AnFormatMath
        { get { return _anFormatMath; } set { _anFormatMath = value; } }

        /// <summary>Gets or sets the flag indicating whether analysis input and output is written JSON format.</summary>
        public bool AnFormatJson
        { get { return _anFormatJson; } set { _anFormatJson = value; } }

        /// <summary>Gets or sets the flag indicating whether analysis input and output is written XML format.</summary>
        public bool AnFormatXml
        { get { return _anFormatXml; } set { _anFormatXml = value; } }


        private AnalysisResults _anResults;

        /// <summary>Analysis reaults whae analysis request data and analysis results ar stored.</summary>
        protected AnalysisResults AnResults
        {
            get
            {
                if (_anResults == null)
                    _anResults = new AnalysisResults();
                return _anResults;
            }
        }

        #endregion SerializationSupport


        #region Messages

        /// <summary>Gets a flag telling whether analysis input data is ready.</summary>
        public virtual bool IsAnInputReady()
        {
            bool ret = false;
            bool lockAcquired = false;
            try
            {
                lockAcquired = LockFileMutex.WaitOne(-1 /* wait indefinitely */);
                if (File.Exists(MsgAnInputReadyPath))
                    ret =  true;
                else
                    ret = false;
            }
            catch (AbandonedMutexException ex)
            {
                if (ex.Mutex != null)
                {
                    ex.Mutex.ReleaseMutex();
                    lockAcquired = false;
                }
            }
            finally
            {
                if (lockAcquired)
                    LockFileMutex.ReleaseMutex();
            }
            return ret;
        }

        /// <summary>Waits until analysis data is ready.
        /// REMARK:
        /// Currently this file just checks existence of the file in a loop.
        /// This should be changed in the future because it is not the best solution.</summary>
        public virtual void WaitAnInputReady()
        {
            while (!IsAnInputReady())
            {
                Thread.Sleep(100);
            }
        }

        /// <summary>Clears the analysis data ready flag.</summary>
        public virtual void ClearAnInputReady()
        {
            bool lockAcquired = false;
            try
            {
                lockAcquired = LockFileMutex.WaitOne(-1 /* wait indefinitely */);
                try
                {
                    File.Delete(MsgAnInputReadyPath);
                }
                catch { }
            }
            catch (AbandonedMutexException ex)
            {
                if (ex.Mutex != null)
                {
                    ex.Mutex.ReleaseMutex();
                    lockAcquired = false;
                }
            }
            finally
            {
                if (lockAcquired)
                    LockFileMutex.ReleaseMutex();
            }
        }

        /// <summary>Sets the analysis input ready flag.</summary>
        public virtual void SetAnInputReady()
        {
            bool lockAcquired = false;
            try
            {
                lockAcquired = LockFileMutex.WaitOne(-1 /* wait indefinitely */);
                string path = MsgAnInputReadyPath;
                using (TextWriter sw = new StreamWriter(path))
                { }
            }
            catch (AbandonedMutexException ex)
            {
                if (ex.Mutex != null)
                {
                    ex.Mutex.ReleaseMutex();
                    lockAcquired = false;
                }
            }
            finally
            {
                if (lockAcquired)
                    LockFileMutex.ReleaseMutex();
            }
        }

        /// <summary>Gets a flag telling whether analysis output data is ready.</summary>
        public virtual bool IsAnResultsReady()
        {
            bool ret = false;
            bool lockAcquired = false;
            try
            {
                lockAcquired = LockFileMutex.WaitOne(-1 /* wait indefinitely */);
                if (File.Exists(MsgAnResultsReadyPath))
                    ret =  true;
                else
                    ret = false;
            }
            catch (AbandonedMutexException ex)
            {
                if (ex.Mutex != null)
                {
                    ex.Mutex.ReleaseMutex();
                    lockAcquired = false;
                }
            }
            finally
            {
                if (lockAcquired)
                    LockFileMutex.ReleaseMutex();
            }
            return ret;
        }

        /// <summary>Waits until analysis data is ready.
        /// REMARK:
        /// Currently this file just checks existence of the file in a loop.
        /// This should be changed in the future because it is not the best solution.</summary>
        public virtual void WaitAnResultsReady()
        {
            while (!IsAnResultsReady())
            {
                Thread.Sleep(100);
            }
        }

        /// <summary>Clears the analysis results ready flag.</summary>
        public virtual void ClearAnResultsReady()
        {
            bool lockAcquired = false;
            try
            {
                lockAcquired = LockFileMutex.WaitOne(-1 /* wait indefinitely */);
                try
                {
                    File.Delete(MsgAnResultsReadyPath);
                }
                catch { }
            }
            catch (AbandonedMutexException ex)
            {
                if (ex.Mutex != null)
                {
                    ex.Mutex.ReleaseMutex();
                    lockAcquired = false;
                }
            }
            finally
            {
                if (lockAcquired)
                    LockFileMutex.ReleaseMutex();
            }
        }

        /// <summary>Sets the analysis results ready flag.</summary>
        public virtual void SetAnResultsReady()
        {
            bool lockAcquired = false;
            try
            {
                lockAcquired = LockFileMutex.WaitOne(-1 /* wait indefinitely */);
                string path = MsgAnResultsReadyPath;
                using (TextWriter sw = new StreamWriter(path))
                { }
            }
            catch (AbandonedMutexException ex)
            {
                if (ex.Mutex != null)
                {
                    ex.Mutex.ReleaseMutex();
                    lockAcquired = false;
                }
            }
            finally
            {
                if (lockAcquired)
                    LockFileMutex.ReleaseMutex();
            }
        }


        /// <summary>Gets a flag telling whether direct analysis is busy.</summary>
        public virtual bool IsAnBusy()
        {
            bool ret = false;
            bool lockAcquired = false;
            try
            {
                lockAcquired = LockFileMutex.WaitOne(-1 /* wait indefinitely */);
                if (File.Exists(MsgAnBusyPath))
                    ret = true;
                else
                    ret = false;
            }
            catch (AbandonedMutexException ex)
            {
                if (ex.Mutex != null)
                {
                    ex.Mutex.ReleaseMutex();
                    lockAcquired = false;
                }
            }
            finally
            {
                if (lockAcquired)
                    LockFileMutex.ReleaseMutex();
            }
            return ret;
        }

        /// <summary>Waits until analysis is ready.
        /// REMARK:
        /// Currently this file just checks existence of the file in a loop.
        /// This should be changed in the future because it is not the best solution.</summary>
        public virtual void WaitAnReady()
        {
            while (IsAnBusy())
            {
                Thread.Sleep(100);
            }
        }

        /// <summary>Clears the analysis busy flag.</summary>
        public virtual void ClearAnBusy()
        {
            bool lockAcquired = false;
            try
            {
                lockAcquired = LockFileMutex.WaitOne(-1 /* wait indefinitely */);
                try
                {
                    File.Delete(MsgAnBusyPath);
                }
                catch { }
            }
            catch (AbandonedMutexException ex)
            {
                if (ex.Mutex != null)
                {
                    ex.Mutex.ReleaseMutex();
                    lockAcquired = false;
                }
            }
            finally
            {
                if (lockAcquired)
                    LockFileMutex.ReleaseMutex();
            }
        }

        /// <summary>Sets the analysis busy flag.</summary>
        public virtual void SetAnBusy()
        {
            bool lockAcquired = false;
            try
            {
                lockAcquired = LockFileMutex.WaitOne(-1 /* wait indefinitely */);
                string path = MsgAnBusyPath;
                using (TextWriter sw = new StreamWriter(path))
                { }
            }
            catch (AbandonedMutexException ex)
            {
                if (ex.Mutex != null)
                {
                    ex.Mutex.ReleaseMutex();
                    lockAcquired = false;
                }
            }
            finally
            {
                if (lockAcquired)
                    LockFileMutex.ReleaseMutex();
            }
        }

        /// <summary>Clears messages from the working directory.</summary>
        public void ClearMessages()
        {
            bool lockAcquired = false;
            try
            {
                lockAcquired = LockFileMutex.WaitOne(-1 /* wait indefinitely */);
                ClearAnInputReady();
                ClearAnResultsReady();
                ClearAnBusy();
            }
            catch (AbandonedMutexException ex)
            {
                if (ex.Mutex != null)
                {
                    ex.Mutex.ReleaseMutex();
                    lockAcquired = false;
                }
            }
            finally
            {
                if (lockAcquired)
                    LockFileMutex.ReleaseMutex();
            }
        }

        #endregion Messages


        #region OperationBasic


        /// <summary>Stores analysis input data to the specified file in the standard format
        /// (Mathematica-like, but with C style numbers).
        /// Does not perform any locking.</summary>
        /// <param name="inputFilePath">Full path of the file that data is written to.</param>
        /// <param name="anpt">Analysis input data.</param>
        protected virtual void WriteAnalysisInputMath(string filePath, AnalysisResults anpt)
        {
            AnalysisResults.SaveRequestMath(anpt, filePath);
        }

        /// <summary>Stores analysis input data to the specified file in the JSON format. 
        /// Does not perform any locking.</summary>
        /// <param name="inputFilePath">Full path of the file that data is written to.</param>
        /// <param name="anpt">Analysis input data.</param>
        protected virtual void WriteAnalysisInputJson(string filePath, AnalysisResults anpt)
        {
            lock (Lock)
            {
                AnalysisRequestDto anInputDTO = new AnalysisRequestDto();
                anInputDTO.CopyFrom(anpt);
                SerializerJson.Serialize<AnalysisRequestDto>(anInputDTO, filePath);
            }
        }

        /// <summary>Stores analysis input data to the specified file in the XML format 
        /// Does not perform any locking.</summary>
        /// <param name="inputFilePath">Full path of the file that data is written to.</param>
        /// <param name="anpt">Analysis results.</param>
        protected virtual void WriteAnalysisInputXml(string filePath, AnalysisResults anpt)
        {
            throw new NotImplementedException("Saving analysis input in XML format is not yet implemented.");
        }


        /// <summary>Reads analysis input data (request) from the specified file in the standard IGLib format,
        /// and stores the data in the specified object.</summary>
        /// <param name="inputFilePath">Path to the file that data is read from.</param>
        /// <param name="anpt">Analysis results object where the data is stored.</param>
        protected virtual void ReadAnalysisInputMath(string filePath, ref AnalysisResults anpt)
        {
            AnalysisResults.LoadRequestMath(filePath, ref anpt);
            // throw new NotImplementedException("Reading analysis input in standard IGLIB format is not yet implemented.");
        }

        /// <summary>Reads analysis input data (request) from the specified file in the JSON format,
        /// and stores the data in the specified object.</summary>
        /// <param name="inputFilePath">Path to the file that data is read from.</param>
        /// <param name="anpt">Analysis results object where the data is stored.</param>
        protected virtual void ReadAnalysisInputJson(string filePath, ref AnalysisResults anpt)
        {
            AnalysisRequestDto dtoRestored = SerializerJson.DeserializeFile<AnalysisRequestDto>(filePath);
            dtoRestored.CopyTo(ref anpt);
        }

        /// <summary>Reads analysis input data (request) from the specified file in the XML format,
        /// and stores the data in the specified object.</summary>
        /// <param name="inputFilePath">Path to the file that data is read from.</param>
        /// <param name="anpt">Analysis results object where the data is stored.</param>
        protected virtual void ReadAnalysisInputXml(string filePath, ref AnalysisResults anpt)
        {
            throw new NotImplementedException("Reading analysis input in standard XML format is not yet implemented.");
        }


        /// <summary>Stores analysis results to the specified file in the standard format 
        /// (Mathematica-like, but with C style numbers).
        /// Does not perform any locking.</summary>
        /// <param name="inputFilePath">Full path of the file that data is written to.</param>
        /// <param name="anpt">Analysis output data.</param>
        protected virtual void WriteAnalysisOutputMath(string filePath, AnalysisResults anpt)
        {
            AnalysisResults.SaveMath(anpt, filePath);
        }

        /// <summary>Stores analysis output data to the specified file in the JSON format 
        /// Does not perform any locking.</summary>
        /// <param name="inputFilePath">Full path of the file that data is written to.</param>
        /// <param name="anpt">Analysis results.</param>
        protected virtual void WriteAnalysisOutputJson(string filePath, AnalysisResults anpt)
        {
            lock (Lock)
            {
                AnalysisResultsDto anOutputDTO = new AnalysisResultsDto();
                anOutputDTO.CopyFrom(anpt);
                SerializerJson.Serialize<AnalysisResultsDto>(anOutputDTO, filePath);
            }
        }

        /// <summary>Stores analysis output data to the specified file in the XML format .
        /// Does not perform any locking.</summary>
        /// <param name="inputFilePath">Full path of the file that data is written to.</param>
        /// <param name="anpt">Analysis results.</param>
        protected virtual void WriteAnalysisOutputXml(string filePath, AnalysisResults anpt)
        {
            throw new NotImplementedException("Saving analysis output in XML format is not yet implemented.");
        }


        /// <summary>Reads analysis output data (request) from the specified file in the standard IGLib format,
        /// and stores the data in the specified object.</summary>
        /// <param name="inputFilePath">Path to the file that data is read from.</param>
        /// <param name="anpt">Analysis results object where the data is stored.</param>
        protected virtual void ReadAnalysisOutputMath(string filePath, ref AnalysisResults anpt)
        {
            AnalysisResults.LoadMath(filePath, ref anpt);
        }

        /// <summary>Reads analysis output data (request) from the specified file in the JSON format,
        /// and stores the data in the specified object.</summary>
        /// <param name="inputFilePath">Path to the file that data is read from.</param>
        /// <param name="anpt">Analysis results object where the data is stored.</param>
        protected virtual void ReadAnalysisOutputJson(string filePath, ref AnalysisResults anpt)
        {
            AnalysisResultsDto dtoRestored = SerializerJson.DeserializeFile<AnalysisResultsDto>(filePath);
            dtoRestored.CopyTo(ref anpt);
        }

        /// <summary>Reads analysis output data (request) from the specified file in the XML format,
        /// and stores the data in the specified object.</summary>
        /// <param name="inputFilePath">Path to the file that data is read from.</param>
        /// <param name="anpt">Analysis results object where the data is stored.</param>
        protected virtual void ReadAnalysisOutputXml(string filePath, ref AnalysisResults anpt)
        {
            throw new NotImplementedException("Reading analysis output in standard XML format is not yet implemented.");
        }

        #endregion OperationBasic


        #region OperationClient


        /// <summary>Client writes analysis input data for calculation of analysis results.
        /// Messages are set nad cleared appropriately.</summary>
        /// <param name="inputParameters">Parameters to be written.</param>
        public virtual void ClientWriteAnInput(AnalysisResults anInput)
        {
            SetAnBusy();
            ClearAnInputReady();
            if (AnFormatMath)
                WriteAnalysisInputMath(AnInMathPath, anInput);
            if (AnFormatJson)
                WriteAnalysisInputJson(AnInJsonPath, anInput);
            if (AnFormatXml)
                WriteAnalysisInputXml(AnInXmlPath, anInput);
            SetAnInputReady();
        }

        /// <summary>Client reads analysis results.
        /// Messages are set and cleared appropriately.</summary>
        /// <param name="anres">Object where results are written.</param>
        public virtual void ClientReadAnOutput(ref AnalysisResults anres)
        {
            if (AnFormatJson && File.Exists(AnOutJsonPath))
                ReadAnalysisOutputJson(AnOutJsonPath, ref anres);
            else if (AnFormatMath && File.Exists(AnOutMathPath))
                ReadAnalysisOutputMath(AnOutMathPath, ref anres);
            else if (AnFormatXml && File.Exists(AnOutXmlPath))
                ReadAnalysisOutputXml(AnOutXmlPath, ref anres);
            ClearAnResultsReady();
        }

        /// <summary>Sends request to the server for calculation of analysis response.</summary>
        public virtual void ClientSendAnalysisRequest()
        {
            ServerAnalyse();
        }

        /// <summary>Calculates analysis results by using the analysis server.
        /// Writes analysis input, sends request to the server, and reads the calculated results.</summary>
        /// <param name="inputParameters">Intput parameters for which approximation is calculated.</param>
        /// <param name="outputValues">Analysis results object where approximation output values are stored.</param>
        public virtual void ClientCalculateAnalysisResults(ref AnalysisResults anRes)
        {
            ClientWriteAnInput(anRes);
            ClientSendAnalysisRequest();
            ClientReadAnOutput(ref anRes);
        }

        /// <summary>Performs client-side test calculation of analysis response.</summary>
        /// <param name="inputFilePath">Path to the JSON file where input parameters are read from.
        /// The file pointed at must exist.</param>
        /// <param name="reqObjective">Flag indicating whether objective function must be calculated.</param>
        /// <param name="reqConstraints">Flag indicating whether constraint functions must be calculated.</param>
        /// <param name="reqGradObjective">Flag indicating whether objective function gradientmust be calculated.</param>
        /// <param name="reqGradOConstraints">Fleg indicating whether constraint function gradients must be calculated.</param>
        /// <param name="deletedFilePath">Path of a file where the calculated analysis response in JSON is written to.
        /// It can be null or empty string, in this case response is not written to a file (but it is 
        /// output on console).</param>
        public virtual void ClientTestCalculateAnalysisResults(string inputFilePath, 
            bool reqObjective, bool reqConstraints, bool reqObjectiveGradient, bool reqConstraintGradients,
            string outputFilePath)
        {
            if (!File.Exists(inputFilePath))
                throw new ArgumentException("File with input parameters for direct analysis does not exist: "
                    + Environment.NewLine + "  " + inputFilePath + ".");
            Console.WriteLine();
            Console.WriteLine("Performing test analysis...");
            IVector inputParameters = null;
            Vector.LoadJson(inputFilePath, ref inputParameters);
            Console.WriteLine("Input parameters read from " + inputFilePath + ".");
            AnalysisResults anRes = new AnalysisResults();
            anRes.Parameters = inputParameters;
            anRes.ReqObjective = reqObjective;
            anRes.ReqConstraints = reqConstraints;
            anRes.ReqObjectiveGradient = reqObjectiveGradient;
            anRes.ReqConstraintGradients = reqConstraintGradients;
            ClientCalculateAnalysisResults(ref anRes);
            Console.WriteLine();
            Console.WriteLine("Direct analysis results:");
            Console.WriteLine(anRes);
            //Console.WriteLine("Input parameters: " + Environment.NewLine + "  " + inputParameters);
            //Console.WriteLine("Approximated values: " + Environment.NewLine + "  " + outputValues);
            Console.WriteLine();
            if (!string.IsNullOrEmpty(outputFilePath))
            {
                AnalysisResults.SaveJson(anRes, outputFilePath);
                Console.WriteLine();
                Console.WriteLine("Analysis results saved to " + outputFilePath + ".");
            }
            Console.WriteLine("... approximation done.");
            Console.WriteLine();
        }


        #endregion OperationClient


        #region OperationServer

        private IAnalysis _analysis;

        public virtual IAnalysis Analysis
        {
            get { return _analysis; }
            set { _analysis = value; }
        }

        /// <summary>Performs direct analysis (in optimization) with prescribed analysis input data, and saves results.
        /// Messages are set and cleared appropriately.
        /// This method reads analysis input from standard location, performs calculation, calculates
        /// analysis results and stores them to the standard location.</summary>
        public virtual void ServerAnalyse()
        {
            lock (Lock)
            {
                if (!IsAnInputReady())
                {
                    //Console.WriteLine();
                    //Console.WriteLine("WARNING: Neural approximation input data is not ready.");
                    //Console.WriteLine();
                    throw new InvalidOperationException("Analysis input ready flag is not set.");
                }
                if (!IsAnBusy())
                    SetAnBusy();
                if (IsAnResultsReady())
                    ClearAnResultsReady();
                AnalysisResults anRes = AnResults;
                anRes.ResetResults();
                if (AnFormatJson && File.Exists(AnInJsonPath))
                    ReadAnalysisInputJson(AnInJsonPath, ref anRes);
                else if (AnFormatMath && File.Exists(AnInMathPath))
                    ReadAnalysisInputMath(AnInMathPath, ref anRes);
                else if (AnFormatXml && File.Exists(AnInXmlPath))
                    ReadAnalysisInputXml(AnInXmlPath, ref anRes);
                // Calculate analysis results:
                Analysis.Analyse(anRes);
                if (AnFormatJson)
                    WriteAnalysisOutputJson(AnOutJsonPath, anRes);
                if (AnFormatMath)
                    WriteAnalysisOutputMath(AnOutMathPath, anRes);
                if (AnFormatXml)
                    WriteAnalysisOutputXml(AnOutXmlPath, anRes);
                SetAnResultsReady();
                // ClearNeuralBusy();  // this should be done by client!!
            }
        }


        #endregion OperationServer


    }

}