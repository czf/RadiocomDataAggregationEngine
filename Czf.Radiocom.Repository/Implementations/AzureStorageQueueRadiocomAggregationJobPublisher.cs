using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Queues;
using Czf.Radiocom.Repository.Contracts;
using Microsoft.Extensions.Options;

namespace Czf.Radiocom.Repository.Implementations
{
    public class AzureStorageQueueRadiocomAggregationJobPublisher : IRadiocomAggregationJobPublisher
    {
        private readonly QueueClient _artistClient;
        private readonly QueueClient _artistWorkClient;

        public AzureStorageQueueRadiocomAggregationJobPublisher(IOptions<AzureStorageQueueRadiocomAggregationJobPublisherOptions> options)
        {

            if (options == null)
            {
                throw new ArgumentNullException("options is null");
            }

            Uri queueUri = new Uri(options.Value.ArtistQueueUri);
            _artistClient = new QueueClient(queueUri, new DefaultAzureCredential());
            queueUri = new Uri(options.Value.ArtistWorkQueueUri);
            _artistWorkClient = new QueueClient(queueUri, new DefaultAzureCredential());
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
