namespace Tools.Registry;

public class ToolGroupNotFoundException(string groupName)
    : Exception($"No handlers registered for group '{groupName}'.")
{
    public string GroupName { get; } = groupName;
}
