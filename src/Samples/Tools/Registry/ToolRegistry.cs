using Microsoft.Extensions.AI;

namespace Tools.Registry;

public sealed class ToolRegistry : IToolRegistry
{
    private readonly Dictionary<string, IToolHandler> _handlers;
    private readonly Dictionary<string, List<IToolHandler>> _groups;

    public ToolRegistry(IEnumerable<IToolHandler> handlers)
        : this(handlers.Select(h => new ToolHandlerRegistration(h, [])))
    {
    }

    public ToolRegistry(IEnumerable<ToolHandlerRegistration> registrations)
    {
        _handlers = new(StringComparer.OrdinalIgnoreCase);
        _groups = new(StringComparer.OrdinalIgnoreCase);

        foreach (var reg in registrations)
        {
            if (!_handlers.TryAdd(reg.Handler.ToolName, reg.Handler))
                throw new DuplicateToolHandlerException(reg.Handler.ToolName);

            foreach (var group in reg.Groups)
            {
                if (!_groups.TryGetValue(group, out var list))
                    _groups[group] = list = [];

                list.Add(reg.Handler);
            }
        }
    }

    public IToolHandler GetHandler(string toolName)
    {
        if (!_handlers.TryGetValue(toolName, out var handler))
            throw new ToolHandlerNotFoundException(toolName);

        return handler;
    }

    public List<AITool> GetAllDeclarationOnlyTools()
    {
        return [.. _handlers.Values.SelectMany(h => h.GetDeclarationOnlyTools())];
    }

    public List<AITool> GetDeclarationOnlyTools(string group)
    {
        if (!_groups.TryGetValue(group, out var handlers))
            throw new ToolGroupNotFoundException(group);

        return [.. handlers.SelectMany(h => h.GetDeclarationOnlyTools())];
    }
       
}
