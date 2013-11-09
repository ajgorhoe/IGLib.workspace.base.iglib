using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;
using IG.Num;
using IG.Neural;

namespace IG.Neural
{
    /// <summary>Approximator function of 2 variables that is based on a general approximator function of vector argument.</summary>
    /// $A Igor Oct11, Tako78 Nov23;
    public class Func2dFromApproximator : Func2dBase, IFunc2d, IScalarFunctionUntransformed
    {

        protected Func2dFromApproximator() { }

        /// <summary>Constructs a approximator function of 2 variables from the specified approximator function 
        /// of a vector argument.</summary>
        /// <param name="scalarFunction">Original approximator function of vector argument used for evaluation.</param>
        public Func2dFromApproximator(INeuralApproximator originalFunction)
        {
            this.OriginalFunction = originalFunction;
        }

        #region Data

        INeuralApproximator _originalFunction;

        /// <summary>Approximator function of vector argument that is used for evaluation of the current function value, 
        /// gradient, and Hessian.</summary>
        public INeuralApproximator OriginalFunction
        {
            get { return _originalFunction; }
            protected set
            {
                this.ValueDefined = false; this.GradientDefined = false; this.HessianDefined = false;
                if (value != null)
                {
                    //this.ValueDefined = value.ValueDefined;
                    //this.GradientDefined = value.GradientDefined;
                    //this.HessianDefined = value.HessianDefined;
                }
                _originalFunction = value;
            }
        }

        #endregion Data

        public override double Value(double x, double y)
        {
            double z = 0;
            IVector zOutput = new Vector(1);
            IVector xy = new Vector(2);
            xy[0] = x;
            xy[1] = y;
            _originalFunction.CalculateOutput(xy, ref zOutput);
            z = zOutput[0];
            return z;
        }

        public override void Gradient(double x, double y, out double gradx, out double grady)
        {
            throw new NotImplementedException();
        }

        public override void Hessian(double x, double y, out double dxx, out double dyy, out double dxy)
        {
            throw new NotImplementedException();
        }
    }
}
