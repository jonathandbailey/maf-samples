using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Api.Samples.AgUiStateSnapShot;

public class AGUIAgent(AIAgent agent) : DelegatingAIAgent(agent)
{
    private const string ApplicationJsonMediaType = "application/json";

    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
        IEnumerable<ChatMessage> messages, 
        AgentThread? thread = null,
        AgentRunOptions? options = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var stateBytes = JsonSerializer.SerializeToUtf8Bytes("This is an AG-UI STATE_SNAPSHOT pre agent run.");

        yield return new AgentResponseUpdate
        {
            Contents = [new DataContent(stateBytes, ApplicationJsonMediaType)]
        };

        await foreach(var agentResponse in base.RunCoreStreamingAsync(messages, thread, options, cancellationToken))
        {
            yield return agentResponse;
        }

        stateBytes = JsonSerializer.SerializeToUtf8Bytes("This is an AG-UI STATE_SNAPSHOT post agent run.");

        yield return new AgentResponseUpdate
        {
            Contents = [new DataContent(stateBytes, ApplicationJsonMediaType)]
        };
    }
}