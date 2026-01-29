# AG-UI State Snapshot Events Sample

## Overview

This sample demonstrates how to send custom state snapshot events to clients while streaming responses from an AI agent using the AG-UI protocol. This pattern enables real-time status updates and progress indicators in your agent applications, providing users with immediate feedback about what the agent is doing during long-running operations.

Key scenarios include:
- Sending progress updates during multi-step agent workflows
- Providing real-time status information to client applications
- Implementing custom UI state management through Server-Sent Events (SSE)
- Enhancing user experience with intermediate feedback before final responses

## How It Works

The `AGUIAgent` extends `DelegatingAIAgent` and intercepts the streaming response to inject custom state snapshot events:

1. **Initial Status**: Sends a "In Progress" status update before processing begins
2. **Stream Agent Response**: Passes through all agent responses from the underlying agent
3. **Completion Status**: Sends a "Completed" status update after processing finishes

These status updates are serialized as JSON `DataContent` and sent through the same SSE stream as the agent's responses, allowing clients to receive and process them in real-time.

## Key Components

### AGUIAgent

The custom agent that injects state snapshot events into the response stream:

```csharp
public class AGUIAgent(AIAgent agent) : DelegatingAIAgent(agent)
{
    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(...)
    {
        // 1. Send initial "In Progress" status
        yield return AGUIExtensions.CreateStatusSnapshotUpdate(
            InProgress, 
            AgentProcessingRequest);

        // 2. Stream all agent responses
        await foreach(var agentResponse in base.RunCoreStreamingAsync(...))
        {
            yield return agentResponse;
        }

        // 3. Send final "Completed" status
        yield return AGUIExtensions.CreateStatusSnapshotUpdate(
            Completed, 
            AgentCompletedRequest);
    }
}
```

### AGUIExtensions

Helper methods for creating state snapshot updates:

- `CreateStatusSnapshotUpdate()`: Creates an `AgentResponseUpdate` containing a serialized state snapshot with status and message

### AGUIStatusUpdate

Data models for state snapshots:

- `AGUISnapshot<T>`: Generic wrapper for any state snapshot with a type identifier
- `AGUIStatusUpdate`: Specific implementation for status updates with status and message properties

## Usage

### Agent Setup

The agent is registered in `Program.cs`:

```csharp
var agUiAgent = await AGUISnapShotExtensions.CreateAgent(agentFactory);
app.MapAGUI(Routes.AGUISnapshotRoute, agUiAgent);
```

### Endpoint

The agent is exposed at the `/ag-ui/snapshot` route and uses the AG-UI protocol for streaming responses.

### State Snapshot Format

State snapshots are sent as JSON objects in `DataContent` with media type `application/json`:

```json
{
  "Type": "StatusUpdate",
  "Data": {
    "Type": "StatusUpdate",
    "Status": "In Progress",
    "Message": "The agent is currently processing your request."
  }
}
```

## When to Use This Pattern

**Use state snapshot events when you need:**
- Progress indicators for long-running agent operations
- Real-time status updates in client applications
- Custom UI state management synchronized with agent processing
- Intermediate feedback before final responses arrive
- Multi-step workflow visibility for users

**Consider alternatives when:**
- Your agent responses are fast and don't need progress updates
- Simple response streaming without custom state is sufficient
- You don't need to track intermediate processing states

## Extending the Pattern

You can extend this pattern to support various types of state snapshots:

```csharp
// Custom state snapshot types
public class ProgressSnapshot(int current, int total, string step)
{
    public string Type { get; init; } = "Progress";
    public int Current { get; init; } = current;
    public int Total { get; init; } = total;
    public string Step { get; init; } = step;
}

// Send custom state snapshots
yield return CreateCustomSnapshot(new ProgressSnapshot(1, 5, "Analyzing input"));
```

## AG-UI Protocol

The AG-UI protocol uses Server-Sent Events (SSE) to stream responses from the agent to clients. State snapshots are sent as `DataContent` within the `AgentResponseUpdate` stream, allowing clients to:

- Receive real-time updates without polling
- Process different content types (text, tool calls, state snapshots) in a unified stream
- Maintain connection state throughout the agent's processing

## Related Samples

- **Tools/ManualToolCall**: Demonstrates manual tool execution with custom logic
- See other samples for additional patterns with agents and streaming responses

## Key Concepts

- **DelegatingAIAgent**: Base class for wrapping and extending agent behavior
- **AgentResponseUpdate**: Container for content streamed from agents to clients
- **DataContent**: Content type for arbitrary data with media type specification
- **Server-Sent Events (SSE)**: Protocol for real-time server-to-client streaming
- **AG-UI Protocol**: Microsoft Agents Framework protocol for agent UI communication
- **State Snapshots**: Custom events sent during agent processing to update client state