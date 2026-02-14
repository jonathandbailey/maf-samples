# Telemetry Sample

## Overview

This sample demonstrates how to add OpenTelemetry tracing to AI agent test runs and export the telemetry to a local .NET Aspire Dashboard. It showcases how to instrument agent invocations and tool calls using `System.Diagnostics.Activity`, giving you full visibility into agent behavior through distributed tracing.

Key scenarios include:
- Instrumenting agent invocations with OpenTelemetry traces
- Creating parent-child spans for agent and tool call relationships
- Exporting telemetry to the Aspire Dashboard via OTLP/gRPC
- Using semantic conventions for generative AI tracing (`gen_ai.*` attributes)
- Configuring telemetry settings through a checked-in config file

## How It Works

The `TelemetryAgentTests` class extends the TDD agent testing pattern by wrapping agent execution in OpenTelemetry activities:

1. **Telemetry Initialization**: The test constructor initializes the `TracerProvider` with OTLP export to the Aspire Dashboard
2. **Agent Invocation Span**: A parent activity is started before the agent runs, capturing the agent name and prompt
3. **Tool Call Spans**: Child activities are created for each tool call returned by the agent, linked to the parent span
4. **Flush on Dispose**: The `TracerProvider` is disposed after each test to ensure all spans are exported

The result is a trace hierarchy visible in the Aspire Dashboard:

```
invoke_agent Planning
  └── execute_tool RequestInformation
```

## Key Components

### TelemetryAgentTests

The test class that instruments agent execution with tracing:

```csharp
[Fact]
public async Task PlanningAgent_ShouldRequestMissingInformationToolCall_WhenTravelPlanIsIncomplete()
{
    var agent = await AgentFactoryHelper.CreateMockPlanningAgent();

    var chatMessage = TravelPlanHelper.CreateTravelPlanMessage(_travePlanState);

    var activity = AgentTelemetry.Start(chatMessage.Text);

    var response = await agent.RunAsync(chatMessage);

    foreach (var functionCallContent in response.FunctionCalls())
    {
        using var toolActivity = AgentTelemetry.ToolCall(
            functionCallContent.Name,
            functionCallContent.Arguments?[ToolCallArgumentKey],
            activity);
    }

    activity?.Dispose();

    response.FunctionCalls()
        .Should().HaveCount(1).And
        .ShouldContainCall(RequestInformationToolName).And
        .ShouldHaveArgumentKey(ToolCallArgumentKey).And
        .ShouldHaveArgumentOfType<RequestInformationDto>(ToolCallArgumentKey).And
        .ShouldHaveRequiredInputs(ToolCallArgumentKey, _expectedKeys.Count, _expectedKeys);
}
```

### AgentTelemetry

Static helper that creates OpenTelemetry activities with generative AI semantic conventions:

- `Start(input)`: Creates a parent span for the agent invocation with `gen_ai.agent.name` and `gen_ai.prompt` attributes
- `ToolCall(key, arguments, parent)`: Creates a child span for tool execution with `gen_ai.tool.name` and `gen_ai.tool.parameters` attributes

### TelemetryHelper

Configures the OpenTelemetry `TracerProvider` with OTLP export:

- Reads endpoint and API key from `AspireDashboardSettings`
- Registers the `TDD*` activity source for trace collection
- Exports spans via OTLP/gRPC to the Aspire Dashboard

### Configuration

Aspire Dashboard settings are stored in `appsettings.json` (checked into the repo):

```json
{
  "AspireDashboard": {
    "OtlpEndpoint": "https://localhost:21291",
    "OtlpApiKey": "537f36931ab1e7b3e3a919d4cc7ccb87"
  }
}
```

These values match the local Aspire Dashboard's OTLP ingestion endpoint. Update them if your dashboard uses a different port or API key.

## Usage

### Prerequisites

Start the Aspire Dashboard before running the tests. You can run it as a standalone container:

```bash
docker run --rm -it -d -p 18888:18888 -p 4317:18889 --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:9.2
```

Or use the project's Aspire AppHost:

```bash
dotnet run --project src/AppHost/AppHost.csproj
```

### Running Tests

```bash
dotnet test src/Samples/Tests/TDD.csproj --filter "FullyQualifiedName~Telemetry"
```

Or run from Visual Studio Test Explorer.

After the test completes, open the Aspire Dashboard (default: `http://localhost:18888`) and navigate to the **Traces** view to inspect the agent and tool call spans.

## When to Use This Pattern

**Use telemetry instrumentation when you need:**
- Visibility into agent execution flow and timing
- Debugging tool call sequences and arguments
- Monitoring agent behavior in CI/CD pipelines
- Collecting traces for performance analysis
- Understanding parent-child relationships between agent and tool call spans

**Consider alternatives when:**
- Simple unit tests where console output is sufficient
- No Aspire Dashboard or OTLP collector is available
- Minimal agent logic that doesn't warrant tracing overhead

## Related Samples

- **Agents/**: TDD patterns for testing agent behavior without telemetry
- **Tools/ManualToolCall**: Manual tool execution patterns
- **AGUI/StateSnapShotEvents**: State management and event handling

## Key Concepts

- **OpenTelemetry**: Vendor-neutral observability framework for distributed tracing
- **ActivitySource**: .NET API for creating trace spans (`System.Diagnostics`)
- **OTLP/gRPC**: Protocol for exporting telemetry data to collectors and dashboards
- **Aspire Dashboard**: Local .NET dashboard for viewing traces, metrics, and logs
- **Semantic Conventions**: Standardized attribute names (`gen_ai.*`) for generative AI tracing
- **TracerProvider**: Configures trace collection, processing, and export
