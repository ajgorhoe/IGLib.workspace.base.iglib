// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IG.Lib;


namespace IG.Reflection
{



    /// <summary>Registry of type data for types derived from <typeparamref name="BaseType"/>.
    /// <para>For each type in the registry, an <typeparamref name="InfoType"/> object is registered that contains
    /// data and eventually some utilities related to that type, including the corresponding <see cref="Type"/> object.</para>
    /// <para>Type can be registered under one or more type names, representing access key to the correspongding type info.</para>
    /// <para>The registry of types provides services such as registration of type data under the specified type name(s),
    /// conversion of registered type names to the appropriate <see cref="Type"/> objects (such that they can be used in
    /// reflection, e.g. for creation of object instances), creation of object instances for types with parameterless
    /// constructor, etc.</para>
    /// <para>Typically, a registry will be implemented as static property in some base class.</para></summary>
    /// <typeparam name="BaseType">Base type of the types that can be registered in the current registry.</typeparam>
    /// <typeparam name="InfoType">Type of objects that hold information on the registered type and eventually some utilities
    /// for that type. Must derive from <see cref="TypeInfoBase"/>.</typeparam>
    public class DerivedTypesRegistry<BaseType, InfoType>: ILockable
        where InfoType: TypeInfoBase
    {

        public DerivedTypesRegistry()
        {  }

        public virtual object Lock { get; } = new object();

        private Dictionary<string, InfoType> DerivedTypeNames { get; } = new Dictionary<string, InfoType>();

        /// <summary>Registers the type information <paramref name="typeData"/> under the name <paramref name="typeName"/>.
        /// <para>Type (<see cref="TypeInfoBase.ClassType"/>) must be derived from (or equal to) <typeparamref name="BaseType"/>. 
        /// Each specific type can be registered with several type names.</para>
        /// <para>It is preeferable to use <see cref="Register(InfoType)"/> for type registration because this method registers 
        /// the type under its actual names (simple, fully qualified and assembly qualified).</para></summary>
        /// <param name="typeName">Name under which the type info is registered, case-sensitive. Any type can be registered under 
        /// several names.</param>
        /// <param name="typeData">Type information that is registered. It must be assissignable to <paramref name="typeData"/>.</param>
        public virtual void Register(string typeName, InfoType typeData)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException("The type name to be registered is not valid (null of empty or whitespace string).");
            if (typeData == null)
                throw new ArgumentNullException(nameof(typeData), "The type data to register is not specified (null reference).");
            Type registeredClassType = typeData.ClassType;
            if (!typeof(BaseType).IsAssignableFrom(registeredClassType))
            {
                throw new InvalidOperationException($"Cannot register the type {registeredClassType.FullName}: not a {nameof(BaseType)}.");
            }
            lock (Lock)
            {
                DerivedTypeNames[typeName] = typeData;
            }
        }


        /// <summary>Registeers the specified type information (<paramref name="typeData"/>) under the registered type's 
        /// (<see cref="TypeInfoBase.ClassType"/>) simple, fully qualidfied and assembly qualified names by calling the 
        /// <see cref="Register(string, InfoType)"/></summary>
        /// <param name="typeData">Type data that is registered.</param>
        public virtual void Register(InfoType typeData)
        {
            if (typeData == null)
            {
                throw new ArgumentNullException(nameof(typeData), "Type data is not specified (nulll reference).");
            }
            Type registeredClassType = typeData.ClassType;
            lock (Lock)
            {
                Register(registeredClassType.FullName, typeData);
                Register(registeredClassType.Name, typeData);
                Register(registeredClassType.AssemblyQualifiedName, typeData);
            }
        }


        /// <summary>Tries to resolve and return the type data object that corresponds to the specified type name (<paramref name="typeName"/>).
        /// <para>The method tries to establish the type corresponding to its name by looking into registered types in the 
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
        /// <para>The method first tries to establish the type corresponding to the specified name by looking into registered types in the 
        /// <see cref="DerivedTypeNames"/> registry. If not successful, it attempts to use reflection (<see cref="Type.GetType(string)"/>).</para></summary>
        /// <param name="derivedTypeName">Name of the type for which a corresponding <see cref="Type"/> object is looked for.
        /// Default is true.</param>
        /// <param name="tryGetIfNotRegistered">If true then the method tries to infer the type from the specified <paramref name="derivedTypeName"/>
        /// even if the type information is not registered under <paramref name="derivedTypeName"/>.</param>
        /// <returns>The type that corresponds to the specified type name.</returns>
        /// <param name="throwIfCannotResolve">If true then exception is thrown when the type name cannot be resolved. If false then nulll is returned instead.</param>
        public virtual Type GetType(string derivedTypeName, bool tryGetIfNotRegistered = false, bool throwIfCannotResolve = true)
        {
            InfoType info = GetTypeData(derivedTypeName, throwIfCannotResolve = false);
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
                    if (throwIfCannotResolve)
                        throw;
                }
            }
            if (throwIfCannotResolve)
                return null;
            throw new InvalidOperationException($"Could not infer object type from type name {derivedTypeName}");
        }


        /// <summary>Attempts to create and return an instance of the type specified by its name (<paramref name="derivedTypeName"/>).
        /// It first calls <see cref="GetType(string, bool, bool)"/> to get the type corresponding to <paramref name="derivedTypeName"/>, 
        /// then it uses reflection to invoke parameter-less constructor to create an instance. If parameterless constructor is also 
        ///  defined on the type then the method fails.</summary>
        /// <param name="derivedTypeName">Name of the type whose instance should be created and returned.</param>
        /// <param name="throwIfCannotCreate">If true then exception is thrown when the object of the specified type cannot be resolved. 
        /// If false then nulll is returned instead. Default is true.</param>
        /// <returns>Instance of the type whose name is provided, if successful. If not, either null is returned or exception thrown, dependent
        /// on the value of <paramref name="throwIfCannotCreate"/></returns>
        public virtual BaseType CreateInstance(string derivedTypeName, bool throwIfCannotCreate = true)
        {
            try
            {
                Type derivedType = GetType(derivedTypeName, true, throwIfCannotCreate);
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
