using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{


    /// <summary>Scalar function of 2 variables that is generated from expressions for evaluation of function value, 
    /// gradient components, etc.</summary>
    /// $A Igor Oct11;
    public class Func2dFromScalarScript : Func2dFromScalarFunction, IFunc2d, IScalarFunctionUntransformed
    {

        protected Func2dFromScalarScript() { }

        /// <summary>Constructs a scalar function of 2 variables from the specified scalar function 
        /// of a vector argument.</summary>
        /// <param name="scalarFunction">Original scalar function of vector argument used for evaluation.</param>
        protected Func2dFromScalarScript(IScalarFunction originalFunction): base(originalFunction)
        {  }


        // TODO: add different constructurs (with different sets of arguments) for convenience!

        // TODO: maybe add default variable names such as "x", "y", ..., and dd functions where you don't need to specify these


        /// <summary>Constructs a scalar function of 2 variables from the specified scalar function 
        /// of a vector argument.</summary>
        /// <param name="scalarFunction">Original scalar function of vector argument used for evaluation.</param>
        public Func2dFromScalarScript(string varName1, string varName2, string valueExpression,
                    string[] gradientExpressions, string[][] hessianExpressions):
            base()
        {
            OriginalFunction = CreateScalarFunction(varName1, varName2, valueExpression, gradientExpressions, hessianExpressions);
        }


        /// <summary>Creates a scalar function form expressions that specify evaluation of function value, gradient, and Hessian.</summary>
        /// <param name="varName1"></param>
        /// <param name="varName2"></param>
        /// <param name="valueExpression"></param>
        /// <param name="gradientExpressions"></param>
        /// <param name="HessianExpressions"></param>
        /// <returns></returns>
        public virtual IScalarFunction CreateScalarFunction(string varName1, string varName2, string valueExpression,
            string[] gradientExpressions, string[][] hessianExpressions)
        {
            return Func2dFromScalarScript.CreateScalarFunction(new string[]{varName1, varName2},
                valueExpression, gradientExpressions, hessianExpressions);
        }

        /// <summary>Creates and returns a scalar function that is formed on basis of expressions that define how
        /// function value and eventually gradient and hessian are calculated (if defined).</summary>
        /// <param name="variableNames"></param>
        /// <param name="valueExpression"></param>
        /// <param name="gradientExpressions"></param>
        /// <param name="hessianExpressions"></param>
        /// <returns></returns>
        public static IScalarFunction CreateScalarFunction(string[] variableNames, string valueExpression,
            string[] gradientExpressions, string[][] hessianExpressions)
        {
            if (variableNames == null)
                throw new ArgumentNullException("No Variabel Names Defined.");
            if (valueExpression == null)
                throw new ArgumentNullException("No Function Defined.");
            
            // Defined ScalarFinction
            IScalarFunction ret;
            ScalarFunctionLoader loader = new ScalarFunctionLoader();
            int numParameters = variableNames.Length;

            loader.NumParameters = numParameters;
            loader.IndependentVariableNames = variableNames;
            loader.ValueDefinitionString = valueExpression;
            ret = loader.CreateScalarFunction();
            

            // REMARK: for implementation, see the function below:
            //ScalarFunctionLoader.Example();

            return ret;
        }


    }  // class Func2dFromScalarFunction


}
