using Microsoft.Extensions.DependencyInjection;
using System;

namespace Intellectus.AIAgent.Framework
{
    public static class Extensions
    {
        public static void AddIntellectusAIAgentFramework(this IServiceCollection services, Action<ConversationalAgentSettings> configureSettings)
        {
            var settings = new ConversationalAgentSettings();

            configureSettings(settings);
            
            services.AddSingleton(settings);
            services.AddScoped<IConversationalAgent, ConversationalAgent>();
        }
    }
}
