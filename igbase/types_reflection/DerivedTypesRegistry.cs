// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IG.Lib;


namespace IG.Reflection
{


    /// <summary>Base class for type information classes that are used with the <see cref="DerivedTypesRegistry{BaseType, InfoType}"/>.
    /// It has the <see cref="ClassType"/> property, which is the minimal information necessary.</summary>
    public class TypeInfoBase
    {

        public TypeInfoBase(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "Type is not specified (null referene).");
            }
            ClassType = type;
        }

        public Type ClassType { get; protected set; }

    }


    /// <summary>Derived types registry, similar to <see cref="DerivedTypesRegistry{BaseType, TypeInfoBase}"/>, but with
    /// added registration method base on type.</summary>
    /// <typeparam name="BaseType">Base type for types that can be registered in the current registry.</typeparam>
    public class DerivedTypeRgistryBasic<BaseType> : DerivedTypesRegistry<BaseType, TypeInfoBase>,
        ILockable
    {


        /// <summary>Creates the type data objject of type <typeparamref name="InfoType"/> for the specified <see cref="Type"/>
        /// <paramref name="classType"/>.
        /// <para>This method is used in <see cref="RegisterDerivedType(InfoType)"/></para></summary>
        /// <param name="classType"></param>
        protected virtual TypeInfoBase CreateTypeData(Type classType)
        {
            return new TypeInfoBase(classType);
        }

        /// <summary>Registeers the specified type (<typeparamref name="TApproximator"/>) under its simple, fully specified and assembly specified names
        /// by calling the <see cref="RegisterDerivedType(TypeInfoBase, string)"/>.
        /// <see cref="RegisterDerivedType(TypeInfoBase, string)"/> via <see cref="RegisterDerivedType(InfoType)"/>.
        /// <para>This method can onnly be used if <see cref="CreateTypeData(Type)"/> is able to create an info object of type <typeparamref name="InfoType"/>.</para></summary>
        /// <typeparam name="TApproximator">Type that is registered, must be assignable to <typeparamref name="BaseType"/>.</typeparam>
        public virtual void RegisterDerivedType<TApproximator>()
            where TApproximator : BaseType
        {
            Register(CreateTypeData(typeof(TApproximator)));
        }

    }


    /// <summary>Registry of derived types in a certain type hierarchy.
    /// <para>The registry of derived types provides services such as registration of types under specific type names,
    /// conversion of registered type names to the appropriate <see cref="Type"/> objects (such that they can be used in
    /// reflection, e.g. for creation of object instances), creation of object instances for types with parameterless
    /// constructor, etc.</para>
    /// <para>Typically, a registry will be implemented as static property in some base class.</para></summary>
    /// <typeparam name="BaseType">Base type of the types that can be registered in the current registry.</typeparam>
    /// <typeparam name="InfoType">Type of the class that holds information on the registered type.</typeparam>
    public class DerivedTypesRegistry<BaseType, InfoType>: ILockable
        where InfoType: TypeInfoBase
    {

        public virtual object Lock { get; } = new object();

        private Dictionary<string, InfoType> DerivedTypeNames { get; } = new Dictionary<string, InfoType>();

        /// <summary>Registers the ype <paramref name="typeData"/> under name <paramref name="typeName"/>.
        /// <para>Type must be assignable to <typeparamref name="BaseType"/>. Each specific type can be registered
        /// with several type names. It is advisable to use <see cref="Register(InfoType)"/> or <see cref="RegisterDerivedType{T}"/>
        /// for type registration because these methods register the type under its actual names (simple, fully qualified and assembly 
        /// qualified).</para></summary>
        /// <param name="typeData">Type that is registered. It must be assissignable to <paramref name="typeData"/>.</param>
        /// <param name="typeName">Name under which the type is registered, case-sensitive. Any type can be registered under several names.</param>
        public virtual void Register(InfoType typeData, string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException("The type name to be registered is not valid (null of empty or whitespace string).");
            if (typeData == null)
                throw new ArgumentNullException(nameof(typeData), "The type to register is not specified (null reference).");
            Type registeredClassType = typeData.ClassType;
            if (!typeof(BaseType).IsAssignableFrom(registeredClassType))
            {
                throw new InvalidOperationException($"Cannot register neural type {registeredClassType.FullName}: not subtype of {nameof(BaseType)}.");
            }
            lock (Lock)
            {
                DerivedTypeNames[typeName] = typeData;
            }
        }


        /// <summary>Registeers the specified type (<paramref name="typeData"/>) under its simple, fully specified and assembly specified names
        /// by calling the <see cref="Register(InfoType, string)"/></summary>
        /// <param name="typeData">Type that is registered.</param>
        public virtual void Register(InfoType typeData)
        {
            if (typeData == null)
            {
                throw new ArgumentNullException(nameof(typeData), "Approximator type is not specified (nulll reference).");
            }
            Type registeredClassType = typeData.ClassType;
            lock (Lock)
            {
                Register(typeData, registeredClassType.FullName);
                Register(typeData, registeredClassType.Name);
                Register(typeData, registeredClassType.AssemblyQualifiedName);
            }
        }


        /// <summary>Tries to resolve and return the type that corresponds to the specified type name (<paramref name="typeName"/>).
        /// <para>The method first tries to establish the type corresponding to its name by looking into registered types in the 
        /// <see cref="DerivedTypeNames"/> registry. If not successful, it attempts to use reflection (<see cref="Type.GetType(string)"/>).</para></summary>
        /// <param name="typeName">Tape name.</param>
        /// <param name="throwIfCannotCreate">If true then exception is thrown when the type cannot be resolved. If false then nulll is returned instead.
        /// Default is true.</param>
        /// <returns>The type that corresponds to the specified type name.</returns>
        public virtual InfoType GetTypeData(string typeName, bool throwIfCannotCreate = true)
        {
            if (DerivedTypeNames.ContainsKey(typeName))
            {
                return DerivedTypeNames[typeName];
            }
            return null;
        }


        /// <summary>Tries to resolve and return the type that corresponds to the specified type name (<paramref name="derivedTypeName"/>).
        /// <para>The method first tries to establish the type corresponding to its name by looking into registered types in the 
        /// <see cref="DerivedTypeNames"/> registry. If not successful, it attempts to use reflection (<see cref="Type.GetType(string)"/>).</para></summary>
        /// <param name="derivedTypeName">Tape name.</param>
        /// <param name="throwIfCannotCreate">If true then exception is thrown when the type cannot be resolved. If false then nulll is returned instead.
        /// Default is true.</param>
        /// <param name="tryGetIfNotRegistered">If true then the method tries to infr the type from the specified <paramref name="derivedTypeName"/>
        /// even if the type information is not registered under <paramref name="derivedTypeName"/>.</param>
        /// <returns>The type that corresponds to the specified type name.</returns>
        public virtual Type GetType(string derivedTypeName, bool tryGetIfNotRegistered = false, bool throwIfCannotCreate = true)
        {
            InfoType info = GetTypeData(derivedTypeName, throwIfCannotCreate = false);
            if (info != null)
            {
                return info.ClassType;
            }
            if (tryGetIfNotRegistered)
            {
                try
                {
                    // Coulld not find a registered type information for the given type, try to infer the type vira reflection:
                    Type derivedType = Type.GetType(derivedTypeName);
                    if (derivedType == null)
                    {
                        throw new InvalidOperationException($"Could not find a neural network approximator type named {derivedTypeName}.");
                    }
                    if (!typeof(BaseType).IsAssignableFrom(derivedType))
                    {
                        throw new InvalidOperationException($"The type name {derivedTypeName} resolved to {derivedType.FullName}: NOT an {nameof(BaseType)}.");
                    }
                }
                catch
                {
                    if (throwIfCannotCreate)
                        throw;
                }
            }
            if (throwIfCannotCreate)
                return null;
            throw new InvalidOperationException($"Could not infer object type from type name {derivedTypeName}");
        }


        /// <summary>Attempts to create and return an instance of the type specified by its name (<paramref name="derivedTypeName"/>).
        /// It first calls <see cref="GetType(string, bool)"/> to get the type corresponding to <paramref name="derivedTypeName"/>, 
        /// then it uses reflection to invoke parameter-less constructor to create an instance. If parameterless constructor is not 
        /// defined on the type then the dmethod will fail.</summary>
        /// <param name="derivedTypeName">Nae of the type whose instance is created and returned.</param>
        /// <param name="throwIfCannotCreate">If true then exception is thrown when the type cannot be resolved. If false then nulll is returned instead.
        /// Default is true.</param>
        /// <returns>Insance of the type whose name is provided, if successful. If not, either null is returned or exception thrown, dependent
        /// on value of <paramref name="throwIfCannotCreate"/></returns>
        public virtual BaseType CreateInstance(string derivedTypeName, bool throwIfCannotCreate = true)
        {
            try
            {
                Type derivedType = GetType(derivedTypeName, throwIfCannotCreate);
                if (derivedType == null)
                {
                    throw new InvalidOperationException($"Could not resolve a neural approximatior type name {derivedTypeName}.");
                }
                return (BaseType)Activator.CreateInstance(derivedType);
            }
            catch
            {
                if (throwIfCannotCreate)
                    throw;
            }
            return default(BaseType);
        }


    }
}
