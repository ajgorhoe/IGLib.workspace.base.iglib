// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{


    /// <summary>Interface for direct analysis classes used in optimization problems.</summary>
    /// $A Igor May08 Jun10;
    public interface IAnalysis
    {

        #region Actions

        /// <summary>Performs analysis - calculates requested results and writes them
        /// to the provided data structure.</summary>
        /// <param name="analysisData">Data structure where analysis request parameters are
        /// obtained and where analysis results are written.</param>
        void Analyse(IAnalysisResults analysisData);

        #endregion Actions

        #region Characteristics

        /// <summary>Number of parameters.</summary>
        int NumParameters { get; set; }

        /// <summary>Number of objective functions (normally 1 for this type, but can be 0).</summary>
        int NumObjectives { get; set; }

        /// <summary>Number of constraints.</summary>
        int NumConstraints { get; set; }

        /// <summary>Number of equality constraints.</summary>
        int NumEqualityConstraints { get; set; }


        #endregion Characteristics

    }

    /// <summary>Base class for direct analysis classes that define optimization problems.
    /// Represent the complete definition of the optimization problem (without initial parameters).
    /// Concrete analysis classes (e.g. representing optimization examples or complex direct problems
    /// solved through numerical simulation) should be derived from this class.</summary>
    /// $A Igor May08 Jun10;
    public abstract class AnalysisBase : IAnalysis, ILockable
    {

        #region ILockable

        private object _lock = new object();

        public object Lock
        {
            get { return _lock; }
        }

        #endregion ILockable


        #region Actions

        /// <summary>Performs analysis - calculates requested results and writes them
        /// to the provided data structure.</summary>
        /// <param name="analysisData">Data structure where analysis request parameters are
        /// obtained and where analysis results are written.</param>
        public abstract void Analyse(IAnalysisResults analysisData);

        #endregion Actions

        #region Characteristics

        // Below are characteristic properties of the analysis, value -1 means that a certain property is 
        // currently unknown. 

        protected int _numParameters = -1;

        protected int _numObjectives = -1;

        protected int _numConstraints = -1;

        protected int _numEqualityConstraints = -1;

        /// <summary>Number of parameters.</summary>
        public virtual int NumParameters
        {
            get { lock (Lock) { return _numParameters; } }
            set { lock (Lock) { _numParameters = value; } }
        }

        /// <summary>Number of objective functions (normally 1 for this type, but can be 0).</summary>
        public virtual int NumObjectives
        {
            get { lock (Lock) { return _numObjectives; } }
            set { lock (Lock) { _numObjectives = value; } }
        }

        /// <summary>Number of constraints.</summary>
        public virtual int NumConstraints
        {
            get { lock (Lock) { return _numConstraints; } }
            set { lock (Lock) { _numConstraints = value; } }
        }

        /// <summary>Number of equality constraints.</summary>
        public virtual int NumEqualityConstraints
        {
            get { lock (Lock) { return _numEqualityConstraints; } }
            set { lock (Lock) { _numEqualityConstraints = value; } }
        }

        #endregion Characteristics


    }


}
