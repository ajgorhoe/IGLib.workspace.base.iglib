// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

 // Real function from which the dynamically loaded functions in scripts will inherit.


using System;
using System.Collections;
using System.Text;
using System.IO;

using IG.Lib;
using IG.Num;

namespace IG.Lib
{


    /// <summary>Base class for loadable scripts that define real functions of one variable.</summary>
    /// $A Igor Jun10 Sep11;
    public abstract class LoadableScriptRealFunctionBase : LoadableScriptBase
    {

        /// <summary>Creates and returns a new real function object where object is of real function class
        /// that has been dynamically loaded from the current script.</summary>
        public abstract LoadableRealFunctionBase CreateRealFunction();

        /// <summary>Creates and returns a new real function object where object is of real function class
        /// that has been dynamically loaded from the current script.</summary>
        /// <param name="Kx">Stretch of the reference function graph in x direction.</param>
        /// <param name="Sx">Shift of the reference function graph in x direction.</param>
        /// $A Igor Sep11;
        public abstract LoadableRealFunctionBase CreateRealFunction(double Kx, double Sx);

        /// <summary>Creates and returns a new real function object where object is of real function class
        /// that has been dynamically loaded from the current script.</summary>
        /// <param name="Kx">Stretch of the reference function graph in x direction.</param>
        /// <param name="Sx">Shift of the reference function graph in x direction.</param>
        /// <param name="Ky">Stretch of the reference function graph in y direction.</param>
        /// <param name="Sy">Shift of the reference function graph in y direction.</param>
        /// $A Igor Sep11;
        public abstract LoadableRealFunctionBase CreateRealFunction(double Kx, double Sx, double Ky, double Sy);

        /// <summary>Script initialization, defined just for formal reasons.</summary>
        protected override void InitializeThis(string[] arguments)
        {  }

        /// <summary>Script execution, defined just for formal reasons.</summary>
        protected override string RunThis(string[] arguments)
        {
            throw new InvalidOperationException("This loadable script is not runnable, it just loads dynamic real function definition.");
            // return null;
        }

    } // abstract class LoadableScriptRealFunctionBase


    /// <summary>A RealFunction class used as base class for dynamically loaded functions.
    /// Bunctions loaded from scripts will inherit from this class, which enables script
    /// writers to assemble just the minimum necessary amount of code.</summary>
    /// $A Igor Jun10;
    public abstract class LoadableRealFunctionBase : RealFunction
    {

        /// <summary>Default function constructor, result is reference function (witout shifting or stretching).</summary>
        public LoadableRealFunctionBase()
            : this(1.0, 0.0, 1.0, 0.0)
        { }

        /// <summary>Function constructor.</summary>
        /// <param name="Kx">Stretch of the reference function graph in x direction.</param>
        /// <param name="Sx">Shift of the reference function graph in x direction.</param>
        public LoadableRealFunctionBase(double Kx, double Sx)
            : this(Kx, Sx, 1.0, 0.0)
        { }


        /// <summary>Function constructor.</summary>
        /// <param name="Kx">Stretch of the reference function graph in x direction.</param>
        /// <param name="Sx">Shift of the reference function graph in x direction.</param>
        /// <param name="Ky">Stretch of the reference function graph in y direction.</param>
        /// <param name="Sy">Shift of the reference function graph in y direction.</param>
        public LoadableRealFunctionBase(double Kx, double Sx, double Ky, double Sy)
        {
            Name = null;
            Description = null;
            InitDynamic();
            SetTransformationParameters(Kx, Sx, Ky, Sy);
        }


        #region DynamicVariables

        // The following variables must be set in dynamically loaded functions:

        protected string
            _returnedValueName = null,  // name of variable that stores returned value in functions
            _functionArgumentName = null,  // name of function argument
            _independentVariableName = null,  // name of the variable used in string definitions of functions (to be compiled)
            _valueDefinitionString = null, // string definition of function value (to be compiled)
            _derivativeDefinitionString = null, // string definition of function derivative (to be compiled)
            _secondDerivativeDefinitionString = null, // string definition of function second derivative (to be compiled)
            _integralDefinitionString = null,   // string definition of function indefinite integral (to be compiled)
            _inverseDefinitionString = null; // string definition of function inverse (to be compiled)

