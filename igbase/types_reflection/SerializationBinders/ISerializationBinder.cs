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
    /// <remarks><para>This is a placeholder interface. It does not add anything to what is already contained in the framework's base
    /// class <see cref="SerializationBinder"/>, and omits the <see cref="SerializationBinder.BindToName(Type, out string, out string)"/>.
    /// The interface may be extended in the future to enable some additionall manipulation, e.g. specify how names are matched (such
    /// as whether only simple assembly name is matched or it is matched at all, whether the same as original type name is used, whether
    /// type name must be namespaced, etc. For now, this is not used.</para></remarks>
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
