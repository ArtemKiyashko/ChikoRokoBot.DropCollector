using System;
using AngleSharp;
using Azure.Identity;
using Azure.Storage.Queues;
using ChikoRokoBot.DropCollector.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ChikoRokoBot.DropCollector.Startup))]
namespace ChikoRokoBot.DropCollector
{
	public class Startup : FunctionsStartup
	{
        private IConfigurationRoot _functionConfig;
        private DropCollectorOptions _dropCollectorOptions = new();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _functionConfig = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.Services.Configure<DropCollectorOptions>(_functionConfig.GetSection("DropCollectorOptions"));
            _functionConfig.GetSection("DropCollectorOptions").Bind(_dropCollectorOptions);

            builder.Services.AddTransient<IBrowsingContext>((provider) => { return new BrowsingContext(Configuration.Default.WithDefaultLoader()); });

            builder.Services.AddAzureClients(clientBuilder => {
                clientBuilder.UseCredential(new DefaultAzureCredential());
                clientBuilder
                    .AddQueueServiceClient(_dropCollectorOptions.StorageAccount)
                    .ConfigureOptions((options) => { options.MessageEncoding = QueueMessageEncoding.Base64; });
            });

            builder.Services.AddScoped<QueueClient>((factory) => {
                var service = factory.GetRequiredService<QueueServiceClient>();
                var client = service.GetQueueClient(_dropCollectorOptions.QueueName);
                client.CreateIfNotExists();
                return client;
            });
        }
    }
}

