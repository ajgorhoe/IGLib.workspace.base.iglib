// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

// DATA TRANSFER OBJECTS (DTO) THAT FACILITATE SERIALIZATION OF BOUNDING BOX OBJECTS.

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

    #region BoundingBox

    /// <summary>Base class for various DTO (Data Transfer Objects) for bounding boxes.
    /// Used to store a state of a bounding box.</summary>
    /// <typeparam name="BoxType">Type parameter specifying the specific bounding box type for which concrete DTO
    /// is designed.</typeparam>
    /// $A Igor Jun09;
    public abstract class BoundingBoxDtoBase<BoxType> : SerializationDtoBase<BoxType, IBoundingBox>
        where BoxType : class, IBoundingBox
    {

        #region Construction

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public BoundingBoxDtoBase()
            : base()
        { }

        /// <summary>Constructor, prepares the current DTO for storing a vector of the specified dimension.</summary>
        /// <param name="dimension">Dimension of a vector that is stored in the current DTO.</param>
        public BoundingBoxDtoBase(int dimension)
            : this()
        {
            this.Dimension = dimension;
        }

        #endregion Construction

        #region Data

        /// <summary>Dimension of the bounding box.</summary>
        public int Dimension;

        /// <summary>Minimal values of coordinates.</summary>
        public double[] Min = null;

        /// <summary>Maximal values of coordinates.</summary>
        public double[] Max = null;


        #endregion Data


        #region Operation

        /// <summary>Creates and returns a new bounding box of the specified dimension.</summary>
        /// <param name="dimension">Bounding box dimension.</param>
        public abstract BoxType CreateBoundingBox(int dimension);

        /// <summary>Creates and returns a new bounding box of the specified type and dimension.</summary>
        public override BoxType CreateObject()
        {
            return CreateBoundingBox(this.Dimension);
        }

        /// <summary>Copies data to the current DTO from a bounding box object.</summary>
        /// <param name="box">Bounding box object from which data is copied.</param>
        protected override void CopyFromPlain(IBoundingBox box)
        {
            if (box == null)
            {
                Dimension = 0;
                SetNull(true);
            }
            else
            {
                SetNull(false);
                Dimension = box.Dimension;
                if (Dimension < 1)
                {
                    Min = Max = null;
                }
                else
                {
                    if (Min == null)
                        Min = new double[Dimension];
                    else if (Min.Length != Dimension)
                        Min = new double[Dimension];
                    if (Max == null)
                        Max = new double[Dimension];
                    else if (Max.Length != Dimension)
                        Max = new double[Dimension];
                }
                for (int i = 0; i < Dimension; ++i)
                {
                    Min[i] = box.GetMin(i);
                    Max[i] = box.GetMax(i);
                }
            }
        }

        /// <summary>Copies data from the current DTO to a bounding box object.</summary>
        /// <param name="box">Bounding box object that data is copied to.</param>
        protected override void CopyToPlain(ref IBoundingBox box)
        {
            if (GetNull())
                box = null;
            else
            {
                if (box == null)
                    box = CreateObject();
                else if (box.Dimension != this.Dimension)
                    box = CreateObject();
                box.Reset();
                box.Update(Min);
                box.Update(Max);
            }
        }

        #endregion Operation


        #region Misc


        #endregion Misc

    }  // abstract class BoundingBoxDtoBase


    /// <summary>DTO (data transfer object) for vector interface (IVector).</summary>
    /// $A Igor Jun09;
    public class BoundingBoxDtoBase : BoundingBoxDtoBase<IBoundingBox>
    {

        #region Construction

        /// <summary>Creates a DTO for storing state of a bounding box of any bounding box type.</summary>
        public BoundingBoxDtoBase()
            : base()
        { }

        /// <summary>Creates a DTO for storing a bounding box object of any bounding box type, with specified dimension.</summary>
        /// <param name="dimension">Bounding box dimension.</param>
        public BoundingBoxDtoBase(int dimension)
            : base(dimension)
        { }

        #endregion Construction

        /// <summary>Creates and returns a new bounding box cast to the interface type IBoundingBox.</summary>
        /// <param name="dimension">Bounding box dimension.</param>
        public override IBoundingBox CreateBoundingBox(int dimension)
        {
            return new BoundingBox(dimension);
        }

    } // class BoundingBoxDtoBase



    /// <summary>Data Transfer Object (DTO) for bounding boxes of type IG.Num.BoundingBox.
    /// Used to store, transfer, serialize and deserialize objects of type BoundingBox.</summary>
    /// $A Igor Aug09;
    public class BoundingBoxDto : BoundingBoxDtoBase<BoundingBox>
    {

        #region Construction

        /// <summary>Creates a DTO for storing state of a bounding box object of any vector type</summary>
        public BoundingBoxDto()
            : base()
        { }

        /// <summary>Creates a DTO for storing a bounding box object of any bounding box type, with specified dimension.</summary>
        /// <param name="dimension">Bounding box dimension.</param>
        public BoundingBoxDto(int dimension)
            : base(dimension)
        { }

        #endregion Construction

        /// <summary>Creates and returns a new bounding box of the specified dimension.</summary>
        /// <param name="dimension">Bounding box dimension.</param>
        public override BoundingBox CreateBoundingBox(int dimension)
        {
            return new BoundingBox(dimension);
        }

    }  // class VectorDto


    #endregion BoundingBox


}