using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http.Headers;

namespace Intellectus.AIAgent.MCPServer.Client
{
    public static class ClientExtensions
    {
        public static IServiceCollection AddIntellectusMCPClient(this IServiceCollection services,
                                                                 Action<MCPClientSettings> getSettings)
        {
            var settings = new MCPClientSettings();

            getSettings(settings);

            services.AddSingleton(settings);

            // Register HttpClient via IHttpClientFactory
            services.AddHttpClient("MCPClient", client =>
            {
                client.BaseAddress = new Uri(settings.ServerBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
            });

            services.AddScoped<IMCPClient, MCPClient>();

            return services;
        }
    }
}
