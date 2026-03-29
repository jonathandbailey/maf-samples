using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Tools.Registry;
using Tools.Registry.Interfaces;

namespace Tools.Tests
{
    public class ToolRegistryTests
    {
        [Fact]
        public async Task GetHandler_WhenCalledWithRegisteredToolName_ExecutesAndReturnsExpectedResult()
        {
            var services = new ServiceCollection();
            services.AddTools(typeof(FakeToolHandler));
            var provider = services.BuildServiceProvider();

            var registry = provider.GetRequiredService<IToolRegistry>();

            var handler = registry.GetHandler(FakeToolHandler.Name);

            handler.Should().NotBeNull();
            handler.ToolName.Should().Be(FakeToolHandler.Name);

            var call = new FunctionCallContent(
                callId: "test-call-id",
                name: FakeToolHandler.Name,
                arguments: null);

            var context = new ToolHandlerContext(Guid.NewGuid());

            var results = new List<ToolHandlerUpdate>();
            await foreach (var update in handler.ExecuteAsync(call, context, CancellationToken.None))
            {
                results.Add(update);
            }

            results.Should().HaveCount(1);

            var toolResult = results[0].Should().BeOfType<ToolResultUpdate>().Subject;
            
            toolResult.FunctionResultContent.CallId.Should().Be("test-call-id");
            toolResult.FunctionResultContent.Result.Should().Be(FakeToolHandler.ResultText);
        }

        [Fact]
        public void GetDeclarationOnlyTools_WhenCalledWithTestGroup_ReturnsFakeToolDeclaration()
        {
            var services = new ServiceCollection();
            services.AddTools(typeof(FakeToolHandler));
         
            var provider = services.BuildServiceProvider();

            var registry = provider.GetRequiredService<IToolRegistry>();

            var tools = registry.GetDeclarationOnlyTools("test-group");

            tools.Should().NotBeEmpty();
            tools.Should().ContainSingle(t => t.Name == FakeToolHandler.Name);
        }
    }
}
