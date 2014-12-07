// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// CLASSES FOR DATA TRANSFER OBJECTS THAT FACILITATE SERIALIZATION OF (OPTIMIZATION) ANALYSIS RESULTS.

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using IG.Lib;

namespace IG.Num
{

    // REMARK:
    // DTOs can usually contain only rough data without any logic, therefore it is probably unnecessary to 
    // have private variables and public properties. Consider changing all properties to simple variables 
    // and deleting private variables behind these properties.


    /// <summary>DTO (Data Transfer Objects) for storing contents of direct analysis request (in optimization problems).</summary>
    /// $A Igor Aug10;
    public class AnalysisRequestDto : SerializationDtoBase<AnalysisResults, AnalysisResults>
    {

        #region Construction

        /// <summary>Default constructor.</summary>
        public AnalysisRequestDto()
        {  }

        #endregion Construction

        #region Data

        private int _numParameters = 1;

        private int _numObjectives = 1;

        private int _numConstraints = 0;

        private int _numEqualityConstraints = 0;

        private VectorDtoBase _parameters;

        private bool _reqObjective = true;

        private bool _reqConstraints = true;

        private bool _reqObjectiveGradient = false;

        private bool _reqConstraintGradients = false;

        private bool _reqObjectiveHessian = false;

        private bool _reqConstraintHessians = false;

        /// <summary>Number of parameters of an optimization problem.</summary>
        public int NumParameters
        { get { return _numParameters; } set { _numParameters = value; } }

        /// <summary>Number of objective functions (usually 1, always less or equal to 1 - 
        /// since this is not meant for multiobjective optimization).</summary>
        public int NumObjectives
        { get { return _numObjectives; } set { _numObjectives = value; } }

        /// <summary>Number of constraint functions in the optimization problem.</summary>
        public int NumConstraints
        { get { return _numConstraints; } set { _numConstraints = value; } }

        /// <summary>Number of equality constraints.</summary>
        public int NumEqualityConstraints
        { get { return _numEqualityConstraints; } set { _numEqualityConstraints = value; } }

        /// <summary>Vector of parameters at which response functons are requested.</summary>
        public VectorDtoBase Parameters
        { get { return _parameters; } set { _parameters = value; } }

        /// <summary>Flag indicating whether calculation of objective function is required or not.</summary>
        public bool RequestedObjective
        { get { return _reqObjective; } set { _reqObjective = value; } }

        /// <summary>Flag indicating whether calculation of constraint functions is required or not.</summary>
        public bool RequestedConstraints
        { get { return _reqConstraints; } set { _reqConstraints = value; } }

        /// <summary>Flag indicating whether calculation of objective function gradient is required or not.</summary>
        public bool RequestedObjectiveGradient
        { get { return _reqObjectiveGradient; } set { _reqObjectiveGradient = value; } }

        /// <summary>Flag indicating whether calculation of constraint function gradients is required or not.</summary>
        public bool RequestedConstraintGradients
        { get { return _reqConstraintGradients; } set { _reqConstraintGradients = value; } }

        /// <summary>Flag indicating whether calculation of objective function Hessian is required or not.</summary>
        public bool RequestedObjectiveHessian
        { get { return _reqObjectiveHessian; } set { _reqObjectiveHessian = value; } }

        /// <summary>Flag indicating whether calculation of constraint function Hessians is required or not.</summary>
        public bool RequestedConstraintHessians
        { get { return _reqConstraintHessians; } set { _reqConstraintHessians = value; } }

        #endregion Data


        #region Operation

        /// <summary>Creates and returns a new analysis request object.</summary>
        public override AnalysisResults CreateObject()
        {
            return new AnalysisResults();
        }

