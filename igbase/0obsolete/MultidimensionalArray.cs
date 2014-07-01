using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IG.Lib
{



#if false


    // TDO: Elaborate a bit.
    // Add more efficient operations for 2D and 3D array.


    /// <summary>Twodimensional array of elements.</summary>
    /// $A Igor xx;
    public class Array2D<ElementType> : MultiDimensionalArray<ElementType>
    {

        public Array2D(int d1, int d2)
            : base(true, new int[] {d1, d2})
        {  }


        /// <summary>Sets dimensions of the current multidimensional array.
        /// <para>Array of dimensions is copied rather than only setting reference to the array.</para></summary>
        /// <param name="dimensions">Dimensions to be set (array or a set of integer parameters).</param>
        public new void SetDimensions(params int[] dimensions)
        {
            if (dimensions.Length != 2)
                throw new ArgumentException("Number of dimensions specified is different than 2.");
            base.SetDimensions(dimensions);
        }

        /// <summary>Sets dimensions of the current multidimensional array.</summary>
        /// <param name="copyDimensions">If true then the array of dimensions is copied, otherwise a reference to the array is set directly.</param>
        /// <param name="dimensions">Dimensions to be set (array or a set of integer parameters).</param>
        public new void SetDimensions(bool copyDimensions, params int[] dimensions)
        {
            if (dimensions.Length != 2)
                throw new ArgumentException("Number of dimensions specified is different than 2.");
            base.SetDimensions(copyDimensions, dimensions);
        }

        /// <summary>Sets dimensions of the current dwodimensional array to the specified values.</summary>
        /// <param name="d1">First dimension.</param>
        /// <param name="d2">Second dimension.</param>
        public void SetDimensions(int d1, int d2)
        {
            base.SetDimensions(false, new int[] { d1, d2 });
        }


    }


    /// <summary>Threedimensional array of elements.</summary>
    /// $A Igor xx;
    public class Array3d<ElementType> : MultiDimensionalArray<ElementType>
    {

        public Array3d(int d1, int d2)
            : base(true, new int[] {d1, d2})
        {  }

    }




    /// <summary>Multidimensional array of elements.</summary>
    /// $A Igor xx;
    public class MultiDimensionalArray<ElementType>
    {

        private MultiDimensionalArray()
        {
            SetDimensions(null);
        }

        /// <summary>Constructs a multidimensional array with specified dimensions.
        /// <para>Array of dimensions is copied rather than only setting reference to the array.</para></summary>
        /// <param name="dimensions">Dimensions to be set (array or a set of integer parameters).</param>
        public MultiDimensionalArray(params int[] dimensions)
        {
            SetDimensions(dimensions);
        }


        /// <summary>Constructs a multidimensional array with the specified dimensions.</summary>
        /// <param name="copyDimensions">If true then the array of dimensions is copied, otherwise a reference to the array is set directly.</param>
        /// <param name="dimensions">Dimensions to be set (array or a set of integer parameters).</param>
        public MultiDimensionalArray(bool copyDimensions, params int[] dimensions)
        {
            SetDimensions(copyDimensions, dimensions);
        }


        protected ElementType[] _elements;

        protected int _numDimensions;

        /// <summary>Number of dimensions of the multidimensional array.</summary>
        public int NumDimensions
        {
            get { return _numDimensions; }
            protected set
            {
                _numDimensions = value;
            }
        }

        protected int _numElements;

        /// <summary>Number of elements of the mulltidimensional array.</summary>
        public int NumElements
        {
            get { return _numElements; }
            protected set { _numElements = value; }
        }

        protected int[] _dimensions;

        /// <summary>Dimensions of the multidimensional array.
        /// <para>Setter sets the dimensions directly, it does not copy them.</para></summary>
        protected int[] Dimensions
        {
            get { return _dimensions; }
            set
            {
                if (value == null)
                {
                    _dimensions = null;
                    _numDimensions = 0;
                    _numElements = 1;
                    _elements = new ElementType[_numElements];
                }
                else
                {
                    _dimensions = value;
                    _numDimensions = _dimensions.Length;
                    _numElements = GetNumElements(value, _numDimensions);
                    _elements = new ElementType[_numElements];
                }
            }
        }


        /// <summary>Sets dimensions of the current multidimensional array.
        /// <para>Array of dimensions is copied rather than only setting reference to the array.</para></summary>
        /// <param name="dimensions">Dimensions to be set (array or a set of integer parameters).</param>
        public void SetDimensions(params int[] dimensions)
        {
            SetDimensions(true /* copyDimensions */, dimensions);
        }

        /// <summary>Sets dimensions of the current multidimensional array.</summary>
        /// <param name="copyDimensions">If true then the array of dimensions is copied, otherwise a reference to the array is set directly.</param>
        /// <param name="dimensions">Dimensions to be set (array or a set of integer parameters).</param>
        public void SetDimensions(bool copyDimensions, params int[] dimensions)
        {
            if (copyDimensions)
            {
                int numDim = dimensions.Length;
                int[] dims = new int[numDim];
                for (int i = 0; i < numDim; ++i)
                    dims[i] = dimensions[i];
                Dimensions = dims;
            }
            else
                Dimensions = dimensions;
        }

        /// <summary>Gets or sets the specified element of the current multidimensional array.</summary>
        /// <param name="indices">Indices of the element.</param>
        public ElementType this[params int[] indices]
        {
            get
            {
                int flatIndex = GetFlatIndex(_dimensions, _numDimensions, indices);
                return _elements[flatIndex];
            }
            set
            {
                int flatIndex = GetFlatIndex(_dimensions, _numDimensions, indices);
                _elements[flatIndex] = value;
            }
        }


        /// <summary>Returns a flat (one dimensional) index that corresponds to the specified element indices in the current multidimensional array.</summary>
        /// <param name="indices">Indces of the element for which flat index is returned.</param>
        public int GetFlatIndex(params int[] indices)
        {
            return GetFlatIndex(_dimensions, _numDimensions, indices);
        }

        /// <summary>Calculates indices of the element with the specified flat (one dimensional) index in the current multidimensional array.</summary>
        /// <param name="flatIndex">Flat (one dimensional) index of the element for which multidimensional indices are calculated.</param>
        /// <param name="indices">Argument where the calculated indices are stored. It is reallocated if necessary.</param>
        public void GetIndices(int flatIndex, ref int[] indices)
        {
            GetIndices(_dimensions, _numDimensions, ref indices);
        }



        #region StaticMethods

        /// <summary>Returns number of elements of a multidimensional array wiht speecified dimensions.
        /// <para>This is a quicker variant where number of dimensions are specified separately.</para></summary>
        /// <param name="dimensions">Array of dimensions of the multidimensional array.</param>
        /// <param name="numDimensions">Nuber of dimensions, must be equal to the length of the aray of dimensions.
        /// Passing this as parameterr makes the method quicker.</param>
        /// <returns>Number of elements of the multidimensional array with speecified dimensions.</returns>
        protected int GetNumElements(int[] dimensions, int numDimensions)
        {
            if (dimensions == null)
                return 1;
            int numEl = 1;
            for (int i = 0; i < numDimensions; ++i)
                numEl *= dimensions[i];
            return numEl;
        }

        /// <summary>Returns number of elements of a multidimensional array wiht speecified dimensions.</summary>
        /// <param name="dimensions">Array of dimensions of the multidimensional array.</param>
        /// <returns>Number of elements of the multidimensional array with speecified dimensions.</returns>
        public int GetNumElements(int[] dimensions)
        {
            if (dimensions == null)
                return 1;
            int numEl = 1;
            int numDimensions = dimensions.Length;
            for (int i = 0; i < numDimensions; ++i)
                numEl *= dimensions[i];
            return numEl;
        }


        /// <summary>Returns flat array index corresponding to the specified element of a multidimensional array,
        /// i.e. index of such array element when elements are stored in one dimensional array.</summary>
        /// <param name="dimensions">Dimensions of the multidimensional array.</param>
        /// <param name="numDimensions">Number of dimensions of the array, specified as argument to increase thespeed.</param>
        /// <param name="indices">Indices of multidimensional array elements.</param>
        /// <returns>Flat array index of the specified element of a multidimensional array.</returns>
        protected int GetFlatIndex(int[] dimensions, int numDimensions, int[] indices)
        {
            if (dimensions == null)
                return 0;
            int index = 0;
            int whichDim = numDimensions - 1;  // start with the last dimension 
            int factor = 1;  // number of elements corresponding to increase of index in particular dimension 
            while (whichDim >= 0)
            {
                index += indices[whichDim] * factor;
                factor *= dimensions[whichDim];
                --whichDim;
            }
            return index;
        }


        /// <summary>Returns flat array index corresponding to the specified element of a multidimensional array,
        /// i.e. index of such array element when elements are stored in one dimensional array.</summary>
        /// <param name="dimensions">Dimensions of the multidimensional array.</param>
        /// <param name="indices">Indices of multidimensional array elements.</param>
        /// <returns>Flat array index of the specified element of a multidimensional array.</returns>
        public int GetFlatIndex(int[] dimensions, int[] indices)
        {
            if (dimensions == null)
                return 0;
            int index = 0;
            int numDimensions = dimensions.Length;
            int whichDim = numDimensions - 1;  // start with the last dimension 
            int factor = 1;  // number of elements corresponding to increase of index in particular dimension 
            while (whichDim >= 0)
            {
                index += indices[whichDim] * factor;
                factor *= dimensions[whichDim];
                --whichDim;
            }
            return index;
        }


        /// <summary>Returns flat array index corresponding to the specified element of a multidimensional array,
        /// i.e. index of such array element when elements are stored in one dimensional array.</summary>
        /// <param name="dimensions">Dimensions of the multidimensional array.</param>
        /// <param name="numDimensions">Number of dimensions of the array, specified as argument to increase thespeed.</param>
        /// <param name="flatIndex">Flat (one dimensional) index of an element.</param>
        /// <returns>Flat array index of the specified element of a multidimensional array.</returns>
        /// <param name="indices">Indices of multidimensional array elements.</param>
        protected void GetIndices(int[] dimensions, int numDimensions, int flatIndex, ref int[] indices)
        {
            if (dimensions == null)
            {
                indices = null;
                return;
            }
            if (indices == null)
                indices = new int[numDimensions];
            else if (indices.Length != numDimensions)
                indices = new int[numDimensions];
            int whichDim = numDimensions - 1;  // start with the last dimension 
            int factor = 1;  // number of elements corresponding to increase of index in particular dimension 
            int elementsRest = flatIndex + 1;
            while (whichDim >= 0)
            {
                int currentDim = dimensions[whichDim];
                int currentIndex = elementsRest % (currentDim * factor);
                elementsRest -= currentIndex * factor;
                indices[whichDim] = currentIndex;
                factor *= dimensions[whichDim];
                --whichDim;
            }
            if (elementsRest != 0)
                throw new InvalidOperationException("Number of elements rest is not 0 after calculation of multidimensional indices. "
                    + Environment.NewLine + "  Reason: invalid parameters or wrong operation.");
        }


        /// <summary>Returns flat array index corresponding to the specified element of a multidimensional array,
        /// i.e. index of such array element when elements are stored in one dimensional array.</summary>
        /// <param name="dimensions">Dimensions of the multidimensional array.</param>
        /// <param name="flatIndex">Flat (one dimensional) index of an element.</param>
        /// <returns>Flat array index of the specified element of a multidimensional array.</returns>
        /// <param name="indices">Indices of multidimensional array elements.</param>
        public void GetIndices(int[] dimensions, int flatIndex, ref int[] indices)
        {
            if (dimensions == null)
            {
                indices = null;
                return;
            }
            int numDimensions = dimensions.Length;
            if (indices == null)
                indices = new int[numDimensions];
            else if (indices.Length != numDimensions)
                indices = new int[numDimensions];
            int whichDim = numDimensions - 1;  // start with the last dimension 
            int factor = 1;  // number of elements corresponding to increase of index in particular dimension 
            int elementsRest = flatIndex + 1;
            while (whichDim >= 0)
            {
                int currentDim = dimensions[whichDim];
                int currentIndex = elementsRest % (currentDim * factor);
                elementsRest -= currentIndex * factor;
                indices[whichDim] = currentIndex;
                factor *= dimensions[whichDim];
                --whichDim;
            }
            if (elementsRest != 0)
                throw new InvalidOperationException("Number of elements rest is not 0 after calculation of multidimensional indices. "
                    + Environment.NewLine + "  Reason: invalid parameters or wrong operation.");
        }



        #endregion StaticMethods

    } // class mMultidimensionalArray



#endif


}
