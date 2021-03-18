using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Repository.Contracts;

namespace RadiocomDataAggregationEngine
{
   
    public class RadiocomCompletedCollectorInitiateJobsEngine
    {
        private readonly IRadiocomAggregationJobPublisher _radiocomAggregationJobPublisher;
        private readonly IRadiocomArtistRepository _radiocomArtistRepository;
        private readonly IRadiocomArtistWorkRepository _radiocomArtistWorkRepository;
        public RadiocomCompletedCollectorInitiateJobsEngine(
            IRadiocomAggregationJobPublisher radiocomAggregationJobPublisher,
            IRadiocomArtistRepository radiocomArtistRepository,
            IRadiocomArtistWorkRepository radiocomArtistWorkRepository)
        {
            _radiocomAggregationJobPublisher = radiocomAggregationJobPublisher;
            _radiocomArtistRepository = radiocomArtistRepository;
            _radiocomArtistWorkRepository = radiocomArtistWorkRepository;
        }

        public async Task InitiateJobs()
        {
            foreach (var id in await _radiocomArtistRepository.GetArtistIds())
            {
                await _radiocomAggregationJobPublisher.PublishArtistAsync(id);
            }

            foreach (var id in await _radiocomArtistWorkRepository.GetArtistWorkIds())
            {
                await _radiocomAggregationJobPublisher.PublishArtistWorkAsync(id);
            }
        }
    }
}
