// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Lib;
using IG.Num;

namespace IG.Lib
{


    /// <summary>Base class for loadable scripts for optimization problems.
    /// AGREEMENTS:
    /// The first argument to initialization and executable method is working directory where
    /// data between different applications is exchanged.
    /// If overrigding the <see cref="InitializeThis"/>(...) method, call the base class' method first, 
    /// and keep the agreement that the first argument must be the working directory.
    /// When implementing the executable (<see cref="RunThis"/>(...)) method, its first argument should also be
    /// the working directory (the same as for initialization). This is to enable use of the
    /// class in scenarios where initialization and execution arguments must be the same.</summary>
    /// $A Igor Jul09
    public abstract class LoadableScriptOptBase : LoadableScriptBase,
        ILoadableScript, ILockable
    {

        #region Initialization

        /// <summary>Creates the <typeparamref name="LoadableScriptOptBase"/> object.</summary>
        public LoadableScriptOptBase(): base()
        {  }

        /// <summary>Inializes the current script object.</summary>
        /// <param name="arguments">Initialization arguments.
        /// The first argument must be the working directory path.</param>
        protected override void InitializeThis(string[] arguments)
        {
            if (arguments != null)
            {
                if (arguments.Length >= 1)
                    OptimizationDirectory = arguments[0];
            }
        }

        #endregion Initialization


        #region Auxiliary

        protected StopWatch _timer = null;

        protected StopWatch Timer
        {
            get
            {
                if (_timer == null)
                    _timer = new StopWatch();
                return _timer;
            }
        }

        IRandomGenerator _randGen;

        /// <summary>Random generator used by the current object.
        /// <para>Lazy evaluation, created when needed for the first time.</para>
        /// <para>The generator is thread safe and initialized with a time dependent seed.</para></summary>
        public virtual IRandomGenerator Random
        {
            get
            {
                lock (Lock)
                {
                    if (_randGen == null)
                        _randGen = RandomGenerator.CreateThreadSafe();
                    return _randGen;
                }
            }
            protected set { lock (Lock) { _randGen = value; } }
        }

        #endregion Auxiliary


        #region General

        string _optimizationDirectory;

        /// <summary>Whether to check existence of optimization directory when set. Also applies to working directory.</summary>
        protected bool _checkOptimizationDirectoryExistence = true;

        /// <summary>Optimization directory. This directory is a base directory for data used optimization
        /// and neural network - based approximation servers and
        /// for other directories that contain data for specific tasks.</summary>
        public virtual string OptimizationDirectory
        {
            get 
            { 
                lock (Lock) 
                {
                    return _optimizationDirectory; 
                } 
            }
            set
            {
                lock (Lock)
                {
                    _optimizationDirectory = value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        UtilSystem.StandardizeDirectoryPath(ref _optimizationDirectory);
                        if (_checkOptimizationDirectoryExistence)
                            if (!Directory.Exists(value))
                                throw new ArgumentException("Optimization directory does not exist."
                                    + Environment.NewLine + "  directory: " + value
                                    + Environment.NewLine + "  full path: " + Path.GetFullPath(value));
                    }
                }
            }
        }


        string _workingDirectory; 

