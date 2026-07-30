using Intellectus.AIAgent.Framework;
using Intellectus.AIAgent.MCPServer;
using MCPServer.Sample;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Intellectus AI Agent MCP Server...");

var builder = Host.CreateApplicationBuilder(args);

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

// Register tools with DI
builder.Services.AddScoped<ITool, SalesTool>();
builder.Services.AddScoped<ITool, ProductTool>();

// Register the framework and configure settings
builder.Services.AddIntellectusAIAgentMCPServer(settings =>
{
    settings.OpenAIAPIKey = apiKey;
    settings.OpenAILLMModel = "gpt-4o-mini";
    settings.ReasoningResult = @"<ProductName>:<Year>
                                    Year is optional.
                                ";
    //Add tools without using DI
    //settings.Tools = new List<ITool> { new SalesTool(), new ProductTool() };
});

await builder.Build().RunAsync();