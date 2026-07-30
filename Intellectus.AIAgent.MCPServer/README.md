# Intellectus AI Agent MCP Server

|**Packages**|Version|Downloads|
|---------------------------|:---:|:---:|
|*Intellectus.AIAgent.Framework*|[![Nuget Version](https://img.shields.io/nuget/v/Intellectus.AIAgent.Framework)](https://www.nuget.org/packages/Intellectus.AIAgent.Framework)|[![Downloads count](https://img.shields.io/nuget/dt/Intellectus.AIAgent.Framework)](https://www.nuget.org/packages/Intellectus.AIAgent.Framework)|

## Overview

Library provides an `OpenAI Agent` for .NET applications. 

The agent is designed to facilitate communication between your application and `OpenAI's large language models (LLMs)`, 

enabling you to build intelligent conversational interfaces.

You can tell the Agent about your tools & the Agent can figure out which tool to use, given a natural language input.

## Model Context Protocol (MCP) Server

You can use the `Intellectus.AIAgent.MCPServer` package to build an MCP Server.

All your Tools reside in your Server. The Server communicates with OpenAI LLMs.

The Clients talk to the Server and get a response back.

Hook up your server as shown below:

```csharp
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

// Register the server and configure settings
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
```

[Sample MCP Server](https://github.com/VeritasSoftware/AgentFramework/tree/master/MCPServer.Sample)

[Sample Client](https://github.com/VeritasSoftware/AgentFramework/tree/master/MCPServer.Client.Sample)

[Tests](https://github.com/VeritasSoftware/AgentFramework/blob/master/AIAgentFrameworkTests/MCPServerTests.cs)