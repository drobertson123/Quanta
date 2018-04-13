using System;
using System.Linq;

namespace Quanta.Tests.Example.Model
{
    // Note: 

    [Serializable]
    public struct OptionSequenceMeta
    {
        public string Symbol;
        public string UnderlyingSymbol;
        public OptionRight Right;
        public long Expiration;                 // NodaTime: instant.Ticks
        public int Multiplier;
        public float Strike;
        public OptionStyle Style;
        public SecurityTypes SecurityType;
        public QuoteInterval Interval;
        public SampleTimes SampleTime;
        public QuoteLag Delay;
    }
}
