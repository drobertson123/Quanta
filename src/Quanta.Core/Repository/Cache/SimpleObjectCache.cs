using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Quanta.Core.Repository.Cache
{

    //NOTE: This is based on a ConcurrentDictionary in an effort to mostly be thread safe
    //      The cacheMonitor
    public class SimpleObjectCache<TKey, TValue>
    {
        protected ConcurrentDictionary<TKey, TValue> _cache;
        protected Dictionary<TKey, DateTime> _cacheMonitor;
        protected int _cacheSize;
        protected int _hitCount;
        protected int _missCount;
        public SimpleObjectCache(int cacheSize)
        {
            _cacheSize = cacheSize;

            _cache = new ConcurrentDictionary<TKey, TValue>(10, cacheSize + 1);
            _cacheMonitor = new Dictionary<TKey, DateTime>(_cacheSize + 1);
        }
        public void Bump(TKey key)
        {
            if (_cacheMonitor.ContainsKey(key)) _cacheMonitor[key] = DateTime.Now;
        }

        protected void Trim()
        {
            //if (_cacheMonitor.Count > _cacheSize)
            //{
            //    TKey key = _cacheMonitor.Min(s => s.Value).Key;
            //    if (_cache.TryRemove(key, out TValue obj)) _cacheMonitor.Remove(key);
            //}
        }

        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_cache).Keys;

        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_cache).Values;

        #region CacheStats
        public int CacheSize => _cacheSize;
        public int Count => ((IDictionary<TKey, TValue>)_cache).Count;
        public int HitCount => _hitCount;
        public int MissCount => _missCount;
        public void ResetStats()
        {
            _hitCount = 0;
            _missCount = 0;
        }
        #endregion
        public bool IsReadOnly => ((IDictionary<TKey, TValue>)_cache).IsReadOnly;

        public TValue this[TKey key]
        {
            get
            {
                TValue item;
                TryGetValue(key, out item);
                return item;
            }

            set
            {
                _cacheMonitor[key] = DateTime.Now;
                ((IDictionary<TKey, TValue>)_cache)[key] = value;
                Trim();
            }
        }
        public bool ContainsKey(TKey key) => ((IDictionary<TKey, TValue>)_cache).ContainsKey(key);

        public void Add(TKey key, TValue value)
        {
            _cacheMonitor[key] = DateTime.Now;
            ((IDictionary<TKey, TValue>)_cache).Add(key, value);
            Trim();
        }

        //bool IDictionary<TKey, TValue>.Remove(TKey key) => ((IDictionary<TKey, TValue>)_cache).Remove(key) && ((IDictionary<TKey, DateTime>)_cacheMonitor).Remove(key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool success = ((IDictionary<TKey, TValue>)_cache).TryGetValue(key, out value);
            if (success)
            {
                Bump(key);
                _hitCount++;
            }
            else _missCount++;

            return success;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>)_cache).Add(item);
            ((IDictionary<TKey, DateTime>)_cacheMonitor)[item.Key] = DateTime.Now;
            Trim();
        }

        public void Clear()
        {
            ((IDictionary<TKey, TValue>)_cache).Clear();
            ((IDictionary<TKey, DateTime>)_cacheMonitor).Clear();
            ResetStats();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)_cache).Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)_cache).CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)_cache).Remove(item) && ((IDictionary<TKey, DateTime>)_cacheMonitor).Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IDictionary<TKey, TValue>)_cache).GetEnumerator();

        //IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<TKey, TValue>)_cache).GetEnumerator();
    }
}
