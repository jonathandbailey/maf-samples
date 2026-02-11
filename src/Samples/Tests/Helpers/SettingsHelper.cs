using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace TDD.Helpers;

public static class SettingsHelper
{
    private const string LanguageModelSettingsDeploymentName = "LanguageModelSettings:DeploymentName";
    private const string LanguageModelSettingsEndpoint = "LanguageModelSettings:EndPoint";

    public static IOptions<LanguageModelSettings> GetLanguageModelSettings()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<PlanningAgentTests>()
            .Build();

        var deploymentName = configuration[LanguageModelSettingsDeploymentName];

        var endpoint = configuration[LanguageModelSettingsEndpoint];

        ArgumentException.ThrowIfNullOrEmpty(endpoint, LanguageModelSettingsEndpoint);
        ArgumentException.ThrowIfNullOrEmpty(deploymentName, LanguageModelSettingsDeploymentName);

        var languageModelSettings = Options.Create(new LanguageModelSettings
        {
            DeploymentName = deploymentName,
            EndPoint = endpoint,
        });

        return languageModelSettings;
    }
}