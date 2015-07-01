// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

using IG.Lib;

namespace IG.Num
{


    /// <summary>Extension methods for analysis results classes.</summary>
    /// $A Igor Apr11;
    public static class AnalysisResultsExtensions
    {

        /// <summary>Returns a string representation of the current analysis results object in a standard IGLib form.
        /// Extension method for IAnalysisResults.</summary>
        /// <param name="anres">Object whose string representation is returned.</param>
        public static string ToString(IAnalysisResults anres)
        {
            return AnalysisResults.ToString(anres);
        }


        /// <summary>Returns a string representation of the current analysis results object in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).
        /// Extension method for IAnalysisResults.</summary>
        /// <param name="anres">Object whose string representation is returned.</param>
        public static string ToStringMath(IAnalysisResults anres)
        {
            return AnalysisResults.ToStringMath(anres);
        }

        
        /// <summary>Returns a string representation of the current analysis request object in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).
        /// Extension method for IAnalysisResults.</summary>
        /// <param name="anres">Object whose string representation is returned.</param>
        public static string ToStringMathRequest(this IAnalysisResults anres)
        {
            return AnalysisResults.ToStringRequestMath(anres);
        }



        /// <summary>Saves the current analysis request to a file.
        /// If the file already exists then it is overwritten.
        /// Extension method for IAnalysisResults.</summary>
        /// <param name="anres">Object from which analysis request is saved.</param>
        /// <param name="inputFilePath">Path to the file where analysis request is stored.</param>
        public static void SaveRequestMath(this IAnalysisResults anres, string filePath)
        {
            AnalysisResults.SaveRequestMath(anres, filePath);
        }


        /// <summary>Saves the current analysis request to a file.
        /// Extension method for IAnalysisResults.</summary>
        /// <param name="anres">Object from which analysis request is saved.</param>
        /// <param name="inputFilePath">Path to the file where analysis request is stored.</param>
        /// <param name="append">If true then contents are appended to the end of the file if the file already exists.</param>
        public static void SaveRequestMath(this IAnalysisResults anres, string filePath, bool append)
        {
            AnalysisResults.SaveRequestMath(anres, filePath, append);
        }


        /// <summary>Saves the current analysis results to a file.
        /// If the file already exists then it is overwritten.
        /// Extension method for IAnalysisResults.</summary>
        /// <param name="anres">Object from which analysis results are saved.</param>
        /// <param name="inputFilePath">Path to the file where analysis results are stored.</param>
        public static void SaveMath(this IAnalysisResults anres, string filePath)
        {
            AnalysisResults.SaveMath(anres, filePath);
        }


        /// <summary>Saves the current analysis results to a file.
        /// Extension method for IAnalysisResults.</summary>
        /// <param name="anres">Object from which analysis results are saved.</param>
        /// <param name="inputFilePath">Path to the file where analysis results are stored.</param>
        /// <param name="append">If true then contents are appended to the end of the file if the file already exists.</param>
        public static void SaveMath(this IAnalysisResults anres, string filePath, bool append)
        {
            AnalysisResults.SaveMath(anres, filePath, append);
        }


    } // static class AnalysisTesultsExtensions



    /// <summary>Single objective optimization analysis results.
    /// Used to transfer parameters input (e.g. vector of parameters, request flags)
    /// to the analysis function and to store analysis output results (e.g. objective and 
    /// constraint functions, their gradients, error codes, and flags indicating what 
    /// has actually been calculated).
    /// REMARKS:
    /// Property CopyReferences specifies whether only references are copied when individial object
    /// fields are assigned and set (when the property is true), or values are actually copied
    /// (when false - deep copy). Each setter method also has the variant that always copies only
    /// the reference (function name appended by "Reference"). This makes possible to avoid duplication
    /// of allocated data and also to avoid having different data with the same references.
    /// AGREEMENTS:
    /// Optimization problem is defined as
    ///     minimize f(x), subject to:
    ///     c_i(x)<=0, i=0...NI-1
    ///     c_j(x)=0,  j=NI...NI+NE-1.
    /// Here x is vector of parameters, f(x) is the objective function, and c_i(x) and c_j(c)
    /// are constraint functions. NI is number of inequality constraints and NE is number of equality constraints.
    /// If there are equality constraints then they are listed after inequality constraints.</summary>
    /// $A Igor Jan08 Jun08;
    public class AnalysisResults : IAnalysisResults
    {

        #region Constructors

        // TODO: VERIFY AND ELABORATE CONSTRUCTORS!!!!

        /// <summary>1 parameter, no constraints, 1 objective function.
        /// No gradients required</summary>
        public AnalysisResults()
        {  }

        /// <summary>1 parameter, no constraints, 1 objective function.</summary>
        /// <param name="reqGradients">Whether gradient of the objective function is required.</param>
        public AnalysisResults(bool reqGradients)
        { }

        /// <summary>Specified number of parameters, 1 objective, no constraints.
        /// No gradients required.</summary>
        /// <param name="numParameters">Number of parameters.</param>
        public AnalysisResults(int numParameters) 
        {
            this.NumParameters = numParameters;
        }

        /// <summary>Specified number of parameters, 1 objective, no constraints.
        /// No gradients required.</summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="reqGradients">Whether gradients are requested.</param>
        public AnalysisResults(int numParameters, bool reqGradients)
        {
            this.NumParameters = numParameters;
            this.ReqObjectiveGradient = true;
        }

        /// <summary>Specified number of parameters and constraints, 1 objective.
        /// No gradients required.</summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        public AnalysisResults(int numParameters, int numConstraints)
        {
            this.NumParameters = numParameters;
            this.NumConstraints = numConstraints;
        }

        /// <summary>Specified number of parameters and constraints, 1 objective.
        /// No gradients required.</summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        /// <param name="reqGradients">Whether calcullation of gradients is required.</param>
        public AnalysisResults(int numParameters, int numConstraints, bool reqGradients)
        {
            this.NumParameters = numParameters;
            this.NumConstraints = numConstraints;
            this.ReqObjective = true;
            this.ReqConstraints = true;
            this.ReqObjectiveGradient = reqGradients;
            this.ReqConstraintGradients = reqGradients;
        }

        #endregion Constructors


        #region InternalData


        protected int _numParameters = 0;

        protected int _numObjectives = 1;

        protected int _numConstraints = 0;

        protected int _numEqualityConstraints = 0;


        protected IVector _parameters;

        protected double _objective;

        protected IVector _objectiveGradient;

        protected IMatrix _objectiveHessian;

        protected List<double> _constraints;

        protected List<IVector> _constraintGradients;

        protected List<IMatrix> _constraintHessians;


        protected bool _copyReferences = false;


        protected bool _reqObjective = false;

        protected bool _reqConstraints = false;

        protected bool _reqObjectiveGradient = false;

        protected bool _reqConstraintGradients = false;

        protected bool _reqObjectiveHessian = false;

        protected bool _reqConstraintHessians = false;

        protected int _errorCode = 0;

        protected string _errorString = null;

        protected bool _calcObjective = false;

        protected bool _calcConstraints = false;

        protected bool _calcObjectiveGradient = false;

        protected bool _calcConstraintGradients = false;

        protected bool _calcObjectiveHessian = false;

        protected bool _calcConstraintHessians = false;



        #endregion InternalData


        #region Characteristics

        // CHARACTERISTICS (DIMENSIONS) OF THE OPTIMIZATION PROBLEM:

        /// <summary>Number of parameters.</summary>
        public virtual int NumParameters 
        { 
            get { return _numParameters; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Number of parameters can not be less than 1.");
                if (value != _numParameters)
                {
                    if (Parameters!=null)
                        if (value!=Parameters.Length)
                            Parameters = null;
                    // TODO: Consider why not recetting the CalculatedObjective and CalculatedConstraints flags?
                    // And why this is necessary in the first place??
                    CalculatedObjectiveGradient = false;
                    CalculatedObjectiveHessian = false;
                    CalculatedConstraintGradients = false;
                    CalculatedConstraintHessians = false;
                }
                _numParameters = value;
            }
        }

        /// <summary>Number of objective functions (normally 1 for this type, but can be 0).</summary>
        public virtual int NumObjectives
        {
            get { return _numObjectives; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Number of objective functions can not be less than 0.");
                if (value > 1)
                    throw new ArgumentException("Numbe of objective functions can not be greater than 1 for this class.");
                //if (value != NumObjectives)
                //{
                //    CalculatedObjective = false;
                //    CalculatedObjectiveGradient = false;
                //    CalculatedObjectiveHessian = false;
                //}
                _numObjectives = value;
            }
        }

        /// <summary>Number of constraints.</summary>
        public virtual int NumConstraints
        {
            get { return _numConstraints; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Number of constraints can not be less than 0.");
                //if (value != _numConstraints)
                //{
                //    CalculatedConstraints = false;
                //    CalculatedConstraintGradients = false;
                //    CalculatedConstraintHessians = false;
                //}
                if (value < _numEqualityConstraints)
                    _numEqualityConstraints = value;
                _numConstraints = value;
            }
        }

        /// <summary>Number of equality constraints.</summary>
        public virtual int NumEqualityConstraints
        {
            get { return _numEqualityConstraints; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Number of equality constraints can not be less than 0.");
                //if (value != NumEqualityConstraints)
                //{
                //    CalculatedConstraints = false;
                //    CalculatedConstraintGradients = false;
                //    CalculatedConstraintHessians = false;
                //}
                if (NumEqualityConstraints > NumConstraints)
                    NumConstraints = NumEqualityConstraints;
                _numEqualityConstraints = value;
            }
        }

        /// <summary>Returns number of inequality constraints.</summary>
        public virtual int NumInequalityConstraints
        {
            get { return (NumConstraints - NumEqualityConstraints); }
        }

        /// <summary>Returns true if the specified constraint is an equality constraint, and false otherwise.</summary>
        /// <param name="which">Index of constraint to determine the type.</param>
        /// <returns>True if the specified constraint is equality constraint, false otherwise.</returns>
        public virtual bool IsEqualityConstraint(int which)
        {
            if (which < 0 || which > NumConstraints)
                throw new IndexOutOfRangeException("Can not establish whether constraint is equality type, index out of range."
                    + Environment.NewLine + "  Constraint index: " + which + ", should be between 0 and "
                    + (NumConstraints-1) + ".");
            if (which >= NumInequalityConstraints)
                return true;
            else
                return false;
        }


        #endregion Characteristics


        #region Operation

        // OPERATION PARAMETERS:

        /// <summary>Indicates whether just references can be copied when setting optimization
        /// parameters or results.
        /// If false then deep copy is always performed.
        /// Default is false.</summary>
        public virtual bool CopyReferences
        {
            get { return _copyReferences; }
            set
            {
                bool previous = _copyReferences;
                _copyReferences = value;
                if (previous == true && value == false)
                {
                    // If we have copied references before, but now we copy values, we must
                    // make copies of all previous objects (we can do this simply by assignment
                    // because assignment will now perform deep copying rather than just reference copying):

                    Copy(this);
 
                }
            }
        }

        #endregion Operation


        #region Parameters

        // OPTIMIZATION PARAMETERS:

        /// <summary>Optimization parameters.
        /// If CopyReferences=true (false by default) then only the reference is copied when assigning.</summary>
        public virtual IVector Parameters 
        {
            get { return _parameters; }
            set
            {
                Calculated = false;
                if (CopyReferences || value == null)
                    _parameters = value;
                else
                    _parameters = value.GetCopy();
                if (value != null)
                    NumParameters = value.Length;
            }
        }

        /// <summary>Returns vector of optimization parameters.</summary>
        public virtual IVector GetParameters()
        { return Parameters; }

        /// <summary>Sets the vector of optimization parameters.
        /// If CopyReferences=true (false by default) then only the reference is copied.</summary>
        /// <param name="value">Value to be assigned.</param>
        public virtual void SetParameters(IVector value)
        { Parameters = value; }

