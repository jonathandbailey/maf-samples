using A2A.Client;
using A2A.Client.Services;
using AGUI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Shared.Agents;
using Tools.ManualToolCall;

namespace Api;

public static class ApiExtensions
{

    public static async Task MapSamples(this WebApplication app)
    {
        var agentFactory = app.Services.GetRequiredService<IAgentFactory>();

        var a2ADiscoveryService = app.Services.GetRequiredService<IA2AAgentDiscoveryService>();

        var agUiAgent = await agentFactory.CreateAgUiSnapShotAgent();

        app.MapAGUI(Routes.AGUISnapshotRoute, agUiAgent);

        var toolCallAgent = await agentFactory.CreateManualToolCallAgent();

        app.MapAGUI(Routes.ManualToolCallRoute, toolCallAgent);

        var a2AAgent = await agentFactory.CreateA2AAgentTask(a2ADiscoveryService);

        app.MapAGUI(Routes.AGUIA2A, a2AAgent);
    }
}