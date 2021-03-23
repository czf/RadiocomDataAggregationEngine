using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Czf.Radiocom.Aggregation.Entities;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace Czf.Radiocom.Aggregation.Cache
{
    public class TableStorageArtistWorkTimeSeriesCache : IArtistWorkTimeSeriesCache
    {
        private const string TABLE_NAME = "ArtistWorkEventCache";
        private readonly CloudTableClient _cloudTableClient;
        private readonly CloudTable _cacheTable;
        public TableStorageArtistWorkTimeSeriesCache(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudTableClient = storageAccount.CreateCloudTableClient();
            _cacheTable = _cloudTableClient.GetTableReference(TABLE_NAME);
            _cacheTable.CreateIfNotExists();
        }

        public IEnumerable<IAggregatedEvent> FetchArtistArtistWorkAggregatedEvents(int artistId, TimeSeries timeSeries)
        {
            //artistId == TimeSeriesValueEntity.ArtistId && timeSeries == TimeSeriesValueEntity.TimeSeries
            var query = new TableQuery<TimeSeriesValueEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterConditionForInt(
                        nameof(TimeSeriesValueEntity.ArtistId),
                        QueryComparisons.Equal,
                        artistId),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(
                        nameof(TimeSeriesValueEntity.TimeSeries),
                        QueryComparisons.Equal,
                        timeSeries.ToString())));

            IEnumerable<TimeSeriesValueEntity> timeSeriesValueEntities =
                _cacheTable.ExecuteQuery(query);

            List<AggregatedEvent> aggregatedEvents = new List<AggregatedEvent>();
            foreach (var entity in timeSeriesValueEntities)
            {
                AggregatedEvent aggregatedEvent = new AggregatedEvent()
                {
                    AggregatedEventSum = entity.TimeSeriesTotal,
                    AggregationTimeSeries = entity.TimeSeries,
                    Id = entity.ArtistWorkId,
                    AggregatedEventSumSource = entity.TimeSeriesValues.Select(x => new AggregatedEventSource() { Timestamp = x.Timestamp, Value = x.Value })
                };
                aggregatedEvents.Add(aggregatedEvent);
            }
            return aggregatedEvents;
        }

        public async Task<IEnumerable<IAggregatedEvent>> FetchTimeSeriesAggregatedEventsAsync(IEnumerable<int> artistWorkIds, TimeSeries timeSeries)
        {
            List<TimeSeriesValueEntity> entities = new List<TimeSeriesValueEntity>();
            if (!artistWorkIds.Any())
            {
                var query = new TableQuery<TimeSeriesValueEntity>().Where(
                    TableQuery.GenerateFilterCondition(
                        nameof(TimeSeriesValueEntity.TimeSeries),
                        QueryComparisons.Equal,
                        timeSeries.ToString())
                    );
                entities = _cacheTable.ExecuteQuery(query).ToList();
            }
            else
            {
                TableBatchOperation batchOperation = new TableBatchOperation();
                foreach (var idBatch in artistWorkIds.Batch(50))
                {
                    foreach(var id in idBatch)
                    {
                        batchOperation.Retrieve<TimeSeriesValueEntity>(id.ToString(), timeSeries.ToString());
                    }
                    TableBatchResult batchResult = await _cacheTable.ExecuteBatchAsync(batchOperation);
                    entities.AddRange(batchResult.Select(x => (TimeSeriesValueEntity)x.Result));
                    
                    batchOperation.Clear();
                }
               
            }
            return entities.Select(entity =>
            {
                
                return new AggregatedEvent()
                {
                    AggregatedEventSum = entity.TimeSeriesTotal,
                    AggregationTimeSeries = entity.TimeSeries,
                    Id = entity.ArtistWorkId,
                    AggregatedEventSumSource = entity.TimeSeriesValues.Select(x => new AggregatedEventSource() { Timestamp = x.Timestamp, Value = x.Value })
                };
            });
        }

        public async Task<IEnumerable<ITimeSeriesValue>> FetchTimeSeriesValuesAsync(int artistWorkId, TimeSeries timeSeries)
        {
            var operation = TableOperation.Retrieve<TimeSeriesValueEntity>(artistWorkId.ToString(), timeSeries.ToString());
            var execResult = await _cacheTable.ExecuteAsync(operation);
            TimeSeriesValueEntity entityResult = (TimeSeriesValueEntity)execResult.Result;
            List<TimeSeriesValue> seriesValues = new List<TimeSeriesValue>();
            foreach (var item in entityResult.TimeSeriesValues)
            {
                seriesValues.Add(new TimeSeriesValue() { Timestamp = item.Timestamp, Value = item.Value });
            }
            return seriesValues;
        }

        public async Task StoreTimeSeriesValuesAsync(IEnumerable<ITimeSeriesValue> timeSeriesEvents, int ArtistWorkId, TimeSeries timeSeries, int artistId)
        {
            var entity =
                new TimeSeriesValueEntity() { ArtistWorkId = ArtistWorkId, ArtistId= artistId, TimeSeries = timeSeries, TimeSeriesValues = timeSeriesEvents, TimeSeriesTotal = timeSeriesEvents.Sum(x=>x.Value) };
            var operation = TableOperation.InsertOrReplace(entity);
            
            await _cacheTable.ExecuteAsync(operation);
        }

        private class TimeSeriesValueEntity : ITableEntity
        {
            private int _artistWorkId;
            private TimeSeries _timeSeries;
            public int ArtistWorkId
            {
                get => _artistWorkId;
                set
                {
                    _artistWorkId = value;
                }
            }
            
            public TimeSeries TimeSeries
            {
                get => _timeSeries;
                set
                {
                    _timeSeries = value;
                    RowKey = value.ToString();
                }
            }
            public int ArtistId { get; set; }
            public long TimeSeriesTotal { get; set; }
            public IEnumerable<ITimeSeriesValue> TimeSeriesValues { get; set; }

            public string PartitionKey { get => _artistWorkId.ToString(); set => _artistWorkId = int.Parse(value); }
            public string RowKey { get => _timeSeries.ToString(); set => _timeSeries = Enum.Parse<TimeSeries>(value); }
            public DateTimeOffset Timestamp { get; set; }
            public string ETag { get; set; }

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                ArtistId = properties[nameof(ArtistId)].Int32Value.Value;
                ArtistWorkId = properties[nameof(ArtistWorkId)].Int32Value.Value;
                TimeSeries = Enum.Parse<TimeSeries>(properties[nameof(TimeSeries)].StringValue);
                TimeSeriesValues = JsonConvert.DeserializeObject<List<TimeSeriesValue>>(properties[nameof(TimeSeriesValues)].StringValue);
                TimeSeriesTotal = properties[nameof(TimeSeriesTotal)].Int64Value.Value;
            }

            public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                
                Dictionary<string, EntityProperty> result = new Dictionary<string, EntityProperty>();
                result[nameof(ArtistWorkId)] = new EntityProperty( ArtistWorkId);
                result[nameof(TimeSeries)] = new EntityProperty( TimeSeries.ToString());
                result[nameof(TimeSeriesValues)] = EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(TimeSeriesValues));
                result[nameof(TimeSeriesTotal)] = EntityProperty.GeneratePropertyForLong(TimeSeriesTotal);
                result[nameof(ArtistId)] = new EntityProperty(ArtistId);
                return result;
            }
        }
    }
}
