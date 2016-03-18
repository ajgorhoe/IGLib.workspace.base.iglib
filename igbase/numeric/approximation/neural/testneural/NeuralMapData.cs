using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;
using IG.Num;
using IG.Neural;


namespace IG.Neural
{

    #region Dto


    /// <summary>DTO (data transfer object) for neural data element mapping definition.</summary>
    /// <typeparam name="ElementType">Actual type of the object whose data is represented by the current DTO.</typeparam>
    /// $A Igor Jul 19; tako78 Jul 19 
    public abstract class MapNeuralImputOutputElementDefinitionDto<ElementType> : SerializationDtoBase<ElementType, MappingDefinitionElement>
        where ElementType : MappingDefinitionElement
    {

        public MapNeuralImputOutputElementDefinitionDto()
            : base()
        {
        }

        #region Data

        public int OriginalElementIndex = -1;

        public bool OriginalElementIndexSpecified = false;

        public int MappedElementIndex = -1;

        public bool MappedElementIndexSpecified = false;

        public string OriginalName;

        public string OriginalTitle;

        public string OriginalDescription;

        #endregion

        #region Operation

        public override ElementType CreateObject()
        {
            throw new NotImplementedException("CraeateObject() is not implemented bnecause the return type is abstract.");
        }

        protected override void CopyFromPlain(MappingDefinitionElement obj)
        {
            if (obj == null)
                this.SetNull(true);
            else
            {
                this.OriginalElementIndex = obj.OriginalElementIndex;
                this.OriginalElementIndexSpecified = obj.OriginalElementIndexSpecified;
                this.MappedElementIndex = obj.MappedElementIndex;
                this.MappedElementIndexSpecified = obj.MappedElementIndexSpecified;
                this.OriginalName = obj.OriginalName;
                this.OriginalTitle = obj.OriginalTitle;
                this.OriginalDescription = obj.OriginalDescription;              
            }

        }

        protected override void CopyToPlain(ref MappingDefinitionElement obj)
        {
            if (GetNull())
                obj = null;
            else
            {
                obj.OriginalElementIndex = this.OriginalElementIndex;
                obj.OriginalElementIndexSpecified = this.OriginalElementIndexSpecified;                
                obj.MappedElementIndex = this.MappedElementIndex;
                obj.MappedElementIndexSpecified = this.MappedElementIndexSpecified;
                obj.OriginalName = this.OriginalName;
                obj.OriginalTitle = this.OriginalTitle;
                obj.OriginalDescription = this.OriginalDescription;
            }
        }

        #endregion

    }


    /// <summary>DTO (data transfer object) for neural data input element mapping definition.</summary>
    /// $A Igor Jul 19; tako78 Jul 19 
    public class MapImputElementDefinitionDto : MapNeuralImputOutputElementDefinitionDto<InputMappingDefinitionElement>
    {
        public MapImputElementDefinitionDto()
            : base()
        { }

        #region Operation

        public override InputMappingDefinitionElement CreateObject()
        {
            return new InputMappingDefinitionElement(this.OriginalElementIndex, this.MappedElementIndex, this.OriginalName, this.OriginalTitle, this.OriginalDescription);
        }

        protected override void CopyFromPlain(MappingDefinitionElement obj)
        {
            base.CopyFromPlain(obj);
        }

        protected override void CopyToPlain(ref MappingDefinitionElement obj)
        {
            base.CopyToPlain(ref obj);
        }

        #endregion
    }


    /// <summary>DTO (data transfer object) for neural data output element mapping definition.</summary>
    /// $A Igor Jul 19; tako78 Jul 19 
    public class MapOutputElementDefinitionDto : MapNeuralImputOutputElementDefinitionDto<OutputMappingDefinitionElement>
    {
        public MapOutputElementDefinitionDto()
            : base()
        { }

        #region Operation

        public override OutputMappingDefinitionElement CreateObject()
        {
            return new OutputMappingDefinitionElement(this.OriginalElementIndex, this.MappedElementIndex, this.OriginalName, this.OriginalTitle, this.OriginalDescription);
        }

        protected override void CopyFromPlain(MappingDefinitionElement obj)
        {
            base.CopyFromPlain(obj);
        }

        protected override void CopyToPlain(ref MappingDefinitionElement obj)
        {
            base.CopyToPlain(ref obj);
        }

        #endregion
    }


    public class MapDataDefinitionDto : SerializationDto<MappingDefinition>
    {

        public MapDataDefinitionDto()
            : base()
        { }

        #region Data

        public MapImputElementDefinitionDto[] Input = null;

