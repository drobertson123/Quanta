using System;
using System.Linq;
using Quanta.Core.Data;

namespace Quanta.Core.Options.Model.Sequence
{
    public class OptionSequenceMap : DataMap<OptionQuote, OptionSequenceStruct>
    {
        public OptionSequenceMap()
        {
            SetCreateDataFunc((data, meta) =>
                {
                    OptionSequenceMeta sequenceMeta = (OptionSequenceMeta)meta;
                    OptionQuote quote = new OptionQuote
                    {
                        Symbol = sequenceMeta.Symbol,
                        UnderlyingSymbol = sequenceMeta.UnderlyingSymbol,
                        UnderlyingPrice = data.UnderlyingPrice,
                        Right = sequenceMeta.Right,
                        Expiration = sequenceMeta.Expiration,
                        Multiplier = sequenceMeta.Multiplier,
                        Strike = sequenceMeta.Strike,
                        Style = sequenceMeta.Style,
                        Bid = data.Bid,
                        Ask = data.Ask,
                        OpenInterest = data.OpenInterest,
                        Volume = data.Volume,
                        SecurityType = sequenceMeta.SecurityType,
                        Interval = sequenceMeta.Interval,
                        SampleTime = sequenceMeta.SampleTime,
                        Delay = sequenceMeta.Delay,
                        AsOf = data.AsOf
                    };

                    return quote;
                });

            SetCreateStructFunc((data) =>
                new OptionSequenceStruct
                {
                    UnderlyingPrice = data.UnderlyingPrice,
                    Bid = data.Bid,
                    Ask = data.Ask,
                    OpenInterest = data.OpenInterest,
                    Volume = data.Volume,
                    AsOf = data.AsOf
                });
        }
    }
}
