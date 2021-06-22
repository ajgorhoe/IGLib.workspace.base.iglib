//// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using IG.Lib;


//namespace IG.Reflection
//{

//    /// <summary>Registry of derived types in a certain type hierarchy.
//    /// <para>The registry of derived types provides services such as registration of types under specific type names,
//    /// conversion of registered type names to the appropriate <see cref="Type"/> objects (such that they can be used in
//    /// reflection, e.g. for creation of object instances), creation of object instances for types with parameterless
//    /// constructor, etc.</para>
//    /// <para>Typically, a registry will be implemented as static property in some base class.</para></summary>
//    /// <typeparam name="BaseType"></typeparam>
//    public class DerivedTypesRegistryPlain<BaseType>: ILockable
//    {



//        public virtual object Lock { get; } = new object();

//        private Dictionary<string, Type> DerivedTypeNames { get; } = new Dictionary<string, Type>();

//        /// <summary>Registers the ype <paramref name="derivedType"/> under name <paramref name="typeName"/>.
//        /// <para>Type must be assignable to <typeparamref name="BaseType"/>. Each specific type can be registered
//        /// with several type names. It is advisable to use <see cref="RegisterDerivedType(Type)"/> or <see cref="RegisterDerivedType{TApproximator}"/>
//        /// for type registration because these methods register the type under its actual names (simple, fully qualified and assembly 
//        /// qualified).</para></summary>
//        /// <param name="derivedType">Type that is registered. It must be assissignable to <paramref name="derivedType"/>.</param>
//        /// <param name="typeName">Name under which the type is registered, case-sensitive. Any type can be registered under several names.</param>
//        public virtual void RegisterDerivedType(Type derivedType, string typeName)
//        {
//            if (string.IsNullOrWhiteSpace(typeName))
//                throw new ArgumentException("The neural approximator type name to be registered is not valid (null of empty or whitespace string).");
//            if (derivedType == null)
//                throw new ArgumentNullException(nameof(derivedType), "Approximator type to registe is not specified (null reference).");
//            if (!typeof(BaseType).IsAssignableFrom(derivedType))
//            {
//                throw new InvalidOperationException($"Cannot register neural approximator type {derivedType.FullName}: not an {nameof(BaseType)}.");
//            }
//            lock (Lock)
//            {
//                DerivedTypeNames[typeName] = derivedType;
//            }
//        }


//        /// <summary>Registeers the specified type (<paramref name="derivedType"/>) under its simple, fully specified and assembly specified names
//        /// by calling the <see cref="RegisterDerivedType(Type, string)"/></summary>
//        /// <param name="derivedType">Type that is registered.</param>
//        public virtual void RegisterDerivedType(Type derivedType)
//        {
//            if (derivedType == null)
//            {
//                throw new ArgumentNullException(nameof(derivedType), "Approximator type is not specified (nulll reference).");
//            }
//            RegisterDerivedType(derivedType, derivedType.FullName);
//            RegisterDerivedType(derivedType, derivedType.Name);
//            RegisterDerivedType(derivedType, derivedType.AssemblyQualifiedName);
//        }

//        /// <summary>Registeers the specified type (<typeparamref name="TApproximator"/>) under its simple, fully specified and assembly specified names
//        /// by calling the <see cref="RegisterDerivedType(Type, string)"/> via <see cref="RegisterDerivedType(Type)"/>.</summary>
//        /// <typeparam name="TApproximator">Type that is registered, must be assignable to <typeparamref name="BaseType"/>.</typeparam>
//        public virtual void RegisterDerivedType<TApproximator>()
//            where TApproximator : BaseType
//        {
//            RegisterDerivedType(typeof(TApproximator));
//        }

//        /// <summary>Tries to resolve and return the type that corresponds to the specified type name (<paramref name="derivedTypeName"/>).
//        /// <para>The method first tries to establish the type corresponding to its name by looking into registered types in the 
//        /// <see cref="DerivedTypeNames"/> registry. If not successful, it attempts to use reflection (<see cref="Type.GetType(string)"/>).</para></summary>
//        /// <param name="derivedTypeName">Tape name.</param>
//        /// <param name="throwIfCannotCreate">If true then exception is thrown when the type cannot be resolved. If false then nulll is returned instead.
//        /// Default is true.</param>
//        /// <returns>The type that corresponds to the specified type name.</returns>
//        public virtual Type GetDerivedType(string derivedTypeName, bool throwIfCannotCreate = true)
//        {
//            if (DerivedTypeNames.ContainsKey(derivedTypeName))
//            {
//                return DerivedTypeNames[derivedTypeName];
//            }
//            try
//            {
//                Type derivedType = Type.GetType(derivedTypeName);
//                if (derivedType == null)
//                {
//                    throw new InvalidOperationException($"Could not find a neural network approximator type named {derivedTypeName}.");
//                }
//                if (!derivedType.IsAssignableFrom(typeof(BaseType)))
//                {
//                    throw new InvalidOperationException($"The type name {derivedTypeName} resolved to {derivedType.FullName}: NOT an {nameof(BaseType)}.");
//                }
//            }
//            catch
//            {
//                if (throwIfCannotCreate)
//                    throw;
//            }
//            return null;
//        }

//        /// <summary>Attempts to create and return an instance of the type specified by its name (<paramref name="derivedTypeName"/>).
//        /// It first calls <see cref="GetDerivedType(string, bool)"/> to get the type corresponding to <paramref name="derivedTypeName"/>, 
//        /// then it uses reflection to invoke parameter-less constructor to create an instance. If parameterless constructor is not 
//        /// defined on the type then the dmethod will fail.</summary>
//        /// <param name="derivedTypeName">Nae of the type whose instance is created and returned.</param>
//        /// <param name="throwIfCannotCreate">If true then exception is thrown when the type cannot be resolved. If false then nulll is returned instead.
//        /// Default is true.</param>
//        /// <returns>Insance of the type whose name is provided, if successful. If not, either null is returned or exception thrown, dependent
//        /// on value of <paramref name="throwIfCannotCreate"/></returns>
//        public virtual BaseType CreateInstance(string derivedTypeName, bool throwIfCannotCreate = true)
//        {
//            try
//            {
//                Type derivedType = GetDerivedType(derivedTypeName, throwIfCannotCreate);
//                if (derivedType == null)
//                {
//                    throw new InvalidOperationException($"Could not resolve a neural approximatior type name {derivedTypeName}.");
//                }
//                return (BaseType)Activator.CreateInstance(derivedType);
//            }
//            catch
//            {
//                if (throwIfCannotCreate)
//                    throw;
//            }
//            return default(BaseType);
//        }



//    }
//}
