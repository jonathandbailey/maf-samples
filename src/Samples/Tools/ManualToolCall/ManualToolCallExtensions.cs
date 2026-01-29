using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Shared.Agents;

namespace Tools.ManualToolCall;

public static class ManualToolCallExtensions
{
    public static async Task<AIAgent> CreateManualToolCallAgent(this IAgentFactory agentFactory)
    {
        var agent = await agentFactory.Create(AgentTools.GetDeclarationOnlyTools());

        return new ManualToolCallAgent(agent);
    }

    public static void AddToolCalls(this Dictionary<string, FunctionCallContent> tools, IList<AIContent> contents)
    {
        foreach (var content in contents)
        {
            if (content is FunctionCallContent callContent)
            {
                tools[callContent.Name] = callContent;
            }
        }
    }
}