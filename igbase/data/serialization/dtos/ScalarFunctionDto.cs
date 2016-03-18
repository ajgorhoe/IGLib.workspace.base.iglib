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



    /// <summary>Base class for various DTO (Data Transfer Objects) for scalar function controllers.
    /// Used to store a state of a scalar function.</summary>
    /// <para>Beside as data transfer object, this class provides a variety of manipulations that are necessary
    /// when defining scalar functions from scripts (user definitions).</para>
    /// <typeparam name="FunctionControllerType">Type parameter specifying the specific scalar function controller type for which concrete DTO
    /// is designed.</typeparam>
    /// $A Igor Feb16;
    public abstract class ScalarFunctionScriptDtoBase<FunctionControllerType> : SerializationDtoBase<FunctionControllerType, ScalarFunctionScriptController>
        where FunctionControllerType : ScalarFunctionScriptController
    {

        #region Construction

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public ScalarFunctionScriptDtoBase()
            : base()
        { }

        /// <summary>Constructor, prepares the current DTO for storing a scalar function of the specified dimension.</summary>
        /// <param name="spaceDimension">Dimension of a vector that is stored in the current DTO.</param>
        public ScalarFunctionScriptDtoBase(int spaceDimension)
            : this()
        {
            this.Dimension = spaceDimension;
        }

        #endregion Construction



        #region Data


        /// <summary>Dimension of the parameter space.</summary>
        public virtual int Dimension
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

        /// <summary>Whether function value is defined for the represented function.</summary>
        public virtual bool IsValueDefined
        { get; set; }

        /// <summary>Whether function gradient is defined for the represented function.</summary>
        public virtual bool IsGradientDefined
        { get; set; }

        /// <summary>Definition of function value.</summary>
        public virtual string ValueDefinitonString
        { get; set; }

        
        /// <summary>Gradient definition strings (separately for each gradient component).</summary>
        public virtual string[] GradientDefinitionStrings
        { get; set; }

        public virtual string ZType
        { get; set; }

        public virtual InputOutputDataDefinitonDto ZDataDefinition
        { get; set; }



        #endregion Data


        #region Operation

        /// <summary>Creates and returns a new scalar function in the space of specific dimension.</summary>
        /// <param name="dimension">Vector dimension.</param>
        public abstract FunctionControllerType CreateScalarFunctionController(int dimension);

        /// <summary>Creates and returns a new vector of the specified type and dimension.</summary>
        public override FunctionControllerType CreateObject()
        {
            return CreateScalarFunctionController(this.Dimension);
        }

        /// <summary>Copies data to the current DTO from a scaalr function object.</summary>
        /// <param name="functionController">Scalar function object from which data is copied.</param>
        protected override void CopyFromPlain(ScalarFunctionScriptController functionController)
        {
            this.Name = functionController.Name;
            this.Description = functionController.Description;
            this.Dimension = functionController.Dimension;
            this.ParameterNames = functionController.ParameterNames;
            this.ValueDefinitonString = functionController.ValueDefinitonString;
            this.GradientDefinitionStrings = functionController.GradientDefinitionStrings;
            this.IsValueDefined = functionController.IsValueDefined;
            this.IsGradientDefined = functionController.IsGradientDefined;

            this.ZType = functionController.GetType().FullName;
            this.ZDataDefinition = null;
        }

        /// <summary>Copies data from the current DTO to a scalar function object.</summary>
        /// <param name="functionContrller">Scalar function object that data is copied to.</param>
        protected override void CopyToPlain(ref ScalarFunctionScriptController functionContrller)
        {
             functionContrller.Name = this.Name;
             functionContrller.Description = this.Description;
             functionContrller.Dimension = this.Dimension;
             functionContrller.ParameterNames = this.ParameterNames;
             functionContrller.ValueDefinitonString = this.ValueDefinitonString;
             functionContrller.GradientDefinitionStrings = this.GradientDefinitionStrings;

            if (!this.IsValueDefined && string.IsNullOrWhiteSpace(functionContrller.ValueDefinitonString))
                functionContrller.ValueDefinitonString = null;
            if (!this.IsGradientDefined)
                functionContrller.GradientDefinitionStrings = null;
        }

        #endregion Operation
        



    }  // abstract class ScalarFunctionDtoBase<ScalarFunctionType>


    /// <summary>DTO (data transfer object) for vector interface (IVector).</summary>
    /// $A Igor Feb16;
    public class ScalarFunctionScriptDto : ScalarFunctionScriptDtoBase<ScalarFunctionScriptController>
    {

        #region Construction

        /// <summary>Creates a DTO for storing state of a script based scalar function objects of any type.</summary>
        public ScalarFunctionScriptDto()
            : base()
        { }

        /// <summary>Creates a DTO for storing a scalar function object of any vector type, with specified dimension.</summary>
        /// <param name="dimension">Vector dimension.</param>
        public ScalarFunctionScriptDto(int dimension)
            : base(dimension)
        { }

        #endregion Construction

        /// <summary>Creates and returns a new vector cast to the interface type IVector.</summary>
        /// <param name="length">Vector dimension.</param>
        public override ScalarFunctionScriptController CreateScalarFunctionController(int length)
        {
            return new ScalarFunctionScriptController(length);
        }

    } // class ScalarFunctionScriptDto

    

}