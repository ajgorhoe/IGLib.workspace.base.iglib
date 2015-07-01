// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// CLASSES FOR DATA TRANSFER OBJECTS THAT FACILITATE SERIALIZATION OF BASIC OBJECTS.


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

    // REMRKS:
    // Almost always base class for DTOS will be SerializationDtoBase<Type>
    // instead of SerializationDtoBase<Type, BaseType>. Use of class with different BaseType
    // can save some work in the case of inheritance, but requires much more expertise, and
    // pay-off is in most cases too small.


    /// <summary>Generic base class for Data Transfer Objects (DTO).
    /// This class is used as template for producing concrete DTO classes. Such DTOs are used for serialization and
    /// deserialization of state of various kinds of objects that need to be transfered between applications,
    /// across platforms, or simply stored in files for future use.
    /// </summary>
    /// <typeparam name="Type">Type for which DTO is used.</typeparam>
    /// <seealso cref="SerializationDtoBase<Type, Type>"/>
    /// <remarks>There is an agreement that all derived classes must have a public argument-less (default) constructor.
    /// Generic classes are usually not used for serialization/deserialization. Only derived types 
    /// where both type parameters are fixed are normally used for this purpos.
    /// This class is essentially equal to SerializationDtoBase<Type, BaseType> where BaseType is the same
    /// as Type.
    /// IMPORTANT:
    /// This class does not have base type specified, i.e. it is used for situation where actual object type is not
    /// sidtinguished from base type. Base type in the variant with two types (<see cref="SerializationDtoBase<Type, BaseType>"/>)
    /// is used just occasionally because of the benefit of defining copying operation only for base type and use it for different
    /// derived types. </remarks>
    /// $A Igor Jun09;
    public interface ISerializationDto<Type> : ISerializationDto<Type, Type>
    {  }

    /// <summary>This interface facilitates use of static helper methods for copying on the
    /// SerializationDtoBase class. Contains those methods of ISerializationDto that don't 
    /// use BaseType.</summary>
    /// <typeparam name="Type"></typeparam>
    public interface ISerializationDtoAux<Type>
    {


        /// <summary>Returns a flag indicating whether the object represented by the current DTO is null.</summary>
        /// <returns>Flag indicating whether the object represented by the current DTO is null.</returns>
        /// <remarks>Implementing this as a function rather than a property prevents serialization of the flag,
        /// since the flag isn't meant to contain data, but to instruct operations that the object represented
        /// by the current DTO is null. This is useful in some scenarios where non-null DTOs are needed for null
        /// objects.</remarks>
        bool GetNull();

        /// <summary>Sets a flag indicating whether the object represented by the current DTO is null.</summary>
        /// <param name="isNull">If true, this flag indicates that the object represented by the current DTO
        /// is null, although the DTO itself is not null.</param>
        /// <remarks>Implementing this as a function rather than a property prevents serialization of the flag,
        /// since the flag isn't meant to contain data, but to instruct operations that the object represented
        /// by the current DTO is null. This is useful in some scenarios where non-null DTOs are needed for null
        /// objects.</remarks>
        void SetNull(bool isNull);


        /// <summary>Creates and returns a new object of the type whose data is represented by the
        /// current DTO (Data Transfer Object).
        /// WARNING: Implement thread locking in overriding functions!</summary>
        /// <remarks>Therad locking should be performed in overriding functions!</remarks>
        Type CreateObject();

        /// <summary>Copies data to the current DTO from an object of type Type.</summary>
        /// <param name="obj">Object whose data is copied to the current DTO.</param>
        void CopyFrom(Type obj);


        /// <summary>Copies data from the current DTO to an object of type Type.
        /// Object is created anew if necessary by using the CreateObject() method.</summary>
        /// <param name="obj">Object to which data is copied.</param>
        void CopyTo(ref Type obj);
    }

    /// <summary>Interface for Data Transfer Objects (DTO).
    /// This class is used as template for producing concrete DTO classes. Such DTOs are used for serialization and
    /// deserialization of state of various kinds of objects that need to be transfered between applications,
    /// across platforms, or simply stored in files for future use.
    /// WARNING:
    /// In most cases ISerializationDto<Type> will be used. Different BaseType and Type are used only in 
    /// relatively rare cases where different derived types all have the same data that is copied to DTO. 
    /// Otherwise the advantage of this can not be used because of single inheritance.
    /// </summary>
    /// <typeparam name="Type">Type for which DTO is used.</typeparam>
    /// <typeparam name="BaseType">Base type of the type for which DTO is used, and on which copy operations
    /// will be defined. In this way, we can avoid defining these operations for each specific type,
    /// but only define them for a specific type, since operations may be similar for all derived types.</typeparam>
    /// <remarks>There is an agreement that all derived classes must have a public argument-less (default) constructor.
    /// Generic classes are usually not used for serialization/deserialization. Only derived types 
    /// where both type parameters are fixed are normally used for this purpos.
    /// IMPORTANT:
    /// Base type is used just for being able to define copying operations only once - for the base type - and 
    /// using it for different derived types. In many occasions this will not be needed, and in these occasions
    /// one should just use the derived type that does not have base type as type parameter. 
    /// </remarks>
    /// $A Igor Jun09;
    public interface ISerializationDto<Type, BaseType> : ISerializationDtoAux<Type>
    {
        
        /// <summary>Copies data to the current DTO from an object of type BaseType.</summary>
        /// <param name="obj">Object whose data is copied to the current DTO.</param>
        void CopyFromBase(BaseType obj);

        /// <summary>Copies data from the current DTO to an object of the base type.
        /// Object is created anew if necessary by using the CreateObject() method.</summary>
        /// <param name="obj">Object to which data is copied.</param>
        void CopyToBase(ref BaseType obj);
                

        /// <summary>Copies data to the current DTO from an object of type object. 
        /// The necessary casts are performed.</summary>
        /// <param name="obj">Object whose data is copied to the current DTO.</param>
        void CopyFromObject(object obj);
        

        /// <summary>Copies data from the current DTO to an object of type object.
        /// Object is created anew if necessary by using the CreateObject() method.
        /// The necessary casts are performed.</summary>
        /// <param name="obj">Object to which data is copied.</param>
        void CopyToObject(ref object obj);

    }



    /// <summary>Auxiliary base class for DTOs, contains some static methods.
    /// Provided for simplicity, such that one does not need to state type parameters</summary>
    public abstract class SerializationDto // : ILockable
    {
        protected SerializationDto() : base() { }

        #region AuxiliaryStaticCopyMethods

        /// <summary>Replacement for <see cref="CopyToObject<DtoType>"/> for cases where object can not be passed by reference.
        /// The returned object must be assigned to object (property, list element, etc.) to which object state is copied.</summary>
        public static ObjectType CopyToObjectReturned<DtoType, ObjectType>(DtoType dto, ObjectType obj)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            CopyToObject<DtoType, ObjectType>(dto, ref obj);
            return obj;
        }


        /// <summary>Copies object state form the specified DTO (data transfer object) to the specified object.</summary>
        /// <typeparam name="DtoType">Type of the DTO, must inherit from <see cref="SerializationDtoBase<Type, BaseType>"/>
        /// and must have a public argumentlsee constructor.</typeparam>
        /// <param name="dto">Data transfer object (DTO) from which data is copied.</param>
        /// <param name="obj">Object to which data is copied.</param>
        public static void CopyToObject<DtoType, ObjectType>(DtoType dto, ref ObjectType obj)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            if (dto == null)
                obj = null;
            else if (dto.GetNull())
                obj = null;
            else
                dto.CopyTo(ref obj);
        }


        /// <summary>Replacement for <see cref="CopyObjectFromObject<DtoType>"/> for cases where object can not be passed by reference.
        /// The returned object must be assigned to object (property, list element, etc.) to which object state is copied.</summary>
        public static DtoType CopyFromObjectReturned<DtoType, ObjectType>(ObjectType obj, DtoType dto)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            CopyFromObject<DtoType, ObjectType>(obj, ref dto);
            return dto;
        }


        /// <summary>Copies object state form the specified object to the corresponding DTO (data transfer object).</summary>
        /// <typeparam name="DtoType">Type of the DTO, must inherit from <see cref="SerializationDtoBase<Type, BaseType>"/>
        /// and must have a public argumentlsee constructor.</summary>
        /// <typeparam name="DtoType">Type of the DTO, must inherit from <see cref="SerializationDtoBase<Type, BaseType>"/>
        /// and must have a public argumentlsee constructor.</typeparam>
        /// <param name="obj">Object from which data is copied.</param>
        /// <param name="dto">Data transfer object (DTO) to which data is copied.</param>
        public static void CopyFromObject<DtoType, ObjectType>(ObjectType obj, ref DtoType dto)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            if (dto == null)
                dto = new DtoType();
            if (obj == null)
                dto.SetNull(true);
            else
            {
                dto.SetNull(false);
                dto.CopyFrom(obj);
            }
        }


        /// <summary>Replacement for <see cref="CopyArrayToObject"/> for cases where object can not be passed by reference.
        /// The returned object must be assigned to object (property, list element, etc.) to which data is copied.</summary>
        /// <example>tabObj = CopyArrayToObjectReturned&lt;DtoType&gt;(tabDto, tabObj);</example>
        public static ObjectType[] CopyArrayToObjectReturned<DtoType, ObjectType>(DtoType[] tabDto, ObjectType[] tabObj)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            CopyArrayToObject<DtoType, ObjectType>(tabDto, ref tabObj);
            return tabObj;
        }


        /// <summary>Copies array of DTOs (Data Transfer Objects) to an array of appropriate objects.</summary>
        /// <typeparam name="DtoType">Type of the DTO, must inherit from <see cref="SerializationDtoBase<Type, BaseType>"/>
        /// and must have a public argumentlsee constructor.</typeparam>
        /// <param name="tabDto">Table of DTOs from which data (object states) is copied.</param>
        /// <param name="tabObj">Table of objects to which data (object states) is copied.</param>
        public static void CopyArrayToObject<DtoType, ObjectType>(DtoType[] tabDto, ref ObjectType[] tabObj)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            if (tabDto == null)
                tabObj = null;
            else
            {
                int numElements = tabDto.Length;
                if (tabObj==null)
                    tabObj = new ObjectType[numElements];
                else if (tabObj.Length != tabDto.Length)
                    tabObj = new ObjectType[numElements];
                for (int i = 0; i < numElements; ++i)
                {
                    DtoType elementDto = tabDto[i];
                    if (elementDto == null)
                        tabObj[i] = null;
                    else if (elementDto.GetNull())
                        tabObj[i] = null;
                    else
                    {
                        elementDto.CopyTo(ref (tabObj[i]));
                    }
                }
            }
        }



        /// <summary>Replacement for <see cref="CopyArrayFromObject"/> for cases where object can not be passed by reference.
        /// The returned object must be assigned to object (property, list element, etc.) to which data is copied.</summary>
        /// <example>tabDto = CopyArrayFromObjectReturned&lt;DtoType&gt;(tabObj, tabDto)</example>
        public static DtoType[] CopyArrayFromObjectReturned<DtoType, ObjectType>(ObjectType[] tabObj, DtoType[] tabDto)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            CopyArrayFromObject<DtoType, ObjectType>(tabObj, ref tabDto);
            return tabDto;
        }

        /// <summary>Copies array of objects to an array of DTOs.</summary>
        /// <typeparam name="DtoType">Type of the DTO, must inherit from <see cref="SerializationDtoBase<Type, BaseType>"/>
        /// and must have a public argumentlsee constructor.</typeparam>
        /// <param name="tabObj">Table of objects from which data (object states) is copied.</param>
        /// <param name="tabDto">Table of DTOs to which data (object states) is copied.</param>
        public static void CopyArrayFromObject<DtoType, ObjectType>(ObjectType[] tabObj, ref DtoType[] tabDto)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            if (tabObj == null)
                tabDto = null;
            else
            {
                int numElements = tabObj.Length;
                if (tabDto == null)
                    tabDto = new DtoType[numElements];
                else if (tabDto.Length != tabObj.Length)
                    tabDto = new DtoType[numElements];
                for (int i = 0; i < numElements; ++i)
                {
                    if (tabDto[i] == null)
                        tabDto[i] = new DtoType();
                    DtoType elementDto = tabDto[i];
                    ObjectType elementObj = tabObj[i];
                    if (elementObj == null)
                        elementDto.SetNull(true);
                    else
                        elementDto.CopyFrom(elementObj);
                }
            }
        }



        /// <summary>Replacement for <see cref="CopyListToObject"/> for cases where object can not be passed by reference.
        /// The returned object must be assigned to object (property, list element, etc.) to which data is copied.</summary>
        /// <example>listObj = CopyListToObjectReturned&lt;DtoType&gt;(tabDto, listObj)</example>

        public static List<ObjectType> CopyListToObjectReturned<DtoType, ObjectType>(DtoType[] tabDto, List<ObjectType> listObj)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            CopyListToObject<DtoType, ObjectType>(tabDto, ref listObj);
            return listObj;
        }

        /// <summary>Copies array of DTOs (Data Transfer Objects) to a list of appropriate objects.</summary>
        /// <typeparam name="DtoType">Type of the DTO, must inherit from <see cref="SerializationDtoBase<Type, BaseType>"/>
        /// and must have a public argumentlsee constructor.</typeparam>
        /// <param name="tabDto">Table of DTOs from which data (object states) is copied.</param>
        /// <param name="tabObj">List of objects to which data (object states) is copied.</param>
        public static void CopyListToObject<DtoType, ObjectType>(DtoType[] tabDto, ref List<ObjectType> listObj)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            if (tabDto == null)
                listObj = null;
            else
            {
                int numElements = tabDto.Length;
                if (listObj == null)
                    listObj = new List<ObjectType>();
                if (listObj.Count != tabDto.Length)
                    Util.ResizeList<ObjectType>(ref listObj, tabDto.Length, null);
                for (int i = 0; i < numElements; ++i)
                {
                    DtoType elementDto = tabDto[i];
                    if (elementDto == null)
                        listObj[i] = null;
                    else if (elementDto.GetNull())
                        listObj[i] = null;
                    else
                    {
                        ObjectType elementObj = listObj[i];
                        elementDto.CopyTo(ref elementObj);
                        listObj[i] = elementObj;
                    }
                }
            }
        }


        /// <summary>Replacement for <see cref="CopyArrayFromObject"/> for cases where object can not be passed by reference.
        /// The returned object must be assigned to object (property, list element, etc.) to which data is copied.</summary>
        /// <example>tabDto = CopyArrayFromObjectReturned&lt;DtoType&gt;(tabObj, tabDto)</example>
        public static DtoType[] CopyListFromObjectReturned<DtoType, ObjectType>(List<ObjectType> tabObj, DtoType[] tabDto)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            CopyListFromObject<DtoType, ObjectType>(tabObj, ref tabDto);
            return tabDto;
        }

        /// <summary>Copies array of objects to a list of DTOs.</summary>
        /// <typeparam name="DtoType">Type of the DTO, must inherit from <see cref="SerializationDtoBase<Type, BaseType>"/>
        /// and must have a public argumentlsee constructor.</typeparam>
        /// <param name="tabObj">List of objects from which data (object states) is copied.</param>
        /// <param name="tabDto">Table of DTOs to which data (object states) is copied.</param>
        /// <remarks>Parameter <paramref name="tabDto"/> is an array because DTOs are normally not put into lists
        /// (since generic types can not be correctly serialized and deserialized by all kinds of serializers).</remarks>
        public static void CopyListFromObject<DtoType, ObjectType>(List<ObjectType> tabObj, ref DtoType[] tabDto)
            where DtoType : class, ISerializationDtoAux<ObjectType>, new()
            where ObjectType : class
        {
            if (tabObj == null)
                tabDto = null;
            else
            {
                int numElements = tabObj.Count;
                if (tabDto==null)
                    tabDto = new DtoType[numElements];
                else if (tabDto.Length != tabObj.Count)
                    tabDto = new DtoType[numElements];
                for (int i = 0; i < numElements; ++i)
                {
                    if (tabDto[i] == null)
                        tabDto[i] = new DtoType();
                    DtoType elementDto = tabDto[i];
                    ObjectType elementObj = tabObj[i];
                    if (elementObj == null)
                        elementDto.SetNull(true);
                    else
                        elementDto.CopyFrom(elementObj);
                }
            }
        }

        // BaseType BaseType

        #endregion AuxiliaryStaticCopyMethods

    }


    /// <summary>Generic base class for Data Transfer Objects (DTO).
    /// This class is used as template for producing concrete DTO classes. Such DTOs are used for serialization and
    /// deserialization of state of various kinds of objects that need to be transfered between applications,
    /// across platforms, or simply stored in files for future use.
    /// </summary>
    /// <typeparam name="Type">Type for which DTO is used.</typeparam>
    /// <seealso cref="SerializationDtoBase<Type, Type>"/>
    /// <remarks>There is an agreement that all derived classes must have a public argument-less (default) constructor.
    /// Generic classes are usually not used for serialization/deserialization. Only derived types 
    /// where both type parameters are fixed are normally used for this purpos.
    /// This class is essentially equal to SerializationDtoBase<Type, BaseType> where BaseType is the same
    /// as Type.
    /// IMPORTANT:
    /// This class does not have base type specified, i.e. it is used for situation where actual object type is not
    /// sidtinguished from base type. Base type in the variant with two types (<see cref="SerializationDtoBase<Type, BaseType>"/>)
    /// is used just occasionally because of the benefit of defining copying operation only for base type and use it for different
    /// derived types. </remarks>
    /// $A Igor Jun09;
    public abstract class SerializationDto<Type> : SerializationDtoBase<Type, Type>, 
            ISerializationDto<Type>, ILockable
        where Type: class
    {
        public SerializationDto(): base()
        {  }
    }

    /// <summary>Generic base class for Data Transfer Objects (DTO).
    /// This class is used as template for producing concrete DTO classes. Such DTOs are used for serialization and
    /// deserialization of state of various kinds of objects that need to be transfered between applications,
    /// across platforms, or simply stored in files for future use.
    /// WARNING:
    /// In most cases ISerializationDto<Type> will be used. Different BaseType and Type are used only in 
    /// relatively rare cases where different derived types all have the same data that is copied to DTO. 
    /// Otherwise the advantage of this can not be used because of single inheritance.
    /// </summary>
    /// <typeparam name="Type">Type for which DTO is used.</typeparam>
    /// <typeparam name="BaseType">Base type of the type for which DTO is used, and on which copy operations
    /// will be defined. In this way, we can avoid defining these operations for each specific type,
    /// but only define them for a specific type, since operations may be similar for all derived types.</typeparam>
    /// <remarks>There is an agreement that all derived classes must have a public argument-less (default) constructor.
    /// Generic classes are usually not used for serialization/deserialization. Only derived types 
    /// where both type parameters are fixed are normally used for this purpos.
    /// IMPORTANT:
    /// Base type is used just for being able to define copying operations only once - for the base type - and 
    /// using it for different derived types. In many occasions this will not be needed, and in these occasions
    /// one should just use the derived type that does not have base type as type parameter. 
    /// </remarks>
    /// $A Igor Jun09;
    public abstract class SerializationDtoBase<Type, BaseType> : SerializationDto,
        ILockable,  ISerializationDto<Type, BaseType>
        where BaseType: class  where Type: class, BaseType
    {

        public SerializationDtoBase()
        { }

        #region Data

        protected bool _isNull = false;

        /// <summary>Returns a flag indicating whether the object represented by the current DTO is null.</summary>
        /// <returns>Flag indicating whether the object represented by the current DTO is null.</returns>
        /// <remarks>Implementing this as a function rather than a property prevents serialization of the flag,
        /// since the flag isn't meant to contain data, but to instruct operations that the object represented
        /// by the current DTO is null. This is useful in some scenarios where non-null DTOs are needed for null
        /// objects.</remarks>
        public virtual bool GetNull()
        {
            lock (Lock) { return _isNull; }
        }

        /// <summary>Sets a flag indicating whether the object represented by the current DTO is null.</summary>
        /// <param name="isNull">If true, this flag indicates that the object represented by the current DTO
        /// is null, although the DTO itself is not null.</param>
        /// <remarks>Implementing this as a function rather than a property prevents serialization of the flag,
        /// since the flag isn't meant to contain data, but to instruct operations that the object represented
        /// by the current DTO is null. This is useful in some scenarios where non-null DTOs are needed for null
        /// objects.</remarks>
        public virtual void SetNull(bool isNull)
        { lock (Lock) { this._isNull = isNull; } } 

        #endregion Data

        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking


        #region Operation

        /// <summary>Creates and returns a new object of the type whose data is represented by the
        /// current DTO (Data Transfer Object).
        /// WARNING: Implement thread locking in overriding functions!</summary>
        /// <remarks>Therad locking should be performed in overriding functions!</remarks>
        public abstract Type CreateObject();

        /// <summary>Copies contents of the specified object to the current DTO (Data Transfer Object).</summary>
        /// <param name="obj">Object whose data is copied.</param>
        /// <remarks>If the specified object is null then the _isNull property of the current DTO is set to true.
        /// Therad locking is usually not needed in overriding functions because it is 
        /// normally implemeted in calling functions.</remarks>
        protected abstract void CopyFromPlain(BaseType obj);

        /// <summary>Copies contents of the current DTO (Data Transfer Object) to the specified object.</summary>
        /// <param name="obj">Object that data is copied to.</param>
        /// <remarks>If _isNull property of the current DTO is set to true then the object becomes null.
        /// Therad locking is usually not needed in overriding functions because it is 
        /// normally implemeted in calling functions.</remarks>
        protected abstract void CopyToPlain(ref BaseType obj);
        
        /// <summary>Copies data to the current DTO from an object of type BaseType.</summary>
        /// <param name="obj">Object whose data is copied to the current DTO.</param>
        public virtual void CopyFromBase(BaseType obj)
        {
            lock (Lock)
            {
                if (obj == null)
                    SetNull(true);
                else
                {
                    SetNull(false);
                    CopyFromPlain(obj);
                }
            }
        }

        /// <summary>Copies data from the current DTO to an object of the base type.
        /// Object is created anew if necessary by using the CreateObject() method.</summary>
        /// <param name="obj">Object to which data is copied.</param>
        public virtual void CopyToBase(ref BaseType obj)
        {
            lock (Lock)
            {
                if (GetNull())
                    obj = null; // default(BaseType);  
                else
                {
                    if (obj == null)
                        obj = CreateObject();
                    CopyToPlain(ref obj);
                }
            }
        }


        /// <summary>Copies data to the current DTO from an object of type Type.</summary>
        /// <param name="obj">Object whose data is copied to the current DTO.</param>
        public virtual void CopyFrom(Type obj)
        {
            lock (Lock)
            {
                CopyFromPlain(obj);
            }
        }

        /// <summary>Copies data from the current DTO to an object of type Type.
        /// Object is created anew if necessary by using the CreateObject() method.</summary>
        /// <param name="obj">Object to which data is copied.</param>
        public virtual void CopyTo(ref Type obj)
        {
            if (GetNull())
                obj = null;
            else
            {
                BaseType objBase = obj;
                CopyToBase(ref objBase);
                obj = (Type) objBase;
            }
        }

        /// <summary>Copies data to the current DTO from an object of type object. 
        /// The necessary casts are performed.</summary>
        /// <param name="obj">Object whose data is copied to the current DTO.</param>
        public virtual void CopyFromObject(object obj)
        {
            // Remark: thread locking is performed in the called function.
            CopyFromBase((BaseType) obj);
        }

        /// <summary>Copies data from the current DTO to an object of type object.
        /// Object is created anew if necessary by using the CreateObject() method.
        /// The necessary casts are performed.</summary>
        /// <param name="obj">Object to which data is copied.</param>
        public virtual void CopyToObject(ref object obj)
        {
            lock (Lock)
            {
                if (GetNull())
                    obj = null;
                else
                {
                    if (obj == null)
                        obj = CreateObject();
                    BaseType objBase = (BaseType)obj;
                    CopyToBase(ref objBase);
                    obj = objBase;
                }
            }
        }


        #endregion Operation


        #region Serialization

        // REMARK: serialization methods below could be the final form, maybe they can just be uncommented.

        ///// <summary>Serializes the current DTO (Data Transfer Object) and outputs it to a stream.</summary>
        ///// <param name="serializer">Serializer used for serialization.</param>
        ///// <param name="serializationStream">Stream on which the serialized object is output.</param>
        //public virtual void Serialize(ISerializer serializer, Stream serializationStream)
        //{
        //    serializer.Serialize<SerializationDtoBase<Type, BaseType>>(this, serializationStream);
        //}

        ///// <summary>Serializes the current DTO (Data Transfer Object) and outputs it to a file.
        ///// If the specified file exists then it is overwritten.</summary>
        ///// <param name="serializer">Serializer used for serialization.</param>
        ///// <param name="inputFilePath">Path to the file that serialized object is written to.</param>
        //void Serialize(ISerializer serializer, string inputFilePath)
        //{
        //    serializer.Serialize<SerializationDtoBase<Type, BaseType>>(this, inputFilePath);
        //}

        ///// <summary>Serializes the current DTO (Data Transfer Object) and outputs it to a file.</summary>
        ///// <param name="serializer">Serializer used for serialization.</param>
        ///// <param name="inputFilePath">Path to the file that serialized object is written to.</param>
        ///// <param name="append">If true then the generated contents is appended to a file.</param>
        //void Serialize<T>(ISerializer serializer, string inputFilePath, bool append)
        //{
        //    serializer.Serialize<SerializationDtoBase<Type, BaseType>>(this, inputFilePath, append);
        //}

        ///// <summary>Serializes the current DTO (Data Transfer Object) to a string and returns it.</summary>
        ///// <param name="serializer">Serializer used for serialization.</param>
        ///// <returns>String containing the serialized object.</returns>
        //string Serialize<T>(ISerializer serializer)
        //{
        //    return serializer.Serialize<SerializationDtoBase<Type, BaseType>>(this);
        //}



        #endregion Serialization


        #region Deserialization

        // REMARK: it needs to be considered how deserialization methods are organised. 

        ///// <summary>Deserializes an object from the specified stream containing JSON representation of the object.
        ///// Deserialized object is instantiated and returned.</summary>
        ///// <typeparam name="T">Type of the deserialized object.</typeparam>
        ///// <param name="deserializationStream">Stream from which object is deserialized.</param>
        ///// <returns>Object that is deserialized from the stream.</returns>
        //void Deserialize(ISerializer serializer, Stream deserializationStream, ref Type obj)
        //{
        //    SerializationDtoBase<Type, BaseType> dto = null;
        //    dto = serializer.Deserialize<SerializationDtoBase<Type, BaseType>>(deserializationStream);
        //    dto.CopyTo(ref obj);
        //}

        ///// <summary>Deserializes an object from JSON - serialized string representation and returns it.</summary>
        ///// <typeparam name="T">Type of the deserialized object.</typeparam>
        ///// <param name="strSerialized">String containing the serialized object.</param>
        ///// <returns>Object instantiated form the serialiyed representation.</returns>
        //T DeserializeString<T>(string strSerialized);

        ///// <summary>Deserializes an object from JSON - serialized file and returns it.</summary>
        ///// <typeparam name="T">Type of the deserialized object.</typeparam>
        ///// <param name="inputFilePath">Path to thefile. File must contain the appropriately serialized 
        ///// object of the correct type.</param>
        ///// <returns>Object deserialized from the file.</returns>
        //T DeserializeFile<T>(string inputFilePath);

        #endregion Deserialization


        #region Misc

        /// <summary>Creates and returns string representation of the current DTO (data transfer object).</summary>
        /// <remarks>This is a generic method that first copies DTO to an object that it represents, and then
        /// calls the ToString() method of that object. This solution is slow, but this method usually does
        /// not need to be faster because ToString() method is usually only used for tests of this kind of 
        /// classes.</remarks>
        public override string ToString()
        {
            Type obj = null;
            this.CopyTo(ref obj);
            return obj.ToString();
        }

        ///// <summary>Base class for Data Transfer Objects (DTO).
        ///// Derived classes are used to store, transfer, serialize and deserialize objects of different types.</summary>
        ///// $A Igor Aug09;
        //public class SerializationDtoBase
        //{

        //}


        #endregion Misc

    }  // abstract class SerializationDtoBase<Type, BaseType>




    /// <summary>Interface for classes whose type information can be stored in the corresponding data transfer 
    /// objects (DTOs) when copying contents to DTOS. This enables deserialization of serialized objects that is 
    /// type dependent, without knowing in advance what is the type of serialized objects.</summary>
    public interface ITypedSerializable
    {

        Type TypeDto { get; }

    }

    public interface ISerializationDtoTyped
    {
        string TypeFullName
        { get; set; }

        string TypeAssemblyQualifiedName
        { get; set; }

    }


    public abstract class SerializationDtoTypedBase<Type, BaseType> : SerializationDtoBase<Type, BaseType>
        where BaseType: class, ITypedSerializable, new()
        where Type: class, BaseType, ITypedSerializable, new()
    {

        public SerializationDtoTypedBase(): base()
        { }

        private string _typeFullName, _typeAssemblyQualifiedName;

        /// <summary>Full name of the type of object whose data is contined by the current DTO.</summary>
        public string TypeFullName
        { get { return _typeFullName; } set { _typeFullName = value; } }

        /// <summary>Assembly qualified name of the type of object whose data is contained by the current DTO.</summary>
        public string TypeAssemblyQualifiedName
        { get { return _typeAssemblyQualifiedName; } set { _typeAssemblyQualifiedName = value; } }

        public object CreateObjectFromType()
        {
            object ret = null;

            return ret;
        }




        /// <summary></summary>
        /// <typeparam name="Type">Type for which DTO is used.</typeparam>
        /// <seealso cref="SerializationDtoBase<Type, Type>"/>
        /// $A Igor Jun09;
        public abstract class SerializationDtoTyped<CommonType> : SerializationDtoTypedBase<CommonType, CommonType>,
                ISerializationDto<CommonType>, ILockable
            where CommonType : class, ITypedSerializable, new()
        {
            public SerializationDtoTyped()
                : base()
            { }
        }


    }  // abstract class SerializationDtoTypedBase<Type, BaseType>

}