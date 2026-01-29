namespace A2A.Server.Tasks;


public class A2ATaskManager : IA2ATaskManager
{
    public ITaskManager TaskManager { get; } = new TaskManager();

    public A2ATaskManager()
    {
        TaskManager.OnTaskCreated += OnTaskCreated;
    }

    private async Task OnTaskCreated(AgentTask agentTask, CancellationToken cancellationToken)
    {
        var message = new AgentMessage
        {
            Role = MessageRole.Agent,
            ContextId = agentTask.ContextId,
            Parts = [ new TextPart { Text = "Completed." } ]
        };

        await TaskManager.UpdateStatusAsync(agentTask.Id, TaskState.Completed, message, final: true, cancellationToken);
    }
}

public interface IA2ATaskManager
{
    ITaskManager TaskManager { get; }
}