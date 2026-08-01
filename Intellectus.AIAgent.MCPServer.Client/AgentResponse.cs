using System.Text.Json.Serialization;

namespace Intellectus.AIAgent.MCPServer.Client
{
    public class AgentResponse
    {
        [JsonPropertyName("requestId")]
        public string RequestId { get; set; } = string.Empty;
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;
        [JsonPropertyName("reasoningResult")]
        public string ReasoningResult { get; set; } = string.Empty;
        [JsonPropertyName("toolName")]
        public string ToolName { get; set; } = string.Empty;
        [JsonPropertyName("toolOutput")]
        public object? ToolOutput { get; set; }
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;
    }
}
