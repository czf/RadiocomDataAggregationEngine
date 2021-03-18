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

namespace Czf.Radiocom.Aggregation.App
{
    public class ArtistsInfo
    {
        public class ArtistInfoRequest
        {
            public IEnumerable<int> ids { get; set; }
            public string v { get; set; }
        }

        [FunctionName("ArtistsInfo")]
        public async Task<IActionResult> Run(
            //[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, [FromQuery] int[] ids,
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]  ArtistInfoRequest request, HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ArtistInfoRequest>(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult("result");
        }
    }
}
