using Api.Common.Agents;
using Api.Common.Api;
using Api.Samples.AgUiStateSnapShot;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;

namespace Api.Samples.AGUIStateSnapShot;

public static  class AGUISnapShotExtensions
{
    public static async Task MapAGUISnapShotExample(this WebApplication app)
    {
        var agentFactory = app.Services.GetRequiredService<IAgentFactory>();

        var agent = await agentFactory.Create();

        app.MapAGUI(Routes.AGUISnapshotRoute, new AGUIAgent(agent));
    }
}