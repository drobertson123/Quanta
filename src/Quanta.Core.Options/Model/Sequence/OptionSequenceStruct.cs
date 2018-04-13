using NodaTime;
using System;
using System.Linq;

namespace Quanta.Core.Options.Model.Sequence
{
    public struct OptionSequenceStruct
    {
        public Decimal UnderlyingPrice;
        public Decimal Bid;
        public Decimal Ask;
        public Int32 OpenInterest;
        public Int32 Volume;
        public Instant AsOf;
    }
}
