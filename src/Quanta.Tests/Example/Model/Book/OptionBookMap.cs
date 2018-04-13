using System;
using NodaTime;
using Quanta.Data;
using System.Runtime.CompilerServices;

namespace Quanta.Tests.Example.Model.Book
{
    public class OptionBookMap : DataMap<OptionQuote, OptionBookStruct, OptionBookMeta>
    {
        public unsafe OptionBookMap()
        {
            SetCreateDataFunc((data, meta) =>
                 new OptionQuote()
                 {
                     Symbol = Unsafe.Read<String>(data.Symbol),
                     UnderlyingSymbol = meta.UnderlyingSymbol,
                     UnderlyingPrice = (float)data.UnderlyingPrice,
                     Right = data.Right,
                     Expiration = Instant.FromUnixTimeTicks(data.Expiration),
                     Multiplier = data.Multiplier,
                     Strike = data.Strike,
                     Style = (OptionStyle)data.Style,
                     Bid = (float)data.Bid,
                     Ask = (float)data.Ask,
                     OpenInterest = data.OpenInterest,
                     Volume = data.Volume,
                     SecurityType = (SecurityTypes)meta.SecurityType,
                     Interval = (QuoteInterval)meta.Interval,
                     SampleTime = (SampleTimes)meta.SampleTime,
                     Delay = (QuoteLag)meta.Delay,
                     AsOf = Instant.FromUnixTimeTicks(meta.AsOf)
                 });

            SetCreateStructFunc((data) =>
                {  
                    OptionBookStruct book = new OptionBookStruct
                    {
                        UnderlyingPrice = (float)data.UnderlyingPrice,
                        Right = (OptionRight)data.Right,
                        Expiration = (long)data.Expiration.ToUnixTimeTicks(),
                        Multiplier = (int)data.Multiplier,
                        Strike = (float)data.Strike,
                        Style = (OptionStyle)data.Style,
                        Bid = (float)data.Bid,
                        Ask = (float)data.Ask,
                        OpenInterest = data.OpenInterest,
                        Volume = data.Volume
                    };

                    // Fixed Size Buffers can't be assigned during construction
                    // Use Unsafe.Write to assign them after creating the struct
                    Unsafe.Write<string>(book.Symbol, data.Symbol);

                    return book; }                
                );
        }
    }
}