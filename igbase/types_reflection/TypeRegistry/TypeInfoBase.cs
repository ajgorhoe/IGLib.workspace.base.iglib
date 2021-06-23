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


}
