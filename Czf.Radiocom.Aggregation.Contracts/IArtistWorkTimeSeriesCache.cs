using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts.Values;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IArtistWorkTimeSeriesCache
    {
        Task StoreTimeSeriesValuesAsync(IEnumerable<ITimeSeriesValue> timeSeriesEvents, int ArtistWorkId, TimeSeries timeSeries, int artistId);
        Task<IEnumerable<ITimeSeriesValue>> FetchTimeSeriesValuesAsync(int ArtistWorkId, TimeSeries timeSeries);
        
        /// <summary>
        /// Returns ArtistWork AggregatedEvent entires for a specified artistId and timeseries
        /// </summary>
        IEnumerable<IAggregatedEvent> FetchArtistArtistWorkAggregatedEvents(int artistId, TimeSeries timeSeries);

        Task<IEnumerable<IAggregatedEvent>> FetchTimeSeriesAggregatedEventsAsync(IEnumerable<int> artistWorkIds, TimeSeries timeSeries);
    }
}
