// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;

namespace IG.Num
{

    /// <summary>DTO (data transfer object) for sampled data element (one point with vectors of 
    /// input and output parameters).</summary>
    /// $A Igor Mar11;
    public class SampledDataElementDto : SerializationDto<SampledDataElement>
    {

        public SampledDataElementDto()
            : base()
        {  }


        #region Data

        /// <summary>Idex of the element in arrays or list (or in <see cref="SampledDataSet"/>
        /// objects), auxiliary information that facilitates identification and access to specific data chunk out 
        /// of those that are worked out.</summary>
        public int Index;

        protected VectorDtoBase _inputParameters;

        protected VectorDtoBase _outputValues;

        /// <summary>Vector of input parameters of a single data element.</summary>
        public virtual VectorDtoBase InputParameters
        { get { return _inputParameters; } set { _inputParameters = value; } }

        /// <summary>Vector of input parameters of a single data element.</summary>
        public virtual VectorDtoBase OutputValues
        { get { return _outputValues; } set { _outputValues = value; } }


        #endregion Data


        /// <summary>Creates and returns an object consistent with current contents of the vector.</summary>
        public override SampledDataElement CreateObject()
        {
            SampledDataElement ret;
            ret = new SampledDataElement(null, null);
            return ret;
        }
        
        protected override void CopyFromPlain(SampledDataElement obj)
        {
            this.Index = obj.Index;
            if (obj.InputParameters == null)
                this.InputParameters = null;
            else
            {
                this.InputParameters = new VectorDtoBase();
                this.InputParameters.CopyFrom(obj.InputParameters);
            }
            if (obj.OutputValues == null)
                this.OutputValues = null;
            else
            {
                this.OutputValues = new VectorDtoBase();
                this.OutputValues.CopyFrom(obj.OutputValues);
            }
        }

        protected override void CopyToPlain(ref SampledDataElement obj)
        {
            obj.Index = this.Index;
            if (this.GetNull())
                obj = null;
            else
            {
                obj = this.CreateObject();
                IVector inputParameters = null;
                IVector outputValues = null;
                if (this.InputParameters != null)
                    this.InputParameters.CopyTo(ref inputParameters);
                if (this.OutputValues != null)
                    this.OutputValues.CopyTo(ref outputValues);
                obj.InputParameters = inputParameters;
                obj.OutputValues = outputValues;
            }
        }


    }  // class SampledDataElementDto


    /// <summary>Data Transfer Object (DTO) for sampled data set.</summary>
    public class SampledDataSetDto : SerializationDto<SampledDataSet>
    {

        public SampledDataSetDto()
            : base()
        { }

        #region Data


        protected int _inputLength = 0;

        protected int _outputLength = 0;

        protected int _length = 0;

        protected SampledDataElementDto[] _elements;

        public virtual int Length
        { get { return _length; } set { _length = value; } }

        /// <summary>Number of input parameters in sampled data.</summary>
        public virtual int InputLength
        {
            get { return _inputLength; }
            set { _inputLength = value; }
        }

        /// <summary>Number of output values in sampled data.</summary>
        public virtual int OutputLength
        {
            get { return _outputLength; }
            set { _outputLength = value; }
        }

        /// <summary>Element of the sempled set (input/output pairs).</summary>
        public virtual SampledDataElementDto[] Elements
        {
            get { return _elements; }
            set { _elements = value; }
        }

        #endregion Data

        #region Operation

        /// <summary>Creates and returns a new object of the appropriate type.</summary>
        public override SampledDataSet CreateObject()
        {
            SampledDataSet ret = new SampledDataSet(this.InputLength, this.OutputLength);
            return ret;
        }


        /// <summary>Copies the specified sampled data set to the current DTO.</summary>
        /// <param name="dataSet">Object that is copied to the current DTO.</param>
        protected override void CopyFromPlain(SampledDataSet dataSet)
        {
            this.InputLength = dataSet.InputLength;
            this.OutputLength = dataSet.OutputLength;
            this.Length = dataSet.Length;
            List<SampledDataElement> elements = dataSet.ElementList;
            if (elements == null)
                this.Elements = null;
            else
            {
                this.Elements = new SampledDataElementDto[elements.Count];
                for (int i = 0; i < elements.Count; ++i)
                {
                    if (elements[i] == null)
                        this.Elements[i] = null;
                    else
                    {
                        this.Elements[i] = new SampledDataElementDto();
                        this.Elements[i].CopyFrom(elements[i]);
                    }
                }
            }
        }


