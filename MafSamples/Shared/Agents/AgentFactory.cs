using System.ClientModel;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace Shared.Agents;

public class AgentFactory(IOptions<LanguageModelSettings> settings) : IAgentFactory
{
    private const string AgentInstructions = "You are a helpful assistant that answers questions.";
    private const string AssistantName = "Assistant";

    public async Task<AIAgent> Create(List<AITool>? tools = null)
    {
        var chatClient = new AzureOpenAIClient(new Uri(settings.Value.EndPoint),
                new ApiKeyCredential(settings.Value.ApiKey))
            .GetChatClient(settings.Value.DeploymentName);

        ChatOptions chatOptions = new()
        {
            Instructions = AgentInstructions,
            Tools = tools
        };

        var clientChatOptions = new ChatClientAgentOptions
        {
            Name = AssistantName,

            ChatOptions = chatOptions
        };

        var agent = chatClient.AsIChatClient()
            .AsBuilder()
            .BuildAIAgent(options: clientChatOptions);

        return await Task.FromResult(agent);
    }
}

public interface IAgentFactory
{
    Task<AIAgent> Create(List<AITool>? tools = null);
}