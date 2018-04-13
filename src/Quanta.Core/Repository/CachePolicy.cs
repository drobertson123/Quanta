using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Quanta.Core.Repository
{
    public interface ICachePolicy
    {
        ICachePolicy CreatePolicy();

        DateTimeOffset? AbsoluteExpiration { get; set; }
        TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        IList<IChangeToken> ExpirationTokens { get; }
        IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; }
        CacheItemPriority Priority { get; set; }
        long? Size { get; set; }
        TimeSpan? SlidingExpiration { get; set; }
    }
    public class CachePolicy : MemoryCacheEntryOptions, ICachePolicy
    {
        // A copy of the standard MemoryCacheEntryOptions with properties made explicit and some defaults set
        
        public CachePolicy() { }

        // factory method CreatePolicy
        // Note: this can/should be overridden if this class is inherited
        public virtual ICachePolicy CreatePolicy() => (ICachePolicy)this.MemberwiseClone();
        public new DateTimeOffset? AbsoluteExpiration { get; set; } = null;
        public new TimeSpan? AbsoluteExpirationRelativeToNow { get; set; } = null;
        public new IList<IChangeToken> ExpirationTokens { get; }
        public new IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; }
        public new CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;
        public new long? Size { get; set; }
        public new TimeSpan? SlidingExpiration { get; set; } = new TimeSpan(1, 0, 0);  // 1 Hour

    }
}
