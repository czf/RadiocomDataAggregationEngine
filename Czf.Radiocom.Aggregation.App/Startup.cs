using System;
using System.Collections.Generic;
using System.Text;
using Czf.Radiocom.Event.Repository.Contracts;
using Czf.Radiocom.Event.Repository.Implementations;
using Czf.Radiocom.Repository.Contracts;
using Czf.Radiocom.Repository.Implementations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiocomDataAggregationEngine;
using static Czf.Radiocom.Event.Repository.Implementations.SqlArtistEventRepository;
using static Czf.Radiocom.Event.Repository.Implementations.SqlConnectionFactory;
using static Czf.Radiocom.Repository.Implementations.AzureStorageQueueRadiocomAggregationJobPublisher;
//using Microsoft.Extensions.DependencyInjection; //microsoft.extensions.http

[assembly: FunctionsStartup(typeof(Czf.Radiocom.Aggregation.App.Startup))]
namespace Czf.Radiocom.Aggregation.App
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services//.AddLogging()
                .AddOptions()
                .AddSingleton<RadiocomCompletedCollectorInitiateJobsEngine>()
                .AddSingleton<RadiocomDataArtistEventAggregationEngine>()
                .AddSingleton<IArtistEventsRepository, SqlArtistEventRepository>()
                .AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

            if (Convert.ToBoolean(Environment.GetEnvironmentVariable("IsLocalEnvironment")))
            {

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
                .AddOptions<SqlArtistEventRepositoryOptions>()
                .Configure<IConfiguration>((settings, configuration) => {
                    configuration.GetSection(SqlArtistEventRepositoryOptions.SqlRadiocomRepository).Bind(settings);
                });

            builder.Services
                .AddOptions<SqlConnectionFactoryOptions>()
                .Configure<IConfiguration>((settings, configuration) => {
                    configuration.GetSection(SqlConnectionFactoryOptions.SqlConnectionFactory).Bind(settings);
                });
        }
    }
}
