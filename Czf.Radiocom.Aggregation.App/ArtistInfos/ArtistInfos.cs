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
using Czf.Radiocom.Aggregation.Contracts;
using RadiocomDataAggregationEngine.Artist;

namespace Czf.Radiocom.Aggregation.App.ArtistInfos
{
    public class ArtistInfos
    {
        private readonly ArtistInfoRequestEngine _artistInfosRequestEngine;

        public class ArtistInfosRequest : IArtistInfosRequest
        {
            public IEnumerable<int> ArtistIds { get; set; }
        }

        public class ArtistInfoResponse
        {
            public Dictionary<int, string> Artists { get; set; }
        }

        public ArtistInfos(ArtistInfoRequestEngine artistInfosRequestEngine) 
        {
            _artistInfosRequestEngine = artistInfosRequestEngine;
        }


        [FunctionName("ArtistInfos")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

           

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ArtistInfosRequest data = JsonConvert.DeserializeObject<ArtistInfosRequest>(requestBody);
            IEnumerable<IArtistInfo> result =  await _artistInfosRequestEngine.ProcessArtistInfosRequest(data);



            return new OkObjectResult(result);
        }
    }
}
