using FluentAssertions;
using TDD.Common;
using TDD.Common.Dto;
using TDD.Common.Helpers;

namespace TDD.Part03_DataDriven;

public class DataDrivenAgentTests : IDisposable
{
    private const string Destination = "Paris";
    private const int NumberOfTravelers = 2;
    private static readonly DateTime DepartureDate = new(2026, 5, 1);

    private const string RequestInformationToolName = "RequestInformation";
    private const string ToolCallArgumentKey = "requestInformationDto";

    private readonly List<string> _expectedKeys = ["Origin", "ReturnDate"];

    private readonly TravelPlanDto _travePlanState = new(Destination: Destination, DepartureDate: DepartureDate, NumberOfTravelers: NumberOfTravelers);

    public static IEnumerable<object[]> TravelPlanningScenarios()
    {
        var scenarios = ScenarioLoader.LoadPlanningWorkflowScenarios();
        foreach (var scenario in scenarios)
        {
            yield return [scenario];
        }
    }


    public DataDrivenAgentTests()
    {
        TelemetryHelper.Initialize(SettingsHelper.GetAspireDashboardSettings());
    }

    public void Dispose()
    {
        TelemetryHelper.Dispose();
    }

    [Theory]
    [MemberData(nameof(TravelPlanningScenarios))]
    public async Task PlanningAgent_ShouldRequestMissingInformationToolCall_WhenTravelPlanIsIncomplete(TravelPlanningScenario scenario)
    {
        var agent = await AgentFactoryHelper.CreateMockPlanningAgent();

        var chatMessage = TravelPlanHelper.CreateTravelPlanMessage(scenario.TravelPlan);

        var activity = AgentTelemetry.Start(chatMessage.Text);

        var response = await agent.RunAsync(chatMessage);

        foreach (var functionCallContent in response.FunctionCalls())
        {
            using var toolActivity = AgentTelemetry.ToolCall(functionCallContent.Name, functionCallContent.Arguments, activity);
        }

        activity?.Dispose();

        foreach (var toolCall in scenario.ToolCalls)
        {
            response.FunctionCalls().Should()
                .ShouldContainCall(toolCall);
        }
        
    }
}