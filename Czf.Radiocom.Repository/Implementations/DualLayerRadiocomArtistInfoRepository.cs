using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Repository.Contracts;
using Microsoft.Extensions.Logging;

namespace Czf.Radiocom.Repository.Implementations
{
    public class DualLayerRadiocomArtistInfoRepository : IRadiocomArtistInfoRepository
    {
        private readonly IArtistInfoCache _artistInfoCache;
        private readonly IRadiocomArtistInfoRepository _sourceRadiocomArtistInfoRepository;
        private readonly ILogger _logger;

        public DualLayerRadiocomArtistInfoRepository(IArtistInfoCache artistInfoCache, IRadiocomArtistInfoRepository sourceRadiocomArtistInfoRepository, ILogger logger)
        {
            _artistInfoCache = artistInfoCache;
            _sourceRadiocomArtistInfoRepository = sourceRadiocomArtistInfoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<IArtistInfo>> GetArtistInfosAsync(IEnumerable<int> ids)
        {
            List<IArtistInfo> result = new List<IArtistInfo>();
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
                await _artistInfoCache.StoreArtistInfosAsync(sourceArtistInfos);
            }
            return result;
        }

        
    }
}
