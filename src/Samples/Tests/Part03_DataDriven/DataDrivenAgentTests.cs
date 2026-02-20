using FluentAssertions;
using System.Diagnostics;
using TDD.Common;
using TDD.Common.Helpers;

namespace TDD.Part03_DataDriven;

public class DataDrivenAgentTests : IDisposable
{
    private static readonly ActivitySource TestActivitySource = new("Travel.Tests", "1.0.0");


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
    [Trait("Category", "Unit")]
    public async Task PlanningAgent_ShouldRequestMissingInformationToolCall_WhenTravelPlanIsIncomplete(TravelPlanningScenario scenario)
    {
        using var testActivity = TestActivitySource.StartActivity($"TestCase: {scenario.Name}");

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

    public static IEnumerable<object[]> TravelPlanningScenarios()
    {
        var scenarios = ScenarioLoader.LoadPlanningWorkflowScenarios();
        foreach (var scenario in scenarios)
        {
            yield return [scenario];
        }
    }
}