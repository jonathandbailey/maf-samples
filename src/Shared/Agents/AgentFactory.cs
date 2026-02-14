using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using Shared.Settings;

namespace Shared.Agents;

public class AgentFactory : IAgentFactory
{
    private readonly ChatClient _chatClient;

    public AgentFactory(IOptions<LanguageModelSettings> settings)
    {
        var credential = new ChainedTokenCredential(
            new VisualStudioCredential(),
            new AzureCliCredential(),
            new AzureDeveloperCliCredential()
        );

        _chatClient = new AzureOpenAIClient(new Uri(settings.Value.EndPoint), credential)
            .GetChatClient(settings.Value.DeploymentName);
    }

    private const string AgentInstructions = "You are a helpful assistant that answers questions.Use the tools provided to assist with your tasks.";
    private const string AssistantName = "Assistant";

    public async Task<AIAgent> Create(List<AITool>? tools = null)
    {
        

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

        var agent = _chatClient.AsIChatClient()
            .AsBuilder()
            .BuildAIAgent(options: clientChatOptions);

        return await Task.FromResult(agent);
    }

    public async Task<AIAgent> Create(string template, List<AITool>? tools = null)
    {

        var agentFactory = new CustomPromptAgentFactory(_chatClient.AsIChatClient(), tools: tools);
        var agent = await agentFactory.CreateFromYamlAsync(template);

        return agent;
    }

    public async Task<AIAgent> Create(IChatClient chatClient, string template, List<AITool>? tools = null)
    {

        var agentFactory = new CustomPromptAgentFactory(chatClient, tools: tools);
        var agent = await agentFactory.CreateFromYamlAsync(template);

        return agent;
    }
}

public interface IAgentFactory
{
    Task<AIAgent> Create(List<AITool>? tools = null);
    Task<AIAgent> Create(string template, List<AITool>? tools = null);
    Task<AIAgent> Create(IChatClient chatClient, string template, List<AITool>? tools = null);
}