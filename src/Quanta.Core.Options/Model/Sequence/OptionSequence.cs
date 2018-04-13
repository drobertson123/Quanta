using System;
using System.Collections.Generic;
using NodaTime;
using Quanta.Core.Data;

namespace Quanta.Core.Options.Model.Sequence
{
    public class OptionSequenceComparer : Comparer<Instant>
    {
        public override int Compare(Instant x, Instant y) => x.CompareTo(y);
    }

    ///// <summary>
    ///// An Option Series is the end of day quote for the entire life of a single option symbol.
    ///// </summary>
    //public class OptionSequence : Sequence<Instant, OptionQuote, OptionSequenceMeta>
    //{
    //    public OptionSequence()
    //        : base(new OptionSequenceComparer(), (q) => q.AsOf) { }

    //    //public OptionSequence(IEnumerable<OptionQuote> collection)
    //    //    : base(collection, new OptionSequenceComparer(), (q) => q.AsOf) { }
    //}
}
