using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Microsoft.Extensions.Logging;

namespace RadiocomDataAggregationEngine.Artist
{
    /// <summary>
    /// Used to find the top ArtistWorkAggregatedEvents for a given artist
    /// </summary>
    public class Artist_TopArtistWorkAggregatedEventsRequestEngine
    {
        private readonly ILogger<Artist_TopArtistWorkAggregatedEventsRequestEngine> _logger;
        private readonly IArtistWorkTimeSeriesCache _artistWorkTimeSeriesCache;

        public Artist_TopArtistWorkAggregatedEventsRequestEngine(ILogger<Artist_TopArtistWorkAggregatedEventsRequestEngine> logger, 
            IArtistWorkTimeSeriesCache artistWorkTimeSeriesCache)
        {
            _logger = logger;
            _artistWorkTimeSeriesCache = artistWorkTimeSeriesCache;
        }

        public IEnumerable<IAggregatedEvent> GetTopNArtistWorkAggregatedEvents(IArtist_TopArtistWorkAggregatedEventsRequest request)
        {
            var events = _artistWorkTimeSeriesCache.FetchArtistArtistWorkAggregatedEvents(request.ArtistId, request.TimeSeries);
            return events.OrderBy(x => x.AggregatedEventSum).Take(request.TopN);
        }
    }
}