        /// <summary>Sets the vector of optimization parameters.
        /// Only the reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        public virtual void SetParametersReference(IVector reference)
        {
            Calculated = false;
            _parameters = reference;
            if (reference != null)
                NumParameters = reference.Length;
        }

        /// <summary>Returns specific optimization parameter.
        /// Throws exception if not defined or index out of bounds.</summary>
        /// <param name="index">Index of parameter to be returned (counting from 0).</param>
        public virtual double GetParameter(int index)
        { return Parameters[index]; }

        /// <summary>Sets the specified optimization parameter.</summary>
        /// <param name="index">Index of parameter to be set (counting from 0).</param>
        /// <param name="value">Parameter value.</param>
        public virtual void SetParameter(int index, double value)
        {
            AllocateParameters();  // this performs Calculated=false
            Parameters[index] = value;
        }

        #endregion Parameters


        #region Results

        // OPTIMIZATION RESULTS:

        // OBJECTIVE FUNCTION:

        /// <summary>Value of the objective function.</summary>
        public virtual double Objective
        { get { return _objective; } set { _objective = value; } }

        /// <summary>Returns the value of the objective function.</summary>
        public virtual double GetObjective()
        { return Objective; }

        /// <summary>Sets the value of the objective function.</summary>
        /// <param name="value">Value to be assigned to the objective function.</param>
        public virtual void SetObjective(double value)
        { Objective = value; }


        // OBJECTIVE FUNCTION GRADIENT:

        /// <summary>Objective function gradient.
        /// If CopyReferences=true (false by default) then only the reference is copied in assignments.</summary>
        public virtual IVector ObjectiveGradient 
        {
            get { return _objectiveGradient; }
            set
            {
                if (CopyReferences || value == null)
                    _objectiveGradient = value;
                else
                    _objectiveGradient = value.GetCopy();
            }
        }

        /// <summary>Returns the objective function gradient.</summary>
        public virtual IVector GetObjectiveGradient()
        { return ObjectiveGradient; }

        /// <summary>Sets the objective function gradient.
        /// If CopyReferences=true (false by default) then only the reference is copied.</summary>
        /// <param name="value">Value to be assigned.</param>
        public virtual void SetObjectiveGradient(IVector value)
        { ObjectiveGradient = value; }


        /// <summary>Sets the objective function gradient.
        /// Only the reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        public virtual void SetObjectiveGradientReference(IVector reference)
        {
            _objectiveGradient = reference;
        }

        /// <summary>Returns the specified component of the objective function gradient.</summary>
        /// <param name="index">Index of the component.</param>
        public virtual double GetObjectiveGradient(int index)
        { return ObjectiveGradient[index]; }

        /// <summary>Sets the specified component of the objective function gradient.</summary>
        /// <param name="index">Index of objective gradient component to be set.</param>
        /// <param name="value">Value of the objective gradient component.</param>
        public virtual void SetObjectiveGradient(int index, double value)
        {
            AllocateObjectiveGradient();
            ObjectiveGradient[index] = value; 
        }


        // CONSTRAINT FUNCTIONS:

        /// <summary>Constraint function values.
        /// If CopyReferences=true (false by default) then only the list reference is copied in assignments.</summary>
        public virtual List<double> Constraints 
        {
            get { return _constraints; }
            set
            {
                if (CopyReferences || value == null)
                    _constraints = value;
                else
                {
                    AllocateConstraintsList();
                    for (int which = 0; which < NumConstraints; ++which)
                        _constraints[which] = value[which];
                }
            }
        }

        /// <summary>Returns a list of constraint function values.</summary>
        public virtual List<double> GetConstraints()
        { return Constraints; }

        /// <summary>Sets the list of constraint function values.
        /// If CopyReferences=true (false by default) then only the list reference is copied.</summary>
        /// <param name="values">Values of constraint functions.</param>
        public virtual void SetConstraints(List<double> values)
        { Constraints = values; }

        /// <summary>Sets the list of constraint function values.
        /// Only the list reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        public virtual void SetConstraintsReference(List<double> reference)
        { _constraints = reference; }

        /// <summary>Returns the specified constraint function value.</summary>
        /// <param name="which">Specifies which constraint function to return (counting from 0).</param>
        public virtual double GetConstraint(int which)
        { return Constraints[which]; }


        /// <summary>Sets the specified constraint function value.</summary>
        /// <param name="which">Specifies which constraint function is set (counting from 0).</param>
        /// <param name="value">Assigned value of the constraint function.</param>
        public virtual void SetConstraint(int which, double value)
        {
            AllocateConstraintsList();
            Constraints[which] = value;
        }


        // CONSTRAINT FUNCTION GRADIENTS:

        /// <summary>Constraint function gradients.
        /// If CopyReferences=true (false by default) then only the list reference is copied in assignments.</summary>
        public virtual List<IVector> ConstraintGradients 
        {
            get { return _constraintGradients; }
            set
            {
                if (CopyReferences || value == null)
                    _constraintGradients = value;
                else
                {
                    AllocateConstraintGradientsList();
                    for (int which = 0; which < NumConstraints; ++which)
                    {
                        if (value[which] == null)
                            _constraintGradients[which] = null;
                        else
                            _constraintGradients[which] = value[which].GetCopy();
                    }
                }
            }
        }

        /// <summary>Returns a list of constraint function gradients.</summary>
        public virtual List<IVector> GetConstraintGradients()
        { return ConstraintGradients; }

        /// <summary>Sets constraint function gradients.
        /// If CopyReferences=true (false by default) then only the list reference is copied.</summary>
        /// <param name="values">Value to be assigned.</param>
        public virtual void SetConstraintGradients(List<IVector> values)
        { ConstraintGradients = values; }

        /// <summary>Sets constraint function gradients.
        /// Only the list reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        public virtual void SetConstraintGradientsReference(List<IVector> reference)
        { _constraintGradients = reference; }

        /// <summary>Returns the gradient of the specified constraint function.</summary>
        /// <param name="which">Specifies which constraint function to take (couonted from 0).</param>
        public virtual IVector GetConstraintGradient(int which)
        { return ConstraintGradients[which]; }


        /// <summary>Returns the specific constraint function gradient component.</summary>
        /// <param name="which">Specifies which constraint function to take (couonted from 0).</param>
        /// <param name="index">Specifies gradient component (conted form 0).</param>
        public virtual double GetConstraintGradient(int which, int index)
        { return ConstraintGradients[which][index]; }

        /// <summary>Sets the specified constraint function gradient.
        /// If CopyReferences=true (false by default) then only the reference is copied.</summary>
        /// <param name="which">Specifies which constraint function to take (couonted from 0).</param>
        /// <param name="value">Value of the gradient to be assigned.</param>
        public virtual void SetConstraintGradient(int which, IVector value)
        {
            AllocateConstraintGradientsList();
            if (CopyReferences || value == null)
                _constraintGradients[which] = value;
            else
            {
                    _constraintGradients[which] = value.GetCopy();
            }
        }

        /// <summary>Sets the specified constraint function gradient.
        /// Only the reference is copied.</summary>
        /// <param name="which">Specifies which constraint function to take (couonted from 0).</param>
        /// <param name="reference">Gradient reference to be assigned.</param>
        public virtual void SetConstraintGradientReference(int which, IVector reference)
        {
            AllocateConstraintGradientsList();
            _constraintGradients[which] = reference;
        }

        /// <summary>Sets the specified constraint fuction gradient component.</summary>
        /// <param name="which">Specifies which constraint function to take (couonted from 0).</param>
        /// <param name="index">Specified index of gradient coponent to be set.</param>
        /// <param name="value">Value to be assigned to the specified component.</param>
        public virtual void SetConstraintGradient(int which, int index, double value)
        {
            AllocateConstraintGradient(which);
            ConstraintGradients[which][index] = value;
        }


        // OBJECTIVE FUNCTION HESSIAN:

        /// <summary>Objective function Hessian (matrix of second derivatives).
        /// If CopyReferences=true (false by default) then only the reference is copied in assignments.</summary>
        public virtual IMatrix ObjectiveHessian 
        {
            get { return _objectiveHessian; }
            set
            {
                if (CopyReferences || value == null)
                    _objectiveHessian = value;
                else
                {
                    _objectiveHessian = value.GetCopy();
                }
            }
        }

        /// <summary>Returns the objective function's Hessian.</summary>
        public virtual IMatrix GetObjectiveHessian()
        { return ObjectiveHessian; }

        /// <summary>Sets the objective functions' Hessian.
        /// If CopyReferences=true (false by default) then only the reference is copied.</summary>
        /// <param name="value">Value to be assigned.</param>
        public virtual void SetObjectiveHessian(IMatrix value)
        { ObjectiveHessian = value; }

        /// <summary>Sets the objective functions' Hessian.
        /// Only the reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        public virtual void SetObjectiveHessianReference(IMatrix reference)
        { _objectiveHessian = reference; }

        /// <summary>Returns the specified component of the objective function Hessian.</summary>
        /// <param name="rowIndex">Row index of the component (counting from 0).</param>
        /// <param name="columnIndex">Column index of the component (counting from 0).</param>
        public virtual double GetObjectiveHessian(int rowIndex, int columnIndex)
        { return ObjectiveHessian[rowIndex, columnIndex]; }

        /// <summary>Sets the specified component of the objective function's Hessian.</summary>
        /// <param name="rowIndex">Row index of the component (counting from 0).</param>
        /// <param name="columnIndex">Column index of the component (counting from 0).</param>
        /// <param name="value">Value to be assigned to Hessian.</param>
        public virtual void SetObjectiveHessian(int rowIndex, int columnIndex, double value)
        {
            AllocateObjectiveHessian();
            ObjectiveHessian[rowIndex, columnIndex] = value;
        }


        // CONSTRAINT FUNCTION HESSIANS:

        /// <summary>Constraint functions' Hessians (matrices of second derivatives).
        /// If CopyReferences=true (false by default) then only the list reference is copied in assignments.</summary>
        public virtual List<IMatrix> ConstraintHessians
        {
            get { return _constraintHessians; }
            set 
            {
                if (CopyReferences || value == null)
                    _constraintHessians = value;
                else
                {
                    AllocateConstraintHessiansList();
                    for (int which = 0; which < NumConstraints; ++which)
                    {
                        if (value[which] == null)
                            _constraintHessians[which] = null;
                        else
                            _constraintHessians[which] = value[which].GetCopy();
                    }
                }
            }
        }

        /// <summary>Returns the list of constraint functions' Hessians.</summary>
        public virtual List<IMatrix> GetConstraintHessians()
        { return ConstraintHessians; }

        /// <summary>Sets constraint functios' Hessians.
        /// If CopyReferences=true (false by default) then only the list reference is copied.</summary>
        /// <param name="values">List of Hessians to be assigned.</param>
        public virtual void SetConstraintHessians(List<IMatrix> values)
        { ConstraintHessians = values; }

        /// <summary>Sets constraint functios' Hessians.
        /// Only the list reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        public virtual void SetConstraintHessiansReference(List<IMatrix> reference)
        { _constraintHessians = reference; }

        /// <summary>Returns Hessian of the specified constraint function.</summary>
        /// <param name="which">Specifies which constraint function it applies to (counting from 0).</param>
        public virtual IMatrix GetConstraintHessian(int which)
        { return ConstraintHessians[which]; }

        /// <summary>Returns the specified component of Hessian of the specified constraint function.</summary>
        /// <param name="which">Specifies which constraint function it applies to (counting from 0).</param>
        /// <param name="rowIndex">Row index of the component (counting from 0).</param>
        /// <param name="columnIndex">Column index of the component (counting from 0).</param>
        public virtual double GetConstraintHessian(int which, int rowIndex, int columnIndex)
        { return ConstraintHessians[which][rowIndex, columnIndex]; }

        /// <summary>Sets the specified constraint function's Hessian.
        /// If CopyReferences=true (false by default) then only the reference is copied.</summary>
        /// <param name="which">Specifies which constraint function it applies to (counting from 0).</param>
        /// <param name="value">Hessian matrix to be assigned.</param>
        public virtual void SetConstraintHessian(int which, IMatrix value)
        {
            AllocateConstraintHessiansList();
            if (CopyReferences || value == null)
                ConstraintHessians[which] = value;
            else
            {
                ConstraintHessians[which] = value.GetCopy();
            }
        }

