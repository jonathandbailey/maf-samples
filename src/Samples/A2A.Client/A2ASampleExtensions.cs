using A2A.Client.Services;
using A2A.Client.Settings;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Options;
using Shared.Agents;

namespace A2A.Client;

public static  class A2ASampleExtensions
{
    public static async Task<AIAgent> CreateA2AAgentTask(this IAgentFactory agentFactory,
        IA2AAgentDiscoveryService a2ADiscoveryService,
        IOptions<A2ADiscoverySettings> discoverySettings)
    {
        var settings = discoverySettings.Value;
        var a2AAgent = await a2ADiscoveryService.DiscoverAgentAsync(settings.Url, settings.Path);

        var tools = AgentTools.CreateToolsFromA2ACard(a2AAgent.Agent, a2AAgent.Card);

        var agent = await agentFactory.Create([..tools]);

        return agent;
    }
}