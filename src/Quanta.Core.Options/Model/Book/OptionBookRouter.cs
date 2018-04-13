using System;
using System.Globalization;
using System.IO;
using Quanta.Core.Repository;

namespace Quanta.Core.Options.Model.Book
{
    public class OptionBookRouter : Router<OptionBookMeta>
    {
        private const string DATA_ZONE = @"OptionsData\DailyBooks";
        private const string DATA_EXTENSION = "optbook";
        public OptionBookRouter() : this(string.Empty) { }
        public OptionBookRouter(string root)
        {
            Root = root;
            SetRouteFunc(meta =>
                                  {
                                      string symbol = meta.UnderlyingSymbol.Trim().ToUpper();
                                      string dateof = meta.AsOf.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                                      string subSection = $"_{symbol}";
                                      string fileName = $"{dateof}_{symbol}.{DATA_EXTENSION}";

                                      return Path.Combine(Root, DATA_ZONE, subSection, fileName);
                                  });
        }
        public string Root { get; set; }

    }
}
