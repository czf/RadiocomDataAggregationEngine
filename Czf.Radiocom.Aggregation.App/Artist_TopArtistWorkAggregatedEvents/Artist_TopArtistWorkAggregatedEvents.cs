using System.IO;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RadiocomDataAggregationEngine.Artist;

namespace Czf.Radiocom.Aggregation.App.Artist_TopArtistWorkAggregatedEvents
{
    public class Artist_TopArtistWorkAggregatedEvents
    {
        private readonly Artist_TopArtistWorkAggregatedEventsRequestEngine _artist_TopArtistWorkAggregatedEventsRequestEngine;

        public Artist_TopArtistWorkAggregatedEvents(Artist_TopArtistWorkAggregatedEventsRequestEngine artist_TopArtistWorkAggregatedEventsRequestEngine)
        {
            _artist_TopArtistWorkAggregatedEventsRequestEngine = artist_TopArtistWorkAggregatedEventsRequestEngine;
        }

        [FunctionName("Artist_TopArtistWorkAggregatedEvents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Artist_TopArtistWorkAggregatedEventsRequest>(requestBody);
            var result = _artist_TopArtistWorkAggregatedEventsRequestEngine.GetTopNArtistWorkAggregatedEvents(data);


            return new OkObjectResult(result);
        }

        private class Artist_TopArtistWorkAggregatedEventsRequest : IArtist_TopArtistWorkAggregatedEventsRequest
        {
            public int ArtistId { get; set; }
            public int TopN { get; set; }
            public TimeSeries TimeSeries{get; set;}
        }
    }
}

