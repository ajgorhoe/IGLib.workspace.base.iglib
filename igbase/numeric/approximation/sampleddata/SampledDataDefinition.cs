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


    /// <summary>Base class for input or output data element definition for vector functions,
    /// approximations, etc.</summary>
    /// $A Igor Feb11;
    public abstract class InputOutputElementDefinition
    {

        #region Construction


        /// <summary>Constructor.</summary>
        /// <param name="elementIndex">Index of the input or output data element specified by the current definision.
        /// If less than 0 is specified then it is considered that element index is not known or defined in the current context.</param>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="description">Element description. Can be an arbvitrary string.</param>
        public InputOutputElementDefinition(int elementIndex, string name, string title, string description)
        {
            this.ElementIndex = elementIndex;
            this.Name = name;
            this.Title = title;
            this.Description = description;
        }

        /// <summary>Constructor. Element index is unknown, there is no element description.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        public InputOutputElementDefinition(string name)
            : this(-1, name, null, null)
        { }

        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        public InputOutputElementDefinition(string name, string title)
            : this(-1, name, title, null)
        { }

        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="description">Element description. Can be an arbitrary string.</param>
        public InputOutputElementDefinition(string name, string title, string description)
            : this(-1, name, title, description)
        { }

        #endregion Construction


        #region Data


        protected string _name;

        protected string _nameAlt;

        protected string _title;

        protected string _description;

        protected bool _isInput = true;

        protected bool _elementindexSpecified = false;

        protected int _elementIndex = -1;

        protected bool _boundsDefined = false;

        protected double _minValue;

        protected double _maxValue;

        protected bool _targetValueDefined = false;

        protected double _targetValue = 0.0;

        protected bool _scalingLengthDefined = false;

        protected double _scalingLength = 0.0;

        /// <summary>Unique name of the data element described by the current definition. 
        /// Considered a kind of variable name that distinguishes between data by short names.</summary>
        /// <remarks>There is an agreement that element names should follow conventions for valid variable names 
        /// in programming languages C++, C# and Java.</remarks>
        public virtual string Name
        { get { return _name; } set { _name = value; } }

        /// <summary>Alternative name of the data element described by the current definition.
        /// <para>Used in transformations between different data sets where parameters may be named differently.</para></summary>
        /// <remarks>There is an agreement that element names should follow conventions for valid variable names 
        /// in programming languages C++, C# and Java.</remarks>
        public virtual string NameAlt
        { get { return _nameAlt; } set { _nameAlt = value; } }

        /// <summary>A title describing what given data element represents. Titles can contain special 
        /// characters and spaces, but should be shorter than descriptions.</summary>
        public virtual string Title
        { get { return _title; } set { _title = value; } }

        /// <summary>Describes the meaning of a data element used as part of input or output data.</summary>
        public virtual string Description
        { get { return _description; } set { _description = value; } }

        /// <summary>Flag specifying whether a data element is input or output element.</summary>
        public virtual bool IsInput
        { get { return _isInput; } set { _isInput = value; } }

        /// <summary>Specifies whether element index is specified for the data element described by the current definition.</summary>
        ///<remarks>If not specified, then by agreement the ElementIndex is set to -1.
        /// Getter of this property automatically set ElementIndex to -1 if the property is set to false.</remarks>
        public virtual bool ElementIndexSpecified
        {
            get { return _elementindexSpecified; }
            set
            {
                _elementindexSpecified = value;
                if (value == false)
                    _elementIndex = -1;
            }
        }

        /// <summary>Specifies the index f the element described by the current definition, within the data vextor (either input or output).</summary>
        ///<remarks>If not specified, then by agreement the ElementIndex is set to -1.
        /// Getter of this property automatically set ElementIndexSpecified flag to false if the property is set to less than 0,
        /// and to true otherwise.</remarks>
        public virtual int ElementIndex
        {
            get { return _elementIndex; }
            set
            {
                _elementIndex = value;
                if (value < 0)
                    _elementindexSpecified = false;
                else
                    _elementindexSpecified = true;
            }
        }

        /// <summary>Flag indicating whethe minimal and maximal value are defined for the  
        /// data element described by the current definition.</summary>
        public virtual bool BoundsDefined
        { get { return _boundsDefined; } set { _boundsDefined = value; } }

        /// <summary>Minimal value for the data element described by the current definition.</summary>
        public virtual double MinimalValue
        { get { return _minValue; } set { _minValue = value; } }

        /// <summary>Maximal value for the data element described by the current definition.</summary>
        public virtual double MaximalValue
        { get { return _maxValue; } set { _maxValue = value; } }

        /// <summary>Flag indicating whether target value is defined for the data element described by the current definition.</summary>
        public virtual bool TargetValueDefined
        { get { return _targetValueDefined; } set { _targetValueDefined = value; } }

        /// <summary>Target value of the current element. Used for optimization.</summary>
        public virtual double TargetValue
        { get { return _targetValue; } set { _targetValue = value; } }

        /// <summary>Flag indicating whether scaling length is defined for the data element described by the current definition.</summary>
        public virtual bool ScalingLengthDefined
        { get { return _scalingLengthDefined; } set { _scalingLengthDefined = value; } }

        /// <summary>Scaling length, used for optimization and other tasks where scaling of input or output quantities is important.</summary>
        public virtual double ScalingLength
        { get { return _scalingLength; } set { _scalingLength = value; } }

        
        #region Operation

        /// <summary>Copies data from one input/output data element definition to another.</summary>
        /// <param name="original">Source object that data is copied from.</param>
        /// <param name="copy">Object to which data is copied.</param>
        /// <remarks>Operation is performed only if both sorce and destination objects are non-null.</remarks>
        public static void CopyPlain(InputOutputElementDefinition original, InputOutputElementDefinition copy)
        {
            if (original != null && copy != null)
            {
                copy.Name = original.Name;
                copy.NameAlt = original.NameAlt;
                copy.Title = original.Title;
                copy.Description = original.Description;
                copy.IsInput = original.IsInput;
                copy.ElementIndexSpecified = original.ElementIndexSpecified;
                copy.ElementIndex = original.ElementIndex;
                copy.BoundsDefined = original.BoundsDefined;
                copy.MinimalValue = original.MinimalValue;
                copy.MaximalValue = original.MaximalValue;
                copy.TargetValueDefined = original.TargetValueDefined;
                copy.TargetValue = original.TargetValue;
                copy.ScalingLengthDefined = original.ScalingLengthDefined;
                copy.ScalingLength = original.ScalingLength;
            }
        }
        
        /// <summary>Calculates randomly distorted bounds on element values and stores them in the specified variables.
        /// <para>Before being randomly changed, input bounds are multiplied by the specified factor.</para>
        /// <para>Global random generator is used to generate random componenets of the distorted bounds.</para></summary>
        /// <param name="DistortionFactor">Factor by which interval bounds are multiplied before randomly distorted.</param>
        /// <param name="RandomFactor">Factor that specifies level of randomness, from 0 (no randomness in 
        /// setting distorted bounds) to 0.4 inclusively. Factor 0.5 would mean that amplitude of additional random
        /// distortion is half of the length of the interval [min, max].</param>
        /// <param name="distortedMinimalValue">Variable where distorted lower bound is stored.</param>
        /// <param name="distrotedMaximalValue">Variable where distorted upper bound is stored.</param>
        public void GetDistortedBounds(double DistortionFactor, double RandomFactor,
            ref double distortedMinimalValue, ref double distrotedMaximalValue)
        {
            GetDistortedBounds(DistortionFactor, RandomFactor,
                ref distortedMinimalValue, ref distrotedMaximalValue, 
                null /* RandomGenerator; Global generator will be taken. */);
        }

        /// <summary>Calculates randomly distorted bounds on element values and stores them in the specified variables.
        /// <para>Before being randomly changed, input bounds are multiplied by the specified factor.</para></summary>
        /// <param name="DistortionFactor">Factor by which interval bounds are multiplied before randomly distorted.</param>
        /// <param name="RandomFactor">Factor that specifies level of randomness, from 0 (no randomness in 
        /// setting distorted bounds) to 0.4 inclusively. Factor 0.5 would mean that amplitude of additional random
        /// distortion is half of the length of the interval [min, max].</param>
        /// <param name="distortedMinimalValue">Variable where distorted lower bound is stored.</param>
        /// <param name="distrotedMaximalValue">Variable where distorted upper bound is stored.</param>
        /// <param name="randomGenerator">Random generator used to randomly generate distorted bounds.</param>
        public void GetDistortedBounds(double DistortionFactor, double RandomFactor,
            ref double distortedMinimalValue, ref double distrotedMaximalValue, IRandomGenerator randomGenerator)
        {
            if (DistortionFactor == 0.0)
                DistortionFactor = 0.5;
            if (RandomFactor < 0)
                RandomFactor = -RandomFactor;
            if (RandomFactor > 0.4)
                RandomFactor = 0.4;
            if (randomGenerator == null)
                randomGenerator = RandomGenerator.Global;
            double min = this.MinimalValue, max = this.MaximalValue;
            if (this.BoundsDefined)
            {
                // First, multiply bounds by distortion factor.
                if (DistortionFactor < 0)
                {
                    // Swap bounds first if the factor is negative:
                    M.Swap(ref min, ref max);
                }
                min *= DistortionFactor;
                max *= DistortionFactor;
                // Then, define bounds for the distorted bounds: 
                double length = max - min;
                double minLower = min - length * RandomFactor;
                double minUpper = min + length * RandomFactor;
                double maxLower = max - length * RandomFactor;
                double maxUpper = max + length * RandomFactor;
                // Correct if something changed its sign at this stage:
                if (Math.Sign(minLower) != Math.Sign(min))
                    minLower = 0.5 * min;
                if (Math.Sign(minUpper) != Math.Sign(min))
                    minUpper = 0.5 * min;
                if (Math.Sign(maxLower) != Math.Sign(max))
                    maxLower = 0.5 * max;
                if (Math.Sign(maxUpper) != Math.Sign(max))
                    maxUpper = 0.5 * max;
                // Correct if the order of lower/upper bound limits is not correct:
                if (minLower > minUpper)
                    M.Swap(ref minLower, ref minUpper);
                if (maxLower > maxUpper)
                    M.Swap(ref maxLower, ref maxUpper);
                if (minUpper > maxLower)
                {
                    if (minUpper > maxUpper)
                        M.Swap(ref minUpper, ref maxUpper);
                    else if (minUpper > maxLower)
                        M.Swap(ref minUpper, ref maxLower);
                    if (minLower > maxLower)
                        M.Swap(ref minLower, ref maxLower);
                    if (minLower > minUpper)
                        M.Swap(ref minLower, ref minUpper);
                    if (maxLower > maxUpper)
                        M.Swap(ref maxLower, ref maxUpper);
                }
                // Determine the final values of distorted bounds:
                if (minLower == minUpper)
                    min = minLower;
                else
                    min = randomGenerator.NextDouble(minLower, minUpper);
                if (maxLower == maxUpper)
                    max = maxLower;
                else
                    max = randomGenerator.NextDouble(maxLower, maxUpper);
            }
            distortedMinimalValue = min;
            distrotedMaximalValue = max;
        }

        /// <summary>Returns a default name for the specified input data element.</summary>
        /// <param name="which">Index of the element (zero-based).</param>
        public static string GetDefaultInputElementName(int which)
        {
            return "Input_" + which.ToString("000000");
        }

        /// <summary>Returns a default name for the specified output data element.</summary>
        /// <param name="which">Index of the element (zero-based).</param>
        public static string GetDefaultOutputElementName(int which)
        {
            return "Output_" + which.ToString("000000");
        }

        /// <summary>Returns a default title for the specified input data element.</summary>
        /// <param name="which">Index of the element (zero-based).</param>
        public static string GetDefaultInputElementTitle(int which)
        {
            return "Input " + which.ToString();
        }

        /// <summary>Returns a default title for the specified output data element.</summary>
        /// <param name="which">Index of the element (zero-based).</param>
        public static string GetDefaultOutputElementTitle(int which)
        {
            return "Output " + which.ToString();
        }

        /// <summary>Returns a default description for the specified input data element.</summary>
        /// <param name="which">Index of the element (zero-based).</param>
        public static string GetDefaultInputElementDescription(int which)
        {
            return "Input data element No. " + which.ToString() + ".";
        }

        /// <summary>Returns a default description for the specified output data element.</summary>
        /// <param name="which">Index of the element (zero-based).</param>
        public static string GetDefaultOutputElementDescription(int which)
        {
            return "Output data element No. " + which.ToString() + ".";
        }

        #endregion Operation


        #endregion Data


        public override string ToString()
        {
            return
                // "Output element:" + Environment.NewLine +
                "  Element index: " + ElementIndex + Environment.NewLine +
                "  Name: " + Name + Environment.NewLine +
                "  Description: " + Description + Environment.NewLine;
        }

    }  // class InputOutputElementDefinition


    /// <summary>Input data element definition for vector functions, approximations, etc.</summary>
    /// $A Igor Feb11;
    public class InputElementDefinition : InputOutputElementDefinition
    {

        #region Construction

        /// <summary>Constructor.</summary>
        /// <param name="elementIndex">Index of the input or output data element specified by the current definision.
        /// If less than 0 is specified then it is considered that element index is not known or defined in the current context.</param>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="description">Element description. Can be an arbvitrary string.</param>
        public InputElementDefinition(int elementIndex, string name = null, string title = null, string description = null) :
            base(elementIndex, name, title, description)
        { }

        /// <summary>Constructor. Element index is unknown, there is no element description.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        public InputElementDefinition(string name)
            : this(-1, name, null, null)
        { }

        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        public InputElementDefinition(string name, string title)
            : this(-1, name, title, null)
        { }

        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="description">Element description. Can be an arbitrary string.</param>
        public InputElementDefinition(string name, string title, string description)
            : this(-1, name, title, description)
        { }

        #endregion Construction

        #region Data

        protected bool _defaultValueDefined = false;

        protected double _defaultValue;

        protected bool _optimizationIndexDefined = false;

        protected int _optimizationIndex;

        protected double _discretizationStep = 0.0;  // Required by IJS

        protected int _numSamplingPoints = 10;

        /// <summary>Flag indicating whether default value is defined for the input parameter 
        /// described by the current eleemnt description.</summary>
        public virtual bool DefaultValueDefined
        { get { return _defaultValueDefined; } set { _defaultValueDefined = value; } }

        /// <summary>Default value for the output data element described by the current definition.</summary>
        public virtual double DefaultValue
        { get { return _defaultValue; } set { _defaultValue = value; } }

        /// <summary>Flag indicating whether optimization parameter index is defined for the input parameter 
        /// described by the current element description.
        /// This index tells which optimization parameter corresponds to the current sampled data input parameter.</summary>
        public virtual bool OptimizationIndexSpecified
        { get { return _optimizationIndexDefined; } set { _optimizationIndexDefined = value; } }

        /// <summary>Optimization parameter index of the data element described by the current definition.
        /// This index tells which optimization parameter corresponds to the current sampled data input parameter.</summary>
        public virtual int OptimizationIndex
        { get { return _optimizationIndex; } set { _optimizationIndex = value; } }

        /// <summary>Discretization step that is used in cases where parameter the input parameter has
        /// discrete values. Discretization starts at MinValue.</summary>
        /// <remarks>This field was required by the Jozef Stefan optimization group.</remarks>
        public virtual double DiscretizationStep
        { get { return _discretizationStep; } set { _discretizationStep = value; } }


        /// <summary>Number of sampling points along the correspoinding input parameter.</summary>
        public virtual int NumSamplingPoints
        { get { return _numSamplingPoints; } set { _numSamplingPoints = value; } }

        #endregion Data


        #region Operation


        /// <summary>Creates and returns a deep copy of the current input data element definition.</summary>
        /// <returns></returns>
        public InputElementDefinition GetCopy()
        {
            InputElementDefinition ret = new InputElementDefinition(this.Name);
            InputElementDefinition.CopyPlain(this, ret);
            return ret;
        }

        /// <summary>Copies data from one input data element definition to another.</summary>
        /// <param name="original">Source object that data is copied from.</param>
        /// <param name="copy">Object to which data is copied.</param>
        /// <remarks>Operation is performed only if both sorce and destination objects are non-null.</remarks>
        public static void CopyPlain(InputElementDefinition original, InputElementDefinition copy)
        {
            if (original != null && copy != null)
            {
                InputOutputElementDefinition.CopyPlain(original, copy);
                copy.DefaultValueDefined = original.DefaultValueDefined;
                copy.DefaultValue = original.DefaultValue;
                copy.OptimizationIndexSpecified = original.OptimizationIndexSpecified;
                copy.OptimizationIndex = original.OptimizationIndex;
                copy.DiscretizationStep = original.DiscretizationStep;
                copy.NumSamplingPoints = original.NumSamplingPoints;
             }
        }


        /// <summary>Copies data from one input data element definition to another.</summary>
        /// <param name="original">Source object that data is copied from.</param>
        /// <param name="copy">Object to which data is copied.</param>
        /// <remarks>If original object is null then destination object is also set to null.
        /// If destination object is nul then it is allocated anew.</remarks>
        public static void Copy(InputElementDefinition original, ref InputElementDefinition copy)
        {
            if (original == null)
                copy = null;
            else
            {
                if (copy == null)
                    copy = new InputElementDefinition(original.Name);
                InputElementDefinition.CopyPlain(original, copy);
            }
        }

        #endregion Operation

        public override string ToString()
        {
            return
                // "Input element:" + Environment.NewLine +
                "  Input element index: " + ElementIndex + Environment.NewLine +
                "  Name: " + Name + Environment.NewLine +
                "  Title: " + Title + Environment.NewLine +
                "  Description: " + Description +
                "  Default value: " + DefaultValue.ToString() + (DefaultValueDefined? "" : " - not defined.") + Environment.NewLine +
                "  Bounds: [" + MinimalValue + ", " + MaximalValue + "]" + (BoundsDefined? "" : " - not defined.") + Environment.NewLine +
                "  Num. Sampling points: " + NumSamplingPoints + Environment.NewLine;
        }


    }  // class class InputElementDefinition


    /// <summary>Input data element definition for vector functions, approximations, etc.</summary>
    /// $A Igor Feb11;
    public class OutputElementDefinition : InputOutputElementDefinition
    {

        #region Construction

        /// <summary>Constructor.</summary>
        /// <param name="elementIndex">Index of the input or output data element specified by the current definision.
        /// If less than 0 is specified then it is considered that element index is not known or defined in the current context.</param>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="description">Element description. Can be an arbvitrary string.</param>
        public OutputElementDefinition(int elementIndex, string name = null, string title = null, string description = null) :
            base(elementIndex, name, title, description)
        { }

        /// <summary>Constructor. Element index is unknown, there is no element description.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        public OutputElementDefinition(string name)
            : this(-1, name, null, null)
        { }

        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        public OutputElementDefinition(string name, string title)
            : this(-1, name, title, null)
        { }

        /// <summary>Constructor. Element index is unknown.</summary>
        /// <param name="name">Element name.
        /// Should comply with conventions for valid variable names in languages C++, C# and Java.</param>
        /// <param name="title">Short descriptive title (like name, but can contain spaces and special characters).</param>
        /// <param name="description">Element description. Can be an arbitrary string.</param>
        public OutputElementDefinition(string name, string title, string description)
            : this(-1, name, title, description)
        { }


        #endregion Construction


        #region Operation


        /// <summary>Creates and returns a deep copy of the current output data element definition.</summary>
        public OutputElementDefinition GetCopy()
        {
            OutputElementDefinition ret = new OutputElementDefinition(this.Name);
            OutputElementDefinition.CopyPlain(this, ret);
            return ret;
        }

        /// <summary>Copies data from one output data element definition to another.</summary>
        /// <param name="original">Source object that data is copied from.</param>
        /// <param name="copy">Object to which data is copied.</param>
        /// <remarks>Operation is performed only if both sorce and destination objects are non-null.</remarks>
        public static void CopyPlain(OutputElementDefinition original, OutputElementDefinition copy)
        {
            if (original != null && copy != null)
            {
                InputOutputElementDefinition.CopyPlain(original, copy);
                //copy.DefaultValueDefined = original.DefaultValueDefined;
                //copy.DefaultValue = original.DefaultValue;
                //copy.OptimizationIndexSpecified = original.OptimizationIndexSpecified;
                //copy.OptimizationIndex = original.OptimizationIndex;
                //copy.DiscretizationStep = original.DiscretizationStep;
            }
        }


        /// <summary>Copies data from one output data element definition to another.</summary>
        /// <param name="original">Source object that data is copied from.</param>
        /// <param name="copy">Object to which data is copied.</param>
        /// <remarks>If original object is null then destination object is also set to null.
        /// If destination object is nul then it is allocated anew.</remarks>
        public static void Copy(OutputElementDefinition original, ref OutputElementDefinition copy)
        {
            if (original == null)
                copy = null;
            else
            {
                if (copy == null)
                    copy = new OutputElementDefinition(original.Name);
                OutputElementDefinition.CopyPlain(original, copy);
            }
        }

        #endregion Operation


        public override string ToString()
        {
            return
                //"Output element:" + Environment.NewLine +
                "  Output Element index: " + ElementIndex + Environment.NewLine +
                "  Name: " + Name + Environment.NewLine +
                "  Title: " + Title + Environment.NewLine +
                "  Description: " + Description + Environment.NewLine +
                "  Bounds: [" + MinimalValue + ", " + MaximalValue + "]" + (BoundsDefined ? "" : " - not defined.") + Environment.NewLine;
        }

    } // class OutputElementDefinition


    /// <summary>Definition of input and output data for vector functions, approximations, etc.
    /// Contains descriptiove information about individual eleemnts of input and output, default values and eventual bounds
    /// of input parameters, etc.</summary>
    /// $A Igor Mar11; 
    public class InputOutputDataDefiniton
    {

        public InputOutputDataDefiniton()
        { }

        
        #region Data

        /// <summary>Default value of the <see cref="Name"/> property of the input and output data definitions.</summary>
        public static string DefaultName = null; // "SampledData";

        /// <summary>Default value of the <see cref="Description"/> property of the input and output data definitions.</summary>
        public static string DefaultDescription = null; // "This contains definition of input and output data elements.";

        protected string _name = DefaultName;

        /// <summary>Name of the current definition of input parameters and output values of a model.
        /// <para>Default value is specified by the static property <see cref="InputOutputDataDefiniton.DefaultName"/>.</para></summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected string _description = DefaultDescription;

        /// <summary>Description of the current definition of input parameters and output values of a model.
        /// <para>Default value is specified by the static property <see cref="InputOutputDataDefiniton.DefaultDescription"/>.</para></summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }



        /// <summary>Gets number of input parameters.</summary>
        public int InputLength
        { get { if (_input == null) return 0; else return _input.Count; } }

        /// <summary>Gets number of output values.</summary>
        public int OutputLength
        { get { if (_output == null) return 0; else return _output.Count; } }

        #region DataOperation

        protected List<InputElementDefinition> _input = new List<InputElementDefinition>();

        public virtual List<InputElementDefinition> InputElementList
        {
            get
            {
                if (_input == null)
                    _input = new List<InputElementDefinition>();
                return _input;
            }
        }

        /// <summary>Returns the specified input element definition.</summary>
        /// <param name="which">Specifies which input element definition is returned.</param>
        public virtual InputElementDefinition GetInputElement(int which)
        {
            if (which < 0 || which >= InputLength)
                throw new IndexOutOfRangeException("Index of input parameter definition out of range: " + which + ".");
            return _input[which];
        }

        /// <summary>Sets the input element definition with the specified index to the specified object.</summary>
        /// <param name="which">Index of input element definition.</param>
        /// <param name="element">Imput element definition object to which the specified input element definition is set.</param>
        /// <exception cref="IndexOutOfRangeException">When index is out of range.</exception>
        public virtual void SetInputElement(int which, InputElementDefinition element)
        {
            if (which < 0 || which >= InputLength)
                throw new IndexOutOfRangeException("Index of input parameter definition out of range: " + which + ".");
            _input[which] = element;
        }

        /// <summary>Adds the specified input element definition to the list of input element definitions.</summary>
        /// <param name="element">Input element to be added.</param>
        /// <param name="assignIndex">If true then element index is automatically assigned to the element.</param>
        public virtual void AddInputElement(InputElementDefinition element, bool assignIndex = false)
        {
            if (element == null)
                throw new ArgumentNullException("Input element definition to be added is not specified (null reference).");
            if (assignIndex)
                element.ElementIndex = _input.Count;
            _input.Add(element);
        }

        /// <summary>Gets index of the input element that has the specified name, or -1
        /// if there is no such input element.</summary>
        /// <param name="inputName">Name of the input element whose index is to be returned.</param>
        /// <remarks>This method is slow when the number of elements is large, becase it iterates through all elements
        /// before the matching element is found. If necessary, indexing can be introduce later in order to achieve 
        /// improvement.</remarks>
        public virtual int GetInputIndex(string inputName)
        {
            if (string.IsNullOrEmpty(inputName))
                return -1;
            int num = _input.Count;
            for (int i = 0; i < num; ++i)
            {
                if (_input[i].Name == inputName)
                    return i;
            }
            return -1;
        }


        /// <summary>Gets index of the input element that has the specified alternative name, or -1
        /// if there is no such input element.</summary>
        /// <param name="inputNameAlt">Slternative name of the input element whose index is to be returned.</param>
        /// <remarks>This method is slow when the number of elements is large, becase it iterates through all elements
        /// before the matching element is found. If necessary, indexing can be introduce later in order to achieve 
        /// improvement.</remarks>
        public virtual int GetInputIndexByNameAlt(string inputNameAlt)
        {
            if (string.IsNullOrEmpty(inputNameAlt))
                return -1;
            int num = _input.Count;
            for (int i = 0; i < num; ++i)
            {
                if (_input[i].NameAlt == inputNameAlt)
                    return i;
            }
            return -1;
        }


        /// <summary>Gets index of the input element that has the specified name or alternative name, or -1
        /// if there is no such input element.</summary>
        /// <param name="inputNameOrNameAlt">Slternative name of the input element whose index is to be returned.</param>
        /// <remarks>This method is slow when the number of elements is large, becase it iterates through all elements
        /// before the matching element is found. If necessary, indexing can be introduced later in order to achieve 
        /// improvement.</remarks>
        public virtual int GetInputIndexByNameOrNameAlt(string inputNameOrNameAlt)
        {
            if (string.IsNullOrEmpty(inputNameOrNameAlt))
                return -1;
            int num = _input.Count;
            for (int i = 0; i < num; ++i)
            {
                if (_input[i].Name == inputNameOrNameAlt ||
                    _input[i].NameAlt == inputNameOrNameAlt)
                    return i;
            }
            return -1;
        }


        protected List<OutputElementDefinition> _output = new List<OutputElementDefinition>();

        public virtual List<OutputElementDefinition> OutputElementList
        {
            get
            {
                if (_output == null)
                    _output = new List<OutputElementDefinition>();
                return _output;
            }
        }

        public virtual OutputElementDefinition GetOutputElement(int which)
        {
            if (which < 0 || which >= OutputLength)
                throw new IndexOutOfRangeException("Index of iutput value definition out of range: " + which + ".");
            return _output[which];
        }

        /// <summary>Sets the output element definition with the specified index to the specified object.</summary>
        /// <param name="which">Index of output element definition.</param>
        /// <param name="element">Output element definition object to which the specified output element definition is set.</param>
        /// <exception cref="IndexOutOfRangeException">When index is out of range.</exception>
        public virtual void SetOutputElement(int which, OutputElementDefinition element)
        {
            if (which < 0 || which >= OutputLength)
                throw new IndexOutOfRangeException("Index of output value definition out of range: " + which + ".");
            _output[which] = element;
        }

        /// <summary>Adds the specified output element definition to the list of output element definitionss.</summary>
        /// <param name="element">Output element to be added.</param>
        /// <param name="assignIndex">If true then element index is automatically assigned to the element.</param>
        public virtual void AddOutputElement(OutputElementDefinition element, bool assignIndex = false)
        {
            if (element == null)
                throw new ArgumentNullException("Output element definition to be added is not specified (null reference).");
            if (assignIndex)
                element.ElementIndex = _output.Count;
            _output.Add(element);
        }

        /// <summary>Gets index of the output element that has the specified name, or -1
        /// if there is no such output element.</summary>
        /// <param name="outputName">Name of the output element whose index is to be returned.</param>
        /// <remarks>This method is slow when the number of elements is large, becase it iterates through all elements
        /// before the matching element is found. If necessary, indexing can be introduce later in order to achieve 
        /// improvement.</remarks>
        public virtual int GetOutputIndex(string outputName)
        {
            if (string.IsNullOrEmpty(outputName))
                return -1;
            int num = _output.Count;
            for (int i = 0; i < num; ++i)
            {
                if (_output[i].Name == outputName)
                    return i;
            }
            return -1;
        }

        /// <summary>Gets index of the output element that has the specified alternative name, or -1
        /// if there is no such output element.</summary>
        /// <param name="outputNameAlt">Alternative name of the output element whose index is to be returned.</param>
        /// <remarks>This method is slow when the number of elements is large, becase it iterates through all elements
        /// before the matching element is found. If necessary, indexing can be introduce later in order to achieve 
        /// improvement.</remarks>
        public virtual int GetOutputIndexByNameAlt(string outputNameAlt)
        {
            if (string.IsNullOrEmpty(outputNameAlt))
                return -1;
            int num = _output.Count;
            for (int i = 0; i < num; ++i)
            {
                if (_output[i].NameAlt == outputNameAlt)
                    return i;
            }
            return -1;
        }

        /// <summary>Gets index of the output element that has the specified name or alternative name, or -1
        /// if there is no such output element.</summary>
        /// <param name="outputNameOrNameAlt">Name or alternative name of the output element whose index is to be returned.</param>
        /// <remarks>This method is slow when the number of elements is large, becase it iterates through all elements
        /// before the matching element is found. If necessary, indexing can be introduced later in order to achieve 
        /// improvement.</remarks>
        public virtual int GetOutputIndexByNameOrNameAlt(string outputNameOrNameAlt)
        {
            if (string.IsNullOrEmpty(outputNameOrNameAlt))
                return -1;
            int num = _output.Count;
            for (int i = 0; i < num; ++i)
            {
                if (_output[i].Name == outputNameOrNameAlt ||
                    _output[i].NameAlt == outputNameOrNameAlt)
                    return i;
            }
            return -1;
        }

        /// <summary>Copies input parameters from the specified vector to the current data definition's 
        /// default input values.
        /// <para>The default value flag is set on every element.</para></summary>
        /// <param name="inputParameters">Vector from which default input values are copied.</param>
        /// <exception cref="ArgumentException">If <paramref name="inputParameters"/> is null or of incorrect dimension.</exception>
        public virtual void CopyDefaultInputFrom(IVector inputParameters)
        {
            if (inputParameters == null)
                throw new ArgumentException("Vector of default input parameters is not specified.");
            if (inputParameters.Length != this.InputLength)
                throw new ArgumentException("Vector of default input parameters is of dimension " + inputParameters.Length + 
                    ", should be of dimension " + this.InputLength);
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition element = GetInputElement(i);
                element.DefaultValue = inputParameters[i];
                element.DefaultValueDefined = true;
            }
        }

        /// <summary>Stores default values of input parameters from the current data definition in
        /// the specified vector.
        /// <para>Vector is resized if necessary.</para></summary>
        /// <param name="inputParameters">Vector where default values of input parameters are stored.</param>
        /// <exception cref="InvalidOperationException">When default values are not defined for all input parameters.</exception>
        public virtual void CopyDefaultInputTo(ref IVector inputParameters)
        {
            VectorBase.Resize(ref inputParameters, this.InputLength);
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition element = GetInputElement(i);
                if (!element.DefaultValueDefined)
                    throw new InvalidOperationException("Default value is not defined for input parameter " + i + ".");
                inputParameters[i] = element.DefaultValue;
            }
        }

        /// <summary>Extracts input bounds as defined on the current data definition object,
        /// and stores them in the specified bounding box.
        /// <para>If some bounds are not defined then they are also not defined on the bounding 
        /// box where bounds are stored.</para>
        /// <para>After the call, bounding box will have bounds defined exacttly as they are set
        /// in the current input data definition.</para></summary>
        /// <param name="bounds">Bounding box on which the extracted bounds are set.</param>
        public virtual void GetInputBounds(ref IBoundingBox bounds)
        {
            int dimension = this.InputLength;
            if (bounds == null)
                bounds = new BoundingBox(InputLength);
            else if (bounds.Dimension != dimension)
                bounds = new BoundingBox(dimension);
            bounds.Reset();
            for (int i = 0; i < dimension; ++i)
            {
                InputElementDefinition element = GetInputElement(i);
                if (element.BoundsDefined)
                    bounds.Update(i, element.MinimalValue, element.MaximalValue);
            }
        }

        /// <summary>Extracts output bounds as defined on the current data definition object,
        /// and stores them in the specified bounding box.
        /// <para>If some bounds are not defined then they are also not defined on the bounding 
        /// box where bounds are stored.</para>
        /// <para>After the call, bounding box will have bounds defined exacttly as they are set
        /// in the current output data definition.</para></summary>
        /// <param name="bounds">Bounding box on which the extracted bounds are set.</param>
        public virtual void GetOutputBounds(ref IBoundingBox bounds)
        {
            int dimension = this.OutputLength;
            if (bounds == null)
                bounds = new BoundingBox(OutputLength);
            else if (bounds.Dimension != dimension)
                bounds = new BoundingBox(dimension);
            bounds.Reset();
            for (int i = 0; i < dimension; ++i)
            {
                OutputElementDefinition element = GetOutputElement(i);
                if (element.BoundsDefined)
                    bounds.Update(i, element.MinimalValue, element.MaximalValue);
            }
        }



        /// <summary>Returns a value indicating whether any input element has Name property defined.</summary>
        public virtual bool IsAnyInputNameDefined()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (!string.IsNullOrEmpty(def.Name))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has Name property defined.</summary>
        public virtual bool IsAnyOutputNameDefined()
        {
            for (int i = 0; i < OutputLength; ++i)
            {
                OutputElementDefinition def = GetOutputElement(i);
                if (def != null)
                {
                    if (!string.IsNullOrEmpty(def.Name))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has Name property defined.</summary>
        public virtual bool IsAnyNameDefined()
        {
            return (IsAnyInputNameDefined() || IsAnyOutputNameDefined());
        }

        
        /// <summary>Returns a value indicating whether any input element has Title property defined.</summary>
        public virtual bool IsAnyInputTitleDefined()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (!string.IsNullOrEmpty(def.Title))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has Title property defined.</summary>
        public virtual bool IsAnyOutputTitleDefined()
        {
            for (int i = 0; i < OutputLength; ++i)
            {
                OutputElementDefinition def = GetOutputElement(i);
                if (def != null)
                {
                    if (!string.IsNullOrEmpty(def.Title))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has Title property defined.</summary>
        public virtual bool IsAnyTitleDefined()
        {
            return (IsAnyInputTitleDefined() || IsAnyOutputTitleDefined());
        }



        /// <summary>Returns a value indicating whether any input element has Description property defined.</summary>
        public virtual bool IsAnyInputDescriptionDefined()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (!string.IsNullOrEmpty(def.Description))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has Description property defined.</summary>
        public virtual bool IsAnyOutputDescriptionDefined()
        {
            for (int i = 0; i < OutputLength; ++i)
            {
                OutputElementDefinition def = GetOutputElement(i);
                if (def != null)
                {
                    if (!string.IsNullOrEmpty(def.Description))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has Description property defined.</summary>
        public virtual bool IsAnyDescriptionDefined()
        {
            return (IsAnyInputDescriptionDefined() || IsAnyOutputDescriptionDefined());
        }



        /// <summary>Returns a value indicating whether any input element has DiscretizationStep property defined.</summary>
        public virtual bool IsAnyInputDiscretizationStepDefined()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (def.DiscretizationStep>0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has DiscretizationStep property defined.</summary>
        public virtual bool IsAnyOutputDiscretizationStepDefined()
        {
            return false;   // Output elements can not have discretization step defined.
            //for (int i = 0; i < OutputLength; ++i)
            //{
            //    OutputElementDefinition def = GetOutputElement(i);
            //    if (def != null)
            //    {
            //        if (def.DiscretizationStep > 0)
            //            return true;
            //    }
            //}
            //return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has DiscretizationStep property defined.</summary>
        public virtual bool IsAnyDiscretizationStepDefined()
        {
            return (IsAnyInputDiscretizationStepDefined() || IsAnyOutputDiscretizationStepDefined());
        }



        /// <summary>Returns a value indicating whether any input element has NumSamplingPoints property defined.</summary>
        public virtual bool IsAnyInputNumSamplingPointsDefined()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (def.NumSamplingPoints > 0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has NumSamplingPoints property defined.</summary>
        public virtual bool IsAnyOutputNumSamplingPointsDefined()
        {
            return false;   // Output elements can not have NumSamplingPoints step defined.
            //for (int i = 0; i < OutputLength; ++i)
            //{
            //    OutputElementDefinition def = GetOutputElement(i);
            //    if (def != null)
            //    {
            //        if (def.NumSamplingPoints > 0)
            //            return true;
            //    }
            //}
            //return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has NumSamplingPoints property defined.</summary>
        public virtual bool IsAnyNumSamplingPointsDefined()
        {
            return (IsAnyInputNumSamplingPointsDefined() || IsAnyOutputNumSamplingPointsDefined());
        }



        // Name Name Name 

        /// <summary>Returns a value indicating whether any input element has bounds defined.</summary>
        public virtual bool IsAnyInputBoundDefined()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (def.BoundsDefined)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has bounds defined.</summary>
        public virtual bool IsAnyOutputBoundDefined()
        {
            for (int i = 0; i < OutputLength; ++i)
            {
                OutputElementDefinition def = GetOutputElement(i);
                if (def != null)
                {
                    if (def.BoundsDefined)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has bounds defined.</summary>
        public virtual bool IsAnyBoundDefined()
        {
            return (IsAnyInputBoundDefined() || IsAnyOutputBoundDefined());
        }



        /// <summary>Returns a value indicating whether any input element has scaling length defined.</summary>
        public virtual bool IsAnyInputScalingLengthDefined()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (def.ScalingLengthDefined)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has scaling length defined.</summary>
        public virtual bool IsAnyOutputScalingLengthDefined()
        {
            for (int i = 0; i < OutputLength; ++i)
            {
                OutputElementDefinition def = GetOutputElement(i);
                if (def != null)
                {
                    if (def.ScalingLengthDefined)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has scaling length defined.</summary>
        public virtual bool IsAnyScalingLengthDefined()
        {
            return (IsAnyInputScalingLengthDefined() || IsAnyOutputScalingLengthDefined());
        }


        /// <summary>Returns a value indicating whether any input element has target value defined.</summary>
        public virtual bool IsAnyInputTargetValueDefined()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (def.TargetValueDefined)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has target value defined.</summary>
        public virtual bool IsAnyOutputTargetValueDefined()
        {
            for (int i = 0; i < OutputLength; ++i)
            {
                OutputElementDefinition def = GetOutputElement(i);
                if (def != null)
                {
                    if (def.TargetValueDefined)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has target value defined.</summary>
        public virtual bool IsAnyTargetValueDefined()
        {
            return (IsAnyInputTargetValueDefined() || IsAnyOutputTargetValueDefined());
        }



        /// <summary>Returns a value indicating whether any input element has default value defined.</summary>
        public virtual bool IsAnyInputDefaultValueDefined()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (def.DefaultValueDefined)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has default value defined.</summary>
        public virtual bool IsAnyOutputDefaultValueDefined()
        {
            return false;  // output elements can not have default values.
            //for (int i = 0; i < OutputLength; ++i)
            //{
            //    OutputElementDefinition def = GetOutputElement(i);
            //    if (def != null)
            //    {
            //        if (def.DefaultValueDefined)
            //            return true;
            //    }
            //}
            //return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has default value defined.</summary>
        public virtual bool IsAnyDefaultValueDefined()
        {
            return (IsAnyInputDefaultValueDefined() || IsAnyOutputDefaultValueDefined());
        }



        /// <summary>Returns a value indicating whether any input element has element index specified.</summary>
        public virtual bool IsAnyInputElementIndexSpecified()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (def.ElementIndexSpecified)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has element index specified.</summary>
        public virtual bool IsAnyOutputElementIndexSpecified()
        {
            for (int i = 0; i < OutputLength; ++i)
            {
                OutputElementDefinition def = GetOutputElement(i);
                if (def != null)
                {
                    if (def.ElementIndexSpecified)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has element index specified.</summary>
        public virtual bool IsAnyElementIndexSpecified()
        {
            return (IsAnyInputElementIndexSpecified() || IsAnyOutputElementIndexSpecified());
        }


        /// <summary>Returns a value indicating whether any input element has element index specified.</summary>
        public virtual bool IsAnyInputOptimizationIndexSpecified()
        {
            for (int i = 0; i < InputLength; ++i)
            {
                InputElementDefinition def = GetInputElement(i);
                if (def != null)
                {
                    if (def.OptimizationIndexSpecified)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Returns a value indicating whether any output element has element index specified.</summary>
        public virtual bool IsAnyOutputOptimizationIndexSpecified()
        {
            return false;   // output elements can not have optimization index specified.
            //for (int i = 0; i < OutputLength; ++i)
            //{
            //    OutputElementDefinition def = GetOutputElement(i);
            //    if (def != null)
            //    {
            //        if (def.OptimizationIndexSpecified)
            //            return true;
            //    }
            //}
            //return false;
        }

        /// <summary>Returns a value indicating whether any element (either input or output) has element index specified.</summary>
        public virtual bool IsAnyOptimizationIndexSpecified()
        {
            return (IsAnyInputOptimizationIndexSpecified() || IsAnyOutputOptimizationIndexSpecified());
        }



        #endregion DataOperation

        #endregion Data


        #region StaticMethods

        private static string _defaultInputElementNameBase = "InputParameter";

        /// <summary>Default name base for input data elements.
        /// <para>When set, the value must be a legal variable name in languages like C#, which is checked by <see cref="UtilStr.IsVariableName(string)"/> function.</para>
        /// <para>Initially set to "InputParameter" but can be changed programatically.</para></summary>
        public static string DefaultInputElementNameBase
        {
            get { return _defaultInputElementNameBase; }
            set
            {
                if (value != _defaultInputElementNameBase)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("Specified default base of input data element name \""+ value + "\" is not a legal variable name.");
                    _defaultInputElementNameBase = value;
                }
            }
        }

        private static string _defaultOutputElementNameBase = "OutputValue";

        /// <summary>Default name base for output data elements.
        /// <para>When set, the value must be a legal variable name in languages like C#, which is checked by <see cref="UtilStr.IsVariableName(string)"/> function.</para>
        /// <para>Initially set to "OutputValue" but can be changed programatically.</para></summary>
        public static string DefaultOutputElementNameBase
        {
            get { return _defaultOutputElementNameBase; }
            set
            {
                if (value != _defaultOutputElementNameBase)
                {
                    if (!UtilStr.IsVariableName(value))
                        throw new ArgumentException("Specified default base of output data element name \""+ value + "\" is not a legal variable name.");
                    _defaultOutputElementNameBase = value;
                }
            }
        }


        private static int _defaultElementNameNumDigits = 2;

        /// <summary>Default number of digits in generated input or output data element names (this refers to digits appended to name base).
        /// <para>When set, it must be greater than 0.</para>
        /// <para>Initially set to 2 but can be changed programatically.</para></summary>
        public static int DefaultElementNameNumDigits
        {
            get { return _defaultElementNameNumDigits; }
            set
            {
                if (value != _defaultElementNameNumDigits)
                {
                    if (value < 1)
                        throw new ArgumentException("Specified default number of digits in element data name " + value + " is not greater than 0.");
                    _defaultElementNameNumDigits = value;
                }
            }
        }

        private static int _defaultElementNameStatingIndex = 0;

        /// <summary>Default starting index in generated input or output data element names (this refers to digits appended to name base).
        /// <para>When set, it must be greater or equal than 0.</para>
        /// <para>Initially set to 0 but can be changed programatically.</para></summary>
        public static int DefaultElementNameStartingIndex
        {
            get { return _defaultElementNameStatingIndex; }
            set
            {
                if (value != _defaultElementNameStatingIndex)
                {
                    if (value < 0)
                        throw new ArgumentException("Specified default starting index in element data name " + value + " is not greater or equal to 0.");
                    _defaultElementNameStatingIndex = value;
                }
            }
        }



        /// <summary>Retuns a base string, number of digits and number starting index for generation of data definition input element names, which it tries to infer from the specified definition data.
        /// <para>If inference was impossible then values are returned.</para>
        /// <para>The returned data is helpful in generation of names for new data elements.</para></summary>
        /// <param name="nameBase">Variable where name base is stored. If it can not be inferred from <paramref name="def"/> then default is stored 
        /// (property <see cref="InputOutputDataDefiniton.DefaultInputElementNameBase"/>).</param>
        /// <param name="numDigits">Variable where number of digits in element names is stored. If it can not be inferred from <paramref name="def"/> then default is stored 
        /// (property <see cref="InputOutputDataDefiniton.DefaultElementNameNumDigits"/>).</param>
        /// <param name="startingIndex">Variable where number of digits in element names is stored. If it can not be inferred from <paramref name="def"/> then default is stored 
        /// (property <see cref="InputOutputDataDefiniton.DefaultElementNameStartingIndex"/>).</param>
        /// <param name="def">Definition data from which name is inferred.
        /// <para>Can be null (in which case inference is skipped and default values returned).</para></param>
        /// <param name="checkLegalVAriableName">Indicates whether inferrence is only performed using legal variable names of existing elements, as defined by <see cref="UtilStr.IsVariableName(string)"/>  method.</param>
        public static void GetInputElementNameParameters(out string nameBase, out int numDigits, out int startingIndex, InputOutputDataDefiniton def = null, bool checkLegalVAriableName = true)
        {
            GetElementNameParameters(false /* outputElements */, out nameBase, out numDigits, out startingIndex, def, checkLegalVAriableName);
        }

        /// <summary>Retuns a base string, number of digits and number starting index for generation of data definition input element names, which it tries to infer from the specified definition data.
        /// <para>If inference was impossible then values are returned.</para>
        /// <para>The returned data is helpful in generation of names for new data elements.</para></summary>
        /// <param name="nameBase">Variable where name base is stored. If it can not be inferred from <paramref name="def"/> then default is stored 
        /// (property <see cref="InputOutputDataDefiniton.DefaultInputElementNameBase"/>).</param>
        /// <param name="numDigits">Variable where number of digits in element names is stored. If it can not be inferred from <paramref name="def"/> then default is stored 
        /// (property <see cref="InputOutputDataDefiniton.DefaultElementNameNumDigits"/>).</param>
        /// <param name="startingIndex">Variable where number of digits in element names is stored. If it can not be inferred from <paramref name="def"/> then default is stored 
        /// (property <see cref="InputOutputDataDefiniton.DefaultElementNameStartingIndex"/>).</param>
        /// <param name="def">Definition data from which name is inferred.
        /// <para>Can be null (in which case inference is skipped and default values returned).</para></param>
        /// <param name="checkLegalVAriableName">Indicates whether inferrence is only performed using legal variable names of existing elements, as defined by <see cref="UtilStr.IsVariableName(string)"/>  method.</param>
        public static void GetOutputElementNameParameters(out string nameBase, out int numDigits, out int startingIndex, InputOutputDataDefiniton def = null, bool checkLegalVAriableName = true)
        {
            GetElementNameParameters(true /* outputElements */, out nameBase, out numDigits, out startingIndex, def, checkLegalVAriableName);
        }


        /// <summary>Retuns a base string, number of digits and number starting index for generation of data definition input or output element names, which it tries to infer from the specified definition data.
        /// <para>If inference was impossible then values are returned.</para>
        /// <para>The returned data is helpful in generation of names for new data elements.</para></summary>
        /// <param name="outputElements">Indicates whether name generation parameters are obtained for output elements (if false then input elements are considered instead).</param>
        /// <param name="nameBase">Variable where name base is stored. If it can not be inferred from <paramref name="def"/> then default is stored 
        /// (property <see cref="InputOutputDataDefiniton.DefaultInputElementNameBase"/>).</param>
        /// <param name="numDigits">Variable where number of digits in element names is stored. If it can not be inferred from <paramref name="def"/> then default is stored 
        /// (property <see cref="InputOutputDataDefiniton.DefaultElementNameNumDigits"/>).</param>
        /// <param name="startingIndex">Variable where number of digits in element names is stored. If it can not be inferred from <paramref name="def"/> then default is stored 
        /// (property <see cref="InputOutputDataDefiniton.DefaultElementNameStartingIndex"/>).</param>
        /// <param name="def">Definition data from which name is inferred.
        /// <para>Can be null (in which case inference is skipped and default values returned).</para></param>
        /// <param name="checkLegalVAriableName">Indicates whether inferrence is only performed using legal variable names of existing elements, as defined by <see cref="UtilStr.IsVariableName(string)"/>  method.</param>
        protected static void GetElementNameParameters(bool outputElements, out string nameBase, out int numDigits, out int startingIndex, InputOutputDataDefiniton def = null, bool checkLegalVAriableName = true)
        {
            if (outputElements)
                nameBase = DefaultOutputElementNameBase;
            else
                nameBase = DefaultInputElementNameBase;
            numDigits = DefaultElementNameNumDigits;
            startingIndex = DefaultElementNameStartingIndex;
            bool baseEstablished = false;
            bool numDigitsEstablished = false;
            bool startingIndexEstablished = false;
            if (def != null)
            {
                int numElements = 0;
                if (outputElements)
                    numElements = def.OutputLength;
                else
                    numElements = def.InputLength;
                for (int whichElement = 0; whichElement < numElements && !(baseEstablished && numDigitsEstablished && startingIndexEstablished); ++whichElement)
                {
                    InputOutputElementDefinition elDef = null;
                    if (outputElements)
                        def.GetOutputElement(whichElement);
                    else
                        def.GetInputElement(whichElement);
                    if (elDef != null)
                    {
                        string name = elDef.Name;
                        if (UtilStr.IsVariableName(name) || (!checkLegalVAriableName && !string.IsNullOrEmpty(name)) ) // !string.IsNullOrEmpty(name))
                        {
                            int len = name.Length;
                            int numDigitsCurrent = 0;
                            for (int whichDigit = 0; whichDigit < len; ++ whichDigit)
                            {
                                char ch = name[len - 1 - whichDigit];
                                if (char.IsDigit(ch))
                                {
                                    ++numDigitsCurrent;
                                }
                            }
                            if (numDigitsCurrent < len || !checkLegalVAriableName)
                            {
                                string nameBaseCurrent = name.Substring(0, len - numDigitsCurrent);
                                if (UtilStr.IsVariableName(nameBaseCurrent) || (!checkLegalVAriableName) )
                                {
                                    if (!baseEstablished)
                                    {
                                        if (UtilStr.IsVariableName(nameBaseCurrent) || (!checkLegalVAriableName && !string.IsNullOrEmpty(nameBaseCurrent)) )
                                        {
                                            nameBase = nameBaseCurrent;
                                            baseEstablished = true;
                                        }
                                    }
                                    if (numDigitsCurrent > 0 && !(numDigitsEstablished && startingIndexEstablished))
                                    {
                                        numDigits = numDigitsCurrent;
                                        numDigitsEstablished = true;
                                        string digitString = name.Substring(len - numDigitsCurrent, numDigitsCurrent);
                                        int indexValue = 0;
                                        bool parsed = int.TryParse(digitString, out indexValue);
                                        if (parsed)
                                        {
                                            startingIndex = indexValue - whichElement;
                                            startingIndexEstablished = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>Generates and returns name of the input data element with the specified index. 
        /// <para>Existing data definition may be eventually used to infer generation parameters (base name, number of digits appended to base name as index, and starting index).</para></summary>
        /// <param name="elementIndex">Index of the element wose name is generated.</param>
        /// <param name="def">Model definition data used to infer parameters of name generation. 
        /// <para>If specified then base name, number of appended digita and starting index are inferred from this data.</para>
        /// <para>Otherwise, default values are taken (i.e. <see cref="DefaultInputElementNameBase"/>, <see cref="DefaultElementNameNumDigits"/>, <see cref="DefaultElementNameStartingIndex"/>).</para> </param>
        /// <param name="checkLegalVAriableName">If true (which is default) then parameters are only infered from those names in the existing data definition (if specified)
        /// that satisfy variable name rules in programming languages such as C++, Java or C#.</param>
        /// <returns></returns>
        public static string CreateInputElementName(int elementIndex, InputOutputDataDefiniton def = null, bool checkLegalVAriableName = true)
        {
            if (elementIndex < 0)
                throw new ArgumentException("Input data element index must be greater or equal to 0. Specified: " + elementIndex + ".");
            string nameBase;
            int numDigits;
            int startingIndex;
            GetInputElementNameParameters(out nameBase, out numDigits, out startingIndex, def, checkLegalVAriableName);
            return nameBase + (elementIndex + startingIndex).ToString(new string('0',numDigits));  // new string(...) creates format specification - string with as many '0'-s as digits appended
        }

        /// <summary>Generates and returns name of the output data element with the specified index. 
        /// <para>Existing data definition may be eventually used to infer generation parameters (base name, number of digits appended to base name as index, and starting index).</para></summary>
        /// <param name="elementIndex">Index of the element wose name is generated.</param>
        /// <param name="def">Model definition data used to infer parameters of name generation. 
        /// <para>If specified then base name, number of appended digita and starting index are inferred from this data.</para>
        /// <para>Otherwise, default values are taken (i.e. <see cref="DefaultOutputElementNameBase"/>, <see cref="DefaultElementNameNumDigits"/>, <see cref="DefaultElementNameStartingIndex"/>).</para> </param>
        /// <param name="checkLegalVAriableName">If true (which is default) then parameters are only infered from those names in the existing data definition (if specified)
        /// that satisfy variable name rules in programming languages such as C++, Java or C#.</param>
        /// <returns></returns>
        public static string CreateOutputElementName(int elementIndex, InputOutputDataDefiniton def = null, bool checkLegalVAriableName = true)
        {
            if (elementIndex < 0)
                throw new ArgumentException("Output data element index must be greater or equal to 0. Specified: " + elementIndex + ".");
            string nameBase;
            int numDigits;
            int startingIndex;
            GetOutputElementNameParameters(out nameBase, out numDigits, out startingIndex, def, checkLegalVAriableName);
            return nameBase + (elementIndex + startingIndex).ToString(new string('0',numDigits));  // new string(...) creates format specification - string with as many '0'-s as digits appended
        }


        /// <summary>Creates and returns a data definition with fields set to default values.</summary>
        /// <param name="inputLength">Number of input elements.</param>
        /// <param name="outputLength">Number of output elements.</param>
        public static InputOutputDataDefiniton CreateDefault(int inputLength, int outputLength)
        {
            InputOutputDataDefiniton ret = new InputOutputDataDefiniton();
            for (int i = 0; i < inputLength; ++i)
            {
                InputElementDefinition inputElement = new InputElementDefinition(i,
                    "param_" + i.ToString(), "Input parameter " + i + ".",
                    "This is description for input parameter No. " + i + ".");
                ret.AddInputElement(inputElement);
            }
            for (int i = 0; i < outputLength; ++i)
            {
                OutputElementDefinition outputElement = new OutputElementDefinition(i,
                    "val_" + i.ToString(), "Output value " + i + ".",
                    "This is description for output value No. " + i + ".");
                ret.AddOutputElement(outputElement);
            }
            return ret;
        }


        /// <summary>Creates and returns definition a new definition data with specified dimensions of input and otput spaces
        /// (domains and codomains in case of functions) and eventually assigns names and descriptions to input and
        /// output elements.
        /// <para>Default values for parameters are such that meaningful data is generated by default (apart from
        /// <paramref name="assignNames"/> and <paramref name="assignDescriptions"/>).</para></summary>
        /// <param name="inputLength">Number of input parameters.</param>
        /// <param name="outputLength">Number of outpput values.</param>
        /// <param name="assignNames">Whether names are assigned to elements. Default is false.</param>
        /// <param name="assignDescriptions">Whether descriptions are assigned (default is false).</param>
        /// <param name="inputNameBase">Base for input parameters names. If null (which is default) or empty string then <see cref="DefaultInputElementNameBase"/> is taken.</param>
        /// <param name="outputNameBase">Base for output values names. If null (which is default) or empty string then <see cref="DefaultOutputElementNameBase"/> is taken.</param>
        /// <param name="numNameDigits">Number of digits incorporated into names. If less than 1 (default is -1) then <see cref="DefaultElementNameNumDigits"/> is taken.</param>
        /// <param name="startIndex">Number where digits appended to variable names start. If less than 0 (default is -1) then <see cref="DefaultElementNameStartingIndex"/> is taken.
        /// <para>If descriptions are also generated then this also applies to them.</para></param>
        /// <param name="digitSeparator">Separator that is printed between element's name base and digit (when specified).</param>
        /// <param name="inputDescriptionBase">Base for input element description, to which sequential number is added.</param>
        /// <param name="outputDescriptionBase">Base for output element description, to which sequential number is added.</param>
        /// <param name="digitSeparatorDescriptions">Separator that separates (when specified) description base and number.</param>
        /// <param name="descriptionsEnd">Final part that comes after the sequential number, with which description is 
        /// ended (when specified)</param>
        public static InputOutputDataDefiniton Create(int inputLength, int outputLength,
            bool assignNames = false, bool assignDescriptions = false, string inputNameBase = null , 
            string outputNameBase = null, int numNameDigits = -1, int startIndex = -1, string digitSeparator = "_", 
            string inputDescriptionBase = "Input parameter No. ",  string outputDescriptionBase = "Output value No. ", 
            string digitSeparatorDescriptions = "", string descriptionsEnd = ".")
        {
            if (string.IsNullOrEmpty(inputNameBase))
                inputNameBase = DefaultInputElementNameBase;
            if (string.IsNullOrEmpty(outputNameBase))
                outputNameBase = DefaultOutputElementNameBase;
            if (numNameDigits < 1)
                numNameDigits = DefaultElementNameNumDigits;
            if (startIndex < 0)
                startIndex = DefaultElementNameStartingIndex;
            InputOutputDataDefiniton ret = new InputOutputDataDefiniton();
            string digitFormatString = new string('0', numNameDigits);
            for (int i = 0; i < inputLength; ++i)
            {
                InputElementDefinition inputElement = new InputElementDefinition(i, null, null);
                if (assignNames)
                {
                    inputElement.Name = inputNameBase + digitSeparator + (startIndex + i).ToString(digitFormatString);
                    inputElement.Title = inputElement.Name;
                }
                if (assignDescriptions)
                {
                    inputElement.Description = inputDescriptionBase + digitSeparatorDescriptions +
                        (startIndex + i).ToString() + descriptionsEnd;
                }
                ret.AddInputElement(inputElement, true /* assign element index */);
            }
            for (int i = 0; i < outputLength; ++i)
            {

                OutputElementDefinition outputElement = new OutputElementDefinition("");
                if (assignNames)
                {
                    outputElement.Name = outputNameBase + digitSeparator + (startIndex + i).ToString(digitFormatString);
                    outputElement.Title = outputElement.Name;
                }
                if (assignDescriptions)
                {
                    outputElement.Description = outputDescriptionBase + digitSeparatorDescriptions +
                        (startIndex + i).ToString() + descriptionsEnd;
                }
                ret.AddOutputElement(outputElement, true /* assign element index */);
            }
            return ret;
        }



        /// <summary>Saves input/output data definition to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="dataDef">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file where training data is saved.</param>
        public static void SaveJson(InputOutputDataDefiniton dataDef, string filePath)
        {
            InputOutputDataDefinitonDto dtoOriginal = new InputOutputDataDefinitonDto();
            dtoOriginal.CopyFrom(dataDef);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<InputOutputDataDefinitonDto>(dtoOriginal, filePath);
        }

        /// <summary>Restores input/output data definition from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which training data is restored.</param>
        /// <param name="dataDefRestored">Narual networ approximator's definition data that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref InputOutputDataDefiniton dataDefRestored)
        {
            ISerializer serializer = new SerializerJson();
            InputOutputDataDefinitonDto dtoRestored = serializer.DeserializeFile<InputOutputDataDefinitonDto>(filePath);
            dataDefRestored = new InputOutputDataDefiniton();
            dtoRestored.CopyTo(ref dataDefRestored);
        }

        /// <summary>Suplements the specified data definition object with information that is extracted
        /// from the training data set. Some data is updated independently of the training data set, and 
        /// this data is updated even if the training data set is not specified.
        /// If bounds on input parameters and output values are not defined then bounds are set to bounds within
        /// which training data fits.
        /// If default values and target values are not specified then these values are set to centers of intervals.
        /// If names, titles and descriptions are not specified (null or empty string) then default strings are provided.
        /// If optimization parameter indices are not specified for input parameters then these indices are taken the
        /// same as elementindices.
        /// For fields that have the corresponding "...Defined" flag, data is recalculated only if that flag is set to false.
        /// In order to ensure recalculation of such fields, set the corresponding flags to false (for example, 
        /// BoundsDefined for MinimalValue and MaximalValue, ScalingLengthDefined for ScalingLength, TargetValueDefined for
        /// TargetValue, etc.).
        /// </summary>
        /// <param name="data">Definition data that is updated.</param>
        /// <param name="trainingData">Training data that is used to calculate some values. If null then values
        /// which would be calculated form training data are not updated.</param>
        public static void SupplementDataDefinition(InputOutputDataDefiniton data, SampledDataSet trainingData)
        {
            if (data == null)
                throw new ArgumentNullException("Input/output data definition object to be updated is not defined (null reference).");
            else
            {
                // First, set those fields (if not yet set) that can be generated and do not depend on training data:
                for (int i = 0; i < data.InputLength; ++i)
                {
                    InputElementDefinition inputEl = data.GetInputElement(i);
                    if (string.IsNullOrEmpty(inputEl.Name))
                        inputEl.Name = "param_" + i;
                    if (string.IsNullOrEmpty(inputEl.Title))
                        inputEl.Title = "Input parameter " + i;
                    if (string.IsNullOrEmpty(inputEl.Description))
                        inputEl.Description = "Input parameter No. " + i + ".";
                    inputEl.IsInput = true;
                    if (!inputEl.ElementIndexSpecified)
                    {
                        inputEl.ElementIndex = i;
                        inputEl.ElementIndexSpecified = true;
                    }
                    if (inputEl.ElementIndexSpecified && inputEl.ElementIndex != i)
                    {
                        Console.WriteLine();
                        Console.WriteLine("WARNING:");
                        Console.WriteLine("Element index (" + inputEl.ElementIndex + ") does not correspond to sequential number ("
                            + i + ") of input element.");
                        Console.WriteLine();
                    }
                    if (!inputEl.OptimizationIndexSpecified)
                    {
                        inputEl.OptimizationIndex = i;
                        inputEl.OptimizationIndexSpecified = true;
                    }
                }
                for (int i = 0; i < data.OutputLength; ++i)
                {
                    OutputElementDefinition outputEl = data.GetOutputElement(i);
                    if (string.IsNullOrEmpty(outputEl.Name))
                        outputEl.Name = "out_" + i;
                    if (string.IsNullOrEmpty(outputEl.Title))
                        outputEl.Title = "Output value " + i;
                    if (string.IsNullOrEmpty(outputEl.Description))
                        outputEl.Description = "Output value No. " + i + ".";
                    outputEl.IsInput = false;
                    if (!outputEl.ElementIndexSpecified)
                    {
                        outputEl.ElementIndex = i;
                        outputEl.ElementIndexSpecified = true;
                    }
                    if (outputEl.ElementIndexSpecified && outputEl.ElementIndex != i)
                    {
                        Console.WriteLine();
                        Console.WriteLine("WARNING:");
                        Console.WriteLine("Element index (" + outputEl.ElementIndex + ") does not correspond to sequential number ("
                            + i + ") of output element.");
                        Console.WriteLine();
                    }
                }
            }
            // Then, set those fields that depend on training data and are not defined yet:
            if (trainingData != null)
            {
                IBoundingBox inputBox = null;
                IBoundingBox outputBox = null;
                trainingData.GetInputRange(ref inputBox);
                trainingData.GetOutputRange(ref outputBox);
                for (int i = 0; i < data.InputLength; ++i)
                {
                    InputElementDefinition inputEl = data.GetInputElement(i);
                    if (!inputEl.BoundsDefined)
                    {
                        inputEl.MinimalValue = inputBox.GetMin(i);
                        inputEl.MaximalValue = inputBox.GetMax(i);
                        inputEl.BoundsDefined = true;
                    }
                    if (!inputEl.DefaultValueDefined)
                    {
                        inputEl.DefaultValue = 0.5 * (inputBox.GetMin(i) + inputBox.GetMax(i));
                        inputEl.DefaultValueDefined = true;
                    }
                    if (!inputEl.TargetValueDefined)
                    {
                        inputEl.TargetValue = 0.5 * (inputBox.GetMin(i) + inputBox.GetMax(i));
                        inputEl.TargetValueDefined = true;
                    }
                    if (!inputEl.ScalingLengthDefined)
                    {
                        inputEl.ScalingLength = inputBox.GetMax(i) - inputBox.GetMin(i);
                        inputEl.ScalingLengthDefined = true;
                    }
                }
                for (int i = 0; i < data.OutputLength; ++i)
                {
                    OutputElementDefinition outputEl = data.GetOutputElement(i);
                    if (!outputEl.BoundsDefined)
                    {
                        outputEl.MinimalValue = outputBox.GetMin(i);
                        outputEl.MaximalValue = outputBox.GetMax(i);
                        outputEl.BoundsDefined = true;
                    }
                    if (!outputEl.TargetValueDefined)
                    {
                        outputEl.TargetValue = 0.5 * (outputBox.GetMin(i) + outputBox.GetMax(i));
                        outputEl.TargetValueDefined = true;
                    }
                    if (!outputEl.ScalingLengthDefined)
                    {
                        outputEl.ScalingLength = outputBox.GetMax(i) - outputBox.GetMin(i);
                        outputEl.ScalingLengthDefined = true;
                    }
                }
            }
        }


        /// <summary>Suplements the specified data definition object with information that is extracted
        /// from the training data set and with automatically generated information.</summary>
        /// <param name="dataDefinitionPath">Path to the original data definition file. It can be unspecified,
        /// in this case the initial data definition object is created and initialized with some default values.</param>
        /// <param name="trainingDataPath">Path to the training data. It can be unspecified, in this case training
        /// data is not used, and only the data that can have some default values is updated.</param>
        public static void SupplementDataDefinition(string dataDefinitionPath, string trainingDataPath)
        {
            SupplementDataDefinition(dataDefinitionPath, trainingDataPath, dataDefinitionPath /* updatedDataDefinitionPath */ );
        }

        /// <summary>Suplements the specified data definition object with information that is extracted
        /// from the training data set and with automatically generated information.</summary>
        /// <param name="dataDefinitionPath">Path to the original data definition file. It can be unspecified,
        /// in this case the initial data definition object is created and initialized with some default values.</param>
        /// <param name="trainingDataPath">Path to the training data. It can be unspecified, in this case training
        /// data is not used, and only the data that can have some default values is updated.</param>
        /// <param name="updatedDataDefinitionPath">Path where the updated data defiition is saved.
        /// It can be the same path as for the original file.</param>
        public static void SupplementDataDefinition(string dataDefinitionPath, string trainingDataPath,
            string updatedDataDefinitionPath)
        {
            if (string.IsNullOrEmpty(updatedDataDefinitionPath))
                throw new ArgumentException("Path of the file to store updated data definitions is not specified.");
            InputOutputDataDefiniton dataDef = null;
            SampledDataSet trainingData = null;
            if (File.Exists(dataDefinitionPath))
                InputOutputDataDefiniton.LoadJson(dataDefinitionPath, ref dataDef);
            else if (!string.IsNullOrEmpty(dataDefinitionPath))
                throw new ArgumentException("The specified data definition file does not exist: " + dataDefinitionPath);
            if (File.Exists(trainingDataPath))
                SampledDataSet.LoadJson(trainingDataPath, ref trainingData);
            else if (!string.IsNullOrEmpty(trainingDataPath))
                throw new ArgumentException("The specified data training data file does not exist: " + trainingDataPath);
            if (dataDef == null && trainingData == null)
                throw new ArgumentException("Neither original data definition nor training data could be read.");
            int inputLength = 0, outputLength = 0;
            if (dataDef != null)
            {
                inputLength = dataDef.InputLength;
                outputLength = dataDef.OutputLength;
            }
            else if (trainingData != null)
            {
                inputLength = trainingData.InputLength;
                outputLength = trainingData.OutputLength;
            }
            if (dataDef == null)
                dataDef = CreateDefault(inputLength, outputLength);
            SupplementDataDefinition(dataDef, trainingData);
            SaveJson(dataDef, updatedDataDefinitionPath);
        }

        /// <summary>Copies data from one data definition to another.</summary>
        /// <param name="original">Source object that data is copied from.</param>
        /// <param name="copy">Object to which data is copied.</param>
        /// <remarks>Operation is performed only if both sorce and destination objects are non-null.</remarks>
        public static void CopyPlain(InputOutputDataDefiniton original, InputOutputDataDefiniton copy)
        {
            if (original != null && copy != null)
            {
                copy.Name = original.Name;
                copy.Description = original.Description;
                copy.InputElementList.Clear();
                copy.OutputElementList.Clear();
                for (int i = 0; i < original.InputLength; ++i)
                {
                    InputElementDefinition orig = original.GetInputElement(i);
                    InputElementDefinition target = null;
                    InputElementDefinition.Copy(orig, ref target);
                    copy.AddInputElement(target);
                }
                for (int i = 0; i < original.OutputLength; ++i)
                {
                    OutputElementDefinition orig = original.GetOutputElement(i);
                    OutputElementDefinition target = null;
                    OutputElementDefinition.Copy(orig, ref target);
                    copy.AddOutputElement(target);
                }
            }
        }


        /// <summary>Copies data from one data definition to another.</summary>
        /// <param name="original">Source object that data is copied from.</param>
        /// <param name="copy">Object to which data is copied.</param>
        /// <remarks>If original object is null then destination object is also set to null.
        /// If destination object is nul then it is allocated anew.</remarks>
        public static void Copy(InputOutputDataDefiniton original, ref InputOutputDataDefiniton copy)
        {
            if (original == null)
                copy = null;
            else
            {
                if (copy == null)
                    copy = new InputOutputDataDefiniton();
                InputOutputDataDefiniton.CopyPlain(original, copy);
            }
        }


        #endregion StaticMethods



        #region ExampleData

        /// <summary>Creates and returns an example data definition.</summary>
        /// <param name="inputLength">Number of input elements.</param>
        /// <param name="outputLength">Number of output elements.</param>
        public static InputOutputDataDefiniton CreateExample(int inputLength, int outputLength)
        {
            InputOutputDataDefiniton ret = new InputOutputDataDefiniton();
            for (int i = 0; i < inputLength; ++i)
            {
                InputElementDefinition inputElement = new InputElementDefinition(i,
                    "param_" + i.ToString(), "Input parameter " + i + ".",
                    "This is description of input parameter No. " + i + ".");
                ret.AddInputElement(inputElement);
            }
            for (int i = 0; i < outputLength; ++i)
            {
                OutputElementDefinition outputElement = new OutputElementDefinition(i,
                    "val_" + i.ToString(), "Output value " + i + ".",
                    "This is description of output value No. " + i + ".");
                ret.AddOutputElement(outputElement);
            }
            return ret;
        }

        #endregion ExampleData


        /// <summary>Returns string representation of the current object.</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Input/output data definition: ");
            if (InputElementList == null)
                sb.AppendLine("There is no input data definition.");
            else
            {
                sb.AppendLine("Number of input elements:  " + InputLength);
                sb.AppendLine("Number of output elements: " + OutputLength);
                sb.AppendLine();

                sb.AppendLine("Input data: ");
                for (int i = 0; i < InputElementList.Count; ++i)
                {
                    sb.AppendLine("  Input element " + i + ":");
                    if (InputElementList[i] == null)
                        sb.AppendLine("  null.");
                    else
                        sb.AppendLine(InputElementList[i].ToString());
                }
            }
            if (OutputElementList == null)
                sb.AppendLine("There is no output data definition.");
            else
            {
                sb.AppendLine("Output data: ");
                for (int i = 0; i < OutputElementList.Count; ++i)
                {
                    sb.AppendLine("  Output element " + i + ":");
                    if (OutputElementList[i] == null)
                        sb.AppendLine("  null.");
                    else
                        sb.AppendLine(OutputElementList[i].ToString());
                }
            }
            return sb.ToString();
        }


    }  // class InputOutputDataDefiniton



}