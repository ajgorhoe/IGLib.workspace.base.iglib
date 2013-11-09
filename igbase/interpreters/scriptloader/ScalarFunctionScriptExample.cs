// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// EXAMPLE SCRIPT FOR REAL FUNCTION DYNAMICALLY GENERATED FROM STRING DEFINITION via loadable script.
// This script is used for dynamically loading th


using System;

using IG.Lib;
using IG.Num;


namespace IG.Script
{

    /// <summary>Example script for definition of a new real function class.</summary>
    /// $A Igor Jun10 Aug10;
    public class ScritpScalarFunctionExample : LoadableScriptScalarFunctionBase, ILoadableScript
    {

        /// <summary>Creates  and returns a new scalar function object where functions are defined by compiled user defined strings.</summary>
        public override LoadableScalarFunctionBase CreateScalarFunction()
        {
            return new Container.Function();
        }

        /// <summary>Creates  and returns a new scalar function object where functions are defined by compiled user defined strings,
        /// and with affine transformation of parameters.</summary>
        /// <param name="transf">Affine transformation used to transform function parameters from the reference to the actual frame.</param>
        /// $A Igor Sep11;
        public override LoadableScalarFunctionBase CreateScalarFunction(IAffineTransformation transf)
        {
            return new Container.Function(transf);
        }

        /// <summary>Container class inherits from M in order to enable use of comfortable mathematical functions.</summary>
        /// $A Igor Jun10 Aug10;
        public class Container : M
        {

            public class Function : LoadableScalarFunctionBase
            {

                #region Construction
                public Function() : base() { }
                public Function(IAffineTransformation transf): base(transf) { }
                #endregion construction

                #region Generated

                protected override void InitDynamic()
                {
                    _numParam = 2;
                    _independentVariableNames = new string[] {"x", "y"};
                    _returnedValueName = "ret";
                    _functionArgumentParametersName = "parameters";
                    _functionArgumentGradientName = "gradient";
                    _functionArgumentHessianName = "hessian";
                    _valueDefinitionString = "pow(x,2) + y";
                    _gradientDefinitionStrings = new string[] {
                        "2 * x" /* grad_0 */,
                        "1" /* grad_1 */
                    };
                    _hessianDefinitionStrings = new string[][] {
                        new string [] { /* hessian_1 */
                            "2", /* hessian_0_0 */
                            "0" /* hessian_0_1 */
                        },
                        new string[] { /* hessian_1 */
                            null, /* hessian_1_0 */
                            "0" /* hessian_1_1 */
                        }
                    };

                    _numParam = 2;

                    _valueDefined = true;
                    _gradientDefined = true;
                    _hessianDefined = false;
                }

                public override double ReferenceValue(IVector parameters) 
                {
                    if (parameters == null)
                        throw new ArgumentException("Vector of function parameters is not specified (null reference)." );
                    if (parameters.Length != _numParam)
                        throw new ArgumentException("Vector of function parameters has wrong dimension (" 
                            + parameters.Length + " instead of " + _numParam + ").");
                    double x = parameters[0];
                    double y = parameters[1];
                    double ret;
                    ret = zero + pow(x,2) + y;
                    return (double) ret;
                }


                public override void ReferenceGradientPlain(IVector parameters, IVector gradient) 
                {
                    double x = parameters[0];
                    double y = parameters[1];
                    double ret;
                    ret = zero + 2 * x;
                    gradient[0] = (double) ret;
                    ret = zero + 1;
                    gradient[1] = (double) ret;
                }

                public override void ReferenceHessianPlain(IVector parameters, IMatrix hessian) 
                {
                    double x = parameters[0];
                    double y = parameters[1];
                    double ret;
                    // Calculate components of Hessian that are defined:
                    ret = zero + 2;
                    hessian[0, 0] = (double) ret;
                    ret = zero + 0;
                    hessian[0, 1] = (double) ret;
                    ret = zero + 0;
                    hessian[1, 1] = (double) ret;
                    // Calculate components of Hessian that are not defined by symmetry:
                    hessian[1,0] = hessian[0, 1];
                }

                #endregion Generated

            }  // class Container.Function

        }  // class Container

    }  // class ScritpScalarFunction

}

