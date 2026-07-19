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
                            .AddReasoningResultContent(@"<ToolInput>:<Year>
                                                        Year is optional.
                                                        ")
                            .ToAgent();
        }

        [Theory]
        [InlineData("What is the sales in 2026 of xyz?", "TOOL:SalesTool:xyz:2026", 1)]
        [InlineData("What is the sales of xyz?", "TOOL:SalesTool:xyz", 3)]
        [InlineData("Give me information about xyz.", "TOOL:ProductTool:xyz", 5)]
        public async Task AIAgent_Tests(string input, string reasoningResult, int sleep)
        {
            Thread.Sleep(1000 * sleep); // Sleep to avoid rate limiting issues with OpenAI API

            var response = await _agent.RespondAsync(input);

            Assert.NotNull(response);
            Assert.False(string.IsNullOrWhiteSpace(response.Response), "Agent response should not be empty.");
            Assert.NotNull(response.ToolOutput);
            Assert.False(string.IsNullOrWhiteSpace(response.ReasoningResult), "Reasoning result should not be empty.");
            Assert.Equal(reasoningResult, response.ReasoningResult.Replace(" ", ""));
        }
    }
}