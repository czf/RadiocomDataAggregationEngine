using System;
using System.Linq;
using Microsoft.Azure.Cosmos.Table;
using Czf.Radiocom.Event.Repository.Contracts;
using Czf.Radiocom.Event.Repository.Implementations;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using static Czf.Radiocom.Event.Repository.Implementations.SqlArtistEventRepository;
using static Czf.Radiocom.Event.Repository.Implementations.SqlConnectionFactory;
using Czf.Radiocom.Aggregation.Cache;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using System.Collections.Generic;

namespace Czf.Radiocom.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var options = Substitute.For<IOptions<SqlArtistEventRepositoryOptions>>();
            SqlArtistEventRepositoryOptions optionsValue = new SqlArtistEventRepositoryOptions()
            {
                ConnectionString = "Server=.;Database=radiocom;Trusted_Connection=True;",
                
            };
            options.Value.Returns(optionsValue);

            var factoryOptions = Substitute.For<IOptionsMonitor<SqlConnectionFactoryOptions>>();
            SqlConnectionFactoryOptions factoryOptionsValue = new SqlConnectionFactoryOptions()
            {
                SetAzureAdAccessToken = false
            };
            factoryOptions.CurrentValue.Returns(factoryOptionsValue);

            //var connectionFactory = Substitute.For<IDbConnectionFactory>();
            //connectionFactory.GetConnection(Arg.Any<string>());

            SqlConnectionFactory connectionFactory = new SqlConnectionFactory(factoryOptions);
            SqlArtistEventRepository sqlArtistEventRepository = new SqlArtistEventRepository(options, connectionFactory);
            var r = sqlArtistEventRepository.GetEventsForTimeSeriesAsync(Aggregation.Contracts.Values.TimeSeries.OneYear, 108).Result;
            var f = r.GroupBy(x => new DateTime(x.TimeStamp.Year, x.TimeStamp.Month, 1)).OrderBy(x => x.Key).Select(x=>new { x.Key, Count = x.Count() }).ToList();
        }

        [Test]
        public void Test2() 
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var tableReference = client.GetTableReference("ArtistEventCache");
            tableReference.CreateIfNotExists();
            var operation  = TableOperation.Insert(new entity() { Id = 44, ArtistName = "some name" , RowKey = string.Empty });
            var operationResult = tableReference.Execute(operation);
            
        }
        public class entity : TableEntity
        {
            private int _Id;
            public int Id
            {
                get => _Id;
                set
                {
                    _Id = value;
                    PartitionKey = value.ToString();
                }
            }
            public string ArtistName { get; set; }

        }

        [Test]
        public async Task Test3()
        {
            TableStorageArtistTimeSeriesCache cacheStorage = new TableStorageArtistTimeSeriesCache("UseDevelopmentStorage=true");
            List<TempSeriesValue> l = new List<TempSeriesValue>()
            {
                new TempSeriesValue(){Timestamp = DateTimeOffset.Now.AddDays(-1), Value = 1},
                new TempSeriesValue(){Timestamp = DateTimeOffset.Now.AddDays(-2), Value = 2},
                new TempSeriesValue(){Timestamp = DateTimeOffset.Now.AddDays(-3), Value = 3},
                new TempSeriesValue(){Timestamp = DateTimeOffset.Now.AddDays(-4), Value = 4},
                new TempSeriesValue(){Timestamp = DateTimeOffset.Now.AddDays(-5), Value = 5},
                new TempSeriesValue(){Timestamp = DateTimeOffset.Now.AddDays(-6), Value = 6},
                new TempSeriesValue(){Timestamp = DateTimeOffset.Now.AddDays(-7), Value = 7}
            };
            await cacheStorage.StoreTimeSeriesValuesAsync(l, 11, Aggregation.Contracts.Values.TimeSeries.SevenDays);
        }

        public class TempSeriesValue : ITimeSeriesValue
        {
            public DateTimeOffset Timestamp { get ; set ; }
            public long Value { get ; set ; }
        }

        [Test]
        public async Task Test4() 
        {
            TableStorageArtistTimeSeriesCache cacheStorage = new TableStorageArtistTimeSeriesCache("UseDevelopmentStorage=true");
            //List<TempSeriesValue> l = new List<TempSeriesValue>(){
            //    new TempSeriesValue() { Timestamp = DateTimeOffset.Now.AddDays(-1), Value = 1 },
            //    new TempSeriesValue() { Timestamp = DateTimeOffset.Now.AddDays(-2), Value = 2 },
            //    new TempSeriesValue() { Timestamp = DateTimeOffset.Now.AddDays(-3), Value = 3 },
            //    new TempSeriesValue() { Timestamp = DateTimeOffset.Now.AddDays(-4), Value = 4 },
            //    new TempSeriesValue() { Timestamp = DateTimeOffset.Now.AddDays(-5), Value = 5 },
            //    new TempSeriesValue() { Timestamp = DateTimeOffset.Now.AddDays(-6), Value = 6 },
            //    new TempSeriesValue() { Timestamp = DateTimeOffset.Now.AddDays(-7), Value = 7 }
            //};
            var result = await cacheStorage.FetchTimeSeriesValuesAsync(11, Aggregation.Contracts.Values.TimeSeries.SevenDays);

        }

    }
}
//https://docs.microsoft.com/en-us/azure/cosmos-db/tutorial-develop-table-dotnet?toc=https%3A%2F%2Fdocs.microsoft.com%2Fen-us%2Fazure%2Fstorage%2Ftables%2Ftoc.json&bc=https%3A%2F%2Fdocs.microsoft.com%2Fen-us%2Fazure%2Fbread%2Ftoc.json#install-the-required-nuget-package