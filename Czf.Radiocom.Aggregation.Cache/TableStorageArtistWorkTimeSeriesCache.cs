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

        public async Task<IEnumerable<ITimeSeriesValue>> FetchTimeSeriesValuesAsync(int ArtistWorkId, TimeSeries timeSeries)
        {
            var operation = TableOperation.Retrieve<TimeSeriesValueEntity>(ArtistWorkId.ToString(), timeSeries.ToString());
            var execResult = await _cacheTable.ExecuteAsync(operation);
            TimeSeriesValueEntity entityResult = (TimeSeriesValueEntity)execResult.Result;
            List<TimeSeriesValue> seriesValues = new List<TimeSeriesValue>();
            foreach (var item in entityResult.TimeSeriesValues)
            {
                seriesValues.Add(new TimeSeriesValue() { Timestamp = item.Timestamp, Value = item.Value });
            }
            return seriesValues;
        }

        public async Task StoreTimeSeriesValuesAsync(IEnumerable<ITimeSeriesValue> timeSeriesEvents, int ArtistWorkId, TimeSeries timeSeries)
        {
            var entity =
                new TimeSeriesValueEntity() { ArtistWorkId = ArtistWorkId, TimeSeries = timeSeries, TimeSeriesValues = timeSeriesEvents, TimeSeriesTotal = timeSeriesEvents.Sum(x=>x.Value) };
            var operation = TableOperation.InsertOrReplace(entity);
            
            await _cacheTable.ExecuteAsync(operation);
        }

        private class TimeSeriesValueEntity : ITableEntity
        {
            private int _ArtistWorkId;
            private TimeSeries _timeSeries;
            public int ArtistWorkId
            {
                get => _ArtistWorkId;
                set
                {
                    _ArtistWorkId = value;
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

            public string PartitionKey { get => _ArtistWorkId.ToString(); set => _ArtistWorkId = int.Parse(value); }
            public string RowKey { get => _timeSeries.ToString(); set => _timeSeries = Enum.Parse<TimeSeries>(value); }
            public DateTimeOffset Timestamp { get; set; }
            public string ETag { get; set; }

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
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

                return result;
            }
        }
    }
}
