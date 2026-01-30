using Microsoft.Agents.AI.A2A;

namespace A2A.Client.Services;

public class A2AAgentDiscoveryService : IA2AAgentDiscoveryService
{
    public async Task<A2AAgent> DiscoverAgentAsync(string url, string path)
    {
        var cardResolver = new A2ACardResolver(new Uri(url), new HttpClient(), agentCardPath: path);

        var card = await cardResolver.GetAgentCardAsync();

        var client = new A2AClient(new Uri(card.Url), new HttpClient());

        var agent = new A2AAgent(client, name: card.Name, description: card.Description);

        return agent;
    }
}

public interface IA2AAgentDiscoveryService
{
    Task<A2AAgent> DiscoverAgentAsync(string url, string path);
}