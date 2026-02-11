using System.ComponentModel;
using Microsoft.Extensions.AI;

namespace TDD;

public static class PlanningTools
{

    private static readonly Dictionary<string, AIFunction> Tools = new();
    [Description("Request all missing pieces of information from the user in a single batch.")]
    private static string RequestInformation(
        [Description("The information request containing the message, reasoning, and required inputs")] RequestInformationDto requestInformationDto)
        => $"The information requested is: {requestInformationDto.Message}";

    static PlanningTools()
    {
        var function = AIFunctionFactory.Create(RequestInformation);

        Tools[function.Name] = function;
    }

    public static List<AITool> GetDeclarationOnlyTools()
    {
        return Tools.Select(toolMeta => toolMeta.Value.AsDeclarationOnly()).Cast<AITool>().ToList();
    }
}