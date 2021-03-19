using System;
using System.Collections.Generic;
using System.Text;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Aggregation.Contracts.Values;

namespace Czf.Radiocom.Aggregation.Entities
{
    public class AggregatedEvent : IAggregatedEvent
    {
        public int Id { get; set; }
        public long AggregatedEventSum { get; set; }
        public TimeSeries AggregationTimeSeries { get; set; }
        public IEnumerable<ITimeSeriesValue> AggregatedEventSumSource { get; set; }
    }

    public class AggregatedEventSource : ITimeSeriesValue
    {
        public DateTimeOffset Timestamp { get; set; }
        public long Value { get; set; }
    }
}
