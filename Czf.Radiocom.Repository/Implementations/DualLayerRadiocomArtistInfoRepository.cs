using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Czf.Radiocom.Repository.Contracts;
using Microsoft.Extensions.Logging;

namespace Czf.Radiocom.Repository.Implementations
{
    public class DualLayerRadiocomArtistInfoRepository : IRadiocomArtistInfoRepository
    {
        private readonly IArtistInfoCache _artistInfoCache;
        private readonly IRadiocomArtistInfoRepository _sourceRadiocomArtistInfoRepository;
        private readonly ILogger _logger;
        private readonly IArtistTimeSeriesCache _artistTimeSeriesCache;

        public DualLayerRadiocomArtistInfoRepository(
            IArtistInfoCache artistInfoCache, 
            IRadiocomArtistInfoRepository sourceRadiocomArtistInfoRepository, 
            ILogger<DualLayerRadiocomArtistInfoRepository> logger,
            IArtistTimeSeriesCache artistTimeSeriesCache)
        {
            _artistInfoCache = artistInfoCache;
            _sourceRadiocomArtistInfoRepository = sourceRadiocomArtistInfoRepository;
            _logger = logger;
            _artistTimeSeriesCache = artistTimeSeriesCache;

        }

        public async Task<IEnumerable<IArtistInfo>> GetArtistInfosAsync(IEnumerable<int> ids)
        {
            List<IArtistInfo> result = new List<IArtistInfo>();
            if (!ids.Any())
            {
                result = await GetAllArtistInfosAsync();
            }
            else
            {
                List<int> sourceLookupIds = new List<int>();
                foreach (var id in ids)
                {
                    if (_artistInfoCache.TryGetArtistInfo(id, out IArtistInfo artistInfo))
                    {
                        result.Add(artistInfo);
                    }
                    else
                    {
                        sourceLookupIds.Add(id);
                    }
                }
                if (sourceLookupIds.Any())
                {
                    IEnumerable<IArtistInfo> sourceArtistInfos = await _sourceRadiocomArtistInfoRepository.GetArtistInfosAsync(sourceLookupIds);
                    result.AddRange(sourceArtistInfos);
                    _logger.LogInformation(sourceArtistInfos.Count().ToString());
                    await _artistInfoCache.StoreArtistInfosAsync(sourceArtistInfos);
                }
            }
            return result;
        }

        private async Task<List<IArtistInfo>> GetAllArtistInfosAsync()
        {
            List<IArtistInfo> result;
            IEnumerable<IAggregatedEvent> events = await _artistTimeSeriesCache.FetchTimeSeriesAggregatedEventsAsync(Enumerable.Empty<int>(), TimeSeries.ThreeMonths);
            if (events.Any()) 
            {
                result = (await GetArtistInfosAsync(events.Select(x => x.Id))).ToList();
            }
            else
            {
                result = new List<IArtistInfo>();
            }
            return result;
        }
    }
}
