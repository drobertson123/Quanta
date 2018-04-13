using System;
using System.Runtime.InteropServices;
using NodaTime;

namespace Quanta.Core.Options.Model.Book
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct OptionBookStruct
    {
        public fixed byte Symbol[21];
        public OptionRight Right;               // (int)OptionRight
        public UInt32 Expiration;                 // NodaTime: instant.Ticks
        public Int16 Multiplier;
        public float Strike;
        public OptionStyle Style;                     // (int)OptionStyle 
        public float Bid;
        public float Ask;
        public UInt32 OpenInterest;
        public UInt32 Volume;
    }
}
