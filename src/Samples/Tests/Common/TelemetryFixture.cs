using TDD.Common.Helpers;

namespace TDD.Common;

/// <summary>
/// Shared test fixture for initializing telemetry once per test class.
/// Use with IClassFixture&lt;TelemetryFixture&gt; to ensure telemetry is initialized
/// once for all tests in a class, rather than per test method.
/// </summary>
public class TelemetryFixture : IDisposable
{
    public TelemetryFixture()
    {
        TelemetryHelper.Initialize(SettingsHelper.GetAspireDashboardSettings());
    }

    public void Dispose()
    {
        TelemetryHelper.Dispose();
    }
}
