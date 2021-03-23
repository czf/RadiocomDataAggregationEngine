using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Czf.Radiocom.Aggregation.Contracts;
using RadiocomDataAggregationEngine.ArtistWork;

namespace Czf.Radiocom.Aggregation.App
{
    public class ArtistWorkAggregatedEvents
    {
        private readonly ArtistWorkAggregatedEventRequestEngine _ArtistWorkAggregatedEventRequestEngine;

        private class ArtistWorkAggregatedEventsRequest : IArtistWorkAggregatedEventsRequest
        {
            public IEnumerable<int> ArtistWorkIds { get; set; }
            public TimeSeries TimeSeries { get; set; }
        }

        public ArtistWorkAggregatedEvents(ArtistWorkAggregatedEventRequestEngine ArtistWorkAggregatedEventRequestEngine)
        {
            _ArtistWorkAggregatedEventRequestEngine = ArtistWorkAggregatedEventRequestEngine;
        }

        [FunctionName("ArtistWorkAggregatedEvents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]  HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ArtistWorkAggregatedEventsRequest>(requestBody);
            var result = await _ArtistWorkAggregatedEventRequestEngine.GetArtistWorkAggregatedEventsAsync(data);
            

            return new OkObjectResult(result);
        }
    }
}
