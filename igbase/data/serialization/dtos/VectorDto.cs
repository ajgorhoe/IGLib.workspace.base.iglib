// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// CLASSES FOR DATA TRANSFER OBJECTS (DTO) THAT FACILITATE SERIALIZATION OF VECTOR OBJECTS.

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using IG.Num;
using System.Collections.Generic;

namespace IG.Lib
{



    /// <summary>Base class for various vector DTO (Data Transfer Objects) for vectors.
    /// Used to store a state of a vector.</summary>
    /// <typeparam name="VectorType">Type parameter specifying the specific vector type for which concrete DTO
    /// is designed.</typeparam>
    /// $A Igor Jun09;
    public abstract class VectorDtoBase<VectorType> : SerializationDtoBase<VectorType, IVector>
        where VectorType : class, IVector
    {

        #region Construction

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public VectorDtoBase()
            : base()
        { }

        /// <summary>Constructor, prepares the current DTO for storing a vector of the specified dimension.</summary>
        /// <param name="length">Dimension of a vector that is stored in the current DTO.</param>
        public VectorDtoBase(int length)
            : this()
        {
            this._length = length;
            AllocateComponents(length);
        }

        #endregion Construction

        #region Data

        protected int _length;

        protected double[] _elements;


        /// <summary>Allocates the array that stores vector elements.</summary>
        /// <param name="dim">Vector dimension.</param>
        protected virtual void AllocateComponents(int dim)
        {
            if (dim <= 0)
                this.SetNull(true);
            else
                this.SetNull(false);
            int dim1 = 0;
            if (_elements != null)
            {
                dim1 = _elements.Length;
            }
            if (dim1 != dim)
            {
                if (dim <= 0)
                {
                    _elements = null;
                }
                else
                {
                    _elements = new double[dim];
                }
            }
        }

        /// <summary>Vector dimension.</summary>
        public int Length
        {
            get { return _length; }
            set
            {
                _length = value;
                AllocateComponents(value);

                //if (value <= 0)
                //    this.SetIsNull(true);
                //else
                //    this.SetIsNull(false);
                //int dimComponents = 0;
                //// Make elements consistent with the new dimension:
                //if (Elements != null)
                //    dimComponents = Elements.Length;
                //if (value != dimComponents)
                //{
                //    if (value > 0)
                //    {
                //        Elements = new double[value];
                //        this.SetIsNull(false);
                //    } else
                //    {
                //        Elements = null;
                //        this.SetIsNull(true);
                //    }
                //}
            }
        }

        /// <summary>Vector elements.</summary>
        public double[] Elements
        {
            get { return _elements; }
            set
            {
                _elements = value;
                // Make dimension consistent with new array of elements:
                if (value == null)
                    _length = 0;
                else
                    _length = value.Length;
            }
        }

        #endregion Data


        #region Operation

        /// <summary>Creates and returns a new vector of the specified dimension.</summary>
        /// <param name="length">Vector dimension.</param>
        public abstract VectorType CreateVector(int length);

        /// <summary>Creates and returns a new vector of the specified type and dimension.</summary>
        public override VectorType CreateObject()
        {
            return CreateVector(this.Length);
        }

        /// <summary>Copies data to the current DTO from a vector object.</summary>
        /// <param name="vec">Vector object from which data is copied.</param>
        protected override void CopyFromPlain(IVector vec)
        {
            if (vec == null)
            {
                Length = 0;
                SetNull(true);
            }
            else
            {
                this.SetNull(false);
                int dimComponents = 0;
                if (Elements != null)
                    dimComponents = Elements.Length;
                if (dimComponents != vec.Length && vec.Length > 0)
                {
                    Elements = new double[vec.Length];
                }
                this.Length = vec.Length;
                for (int i = 0; i < Length; ++i)
                    this.Elements[i] = vec[i];
            }
        }

        /// <summary>Copies data from the current DTO to a vector object.</summary>
        /// <param name="vec">Vector object that data is copied to.</param>
        protected override void CopyToPlain(ref IVector vec)
        {
            if (GetNull())
                vec = null;
            else
            {
                if (vec == null)
                    vec = CreateObject();
                if (vec.Length != Length)
                    vec = CreateObject();
                for (int i = 0; i < Length; ++i)
                    vec[i] = this.Elements[i];

            }
        }

        #endregion Operation


        #region Misc

        ///// <summary>Creates and returns a string representation of the curren vector DTO.</summary>
        //public override string ToString()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("VectorDTO, tpe = " + typeof(VectorType).ToString() + ": ");
        //    sb.AppendLine("  Dimension: " + Length );
        //    sb.AppendLine("  Components: ");
        //    sb.Append("  {");
        //    for (int i = 0; i < Length; ++i)
        //    {
        //        sb.Append(Elements[i]);
        //        if (i == Length - 1)
        //            sb.Append("}");
        //        else
        //            sb.Append(", ");
        //    }
        //    sb.AppendLine();
        //    return sb.ToString();
        //}

        #endregion Misc

    }  // abstract class VectorDtoBase


    /// <summary>DTO (data transfer object) for vector interface (IVector).</summary>
    /// $A Igor Jun09;
    public class VectorDtoBase : VectorDtoBase<IVector>
    {

        #region Construction

        /// <summary>Creates a DTO for storing state of a vector object of any vector type</summary>
        public VectorDtoBase()
            : base()
        { }

        /// <summary>Creates a DTO for storing a vector object of any vector type, with specified dimension.</summary>
        /// <param name="length">Vector dimension.</param>
        public VectorDtoBase(int length)
            : base(length)
        { }

        #endregion Construction

        /// <summary>Creates and returns a new vector cast to the interface type IVector.</summary>
        /// <param name="length">Vector dimension.</param>
        public override IVector CreateVector(int length)
        {
            return new Vector(length);
        }

    } // class VectorDtoBase


    /// <summary>Data Transfer Object (DTO) for vectors of type IG.Num.Vector.
    /// Used to store, transfer, serialize and deserialize objects of type Vector.</summary>
    /// $A Igor Aug09;
    public class VectorDto : VectorDtoBase<Vector>
    {

        #region Construction

        /// <summary>Creates a DTO for storing state of a vector object of any vector type</summary>
        public VectorDto()
            : base()
        { }

        /// <summary>Creates a DTO for storing a vector object of any vector type, with specified dimension.</summary>
        /// <param name="length">Vector dimension.</param>
        public VectorDto(int length)
            : base(length)
        { }

        #endregion Construction

        /// <summary>Creates and returns a new vector of the specified dimension.</summary>
        /// <param name="length">Vector dimension.</param>
        public override Vector CreateVector(int length)
        {
            return new Vector(length);
        }

    }  // class VectorDto



}