using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shared.Infrastructure;
using Shared.Settings;

namespace Tests;

public class Test1
{
    private const string AgentTemplateFolder = "Templates";
    private const string PlanningYaml = "planning.yaml";

    [Fact]
    public async Task LoadTemplate()
    {
        var fileStorageSettings = Options.Create(new FileStorageSettings
        {
            AgentTemplateFolder = AgentTemplateFolder,
            
        });

        var mockLogger = new Mock<ILogger<AgentTemplateRepository>>();

        var templateRepository = new AgentTemplateRepository(mockLogger.Object, fileStorageSettings);

        var template = await templateRepository.LoadAsync(PlanningYaml);

        template.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RunAgent()
    {

    }
}
