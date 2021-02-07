using System.Collections.Generic;
using Czf.Radiocom.Aggregation.Contracts.Values;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface ITimeSeriesEngine
    {
        IEnumerable<ITimeSeriesValue> ProcessTimeSeries(IEnumerable<ITimeSeriesEvent> events, TimeSeries timeSeries);
    }
}
