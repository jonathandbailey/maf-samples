using Microsoft.Agents.AI;
using Microsoft.Agents.AI.A2A;
using Microsoft.Extensions.AI;

namespace A2A.Client.Tasks;

public class AGUIAgent(AIAgent agent, A2AAgent a2Agent) : DelegatingAIAgent(agent)
{
    protected override IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
        IEnumerable<ChatMessage> messages, AgentSession? session = null,
        AgentRunOptions? options = null, CancellationToken cancellationToken = new CancellationToken())
    {
        return base.RunCoreStreamingAsync(messages, session, options, cancellationToken);
    }
}