using System;
using System.Collections.Generic;
using System.Text;
using Czf.Radiocom.Aggregation.Contracts.Values;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IAggregatedEvent
    {
        public int Id { get; set; }
        public long AggregatedEventSum { get; set; }
        public TimeSeries AggregationTimeSeries { get; set; }
        public IEnumerable<ITimeSeriesValue> AggregatedEventSumSource { get; set; }
    }
}
