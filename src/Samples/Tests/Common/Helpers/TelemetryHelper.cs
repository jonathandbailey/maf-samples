using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TDD.Common.Helpers;

public static class TelemetryHelper 
{
    private static TracerProvider? _tracerProvider;

    public static void Initialize()
    {
  
        var otlpEndpoint = "https://localhost:21291";

        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("TDD"))
            .AddSource("TDD*")
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                options.Headers = "x-otlp-api-key=537f36931ab1e7b3e3a919d4cc7ccb87";

            })
            .Build();
    }

    public static void Dispose()
    {
        _tracerProvider?.Dispose();
    }
}