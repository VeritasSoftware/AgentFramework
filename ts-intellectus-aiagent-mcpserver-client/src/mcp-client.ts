export interface IMCPClient {
  postAsync(id: number, userInput: string): Promise<MCPResponse>;
}

export class MCPClient implements IMCPClient {
  private baseUrl: string = "";

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  async postAsync(id: number, userInput: string): Promise<MCPResponse> {
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

      const serverResult: ServerResult = <ServerResult>JSON.parse(data);

      var str = serverResult?.result?.content[0]?.text;
      str = str?.replace("\\n", "\n").replace("\\\"", "\"");

      if (!str) {
        throw new Error("No text content found in the server result.");
      }

      const mcpResponse: MCPResponse = <MCPResponse>JSON.parse(str);

      return mcpResponse;
    }
    catch (error) {
      console.error("Error in postAsync:", error);
      throw error;
    }
  }
}

export class MCPRequest {
    jsonrpc: string = "2.0";
    id: number = 1;
    method: string = "tools/call";
    params: MCPRequestParameters = new MCPRequestParameters();

    constructor(id: number, userInput: string, name: string = "ai_agent_respond") {
        this.id = id;
        this.params.arguments["userInput"] = userInput;
        this.params.name = name;
    }
}

export class MCPRequestParameters {
    name: string  = "ai_agent_respond";
    arguments: Record<string, string> = {};
}

export class Content {
    type: string = "text";
    text: string = "";
}

export class Result {
    content: Content[] = [];
}

export class ServerResult {
    result?: Result;
    id: number = 1;
    jsonrpc: string = "2.0";    
}

export class MCPResponse {
    agentResponse?: AgentResponse;
    timestampUtc: Date = new Date();
}

export class AgentResponse {
    requestId: string = "";
    response: string = "";
    reasoningResult: string = "";
    toolName: string = "";
    toolOutput: any = null;
    error: string = "";
}