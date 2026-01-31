# A2A (Agent-to-Agent) Tasks Sample

## Overview

This sample demonstrates how to implement an Agent-to-Agent (A2A) task-based communication pattern using the Microsoft Agent Framework. The A2A protocol enables asynchronous communication between agents, where one agent can delegate tasks to another agent and receive status updates and results over time. This pattern is essential for building distributed agent systems that need to coordinate work across multiple specialized agents.

Key scenarios include:
- Asynchronous task delegation between agents
- Long-running agent workflows with status tracking
- Agent discovery and dynamic tool integration
- Distributed agent orchestration in multi-agent systems
- Real-time task status updates and result delivery

## How It Works

The `A2ATaskManager` implements the A2A protocol by managing agent tasks through lifecycle events:

1. **Task Creation**: When a new task is created, the manager extracts the message history from the task context
2. **Agent Execution**: Creates an agent with appropriate tools and executes the task using the chat messages
3. **Status Updates**: Updates the task status to `Completed` with the agent's response when processing finishes
4. **Agent Card Discovery**: Provides agent card information to facilitate agent-to-agent discovery and communication

These operations are handled through event-driven callbacks that respond to task lifecycle events, enabling seamless asynchronous agent coordination.

## Key Components

### A2ATaskManager

The core manager that orchestrates A2A task execution:

```csharp
public class A2ATaskManager : IA2ATaskManager
{
    public ITaskManager TaskManager { get; } = new TaskManager();

    public A2ATaskManager(IAgentFactory agentFactory, IA2ACardService cardService)
    {
        TaskManager.OnTaskCreated += OnTaskCreated;
        TaskManager.OnAgentCardQuery += OnAgentCardQuery;
    }

    private async Task OnTaskCreated(AgentTask agentTask, CancellationToken cancellationToken)
    {
        // 1. Extract chat messages from task history
        var chatMessages = agentTask.ExtractTextPartsFromMessageHistory();

        // 2. Create agent with tools
        var agent = await _agentFactory.Create(AgentTools.GetTools());

        // 3. Execute agent
        var response = await agent.RunAsync(chatMessages, cancellationToken: cancellationToken);

        // 4. Extract and format response
        var textParts = response.ExtractChatMessageTextFromAgentResponse();
        var message = new AgentMessage { Role = MessageRole.Agent, ... };

        // 5. Update task status with result
        await TaskManager.UpdateStatusAsync(agentTask.Id, TaskState.Completed, message, ...);
    }

    private Task<AgentCard> OnAgentCardQuery(string url, CancellationToken cancellationToken)
    {
        return _cardService.GetAgentCard(url);
    }
}
```

### A2ACardService

Service for managing agent card information that describes agent capabilities:

- `GetAgentCard()`: Retrieves agent card metadata based on URL for agent discovery

### A2ASampleExtensions

Extension methods for wiring up the A2A endpoint:

- `MapA2ATaskSample()`: Configures the A2A endpoint with the task manager and agent card settings

## Usage

### Server Setup

The A2A server is registered in `Program.cs`:

```csharp
builder.Services.AddSingleton<IA2ATaskManager, A2ATaskManager>();
builder.Services.AddSingleton<IA2ACardService, A2ACardService>();
builder.Services.AddSingleton<IAgentFactory, AgentFactory>();

var app = builder.Build();
app.MapA2ATaskSample();
```

### Configuration

Agent cards are configured in `appsettings.json`:

```json
{
  "CardSettings": {
    "AgentCards": [
      {
        "Name": "Weather",
        "Url": "/a2a/weather"
      }
    ]
  }
}
```

### Client Integration

Client agents can discover and interact with A2A agents:

```csharp
// Discover the A2A agent
var a2AAgent = await a2ADiscoveryService.DiscoverAgentAsync(settings.Url, settings.Path);

// Create tools from the agent card
var tools = AgentTools.CreateToolsFromA2ACard(a2AAgent.Agent, a2AAgent.Card);

// Create agent with discovered tools
var agent = await agentFactory.Create([..tools]);
```

## Task Lifecycle

1. **Task Submission**: Client submits a task to the A2A endpoint with context and message history
2. **Task Creation Event**: `OnTaskCreated` fires, triggering agent execution
3. **Agent Processing**: Agent processes the task with available tools
4. **Status Updates**: Task status is updated as `Completed` with the agent's response
5. **Result Delivery**: Final response is delivered back to the requesting client/agent

## When to Use This Pattern

**Use A2A tasks when you need:**
- Asynchronous communication between autonomous agents
- Task delegation to specialized agents with specific capabilities
- Distributed agent systems with multiple cooperating agents
- Long-running agent workflows that report status over time
- Dynamic agent discovery and integration

**Consider alternative patterns when:**
- Synchronous request-response is sufficient (use AG-UI protocol)
- Single-agent scenarios without agent coordination needs
- Simple tool-based interactions without task state management
