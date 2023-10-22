using AngleSharp;
using ChikoRokoBot.DropCollector.Options;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChikoRokoBot.DropCollector.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBrowsingContext(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
            var browsingContebxtConfiguration = Configuration.Default.WithDefaultCookies().WithDefaultLoader();
            var webProxyOptions = new WebProxyOptions();
            configuration.GetSection(nameof(WebProxyOptions)).Bind(webProxyOptions);

            if (!string.IsNullOrEmpty(webProxyOptions.Address) && webProxyOptions.Port != default)
            {
                var proxy = new WebProxy(webProxyOptions.Address, webProxyOptions.Port);

                if (!string.IsNullOrEmpty(webProxyOptions.Username))
                    proxy.Credentials = new NetworkCredential(webProxyOptions.Username, webProxyOptions.Password);

                var handler = new HttpClientHandler
                {
                    Proxy = proxy,
                    PreAuthenticate = true,
                    UseDefaultCredentials = false,
                };

                browsingContebxtConfiguration = browsingContebxtConfiguration.WithRequesters(handler);
            }

            services.AddScoped<IBrowsingContext>((provider) => new BrowsingContext(browsingContebxtConfiguration));

            return services;
        }
	}
}

