using System.Text.Json;
using TDD.Common.Dto;
using Xunit.Abstractions;

namespace TDD.Part03_DataDriven;

/// <summary>
/// Implements IXunitSerializable so xUnit can serialize/deserialize individual
/// scenario instances — enabling targeted re-runs of failing test cases from
/// the CLI (--filter) or the VS Test Explorer.
///
/// Note: IXunitSerializable requires a parameterless constructor and a mutable
/// class, so this is a class rather than a record.
/// </summary>
public class TravelPlanningScenario : IXunitSerializable
{
    public string Name { get; private set; } = string.Empty;
    public TravelPlanDto TravelPlan { get; private set; } = new TravelPlanDto();
    public List<string> ToolCalls { get; private set; } = [];

    // Parameterless constructor required by IXunitSerializable
    public TravelPlanningScenario() { }

    public TravelPlanningScenario(string name, TravelPlanDto travelPlan, List<string> toolCalls)
    {
        Name = name;
        TravelPlan = travelPlan;
        ToolCalls = toolCalls;
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(Name), Name);
        info.AddValue(nameof(TravelPlan), JsonSerializer.Serialize(TravelPlan));
        info.AddValue(nameof(ToolCalls), JsonSerializer.Serialize(ToolCalls));
    }

    public void Deserialize(IXunitSerializationInfo info)
    {
        Name = info.GetValue<string>(nameof(Name))!;
        TravelPlan = JsonSerializer.Deserialize<TravelPlanDto>(info.GetValue<string>(nameof(TravelPlan))!)!;
        ToolCalls = JsonSerializer.Deserialize<List<string>>(info.GetValue<string>(nameof(ToolCalls))!)!;
    }

    public override string ToString() => Name;
}
