// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Interface for optimization algorithms.
    /// $A Igor Feb10;</summary>
    public interface IOptimizer: ILockable
    {

        #region Actions

        /// <summary>Performs optimization using initial data and problem definition contained in this object.</summary>
        void Optimize();

        #endregion Actions


        #region Operation

        /// <summary>Indicates whether just references can be copied when setting optimization
        /// parameters or results or auxiliary data.
        /// If false then deep copy is always be performed.
        /// Default is false.</summary>
        bool CopyReferences { get; set; }

        #endregion Operation

        #region Characteristics

        /// <summary>Number of parameters.</summary>
        int NumParameters { get; }

        /// <summary>Number of objective functions (normally 1 for this type, but can be 0).</summary>
        int NumObjectives { get; }

        /// <summary>Number of constraints.</summary>
        int NumConstraints { get; }

        /// <summary>Number of equality constraints.</summary>
        int NumEqualityConstraints { get; }

        #endregion Characteristics

        #region Analysis

        /// <summary>Definition of the direct problem (direct analysis that calculates the response functions).</summary>
        IAnalysis Analysis { get; set; }

        #endregion Analysis

        #region Input

        /// <summary>Gets or sets initial guess used in optimization.</summary>
        IVector InitialGuess { get; set; }

        /// <summary>Gets or sets initial step used in optimization.</summary>
        IVector InitialStep { get; set; }

        /// <summary>Gets or sets the main tolerance (its exact meaning depends on the algorithm in use).</summary>
        double Tolerance { get; set; }

        /// <summary>Gets or sets maximal number of iterations.</summary>
        int MaxIterations { get; set; }

        /// <summary>Gets or sets maximal number of analyses.</summary>
        int MaxAnalyses { get; set; }

        #endregion Input

        #region Results

        /// <summary>Optimization results.</summary>
        IAnalysisResults Results { get; }

        #endregion Results

        #region Auxiliary

        /// <summary>Whether current guess is kept or not.</summary>
        bool KeepCurrentGuess { get; }

        /// <summary>Results of the current guess (usually last analysis that has been performed).</summary>
        IAnalysisResults CurrentGuess { get; }

        /// <summary>Whether best results are kept or not.</summary>
        bool KeepBestGuess { get; }

        /// <summary>The best results so far.</summary>
        IAnalysisResults BestGuess { get;  }

        #endregion Auxiliary

    }

    public abstract class OptimizerBase : IOptimizer
    {

        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking


        #region Data

        IOptimizationData _optimizationData;

        /// <summary>Gets optimization data used by the current optimizer.
        /// This structure contains information about optimization problem and algorithm parameters.</summary>
        /// <remarks>Protected internal setter.
        /// Setter should be overridden by overriding the SetOptimizationData() method!</remarks>
        public virtual IOptimizationData OptimizationData
        {
            get { return _optimizationData; }
            protected internal set { SetOptimizationData(value); }
        }

        /// <summary>Sets the optimization data where information about optimization problem and algorithm parameters
        /// can be obtained.</summary>
        /// <param name="data"></param>
        public virtual void SetOptimizationData(IOptimizationData data)
        {
            if (data == null)
                throw new ArgumentNullException("Optimization data not specified (null reference).");
            _optimizationData = data;
        }

        IOptimizationResults _optimizationResults;

        /// <summary>Gets optimization results.</summary>
        /// <remarks>This property has protected internal setter.</remarks>
        public virtual IOptimizationResults OptimizationResults
        {
            get { return _optimizationResults; }
            protected internal set { SetOptimizationResults(value); }
        }

        /// <summary>Sets the optimization data where information about optimization problem and algorithm parameters
        /// can be obtained.</summary>
        /// <param name="results">object on which results are set.</param>
        public virtual void SetOptimizationResults(IOptimizationResults results)
        {

            _optimizationResults = results;
        }

        #endregion Data



        #region Actions

        /// <summary>Performs optimization.
        /// This method should be overridden in derived classes.</summary>
        /// <remarks> Methods BeforeOptimization() and AfterOptimization() should be called at the 
        /// beginning and end of this method.</remarks>
        public abstract void Optimize();

        /// <summary>Auxiliary housekeeping method that should be called at the beginning of Optimize.</summary>
        protected virtual void BeforeOptimization()
        {
            Calculated = false;
        }

        /// <summary>Auxiliary housekeeping method that should be called at the end of Optimize.</summary>
        protected virtual void AfterOptimization()
        {
            Calculated = true;
        }

        #endregion Actions


        #region Operation


        /// <summary>Indicates whether just references can be copied when setting optimization
        /// parameters or results.
        /// If false then deep copy is always be performed.
        /// Default is false.</summary>
        public virtual bool CopyReferences
        {
            get { return OptimizationData.CopyReferences; }
            set { OptimizationData.CopyReferences = value; }
        }

        #endregion Operation


        #region Characteristics


        /// <summary>Number of parameters.</summary>
        public int NumParameters
        {
            get { return OptimizationData.NumParameters; }
            protected set { OptimizationData.NumParameters = value; }
        }

        /// <summary>Number of objective functions (normally 1 for this type, but can be 0).</summary>
        public int NumObjectives
        {
            get { return OptimizationData.NumObjectives; }
            protected set { OptimizationData.NumObjectives = value; }
        }

        /// <summary>Number of constraints.</summary>
        public int NumConstraints
        {
            get { return OptimizationData.NumConstraints; }
            protected set { OptimizationData.NumConstraints = value; }
        }

        /// <summary>Number of equality constraints.</summary>
        public int NumEqualityConstraints
        { 
            get { return OptimizationData.NumEqualityConstraints; }
            protected set { OptimizationData.NumEqualityConstraints = value; }
        }

        #endregion Characteristics


        #region Analysis


        IAnalysis _analysis;

        /// <summary>Definition of the direct problem (direct analysis).</summary>
        public IAnalysis Analysis
        {
            get { return _analysis; }
            set
            {
                _analysis = value;
                if (value != null)
                {
                    if (value.NumParameters > -1)
                        NumParameters = value.NumParameters;
                    if (value.NumObjectives > -1)
                        NumObjectives = value.NumObjectives;
                    if (value.NumConstraints > -1)
                        NumConstraints = value.NumConstraints;
                    if (value.NumEqualityConstraints > -1)
                        NumEqualityConstraints = value.NumEqualityConstraints;
                }
            }
        }  // Analysis

        #endregion Analysis


        #region Input


         /// <summary>Gets or sets initial guess used in optimization.</summary>
        public IVector InitialGuess 
        {
            get { return OptimizationData.InitialGuess; }
            set { OptimizationData.InitialGuess = value; }
        }


        /// <summary>Gets or sets initial step used in optimization.</summary>
        public IVector InitialStep
        {
            get { return OptimizationData.InitialStep; }
            set { OptimizationData.InitialStep = value; }
        }


        /// <summary>Gets or sets the main tolerance (its exact meaning depends on the algorithm in use).</summary>
        public double Tolerance 
        { 
            get { return OptimizationData.Tolerance; }
            set { OptimizationData.Tolerance = value; }
        }

        /// <summary>Gets or sets maximal number of iterations.</summary>
        public int MaxIterations { 
            get { return OptimizationData.MaxIterations; }
            set { OptimizationData.MaxIterations = value; }
        }

        /// <summary>Gets or sets maximal number of analyses.</summary>
        public int MaxAnalyses 
        { 
            get { return OptimizationData.MaxAnalyses; }
            set { OptimizationData.MaxAnalyses = value; }
        }

        #endregion Input

        #region Results


        public bool Calculated
        { 
            get { return OptimizationResults.Calculated; }
            protected set { OptimizationResults.Calculated = value; }
        }


        /// <summary>Optimization results.</summary>
        public virtual IAnalysisResults Results 
        {
            get { return OptimizationResults.Results; }
            protected set { OptimizationResults.Results = value; }
       }

        #endregion Results

        #region Auxiliary


        /// <summary>Whether current guess is kept or not.</summary>
        public virtual bool KeepCurrentGuess { 
            get { return OptimizationResults.KeepCurrentGuess; }
            set { OptimizationResults.KeepCurrentGuess = value; }
        }


        /// <summary>Results of the current guess (usually last analysis that has been performed).</summary>
        public virtual IAnalysisResults CurrentGuess 
        {
            get { return OptimizationResults.CurrentGuess; }
            protected set { OptimizationResults.CurrentGuess = value; }
        }

        protected bool _keepBestGuess = false;

        /// <summary>Whether best results are kept or not.</summary>
        public virtual bool KeepBestGuess 
        { 
            get { return OptimizationResults.KeepBestGuess; }
            set { OptimizationResults.KeepBestGuess = value; }
        }


        /// <summary>The best results so far.</summary>
        public virtual IAnalysisResults BestGuess {
            get { return OptimizationResults.BestGuess; }
            protected set { OptimizationResults.BestGuess = value; }

        }

        #endregion Auxiliary



    }  // abstract class OptimizationBase

}