using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Moq;
using Shared.Agents;
using TDD.Common.Dto;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace TDD.Common.Helpers;

public class AgentFactoryHelper
{
    private const string PlanningYaml = "planning.yaml";

    public static async Task<AIAgent> CreateMockPlanningAgent()
    {
        var mockChatClient = new Mock<IChatClient>();

        var requestInfoDto = new RequestInformationDto(
            Message: "Please provide the missing information",
            Thought: "Need to request missing travel information",
            RequiredInputs: ["Origin", "ReturnDate"]
        );

        var requestInfoElement = System.Text.Json.JsonSerializer.SerializeToElement(requestInfoDto);

        var functionCallContent = new FunctionCallContent(
            callId: "call_123",
            name: "RequestInformation",
            arguments: new Dictionary<string, object?>
            {
                ["requestInformationDto"] = requestInfoElement
            }
        );

        var responseMessage = new ChatMessage(ChatRole.Assistant, [functionCallContent]);

        mockChatClient
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IList<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatResponse(responseMessage));

        var templateRepository = InfrastructureHelper.Create();

        var agentFactory = new AgentFactory();

        var template = await templateRepository.LoadAsync(PlanningYaml);

        var agent = await agentFactory.Create(mockChatClient.Object, template, PlanningTools.GetDeclarationOnlyTools());

        return agent;
    }
}