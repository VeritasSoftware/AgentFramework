using AgentFramework.Core;
using AgentFramework.Demo;
using OpenAI.Chat;

var apiKey = "";//Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable.");
    return;
}

// Create a ChatClient for the chosen model
var chatClient = new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);

// Create a ConversationalAgent with the ChatClient and the tools and the reasoning result content
var agent = new ConversationalAgent(chatClient, new List<ITool> { new SalesTool(), new ProductTool() },
                                    @"<ToolInput>:<Year>
                                        Year is optional.
                                    ");

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
