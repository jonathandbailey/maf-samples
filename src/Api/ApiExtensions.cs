using A2A.Client;
using A2A.Client.Services;
using A2A.Client.Settings;
using AGUI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.Options;
using Shared.Agents;
using Tools.ManualToolCall;

namespace Api;

public static class ApiExtensions
{

    public static async Task MapSamples(this WebApplication app)
    {
        var agentFactory = app.Services.GetRequiredService<IAgentFactory>();

        var a2ADiscoveryService = app.Services.GetRequiredService<IA2AAgentDiscoveryService>();

        var discoverySettings = app.Services.GetRequiredService<IOptions<A2ADiscoverySettings>>();

        var agUiAgent = await agentFactory.CreateAgUiSnapShotAgent();

        app.MapAGUI(Routes.AGUISnapshotRoute, agUiAgent);

        var toolCallAgent = await agentFactory.CreateManualToolCallAgent();

        app.MapAGUI(Routes.ManualToolCallRoute, toolCallAgent);

        var a2AAgent = await agentFactory.CreateA2AAgentTask(a2ADiscoveryService, discoverySettings);

        app.MapAGUI(Routes.AGUIA2A, a2AAgent);
    }
}