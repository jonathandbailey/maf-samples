using A2A;
using AGUI.StateSnapShotEvents;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Shared.Agents;
using Tools.ManualToolCall;

namespace Api;

public static class ApiExtensions
{

    public static async Task MapSamples(this WebApplication app)
    {
        var agentFactory = app.Services.GetRequiredService<IAgentFactory>();

        var agUiAgent = await agentFactory.CreateAgUiSnapShotAgent();

        app.MapAGUI(Routes.AGUISnapshotRoute, agUiAgent);

        var toolCallAgent = await agentFactory.CreateManualToolCallAgent();

        app.MapAGUI(Routes.ManualToolCallRoute, toolCallAgent);

        var a2AAgent = await agentFactory.CreateA2AAgent();

        app.MapAGUI(Routes.AGUIA2A, a2AAgent);
    }
}