using System;
using System.Collections.Generic;
using Quanta.Data;

namespace Quanta.Tests.Example.Model.Sequence
{
    public class OptionSequenceComparer : Comparer<OptionQuote>
    {
        public override int Compare(OptionQuote x, OptionQuote y) => x.AsOf.CompareTo(y.AsOf);
    }

    /// <summary>
    /// An Option Series is the end of day quote for the entire life of a single option symbol.
    /// </summary>
    public class OptionSequence : Sequence<OptionQuote, OptionSequenceMeta>
    {
        public OptionSequence()
            : base(new OptionSequenceComparer()) { }

        public OptionSequence(IEnumerable<OptionQuote> collection)
            : base(collection, new OptionSequenceComparer()) { }
    }
}
