using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NodaTime;
using Quanta.Core.Options.Model;
using Quanta.Core.Options.Model.Book;
using Quanta.Core.Options.Model.Sequence;
using Xunit;
using Xunit.Abstractions;

namespace Quanta.Core.Tests.Repository
{
    public class SessionTests
    {
        private readonly ITestOutputHelper _output;
        public SessionTests(ITestOutputHelper output) => _output = output;

        //public static List<OptionQuote> GetOptions(OptionSpec _spec, int count)
        //{
        //    List<OptionQuote> options = new List<OptionQuote>(count);
        //    Instant SampleDate = Instant.FromDateTimeUtc(new DateTime(2000, 1, 1, 20, 0, 0, DateTimeKind.Utc));

        //    for (int i = 0; i < count; i++)
        //    {
        //        OptionQuote quote = new OptionQuote
        //        {
        //            AsOf = SampleDate,
        //            Symbol = _spec.Symbol,
        //            UnderlyingPrice = 55.75f,
        //            Bid = 0.35f,
        //            Ask = 0.45f,
        //            OpenInterest = 5000,
        //            Volume = 1000
        //        };

        //        options.Add(quote);

        //        SampleDate = SampleDate.Plus(Duration.FromDays(1));
        //    }

        //    return options;
        //}
        public static bool IsEquivalent(OptionQuote quote1, OptionQuote quote2) => quote1.Symbol == quote2.Symbol
            && quote1.SecurityType == quote2.SecurityType
            && quote1.Interval == quote2.Interval
            && quote1.SampleTime == quote2.SampleTime
            && quote1.Delay == quote2.Delay
            && quote1.AsOf == quote2.AsOf
            && quote1.UnderlyingSymbol == quote2.UnderlyingSymbol
            && quote1.UnderlyingPrice == quote2.UnderlyingPrice
            && quote1.Right == quote2.Right
            && quote1.Expiration == quote2.Expiration
            && quote1.Multiplier == quote2.Multiplier
            && quote1.Strike == quote2.Strike
            && quote1.Style == quote2.Style
            && quote1.Bid == quote2.Bid
            && quote1.Ask == quote2.Ask
            && quote1.OpenInterest == quote2.OpenInterest
            && quote1.Volume == quote2.Volume;

        public static unsafe OptionBook AddQuotes(OptionBook book, int expCount, int strikeCount)
        {
            OptionBookMap map = new OptionBookMap();
            Instant expInst = Instant.FromDateTimeUtc(new DateTime(2018, 3, 29, 20, 0, 0, DateTimeKind.Utc));

            //GOOGL180329C01052500	
            for (int i = 0; i < expCount; i++)
            {
                for (Decimal strike = 800; strike < (800 + (strikeCount * 5)); strike += 5)
                {
                    OptionBookStruct opt = new OptionBookStruct()
                    {
                        //UnderlyingPrice = 100,
                        Right = OptionRight.Put,
                        Expiration = (UInt32)Instant.FromDateTimeUtc(new DateTime(2018, 3, 29, 20, 0, 0, DateTimeKind.Utc)).ToUnixTimeSeconds(),
                        Multiplier = 100,
                        Strike = (float)((1000000 + i) / 1000),
                        Style = OptionStyle.American,
                        Bid = (float)3m,
                        Ask = (float)3.55m,
                        OpenInterest = 5000,
                        Volume = 1000
                    };

                    string putSymbol = GetOptionName("GOOGL", expInst.ToDateTimeUtc(), (decimal)opt.Strike, opt.Right);
                    int len = putSymbol.Length;
                    Marshal.Copy(Encoding.ASCII.GetBytes(putSymbol), 0, (IntPtr)opt.Symbol, len);

                    OptionQuote putQuote = map.ToData(opt, book.MetaData);
                    book.Add(putQuote);

                    opt.Right = OptionRight.Call;
                    string callSymbol = GetOptionName("GOOGL", expInst.ToDateTimeUtc(), (decimal)opt.Strike, opt.Right);
                    len = callSymbol.Length;
                    Marshal.Copy(Encoding.ASCII.GetBytes(callSymbol), 0, (IntPtr)opt.Symbol, len);

                    OptionQuote callQuote = map.ToData(opt, book.MetaData);
                    book.Add(callQuote);
                }

                expInst = expInst.Plus(Duration.FromDays(7));
            }

            return book;
        }

