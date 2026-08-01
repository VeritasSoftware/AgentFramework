using Intellectus.AIAgent.Framework;
using System.Text.Json.Serialization;

namespace Intellectus.AIAgent.MCPServer
{
    public class MCPResponse
    {
        public AgentResponse? AgentResponse { get; set; }
        public DateTime? TimestampUtc { get; set; } = DateTime.UtcNow;
    }    
}
