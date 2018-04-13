using System;
using NodaTime;

namespace Quanta.Tests.Example.Model
{
    public class OptionQuote : QuoteBase
    {
        public OptionQuote() { }
        public string UnderlyingSymbol { get; set; }
        public float UnderlyingPrice { get; set; }
        public OptionRight Right { get; set; }
        public Instant Expiration { get; set; }
        public int Multiplier { get; set; }
        public float Strike { get; set; }
        public OptionStyle Style { get; set; }
        public float Bid { get; set; }
        public float Ask { get; set; }
        public Int64 OpenInterest { get; set; }
        public Int64 Volume { get; set; }
    }
}
