using Quanta.Core.Options.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quanta.DataLoader.DeltaNeutral
{
    public class OptionsReader : CSVBridge<OptionQuote>
    {
        
        public async TIOrderedEnumerable<OptionQuote> info GetData(string file)
        {

            Func<CsvReader, bool> filter = reader => reader.GetField(0).AsSymbol() == filterSymbol.AsSymbol();

            (IEnumerable<OptionQuote> info, ExceptionList errors) = await GetData(filePath, filter);

            return (info.ToList().OrderBy(q => q.AsOf), errors);
        }

    }
}
