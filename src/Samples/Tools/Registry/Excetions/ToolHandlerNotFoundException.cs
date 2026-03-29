namespace Tools.Registry.Excetions;

public class ToolHandlerNotFoundException(string toolName)
    : Exception($"No handler registered for tool '{toolName}'.")
{
    public string ToolName { get; } = toolName;
}
