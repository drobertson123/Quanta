using System;
using Quanta.Core.Data;

namespace Quanta.Core.Options.Model.Book
{
    public class OptionBook : Set<OptionQuote, OptionBookMeta>
    {
        /// <summary>
        /// The Option Book is data on all the options available on a single day for a specific underlying symbol. 
        /// </summary>
        public OptionBook()
        {

        }
    }
}
