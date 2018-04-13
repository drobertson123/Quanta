using System;
using NodaTime;
using Quanta.Core.Data;

namespace Quanta.Core.Options.Model.Book
{
    [Serializable]
    public struct OptionBookMeta : IMetaData
    {
        public string UnderlyingSymbol;
        public Decimal UnderlyingPrice;
        public SecurityTypes SecurityType;              // (int)SecurityTypes
        public QuoteInterval Interval;                  // (int)QuoteInterval
        public SampleTimes SampleTime;                // (int)SampleTimes
        public QuoteLag Delay;                     // (int)QuoteLag
        public Instant AsOf;
    }
}