        public MapOutputElementDefinitionDto[] Output = null;

        public int InputLength = 10;

        public int OutputLength = 10;

        public bool CheckInputUniqueness = false;

        public bool CheckOutputUniqueness = false;

        public bool CheckInputNameConsistency = false;

        public bool CheckOutputNameConsistency = false;

        #endregion

        #region Operation

        public override MappingDefinition CreateObject()
        {
            return new MappingDefinition();
        }

        protected override void CopyFromPlain(MappingDefinition obj)
        {
            if (obj == null)
                this.SetNull(true);
            else
            {
                this.InputLength = 0;
                this.OutputLength = 0;
                this.CheckInputUniqueness = false;
                this.CheckInputNameConsistency = false;
                this.CheckOutputUniqueness = false;
                this.CheckOutputNameConsistency = false;

                // Copy list of input element definitions:
                SerializationDto.CopyListFromObject
                    <MapImputElementDefinitionDto, InputMappingDefinitionElement>
                    (obj.MapInputElementList, ref this.Input);

                // Copy list of output element definitions:
                SerializationDto.CopyListFromObject
                    <MapOutputElementDefinitionDto, OutputMappingDefinitionElement>
                    (obj.MapOutputElementList, ref this.Output);

                this.InputLength = obj.MappedInputLength;
                this.CheckInputUniqueness = obj.CheckInputUniqueness;
                this.CheckInputNameConsistency = obj.CheckInputNameConsistency;
                
                this.OutputLength = obj.MappedOutputLength;
                this.CheckOutputUniqueness = obj.CheckOutputUniqueness;
                this.CheckOutputNameConsistency = obj.CheckOutputNameConsistency;
            }
        }

        protected override void CopyToPlain(ref MappingDefinition obj)
        {
            if (this.GetNull())
                obj = null;
            else
            {
                if (obj == null)
                    obj = this.CreateObject();
                obj.CheckInputUniqueness = this.CheckInputUniqueness;
                obj.CheckInputNameConsistency = this.CheckInputNameConsistency;
                obj.CheckOutputUniqueness = this.CheckOutputUniqueness;
                obj.CheckOutputNameConsistency = this.CheckOutputNameConsistency;

                // Copy list of input element definitions:
                SerializationDto.CopyListToObjectReturned
                    <MapImputElementDefinitionDto, InputMappingDefinitionElement>
                    (this.Input, obj.MapInputElementList);

                // Copy list of output element definitions:
                SerializationDto.CopyListToObjectReturned
                    <MapOutputElementDefinitionDto, OutputMappingDefinitionElement>
                    (this.Output, obj.MapOutputElementList);
            }
        }

        #endregion
    }


    #endregion Dto


    #region MapData


    /// <summary>Base class for input or output data element mapping definition.</summary>
    /// $A Igor Jul 19; tako78 Jul 19 
    public abstract class MappingDefinitionElement
    {
        #region Construction


        /// <summary>Constructor.</summary>
        /// <param name="originalElementIndex">Index of the original input or output data element specified by the definision.
        /// If less than 0 is specified then it is considered that element index is not known or defined in the current context.</param>
        /// <param name="mappedElementIndex">Index of the mapped input or output data element specified by the current definision.
        /// If less than 0 is specified then it is considered that element index is not known or defined in the current context.</param>
        /// <param name="originalName">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="originalTitle">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="originalDescription">Element description. Can be an arbvitrary string.</param>
        /// $A Igor Jul 19; tako78 Jul 19 
        public MappingDefinitionElement(int originalElementIndex, int mappedElementIndex, string originalName, string originalTitle, string originalDescription)
        {
            this.OriginalElementIndex = originalElementIndex;
            this.MappedElementIndex = mappedElementIndex;
            this.OriginalName = originalName;
            this.OriginalTitle = originalTitle;
            this.OriginalDescription = originalDescription;
        }


        /// <summary>Constructor. Element index is unknown, there is no element description.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// $A Igor Jul 19; tako78 Jul 19 
        public MappingDefinitionElement(string name)
            : this(-1, -1, name, null, null)
        {  }


        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// $A Igor Jul 19; tako78 Jul 19 
        public MappingDefinitionElement(string name, string title)
            : this(-1, -1, name, title, null)
        {  }


        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="description">Element description. Can be an arbitrary string.</param>
        /// $A Igor Jul 19; tako78 Jul 19 
        public MappingDefinitionElement(string name, string title, string description)
            : this(-1, -1, name, title, description)
        {  }


        #endregion


        #region Data


        protected int _originalElementIndex = -1;


