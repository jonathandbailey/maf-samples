using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shared.Infrastructure;
using Shared.Settings;

namespace TDD.Common.Helpers;

public static class InfrastructureHelper
{
    private const string AgentTemplateFolder = "Common/Templates";

    public static IAgentTemplateRepository Create()
    {
        var fileStorageSettings = Options.Create(new FileStorageSettings
        {
            AgentTemplateFolder = AgentTemplateFolder,

        });

        var mockLogger = new Mock<ILogger<AgentTemplateRepository>>();

        var templateRepository = new AgentTemplateRepository(mockLogger.Object, fileStorageSettings);

        return templateRepository;
    }
}