        /// <summary>Copies contents of the current DTO to the specified sempled data set.</summary>
        /// <param name="dataSet">Object that the current DTO contents are copied to.</param>
        protected override void CopyToPlain(ref SampledDataSet dataSet)
        {
            if (this.GetNull())
                dataSet = null;
            else
            {
                dataSet = this.CreateObject();
                dataSet.InputLength = this.InputLength;
                dataSet.OutputLength = OutputLength;
                if (this.Elements != null)
                    for (int i = 0; i < Elements.Length; ++i)
                    {
                        SampledDataElement element = null;
                        if (Elements[i] != null)
                            Elements[i].CopyTo(ref element);
                        dataSet.ElementList.Add(element);
                    }
            }
        }

        #endregion Operation

    }  // class SampledDataSetDto


    /// <summary>DTO (data transfer object) for data element definition.</summary>
    /// <typeparam name="ElementType">Actual type of the object whose data is represented by the current DTO.</typeparam>
    /// $A Ifor Mar11;
    public abstract class InputOutputElementDefinitionDto<ElementType> : SerializationDtoBase<ElementType, InputOutputElementDefinition>
        where ElementType : InputOutputElementDefinition
    {

        public InputOutputElementDefinitionDto()
            : base()
        {
        }

        #region Data

        /// <summary>Unique name of the data element described by the current definition. 
        /// Considered a kind of variable name that distinguishes between data by short names.</summary>
        /// <remarks>There is an agreement that element names should follow conventions for valid variable names 
        /// in programming languages C++, C# and Java.</remarks>
        public string Name;

        /// <summary>Alternative name of the data element described by the current definition.
        /// <para>Used in transformations between different data sets where parameters may be named differently.</para></summary>
        /// <remarks>There is an agreement that element names should follow conventions for valid variable names 
        /// in programming languages C++, C# and Java.</remarks>
        public string NameAlt;

        /// <summary>A title describing what given data element represents. Titles can contain special 
        /// characters and spaces, but should be shorter than descriptions.</summary>
        public string Title;

        /// <summary>Describes the meaning of a data element used as part of input or output data.</summary>
        public string Description;

        /// <summary>Flag specifying whether a data element is input or output element.</summary>
        public bool IsInput;

        /// <summary>Specifies whether element index is specified for the data element described by the current definition.</summary>
        ///<remarks>If not specified, then by agreement the ElementIndex is set to -1.
        /// Getter of this property automatically set ElementIndex to -1 if the property is set to false.</remarks>
        public bool ElementIndexSpecified = false;


        /// <summary>Specifies the index f the element described by the current definition, within the data vextor (either input or output).</summary>
        ///<remarks>If not specified, then by agreement the ElementIndex is set to -1.
        /// Getter of this property automatically set ElementIndexSpecified flag to false if the property is set to less than 0,
        /// and to true otherwise.</remarks>
        public int ElementIndex = -1;

        /// <summary>Flag indicating whethe minimal and maximal value are defined for the input 
        /// data element described by the current definition.</summary>
        public bool BoundsDefined = false;

        /// <summary>Minimal value for the output data element described by the current definition.</summary>
        public double MinimalValue;

        /// <summary>Maximal value for the output data element described by the current definition.</summary>
        public double MaximalValue;


        /// <summary>Flag indicating whether target value is defined for the data element described by the current definition.</summary>
        public bool TargetValueDefined = false;

        /// <summary>Target value of the current element. Used for optimization.</summary>
        public double TargetValue = 0.0;

        /// <summary>Flag indicating whether scaling length is defined for the data element described by the current definition.</summary>
        public bool ScalingLengthDefined = false;

        /// <summary>Scaling length, used for optimization and other tasks where scaling of input or output quantities is important.</summary>
        public double ScalingLength = 0.0;


        #endregion Data


        #region Operation

        /// <summary>Creates and returns an object consistent with current DTO.</summary>
        public override ElementType CreateObject()
        {
            throw new NotImplementedException("CraeateObject() is not implemented bnecause the return type is abstract.");
        }

        /// <summary>Copies data from the specified object to the current DTO (data transfer object).</summary>
        /// <param name="obj">Object that data is copied from.</param>
        protected override void CopyFromPlain(InputOutputElementDefinition obj)
        {
            if (obj == null)
                this.SetNull(true);
            else
            {
                this.Name = obj.Name;
                this.NameAlt = obj.NameAlt;
                this.Title = obj.Title;
                this.Description = obj.Description;
                this.IsInput = obj.IsInput;
                this.ElementIndexSpecified = obj.ElementIndexSpecified;
                this.ElementIndex = obj.ElementIndex;
                this.BoundsDefined = obj.BoundsDefined;
                this.MinimalValue = obj.MinimalValue;
                this.MaximalValue = obj.MaximalValue;
                this.TargetValueDefined = obj.TargetValueDefined;
                this.TargetValue = obj.TargetValue;
                this.ScalingLengthDefined = obj.ScalingLengthDefined;
                this.ScalingLength = obj.ScalingLength;
            }

        }

        /// <summary>Copies data from the current DTO to the specified object.</summary>
        /// <param name="obj">Object that data is copied to.</param>
        protected override void CopyToPlain(ref InputOutputElementDefinition obj)
        {
            if (GetNull())
                obj = null;
            else
            {
                obj.IsInput = this.IsInput;
                obj.ElementIndexSpecified = this.ElementIndexSpecified;
                obj.ElementIndex = this.ElementIndex;
                obj.Name = this.Name;
                obj.NameAlt = this.NameAlt;
                obj.Title = this.Title;
                obj.Description = this.Description;
                obj.BoundsDefined = this.BoundsDefined;
                obj.MinimalValue = this.MinimalValue;
                obj.MaximalValue = this.MaximalValue;
                obj.TargetValueDefined = this.TargetValueDefined;
                obj.TargetValue = this.TargetValue;
                obj.ScalingLengthDefined = this.ScalingLengthDefined;
                obj.ScalingLength = this.ScalingLength;
            }
        }

        #endregion Operation

    } // abstract class InputOutputElementDefinitionDto


