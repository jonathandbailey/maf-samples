using Microsoft.Agents.AI;

namespace A2A.Client.Tasks;

public class AGUIAgent(AIAgent agent) : DelegatingAIAgent(agent)
{
}