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
    public class DualLayerRadiocomArtistWorkInfoRepository : IRadiocomArtistWorkInfoRepository
    {
        private readonly IArtistWorkInfoCache _artistWorkInfoCache;
        private readonly IRadiocomArtistWorkInfoRepository _sourceRadiocomArtistWorkInfoRepository;
        private readonly ILogger _logger;
        private readonly IArtistWorkTimeSeriesCache _artistWorkTimeSeriesCache;

        public DualLayerRadiocomArtistWorkInfoRepository(IArtistWorkInfoCache ArtistWorkInfoCache, 
            IRadiocomArtistWorkInfoRepository sourceRadiocomArtistWorkInfoRepository, 
            ILogger<DualLayerRadiocomArtistWorkInfoRepository> logger,
            IArtistWorkTimeSeriesCache artistWorkTimeSeriesCache)
        {
            _artistWorkInfoCache = ArtistWorkInfoCache;
            _sourceRadiocomArtistWorkInfoRepository = sourceRadiocomArtistWorkInfoRepository;
            _logger = logger;
            _artistWorkTimeSeriesCache = artistWorkTimeSeriesCache;
        }

        public async Task<IEnumerable<IArtistWorkInfo>> GetArtistWorkInfosAsync(IEnumerable<int> ids)
        {
            List<IArtistWorkInfo> result = new List<IArtistWorkInfo>();
            List<int> sourceLookupIds = new List<int>();
            if (!ids.Any())
            {
                result = await GetAllArtistWorkInfosAsync();
            }
            else
            {
                foreach (var id in ids)
                {
                    if (_artistWorkInfoCache.TryGetArtistWorkInfo(id, out IArtistWorkInfo ArtistWorkInfo))
                    {
                        result.Add(ArtistWorkInfo);
                    }
                    else
                    {
                        _logger.LogInformation(id.ToString());
                        sourceLookupIds.Add(id);
                    }
                }
                if (sourceLookupIds.Any())
                {
                    IEnumerable<IArtistWorkInfo> sourceArtistWorkInfos = await _sourceRadiocomArtistWorkInfoRepository.GetArtistWorkInfosAsync(sourceLookupIds);
                    result.AddRange(sourceArtistWorkInfos);
                    _logger.LogInformation("store ids");
                    await _artistWorkInfoCache.StoreArtistWorkInfosAsync(sourceArtistWorkInfos);
                }
            }
            return result;
        }

        private async Task<List<IArtistWorkInfo>> GetAllArtistWorkInfosAsync()
        {
            List<IArtistWorkInfo> result;
            IEnumerable<IAggregatedEvent> events = await _artistWorkTimeSeriesCache.FetchTimeSeriesAggregatedEventsAsync(Enumerable.Empty<int>(), TimeSeries.ThreeMonths);
            if (events.Any())
            {
                result = (await GetArtistWorkInfosAsync(events.Select(x => x.Id))).ToList();
            }
            else
            {
                result = new List<IArtistWorkInfo>();
            }
            return result;
        }
    }
}
