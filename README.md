# Welcome to Microsoft Agent Framework Samples

Welcome to this collection of **Microsoft Agent Framework** samples.The primary objective of this repository is to build upon and extend the official framework examples, providing deeper dives into specific implementation patterns and architectural designs.

## Tech Stack
- .NET 10
- C#
- .NET Aspire

## Samples

### AG-UI State Snapshot Events
This examples demonstrates how to publish AG-UI status update events while streaming from an LLM.

**Key Features**
- Extending an Agent using DelegatingAgent
- Using DataContent (JSON) to return AG-UI STATE_SNAPSHOT events.


You can view the full doc and sample here : 

## Getting Started

### Azure Open AI Configuration Settings
The samples are built against an Azure Open AI resource so DeploymentName, Endpoint, and ApiKey are required in the appSettings.Development.json

```json
"LanguageModelSettings": {
  "DeploymentName": "",
  "Endpoint": "",
  "ApiKey": ""
}