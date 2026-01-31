using A2A.Server.Settings;
using Microsoft.Extensions.Options;

namespace A2A.Server.Services;


public class A2ACardService : IA2ACardService
{
    private readonly List<AgentCard> _agentCards;

    public A2ACardService(IOptions<CardSettings> cardSettings)
    {
        _agentCards = cardSettings.Value.AgentCards;
    }

    public Task<AgentCard> GetAgentCard(string url)
    {
        var card = _agentCards.FirstOrDefault(ac => ac.Url == url);

        return Task.FromResult(card ?? throw new A2AException($"Card Not found with Url : {url} "));
    }
}

public interface IA2ACardService
{
    Task<AgentCard> GetAgentCard(string url);
}
