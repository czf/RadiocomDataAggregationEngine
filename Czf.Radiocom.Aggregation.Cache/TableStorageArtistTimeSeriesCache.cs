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
    public class TableStorageArtistTimeSeriesCache : IArtistTimeSeriesCache
    {
        private const string TABLE_NAME = "ArtistEventCache";
        private readonly CloudTableClient _cloudTableClient;
        private readonly CloudTable _cacheTable;
        public TableStorageArtistTimeSeriesCache(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudTableClient = storageAccount.CreateCloudTableClient();
            _cacheTable = _cloudTableClient.GetTableReference(TABLE_NAME);
            _cacheTable.CreateIfNotExists();
        }

        public async Task<IEnumerable<ITimeSeriesValue>> FetchTimeSeriesValuesAsync(int artistId, TimeSeries timeSeries)
        {
            var operation = TableOperation.Retrieve<TimeSeriesValueEntity>(artistId.ToString(), timeSeries.ToString());
            var execResult = await _cacheTable.ExecuteAsync(operation);
            TimeSeriesValueEntity entityResult = (TimeSeriesValueEntity)execResult.Result;
            List<TimeSeriesValue> seriesValues = new List<TimeSeriesValue>();
            foreach (var item in entityResult.TimeSeriesValues)
            {
                seriesValues.Add(new TimeSeriesValue() { Timestamp = item.Timestamp, Value = item.Value });
            }
            return seriesValues;
        }

        public async Task StoreTimeSeriesValuesAsync(IEnumerable<ITimeSeriesValue> timeSeriesEvents, int artistId, TimeSeries timeSeries)
        {
            var entity =
                new TimeSeriesValueEntity() { ArtistId = artistId, TimeSeries = timeSeries, TimeSeriesValues = timeSeriesEvents, TimeSeriesTotal = timeSeriesEvents.Sum(x => x.Value) };
            var operation = TableOperation.InsertOrReplace(entity);

            await _cacheTable.ExecuteAsync(operation);
        }
        public async Task<IAggregatedEvent> FetchTimeSeriesAggregatedEventAsync(int artistId, TimeSeries timeSeries)
        {
            var operation = TableOperation.Retrieve<TimeSeriesValueEntity>(artistId.ToString(), timeSeries.ToString());
            TimeSeriesValueEntity entity = (TimeSeriesValueEntity)( await _cacheTable.ExecuteAsync(operation)).Result;
            AggregatedEvent aggregatedEvent = new AggregatedEvent()
            {
                AggregatedEventSum = entity.TimeSeriesTotal,
                AggregationTimeSeries = entity.TimeSeries,
                Id = entity.ArtistId,
                AggregatedEventSumSource = entity.TimeSeriesValues.Select(x => new AggregatedEventSource() { Timestamp = x.Timestamp, Value = x.Value })
            };
            return aggregatedEvent;
        }

        public async Task<IEnumerable<IAggregatedEvent>> FetchTimeSeriesAggregatedEventsAsync(IEnumerable<int> artistIds, TimeSeries timeSeries)
        {
            List<IAggregatedEvent> events = new List<IAggregatedEvent>();
            if (artistIds.Any())
            {
                foreach (var id in artistIds)
                {
                    events.Add(await FetchTimeSeriesAggregatedEventAsync(id, timeSeries));
                }
            }
            else
            {
                events.AddRange(FetchAllTimeSeriesAggregatedEvents(timeSeries));
            }
            return events;
        }
        private IEnumerable<IAggregatedEvent> FetchAllTimeSeriesAggregatedEvents(TimeSeries timeSeries)
        {
            List<AggregatedEvent> result = new List<AggregatedEvent>();
            var query = new TableQuery<TimeSeriesValueEntity>().Where(
                TableQuery.GenerateFilterCondition(
                    nameof(TimeSeriesValueEntity.RowKey),
                    QueryComparisons.Equal,
                    timeSeries.ToString())
                );
            IEnumerable<TimeSeriesValueEntity> entities = _cacheTable.ExecuteQuery(query);
            foreach (var entity in entities)
            {
                AggregatedEvent aggregatedEvent = new AggregatedEvent()
                {
                    AggregatedEventSum = entity.TimeSeriesTotal,
                    AggregationTimeSeries = entity.TimeSeries,
                    Id = entity.ArtistId,
                    AggregatedEventSumSource = entity.TimeSeriesValues.Select(x => new AggregatedEventSource() { Timestamp = x.Timestamp, Value = x.Value })
                };
                result.Add(aggregatedEvent);
            }
            return result;
        }


        private class TimeSeriesValueEntity : ITableEntity
        {
            private int _artistId;
            private TimeSeries _timeSeries;
            public int ArtistId
            {
                get => _artistId;
                set
                {
                    _artistId = value;
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
            public long TimeSeriesTotal { get; set; }
            public IEnumerable<ITimeSeriesValue> TimeSeriesValues { get; set; }

            public string PartitionKey { get => _artistId.ToString(); set => _artistId = int.Parse(value); }
            public string RowKey { get => _timeSeries.ToString(); set => _timeSeries = Enum.Parse<TimeSeries>(value); }
            public DateTimeOffset Timestamp { get; set; }
            public string ETag { get; set; }

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                ArtistId = properties[nameof(ArtistId)].Int32Value.Value;
                TimeSeries = Enum.Parse<TimeSeries>(properties[nameof(TimeSeries)].StringValue);
                TimeSeriesValues = JsonConvert.DeserializeObject<List<TimeSeriesValue>>(properties[nameof(TimeSeriesValues)].StringValue);
                TimeSeriesTotal = properties[nameof(TimeSeriesTotal)].Int64Value.Value;
            }

            public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                
                Dictionary<string, EntityProperty> result = new Dictionary<string, EntityProperty>();
                result[nameof(ArtistId)] = new EntityProperty( ArtistId);
                result[nameof(TimeSeries)] = new EntityProperty( TimeSeries.ToString());
                result[nameof(TimeSeriesValues)] = EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(TimeSeriesValues));
                result[nameof(TimeSeriesTotal)] = EntityProperty.GeneratePropertyForLong(TimeSeriesTotal);
                



                return result;
            }
        }
    }
}
