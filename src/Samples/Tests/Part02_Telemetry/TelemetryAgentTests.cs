using FluentAssertions;
using TDD.Common;
using TDD.Common.Dto;
using TDD.Common.Helpers;

namespace TDD.Part02_Telemetry
{
    public class TelemetryAgentTests : IClassFixture<TelemetryFixture>
    {
        private const string Destination = "Paris";
        private const int NumberOfTravelers = 2;
        private static readonly DateTime DepartureDate = new(2026, 5, 1);

        private const string RequestInformationToolName = "RequestInformation";
        private const string ToolCallArgumentKey = "requestInformationDto";

        private readonly List<string> _expectedKeys = ["Origin", "ReturnDate"];

        private readonly TravelPlanDto _travePlanState = new(Destination: Destination, DepartureDate: DepartureDate, NumberOfTravelers: NumberOfTravelers);

        [Fact]
        [Trait("Category", "Unit")]
        public async Task PlanningAgent_ShouldRequestMissingInformationToolCall_WhenTravelPlanIsIncomplete()
        {
            var agent = await AgentFactoryHelper.CreateMockPlanningAgent();

            var chatMessage = TravelPlanHelper.CreateTravelPlanMessage(_travePlanState);

            using var activity =  AgentTelemetry.Start(chatMessage.Text);

            var response = await agent.RunAsync(chatMessage);
    
            foreach (var functionCallContent in response.FunctionCalls())
            {
                using var toolActivity = AgentTelemetry.ToolCall(functionCallContent.Name, functionCallContent.Arguments, activity);
            }
       
            response.FunctionCalls()
                .Should().HaveCount(1).And
                .ShouldContainCall(RequestInformationToolName).And
                .ShouldHaveArgumentKey(ToolCallArgumentKey).And
                .ShouldHaveArgumentOfType<RequestInformationDto>(ToolCallArgumentKey).And
                .ShouldHaveRequiredInputs(ToolCallArgumentKey, _expectedKeys.Count, _expectedKeys);
        }

    }
}
