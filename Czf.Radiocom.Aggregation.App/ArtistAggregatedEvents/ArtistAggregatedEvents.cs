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
using RadiocomDataAggregationEngine.Artist;

namespace Czf.Radiocom.Aggregation.App
{
    public class ArtistAggregatedEvents
    {
        private readonly ArtistAggregatedEventRequestEngine _artistAggregatedEventRequestEngine;

        private class ArtistAggregatedEventsRequest : IArtistAggregatedEventsRequest
        {
            public IEnumerable<int> ArtistIds { get; set; }
            public TimeSeries TimeSeries { get; set; }
        }

        public ArtistAggregatedEvents(ArtistAggregatedEventRequestEngine artistAggregatedEventRequestEngine)
        {
            _artistAggregatedEventRequestEngine = artistAggregatedEventRequestEngine;
        }

        [FunctionName("ArtistAggregatedEvents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]  HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ArtistAggregatedEventsRequest>(requestBody);
            var result = await _artistAggregatedEventRequestEngine.GetArtistAggregatedEventsAsync(data);
            

            return new OkObjectResult(result);
        }
    }
}
