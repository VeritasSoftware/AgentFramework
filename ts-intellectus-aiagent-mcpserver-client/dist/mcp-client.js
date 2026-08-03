"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AgentResponse = exports.MCPResponse = exports.ServerResult = exports.Result = exports.Content = exports.MCPRequestParameters = exports.MCPRequest = exports.MCPClient = void 0;
class MCPClient {
    baseUrl = "";
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
    }
    async postAsync(id, userInput) {
        const url = `${this.baseUrl}/mcp`;
        const request = new MCPRequest(id, userInput);
        try {
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json, text/event-stream"
                },
                body: JSON.stringify(request),
            });
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            const responseData = await response.text();
            if (!responseData) {
                throw new Error("No response data received from the server.");
            }
            console.log("Raw response data:", responseData);
            var r = new RegExp(`.*?(?<data>\{.*\})`);
            var m = r.exec(responseData);
            if (m === null || !m.groups || !m.groups["data"]) {
                throw new Error("Failed to extract JSON data from the response.");
            }
            console.log(m.groups?.data);
            var data = m.groups?.data || "";
            console.log("Extracted JSON data:", data);
            const serverResult = JSON.parse(data);
            var str = serverResult?.result?.content[0]?.text;
            str = str?.replace("\\n", "\n").replace("\\\"", "\"");
            if (!str) {
                throw new Error("No text content found in the server result.");
            }
            const mcpResponse = JSON.parse(str);
            return mcpResponse;
        }
        catch (error) {
            console.error("Error in postAsync:", error);
            throw error;
        }
    }
}
exports.MCPClient = MCPClient;
class MCPRequest {
    jsonrpc = "2.0";
    id = 1;
    method = "tools/call";
    params = new MCPRequestParameters();
    constructor(id, userInput, name = "ai_agent_respond") {
        this.id = id;
        this.params.arguments["userInput"] = userInput;
        this.params.name = name;
    }
}
exports.MCPRequest = MCPRequest;
class MCPRequestParameters {
    name = "ai_agent_respond";
    arguments = {};
}
exports.MCPRequestParameters = MCPRequestParameters;
class Content {
    type = "text";
    text = "";
}
exports.Content = Content;
class Result {
    content = [];
}
exports.Result = Result;
class ServerResult {
    result;
    id = 1;
    jsonrpc = "2.0";
}
exports.ServerResult = ServerResult;
class MCPResponse {
    agentResponse;
    timestampUtc = new Date();
}
exports.MCPResponse = MCPResponse;
class AgentResponse {
    requestId = "";
    response = "";
    reasoningResult = "";
    toolName = "";
    toolOutput = null;
    error = "";
}
exports.AgentResponse = AgentResponse;
//# sourceMappingURL=mcp-client.js.map