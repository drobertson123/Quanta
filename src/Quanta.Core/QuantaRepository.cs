using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Quanta.Core.Data;
using Quanta.Core.Repository;

namespace Quanta.Core
{
    public class QuantaRepository<TSession,TSet, TData, TStruct, TMap, TMeta>
        where TSession : Session<TSession, TSet, TData, TStruct, TMap, TMeta>, new()
        where TSet : Set<TData, TMeta>, new()       
        where TStruct : struct
        where TMap : DataMap<TData, TStruct>, new()
        where TMeta : IMetaData
    {
        private readonly Router<TMeta> _router;
        private readonly IMemoryCache _cache;
        private readonly ICachePolicy _policy;

        public QuantaRepository(Router<TMeta> router):this(router, new MemCache(), new CachePolicy()) { }
        public QuantaRepository(Router<TMeta> router, ICachePolicy policy) : this(router, new MemCache(), policy) { }
        public QuantaRepository(Router<TMeta> router, IMemoryCache cache) : this(router, cache, new CachePolicy()) { }
        public QuantaRepository(Router<TMeta> router, CacheOptions options, ICachePolicy policy) : this(router, new MemCache(options), policy) { }
        public QuantaRepository(Router<TMeta> router, IMemoryCache cache, ICachePolicy policy)
        {
            _router = router;
            _cache = cache;
            _policy = policy;
        }

        public virtual TSet Read(TMeta key)
        {
            TSession session = GetOrCreate(key);
            return session.Read();
        }

        public void Write(TMeta key, TSet items, WriteType writeType = WriteType.Update)
        {
            TSession session = GetOrCreate(key);
            session.Write(items, writeType);
        }

        public void Write(IDictionary<TMeta, TSet> items, WriteType writeType = WriteType.Update)
        {
            foreach(KeyValuePair<TMeta, TSet> item in items)
            {
                TSession session = GetOrCreate(item.Key);
                session.Write(item.Value, writeType);
            }
        }

        internal virtual TSession GetOrCreate(TMeta key)
        {
            string mapKey = _router.GetKey(key);

            return _cache.GetOrCreate<TSession>(mapKey, entry =>
               {
                   TSession session = new TSession();
                   session.Initialize(_router.GetRoute(key), mapKey);
                   return session;
               });
        }
    }
}
