using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Intellectus.AIAgent.MCPServer.Client
{
    public class MCPResponse
    {
        [JsonPropertyName("agentResponse")]
        public AgentResponse? AgentResponse { get; set; }
        [JsonPropertyName("timestampUtc")]
        public DateTime? TimestampUtc { get; set; } = DateTime.UtcNow;
    }

    public class Content
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("content")]
        public List<Content> Content { get; set; }
    }

    public class ServerResult
    {
        [JsonPropertyName("result")]
        public Result Result { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; }
    }
}
