using System.Text.Json;
using AGUI.StateSnapShotEvents;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AGUI;

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