        /// <summary>Copies data to the current DTO from an analysis results object.</summary>
        /// <param name="anres">Analysis results object from which data is copied.</param>
        protected override void CopyFromPlain(AnalysisResults anres)
        {
            if (anres == null)
            {
                SetNull(true);
            }
            else
            {
                SetNull(false);
                this.NumParameters = anres.NumParameters;
                this.NumObjectives = anres.NumObjectives;
                this.NumConstraints = anres.NumConstraints;
                this.NumEqualityConstraints = anres.NumEqualityConstraints;
                if (anres.Parameters == null)
                    this.Parameters = null;
                else
                {
                    this.Parameters = new VectorDtoBase();
                    this.Parameters.CopyFrom(anres.Parameters);
                }
                this.RequestedObjective = anres.ReqObjective;
                this.RequestedConstraints = anres.ReqConstraints;
                this.RequestedObjectiveGradient = anres.ReqObjectiveGradient;
                this.RequestedConstraintGradients = anres.ReqConstraintGradients;
                this.RequestedObjectiveHessian = anres.ReqObjectiveHessian;
                this.RequestedConstraintHessians = anres.ReqConstraintHessians;
            }
        }

        /// <summary>Copies data from the current DTO to a vector object.</summary>
        /// <param name="anres">Analysis results object that data is copied to.</param>
        protected override void CopyToPlain(ref AnalysisResults anres)
        {
            if (GetNull())
                anres = null;
            else
            {
                anres = new AnalysisResults();
                anres.NumParameters = NumParameters;
                anres.NumObjectives = NumObjectives;
                anres.NumConstraints = NumConstraints;
                anres.NumEqualityConstraints = NumEqualityConstraints;
                if (Parameters == null)
                    anres.Parameters = null;
                else
                {
                    IVector vec = null;
                    Parameters.CopyTo(ref vec);
                    anres.SetParametersReference(vec);
                }
                anres.ReqObjective = RequestedObjective;
                anres.ReqConstraints = RequestedConstraints;
                anres.ReqObjectiveGradient = RequestedObjectiveGradient;
                anres.ReqConstraintGradients = RequestedConstraintGradients;
                anres.ReqObjectiveHessian = RequestedObjectiveHessian;
                anres.ReqConstraintHessians = RequestedConstraintHessians;

                anres.Calculated = false;
            }
        }

        #endregion Operation

    }  // class AnalysisRequestDTO



    /// <summary>DTO (Data Transfer Objects) for storing contents of direct analysis request (in optimization problems).</summary>
    /// $A Igor Aug10;
    public class AnalysisResultsDto : AnalysisRequestDto
    {

        #region Construction

        /// <summary>Default constructor.</summary>
        public AnalysisResultsDto()
            : base()
        {  }

        #endregion Construction

        #region Data

        protected int _errorCode = 0;

        protected string _errorString = null;

        private bool _calculatedObjective = true;

        private bool _calculatedConstraints = true;

        private bool _calculatedObjectiveGradient = false;

        private bool _calculatedConstraintGradients = false;

        private bool _calculatedObjectiveHessian = false;

        private bool _calculatedConstraintHessians = false;


        private double _objective;

        private double[] _constraints;

        private VectorDtoBase _objectiveGratient;

        private VectorDtoBase[] _constraintGradients;

        private MatrixDtoBase _objectiveHessian;

        private MatrixDtoBase[] _constraintHessians;

        /// <summary>Error code.
        ///   0 - everything is OK.
        ///   negative value - something went wrong.</summary>
        public virtual int ErrorCode
        { get { return _errorCode; } set { _errorCode = value; } }

        /// <summary>Error string indicating what went wrong.</summary>
        public virtual String ErrorString
        { get { return _errorString; } set { _errorString = value; } }

        /// <summary>Flag indicating whether calculation of objective function is required or not.</summary>
        public bool CalculatedObjective
        { get { return _calculatedObjective; } set { _calculatedObjective = value; } }

        /// <summary>Flag indicating whether calculation of constraint functions is required or not.</summary>
        public bool CalculatedConstraints
        { get { return _calculatedConstraints; } set { _calculatedConstraints = value; } }

        /// <summary>Flag indicating whether calculation of objective function gradient is required or not.</summary>
        public bool CalculatedObjectiveGradient
        { get { return _calculatedObjectiveGradient; } set { _calculatedObjectiveGradient = value; } }

