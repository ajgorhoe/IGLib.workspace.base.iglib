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

    public class SerializationBinderCascaded : SerializationBinderCascadedImmutable, ISerializationBinder
    {

        public SerializationBinderCascaded(params SerializationBinderBase[] binders) : base(binders)
        { }

        public SerializationBinderCascaded(IEnumerable<SerializationBinderBase> binders) : base(binders)
        { }

        public virtual SerializationBinderCascaded AddSerializationBinders(params SerializationBinderBase[] binders)
        {
            AddBindersInternal(binders);
            return this;
        }

    }


    public class SerializationBinderCascadedImmutable : SerializationBinderBase, ISerializationBinder
    {


        public SerializationBinderCascadedImmutable(params SerializationBinderBase[] binders)
        {
            AddBindersInternal(binders);
        }

        public SerializationBinderCascadedImmutable(IEnumerable<SerializationBinderBase> binders) : this(binders.ToArray())
        {
        }


        private List<SerializationBinderBase> ContainedBinders = new List<SerializationBinderBase>();

        protected void AddBindersInternal(params SerializationBinderBase[] binders)
        {
            if (binders != null)
            {
                foreach (SerializationBinderBase binder in binders)
                {
                    ContainedBinders.Add(binder);
                }
            }
        }

        protected List<SerializationBinderPlain> BindingInformatin { get; }


        public override Type BindToType(string assemblyName, string typeName)
        {
            foreach (SerializationBinderBase binder in ContainedBinders)
            {
                if (binder != null)
                {
                    Type matchedBoundType = binder.BindToType(assemblyName, typeName);
                    if (matchedBoundType != null)
                    {
                        // Console.WriteLine($"Binding succeeded: type name: {typeName}, assembly name: {assemblyName}, type: {matchedBoundType.FullName}");
                        return matchedBoundType;
                    }
                }
            }
            // Console.WriteLine($"Binding not succeeded: type name: {typeName}, assembly name: {assemblyName}");
            return null;
        }

    }




}
