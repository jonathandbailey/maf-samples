using System.Text.Json;
using FluentAssertions;
using TDD.Common;
using TDD.Common.Dto;
using TDD.Common.Helpers;

namespace TDD.Telemetry;

public class TelemetryAgentTests : IDisposable
{
    private const string Destination = "Paris";
    private const int NumberOfTravelers = 2;
    private static readonly DateTime DepartureDate = new(2026, 5, 1);

    private const string RequestInformationToolName = "RequestInformation";
    private const string ToolCallArgumentKey = "requestInformationDto";

    private readonly List<string> _expectedKeys = ["Origin", "ReturnDate"];

    private readonly TravelPlanDto _travePlanState = new(Destination: Destination, DepartureDate: DepartureDate, NumberOfTravelers: NumberOfTravelers);

    public TelemetryAgentTests()
    {
        TelemetryHelper.Initialize();
    }

    public void Dispose()
    {
        TelemetryHelper.Dispose();
    }

    [Fact]
    public async Task PlanningAgent_ShouldRequestMissingInformationToolCall_WhenTravelPlanIsIncomplete()
    {
        var agent = await AgentFactoryHelper.CreateMockPlanningAgent();

        var chatMessage = TravelPlanHelper.CreateTravelPlanMessage(_travePlanState);

        using var activity =  AgentTelemetry.Start(chatMessage.Text);

        var response = await agent.RunAsync(chatMessage);

        response.FunctionCalls()
            .Should().HaveCount(1).And
            .ShouldContainCall(RequestInformationToolName).And
            .ShouldHaveArgumentKey(ToolCallArgumentKey).And
            .ShouldHaveArgumentOfType<RequestInformationDto>(ToolCallArgumentKey).And
            .ShouldHaveRequiredInputs(ToolCallArgumentKey, _expectedKeys.Count, _expectedKeys);

        var functionCalls = response.FunctionCalls();

        foreach (var functionCallContent in functionCalls)
        {
            using var toolActivity = AgentTelemetry.ToolCall(functionCallContent.Name, functionCallContent?.Arguments?[ToolCallArgumentKey], activity);
        }
    }

}
