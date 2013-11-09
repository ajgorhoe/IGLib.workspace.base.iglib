// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Information about optimization data and algorithm parameters.</summary>
    /// <remarks>Properties on this kind of classes usually have public getters and setters.
    /// It is a habit to protect the whole object inside another class, and access individual
    /// components by properties and methods that can be more restrictive.</remarks>
    /// $A Igor Jan08 Jun08;
    public interface IOptimizationData
    {

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


        #region Operation

        /// <summary>Indicates whether just references can be copied when setting optimization
        /// parameters or results or auxiliary data.
        /// If false then deep copy is always be performed.
        /// Default is false.</summary>
        bool CopyReferences { get; set; }

        #endregion Operation


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

    }  // interface IOptimizationData

    /// <summary>Base class for holding information about optimization data and algorithm parameters.</summary>
    /// <remarks>Properties on this kind of classes usually have public getters and setters.
    /// It is a habit to protect the whole object inside another class, and access individual
    /// components by properties and methods that can be more restrictive.</remarks>
    /// $A Igor Jan08 Jun08;
    public abstract class OptimizationDataBase: IOptimizationData
    {


        #region Characteristics

        protected int _numParameters = -1;

        protected int _numObjectives = -1;

        protected int _numConstraints = -1;

        protected int _numEqualityConstraints = -1;

        /// <summary>Number of parameters.</summary>
        public int NumParameters
        {
            get
            {
                return _numParameters;
                // if (Analysis == null)
                //    return _numParameters;
                //else
                //    return Analysis.NumParameters;
            }
            set { _numParameters = value; }
        }

        /// <summary>Number of objective functions (normally 1 for this type, but can be 0).</summary>
        public int NumObjectives
        {
            get
            {
                return _numObjectives;
                //if (Analysis == null)
                //    return _numObjectives;
                //else
                //    return Analysis.NumObjectives;
            }
            set { _numObjectives = value; }
        }

        /// <summary>Number of constraints.</summary>
        public int NumConstraints
        {
            get
            {
                return _numConstraints;
                //if (Analysis == null)
                //    return _numConstraints;
                //else
                //    return Analysis.NumConstraints;
            }
            set { _numConstraints = value; }
        }

        /// <summary>Number of equality constraints.</summary>
        public int NumEqualityConstraints
        {
            get
            {
                return _numEqualityConstraints;
                //if (Analysis == null)
                //    return _numEqualityConstraints;
                //else 
                //    return Analysis.NumEqualityConstraints;
            }
            set { _numEqualityConstraints = value; }
        }

        #endregion Characteristics


        #region Operation

        // OPERATION PARAMETERS:

        protected bool _copyReferences = false;

        /// <summary>Indicates whether just references can be copied when setting optimization
        /// parameters or results.
        /// If false then deep copy is always be performed.
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

                    InitialGuess = InitialGuess;
                    InitialStep = InitialStep;
                    //Results = Results;
                    //CurrentGuess = CurrentGuess;
                    //BestGuess = BestGuess;

                }
            }
        }

        #endregion Operation


        #region Input


        protected IVector _initialGuess;

        /// <summary>Gets or sets initial guess used in optimization.</summary>
        public IVector InitialGuess
        {
            get { return _initialGuess; }
            set
            {
                // Calculated = false;
                if (CopyReferences || value == null)
                    _initialGuess = value;
                else
                    _initialGuess = value.GetCopy();
                if (value != null)
                {
                    if (NumParameters != 0)
                        if (value.Length != NumParameters)
                            throw new ArgumentException("Initial guess does not have the right dimension: "
                                + value.Length + " instead of " + NumParameters + ".");
                        else
                            NumParameters = value.Length;
                }
            }
        }

        protected IVector _initialStep;

        /// <summary>Gets or sets initial step used in optimization.</summary>
        public IVector InitialStep
        {
            get { return _initialGuess; }
            set
            {
                // Calculated = false;
                if (CopyReferences || value == null)
                    _initialStep = value;
                else
                    _initialStep = value.GetCopy();
                if (value != null)
                {
                    if (NumParameters != 0)
                        if (value.Length != NumParameters)
                            throw new ArgumentException("Initial guess does not have the right dimension: "
                                + value.Length + " instead of " + NumParameters + ".");
                        else
                            NumParameters = value.Length;
                }
            }
        }

        protected double _tolerance = 0.0;

        /// <summary>Gets or sets the main tolerance (its exact meaning depends on the algorithm in use).</summary>
        public double Tolerance { get { return _tolerance; } set { _tolerance = value; } }

        protected int _maxIterations = 0;

        /// <summary>Gets or sets maximal number of iterations.</summary>
        public int MaxIterations { get { return _maxIterations; } set { _maxIterations = value; } }

        protected int _maxAnalyses = 0;

        /// <summary>Gets or sets maximal number of analyses.</summary>
        public int MaxAnalyses { get { return _maxAnalyses; } set { _maxAnalyses = value; } }

        #endregion Input

    }  // abstract class OptimizationDataBase

    /// <summary>Information about optimization data and algorithm parameters.</summary>
    /// <remarks>Properties on this kind of classes usually have public getters and setters.
    /// It is a habit to protect the whole object inside another class, and access individual
    /// components by properties and methods that can be more restrictive.</remarks>
    /// $A Igor Jan08 Jun08;
    public class OptimizationData : OptimizationDataBase
    {

    }  // class OptimizationData


}