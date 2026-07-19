using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using System;

namespace Intellectus.AIAgent.Framework
{
    public static class Extensions
    {
        public static void AddIntellectusAIAgentFramework(this IServiceCollection services, Action<AgentSettings> configureSettings)
        {
            var settings = new AgentSettings();

            configureSettings(settings);
            
            services.AddSingleton(settings);
            services.AddSingleton(new ChatClient(settings.OpenAILLMModel, settings.OpenAIAPIKey));
            services.AddScoped<IAgent, Agent>();
        }
    }
}
