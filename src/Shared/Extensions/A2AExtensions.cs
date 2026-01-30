using System.Text;
using A2A;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Shared.Extensions;

public static class A2AExtensions
{
    public static List<ChatMessage> ExtractTextPartsFromMessageHistory(this AgentTask agentTask)
    {
        if (agentTask.History == null || agentTask.History.Count == 0)
        {
            throw new InvalidOperationException("AgentTask history is null or empty.");
        }

        var chatMessages = new List<ChatMessage>();

        foreach (var agentMessage in agentTask.History)
        {
            var parts = agentMessage.Parts.OfType<TextPart>().ToList();

            var content = new List<AIContent>();
            
            foreach (var textPart in parts)
            {
                content.Add(new TextContent(textPart.Text));
            }

            chatMessages.Add(new ChatMessage(agentMessage.Role.ToChatRole(), content));
        }

        if (chatMessages.Count == 0)
        {
            throw new InvalidOperationException("No valid chat messages could be extracted from the AgentTask history.");
        }

        return chatMessages;
    }

    public static string ExtractTextPartsFromMessage(this AgentTask agentTask)
    {
        if (agentTask.Status.Message == null)
        {
            throw new InvalidOperationException("AgentTask Status Message is null.");
        }

        if (agentTask.Status.Message.Parts.Count == 0)
        {
            throw new InvalidOperationException("AgentTask Status Message Parts is empty.");
        }

        var parts = agentTask.Status.Message.Parts.OfType<TextPart>().ToList();

        var stringBuilder = new StringBuilder();

        foreach (var textPart in parts)
        {
            stringBuilder.Append(textPart.Text);
        }

        return stringBuilder.ToString();
    }

    public static List<TextPart> ExtractChatMessageTextFromAgentResponse(this AgentResponse response)
    {
        if (response.Messages == null || response.Messages.Count == 0)
        {
            throw new InvalidOperationException("AgentResponse messages are null or empty.");
        }

        var textParts = new List<TextPart>();

        foreach (var chatMessage in response.Messages)
        {
            foreach (var content in chatMessage.Contents)
            {
                if (content is TextContent textContent)
                {
                    textParts.Add(new TextPart { Text = textContent.Text });
                }
            }
        }

        if (textParts.Count == 0)
        {
            throw new InvalidOperationException("No valid text parts could be extracted from the AgentResponse messages.");
        }

        return textParts;
    }

    private static ChatRole ToChatRole(this MessageRole role) => role switch
    {
        MessageRole.User => ChatRole.User,
        MessageRole.Agent => ChatRole.Assistant,
       
        _ => throw new ArgumentOutOfRangeException(nameof(role), $"Unsupported MessageRole: {role}")
    };
}