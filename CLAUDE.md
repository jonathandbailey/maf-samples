# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Microsoft Agent Framework (MAF) samples repository demonstrating AI agent patterns in .NET 10 with C#. Built on Microsoft.Agents.AI (preview) with Azure OpenAI as the LLM provider. Uses Azure credential-based authentication (Azure CLI, Visual Studio, or Managed Identity).

## Build & Run Commands

```bash
# Build entire solution
dotnet build MafSamples.slnx

# Run tests
dotnet test src/Samples/Tests/TDD.csproj

# Run a single test
dotnet test src/Samples/Tests/TDD.csproj --filter "FullyQualifiedName~MethodName"

# Run the API
dotnet run --project src/Api/Api.csproj

# Run via Aspire orchestration
dotnet run --project src/AppHost/AppHost.csproj
```

## Architecture

**Solution structure**: `Shared` (core) -> `Samples` (implementations) -> `Api` (host)

- **src/Shared/** - Core agent infrastructure: `AgentFactory` (creates agents via Azure OpenAI), `AgentTools` (tool definitions with `AIFunctionFactory`), `CustomPromptAgentFactory` (YAML-based agent creation), settings classes, and extension methods
- **src/Api/** - ASP.NET Core host. `Program.cs` registers services via DI; `ApiExtensions.MapSamples()` wires all sample endpoints
- **src/AppHost/** - .NET Aspire distributed app host for orchestrating multiple services
- **src/ServiceDefaults/** - Shared OpenTelemetry, resilience, and health check configuration
- **src/Samples/** - Three sample implementations:
  - **AGUI/StateSnapShotEvents/** - Extends agents via `DelegatingAIAgent` to inject AG-UI STATE_SNAPSHOT events during streaming
  - **Tools/ManualToolCall/** - Intercepts and manually executes tool calls from the LLM
  - **A2A.Client/ & A2A.Server/** - Agent-to-Agent task delegation with card-based discovery

## Key Patterns

- **Agent creation**: `AgentFactory.Create()` builds agents from `ChatClient` with optional YAML templates and tools. Uses `ChainedTokenCredential` for Azure auth
- **Tool definition**: Static `AIFunctionFactory.Create()` methods, stored in dictionaries. Use `.AsDeclarationOnly()` for tools that should be detected but not auto-executed
- **Agent extension**: `DelegatingAIAgent` base class wraps agents to intercept/augment behavior
- **Configuration**: Options pattern (`IOptions<T>`) with settings in `appsettings.Development.json` and User Secrets for credentials

## Testing

- **Framework**: xUnit with FluentAssertions and Moq
- **Test project**: `src/Samples/Tests/` - tests use real Azure OpenAI calls (integration tests)
- **Custom assertion extensions**: `AgentResponseHelper` in `Tests/Helpers/` provides fluent assertion chains for `AgentResponse`: `.FunctionCalls()`, `.ShouldContainCall()`, `.ShouldHaveArgumentOfType<T>()`, `.ShouldHaveRequiredInputs()`
- **Test helpers**: `SettingsHelper` loads config from User Secrets, `InfrastructureHelper` creates template repositories, `TravelPlanHelper` builds test messages
- **YAML templates**: Agent prompt templates in `Tests/Templates/` (copied to output via `PreserveNewest`)

## Package Management

Centralized in `Directory.Packages.props` - all NuGet versions are managed there, not in individual .csproj files.
