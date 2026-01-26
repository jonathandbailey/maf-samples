using Api.Common.AGUI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace Api.Samples.AgUiStateSnapShot;

public class AGUIAgent(AIAgent agent) : DelegatingAIAgent(agent)
{
    private const string InProgress = "In Progress";
    private const string Completed = "Completed";
    private const string AgentProcessingRequest = "The agent is currently processing your request.";
    private const string AgentCompletedRequest = "The agent has completed processing your request.";

    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
        IEnumerable<ChatMessage> messages, 
        AgentThread? thread = null,
        AgentRunOptions? options = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
       
        yield return AGUIExtensions.CreateStatusSnapshotUpdate(InProgress, AgentProcessingRequest);

        await foreach(var agentResponse in base.RunCoreStreamingAsync(messages, thread, options, cancellationToken))
        {
            yield return agentResponse;
        }

        yield return AGUIExtensions.CreateStatusSnapshotUpdate(Completed, AgentCompletedRequest);
    }
}