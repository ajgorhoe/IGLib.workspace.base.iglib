using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{


    /// <summary>Scalar function of 3 variables that is generated from expressions for evaluation of function value, 
    /// gradient components, etc.</summary>
    /// $A Igor Oct11;
    public class Func3dFromScript : Func3dFromScalarFunction, IFunc3d, IScalarFunctionUntransformed
    {

        protected Func3dFromScript() { }

        /// <summary>Constructs a scalar function of 3 variables from the specified scalar function 
        /// of a vector argument.</summary>
        /// <param name="scalarFunction">Original scalar function of vector argument used for evaluation.</param>
        protected Func3dFromScript(IScalarFunction originalFunction)
            : base(originalFunction)
        { }


        // TODO: add different constructurs (with different sets of arguments) for convenience!

        // TODO: maybe add default variable names such as "x", "y", ..., and dd functions where you don't need to specify these

        /// <summary>Constructs a scalar function of 3 variables from the specified scalar function 
        /// of a vector argument.</summary>
        /// <param name="scalarFunction">Original scalar function of vector argument used for evaluation.</param>
        public Func3dFromScript(string varName1, string varName2, string varName3, string valueExpression,
                    string[] gradientExpressions, string[][] hessianExpressions) :
            base()
        {
            OriginalFunction = CreateScalarFunction(varName1, varName2, varName3, valueExpression, gradientExpressions, hessianExpressions);
        }


        /// <summary>Creates a scalar function form expressions that specify evaluation of function value, gradient, and Hessian.</summary>
        /// <param name="varName1"></param>
        /// <param name="varName2"></param>
        /// <param name="varName3"></param>
        /// <param name="valueExpression"></param>
        /// <param name="gradientExpressions"></param>
        /// <param name="HessianExpressions"></param>
        /// <returns></returns>
        public virtual IScalarFunction CreateScalarFunction(string varName1, string varName2, string varName3, string valueExpression,
            string[] gradientExpressions, string[][] hessianExpressions)
        {
            return Func2dFromScalarScript.CreateScalarFunction(new string[] { varName1, varName2, varName3 },
                valueExpression, gradientExpressions, hessianExpressions);
        }


    }  // class Func3dFromScript


}