        /// <summary>Sets the specified constraint function's Hessian.
        /// Only the reference is copied.</summary>
        /// <param name="which">Specifies which constraint function it applies to (counting from 0).</param>
        /// <param name="reference">Hessian matrix reference to be assigned.</param>
        public virtual void SetConstraintHessianReference(int which, IMatrix reference)
        {
            AllocateConstraintHessiansList();
            ConstraintHessians[which] = reference;
        }

        /// <summary>Sets the specified component of the specified constraint's Hessian.</summary>
        /// <param name="which">Specifies which constraint function it applies to (counting from 0).</param>
        /// <param name="rowIndex">Row index of the component (counting from 0).</param>
        /// <param name="columnIndex">Column index of the component (counting from 0).</param>
        /// <value>Value to which Hessian element is set.</value>
        public virtual void SetConstraintHessian(int which, int rowIndex, int columnIndex, double value)
        {
            AllocateConstraintHessian(which);
            ConstraintHessians[which][rowIndex, columnIndex] = value; 
        }

        #endregion Results


        #region Allocation

        // ALLOCATION OF FIELDS:

        /// <summary>Sets all objects (parameters and result objects) to null.
        /// If the references are not assigned elsewhere, these objects become eligible for garbage collection.</summary>
        public virtual void NullifyAll()
        {
            Parameters = null;
            ObjectiveGradient = null;
            ObjectiveHessian = null;
            Constraints = null;
            ConstraintGradients = null;
            ConstraintHessians = null;
        }

        /// <summary>Sets all result objects to null.
        /// If the references are not assigned elsewhere, these objects become eligible for garbage collection.</summary>
        public virtual void NullifyResults()
        {
            ObjectiveGradient = null;
            ObjectiveHessian = null;
            Constraints = null;
            ConstraintGradients = null;
            ConstraintHessians = null;
        }


        /// <summary>Allocates space for parameters.</summary>
        public virtual void AllocateParameters()
        {
            Calculated = false;
            if (_parameters == null)
                _parameters = new Vector(NumParameters);
            else
            {
                if (Parameters.Length != NumParameters)
                    _parameters = new Vector(NumParameters);
            }
        }


        /// <summary>Allocates space for all requested result objects.</summary>
        public virtual void AllocateRequested()
        {
            if (ReqObjectiveGradient)
                AllocateObjectiveGradient();
            if (ReqObjectiveHessian)
                AllocateObjectiveHessian();
            if (ReqConstraints)
                AllocateConstraintsList();
            if (ReqConstraintGradients)
                AllocateConstrainGradients();
            if (ReqConstraintHessians)
                AllocateConstraintHessians();

        }


        /// <summary>Allocate space for objective function gradient.</summary>
        public virtual void AllocateObjectiveGradient()
        {
            if (_objectiveGradient == null)
                _objectiveGradient = new Vector(NumParameters);
            else
            {
                if (_objectiveGradient.Length!=NumParameters)
                    _objectiveGradient = new Vector(NumParameters);
            }
        }

        /// <summary>Allocates space for objective function Hessian.</summary>
        public virtual void AllocateObjectiveHessian()
        {
            if (_objectiveHessian == null)
                _objectiveHessian = new Matrix(NumParameters, NumParameters);
            else
            {
                if (_objectiveHessian.RowCount != NumParameters ||
                    _objectiveHessian.ColumnCount != NumParameters)
                    _objectiveHessian = new Matrix(NumParameters, NumParameters);
            }
        }

        /// <summary>Allocates space for list of constraint functions.</summary>
        public virtual void AllocateConstraintsList()
        {
            Util.ResizeList(ref _constraints, NumConstraints, 0.0, true /* ReduceCapacity */);
        }

        /// <summary>Allocates space for list of constraint function gradients.</summary>
        public virtual void AllocateConstraintGradientsList()
        {
            Util.ResizeList(ref _constraintGradients, NumConstraints, null, true /* ReduceCapacity */);
        }

        /// <summary>Allocates space for constraint function gradients (including for the list, if necessarty).</summary>
        public virtual void AllocateConstrainGradients()
        {
            AllocateConstraintGradientsList();
            for (int which = 0; which < NumConstraints; ++which)
            {
                IVector v = _constraintGradients[which];
                if (v == null)
                    _constraintGradients[which] = new Vector(NumParameters);
                else 
                {
                    if (v.Length != NumParameters)
                        _constraintGradients[which] = new Vector(NumParameters);
                }
            }
        }

        /// <summary>Allocates space for the specified constraint function gradient.</summary>
        /// <param name="which">Specifies which constraint function it applies to (countinf form 0).</param>
        public virtual void AllocateConstraintGradient(int which)
        {
            AllocateConstraintGradientsList();
            IVector v = _constraintGradients[which];
            if (v == null)
                _constraintGradients[which] = new Vector(NumParameters);
            else
            {
                if (v.Length != NumParameters)
                    _constraintGradients[which] = new Vector(NumParameters);
            }
        }

        /// <summary>Allocates space for the list of constraint functions' Hessians.</summary>
        public virtual void AllocateConstraintHessiansList()
        {
            Util.ResizeList(ref _constraintHessians, NumConstraints, null, true /* ReduceCapacity */);
        }

        /// <summary>Allocates space for constraint functions' Hessians (including space for the list, if necessary).</summary>
        public virtual void AllocateConstraintHessians()
        {
            AllocateConstraintHessiansList();
            for (int which = 0; which < NumConstraints; ++which)
            {
                IMatrix m = _constraintHessians[which];
                if (m == null)
                    _constraintHessians[which] = new Matrix(NumParameters, NumParameters);
                else
                {
                    if (m.RowCount != NumParameters || m.ColumnCount != NumParameters)
                        _constraintHessians[which] = new Matrix(NumParameters, NumParameters);
                }
            }
        }

        /// <summary>Allocates space for the specified constraint fucnction's Hessian.</summary>
        /// <param name="which">Specifies which constraint function it applies to (conting form 0).</param>
        public virtual void AllocateConstraintHessian(int which)
        {
            AllocateConstraintHessiansList();
            IMatrix m = _constraintHessians[which];
            if (m == null)
                _constraintHessians[which] = new Matrix(NumParameters, NumParameters);
            else
            {
                if (m.RowCount != NumParameters || m.ColumnCount != NumParameters)
                    _constraintHessians[which] = new Matrix(NumParameters, NumParameters);
            }
        }

        #endregion Allocation


        #region Flags

        // REQUEST FLAGS:

        /// <summary>Indicates whether calculation of objective function is/was requested.</summary>
        public virtual bool ReqObjective
        { get { return _reqObjective; } set { _reqObjective = value; } }

        /// <summary>Indicates whether calculation of objective function gradient is/was requested.</summary>
        public virtual bool ReqObjectiveGradient
        { get { return _reqObjectiveGradient; } set { _reqObjectiveGradient = value; } }

        /// <summary>Indicates whether calculation of objective function Hessian is/was requested.</summary>
        public virtual bool ReqObjectiveHessian
        { get { return _reqObjectiveHessian; } set { _reqObjectiveHessian = value; } }
        
        /// <summary>Indicates whether calculation of constraint functions is/was requested.</summary>
        public virtual bool ReqConstraints
        { get { return _reqConstraints; } set { _reqConstraints = value; } }

        /// <summary>Indicates whether calculation of constraint functions gradient is/was requested.</summary>
        public virtual bool ReqConstraintGradients
        { get { return _reqConstraintGradients; } set { _reqConstraintGradients = value; } } 

        /// <summary>Indicates whether calculation of constraint functions Hessian is/was requested.</summary>
        public virtual bool ReqConstraintHessians
        { get { return _reqConstraintHessians; } set { _reqConstraintHessians = value; } }


        // CALCULATED FLAGS:

        /// <summary>Sets all calculated flags to false, error code to 0 (no error) and error string to null.</summary>
        public virtual void ResetResults()
        {
            CalculatedObjective = false;
            CalculatedObjectiveGradient = false;
            CalculatedObjectiveHessian = false;
            CalculatedConstraints = false;
            CalculatedConstraintGradients = false;
            CalculatedConstraintHessians = false;
        }

        /// <summary>Error code.
        ///   0 - everything is OK.
        ///   negative value - something went wrong.</summary>
        public virtual int ErrorCode
        { get { return _errorCode; } set { _errorCode = value; } }

        /// <summary>Error string indicating what went wrong.</summary>
        public virtual String ErrorString
        { get { return _errorString; } set { _errorString = value; } }


        /// <summary>Collectively gets or sets calculated flags.
        /// Set false: all calculated flags are set to false.
        /// Set true: all calculated flags for which the corresponding request flags are true, 
        ///     are set to truee, others are set to false.
        /// Get: returns true if all the flags for which the corresponding request flags are true,
        ///     are also true. Otherwise returns false.
        /// </summary>
        public virtual  bool Calculated
        { 
            get {
                return ( 
                    (!ReqObjective ||  CalculatedObjective) &&
                    (!ReqObjectiveGradient || CalculatedObjectiveGradient) &&
                    (!ReqObjectiveHessian || CalculatedObjectiveHessian) &&
                    (!ReqConstraints || CalculatedConstraints) && 
                    (!ReqConstraintGradients || CalculatedConstraintGradients) &&
                    (!ReqConstraintHessians || CalculatedConstraintHessians)
                  );
            }
            set
            {
                if (value == false)
                {
                    CalculatedObjective = false;
                    CalculatedObjectiveGradient = false;
                    CalculatedObjectiveHessian = false;
                    CalculatedConstraints = false;
                    CalculatedConstraintGradients = false;
                    CalculatedConstraintHessians = false;
                }
                else
                {
                    if (ReqObjective == true)
                        CalculatedObjective = true;
                    if (ReqObjectiveGradient == true)
                        CalculatedObjectiveGradient = true;
                    if (ReqObjectiveHessian == true)
                        CalculatedObjectiveHessian = true;
                    if (ReqConstraints == true)
                        CalculatedConstraints = true;
                    if (ReqConstraintGradients == true)
                        CalculatedConstraintGradients = true;
                    if (ReqConstraintHessians)
                        CalculatedConstraintHessians = true;
                }
            }
        }

        /// <summary>Indicates whether calculation of objective function is/was requested.</summary>
        public virtual bool CalculatedObjective
        { get { return _calcObjective; } set { _calcObjective = value;  } }

        /// <summary>Indicates whether calculation of objective function gradient is/was requested.</summary>
        public virtual bool CalculatedObjectiveGradient
        { get { return _calcObjectiveGradient; } set { _calcObjectiveGradient = value; } }

        /// <summary>Indicates whether calculation of objective function Hessian is/was requested.</summary>
        public virtual bool CalculatedObjectiveHessian
        { get { return _calcObjectiveHessian; } set { _calcObjectiveHessian = value; } }

        /// <summary>Indicates whether calculation of constraint functions is/was requested.</summary>
        public virtual bool CalculatedConstraints
        { get { return _calcConstraints; } set { _calcConstraints = value; } }

        /// <summary>Indicates whether calculation of constraint functions gradient is/was requested.</summary>
        public virtual bool CalculatedConstraintGradients
        { get { return _calcConstraintGradients; } set { _calcConstraintGradients = value; } }

        /// <summary>Indicates whether calculation of constraint functions Hessian is/was requested.</summary>
        public virtual bool CalculatedConstraintHessians
        { get { return _calcConstraintHessians; } set { _calcConstraintHessians = value; } }

        #endregion Flags


        #region Helper methods

        
        /// <summary>Sets the dimension of the analysis results object according to the
        /// specified values.
        /// <para>Number of objective functions is set to 1.</para></summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        public virtual void SetDimensions(int numParameters, int numConstraints)
        {
            SetDimensions(numParameters, 1 /* numObjectives */, numConstraints);
        }


