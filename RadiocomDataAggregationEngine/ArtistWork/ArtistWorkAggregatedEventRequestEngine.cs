using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Microsoft.Extensions.Logging;

namespace RadiocomDataAggregationEngine.ArtistWork
{
    public class ArtistWorkAggregatedEventRequestEngine
    {
        private readonly ILogger<ArtistWorkAggregatedEventRequestEngine> _logger;
        private readonly IArtistWorkTimeSeriesCache _artistWorkTimeSeriesCache;

        public ArtistWorkAggregatedEventRequestEngine(ILogger<ArtistWorkAggregatedEventRequestEngine> logger, 
            IArtistWorkTimeSeriesCache artistWorkTimeSeriesCache)
        {
            _logger = logger;
            _artistWorkTimeSeriesCache = artistWorkTimeSeriesCache;
        }

        public async Task<IEnumerable<IAggregatedEvent>> GetArtistWorkAggregatedEventsAsync(IArtistWorkAggregatedEventsRequest request)
        {
            IEnumerable<IAggregatedEvent> aggregatedEvents =
                await _artistWorkTimeSeriesCache.FetchTimeSeriesAggregatedEventsAsync(request.ArtistWorkIds, request.TimeSeries);
            return aggregatedEvents;
        }

    }
}
