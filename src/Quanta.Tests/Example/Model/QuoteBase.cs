using System;
using System.Linq;
using NodaTime;

namespace Quanta.Tests.Example.Model
{
    public interface IQuote
    {
        string Symbol { get; set; }
        Instant AsOf { get; set; }
    }
    public abstract class QuoteBase : IQuote
    {
        public string Symbol { get; set; }
        public virtual SecurityTypes SecurityType { get; set; }
        public virtual QuoteInterval Interval { get; set; }
        public virtual SampleTimes SampleTime { get; set; }
        public virtual QuoteLag Delay { get; set; }
        public Instant AsOf { get; set; }

    }
}
