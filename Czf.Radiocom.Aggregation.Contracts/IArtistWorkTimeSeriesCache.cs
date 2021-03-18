using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts.Values;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IArtistWorkTimeSeriesCache
    {
        Task StoreTimeSeriesValuesAsync(IEnumerable<ITimeSeriesValue> timeSeriesEvents, int ArtistWorkId, TimeSeries timeSeries);
        Task<IEnumerable<ITimeSeriesValue>> FetchTimeSeriesValuesAsync(int ArtistWorkId, TimeSeries timeSeries);
    }
}
