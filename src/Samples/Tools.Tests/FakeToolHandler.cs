using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using Tools.Registry;

namespace Tools.Tests;

[ToolGroup("test-group")]
internal sealed class FakeToolHandler : IToolHandler
{
    public const string Name = "fake_tool";
    public const string ResultText = "fake_tool_result";

    public string ToolName => Name;

    public async IAsyncEnumerable<ToolHandlerUpdate> ExecuteAsync(
        FunctionCallContent call,
        ToolHandlerContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        yield return new ToolResultUpdate(
            new FunctionResultContent(call.CallId, ResultText));
    }

    public List<AITool> GetDeclarationOnlyTools()
    {
        static string FakeTool() => ResultText;

        var function = AIFunctionFactory.Create(FakeTool, new AIFunctionFactoryOptions
        {
            Name = Name,
            Description = "A fake tool for testing."
        });

        return [function.AsDeclarationOnly()];
    }
}