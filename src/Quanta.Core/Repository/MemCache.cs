using System;
using Microsoft.Extensions.Caching.Memory;

namespace Quanta.Core.Repository
{
    public class MemCache : MemoryCache
    {
        public MemCache(CacheOptions options) : base(options) { }
        public MemCache() : base(new CacheOptions()) { }
    }
}
