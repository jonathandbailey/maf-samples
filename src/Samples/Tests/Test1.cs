using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shared.Agents;
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
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Test1>()
            .Build();

        var languageModelSettings = Options.Create(new LanguageModelSettings
        {
            DeploymentName = configuration["LanguageModelSettings:DeploymentName"] ?? string.Empty,
            EndPoint = configuration["LanguageModelSettings:EndPoint"] ?? string.Empty,
        });

        var agentFactory = new AgentFactory(languageModelSettings);

        var fileStorageSettings = Options.Create(new FileStorageSettings
        {
            AgentTemplateFolder = AgentTemplateFolder,

        });

        var mockLogger = new Mock<ILogger<AgentTemplateRepository>>();

        var templateRepository = new AgentTemplateRepository(mockLogger.Object, fileStorageSettings);

        var template = await templateRepository.LoadAsync(PlanningYaml);

        var agent = await agentFactory.Create(template);

        var response = await agent.RunAsync("What is the capital of France?");


    }
}
