using Microsoft.Extensions.AI;

namespace Tools.Registry;

public abstract record ToolHandlerUpdate;

public sealed record ToolResultUpdate(FunctionResultContent FunctionResultContent) : ToolHandlerUpdate;


public record ToolHandlerContext(Guid ThreadId);
