using Api.Common.Api;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Shared.Agents;

namespace Samples.AGUIStateSnapShot;

public static  class AGUISnapShotExtensions
{
    public static async Task MapAGUISnapShotExample(this WebApplication app)
    {
        var agentFactory = app.Services.GetRequiredService<IAgentFactory>();

        var agent = await agentFactory.Create();

        app.MapAGUI(Routes.AGUISnapshotRoute, new AGUIAgent(agent));
    }
}