using AIAgentFrameworkTests.Models;
using Intellectus.AIAgent.MCPServer.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace AIAgentFrameworkTests
{
    public class OpenAIReasoning
    {
        IServiceProvider _serviceProvider;

        public OpenAIReasoning()            
        {
            var services = new ServiceCollection();

            services.AddIntellectusMCPClient(settings =>
            {
                settings.ServerBaseUrl = "http://localhost:5000";
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task MCPClient_SalesByYear_OpenAI_IncorrectReasoning()
        {
            // Arrange
            var id = 1;
            var userInput = "What is the sales in last year of xyz?";

            var toolName = "SalesTool"; // expected tool name
            var productName = "xyz"; // expected product name
            var totalSales = 500.50m; // expected total sales
            var unitsSold = 50; // expected units sold

            // Incorrect Open AI reasoning result
            // Incorrectly assumes last year is 2022 instead of 2025
            // expected reasoning result
            var reasoningResult = $"TOOL:SalesTool:xyz:{DateTime.UtcNow.Year - 1}";
            var year = DateTime.UtcNow.Year - 1; // expected year

            var client = _serviceProvider.GetRequiredService<IMCPClient>();

            // Act
            var response = await client.PostAsync(id, userInput);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.AgentResponse);
            Assert.False(string.IsNullOrWhiteSpace(response.AgentResponse.Response), "Agent response should not be empty.");
            Assert.NotNull(response.AgentResponse.ToolOutput);
            Assert.False(string.IsNullOrWhiteSpace(response.AgentResponse.ReasoningResult), "Reasoning result should not be empty.");
            Assert.Equal(toolName, response.AgentResponse.ToolName);
            Assert.NotNull(response.AgentResponse.ToolOutput);
            var toolOutputObj = response.AgentResponse.ToolOutput;
            var toolOutput = DeserializeToolOutput<SalesData>(toolOutputObj);
            Assert.Equal(productName, toolOutput.ProductName);
            Assert.Equal(totalSales, toolOutput.TotalSales);
            Assert.Equal(unitsSold, toolOutput.UnitsSold);
            // Incorrect OpenAI reasoning result
            Assert.NotEqual(reasoningResult, response.AgentResponse.ReasoningResult.Replace(" ", ""));
            Assert.NotEqual(year, toolOutput.Year);
        }

        private T DeserializeToolOutput<T>(object toolOutputObj)
        {
            return toolOutputObj switch
            {
                JsonElement je => je.Deserialize<T>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!,
                T sd => sd,
                _ => throw new InvalidCastException($"Unexpected ToolOutput type: {toolOutputObj?.GetType().FullName}")
            };
        }
    }
}
