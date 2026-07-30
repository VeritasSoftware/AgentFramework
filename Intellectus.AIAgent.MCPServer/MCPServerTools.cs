using Intellectus.AIAgent.Framework;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Intellectus.AIAgent.MCPServer
{
    [McpServerToolType]
    public class MCPServerTools
    {
        [McpServerTool, Description("AI Agent Respond.")]
        public static async Task<AgentResponse> AIAgentRespond([Description("The user input")] string userInput,                                                              
                                                                IAgent agent,
                                                                ILogger logger = null)
        {
            logger?.LogInformation($"User input: {userInput}");

            var response = await agent.RespondAsync(userInput);

            return response;
        }
    }
}
