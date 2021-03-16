using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using RadiocomDataAggregationEngine;

namespace Czf.Radiocom.Aggregation.App
{
    public class RunArtistAggregationJob
    {
        private readonly RadiocomDataArtistEventAggregationEngine _radiocomDataArtistEventAggregationEngine;

        public RunArtistAggregationJob(RadiocomDataArtistEventAggregationEngine radiocomDataArtistEventAggregationEngine)
        {
            _radiocomDataArtistEventAggregationEngine = radiocomDataArtistEventAggregationEngine;
        }

        [FunctionName("RunArtistAggregationJob")]
        public async Task Run([QueueTrigger("czf-radiocom-artist-aggregationjob")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            await _radiocomDataArtistEventAggregationEngine.ProcessArtist(int.Parse(myQueueItem));
        }
    }
}
