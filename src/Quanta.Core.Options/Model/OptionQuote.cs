using System;
using NodaTime;

namespace Quanta.Core.Options.Model
{
    public class OptionQuote : QuoteBase
    {
        public OptionQuote() { }
        public string UnderlyingSymbol { get; set; }
        public Decimal UnderlyingPrice { get; set; }
        public OptionRight Right { get; set; }
        public Instant Expiration { get; set; }
        public Int16 Multiplier { get; set; }
        public Decimal Strike { get; set; }
        public OptionStyle Style { get; set; }
        public Decimal Bid { get; set; }
        public Decimal Ask { get; set; }
        public Int32 OpenInterest { get; set; }
        public Int32 Volume { get; set; }
    }
}
