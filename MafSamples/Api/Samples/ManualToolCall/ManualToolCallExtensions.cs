using Api.Common.Agents;
using Api.Common.Api;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;

namespace Api.Samples.ManualToolCall;

public static class ManualToolCallExtensions
{
    
    public static async Task MapManualToolCallExample(this WebApplication app)
    {
        var agentFactory = app.Services.GetRequiredService<IAgentFactory>();

        var agent = await agentFactory.Create(Tools.GetTools());

        app.MapAGUI(Routes.ManualToolCallRoute, new ManualToolCallAgent(agent));

    }

    public static void AddToolCalls(this Dictionary<string, FunctionCallContent> tools, IList<AIContent> contents)
    {
        foreach (var content in contents)
        {
            if (content is FunctionCallContent callContent)
            {
                tools[callContent.Name] = callContent;
            }
        }
    }
}