# TypeScript Client for Intellectus AI Agent MCP Server

|**Packages**|Version|Downloads|
|---------------------------|:---:|:---:|
|*Intellectus.AIAgent.Framework*|[![Nuget Version](https://img.shields.io/nuget/v/Intellectus.AIAgent.Framework)](https://www.nuget.org/packages/Intellectus.AIAgent.Framework)|[![Downloads count](https://img.shields.io/nuget/dt/Intellectus.AIAgent.Framework)](https://www.nuget.org/packages/Intellectus.AIAgent.Framework)|

The client library provides a simple way to communicate with an MCP Server using HTTP. 

It abstracts the complexities of making requests and handling responses, allowing developers to focus on building their applications.

Below Unit Test demonstrates a sample request to the MCP Server using the client library.

```typescript
import { MCPClient} from 'ts-intellectus-aiagent-mcpserver-client'
```

```typescript
describe('MCP Client', () => {
    it('Sales by Year', async () => {
      let id = 1;
      let userInput = "What is the sales in 2026 of xyz?";

      var toolName = "SalesTool"; // expected tool name
      var productName = "xyz"; // expected product name
      var totalSales = 500.50; // expected total sales
      var unitsSold = 50; // expected units sold
      var year = 2026; // expected year

      const client = new MCPClient(`http://localhost:5000`);

      var response = await client.postAsync(id, userInput);

      console.log("Response from MCP Client:", response);
      
      expect(response).not.toBeNull();
      expect(response.agentResponse).not.toBeNull();
      expect(response.agentResponse?.toolName).toBe(toolName);
      expect(response.agentResponse?.toolOutput).not.toBeNull();
      expect(response.agentResponse?.toolOutput.productName).toBe(productName);
      expect(response.agentResponse?.toolOutput.totalSales).toBe(totalSales);
      expect(response.agentResponse?.toolOutput.unitsSold).toBe(unitsSold);
      expect(response.agentResponse?.toolOutput.year).toBe(year);
    });
});
```

[Sample MCP Server](https://github.com/VeritasSoftware/AgentFramework/tree/master/MCPServer.Sample)

[Client Tests](https://github.com/VeritasSoftware/AgentFramework/tree/master/ts-intellectus-aiagent-mcpserver-client/src/mcp-client.test.ts)