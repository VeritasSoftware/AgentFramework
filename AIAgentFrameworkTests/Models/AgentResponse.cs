namespace AIAgentFrameworkTests.Models
{
    public class AgentResponse
    {
        public string RequestId { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string ReasoningResult { get; set; } = string.Empty;
        public string ToolName { get; set; } = string.Empty;
        public object? ToolOutput { get; set; } = null;
        public string Error { get; set; } = string.Empty;
    }
}
