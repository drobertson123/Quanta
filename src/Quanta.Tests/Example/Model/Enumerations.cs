using System;
using System.Linq;

namespace Quanta.Tests.Example.Model
{
    public enum SecurityTypes
    {
        Base = 0,
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
        Other = 1000
    }

    public enum MarketType
    {
        Equity,
        Option,
        Future,
        Currency,
        Bond,
        Commodity
    }

    [Flags]
    public enum MarketStates
    {
        Closed,
        PreMarket,
        Regular,
        PostMarket,
        Extended
    }
    public enum OptionRight
    {
        Unknown = 0,
        Put = 1,
        Call = 2
    }

    public enum OptionStyle
    {
        American = 1,
        European = 2,
        Binary = 3,
        Other = 1000
    }

    public enum OptionSpecialCategory
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
        Other = 1000
    }
    public enum SampleTimes
    {
        Unknown = 0,
        IntraDay = 1,
        EndOfDay = 2,
        PreMarket = 3,
        PostMarket = 4,
        Expiration = 5,
        Other = 1000
    }

    public enum QuoteLag
    {
        Unknown = 0,
        RealTime = 1,
        RealTimeDelayed = 2,
        Historical = 3,
        Other = 1000
    }

    public enum QuoteInterval
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
        Custom = 1000
    }

    public enum DataRequestRange
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
        Custom = 1000
    }
}
