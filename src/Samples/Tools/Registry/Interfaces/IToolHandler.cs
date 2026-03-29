using Microsoft.Extensions.AI;

namespace Tools.Registry.Interfaces;

public interface IToolHandler
{
    string ToolName { get; }

    IAsyncEnumerable<ToolHandlerUpdate> ExecuteAsync(
        FunctionCallContent call,
        ToolHandlerContext context,
        CancellationToken cancellationToken);

    List<AITool> GetDeclarationOnlyTools();
}
