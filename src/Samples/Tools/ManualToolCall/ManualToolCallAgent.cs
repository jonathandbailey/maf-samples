using System.Runtime.CompilerServices;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Shared.Agents;


namespace Tools.ManualToolCall;

public class ManualToolCallAgent(AIAgent agent) : DelegatingAIAgent(agent)
{
    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? thread = null,
        AgentRunOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var agentSession = await InnerAgent.GetNewSessionAsync(cancellationToken);
        
        var tools = new Dictionary<string, FunctionCallContent>();

        await foreach (var agentResponse in InnerAgent.RunStreamingAsync(messages, agentSession, options, cancellationToken))
        {
            tools.AddToolCalls(agentResponse.Contents);

            yield return agentResponse;
        }

        var toolResults = new List<AIContent>();

        foreach (var functionCallContent in tools)
        {
            var function = AgentTools.Get(functionCallContent.Key);

            var result = await function.InvokeAsync(new AIFunctionArguments(functionCallContent.Value.Arguments), cancellationToken);

            toolResults.Add(new FunctionResultContent(
                functionCallContent.Value.CallId,
                result));
        }

        var toolMessage = new ChatMessage(ChatRole.Tool, toolResults);
        
        await foreach (var update in InnerAgent.RunStreamingAsync([toolMessage], agentSession, cancellationToken: cancellationToken))
        {      
            yield return update;
        }
    }
}