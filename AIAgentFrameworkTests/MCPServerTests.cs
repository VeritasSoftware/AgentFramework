using AIAgentFrameworkTests.Models;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.Text.Json;

namespace AIAgentFrameworkTests
{
    public class MCPServerTests : IAsyncLifetime
    {
        McpClient? _mcpClient;

        public async Task InitializeAsync()
        {
            var transport = new StdioClientTransport(new()
            {
                Command = "dotnet run",
                Arguments = ["--project", @"..\..\..\..\MCPServer.Sample"],
                Name = "Intellectus AI Agent MCP Server",
            });
            _mcpClient = await McpClient.CreateAsync(transport);
        }

        [Theory]
        [InlineData("What is the sales in 2026 of xyz?", "SalesTool", 1)]
        [InlineData("What is the sales of xyz?", "SalesTool", 3)]
        [InlineData("Give me information about xyz.", "ProductTool", 5)]
        public async Task AIAgent_MCPServer_Tests(string userInput, string toolName, int sleep)
        {
            Thread.Sleep(sleep * 1000);

            // Arrange
            var param = new CallToolRequestParams
            {
                Name = "ai_agent_respond",
                Arguments = new Dictionary<string, JsonElement>
                {
                    { "userInput", JsonSerializer.SerializeToElement(userInput) }
                }
            };

            // Act
            var result = await _mcpClient!.CallToolAsync(param);

            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            var response = JsonSerializer.Deserialize<MCPResponse>(result.Content[0].ToString(), options);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.AgentResponse);
            Assert.False(string.IsNullOrWhiteSpace(response.AgentResponse.Response), "Agent response should not be empty.");
            Assert.NotNull(response.AgentResponse.ToolOutput);
            Assert.False(string.IsNullOrWhiteSpace(response.AgentResponse.ReasoningResult), "Reasoning result should not be empty.");
            Assert.Equal(toolName, response.AgentResponse.ToolName);
        }

        public async Task DisposeAsync()
        {
            if (_mcpClient != null)
            {
                await _mcpClient.DisposeAsync();
            }
        }
    }
}
