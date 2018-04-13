using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Quanta.Core.Data
{
    /// <summary>
    /// An unordered Set of data that will typically be used as a whole.
    /// </summary>
    /// <remarks>Note: implementations of this class should use a custom IEqualityComparer</remarks>
    public class Set<TData, TMeta> : HashSet<TData>
        where TMeta:IMetaData
    {
        public Set() { }
        /// <param name="capacity"></param>
        public Set(int capacity) : base(capacity) { }
        public Set(int capacity, IEqualityComparer<TData> comparer) : base(capacity, comparer) { }
        public Set(IEnumerable<TData> collection) : base(collection) { }
        public Set(IEnumerable<TData> collection, IEqualityComparer<TData> comparer) : base(collection, comparer) { }
        public Set(IEqualityComparer<TData> comparer) : base(comparer) { }
        protected Set(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public virtual TMeta MetaData { get; set; }

        public void Append(Set<TData, TMeta> items) => UnionWith(items);

        public void Remove(Set<TData, TMeta> items)
        {
            foreach (TData item in items) Remove(item);
        }

        public void Update(Set<TData, TMeta> items)
        {
            Remove(items);
            Append(items);
        }
    }
}

