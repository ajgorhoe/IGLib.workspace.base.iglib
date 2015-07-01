// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

// CLASSES FOR DATA TRANSFER OBJECTS (DTO) THAT FACILITATE SERIALIZATION OF MATRIX OBJECTS.

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

    /// <summary>Data transfer object for holding arrays of objects of the same type.</summary>
    /// <typeparam name="ElementType">Type of elements.</typeparam>
    /// <typeparam name="ElementDtoType">Type of a DTO that is used to trasfer element data.</typeparam>
    /// $A Igor Jul09;
    public class ArrayDto<ElementType, ElementDtoType> : ArrayDto<ElementType, ElementType, ElementDtoType>
        where ElementType : class
        where ElementDtoType : SerializationDtoBase<ElementType, ElementType>, new()
    {

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public ArrayDto()
            : base()
        { }

    } // ArrayDto<ElementType, ElementDtoType>


    /// <summary>Data transfer object for holding arrays of objects of the same type.</summary>
    /// <typeparam name="ElementType">Type of elements.</typeparam>
    /// <typeparam name="ElementBaseType">Base type of elements.</typeparam>
    /// <typeparam name="ElementDtoType">Type of a DTO that is used to trasfer element data.</typeparam>
    /// $A Igor Jul09;
    public class ArrayDto<ElementType, ElementBaseType, ElementDtoType>: SerializationDtoBase<ElementType[], ElementType[]>
        where ElementBaseType: class
        where ElementType : class, ElementBaseType
        where ElementDtoType: SerializationDtoBase<ElementType, ElementBaseType>, new()
    {
        #region Construction

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public ArrayDto()
            : base()
        { }

        #endregion Construction

        public ElementDtoType[] Elements;

        /// <summary>Creates the array of objects and returns it.
        /// <para>The created array has the same dimension as the number of elements of the current
        /// DTO, or is null as specified by the current DTO (i.e. <see cref="SerializationDtoBase{T,BT}.GetNull()"/> = true).</para></summary>
        /// <returns></returns>
        public override ElementType[] CreateObject()
        {
            if (Elements == null)
                return null;
            else if (this.GetNull())
                return null;
            else
                return new ElementType[Elements.Length];
        }

        /// <summary>Copies data from the specified array object. All elements are copied to 
        /// appropriate DTOS on the array of elements (internal variable <see cref="Elements"/>).</summary>
        /// <param name="obj">Object that data is copied form.</param>
        protected override void CopyFromPlain(ElementType[] obj)
        {
            if (obj == null)
            {
                this.SetNull(true);
                Elements = null;
            } else
            {
                this.SetNull(false);
                Elements = new ElementDtoType[obj.Length];
                for (int i = 0; i < obj.Length; ++i)
                {
                    ElementDtoType element;
                    Elements[i] = element = new ElementDtoType();
                    element.CopyFrom(obj[i]);
                }
            }
        }

        /// <summary>Copies data from the current DTO to the specified array objects.
        /// <para>All data of contained elements is copied to elements of the array.</para>
        /// <para>Array is created anew / resized if necessary.</para></summary>
        /// <param name="obj">Objects that data are copied to.</param>
        protected override void CopyToPlain(ref ElementType[] obj)
        {
            if (this.GetNull())
                obj = null;
            else if (this.Elements == null)
                obj = null;
            else
            {
                if (obj.Length!=Elements.Length)
                    obj = new ElementType[Elements.Length];
                for (int i=0; i<Elements.Length; ++i)
                {
                    ElementType el = obj[i];
                    ElementDtoType elDto = Elements[i];
                    if (elDto == null)
                        el = null;
                    else
                        elDto.CopyTo(ref el);
                    obj[i] = el;
                }
            }
        }

        #region Examples

        /// <summary>An example of using the <see cref="ArrayDto{ElementType, ElementDtoType}"/> for storing an array of vecctors
        /// into a JSON file.</summary>
        /// <param name="inputFilePath">Path of the file to which vectors are saved. Must not be null or empty string.
        /// If the file already exists then user is promped whether to store vectors or not.</param>
        public static void ExampleVectorArray(string filePath)
        {
            bool append = false;
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Path of the JSON file to store array of vectors is not specified.");
            UtilSystem.StandardizeDirectoryPath(ref filePath);
            if (File.Exists(filePath))
            {
                Console.WriteLine();
                Console.WriteLine("Warning! File for storing vector array already exists: ");
                Console.WriteLine("  " + filePath);
                Console.WriteLine();
                bool overwrite = false;
                Console.Write("Do you want to overwrite the file wiht array of vectors (0/1)?");
                UtilConsole.Read(ref overwrite);
                Console.WriteLine();
                if (overwrite == false)
                {
                    Console.WriteLine("Writing will not be performed, aborting.");
                    Console.WriteLine();
                    return;
                }
            }
            Vector[] storedVectors = new Vector[]
                            {
                                new Vector(new double[]{1.1, 1.2}), 
                                new Vector(new double[]{2.1, 2.2, 2.3, 2.4})
                            };
            ArrayDto<Vector, IVector, VectorDto> dtoOriginal =
                new ArrayDto<Vector, IVector, VectorDto>();
            dtoOriginal.CopyFrom(storedVectors);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<ArrayDto<Vector, IVector, VectorDto>>(dtoOriginal, filePath, append);
            Console.WriteLine("Vector arry written to the file: ");
            Console.WriteLine("  " + filePath);
            Console.WriteLine();
        }

        #endregion Examples

    } // ArrayDto<ElementType, ElementBaseType, ElementDtoType>


    /// <summary>Data transfer object for holding data from lists of objects of the same type.</summary>
    /// <typeparam name="ElementType">Type of elements.</typeparam>
    /// <typeparam name="ElementDtoType">Type of a DTO that is used to trasfer element data.</typeparam>
    /// $A Igor Jul09;
    public class ListDto<ElementType, ElementDtoType> : ListDto<ElementType, ElementType, ElementDtoType>
        where ElementType : class
        where ElementDtoType : SerializationDtoBase<ElementType, ElementType>, new()
    {

        public ListDto()
            : base()
        {  }

    }


    /// <summary>Data transfer object for holding lists of objects of the same type.</summary>
    /// <typeparam name="ElementType">Type of elements.</typeparam>
    /// <typeparam name="ElementBaseType">Base type of elements.</typeparam>
    /// <typeparam name="ElementDtoType">Type of a DTO that is used to trasfer element data.</typeparam>
    /// $A Igor Jul09;
    public class ListDto<ElementType, ElementBaseType, ElementDtoType> : SerializationDtoBase<List<ElementType>, List<ElementType>>
        where ElementBaseType : class
        where ElementType : class, ElementBaseType
        where ElementDtoType : SerializationDtoBase<ElementType, ElementBaseType>, new()
    {
        #region Construction

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public ListDto()
            : base()
        { }

        #endregion Construction

        public ElementDtoType[] Elements;

        /// <summary>Creates the array of objects and returns it.
        /// <para>The created array has the same dimension as the number of elements of the current
        /// DTO, or is null as specified by the current DTO (i.e. <see cref="SerializationDtoBase<T,BT>.GetNull()"/> = true).</para></summary>
        /// <returns></returns>
        public override List<ElementType> CreateObject()
        {
            if (Elements == null)
                return null;
            else if (this.GetNull())
                return null;
            else
                return new List<ElementType>(Elements.Length);
        }

        /// <summary>Copies data from the specified list object. All elements are copied to 
        /// appropriate DTOS on the array of elements (internal variable <see cref="Elements"/>).</summary>
        /// <param name="obj">Object that data is copied form.</param>
        protected override void CopyFromPlain(List<ElementType> obj)
        {
            if (obj == null)
            {
                this.SetNull(true);
                Elements = null;
            }
            else
            {
                this.SetNull(false);
                Elements = new ElementDtoType[obj.Count];
                for (int i = 0; i < obj.Count; ++i)
                {
                    ElementDtoType element;
                    Elements[i] = element = new ElementDtoType();
                    element.CopyFrom(obj[i]);
                }
            }
        }

        /// <summary>Copies data from the current DTO to the specified list objects.
        /// <para>All data of contained elements is copied to elements of the array.</para>
        /// <para>Array is created anew / resized if necessary.</para></summary>
        /// <param name="obj">Objects that data are copied to.</param>
        protected override void CopyToPlain(ref List<ElementType> obj)
        {
            if (this.GetNull())
                obj = null;
            else if (this.Elements == null)
                obj = null;
            else
            {
                if (obj.Count != Elements.Length)
                    Util.ResizeList(ref obj, Elements.Length, null);
                for (int i = 0; i < Elements.Length; ++i)
                {
                    ElementType el = obj[i];
                    ElementDtoType elDto = Elements[i];
                    if (elDto == null)
                        el = null;
                    else
                        elDto.CopyTo(ref el);
                    obj[i] = el;
                }
            }
        }


        #region Examples

        /// <summary>An example of using the <see cref="ArrayDto{ElementType, ElementDtoType}"/> for storing an array of vecctors
        /// into a JSON file.</summary>
        /// <param name="inputFilePath">Path of the file to which vectors are saved. Must not be null or empty string.
        /// If the file already exists then user is promped whether to store vectors or not.</param>
        public static void ExampleVectorArray(string filePath)
        {
            bool append = false;
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Path of the JSON file to store list of vectors is not specified.");
            UtilSystem.StandardizeDirectoryPath(ref filePath);
            if (File.Exists(filePath))
            {
                Console.WriteLine();
                Console.WriteLine("Warning! File for storing list of vectors already exists: ");
                Console.WriteLine("  " + filePath);
                Console.WriteLine();
                bool overwrite = false;
                Console.Write("Do you want to overwrite the file wiht list of vectors (0/1)?");
                UtilConsole.Read(ref overwrite);
                Console.WriteLine();
                if (overwrite == false)
                {
                    Console.WriteLine("Writing will not be performed, aborting.");
                    Console.WriteLine();
                    return;
                }
            }
            List<Vector> storedVectors = new List<Vector>(
                            new Vector[]
                            {
                                new Vector(new double[]{1.1, 1.2}), 
                                new Vector(new double[]{2.1, 2.2, 2.3, 2.4})
                            });
            ListDto<Vector, IVector, VectorDto> dtoOriginal =
                new ListDto<Vector, IVector, VectorDto>();
            dtoOriginal.CopyFrom(storedVectors);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<ListDto<Vector, IVector, VectorDto>>(dtoOriginal, filePath, append);
            Console.WriteLine("List of vectors written to the file: ");
            Console.WriteLine("  " + filePath);
            Console.WriteLine();
        }

        #endregion Examples



    } // ListDto<ElementType, ElementBaseType, ElementDtoType>


}