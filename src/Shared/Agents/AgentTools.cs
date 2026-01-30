using System.ComponentModel;
using A2A;
using Microsoft.Agents.AI.A2A;
using Microsoft.Extensions.AI;
using Shared.Extensions;

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

    public static List<AITool> GetDeclarationOnlyTools()
    {
        return ToolMetas.Select(toolMeta => toolMeta.Value.AsDeclarationOnly()).Cast<AITool>().ToList();
    }

    public static List<AITool> GetTools()
    {
        return ToolMetas.Select(toolMeta => toolMeta.Value).Cast<AITool>().ToList();
    }



    public static AIFunction Get(string name)
    {
        return ToolMetas[name];
    }

    public static List<AIFunction> CreateToolsFromA2ACard(A2AAgent a2AAgent, AgentCard agentCard)
    {
        return agentCard.Skills.Select(skill => AIFunctionFactory.Create(RunAgentAsync, Create(skill))).ToList();

        async Task<string> RunAgentAsync(string message, CancellationToken cancellationToken)
        {
            var response = await a2AAgent.RunAsync(message, cancellationToken: cancellationToken);

            if (response.RawRepresentation is AgentTask agentTask)
            {
                return agentTask.ExtractTextPartsFromMessage();
            }

            return "Unable to extract message from response.";
        }
    }

    private static AIFunctionFactoryOptions Create(AgentSkill skill)
    {
        var additionalProperties = new Dictionary<string, object?>
        {
            ["tags"] = skill.Tags,
            ["examples"] = skill.Examples ?? [],
            ["inputModes"] = skill.InputModes ?? [],
            ["outputModes"] = skill.OutputModes ?? []
        };

        return new AIFunctionFactoryOptions
        {
            Name = skill.Name,
            Description = skill.Description,
            AdditionalProperties = additionalProperties
        };
    }
}
