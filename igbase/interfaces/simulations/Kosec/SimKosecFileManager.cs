// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Base class for interface with simulator of G. Kosec for convection problems in a cave with obstacles.</summary>
    public abstract class SimKosecFileManagerConvBase: SimKosecFileManagerBase, 
        IResponseEvaluatorVectorSimple, ILockable
    {

        #region Construction

        /// <summary>Constructor.</summary>
        /// <param name="dataDirectory">Base directory for simulation.</param>
        public SimKosecFileManagerConvBase(string dataDirectory)
            : base(dataDirectory)
        {
            Init();
        }

        /// <summary>Constructor.</summary>
        /// <param name="dataDirectory">Base directory for simulation.</param>
        /// <param name="thickness">Obstacle thickness.</param>
        /// <param name="obstacleLength">Obstacle length.</param>
        public SimKosecFileManagerConvBase(string dataDirectory, double thickness, double obstacleLength)
            : base(dataDirectory)
        {
            Init();
            this.ObstacleThickness = thickness;
            this.ObstacleLength = obstacleLength;
        }

        /// <summary>Constructor.</summary>
        /// <param name="dataDirectory">Base directory for simulation.</param>
        /// <param name="thickness">Obstacle thickness.</param>
        public SimKosecFileManagerConvBase(string dataDirectory, double thickness)
            : base(dataDirectory)
        {
            Init();
            this.ObstacleThickness = thickness;
        }

        /// <summary>Initializes internal variables. Called at the beginning of all constructors.
        /// <para>To be overridden in derived classes!</para></summary>
        protected override void Init()
        {
            ObstacleThickness = 0.1;
            ObstacleLength = 0.4;
            SimulationTime = 20.0;
            MinX = 0;
            MaxX = 1;
            MinY = 0;
            MaxY = 1;
            NumX = int.Parse(GetInputFieldValue("NumX0"));
            NumY = int.Parse(GetInputFieldValue("NumY0"));
            DX = (MaxX - MinX) / (double) ((NumX) - 1);
            DY = (MaxY - MinY) / (double) ((NumY) - 1);
        }

        #endregion Construction

        protected int _numParameters = 0;

        /// <summary>Thickness of obstacless.</summary>
        public double ObstacleThickness = 0.1;

        /// <summary>Length of obstacle.</summary>
        public double ObstacleLength = 0.4;

        /// <summary>Simulation time.</summary>
        public double SimulationTime = 20.0;

        /// <summary>Domain upper bound in X direction.</summary>
        public double MaxX = 1;

        /// <summary>Domain upper bound in X direction.</summary>
        public double MaxY = 1;

        /// <summary>Number of nodes in X direction.</summary>
        public int NumX = 0;

        /// <summary>Number of nodes in Y direction.</summary>
        public int NumY = 0;

        /// <summary>Step in X direction.</summary>
        public double DX = 0;
        
        /// <summary>Step in Y direction.</summary>
        public double DY = 0;

        /// <summary>Domain lower bound in X direction.</summary>
        public double MinX = 0;

        /// <summary>Domain lower bound in Y direction.</summary>
        public double MinY = 0;

        /// <summary>Installs data about input fields that can be queried and set in the input file.</summary>
        protected override void InstallInputFields()
        {
            base.InstallInputFields();
            AddInputFieldDefinitions(
                new InputFieldDefinition(56, "Obstacle1:x0", "zacetna pozicija prve ovire"),
                new InputFieldDefinition(57, "Obstacle1:x1", "koncna pozicija prve ovire"),
                new InputFieldDefinition(58, "Obstacle2:x0"),
                new InputFieldDefinition(59, "Obstacle2:x1"),
                new InputFieldDefinition(60, "Obstacle3:x0"),
                new InputFieldDefinition(61, "Obstacle3:x1"),
                new InputFieldDefinition(62, "Thickness", "debelina ovir")
                );
        }

        /// <summary>Installs definitions of default values of input fields that are automatically set
        /// before running the simulation, and eventually definitios for mappings between (optimization) input parameters
        /// and input fields in input file. 
        /// <para>The latter are eventually used when for each input (optimization) parameter
        /// there exists a field that corresponds to that parameter. Many times this is not true because a single optimization
        /// parameters can affect a whole set of input fields.</para>
        /// <para>When running simulator by calling <see cref="CalculateVectorResponse"/>, input is prepared in 
        /// the following order: First default parameters are set, then automatic mappings are performed (if any are defined)
        /// and finally the manual mappings are performed by calling <see cref="UpdateInputParametersManual"/>, thus
        /// manually defined parameter mapping overrides all others when defined.</para></summary>
        protected override void InstallInputMappings()
        {
            OptimizationParameterDefinitions.Clear();
            // AddOptimizationParameterDefinitions( );
            DefaultInputValues.Clear();
            AddDefaultInputValues(
                    new InputFieldDefinition(56, "Obstacle1:x0", "zacetna pozicija prve ovire", 0.ToString()),
                    new InputFieldDefinition(57, "Obstacle1:x1", "koncna pozicija prve ovire", 0.ToString()),
                    new InputFieldDefinition(58, "Obstacle2:x0", "", 0.ToString()),
                    new InputFieldDefinition(59, "Obstacle2:x1", "", 0.ToString()),
                    new InputFieldDefinition(60, "Obstacle3:x0", "", 0.ToString()),
                    new InputFieldDefinition(61, "Obstacle3:x1", "", 0.ToString()),
                    //new InputFieldDefinition(62, "Thickness", "debelina ovir", Thickness.ToString()),
                    new InputFieldDefinition(62, "Thickness", "debelina ovir", ObstacleThickness.ToString()),
                    new InputFieldDefinition(8, "time", "cas simulacije", SimulationTime.ToString() )
                );
        }


        ///// <summary>Gets the current values of input parameters form the input file.
        ///// This method must be overridden in derived concrete classes.</summary>
        ///// <param name="inputParameters">Vector object where current values of input parameters are stored.</param>
        //protected override void GetInputParametersManual(ref IVector inputParameters)
        //{
        //    // TODO: Remove this method from the current class and implement it in other classes.

        //    throw new NotImplementedException();
        //}

        
        ///// <summary>Prepares current values of input parameters in the simulation input thet
        ///// will be written to the simulation input file.
        ///// This method must be overridden in derived concrete classes.
        ///// Only mappings form input parameters to simulation input that are manually defined must 
        ///// be performed by this function, since automatic mappings are already included in functions
        ///// such as <see cref="WriteInputParameters"/>.</summary>
        ///// <param name="inputParameters">Vector of input (optimization) parameter values to be set.</param>
        //protected override void UpdateInputParametersManual(IVector inputParameters);

    } // class SimKosecFileManagerConvBase


    /// <summary>Interface with simulator of G. Kosec for convection problems in a cave with 2 obstacles.</summary>
    public class SimKosecFileManagerConv2: SimKosecFileManagerConvBase, 
        IResponseEvaluatorVectorSimple, ILockable
    {

        #region Construction

        public SimKosecFileManagerConv2(string dataDirectory)
            : base(dataDirectory)
        {  }  // SimKosecFileManagerConv2(string)
        
        /// <summary>Constructor.</summary>
        /// <param name="dataDirectory">Base directory for simulation.</param>
        /// <param name="thickness">Obstacle thickness.</param>
        /// <param name="obstacleLength">Obstacle length.</param>
        public SimKosecFileManagerConv2(string dataDirectory, double thickness, double obstacleLength)
            : base(dataDirectory, thickness, obstacleLength)
        {  }

        /// <summary>Constructor.</summary>
        /// <param name="dataDirectory">Base directory for simulation.</param>
        /// <param name="thickness">Obstacle thickness.</param>
        public SimKosecFileManagerConv2(string dataDirectory, double thickness)
            : base(dataDirectory, thickness)
        {  }

        
        /// <summary>Initializes internal variables. Called at the beginning of all constructors.
        /// <para>To be overridden in derived classes!</para></summary>
        protected override void Init()
        {
            base.Init();
            NumInputParameters = 2;
            NumOutputValues = 2;
            IBoundingBox bounds = new BoundingBox(2);
            bounds.SetBounds(0, MinX+ 0.1 * (MaxX - MinX), MinX+0.9*(MaxX - MinX) - ObstacleLength);
            bounds.SetBounds(1, MinX+ 0.1 * (MaxX - MinX), MinX+0.9*(MaxX - MinX) - ObstacleLength);
        }

        #endregion Construction

        #region Override


        /// <summary>Repairs simulation parameters, if necessary, in such a way that values are consistent with
        /// simuation data (e.g. spacing of nodes).</summary>
        /// <param name="parameters">Vector of parameters to be repaired. Repaired values are stored in the same vector.</param>
        /// <returns>true if parameters were corrected, false otherwise.</returns>
        public override bool RepairInputParameters(IVector parameters)
        {
            SimKosecFileManagerConvBase sim = this ;
            int dim = 2;
            bool ret = false;
            if (parameters == null)
                throw new ArgumentException("Vector of parameters is null.");
            if (parameters.Length != dim)
                throw new ArgumentException("Wrong dimension of vector of parameters: " + parameters.Length
                    + " instead of " + dim + ".");
            double p0 = parameters[0];
            p0 = sim.MinX + Math.Round((p0 - sim.MinX) / sim.DX) * sim.DX;
            if (p0 != parameters[0])
            {
                ret = true;
                parameters[0] = p0;
            }
            double p1 = parameters[1];
            p1 = sim.MinY + Math.Round((p1 - sim.MinY) / sim.DY) * sim.DY;
            if (p1 != parameters[1])
            {
                ret = true;
                parameters[1] = p1;
            }
            return ret;
        }


        /// <summary>Gets the current values of input parameters form the input file.
        /// This method must be overridden in derived concrete classes.</summary>
        /// <param name="inputParameters">Vector object where current values of input parameters are stored.</param>
        protected override void GetInputParametersManual(ref IVector inputParameters)
        {
            if (inputParameters == null)
                inputParameters = new Vector(NumInputParameters);
            if (inputParameters.Length != NumInputParameters)
                inputParameters = new Vector(NumInputParameters);
            double x1_1 = double.Parse(GetInputFieldValue("Obstacle1:x0"));
            double x1_2 = double.Parse(GetInputFieldValue("Obstacle1:x1"));
            double x2_1 = double.Parse(GetInputFieldValue("Obstacle2:x0"));
            double x2_2 = double.Parse(GetInputFieldValue("Obstacle2:x1"));
            inputParameters[0] = x1_1;
            inputParameters[1] = x2_1;
        }

        /// <summary>Prepares current values of input parameters in the simulation input thet
        /// will be written to the simulation input file.
        /// This method must be overridden in derived concrete classes.
        /// Only mappings form input parameters to simulation input that are manually defined must 
        /// be performed by this function, since automatic mappings are already included in functions
        /// such as <see cref="WriteInputParameters"/>.</summary>
        /// <param name="inputParameters">Vector of input (optimization) parameter values to be set.</param>
        protected override void UpdateInputParametersManual(IVector inputParameters)
        {
            if (inputParameters == null)
                throw new ArgumentException("Vector of input parameters not specified (null reference).");
            else if (inputParameters.Length != NumInputParameters)
                throw new ArgumentException("Dimension of input parameters is different than " + NumInputParameters + ".");
            double x1_1 = inputParameters[0];
            double x2_1 = inputParameters[1];
            double x1_2 = x1_1 + ObstacleLength;
            double x2_2 = x2_1 + ObstacleLength;
            if (x1_1 < MinX)
                throw new ArgumentException("x1_1 < " + MinX + ".");
            if (x1_2 > MaxX)
                throw new ArgumentException("x1_2 < " + MaxX + ".");
            if (x2_1 < MinX)
                throw new ArgumentException("x2_1 < " + MinX + ".");
            if (x2_2 > MaxX)
                throw new ArgumentException("x2_2 < " + MaxX + ".");
            SetInputFieldValue("Obstacle1:x0", x1_1.ToString());
            SetInputFieldValue("Obstacle1:x1", x1_2.ToString());
            SetInputFieldValue("Obstacle2:x0", x2_1.ToString());
            SetInputFieldValue("Obstacle2:x1", x2_2.ToString());
        }


        #endregion Override


    } // class SimKosecFileManagerConv2


    /// <summary>Interface with simulator of G. Kosec for convection problems in a cave with 3 obstacles.</summary>
    public class SimKosecFileManagerConv3 : SimKosecFileManagerConvBase,
        IResponseEvaluatorVectorSimple, ILockable
    {

        #region Construction


        public SimKosecFileManagerConv3(string dataDirectory)
            : base(dataDirectory)
        {  } // SimKosecFileManagerConv2(string)

        #endregion Construction


        #region Override

        protected new int NumInputParameters = 3;

        /// <summary>Repairs simulation parameters, if necessary, in such a way that values are consistent with
        /// simuation data (e.g. spacing of nodes).</summary>
        /// <param name="parameters">Vector of parameters to be repaired. Repaired values are stored in the same vector.</param>
        /// <returns>true if parameters were corrected, false otherwise.</returns>
        public override bool RepairInputParameters(IVector parameters)
        {
            throw new NotImplementedException("Not yet implemented for type " + this.GetType().Name);
            //SimKosecFileManagerConvBase sim = this;
            //int dim = 2;
            //bool ret = false;
            //if (parameters == null)
            //    throw new ArgumentException("Vector of parameters is null.");
            //if (parameters.Length != dim)
            //    throw new ArgumentException("Wrong dimension of vector of parameters: " + parameters.Length
            //        + " instead of " + dim + ".");
            //double p0 = parameters[0];
            //p0 = sim.MinX + Math.Round((p0 - sim.MinX) / sim.DX) * sim.DX;
            //if (p0 != parameters[0])
            //{
            //    ret = true;
            //    parameters[0] = p0;
            //}
            //double p1 = parameters[1];
            //p1 = sim.MinY + Math.Round((p1 - sim.MinY) / sim.DY) * sim.DY;
            //if (p1 != parameters[1])
            //{
            //    ret = true;
            //    parameters[1] = p1;
            //}
            //return ret;
        }

        /// <summary>Prepares current values of input parameters in the simulation input thet
        /// will be written to the simulation input file.
        /// This method must be overridden in derived concrete classes.
        /// Only mappings form input parameters to simulation input that are manually defined must 
        /// be performed by this function, since automatic mappings are already included in functions
        /// such as <see cref="WriteInputParameters"/>.</summary>
        /// <param name="inputParameters">Vector of input (optimization) parameter values to be set.</param>
        protected override void UpdateInputParametersManual(IVector inputParameters)
        {
            throw new NotImplementedException();
            //if (inputParameters == null)
            //    throw new ArgumentException("Vector of input parameters not specified (null reference).");
            //else if (inputParameters.Length != NumInputParameters)
            //    throw new ArgumentException("Dimension of input parameters is different than " + NumInputParameters + ".");
            //double x1_1 = inputParameters[0];
            //double x2_1 = inputParameters[1];
            //double x3_1 = inputParameters[2];
            //double x1_2 = x1_1 + ObstacleLength;
            //double x2_2 = x2_1 + ObstacleLength;
            //double x3_2 = x3_1 + ObstacleLength;
            //if (x1_1 < MinX)
            //    throw new ArgumentException("x1_1 < " + MinX + ".");
            //if (x1_2 > MaxX)
            //    throw new ArgumentException("x1_2 < " + MaxX + ".");
            //if (x2_1 < MinX)
            //    throw new ArgumentException("x2_1 < " + MinX + ".");
            //if (x2_2 > MaxX)
            //    throw new ArgumentException("x2_2 < " + MaxX + ".");
            //if (x3_1 < MinX)
            //    throw new ArgumentException("x3_1 < " + MinX + ".");
            //if (x3_2 > MaxX)
            //    throw new ArgumentException("x3_2 < " + MaxX + ".");
            //SetInputFieldValue("Obstacle1:x0", x1_1.ToString());
            //SetInputFieldValue("Obstacle1:x1", x1_2.ToString());
            //SetInputFieldValue("Obstacle2:x0", x2_1.ToString());
            //SetInputFieldValue("Obstacle2:x1", x2_2.ToString());
        }


        /// <summary>Gets the current values of input parameters form the input file.
        /// This method must be overridden in derived concrete classes.</summary>
        /// <param name="inputParameters">Vector object where current values of input parameters are stored.</param>
        protected override void GetInputParametersManual(ref IVector inputParameters)
        {
            throw new NotImplementedException();
            //if (inputParameters == null)
            //    inputParameters = new Vector(NumInputParameters);
            //if (inputParameters.Length != NumParameters)
            //    inputParameters = new Vector(NumInputParameters);
            //double x1_1 = double.Parse(GetInputFieldValue("Obstacle1:x0"));
            //double x1_2 = double.Parse(GetInputFieldValue("Obstacle1:x1"));
            //double x2_1 = double.Parse(GetInputFieldValue("Obstacle2:x0"));
            //double x2_2 = double.Parse(GetInputFieldValue("Obstacle2:x1"));
            //double x3_1 = double.Parse(GetInputFieldValue("Obstacle2:x0"));
            //double x3_2 = double.Parse(GetInputFieldValue("Obstacle2:x1"));
            //inputParameters[0] = x1_1;
            //inputParameters[1] = x2_1;
            //inputParameters[2] = x3_1;
        }

        #endregion Override


    } // class SimKosecFileManagerConv3

}