        //protected bool
        //    _valueDefined = false,  // whether calculation of function value is implemented
        //    _derivativeDefined = false,  // whether calculation of function derivative is implemented
        //    _secondDerivativeDefined = false,  // whether calculation of function second derivative is implemented
        //    _integralDefined = false,  // whether calculation of function indefinite integral is implemented
        //    _inverseDefined = false;  // whether calculation of function inverse is implemented

        #endregion DynamicVariables

        #region DynamicMethods

        /// <summary>Initialization of variables that are used by dynamic loading mechanisim.</summary>
        protected virtual void InitDynamic()
        {
            throw new NotImplementedException("The initialization of dynamically loaded real function is not defined.");
        }

        //protected override double RefValue(double x) { return base.RefValue(x); }
        //protected override double RefDerivative(double x) { return base.RefDerivative(x); }
        //protected override double RefSecondDerivative(double x) { return base.RefSecondDerivative(x); }
        //protected override double RefIntegral(double x) { return base.RefIntegral(x); }
        //protected override double RefInverse(double y) { return base.RefInverse(y); }


        #endregion DynamicMethods

        /// <summary>Function name.</summary>
        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    _name = "DynamicFunction"; 
                return _name;
            }
            protected internal set
            {
                _name = value;
            }
        }

        /// <summary>Function description.</summary>
        public override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Dynamically generated function " + Name + ".");
                    if (!string.IsNullOrEmpty(_valueDefinitionString))
                        sb.AppendLine("  f(" + _independentVariableName + ") = " + _valueDefinitionString + ".");
                    if (!string.IsNullOrEmpty(_derivativeDefinitionString))
                        sb.AppendLine("  f'(" + _independentVariableName + ") = " + _derivativeDefinitionString + ".");
                    if (!string.IsNullOrEmpty(_secondDerivativeDefinitionString))
                        sb.AppendLine("  f''(" + _independentVariableName + ") = " + _secondDerivativeDefinitionString + ".");
                    _description = sb.ToString();
                }
                return _description;
            }
            protected internal set
            {
                _description = value;
            }
        }

        /// <summary>Whether calculation of value is implemented for the current function.</summary>
        public override bool ValueDefined
        {
            get { return _valueDefined; }
            protected internal set
            {
                bool a = base.ValueDefined;
                throw new InvalidOperationException(
                    "Can not set a flag for value defined for function " + Name + ".");
            }
        }


        /// <summary>Whether calculation of derivative is implemeted for the current function.</summary>
        public override bool DerivativeDefined
        {
            get { return _derivativeDefined; }
            protected internal set
            {
                throw new InvalidOperationException(
                    "Can not set a flag for derivative defined for function " + Name + ".");
            }
        }

        /// <summary>Whether calculation of second derivative is implemented for the current function.</summary>
        public override bool SecondDerivativeDefined
        {
            get { return _secondDerivativeDefined; }
            protected internal set
            {
                throw new InvalidOperationException(
                    "Can not set a flag for second derivative defined for function " + Name + ".");
            }
        }



        /// <summary>Return a flag indicating whether calculation of specific higher order derivatives.</summary>
        /// <param name="order">Order of derivative for which definition is queried.</param>
        public override bool HigherDerivativeDefined(int order)
        {
            if (order <= 0)
                throw new ArgumentException("Can not get a flag for defined derivative of nonpositive order. Function: "
                    + Name + ".");
            if (order == 0)
                return ValueDefined;
            if (order == 1)
                return DerivativeDefined;
            if (order == 2)
                return SecondDerivativeDefined;
            return false;
        }


        /// <summary>Sets the internal variable that specifies which is the highest order derivative devined
        /// (-1 for unlimited).</summary>
        /// <param name="order">Highest order for which derivative is defined. -1 means that all derivatives are 
        /// defined.</param>
        protected internal override void setHighestDerivativeDefined(int order)
        {
            throw new InvalidOperationException("Can not set the highest order derivative that is defined. Function:  "
              + Name + ".");
        }

        /// <summary>Wether analytic indefinite integral is impelemented.</summary>
        public override bool IntegralDefined
        {
            get { return _integralDefined; }
            protected internal set
            {
                throw new InvalidOperationException(
                    "Can not set a flag for integral defined for function " + Name + ".");
            }
        }

        /// <summary>Whether calculation of inverse function is implemented.</summary>
        public override bool InverseDefined
        {
            get { return _inverseDefined; }
            protected internal set
            {
                throw new InvalidOperationException(
                    "Can not set a flag for inverse defined for function " + Name + ".");
            }
        }

    } // abstract class LoadableRealFunctionBase

}


