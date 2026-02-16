using System.Diagnostics;
using System.Text.Json;

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

    public static Activity? ToolCall(string key, object? arguments, Activity? parent)
    {
        var tags = new ActivityTagsCollection
        {
            { "gen_ai.tool.name", key },
        };

        var jsonArgs = JsonSerializer.Serialize(arguments);

        var inputEvent = new ActivityEvent("ToolInput", tags: new ActivityTagsCollection
        {
            { "arguments", jsonArgs }
        });

        var source = Source.StartActivity($"execute_tool {key}", ActivityKind.Internal, parent?.Id, tags);

        source?.AddEvent(inputEvent);

        return source;
    }



}