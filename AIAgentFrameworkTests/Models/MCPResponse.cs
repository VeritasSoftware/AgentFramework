namespace AIAgentFrameworkTests.Models
{
    public class MCPResponse
    {
        public AgentResponse? AgentResponse { get; set; }
        public DateTime? TimestampUtc { get; set; } = DateTime.UtcNow;
    }    
}
