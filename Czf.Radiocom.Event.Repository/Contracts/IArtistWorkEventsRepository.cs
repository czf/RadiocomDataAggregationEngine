using System.Collections.Generic;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Czf.Radiocom.Aggregation.Entities;

namespace Czf.Radiocom.Event.Repository.Contracts
{
    public interface IArtistWorkEventsRepository
    {

        /// <summary>
        /// Return events in ascending order by timestamp
        /// </summary>
        Task<IEnumerable<ArtistWorkEvent>> GetEventsForTimeSeriesAsync(TimeSeries timeSeries, int artistWorkId);
    }
}