        protected bool _originalElementindexSpecified = false;


        protected int _mappedElementIndex = -1;


        protected bool _mappedElementindexSpecified = false;


        protected string _originalName;


        protected string _orirginalTitle;


        protected string _originalDescription;


        /// <summary>Specifies the original index of the element described by the mapped definition, within the data vextor (either input or output).</summary>
        /// <remarks>If not specified, then by agreement the OriginalElementIndex is set to -1.
        /// Getter of this property automatically set OriginalElementIndexSpecified flag to false if the property is set to less than 0,
        /// and to true otherwise.</remarks>
        /// $A Igor Jul 19; tako78 Jul 19 
        public virtual int OriginalElementIndex
        {
            get { return _originalElementIndex; }
            set
            {
                _originalElementIndex = value;
                if (value < 0)
                    _originalElementindexSpecified = false;
                else
                    _originalElementindexSpecified = true;
            }
        }


        /// <summary>Specifies whether original element index is specified for the data element described by the mapped definition.</summary>
        /// <remarks>If not specified, then by agreement the OriginalElementIndex is set to -1.
        /// Getter of this property automatically set OriginalElementIndex to -1 if the property is set to false.</remarks>
        /// $A Igor Jul 19; tako78 Jul 19
        public virtual bool OriginalElementIndexSpecified
        {
            get { return _originalElementindexSpecified; }
            set
            {
                _originalElementindexSpecified = value;
                if (value == false)
                    _originalElementIndex = -1;
            }
        }


        /// <summary>Specifies the mapped index of the element described by the current definition, within the data vextor (either input or output).</summary>
        /// <remarks>If not specified, then by agreement the MappedElementIndex is set to -1.
        /// Getter of this property automatically set MappedElementIndexSpecified flag to false if the property is set to less than 0,
        /// and to true otherwise.</remarks>
        /// $A Igor Jul 19; tako78 Jul 19 
        public virtual int MappedElementIndex
        {
            get { return _mappedElementIndex; }
            set
            {
                _mappedElementIndex = value;
                if (value < 0)
                    _mappedElementindexSpecified = false;
                else
                    _mappedElementindexSpecified = true;
            }
        }


        /// <summary>Specifies whether mapped element index is specified for the data element described by the mapped definition.</summary>
        /// <remarks>If not specified, then by agreement the MappedElementIndex is set to -1.
        /// Getter of this property automatically set MappedElementIndex to -1 if the property is set to false.</remarks>
        /// $A Igor Jul 19; tako78 Jul 19
        public virtual bool MappedElementIndexSpecified
        {
            get { return _mappedElementindexSpecified; }
            set
            {
                _mappedElementindexSpecified = value;
                if (value == false)
                    _mappedElementIndex = -1;
            }
        }


        /// <summary>Unique name of the data element described by the mapped definition. 
        /// Considered a kind of variable name that distinguishes between data by short names.</summary>
        /// <remarks>There is an agreement that element names should follow conventions for valid variable names 
        /// in programming languages C++, C# and Java.</remarks>
        /// $A Igor Jul 19; tako78 Jul 19
        public virtual string OriginalName
        { get { return _originalName; } set { _originalName = value; } }


        /// <summary>A title describing what given data element represents. Titles can contain special 
        /// characters and spaces, but should be shorter than descriptions.</summary>
        /// $A Igor Jul 19; tako78 Jul 19
        public virtual string OriginalTitle
        { get { return _orirginalTitle; } set { _orirginalTitle = value; } }


        /// <summary>Describes the meaning of a data element used as part of neural network input or output data.</summary>
        /// $A Igor Jul 19; tako78 Jul 19
        public virtual string OriginalDescription
        { get { return _originalDescription; } set { _originalDescription = value; } }


        #endregion
    } // class MappingDefinitionElement


    /// <summary>Input data element mapping definition for neural networks.</summary>
    /// $A Igor Jul 19; tako78 Jul 19
    public class InputMappingDefinitionElement : MappingDefinitionElement
    {

        #region Constructor


        /// <summary>Constructor.</summary>
        /// <param name="originalElementIndex">OriginalIndex of the input or output data element specified by the mapped definision.
        /// If less than 0 is specified then it is considered that original element index is not known or defined in the current context.</param>
        /// <param name="mappedElemntIndex">MappedIndex of the input or output data element specified by the current definision.
        /// If less than 0 is specified then it is considered that original element index is not known or defined in the current context.</param>
        /// <param name="originalName">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="originalTitle">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="originalDescription">Element description. Can be an arbvitrary string.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public InputMappingDefinitionElement(int originalElementIndex, int mappedElemntIndex, string originalName, string originalTitle, string originalDescription) :
            base(originalElementIndex, mappedElemntIndex, originalName, originalTitle, originalDescription)
        {   }


