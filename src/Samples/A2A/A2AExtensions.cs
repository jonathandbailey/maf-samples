using A2A.Tasks;
using Microsoft.Agents.AI;
using Shared.Agents;

namespace A2A;

public static  class A2AExtensions
{
    public static async Task<AIAgent> CreateA2AAgent(this IAgentFactory agentFactory)
    {
        var agent = await agentFactory.Create();

        return new AGUIAgent(agent);
    }
}