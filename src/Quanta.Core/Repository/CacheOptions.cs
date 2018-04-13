using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

namespace Quanta.Core.Repository
{
    public class CacheOptions : MemoryCacheOptions
    {
        public new ISystemClock Clock { get; set; }
        public new double CompactionPercentage { get; set; } = .30d;
        public new TimeSpan ExpirationScanFrequency { get; set; } = new TimeSpan(0, 1, 0); // Scans once a minute
        public new long? SizeLimit { get; set; }
    }
}
