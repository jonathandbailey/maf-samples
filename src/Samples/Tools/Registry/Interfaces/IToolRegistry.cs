using Microsoft.Extensions.AI;

namespace Tools.Registry.Interfaces;

public interface IToolRegistry
{
    IToolHandler GetHandler(string toolName);
    List<AITool> GetAllDeclarationOnlyTools();
    List<AITool> GetDeclarationOnlyTools(string group);
}
