using AgentFramework.Demo;
using Intellectus.AIAgent.Framework;

namespace AIAgentFrameworkTests
{
    public class AgentWithoutDITests
    {
        private readonly IAgent _agent;

        public AgentWithoutDITests() 
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            _agent = new AgentBuilder()
                            .AddTool(new ProductTool())
                            .AddTool(new SalesTool())
                            .AddOpenAIAPIKey(apiKey)
                            .AddOpenAILLM("gpt-4o-mini")
                            .AddReasoningResult(@"<ProductName>:<Year>
                                                    Year is optional.
                                                 ")
                            .ToAgent();
        }

        [Theory]
        [InlineData("What is the sales in 2026 of xyz?", "SalesTool", 1)]
        [InlineData("What is the sales of xyz?", "SalesTool", 3)]
        [InlineData("Give me information about xyz.", "ProductTool", 5)]
        public async Task AIAgent_Tests(string input, string toolName, int sleep)
        {
            Thread.Sleep(1000 * sleep); // Sleep to avoid rate limiting issues with OpenAI API

            var response = await _agent.RespondAsync(input);

            Assert.NotNull(response);
            Assert.False(string.IsNullOrWhiteSpace(response.Response), "Agent response should not be empty.");
            Assert.NotNull(response.ToolOutput);
            Assert.False(string.IsNullOrWhiteSpace(response.ReasoningResult), "Reasoning result should not be empty.");
            Assert.Equal(toolName, response.ToolName);
        }
    }
}