        /// <summary>Constructor. Element index is unknown, there is no element description.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public InputMappingDefinitionElement(string name)
            : this(-1, -1, name, null, null)
        {  }


        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public InputMappingDefinitionElement(string name, string title)
            : this(-1, -1, name, title, null)
        {  }


        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="description">Element description. Can be an arbitrary string.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public InputMappingDefinitionElement(string name, string title, string description)
            : this(-1, -1, name, title, description)
        { }


        #endregion
    
    } // class InputMappingDefinitionElement


    /// <summary>Output data element mapping definition for neural networks.</summary>
    /// $A Igor Jul 19; tako78 Jul 19
    public class OutputMappingDefinitionElement : MappingDefinitionElement
    {

        #region Constructor


        /// <summary>Constructor.</summary>
        /// <param name="originalElementIndex">OriginalIndex of the input or output data element specified by the mapped definision.
        /// If less than 0 is specified then it is considered that element index is not known or defined in the current context.</param>
        /// <param name="mappedElementIndex">MappedIndex of the input or output data element specified by the current definision.
        /// If less than 0 is specified then it is considered that element index is not known or defined in the current context.</param>
        /// <param name="originalName">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="originalTitle">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="originalDescription">Element description. Can be an arbvitrary string.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public OutputMappingDefinitionElement(int originalElementIndex, int mappedElementIndex, string originalName, string originalTitle, string originalDescription) :
            base(originalElementIndex, mappedElementIndex, originalName, originalTitle, originalDescription)
        {   }


        /// <summary>Constructor. Element index is unknown, there is no element description.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public OutputMappingDefinitionElement(string name)
            : this(-1, -1, name, null, null)
        {  }


        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public OutputMappingDefinitionElement(string name, string title)
            : this(-1, -1, name, title, null)
        {  }


        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="description">Element description. Can be an arbitrary string.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public OutputMappingDefinitionElement(string name, string title, string description)
            : this(-1, -1, name, title, description)
        { }


        #endregion

    } // class OutputMappingDefinitionElement


    /// <summary>Definition of input and output data mapping.
    /// Contains Mapped and Original Index, descriptiove information about individual eleemnts of input and output and default valuess.</summary>
    /// $A Igor Jul 19; tako78 Jul 19
    public class MappingDefinition
    {

        #region Data


        protected bool _checkInputUniqueness;


        protected bool _checkOutputUniqueness;


        protected bool _checkInputNameConsistency;


        protected bool _checkOutputNameConsistency;


        protected List<InputMappingDefinitionElement> _input = new List<InputMappingDefinitionElement>();


        protected List<OutputMappingDefinitionElement> _output = new List<OutputMappingDefinitionElement>();


        /// <summary>Gets number of input parameters.</summary>
        /// $A Igor Jul 19; tako78 Jul 19
        public int MappedInputLength
        { get { if (_input == null) return 0; else return _input.Count; } }


        /// <summary>Gets number of output values.</summary>
        /// $A Igor Jul 19; tako78 Jul 19
        public int MappedOutputLength
        { get { if (_output == null) return 0; else return _output.Count; } }


        /// <summary>Gets true if original and mapped input element indexes are unique.</summary> 
        /// $A Igor Jul 19; tako78 Jul 19
        public bool CheckInputUniqueness
        {
            get { return _checkInputUniqueness; }
            set { _checkInputUniqueness = value; }
        }


        /// <summary>Gets true if original and mapped output element indexes are unique.</summary> 
        /// $A Igor Jul 19; tako78 Jul 19
        public bool CheckOutputUniqueness
        { 
            get { return _checkOutputUniqueness; }
            set { _checkOutputUniqueness = value; }
        }



        /// <summary>Gets true if names in mappingdata file and names in definitiondata file are consistent.</summary>
        /// $A Igor Jul 19; tako78 Jul 19 
        public bool CheckInputNameConsistency
        {
            get { return _checkInputNameConsistency; }
            set { _checkInputNameConsistency = value; }
        }


        /// <summary>Gets true if names in mappingdata file and names in definitiondata file are consistent.</summary>
        /// $A Igor Jul 19; tako78 Jul 19 
        public bool CheckOutputNameConsistency
        {
            get { return _checkOutputNameConsistency; }
            set { _checkOutputNameConsistency = value; }
        }


        #endregion


        #region DataOperation


