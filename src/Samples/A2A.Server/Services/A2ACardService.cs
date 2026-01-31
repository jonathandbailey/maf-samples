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

    private void UpdateRelativeUrl(CardSettings cardSettings)
    {
        foreach (var agentCard in _agentCards)
        {
            agentCard.Url = $"{cardSettings.BaseUrl}{agentCard.Url}";
        }
    }

    public Task<AgentCard> GetAgentCard(string url)
    {
        var path = new Uri(url).AbsolutePath;
        
        var card = _agentCards.FirstOrDefault(ac => ac.Url == path);

        return Task.FromResult(card ?? throw new A2AException($"Card Not found with Url : {url} "));

    }
}

public interface IA2ACardService
{
    Task<AgentCard> GetAgentCard(string url);
}
