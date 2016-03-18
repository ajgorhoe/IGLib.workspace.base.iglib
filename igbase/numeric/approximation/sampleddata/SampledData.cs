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



    /// <summary>A single element of a sampled data, contains vector of input parameters and vector of output values.</summary>
    /// $A Igor Jun08;
    [Serializable]
    public class SampledDataElement
    {

        /// <summary>Creates a new sampled data element with the specified vectors of input parameters 
        /// and corresponding output values.</summary>
        /// <param name="input">Input parameters.</param>
        /// <param name="output">Corresponding output values.</param>
        public SampledDataElement(IVector input, IVector output)
        {
            _inputParameters = input;
            _outputValues = output;
        }

        #region Data

        private int _index;

        /// <summary>Idex of the current sampled data element in arrays or list (or in <see cref="SampledDataSet"/>
        /// objects), auxiliary information that facilitates identification and access to specific data chunk out 
        /// of those that are worked out.</summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        protected IVector _inputParameters;

        /// <summary>Vector of input parameters of a single element of sampled data.</summary>
        public virtual IVector InputParameters
        { get { return _inputParameters; } set { _inputParameters = value; } }

        protected IVector _outputValues;

        /// <summary>Vector of input parameters of a single element of sampled data.</summary>
        public virtual IVector OutputValues
        { get { return _outputValues; } set { _outputValues = value; } }

        /// <summary>Returns number of input parameters of the current sampled data element.</summary>
        public virtual int InputLength
        { get { if (InputParameters == null) return 0; else return InputParameters.Length; } }

        /// <summary>Returns number of output values of the current sampled data element.</summary>
        public virtual int OutputLength
        { get { if (OutputValues == null) return 0; else return OutputValues.Length; } }

        #endregion Data

        #region StaticTools

        /// <summary>Updates indices of sampled data elements contained in the specified collection in such
        /// a way that they correspond with their position in collection.
        /// <para>In this way, element indices can be used to directly access elements within the collection
        /// (without a search).</para></summary>
        /// <param name="elements">Collection of sampled data elements for which element indices are updated.</param>
        public static void UpdateElemetIndices(IEnumerable<SampledDataElement> elements)
        {
            int currentIndex = 0;
            foreach (SampledDataElement el in elements)
            {
                el.Index = currentIndex;
                ++currentIndex;
            }
        }

        #endregion StaticTools

    }  // class SampledDataElement


    /// <summary>Sampled data consisting of elements of which each contains vector of input parameters and output values.
    /// <para>NOT thread safe.</para></summary>
    /// $A Igor Jun08;
    [Serializable]
    public class SampledDataSet
    {

        public SampledDataSet()
            : this(0, 0)
        { }

        public SampledDataSet(int inputLength, int outputLength)
        {
            this.InputLength = inputLength;
            this.OutputLength = outputLength;
        }


        #region Data

        protected internal List<SampledDataElement> ElementList = new List<SampledDataElement>();

        public List<SampledDataElement> GetElementList()
        {
            return ElementList;
        }

        /// <summary>Updates indices of sampled data elements contained in the current sampled set in such
        /// a way that they correspond with their (current) sequential position in the set.
        /// <para>In this way, element indices can be used to directly access sampled data elements
        /// (without a search).</para></summary>
        public void UpdateElementIndices()
        {
            if (ElementList != null)
            {
                SampledDataElement.UpdateElemetIndices(ElementList);
            }
        }

        /// <summary>Returna a copy of the list of data elements.
        /// <para>WARNING:</para>
        /// <para>List returned is a copy of the internal list, therefore changes performed on the list are not reflected on internal
        /// state of the data set object, but changes performed on its elements are.</para></summary>
        public List<SampledDataElement> GetElementListCopy()
        {
            return new List<SampledDataElement>(ElementList);
        }

        /// <summary>Creates and returns a list of all sampled data elemets of the current object that are sorted
        /// according to the specified comparer object (that implements the 
        /// <see cref="IComparer{SampledDataElement}"/> interface).</summary>
        /// <param name="comparer">Comparer that is used for sorting the returned list.</param>
        public List<SampledDataElement> GetSortedElemetnList(IComparer<SampledDataElement> comparer)
        {
            List<SampledDataElement> ret = GetElementListCopy();
            ret.Sort(comparer);
            return ret;
        }

        /// <summary>Creates and returns a list of all sampled data elemets of the current object that are sorted
        /// according to distance between input parameters and a specified reference point.</summary>
        /// <param name="referencePoint">Reference point, sampled data elements are sorted according to the distance
        /// to this point.</param>
        /// <param name="distanceFunction">Delegate that definines distance between two vectors for the purpose of sorting.</param>
        public List<SampledDataElement> GetInputDistanceSortedElemetnList(IVector referencePoint,
            DistanceDelegate distanceFunction)
        {
            List<SampledDataElement> ret = GetElementListCopy();
            ComparerInputDistance comparer = new ComparerInputDistance(referencePoint, distanceFunction);
            ret.Sort(comparer);
            return ret;
        }

        /// <summary>Creates and returns a list of all sampled data elemets of the current object that are sorted
        /// according to distance between output values and a specified reference point in the output values space.</summary>
        /// <param name="referencePoint">Reference point, sampled data elements are sorted according to the distance
        /// to this point.</param>
        /// <param name="distanceFunction">Delegate that definines distance between two vectors for the purpose of sorting.</param>
        public List<SampledDataElement> GetOutputDistanceSortedElemetnList(IVector referencePoint,
            DistanceDelegate distanceFunction)
        {
            List<SampledDataElement> ret = GetElementListCopy();
            ComparerOutputDistance comparer = new ComparerOutputDistance(referencePoint, distanceFunction);
            ret.Sort(comparer);
            return ret;
        }

        /// <summary>Gets or sets sampled data, as an array of data elements.</summary>
        public SampledDataElement[] Data
        {
            get
            {
                if (ElementList == null)
                {
                    return null;
                }
                else
                    return ElementList.ToArray();
            }
            set
            {
                if (value == null)
                    ElementList.Clear();
                else
                    ElementList = new List<SampledDataElement>(value);
            }
        }

        /// <summary>Gets the number of sampled data elements (input/output pairs) contained by the current sampled data set.</summary>
        public int Length
        { get { if (ElementList == null) return 0; else return ElementList.Count; } }

        protected int _inputLength = -1;

        /// <summary>Number of input parameters in sampled data elements. Less than 0 means unspecified.</summary>
        public int InputLength
        {
            get {
                if (_inputLength < 0)
                {
                    // Try to figure out number of input parameters from data:
                    if (Length > 0)
                    {
                        IVector inputs = this[0].InputParameters;
                        if (inputs != null)
                            _inputLength = inputs.Length;
                    }
                }
                return _inputLength;
            }
            set
            {
                if (_inputLength >= 0 && value != _inputLength)
                {
                    // A new input length is specified, therefore we must clear the data.
                    ElementList.Clear();
                }
                _inputLength = value;
            }
        }

        protected int _outputLength = -1;

        /// <summary>Number of output values in sampled data elements. Less than 0 means unspecified.</summary>
        public int OutputLength
        {
            get {
                if (_outputLength < 0)
                {
                    // Try to figure out number of output parameters from data:
                    if (Length > 0)
                    {
                        IVector outputs = this[0].OutputValues;
                        if (outputs != null)
                            _outputLength = outputs.Length;
                    }
                }
                return _outputLength; }
            set
            {
                if (_outputLength >= 0 && value != _outputLength)
                {
                    // A new input length is specified, therefore we must clear the data.
                    ElementList.Clear();
                }
                _outputLength = value;
            }
        }

        #region DataOperations


        /// <summary>Gets or sets specific data element.</summary>
        /// <param name="which">Index of data element.</param>
        public SampledDataElement this[int which]
        {
            get { return ElementList[which]; }
            set { ElementList[which] = value; }
        }

        public virtual void Clear()
        { ElementList.Clear(); }

        /// <summary>Adda a new element to sampled data.</summary>
        /// <param name="element">Data element that is added to the sampled data set.
        /// If element is null then nothing is added (but no exception thrown).</param>
        public void AddElement(SampledDataElement element)
        {
            if (element != null)
            {
                if (InputLength > 0 && element.InputLength != InputLength)
                    throw new ArgumentException("Added sampled data element has wrong number of input parameters, "
                        + element.InputLength + " instead of " + InputLength);
                if (OutputLength > 0 && element.OutputLength != OutputLength)
                    throw new ArgumentException("Added sampled data element has wrong number of output values, "
                        + element.OutputLength + " instead of " + OutputLength);
                if (InputLength <= 0)
                    InputLength = element.InputLength;
                if (OutputLength <= 0)
                    OutputLength = element.OutputLength;
                ElementList.Add(element);
            }
        }

        /// <summary>Adds elements of another sampled data ser to the current sampled data.
        /// Only references are copied.</summary>
        /// <param name="addedSet">Sampled data whose elements are added to the current sampled data.</param>
        public void Add(SampledDataSet addedSet)
        {
            if (addedSet != null)
            {
                for (int i = 0; i < addedSet.Length; ++i)
                    AddElement(addedSet[i]);
            }
        }

        /// <summary>Adds array of sampled data elements to teh current sampled data set.
        /// Only references are copied.</summary>
        /// <param name="addedSet">Sampled data whose elements are added to the current sampled data set.</param>
        public void Add(params SampledDataElement[] addedSet)
        {
            if (addedSet != null)
            {
                for (int i = 0; i < addedSet.Length; ++i)
                    AddElement(addedSet[i]);
            }
        }

        /// <summary>Returns the vector of input parameters of the specified element of the sampled data set.</summary>
        /// <param name="which">Index of the sampled data element within the sampled data set.</param>
        public IVector GetInputParameters(int which)
        {
            SampledDataElement element = this[which];
            if (element == null)
                throw new InvalidDataException("Can not get input parameters No. " + which + ": sampled data element not defined (null reference).");
            return element.InputParameters;
        }

        /// <summary>Sets the vector of input parameters of the specified element of the sampled data set.</summary>
        /// <param name="which">Index of the sampled data element within the sampled data set.</param>
        /// <param name="parameters">Vector of input parameters to be set.</param>
        public void SetInputParameters(int which, IVector parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("Vector of input parameters to be set is not specified (null reference).");
            if (this.InputLength > 0 && parameters.Length != this.InputLength)
                throw new ArgumentException("Vector of input parameters No. " + which + " to be set has wrong dimension, "
                    + parameters.Length + " instead of " + this.InputLength);
            SampledDataElement element = this[which];
            if (element == null)
                throw new InvalidDataException("Can not set input input parameters No. " + which + ": data element not defined (null reference).");
            element.InputParameters = parameters;
        }

        /// <summary>Returns the vector of output values of the specified element of the sampled data set.</summary>
        /// <param name="which">Index of the data element within the sampled set.</param>
        public virtual IVector GetOutputValues(int which)
        {
            SampledDataElement element = this[which];
            if (element == null)
                throw new InvalidDataException("Can not get output values No. " + which + ": data element not defined (null reference).");
            return element.OutputValues;
        }

        /// <summary>Sets the vector of output values of the specified element of the sampled data set.</summary>
        /// <param name="which">Index of the data element within the sampled data set.</param>
        /// <param name="values">Vector of output values to be set.</param>
        public void SetOutputValues(int which, IVector values)
        {
            if (values == null)
                throw new ArgumentNullException("Vector of output values to be set is not specified (null reference).");
            if (this.OutputLength > 0 && values.Length != this.OutputLength)
                throw new ArgumentException("Vector of output values No. " + which + " to be set has wrong dimension, "
                    + values.Length + " instead of " + this.OutputLength);
            SampledDataElement element = this[which];
            if (element == null)
                throw new InvalidDataException("Can not set output values No. " + which + ": data element not defined (null reference).");
            element.OutputValues = values;
        }


        /// <summary>Calculates range of input parameters of the current sampled data set, and stores
        /// it to the specified bounding box.</summary>
        /// <param name="bounds">Bounding box wehere bounds on input parameters are stored.</param>
        public void GetInputRange(ref IBoundingBox bounds)
        {
            if (this.Length == 0)
                bounds = null;
            else
            {
                if (bounds == null)
                    bounds = new BoundingBox(this.InputLength);
                else
                    bounds.SetDimensionAndReset(this.InputLength);
                for (int i = 0; i < this.Length; ++i)
                    bounds.Update(this.GetInputParameters(i));
            }
        }

        /// <summary>Calculates range of output values of the current sampled data set, and stores
        /// it to the specified bounding box.</summary>
        /// <param name="bounds">Bounding box wehere bounds on output values are stored.</param>
        public void GetOutputRange(ref IBoundingBox bounds)
        {
            if (this.Length == 0)
                bounds = null;
            else
            {
                if (bounds == null)
                    bounds = new BoundingBox(this.OutputLength);
                else
                    bounds.SetDimensionAndReset(this.OutputLength);
                for (int i = 0; i < this.Length; ++i)
                    bounds.Update(this.GetOutputValues(i));
            }
        }


        #endregion DataOperations

        #endregion Data


        #region Utilities

        // Extracting vector of input parameters:

        /// <summary>Extracts vectors of input parameters from the current sampled data set, and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        public void ExtractInputs(ref IVector[] extracted)
        {
            ExtractFilteredData(null /* filterIndices */, ref extracted,
                true /* complement */, false /* outputData */, false /* copyValues */);
        }

        /// <summary>Extracts the specified vectors of input parameters from the current sampled data set, and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).
        /// An index list determines which vectors to extract (i.e. which data elements to exclude).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        /// <param name="filterIndices">List of indices of those elements of the current sampled data set that are extracted.</param>
        public void ExtractInputs(IndexList filterIndices, ref IVector[] extracted)
        {
            ExtractFilteredData(filterIndices /* filterIndices */, ref extracted,
                false /* complement */, false /* outputData */, false /* copyValues */);
        }

        /// <summary>Extracts the specified vectors (complement of the index list) of input parameters from the current sampled data set, 
        /// and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).
        /// An index list determines which vectors NOT to extract (i.e. which data elements to include).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        /// <param name="filterIndices">List of indices of those elements of the current sampled data set that are extracted.</param>
        public void ExtractInputsComplement(IndexList filterIndices, ref IVector[] extracted)
        {
            ExtractFilteredData(filterIndices /* filterIndices */, ref extracted,
                true /* complement */, false /* outputData */, false /* copyValues */);
        }

        // Extracting vectors of output parameters:

        /// <summary>Extracts vectors of output values from the current sampled data set, and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        public void ExtractOutputs(ref IVector[] extracted)
        {
            ExtractFilteredData(null /* filterIndices */, ref extracted,
                true /* complement */, true /* outputData */, false /* copyValues */);
        }

        /// <summary>Extracts the specified vectors of output values from the current sampled data set, and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).
        /// An index list determines which vectors to extract (i.e. which data elements to exclude).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        /// <param name="filterIndices">List of indices of those elements of the current sampled data set that are extracted.</param>
        public void ExtractOutputs(IndexList filterIndices, ref IVector[] extracted)
        {
            ExtractFilteredData(filterIndices /* filterIndices */, ref extracted,
                false /* complement */, true /* outputData */, false /* copyValues */);
        }

        /// <summary>Extracts the specified vectors (complement of the index list) of output values from the current sampled data set, 
        /// and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).
        /// An index list determines which vectors NOT to extract (i.e. which data elements to include).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        /// <param name="filterIndices">List of indices of those elements of the current sampled data set that are extracted.</param>
        public void ExtractOutputsComplement(IndexList filterIndices, ref IVector[] extracted)
        {
            ExtractFilteredData(filterIndices /* filterIndices */, ref extracted,
                true /* complement */, true /* outputData */, false /* copyValues */);
        }


        // Copying vector of input parameters:

        /// <summary>Copies vectors of input parameters from the current sampled data set, and stores them to the specified array.
        /// References of the extracted vectors aer stored (no deep copying performed).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        public void CopyInputs(ref IVector[] extracted)
        {
            ExtractFilteredData(null /* filterIndices */, ref extracted,
                true /* complement */, false /* outputData */, false /* copyValues */);
        }

        /// <summary>Copies the specified vectors of input parameters from the current sampled data set, and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).
        /// An index list determines which vectors to extract (i.e. which sampled elements to exclude).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        /// <param name="filterIndices">List of indices of those elements of the current sampled data set that are extracted.</param>
        public void CopyInputs(IndexList filterIndices, ref IVector[] extracted)
        {
            ExtractFilteredData(filterIndices /* filterIndices */, ref extracted,
                false /* complement */, false /* outputData */, false /* copyValues */);
        }

        /// <summary>Copies the specified vectors (complement of the index list) of input parameters from the current sampled data set, 
        /// and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).
        /// An index list determines which vectors NOT to extract (i.e. which data elements to include).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        /// <param name="filterIndices">List of indices of those elements of the current sampled data set that are extracted.</param>
        public void CopyInputsComplement(IndexList filterIndices, ref IVector[] extracted)
        {
            ExtractFilteredData(filterIndices /* filterIndices */, ref extracted,
                true /* complement */, false /* outputData */, false /* copyValues */);
        }

        // Copying vectors of output parameters:

        /// <summary>Copies vectors of output values from the current sampled data set, and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        public void CopyOutputs(ref IVector[] extracted)
        {
            ExtractFilteredData(null /* filterIndices */, ref extracted,
                true /* complement */, true /* outputData */, false /* copyValues */);
        }

        /// <summary>Copies the specified vectors of output values from the current sampled data set, and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).
        /// An index list determines which vectors to extract (i.e. which data elements to exclude).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        /// <param name="filterIndices">List of indices of those elements of the current sampled data set that are extracted.</param>
        public void CopyOutputs(IndexList filterIndices, ref IVector[] extracted)
        {
            ExtractFilteredData(filterIndices /* filterIndices */, ref extracted,
                false /* complement */, true /* outputData */, false /* copyValues */);
        }

        /// <summary>Copies the specified vectors (complement of the index list) of output values from the current data set, 
        /// and stores them to the specified table.
        /// References of the extracted vectors aer stored (no deep copying performed).
        /// An index list determines which vectors NOT to extract (i.e. which sampled data elements to include).</summary>
        /// <param name="extracted">Table where extracted vectors are stored.</param>
        /// <param name="filterIndices">List of indices of those elements of the current data set that are extracted.</param>
        public void CopyOutputsComplement(IndexList filterIndices, ref IVector[] extracted)
        {
            ExtractFilteredData(filterIndices /* filterIndices */, ref extracted,
                true /* complement */, true /* outputData */, false /* copyValues */);
        }


        /// Extract extracts 


        /// <summary>Extract the specified input or output vectors from the current set, and stored them in
        /// the specified array of vectors.</summary>
        /// <param name="filterIndices">List of filter indices that specify which element to extract from or omit.
        /// Can be null, in this case it is taken that there are no filter indices.</param>
        /// <param name="extracted">Array where the extracted vectors are stored.</param>
        /// <param name="complement">If true then vectors are extracted from those elements whose indices are NOT contained in filterIndices list.
        /// If false then vectors are extracted from those elements whose indices ARE i nthe filterIndices list.</param>
        /// <param name="outputData">If true then vectors of output values are extracted, otherwise vectors of input parameters are extracted.</param>
        /// <param name="copyValues">If true then extracted vectors are copied component-wise. If false then only references of extracted vectors are copied.</param>
        public void ExtractFilteredData(IndexList filterIndices, ref IVector[] extracted,
            bool complement, bool outputData, bool copyValues)
        {
            int numElements = this.Length;
            int numFilterIndices = 0;  // number of indices in the filter
            if (filterIndices != null)
                numFilterIndices = filterIndices.Length;
            int numToExtract; // number of vectors to extract
            if (complement)
                numToExtract = numElements - numFilterIndices;
            else
                numToExtract = numFilterIndices;
            int whichElement = 0;  // current element of this data set
            int whichExtracted = 0;  // current element of extracted vectors array
            int whichFilterIndex = 0;  // current element in filter index list
            if (extracted == null)
                extracted = new IVector[numToExtract];
            else if (extracted.Length != numToExtract)
                extracted = new IVector[numToExtract];
            while (whichExtracted < numToExtract)
            {
                bool extractThis;  // whether to extract from the current element
                if (complement)
                {
                    // Extracted data is from those elements whose indices are NOT contained in filterIndices
                    extractThis = true;  // this holds in the case that filter indices do not contain index of the current element
                    if (whichFilterIndex < numFilterIndices)
                        if (filterIndices[whichFilterIndex] == whichElement)
                        {
                            extractThis = false;
                            ++whichFilterIndex;
                        }
                }
                else
                {
                    // Extracted data is from those elements whose indices ARE contained in filterIndices
                    extractThis = false;  // this holds in the case that filter indices do not contain index of the current element
                    if (whichFilterIndex < numFilterIndices)
                        if (filterIndices[whichFilterIndex] == whichElement)
                        {
                            extractThis = true;
                            ++whichFilterIndex;
                        }
                }
                if (extractThis)
                {
                    if (whichElement >= numElements)
                    {
                        throw new InvalidOperationException("UnExpectedly, no elements in the samped data set left for extraction." + Environment.NewLine
                            + "  Number of elements: " + numElements + ", current: " + whichElement + Environment.NewLine
                            + "  Number of indices:  " + numFilterIndices + ", current: " + whichFilterIndex + Environment.NewLine
                            + "  Number to extract:  " + numToExtract + ", current: " + whichExtracted);
                    }
                    IVector extractedVector;
                    if (outputData)
                        extractedVector = GetOutputValues(whichElement);
                    else
                        extractedVector = GetInputParameters(whichElement);
                    if (copyValues)
                    {
                        Vector.Copy(extractedVector, ref extracted[whichExtracted]);
                    }
                    else
                        extracted[whichExtracted] = extractedVector;
                    ++whichExtracted;
                    ++whichElement;
                }
            }
        }


        #region DuplicatesAndNull

        /// <summary>Returns number of null elements of the current sampled data set.</summary>
        /// $A Igor Feb12;
        public int GetNumNullElemets()
        {
            return GetNumNullElemets(this);
        }

        /// <summary>Returns number of elements of the current sampled data set with duplicated input parameters. 
        /// <para>Vectors of input parameters are considered the same if both are null or all components 
        /// are the same (i.e., comparison is performed component-wise rather than by reference).</para>
        /// <para>elements that are null are not counted as duplicates.</para>
        /// <para>For each group of n elements with the same input parameters, n-1 is added to the returned number.</para></summary>
        /// $A Igor Feb12;
        public int GetNumInputDuplicates()
        {
            return GetNumInputDuplicates(this);
        }

        /// <summary>Removes elements with duplibated input parameters from the current sampled data set, 
        /// leaving only a single element with specified input parameters.
        /// Elements that are null are also removed.</summary>
        /// $A Igor Feb12;
        public void RemoveInputDuplicates()
        {
            RemoveInputDuplicates(this);
        }

        #endregion DuplicatesAndNull

        #endregion Utilities


        #region SortingAndDistance


        // DISTANCE OPERATIONS:

        public delegate double DistanceDelegate(IVector v1, IVector v2);

        // input input Input Input input parameters input parameters Input parameters

        /// <summary>Comparer that compares two data elements of type <see cref="SampledDataElement"/>
        /// according to the distance of their input parameter vectors to a specified reference point (vector) 
        /// in the input parameter space.
        /// <para>Measure of distance is defined by the <see cref="DistanceDelegate"/> delegate.</para>
        /// <para>Beside its basic comparison function, this class also contains a number of methods for 
        /// calculation of distance between vectors or data elemets according to definition by the
        /// distacnce calculation delegate.</para></summary>
        /// $A Igor Dec11;
        public class ComparerInputDistance : Comparer<SampledDataElement>, IComparer<SampledDataElement>
        {

            #region Construction

            private ComparerInputDistance() { }  // disable argumentless constructor

            /// <summary>Constructs a new comparer according to input distance to a reference poiont (type <see cref="IVector"/>).
            /// <para>Distance between two points is calculated as Euclidean distance.</para>
            /// <para>This method is protected because it is meant that distance delegate must be provided.
            /// However, classes can be derived that allows that distance delegate is not specified.</para></summary>
            /// <param name="referencePoint">Reference point. Data elements are compared by their distance to this point.</param>
            /// <param name="immutable">If true then the reference point is copied rather than oly its reference would be taken.</param>
            protected ComparerInputDistance(IVector referencePoint, bool immutable)
            {
                this.IsImmutable = immutable;
                if (referencePoint == null)
                    throw new ArgumentException("The reference point for measuring distance is not specified (null argument).");
                if (immutable)
                    this._referencePoint = referencePoint.GetCopy();
                else
                    this._referencePoint = referencePoint;
                this._distanceFunction = VectorBase.Distance;
            }

            /// <summary>Constructs a new comparer according to input distance to a reference poiont (type <see cref="IVector"/>).</summary>
            /// <param name="referencePoint">Reference point. Data elements are compared by their distance to this point.</param>
            /// <param name="distanceFunction">Delegate used for calculation of distance between two points.</param>
            /// <param name="immutable">If true then a copy of the reference point is stored internally rather than just 
            /// its reference, so it can not be changed.</param>
            public ComparerInputDistance(IVector referencePoint, DistanceDelegate distanceFunction, bool immutable)
            {
                this.IsImmutable = immutable;
                if (referencePoint == null)
                    throw new ArgumentException("The reference point for measuring distance is not specified (null argument).");
                if (distanceFunction == null)
                    throw new ArgumentException("Function or delegate for calculating distance between two points is not specified.");
                if (immutable)
                    this._referencePoint = referencePoint.GetCopy();
                else
                    this._referencePoint = referencePoint;
                this._distanceFunction = distanceFunction;
            }

            /// <summary>Constructs a new comparer according to input distance to a reference poiont (type <see cref="IVector"/>).
            /// <para>Object constructed in this way is not immutable.</para></summary>
            /// <param name="referencePoint">Reference point. Data elements are compared by their distance to this point.</param>
            /// <param name="distanceFunction">Delegate used for calculation of distance between two points.
            /// its reference, so it can not be changed.</param>
            public ComparerInputDistance(IVector referencePoint, DistanceDelegate distanceFunction) :
                this(referencePoint, distanceFunction, false /* immutable */)
            { }

            #endregion Construction

            private bool _isImmutable = false;

            /// <summary>Whether the current object is immutable or not.</summary>
            public bool IsImmutable
            {
                get { return _isImmutable; }
                protected set { _isImmutable = value; }
            }

            protected IVector _referencePoint;

            /// <summary>Reference point in the space of input parameters.
            /// <para>Data elements are compared by the distance of their input vectors to this point.</para></summary>
            public IVector ReferencePoint
            {
                get { return _referencePoint; }
                set
                {
                    if (_isImmutable)
                        throw new InvalidOperationException("Can not set the reference point because the comparer object is configured as immutable.");
                    _referencePoint = value;
                }
            }

            protected DistanceDelegate _distanceFunction;

            /// <summary>Delegate that calculates distance between two vectors.</summary>
            public DistanceDelegate DistanceFunction
            {
                get { return _distanceFunction; }
                set
                {
                    if (_isImmutable)
                        throw new InvalidOperationException("Can not set the distance function because the comparer object is configured as immutable.");
                    _distanceFunction = value;
                }
            }



            /// <summary>Compares two data elemets and returns -1 if the first element is smaller than the second,
            /// 0 if they are equal and 1 if the first element is larger. Comparison is done accorging to the distance
            /// of the data element's input parameters (defined by the distance delegate of type <see cref="DistanceDelegate"/>)
            /// to the reference point.</summary>
            /// <param name="el1">The first data element to be compared.</param>
            /// <param name="el2">The second data element to be compared.</param>
            /// <returns>-1 if the first compared element is smaller than the second one, 0 if it is equal and 1 
            /// if it is greater than the cecond one, according to its distance to a reference point in space of input parameters.</returns>
            public override int Compare(SampledDataElement el1, SampledDataElement el2)
            {
                double d1 = _distanceFunction(el1.InputParameters, _referencePoint);
                double d2 = _distanceFunction(el2.InputParameters, _referencePoint);
                return Comparer<double>.Default.Compare(d1, d2);
            }

            /// <summary>Returns distance in the input parameters space between two points, as defined by the 
            /// distance calculation delegate of the current comparer object.</summary>
            /// <param name="v1">The first point.</param>
            /// <param name="v2">The second point.</param>
            public virtual double Distance(IVector v1, IVector v2)
            {
                if (v1 == null || v2 == null)
                {
                    if (v1 == null)
                        throw new ArgumentException("The first vector operand in input distance calculation is not specified (null reference).");
                    else
                        throw new ArgumentException("The second vector operand in input distance calculation is not specified (null reference).");
                }
                else if (v1.Length != v2.Length)
                    throw new ArgumentException("Vectors between which distance is calculated do not have the same dimension.");
                return _distanceFunction(v1, v2);
            }

            /// <summary>Returns distance in the input parameters space between input vectors of two tarining 
            /// elemets, as defined by the  distance calculation delegate of the current comparer object.</summary>
            /// <param name="el1">The first data element.</param>
            /// <param name="el2">The second data element.</param>
            public double Distance(SampledDataElement el1, SampledDataElement el2)
            {
                if (el1 == null || el2 == null)
                    throw new ArgumentException("One of the data elements whose input distance is calculated is not specified (null reference).");
                return Distance(el1.InputParameters, el2.InputParameters);
            }

            /// <summary>Returns distance in the input parameters space between input vector of the specified tarining 
            /// elemet and the specified point, as defined by the distance calculation delegate of the current comparer object.</summary>
            /// <param name="el1">The first data element.</param>
            /// <param name="p2">The second data element.</param>
            public double Distance(SampledDataElement el1, IVector p2)
            {
                if (el1 == null)
                    throw new ArgumentException("Data element whose input distance to the specified pont is calculated is not specified (null reference).");
                return Distance(el1.InputParameters, p2);
            }

            /// <summary>Returns distance in the input parameters space between the specified vector and the 
            /// reference point of the current comparer object, as defined by the distance calculation delegate 
            /// of the current comparer object.</summary>
            /// <param name="v">Vector whose distance to the reference point is returned.</param>
            public double Distance(IVector v)
            {
                return Distance(v, _referencePoint);
            }


            /// <summary>Returns distance in the input parameters space between the specified data element's input 
            /// parameters vector and the  reference point of the current comparer object, as defined by the distance 
            /// calculation delegate  of the current comparer object.</summary>
            /// <param name="el">Data element whose distance of input parameters to the reference point is returned.</param>
            public double Distance(SampledDataElement el)
            {
                return Distance(el.InputParameters, _referencePoint);
            }

        } // class InputDistanceComparer


        /// <summary>Comparer that compares two data elements of type <see cref="SampledDataElement"/>
        /// according to the distance of their output values vectors to a specified reference point (vector) 
        /// in the output values space.
        /// <para>Measure of distance is defined by the <see cref="DistanceDelegate"/> delegate.</para>
        /// <para>Beside its basic comparison function, this class also contains a number of methods for 
        /// calculation of distance between vectors or data elemets according to definition by the
        /// distacnce calculation delegate.</para></summary>
        /// $A Igor Dec11;
        public class ComparerOutputDistance : Comparer<SampledDataElement>, IComparer<SampledDataElement>
        {

            #region Construction

            private ComparerOutputDistance() { }  // disable argumentless constructor

            /// <summary>Constructs a new comparer according to output distance to a reference poiont (type <see cref="IVector"/>).
            /// <para>Distance between two points is calculated as Euclidean distance.</para>
            /// <para>This method is protected because it is meant that distance delegate must be provided.
            /// However, classes can be derived that allow that distance delegate is not specified.</para></summary>
            /// <param name="referencePoint">Reference point. Data elements are compared by their distance to this point.</param>
            /// <param name="immutable">If true then a copy of the reference point is created. Otherwise, only its reference is taken.</param>
            protected ComparerOutputDistance(IVector referencePoint, bool immutable)
            {
                this.IsImmutable = immutable;
                if (referencePoint == null)
                    throw new ArgumentException("The reference point for measuring distance is not specified (null argument).");
                if (immutable)
                    this._referencePoint = referencePoint.GetCopy();
                else
                    this._referencePoint = referencePoint;
                this._distanceFunction = VectorBase.Distance;
            }

            /// <summary>Constructs a new comparer according to output distance to a reference poiont (type <see cref="IVector"/>).</summary>
            /// <param name="referencePoint">Reference point. Data elements are compared by their distance to this point.</param>
            /// <param name="distanceFunction">Delegate used for calculation of distance between two points.</param>
            /// <param name="immutable">If true then a copy of the reference point is stored internally rather than just 
            /// its reference, so it can not be changed.</param>
            public ComparerOutputDistance(IVector referencePoint, DistanceDelegate distanceFunction, bool immutable)
            {
                this.IsImmutable = immutable;
                if (referencePoint == null)
                    throw new ArgumentException("The reference point for measuring distance is not specified (null argument).");
                if (distanceFunction == null)
                    throw new ArgumentException("Function or delegate for calculating distance between two points is not specified.");
                if (immutable)
                    this._referencePoint = referencePoint.GetCopy();
                else
                    this._referencePoint = referencePoint;
                this._distanceFunction = distanceFunction;
            }

            /// <summary>Constructs a new comparer according to output distance to a reference poiont (type <see cref="IVector"/>).
            /// <para>Object constructed in this way is not immutable.</para></summary>
            /// <param name="referencePoint">Reference point. Data elements are compared by their distance to this point.</param>
            /// <param name="distanceFunction">Delegate used for calculation of distance between two points.</param>
            public ComparerOutputDistance(IVector referencePoint, DistanceDelegate distanceFunction) :
                this(referencePoint, distanceFunction, false /* immutable */)
            { }

            #endregion Construction


            private bool _isImmutable = false;

            /// <summary>Whether the current object is immutable or not.</summary>
            public bool IsImmutable
            {
                get { return _isImmutable; }
                protected set { _isImmutable = value; }
            }

            protected IVector _referencePoint;

            /// <summary>Reference point in the space of output values.
            /// <para>Data elements are compared by the distance of their output vectors to this point.</para></summary>
            public IVector ReferencePoint
            {
                get { return _referencePoint; }
                set
                {
                    if (_isImmutable)
                        throw new InvalidOperationException("Can not set the reference point because the comparer object is configured as immutable.");
                    _referencePoint = value;
                }
            }

            protected DistanceDelegate _distanceFunction;

            /// <summary>Delegate that calculates distance between two vectors.</summary>
            public DistanceDelegate DistanceFunction
            {
                get { return _distanceFunction; }
                set
                {
                    if (_isImmutable)
                        throw new InvalidOperationException("Can not set the distance function becaus the comparer object is configured as immutable.");
                    _distanceFunction = value;
                }
            }


            /// <summary>Compares two data elemets and returns -1 if the first element is smaller than the second,
            /// 0 if they are equal and 1 if the first element is larger. Comparison is done accorging to the distance
            /// of the data element's output values (defined by the distance delegate of type <see cref="DistanceDelegate"/>)
            /// to the reference point.</summary>
            /// <param name="el1">The first data element to be compared.</param>
            /// <param name="el2">The second data element to be compared.</param>
            /// <returns>-1 if the first compared element is smaller than the second one, 0 if it is equal and 1 
            /// if it is greater than the cecond one, according to its distance to a reference point in space of outpu values.</returns>
            public override int Compare(SampledDataElement el1, SampledDataElement el2)
            {
                double d1 = _distanceFunction(el1.OutputValues, _referencePoint);
                double d2 = _distanceFunction(el2.OutputValues, _referencePoint);
                return Comparer<double>.Default.Compare(d1, d2);
            }

            /// <summary>Returns distance in the output values space between two points, as defined by the 
            /// distance calculation delegate of the current comparer object.</summary>
            /// <param name="v1">The first point.</param>
            /// <param name="v2">The second point.</param>
            public virtual double Distance(IVector v1, IVector v2)
            {
                if (v1 == null || v2 == null)
                {
                    if (v1 == null)
                        throw new ArgumentException("The first vector operand in output distance calculation is not specified (null reference).");
                    else
                        throw new ArgumentException("The second vector operand in output distance calculation is not specified (null reference).");
                }
                else if (v1.Length != v2.Length)
                    throw new ArgumentException("Vectors between which distance is calculated do not have the same dimension.");
                return _distanceFunction(v1, v2);
            }


            /// <summary>Returns distance in the output values space between output vectors of two tarining 
            /// elemets, as defined by the  distance calculation delegate of the current comparer object.</summary>
            /// <param name="el1">The first data element.</param>
            /// <param name="el2">The second data element.</param>
            public double Distance(SampledDataElement el1, SampledDataElement el2)
            {
                if (el1 == null || el2 == null)
                    throw new ArgumentException("One of the data elements whose output distance is calculated is not specified (null reference).");
                return Distance(el1.OutputValues, el2.OutputValues);
            }

            /// <summary>Returns distance in the output values space between output vector of the specified tarining 
            /// elemet and the specified point, as defined by the distance calculation delegate of the current comparer object.</summary>
            /// <param name="el1">The first data element.</param>
            /// <param name="p2">The second data element.</param>
            public double Distance(SampledDataElement el1, IVector p2)
            {
                if (el1 == null)
                    throw new ArgumentException("Data element whose output distance to the specified pont is calculated is not specified (null reference).");
                return Distance(el1.OutputValues, p2);
            }

            /// <summary>Returns distance in the output values space between the specified vector and the 
            /// reference point of the current comparer object, as defined by the distance calculation delegate 
            /// of the current comparer object.</summary>
            /// <param name="v">Vector whose distance to the reference point is returned.</param>
            public double Distance(IVector v)
            {
                return Distance(v, _referencePoint);
            }


            /// <summary>Returns distance in the output values space between the specified data element's output 
            /// parameters vector and the  reference point of the current comparer object, as defined by the distance 
            /// calculation delegate  of the current comparer object.</summary>
            /// <param name="el">Data element whose distance of output values to the reference point is returned.</param>
            public double Distance(SampledDataElement el)
            {
                return Distance(el.OutputValues, _referencePoint);
            }

        } // class OutputDistanceComparer


        #endregion SortingAndDistance


        #region StaticMethods

        #region BinarySave


        /// <summary>Saves the specified sempled data to the specified file in binary format.
        /// The file is owerwritten if it exists.</summary>
        /// <param name="sampledData">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file where sampled data is saved.</param>
        public static void SaveBinary(SampledDataSet sampledData, string filePath)
        {
            UtilSystem.SaveBinary<SampledDataSet>(sampledData, filePath);

            // REMARK: binary serializatio nis done directly on objects of this type, not on DTOs.
            // This is because DTOs can not be marked [Serializable] (otherwise serialization to JSON
            // woule be problematic).

            //SampledDataSetDto dtoOriginal = new SampledDataSetDto();
            //dtoOriginal.CopyFrom(sampledData);
            //UtilSystem.SaveBinary<SampledDataSetDto>(dtoOriginal, inputFilePath);
        }

        /// <summary>Restores sampled data from the specified file in binary format.</summary>
        /// <param name="filePath">File from which sampled data is restored.</param>
        /// <param name="dataDefRestored">Sampled data that is restored.</param>
        public static void LoadBinary(string filePath, ref SampledDataSet dataDefRestored)
        {
            dataDefRestored = UtilSystem.LoadBinary<SampledDataSet>(filePath);

            // REMARK: binary serializatio nis done directly on objects of this type, not on DTOs.
            // This is because DTOs can not be marked [Serializable] (otherwise serialization to JSON
            // woule be problematic).

            //ISerializer serializer = new SerializerJson();
            //SampledDataSetDto dtoRestored = serializer.DeserializeFile<SampledDataSetDto>(inputFilePath);
            //dataDefRestored = new SampledDataSet();
            //dtoRestored.CopyTo(ref dataDefRestored);
        }


        #endregion BinarySave

        #region JSON

        /// <summary>Saves the specified sempled data to the specified JSON file.
        /// The file is owerwritten if it exists.</summary>
        /// <param name="sampledData">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file where sampled data is saved.</param>
        public static void SaveJson(SampledDataSet sampledData, string filePath)
        {
            SampledDataSetDto dtoOriginal = new SampledDataSetDto();
            dtoOriginal.CopyFrom(sampledData);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<SampledDataSetDto>(dtoOriginal, filePath);
        }

        /// <summary>Restores sampled data from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which sampled data is restored.</param>
        /// <param name="dataDefRestored">Sampled data that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref SampledDataSet dataDefRestored)
        {
            ISerializer serializer = new SerializerJson();
            SampledDataSetDto dtoRestored = serializer.DeserializeFile<SampledDataSetDto>(filePath);
            dataDefRestored = new SampledDataSet();
            dtoRestored.CopyTo(ref dataDefRestored);
        }

        /// <summary>Loads sampled data and Definition data from multible CSV files.
        /// Sampled data consist of one output and multiple input parameters.
        /// Input parameters are the same in all files, output parameter are different.</summary>
        /// <param name="sampledDat">Sampled data set.</param>
        /// <param name="directoryPath">Path to the file where sampled data are saved.</param>
        /// <param name="fileNames">Name of the files where sampled data are saved.</param>
        /// $A Tako78 Mar11;
        public static void LoadSampledDataCombinedOutputsJSON(ref SampledDataSet sampledDat, string directoryPath, params string[] fileNames)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentNullException("File path not specified.");
            if (fileNames == null)
                throw new ArgumentNullException("File names not specified.");
            if (fileNames.Length < 1)
                throw new ArgumentException("No file names (array length 0).");
            int numFiles = fileNames.Length;
            string[] paths = new string[numFiles];
            for (int i = 0; i < numFiles; ++i)
            {
                paths[i] = Path.Combine(directoryPath, fileNames[i]);
            }
            LoadSampledDataCombinedOutputsJSON(ref sampledDat, paths);
        }



        /// <summary>Loads sampled data and Definition data from multiple CSV files.
        /// Sampled data consist of one output and multiple input parameters.
        /// Input parameters are the same in all files but output parameter should be different.</summary>
        /// <param name="fileNames">Path to the file where sampled data are saved.</param>
        /// <param name="sampledData">Sampled data set.</param>
        /// $A Tako78 Mar11;
        public static void LoadSampledDataCombinedOutputsJSON(ref SampledDataSet sampledData, params string[] fileNames)
        {
            if (string.IsNullOrEmpty(fileNames[0]))
                throw new ArgumentNullException("File names not specified.");
            if (sampledData == null)
                sampledData = new SampledDataSet();
            int filesNum = fileNames.Length;
            SampledDataSet[] inputSets = new SampledDataSet[filesNum];
            for (int i = 0; i < filesNum; ++i)
            {
                LoadSampledDataJson(fileNames[i], ref  inputSets[i]);
            }
            SampledDataCombineOutputs(ref sampledData, inputSets);
        }

        /// <summary>Saves network's sampled data to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="filePath">Path to the file where sampled data is saved.</param>
        /// <param name="sampledData">Sampled data to be saved to a JSON file.</param>
        /// $A Tako78 Mar11;
        public static void SaveSampledDataJson(string filePath, SampledDataSet sampledData)
        {
            {
                SampledDataSetDto dtoOriginal = new SampledDataSetDto();
                dtoOriginal.CopyFrom(sampledData);
                ISerializer serializer = new SerializerJson();
                serializer.Serialize<SampledDataSetDto>(dtoOriginal, filePath);
            }
        }

        /// <summary>Saves network's definition data to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="filePath">Path to the file where definition data is saved.</param>
        /// <param name="definitionData">Definition data to be saved to a JSON file.</param>
        /// $A Tako78 Maj31;
        public static void SaveDefinitionDataJson(string filePath, InputOutputDataDefiniton definitionData)
        {
            {
                InputOutputDataDefinitonDto dtoOriginal = new InputOutputDataDefinitonDto();
                dtoOriginal.CopyFrom(definitionData);
                ISerializer serializer = new SerializerJson();
                serializer.Serialize<InputOutputDataDefinitonDto>(dtoOriginal, filePath);
            }
        }

        /// <summary>Restores sampled data from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which sampled data is restored.</param>
        /// <param name="sampledData">Sampled data to be loaded.</param>
        /// $A Tako78 Mar11;
        public static void LoadSampledDataJson(string filePath, ref SampledDataSet sampledData)
        {
            {
                ISerializer serializer = new SerializerJson();
                SampledDataSetDto dtoRestored = serializer.DeserializeFile<SampledDataSetDto>(filePath);
                dtoRestored.CopyTo(ref sampledData);
            }
        }

        #endregion

        #region CSV

        /// <summary>Loads sampled data and definition data from single CSV file.</summary>
        /// <param name="filePath">Path to the file where sampled data are saved.</param>
        /// <param name="inputLenght">Lenght of input parameters.</param>
        /// <param name="outputLenght">Lenght of output parameters.</param>
        /// <param name="namesSpecified">Flag if names are specified in the file.</param>
        /// <param name="descriptionSpecified">Flag if definitions (descriptions, defaultValue, boundDefiner, minValue, maxValue) are specified in the file.</param>
        /// <param name="titleSpecified">Specifies whether title is specified in the file.</param>
        /// <param name="sampledData">Sampled data set.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Mar11; June27;
        public static void LoadSampledDataCSVinOneLine(string filePath, int inputLenght, int outputLenght,
           bool namesSpecified, bool descriptionSpecified, bool titleSpecified,
           ref SampledDataSet sampledData, ref InputOutputDataDefiniton definitionData)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("File path not specified.");
            // variables 
            int variables = 0;
            string str = null;
            StreamReader reader = null;
            List<double[]> data = new List<double[]>();

            // data names not sbecified, descriptions are specified
            if ((!namesSpecified && descriptionSpecified) || (!namesSpecified && titleSpecified))
                throw new ArgumentException("Data names not specified, but descriptions are.");
            // open selected file
            reader = File.OpenText(filePath);
            // data names not specified
            if (!namesSpecified)
                definitionData = null;  // no definitions, set to null
            else
            {
                if (definitionData == null)
                    definitionData = new InputOutputDataDefiniton();
                else
                {
                    definitionData.InputElementList.Clear();
                    definitionData.OutputElementList.Clear();
                }

                // split line of names
                string line;
                line = reader.ReadLine();
                string[] names = line.Split(';');
                if (names.Length == 1)
                    names = line.Split(',');

                string[] titles = null;
                string[] descriptions = null;
                string[] defaultValue = null;
                string[] minValue = null;
                string[] maxValue = null;
                string[] boundDefiner = null;

                if (titleSpecified)
                {
                    // split line of descriptions
                    line = reader.ReadLine();
                    titles = line.Split(';');
                    if (titles.Length == 1)
                        titles = line.Split(',');
                }
                // descriptions sbecified
                if (descriptionSpecified)
                {
                    // split line of descriptions
                    line = reader.ReadLine();
                    descriptions = line.Split(';');
                    if (descriptions.Length == 1)
                        descriptions = line.Split(',');
                    // split line of defaultValues
                    line = reader.ReadLine();
                    defaultValue = line.Split(';');
                    if (defaultValue.Length == 1)
                        defaultValue = line.Split(',');
                    // split line of bound definer
                    line = reader.ReadLine();
                    boundDefiner = line.Split(';');
                    if (boundDefiner.Length == 1)
                        boundDefiner = line.Split(',');
                    // split line of minimum value
                    line = reader.ReadLine();
                    minValue = line.Split(';');
                    if (minValue.Length == 1)
                        minValue = line.Split(',');
                    // split line of maximum value
                    line = reader.ReadLine();
                    maxValue = line.Split(';');
                    if (maxValue.Length == 1)
                        maxValue = line.Split(',');
                }

                if (names == null)
                    throw new InvalidDataException("Could not read names.");
                if (names.Length != inputLenght + outputLenght)
                    throw new InvalidDataException("Wrong number of names in data definition, "
                        + names.Length + "instead of " + (inputLenght + outputLenght).ToString());
                if (titleSpecified)
                {
                    if (titles == null)
                        throw new InvalidDataException("Could not read descriptions.");
                    if (titles.Length != inputLenght + outputLenght)
                    {
                        throw new InvalidDataException("Wrong number of definitions in data definition.");
                        throw new InvalidDataException("Description lenght " + titles.Length + " instead of " + (inputLenght + outputLenght).ToString());
                    }
                }
                if (descriptionSpecified)
                {
                    if (descriptions == null)
                        throw new InvalidDataException("Could not read descriptions.");
                    if (descriptions.Length != inputLenght + outputLenght)
                    {
                        throw new InvalidDataException("Wrong number of definitions in data definition.");
                        throw new InvalidDataException("Description lenght " + descriptions.Length + " instead of " + (inputLenght + outputLenght).ToString());
                    }
                    if (defaultValue.Length != inputLenght + outputLenght)
                        throw new InvalidDataException("Default value lenght " + defaultValue.Length + " instead of " + (inputLenght).ToString());
                    if (boundDefiner.Length != inputLenght + outputLenght)
                        throw new InvalidDataException("Bound definition lenght " + boundDefiner.Length + " instead of " + (inputLenght).ToString());
                    if (minValue.Length != inputLenght + outputLenght)
                        throw new InvalidDataException("Minimum value lenght " + minValue.Length + " instead of " + (inputLenght).ToString());
                    if (maxValue.Length != inputLenght + outputLenght)
                        throw new InvalidDataException("Maximum value lenght " + maxValue.Length + " instead of " + (inputLenght).ToString());
                }

                // add definition data (name and description)
                for (int i = 0; i < inputLenght; i++)
                {
                    double tempDefaultValue = 0.0;
                    double tempMinValue = 0.0;
                    double tempMaxtValue = 0.0;
                    bool tempBoundDefinertValue = false;
                    string name = null, description = null, title = null;
                    if (names != null)
                        name = names[i];
                    if (titles != null)
                        title = titles[i];
                    if (descriptions != null)
                        description = descriptions[i];
                    if (defaultValue != null)
                        tempDefaultValue = double.Parse(defaultValue[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                    if (boundDefiner != null)
                    {
                        int tmp;
                        tmp = int.Parse(boundDefiner[i]);
                        if (tmp == 1)
                            tempBoundDefinertValue = true;
                        else if (tmp == 0)
                            tempBoundDefinertValue = false;
                    }
                    if (minValue != null)
                        tempMinValue = double.Parse(minValue[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                    if (maxValue != null)
                        tempMaxtValue = double.Parse(maxValue[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                    InputElementDefinition def = new InputElementDefinition(i, name, name /* $$$$ should be title */, description);
                    def.DefaultValue = tempDefaultValue;
                    def.BoundsDefined = tempBoundDefinertValue;
                    def.MinimalValue = tempMinValue;
                    def.MaximalValue = tempMaxtValue;
                    definitionData.AddInputElement(def);
                }
                for (int i = 0; i < outputLenght; i++)
                {
                    string name = null, description = null, title = null;
                    if (names != null)
                        name = names[i + inputLenght];
                    if (titles != null)
                        title = titles[i + inputLenght];
                    if (descriptions != null)
                        description = descriptions[i + inputLenght];

                    definitionData.AddOutputElement(new OutputElementDefinition(i, name, title, description));
                }
            }
            // read the sampled data  
            while ((str = reader.ReadLine()) != null)
            {
                // split sampledData 
                string[] strs = str.Split(';');
                if (strs.Length == 1)
                    strs = str.Split(',');

                // allocate data array      
                variables = strs.Length;

                // parse sampled data     
                double[] tempLineData = new double[inputLenght + outputLenght];
                int j1;
                for (j1 = 0; j1 < variables; j1++)
                {
                    if (strs.Length != (inputLenght + outputLenght))
                    {
                        throw new InvalidDataException("Sampled data out of range in line " + data.Count);
                    }
                    tempLineData[j1] = double.Parse(strs[j1], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
                data.Add(tempLineData);
            }

            // add sampled data
            if (sampledData == null)
                sampledData = new SampledDataSet();
            sampledData.Clear();
            sampledData.InputLength = inputLenght;
            sampledData.OutputLength = outputLenght;

            for (int i = 0; i < data.Count; ++i)
            {
                double[] row = data[i];
                Vector inputData = new Vector(inputLenght);
                for (int j = 0; j < inputLenght; ++j)
                {
                    inputData[j] = row[j];
                }
                Vector outputData = new Vector(outputLenght);
                for (int k = 0; k < outputLenght; k++)
                {
                    outputData[k] = row[k + inputLenght];
                }
                sampledData.AddElement(new SampledDataElement(inputData, outputData));
            }
        }

        /// <summary>Loads sampled data and definition data from single CSV file.</summary>
        /// <param name="filePath">Path to the file where sampled data are saved.</param>
        /// <param name="inputLenght">Lenght of input parameters.</param>
        /// <param name="outputLenght">Lenght of output parameters.</param>
        /// <param name="namesSpecified">Flag if names are specified in the file.</param>
        /// <param name="titleSpecified">Determines whether title is specified in the file.</param>
        /// <param name="descriptionSpecified">Flag if descriptions are specified in the file.</param>
        /// <param name="sampledData">Sampled data set.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Apr11, June24;
        public static void LoadSampledDataCSV(string filePath, int inputLenght, int outputLenght,
           bool namesSpecified, bool titleSpecified, bool descriptionSpecified,
           ref SampledDataSet sampledData, ref InputOutputDataDefiniton definitionData)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("File path not specified.");
            // variables 
            int variables = 0;
            string str = null;
            StreamReader reader = null;
            List<double[]> data = new List<double[]>();

            // data names not sbecified, descriptions are specified 
            if ((!namesSpecified && descriptionSpecified) || (!namesSpecified && titleSpecified)) 
                throw new ArgumentException("Data names not specified, but descriptions or titles are.");
            // open selected file
            reader = File.OpenText(filePath);
            // data names not specified
            if (!namesSpecified)
                definitionData = null;  // no definitions, set to null
            else
            {
                if (definitionData == null)
                    definitionData = new InputOutputDataDefiniton();
                else
                {
                    definitionData.InputElementList.Clear();
                    definitionData.OutputElementList.Clear();
                }

                // split line of names
                string line;
                line = reader.ReadLine();
                string[] names = line.Split(';');
                if (names.Length == 1)
                    names = line.Split(',');


                string[] titles = null;
                // title specified
                if (titleSpecified)
                {
                    // split line of titles
                    line = reader.ReadLine();
                    titles = line.Split(';');
                    if (titles.Length == 1)
                        titles = line.Split(',');
                }

                string[] descriptions = null;
                // descriptions specified
                if (descriptionSpecified)
                {
                    // split line of descriptions
                    line = reader.ReadLine();
                    descriptions = line.Split(';');
                    if (descriptions.Length == 1)
                        descriptions = line.Split(',');
                }

                if (names == null)
                    throw new InvalidDataException("Could not read names.");
                if (names.Length != inputLenght + outputLenght)
                    throw new InvalidDataException("Wrong number of names in data definition, "
                        + names.Length + "instead of " + (inputLenght + outputLenght).ToString());
                if (titleSpecified)
                {
                    if (titles == null)
                        throw new InvalidDataException("Could not read titles.");
                    if (titles.Length != inputLenght + outputLenght)
                    {
                        throw new InvalidDataException("Wrong number of definitions in data titles.");
                        throw new InvalidDataException("Titles lenght " + titles.Length + " instead of " + (inputLenght + outputLenght).ToString());
                    }
                }
                if (descriptionSpecified)
                {
                    if (descriptions == null)
                        throw new InvalidDataException("Could not read descriptions.");
                    if (descriptions.Length != inputLenght + outputLenght)
                    {
                        throw new InvalidDataException("Wrong number of definitions in data definition.");
                        throw new InvalidDataException("Description lenght " + descriptions.Length + " instead of " + (inputLenght + outputLenght).ToString());
                    }
                }

                // add definition data to SampledDataDefinition (name, title and description)
                for (int i = 0; i < inputLenght; i++)
                {
                    string name = null, title = null, description = null;
                    if (names != null)
                        name = names[i];
                    if (titles != null)
                        title = descriptions[i];
                    if (descriptions != null)
                        description = descriptions[i];
                    definitionData.AddInputElement(new InputElementDefinition(i, name, title /* $$$$ should be title */, description));
                }
                for (int i = 0; i < outputLenght; i++)
                {
                    string name = null, title = null, description = null;
                    if (names != null)
                        name = names[i + inputLenght];
                    if (titles != null)
                        title = titles[i + inputLenght];
                    if (descriptions != null)
                        description = descriptions[i + inputLenght];

                    definitionData.AddOutputElement(new OutputElementDefinition(i, name, title, description));
                }
            }
            // read the sampled data
            while ((str = reader.ReadLine()) != null)
            {
                // split sampledData 
                string[] strs = str.Split(';');
                if (strs.Length == 1)
                    strs = str.Split(',');

                // allocate data array      
                variables = strs.Length;

                // parse sampled data     
                double[] tempLineData = new double[inputLenght + outputLenght];
                int j1;
                for (j1 = 0; j1 < variables; j1++)
                {
                    if (strs.Length != (inputLenght + outputLenght))
                    {
                        throw new ArgumentException("Sampled data out of range in line " + data.Count);
                    }
                    tempLineData[j1] = double.Parse(strs[j1]);
                }
                data.Add(tempLineData);
            }

            // add sampled data to SampledDataSet
            if (sampledData == null)
                sampledData = new SampledDataSet();
            sampledData.Clear();
            sampledData.InputLength = inputLenght;
            sampledData.OutputLength = outputLenght;

            for (int i = 0; i < data.Count; ++i)
            {
                double[] row = data[i];
                Vector inputData = new Vector(inputLenght);
                for (int j = 0; j < inputLenght; ++j)
                {
                    inputData[j] = row[j];
                }
                Vector outputData = new Vector(outputLenght);
                for (int k = 0; k < outputLenght; k++)
                {
                    outputData[k] = row[k + inputLenght];
                }
                sampledData.AddElement(new SampledDataElement(inputData, outputData));
            }
        }

        /// <summary>Loads definition data from CSV file.</summary>
        /// <param name="filePath">Path to the file where definition data are saved.</param>
        /// <param name="inputLenght">Lenght of input parameters.</param>
        /// <param name="outputLenght">Lenght of output parameters.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Mar11; June24;
        public static void LoadDefinitionDataCSV(string filePath, int inputLenght, int outputLenght,
            ref InputOutputDataDefiniton definitionData)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("File path not specified.");

            if (definitionData == null)
                definitionData = new InputOutputDataDefiniton();

            // variables            
            string line = null; ;
            StreamReader reader = null;
            string[] lineIndex = null;

            string[] inputNames = new string[inputLenght];
            string[] inputTitles = new string[inputLenght];
            string[] inputDescriptions = new string[inputLenght];
            double[] defaultValues = new double[inputLenght];
            bool[] bounds = new bool[inputLenght];
            double[] minValues = new double[inputLenght];
            double[] maxValues = new double[inputLenght];
            string[] outputNames = new string[outputLenght];
            string[] outputTitles = new string[outputLenght];
            string[] outputDescriptions = new string[outputLenght];

            // open selected file
            reader = File.OpenText(filePath);

            // split 1st line of names
            line = reader.ReadLine();
            lineIndex = line.Split(';');
            if (lineIndex.Length == 1)
                lineIndex = line.Split(',');

            // check if input index is specified
            if ((lineIndex[0] == "Input") || (lineIndex[0] == "Input"))
            {
                //read input definition from CSV file
                int n = 0;
                do
                {
                    line = reader.ReadLine();
                    lineIndex = line.Split(';');
                    if (lineIndex.Length == 1)
                        lineIndex = line.Split(',');

                    if (lineIndex[0] == "name" || lineIndex[0] == "Name")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                inputNames[i - 1] = lineIndex[i];
                        }
                        continue;
                    }
                    if (lineIndex[0] == "title" || lineIndex[0] == "Title")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                inputTitles[i - 1] = lineIndex[i];
                        }
                        continue;
                    }
                    if (lineIndex[0] == "description" || lineIndex[0] == "Description")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                inputDescriptions[i - 1] = lineIndex[i];
                        }
                        continue;
                    }
                    if (lineIndex[0] == "default" || lineIndex[0] == "Default")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                defaultValues[i - 1] = double.Parse(lineIndex[i]);
                        }
                        continue;
                    }
                    if (lineIndex[0] == "bounds" || lineIndex[0] == "Bounds")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                            {
                                int tmp;
                                tmp = int.Parse(lineIndex[i]);
                                if (tmp == 1)
                                    bounds[i - 1] = true;
                                else if (tmp == 0)
                                    bounds[i - 1] = false;
                            }
                        }
                        continue;
                    }
                    if (lineIndex[0] == "min" || lineIndex[0] == "Min")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                minValues[i - 1] = double.Parse(lineIndex[i]);
                        }
                        continue;
                    }
                    if (lineIndex[0] == "max" || lineIndex[0] == "Max")
                    {
                        for (int i = 1; i < inputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                maxValues[i - 1] = double.Parse(lineIndex[i]);
                        }
                        continue;
                    }
                    n++;
                }
                while ((lineIndex[0] != "Output") && (lineIndex[0] != "output") && (n < 5));
            }
            else
                throw new InvalidDataException("Input index not specified.");

            // check if output index is specified
            if ((lineIndex[0] == "Output") || (lineIndex[0] == "output"))
            {
                //read output definition from CSV file
                do
                {
                    //line = reader.ReadLine();
                    lineIndex = line.Split(';');
                    if (lineIndex.Length == 1)
                        lineIndex = line.Split(',');

                    if (lineIndex[0] == "name" || lineIndex[0] == "Name")
                    {
                        for (int i = 1; i < outputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                outputNames[i - 1] = lineIndex[i];
                        }
                    }
                    if (lineIndex[0] == "title" || lineIndex[0] == "Title")
                    {
                        for (int i = 1; i < outputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                outputTitles[i - 1] = lineIndex[i];
                        }
                    }
                    if (lineIndex[0] == "description" || lineIndex[0] == "Description")
                    {
                        for (int i = 1; i < outputLenght + 1; i++)
                        {
                            if (lineIndex != null)
                                outputDescriptions[i - 1] = lineIndex[i];
                        }
                    }
                }
                while ((line = reader.ReadLine()) != null);
            }
            else
                throw new InvalidDataException("Output index not specified.");

            // write input definition
            for (int i = 0; i < inputLenght; i++)
            {
                string inputName = null;
                string inputTitle = null;
                string inputDescription = null;
                double defaultValue = 0.0;
                bool bound = false;
                double minValue = 0.0;
                double maxValue = 0.0;

                inputName = inputNames[i];
                inputTitle = inputTitles[i];
                inputDescription = inputDescriptions[i];
                defaultValue = defaultValues[i];
                bound = bounds[i];
                minValue = minValues[i];
                maxValue = maxValues[i];

                InputElementDefinition def = new InputElementDefinition(inputName, inputTitle, inputDescription);
                def.DefaultValue = defaultValue;
                def.BoundsDefined = bound;
                def.MinimalValue = minValue;
                def.MaximalValue = maxValue;
                definitionData.AddInputElement(def);
            }

            // write output definition
            for (int i = 0; i < outputLenght; i++)
            {
                string outputName = null;
                string outputTitle = null;
                string outputDescription = null;

                outputName = outputNames[i];
                outputTitle = outputTitles[i];
                outputDescription = outputDescriptions[i];

                definitionData.AddOutputElement(new OutputElementDefinition(outputName, outputTitle, outputDescription));
            }
        }

        /// <summary>Saves sampled data and Definition data to single CSV file.</summary>
        /// <param name="filePath">Path to the file where sampled data will be saved.</param>
        /// <param name="sampledData">Sampled data set.</param>
        /// <param name="namesSpecified">Flag if names will be written in the file.</param>
        /// <param name="titleSpecified">Whether title will be written.</param>
        /// <param name="descriptionSpecified">Flag if descriptions (descriptions, defaultValue, boundDefiner, minValue, maxValue) will be written in the file.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Mar11; June27;
        public static void SaveSampledDataCSVinOneLine(string filePath, SampledDataSet sampledData,
            bool namesSpecified, bool titleSpecified, bool descriptionSpecified, InputOutputDataDefiniton definitionData)
        {
            int inputVariables;
            int outputVariables;
            int numSamples;
            inputVariables = sampledData.InputLength;
            outputVariables = sampledData.OutputLength;
            numSamples = sampledData.Length;
            // data names not sbecified, descriptions are specified
            if (!namesSpecified && descriptionSpecified)
                throw new ArgumentException("Data names not specified, but descriptions are.");

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                // data names not specified
                if (!namesSpecified)
                    definitionData = null;
                else
                {
                    string[] name = new string[inputVariables + outputVariables];
                    string[] title = new string[inputVariables + outputVariables];
                    string[] description = new string[inputVariables + outputVariables];
                    double[] defaultValue = new double[inputVariables];
                    bool[] boundsDefined = new bool[inputVariables];
                    double[] minimalValue = new double[inputVariables];
                    double[] maximalValue = new double[inputVariables];
                    // write data names
                    for (int i = 0; i < inputVariables; i++)
                    {
                        name[i] = definitionData.GetInputElement(i).Name;
                        writer.Write(name[i] + ";");
                    }
                    for (int j = 0; j < outputVariables; j++)
                    {
                        name[j] = definitionData.GetOutputElement(j).Name;
                        writer.Write(name[j]);
                        if (j < outputVariables - 1)
                            writer.Write(";");
                    }
                    writer.WriteLine();
                    if (titleSpecified)
                    {
                        // write titles
                        for (int i = 0; i < inputVariables; i++)
                        {
                            title[i] = definitionData.GetInputElement(i).Title;
                            writer.Write(title[i] + ";");
                        }
                        for (int j = 0; j < outputVariables; j++)
                        {
                            title[j] = definitionData.GetOutputElement(j).Title;
                            writer.Write(title[j]);
                            if (j < outputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                    }
                    // data descriptions specified
                    if (descriptionSpecified)
                    {
                        // write descriptions data 
                        for (int i = 0; i < inputVariables; i++)
                        {
                            description[i] = definitionData.GetInputElement(i).Description;
                            writer.Write(description[i] + ";");
                        }
                        for (int j = 0; j < outputVariables; j++)
                        {
                            description[j] = definitionData.GetOutputElement(j).Description;
                            writer.Write(description[j]);
                            if (j < outputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                        for (int i = 0; i < inputVariables; i++)
                        {
                            defaultValue[i] = definitionData.GetInputElement(i).DefaultValue;
                            writer.Write(Convert.ToString(defaultValue[i]));
                            if (i < inputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                        for (int i = 0; i < inputVariables; i++)
                        {
                            boundsDefined[i] = definitionData.GetInputElement(i).BoundsDefined;
                            writer.Write(Convert.ToInt16(boundsDefined[i]));
                            if (i < inputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                        for (int i = 0; i < inputVariables; i++)
                        {
                            minimalValue[i] = definitionData.GetInputElement(i).MinimalValue;
                            writer.Write(Convert.ToString(minimalValue[i]));
                            if (i < inputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                        for (int i = 0; i < inputVariables; i++)
                        {
                            maximalValue[i] = definitionData.GetInputElement(i).MaximalValue;
                            writer.Write(Convert.ToString(maximalValue[i]));
                            if (i < inputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                    }
                }
                // write data
                for (int i = 0; i < numSamples; i++)
                {
                    IVector inputData = sampledData.GetInputParameters(i);

                    for (int j = 0; j < inputVariables; j++)
                    {
                        Console.Write(inputData[j] + ";");
                        writer.Write(inputData[j] + ";");
                    }
                    IVector outputData = sampledData.GetOutputValues(i);
                    for (int k = 0; k < outputVariables; k++)
                    {
                        Console.Write(outputData[k]);
                        writer.Write(outputData[k]);
                        if (k < outputVariables - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                }
                writer.Close();
            }
        }

        /// <summary>Saves sampled data and Definition data to single CSV file.</summary>
        /// <param name="filePath">Path to the file where sampled data will be saved.</param>
        /// <param name="sampledData">Sampled data set.</param>
        /// <param name="namesSpecified">Flag if names will be written in the file.</param>
        /// <param name="titlesSpecified">Whether title will be written to the file.</param>
        /// <param name="descriptionSpecified">Flag if descriptions will be written in the file.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Mar11; June27;
        public static void SaveSampledDataCSV(string filePath, SampledDataSet sampledData,
            bool namesSpecified, bool titlesSpecified, bool descriptionSpecified, InputOutputDataDefiniton definitionData)
        {
            if (sampledData == null)
                throw new ArgumentNullException("No data in sampled data block.");

            int inputVariables;
            int outputVariables;
            int numSamples;
            inputVariables = sampledData.InputLength;
            outputVariables = sampledData.OutputLength;
            numSamples = sampledData.Length;
            // data names not sbecified, descriptions are specified
            if ((!namesSpecified && descriptionSpecified) || (!namesSpecified && titlesSpecified))
                throw new ArgumentException("Data names not specified, but descriptions or titles are.");

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                // data names not specified
                if (!namesSpecified)
                    definitionData = null;
                else
                {
                    string[] name = new string[inputVariables + outputVariables];
                    string[] title = new string[inputVariables + outputVariables];
                    string[] description = new string[inputVariables + outputVariables];
                    // write data names
                    for (int i = 0; i < inputVariables; i++)
                    {
                        name[i] = definitionData.GetInputElement(i).Name;
                        writer.Write(name[i] + ";");
                    }
                    for (int j = 0; j < outputVariables; j++)
                    {
                        name[j] = definitionData.GetOutputElement(j).Name;
                        writer.Write(name[j]);
                        if (j < outputVariables - 1)
                            writer.Write(";");
                    }
                    writer.WriteLine();
                    if (titlesSpecified)
                    {
                        // write data titles
                        for (int i = 0; i < inputVariables; i++)
                        {
                            title[i] = definitionData.GetInputElement(i).Title;
                            writer.Write(title[i] + ";");
                        }
                        for (int j = 0; j < outputVariables; j++)
                        {
                            title[j] = definitionData.GetOutputElement(j).Title;
                            writer.Write(title[j]);
                            if (j < outputVariables - 1)
                                writer.Write(";");
                        }
                    }
                    // data descriptions specified
                    if (descriptionSpecified)
                    {
                        // write descriptions data 
                        for (int i = 0; i < inputVariables; i++)
                        {
                            description[i] = definitionData.GetInputElement(i).Description;
                            writer.Write(description[i] + ";");
                        }
                        for (int j = 0; j < outputVariables; j++)
                        {
                            description[j] = definitionData.GetOutputElement(j).Description;
                            writer.Write(description[j]);
                            if (j < outputVariables - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                    }
                }
                // write data
                for (int i = 0; i < numSamples; i++)
                {
                    IVector inputData = sampledData.GetInputParameters(i);
                    for (int j = 0; j < inputVariables; j++)
                    {
                        Console.Write(inputData[j] + ";");
                        writer.Write(inputData[j] + ";");
                    }
                    IVector outputData = sampledData.GetOutputValues(i);
                    for (int k = 0; k < outputVariables; k++)
                    {
                        Console.Write(outputData[k]);
                        writer.Write(outputData[k]);
                        if (k < outputVariables - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                }
                writer.Close();
            }
        }

        /// <summary>Saves definition data to CSV file.</summary>
        /// <param name="filePath">Path to the file where definition data will be saved.</param>
        /// <param name="definitionData">Definition data set.</param>
        /// $A Tako78 Mar11; June27;
        public static void SaveDefinitionDataCSV(string filePath, InputOutputDataDefiniton definitionData)
        {
            int inputLenght;
            int outputLenght;

            inputLenght = definitionData.InputLength;
            outputLenght = definitionData.OutputLength;

            string[] inputNames = new string[inputLenght];
            string[] inputTitles = new string[inputLenght];
            string[] inputDescriptions = new string[inputLenght];
            double[] defaultValues = new double[inputLenght];
            bool[] bounds = new bool[inputLenght];
            double[] minValues = new double[inputLenght];
            double[] maxValues = new double[inputLenght];
            string[] outputNames = new string[outputLenght];
            string[] outputTitles = new string[outputLenght];
            string[] outputDescriptions = new string[outputLenght];

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                if (definitionData == null)
                    throw new ArgumentNullException("No data in definition block.");
                else
                {
                    Console.WriteLine("Input");
                    writer.Write("Input");
                    writer.WriteLine();
                    // write data input names
                    Console.Write("Name;");
                    writer.Write("Name;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        inputNames[i] = definitionData.GetInputElement(i).Name;
                        Console.Write(inputNames[i]);
                        writer.Write(inputNames[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data input Titles
                    Console.Write("Title;");
                    writer.Write("Title;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        inputTitles[i] = definitionData.GetInputElement(i).Title;
                        Console.Write(inputNames[i]);
                        writer.Write(inputNames[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data input descriptions
                    Console.Write("Description;");
                    writer.Write("Description;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        inputDescriptions[i] = definitionData.GetInputElement(i).Description;
                        Console.Write(inputDescriptions[i]);
                        writer.Write(inputDescriptions[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data default value
                    Console.Write("Default;");
                    writer.Write("Default;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        defaultValues[i] = definitionData.GetInputElement(i).DefaultValue;
                        Console.Write(defaultValues[i]);
                        writer.Write(defaultValues[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data bound
                    Console.Write("Bounds;");
                    writer.Write("Bounds;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        bounds[i] = definitionData.GetInputElement(i).BoundsDefined;
                        if (i < inputLenght - 1)
                        {
                            if (bounds[i] == true)
                            {
                                Console.Write("1");
                                writer.Write("1");
                            }
                            else if (bounds[i] == false)
                            {
                                Console.Write("0");
                                writer.Write("0");
                            }
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data min value
                    Console.Write("Min;");
                    writer.Write("Min;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        minValues[i] = definitionData.GetInputElement(i).MinimalValue;
                        Console.Write(minValues[i]);
                        writer.Write(minValues[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data max value
                    Console.Write("Max;");
                    writer.Write("Max;");
                    for (int i = 0; i < inputLenght; i++)
                    {
                        maxValues[i] = definitionData.GetInputElement(i).MaximalValue;
                        Console.Write(maxValues[i]);
                        writer.Write(maxValues[i]);
                        if (i < inputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    Console.WriteLine("Output");
                    writer.Write("Output");
                    writer.WriteLine();
                    // write data output name
                    Console.Write("Name;");
                    writer.Write("Name;");
                    for (int i = 0; i < outputLenght; i++)
                    {
                        outputNames[i] = definitionData.GetInputElement(i).Name;
                        Console.Write(outputNames[i]);
                        writer.Write(outputNames[i]);
                        if (i < outputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data output titles
                    Console.Write("Title;");
                    writer.Write("Title;");
                    for (int i = 0; i < outputLenght; i++)
                    {
                        outputTitles[i] = definitionData.GetInputElement(i).Title;
                        Console.Write(outputTitles[i]);
                        writer.Write(outputTitles[i]);
                        if (i < outputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    // write data output description
                    Console.Write("Description;");
                    writer.Write("Description;");
                    for (int i = 0; i < outputLenght; i++)
                    {
                        outputDescriptions[i] = definitionData.GetInputElement(i).Description;
                        Console.Write(outputDescriptions[i]);
                        writer.Write(outputDescriptions[i]);
                        if (i < outputLenght - 1)
                        {
                            Console.Write(";");
                            writer.Write(";");
                        }
                    }
                    Console.WriteLine();
                }
                writer.Close();
            }
        }

        /// <summary>Loads sampled data and Definition data from multible CSV files.
        /// Sampled data consist of one output and multiple input parameters.
        /// Input parameters are the same in all files, output parameter are different.</summary>
        /// <param name="result">Sampled data set with combined outputs.</param>
        /// <param name="individualSets">Different sampled data sets with the same inputs and different outputs.</param>
        /// $A Tako78 Mar11;
        public static void SampledDataCombineOutputs(ref SampledDataSet result, params SampledDataSet[] individualSets)
        {
            if (individualSets == null)
                throw new ArgumentNullException("Sampled data sets to be combined are not specified.");
            if (individualSets.Length < 1)
                throw new ArgumentException("No sampled data sets to combine (array length is 0).");

            int numSets = individualSets.Length;
            SampledDataSet firstSet = individualSets[0];
            if (firstSet == null)
                throw new ArgumentException("Sampled data set No. 0 not specified (null reference).");
            int numElements = firstSet.Length;
            int inputLength = firstSet.InputLength;
            int outputLength = firstSet.OutputLength; ;
            for (int i = 0; i < numSets; ++i)
            {
                SampledDataSet currentSet = individualSets[i];
                if (currentSet.Length != numElements)
                    throw new ArgumentException("Inconsistent sampled data set No. " + i + ": inconsistent number of elements.");
            }
            if (result == null)
                result = new SampledDataSet();

            SampledDataSet tmpSampledData = new SampledDataSet();
            IVector inputData = null;
            IVector tmpOutputData = null;
            IVector outputData = new Vector(numSets);

            //Read input sampled data from first file         
            tmpSampledData = individualSets[0];
            //Read output sampled data from all files
            for (int i = 0; i < tmpSampledData.Length; ++i)
            {
                inputData = tmpSampledData.GetInputParameters(i);
                for (int j = 0; j < numSets; j++)
                {
                    Vector tmp = new Vector(numSets);
                    tmpSampledData = individualSets[j];
                    tmpOutputData = tmpSampledData.GetOutputValues(i);
                    tmp[j] = tmpOutputData[0];
                    outputData[j] = tmp[j];
                }
                Console.WriteLine("Input");
                Console.WriteLine(inputData);
                Console.WriteLine("Output");
                Console.WriteLine(outputData);
                result.AddElement(new SampledDataElement(inputData, outputData));
            }
        }

        #endregion


        #region DuplicatesAndNullStatic

        /// <summary>Returns number of null elements of the specified sampled data set.</summary>
        /// <param name="sampledDataSet">Sampled set for which number of null elements is returned.</param>
        /// $A Igor Feb12;
        public static int GetNumNullElemets(SampledDataSet sampledDataSet)
        {
            if (sampledDataSet == null)
                return 0;
            int ret = 0;
            for (int i = 0; i < sampledDataSet.Length; ++i)
            {
                if (sampledDataSet[i] == null)
                    ++ret;
            }
            return ret;
        }

        /// <summary>Returns the number of elements of the specified sampled data set with duplicated input parameters. 
        /// <para>Vectors of input parameters are considered the same if both are null or all components 
        /// are the same (i.e., comparison is performed component-wise rather than by reference).</para>
        /// <para>elements that are null are not counted as duplicates.</para>
        /// <para>For each group of n elements with the same input parameters, n-1 is added to the returned number.</para></summary>
        /// <param name="sampledSet">Sampled data set for which number of duplicated input parameters is returned.</param>
        /// $A Igor Feb12;
        public static int GetNumInputDuplicates(SampledDataSet sampledSet)
        {
            int ret = 0;
            List<SampledDataElement> elements = sampledSet.ElementList;
            int num = sampledSet.Length; // number of elements
            if (num > 1)
            {
                // We shall assign indices to elements of the list in order to later sort the list in the original order.
                int[] originalIndices = new int[num];
                for (int i = 0; i < num; ++i)
                {
                    if (elements[i] != null)
                    {
                        originalIndices[i] = elements[i].Index;
                        // We must remember the original indices in order to restore them later:
                        elements[i].Index = i;
                    }
                }
                // Now sort vectors in the list by any regular comparison method on input parameters:
                elements.Sort(delegate(SampledDataElement el1, SampledDataElement el2)
                {
                    if (el1 == null)
                    {
                        if (el2 == null) return 0;
                        return -1;
                    }
                    else if (el2 == null)
                        return 1;
                    return VectorBase.Compare(el1.InputParameters, el2.InputParameters);
                }
                );
                // Count the duplicates:
                for (int i = num - 2; i >= 0; --i)
                {
                    if (elements[i] != null && elements[i + 1] != null)
                    {
                        if (VectorBase.Compare(elements[i].InputParameters, elements[i + 1].InputParameters) == 0)
                            ++ret;
                    }
                }
                // Sort elements by the assigned indices in order to restore the original order:
                elements.Sort(delegate(SampledDataElement el1, SampledDataElement el2)
                {
                    if (el1 == null)
                    {
                        if (el2 == null) return 0;
                        return -1;
                    }
                    else if (el2 == null)
                        return 1;
                    return Comparer<int>.Default.Compare(el1.Index, el2.Index);
                });
                // Restore the original indices:
                for (int i = 0; i < elements.Count; ++i)
                {
                    if (elements[i] != null)
                        elements[i].Index = originalIndices[elements[i].Index];
                }
            }
            return ret;
        }  // GetNumInputDuplicates

        /// <summary>Removes elements with duplicated input parameters, leaving only a single element with specified input parameters.
        /// Elements that are null are also removed. 
        /// <para>Vectors of input parameters are considered the same if both are null or all components 
        /// are the same (i.e., comparison is performed component-wise rather than by reference).</para></summary>
        /// <param name="sampledSet">Sampled data set from which elemets with duplicated input parameters are removed.</param>
        /// $A Igor Feb12;
        public static void RemoveInputDuplicates(SampledDataSet sampledSet)
        {
            List<SampledDataElement> elements = sampledSet.ElementList;
            int num = sampledSet.Length; // number of elements
            if (num > 1)
            {
                // Remark: In this method, null elements are removed. However, all further procedures are performed 
                // as if null elements remained in the list (for better reusability of code in other methods, but you can comment these parts if you want). 
                // First, remove eventual null elements: 
                for (int i = num - 1; i >= 0; --i)
                {
                    if (elements[i] == null)
                        elements.RemoveAt(i);
                }
                // We shall assign indices to elements of the list in order to sort the list later in the original order.
                // Therefore, we must remember the original indices in order to restore them:
                int[] originalIndices = new int[num];
                for (int i = 0; i < num; ++i)
                {
                    if (elements[i] != null)
                    {
                        originalIndices[i] = elements[i].Index;
                        elements[i].Index = i;
                    }
                }
                // Now sort vectors in the list by any regular comparison method on input parameters:
                elements.Sort(delegate(SampledDataElement el1, SampledDataElement el2)
                {
                    if (el1 == null)
                    {
                        if (el2 == null) return 0;
                        return -1;
                    }
                    else if (el2 == null)
                        return 1;
                    return VectorBase.Compare(el1.InputParameters, el2.InputParameters);
                }
                );
                // Remove the duplicates:
                for (int i = num - 2; i >= 0; --i)
                {
                    if (elements[i] != null && elements[i + 1] != null)
                    {
                        if (VectorBase.Compare(elements[i].InputParameters, elements[i + 1].InputParameters) == 0)
                            elements.RemoveAt(i);
                    }
                }
                // Finally, sort by the assigned indices (that correspond to the original positions in the list) 
                elements.Sort(delegate(SampledDataElement el1, SampledDataElement el2)
                {
                    if (el1 == null)
                    {
                        if (el2 == null) return 0;
                        return -1;
                    }
                    else if (el2 == null)
                        return 1;
                    return Comparer<int>.Default.Compare(el1.Index, el2.Index);
                });
                // Restore the original indices:
                for (int i = 0; i < elements.Count; ++i)
                {
                    if (elements[i] != null)
                        elements[i].Index = originalIndices[elements[i].Index];
                }
            }
        }  // RemoveInputDuplicates(...)

        #endregion DuplicatesAndNullStatic

        #endregion StaticMethods


        #region ExampleData


        /// <summary>Craates and returns a sample data set object where input parameters are 
        /// calculated randomly in a box-like domain, and output parameters are calculated by 
        /// quadratic functions with random coefficients.
        /// Domain where sampling points are generated is a cartesian product of intervals [-1, 1].</summary>
        /// <param name="inputLength">Dimension of input data (parameters).</param>
        /// <param name="outputLength">Number of output values.</param>
        /// <param name="numElements">Number of input/output pairs used as sampled data.</param>
        public static SampledDataSet CreateExampleLinear(int inputLength, int outputLength, int numElements)
        {
            return CreateExampleLinear(inputLength, outputLength, numElements,
                BoundingBox.Create(inputLength, -1.0, 1.0));
        }

        /// <summary>Craates and returns a sample data set object where input parameters are 
        /// calculated randomly in the specified box-like domain, and output parameters are calculated by 
        /// quadratic functions with random coefficients.</summary>
        /// <param name="inputLength">Dimension of input data (parameters).</param>
        /// <param name="outputLength">Number of output values.</param>
        /// <param name="numElements">Number of input/output pairs used as sampled data.</param>
        /// <param name="region">Bounding box defining the region in the space (of dimension inputLength)
        /// from which samples (input parameters) are taken randomly.</param>
        public static SampledDataSet CreateExampleLinear(int inputLength, int outputLength, int numElements,
            IBoundingBox region)
        {
            return CreateExampleLinear(inputLength, outputLength, numElements,
                region, RandomGenerator.Global);
        }

        /// <summary>Craates and returns a sample data set object where input parameters are 
        /// calculated randomly in the specified box-like domain, and output parameters are calculated by 
        /// quadratic functions with random coefficients.</summary>
        /// <param name="inputLength">Dimension of input data (parameters).</param>
        /// <param name="outputLength">Number of output values.</param>
        /// <param name="numElements">Number of input/output pairs used as sampled dataa.</param>
        /// <param name="region">Bounding box defining the region in the space (of dimension inputLength)
        /// from which samples (input parameters) are taken randomly.</param>
        /// <param name="rand">Random number generator that is used for sampling.</param>
        public static SampledDataSet CreateExampleLinear(int inputLength, int outputLength, int numElements,
            IBoundingBox region, IRandomGenerator rand)
        {
            SampledDataSet ret = new SampledDataSet(inputLength, outputLength);
            // IRandomGenerator rand = RandomGenerator.Global;
            // Create a table of random quadratic functions for calculation of outputs:
            IScalarFunctionUntransformed[] functions = new IScalarFunctionUntransformed[outputLength];
            for (int i = 0; i < outputLength; ++i)
            {
                IVector b = new Vector(inputLength);
                b.SetRandom(rand);
                double c = rand.NextDouble();
                IMatrix Q = new Matrix(inputLength, inputLength, 0.0 /* all elements are 0 to obtain linear functions */); ;
                functions[i] = new ScalarFunctionLinear(b, c);
            }
            ret = CreateExample(inputLength, outputLength, numElements,
                region, functions, rand);
            return ret;
        }

        // quadratic quadratic quadratic

        /// <summary>Craates and returns a sample data set object where input parameters are 
        /// calculated randomly in a box-like domain, and output parameters are calculated by 
        /// quadratic functions with random coefficients.
        /// Domain where sampling points are generated is a cartesian product of intervals [-1, 1].</summary>
        /// <param name="inputLength">Dimension of input data (parameters).</param>
        /// <param name="outputLength">Number of output values.</param>
        /// <param name="numElements">Number of input/output pairs used as sampled data.</param>
        public static SampledDataSet CreateExampleQuadratic(int inputLength, int outputLength, int numElements)
        {
            return CreateExampleQuadratic(inputLength, outputLength, numElements,
                BoundingBox.Create(inputLength, -1.0, 1.0));
        }

        /// <summary>Craates and returns a sample data set object where input parameters are 
        /// calculated randomly in the specified box-like domain, and output parameters are calculated by 
        /// quadratic functions with random coefficients.</summary>
        /// <param name="inputLength">Dimension of input data (parameters).</param>
        /// <param name="outputLength">Number of output values.</param>
        /// <param name="numElements">Number of input/output pairs used as sampled data.</param>
        /// <param name="region">Bounding box defining the region in the space (of dimension inputLength)
        /// from which samples (input parameters) are taken randomly.</param>
        public static SampledDataSet CreateExampleQuadratic(int inputLength, int outputLength, int numElements,
            IBoundingBox region)
        {
            return CreateExampleQuadratic(inputLength, outputLength, numElements,
                region, RandomGenerator.Global);
        }

        /// <summary>Craates and returns a sample data set object where input parameters are 
        /// calculated randomly in the specified box-like domain, and output parameters are calculated by 
        /// quadratic functions with random coefficients.</summary>
        /// <param name="inputLength">Dimension of input data (parameters).</param>
        /// <param name="outputLength">Number of output values.</param>
        /// <param name="numElements">Number of input/output pairs used as sampled data.</param>
        /// <param name="region">Bounding box defining the region in the space (of dimension inputLength)
        /// from which samples (input parameters) are taken randomly.</param>
        /// <param name="rand">Random number generator that is used for sampling.</param>
        public static SampledDataSet CreateExampleQuadratic(int inputLength, int outputLength, int numElements,
            IBoundingBox region, IRandomGenerator rand)
        {
            SampledDataSet ret = new SampledDataSet(inputLength, outputLength);
            // IRandomGenerator rand = RandomGenerator.Global;
            // Create a table of random quadratic functions for calculation of outputs:
            IScalarFunctionUntransformed[] functions = new IScalarFunctionUntransformed[outputLength];
            for (int i = 0; i < outputLength; ++i)
            {
                IMatrix Q = new Matrix(inputLength, inputLength);
                Q.SetRandom(rand);
                Matrix.SymmetricPartPlain(Q, Q);
                IVector b = new Vector(inputLength);
                b.SetRandom(rand);
                double c = rand.NextDouble();
                functions[i] = new ScalarFunctionQuadratic(Q, b, c);
                if (numElements < ScalarFunctionQuadratic.GetNumConstants(inputLength))
                {
                    Console.WriteLine();
                    Console.WriteLine("WARNING " + Environment.NewLine + "  in creating test samples on basis of quadratic function: ");
                    Console.WriteLine("  Number of samples " + numElements + " is not greater than number of constants");
                    Console.WriteLine("    that determine the function - " + ScalarFunctionQuadratic.GetNumConstants(inputLength));
                    Console.WriteLine();
                }
            }
            ret = CreateExample(inputLength, outputLength, numElements,
                region, functions, rand);
            return ret;
        }


        /// <summary>Craates and returns a sample data set object where input parameters are 
        /// calculated randomly in the specified box-like domain, and output parameters are calculated by 
        /// the specified scalar functions.</summary>
        /// <param name="inputLength">Dimension of input data (parameters).</param>
        /// <param name="outputLength">Number of output values.</param>
        /// <param name="numElements">Number of input/output pairs used as sampled data.</param>
        /// <param name="region">Bounding box defining the region in the space (of dimension inputLength)
        /// from which samples (input parameters) are taken randomly.</param>
        /// <param name="functions">Scalar-valued functions (of vector argument) that are applied to input parameters
        /// to produce output values of the sampled data.</param>
        /// <param name="rand">Random number generator that is used for sampling.</param>
        public static SampledDataSet CreateExample(int inputLength, int outputLength, int numElements,
            IBoundingBox region, IScalarFunctionUntransformed[] functions, IRandomGenerator rand)
        {
            SampledDataSet ret = new SampledDataSet(inputLength, outputLength);
            if (region == null)
                throw new ArgumentNullException("Bounding box defining input data domain is not specified (null reference).");
            if (region.Dimension != inputLength)
                throw new ArgumentException("Ivalid dimension of bounding box defining the input data domain: "
                    + region.Dimension + " instead of " + inputLength + ".");
            if (functions == null)
                throw new ArgumentNullException("Array of functions for calculation of sample outputs is not specified (null reference).");
            if (functions.Length != outputLength)
                throw new ArgumentException("Invalid number of function for calculateion of sample outputs: "
                    + functions.Length + " instead of " + outputLength + ".");
            if (rand == null)
                throw new ArgumentNullException("Random number generator for generation of sampling points not specified (null reference).");
            for (int i = 0; i < numElements; ++i)
            {
                IVector inputParameters = new Vector(inputLength);
                region.GetRandomPoint(ref inputParameters, rand);
                IVector outputValues = new Vector(outputLength);
                for (int k = 0; k < outputLength; ++k)
                    outputValues[k] = functions[k].Value(inputParameters);
                ret.AddElement(new SampledDataElement(inputParameters, outputValues));
            }
            return ret;
        }

        /// <summary>Craates and returns a sample object of the encompassing class.</summary>
        /// <param name="inputLength">Dimension of input data (parameters).</param>
        /// <param name="outputLength">Number of output values.</param>
        /// <param name="numElements">Number of input/output pairs used as sampled data.</param>
        public static SampledDataSet CreateExampleSimple(int inputLength, int outputLength, int numElements)
        {
            SampledDataSet ret = new SampledDataSet(inputLength, outputLength);
            ret.InputLength = inputLength;
            ret.OutputLength = ret.OutputLength;
            for (int i = 1; i < numElements; ++i)
            {
                IVector inpVec = new Vector(inputLength);
                IVector outVec = new Vector(outputLength);
                for (int j = 0; j < inputLength; ++j)
                {
                    inpVec[j] = (double)i + (double)j * 0.001;
                }
                for (int j = 0; j < outputLength; ++j)
                {
                    outVec[j] = 10.0 * (double)i + (double)j * 0.001;
                }
                SampledDataElement sampledElement = new SampledDataElement(inpVec, outVec);
                ret.AddElement(sampledElement);
            }
            return ret;
        }

        #endregion ExampleData


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Sampled data: ");
            sb.AppendLine("  Number of elements: " + this.Length);
            sb.AppendLine("  Data: ");
            if (ElementList == null)
                sb.AppendLine("  null.");
            else
            {
                for (int i = 0; i < ElementList.Count; ++i)
                {
                    sb.AppendLine("  Element " + i);
                    SampledDataElement element = ElementList[i];
                    if (element == null)
                        sb.AppendLine("    null.");
                    else
                    {
                        sb.AppendLine("  Input parameters: ");
                        if (element.InputParameters == null)
                            sb.AppendLine("    null.");
                        else
                            sb.AppendLine("  " + element.InputParameters.ToString());
                        sb.AppendLine("  Output values: ");
                        if (element.OutputValues == null)
                            sb.AppendLine("    null.");
                        else sb.AppendLine("  " + element.OutputValues.ToString());
                    }
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }


    }  // class SampledDataSet



}