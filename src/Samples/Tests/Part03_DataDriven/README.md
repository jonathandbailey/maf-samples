# Data-Driven Agent Tests

## Overview

This sample builds on the telemetry patterns from Part 02 by introducing data-driven testing with xUnit's `[Theory]` attribute. Instead of writing one test per scenario, test cases are loaded from a JSON file — making it easy to grow coverage without touching test code.

Key scenarios include:
- Parameterising agent tests with `[Theory]` and `MemberData`
- Loading test scenarios from an external JSON file
- Using `IXunitSerializable` to support individual test re-runs from the CLI and Test Explorer
- Wrapping each scenario with its own OpenTelemetry span for trace-level visibility per test case

## How It Works

`DataDrivenAgentTests` reads a list of `TravelPlanningScenario` objects from `Data/PlanningAgentScenarios.json` and feeds them into a single `[Theory]` test method via `MemberData`:

1. **Scenario loading**: `ScenarioLoader` reads and deserialises the JSON file at test startup
2. **Test parameterisation**: `TheoryData<TravelPlanningScenario>` passes each scenario as a separate test case
3. **Per-scenario tracing**: An outer `TestCase: {scenario.Name}` activity wraps the agent and tool spans, grouping everything for that scenario in the Aspire Dashboard
4. **Assertions**: The test verifies that each expected tool call in `scenario.ToolCalls` appears in the agent response

The result in the Aspire Dashboard is one trace per scenario:

```
TestCase: MissingOriginAndReturnDate
  └── invoke_agent Planning
        └── execute_tool RequestInformation

TestCase: MissingOriginOnly
  └── invoke_agent Planning
        └── execute_tool RequestInformation
```

## Key Components

### DataDrivenAgentTests

The test class — a single `[Theory]` replaces what would otherwise be three separate `[Fact]` methods:

```csharp
[Theory]
[MemberData(nameof(TravelPlanningScenarios))]
[Trait("Category", "Unit")]
public async Task PlanningAgent_ShouldRequestMissingInformationToolCall_WhenTravelPlanIsIncomplete(
    TravelPlanningScenario scenario)
{
    using var testActivity = TestActivitySource.StartActivity($"TestCase: {scenario.Name}");

    var agent = await AgentFactoryHelper.CreateMockPlanningAgent();
    var chatMessage = TravelPlanHelper.CreateTravelPlanMessage(scenario.TravelPlan);

    using var activity = AgentTelemetry.Start(chatMessage.Text);
    var response = await agent.RunAsync(chatMessage);

    foreach (var functionCallContent in response.FunctionCalls())
        using var toolActivity = AgentTelemetry.ToolCall(functionCallContent.Name, functionCallContent.Arguments, activity);

    foreach (var toolCall in scenario.ToolCalls)
        functionCalls.Should().ShouldContainCall(toolCall);
}
```

### TravelPlanningScenario

Implements `IXunitSerializable` so xUnit can serialise and deserialise scenario instances. This is what enables **targeted re-runs** — when a scenario fails you can re-run just that case:

```bash
dotnet test --filter "DisplayName~MissingOriginOnly"
```

Because `IXunitSerializable` requires a mutable class (it populates fields in `Deserialize`), `TravelPlanningScenario` is a `class` rather than a `record`. The `Serialize`/`Deserialize` methods use `System.Text.Json` to round-trip the nested `TravelPlanDto` and `ToolCalls` list.

### ScenarioLoader

Loads and deserialises `PlanningAgentScenarios.json` from the build output directory:

```csharp
var filePath = Path.Combine(AppContext.BaseDirectory, DataPath, Filename);
var json = File.ReadAllText(filePath);
return JsonSerializer.Deserialize<List<TravelPlanningScenario>>(json, SerializerOptions) ?? [];
```

The JSON file is copied to the output directory via a `PreserveNewest` entry in `TDD.csproj`.

### PlanningAgentScenarios.json

Each entry represents one test case — add new scenarios here without touching test code:

```json
[
  {
    "Name": "MissingOriginAndReturnDate",
    "TravelPlan": { "Origin": null, "Destination": "Paris", ... },
    "ToolCalls": [ "RequestInformation" ]
  }
]
```

`Name` is used as the test display name in Test Explorer and in telemetry span names.

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
# All data-driven tests
dotnet test src/Samples/Tests/TDD.csproj --filter "FullyQualifiedName~DataDriven"

# A single scenario by name
dotnet test src/Samples/Tests/TDD.csproj --filter "DisplayName~MissingOriginOnly"
```

After the tests complete, open the Aspire Dashboard (default: `http://localhost:18888`) and navigate to the **Traces** view to see one trace per scenario.

## When to Use This Pattern

**Use data-driven tests when you need:**
- Coverage across multiple input variations without duplicating test logic
- Non-developers (QA, product) to contribute test cases by editing JSON
- A growing scenario library that evolves independently of test code
- Per-scenario telemetry to diagnose which inputs trigger unexpected behaviour

**Consider alternatives when:**
- Scenarios are highly interdependent and share complex setup
- The variations are better expressed as separate, named `[Fact]` tests with distinct assertion chains
- You need different assertions per scenario (data-driven works best when the assertion shape is consistent)

## Adding New Scenarios

1. Open `Data/PlanningAgentScenarios.json`
2. Add a new entry with a unique `Name`, the desired `TravelPlan` state, and the expected `ToolCalls`
3. Run the tests — no code changes needed

## Related Samples

- **Part01_Agents**: Foundation TDD patterns with a live Azure OpenAI agent
- **Part02_Telemetry**: Adds OpenTelemetry tracing to a single-test scenario
- **Tools/ManualToolCall**: Manual tool execution patterns

## Key Concepts

- **Theory / MemberData**: xUnit attributes for parameterised tests
- **IXunitSerializable**: Interface enabling xUnit to serialise test parameters for individual re-runs
- **ScenarioLoader**: Externalises test data so scenarios can grow without modifying test code
- **TheoryData\<T\>**: Strongly-typed collection for passing test parameters via `MemberData`
- **Per-scenario spans**: Wrapping each theory case in its own OpenTelemetry activity for trace-level test visibility
