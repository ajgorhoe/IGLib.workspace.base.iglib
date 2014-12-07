// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IG.Num
{


    /// <summary>Single objective optimization analysis results.
    /// Used to transfer parameters input (e.g. vector of parameters, request flags)
    /// to the analysis function and to store analysis output results (e.g. objective and 
    /// constraint functions, their gradients, error codes, and flags indicating what 
    /// has actually been calculated).
    /// REMARKS:
    ///   Property CopyReferences specifies whether only references are copied when individial object
    /// fields are assigned and set (when the property is true), or values are actually copied
    /// (when false - deep copy). Each setter method also has the variant that always copies only
    /// the reference (function name appended by "Reference"). This makes possible to avoid duplication
    /// of allocated data and also to avoid having different data with the same references.
    ///   In the beginning of analysis functions, call ResetResults().</summary>
    /// $A Igor Jun08;
    public interface IAnalysisResults
    {

        #region Characteristics

        // CHARACTERISTICS (DIMENSIONS) OF THE OPTIMIZATION PROBLEM:

        /// <summary>Number of parameters.</summary>
        int NumParameters { get; set; }

        /// <summary>Number of objective functions (normally 1 for this type, but can be 0).</summary>
        int NumObjectives { get; set; }

        /// <summary>Number of constraints.</summary>
        int NumConstraints { get; set; }

        /// <summary>Number of equality constraints.</summary>
        int NumEqualityConstraints { get; set; }

        #endregion Characteristics

        #region Operation

        // OPERATION PARAMETERS:

        /// <summary>Indicates whether just references can be copied when setting optimization
        /// parameters or results.
        /// If false then deep copy is always be performed.
        /// Default is false.</summary>
        bool CopyReferences { get; set; }

        #endregion Operation

        #region Parameters

        // OPTIMIZATION PARAMETERS:

        /// <summary>Optimization parameters.
        /// If CopyReferences=true (false by default) then only the reference is copied when assigning.</summary>
        IVector Parameters { get; set; }

        /// <summary>Returns vector of optimization parameters.</summary>
        IVector GetParameters();

        /// <summary>Sets the vector of optimization parameters.
        /// If CopyReferences=true (false by default) then only the reference is copied.</summary>
        /// <param name="value">Value to be assigned.</param>
        void SetParameters(IVector value);

        /// <summary>Sets the vector of optimization parameters.
        /// Only the reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        void SetParametersReference(IVector reference);

        /// <summary>Returns specific optimization parameter.
        /// Throws exception if not defined or index out of bounds.</summary>
        /// <param name="index">Index of parameter to be returned (counting from 0).</param>
        double GetParameter(int index);

        /// <summary>Sets the specified optimization parameter.</summary>
        /// <param name="index">Index of parameter to be set (counting from 0).</param>
        /// <param name="value">Parameter value.</param>
        void SetParameter(int index, double value);

        #endregion Parameters

        #region Results

        // OPTIMIZATION RESULTS:

        // OBJECTIVE FUNCTION:

        /// <summary>Value of the objective function.</summary>
        double Objective { get; set; }

        /// <summary>Returns the value of the objective function.</summary>
        double GetObjective();

        /// <summary>Sets the value of the objective function.</summary>
        /// <param name="value">Value to be assigned to the objective function.</param>
        void SetObjective(double value);


        // OBJECTIVE FUNCTION GRADIENT:

        /// <summary>Objective function gradient.
        /// If CopyReferences=true (false by default) then only the reference is copied in assignments.</summary>
        IVector ObjectiveGradient { get; set; }

        /// <summary>Returns the objective function gradient.</summary>
        IVector GetObjectiveGradient();

        /// <summary>Sets the objective function gradient.
        /// If CopyReferences=true (false by default) then only the reference is copied.</summary>
        /// <param name="value">Value to be assigned.</param>
        void SetObjectiveGradient(IVector value);

        /// <summary>Sets the objective function gradient.
        /// Only the reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        void SetObjectiveGradientReference(IVector reference);

        /// <summary>Returns the specified component of the objective function gradient.</summary>
        /// <param name="index">Index of the component.</param>
        double GetObjectiveGradient(int index);

        /// <summary>Sets the specified component of the objective function gradient.</summary>
        /// <param name="index">Index of objective gradient component to be set.</param>
        /// <param name="value">Value of the objective gradient component.</param>
        void SetObjectiveGradient(int index, double value);


        // CONSTRAINT FUNCTIONS:

        /// <summary>Constraint function values.
        /// If CopyReferences=true (false by default) then only the list reference is copied in assignments.</summary>
        List<double> Constraints { get; set; }

        /// <summary>Returns a list of constraint function values.</summary>
        List<double> GetConstraints();

        /// <summary>Sets the list of constraint function values.
        /// If CopyReferences=true (false by default) then only the list reference is copied.</summary>
        /// <param name="values">Values of constraint functions.</param>
        void SetConstraints(List<double> values);

        /// <summary>Sets the list of constraint function values.
        /// Only the list reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        void SetConstraintsReference(List<double> reference);

        /// <summary>Returns the specified constraint function value.</summary>
        /// <param name="which">Specifies which constraint function to return (counting from 0).</param>
        double GetConstraint(int which);

        /// <summary>Sets the specified constraint function value.</summary>
        /// <param name="which">Specifies which constraint function is set (counting from 0).</param>
        /// <param name="value">Assigned value of the constraint function.</param>
        void SetConstraint(int which, double value);


        // CONSTRAINT FUNCTION GRADIENTS:

        /// <summary>Constraint function gradients.
        /// If CopyReferences=true (false by default) then only the list reference is copied in assignments.</summary>
        List<IVector> ConstraintGradients { get; set; }

        /// <summary>Returns a list of constraint function gradients.</summary>
        List<IVector> GetConstraintGradients();

        /// <summary>Sets constraint function gradients.
        /// If CopyReferences=true (false by default) then only the list reference is copied.</summary>
        /// <param name="values">Value to be assigned.</param>
        void SetConstraintGradients(List<IVector> values);

        /// <summary>Sets constraint function gradients.
        /// Only the list reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        void SetConstraintGradientsReference(List<IVector> reference);

        /// <summary>Returns the gradient of the specified constraint function.</summary>
        /// <param name="which">Specifies which constraint function to take (couonted from 0).</param>
        IVector GetConstraintGradient(int which);

        /// <summary>Returns the specific constraint function gradient component.</summary>
        /// <param name="which">Specifies which constraint function to take (couonted from 0).</param>
        /// <param name="index">Specifies gradient component (conted form 0).</param>
        double GetConstraintGradient(int which, int index);

        /// <summary>Sets the specified constraint function gradient.
        /// If CopyReferences=true (false by default) then only the reference is copied.</summary>
        /// <param name="which">Specifies which constraint function to take (couonted from 0).</param>
        /// <param name="value">Value of the gradient to be assigned.</param>
        void SetConstraintGradient(int which, IVector value);

        /// <summary>Sets the specified constraint function gradient.
        /// Only the reference is copied.</summary>
        /// <param name="which">Specifies which constraint function to take (couonted from 0).</param>
        /// <param name="reference">Gradient reference to be assigned.</param>
        void SetConstraintGradientReference(int which, IVector reference);

        /// <summary>Sets the specified constraint fuction gradient component.</summary>
        /// <param name="which">Specifies which constraint function to take (couonted from 0).</param>
        /// <param name="index">Specified index of gradient coponent to be set.</param>
        /// <param name="value">Value to be assigned to the specified component.</param>
        void SetConstraintGradient(int which, int index, double value);


        // OBJECTIVE FUNCTION HESSIAN:

        /// <summary>Objective function Hessian (matrix of second derivatives).
        /// If CopyReferences=true (false by default) then only the reference is copied in assignments.</summary>
        IMatrix ObjectiveHessian { get; set; }

        /// <summary>Returns the objective function's Hessian.</summary>
        IMatrix GetObjectiveHessian();

        /// <summary>Sets the objective functions' Hessian.
        /// If CopyReferences=true (false by default) then only the reference is copied.</summary>
        /// <param name="value">Value to be assigned.</param>
        void SetObjectiveHessian(IMatrix value);

        /// <summary>Sets the objective functions' Hessian.
        /// Only the reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        void SetObjectiveHessianReference(IMatrix reference);

        /// <summary>Returns the specified component of the objective function Hessian.</summary>
        /// <param name="rowIndex">Row index of the component (counting from 0).</param>
        /// <param name="columnIndex">Column index of the component (counting from 0).</param>
        double GetObjectiveHessian(int rowIndex, int columnIndex);

        /// <summary>Sets the specified component of the objective function's Hessian.</summary>
        /// <param name="rowIndex">Row index of the component (counting from 0).</param>
        /// <param name="columnIndex">Column index of the component (counting from 0).</param>
        /// <param name="value">Value to be assigned to Hessian.</param>
        void SetObjectiveHessian(int rowIndex, int columnIndex, double value);


        // CONSTRAINT FUNCTION HESSIANS:

        /// <summary>Constraint functions' Hessians (matrices of second derivatives).
        /// If CopyReferences=true (false by default) then only the list reference is copied in assignments.</summary>
        List<IMatrix> ConstraintHessians { get; set; }

        /// <summary>Returns the list of constraint functions' Hessians.</summary>
        List<IMatrix> GetConstraintHessians();

        /// <summary>Sets constraint functios' Hessians.
        /// If CopyReferences=true (false by default) then only the list reference is copied.</summary>
        /// <param name="values">List of Hessians to be assigned.</param>
        void SetConstraintHessians(List<IMatrix> values);

        /// <summary>Sets constraint functios' Hessians.
        /// Only the list reference is copied.</summary>
        /// <param name="reference">Reference to be assigned.</param>
        void SetConstraintHessiansReference(List<IMatrix> reference);

        /// <summary>Returns Hessian of the specified constraint function.</summary>
        /// <param name="which">Specifies which constraint function it applies to (counting from 0).</param>
        IMatrix GetConstraintHessian(int which);

        /// <summary>Returns the specified component of Hessian of the specified constraint function.</summary>
        /// <param name="which">Specifies which constraint function it applies to (counting from 0).</param>
        /// <param name="rowIndex">Row index of the component (counting from 0).</param>
        /// <param name="columnIndex">Column index of the component (counting from 0).</param>
        double GetConstraintHessian(int which, int rowIndex, int columnIndex);

        /// <summary>Sets the specified constraint function's Hessian.
        /// If CopyReferences=true (false by default) then only the reference is copied.</summary>
        /// <param name="which">Specifies which constraint function it applies to (counting from 0).</param>
        /// <param name="value">Hessian matrix to be assigned.</param>
        void SetConstraintHessian(int which, IMatrix value);

        /// <summary>Sets the specified constraint function's Hessian.
        /// Only the reference is copied.</summary>
        /// <param name="which">Specifies which constraint function it applies to (counting from 0).</param>
        /// <param name="reference">Hessian matrix reference to be assigned.</param>
        void SetConstraintHessianReference(int which, IMatrix reference);

        /// <summary>Sets the specified component of the specified constraint's Hessian.</summary>
        /// <param name="which">Specifies which constraint function it applies to (counting from 0).</param>
        /// <param name="rowIndex">Row index of the component (counting from 0).</param>
        /// <param name="columnIndex">Column index of the component (counting from 0).</param>
        /// <param name="value">Value to be set.</param>
        void SetConstraintHessian(int which, int rowIndex, int columnIndex, double value);

        #endregion Results

        #region Allocation

        // ALLOCATION OF FIELDS:

        /// <summary>Sets all objects (parameter and result objects) to null.
        /// If the references are not assigned elsewhere, these objects become eligible for garbage collection.</summary>
        void NullifyAll();

        /// <summary>Sets all result objects to null.
        /// If the references are not assigned elsewhere, these objects become eligible for garbage collection.</summary>
        void NullifyResults();

        /// <summary>Allocates space for parameters.</summary>
        void AllocateParameters();

        /// <summary>Allocates space for all requested result objects.</summary>
        void AllocateRequested();

        /// <summary>Allocate space for objective function gradient.</summary>
        void AllocateObjectiveGradient();

        /// <summary>Allocates space for objective function Hessian.</summary>
        void AllocateObjectiveHessian();

        /// <summary>Allocates space for list of constraint functions.</summary>
        void AllocateConstraintsList();

        /// <summary>Allocates space for list of constraint function gradients.</summary>
        void AllocateConstraintGradientsList();

        /// <summary>Allocates space for constraint function gradients (including for the list, if necessarty).</summary>
        void AllocateConstrainGradients();

        /// <summary>Allocates space for the specified constraint function gradient.</summary>
        /// <param name="which">Specifies which constraint function it applies to (countinf form 0).</param>
        void AllocateConstraintGradient(int which);

        /// <summary>Allocates space for the list of constraint functions' Hessians.</summary>
        void AllocateConstraintHessiansList();

        /// <summary>Allocates space for constraint functions' Hessians (including space for the list, if necessary).</summary>
        void AllocateConstraintHessians();

        /// <summary>Allocates space for the specified constraint fucnction's Hessian.</summary>
        /// <param name="which">Specifies which constraint function it applies to (conting form 0).</param>
        void AllocateConstraintHessian(int which);

        #endregion Allocation

        #region Flags

        // REQUEST FLAGS:

        /// <summary>Indicates whether calculation of objective function is/was requested.</summary>
        bool ReqObjective { get; set; }

        /// <summary>Indicates whether calculation of objective function gradient is/was requested.</summary>
        bool ReqObjectiveGradient { get; set; }

        /// <summary>Indicates whether calculation of objective function Hessian is/was requested.</summary>
        bool ReqObjectiveHessian { get; set; }

        /// <summary>Indicates whether calculation of constraint functions is/was requested.</summary>
        bool ReqConstraints { get; set; }

        /// <summary>Indicates whether calculation of constraint functions gradient is/was requested.</summary>
        bool ReqConstraintGradients { get; set; }

        /// <summary>Indicates whether calculation of constraint functions Hessian is/was requested.</summary>
        bool ReqConstraintHessians { get; set; }


        // CALCULATED FLAGS:

        /// <summary>Sets all calculated flags to false, error code to 0 (no error) and error string to null.</summary>
        void ResetResults();

        /// <summary>Error code.
        ///   0 - everything is OK.
        ///   negative value - something went wrong.</summary>
        int ErrorCode { get; set; }

        /// <summary>Error string indicating what went wrong.</summary>
        String ErrorString { get; set; }

        /// <summary>Collectively gets or sets calculated flags.
        /// Set false: all calculated flags are set to false.
        /// Set true: all calculated flags for which the corresponding request flags are true, 
        ///     are set to truee, others are set to false.
        /// Get: returns true if all the flags for which the corresponding request flags are true,
        ///     are also true. Otherwise returns false.
        /// </summary>
        bool Calculated { get; set; }

        /// <summary>Indicates whether calculation of objective function is/was requested.</summary>
        bool CalculatedObjective { get; set; }

        /// <summary>Indicates whether calculation of objective function gradient is/was requested.</summary>
        bool CalculatedObjectiveGradient { get; set; }

        /// <summary>Indicates whether calculation of objective function Hessian is/was requested.</summary>
        bool CalculatedObjectiveHessian { get; set; }

        /// <summary>Indicates whether calculation of constraint functions is/was requested.</summary>
        bool CalculatedConstraints { get; set; }

        /// <summary>Indicates whether calculation of constraint functions gradient is/was requested.</summary>
        bool CalculatedConstraintGradients { get; set; }

        /// <summary>Indicates whether calculation of constraint functions Hessian is/was requested.</summary>
        bool CalculatedConstraintHessians { get; set; }

        #endregion Flags

        #region Helper methods

        /// <summary>Sets the dimension of the analysis results object according to the
        /// specified values.
        /// <para>Number of objective functions is set to 1.</para></summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        void SetDimensions(int numParameters, int numConstraints);


        /// <summary>Sets the dimension of the analysis results object according to the
        /// specified values.</summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numObjectives">Number of objective functions.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        void SetDimensions(int numParameters, int numObjectives, int numConstraints);

        /// <summary>Prepares data storage for results (allocates the appropriate vectors, matrices, lists,
        /// etc.) according to the numbers of parameters, constraint functions, and request flags, and
        /// resets calculation flags to false.
        /// <para>Things whose calculation is not requested are not calculated.</para></summary>
        void PrepareResultStorage();

        /// <summary>Prepares data storage for results (allocates the appropriate vectors, matrices, lists,
        /// etc.) according to the numbers of parameters, constraint functions, and request flags.
        /// <para>Things whose calculation is not requested are not calculated.</para></summary>
        /// <param name="resetCalculatedFlags">Whether the calculation flags are reset to false or not.</param>
        void PrepareResultStorage(bool resetCalculatedFlags);

        /// <summary>Prepares data storage for results (allocates the appropriate vectors, matrices, lists,
        /// etc.) according to the numbers of parameters, constraint functions, and request flags, and
        /// resets calculation flags to false.
        /// <para>This method also sets dimensions before preparing the storage (i.e. number of parameters
        /// and constraints while number of objective functions is set to 1).</para>
        /// <para>Things whose calculation is not requested are not calculated.</para></summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        void PrepareResultStorage(int numParameters, int numConstraints);

        /// <summary>Prepares data storage for results (allocates the appropriate vectors, matrices, lists,
        /// etc.) according to the numbers of parameters, constraint functions, and request flags, and
        /// resets calculation flags to false.
        /// <para>This method also sets dimensions before preparing the storage (i.e. number of parameters,
        /// objective functions and constraints).</para>
        /// <para>Things whose calculation is not requested are not calculated.</para></summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numObjectives">Number of objective functions.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        void PrepareResultStorage(int numParameters, int numObjectives, int numConstraints);
        
        /// <summary>Prepares data storage for results (allocates the appropriate vectors, matrices, lists,
        /// etc.) according to the numbers of parameters, constraint functions, and request flags.
        /// <para>This method also sets dimensions before preparing the storage (i.e. number of parameters,
        /// objective functions and constraints).</para>
        /// <para>Things whose calculation is not requested are not calculated.</para></summary>
        /// <param name="numParameters">Number of parameters.</param>
        /// <param name="numObjectives">Number of objective functions.</param>
        /// <param name="numConstraints">Number of constraints.</param>
        /// <param name="resetCalculatedFlags">Whether the calculation flags are reset to false or not.</param>
        void PrepareResultStorage(int numParameters, int numObjectives, int numConstraints, bool resetCalculatedFlags);

        /// <summary>Copies data from another analysis results.</summary>
        /// <param name="results">Analysis results which data is copied from.</param>
        void Copy(IAnalysisResults results);

        /// <summary>Returns an exact deep copy of the current object.</summary>
        IAnalysisResults GetCopy();

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
        bool IsViolated(int which, double equalityTolerance);

        /// <summary>Returns true if the specified constraint is violated according to the current 
        /// analysis results, false otherwise.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <returns>True if the constraint is violated, false if not. Constraint is violated when
        /// the corresponding constraint function is greater than 0.</returns>
        /// <exception cref="IndexOutOfRangeException">when constraint index is smaller lesser than 0 or
        /// greater than the number of constraints.</exception>
        /// <exception cref="InvalidOperationException">when constraints are not evaluated.</exception>
        bool IsViolated(int which);

        /// <summary>Returns true if the current analysis results represent a feasible point.
        /// Feasible point is one where no constraints are violated.
        /// For unconstraint problems this method always returns true.</summary>
        /// <returns>True if the current analysis results represent a feasible point, false otherwise.</returns>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        bool IsFeasible(double equalityTolerance);

        /// <summary>Returns true if the current analysis results represent a feasible point.
        /// Feasible point is one where no constraints are violated.
        /// For unconstraint problems this method always returns true.
        /// </summary>
        /// <returns>True if the current analysis results represent a feasible point, false otherwise.</returns>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        bool IsFeasible();

        /// <summary>Returns number of violated constraints in the current analysis results.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <param name="equalityTolerance">Tolerance for violation of equality constraints.
        /// If constraint function corresponding to equality conatraint is less or equal than tolerance then
        /// the corresponding constraint is considered non-violated.</param>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        int GetNumViolatedConstraints(double equalityTolerance);
        /// <summary>Returns number of violated constraints in the current analysis results.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        int GetNumViolatedConstraints();

        /// <summary>Returns sum of constraint function values corresponding to violated constraints 
        /// in the current analysis results.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <param name="equalityTolerance">Tolerance for violation of equality constraints.
        /// If constraint function corresponding to equality conatraint is less or equal than tolerance then
        /// the corresponding constraint is considered non-violated.</param>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        double GetSumResiduals(double equalityTolerance);

        /// <summary>Returns sum of constraint function values corresponding to violated constraints 
        /// in the current analysis results.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        double GetSumResiduals();


        /// <summary>Returns the largest constraint function value corresponding to any violated constraint 
        /// in the current analysis results, or 0 if there are no violated constraints.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <param name="equalityTolerance">Tolerance for violation of equality constraints.
        /// If constraint function corresponding to equality conatraint is less or equal than tolerance then
        /// the corresponding constraint is considered non-violated.</param>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        double GetMaximalResidual(double equalityTolerance);

        /// <summary>Returns sum of constraint function values corresponding to violated constraints 
        /// in the current analysis results.
        /// Specific constraint is violated if the corresponding constraint function is greater than 0.</summary>
        /// <exception cref="InvalidOperationException">If constraint optimization results do not contain
        /// evaluated constraint values.</exception>
        double GetMaximalResidual();

        /// <summary>Returns value of the penalty term corresponding to the specified constraint, 
        /// calculated by the specified penalty evaluator.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <param name="evaluator">Penalty function evaluator that evaluates penalty functions.</param>
        double GetPenaltyTerm(int which, IPenaltyEvaluator evaluator);

        /// <summary>Returns sum of the penalty terms corresponding to all constraint, 
        /// calculated by the specified penalty evaluator.</summary>
        /// <param name="evaluator">Penalty function evaluator that evaluates penalty functions.</param>
        double GetSumPenaltyTerms(IPenaltyEvaluator evaluator);

        /// <summary>Returns sum of the penalty terms corresponding to all constraint, 
        /// calculated by the specified penalty evaluator.</summary>
        /// <param name="evaluator">Penalty function evaluator that evaluates penalty functions.</param>
        double GetMaxPenaltyTerm(IPenaltyEvaluator evaluator);
        
        #endregion Helper methods

    } // interface IAnalysisResults


    /// <summary>Classes that evaluates penalty terms corresponding to a specific penalty function.</summary>
    public interface IPenaltyEvaluator
    {

        /// <summary>Whether a single function can be used for evaluating penalty terms
        /// corresponding to different constraints.</summary>
        bool AllowSingleFunction { get; set; }


        #region Penalty Functions

 
        /// <summary>Returns a list of penalty functions used to evaluate penalty terms.</summary>
        List<IRealFunctionPenalty> PenaltyFunctions
        { get; }

        /// <summary>Sets the penalty function used for evaluation of the specified penalty term.</summary>
        /// <param name="which">Specifies which conatraint the penalty function applies to (zero based).
        /// In order to use one penalty function for all constraints, just set the penalty function
        /// with index 0.</param>
        /// <param name="function">Function to be used for evaluation of the specified penalty term.</param>
        void SetPenaltyFunction(int which, IRealFunctionPenalty function);

        /// <summary>Sets the penalty function used for evaluation of the specified penalty term.</summary>
        /// <param name="which">Specifies which conatraint the penalty function applies to (zero based).
        /// In order to use one penalty function for all constraints, just set the penalty function
        /// with index 0.</param>
        /// <param name="barrierLength">Characteristic barrier length of the created function.
        /// This is the length at which function grows from approximately 0 to characteristic height.</param>
        /// <param name="barrierHeight">Characteristic barrier height of the created function.</param>
        void SetPenaltyFunction(int which, double barrierLength, double barrierHeight);

        /// <summary>Sets the penalty function used for evaluation of the specified penalty term.</summary>
        /// <param name="which">Specifies which conatraint the penalty function applies to (zero based).
        /// In order to use one penalty function for all constraints, just set the penalty function
        /// with index 0.</param>
        /// <param name="barrierLength">Characteristic barrier length of the created function.
        /// This is the length at which function grows from approximately 0 to characteristic height.</param>
        /// <param name="barrierHeight">Characteristic barrier height of the created function.</param>
        /// <param name="zeroEnd">Maximal argumet for which penalty function is still 0.</param>
        void SetPenaltyFunction(int which, double barrierLength, double barrierHeight,
            double zeroEnd);

        /// <summary>Returns a flag that tells whether characteristic barrier length can be set for the 
        /// penalty function of the specified constraint.</summary>
        /// <param name="which">Specified the constraiint in question.</param>
        bool CanSetBarrierLength(int which);

        /// <summary>Returns a flag that tells whether characteristic barrier height can be set for the 
        /// penalty function of the specified constraint.</summary>
        /// <param name="which">Specified the constraiint in question.</param>
        bool CansetBarrierHeight(int which);

        /// <summary>Returns a flag that tells whether maxmal argument where function is zero can be set for the 
        /// penalty function of the specified constraint.</summary>
        /// <param name="which">Specified the constraiint in question.</param>
        bool CanSetMaxZero(int which);

        /// <summary>Adds a new penalty function for evaluation of penalty terms to the list.</summary>
        /// <param name="func">Function to be added.</param>
        /// <returns>Index of the added penalty function (0 if this is the first function on the list).</returns>
        int AddPenaltyFunction(IRealFunctionPenalty func);

        /// <summary>Returns penalty function corresonding to the specified constraint.</summary>
        /// <param name="which">Index of penalty function (or the corresponding constraint).</param>
        IRealFunctionPenalty GetPenaltyFunction(int which);


        #endregion Penalty Functions



        /// <summary>Returns true if the penalty function value can be calculated 
        /// for the specified constraint, or false otherwise.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        bool IsPenaltyValueDefined(int which);

        /// <summary>Returns true if the penalty function derivative can be calculated 
        /// for the specified constraint, or false otherwise.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        bool IsPenaltyDerivativeDefined(int which);

        /// <summary>Returns true if the penalty function's second can be calculated 
        /// for the specified constraint, or false otherwise.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        bool IsPenaltySecodDerivativeDefined(int which);


        /// <summary>Returns value of the penalty function for the specified constraint at the
        /// specified value of the corresponding constraint function.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <param name="constraintValue">Value of the constraint function for which penalty value is returned.</param>
        double PenaltyValue(int which, double constraintValue);

        /// <summary>Returns derivative of the penalty function for the specified constraint at the
        /// specified value of the corresponding constraint function, with respect to constraint value.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <param name="constraintValue">Value of the constraint function for which penalty derivative is returned.</param>
        double PenaltyDerivative(int which, double constraintValue);

        /// <summary>Returns second derivative of the penalty function for the specified constraint at the
        /// specified value of the corresponding constraint function, with respect to constraint value.</summary>
        /// <param name="which">Specifies the constraint in question.</param>
        /// <param name="constraintValue">Value of the constraint function for which penalty second derivative is returned.</param>
        double PenaltySecondDerivative(int which, double constraintValue);

    }  // interface IPenaltyEvaluator

}