        public virtual List<InputMappingDefinitionElement> MapInputElementList
        {
            get
            {
                if (_input == null)
                    _input = new List<InputMappingDefinitionElement>();
                return _input;
            }
        }


        public virtual List<OutputMappingDefinitionElement> MapOutputElementList
        {
            get
            {
                if (_output == null)
                    _output = new List<OutputMappingDefinitionElement>();
                return _output;
            }
        }


        public virtual InputMappingDefinitionElement GetMapInputElement(int which)
        {
            if (which < 0 || which >= MappedInputLength)
                throw new IndexOutOfRangeException("Index of input parameter definition out of range: " + which + ".");
            return _input[which];
        }


        public virtual OutputMappingDefinitionElement GetMapOutputElement(int which)
        {
            if (which < 0 || which >= MappedOutputLength)
                throw new IndexOutOfRangeException("Index of output parameter definition out of range: " + which + ".");
            return _output[which];
        }


        public virtual void SetMapInputElement(int which, InputMappingDefinitionElement element)
        {
            if (which < 0 || which >= MappedInputLength)
                throw new IndexOutOfRangeException("Index of input parameter definition out of range: " + which + ".");
            _input[which] = element;
        }


        public virtual void SetMapOutputElement(int which, OutputMappingDefinitionElement element)
        {
            if (which < 0 || which >= MappedOutputLength)
                throw new IndexOutOfRangeException("Index of output parameter definition out of range: " + which + ".");
            _output[which] = element;
        }


        public virtual void AddInputElement(InputMappingDefinitionElement element)
        {
            if (element == null)
                throw new ArgumentNullException("Input element definition to be added is not specified (null reference).");
            _input.Add(element);
        }


        public virtual void AddOutputElement(OutputMappingDefinitionElement element)
        {
            if (element == null)
                throw new ArgumentNullException("Output element definition to be added is not specified (null reference).");
            _output.Add(element);
        }


        #endregion


