import { describe, it, expect } from '@jest/globals';
import { MCPClient } from './mcp-client';

const globalAny: any = global;

describe('MCP Client', () => {
    beforeEach(() => {
        // Reset mock before each test
        globalAny.fetch = jest.fn();
    });

    it('Sales by Year', async () => {
      let id = 1;
      let userInput = "What is the sales in 2026 of xyz?";

      var toolName = "SalesTool"; // expected tool name
      var productName = "xyz"; // expected product name
      var totalSales = 500.50; // expected total sales
      var unitsSold = 50; // expected units sold
      var year = 2026; // expected year

      // Mock a successful fetch response
      globalAny.fetch.mockResolvedValue({
          ok: true,
          text: async () => (`{"result":{"content":[{"type":"text","text":"{\\\u0022agentResponse\\\u0022:{\\\u0022requestId\\\u0022:\\\u0022\\\u0022,\\\u0022response\\\u0022:\\\u0022The sales of the product \\\\u0022xyz\\\\u0022 in 2026 were $500.50, with a total of 50 units sold.\\\u0022,\\\u0022reasoningResult\\\u0022:\\\u0022TOOL: SalesTool:xyz:2026\\\u0022,\\\u0022toolName\\\u0022:\\\u0022SalesTool\\\u0022,\\\u0022toolOutput\\\u0022:{\\\u0022productName\\\u0022:\\\u0022xyz\\\u0022,\\\u0022totalSales\\\u0022:500.50,\\\u0022unitsSold\\\u0022:50,\\\u0022year\\\u0022:2026},\\\u0022error\\\u0022:\\\u0022\\\u0022},\\\u0022timestampUtc\\\u0022:\\\u00222026-08-14T23:30:39.9651669Z\\\u0022}"}]},"id":1,"jsonrpc":"2.0"}`)
      });

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

      expect(globalAny.fetch).toHaveBeenCalledWith("http://localhost:5000/mcp", {"body": "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/call\",\"params\":{\"name\":\"ai_agent_respond\",\"arguments\":{\"userInput\":\"What is the sales in 2026 of xyz?\"}}}", "headers": {"Accept": "application/json, text/event-stream", "Content-Type": "application/json"}, "method": "POST"});
    });

    it('Total Sales', async () => {
      let id = 1;
      let userInput = "What is the sales of xyz?";

      var toolName = "SalesTool"; // expected tool name
      var productName = "xyz"; // expected product name
      var totalSales = 10000.50; // expected total sales
      var unitsSold = 2000; // expected units sold
      let year = undefined; // expected year

      // Mock a successful fetch response
      globalAny.fetch.mockResolvedValue({
          ok: true,
          text: async () => (`{"result":{"content":[{"type":"text","text":"{\\\u0022agentResponse\\\u0022:{\\\u0022requestId\\\u0022:\\\u0022\\\u0022,\\\u0022response\\\u0022:\\\u0022The total sales of the product \\\\u0022xyz\\\\u0022 are $10,000.50, with 2,000 units sold.\\\u0022,\\\u0022reasoningResult\\\u0022:\\\u0022TOOL: SalesTool:xyz\\\u0022,\\\u0022toolName\\\u0022:\\\u0022SalesTool\\\u0022,\\\u0022toolOutput\\\u0022:{\\\u0022productName\\\u0022:\\\u0022xyz\\\u0022,\\\u0022totalSales\\\u0022:10000.50,\\\u0022unitsSold\\\u0022:2000},\\\u0022error\\\u0022:\\\u0022\\\u0022},\\\u0022timestampUtc\\\u0022:\\\u00222026-08-15T01:04:20.7745311Z\\\u0022}"}]},"id":1,"jsonrpc":"2.0"}`)
      });

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

      expect(globalAny.fetch).toHaveBeenCalledWith("http://localhost:5000/mcp", {"body": "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/call\",\"params\":{\"name\":\"ai_agent_respond\",\"arguments\":{\"userInput\":\"What is the sales of xyz?\"}}}", "headers": {"Accept": "application/json, text/event-stream", "Content-Type": "application/json"}, "method": "POST"});
    });

    it('Product Info', async () => {
      let id = 1;
      let userInput = "Give me information about xyz.";

      var toolName = "ProductTool"; // expected tool name
      var productName = "xyz"; // expected product name
      var description = "A high-quality product."; // expected description
      var price = 29.99; // expected price

      // Mock a successful fetch response
      globalAny.fetch.mockResolvedValue({
          ok: true,
          text: async () => (`{"result":{"content":[{"type":"text","text":"{\\\u0022agentResponse\\\u0022:{\\\u0022requestId\\\u0022:\\\u0022\\\u0022,\\\u0022response\\\u0022:\\\u0022The product \\\\u0022xyz\\\\u0022 is described as a high-quality product, and it is priced at $29.99.\\\u0022,\\\u0022reasoningResult\\\u0022:\\\u0022TOOL: ProductTool:xyz\\\u0022,\\\u0022toolName\\\u0022:\\\u0022ProductTool\\\u0022,\\\u0022toolOutput\\\u0022:{\\\u0022productName\\\u0022:\\\u0022xyz\\\u0022,\\\u0022description\\\u0022:\\\u0022A high-quality product.\\\u0022,\\\u0022price\\\u0022:29.99},\\\u0022error\\\u0022:\\\u0022\\\u0022},\\\u0022timestampUtc\\\u0022:\\\u00222026-08-15T01:12:26.5382027Z\\\u0022}"}]},"id":1,"jsonrpc":"2.0"}`)
      });

      const client = new MCPClient(`http://localhost:5000`);

      var response = await client.postAsync(id, userInput);

      console.log("Response from MCP Client:", response);
      
      expect(response).not.toBeNull();
      expect(response.agentResponse).not.toBeNull();
      expect(response.agentResponse?.toolName).toBe(toolName);
      expect(response.agentResponse?.toolOutput).not.toBeNull();
      expect(response.agentResponse?.toolOutput.productName).toBe(productName);
      expect(response.agentResponse?.toolOutput.description).toBe(description);
      expect(response.agentResponse?.toolOutput.price).toBe(price);

      expect(globalAny.fetch).toHaveBeenCalledWith("http://localhost:5000/mcp", {"body": "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/call\",\"params\":{\"name\":\"ai_agent_respond\",\"arguments\":{\"userInput\":\"Give me information about xyz.\"}}}", "headers": {"Accept": "application/json, text/event-stream", "Content-Type": "application/json"}, "method": "POST"});
    });
});