        /// <summary>Flag indicating whether calculation of constraint function gradients is required or not.</summary>
        public bool CalculatedConstraintGradients
        { get { return _calculatedConstraintGradients; } set { _calculatedConstraintGradients = value; } }

        /// <summary>Flag indicating whether calculation of objective function Hessian is required or not.</summary>
        public bool CalculatedObjectiveHessian
        { get { return _calculatedObjectiveHessian; } set { _calculatedObjectiveHessian = value; } }

        /// <summary>Flag indicating whether calculation of constraint function Hessians is required or not.</summary>
        public bool CalculatedConstraintHessians
        { get { return _calculatedConstraintHessians; } set { _calculatedConstraintHessians = value; } }


        /// <summary>Value of the objective function.</summary>
        public Double Objective
        { get { return _objective; } set { _objective = value; } }

        public double[] Constraints
        { get { return _constraints; } set { _constraints = value; } }

        /// <summary>Gradient of the objective function.</summary>
        public VectorDtoBase ObjectiveGradient
        { get { return _objectiveGratient; } set { _objectiveGratient = value; } }

        /// <summary>Gradients of constraint functions.</summary>
        public VectorDtoBase[] ConstraintGradients
        { get { return _constraintGradients; } set { _constraintGradients = value; } }

        /// <summary>Hessian of the objective function.</summary>
        public MatrixDtoBase ObjectiveHessian
        { get { return _objectiveHessian; } set { _objectiveHessian = value; } }

        /// <summary>Hessians of constraint functions.</summary>
        public MatrixDtoBase[] ConstraintHessians
        { get { return _constraintHessians; } set { _constraintHessians = value; } }


        #endregion Data

        #region Operation


        /// <summary>Copies data to the current DTO from an analysis results object.</summary>
        /// <param name="anres">Analysis results object from which data is copied.</param>
        protected override void CopyFromPlain(AnalysisResults anres)
        {
            base.CopyFromPlain(anres);

            if (anres != null)
            {
                this.ErrorCode = anres.ErrorCode;
                this.ErrorString = anres.ErrorString;
                this.CalculatedObjective = anres.CalculatedObjective;
                this.CalculatedConstraints = anres.CalculatedConstraints;
                this.CalculatedObjectiveGradient = anres.CalculatedObjectiveGradient;
                this.CalculatedConstraintGradients = anres.CalculatedConstraintGradients;
                this.CalculatedObjectiveHessian = anres.CalculatedObjectiveHessian;
                this.CalculatedConstraintHessians = anres.CalculatedConstraintHessians;

                Objective = anres.Objective;
                if (anres.Constraints == null)
                    this.Constraints = null;
                else
                {
                    int num = anres.Constraints.Count;
                    this.Constraints = new double[num];
                    for (int i = 0; i < num; ++i)
                        this.Constraints[i] = anres.Constraints[i];
                }
                if (anres.ObjectiveGradient == null)
                    this.ObjectiveGradient = null;
                else
                {
                    this.ObjectiveGradient = new VectorDtoBase();
                    this.ObjectiveGradient.CopyFrom(anres.ObjectiveGradient);
                }
                if (anres.ConstraintGradients == null)
                    this.ConstraintGradients = null;
                else
                {
                    int num = anres.ConstraintGradients.Count;
                    this.ConstraintGradients = new VectorDtoBase[num];
                    for (int i = 0; i < num; ++i)
                    {
                        if (anres.ConstraintGradients[i] == null)
                            this.ConstraintGradients[i] = null;
                        else
                        {
                            this.ConstraintGradients[i] = new VectorDtoBase();
                            this.ConstraintGradients[i].CopyFrom(anres.ConstraintGradients[i]);
                        }
                    }
                }
                
                if (anres.ObjectiveHessian == null)
                    this.ObjectiveHessian = null;
                else
                {
                    this.ObjectiveHessian = new MatrixDtoBase();
                    this.ObjectiveHessian.CopyFrom(anres.ObjectiveHessian);
                }
                if (anres.ConstraintHessians == null)
                    this.ConstraintHessians = null;
                else
                {
                    int num = anres.ConstraintHessians.Count;
                    this.ConstraintHessians = new MatrixDtoBase[num];
                    for (int i = 0; i < num; ++i)
                    {
                        if (anres.ConstraintHessians[i] == null)
                            this.ConstraintHessians[i] = null;
                        else
                        {
                            this.ConstraintHessians[i] = new MatrixDtoBase();
                            this.ConstraintHessians[i].CopyFrom(anres.ConstraintHessians[i]);
                        }
                    }
                }
            }
        }

