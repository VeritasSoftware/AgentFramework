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
    settings.ReasoningResult = @"<ToolInput>:<Year>
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

Console.WriteLine(Environment.NewLine);
Console.WriteLine("Running Agent on threads...");

var inputs = new List<(string input, string reqId)>()
{
    ("What is the sales in 2026 of xyz?", Guid.NewGuid().ToString()),
    ("What is the sales of xyz?", Guid.NewGuid().ToString()),
    ("Give me information about xyz.", Guid.NewGuid().ToString())
};

// General purpose event handler for all tools.
// You can add multiple.
agent.OnAgentResponse += async response =>
{
    Console.WriteLine($"OnAgentResponse: RequestId: {response.RequestId}, Tool Name: {response.ToolName}, Response: {response.Response}");
};

// Tool specific event handler.
// You can Add multiple for the same tool.
agent.OnAgentToolResponse.Add(new AgentToolResponseEvent
{
    ToolName = Constants.PRODUCT_TOOL_NAME,
    OnAgentResponse = HandleProductToolResponse
});

agent.OnAgentToolResponse.Add(new AgentToolResponseEvent
{
    ToolName = Constants.SALES_TOOL_NAME,
    Filter = response => ((SalesData)response.ToolOutput!).Year == null,
    OnAgentResponse = HandleSalesToolByTotalResponse
});

agent.OnAgentToolResponse.Add(new AgentToolResponseEvent
{
    ToolName = Constants.SALES_TOOL_NAME,
    Filter = response => ((SalesData)response.ToolOutput!).Year > 0,
    OnAgentResponse = HandleSalesToolByYearResponse
});

foreach (var input in inputs)
{
    Console.WriteLine(Environment.NewLine);
    Console.WriteLine(input);
    var response = await agent.RespondThreadAsync(input.input, input.reqId);
    Console.WriteLine($"Agent: RequestId: {response.RequestId}, Tool Name: {response.ToolName}, Response: {response.Response}");
    //OR
    //var response = await Task.Run(async () => await agent.RespondAsync(input.input, input.reqId));
}

Console.ReadLine();

async Task HandleProductToolResponse(AgentResponse response)
{
    Console.WriteLine($"{Constants.PRODUCT_TOOL_NAME} event: RequestId: {response.RequestId}, Response: {response.Response}");
}

async Task HandleSalesToolByTotalResponse(AgentResponse response)
{
    Console.WriteLine($"{Constants.SALES_TOOL_NAME} by Total event: RequestId: {response.RequestId}, Response: {response.Response}");
}

async Task HandleSalesToolByYearResponse(AgentResponse response)
{
    Console.WriteLine($"{Constants.SALES_TOOL_NAME} by Year event: RequestId: {response.RequestId}, Response: {response.Response}");
}