        /// <summary>Sets the dimension of the analysis results object according to the
        /// specified values.</summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numObjectives">Number of objective functions.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        public virtual void SetDimensions(int numParameters, int numObjectives, int numConstraints)
        {
            this.NumParameters = numParameters;
            this.NumConstraints = numConstraints;
            this.NumObjectives = numObjectives;
        }
        
        /// <summary>Prepares data storage for results (allocates the appropriate vectors, matrices, lists,
        /// etc.) according to the numbers of parameters, constraint functions, and request flags, and
        /// resets calculation flags to false.
        /// <para>Things whose calculation is not requested are not calculated.</para></summary>
        public virtual void PrepareResultStorage()
        {
            PrepareResultStorage(true);
        }

        /// <summary>Prepares data storage for results (allocates the appropriate vectors, matrices, lists,
        /// etc.) according to the numbers of parameters, constraint functions, and request flags.
        /// <para>Things whose calculation is not requested are not calculated.</para></summary>
        /// <param name="resetCalculatedFlags">Whether the calculation flags are reset to false or not.</param>
        public virtual void PrepareResultStorage(bool resetCalculatedFlags)
        {
            if (resetCalculatedFlags)
                ErrorCode = 0;
            if (ReqObjective)
            {
                if (resetCalculatedFlags)
                    CalculatedObjective = false;
            } else
            {
                CalculatedObjective = false;
            }
            if (ReqConstraints)
            {
                List<double> constr = Constraints;
                if (constr == null)
                    constr = new List<double>();
                Util.ResizeList(ref constr, NumConstraints, 0.0);
                Constraints = constr;
                if (resetCalculatedFlags)
                    CalculatedConstraints = false;
            } else
            {
                CalculatedConstraints = false;
            }
            if (ReqObjectiveGradient)
            {
                IVector gradobj = ObjectiveGradient;
                VectorBase.Resize(ref gradobj, NumParameters);
                ObjectiveGradient = gradobj;
                if (resetCalculatedFlags) 
                    CalculatedObjectiveGradient = false;
            } else
            {
                CalculatedObjectiveGradient = false;
            }
            if (ReqConstraintGradients)
            {
                List<IVector> gradconstr = ConstraintGradients;
                Util.ResizeList(ref gradconstr, NumConstraints, null);
                for (int i = 0; i < NumConstraints; ++i)
                {
                    IVector grad = gradconstr[i];
                    VectorBase.Resize(ref grad, NumParameters);
                    gradconstr[i] = grad;
                }
                ConstraintGradients = gradconstr;
                if (resetCalculatedFlags)
                    CalculatedConstraintGradients = false;
            } else
            {
                CalculatedConstraintGradients = false;
            }
            if (ReqObjectiveHessian)
            {
                IMatrix hessobj = ObjectiveHessian;
                MatrixBase.Resize(ref hessobj, NumParameters, NumParameters);
                ObjectiveHessian = hessobj;
                if (resetCalculatedFlags)
                    CalculatedObjectiveHessian = false;
            }
            else
            {
                CalculatedObjectiveHessian = false;
            }
            if (ReqConstraintHessians)
            {
                List<IMatrix> hessconstr = ConstraintHessians;
                Util.ResizeList(ref hessconstr, NumConstraints, null);
                for (int i = 0; i < NumConstraints; ++i)
                {
                    IMatrix hess = hessconstr[i];
                    MatrixBase.Resize(ref hess, NumParameters, NumParameters);
                    hessconstr[i] = hess;
                }
                ConstraintHessians = hessconstr;
                if (resetCalculatedFlags)
                    CalculatedConstraintHessians = false;
            }
            else
            {
                CalculatedConstraintHessians = false;
            }
        }

        /// <summary>Prepares data storage for results (allocates the appropriate vectors, matrices, lists,
        /// etc.) according to the numbers of parameters, constraint functions, and request flags, and
        /// resets calculation flags to false.
        /// <para>This method also sets dimensions before preparing the storage (i.e. number of parameters
        /// and constraints while number of objective functions is set to 1).</para>
        /// <para>Things whose calculation is not requested are not calculated.</para></summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        public virtual void PrepareResultStorage(int numParameters, int numConstraints)
        {
             PrepareResultStorage(numParameters, 1 /* numObjectives */ , numConstraints);
        }

        /// <summary>Prepares data storage for results (allocates the appropriate vectors, matrices, lists,
        /// etc.) according to the numbers of parameters, constraint functions, and request flags, and
        /// resets calculation flags to false.
        /// <para>This method also sets dimensions before preparing the storage (i.e. number of parameters,
        /// objective functions and constraints).</para>
        /// <para>Things whose calculation is not requested are not calculated.</para></summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numObjectives">Number of objective functions.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        public virtual void PrepareResultStorage(int numParameters, int numObjectives, int numConstraints)
        {
            SetDimensions(NumParameters, numObjectives, numConstraints);
            PrepareResultStorage();
        }

        /// <summary>Prepares data storage for results (allocates the appropriate vectors, matrices, lists,
        /// etc.) according to the numbers of parameters, constraint functions, and request flags.
        /// <para>This method also sets dimensions before preparing the storage (i.e. number of parameters,
        /// objective functions and constraints).</para>
        /// <para>Things whose calculation is not requested are not calculated.</para></summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numObjectives">Number of objective functions.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        /// <param name="resetCalculatedFlags">Whether the calculation flags are reset to false or not.</param>
        public virtual void PrepareResultStorage(int numParameters, int numObjectives, int numConstraints, bool resetCalculatedFlags)
        {
            SetDimensions(NumParameters, numObjectives, numConstraints);
            PrepareResultStorage(resetCalculatedFlags);
        }

        /// <summary>Copies data from another analysis results.</summary>
        /// <param name="results">Analysis results which data is copied from.</param>
        public virtual void Copy(IAnalysisResults results)
        {
            this.CopyReferences = false;
            this.NumParameters = results.NumParameters;
            this.NumObjectives = results.NumObjectives;
            this.NumConstraints = results.NumConstraints;
            this.NumEqualityConstraints = results.NumEqualityConstraints;

            this.Parameters = results.Parameters;

            this.Objective = results.Objective;
            this.ObjectiveGradient = results.ObjectiveGradient;
            this.ObjectiveHessian = results.ObjectiveHessian;

            this.Constraints = results.Constraints;
            this.ConstraintGradients = results.ConstraintGradients;
            this.ConstraintHessians = results.ConstraintHessians;

            this.ReqObjective = results.ReqObjective;
            this.ReqObjectiveGradient = results.ReqObjectiveGradient;
            this.ReqObjectiveHessian = results.ReqObjectiveHessian;
            this.ReqConstraints = results.ReqConstraints;
            this.ReqConstraintGradients = results.ReqConstraintGradients;
            this.ReqConstraintHessians = results.ReqConstraintHessians;
            this.CalculatedObjective = results.CalculatedObjective;
            this.CalculatedObjectiveGradient = results.CalculatedObjectiveGradient;
            this.CalculatedObjectiveHessian = results.CalculatedObjectiveHessian;
            this.CalculatedConstraints = results.CalculatedConstraints;
            this.CalculatedConstraintGradients = results.CalculatedConstraintGradients;
            this.CalculatedConstraintHessians = results.CalculatedConstraintHessians;

            this.ErrorCode = results.ErrorCode;
            this.ErrorString = results.ErrorString;

        }  // Copy()

        /// <summary>Returns an exact deep copy of the current object.</summary>
        public virtual IAnalysisResults GetCopy()
        {
            AnalysisResults ret = new AnalysisResults();
            ret.Copy(this);
            return ret;
        } 

        /// <summary>Returns true if the specified constraint is violated according to the current 
        /// analysis results, false otherwise.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <param name="equalityTolerance">Tolerance for violation of equality constraints.
        /// If constraint function corresponding to equality conatraint is less or equal than tolerance then
        /// the corresponding constraint is considered non-violated.</param>
        /// <returns>True if the constraint is violated, false if not. Constraint is violated when
        /// the corresponding constraint function is greater than 0.</returns>
        /// <exception cref="IndexOutOfRangeException">when constraint index is smaller lesser than 0 or
        /// greater than the number of constraints.</exception>
        /// <exception cref="InvalidOperationException">when constraints are not evaluated.</exception>
        public virtual bool IsViolated(int which, double equalityTolerance)
        {
            if (which < 0)
                throw new IndexOutOfRangeException("Consraint violation check: Constraint number must not be negative.");
            else if (which >= NumConstraints)
                throw new IndexOutOfRangeException("Constraint violation check: constraint index (" 
                    + which + ") is out of range, should be smaller than " + NumConstraints + ".");
            if (!CalculatedConstraints)
                throw new InvalidOperationException("Consraint violation check: conatraints are not evaluated.");
            if (IsEqualityConstraint(which))
            {
                if (Math.Abs(GetConstraint(which)) > equalityTolerance)
                    return true;
            } else
            {
                if (GetConstraint(which) > 0)
                    return true;
            }
            return false;
        }

        /// <summary>Returns true if the specified constraint is violated according to the current 
        /// analysis results, false otherwise.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <returns>True if the constraint is violated, false if not. Constraint is violated when
        /// the corresponding constraint function is greater than 0.</returns>
        /// <exception cref="IndexOutOfRangeException">when constraint index is smaller lesser than 0 or
        /// greater than the number of constraints.</exception>
        /// <exception cref="InvalidOperationException">when constraints are not evaluated.</exception>
        public virtual bool IsViolated(int which)
        {
            return IsViolated(which, 0);
        }

        /// <summary>Returns true if the current analysis results represent a feasible point.
        /// Feasible point is one where no constraints are violated.
        /// For unconstraint problems this method always returns true.</summary>
        /// <returns>True if the current analysis results represent a feasible point, false otherwise.</returns>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        public virtual bool IsFeasible(double equalityTolerance)
        {
            return (GetNumViolatedConstraints(equalityTolerance) <= 0);
        }

        /// <summary>Returns true if the current analysis results represent a feasible point.
        /// Feasible point is one where no constraints are violated.
        /// For unconstraint problems this method always returns true.
        /// </summary>
        /// <returns>True if the current analysis results represent a feasible point, false otherwise.</returns>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        public virtual bool IsFeasible()
        {
            return (GetNumViolatedConstraints() <= 0);
        }


        /// <summary>Returns number of violated constraints in the current analysis results.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <param name="equalityTolerance">Tolerance for violation of equality constraints.
        /// If constraint function corresponding to equality conatraint is less or equal than tolerance then
        /// the corresponding constraint is considered non-violated.</param>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        public virtual int GetNumViolatedConstraints(double equalityTolerance)
        {
            int num = 0;
            if (NumConstraints == 0)
            {
                if (!CalculatedConstraints)
                    return 0;
            } else if (NumConstraints > 0)
            {
                if (!CalculatedConstraints)
                {
                    throw new InvalidOperationException("Can not check constraint violation, constraint functions were not calculated.");
                }
            }
            if (CalculatedConstraints)
            {
                double constraintValue;
                for (int which = 0; which < NumConstraints; ++which)
                {
                    constraintValue = GetConstraint(which);
                    if (IsEqualityConstraint(which))
                    {
                        if (Math.Abs(constraintValue) > equalityTolerance)
                            ++num;
                    } else
                    {
                        if (constraintValue > 0)
                            ++num;
                    }
                }
            }
            return num;
        }

        /// <summary>Returns number of violated constraints in the current analysis results.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        public virtual int GetNumViolatedConstraints()
        {
            return GetNumViolatedConstraints(0);
        }

