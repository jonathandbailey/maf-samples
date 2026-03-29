using Tools.Registry.Interfaces;

namespace Tools.Registry;

public record ToolHandlerRegistration(IToolHandler Handler, IReadOnlyList<string> Groups);
