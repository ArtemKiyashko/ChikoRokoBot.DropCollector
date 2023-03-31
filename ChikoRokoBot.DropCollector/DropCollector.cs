using System;
using System.Text.Json;
using System.Threading.Tasks;
using AngleSharp;
using Azure.Storage.Queues;
using ChikoRokoBot.DropCollector.Options;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChikoRokoBot.DropCollector
{
    public class DropCollector
    {
        private readonly IBrowsingContext _browsingContext;
        private readonly QueueClient _queueClient;
        private readonly DropCollectorOptions _options;

        public DropCollector(IBrowsingContext browsingContext, IOptions<DropCollectorOptions> options, QueueClient queueClient)
        {
            _browsingContext = browsingContext;
            _queueClient = queueClient;
            _options = options.Value;
        }

        [FunctionName("DropCollector")]
        public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            var document = await _browsingContext.OpenAsync(_options.Url.AbsoluteUri);
            
            var rawData = document.QuerySelector(_options.DataElementQuerySelector).InnerHtml;

            await _queueClient.SendMessageAsync(rawData);
        }
    }
}

