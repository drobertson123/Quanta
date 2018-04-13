using NodaTime;
using Quanta.Tests.Example.Model;
using Quanta.Tests.Example.Model.Book;
using System;
using Xunit;

namespace Quanta.Tests.Repository
{
    [Trait("Category", "Integration")]
    public class RouterTests 
    {

        [Fact]
        public void TestGetRoute_Book()
        {
            // Arrange
            OptionBookRouter router = new OptionBookRouter(@"C:\Data Test");

            OptionBookMeta meta = new OptionBookMeta
            {
                UnderlyingSymbol = "GOOGL",
                SecurityType = SecurityTypes.Option,
                Interval = QuoteInterval.OneDay,
                Delay = QuoteLag.Historical,
                AsOf = new DateTime(2018, 3, 30, 11, 0, 0, DateTimeKind.Utc).Ticks
            };

            // Act
            string path = router.GetRoute(meta);

            // Assert
            Assert.True(path == @"C:\Data Test\OptionsData\DailyBooks\_GOOGL\2018-03-30_GOOGL.optbook", "Invalid Path");
        }

        [Fact]
        public void TestGetRoute_Sequence()
        {
            // Arrange


            // Act
            //Router router = this.CreateRouter();


            // Assert

        }
    }
}
