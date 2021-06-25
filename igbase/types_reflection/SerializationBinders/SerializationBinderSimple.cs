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

    /// <summary>A simple <see cref="SerializationBinder"/>, defines a single mapping from the original assembly and
    /// type name to a target type. Matching of assembly and type name is literal and case sensitive.
    /// <para>Use <see cref="SerializationBinderCascaded"/> to combine mulltiple binders of this type in order
    /// to extend the mapping.</para></summary>
    public class SerializationBinderSimple: SerializationBinderBase, ISerializationBinder
    {


        /// <summary>Constructor, specifies internally stored parameters for mapping serialized assembly
        /// and type name to deserialized <see cref="Type"/>.</summary>
        /// <param name="origAssemblyName">Assembly name as found in serialized object, stored to the
        /// <see cref="OriginalAssemblyName"/> property.</param>
        /// <param name="origTypeName">Type name as found in serialized object, stored to the
        /// <see cref="OriginalTypeName"/> property.</param>
        /// <param name="targetType">The type into which deserialization is performed when the 
        /// <paramref name="origAssemblyName"/> and <paramref name="origTypeName"/> are matched.</param>
        public SerializationBinderSimple(string origAssemblyName, string origTypeName, Type targetType)
        {
            if (targetType == null)
            {
                if (!(string.IsNullOrEmpty(origAssemblyName) && string.IsNullOrEmpty(origTypeName)))
                    throw new ArgumentNullException("Target type for serialization binding is not specified (null refeeence).");
            }
            OriginalAssemblyName = origAssemblyName;
            OriginalTypeName = origTypeName;
            TargetType = targetType;
        }

        /// <summary>Stores the original assembly name of the serialized type defined in the mapping. Must be verbatm,
        /// as it appears in the serialized object.</summary>
        public string OriginalAssemblyName { get; }

        /// <summary>Contains the original type name of the serialized type defined in the mapping. Must be verbatm,
        /// as it appears in the serialized object.</summary>
        public string OriginalTypeName { get; }

        /// <summary>Target type of the mapping. When matching combination of <see cref="OriginalAssemblyName"/> and 
        /// <see cref="OriginalTypeName"/> are matched in the serialized stream, this part will be deserialized to this type.</summary>
        public Type TargetType { get; }


        /// <summary>Defines a single mapping from the original assembly and type name to the target type. 
        /// Matching of assembly and type name is verbatim and case sensitive.</summary>
        /// <param name="assemblyName">Assembly name as found in serialized object, must match the 
        /// <see cref="OriginalAssemblyName"/> property verbatim in order to take effect (meaning that 
        /// the property must provide a full assembly name).</param>
        /// <param name="typeName">Type name as found in serialized object, must match the 
        /// <see cref="OriginalTypeName"/> property verbatim to take effect (meaning that the property 
        /// must provide a namespaced type name).</param>
        /// <returns>null when <paramref name="assemblyName"/> and <paramref name="typeName"/> arguments don't
        /// match the corresponding interna properties (<see cref="OriginalAssemblyName"/> and <see cref="OriginalTypeName"/>),
        /// or the mapped type (value of property <see cref="TargetType"/>) when matching occurs.</returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName == OriginalTypeName && assemblyName == OriginalAssemblyName)
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

