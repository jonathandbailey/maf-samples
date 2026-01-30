using A2A.Client.Services;
using A2A.Client.Tasks;
using Microsoft.Agents.AI;
using Shared.Agents;

namespace A2A.Client;

public static  class A2AExtensions
{
    public static async Task<AIAgent> CreateA2AAgentTask(this IAgentFactory agentFactory,
        IA2AAgentDiscoveryService a2ADiscoveryService)
    {

        var a2AAgent = await a2ADiscoveryService.DiscoverAgentAsync("https://localhost:7251/", "/api/a2a/weather/v1/card");
        
        var agent = await agentFactory.Create();

        return new AGUIAgent(agent);
    }
}