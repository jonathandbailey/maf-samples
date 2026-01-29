# AG-UI State Snapshot Events

This sample shows how to return AG-UI STATE_SNAPSHOT events while streaming response from an LLM/Agent using the AG-UI protocol.

## Key Features
- Creating a custom agent using a DelegatingAgent and wiring it to ASP.NET using the MAF AG-UI extensions.
- Returning a custom status update inside the Agent enumeration stream while the Server-Sent-Events connection is open.