        /// <summary>Working directory. This directory is a base directory for data used by the script.
        /// It is usually obtained as parent directory of the optimization directory.</summary>
        public virtual string WorkingDirectory
        {
            get
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(_workingDirectory))
                    {
                        string optDir = OptimizationDirectory;
                        if (!string.IsNullOrEmpty(optDir))
                        {
                            UtilSystem.StandardizeDirectoryPath(ref optDir);
                            _workingDirectory = Directory.GetParent(optDir).FullName;
                            //string dirNameOpt = Path.GetDirectoryName(optDir);
                            //DirectoryInfo dirInfo = Directory.GetParent(dirNameOpt);
                            //_workingDirectory = dirInfo.FullName;
                            UtilSystem.StandardizeDirectoryPath(ref _workingDirectory);
                        }
                    }
                    return _workingDirectory;
                }
            }
            set
            {
                lock (Lock)
                {
                    _workingDirectory = value;
                    UtilSystem.StandardizeDirectoryPath(ref _workingDirectory);
                    if (_checkOptimizationDirectoryExistence && (!string.IsNullOrEmpty(value)))
                        if (!Directory.Exists(value))
                            throw new ArgumentException("Working directory does not exist."
                                + Environment.NewLine + "  directory: " + value
                                + Environment.NewLine + "  full path: " + Path.GetFullPath(value));
                }
            }
        }


        #endregion General


        #region OptAnalysis

        /// <summary>Performs direct analysis for optimization problems.
        /// This method must be overridden in derived classes where one wants to have direct analysis defined.</summary>
        /// <param name="anRes"><see cref="IAnalysisResults"/> object where input parameters are loaded on the call, 
        /// and where analysis results are stored after the call.</param>
        /// <remarks>In order to perform the direct analysis, an embedded <see cref="Analysis"/> object is used.
        /// This object's analysis method calls the current <see cref="Analyse"/> method in order to actually
        /// perform the analysis.</remarks>
        public abstract void Analyse(IAnalysisResults anRes);
        // { throw new NotImplementedException("Direct analysis method is not implemented on the scripting object."); }

        private IAnalysis _analysis;

        /// <summary>Direct analysis object used in optimization.
        /// Initialized when first accessed with the embedded class, whose analysis function calls <see cref="Analyse"/>(...).</summary>
        /// <remarks>When getter is first accessed, this property is initialized with an object of the embedded <see cref="AnalysisScript"/>
        /// class that has Script property set to the current loadable script object.
        /// This object calls the <see cref="Analyse"/>(...) method on the currend loadable script object in order to actually perform the 
        /// direct analysis. This method must be overridden in derived concrete classes.
        /// Various properties of the optimization problem such as number of parameters, number of constraints, etc., must 
        /// be accessed through this object.</remarks>
        public virtual IAnalysis Analysis
        {
            get
            {
                lock (Lock)
                {
                    if (_analysis == null)
                        Analysis = new AnalysisScript(this);
                    return _analysis;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    _analysis = value;
                }
            }
        }

        #endregion OptAanlysis


        #region VectorFunctions



        #endregion Vectorfunctions


        #region EmbeddedClasses

        // Classed for objects that are embedded in a loadable script object of type LoadableScriptOptBase.


        public class AnalysisScript: AnalysisBase, IAnalysis
        {

            
            private AnalysisScript() { }

            /// <summary>Direct analysis class used in optimization problems, 
            /// modified such that it is connected with a loadable script of type <seealso cref="LoadableScriptOptBase"/>.</summary>
            /// <param name="script"></param>
            public AnalysisScript(LoadableScriptOptBase script): base()
            {
                this.Script = script;
            }

            public override void Analyse(IAnalysisResults anRes)
            {
                Script.Analyse(anRes);
            }

            private LoadableScriptOptBase _script;

            /// <summary>Loadable script object of type <see cref="LoadableScriptOptBase"/> in which the current object
            /// is embedded (and obtains all data from it).</summary>
            public LoadableScriptOptBase Script
            { get { return _script; }  protected set { _script = value; } }

            public void Test()
            {

            }

        }  // class LoadableScriptOptBase.AnalysisScript



        #endregion EmbeddedClasses


    }  // class LoadableScriptOptBase


    /// <summary>Controllable version of <see cref="LoadableScriptOptBase"/>, implements <see cref="ILoadableScriptC"/></summary>
    public abstract class LoadableScriptOptShellBaseControllable : LoadableScriptOptBase,
        ILoadableScriptC, ILockable
    {
               
        #region Initialization

        /// <summary>Creates the <typeparamref name="LoadableScriptOptBase"/> object.</summary>
        public LoadableScriptOptShellBaseControllable()
            : base()
        {
            InitControllable();
        }

        /// <summary>Inializes the current script object.</summary>
        /// <param name="arguments">Initialization arguments.
        /// The first argument must be the working directory path.</param>
        protected override void InitializeThis(string[] arguments)
        {
            InitControllable();
            if (arguments != null)
            {
                if (arguments.Length >= 1)
                    OptimizationDirectory = arguments[0];
            }
        }

        /// <summary>Initialiyes the control flags of the script.</summary>
        private void InitControllable()
        {
            _loadable = App.LoadableScriptShellIsLoadableS;
            _runnable = App.LoadableScriptShellIsRunnableS;
        }

        #endregion Initialization

        private bool _loadable = false;

        /// <summary>Either or not the script can be dynamically loaded.</summary>
        public bool IsLoadable
        {
            get { return _loadable; }
            set { throw new InvalidOperationException("Property can not be set."); }
        }

        private bool _runnable = false;

        /// <summary>Either or not the script can be run (some scripts only support other tasks).</summary>
        public bool IsRunnable
        {
            get { return _runnable; }
            set { throw new InvalidOperationException("Property can not be set."); }
        }

    }


    /// <summary>Test optimization script cls.</summary>
    /// $A Igor Jul09;
    public class LoadableScriptOptTest
    {


        public void Test()
        {

            LoadableScriptOptBase script = new LoadableScriptOptDerived();

            LoadableScriptOptDerived.AnalysisScript analysis = new LoadableScriptOptBase.AnalysisScript(script);

        }



        public class LoadableScriptOptDerived : LoadableScriptOptBase
        {

            #region ILoadableScript


            protected override void InitializeThis(string[] arguments)
            {
                throw new NotImplementedException();
            }

            protected override string RunThis(string[] arguments)
            {
                throw new NotImplementedException();
            }

            #endregion ILoadableScript

            public override void Analyse(IAnalysisResults anRes)
            {
                throw new NotImplementedException("This direct analysis method is not defined.");
            }
        }



    } // class LoadableScriptOptTest


}

