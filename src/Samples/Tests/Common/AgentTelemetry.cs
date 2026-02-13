using System.Diagnostics;

namespace TDD.Common;

public static class AgentTelemetry
{
    private const string Name = "Planning";
    
    private static readonly ActivitySource Source = new ActivitySource("TDD.Agent", "1.0.0");

    public static Activity? Start(string input)
    {
        var tags = new ActivityTagsCollection
        {
            { "gen_ai.agent.name", Name },
            { "gen_ai.prompt", input },
        };

        var source = Source.StartActivity($"invoke_agent {Name}", ActivityKind.Internal, null, tags);
     
        return source;
    }

    public static Activity ToolCall(this Activity activity, string key, string arguments, Activity? parent)
    {
        var tags = new ActivityTagsCollection
        {
            { "gen_ai.tool.name", key },
            { "gen_ai.tool.parameters", arguments }
        };

        activity.SetTag("gen_ai.tool.name", key);
        activity.SetTag("gen_ai.tool.parameters", arguments);

        return activity;
    }

    
}