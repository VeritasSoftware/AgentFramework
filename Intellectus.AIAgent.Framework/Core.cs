using Microsoft.Extensions.DependencyInjection;
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

    public class AgentSettings
    {
        public string OpenAIAPIKey { get; set; } = string.Empty;
        public string OpenAILLMModel { get; set; } = "gpt-4o-mini";
        public string ReasoningResult { get; set; } = string.Empty;
        public List<ITool>? Tools { get; set; } = null;
    }

    public class AgentResponse
    {
        public string Response { get; set; } = string.Empty;
        public string ReasoningResult { get; set; } = string.Empty;
        public object? ToolOutput { get; set; } = null;
        public string Error { get; set; } = string.Empty;
    }

    public interface IAgent
    {
        Task<AgentResponse> RespondAsync(string userInput);
    }

    public class Agent : IAgent
    {
        private readonly List<ChatMessage> _history = new();
        private List<ITool>? _tools;
        private readonly AgentSettings _settings;
        private readonly IServiceProvider? _serviceProvider;
        private ChatClient? _chatClient;

        [ActivatorUtilitiesConstructor]
        public Agent(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _settings = _serviceProvider.GetRequiredService<AgentSettings>();

            RefreshTools();

            ResetHistory();
        }

        public Agent(AgentSettings settings, ChatClient chatClient)
        {
            _settings = settings;
            _chatClient = chatClient;

            RefreshTools();

            ResetHistory();
        }

        public void ResetHistory()
        {
            _history.Clear();
            var systemMessage = new SystemChatMessage(string.Format($@"
                    You are a helpful AI assistant with access to these tools:
                    {string.Join("\n", _tools.Select(t => $"{t.Name}: {t.Description}"))}
                    If a tool is needed, respond ONLY in the format:
                    TOOL: <ToolName>:{{0}}
                    If no tool is needed, respond with the final answer directly.
                    ", _settings.ReasoningResult));
            _history.Add(systemMessage);
        }

        public List<ChatMessage> GetHistory()
        {
            return _history;
        }

        public void RefreshTools()
        {
            if (_settings.Tools != null && _settings.Tools.Any())
            {
                _tools = _settings.Tools;
            }
            else if (_serviceProvider != null)
            {
                // If no tools are provided, try to resolve them from the service provider
                _tools = _serviceProvider.GetServices<ITool>().ToList();
            }
        }

        public async Task<AgentResponse> RespondAsync(string userInput)
        {
            _history.Add(new UserChatMessage(userInput));
            
            if (_chatClient == null)
            {
                if (_serviceProvider != null)
                {
                    _chatClient = _serviceProvider.GetRequiredService<ChatClient>();
                }
                else
                {
                    throw new ApplicationException("ChatClient is null");
                }
            }

            var result = await _chatClient.CompleteChatAsync(_history);
            var reasoningResult = result.Value.Content[0].Text.Trim();

            if (reasoningResult.StartsWith("TOOL:"))
            {
                var parts = reasoningResult.Split(':');
                if (parts.Length >= 3)
                {
                    var toolName = parts[1].Trim();
                    var toolInputs = parts.Skip(2).Select(p => p.Trim()).ToArray();

                    RefreshTools(); // Ensure tools are up-to-date

                    var tool = _tools.FirstOrDefault(t => t.Name.Equals(toolName, StringComparison.OrdinalIgnoreCase));
                    if (tool != null)
                    {
                        var toolOutput = await tool.ExecuteAsync(toolInputs);

                        // Feed tool output back to the model
                        _history.Add(new AssistantChatMessage(reasoningResult));
                        _history.Add(new SystemChatMessage($"Tool '{toolName}' returned: {JsonSerializer.Serialize(toolOutput)}"));

                        var response = await RespondAsync("(continue)");

                        _history.Add(new SystemChatMessage($"Tool '{toolName}' returned: {response}"));

                        return new AgentResponse
                        {
                            ReasoningResult = reasoningResult,
                            Response = response.ReasoningResult,
                            ToolOutput = toolOutput
                        };
                    }
                    return new AgentResponse { Error = $"Error: Tool '{toolName}' not found." };
                }
                return new AgentResponse { Error = "Error: Invalid tool response format." };
            }

            _history.Add(new AssistantChatMessage(reasoningResult));
            return new AgentResponse { ReasoningResult = reasoningResult };
        }
    }
}