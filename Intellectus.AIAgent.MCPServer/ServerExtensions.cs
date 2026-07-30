using Intellectus.AIAgent.Framework;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Intellectus.AIAgent.MCPServer
{
    public static class ServerExtensions
    {
        public static IServiceCollection AddIntellectusAIAgentMCPServer(this IServiceCollection services, Action<AgentSettings> configureSettings)
        {
            services.AddIntellectusAIAgentFramework(configureSettings);

            services
                .AddMcpServer()
                .WithStdioServerTransport()
                .WithToolsFromAssembly();

            return services;
        }
    }
}