        /// <summary>Returns sum of constraint function values corresponding to violated constraints 
        /// in the current analysis results.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <param name="equalityTolerance">Tolerance for violation of equality constraints.
        /// If constraint function corresponding to equality conatraint is less or equal than tolerance then
        /// the corresponding constraint is considered non-violated.</param>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        public virtual double GetSumResiduals(double equalityTolerance)
        {
            double sum = 0;
            if (NumConstraints == 0)
            {
                if (!CalculatedConstraints)
                    return 0;
            }
            else if (NumConstraints > 0)
            {
                if (!CalculatedConstraints)
                {
                    throw new InvalidOperationException("Can not sum constraint violatinos, constraint functions were not calculated.");
                }
            }
            if (CalculatedConstraints)
            {
                double constraintValue;
                for (int which = 0; which < NumConstraints; ++which)
                {
                    constraintValue = GetConstraint(which);
                    if (IsEqualityConstraint(which))
                    {
                        if (Math.Abs(constraintValue) > equalityTolerance)
                            sum += constraintValue;
                    } else
                    {
                        if (constraintValue > 0)
                            sum += constraintValue;
                    }
                }
            }
            return sum;
        }

        /// <summary>Returns sum of constraint function values corresponding to violated constraints 
        /// in the current analysis results.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        public virtual double GetSumResiduals()
        {
            return GetSumResiduals(0);
        }


        /// <summary>Returns the largest constraint function value corresponding to any violated constraint 
        /// in the current analysis results, or 0 if there are no violated constraints.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <param name="equalityTolerance">Tolerance for violation of equality constraints.
        /// If constraint function corresponding to equality conatraint is less or equal than tolerance then
        /// the corresponding constraint is considered non-violated.</param>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        public virtual double GetMaximalResidual(double equalityTolerance)
        {
            double max = 0;
            if (NumConstraints == 0)
            {
                if (!CalculatedConstraints)
                    return 0;
            }
            else if (NumConstraints > 0)
            {
                if (!CalculatedConstraints)
                {
                    throw new InvalidOperationException("Can not sum constraint violatinos, constraint functions were not calculated.");
                }
            }
            if (CalculatedConstraints)
            {
                double constraintValue;
                for (int which = 0; which < NumConstraints; ++which)
                {
                    constraintValue = GetConstraint(which);
                    if (IsEqualityConstraint(which))
                    {
                        if (Math.Abs(constraintValue) > equalityTolerance)
                            if (Math.Abs(constraintValue) > max)
                                max = Math.Abs(constraintValue);

                    } else
                    {
                        if (constraintValue > max)
                            max = constraintValue;
                    }
                }
            }
            return max;
        }

        /// <summary>Returns sum of constraint function values corresponding to violated constraints 
        /// in the current analysis results.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        public virtual double GetMaximalResidual()
        {
            return GetMaximalResidual(0);
        }

        /// <summary>Returns value of the penalty term corresponding to the specified constraint, 
        /// calculated by the specified penalty evaluator.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <param name="evaluator">Penalty function evaluator that evaluates penalty functions.</param>
        public virtual double GetPenaltyTerm(int which, IPenaltyEvaluator evaluator)
        {
            double constraintValue;
            if (IsEqualityConstraint(which))
                constraintValue = Math.Abs(GetConstraint(which));
            else constraintValue = GetConstraint(which);
            return evaluator.PenaltyValue(which, constraintValue);
        }

        /// <summary>Returns sum of the penalty terms corresponding to all constraint, 
        /// calculated by the specified penalty evaluator.</summary>
        /// <param name="evaluator">Penalty function evaluator that evaluates penalty functions.</param>
        public virtual double GetSumPenaltyTerms(IPenaltyEvaluator evaluator)
        {
            double sum = 0;
            for (int which = 0; which < NumConstraints; ++which)
                sum += GetPenaltyTerm(which, evaluator);
            return sum;
        }

        /// <summary>Returns sum of the penalty terms corresponding to all constraint, 
        /// calculated by the specified penalty evaluator.</summary>
        /// <param name="evaluator">Penalty function evaluator that evaluates penalty functions.</param>
        public virtual double GetMaxPenaltyTerm(IPenaltyEvaluator evaluator)
        {
            double max = Double.MinValue;
            double pen;
            for (int which = 0; which < NumConstraints; ++which)
            {
                pen = GetPenaltyTerm(which, evaluator);
                if (pen > max)
                    max = pen;
            }
            return max;
        }


        #endregion Helper methods


        #region InputOutput


        #region StaticInputOutput

        /// <summary>Returns a atring representation of a boolean value: "0" for false and "1" for true.</summary>
        /// <param name="value">Value whose string representation is returned.</param>
        /// <returns></returns>
        private static string ToStringMath(bool value)
        {
            if (value)
                return "1";
            else
                return "0";
        }

        protected static void AppendRequestFlagsMath(StringBuilder sb, IAnalysisResults anres)
        {
            if (sb != null)
            {
                if (anres == null)
                    sb.Append("{0, 0, 0, 0}");
                else
                {
                    sb.Append("{");
                    sb.Append(ToStringMath(anres.ReqObjective)); sb.Append(", ");
                    sb.Append(ToStringMath(anres.ReqConstraints)); sb.Append(", ");
                    sb.Append(ToStringMath(anres.ReqObjectiveGradient)); sb.Append(", ");
                    sb.Append(ToStringMath(anres.ReqConstraintGradients)); 
                    sb.Append("}");
                }
            }
        }


        /// <summary>Returns a string representation of the specified analysis results object in a standard IGLib form.</summary>
        /// <param name="anres">Object whose string representation is returned.</param>
        public static string ToString(IAnalysisResults anres)
        {
            return ToStringMath(anres);
        }


        /// <summary>Returns a string representation of the specified analysis request object in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).</summary>
        /// <param name="anres">Object whose string representation is returned.</param>
        public static string ToStringRequestMath(IAnalysisResults anres)
        {
            StringBuilder sb = new StringBuilder();
            if (anres == null)
                sb.Append("{}");
            else
            {
                sb.Append("{"); // outer-most opening bracket
                // Optimization parameters:
                sb.Append(Vector.ToStringMath(anres.Parameters)); 
                // Request flags (what must be calculated):
                sb.Append(", "); AppendRequestFlagsMath(sb, anres);
                // Client data - data that can be interpreted by the direct analysis
                sb.Append(", "); sb.Append("{}");
                sb.Append("}"); // outer-most closing bracket
            }
            return sb.ToString();
        }


        /// <summary>Returns a string representation of the specified analysis results object in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).</summary>
        /// <param name="anres">Object whose string representation is returned.</param>
        public static string ToStringMath(IAnalysisResults anres)
        {
            StringBuilder sb = new StringBuilder();
            if (anres == null)
                sb.Append("{}");
            else
            {
                sb.AppendLine("{"); // outer-most opening bracket
                // Parameters:
                sb.Append("  ");  
                sb.Append(Vector.ToStringMath(anres.Parameters));
                // Results:
                sb.Append(", " + Environment.NewLine);
                sb.Append("  { " + Environment.NewLine);
                // Objective function:
                sb.Append("    " + ToStringMath(anres.CalculatedObjective) + ", " + anres.Objective.ToString(null, CultureInfo.InvariantCulture));
                // Contraint functions:
                sb.Append(", " + Environment.NewLine + "    ");
                sb.Append(ToStringMath(anres.CalculatedConstraints) + ", {");
                for (int i = 0; i < anres.NumConstraints; ++i)
                {
                    sb.Append(anres.GetConstraint(i).ToString(null,CultureInfo.InvariantCulture));
                    if (i < anres.NumConstraints - 1)
                        sb.Append(", ");
                }
                sb.Append("}");
                // Objective gradient:
                sb.Append(", " + Environment.NewLine + "    ");
                sb.Append(ToStringMath(anres.CalculatedObjectiveGradient) + ", ");
                sb.Append(Vector.ToStringMath(anres.ObjectiveGradient));
                // Constraint gradients:
                sb.Append(", " + Environment.NewLine + "    ");
                sb.Append(ToStringMath(anres.CalculatedConstraintGradients) + ", " + Environment.NewLine);
                sb.AppendLine("    { ");
                for (int i = 0; i < anres.NumConstraints; ++i)
                {
                    sb.Append("      ");
                    sb.Append(Vector.ToStringMath(anres.GetConstraintGradient(i)));
                    if (i<anres.NumConstraints-1)
                        sb.AppendLine(", ");
                    else
                        sb.AppendLine();
                }
                sb.Append("    }");
                // Error code:
                sb.Append(", " + Environment.NewLine + "    ");
                sb.Append(anres.ErrorCode);
                sb.AppendLine(" ");
                sb.Append("  }");  // end of results
                // Request flags:
                sb.AppendLine(", ");
                sb.Append("  ");
                AppendRequestFlagsMath(sb, anres);
                sb.AppendLine(" ");
                sb.Append("}"); // outer-most closing bracket
            }
            return sb.ToString();
        }


        // req req

        /// <summary>Reads analysis request data.</summary>
        /// <param name="requestString"></param>
        /// <param name="anres"></param>
        /// $A Igor Mar11; Tako78 Apr11;
        public static void LoadStringRequestMath(string requestString, ref AnalysisResults anres)
        {
            IVector parameters = null;
            bool 
                reqcalcobj = false,
                reqcalcconstr = false, 
                reqcalcgradobj = false, 
                reqcalcgradconstr = false;
            string clientData = null;
            GetAnalysisRequest(requestString, ref parameters,
                ref reqcalcobj, ref reqcalcconstr, ref reqcalcgradobj, ref reqcalcgradconstr, ref clientData);
            int numParameters = 0;
            if (parameters!=null)
                numParameters = parameters.Length;
            if (anres == null)
            {
                anres = new AnalysisResults(numParameters);
            }
            anres.ResetResults();
            anres.Parameters = parameters;
            anres.ReqObjective = reqcalcobj;
            anres.ReqConstraints = reqcalcconstr;
            anres.ReqObjectiveGradient = reqcalcgradobj;
            anres.ReqConstraintGradients = reqcalcgradconstr;
            anres.ReqObjectiveHessian = false;
            anres.ReqConstraintHessians = false;
            // TODO: add clientData to anres! (find out which is the location intended for this!)
        }


        /// <summary>Reads analysis results data.</summary>
        /// <param name="analysisOutputString">String from which data is read.</param>
        /// <param name="anres">Object where results are stored.</param>
        /// $A Igor Mar11; Tako78 Apr11;
        public static void LoadStringMath(string analysisOutputString, ref AnalysisResults anres)
        {
            IVector parameters = null;
            bool
                calcobj = false,
                calcconstr = false,
                calcgradobj = false,
                calcgradconstr = false,
                reqcalcobj = false,
                reqcalcconstr = false,
                reqcalcgradobj = false,
                reqcalcgradconstr = false;
            double obj = 0.0;
            IVector constr = null;
            IVector dobjdp = null;
            IVector[] dconstr = null;
            int errorcode = 0;
            GetAnalysisResult(analysisOutputString, ref parameters, ref calcobj, ref calcconstr,
                ref calcgradobj, ref calcgradconstr, ref obj, ref constr, ref dobjdp, ref dconstr,
                ref errorcode, ref reqcalcobj, ref reqcalcconstr, ref reqcalcgradobj, ref reqcalcgradconstr);
            int numParameters = 0;
            if (parameters != null)
                numParameters = parameters.Length;
            else if (calcgradobj && dobjdp != null)
                numParameters = dobjdp.Length;
            else if (calcgradconstr && dconstr != null)
            {
                if (dconstr.Length > 0)
                    if (dconstr[0] != null)
                        numParameters = dconstr[0].Length;
            }
            int numConstraints = 0;
            if (calcconstr && constr != null)
                numConstraints = constr.Length;
            else if (calcgradconstr && dconstr != null)
                numConstraints = dconstr.Length;
            if (anres == null)
            {
                anres = new AnalysisResults(numParameters, numConstraints);
            }
            if (anres.NumParameters != numParameters)
                anres.NumParameters = numParameters;
            if (anres.NumConstraints != numConstraints)
                anres.NumConstraints = numConstraints;

            anres.ResetResults();
            anres.Parameters = parameters;
            anres.ReqObjective = reqcalcobj;
            anres.ReqConstraints = reqcalcconstr;
            anres.ReqObjectiveGradient = reqcalcgradobj;
            anres.ReqConstraintGradients = reqcalcgradconstr;
            anres.ReqObjectiveHessian = false;
            anres.ReqConstraintHessians = false;
            anres.CalculatedObjectiveHessian = false;
            anres.CalculatedConstraintHessians = false;
            if (calcobj)
            {
                anres.Objective = obj;
                anres.CalculatedObjective = true;
            } else
                anres.CalculatedObjective = false;
            if (calcconstr)
            {
                anres.CalculatedConstraints = true;
                for (int i=0; i<constr.Length; ++i)
                    anres.SetConstraint(i, constr[i]);
            }
            if (calcgradobj)
                anres.ObjectiveGradient = new Vector(dobjdp);

        }



