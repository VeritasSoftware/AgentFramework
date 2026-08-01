using AIAgentFrameworkTests.Models;
using Intellectus.AIAgent.MCPServer.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace AIAgentFrameworkTests
{
    public class MCPClientTests
    {
        IServiceProvider _serviceProvider;

        public MCPClientTests()
        {
            var services = new ServiceCollection();

            services.AddIntellectusMCPClient(settings =>
            {
                settings.ServerBaseUrl = "http://localhost:5000";
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task MCPClient_SalesByYear()
        {
            // Arrange
            var id = 1;
            var userInput = "What is the sales in 2026 of xyz?";

            var toolName = "SalesTool";
            var productName = "xyz"; // expected product name
            var totalSales = 500.50m; // expected total sales
            var unitsSold = 50; // expected units sold
            var year = 2026; // expected year

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
            var toolOutput = toolOutputObj switch
            {
                JsonElement je => je.Deserialize<SalesData>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!,
                SalesData sd => sd,
                _ => throw new InvalidCastException($"Unexpected ToolOutput type: {toolOutputObj?.GetType().FullName}")
            };
            Assert.Equal(productName, toolOutput.ProductName);
            Assert.Equal(totalSales, toolOutput.TotalSales);
            Assert.Equal(unitsSold, toolOutput.UnitsSold);
            Assert.Equal(year, toolOutput.Year);
        }

        [Fact]
        public async Task MCPClient_TotalSales()
        {
            // Arrange
            var id = 1;
            var userInput = "What is the sales of xyz?";

            var toolName = "SalesTool";
            var productName = "xyz"; // expected product name
            var totalSales = 10000.50m; // expected total sales
            var unitsSold = 2000; // expected units sold
            int? year = null; // expected year

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
            var toolOutput = toolOutputObj switch
            {
                JsonElement je => je.Deserialize<SalesData>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!,
                SalesData sd => sd,
                _ => throw new InvalidCastException($"Unexpected ToolOutput type: {toolOutputObj?.GetType().FullName}")
            };
            Assert.Equal(productName, toolOutput.ProductName);
            Assert.Equal(totalSales, toolOutput.TotalSales);
            Assert.Equal(unitsSold, toolOutput.UnitsSold);
            Assert.Equal(year, toolOutput.Year);
        }

        [Fact]
        public async Task MCPClient_ProductInfo()
        {
            // Arrange
            var id = 1;
            var userInput = "Give me information about xyz.";

            var toolName = "ProductTool";
            var productName = "xyz"; // expected product name            
            var description = "A high-quality product."; // expected description
            var price = 29.99m; // expected price

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
            var toolOutput = toolOutputObj switch
            {
                JsonElement je => je.Deserialize<ProductData>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!,
                ProductData sd => sd,
                _ => throw new InvalidCastException($"Unexpected ToolOutput type: {toolOutputObj?.GetType().FullName}")
            };
            Assert.Equal(productName, toolOutput.ProductName);
            Assert.Equal(description, toolOutput.Description);
            Assert.Equal(price, toolOutput.Price);
        }
    }
}
