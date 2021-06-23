// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IG.Lib;

namespace IG.Reflection
{



    /// <summary>Registry of types with a common base type, similar to <see cref="DerivedTypesRegistry{BaseType, TypeInfoBase}"/>, 
    /// with the simple type info class <see cref="TypeInfoBase"/> and with added registration method based on type
    /// (<see cref="RegisterDerivedType{TypeToRegister}"/>). Each type in is registered with a type info object (of type <see cref="TypeInfoBase>"/> 
    /// and can be registered under differeent names (typically, simple type names and qualified names).</summary>
    /// <typeparam name="BaseType">Base type for types that can be registered in the current registry. Registered
    /// types must derive from this type.</typeparam>
    public class DerivedTypesRgistryBasic<BaseType> : DerivedTypesRegistry<BaseType, TypeInfoBase>,
        ILockable
    {

        public DerivedTypesRgistryBasic() : base()
        { }

        /// <summary>Creates the type info object of type <see cref="TypeInfoBase"/> for the specified <see cref="Type"/>
        /// <paramref name="classType"/>.
        /// <para>This method is used by the <see cref="RegisterDerivedType{TypeToRegister}"/> to create the type info object
        /// used in registration.</para></summary>
        /// <param name="classType"></param>
        protected virtual TypeInfoBase CreateTypeData(Type classType)
        {
            return new TypeInfoBase(classType);
        }

        /// <summary>Registeers the type (<typeparamref name="TypeToRegister"/>) under its simple, fully specified and 
        /// assembly specified names by calling the <see cref="DerivedTypesRegistry{BaseType, TypeInfoBase}.Register(TypeInfoBase)"/>.</summary>
        /// <typeparam name="TypeToRegister">Type that is registered, must be assignable to <typeparamref name="BaseType"/>.</typeparam>
        public virtual void RegisterDerivedType<TypeToRegister>()
            where TypeToRegister : BaseType
        {
            Register(CreateTypeData(typeof(TypeToRegister)));
        }

    }


}
