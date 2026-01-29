using Shared.Agents;

namespace A2A.Server.Tasks;


public class A2ATaskManager: IA2ATaskManager
{
    private readonly IAgentFactory _agentFactory;
    public ITaskManager TaskManager { get; } = new TaskManager();

    public A2ATaskManager(IAgentFactory agentFactory)
    {
        _agentFactory = agentFactory;
        TaskManager.OnTaskCreated += OnTaskCreated;
    }

    private async Task OnTaskCreated(AgentTask agentTask, CancellationToken cancellationToken)
    {
        var messageText = agentTask.History.OfType<AgentMessage>().First().Parts.OfType<TextPart>().First().Text;

        var agent = await _agentFactory.Create(AgentTools.GetTools());

        var response = await agent.RunAsync(messageText, cancellationToken: cancellationToken);

        var message = new AgentMessage
        {
            Role = MessageRole.Agent,
            ContextId = agentTask.ContextId,
            Parts = [ new TextPart { Text = response.Text } ]
        };

        await TaskManager.UpdateStatusAsync(agentTask.Id, TaskState.Completed, message, final: true, cancellationToken);
    }
}

public interface IA2ATaskManager
{
    ITaskManager TaskManager { get; }
}