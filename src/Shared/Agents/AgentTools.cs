using System.ComponentModel;
using Microsoft.Extensions.AI;

namespace Shared.Agents;

public static  class AgentTools
{
    private static readonly Dictionary<string, AIFunction> ToolMetas = new();
    
    [Description("Get the weather for a given location.")]
    private static string GetWeather([Description("The location to get the weather for.")] string location)
        => $"The weather in {location} is cloudy with a high of 15°C.";

    static AgentTools()
    {
        var function = AIFunctionFactory.Create(GetWeather);

        ToolMetas[function.Name] = function;
    }

    public static List<AITool> GetTools()
    {
        return ToolMetas.Select(toolMeta => toolMeta.Value.AsDeclarationOnly()).Cast<AITool>().ToList();
    }

    public static AIFunction Get(string name)
    {
        return ToolMetas[name];
    }
}
