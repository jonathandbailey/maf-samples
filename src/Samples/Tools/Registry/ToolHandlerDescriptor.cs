namespace Tools.Registry;

internal sealed record ToolHandlerDescriptor(Type HandlerType, IReadOnlyList<string> Groups);
