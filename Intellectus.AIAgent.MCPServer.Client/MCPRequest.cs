using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Intellectus.AIAgent.MCPServer.Client
{
    internal class MCPRequest
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; } = "tools/call";
        [JsonPropertyName("params")]
        public MCPRequestParameters Parameters { get; set; }

        public MCPRequest(int id, string userInput)
        {
            this.Id = id;

            this.Parameters = new MCPRequestParameters
            {
                Arguments = new Dictionary<string, string>() { { "userInput", userInput } }
            };
        }
    }

    internal class MCPRequestParameters
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "ai_agent_respond";
        [JsonPropertyName("arguments")]
        public Dictionary<string, string> Arguments { get; set; } = new Dictionary<string, string>();
    }
}
