import OpenAI from "openai";
import dotenv from "dotenv";
import { DependencyContainer } from "tsyringe";

export interface ITool {
  name: string;
  description: string;
  parameters: object;
  executeAsync(args: Record<string, any>): Promise<any>;
}

export class ConversationalAgentSettings {
    openAIAPIKey: string = "";
    openAILLMModel: string = "gpt-4o-mini";
    reasoningResultContent: string ="";
    tools?: ITool[];
}

export class ConversationalAgentResponse {
    response: string = "";
    reasoningResult: string = "";
    toolOutput?: any = null;
    error: string = "";
}

export interface IConversationalAgent {
    respondAsync(userInput: string): Promise<ConversationalAgentResponse>;
}

export class ConversationalAgent implements IConversationalAgent {
    name: string;
    private tools: ITool[];
    private conversationHistory: any[] = [];
  
    constructor(
      name: string,
      private container: DependencyContainer,
      private systemPrompt: string
    ) {
      this.name = name;
      this.tools = this.container.resolveAll<ITool>('ITool');
    }
  
    private buildSystemPrompt(): string {
      const toolList = this.tools
        .map(
          t =>
            `Tool: ${t.name}\nDescription: ${t.description}\nParameters: ${JSON.stringify(
              t.parameters
            )}`
        )
        .join('\n\n');
  
      return `${this.systemPrompt}\n\nYou can call tools by responding ONLY in JSON format like:\n{"tool":"<toolName>","arguments":{...}}\n\nAvailable tools:\n${toolList}`;
    }
  
    async respondAsync(userInput: string): Promise<ConversationalAgentResponse> {
      const client = this.container.resolve(OpenAI);
      const model = this.container.resolve<string>('OPENAI_MODEL');
  
      if (this.conversationHistory.length === 0) {
        this.conversationHistory.push({ role: 'system', content: this.buildSystemPrompt() });
      }
  
      this.conversationHistory.push({ role: 'user', content: userInput });
  
      const response = await client.responses.create({
        model,
        input: this.conversationHistory,
      });
  
      const outputText = response.output_text || '';
      let reasoningLog = '';
  
      // Try to parse as JSON tool call
      try {
        const parsed = JSON.parse(outputText);
        if (parsed.tool && parsed.arguments) {
          reasoningLog += `TOOL: ${parsed.tool} ${JSON.stringify(parsed.arguments)}\n`;
  
          const tool = this.tools.find(t => t.name === parsed.tool);
          if (tool) {
            const toolResult = await tool.executeAsync(parsed.arguments);
            reasoningLog += `RESULT: ${toolResult}\n`;
  
            // Add tool result to conversation
            this.conversationHistory.push({
              role: 'assistant',
              content: `Tool ${parsed.tool} returned: ${toolResult}`,
            });
  
            // Get final answer from model
            const followUp = await client.responses.create({
              model,
              input: this.conversationHistory,
            });
  
            const finalText = followUp.output_text || '';
            reasoningLog += `ANSWER: ${finalText}`;
            this.conversationHistory.push({ role: 'assistant', content: finalText });

            var agentResponse = new ConversationalAgentResponse();
            agentResponse.response = finalText;
            agentResponse.reasoningResult = reasoningLog;
            agentResponse.toolOutput = toolResult;
            return agentResponse;
          }
        }
      } catch (e) {
        // Not a JSON tool call — just return answer
        reasoningLog += `ANSWER: ${outputText}`;
        this.conversationHistory.push({ role: 'assistant', content: outputText });
        var agentResponse = new ConversationalAgentResponse();
        agentResponse.reasoningResult = reasoningLog;
        agentResponse.error = `${e}`;
        return agentResponse;
      }
  
      var agentResponse = new ConversationalAgentResponse();
      agentResponse.reasoningResult = reasoningLog;
      return agentResponse;
    }
}
