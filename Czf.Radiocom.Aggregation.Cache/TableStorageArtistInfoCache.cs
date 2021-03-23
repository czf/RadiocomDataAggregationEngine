using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Microsoft.Azure.Cosmos.Table;

namespace Czf.Radiocom.Aggregation.Cache
{
    public class TableStorageArtistInfoCache : IArtistInfoCache
    {
        private const string TABLE_NAME = "ArtistInfoCache";
        private readonly CloudTableClient _cloudTableClient;
        private readonly CloudTable _cacheTable;

        public TableStorageArtistInfoCache(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudTableClient = storageAccount.CreateCloudTableClient();
            _cacheTable = _cloudTableClient.GetTableReference(TABLE_NAME);
            _cacheTable.CreateIfNotExists();
        }

        public async Task StoreArtistInfosAsync(IEnumerable<IArtistInfo> artistInfos)
        {
            TableBatchOperation batchOperation = new TableBatchOperation();
            foreach (var entityBatch in artistInfos.Select(x=> new ArtistInfoEntity(x)).Batch(50))
            {
                foreach(var entity in entityBatch)
                {
                    batchOperation.InsertOrReplace(entity);
                }
                await _cacheTable.ExecuteBatchAsync(batchOperation);
                batchOperation.Clear();
            }
        }

        public bool TryGetArtistInfo(int artistId, out IArtistInfo artistInfo)
        {
            //var query = _cacheTable
            //    .CreateQuery<ArtistInfoEntity>()
            //    .Where(
            //    TableQuery.GenerateFilterCondition(
            //        nameof(ArtistInfoEntity.RowKey),
            //        QueryComparisons.Equal, 
            //        artistId.ToString()));

            var query = new TableQuery<ArtistInfoEntity>().Where(
                TableQuery.GenerateFilterCondition(
                    nameof(ArtistInfoEntity.RowKey),
                    QueryComparisons.Equal,
                    artistId.ToString())
                );
            IEnumerable<ArtistInfoEntity> entities = _cacheTable.ExecuteQuery(query);
            artistInfo = entities?.FirstOrDefault();
            return entities.Any();
            
        }

        private class ArtistInfoEntity : ITableEntity, IArtistInfo
        {
            private int _artistId;
            public ArtistInfoEntity() { }
            public ArtistInfoEntity(IArtistInfo info)
            {
                Id = info.Id;
                Name = info.Name;
                PartitionKey = "KISW";
            }

            public int Id
            {
                get => _artistId;
                set
                {
                    _artistId = value;
                }
            }

            public string Name { get; set; }
            public string PartitionKey { get; set; }
            public string RowKey { get => _artistId.ToString(); set => _artistId = int.Parse(value); }
            public DateTimeOffset Timestamp { get; set; }
            public string ETag { get; set; }

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                Id = properties[nameof(Id)].Int32Value.Value;
                Name = properties[nameof(Name)].StringValue;
            }

            public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                Dictionary<string, EntityProperty> result = new Dictionary<string, EntityProperty>();
                result[nameof(Id)] = EntityProperty.GeneratePropertyForInt(Id);
                result[nameof(Name)] = EntityProperty.GeneratePropertyForString(Name);
                return result;
            }
        }

    }
}
