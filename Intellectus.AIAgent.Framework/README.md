# Intellectus AI Agent Framework for .NET

Library provides an `OpenAI Agent` for .NET applications. 

The agent is designed to facilitate communication between your application and `OpenAI's large language models (LLMs)`, 

enabling you to build intelligent conversational interfaces.

You can tell the Agent about your tools & the Agent can figure out which tool to use, given a natural language input.

## Step 1:

Create your tools by implementing the `ITool` interface. 

This interface defines the structure and behavior of your tools, allowing them to be seamlessly integrated into the agent framework.

The Agent can pass multiple inputs to the `ExecuteAsync` method of the tools, and the tools can return any object as output.

The multiple inputs are based on the `reasoning result` that you provide when creating the Agent instance.

### Interface

```csharp
public interface ITool
{
    string Name { get; }
    string Description { get; }
    Task<object> ExecuteAsync(params string[] input);
}
```

### Sample Tool Implementation

#### Product tool

The `ExecuteAsync` method of the ProductTool class takes a product name as input and returns product information.

```csharp
public class ProductData
{
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class ProductTool : ITool
{
    public string Name => "ProductTool";
    public string Description => "Provides product information for a given product. Input: product name.";

    public Task<object> ExecuteAsync(params string[] input)
    {
        return Task.FromResult((object)new ProductData
        {
            ProductName = input[0],
            Description = "A high-quality product.",
            Price = 29.99m
        });
    }
}
```

#### Sales tool

The `ExecuteAsync` method of the SalesTool class takes a product name and an optional year as input.

```csharp
public class SalesData
{
    public string ProductName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int UnitsSold { get; set; }
    public int? Year { get; set; } = null; // Optional, default to null if not provided
}

public class SalesTool : ITool
{
    public string Name => "SalesTool";
    public string Description => "Provides sales data for a given product. Input: product name, year.";        

    public Task<object> ExecuteAsync(params string[] input)
    {
        var productName = input[0].Trim();
        var year =
            input.Length > 1
            ?
            int.Parse(input[1].Trim())
            :
            0;

        return Task.FromResult((object) 
        (year == 0 
        ?
        //Total sales and units sold for the product without year
        new SalesData
        {
            ProductName = productName,
            TotalSales = 10000.50m,
            UnitsSold = 2000
        }            
        :
        //Total sales and units sold for the product for the specified year
        new SalesData
        {
            ProductName = productName,
            TotalSales = 500.50m,
            UnitsSold = 50,
            Year = year
        }));
    }
}
```

## Step 2:

Wire up the tools in your application and register them with the agent framework.

Add the settings with `OpenAI details (API Key, LLM)`, the `list of tools` and the `reasoning result`.

The tools can be dependency injected too.

The reasoning result is a string that describes the output format of the reasoning which is the expected input for the tools.

The agent will use this information to understand how to interact with the tools during conversations.

In the example, ProductName will be passed to Product tool and ProductName and/or Year (optional) to the Sales tool.

### Using Dependency Injection

Use extension `AddIntellectusAIAgentFramework` to wire up the framework for DI.

```csharp
using AgentFramework.Demo;
using Intellectus.AIAgent.Framework;
using Microsoft.Extensions.DependencyInjection;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable.");
    return;
}

// Create an Agent with the configured settings
var services = new ServiceCollection();

// Register tools with DI
services.AddScoped<ITool, SalesTool>();
services.AddScoped<ITool, ProductTool>();

// Register the framework and configure settings
services.AddIntellectusAIAgentFramework(settings =>
{
    settings.OpenAIAPIKey = apiKey;
    settings.OpenAILLMModel = "gpt-4o-mini";
    settings.ReasoningResult = @"<ProductName>:<Year>
                                    Year is optional.
                                ";
    //Add tools without using DI
    //settings.Tools = new List<ITool> { new SalesTool(), new ProductTool() };
});

var sp = services.BuildServiceProvider();

var agent = sp.GetRequiredService<IAgent>();

Console.WriteLine("Agent ready. Type 'exit' to quit.\n");

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
        break;

    var response = await agent.RespondAsync(input);
    Console.WriteLine($"Agent: {response.Response}\n");
}
```

### Without using Dependency Injection

Use the `AgentBuilder` to build the Agent.

```csharp
using AgentFramework.Demo;
using Intellectus.AIAgent.Framework;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

var agent = new AgentBuilder()
                .AddTool(new ProductTool())
                .AddTool(new SalesTool())
                .AddOpenAIAPIKey(apiKey)
                .AddOpenAILLM("gpt-4o-mini")
                .AddReasoningResult(@"<ProductName>:<Year>
                                        Year is optional.
                                     ")
                .ToAgent();

Console.WriteLine("Agent ready. Type 'exit' to quit.\n");

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
        break;

    var response = await agent.RespondAsync(input);
    Console.WriteLine($"Agent: {response.Response}\n");
}
```

## Agent Response

The `Agent` returns below `AgentResponse`.

The `ToolOutput` property contains the object returned by the Tool.

```csharp
public class AgentResponse
{
    public string Response { get; set; } = string.Empty;
    public string ReasoningResult { get; set; } = string.Empty;
    public object? ToolOutput { get; set; } = null;
    public string Error { get; set; } = string.Empty;
}
```