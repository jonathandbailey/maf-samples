using System.Runtime.CompilerServices;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Api.Samples.ManualToolCall;

public class ManualToolCallAgent(AIAgent agent) : DelegatingAIAgent(agent)
{
    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
     
        await foreach (var agentResponse in base.RunCoreStreamingAsync(messages, thread, options, cancellationToken))
        {
            yield return agentResponse;
        }
    }
}