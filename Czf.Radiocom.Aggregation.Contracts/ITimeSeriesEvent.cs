using System;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface ITimeSeriesEvent
    {
        DateTimeOffset TimeStamp { get; set; }
    }
}
