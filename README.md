# Welcome to Microsoft Agent Framework Samples

Welcome to this collection of **Microsoft Agent Framework** samples. The primary objective of this repository is to build upon and extend the official framework examples, providing deeper dives into specific implementation patterns and architectural designs.

## Tech Stack
- .NET 10
- C#
- .NET Aspire

## Getting Started

### Prerequisites
- .NET 10 SDK
- Azure OpenAI resource

### Azure OpenAI Configuration Settings
The samples are built against an Azure OpenAI resource. Configure the following settings in `appsettings.Development.json` before running the samples:

```json
"LanguageModelSettings": {
  "DeploymentName": "",
  "Endpoint": ""
}
```

**Note**: These samples use Azure credential-based authentication (Azure CLI or Managed Identity) for security. 

## Samples

### AG-UI State Snapshot Events
**Location**: `src/Samples/AGUI/StateSnapShotEvents/`  
**Endpoint**: `/ag-ui/snapshot`

This sample demonstrates how to send custom state snapshot events to clients while streaming responses from an AI agent using the AG-UI protocol. It enables real-time status updates and progress indicators in your agent applications.

**Key Features**
- Extending an Agent using `DelegatingAIAgent`
- Intercepting streaming responses to inject custom events
- Using `DataContent` (JSON) to send AG-UI STATE_SNAPSHOT events
- Real-time progress indicators during long-running operations

**Key Scenarios**
- Sending progress updates during multi-step agent workflows
- Providing real-time status information to client applications
- Implementing custom UI state management through Server-Sent Events (SSE)
- Enhancing user experience with intermediate feedback before final responses

[View Full Documentation](src/Samples/AGUI/StateSnapShotEvents/README.md)

---

### Manual Tool Call
**Location**: `src/Samples/Tools/ManualToolCall/`  
**Endpoint**: `/manual-tool-call`

This sample demonstrates how to manually handle tool/function calls in a custom AI agent by intercepting tool calls from the language model and executing them explicitly in your code. This pattern gives you complete control over when and how tools are executed.

**Key Features**
- Manual interception and execution of tool calls
- Custom control flow between tool detection and execution
- Explicit tool result handling
- Multi-pass agent interaction pattern

**Key Scenarios**
- Implementing custom logic between tool detection and execution
- Batch processing of multiple tool calls before execution
- Adding logging, validation, or authorization checks before tool execution
- Custom error handling or retry logic for tools
- Integration with external systems for tool execution

[View Full Documentation](src/Samples/Tools/ManualToolCall/README.md)

---

### A2A Tasks
**Location**: `src/Samples/A2A.Server/Tasks/`  
**Endpoint**: `/a2a/task`

This sample demonstrates how to implement an Agent-to-Agent (A2A) task-based communication pattern using the Microsoft Agent Framework. The A2A protocol enables asynchronous communication between agents, where one agent can delegate tasks to another agent and receive status updates and results over time.

**Key Features**
- Task lifecycle management through event-driven callbacks
- Agent task creation and execution orchestration
- Agent card discovery for agent-to-agent communication
- Asynchronous status updates and result delivery

**Key Scenarios**
- Asynchronous task delegation between agents
- Long-running agent workflows with status tracking
- Agent discovery and dynamic tool integration
- Distributed agent orchestration in multi-agent systems
- Real-time task status updates and result delivery

[View Full Documentation](src/Samples/A2A.Server/Tasks/README.md)

---

### TDD
**Location**: `src/Samples/Tests/`

This sample demonstrates Test-Driven Development (TDD) patterns for building and testing AI agents using the Microsoft Agent Framework. It showcases how to write unit tests for agent behavior, tool calls, and structured outputs, enabling you to develop agents with confidence through automated testing.

**Key Features**
- Testing agent tool call behavior with declaration-only tools
- Validating structured agent responses using custom assertions
- Testing agent template loading and configuration
- Implementing testable agent patterns with helper utilities

**Key Scenarios**
- Testing agent tool call behavior with declaration-only tools
- Validating structured agent responses using custom assertions
- Testing agent template loading and configuration
- Implementing testable agent patterns with helper utilities
- Verifying agent reasoning and information gathering workflows

[View Full Documentation](src/Samples/Tests/Agents/README.md)

---

### Telemetry
**Location**: `src/Samples/Tests/Telemetry/`

This sample demonstrates how to add OpenTelemetry tracing to AI agent test runs and export the telemetry to a local .NET Aspire Dashboard. It showcases how to instrument agent invocations and tool calls using `System.Diagnostics.Activity`, giving you full visibility into agent behavior through distributed tracing.

**Key Features**
- Instrumenting agent invocations with OpenTelemetry traces
- Creating parent-child spans for agent and tool call relationships
- Exporting telemetry to the Aspire Dashboard via OTLP/gRPC
- Using semantic conventions for generative AI tracing (`gen_ai.*` attributes)

**Key Scenarios**
- Visibility into agent execution flow and timing
- Debugging tool call sequences and arguments
- Monitoring agent behavior in CI/CD pipelines
- Collecting traces for performance analysis
- Understanding parent-child relationships between agent and tool call spans

[View Full Documentation](src/Samples/Tests/Telemetry/README.md)

