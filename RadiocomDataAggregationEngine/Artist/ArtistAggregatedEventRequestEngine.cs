using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Microsoft.Extensions.Logging;

namespace RadiocomDataAggregationEngine.Artist
{
    public class ArtistAggregatedEventRequestEngine
    {
        private readonly ILogger<ArtistAggregatedEventRequestEngine> _logger;
        private readonly IArtistTimeSeriesCache _artistTimeSeriesCache;

        public ArtistAggregatedEventRequestEngine(ILogger<ArtistAggregatedEventRequestEngine> logger, IArtistTimeSeriesCache artistTimeSeriesCache)
        {
            _logger = logger;
            _artistTimeSeriesCache = artistTimeSeriesCache;
        }

        public async Task<IAggregatedEvent> GetArtistAggregatedEventAsync(int id, TimeSeries timeSeries)
        {
            IAggregatedEvent aggregatedEvent = await _artistTimeSeriesCache.FetchTimeSeriesAggregatedEventAsync(id, timeSeries);
            return aggregatedEvent;
        }

        public async Task<IEnumerable<IAggregatedEvent>> GetArtistAggregatedEventsAsync(IArtistAggregatedEventsRequest request)
        {
            IEnumerable<IAggregatedEvent> aggregatedEvents =
                await _artistTimeSeriesCache.FetchTimeSeriesAggregatedEventsAsync(request.ArtistIds, request.TimeSeries);
            return aggregatedEvents;
        }

    }
}
