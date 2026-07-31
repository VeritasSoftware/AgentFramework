using Intellectus.AIAgent.Framework;

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
                .WithHttpTransport(options =>
                {
                    // Stateless mode is recommended for servers that don't need
                    // server-to-client requests like sampling or elicitation.
                    // See https://csharp.sdk.modelcontextprotocol.io/concepts/transports/transports.html for details.
                    options.Stateless = true;
                })
                .WithToolsFromAssembly();

            return services;
        }

        public static WebApplication UseIntellectusAIAgentMCPServer(this WebApplication app)
        {
            app.MapMcp("mcp");
            app.UseHttpsRedirection();

            return app;
        }
    }
}
