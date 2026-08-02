import { describe, it, expect } from '@jest/globals';
import { MCPClient } from './mcp-client';

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

    it('Total Sales', async () => {
      let id = 1;
      let userInput = "What is the sales of xyz?";

      var toolName = "SalesTool"; // expected tool name
      var productName = "xyz"; // expected product name
      var totalSales = 10000.50; // expected total sales
      var unitsSold = 2000; // expected units sold
      let year = undefined; // expected year

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

    it('Product Info', async () => {
      let id = 1;
      let userInput = "Give me information about xyz.";

      var toolName = "ProductTool"; // expected tool name
      var productName = "xyz"; // expected product name
      var description = "A high-quality product."; // expected description
      var price = 29.99; // expected price

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
    });
});