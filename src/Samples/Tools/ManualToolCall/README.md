# Manual Tool Call Sample

## Overview

This sample demonstrates how to manually handle tool/function calls in a custom AI agent by intercepting tool calls from the language model and executing them explicitly in your code. This pattern gives you complete control over when and how tools are executed, making it useful for scenarios where you need to:

- Implement custom logic between tool detection and execution
- Batch process multiple tool calls before executing them
- Add logging, validation, or authorization checks before tool execution
- Manually control the flow of conversation with tools

## How It Works

The `ManualToolCallAgent` extends `DelegatingAIAgent` and overrides the `RunCoreStreamingAsync` method to:

1. **Collect Tool Calls**: First agent invocation collects all `FunctionCallContent` from the agent's response
2. **Manual Execution**: Manually invokes each tool using `AgentTools.Get()` and `function.InvokeAsync()`
3. **Return Results**: Sends tool results back to the agent in a second invocation with a `ChatMessage` of role `Tool`
4. **Stream Final Response**: Streams the final agent response after processing tool results

## Key Components

### ManualToolCallAgent

The custom agent that intercepts and manually handles tool calls:

```csharp
public class ManualToolCallAgent(AIAgent agent) : DelegatingAIAgent(agent)
{
    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(...)
    {
        // 1. First pass: collect tool calls
        var tools = new Dictionary<string, FunctionCallContent>();
        await foreach (var agentResponse in InnerAgent.RunStreamingAsync(...))
        {
            tools.AddToolCalls(agentResponse.Contents);
            yield return agentResponse;
        }

        // 2. Execute tools manually
        var toolResults = new List<AIContent>();
        foreach (var functionCallContent in tools)
        {
            var function = AgentTools.Get(functionCallContent.Key);
            var result = await function.InvokeAsync(...);
            toolResults.Add(new FunctionResultContent(...));
        }

        // 3. Send results back and stream final response
        var toolMessage = new ChatMessage(ChatRole.Tool, toolResults);
        await foreach (var update in InnerAgent.RunStreamingAsync([toolMessage], ...))
        {
            yield return update;
        }
    }
}
```

### ManualToolCallExtensions

Helper methods for creating the agent and processing tool calls:

- `CreateAgent()`: Factory method to create the ManualToolCallAgent with tools
- `AddToolCalls()`: Extension method to extract `FunctionCallContent` from agent responses

## Usage

### Agent Setup

The agent is registered in `Program.cs`:

```csharp
var toolCallAgent = await ManualToolCallExtensions.CreateAgent(agentFactory);
app.MapAGUI(Routes.ManualToolCallRoute, toolCallAgent);
```

### Available Tools

The sample uses tools defined in `Shared.Agents.AgentTools`:

- **GetWeather**: Returns weather information for a specified location

### Endpoint

The agent is exposed at the `/manual-tool-call` route and can be accessed through the AGUI interface.

## When to Use This Pattern

**Use manual tool calling when you need:**
- Custom logging or auditing of tool invocations
- Authorization checks before executing tools
- Batch processing of multiple tool calls
- Integration with external systems for tool execution
- Custom error handling or retry logic for tools

**Consider automatic tool calling when:**
- You want the framework to handle tool execution automatically
- You don't need custom logic between tool detection and execution
- Simpler implementation is preferred

## Comparison with Automatic Tool Calling

| Aspect | Manual Tool Call | Automatic Tool Call |
|--------|-----------------|-------------------|
| Control | Full control over execution | Framework handles execution |
| Complexity | More code required | Simpler implementation |
| Use Cases | Custom logic, validation, logging | Standard tool execution |
| Performance | Can batch/optimize calls | Sequential execution |

## Related Samples

- **AGUI/StateSnapShotEvents**: Demonstrates state management and event handling in AGUI
- See other samples in the `Tools/` directory for different tool handling patterns

## Key Concepts

- **DelegatingAIAgent**: Base class for wrapping and extending agent behavior
- **FunctionCallContent**: Represents a tool/function call from the LLM
- **FunctionResultContent**: Contains the result of a tool execution
- **ChatRole.Tool**: Message role used to send tool results back to the agent
- **AgentSession**: Maintains conversation state across multiple agent invocations
