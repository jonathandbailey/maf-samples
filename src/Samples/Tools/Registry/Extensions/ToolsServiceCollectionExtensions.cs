using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tools.Registry.Interfaces;

namespace Tools.Registry.Extensions;

public static class ToolsServiceCollectionExtensions
{
    public static IServiceCollection AddTools(
        this IServiceCollection services,
        params Type[] markerTypes)
    {
        ArgumentNullException.ThrowIfNull(markerTypes);

        if (markerTypes.Length == 0)
            throw new ArgumentException(
                "At least one marker type must be provided.", nameof(markerTypes));

        return services.AddTools(markerTypes.Select(t => t.Assembly).Distinct().ToArray());
    }

    private static IServiceCollection AddTools(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (services.Any(d => d.ServiceType == typeof(IToolRegistry)))
            throw new InvalidOperationException(
                $"{nameof(AddTools)} has already been called on this service collection. " +
                "Call it once with all required marker types.");

        var handlerInterface = typeof(IToolHandler);

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && handlerInterface.IsAssignableFrom(t)))
            {
                var groups = type.GetCustomAttributes<ToolGroupAttribute>()
                    .Select(a => a.Group)
                    .ToList();

                foreach (var group in groups)
                {
                    if (string.IsNullOrWhiteSpace(group))
                        throw new ArgumentException(
                            $"Handler '{type.FullName}' has a [ToolGroup] attribute with a null, empty, or whitespace group name.",
                            nameof(assemblies));
                }

                services.AddTransient(type);
                services.AddSingleton(new ToolHandlerDescriptor(type, groups));
            }
        }

        services.AddSingleton<IToolRegistry>(sp =>
        {
            var registrations = sp.GetServices<ToolHandlerDescriptor>()
                .Select(d => new ToolHandlerRegistration(
                    (IToolHandler)sp.GetRequiredService(d.HandlerType),
                    d.Groups));

            return new ToolRegistry(registrations);
        });

        return services;
    }
}
