using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RadiocomDataAggregationEngine.ArtistWork;

namespace Czf.Radiocom.Aggregation.App.ArtistWorkInfos
{
    public class ArtistWorkInfos
    {
        private readonly ArtistWorkInfoRequestEngine _artistWorkInfosRequestEngine;

        public class ArtistWorkInfosRequest : IArtistWorkInfoRequest
        {
            public IEnumerable<int> ArtistWorkIds { get; set; }
        }

        public ArtistWorkInfos(ArtistWorkInfoRequestEngine artistWorkInfosRequestEngine)
        {
            _artistWorkInfosRequestEngine = artistWorkInfosRequestEngine;
        }

        [FunctionName("ArtistWorkInfos")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ArtistWorkInfosRequest data = JsonConvert.DeserializeObject<ArtistWorkInfosRequest>(requestBody);
            IEnumerable<IArtistWorkInfo> response = await _artistWorkInfosRequestEngine.ProcessArtistWorkInfosRequest(data);
            

            
            return new OkObjectResult(response);
        }
    }
}

