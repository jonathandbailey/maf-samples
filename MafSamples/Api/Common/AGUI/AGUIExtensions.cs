using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Api.Common.AGUI;

public static class AGUIExtensions
{
    private const string ApplicationJsonMediaType = "application/json";


    public static AgentResponseUpdate CreateStatusSnapshotUpdate(string status, string message)
    {
        var statusUpdate = new AGUIStatusUpdate(status, message);
        var stateBytes = JsonSerializer.SerializeToUtf8Bytes(
            new AGUISnapshot<AGUIStatusUpdate>(statusUpdate.Type, statusUpdate));
        return new AgentResponseUpdate
        {
            Contents = [new DataContent(stateBytes, ApplicationJsonMediaType)]
        };
    }
}