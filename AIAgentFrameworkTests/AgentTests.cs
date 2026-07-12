using AgentFramework.Demo;
using Intellectus.AIAgent.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace AIAgentFrameworkTests
{
    public class AgentTests
    {
        IServiceProvider _serviceProvider;

        public AgentTests() 
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            var services = new ServiceCollection();

            // Register tools with DI
            services.AddScoped<ITool, SalesTool>();
            services.AddScoped<ITool, ProductTool>();

            // Register the framework and configure settings
            services.AddIntellectusAIAgentFramework(settings =>
            {
                settings.OpenAIAPIKey = apiKey;
                settings.OpenAILLMModel = "gpt-4o-mini";
                settings.ReasoningResultContent = @"<ToolInput>:<Year>
                                                    Year is optional.
                                                    ";
                //Add tools without using DI
                //settings.Tools = new List<ITool> { new SalesTool(), new ProductTool() };
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        [Theory]
        [InlineData("What is the sales in 2026 of xyz?", "TOOL:SalesTool:xyz:2026")]
        [InlineData("What is the sales of xyz?", "TOOL:SalesTool:xyz")]
        [InlineData("Give me information about xyz.", "TOOL:ProductTool:xyz")]
        public async Task AIAgent_Tests(string input, string reasoningResult)
        {
            var agent = _serviceProvider.GetRequiredService<IConversationalAgent>();

            var response = await agent.RespondAsync(input);

            Assert.NotNull(response);
            Assert.False(string.IsNullOrWhiteSpace(response.Response), "Agent response should not be empty.");
            Assert.NotNull(response.ToolOutput);
            Assert.False(string.IsNullOrWhiteSpace(response.ReasoningResult), "Reasoning result should not be empty.");
            Assert.Equal(reasoningResult, response.ReasoningResult.Replace(" ", ""));
        }
    }
}
