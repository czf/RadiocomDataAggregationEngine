using System;
using System.Collections.Generic;
using System.Text;
using Czf.Radiocom.Aggregation.Contracts;

namespace Czf.Radiocom.Aggregation.Entities
{
    public class TimeSeriesValue : ITimeSeriesValue
    {
        public DateTimeOffset Timestamp { get; set; }
        public long Value { get; set; }
    }
}
