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

    /// <summary>A cascaded <see cref="SerializationBinder"/> class that maps the assembly and type information of the
    /// serialized objects into classes into which objects shoulld be deserialized, based on definitions from a collection
    /// of the contained elementary <see cref="ISerializationBinder"/> classes. 
    /// Similar to <see cref="SerializationBinderCascadedImmutable"/>, except that binders can also be added (at the end of the
    /// list) after creation.</summary>
    /// <seealso cref="SerializationBinderCascadedImmutable"/>
    public class SerializationBinderCascaded : SerializationBinderCascadedImmutable, ISerializationBinder
    {

        /// <summary>Calls base class' <see cref="SerializationBinderCascadedImmutable(SerializationBinderBase[])"/>
        /// to construct the object.</summary>
        public SerializationBinderCascaded(params SerializationBinderBase[] binders) : base(binders)
        { }

        /// <summary>Calls base class <see cref="SerializationBinderCascadedImmutable(IEnumerable{SerializationBinderBase})"/>
        /// to construct the object.</summary>
        public SerializationBinderCascaded(IEnumerable<SerializationBinderBase> binders) : base(binders)
        { }

        /// <summary>Adds the specified serialization binders at the end of the ordered set of contained binders, 
        /// and returns the current  object to prrovide a fluent interface.</summary>
        /// <param name="binders">Serialization binders (of type <see cref="SerializationBinderBase"/>) that will be 
        /// contained in the constructed serialization binder (can be stated as an array, as comma-separated arguments of
        /// variable number, or as enoty argument to include no builders).</param>
        /// <returns>The current binder object (to provide fluent interface).</returns>
        /// <seealso cref="SerializationBinderCascadedImmutable.AddBindersInternal(SerializationBinderBase[])"/>
        public virtual SerializationBinderCascaded Add(params SerializationBinderBase[] binders)
        {
            AddBindersInternal(binders);
            return this;
        }

        /// <summary>Adds the specified binders at the end of the ordered set of contained binders,
        /// and returns the current  object to prrovide a fluent interface.</summary>
        /// <param name="binders">Collection of serialization binders (of type <see cref="SerializationBinderBase"/>) that will be 
        /// contained in the constructed serialization binder.</param>
        /// <seealso cref="SerializationBinderCascadedImmutable.AddBindersInternal(IEnumerable{SerializationBinderBase})"/>
        public SerializationBinderCascaded Add(IEnumerable<SerializationBinderBase> binders)
        {
            AddBindersInternal(binders);
            return this;
        }


    }


    /// <summary>A cascaded <see cref="SerializationBinder"/> class that maps the assembly and type information of the
    /// serialized objects into classes into which objects shoulld be deserialized, based on definitions from a collection
    /// of the contained elementary <see cref="ISerializationBinder"/> classes. For each pair of assembly name / type name
    /// presented to this object by calling the <see cref="BindToType(string, string)"/> method (see also method description),
    /// the call is forwarded to the same method of each contained serialization binder object until one of them returns a 
    /// mapped type. Else (if all calls return null), null is returned as the mapped type, meaning that there is no user-defined
    /// mapping from assembly and type name to a type, in which case deserializer provides the default binding.
    /// <para>This serialization binder can contain other cascaded binders, allowing for arbitrary tree-like cascading
    /// of binders. Orter in which binders are added specifies the order in which the contained binders are tested whether
    /// they canb provide the type for given assembly name / type name combination.</para>
    /// <para>New binders cannot be added after construction. To avoid this limitation, use the <see cref="SerializationBinderCascaded"/>
    /// class.</para></summary>
    public class SerializationBinderCascadedImmutable : SerializationBinderBase, ISerializationBinder
    {

        /// <summary>Constructs the serialization bider with all the specified binders added.</summary>
        /// <param name="binders">Serialization binders (of type <see cref="SerializationBinderBase"/>) that will be 
        /// contained in the constructed serialization binder (can be stated as an array, as comma-separated arguments of
        /// variable number, or as enoty argument to include no builders).</param>
        public SerializationBinderCascadedImmutable(params SerializationBinderBase[] binders)
        {
            AddBindersInternal(binders);
        }

        /// <summary>Constructs the serialization bider with all the specified binders added.</summary>
        /// <param name="binders">Collection of serialization binders (of type <see cref="SerializationBinderBase"/>) that will be 
        /// contained in the constructed serialization binder.</param>
        public SerializationBinderCascadedImmutable(IEnumerable<SerializationBinderBase> binders) : this(binders.ToArray())
        {
            AddBindersInternal(binders);
        }


        /// <summary>List of contained serialization binders.</summary>
        private List<SerializationBinderBase> ContainedBindersPrivate { get; } = new List<SerializationBinderBase>();

        /// <summary>Enumerable that enables iteration over contained serialization binders (elements of <see cref="ContainedBindersPrivate"/>).</summary>
        protected IEnumerable<SerializationBinder> ContainedBinders { get { return ContainedBindersPrivate; } }

        /// <summary>Adds the specified binders at the end of the ordered set of contained binders (<see cref="ContainedBindersPrivate"/>,
        /// accessible via <see cref="ContainedBinders"/>).</summary>
        /// <param name="binders">Serialization binders (of type <see cref="SerializationBinderBase"/>) that will be 
        /// contained in the constructed serialization binder (can be stated as an array, as comma-separated arguments of
        /// variable number, or as enoty argument to include no builders).</param>
        protected void AddBindersInternal(params SerializationBinderBase[] binders)
        {
            if (binders != null)
            {
                foreach (SerializationBinderBase binder in binders)
                {
                    ContainedBindersPrivate.Add(binder);
                }
            }
        }

        /// <summary>Adds the specified binders at the end of the ordered set of contained binders (<see cref="ContainedBindersPrivate"/>,
        /// accessible via <see cref="ContainedBinders"/>).</summary>
        /// <param name="binders">Collection of serialization binders (of type <see cref="SerializationBinderBase"/>) that will be 
        /// contained in the constructed serialization binder.</param>
        protected void AddBindersInternal(IEnumerable<SerializationBinderBase> binders)
        {
            if (binders != null)
            {
                foreach (SerializationBinderBase binder in binders)
                {
                    ContainedBindersPrivate.Add(binder);
                }
            }
        }


        /// <summary>Specifies the mapping of <paramref name="assemblyName"/> and <paramref name="typeName"/> of the serialized 
        /// object into a type of an object into which it deserializes. This mapping is defined by a collection of elementary
        /// <see cref="ISerializationBinder"/> objects derived from <see cref="SerializationBinderBase"/> class, each of
        /// which defines its own mapping. The overall mapping is defined by this method in such a way that it iterates
        /// (in order of elements) over all serialization binders on the <see cref="ContainedBindersPrivate"/>, and returns as target
        /// type the first non-null <see cref="Type"/> object returned by the <see cref="ISerializationBinder.BindToType(string, string)"/>
        /// method returned by any of the objects contained in <see cref="ContainedBindersPrivate"/> called with the same parameters 
        /// <paramref name="assemblyName"/> and <paramref name="typeName"/>.</summary>
        /// <param name="assemblyName">Assembly name of the serialized object that is matched and combined with <paramref name="typeName"/>
        /// in order </param>
        /// <param name="typeName">Type name of the deserialized object that is matched and combined with <paramref name="assemblyName"/>
        /// in order to determine the type of object into which the given serialized object deseriallies.</param>
        /// <returns>The type into which the serialized oject with the specified <paramref name="assemblyName"/> and <paramref name="typeName"/>
        /// shoulld deserialize.</returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type ret = null;
            foreach (SerializationBinderBase binder in ContainedBinders)
            {
                if (binder != null)
                {
                    Type matchedBoundType = binder.BindToType(assemblyName, typeName);
                    if (matchedBoundType != null)
                    {
                        // Console.WriteLine($"Binding succeeded: type name: {typeName}, assembly name: {assemblyName}, type: {matchedBoundType.FullName}");
                        ret = matchedBoundType;
                        break;
                    }
                }
            }
            // Console.WriteLine($"Binding not succeeded: type name: {typeName}, assembly name: {assemblyName}");
            return ret;
        }

    }



}
