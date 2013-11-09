// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

// CLASSES FOR DATA TRANSFER OBJECTS (DTO) THAT FACILITATE SERIALIZATION OF INDEX LIST OBJECTS.

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


    #region IndexList

    /// <summary>Data Transfer Object (DTO) for index lists of type IndexList.
    /// Used to store, transfer, serialize and deserialize objects of type IndexList.</summary>
    /// $A Igor Aug09;
    public class IndexListDto : SerializationDto<IndexList>
    {

        #region Construction

        /// <summary>Creates a DTO for storing state of a bounding box object of any vector type</summary>
        public IndexListDto()
            : base()
        { }

        #endregion Construction

        #region Data


        /// <summary>Indices contained in the index list.</summary>
        public List<int> Indices = new List<int>();

        #endregion Data

        #region Operation

        /// <summary>Creates and returns a new index list.</summary>
        public override IndexList CreateObject()
        {
            return new IndexList();
        }


        /// <summary>Copies data to the current DTO from an index list object.</summary>
        /// <param name="list">Index list object from which data is copied.</param>
        protected override void CopyFromPlain(IndexList list)
        {
            if (list == null)
            {
                SetNull(true);
            }
            else
            {
                SetNull(false);
                Indices.Clear();
                for (int i = 0; i < list.Length; ++i)
                {
                    Indices.Add(list[i]);
                }
            }
        }

        /// <summary>Copies data from the current DTO to an index list object.</summary>
        /// <param name="list">Index list object that data is copied to.</param>
        protected override void CopyToPlain(ref IndexList list)
        {
            if (GetNull())
                list = null;
            else
            {
                if (list == null)
                    list = CreateObject();
                list.Clear();
                for (int i = 0; i < Indices.Count; ++i)
                    list.Add(Indices[i]);
            }
        }


        #endregion Operation

    }  // class IndexListDto



    #endregion IndexList


}