using OpenAI.Chat;
using System.Text.Json;

namespace AgentFramework.Core
{
    public interface ITool
    {
        string Name { get; }
        string Description { get; }
        Task<object> ExecuteAsync(params string[] input);
    }

    public class ConversationalAgent
    {
        private readonly ChatClient _chatClient;
        private readonly List<ChatMessage> _history = new();
        private readonly List<ITool> _tools;

        public ConversationalAgent(ChatClient chatClient, List<ITool> tools, string reasoningResultContent)
        {
            _chatClient = chatClient;
            _tools = tools;            
                
            var systemMessage = new SystemChatMessage(string.Format($@"
                    You are a helpful AI assistant with access to these tools:
                    {string.Join("\n", _tools.Select(t => $"{t.Name}: {t.Description}"))}
                    If a tool is needed, respond ONLY in the format:
                    TOOL: <ToolName>:{{0}}
                    If no tool is needed, respond with the final answer directly.
                    ", reasoningResultContent));

            _history.Add(systemMessage);
        }

        public async Task<string> RespondAsync(string userInput)
        {
            _history.Add(new UserChatMessage(userInput));

            var result = await _chatClient.CompleteChatAsync(_history);
            var reasoningResult = result.Value.Content[0].Text.Trim();

            if (reasoningResult.StartsWith("TOOL:"))
            {
                var parts = reasoningResult.Split(':', 3);
                if (parts.Length == 3)
                {
                    var toolName = parts[1].Trim();
                    var toolInputs = parts.Skip(2).First().Split(":");

                    var tool = _tools.FirstOrDefault(t => t.Name.Equals(toolName, StringComparison.OrdinalIgnoreCase));
                    if (tool != null)
                    {
                        var toolOutput = await tool.ExecuteAsync(toolInputs);

                        // Feed tool output back to the model
                        _history.Add(new AssistantChatMessage(reasoningResult));
                        _history.Add(new SystemChatMessage($"Tool '{toolName}' returned: {JsonSerializer.Serialize(toolOutput)}"));

                        var response = await RespondAsync("(continue)");

                        _history.Add(new SystemChatMessage($"Tool '{toolName}' returned: {response}"));

                        return response;
                    }
                    return $"Error: Tool '{toolName}' not found.";
                }
                return "Error: Invalid tool response format.";
            }

            _history.Add(new AssistantChatMessage(reasoningResult));
            return reasoningResult;
        }
    }
}