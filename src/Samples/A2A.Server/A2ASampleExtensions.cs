using A2A.Server.Settings;
using A2A.Server.Tasks;
using Microsoft.Extensions.Options;

namespace A2A.Server;

public static class A2ASampleExtensions
{
    private const string Weather = "Weather";
    private const string? WeatherAgentCardConfigurationNotFound = "Weather agent card configuration not found";

    public static void MapA2ATaskSample(this WebApplication app)
    {
        var workflowService = app.Services.GetRequiredService<IA2ATaskManager>();
        
        var cardSettings = app.Services.GetRequiredService<IOptions<CardSettings>>();
        
        var agentCard = cardSettings.Value.AgentCards.First(c => c.Name == Weather)
                              ?? throw new InvalidOperationException(WeatherAgentCardConfigurationNotFound);

        app.MapA2A(workflowService.TaskManager, $"{agentCard.Url}");
    }
}