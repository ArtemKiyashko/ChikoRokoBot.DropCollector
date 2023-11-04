using System;

namespace ChikoRokoBot.DropCollector.Options
{
    public class DropCollectorOptions
    {
		public Uri Url { get; set; } = new Uri("https://artoys.app/en");
        public string DataElementQuerySelector { get; set; } = "script#__NEXT_DATA__";
        public string StorageAccount { get; set; } = "UseDevelopmentStorage=true";
        public string QueueName { get; set; } = "alldrops";
    }
}

