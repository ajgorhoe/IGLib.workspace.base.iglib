// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// EXAMPLE SCRIPT FOR REAL FUNCTION DYNAMICALLY GENERATED FROM STRING DEFINITION via loadable script.
// This script is used for dynamically loading th


using System;

using IG.Lib;
using IG.Num;

namespace IG.Script
{

    /// <summary>Example script for definition of a new real function class.</summary>
    public class ScritpRealFunctionExample : LoadableScriptRealFunctionBase, ILoadableScript
    {

        public override LoadableRealFunctionBase CreateRealFunction()
        {
            return new Container.Function();
        }

        public override LoadableRealFunctionBase CreateRealFunction(double Kx, double Sx)
        {
            return new Container.Function(Kx, Sx);
        }

        public override LoadableRealFunctionBase CreateRealFunction(double Kx, double Sx, double Ky, double Sy)
        {
            return new Container.Function(Kx, Sx, Ky, Sy);
        }


        /// <summary>Container class inherits from M in order to enable use of comfortable mathematical functions.</summary>
        public class Container : M
        {

            public class Function : LoadableRealFunctionBase
            {

                #region Construction
                public Function() : base() { }
                public Function(double Kx, double Sx) : base(Kx, Sx) { }
                public Function(double Kx, double Sx, double Ky, double Sy) : base(Kx, Sx, Ky, Sy) { }
                #endregion construction

                #region Generated

                protected override void InitDynamic()
                {
                    _returnedValueName = "ret";
                    _functionArgumentName = "arg";
                    _independentVariableName = "x";
                    _valueDefinitionString = "x*x + sin(x)";
                    _derivativeDefinitionString = "2*x + cos(x)";
                    _secondDerivativeDefinitionString = "2 - sin(x)";
                    _integralDefinitionString = null;
                    _inverseDefinitionString = null;

                    _valueDefined = true;
                    _derivativeDefined = true;
                    _secondDerivativeDefined = true;
                    _integralDefined = false;
                    _inverseDefined = false;
                }

                protected override double RefValue(double arg) 
                {
                    double x = arg;
                    double ret;
                    ret = zero + x*x + Math.Sin(x);
                    return (double) ret; 
                }

                protected override double RefDerivative(double arg)
                {
                    double x = arg;
                    double ret;
                    ret = zero + 2*x + cos(x);
                    return (double) ret; 
                }

                protected override double RefSecondDerivative(double arg)
                {
                    double x = arg;
                    double ret;
                    ret = zero + 2 - sin(x);
                    return (double) ret; 
                }

                //protected override double RefIntegral(double x) { return base.RefIntegral(x); }
                //protected override double RefInverse(double y) { return base.RefInverse(y); }


                #endregion Generated

            }  // class Container.Function

        }  // class Container

    }  // class ScritpRealFunction

}