    /// <summary>DTO (data transfer object) for data output element definition.</summary>
    /// $A Ifor Mar11;
    public class OutputElementDefinitionDto : InputOutputElementDefinitionDto<OutputElementDefinition>
    {

        public OutputElementDefinitionDto()
            : base()
        { }


        #region Operation


        /// <summary>Creates and returns an object consistent with current DTO.</summary>
        public override OutputElementDefinition CreateObject()
        {
            return new OutputElementDefinition(this.ElementIndex, this.Name, this.Title, this.Description);
        }


        protected override void CopyFromPlain(InputOutputElementDefinition obj)
        {
            // Remark: there is actually no need for override, but this is prepared for any case if 
            // some data is added to this class later.
            base.CopyFromPlain(obj);
        }



        protected override void CopyToPlain(ref InputOutputElementDefinition obj)
        {
            // Remark: there is actually no need for override, but this is prepared for any case if 
            // some data is added to this class later.
            base.CopyToPlain(ref obj);
        }


        #endregion Operation


    } // class OutputElementDefinitionDto


    /// <summary>DTO (data transfer object) for data input element definition.</summary>
    /// $A Ifor Mar11;
    public class InputElementDefinitionDto : InputOutputElementDefinitionDto<InputElementDefinition>
    {

        public InputElementDefinitionDto()
            : base()
        { }


        #region Data

        /// <summary>Flag indicating whether default value is defined for the input parameter 
        /// described by the current eleemnt description.</summary>
        public bool DefaultValueDefined;

        /// <summary>Default value for the output data element described by the current definition.</summary>
        public double DefaultValue;

        /// <summary>Flag indicating whether optimization parameter index is defined for the input parameter 
        /// described by the current element description.
        /// This index tells which optimization parameter corresponds to the current sampled data input parameter.</summary>
        public bool OptimizationIndexSpecified;

        /// <summary>Optimization parameter index of the data element described by the current definition.
        /// This index tells which optimization parameter corresponds to the current sampled data input parameter.</summary>
        public int OptimizationIndex;

        /// <summary>Discretization step that is used in cases where parameter the input parameter has
        /// discrete values. Discretization starts at MinValue.</summary>
        /// <remarks>This field was required by the Jozef Stefan optimization group.</remarks>
        public double DiscretizationStep = 0.0;


        #endregion Data


        #region Operation


        /// <summary>Creates and returns an object consistent with current contents of the vector.</summary>
        public override InputElementDefinition CreateObject()
        {
            return new InputElementDefinition(this.ElementIndex, this.Name, this.Title, this.Description);
        }


