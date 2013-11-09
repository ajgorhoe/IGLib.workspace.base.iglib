using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Scalar function of 3 variables that is based on a general scalar function of vector argument.</summary>
    /// $A Igor Oct11;
    public class Func3dFromScalarFunction : Func3dBase, IFunc3d, IScalarFunctionUntransformed
    {

        protected Func3dFromScalarFunction() { }

        /// <summary>Constructs a scalar function of 3 variables from the specified scalar function 
        /// of a vector argument.</summary>
        /// <param name="scalarFunction">Original scalar function of vector argument used for evaluation.</param>
        public Func3dFromScalarFunction(IScalarFunction originalFunction)
        {
            this.OriginalFunction = originalFunction;
        }

        #region Data

        IScalarFunction _originalFunction;

        /// <summary>Scalar function of vector argument that is used for evaluation of the current function value, gradient, and Hessian.</summary>
        public IScalarFunction OriginalFunction
        {
            get { return _originalFunction; }
            protected set
            {
                this.ValueDefined = false; this.GradientDefined = false; this.HessianDefined = false;
                if (value != null)
                {
                    this.ValueDefined = value.ValueDefined;
                    this.GradientDefined = value.GradientDefined;
                    this.HessianDefined = value.HessianDefined;
                }
                _originalFunction = value;
            }
        }

        #endregion Data


        // TODO: implement basic methods below!

        public override double Value(double x, double y, double z)
        {
            throw new NotImplementedException();
        }

        public override void  Gradient(double x, double y, double z, out double gradx, out double grady, out double gradz)
        {
 	        throw new NotImplementedException();
        }

        public override void Hessian(double x, double y, double z, out double dxx, out double dyy, out double dzz, 
            out double dxy, out double dxz, out double dyz)
        {
            throw new NotImplementedException();
        }


    }  // class Func2dFromScalarFunction


}
