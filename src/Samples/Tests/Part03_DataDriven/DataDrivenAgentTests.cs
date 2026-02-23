using FluentAssertions;
using System.Diagnostics;
using TDD.Common;
using TDD.Common.Helpers;

namespace TDD.Part03_DataDriven;

public class DataDrivenAgentTests : IClassFixture<TelemetryFixture>
{
    private static readonly ActivitySource TestActivitySource = new("Travel.Tests", "1.0.0");

    [Theory]
    [MemberData(nameof(TravelPlanningScenarios))]
    [Trait("Category", "Unit")]
    public async Task PlanningAgent_ShouldRequestMissingInformationToolCall_WhenTravelPlanIsIncomplete(TravelPlanningScenario scenario)
    {
        using var testActivity = TestActivitySource.StartActivity($"TestCase: {scenario.Name}");

        var agent = await AgentFactoryHelper.CreateMockPlanningAgent();

        var chatMessage = TravelPlanHelper.CreateTravelPlanMessage(scenario.TravelPlan);

        using var activity = AgentTelemetry.Start(chatMessage.Text);

        var response = await agent.RunAsync(chatMessage);

        var functionCalls = response.FunctionCalls();

        foreach (var functionCallContent in functionCalls)
        {
            using var toolActivity = AgentTelemetry.ToolCall(functionCallContent.Name, functionCallContent.Arguments, activity);
        }

        foreach (var toolCall in scenario.ToolCalls)
        {
            functionCalls.Should()
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