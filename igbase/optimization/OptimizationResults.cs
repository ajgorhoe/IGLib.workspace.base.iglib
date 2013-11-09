// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Storage of optimization results.
    /// Stores optimal results, best current guess, etc.</summary>
    /// <remarks>Properties on this kind of classes usually have public getters and setters.
    /// It is a habit to protect the whole object inside another class, and access individual
    /// components by properties and methods that can be more restrictive.</remarks>
    /// $A Igor Feb08 Jun08;
    public interface IOptimizationResults
    {

        #region OptimizationData

        IOptimizationData OptimizationData
        { get; set; }

        bool Calculated
        { get; set; }

        #endregion OptimizationData

        #region Results

        /// <summary>Optimization results.</summary>
        IAnalysisResults Results { get; set; }

        #endregion Results

        #region Auxiliary

        /// <summary>Whether current guess is kept or not.</summary>
        bool KeepCurrentGuess { get; set; }

        /// <summary>Results of the current guess (usually last analysis that has been performed).</summary>
        IAnalysisResults CurrentGuess { get; set; }

        /// <summary>Whether best results are kept or not.</summary>
        bool KeepBestGuess { get; set; }

        /// <summary>The best results so far.</summary>
        IAnalysisResults BestGuess { get; set; }

        #endregion Auxiliary


    }


    /// <summary>Base class for storage of optimization results.
    /// Stores optimal analysis results, best current guess, etc.</summary>
    /// <remarks>Properties on this kind of classes usually have public getters and setters.
    /// It is a habit to protect the whole object inside another class, and access individual
    /// components by properties and methods that can be more restrictive.</remarks>
    /// $A Igor Feb08 Jun08;
    public abstract class OptimizationResultsBase
    {

        #region OptimizationData

        IOptimizationData _optimizationData;

        /// <summary>Gets optimization data used when producing the current results.</summary>
        /// <remarks>Protected internal setter.</remarks>
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
            _optimizationData = data;
        }

        /// <summary>Gets a flag indicating whether references should be copied.</summary>
        public bool CopyReferences
        { 
            get {
                if (OptimizationData == null)
                    return false;
                else
                    return OptimizationData.CopyReferences;
                } 
        }

        #endregion OptimizationData





        #region Results

        protected bool _calculated = false;

        public bool Calculated
        { 
            get { return _calculated; } 
            set { _calculated = value; } 
        }

        IAnalysisResults _results;

        /// <summary>Optimization results.</summary>
        public virtual IAnalysisResults Results
        {
            get { return _results; }
            set
            {

                // Calculated = false;
                if (CopyReferences || value == null)
                    _results = value;
                else
                {
                    if (_results == null)
                        _results = new AnalysisResults();
                    _results.Copy(value);
                }
            }
        }

        #endregion Results

        #region Auxiliary

        protected bool _keepCurrentGuess = false;

        /// <summary>Whether current guess is kept or not.</summary>
        public virtual bool KeepCurrentGuess { get { return _keepCurrentGuess; } set { _keepCurrentGuess = true; } }

        IAnalysisResults _currentGuess = null;

        /// <summary>Results of the current guess (usually last analysis that has been performed).</summary>
        public virtual IAnalysisResults CurrentGuess
        {
            get { return _currentGuess; }
            protected set
            {
                if (_currentGuess == null)
                    _currentGuess = new AnalysisResults();
                _currentGuess.Copy(value);
            }
        }

        protected bool _keepBestGuess = false;

        /// <summary>Whether best results are kept or not.</summary>
        public virtual bool KeepBestGuess { get { return _keepBestGuess; } set { _keepBestGuess = true; } }


        protected IAnalysisResults _buestGuess = null;

        /// <summary>The best results so far.</summary>
        public virtual IAnalysisResults BestGuess
        {
            get { return _buestGuess; }
            protected set
            {
                if (_buestGuess == null)
                    _buestGuess = new AnalysisResults();
                _buestGuess.Copy(value);
            }
        }

        #endregion Auxiliary


    }


    public class OptimizationResults: OptimizationResultsBase
    {

    }


}