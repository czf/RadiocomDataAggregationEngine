using System;
using System.Collections.Generic;
using System.Text;
using Czf.Radiocom.Aggregation.Cache;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Event.Repository.Contracts;
using Czf.Radiocom.Event.Repository.Implementations;
using Czf.Radiocom.Repository.Contracts;
using Czf.Radiocom.Repository.Implementations;
using Czf.Radiocom.Shared.Contracts;
using Czf.Radiocom.Shared.Implementations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RadiocomDataAggregationEngine;
using RadiocomDataAggregationEngine.Artist;
using RadiocomDataAggregationEngine.ArtistWork;
using static Czf.Radiocom.Event.Repository.Implementations.SqlArtistEventRepository;
using static Czf.Radiocom.Event.Repository.Implementations.SqlArtistWorkEventRepository;
using static Czf.Radiocom.Repository.Implementations.AzureStorageQueueRadiocomAggregationJobPublisher;
using static Czf.Radiocom.Repository.Implementations.SqlRadiocomArtistInfoRepository;
using static Czf.Radiocom.Repository.Implementations.SqlRadiocomArtistRepository;
using static Czf.Radiocom.Repository.Implementations.SqlRadiocomArtistWorkInfoRepository;
using static Czf.Radiocom.Repository.Implementations.SqlRadiocomArtistWorkRepository;
using static Czf.Radiocom.Shared.Implementations.SqlConnectionFactory;
//using Microsoft.Extensions.DependencyInjection; //microsoft.extensions.http

