using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Intellectus.AIAgent.Framework
{
    public interface ITool
    {
        string Name { get; }
        string Description { get; }
        Task<object> ExecuteAsync(params string[] input);
    }

    public class ConversationalAgentSettings
    {
        public string OpenAIAPIKey { get; set; } = string.Empty;
        public string OpenAILLMModel { get; set; } = "gpt-4o-mini";
        public string ReasoningResultContent { get; set; } = string.Empty;
        public List<ITool> Tools { get; set; } = new List<ITool>();
    }

    public class ConversationalAgentResponse
    {
        public string Response { get; set; } = string.Empty;
        public string ReasoningResult { get; set; } = string.Empty;
        public object? ToolOutput { get; set; } = null;
        public string Error { get; set; } = string.Empty;
    }

    public interface IConversationalAgent
    {
        Task<ConversationalAgentResponse> RespondAsync(string userInput);
    }

    public class ConversationalAgent : IConversationalAgent
    {
        private readonly List<ChatMessage> _history = new();
        private readonly List<ITool> _tools;
        private readonly ConversationalAgentSettings _settings;

        public ConversationalAgent(ConversationalAgentSettings settings)
        {
            _tools = settings.Tools;
            _settings = settings;

            var systemMessage = new SystemChatMessage(string.Format($@"
                    You are a helpful AI assistant with access to these tools:
                    {string.Join("\n", _tools.Select(t => $"{t.Name}: {t.Description}"))}
                    If a tool is needed, respond ONLY in the format:
                    TOOL: <ToolName>:{{0}}
                    If no tool is needed, respond with the final answer directly.
                    ", _settings.ReasoningResultContent));

            _history.Add(systemMessage);
        }

        public async Task<ConversationalAgentResponse> RespondAsync(string userInput)
        {
            _history.Add(new UserChatMessage(userInput));
            
            var chatClient = new ChatClient(_settings.OpenAILLMModel, _settings.OpenAIAPIKey);

            var result = await chatClient.CompleteChatAsync(_history);
            var reasoningResult = result.Value.Content[0].Text.Trim();

            if (reasoningResult.StartsWith("TOOL:"))
            {
                var parts = reasoningResult.Split(':');
                if (parts.Length >= 3)
                {
                    var toolName = parts[1].Trim();
                    var toolInputs = parts.Skip(2).ToArray();

                    var tool = _tools.FirstOrDefault(t => t.Name.Equals(toolName, StringComparison.OrdinalIgnoreCase));
                    if (tool != null)
                    {
                        var toolOutput = await tool.ExecuteAsync(toolInputs);

                        // Feed tool output back to the model
                        _history.Add(new AssistantChatMessage(reasoningResult));
                        _history.Add(new SystemChatMessage($"Tool '{toolName}' returned: {JsonSerializer.Serialize(toolOutput)}"));

                        var response = await RespondAsync("(continue)");

                        _history.Add(new SystemChatMessage($"Tool '{toolName}' returned: {response}"));

                        return new ConversationalAgentResponse
                        {
                            ReasoningResult = reasoningResult,
                            Response = response.ReasoningResult,
                            ToolOutput = toolOutput
                        };
                    }
                    return new ConversationalAgentResponse { Error = $"Error: Tool '{toolName}' not found." };
                }
                return new ConversationalAgentResponse { Error = "Error: Invalid tool response format." };
            }

            _history.Add(new AssistantChatMessage(reasoningResult));
            return new ConversationalAgentResponse { ReasoningResult = reasoningResult };
        }
    }
}