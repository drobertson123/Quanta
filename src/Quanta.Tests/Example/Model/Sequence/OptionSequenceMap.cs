using System;
using System.Linq;
using NodaTime;
using Quanta.Data;

namespace Quanta.Tests.Example.Model.Sequence
{
    public class OptionSequenceMap : DataMap<OptionQuote, OptionSequenceStruct, OptionSequenceMeta>
    {
        public OptionSequenceMap()
        {
            SetCreateDataFunc((data, meta) =>
                 new OptionQuote()
                 {
                     Symbol = meta.Symbol,
                     UnderlyingSymbol = meta.UnderlyingSymbol,
                     UnderlyingPrice = data.UnderlyingPrice,
                     Right = meta.Right,
                     Expiration = Instant.FromUnixTimeTicks(meta.Expiration),
                     Multiplier = meta.Multiplier,
                     Strike = meta.Strike,
                     Style = meta.Style,
                     Bid = data.Bid,
                     Ask = data.Ask,
                     OpenInterest = data.OpenInterest,
                     Volume = data.Volume,
                     SecurityType = meta.SecurityType,
                     Interval = meta.Interval,
                     SampleTime = meta.SampleTime,
                     Delay = meta.Delay,
                     AsOf = Instant.FromUnixTimeTicks(data.AsOf)
                 });

            SetCreateStructFunc((data) =>
                new OptionSequenceStruct
                {
                    UnderlyingPrice = data.UnderlyingPrice,
                    Bid = data.Bid,
                    Ask = data.Ask,
                    OpenInterest = data.OpenInterest,
                    Volume = data.Volume,
                    AsOf = data.AsOf.ToUnixTimeTicks()
                });
        }
    }
}
