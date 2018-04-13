using System;
using System.Linq;
using NodaTime;
using Quanta.Core.Data;

namespace Quanta.Core.Options.Model.Sequence
{
    // Note: 

    [Serializable]
    public struct OptionSequenceMeta : IMetaData
    {
        public string Symbol;
        public string UnderlyingSymbol;
        public OptionRight Right;
        public Instant Expiration;                 // NodaTime: instant.Ticks
        public Int16 Multiplier;
        public Decimal Strike;
        public OptionStyle Style;
        public SecurityTypes SecurityType;
        public QuoteInterval Interval;
        public SampleTimes SampleTime;
        public QuoteLag Delay;
    }
}
