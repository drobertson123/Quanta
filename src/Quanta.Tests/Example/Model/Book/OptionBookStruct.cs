using System;
using System.Runtime.InteropServices;

namespace Quanta.Tests.Example.Model.Book
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct OptionBookStruct
    {
        public fixed char Symbol[15];
        public float UnderlyingPrice;
        public OptionRight Right;               // (int)OptionRight
        public long Expiration;                 // NodaTime: instant.Ticks
        public int Multiplier;
        public float Strike;
        public OptionStyle Style;                     // (int)OptionStyle 
        public float Bid;
        public float Ask;
        public Int64 OpenInterest;
        public Int64 Volume;
    }
}
