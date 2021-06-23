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

    public class SerializationBinderPlain: SerializationBinderBase, ISerializationBinder
    {

        public SerializationBinderPlain(string originalTypeName, Type targetType) :
            this(null, originalTypeName, targetType)
        {  }


        public SerializationBinderPlain(string originalAssemblyName, string originalTypeName, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("Target type for serialization binding is not specified (null refeeence).");
            }
            if (originalAssemblyName == null)
            {
                originalAssemblyName =Assembly.GetExecutingAssembly().FullName;
            }
            OriginalAssemblyName = originalAssemblyName;
            OriginalTypeName = originalTypeName;
            TargetType = targetType;
        }

        public string OriginalAssemblyName { get; }

        public string OriginalTypeName { get; }

        public Type TargetType { get; }


        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName == OriginalTypeName && assemblyName == OriginalAssemblyName)
            {
                if (TargetType == null)
                {
                    throw new InvalidOperationException($"The {nameof(TargetType)} proerty is not specified on {nameof(SerializationBinderPlain)}." + Environment.NewLine
                        + $"  Original type: {OriginalTypeName}, assembly name: {OriginalAssemblyName}.");
                }
                return TargetType;
            }
            return null;
        }

    }


}
