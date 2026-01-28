using System.Runtime.CompilerServices;
using Api.Common.Agents;
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
        var agentThread = await InnerAgent.GetNewThreadAsync(cancellationToken);
        
        var tools = new Dictionary<string, FunctionCallContent>();

        await foreach (var agentResponse in InnerAgent.RunStreamingAsync(messages, agentThread, options, cancellationToken))
        {
            tools.AddToolCalls(agentResponse.Contents);

            yield return agentResponse;
        }

        var toolResults = new List<AIContent>();

        foreach (var functionCallContent in tools)
        {
            var function = Tools.Get(functionCallContent.Key);

            var result = await function.InvokeAsync(new AIFunctionArguments(functionCallContent.Value.Arguments), cancellationToken);

            toolResults.Add(new FunctionResultContent(
                functionCallContent.Value.CallId,
                result));
        }

        var toolMessage = new ChatMessage(ChatRole.Tool, toolResults);
        
        await foreach (var update in InnerAgent.RunStreamingAsync([toolMessage], agentThread, cancellationToken: cancellationToken))
        {      
            yield return update;
        }
    }
}