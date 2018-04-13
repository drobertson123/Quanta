using System;

namespace Quanta.Tests.Example.Model
{
    [Serializable]
    public struct OptionBookMeta
    {
        public string UnderlyingSymbol;
        public SecurityTypes SecurityType;              // (int)SecurityTypes
        public QuoteInterval Interval;                  // (int)QuoteInterval
        public SampleTimes SampleTime;                // (int)SampleTimes
        public QuoteLag Delay;                     // (int)QuoteLag
        public Int64 AsOf;
    }
}
