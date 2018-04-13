using Quanta.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quanta.Tests.Example.Model
{
    public class OptionBook: Set<OptionQuote, OptionSequenceMeta>
    {
        /// <summary>
        /// The Option Book is data on all the options available on a single day for a specific underlying symbol. 
        /// </summary>
        public OptionBook()
        {

        }
    }
}
