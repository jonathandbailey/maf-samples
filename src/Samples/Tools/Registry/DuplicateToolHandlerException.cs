namespace Tools.Registry;

public class DuplicateToolHandlerException(string toolName)
    : Exception($"A handler for tool '{toolName}' has already been registered.")
{
    public string ToolName { get; } = toolName;
}
