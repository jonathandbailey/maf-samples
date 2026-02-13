using Microsoft.Agents.AI;
using Shared.Agents;

namespace TDD.Common.Helpers;

public class AgentFactoryHelper
{
    private const string PlanningYaml = "planning.yaml";

    public static async Task<AIAgent> CreatePlanningAgent()
    {
        var languageModelSettings = SettingsHelper.GetLanguageModelSettings();

        var templateRepository = InfrastructureHelper.Create();

        var agentFactory = new AgentFactory(languageModelSettings);

        var template = await templateRepository.LoadAsync(PlanningYaml);

        var agent = await agentFactory.Create(template, PlanningTools.GetDeclarationOnlyTools());

        return agent;
    }
}