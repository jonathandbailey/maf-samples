namespace Shared.Settings;

public class LanguageModelSettings
{
    public required string DeploymentName { get; init; } 

    public required string EndPoint { get; init; }

    public required string ApiKey { get; init; }
}