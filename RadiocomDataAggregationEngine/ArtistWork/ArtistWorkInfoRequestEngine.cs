using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Repository.Contracts;

namespace RadiocomDataAggregationEngine.ArtistWork
{
    public class ArtistWorkInfoRequestEngine
    {
        private readonly IRadiocomArtistWorkInfoRepository _radiocomArtistWorkInfoRepository;

        public ArtistWorkInfoRequestEngine(IRadiocomArtistWorkInfoRepository radiocomArtistWorkInfoRepository)
        {
            _radiocomArtistWorkInfoRepository = radiocomArtistWorkInfoRepository;
        }

        public Task<IEnumerable<IArtistWorkInfo>> ProcessArtistWorkInfosRequest(IArtistWorkInfoRequest ArtistWorkInfosRequest)
        {
            return _radiocomArtistWorkInfoRepository.GetArtistWorkInfosAsync(ArtistWorkInfosRequest.ArtistWorkIds);
        }
    }
}