[assembly: FunctionsStartup(typeof(Czf.Radiocom.Aggregation.App.Startup))]
namespace Czf.Radiocom.Aggregation.App
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services //.AddLogging()
                .AddOptions()
                .AddSingleton<ArtistAggregatedEventRequestEngine>()

                .AddSingleton<RadiocomCompletedCollectorInitiateJobsEngine>()
                .AddSingleton<RadiocomDataArtistEventAggregationEngine>()
                .AddSingleton<RadiocomDataArtistWorkEventAggregationEngine>()
                .AddSingleton<ArtistWorkAggregatedEventRequestEngine>()
                .AddSingleton<Artist_TopArtistWorkAggregatedEventsRequestEngine>()
                .AddSingleton<IRadiocomArtistRepository, SqlRadiocomArtistRepository>()
                .AddSingleton<IRadiocomArtistWorkRepository, SqlRadiocomArtistWorkRepository>()
                .AddSingleton<IRadiocomArtistInfoRepository, SqlRadiocomArtistInfoRepository>()
                .AddSingleton<IRadiocomArtistWorkInfoRepository, SqlRadiocomArtistWorkInfoRepository>()
                .AddSingleton<DualLayerRadiocomArtistInfoRepository>()
                .AddSingleton<DualLayerRadiocomArtistWorkInfoRepository>()
                .AddSingleton(
                x =>
                {
                    DualLayerRadiocomArtistInfoRepository dualLayerRadiocomArtistInfoRepository =  x.GetService<DualLayerRadiocomArtistInfoRepository>();
                    return new ArtistInfoRequestEngine(dualLayerRadiocomArtistInfoRepository);
                })
                .AddSingleton(
                x =>
                {
                    DualLayerRadiocomArtistWorkInfoRepository dualLayerRadiocomArtistWorkInfoRepository = x.GetService<DualLayerRadiocomArtistWorkInfoRepository>();
                    return new ArtistWorkInfoRequestEngine(dualLayerRadiocomArtistWorkInfoRepository);
                })
                .AddSingleton<IArtistInfoCache, TableStorageArtistInfoCache>(
                x =>
                {
                    string connectionString = x.GetService<IConfiguration>().GetValue<string>("TableStorageCacheConnectionString");
                    return new TableStorageArtistInfoCache(connectionString);
                })
                .AddSingleton<IArtistWorkInfoCache, TableStorageArtistWorkInfoCache>(
                x =>
                {
                    string connectionString = x.GetService<IConfiguration>().GetValue<string>("TableStorageCacheConnectionString");
                    return new TableStorageArtistWorkInfoCache(connectionString);
                })
                .AddSingleton<IArtistTimeSeriesCache, TableStorageArtistTimeSeriesCache>(
                x =>
                {
                    string connectionString = x.GetService<IConfiguration>().GetValue<string>("TableStorageCacheConnectionString");
                    return new TableStorageArtistTimeSeriesCache(connectionString);
                })
                .AddSingleton<IArtistWorkTimeSeriesCache, TableStorageArtistWorkTimeSeriesCache>(
                x =>
                {
                    string connectionString = x.GetService<IConfiguration>().GetValue<string>("TableStorageCacheConnectionString");
                    return new TableStorageArtistWorkTimeSeriesCache(connectionString);
                })
                .AddSingleton<IArtistEventsRepository, SqlArtistEventRepository>()
                .AddSingleton<IArtistWorkEventsRepository, SqlArtistWorkEventRepository>()
                .AddSingleton<IDbConnectionFactory, SqlConnectionFactory>()
                .AddSingleton<ITimeSeriesEngine, TimeSeriesEngine>();


            if (Convert.ToBoolean(Environment.GetEnvironmentVariable("IsLocalEnvironment")))
            {
                builder.Services
                   .AddSingleton<IRadiocomAggregationJobPublisher, LocalStorageQueueRadiocomAggregationJobPublisher>();

                //builder.Services
                //.AddOptions()
                //.AddOptions<LocalStorageQueueRadiocomAggregationJobPublisherOptions>()
                //.Configure<IConfiguration>((settings, configuration) => configuration.GetSection(LocalStorageQueueRadiocomAggregationJobPublisherOptions.LocalStorageQueueRadiocomAggregationJobPublisher).Bind(settings));
            }
            else 
            { 
                builder.Services
                   .AddSingleton<IRadiocomAggregationJobPublisher, AzureStorageQueueRadiocomAggregationJobPublisher>();

                builder.Services
                .AddOptions()
                .AddOptions<AzureStorageQueueRadiocomAggregationJobPublisherOptions>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(AzureStorageQueueRadiocomAggregationJobPublisherOptions.AzureStorageQueueRadiocomAggregationJobPublisher).Bind(settings));
            }

            builder.Services
                .AddOptions<SqlRadiocomArtistRepositoryOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(SqlRadiocomArtistRepositoryOptions.SqlRadiocomRepository).Bind(settings);
                });
            
            builder.Services
                .AddOptions<SqlRadiocomArtistWorkRepositoryOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(SqlRadiocomArtistWorkRepositoryOptions.SqlRadiocomRepository).Bind(settings);
                });

            builder.Services
                .AddOptions<SqlArtistEventRepositoryOptions>()
                .Configure<IConfiguration>((settings, configuration) => {
                    configuration.GetSection(SqlArtistEventRepositoryOptions.SqlRadiocomRepository).Bind(settings);
                });
            builder.Services
                .AddOptions<SqlArtistWorkEventRepositoryOptions>()
                .Configure<IConfiguration>((settings, configuration) => {
                    configuration.GetSection(SqlArtistWorkEventRepositoryOptions.SqlRadiocomRepository).Bind(settings);
                });

            builder.Services
                .AddOptions<SqlConnectionFactoryOptions>()
                .Configure<IConfiguration>((settings, configuration) => {
                    configuration.GetSection(SqlConnectionFactoryOptions.SqlConnectionFactory).Bind(settings);
                });

            builder.Services
                .AddOptions<SqlRadiocomArtistInfoRepositoryOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(SqlRadiocomArtistInfoRepositoryOptions.SqlRadiocomRepository).Bind(settings);
                });
            builder.Services
               .AddOptions<SqlRadiocomArtistWorkInfoRepositoryOptions>()
               .Configure<IConfiguration>((settings, configuration) =>
               {
                   configuration.GetSection(SqlRadiocomArtistWorkInfoRepositoryOptions.SqlRadiocomRepository).Bind(settings);
               });
        }
    }
}
