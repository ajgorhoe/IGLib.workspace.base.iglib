// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{



    /// <summary>File analysis client.
    /// Passes direct analysis request to the server and gets analysis results from it.</summary>
    /// <remarks>WARNING:
    /// This module is taken from Dragonfly opt. server and adapted for purpose of some
    /// projects. If necessary to further develop, synchronize (& possibly merge) with
    /// Dragonfly, otherwise there will be problems with consistent development of both branches.
    /// WARNING:
    /// Only a part of file analysis client is taken from Dragonfly, be careful with sync. (the best
    /// way is to do modifications in Dragonfly's code and then transfer them to IGLib).
    /// </remarks>
    /// $A Igor jul08 Mar11;
    public abstract class OptFileAnalysisClient: AnalysisBase, IAnalysis, ILockable
    {

        /// <summary>Prevent calling argument-less constructor.</summary>
        private OptFileAnalysisClient() { }

        /// <summary>Constructs optimization file client.</summary>
        /// <param name="directoryPath">Directory where data exchange and message files are located.</param>
        public OptFileAnalysisClient(string directoryPath)
        {
            _fileManager = CreateOptFileManager(directoryPath);
        }

        /// <summary>Creates and returns an appropriate file manager for optimization file client/server.
        /// This method must be overridden in the derived classes.</summary>
        /// <param name="directoryPath">Directory where data exchange and message files are located.</param>
        protected virtual OptFileManager CreateOptFileManager(string directoryPath)
        { return new OptFileManager(directoryPath); }


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



        OptFileManager _fileManager;

        /// <summary>Optimization client/server file manager used by the current analysis server.</summary>
        public OptFileManager FileManager
        {
            get { lock (Lock) { return _fileManager; } }
            protected set { lock (Lock) { _fileManager = value; } }
        }


        private AnalysisResults _analysisPoint;

        /// <summary>Last analysis request or results.</summary>
        public AnalysisResults AnalysisPoint
        {
            get { lock (Lock) { return _analysisPoint; } }
            protected set { lock (Lock) { _analysisPoint = value; } }
        }


        ///// <summary>Direct analysis for optimization procedure.
        ///// Sends analysis request to the analysis file server, waits for the server to serve results, and
        ///// obtains results and stores them to the specified object.</summary>
        ///// <param name="analysisResults">Analysis request and results object. On call, this object must contain specification
        ///// of the analysis request. On exit, results of direct analysis are stored in this object.</param>
        //public void Analyse(IAnalysisResults analysisResults)
        //{
        //}


    }  // class OptFileAnalysisClient

}




