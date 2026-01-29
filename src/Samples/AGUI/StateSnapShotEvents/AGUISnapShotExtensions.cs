using Microsoft.Agents.AI;
using Shared.Agents;

namespace AGUI.StateSnapShotEvents;

public static  class AGUISnapShotExtensions
{
    public static async Task<AIAgent> CreateAgUiSnapShotAgent(this IAgentFactory agentFactory)
    {
        var agent = await agentFactory.Create();

        return new AGUIAgent(agent);
    }
}