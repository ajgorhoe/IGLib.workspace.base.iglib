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


    /// <summary>Base class for loadable scripts that define scalar functions of several variables.</summary>
    /// $A Igor Jun10;
    public abstract class LoadableScriptScalarFunctionBase : LoadableScriptBase
    {

        /// <summary>Creates and returns a new scalar function object where the returned object is of scalar function class
        /// that has been dynamically loaded from the current script.</summary>
        public abstract LoadableScalarFunctionBase CreateScalarFunction();

        /// <summary>Creates and returns a new trnsformed scalar function object where object is of scalar function class
        /// that has been dynamically loaded from the current script.
        /// The resulting function is user defined function with affine transformed parameters.</summary>
        /// <param name="transf">Afifne transformation that defines transformation of parameters.</param>
        /// $A Igor Sep11;
        public abstract LoadableScalarFunctionBase CreateScalarFunction(IAffineTransformation transf);

        /// <summary>Script initialization, defined just for formal reasons.</summary>
        protected override void InitializeThis(string[] arguments)
        {  }

        /// <summary>Script execution, defined just for formal reasons.</summary>
        protected override string RunThis(string[] arguments)
        {
            throw new InvalidOperationException("This loadable script is not runnable, it just loads dynamic real function definition.");
            // return null;
        }

    } // abstract class LoadableScriptScalarFunctionBase


    /// <summary>A ScalarFunction class used as base class for dynamically loaded scalar functions.
    /// Functions loaded from scripts will inherit from this class, which enables script
    /// writers to assemble just the minimum necessary amount of code.</summary>
    /// $A Igor Jun10;
    public abstract class LoadableScalarFunctionBase : ScalarFunctionBase
    {

        #region Construction

        /// <summary>Constructs reference scalar function of vector variable defined by the user 
        /// (witout any transfomrations).</summary>
        public LoadableScalarFunctionBase(): this(null)
        { }


        /// <summary>Constructs a scalar function of vector argument defined by the user 
        /// defined function and affine transformation of parameters.</summary>
        /// <param name="transf">Affine transformation that is applied to parameters.
        /// If null then the fuction is identical to the reference (untransformed) user-defined function.</param>
        /// $A Igor Sep10;
        public LoadableScalarFunctionBase(IAffineTransformation transf)
            : base(transf)
        {
            Name = null;
            Description = null;
            InitDynamic();
        }

        #endregion Construction


        #region DynamicVariables

        // The following variables must be set in dynamically loaded functions:

        protected string[] _independentVariableNames = null;  // name of the independent variables used in string definitions of functions (to be compiled)
        protected string
            _returnedValueName = null,  // name of variable that stores returned value in functions
            _functionArgumentParametersName = null,  // name of function argument
            _functionArgumentGradientName = null,
            _functionArgumentHessianName = null,
            _independentVariableName = null,  // name of the variable used in string definitions of functions (to be compiled)
            _valueDefinitionString = null; // string definition of function value (to be compiled)
        protected string[] _gradientDefinitionStrings = null; // string definition of function gradient (to be compiled)
        protected string[][] _hessianDefinitionStrings = null; // string definition of function hessian (to be compiled)

        protected int _numParam; // number of parameters of the functioon

        protected bool
            _valueDefined = false,  // whether calculation of function value is implemented
            _gradientDefined = false,  // whether calculation of function derivative is implemented
            _hessianDefined = false;  // whether calculation of function second derivative is implemented

        #endregion DynamicVariables

        #region DynamicMethods

        /// <summary>Initialization of variables that are used by dynamic loading mechanisim.
        /// This function must be overridden in derived classes.</summary>
        protected abstract void InitDynamic();

        public override double ReferenceValue(IVector parameters) { return 0; }
        public override void ReferenceGradientPlain(IVector parameters, IVector gradient) {  }
        public override void ReferenceHessianPlain(IVector parameters, IMatrix hessian) {  }

        #endregion DynamicMethods

        /// <summary>Gets number of parameters of the current user defined scalar function.</summary>
        public virtual int NumParameters
        {
            get { return _numParam; }
        }

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
                    sb.AppendLine("Dynamically generated scalar function '" + Name + "'.");
                    if (!string.IsNullOrEmpty(_valueDefinitionString))
                        sb.AppendLine("  f(" + _independentVariableName + ") = " + _valueDefinitionString + ".");
                    //if (!string.IsNullOrEmpty(_gradientDefinitionString))
                    //    sb.AppendLine("  Grad f(" + _independentVariableName + ") = " + _gradientDefinitionString + ".");
                    //if (!string.IsNullOrEmpty(_hessianDefinitionString))
                    //    sb.AppendLine("  Hess f(" + _independentVariableName + ") = " + _hessianDefinitionString + ".");
                    _description = sb.ToString();
                }
                return _description;
            }
            protected internal set
            {
                _description = value;
            }
        }

        /// <summary>Whether calculation of value is implemented for the current scalar function.</summary>
        public override bool ValueDefined
        {
            get { return _valueDefined; }
            protected set
            {
                throw new InvalidOperationException(
                    "Can not set a flag for value defined for function " + Name + ".");
            }
        }


        /// <summary>Whether calculation of gradient is implemeted for the current scalar function.</summary>
        public override bool GradientDefined
        {
            get { return _gradientDefined; }
            protected set
            {
                throw new InvalidOperationException(
                    "Can not set a flag for derivative defined for function " + Name + ".");
            }
        }

        /// <summary>Whether calculation of second derivative is implemented for the current function.</summary>
        public override bool HessianDefined
        {
            get { return _hessianDefined; }
            protected set
            {
                throw new InvalidOperationException(
                    "Can not set a flag for second derivative defined for function " + Name + ".");
            }
        }

    } // abstract class LoadableScalarFunctionBase


}


