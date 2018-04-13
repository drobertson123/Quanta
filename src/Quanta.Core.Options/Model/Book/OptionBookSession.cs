using System;
using Quanta.Core.Repository;

namespace Quanta.Core.Options.Model.Book
{
    public class OptionBookSession : Session<OptionBookSession, OptionBook, OptionQuote, OptionBookStruct, OptionBookMap, OptionBookMeta>
    {
        public OptionBookSession() { }
        public OptionBookSession(string fileName, string mapKey) : base(fileName, mapKey) { }
    }
}
