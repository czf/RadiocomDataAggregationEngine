using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Czf.Radiocom.Repository.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using RadiocomDataAggregationEngine;

namespace Czf.Radiocom.Aggregation.App
{
    public class OnCompletedInitiateJobs
    {
        private readonly RadiocomCompletedCollectorInitiateJobsEngine _radiocomCompletedCollectorInitiateJobsEngine;

        public OnCompletedInitiateJobs(
            RadiocomCompletedCollectorInitiateJobsEngine radiocomCompletedCollectorInitiateJobsEngine
            )
        {
            _radiocomCompletedCollectorInitiateJobsEngine = radiocomCompletedCollectorInitiateJobsEngine;
        }

        [FunctionName("OnCompletedPopulateJobs")]
        public async Task Run([QueueTrigger("czf.radiocom.collection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            await _radiocomCompletedCollectorInitiateJobsEngine.InitiateJobs();
        }
    }
}
