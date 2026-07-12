using AgentFramework.Demo;
using Intellectus.AIAgent.Framework;
using Microsoft.Extensions.DependencyInjection;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable.");
    return;
}

// Create a ConversationalAgent with the configured settings

var services = new ServiceCollection();

services.AddIntellectusAIAgentFramework(settings =>
{
    settings.OpenAIAPIKey = apiKey;
    settings.OpenAILLMModel = "gpt-4o-mini";
    settings.ReasoningResultContent = @"<ToolInput>:<Year>
                                        Year is optional.
                                       ";
    settings.Tools = new List<ITool> { new SalesTool(), new ProductTool() };
});

var sp = services.BuildServiceProvider();

var agent = sp.GetRequiredService<IConversationalAgent>();

Console.WriteLine("🤖 Agent ready. Type 'exit' to quit.\n");

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
        break;

    var response = await agent.RespondAsync(input);
    Console.WriteLine($"Agent: {response.Response}\n");
}
