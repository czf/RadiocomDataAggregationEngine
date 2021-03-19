using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Repository.Contracts;

namespace RadiocomDataAggregationEngine.Artist
{
    public class ArtistInfoRequestEngine
    {
        private readonly IRadiocomArtistInfoRepository _radiocomArtistInfoRepository;

        public ArtistInfoRequestEngine(IRadiocomArtistInfoRepository radiocomArtistInfoRepository)
        {
            _radiocomArtistInfoRepository = radiocomArtistInfoRepository;
        }

        public Task<IEnumerable<IArtistInfo>> ProcessArtistInfosRequest(IArtistInfosRequest artistInfosRequest)
        {
            return _radiocomArtistInfoRepository.GetArtistInfosAsync(artistInfosRequest.ArtistIds);
        }
    }
}
