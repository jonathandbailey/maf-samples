using A2A.Server.Services;
using Shared.Agents;
using Shared.Extensions;

namespace A2A.Server.Tasks;


public class A2ATaskManager: IA2ATaskManager
{
    private readonly IAgentFactory _agentFactory;
    private readonly IA2ACardService _cardService;
    public ITaskManager TaskManager { get; } = new TaskManager();

    public A2ATaskManager(IAgentFactory agentFactory, IA2ACardService cardService)
    {
        _agentFactory = agentFactory;
        _cardService = cardService;

        TaskManager.OnTaskCreated += OnTaskCreated;
        TaskManager.OnAgentCardQuery+= OnAgentCardQuery;
    }

    private Task<AgentCard> OnAgentCardQuery(string url, CancellationToken cancellationToken)
    {
        return _cardService.GetAgentCard(url);
    }

    private async Task OnTaskCreated(AgentTask agentTask, CancellationToken cancellationToken)
    {
        var chatMessages = agentTask.ExtractTextPartsFromMessageHistory();

        var agent = await _agentFactory.Create(AgentTools.GetTools());

        var response = await agent.RunAsync(chatMessages, cancellationToken: cancellationToken);

        var textParts = response.ExtractChatMessageTextFromAgentResponse();

        var message = new AgentMessage
        {
            Role = MessageRole.Agent,
            ContextId = agentTask.ContextId,
            Parts = textParts.Cast<Part>().ToList()
        };

        await TaskManager.UpdateStatusAsync(agentTask.Id, TaskState.Completed, message, final: true, cancellationToken);
    }
}

public interface IA2ATaskManager
{
    ITaskManager TaskManager { get; }
}