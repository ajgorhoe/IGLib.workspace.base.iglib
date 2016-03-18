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

    /// <summary>Loadable script base class for the shell application that includes tols for definition
    /// of optiimization problems, definition of vector response functions, a couple of interfaces with 
    /// simulation programs, etc.
    /// AGREEMENTS:
    /// The first argument to initialization and executable method is working directory where
    /// data between different applications is exchanged.
    /// If overrigding the <see cref="InitializeThis"/>(...) method, call the base class' method first, 
    /// and keep the agreement that the first argument must be the working directory.
    /// When implementing the executable (<see cref="RunThis"/>(...)) method, its first argument should also be
    /// the working directory (the same as for initialization). This is to enable use of the
    /// class in scenarios where initialization and execution arguments must be the same.</summary>
    /// $A Igor Aug11; 
    public abstract class LoadableScriptShellBase : LoadableScriptOptShellBaseControllable,  
        ILoadableScriptC, ILockable
    {

        /// <summary>Constructor.</summary>
        public LoadableScriptShellBase()
            : base()
        {  }

        /// <summary>Inializes the current script object.</summary>
        /// <param name="arguments">Initialization arguments.
        /// The first argument must be the working directory path.</param>
        protected override void InitializeThis(string[] arguments)
        {
            base.InitializeThis(arguments);  // this sets the working directory to the first argument
        }


        #region SimulationInterface

        protected IResponseEvaluatorVectorSimple _simulator;


        /// <summary>Simulator that is used to calculate vector response.</summary>
        public abstract IResponseEvaluatorVectorSimple Simulator
        {
            get;
            protected set;
        }

        private bool _simulatorSuppressOutput = false;

        /// <summary>A flag indicating whether simulators should suppress output or not, default is false.
        /// <para>Current value of this flag affects newly created simulators (usually this is implemented in the <see cref="CreateNewSimulator"/> method).</para>
        /// <para>Not all simulator interfaces support suppressing output, therefore for some simulator this flag has no effect.</para></summary>
        public virtual bool SimulatorSuppressOutput
        {
            get { return _simulatorSuppressOutput; }
            set { _simulatorSuppressOutput = value; }
        }

        /// <summary>Creates a new simulator that uses the specified simulator directory, and stores it to the specified variable.
        /// <para>This method always creates simulator anew, regardless of whether the variable to hold it is already assigned.</para></summary>
        /// <param name="simulatorPath">Path to the simulator directory.</param>
        /// <param name="simuator">Variable where the created simulator is stored.</param>
        /// <remarks>In many scripts, it is this method that sould be overridden in order to sort out everything with respect to
        /// simulator creation.</remarks>
        protected virtual void CreateNewSimulator(string simulatorPath, ref IResponseEvaluatorVectorSimple simulator)
        {
            throw new NotImplementedException("Creation of new simulator according to the specified simulator directory is not implemented.");
        }

        
        /// <summary>Repairs simulation parameters, if necessary, in such a way that values are consistent with
        /// simuation data (e.g. spacing of nodes).</summary>
        /// <param name="parameters">Vector of parameters to be repaired. Repaired values are stored in the same vector.</param>
        /// <returns>true if parameters were corrected, false otherwise.</returns>
        /// <remarks>This method should be overridden in the derived classes, in the case that reparation can actual take place.
        /// The base class method does nothing (it only returns false).</remarks>
        public virtual bool RepairSimulationParameters(IVector parameters)
        { return false; }


        /// <summary>Calculates vector response by the specified simulator, and stores output values to the specified vector
        /// variable.</summary>
        /// <param name="simulator">Simulator used to calculate the response.</param>
        /// <param name="inputParameters">Vector of input parameters.</returns>
        /// <paramparam name="outputValues">Variable where the calculated response (output values) is stored.</paramparam>
        /// <remarks><para>This method is seldom overridden in derived classes because it does everything it is expected to do.
        /// The method calls the <see cref="RepairSimulationParameters"/> method, which is often overridden in derived classes.
        /// Warning message is written to console if vectr of input parameters had to be repaired.</para></remarks>
        public virtual void SimulatorCalculateResponse(IResponseEvaluatorVectorSimple simulator, 
            IVector inputParameters, ref IVector outputValues)
        {
            bool parametersRepaired = RepairSimulationParameters(inputParameters);
            if (parametersRepaired)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("WARNING: Vector of simulator's input parameters has been repaired.");
                Console.WriteLine();
            }
            simulator.CalculateVectorResponse(inputParameters, ref outputValues);
        }

        /// <summary>Calculates vector response by the main simulator of the script, and stores output values to the specified vector
        /// variable.</summary>
        /// <param name="inputParameters">Vector of input parameters.</returns>
        /// <paramparam name="outputValues">Variable where the calculated response (output values) is stored.</paramparam>
        /// <remarks><para>This method can not be overridden in derived classes, but its overload where simulator is an argument, can be
        /// overridden.</para></remarks>
        public void SimulatorCalculateResponse(IVector parameters, ref IVector outputValues)
        {
            SimulatorCalculateResponse(Simulator, parameters, ref outputValues);
        }


        protected string _projectName;

        /// <summary>Name of the current project, used in some simulation and other interfaces.</summary>
        public virtual string ProjectName
        {
            get { lock(Lock) { return _projectName; } }
            protected set { lock(Lock) { _projectName = value; } }
        }

        protected string _simulationName;

        /// <summary>Name of the current simulation, used in some simulation and other interfaces.</summary>
        public virtual string SimulationName
        {
            get { lock(Lock) { return _simulationName; } }
            protected set { lock(Lock) { _simulationName = value; } }
        }


        #endregion SimulationInterface



        #region StoredScriptSettings

        /// <summary>In methods of this class you will find all the settings that apply to this script.</summary>
        /// <remarks>Before custom application script is archived, settings should be moved </remarks>
        /// $A Igor Feb12;
        protected new class StoredScriptSettings : ApplicationCommandlineBase
        {

            protected override CommandLineApplicationInterpreter CreateInterpreter() { throw new NotImplementedException("Creation of commandline interpreter is not implemented in this class."); }
            public override void TestMain(string[] args) { throw new NotImplementedException(""); }

            public void TestMain_Basic(string[] args)
            {
                // Store script settings in this method!
            }
        }

        #endregion StoredScriptSettings



    } // class LoadableScriptShell


}
