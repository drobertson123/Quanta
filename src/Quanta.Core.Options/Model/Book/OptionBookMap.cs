using System;
using System.Runtime.InteropServices;
using System.Text;
using NodaTime;
using Quanta.Core.Data;

namespace Quanta.Core.Options.Model.Book
{
    public class OptionBookMap : DataMap<OptionQuote, OptionBookStruct>
    {
        public unsafe OptionBookMap()
        {
            SetCreateDataFunc((data, meta) =>
                {
                    OptionBookMeta bookMeta = (OptionBookMeta)meta;
                    OptionQuote quote = new OptionQuote
                    {
                        UnderlyingSymbol = bookMeta.UnderlyingSymbol,
                        UnderlyingPrice = bookMeta.UnderlyingPrice,
                        Right = data.Right,
                        Expiration = Instant.FromUnixTimeSeconds(data.Expiration),
                        Multiplier = data.Multiplier,
                        Strike = (Decimal)data.Strike,
                        Style = data.Style,
                        Bid = (Decimal)data.Bid,
                        Ask = (Decimal)data.Ask,
                        OpenInterest = (int)data.OpenInterest,
                        Volume = (int)data.Volume,
                        SecurityType = bookMeta.SecurityType,
                        Interval = bookMeta.Interval,
                        SampleTime = bookMeta.SampleTime,
                        Delay = bookMeta.Delay,
                        AsOf = bookMeta.AsOf
                    };

                    quote.Symbol = Marshal.PtrToStringAnsi((IntPtr)data.Symbol);

                    return quote;

                });

            SetCreateStructFunc((data) =>
                {

                    OptionBookStruct book = new OptionBookStruct
                    {
                        Right = data.Right,
                        Expiration = (UInt32)data.Expiration.ToUnixTimeSeconds(),
                        Multiplier = data.Multiplier,
                        Strike = (float)data.Strike,
                        Style = data.Style,
                        Bid = (float)data.Bid,
                        Ask = (float)data.Ask,
                        OpenInterest = (uint)data.OpenInterest,
                        Volume = (uint)data.Volume
                    };

                    int len = data.Symbol.Length;
                    Marshal.Copy(Encoding.ASCII.GetBytes(data.Symbol), 0, (IntPtr)book.Symbol, len);

                    return book;
                }
                );
        }
    }
}