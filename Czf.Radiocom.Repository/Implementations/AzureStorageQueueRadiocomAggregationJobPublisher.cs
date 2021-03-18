using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Queues;
using Czf.Radiocom.Repository.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Czf.Radiocom.Repository.Implementations
{
    public class AzureStorageQueueRadiocomAggregationJobPublisher : IRadiocomAggregationJobPublisher
    {
        private readonly QueueClient _artistClient;
        private readonly QueueClient _artistWorkClient;

        public AzureStorageQueueRadiocomAggregationJobPublisher(IOptions<AzureStorageQueueRadiocomAggregationJobPublisherOptions> options, ILogger<AzureStorageQueueRadiocomAggregationJobPublisher> logger)
        {

            if (options == null)
            {
                throw new ArgumentNullException("options is null");
            }

            if(options.Value.ArtistQueueUri == options.Value.ArtistWorkQueueUri)
            {
                throw new Exception("uris are the same");
            }

            logger.LogInformation($"ArtistQueueUri: {options.Value.ArtistQueueUri}");
            Uri artistQueueUri = new Uri(options.Value.ArtistQueueUri);
            _artistClient = new QueueClient(artistQueueUri, new DefaultAzureCredential());
            Uri artistWorkQueueUri = new Uri(options.Value.ArtistWorkQueueUri);
            logger.LogInformation($"ArtistWorkQueueUri: {options.Value.ArtistWorkQueueUri}");
            _artistWorkClient = new QueueClient(artistWorkQueueUri, new DefaultAzureCredential());
        }

        
        public Task PublishArtistAsync(int id)
        {
            try
            {
                return _artistClient.SendMessageAsync(Base64Encode(id.ToString()));
            }
            catch
            {
                return Task.CompletedTask;
            }
        }
        
        public Task PublishArtistWorkAsync(int id)
        {
            try
            {
                return _artistWorkClient.SendMessageAsync(Base64Encode(id.ToString()));
            }
            catch
            {
                return Task.CompletedTask;
            }
        }

        public class AzureStorageQueueRadiocomAggregationJobPublisherOptions
        {
            public const string AzureStorageQueueRadiocomAggregationJobPublisher = "AzureStorageQueueRadiocomAggregationJobPublisherOptions";
            public string ArtistQueueUri { get; set; }
            public string ArtistWorkQueueUri { get; set; }
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
