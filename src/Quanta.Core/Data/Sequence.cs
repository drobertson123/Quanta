//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Quanta.Core.Data
//{
//    //public interface ISequence<TKey, TData>

//    //{
//    //    IOrderedEnumerable<TData> GetBetween(TKey lowKey, TKey highKey);
//    //}
//    /// <summary>
//    /// An Ordered Set of data that will typically be used as sequential series of information. 
//    /// It is stored in sequence and retrieved in sequence, allowing for efficient reading of all or segments of the data in order.
//    /// </summary>
//    public class Sequence<TKey, TData, TMeta> : SortedDictionary<TKey, TData>
//        where TMeta : IMetaData
//    {
//        private Func<TData, TKey> _keyGen;
//        //private IComparer<TKey> _comparer;
//        public Sequence(Func<TData, TKey> keyGen) => _keyGen = keyGen;
//        public Sequence(IComparer<TKey> comparer, Func<TData, TKey> keyGen)
//            : base(comparer) => _keyGen = keyGen;
//        public Sequence(IDictionary<TKey, TData> dictionary, IComparer<TKey> comparer, Func<TData, TKey> keyGen)
//            : base(dictionary, comparer) => _keyGen = keyGen;

//        public virtual TMeta MetaData { get; set; }

//        public bool IsReadOnly => this.IsReadOnly;

//        public IOrderedEnumerable<TData> GetBetween(TKey lowKey, TKey highKey)
//        {
//            return Keys
//                .Where(key => Comparer.Compare(key, lowKey) > -1 && Comparer.Compare(key, highKey) < 1)
//                .Select(key => this[key])
//                .OrderBy(item => _keyGen(item));
//        }

//        public void Append(Set<TData, TMeta> items)
//        { foreach (TData item in items) if (!Contains(item)) Add(item); }

//        public void Remove(Set<TData, TMeta> items) => Remove((TKey)items.Select(_ => _keyGen(_)));

//        public void Update(Set<TData, TMeta> items)
//        {
//            Remove(items);
//            Append(items);
//        }

//        public void Add(TData item) => Add(_keyGen(item), item);

//        public bool Contains(TData item) => ContainsKey(_keyGen(item));

//        public void CopyTo(TData[] array, int arrayIndex) => Values.CopyTo(array, arrayIndex);

//        public bool Remove(TData item) => Remove(_keyGen(item));
//    }
//}