        /// <summary>Copies data from the current DTO to an analysis results object.</summary>
        /// <param name="anres">Analysis results object that data is copied to.</param>
        protected override void CopyToPlain(ref AnalysisResults anres)
        {
            base.CopyToPlain(ref anres);
            if (anres!=null)
            {
                anres.ErrorCode = this.ErrorCode;
                anres.ErrorString = this.ErrorString;

                anres.CalculatedObjective = this.CalculatedObjective;
                anres.CalculatedConstraints = this.CalculatedConstraints;
                anres.CalculatedObjectiveGradient = this.CalculatedObjectiveGradient;
                anres.CalculatedConstraintGradients = this.CalculatedConstraintGradients;
                anres.CalculatedObjectiveHessian = this.CalculatedObjectiveHessian;
                anres.CalculatedConstraintHessians = this.CalculatedConstraintHessians;

                anres.Objective = Objective;
                if (this.Constraints==null)
                    anres.Constraints = null;
                else
                {
                    int num = this.Constraints.Length;
                    List<double> constr = anres.Constraints;
                    Util.ResizeList<double>(ref constr, num,  0.0, true);
                    for (int i = 0; i < num; ++i)
                        constr[i] = this.Constraints[i];
                    anres.Constraints = constr;
                }
                if (this.ObjectiveGradient == null)
                    anres.ObjectiveGradient = null;
                else
                {
                    IVector grad = anres.ObjectiveGradient;
                    this.ObjectiveGradient.CopyTo(ref grad);
                    anres.ObjectiveGradient = grad;
                }
                if (this.ConstraintGradients == null)
                    anres.ConstraintGradients = null;
                else
                {
                    int num = this.ConstraintGradients.Length;
                    List<IVector> anresGradients = anres.ConstraintGradients;
                    Util.ResizeList<IVector>(ref anresGradients, num, null, true);
                    for (int i = 0; i < num; ++i)
                    {
                        if (this.ConstraintGradients[i] == null)
                            anresGradients[i] = null;
                        else
                        {
                            IVector grad = anresGradients[i];
                            this.ConstraintGradients[i].CopyTo(ref grad);
                            anresGradients[i] = grad;
                        }
                    }
                    anres.ConstraintGradients = anresGradients;
                }

                if (this.ObjectiveHessian == null)
                    anres.ObjectiveHessian = null;
                else
                {
                    IMatrix hess = anres.ObjectiveHessian;
                    this.ObjectiveHessian.CopyTo(ref hess);
                    anres.ObjectiveHessian = hess;
                }
                if (this.ConstraintHessians == null)
                    anres.ConstraintHessians = null;
                else
                {
                    int num = this.ConstraintHessians.Length;
                    List<IMatrix> anresHessians = anres.ConstraintHessians;
                    Util.ResizeList<IMatrix>(ref anresHessians, num, null, true);
                    for (int i = 0; i < num; ++i)
                    {
                        if (this.ConstraintHessians[i] == null)
                            anresHessians[i] = null;
                        else
                        {
                            IMatrix hessian = anresHessians[i];
                            this.ConstraintHessians[i].CopyTo(ref hessian);
                            anresHessians[i] = hessian;
                        }
                    }
                    anres.ConstraintHessians = anresHessians;
                }


                // TODO: complete this!
            }
        }


        #endregion Operation


    }  // class AnalysisResultsDTO


}
