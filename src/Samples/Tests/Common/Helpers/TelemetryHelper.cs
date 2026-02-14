using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TDD.Common.Settings;

namespace TDD.Common.Helpers;

public static class TelemetryHelper
{
    private static TracerProvider? _tracerProvider;

    public static void Initialize(IOptions<AspireDashboardSettings> settings)
    {
        var dashboardSettings = settings.Value;

        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("TDD"))
            .AddSource("TDD*")
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(dashboardSettings.OtlpEndpoint);
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                options.Headers = $"x-otlp-api-key={dashboardSettings.OtlpApiKey}";
            })
            .Build();
    }

    public static void Dispose()
    {
        _tracerProvider?.Dispose();
    }
}
