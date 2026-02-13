using FluentAssertions;
using Shared.Agents;
using TDD.Common;
using TDD.Common.Dto;
using TDD.Common.Helpers;

namespace TDD.Agents;

public class PlanningAgentTests
{
    private const string PlanningYaml = "planning.yaml";

    private const string Destination = "Paris";
    private const int NumberOfTravelers = 2;
    private static readonly DateTime DepartureDate = new(2026, 5, 1);

    private const string RequestInformationToolName = "RequestInformation";
    private const string ToolCallArgumentKey = "requestInformationDto";

    private readonly List<string> _expectedKeys = ["Origin", "ReturnDate"];

    private readonly TravelPlanDto _travePlanState = new(Destination: Destination, DepartureDate: DepartureDate, NumberOfTravelers: NumberOfTravelers);

    [Fact]
    public async Task AgentTemplateRepository_ShouldLoadPlanningTemplate()
    {
        var templateRepository = InfrastructureHelper.Create();

        var template = await templateRepository.LoadAsync(PlanningYaml);

        template.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task PlanningAgent_ShouldRequestMissingInformationToolCall_WhenTravelPlanIsIncomplete()
    {
        var languageModelSettings = SettingsHelper.GetLanguageModelSettings();

        var templateRepository = InfrastructureHelper.Create();

        var agentFactory = new AgentFactory(languageModelSettings);

        var template = await templateRepository.LoadAsync(PlanningYaml);

        var agent = await agentFactory.Create(template, PlanningTools.GetDeclarationOnlyTools());

        var chatMessage = TravelPlanHelper.CreateTravelPlanMessage(_travePlanState);

        var response = await agent.RunAsync(chatMessage);

        response.FunctionCalls()
            .Should().HaveCount(1).And
            .ShouldContainCall(RequestInformationToolName).And
            .ShouldHaveArgumentKey(ToolCallArgumentKey).And
            .ShouldHaveArgumentOfType<RequestInformationDto>(ToolCallArgumentKey).And
            .ShouldHaveRequiredInputs(ToolCallArgumentKey, _expectedKeys.Count, _expectedKeys);
    }

}
