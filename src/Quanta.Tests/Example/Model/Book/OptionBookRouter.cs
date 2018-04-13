using System;
using System.Globalization;
using System.IO;
using NodaTime;
using Quanta.Repository;

namespace Quanta.Tests.Example.Model.Book
{
    public class OptionBookRouter : Router<OptionBookMeta>
    {
        private const string DATA_ZONE = @"OptionsData\DailyBooks";
        private const string DATA_EXTENSION = ".optbook";
        public OptionBookRouter() : this("") { }
        public OptionBookRouter(string root)
        {
            Root = root;
            SetRouteFunc(meta =>
                                  {
                                      string symbol = meta.UnderlyingSymbol.Trim().ToUpper();
                                      string dateof = Instant.FromUnixTimeTicks(meta.AsOf).ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                                      string subSection = $"_{symbol}";
                                      string fileName = $"{dateof}_{symbol}";

                                      return Path.Combine(Root, DATA_ZONE, subSection, fileName, DATA_EXTENSION);
                                  });
        }
        public string Root { get; set; }

    }
}
