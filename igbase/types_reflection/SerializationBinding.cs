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
    public abstract class SerializationBinderBase: SerializationBinder, ISerializationBinder
    {

    }

    public class SerializationBinderBasic: SerializationBinderBase, ISerializationBinder
    {

        public SerializationBinderBasic(string originalTypeName, Type targetType) :
            this(null, originalTypeName, targetType)
        {  }


        public SerializationBinderBasic(string originalAssemblyName, string originalTypeName, Type targetType)
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
                    throw new InvalidOperationException($"The {nameof(TargetType)} proerty is not specified on {nameof(SerializationBinderBasic)}." + Environment.NewLine
                        + $"  Original type: {OriginalTypeName}, assembly name: {OriginalAssemblyName}.");
                }
                return TargetType;
            }
            return null;
        }

    }

    public class SerializationBinderCascaded : SerializationBinderCascadedImmutable, ISerializationBinder
    {

        public SerializationBinderCascaded(params SerializationBinderBase[] binders) : base(binders)
        {  }

        public SerializationBinderCascaded(IEnumerable<SerializationBinderBase> binders) : base(binders)
        {  }

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

        public SerializationBinderCascadedImmutable(IEnumerable<SerializationBinderBase> binders): this(binders.ToArray())
        {
        }


        private List<SerializationBinderBase> ContainedBinders = new List<SerializationBinderBase>();

        protected void AddBindersInternal(params SerializationBinderBase[] binders)
        {
            if (binders!=null)
            {
                foreach (SerializationBinderBase binder in binders)
                {
                    ContainedBinders.Add(binder);
                }
            }
        }

        protected List<SerializationBinderBasic> BindingInformatin { get; }


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
