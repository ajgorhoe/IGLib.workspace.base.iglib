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

    /// <summary>Interface implemented by IGLib's classes based on the framework's <see cref="SerializationBinder"/> class.</summary>
    public interface ISerializationBinder
    {
        Type BindToType(string assemblyName, string typeName);
    }

    /// <summary>Enables to reference classes that both inherit from the framework's <see cref="SerializationBinder"/>
    /// (necessary for assigning the <see cref="IFormatter.Binder"/> property) and implement the <see cref="ISerializationBinder"/>
    /// interface.</summary>
    public abstract class SerializationBinderBase : SerializationBinder, ISerializationBinder
    {
    }

}
