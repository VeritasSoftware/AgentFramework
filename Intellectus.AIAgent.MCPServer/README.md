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

The Clients talk to the Server using HTTP/stdio and get a response back.

## Sample Server

Create a Console project.

In the .csproj,

change the SDK to Web as shown below:

```
<Project Sdk="Microsoft.NET.Sdk.Web">
```

and add below properties in the PropertyGroup.

```
<!-- Set up the MCP server to be a self-contained application that does not rely on a shared framework -->
<SelfContained>true</SelfContained>
<PublishSelfContained>true</PublishSelfContained>

<!-- Set up the MCP server to be a single file executable -->
<PublishSingleFile>true</PublishSingleFile>
```

Then, hook up your server in Program.cs as shown below:

```csharp
using Intellectus.AIAgent.Framework;
using Intellectus.AIAgent.MCPServer;
using MCPServer.Sample;

Console.WriteLine("Intellectus AI Agent MCP Server...");

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.UseIntellectusAIAgentMCPServer();

await app.RunAsync();
```

To call the MCP Server using HTTP, use a JSON request like below:

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "tools/call",
  "params": {
    "name": "ai_agent_respond",
    "arguments": {
      "userInput": "<--Your input here-->"
    }
  }
}
```

[Sample MCP Server](https://github.com/VeritasSoftware/AgentFramework/tree/master/MCPServer.Sample)

[Sample Client](https://github.com/VeritasSoftware/AgentFramework/tree/master/MCPServer.Client.Sample)

[Tests](https://github.com/VeritasSoftware/AgentFramework/blob/master/AIAgentFrameworkTests/MCPServerTests.cs)