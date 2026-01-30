namespace A2A.Server.Services;

public class A2ACardService : IA2ACardService
{
    private readonly List<AgentCard> _agentCards =
    [
        new AgentCard
        {
            Name = "Weather",
            Description = "An agent that provides weather information.",
            Url = "https://localhost:7251/api/a2a/weather",
            Skills =
            [
                new AgentSkill
                {
                    Name = "Get_Weather",
                    Description = "Get current weather information.",
                    InputModes = ["What is the weather like in Paris?"]
                }
            ],
            Version = "1.0"
        }
    ];

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
