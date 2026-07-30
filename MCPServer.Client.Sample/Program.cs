using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.Text.Json;

Console.WriteLine("Client for Intellectus AI Agent MCP Server...");
Console.WriteLine(Environment.NewLine);

var transport = new StdioClientTransport(new()
{
    Command = "dotnet run",
    Arguments = ["--project", @"..\..\..\..\MCPServer.Sample"],
    Name = "Intellectus AI Agent MCP Server",
});
McpClient mcpClient = await McpClient.CreateAsync(transport);

// List all available tools from the MCP server.
Console.WriteLine("Available tools:");
Console.WriteLine(Environment.NewLine);
IList<McpClientTool> tools = await mcpClient.ListToolsAsync();
foreach (McpClientTool tool in tools)
{
    Console.WriteLine($"{tool}: {tool.JsonSchema}");
}

// User input
await CallToolAsync(mcpClient, "What is the sales in 2026 of xyz?");

await CallToolAsync(mcpClient, "What is the sales of xyz?");

await CallToolAsync(mcpClient, "Give me information about xyz.");

await mcpClient.DisposeAsync();

Console.ReadLine();

static async Task CallToolAsync(McpClient mcpClient, string input)
{
    var param = new CallToolRequestParams
    {
        Name = "ai_agent_respond",
        Arguments = new Dictionary<string, JsonElement>
        {
            { "userInput", JsonSerializer.SerializeToElement(input) }
        }
    };

    var result = await mcpClient.CallToolAsync(param);

    Console.WriteLine(Environment.NewLine);
    Console.WriteLine($"Tool userInput: {input}");
    Console.WriteLine($"Tool result: {result.Content[0]}");
}