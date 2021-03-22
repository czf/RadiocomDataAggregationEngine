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
    public class DualLayerRadiocomArtistWorkInfoRepository : IRadiocomArtistWorkInfoRepository
    {
        private readonly IArtistWorkInfoCache _artistWorkInfoCache;
        private readonly IRadiocomArtistWorkInfoRepository _sourceRadiocomArtistWorkInfoRepository;
        private readonly ILogger _logger;

        public DualLayerRadiocomArtistWorkInfoRepository(IArtistWorkInfoCache ArtistWorkInfoCache, IRadiocomArtistWorkInfoRepository sourceRadiocomArtistWorkInfoRepository, ILogger logger)
        {
            _artistWorkInfoCache = ArtistWorkInfoCache;
            _sourceRadiocomArtistWorkInfoRepository = sourceRadiocomArtistWorkInfoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<IArtistWorkInfo>> GetArtistWorkInfosAsync(IEnumerable<int> ids)
        {
            List<IArtistWorkInfo> result = new List<IArtistWorkInfo>();
            List<int> sourceLookupIds = new List<int>();
            foreach (var id in ids)
            {
                if (_artistWorkInfoCache.TryGetArtistWorkInfo(id, out IArtistWorkInfo ArtistWorkInfo))
                {
                    result.Add(ArtistWorkInfo);
                }
                else
                {
                    sourceLookupIds.Add(id);
                }
            }
            if (sourceLookupIds.Any())
            {
                IEnumerable<IArtistWorkInfo> sourceArtistWorkInfos = await _sourceRadiocomArtistWorkInfoRepository.GetArtistWorkInfosAsync(sourceLookupIds);
                result.AddRange(sourceArtistWorkInfos);
                await _artistWorkInfoCache.StoreArtistWorkInfosAsync(sourceArtistWorkInfos);
            }
            return result;
        }

        
    }
}
