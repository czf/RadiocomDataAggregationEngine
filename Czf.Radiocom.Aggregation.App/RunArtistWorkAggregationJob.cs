using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using RadiocomDataAggregationEngine;

namespace Czf.Radiocom.Aggregation.App
{
    public class RunArtistWorkAggregationJob
    {
        private readonly RadiocomDataArtistWorkEventAggregationEngine _radiocomDataArtistEventAggregationEngine;

        public RunArtistWorkAggregationJob(RadiocomDataArtistWorkEventAggregationEngine radiocomDataArtistEventAggregationEngine)
        {
            _radiocomDataArtistEventAggregationEngine = radiocomDataArtistEventAggregationEngine;
        }

        [FunctionName("RunArtistWorkAggregationJob")]
        public async Task Run([QueueTrigger("czf-radiocom-artistwork-aggregationjob")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            await _radiocomDataArtistEventAggregationEngine.ProcessArtistWork(int.Parse(myQueueItem));
        }
    }
}