        public static string GetOptionName(string symbol, DateTime expiration, decimal strike, OptionRight right)
        {
            //if (optionType == ContractType.Unknown) throw new ArgumentException("invalid option class: unknown", nameof(optionType));

            string Sym = symbol.ToUpper();
            string Expiration = expiration.ToString("yyMMdd");
            string Type = right.ToString().Substring(0, 1).ToUpper();
            string Strike = Convert.ToInt32(strike * 1000).ToString("00000000");

            return $"{Sym}{Expiration}{Type}{Strike}";
        }
        public OptionBookMeta GetOptionBookMeta() =>
            new OptionBookMeta
            {
                UnderlyingSymbol = "GOOGL",
                SampleTime = SampleTimes.EndOfDay,
                SecurityType = SecurityTypes.Option,
                Interval = QuoteInterval.OneDay,
                Delay = QuoteLag.Historical,
                AsOf = Instant.FromDateTimeUtc(new DateTime(2018, 3, 24, 11, 0, 0, DateTimeKind.Utc))
            };

        public OptionSequenceMeta GetOptionSequenceMeta() =>
             new OptionSequenceMeta
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

        [Fact]
        public void WriteBookSession()
        {
            // Arrange
            string fileName = @"C:\DataTest2\TestFile.optbook";
            string mapKey = "MAP:DataTest2_TestFile.optbooktesting";

            if (File.Exists(fileName)) File.Delete(fileName);

            OptionBookSession session = new OptionBookSession(fileName, mapKey);

            OptionBookMeta meta = GetOptionBookMeta();

            OptionBook book = new OptionBook
            {
                MetaData = meta
            };

            //int count = 50000;
            book = AddQuotes(book, 10, 100);

            // Act
            Stopwatch watch = new Stopwatch();
            watch.Start();
            session.Write(book);
            watch.Stop();

            // Assert
            _output.WriteLine($"{book.Count} quotes in {watch.Elapsed.TotalSeconds} seconds. That is {(book.Count / watch.Elapsed.TotalSeconds):N} quotes per second.");
            Assert.True(File.Exists(fileName), "File doesn't exist");

        }
        [Fact]
        public void ReadBookSession()
        {
            // Arrange
            int count = 10000;

            string fileName = @"C:\DataTest2\TestFile.optbook";
            string mapKey = "MAP:DataTest2_TestFile.optbooktesting";
            OptionBookSession session = new OptionBookSession(fileName, mapKey);

            // Act
            Stopwatch watch = new Stopwatch();
            watch.Start();
            OptionBook book = session.Read();
            watch.Stop();

            _output.WriteLine($"#1 {book.Count} quotes in {watch.Elapsed.TotalSeconds} seconds. That is {(book.Count / watch.Elapsed.TotalSeconds):N} quotes per second.");

            watch.Restart();
            book = session.Read();
            watch.Stop();

            _output.WriteLine($"#2 {book.Count} quotes in {watch.Elapsed.TotalSeconds} seconds. That is {(book.Count / watch.Elapsed.TotalSeconds):N} quotes per second.");

            watch.Restart();
            book = session.Read();
            watch.Stop();

            _output.WriteLine($"#3 {book.Count} quotes in {watch.Elapsed.TotalSeconds} seconds. That is {(book.Count / watch.Elapsed.TotalSeconds):N} quotes per second.");

            // Assert
            //_output.WriteLine($"{book.Count} quotes in {watch.Elapsed.TotalSeconds} seconds. That is {(book.Count / watch.Elapsed.TotalSeconds):N} quotes per second.");
            Assert.True(book.Count == count, "Wrong number of quotes");

        }

        [Fact]
        public void WriteReadValidateSession()
        {
            // Arrange
            string fileName = @"C:\DataTest2\TestFile.optbook";
            string mapKey = "MAP:DataTest2_TestFile.optbooktesting";

            if (File.Exists(fileName)) File.Delete(fileName);

            OptionBookSession session = new OptionBookSession(fileName, mapKey);
            OptionBookMeta meta = GetOptionBookMeta();
            OptionBook book = new OptionBook
            {
                MetaData = meta
            };

            //int count = 50000;
            book = AddQuotes(book, 10, 100);

            // Act
            Stopwatch watch = new Stopwatch();
            watch.Start();
            session.Write(book);
            watch.Stop();
            _output.WriteLine($"Wrote {book.Count} quotes in {watch.Elapsed.TotalSeconds} seconds. That is {(book.Count / watch.Elapsed.TotalSeconds):N} quotes per second.");

            // New Session
            OptionBookSession session2 = new OptionBookSession(fileName, mapKey);

            watch.Restart();
            OptionBook bookOut = session2.Read();
            watch.Stop();

            _output.WriteLine($"Read {book.Count} quotes in {watch.Elapsed.TotalSeconds} seconds. That is {(book.Count / watch.Elapsed.TotalSeconds):N} quotes per second.");

            OptionQuote quoteIn1 = book.First();
            OptionQuote quoteOut1 = bookOut.First();

            // Assert
            Assert.True(book.MetaData.Equals(bookOut.MetaData), "MetaData does not match");
            Assert.True(book.Count == bookOut.Count, "The item count does not match");
            Assert.True(IsEquivalent(quoteIn1, quoteIn1), "The data sets do not match");
        }
    }
}