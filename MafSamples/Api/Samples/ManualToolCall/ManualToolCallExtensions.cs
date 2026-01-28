using Api.Common.Agents;
using Api.Common.Api;
using Api.Samples.AgUiStateSnapShot;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;

namespace Api.Samples.ManualToolCall;

public static class ManualToolCallExtensions
{
    public static async Task MapManualToolCallExample(this WebApplication app)
    {
        var agentFactory = app.Services.GetRequiredService<IAgentFactory>();

        var agent = await agentFactory.Create();

        app.MapAGUI(Routes.ManualToolCallRoute, new AGUIAgent(agent));
    }
}