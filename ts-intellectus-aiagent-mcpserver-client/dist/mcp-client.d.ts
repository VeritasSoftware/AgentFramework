export interface IMCPClient {
    postAsync(id: number, userInput: string): Promise<MCPResponse>;
}
export declare class MCPClient implements IMCPClient {
    private baseUrl;
    constructor(baseUrl: string);
    postAsync(id: number, userInput: string): Promise<MCPResponse>;
}
export declare class MCPRequest {
    jsonrpc: string;
    id: number;
    method: string;
    params: MCPRequestParameters;
    constructor(id: number, userInput: string, name?: string);
}
export declare class MCPRequestParameters {
    name: string;
    arguments: Record<string, string>;
}
export declare class Content {
    type: string;
    text: string;
}
export declare class Result {
    content: Content[];
}
export declare class ServerResult {
    result?: Result;
    id: number;
    jsonrpc: string;
}
export declare class MCPResponse {
    agentResponse?: AgentResponse;
    timestampUtc: Date;
}
export declare class AgentResponse {
    requestId: string;
    response: string;
    reasoningResult: string;
    toolName: string;
    toolOutput: any;
    error: string;
}
//# sourceMappingURL=mcp-client.d.ts.map