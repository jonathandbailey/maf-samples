namespace Tools.Registry.Excetions;

public class ToolGroupNotFoundException(string groupName)
    : Exception($"No handlers registered for group '{groupName}'.")
{
    public string GroupName { get; } = groupName;
}
