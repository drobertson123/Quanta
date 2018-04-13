using System;
using System.Globalization;
using System.IO;
using NodaTime;
using Quanta.Core.Repository;

namespace Quanta.Core.Options.Model.Sequence
{
    public class OptionSequenceRouter : Router<OptionSequenceMeta>
    {
        private const string DATA_ZONE = @"OptionsData\TimeSeries";
        private const string DATA_EXTENSION = "optseq";

        public OptionSequenceRouter() : this(string.Empty) { }
        public OptionSequenceRouter(string root)
        {
            Root = root;

            SetRouteFunc(meta =>
          {
              string symbol = meta.Symbol.Trim().ToUpper();
              string underlying = meta.UnderlyingSymbol.Trim().ToUpper();
              string subSection = $"_{underlying}";

              Instant exp = meta.Expiration;
              string expFolder =
                  exp.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);

              string fileName = $"_{symbol}.{DATA_EXTENSION}";

              string path =
                  Path.Combine(
                      Root,
                      DATA_ZONE,
                      subSection,
                      expFolder,
                      fileName);

              return path;
          });
        }
        public string Root { get; set; }
    }
}