        #region Checking


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Gets true if original and mapped input element indexes are unique.</returns>
        /// $A Igor Jul 19; tako78 Jul 19
        public bool IsInputDataConsistent()
        {        
            if (this.CheckInputUniqueness)
            {
                IVector _originalElementIndex = new Vector(MappedInputLength);
                IVector _mappedElementIndex = new Vector(MappedInputLength);
                
                // Copy Original and Mapped Input Indexes to Vector.
                for (int i = 0; i < MappedInputLength; i++)
                {
                    InputMappingDefinitionElement _mappingElement = null;
                    _mappingElement = GetMapInputElement(i);
                    _originalElementIndex[i] = _mappingElement.OriginalElementIndex;
                    _mappedElementIndex[i] = _mappingElement.MappedElementIndex;
                }

                // Check if Input Indexes are unique.
                for (int i = 0; i < MappedInputLength; i++)
                {
                    double tmpOriginalIndex = _originalElementIndex[i];
                    double tmpMappedIndex = _mappedElementIndex[i];
                    for (int j = i + 1; j < MappedInputLength; j++)
                    {
                        if (tmpOriginalIndex == _originalElementIndex[j])
                            return false;
                        if (tmpMappedIndex == _mappedElementIndex[j])
                            return false;
                    }
                }                
            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Gets true if original and mapped output element indexes are unique.</returns>
        /// $A Igor Jul 19; tako78 Jul 19
        public bool IsOutputDataConsistent()
        {
            if (this.CheckOutputUniqueness)
            {
                IVector _originalElementIndex = new Vector(MappedOutputLength);
                IVector _mappedElementIndex = new Vector(MappedOutputLength);

                // Copy Original and Mapped Output Indexes to Vector.
                for (int i = 0; i < MappedOutputLength; i++)
                {
                    OutputMappingDefinitionElement _mappingElement = null;
                    _mappingElement = GetMapOutputElement(i);
                    _originalElementIndex[i] = _mappingElement.OriginalElementIndex;
                    _mappedElementIndex[i] = _mappingElement.MappedElementIndex;
                }

                // Check if Output Indexes are unique.
                for (int i = 0; i < MappedOutputLength; i++)
                {
                    double tmpOriginalIndex = _originalElementIndex[i];
                    double tmpMappedIndex = _mappedElementIndex[i];
                    for (int j = i + 1; j < MappedOutputLength; j++)
                    {
                        if (tmpOriginalIndex == _originalElementIndex[j])
                            return false;
                        if (tmpMappedIndex == _mappedElementIndex[j])
                            return false;
                    }
                }                

            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataDefinition">Definition data.</param>
        /// <returns>Gets true if names in mappingdata file and names in definitiondata file are consistent.</returns>
        /// $A Igor Jul 19; tako78 Jul 19
        public bool IsInputDataConsistent(InputOutputDataDefiniton dataDefinition)
        {
            if (dataDefinition != null)
            {
                if (this.CheckInputNameConsistency)
                {
                    int _mappedOriginalElementIndex = 0;
                    string _mappedOriginalNeme = "";
                    string _originalNeme = "";

                    // Check if Name parameter in MappingData file is consistant with Name parameter in DefinitionData file.
                    for (int i = 0; i < MappedInputLength; i++)
                    {
                        InputMappingDefinitionElement _mappingElement = null;
                        _mappingElement = GetMapInputElement(i);
                        _mappedOriginalElementIndex = _mappingElement.OriginalElementIndex;
                        _mappedOriginalNeme = Convert.ToString(_mappingElement.OriginalName);

                        InputElementDefinition _definitionElement = null;
                        _definitionElement = dataDefinition.GetInputElement(_mappedOriginalElementIndex);
                        _originalNeme = _definitionElement.Name;

                        if (_mappedOriginalNeme != _originalNeme)
                            return false;
                    }
                }
                
            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataDefinition">Definition data.</param>
        /// <returns>Gets true if names in mappingdata file and names in definitiondata file are consistent.</returns>
        /// $A Igor Jul 19; tako78 Jul 19
        public bool IsOutputDataConsistent(InputOutputDataDefiniton dataDefinition)
        {
            if (dataDefinition != null)
            {
                if (this.CheckOutputNameConsistency)
                {
                    int _mappedOriginalElementIndex = 0;
                    string _mappedOriginalNeme = "";
                    string _originalNeme = "";

                    // Check if Name parameter in MappingData file is consistant with Name parameter in DefinitionData file.
                    for (int i = 0; i < MappedOutputLength; i++)
                    {
                        OutputMappingDefinitionElement _mappingElement = null;
                        _mappingElement = GetMapOutputElement(i);
                        _mappedOriginalElementIndex = _mappingElement.OriginalElementIndex;
                        _mappedOriginalNeme = Convert.ToString(_mappingElement.OriginalName);

                        OutputElementDefinition _definitionElement = null;
                        _definitionElement = dataDefinition.GetOutputElement(_mappedOriginalElementIndex);
                        _originalNeme = _definitionElement.Name;

                        if (_mappedOriginalNeme != _originalNeme)
                            return false;
                    }
                }

            }
            return true;
        }


        #endregion Checking


        #region StaticMethods


        /// <summary>Saves mapping data definition to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="mapDataDef">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file where data is saved.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public static void SaveJson(MappingDefinition mapDataDef, string filePath)
        {
            MapDataDefinitionDto dtoOriginal = new MapDataDefinitionDto();
            dtoOriginal.CopyFrom(mapDataDef);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<MapDataDefinitionDto>(dtoOriginal, filePath);
        }

        /// <summary>Restores mapping data definition from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which training data is restored.</param>
        /// <param name="mapDataDefRestored">Mapping definition data that is restored by deserialization.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public static void LoadJson(string filePath, ref MappingDefinition mapDataDefRestored)
        {
            ISerializer serializer = new SerializerJson();
            MapDataDefinitionDto dtoRestored = serializer.DeserializeFile<MapDataDefinitionDto>(filePath);
            mapDataDefRestored = new MappingDefinition();
            dtoRestored.CopyTo(ref mapDataDefRestored);
        }


        #endregion


        #region ExampleData


        /// <summary>Creates and returns an example mapping data definition.</summary>
        /// <param name="inputLength">Number of input elements.</param>
        /// <param name="outputLenght">Number of output elements.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public static MappingDefinition CreateExample(int inputLength, int outputLenght)
        {
            MappingDefinition ret = new MappingDefinition();
            for (int i = 0; i < inputLength; ++i)
            {
                InputMappingDefinitionElement inputElement = new InputMappingDefinitionElement(i+1, i,
                    "param_" + i.ToString(), "Input parameter " + i + ".",
                    "This is description of input parameter No. " + i + ".");
                ret.AddInputElement(inputElement);
            }

            for (int i = 0; i < outputLenght; ++i)
            {
                OutputMappingDefinitionElement outputElement = new OutputMappingDefinitionElement(i+1, i,
                    "param_" + i.ToString(), "Input parameter " + i + ".",
                    "This is description of input parameter No. " + i + ".");
                ret.AddOutputElement(outputElement);
            }
            
            return ret;
        }


        #endregion

    } // class MappingDefinition


    #endregion


    #region MapDataProgram


    /// <summary>
    /// 
    /// </summary>
    public interface IDataMapper
    {

        void MapInput(IVector reducedInput, ref IVector originalInput);

        void MapOutput(IVector originalOutput, ref IVector reducedOutput);

    }


    /// <summary>
    /// 
    /// </summary>
    public class DataMapperIdentity: IDataMapper
    {

        public DataMapperIdentity()
        {  }


        /// <summary>Transfers elemts of reducedInput vector in the originalInput vector.</summary>
        /// <param name="reducedInput">reducedInput elements.</param>
        /// <param name="originalInput">originalInput elements.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public virtual void MapInput(IVector reducedInput, ref IVector originalInput)
        {
            Vector.Copy(reducedInput, ref originalInput);
        }


        /// <summary>Transfers elemts of originalOutput vector in the reducedOutput vector.</summary>
        /// <param name="originalOutput">originalOutput elements.</param>
        /// <param name="reducedOutput">reducedOutput elements.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public virtual void MapOutput(IVector originalOutput, ref IVector reducedOutput)
        {
            Vector.Copy(reducedOutput, ref originalOutput);
        }

    } // class DataMapperIdentity


    /// <summary>
    /// 
    /// </summary>
    public class DataMapperSimple: DataMapperIdentity, IDataMapper
    {

        protected DataMapperSimple()
            : base()
        {
            MappingDefinition = null;
            DataDefinition = null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="definition">Data that defines how input and output mappings are performed.
        /// If null then identity mapping is performed.</param>
        /// <param name="dataDefinition">Definition data.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public DataMapperSimple(MappingDefinition definition, InputOutputDataDefiniton dataDefinition)
            : this()
        {
            this.MappingDefinition = definition;
            this.DataDefinition = dataDefinition;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappingDefinitionFilePath">JSON file containing data that defines how input and output mappings are performed.
        /// If null then identity mapping is performed.</param>
        /// <param name="dataDefinitionPath">JSON file containing data definitions.</param>
        /// <exception cref="NotImplementedException">When file path is not specified or file does not exist.</exception>
        /// $A Igor Jul 19; tako78 Jul 19
        public DataMapperSimple(string mappingDefinitionFilePath, string dataDefinitionPath)
            : this()
        {
            MappingDefinition definition = null;
            InputOutputDataDefiniton dataDefinition = null;
            if (!string.IsNullOrEmpty(mappingDefinitionFilePath))
            {

                if (!File.Exists(mappingDefinitionFilePath))
                {
                    throw new NotImplementedException();
                }
                else
                    MappingDefinition.LoadJson(mappingDefinitionFilePath, ref definition);
            }
            if (!string.IsNullOrEmpty(dataDefinitionPath))
            {

                if (!File.Exists(dataDefinitionPath))
                {
                    throw new NotImplementedException();
                }
                else
                    InputOutputDataDefiniton.LoadJson(mappingDefinitionFilePath, ref dataDefinition);
            }
            this.MappingDefinition = definition;
            this.DataDefinition = dataDefinition;
        }


        private MappingDefinition _mappingDefinition;


        /// <summary>
        /// 
        /// </summary>
        public MappingDefinition MappingDefinition
        {
            get { return _mappingDefinition; }
            protected set
            {
                _mappingDefinition = value;
                if (value != null)
                {
                    if (!value.IsInputDataConsistent())
                        throw new InvalidDataException("Mapping input data is not consistent.");
                }
                if (value != null)
                {
                    if (!value.IsOutputDataConsistent())
                        throw new InvalidDataException("Mapping output data is not consistent.");
                }
            }
        }


        private InputOutputDataDefiniton _dataDefinition;


        /// <summary>
        /// 
        /// </summary>
        public InputOutputDataDefiniton DataDefinition
        {
            get { return _dataDefinition; }
            protected set 
            { 
                _dataDefinition = value;
                if (value != null && MappingDefinition != null)
                {
                    if (!MappingDefinition.IsInputDataConsistent(value))
                        throw new InvalidDataException("Mapping inpuut data is not consistent with definition data.");
                }
                if (value != null && MappingDefinition != null)
                {
                    if (!MappingDefinition.IsOutputDataConsistent(value))
                        throw new InvalidDataException("Mapping output data is not consistent with definition data.");
                }
            }
        }


        /// <summary>Transfer elements from reducedInput vector to original output vector using mapping data for reference.
        /// Elements that are not defined in reducedInput vector are copied from definitiondata.</summary>
        /// <param name="reducedInput">Reduced elements.</param>
        /// <param name="originalInput">Original elements for neural network testing.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public override void MapInput(IVector reducedInput, ref IVector originalInput)
        {
            if (MappingDefinition == null)
            {
                base.MapInput(reducedInput, ref originalInput);  // identity map
            } else
            {
                int definitionDataInputLenght = 0;
                int mappingInputDataLenght = 0;
                bool checkInputNameConsistencyFlag = false;
                bool checkInputUniquenessFlag = true;
                IVector _originalElementIndex = null;
                IVector defaultElementValue = null;

                if (DataDefinition == null)
                    throw new ArgumentNullException("No definition data defined.");

                definitionDataInputLenght = DataDefinition.InputLength;
                mappingInputDataLenght = MappingDefinition.MappedInputLength;
                checkInputNameConsistencyFlag = MappingDefinition.CheckInputNameConsistency;
                checkInputUniquenessFlag = MappingDefinition.CheckInputUniqueness;

                if (reducedInput == null)
                    throw new ArgumentNullException("No optimized data defined.");

                // Copy Original Input Index to Vector.
                _originalElementIndex = new Vector(mappingInputDataLenght);
                for (int i = 0; i < mappingInputDataLenght; i++)
                {
                    InputMappingDefinitionElement _mappingElement = null;
                    _mappingElement = MappingDefinition.GetMapInputElement(i);
                    _originalElementIndex[i] = _mappingElement.OriginalElementIndex;
                }

                // Copy DefaultValue from DefinitionData file to Vector.
                defaultElementValue = new Vector(definitionDataInputLenght);
                for (int i = 0; i < definitionDataInputLenght; i++)
                {
                    InputElementDefinition _definitionElement = null;
                    _definitionElement = DataDefinition.GetInputElement(i);
                    defaultElementValue[i] = _definitionElement.DefaultValue;
                }
              
                // Copy Input Data from reduced vector to Original vector.
                originalInput = new Vector(definitionDataInputLenght);
                int indexCopy = 0;
                for (int i = 0; i < definitionDataInputLenght; i++)
                {
                    if (_originalElementIndex[indexCopy] == i)
                    {
                        originalInput[i] = reducedInput[indexCopy];
                        indexCopy++;
                        if (indexCopy == mappingInputDataLenght)
                            return;
                    }
                    else
                    {
                        originalInput[i] = defaultElementValue[i];
                    }
                }
            }
        }


        /// <summary>Transfer elements from originalOutput vector to original output vector using mapping data for reference.
        /// Only defined elements are copied in reducedOutput vector.</summary>
        /// <param name="originalOutput">Original elements from neural network.</param>
        /// <param name="reducedOutput">Reduced elements.</param>
        /// $A Igor Jul 19; tako78 Jul 19
        public override void MapOutput(IVector originalOutput, ref IVector reducedOutput)
        {
            if (MappingDefinition == null)
            {
                base.MapOutput(originalOutput, ref reducedOutput);  // identity map
            } else
            {
                int definitionDataOutputLenght = 0; 
                int mappingOutputDataLenght = 0;
                bool checkOutputNameConsistencyFlag = false;
                bool checkOutputUniquenessFlag = true;
                IVector reducedElementValue = null;

                definitionDataOutputLenght = originalOutput.Length;
                mappingOutputDataLenght = MappingDefinition.MappedOutputLength;
                checkOutputNameConsistencyFlag = MappingDefinition.CheckOutputNameConsistency;
                checkOutputUniquenessFlag = MappingDefinition.CheckOutputUniqueness;

                if (originalOutput == null)
                    throw new ArgumentNullException("No trained data defined: ");

                // Copy MappedData from MappingData file to Vector.
                reducedElementValue = new Vector(mappingOutputDataLenght);
                for (int i = 0; i < mappingOutputDataLenght; i++)
                {
                    OutputMappingDefinitionElement _mappingElement = null;
                    _mappingElement = MappingDefinition.GetMapOutputElement(i);
                    reducedElementValue[i] = _mappingElement.OriginalElementIndex;
                }

                // Copy Output Data from original vector to reduced vector.
                reducedOutput = new Vector(mappingOutputDataLenght);
                int indexCopy = 0;
                for (int i = 0; i < definitionDataOutputLenght; i++)
                {
                    if (reducedElementValue[indexCopy] == i)
                    {
                        reducedOutput[indexCopy] = originalOutput[i];
                        indexCopy++;
                        if (indexCopy == mappingOutputDataLenght)
                            return;
                    }
                }
            }
        }
    } // class DataMapperSimple

     #endregion
}
