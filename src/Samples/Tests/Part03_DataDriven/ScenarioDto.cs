using TDD.Common.Dto;

namespace TDD.Part03_DataDriven;

public record TravelPlanningScenario(
    string Name,
    TravelPlanDto TravelPlan,
    List<string> ToolCalls);