# TDD Sample

## Overview

This sample demonstrates Test-Driven Development (TDD) patterns for building and testing AI agents using the Microsoft Agent Framework. It showcases how to write unit tests for agent behavior, tool calls, and structured outputs, enabling you to develop agents with confidence through automated testing.

Key scenarios include:
- Testing agent tool call behavior with declaration-only tools
- Validating structured agent responses using custom assertions
- Testing agent template loading and configuration
- Implementing testable agent patterns with helper utilities
- Verifying agent reasoning and information gathering workflows

## How It Works

The `PlanningAgentTests` class demonstrates testing a travel planning agent that uses structured reasoning to identify missing information and request it from users:

1. **Template Loading**: Tests that agent templates can be loaded from YAML files
2. **Tool Call Validation**: Verifies the agent correctly invokes tools when information is missing
3. **Structured Output Validation**: Checks that tool call arguments contain the expected data structures
4. **Information Gathering Logic**: Validates the agent's ability to identify and request missing required inputs

The sample uses declaration-only tools (tools that return schemas but don't execute) to test agent behavior in isolation without external dependencies.

## Key Components

### PlanningAgentTests

The main test class that validates agent behavior:

```csharp
[Fact]
public async Task PlanningAgent_ShouldRequestMissingInformationToolCall_WhenTravelPlanIsIncomplete()
{
    // Setup
    var languageModelSettings = SettingsHelper.GetLanguageModelSettings();
    var templateRepository = InfrastructureHelper.Create();
    var agentFactory = new AgentFactory(languageModelSettings);
    var template = await templateRepository.LoadAsync(PlanningYaml);
    var agent = await agentFactory.Create(template, PlanningTools.GetDeclarationOnlyTools());

    // Execute
    var chatMessage = TravelPlanHelper.CreateTravelPlanMessage(_travePlanState);
    var response = await agent.RunAsync(chatMessage);

    // Assert
    response.FunctionCalls()
        .Should().HaveCount(1).And
        .ShouldContainCall(RequestInformationToolName).And
        .ShouldHaveArgumentKey(ToolCallArgumentKey).And
        .ShouldHaveArgumentOfType<RequestInformationDto>(ToolCallArgumentKey).And
        .ShouldHaveRequiredInputs(ToolCallArgumentKey, _expectedKeys.Count, _expectedKeys);
}
```

### PlanningTools

Provides declaration-only tools for testing:

- `GetDeclarationOnlyTools()`: Returns tool schemas without execution logic for isolated testing
- `RequestInformation`: Tool that requests missing information from users with structured output

### Helper Classes

**AgentResponseHelper**
- Custom FluentAssertions extensions for validating agent responses
- Methods for checking function calls, arguments, and structured data

**TravelPlanHelper**
- Creates test messages with travel plan data
- Formats travel information as chat messages

**InfrastructureHelper**
- Sets up infrastructure dependencies for testing
- Configures template repositories and file storage

**SettingsHelper**
- Loads test configuration and settings
- Provides language model settings for test agents

### Data Models

**TravelPlanDto**
- Represents a travel plan with origin, destination, dates, and traveler count
- Uses nullable properties to simulate incomplete data

**RequestInformationDto**
- Structured output format for information requests
- Contains message, reasoning, and list of required inputs

## Usage

### Running Tests

Run the tests using the .NET test runner:

```bash
dotnet test src/Samples/Tests/TDD.csproj
```

Or run from Visual Studio Test Explorer.

### Test Structure

Tests follow the Arrange-Act-Assert pattern:

1. **Arrange**: Set up dependencies (settings, repositories, factories, tools)
2. **Act**: Execute agent with test input
3. **Assert**: Verify agent behavior using FluentAssertions

### Agent Template

The planning agent uses a YAML template (`planning.yaml`) that defines:
- Agent role and description
- Sequential reasoning instructions
- Input/output format specifications
- State validation logic

## When to Use This Pattern

**Use TDD for agents when you need:**
- Reliable, testable agent behavior
- Validation of tool call logic before integration
- Regression testing for agent responses
- Documentation of expected agent behavior through tests
- Confidence when refactoring agent prompts or logic

**Consider alternatives when:**
- Rapid prototyping without test coverage initially
- Simple agents with minimal logic
- Exploratory testing of agent capabilities

## Testing Best Practices

### Declaration-Only Tools

Use declaration-only tools to test agent decision-making without external dependencies:

```csharp
public static List<AITool> GetDeclarationOnlyTools()
{
    return Tools.Select(toolMeta => toolMeta.Value.AsDeclarationOnly())
        .Cast<AITool>()
        .ToList();
}
```

### Custom Assertions

Create fluent assertion extensions for agent-specific validations:

```csharp
response.FunctionCalls()
    .ShouldContainCall("ToolName")
    .ShouldHaveArgumentOfType<ExpectedType>("argumentKey");
```

### Test Helpers

Extract common setup logic into helper classes to keep tests clean and focused:

```csharp
var agent = await AgentFactory.Create(template, tools);
var message = TravelPlanHelper.CreateTravelPlanMessage(state);
```

## Related Samples

- **Tools/ManualToolCall**: Manual tool execution patterns
- **AGUI/StateSnapShotEvents**: State management and event handling
- **A2A.Server/Tasks**: Agent-to-agent communication testing

## Key Concepts

- **Declaration-Only Tools**: Tool schemas without execution logic for testing
- **FluentAssertions**: Expressive assertion library for readable test validation
- **Agent Templates**: YAML-based agent configuration for testable prompt engineering
- **Structured Outputs**: Typed agent responses for reliable parsing and validation
- **Test Helpers**: Reusable utilities for setting up test scenarios
- **TDD**: Test-Driven Development approach for building reliable AI agents
