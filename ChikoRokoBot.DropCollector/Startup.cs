using Azure.Identity;
using Azure.Storage.Queues;
using ChikoRokoBot.DropCollector.Extensions;
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
        private readonly DropCollectorOptions _dropCollectorOptions = new();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _functionConfig = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.Services.Configure<DropCollectorOptions>(_functionConfig.GetSection(nameof(DropCollectorOptions)));

            _functionConfig.GetSection(nameof(DropCollectorOptions)).Bind(_dropCollectorOptions);

            builder.Services.AddBrowsingContext(_functionConfig);

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

