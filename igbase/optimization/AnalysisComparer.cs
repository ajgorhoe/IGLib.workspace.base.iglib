// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IG.Num
{

    #region Comparers



    /// <summary>Compares two sets of analysis results in various different ways.</summary>
    public class AnalysisComparer : IComparer<IAnalysisResults>
    {

        #region Initialization

        public AnalysisComparer()
        { 
            _cmpNumViolated = false;
            _cmpMaxResidual = false;
            _cmpSumResiduals = true;
            _cmpMaxPenalty = false;
            _cmpSumPenalties = false;
            _cmpObjectiveFunction = true;
        }

        /// <summary>Creates analysis results comparer with initialized penalty evaluator
        /// containing one penalty function (common for all constraints) initialized to the default 
        /// penalty function with specified characteristic barrier length and height.</summary>
        /// <param name="barrierLength">Characteristic barrrier length. Within this length the created penalty 
        /// function grows approximately from 0 to characteristic height.</param>
        /// <param name="barrierHeight">Characteristic barrier height.</param>
        public AnalysisComparer(double barrierLength, double barrierHeight) :
            this()
        { PenaltyEvaluator = new PenaltyEvaluator(barrierLength, barrierHeight); }

        /// <summary>Creates analysis results comparer with initialized penalty evaluator with penalty function (common for all constraints)
        /// initialized to the default penalty function with specified characteristic barrier length and
        /// height and transition point where penalty function becomes non-zero.</summary>
        /// <param name="barrierLength">Characteristic barrrier length. Within this length the created penalty 
        /// function grows approximately from 0 to characteristic height.</param>
        /// <param name="barrierHeight">Characteristic barrier height.</param>
        /// <param name="zeroEnd">Maximal argument for which the created function is 0.</param>
        public AnalysisComparer(double barrierLength, 
            double barrierHeight, double maxZero) : this()
        { PenaltyEvaluator = new PenaltyEvaluator(barrierLength, barrierHeight, maxZero); }




        #endregion Initialization



        #region Data

        protected double _equalityConstraintTolerance;

        /// <summary>Gets or sets tolerance for equality constraints.
        /// When absolute value of a constraint function is below this tolerance, the 
        /// corresponding constraint is not considered violated.
        /// Tolerance must not be negative.</summary>
        public double EqualityConstraintTolerance
        {
            get { return _equalityConstraintTolerance; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Analysis results comparer: equality constraint tolerance can not be negative.");
                _equalityConstraintTolerance = value;
            }
        }


        #region Penalty Evaluator

        IPenaltyEvaluator _penaltyEvaluator = null;

        /// <summary>Gets or sets evaluator of penalty terms.
        /// This is needed when e.g. sums of penalty terms are defined.</summary>
        IPenaltyEvaluator PenaltyEvaluator
        {
            get
            {
                if (_penaltyEvaluator == null)
                    _penaltyEvaluator = new PenaltyEvaluator();
                return _penaltyEvaluator;
            }
            set
            {
                _penaltyEvaluator = value;
            }
        }

        #endregion Penalty Evaluator

        #endregion Data


        #region Operation

        protected bool _cmpNumViolated = false;

        protected bool _cmpMaxResidual = false;

        protected bool _cmpSumResiduals = true;

        protected bool _cmpMaxPenalty = false;

        protected bool _cmpSumPenalties = false;

        protected bool _cmpObjectiveFunction = true;

        /// <summary>Whether number of violated constraints is compared.</summary>
        public bool CompareNumViolated
        {
            get { return _cmpNumViolated; }
            set { _cmpNumViolated = value; }
        }

        /// <summary>Whether maximal constraint residual is compared.</summary>
        public bool CompareMaxResidual
        {
            get { return _cmpMaxResidual; }
            set { _cmpMaxResidual = value; }
        }

        /// <summary>Whether sum of constraint residuals is compared.</summary>
        public bool CompareSumResiduals
        {
            get { return _cmpSumResiduals; }
            set { _cmpSumResiduals = value; }
        }

        /// <summary>Whether maximal penalty term is compared.</summary>
        public bool CompareMaxPenalty
        {
            get { return _cmpMaxPenalty; }
            set { _cmpMaxPenalty = value; }
        }

        /// <summary>Whether sum of penalty terms is compared.</summary>
        public bool CompareSumPenalties
        {
            get { return _cmpSumPenalties; }
            set { _cmpSumPenalties = value; }
        }

        /// <summary>Whether value of the objective function is compared.</summary>
        public bool CompareObjectiveFunction
        {
            get { return _cmpObjectiveFunction; }
            set { _cmpObjectiveFunction = value; }
        }

        #endregion Operation



        /// <summary>Compares two sets of analysis results points.</summary
        /// <param name="an1">First set of analysis results.</param>
        /// <param name="an2">Second set of analysis results.</param>
        /// <returns>0 if result sets are equal, -1 if the first set is smaller than the second, 
        /// and 1 if the first set is larger than the second.</returns>
        public int Compare(IAnalysisResults an1, IAnalysisResults an2)
        {
            if (an1 == null)
            {
                if (an2 != null)
                    return 1;
                else
                    return 0;
            }
            else if (an2 == null)
            {
                return -1;
            }
            else
            {
                if (CompareNumViolated)
                {
                    int nv1 = an1.GetNumViolatedConstraints();
                    int nv2 = an2.GetNumViolatedConstraints();
                    if (nv1 < nv2)
                        return -1;
                    else if (nv1 > nv2)
                        return 1;
                }
                if (CompareMaxResidual)
                {
                    double mr1 = an1.GetMaximalResidual(EqualityConstraintTolerance);
                    double mr2 = an2.GetMaximalResidual(EqualityConstraintTolerance);
                    if (mr1 < mr2)
                        return -1;
                    else if (mr1 > mr2)
                        return 1;
                }
                if (CompareSumResiduals)
                {
                    double sr1 = an1.GetMaximalResidual(EqualityConstraintTolerance);
                    double sr2 = an2.GetMaximalResidual(EqualityConstraintTolerance);
                    if (sr1 < sr2)
                        return -1;
                    else if (sr1 > sr2)
                        return 1;
                }
                if (CompareMaxPenalty)
                {
                    double maxpen1 = an1.GetMaxPenaltyTerm(this.PenaltyEvaluator);
                    double maxpen2 = an1.GetMaxPenaltyTerm(this.PenaltyEvaluator);
                    if (maxpen1 < maxpen2)
                        return -1;
                    else if (maxpen1 > maxpen2)
                        return 1;
                }
                if (CompareSumPenalties)
                {
                    double sumpen1 = an1.GetSumPenaltyTerms(this.PenaltyEvaluator);
                    double sumpen2 = an1.GetSumPenaltyTerms(this.PenaltyEvaluator);
                    if (sumpen1 < sumpen2)
                        return -1;
                    else if (sumpen1 > sumpen2)
                        return 1;
                }
                if (CompareObjectiveFunction)
                {
                    double obj1 = an1.Objective;
                    double obj2 = an1.Objective;
                    if (obj1 < obj2)
                        return -1;
                    else if (obj1 > obj2)
                        return 1;
                }
                return 0;
            }
        }
    }  // class AnalysisComparer


    #endregion Comparers



}