        /// <summary>Saves the specified analysis request to a file.
        /// If the file already exists then it is overwritten.</summary>
        /// <param name="anres">Object from which analysis request is saved.</param>
        /// <param name="inputFilePath">Path to the file where analysis request is stored.</param>
        public static void SaveRequestMath(IAnalysisResults anres, string filePath)
        {
            SaveRequestMath(anres, filePath, false /* append */);
        }


        /// <summary>Saves the specified analysis request to a file.</summary>
        /// <param name="anres">Object from which analysis request is saved.</param>
        /// <param name="inputFilePath">Path to the file where analysis request is stored.</param>
        /// <param name="append">If true then contents are appended to the end of the file if the file already exists.</param>
        public static void SaveRequestMath(IAnalysisResults anres, string filePath, bool append)
        {
            using (StreamWriter sw = new StreamWriter(filePath, append))
            {
                sw.Write(ToStringRequestMath(anres));
            }
        }


        /// <summary>Saves the specified analysis results to a file.
        /// If the file already exists then it is overwritten.</summary>
        /// <param name="anres">Object from which analysis results are saved.</param>
        /// <param name="inputFilePath">Path to the file where analysis results are stored.</param>
        public static void SaveMath(IAnalysisResults anres, string filePath)
        {
            SaveMath(anres, filePath, false /* append */);
        }


        /// <summary>Saves the specified analysis results to a file.</summary>
        /// <param name="anres">Object from which analysis results are saved.</param>
        /// <param name="inputFilePath">Path to the file where analysis results are stored.</param>
        /// <param name="append">If true then contents are appended to the end of the file if the file already exists.</param>
        public static void SaveMath(IAnalysisResults anres, string filePath, bool append)
        {
            using (StreamWriter sw = new StreamWriter(filePath, append))
            {
                sw.Write(ToStringMath(anres));
            }
        }

