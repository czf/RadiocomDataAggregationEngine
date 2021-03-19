using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts.Values;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IArtistTimeSeriesCache
    {
        Task StoreTimeSeriesValuesAsync(IEnumerable<ITimeSeriesValue> timeSeriesEvents, int artistId, TimeSeries timeSeries);
        Task<IEnumerable<ITimeSeriesValue>> FetchTimeSeriesValuesAsync(int artistId, TimeSeries timeSeries);
        Task<IAggregatedEvent> FetchTimeSeriesAggregatedEventAsync(int artistId, TimeSeries timeSeries);
        Task<IEnumerable<IAggregatedEvent>> FetchTimeSeriesAggregatedEventsAsync(IEnumerable<int> artistId, TimeSeries timeSeries);
    }
}
