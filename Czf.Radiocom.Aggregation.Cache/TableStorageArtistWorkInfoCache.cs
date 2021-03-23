using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Microsoft.Azure.Cosmos.Table;

namespace Czf.Radiocom.Aggregation.Cache
{
    public class TableStorageArtistWorkInfoCache : IArtistWorkInfoCache
    {
        private const string TABLE_NAME = "ArtistWorkInfoCache";
        private readonly CloudTableClient _cloudTableClient;
        private readonly CloudTable _cacheTable;

        public TableStorageArtistWorkInfoCache(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudTableClient = storageAccount.CreateCloudTableClient();
            _cacheTable = _cloudTableClient.GetTableReference(TABLE_NAME);
            _cacheTable.CreateIfNotExists();
        }

        public async Task StoreArtistWorkInfosAsync(IEnumerable<IArtistWorkInfo> ArtistWorkInfos)
        {
            TableBatchOperation batchOperation = new TableBatchOperation();
            foreach (var entityBatch in ArtistWorkInfos.Select(x=> new ArtistWorkInfoEntity(x)).Batch(50))
            {
                foreach (var entity in entityBatch)
                {
                    batchOperation.InsertOrReplace(entity);
                }
                await _cacheTable.ExecuteBatchAsync(batchOperation);
                batchOperation.Clear();
            }
            
        }

        public bool TryGetArtistWorkInfo(int artistWorkId, out IArtistWorkInfo ArtistWorkInfo)
        {
            var query = new TableQuery<ArtistWorkInfoEntity>().Where(
                TableQuery.GenerateFilterCondition(
                    nameof(ArtistWorkInfoEntity.RowKey),
                    QueryComparisons.Equal,
                    artistWorkId.ToString())
                );
            IEnumerable<ArtistWorkInfoEntity> entities = _cacheTable.ExecuteQuery(query);
            ArtistWorkInfo = entities?.FirstOrDefault();
            return entities.Any();
            
        }

        private class ArtistWorkInfoEntity : ITableEntity, IArtistWorkInfo
        {
            private int _artistWorkId;
            public ArtistWorkInfoEntity() { }
            public ArtistWorkInfoEntity(IArtistWorkInfo info)
            {
                Id = info.Id;
                Title = info.Title;
                ArtistId = info.ArtistId;
                PartitionKey = "KISW";
            }

            public int Id
            {
                get => _artistWorkId;
                set
                {
                    _artistWorkId = value;
                }
            }

            public string Title { get; set; }
            public string PartitionKey { get; set; }
            public string RowKey { get => _artistWorkId.ToString(); set => _artistWorkId = int.Parse(value); }
            public DateTimeOffset Timestamp { get; set; }
            public string ETag { get; set; }
            public int ArtistId { get; set; }

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                Id = properties[nameof(Id)].Int32Value.Value;
                Title = properties[nameof(Title)].StringValue;
                ArtistId = properties[nameof(ArtistId)].Int32Value.Value;
            }

            public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                Dictionary<string, EntityProperty> result = new Dictionary<string, EntityProperty>();
                result[nameof(Id)] = EntityProperty.GeneratePropertyForInt(Id);
                result[nameof(Title)] = EntityProperty.GeneratePropertyForString(Title);
                result[nameof(ArtistId)] = EntityProperty.GeneratePropertyForInt(ArtistId);

                return result;
            }
        }

    }
}
