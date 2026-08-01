# .NET Client for Intellectus AI Agent MCP Server

|**Packages**|Version|Downloads|
|---------------------------|:---:|:---:|
|*Intellectus.AIAgent.Framework*|[![Nuget Version](https://img.shields.io/nuget/v/Intellectus.AIAgent.Framework)](https://www.nuget.org/packages/Intellectus.AIAgent.Framework)|[![Downloads count](https://img.shields.io/nuget/dt/Intellectus.AIAgent.Framework)](https://www.nuget.org/packages/Intellectus.AIAgent.Framework)|

The client library provides a simple way to communicate with an MCP Server using HTTP. 

It abstracts the complexities of making requests and handling responses, allowing developers to focus on building their applications.

Hook up your client in Program.cs as shown below:

```csharp
builder.Services.AddIntellectusMCPClient(settings =>
{
    settings.ServerBaseUrl = "http://localhost:5000";
});
```

Below Unit Test demonstrates a sample request to the MCP Server using the client library.

```csharp
[Fact]
public async Task MCPClient_SalesByYear()
{
    // Arrange
    var id = 1;
    var userInput = "What is the sales in 2026 of xyz?";
    
    var toolName = "SalesTool"; // expected tool name        
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
    var toolOutput = DeserializeToolOutput<SalesData>(toolOutputObj);
    Assert.Equal(productName, toolOutput.ProductName);
    Assert.Equal(totalSales, toolOutput.TotalSales);
    Assert.Equal(unitsSold, toolOutput.UnitsSold);
    Assert.Equal(year, toolOutput.Year);
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
```

[Sample MCP Server](https://github.com/VeritasSoftware/AgentFramework/tree/master/MCPServer.Sample)

[Client Tests](https://github.com/VeritasSoftware/AgentFramework/blob/master/AIAgentFrameworkTests/MCPClientTests.cs)