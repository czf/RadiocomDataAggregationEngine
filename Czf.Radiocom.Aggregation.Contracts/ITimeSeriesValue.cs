using System;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface ITimeSeriesValue
    {
        public DateTimeOffset Timestamp { get; set; }
        public long Value { get; set; }
    }
}
