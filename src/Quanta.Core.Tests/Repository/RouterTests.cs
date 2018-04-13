using System;
using NodaTime;
using Quanta.Core.Options.Model;
using Quanta.Core.Options.Model.Book;
using Quanta.Core.Options.Model.Sequence;
using Xunit;
using Xunit.Abstractions;

namespace Quanta.Core.Tests.Repository
{
    public class RouterTests
    {
        private readonly ITestOutputHelper _output;

        public RouterTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void GetRoute_Book()
        {
            // Arrange
            OptionBookRouter router = new OptionBookRouter(@"C:\Data Test");

            OptionBookMeta meta = new OptionBookMeta
            {
                UnderlyingSymbol = "GOOGL",
                SecurityType = SecurityTypes.Option,
                Interval = QuoteInterval.OneDay,
                Delay = QuoteLag.Historical,
                AsOf = Instant.FromDateTimeUtc(new DateTime(2018, 3, 24, 11, 0, 0, DateTimeKind.Utc))
            };

            // Act
            string path = router.GetRoute(meta);

            // Assert
            Assert.True(path == @"C:\Data Test\OptionsData\DailyBooks\_GOOGL\2018-03-24_GOOGL.optbook", "Invalid Path");
        }

        [Fact]
        public void GetRoute_Sequence()
        {
            // Arrange
            OptionSequenceRouter router = new OptionSequenceRouter(@"C:\Data Test");

            OptionSequenceMeta meta = new OptionSequenceMeta
            {
                Symbol = "GOOGL180329C01025000",
                UnderlyingSymbol = "GOOGL",
                Right = OptionRight.Call,
                Expiration = Instant.FromDateTimeUtc(new DateTime(2018, 3, 29, 11, 0, 0, DateTimeKind.Utc)),               // NodaTime: instant.Ticks
                Multiplier = 100,
                Strike = 720.00m,
                SecurityType = SecurityTypes.Option,
                Interval = QuoteInterval.OneDay,
                Delay = QuoteLag.Historical
            };

            // Act
            string path = router.GetRoute(meta);

            // Assert
            Assert.True(path == @"C:\Data Test\OptionsData\TimeSeries\_GOOGL\2018-03-29\_GOOGL180329C01025000.optseq", "Invalid Path");

        }

        //[Fact]
        //public void IsValueType2()
        //{
        //    int size = Unsafe.SizeOf<Instant>();
        //    _output.WriteLine($"Size of Instant: {size}");
        //    Assert.True(false, $"Size of Instant: {size}");
        //    Assert.True(typeof(Instant).IsValueType, "Not Value Type");

        //}
    }
}
