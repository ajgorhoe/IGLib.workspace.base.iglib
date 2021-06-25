// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using IG.Lib;

namespace IG.Reflection
{

    /// <summary>A <see cref="SerializationBinder"/>, defines a single mapping from the original assembly and
    /// type name to a target type. Matching of assembly and type name may be literal (as in <see cref="SerializationBinderSimple"/>) 
    /// but can be fine tunned with parameters.
    /// <para>Properties used to fine tune the behaior:</para>
    /// <para><see cref="IsAssemblyNameIgnored"/></para>
    /// <para><see cref="IsAssemblyNameIgnored"/></para>
    /// <para><see cref="IsTypeNameSimple"/></para>
    /// </summary>
    /// <remarks><para>Use <see cref="SerializationBinderCascaded"/> to combine mulltiple binders of this type in order
    /// to extend the mapping.</para></remarks>
    public class SerializationBinderElementary: SerializationBinderSimple, ISerializationBinder
    {


        public SerializationBinderElementary(string origTypeName, Type targetType) :
            this(origAssemblyName: null, isAssemblyNameSimple: false, isAssemblyNameIgnored: true, origTypeName: origTypeName,
                isTypeNameSimple: false, targetType: targetType)
        {
            IsAssemblyNameIgnored = true;
        }

        public SerializationBinderElementary(string origTypeName, bool isTypeNameSimple, Type targetType) :
            this(origAssemblyName: null, isAssemblyNameSimple: false, isAssemblyNameIgnored: true, origTypeName: origTypeName, 
                isTypeNameSimple: isTypeNameSimple, targetType: targetType)
        { }

        public SerializationBinderElementary(string origAssemblyName, bool isAssemblyNameSimple, bool isAssemblyNameIgnored, string origTypeName, bool isTypeNameSimple, Type targetType):
            base(origAssemblyName, origTypeName, targetType)
        {
            IsAssemblyNameSimple = isAssemblyNameSimple;
            IsAssemblyNameIgnored = isAssemblyNameIgnored;
            IsTypeNameSimple = isTypeNameSimple;
        }

        /// <summary>Constructor, specifies internally stored parameters for mapping serialized assembly
        /// and type name to deserialized <see cref="Type"/>.</summary>
        /// <param name="origAssemblyName">Assembly name as found in serialized object, stored to the
        /// <see cref="SerializationBinderSimple.OriginalAssemblyName"/> property.</param>
        /// <param name="origTypeName">Type name as found in serialized object, stored to the
        /// <see cref="SerializationBinderSimple.OriginalTypeName"/> property.</param>
        /// <param name="targetType">The type into which deserialization is performed when the 
        /// <paramref name="origAssemblyName"/> and <paramref name="origTypeName"/> are matched, stored
        /// to the <see cref="SerializationBinderSimple.TargetType"/> property.</param>
        public SerializationBinderElementary(string origAssemblyName, string origTypeName, Type targetType):
            base(origAssemblyName, origTypeName, targetType)
        { }

        ///// <summary>Stores the original assembly name of the serialized type defined in the mapping. Must be verbatm,
        ///// as it appears in the serialized object.</summary>
        //public string OriginalAssemblyName { get; }

        ///// <summary>Contains the original type name of the serialized type defined in the mapping. Must be verbatm,
        ///// as it appears in the serialized object.</summary>
        //public string OriginalTypeName { get; }

        ///// <summary>Target type of the mapping. When matching combination of <see cref="OriginalAssemblyName"/> and 
        ///// <see cref="OriginalTypeName"/> are matched in the serialized stream, this part will be deserialized to this type.</summary>
        //public Type TargetType { get; }

        public bool IsAssemblyNameSimple { get; protected set; } = false;

        public bool IsAssemblyNameIgnored { get; protected set; } = false;

        public bool IsTypeNameSimple { get; protected set; } = false;


        /// <summary>Defines a single mapping from the original assembly and type name to the target type. 
        /// Matching of assembly and type name is verbatim and case sensitive.</summary>
        /// <param name="assemblyName">Assembly name as found in serialized object, must match the 
        /// <see cref="SerializationBinderSimple.OriginalAssemblyName"/> property verbatim in order to take effect (meaning that 
        /// the property must provide a full assembly name).</param>
        /// <param name="typeName">Type name as found in serialized object, must match the 
        /// <see cref="SerializationBinderSimple.OriginalTypeName"/> property verbatim to take effect (meaning that the property 
        /// must provide a namespaced type name).</param>
        /// <returns>null when <paramref name="assemblyName"/> and <paramref name="typeName"/> arguments don't
        /// match the corresponding interna properties (<see cref="OriginalAssemblyName"/> and <see cref="OriginalTypeName"/>),
        /// or the mapped type (value of property <see cref="TargetType"/>) when matching occurs.</returns>
        public override Type BindToType(string assemblyName, string typeName)
        {

            bool assemblyNameMatched = false;
            bool typeNameMatched = false;
            // Check assembly name matching:
            if (IsAssemblyNameIgnored)
            {
                assemblyNameMatched = true;
            }
            else if (assemblyName == OriginalAssemblyName)
            {
                assemblyNameMatched = true;
            } 
            else if (IsAssemblyNameSimple)
            {
                try
                {
                    if (assemblyName.Split(',')[0] == OriginalAssemblyName.Split(',')[0])
                    {
                        assemblyNameMatched = true;
                    }
                }
                catch
                {  }
            }
            // Check type name matching:
            if (typeName == OriginalTypeName)
            {
                typeNameMatched = true;
            } 
            else if (IsTypeNameSimple)
            {
                try
                {
                    typeNameMatched = typeName.Split('.').Last() == OriginalTypeName.Split('.').Last();
                }
                catch { }
            }
            if (typeName == OriginalTypeName && assemblyName == OriginalAssemblyName)
            {
                assemblyNameMatched = true;
            }
            if (assemblyNameMatched && typeNameMatched)
            {
                if (TargetType == null)
                {
                    throw new InvalidOperationException($"The {nameof(TargetType)} proerty is not specified on {nameof(SerializationBinderSimple)}." + Environment.NewLine
                        + $"  Original type: {OriginalTypeName}, assembly name: {OriginalAssemblyName}.");
                }
                return TargetType;
            }
            return null;
        }

    }

}

