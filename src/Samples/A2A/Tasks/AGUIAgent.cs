using Microsoft.Agents.AI;

namespace A2A.Tasks;

public class AGUIAgent(AIAgent agent) : DelegatingAIAgent(agent)
{
}