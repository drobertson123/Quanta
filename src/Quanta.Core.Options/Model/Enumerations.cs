using System;
using System.Linq;

namespace Quanta.Core.Options.Model
{
    public enum SecurityTypes : Byte
    {
        Unknown = 0,
        Stock = 1,
        Option = 2,
        Future = 3,
        FX = 4,
        Bond = 5,
        Index = 6,
        ETF = 7,
        ETN = 8,
        Commodity = 9,
        CryptoCurrency = 10,
        Other = 255
    }

    public enum MarketType : Byte
    {
        Unknown = 0,
        Equity = 1,
        Option = 2,
        Future = 3,
        Currency = 4,
        Bond = 5,
        Commodity = 6,
        Other = 255
    }

    public enum MarketStates : Byte
    {
        Unknown = 0,
        Closed = 1,
        PreMarket =2,
        Regular = 3,
        PostMarket = 4,
        Extended = 5
    }
    public enum OptionRight : Byte
    {
        Unknown = 0,
        Put = 1,
        Call = 2
    }

    public enum OptionStyle : Byte
    {
        Unknown = 0,
        American = 1,
        European = 2,
        Binary = 3,
        Other = 255
    }

    public enum OptionSpecialCategory : Byte
    {
        Unknown = 0,
        Standard = 1,
        NonStandard = 2,
        Binary = 3,
        Weekly = 4,
        Quarterly = 5,
        PMSettledSPX = 6,
        Mini = 7,
        Jumbo = 8,
        Other = 255
    }
    public enum SampleTimes : Byte
    {
        Unknown = 0,
        IntraDay = 1,
        EndOfDay = 2,
        PreMarket = 3,
        PostMarket = 4,
        Expiration = 5,
        Other = 255
    }

    public enum QuoteLag : Byte
    {
        Unknown = 0,
        RealTime = 1,
        RealTimeDelayed = 2,
        Historical = 3,
        Other = 255
    }

    public enum QuoteInterval : Byte
    {
        Unknown = 0,
        Tick = 1,
        OneMinute = 2,
        FiveMinutes = 3,
        TenMinutes = 4,
        FifteenMinutes = 5,
        ThirtyMinutes = 6,
        OneHour = 7,
        OneDay = 8,
        OneWeek = 9,
        TwoWeeks = 10,
        OneMonth = 11,
        ThreeMonths = 12,
        HalfYear = 13,
        OneYear = 14,
        MaxTime = 15,
        Custom = 255
    }

    public enum DataRequestRange : Byte
    {
        Unknown = 0,
        Day = 1,
        Week = 2,
        Month = 3,
        Quarter = 4,
        HalfYear = 5,
        YTD = 6,
        Year = 7,
        TwoYears = 8,
        FiveYears = 9,
        TenYears = 10,
        Max = 11,
        Custom = 255
    }
}
