// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// CLASSES FOR DATA TRANSFER OBJECTS (DTO) THAT FACILITATE SERIALIZATION OF VECTOR OBJECTS.

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using IG.Lib;
using System.Collections.Generic;

using IG.Num;

namespace IG.Num
{


    // TODO: generic base type is not elaborated in terms of type parameters. This could be elaborated but it is not priority.


    /// <summary>Base class for various DTO (Data Transfer Objects) for scalar function controllers.
    /// Used to store a state of a scalar function.</summary>
    /// <para>Beside as data transfer object, this class provides a variety of manipulations that are necessary
    /// when defining scalar functions from scripts (user definitions).</para>
    /// <typeparam name="FunctionControllerType">Type parameter specifying the specific scalar function controller type for which concrete DTO
    /// is designed.</typeparam>
    /// $A Igor Feb16;
    public abstract class VectorFunctionScriptDtoBase<FunctionControllerType, ScalarFunctionControllerType, ScalarFunctionDtoType> : 
            SerializationDtoBase<FunctionControllerType, VectorFunctionScriptController>
        where FunctionControllerType : VectorFunctionScriptController
        where ScalarFunctionControllerType: ScalarFunctionScriptController
        where ScalarFunctionDtoType: ScalarFunctionScriptDto, new()


        //<FunctionControllerType, ScalarFunctionControllerType, ScalarFunctionDtoType, 
        //    VectorFunctionType , ScalarFunctionType> : 
        //    SerializationDtoBase<FunctionControllerType, FunctionControllerType>
        //where FunctionControllerType : VectorFunctionScriptControllerBase<VectorFunctionType, ScalarFunctionControllerType, ScalarFunctionType>
        //where ScalarFunctionControllerType: new() // ScalarFunctionScriptControllerBase<ScalarFunctionType>
        //where ScalarFunctionDtoType: ScalarFunctionScriptDtoBase<ScalarFunctionControllerType>, new()
        //where ScalarFunctionType: class, IScalarFunction
        //where ScalarFunctionType: class, IScalarFunction
    {

        #region Construction

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public VectorFunctionScriptDtoBase()
            : base()
        { }

        /// <summary>Constructor, prepares the current DTO for storing a scalar function of the specified dimension.</summary>
        /// <param name="spaceDimension">Dimension of a vector that is stored in the current DTO.</param>
        public VectorFunctionScriptDtoBase(int spaceDimension)
            : this()
        {
            this.NumParameters = spaceDimension;
        }

        #endregion Construction


        #region Data


        /// <summary>Array of scalar function controller DTOs.</summary>
        public ScalarFunctionDtoType[] ScalarFunctions
        {
            get;
            set;
        }


        /// <summary>Dimension of the parameter space.</summary>
        public virtual int NumParameters
        { get; set; }

        /// <summary>Dimension of function codomain (i.e., the number of returned values).</summary>
        public virtual int NumValues
        { get; set; }


        /// <summary>Scalar function name.</summary>
        public virtual string Name
        { get; set; }


        /// <summary>Scalar function description.</summary>
        public virtual string Description
        { get; set; }

        ///// <summary>Specifies whether value is defined for the funciton represented by the current DTO.</summary>
        //public virtual bool IsValueDefined
        //{
        //    get { return (!string.IsNullOrEmpty(ValueDefinitonString)); }
        //}

        ///// <summary>Specifies whether gradient is defined for the funciton represented by the current DTO.</summary>
        //public virtual bool IsGradientDefined
        //{
        //    get { return (GradientDefinitionStrings != null && GradientDefinitionStrings.Length > 0); }
        //}


        /// <summary>Names of function parameters.</summary>
        public virtual string[] ParameterNames
        { get; set; }

        /// <summary>Names of function values, i.e. scalar funcitons that define return values of the vector function.</summary>
        public virtual string[] FunctionNames
        { get; set; }

        ///// <summary>Whether function value is defined for the represented function.</summary>
        //public virtual bool IsValueDefined
        //{ get; set; }

        ///// <summary>Whether function gradient is defined for the represented function.</summary>
        //public virtual bool IsGradientDefined
        //{ get; set; }

        ///// <summary>Definition of function value.</summary>
        //public virtual string ValueDefinitonString
        //{ get; set; }


        ///// <summary>Gradient definition strings (separately for each gradient component).</summary>
        //public virtual string[] GradientDefinitionStrings
        //{ get; set; }

        public virtual string ZType
        { get; set; }

        public virtual InputOutputDataDefinitonDto ZDataDefinition
        { get; set; }



        #endregion Data


        #region Operation


        /// <summary>Creates and returns a new vector cast to the interface type IVector.</summary>
        /// <param name="numParameters">Number of function parameters (dimension of its domain).</param>
        /// <param name="numValues">Numberr of function values (dimension of its codomain).</param>
        public abstract FunctionControllerType CreateVectorFunctionController(int numParameters, int numValues);

        /// <summary>Creates and returns a new vector of the specified type and dimension.</summary>
        public override FunctionControllerType CreateObject()
        {
            return CreateVectorFunctionController(this.NumParameters, this.NumValues);
        }


        ///// <summary>Copies scalar function controller to scalar function DTO.</summary>
        ///// <param name="controller">Scalar function controller from which data is copied.</param>
        ///// <param name="dto">Scalar function DTO to which data is copied.</param>
        //protected abstract void CopyScalar(ScalarFunctionControllerType controller, ref ScalarFunctionDtoType dto);

        ///// <summary>Copies scalar function DTO to scalar function controller.</summary>
        ///// <param name="dto">Scalar function DTO from which data is copied.</param>
        ///// <param name="controller">Scalar function controller to which data is copied.</param>
        //protected abstract void CopyScalar(ScalarFunctionDtoType dto, ref ScalarFunctionControllerType controller);


        /// <summary>Copies data to the current DTO from a scaalr function object.</summary>
        /// <param name="functionController">Scalar function object from which data is copied.</param>
        protected override void CopyFromPlain(VectorFunctionScriptController functionController)
        {
            this.Name = functionController.Name;
            this.Description = functionController.Description;
            this.NumParameters = functionController.NumParameters;
            this.NumValues = functionController.NumValues;
            this.ParameterNames = functionController.ParameterNames;
            this.FunctionNames = functionController.FunctionNames;

            this.ScalarFunctions = new ScalarFunctionDtoType[this.NumValues];
            for (int whichFunction = 0; whichFunction < NumValues; ++ whichFunction)
            {
                ScalarFunctionDtoType scalarDto = new ScalarFunctionDtoType();
                ScalarFunctionScriptController scalarFunctionController = functionController[whichFunction];

                scalarDto.CopyFrom(scalarFunctionController);
                ScalarFunctions[whichFunction] = scalarDto;
            }
            

            //this.ValueDefinitonString = functionController.ValueDefinitonString;
            //this.GradientDefinitionStrings = functionController.GradientDefinitionStrings;
            //this.IsValueDefined = functionController.IsValueDefined;
            //this.IsGradientDefined = functionController.IsGradientDefined;

            this.ZType = functionController.GetType().FullName;
            this.ZDataDefinition = null;
        }

        /// <summary>Copies data from the current DTO to a scalar function object.</summary>
        /// <param name="functionController">Scalar function object that data is copied to.</param>
        protected override void CopyToPlain(ref VectorFunctionScriptController functionController)
        {
            functionController.Name = this.Name;
            functionController.Description = this.Description;
            functionController.NumParameters = this.NumParameters;
            functionController.NumValues = this.NumValues;
            functionController.ParameterNames = this.ParameterNames;
            functionController.FunctionNames = this.FunctionNames;

            if (this.ScalarFunctions != null)
                if (this.ScalarFunctions.Length > this.NumValues)
                {
                    throw new InvalidOperationException("Error when copying from vector function DTO to controller: " + Environment.NewLine
                        + "  Number of scalar functions on the DTO (" + ScalarFunctions.Length + ") is larger than declared number (" 
                        + NumValues + ").");
                }

                // ScalarFunctionScripController[] scalarFunctionControllers = new ScalarFunctionScripController[NumValues];
                for (int whichFunction = 0; whichFunction < NumValues; ++ whichFunction)
            {
                ScalarFunctionScriptController scalarController = null;  //  new ScalarFunctionScripController(NumParameters);
                ScalarFunctionScriptDto scalarDto = null;
                if (this.ScalarFunctions != null)
                {
                    if (this.ScalarFunctions.Length > whichFunction)
                    {
                        scalarDto = this.ScalarFunctions[whichFunction];
                    }
                }
                if (scalarDto != null)
                {
                    scalarDto.CopyTo(ref scalarController);
                }
                functionController[whichFunction] = scalarController;
            }

            // functionContrller.


            //functionContrller.ValueDefinitonString = this.ValueDefinitonString;
            //functionContrller.GradientDefinitionStrings = this.GradientDefinitionStrings;

            //if (!this.IsValueDefined && string.IsNullOrWhiteSpace(functionContrller.ValueDefinitonString))
            //    functionContrller.ValueDefinitonString = null;
            //if (!this.IsGradientDefined)
            //    functionContrller.GradientDefinitionStrings = null;

        }

        #endregion Operation



    }  // abstract class VectorFunctionDtoBase<VectorFunctionType>



    /// <summary>DTO (data transfer object) for vector interface (IVector).</summary>
    /// $A Igor Feb16;
    public class VectorFunctionScriptDto : VectorFunctionScriptDtoBase<VectorFunctionScriptController, ScalarFunctionScriptController, ScalarFunctionScriptDto>
    {

        #region Construction

        /// <summary>Creates a DTO for storing state of a script based scalar function objects of any type.</summary>
        public VectorFunctionScriptDto()
            : base()
        { }

        /// <summary>Creates a DTO for storing a scalar function object of any vector type, with specified dimension.</summary>
        /// <param name="dimension">Vector dimension.</param>
        public VectorFunctionScriptDto(int dimension)
            : base(dimension)
        { }

        #endregion Construction

        /// <summary>Creates and returns a new vector cast to the interface type IVector.</summary>
        /// <param name="numParameters">Number of function parameters (dimension of its domain).</param>
        /// <param name="numValues">Numberr of function values (dimension of its codomain).</param>
        public override VectorFunctionScriptController CreateVectorFunctionController(int numParameters, int numValues)
        {
            return new VectorFunctionScriptController(numParameters, numValues);
        }

    } // class VectorFunctionScriptDto




}