        protected override void CopyFromPlain(InputOutputElementDefinition obj)
        {
            base.CopyFromPlain(obj);
            InputElementDefinition objInput = obj as InputElementDefinition;
            if (objInput != null)
            {
                this.DefaultValueDefined = objInput.DefaultValueDefined;
                this.DefaultValue = objInput.DefaultValue;
                this.OptimizationIndexSpecified = objInput.OptimizationIndexSpecified;
                this.OptimizationIndex = objInput.OptimizationIndex;
                this.DiscretizationStep = objInput.DiscretizationStep;
            }
        }


        protected override void CopyToPlain(ref InputOutputElementDefinition obj)
        {
            if (this.GetNull())
                obj = null;
            else
            {
                if (obj == null)
                    obj = this.CreateObject();
                base.CopyToPlain(ref obj);
                InputElementDefinition objInput = obj as InputElementDefinition;
                if (objInput != null)
                {
                    objInput.DefaultValueDefined = this.DefaultValueDefined;
                    objInput.DefaultValue = this.DefaultValue;
                    objInput.OptimizationIndexSpecified = this.OptimizationIndexSpecified;
                    objInput.OptimizationIndex = this.OptimizationIndex;
                    objInput.DiscretizationStep = this.DiscretizationStep;
                }
            }
        }

        #endregion Operation

    } // class InputElementDefinitionDto


    /// <summary>DTO (data transfer object) for data definition that contains input and output elements.</summary>
    /// $A Ifor Mar11;
    public class InputOutputDataDefinitonDto : SerializationDto<InputOutputDataDefiniton>
    {

        public InputOutputDataDefinitonDto()
            : base()
        { }


        #region Data


        public InputElementDefinitionDto[] Input = null;

        public OutputElementDefinitionDto[] Output = null;

        /// <summary>Name of the current definition of input parameters and output values of a model.
        /// <para>Default value is specified by the static property <see cref="InputOutputDataDefiniton.DefaultName"/>.</para></summary>
        public string Name = InputOutputDataDefiniton.DefaultName;

        /// <summary>Description of the current definition of input parameters and output values of a model.
        /// <para>Default value is specified by the static property <see cref="InputOutputDataDefiniton.DefaultDescription"/>.</para></summary>
        public string Description = InputOutputDataDefiniton.DefaultDescription;

        /// <summary>Gets number of input parameters.</summary>
        public int InputLength = 10;

        /// <summary>Gets number of output values.</summary>
        public int OutputLength = 100;

        #endregion Data


        #region Operation


        /// <summary>Creates and returns an object consistent with current DTO.</summary>
        public override InputOutputDataDefiniton CreateObject()
        {
            return new InputOutputDataDefiniton();
        }



        /// <summary>Copies data from an object to the current DTO.</summary>
        /// <param name="obj">Object which data is copied from.</param>
        protected override void CopyFromPlain(InputOutputDataDefiniton obj)
        {
            if (obj == null)
                this.SetNull(true);
            else
            {
                this.InputLength = 0;
                this.OutputLength = 0;
                // Copy list of input element definitions:
                SerializationDto.CopyListFromObject
                    <InputElementDefinitionDto, InputElementDefinition>
                    (obj.InputElementList, ref this.Input);
                // Copy list of output element definitions:
                SerializationDto.CopyListFromObject
                    <OutputElementDefinitionDto, OutputElementDefinition>
                    (obj.OutputElementList, ref this.Output);

                this.Name = obj.Name;
                this.Description = obj.Description;
                this.InputLength = obj.InputLength;
                this.OutputLength = obj.OutputLength;

            }
        }




        /// <summary>Copies data from an object to the current DTO.</summary>
        /// <param name="obj">Object which data is copied from.</param>
        protected override void CopyToPlain(ref InputOutputDataDefiniton obj)
        {
            if (this.GetNull())
                obj = null;
            else
            {
                if (obj == null)
                    obj = this.CreateObject();
                obj.Name = this.Name;
                obj.Description = this.Description;
                //obj.InputLength = this.InputLength;
                //obj.OutputLength = this.OutputLength;

                // Copy list of input element definitions:
                SerializationDto.CopyListToObjectReturned
                    <InputElementDefinitionDto, InputElementDefinition>
                    (this.Input, obj.InputElementList);

                // Copy list of output element definitions:
                SerializationDto.CopyListToObjectReturned
                    <OutputElementDefinitionDto, OutputElementDefinition>
                    (this.Output, obj.OutputElementList);

            }
        }

        #endregion Operation


    } // class InputOutputDataDefinitonDto


}