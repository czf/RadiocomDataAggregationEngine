using System.Collections.Generic;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Czf.Radiocom.Aggregation.Entities;

namespace Czf.Radiocom.Event.Repository.Contracts
{
    public interface IArtistEventsRepository
    {

        /// <summary>
        /// Return events in ascending order by timestamp
        /// </summary>
        Task<IEnumerable<ArtistEvent>> GetEventsForTimeSeriesAsync(TimeSeries timeSeries, int artistId);
    }
}