        /// <summary>Loads analysis request data from a file in standard mathematical format and stores it in the specified 
        /// analysis results object.</summary>
        /// <param name="inputFilePath">Path to the file from which object is read.</param>
        /// <param name="anResults">Object where data is stored.</param>
        public static void LoadRequestMath(string filePath, ref AnalysisResults anResults)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string str = sr.ReadToEnd();
                LoadStringRequestMath(str, ref anResults);
            }
        }


        /// <summary>Loads analysis results from a file in standard mathematical format and stores it in the specified 
        /// analysis results object.</summary>
        /// <param name="inputFilePath">Path to the file from which object is read.</param>
        /// <param name="anResults">Object where data is stored.</param>
         public static void LoadMath(string filePath, ref AnalysisResults anResults)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string str = sr.ReadToEnd();
                LoadStringMath(str, ref anResults);
            }
        }


        /// <summary>Saves (serializes) the specified analysis request to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="anResulsts">Object that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file in which object is is saved.</param>
        public static void SaveRequestJson(AnalysisResults anResulsts, string filePath)
        {
            SaveJson(anResulsts, filePath, false /* append */ );
        }

        /// <summary>Saves (serializes) the specified analysis request to the specified JSON file.
        /// If the file already exists, contents either overwrites the file or is appended at the end, 
        /// dependent on the value of the append flag.</summary>
        /// <param name="anResults">Object that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file in which object is is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        public static void SaveRequestJson(AnalysisResults anResults, string filePath, bool append)
        {
            AnalysisRequestDto dtoOriginal = new AnalysisRequestDto();
            dtoOriginal.CopyFrom(anResults);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<AnalysisRequestDto>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) analysis request from the specified file in JSON format.</summary>
        /// <param name="inputFilePath">File from which object is restored.</param>
        /// <param name="anResultsRestored">Object that is restored by deserialization.</param>
        public static void LoadRequestJson(string filePath, ref AnalysisResults anResultsRestored)
        {
            ISerializer serializer = new SerializerJson();
            AnalysisRequestDto dtoRestored = serializer.DeserializeFile<AnalysisRequestDto>(filePath);
            dtoRestored.CopyTo(ref anResultsRestored);
        }


        /// <summary>Saves (serializes) the specified analysis results to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="anResulsts">Object that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file in which object is is saved.</param>
        public static void SaveJson(AnalysisResults anResulsts, string filePath)
        {
            SaveJson(anResulsts, filePath, false /* append */ );
        }

        /// <summary>Saves (serializes) the specified analysis results to the specified JSON file.
        /// If the file already exists, contents either overwrites the file or is appended at the end, 
        /// dependent on the value of the append flag.</summary>
        /// <param name="anResults">Object that is saved to a file.</param>
        /// <param name="inputFilePath">Path to the file in which object is is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        public static void SaveJson(AnalysisResults anResults, string filePath, bool append)
        {
            AnalysisResultsDto dtoOriginal = new AnalysisResultsDto();
            dtoOriginal.CopyFrom(anResults);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<AnalysisResultsDto>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) analysis results from the specified file in JSON format.</summary>
        /// <param name="inputFilePath">File from which object is restored.</param>
        /// <param name="anResultsRestored">Object that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref AnalysisResults anResultsRestored)
        {
            ISerializer serializer = new SerializerJson();
            AnalysisResultsDto dtoRestored = serializer.DeserializeFile<AnalysisResultsDto>(filePath);
            dtoRestored.CopyTo(ref anResultsRestored);
        }


        

        #region TadejsMethods

        // REMARKS:
        // Methods are literally copied from Tadej's code and then called in final versions of the methods.
        // In order to update to Tadej's newest code, copy the methods again!



        /// <summary>Read the analysis data from data file
        /// Format: { { p1, p2, … }, { reqcalcobj, reqcalcconstr, reqcalcgradobj, reqcalcgradconstr }, cd } </summary>
        /// <param name="inputFilePath">Path to the file where training data are saved.</param>
        /// <param name="parameters">Input and output parameters: { p1, p2, … }.</param>
        /// <param name="reqcalcobj">Flag: reqcalcobj.</param>
        /// <param name="reqcalcconstr">Flag: reqcalcconstr.</param>
        /// <param name="reqcalcgradobj">Flag: reqcalcgradobj.</param>
        /// <param name="reqcalcgradconstr">Flag: reqcalcgradconstr.</param>
        /// <param name="cd">String: cd.</param>
        /// $A Tako78 Mar11;
        private static void ReadAnalysisRequest(string filePath, ref IVector parameters,
            ref bool reqcalcobj, ref bool reqcalcconstr, ref bool reqcalcgradobj, ref bool reqcalcgradconstr, ref string cd)
        {
            string requestString;
            using (StreamReader sr = new StreamReader(filePath))
            {
                requestString = sr.ReadToEnd();
            }
            GetAnalysisRequest(requestString, ref parameters,
            ref reqcalcobj, ref reqcalcconstr, ref reqcalcgradobj, ref reqcalcgradconstr, ref cd);
        }


        /// <summary>Read the analysis request data from data file
        /// Format: { { p1, p2, … }, { reqcalcobj, reqcalcconstr, reqcalcgradobj, reqcalcgradconstr }, cd } </summary>
        /// <param name="requestString">String with request analysis data.</param>
        /// <param name="parameters">Input and output parameters: { p1, p2, … }.</param>
        /// <param name="reqcalcobj">Flag: reqcalcobj.</param>
        /// <param name="reqcalcconstr">Flag: reqcalcconstr.</param>
        /// <param name="reqcalcgradobj">Flag: reqcalcgradobj.</param>
        /// <param name="reqcalcgradconstr">Flag: reqcalcgradconstr.</param>
        /// <param name="cd">String: cd.</param>
        /// $A Tako78 Mar11;
        private static void GetAnalysisRequest(string requestString, ref IVector parameters,
            ref bool reqcalcobj, ref bool reqcalcconstr, ref bool reqcalcgradobj, ref bool reqcalcgradconstr, ref string cd)
        {
            // Format:
            // { { p1, p2, … }, { reqcalcobj, reqcalcconstr, reqcalcgradobj, reqcalcgradconstr }, cd }

            // Legend:
            //•	calcobj – flag for the objective function
            //•	calcconstr – flag for constraint functions
            //•	calcgradobj – gradient of the objective function
            //•	calcgradconstr – gradients of constraint functions
            //obj – value of the objective functions
            //constr1, constr2, … - values of the constraint functions
            //dobjdp1, dobjdp2, ... – derivatives of the objective function with respect to individual parameters (components of the objective function gradient)
            //dconstr1dp1, …, dconstr2dp1, dconstr2dp2 – derivatives of individual constraint functions with respect to individual optimization parameters – components of gradients of the constraint functions (e.g. dconstr2dp3 is the derivative of the second constraint function with respect to the third parameter)
            //errorcode – integer error code of analysis – 0 for no error, usually a negative number for errors, values are function specific
            //reqcalcob , reqcalcconstr, reqcalcgradobj and reqcalcgradconstr are request flags for calculation of the various values, as have been passed to the analysis function. The same as with parameter values, these flags are requested only for verification. In vast majority of cases these flags will not be used by the optimization program, and they can simply be set to 1.

            if (requestString == null)
                throw new ArgumentNullException("Analysis request string is empty.");
            requestString = requestString.Trim();
            string[] requestLine = requestString.Split('{', '}');
            requestLine[2] = requestLine[2].Trim();
            //read parameter data
            string[] param = requestLine[2].Split(',');
            if (parameters == null)
                parameters = new Vector(param.Length);
            for (int i = 0; i < param.Length; i++)
            {
                param[i] = param[i].Trim();
                parameters[i] = double.Parse(param[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            //read boolean data
            string[] strBool = requestLine[4].Split(',');
            bool[] tmpBool = new bool[strBool.Length];
            for (int i = 0; i < strBool.Length; i++)
            {
                strBool[i] = strBool[i].Trim();
                tmpBool[i] = IG.Lib.UtilStr.ToBoolean(strBool[i]);
            }
            reqcalcobj = tmpBool[0];
            reqcalcconstr = tmpBool[1];
            reqcalcgradobj = tmpBool[2];
            reqcalcgradconstr = tmpBool[3];
            //read cd data
            requestLine[5] = requestLine[5].Trim();
            cd = requestLine[5];
        }

        /// <summary>Read the analysis result data from data file
        /// Format:  </summary>
        /// <param name="inputFilePath">Path to the file where training data are saved.</param>
        /// <param name="parameters">Input and output parameters: { p1, p2, … }.</param>
        /// <param name="calcobj">Flag for the objective function.</param>
        /// <param name="calcconstr">Flag for constraint functions.</param>
        /// <param name="calcgradobj">Gradient of the objective function.</param>
        /// <param name="calcgradconstr">Gradients of constraint functions.</param>
        /// <param name="obj">Value of the objective functions.</param>
        /// <param name="constr">Values of the constraint functions.</param>
        /// <param name="dobjdp">Derivatives of the objective function.</param>
        /// <param name="dconstr">Derivatives of individual constraint functions.</param>
        /// <param name="errorcode">Integer error code of analysis.</param>
        /// <param name="reqcalcobj">Flag for calculation of the various values.</param>
        /// <param name="reqcalcconstr">Flag for calculation of the various values.</param>
        /// <param name="reqcalcgradobj">Flag for calculation of the various values.</param>
        /// <param name="reqcalcgradconstr">Flag for calculation of the various values.</param>
        /// $A Tako78 Apr11;
        private static void ReadAnalysisResult(string filePath, ref IVector parameters, ref bool calcobj, ref bool calcconstr,
            ref bool calcgradobj, ref bool calcgradconstr, ref double obj, ref IVector constr, ref IVector dobjdp, ref IVector[] dconstr,
            ref int errorcode, ref bool reqcalcobj, ref bool reqcalcconstr, ref bool reqcalcgradobj, ref bool reqcalcgradconstr)
        {
            string resultString;
            using (StreamReader sr = new StreamReader(filePath))
            {
                resultString = sr.ReadToEnd();
            }
            GetAnalysisResult(resultString, ref parameters, ref calcobj, ref calcconstr,
                ref calcgradobj, ref calcgradconstr, ref obj, ref constr, ref dobjdp, ref dconstr,
                ref errorcode, ref reqcalcobj, ref reqcalcconstr, ref reqcalcgradobj, ref reqcalcgradconstr);
        }

        /// <summary>Read the analysis result data from data file
        /// Format:  </summary>
        /// <param name="requestString">String with result analysis data.</param>
        /// <param name="parameters">Input and output parameters: { p1, p2, … }.</param>
        /// <param name="calcobj">Flag for the objective function.</param>
        /// <param name="calcconstr">Flag for constraint functions.</param>
        /// <param name="calcgradobj">Gradient of the objective function.</param>
        /// <param name="calcgradconstr">Gradients of constraint functions.</param>
        /// <param name="obj">Value of the objective functions.</param>
        /// <param name="constr">Values of the constraint functions.</param>
        /// <param name="dobjdp">Derivatives of the objective function.</param>
        /// <param name="dconstr">Derivatives of individual constraint functions.</param>
        /// <param name="errorcode">Integer error code of analysis.</param>
        /// <param name="reqcalcobj">Flag for calculation of the various values.</param>
        /// <param name="reqcalcconstr">Flag for calculation of the various values.</param>
        /// <param name="reqcalcgradobj">Flag for calculation of the various values.</param>
        /// <param name="reqcalcgradconstr">Flag for calculation of the various values.</param>
        /// $A Tako78 Apr11;
        private static void GetAnalysisResult(string requestString, ref IVector parameters, ref bool calcobj, ref bool calcconstr,
            ref bool calcgradobj, ref bool calcgradconstr, ref double obj, ref IVector constr, ref IVector dobjdp, ref IVector[] dconstr,
            ref int errorcode, ref bool reqcalcobj, ref bool reqcalcconstr, ref bool reqcalcgradobj, ref bool reqcalcgradconstr)
        {
            if (requestString == null)
                throw new ArgumentNullException("Analysis result string is empty.");
            requestString = requestString.Trim();
            string[] resultLine = requestString.Split('{', '}');
            //Read parameters
            string[] param = resultLine[2].Split(',');
            if (parameters == null)
                parameters = new Vector(param.Length);
            for (int i = 0; i < param.Length; i++)
            {
                param[i] = param[i].Trim();
                parameters[i] = double.Parse(param[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            //Read request flags for calculation
            int tmpReqFlag = resultLine.Length - 3;
            string[] regFlag = resultLine[tmpReqFlag].Split(',');
            bool[] boolReqFlag = new bool[regFlag.Length];
            for (int i = 0; i < regFlag.Length; i++)
            {
                regFlag[i] = regFlag[i].Trim();
                boolReqFlag[i] = IG.Lib.UtilStr.ToBoolean(regFlag[i]);
            }
            reqcalcobj = boolReqFlag[0];
            reqcalcconstr = boolReqFlag[1];
            reqcalcgradobj = boolReqFlag[2];
            reqcalcgradconstr = boolReqFlag[3];
            //Read error code
            tmpReqFlag = resultLine.Length - 5;
            resultLine[tmpReqFlag] = resultLine[tmpReqFlag].Trim();
            string[] tmpError = resultLine[tmpReqFlag].Split(',');
            errorcode = int.Parse(tmpError[1]);
            //Read calcobj, obj, calcconstr
            string[] tmpLine4 = resultLine[4].Split(',');
            calcobj = IG.Lib.UtilStr.ToBoolean(tmpLine4[0]);
            tmpLine4[1] = tmpLine4[1].Trim();
            obj = double.Parse(tmpLine4[1], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
            calcconstr = IG.Lib.UtilStr.ToBoolean(tmpLine4[2]);
            //Read constr
            string[] tmpConstr = resultLine[5].Split(',');
            resultLine[5] = resultLine[5].Trim();
            if (constr == null)
                constr = new Vector(tmpConstr.Length);
            if (resultLine[5] != "")
            {
                for (int i = 0; i < tmpConstr.Length; i++)
                {
                    tmpConstr[i] = tmpConstr[i].Trim();
                    constr[i] = double.Parse(tmpConstr[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
            }
            //Read calcgradobj
            string[] tmpCalcgradobj = resultLine[6].Split(',');
            calcgradobj = IG.Lib.UtilStr.ToBoolean(tmpCalcgradobj[1]);
            //Read dobjdp
            string[] tmpDobjdp = resultLine[7].Split(',');
            resultLine[7] = resultLine[7].Trim();
            if (dobjdp == null)
                dobjdp = new Vector(tmpDobjdp.Length);
            if (resultLine[7] != "")
            {
                for (int i = 0; i < tmpDobjdp.Length; i++)
                {
                    tmpDobjdp[i] = tmpDobjdp[i].Trim();
                    dobjdp[i] = double.Parse(tmpDobjdp[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
            }

            //Read calcgradconstr
            string[] tmpCalcgradconstr = resultLine[8].Split(',');
            calcgradconstr = IG.Lib.UtilStr.ToBoolean(tmpCalcgradconstr[1]);
            //Read dconstr
            List<double[]> dataDconstr = new List<double[]>();
            for (int i = 10; i < (resultLine.Length - 6); i++)
            {

                string[] tmp = resultLine[i].Split(',');
                resultLine[i] = resultLine[i].Trim();
                double[] tmpDconstr = new double[tmp.Length];

                if (resultLine[i] != "")
                {
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        tmp[j] = tmp[j].Trim();
                        tmpDconstr[j] = double.Parse(tmp[j], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                    dataDconstr.Add(tmpDconstr);
                }
                i++;

            }
            if (dconstr == null)
                dconstr = new IVector[dataDconstr.Count];
            else if (dconstr.Length != dataDconstr.Count)
                dconstr = new IVector[dataDconstr.Count];
            for (int i = 0; i < dconstr.Length; ++i)
                dconstr[i] = new Vector(dataDconstr[i]);
        }


        #endregion TadejsMethods



        #endregion StaticInputOutput



        /// <summary>Returns a string representation of the current analysis results object in a standard IGLib form.</summary>
        public override string ToString()
        {
            return ToString(this);
        }


        /// <summary>Returns a string representation of the current analysis results object in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).</summary>
        public string ToStringMath()
        {
            return ToStringMath(this);
        }


        #endregion InputOutput

        
        #region Examples

        /// <summary>Creates an example analysis results object according to parameters, with some arbitrary result components.</summary>
        /// <param name="numParameters">Number of prameters of the optimization problem.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        /// <param name="reqObjective">Whether calculation of the objective function is required.</param>
        /// <param name="reqConstraints">Whether calculation of constraint functions is required.</param>
        /// <param name="reqObjectiveGradient">Whether calculation of objective gradient is required.</param>
        /// <param name="reqConstraintGradients">Whether calculation of constraint gradients is required.</param>
        /// <param name="reqObjectiveHessian">Whether calculation of objective Hessian is required.</param>
        /// <param name="reqConstraintHessians">Whether calculation of constraint Hessians is required.</param>
        public static AnalysisResults CreateExample(int numParameters, int numConstraints,
            bool reqObjective, bool reqConstraints, bool reqObjectiveGradient, bool reqConstraintGradients,
            bool reqObjectiveHessian, bool reqConstraintHessians)
        {
            AnalysisResults anres = new AnalysisResults();
            anres.NumParameters = numParameters;
            anres.NumObjectives = 1;
            anres.NumConstraints = numConstraints;
            Vector param = new Vector(numParameters);
            for (int i = 0; i < numParameters; ++i)
                param[i] = (double)(i+1) * 0.1111;
            anres.SetParametersReference(param);
            anres.ReqObjective = anres.CalculatedObjective = reqObjective;
            if (reqObjective)
                anres.Objective = 1.1111;
            else
                anres.Objective = 0.0;
            anres.ReqConstraints = anres.CalculatedConstraints = reqConstraints;
            if (reqConstraints)
            {
                List<double> c = new List<double>();
                for (int i = 0; i < numConstraints; ++i)
                    c.Add(200.0 + (double) (i+1) / 10.0);
                anres.Constraints = c;
            }
            anres.ReqObjectiveGradient = anres.CalculatedObjectiveGradient = reqObjectiveGradient;
            if (reqObjectiveGradient)
            {
                Vector objGrad = new Vector(numParameters);
                for (int i = 0; i < numParameters; ++i)
                    objGrad[i] = 10.0 + (double) (i+i) / 1000.0;
                anres.ObjectiveGradient = objGrad;
            }
            anres.ReqConstraintGradients = anres.CalculatedConstraintGradients = reqConstraintGradients;
            if (reqConstraintGradients)
            {
                List<IVector> cGrad = new List<IVector>();
                for (int i = 0; i < numConstraints; ++i)
                {
                    Vector grad_i = new Vector(numParameters);
                    for (int j = 0; j < numParameters; ++j)
                    {
                        grad_i[j] = 10.0 * (double) (i+1) + (double) (i+1)*0.1  + 0.001 * (double) (j+1);
                    }
                    cGrad.Add(grad_i);
                }
                anres.ConstraintGradients = cGrad;
            }

            anres.ReqObjectiveHessian = anres.CalculatedObjectiveHessian = reqObjectiveHessian;
            if (reqObjectiveHessian)
            {
                Matrix objHessian = new Matrix(numParameters, numParameters);
                for (int i = 0; i < numParameters; ++i)
                    for (int j = 0; j <= i; ++j)
                    {
                        objHessian[i,j] = (double) (i+1) + ((double) (j+1))/100.0;
                        if (i != j)
                            objHessian[j, i] = objHessian[i, j];
                    }
                anres.ObjectiveHessian = objHessian;
            }
            anres.ReqConstraintHessians = anres.CalculatedConstraintHessians = reqConstraintHessians;
            if (reqConstraintHessians)
            {
                List<IMatrix> cHessians = new List<IMatrix>();
                for (int i = 0; i < numConstraints; ++i)
                {
                    IMatrix hess_i = new Matrix(numParameters, numParameters);
                    for (int j = 0; j < numParameters; ++j)
                        for (int k=0; k<=j; ++ k)
                        {
                            hess_i[j,k] = (double) (i+1) + 0.001 * (double) (j+1) + 0.000001 * (double) (k+1);
                            if (j != k)
                                hess_i[k, j] = hess_i[j, k];
                        }
                    cHessians.Add(hess_i);
                }
                anres.ConstraintHessians = cHessians;
            }
            return anres;
        }

        #endregion Examples


    }  // class AnalysisResults


    /// <summary>Evaluation of penalty functions.</summary>
    /// $A Igor Jul10;
    public class PenaltyEvaluator : IPenaltyEvaluator, ILockable
    {

        #region Initialization

        /// <summary>Constructs non-initialized penalty evaluator (wihout any penlaty functions).
        /// WARNING:
        /// Crated penalty evaluator is not initialized and can not be used for evaluation of penalty
        /// terms right away.</summary>
        public PenaltyEvaluator()
        {  }

        /// <summary>Constructs penalty evaluator with penalty function (common for all constraints)
        /// initialized to the default penalty function with specified characteristic barrier length and
        /// height.</summary>
        /// <param name="barrierLength">Characteristic barrrier length. Within this length the created penalty 
        /// function grows approximately from 0 to characteristic height.</param>
        /// <param name="barrierHeight">Characteristic barrier height.</param>
        public PenaltyEvaluator(double barrierLength, double barrierHeight)
        {
            SetPenaltyFunction(0, barrierLength, barrierHeight);
        }

        /// <summary>Construct penalty evaluator with penalty function (common for all constraints)
        /// initialized to the default penalty function with specified characteristic barrier length and
        /// height and transition point where penalty function becomes non-zero.</summary>
        /// <param name="barrierLength">Characteristic barrrier length. Within this length the created penalty 
        /// function grows approximately from 0 to characteristic height.</param>
        /// <param name="barrierHeight">Characteristic barrier height.</param>
        /// <param name="maxZero">Maximal argument for which the created function is 0.</param>
        public PenaltyEvaluator(double barrierLength, double barrierHeight, double maxZero)
        {
            SetPenaltyFunction(0, barrierLength, barrierHeight, maxZero);
        }

        #endregion Initialization

        protected object _lock = new object();

        /// <summary>Object used for thread locking the current object.</summary>
        public object Lock
        { get { return _lock; } }

        #region Operation Parameters


        #endregion  Parameters

        protected bool _allowSingleFunction = true;

        /// <summary>Whether a single function can be used for evaluating penalty terms
        /// corresponding to different constraints.</summary>
        public bool AllowSingleFunction
        {
            get { return _allowSingleFunction; }
            set { _allowSingleFunction = value; }
        }

        #region Penalty Functions


        /// <summary>Creates a new penalty function with the specified characteristic length and height.
        /// Created function is power penalty function of power 3.
        /// This is used in order to create default penalty functions for this class, such that
        /// one does not need to create penalty functions explicitly.
        /// Subclasses can (should) override this method in orde to change the type of default penalty function.</summary>
        /// <param name="barrierLength">Characteristic barrrier length. Within this length the created penalty 
        /// function grows approximately from 0 to characteristic height.</param>
        /// <param name="barrierHeight">Characteristic height.</param>
        /// <returns>Created penalty function.</returns>
        protected virtual IRealFunctionPenalty CreatePenaltyFunction(double barrierLength,
            double barrierHeight)
        {
            return Func.GetPenaltyPower(barrierLength, barrierHeight, 3);
        }

        /// <summary>Creates a new penalty function with the specified characteristic length and height.
        /// Created function is power penalty function of power 3.
        /// This is used in order to create default penalty functions for this class, such that
        /// one does not need to create penalty functions explicitly.
        /// Subclasses can (should) override this method in orde to change the type of default penalty function.</summary>
        /// <param name="barrierLength">Characteristic barrrier length. Within this length the created penalty 
        /// function grows approximately from 0 to characteristic height.</param>
        /// <param name="barrierHeight">Characteristic height.</param>
        /// <param name="zeroEnd">Maximal argument for which the created function is 0.</param>
        /// <param name="power">Power. Must be greater than 0. For 2 first derivative is continuous
        /// in transition points, for 3 second derivative is also continuous, etc.</param>
        /// <returns>Created penalty function.</returns>
        protected virtual IRealFunctionPenalty CreatePenaltyFunction(double barrierLength, double barrierHeight,
            double zeroEnd, int power)
        {
            return Func.GetPenaltyPower(barrierLength, barrierHeight, zeroEnd, 3);
        }


        protected List<IRealFunctionPenalty> _penaltyFunctions = null;

        /// <summary>Returns a list of penalty functions used to evaluate penalty terms.</summary>
        public List<IRealFunctionPenalty> PenaltyFunctions
        {
            get
            {
                lock (_lock)
                {
                    if (_penaltyFunctions == null)
                        _penaltyFunctions = new List<IRealFunctionPenalty>();
                    return _penaltyFunctions;
                }
            }
        }

        /// <summary>Sets the penalty function used for evaluation of the specified penalty term.</summary>
        /// <param name="which">Specifies which conatraint the penalty function applies to (zero based).
        /// In order to use one penalty function for all constraints, just set the penalty function
        /// with index 0.</param>
        /// <param name="function">Function to be used for evaluation of the specified penalty term.</param>
        public void SetPenaltyFunction(int which, IRealFunctionPenalty function)
        {
            lock (_lock)
            {
                List<IRealFunctionPenalty> funcList = PenaltyFunctions;  // ensure initialization
                while (PenaltyFunctions.Count <= which)
                    PenaltyFunctions.Add(null);
                PenaltyFunctions[which] = function;
            }
        }

        /// <summary>Sets the penalty function used for evaluation of the specified penalty term.</summary>
        /// <param name="which">Specifies which conatraint the penalty function applies to (zero based).
        /// In order to use one penalty function for all constraints, just set the penalty function
        /// with index 0.</param>
        /// <param name="barrierLength">Characteristic barrier length of the created function.
        /// This is the length at which function grows from approximately 0 to characteristic height.</param>
        /// <param name="barrierHeight">Characteristic barrier height of the created function.</param>
        public void SetPenaltyFunction(int which, double barrierLength, double barrierHeight)
        {
            lock (_lock)
            {
                List<IRealFunctionPenalty> funcList = PenaltyFunctions;  // ensure initialization
                while (PenaltyFunctions.Count <= which)
                    PenaltyFunctions.Add(null);
                if (PenaltyFunctions[which] == null)
                    PenaltyFunctions[which] = CreatePenaltyFunction(barrierLength, barrierHeight);
                else
                {
                    IRealFunctionPenalty func = PenaltyFunctions[which];
                    if (!func.CanSetBarrierLength)
                        throw new ArgumentException("Can not set characteristic barrier length for penalty function "
                            + which + ".");
                    if (!func.CanSetBarrierHeight)
                        throw new ArgumentException("Can not set characteristic barrier height for penalty function "
                            + which + ".");
                    func.BarrierLength = barrierLength;
                    func.BarrierHeight = barrierHeight;
                }
            }
        }

        /// <summary>Sets the penalty function used for evaluation of the specified penalty term.</summary>
        /// <param name="which">Specifies which conatraint the penalty function applies to (zero based).
        /// In order to use one penalty function for all constraints, just set the penalty function
        /// with index 0.</param>
        /// <param name="barrierLength">Characteristic barrier length of the created function.
        /// This is the length at which function grows from approximately 0 to characteristic height.</param>
        /// <param name="barrierHeight">Characteristic barrier height of the created function.</param>
        /// <param name="zeroEnd">Maximal argumet for which penalty function is still 0.</param>
        public void SetPenaltyFunction(int which, double barrierLength, double barrierHeight,
            double zeroEnd)
        {
            lock (_lock)
            {
                List<IRealFunctionPenalty> funcList = PenaltyFunctions;  // ensure initialization
                while (PenaltyFunctions.Count <= which)
                    PenaltyFunctions.Add(null);
                if (PenaltyFunctions[which] == null)
                    PenaltyFunctions[which] = CreatePenaltyFunction(barrierLength, barrierHeight);
                else
                {
                    IRealFunctionPenalty func = PenaltyFunctions[which];
                    if (!func.CanSetBarrierLength)
                        throw new ArgumentException("Can not set characteristic barrier length for penalty function "
                            + which + ".");
                    if (!func.CanSetBarrierHeight)
                        throw new ArgumentException("Can not set characteristic barrier height for penalty function "
                            + which + ".");
                    if (!func.CanSetMaxZero)
                        throw new ArgumentException("Can not set maximal zero point for penalty function "
                            + which + ".");
                    func.BarrierLength = barrierLength;
                    func.BarrierHeight = barrierHeight;
                    func.MaxZero = zeroEnd;
                }
            }
        }

        /// <summary>Returns a flag that tells whether characteristic barrier length can be set for the 
        /// penalty function of the specified constraint.</summary>
        /// <param name="which">Specified the constraiint in question.</param>
        public virtual bool CanSetBarrierLength(int which)
        {
            lock (_lock)
            {
                IRealFunctionPenalty func = null;
                if (PenaltyFunctions.Count > which)
                    func = GetPenaltyFunction(which);
                if (func != null)
                    return func.CanSetBarrierLength;
                else
                    return true;
            }
        }

        /// <summary>Returns a flag that tells whether characteristic barrier height can be set for the 
        /// penalty function of the specified constraint.</summary>
        /// <param name="which">Specified the constraiint in question.</param>
        public virtual bool CansetBarrierHeight(int which)
        {
            lock (_lock)
            {
                IRealFunctionPenalty func = null;
                if (PenaltyFunctions.Count > which)
                    func = GetPenaltyFunction(which);
                if (func != null)
                    return func.CanSetBarrierHeight;
                else
                    return true;
            }
        }

        /// <summary>Returns a flag that tells whether maxmal argument where function is zero can be set for the 
        /// penalty function of the specified constraint.</summary>
        /// <param name="which">Specified the constraiint in question.</param>
        public virtual bool CanSetMaxZero(int which)
        {
            lock (_lock)
            {
                IRealFunctionPenalty func = null;
                if (PenaltyFunctions.Count > which)
                    func = GetPenaltyFunction(which);
                if (func != null)
                    return func.CanSetMaxZero;
                else
                    return true;
            }
        }


        /// <summary>Adds a new penalty function for evaluation of penalty terms to the list.</summary>
        /// <param name="func">Function to be added.</param>
        /// <returns>Index of the added penalty function (0 if this is the first function on the list).</returns>
        public int AddPenaltyFunction(IRealFunctionPenalty func)
        {
            lock (_lock)
            {
                PenaltyFunctions.Add(func);
                return PenaltyFunctions.Count - 1;
            }
        }

        /// <summary>Returns penalty function corresonding to the specified constraint.</summary>
        /// <param name="which">Index of penalty function (or the corresponding constraint).</param>
        public IRealFunctionPenalty GetPenaltyFunction(int which)
        {
            lock (_lock)
            {
                if (PenaltyFunctions.Count > which)
                    return PenaltyFunctions[which];
                else
                {
                    if (!AllowSingleFunction)
                        throw new IndexOutOfRangeException("Penalty evaluator: function No. " + which 
                            + " is not defined and single function option is off.");
                    if (PenaltyFunctions.Count < 1)
                        throw new IndexOutOfRangeException("No penalty functions defined on penalty evaluator.");
                    return PenaltyFunctions[0];
                }
            }
        }

        #endregion Penalty Functions


        /// <summary>Returns true if the penalty function value can be calculated 
        /// for the specified constraint, or false otherwise.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        public bool IsPenaltyValueDefined(int which)
        {
            return GetPenaltyFunction(which).ValueDefined;
        }

        /// <summary>Returns true if the penalty function derivative can be calculated 
        /// for the specified constraint, or false otherwise.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        public bool IsPenaltyDerivativeDefined(int which)
        {
            return GetPenaltyFunction(which).DerivativeDefined;
        }

        /// <summary>Returns true if the penalty function's second can be calculated 
        /// for the specified constraint, or false otherwise.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        public bool IsPenaltySecodDerivativeDefined(int which)
        {
            return GetPenaltyFunction(which).SecondDerivativeDefined;
        }


        /// <summary>Returns value of the penalty function for the specified constraint at the
        /// specified value of the corresponding constraint function.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <param name="constraintFunctionValue">Value of constraint function.</param>
        public double PenaltyValue(int which, double constraintFunctionValue)
        {
            return GetPenaltyFunction(which).Value(constraintFunctionValue);
        }

        /// <summary>Returns derivative of the penalty function for the specified constraint at the
        /// specified value of the corresponding constraint function, with respect to constraint value.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <param name="constraintFunctionValue">Value of constraint function.</param>
        public double PenaltyDerivative(int which, double constraintFunctionValue)
        {
            return GetPenaltyFunction(which).Derivative(constraintFunctionValue);
        }

        /// <summary>Returns second derivative of the penalty function for the specified constraint at the
        /// specified value of the corresponding constraint function, with respect to constraint value.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <param name="constraintFunctionValue">Value of constraint function.</param>
        public double PenaltySecondDerivative(int which, double constraintFunctionValue)
        {
            return GetPenaltyFunction(which).SecondDerivative(constraintFunctionValue);
        }

    }  // class PenaltyEvaluatorBase


}


