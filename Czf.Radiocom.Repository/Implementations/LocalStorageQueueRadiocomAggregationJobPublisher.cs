using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Czf.Radiocom.Repository.Contracts;

namespace Czf.Radiocom.Repository.Implementations
{
    public class LocalStorageQueueRadiocomAggregationJobPublisher : IRadiocomAggregationJobPublisher
    {
        private readonly QueueClient _artistClient;
        private readonly QueueClient _artistWorkClient;

        public LocalStorageQueueRadiocomAggregationJobPublisher()
        {
            _artistClient = new QueueClient("http://127.0.0.1:10001/", "artist");
            _artistWorkClient = new QueueClient("http://127.0.0.1:10001/", "artistWork");
        }

        public Task PublishArtistAsync(int id)
        {
            try
            {
                
                return _artistClient.SendMessageAsync(id.ToString());
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

                return _artistWorkClient.SendMessageAsync(id.ToString());
            }
            catch
            {
                return Task.CompletedTask;
            }
        }

    }
}
