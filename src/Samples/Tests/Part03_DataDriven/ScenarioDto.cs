using TDD.Common.Dto;
using Xunit.Abstractions;

namespace TDD.Part03_DataDriven;

public record TravelPlanningScenario(
    string Name,
    TravelPlanDto TravelPlan,
    List<string> ToolCalls) : IXunitSerializable
{
    public TravelPlanningScenario() : this(string.Empty, new TravelPlanDto(), [])
    {
    }

    public void Deserialize(IXunitSerializationInfo info)
    {
    }

    public void Serialize(IXunitSerializationInfo info)
    {
    }

    public override string ToString() => Name;
}