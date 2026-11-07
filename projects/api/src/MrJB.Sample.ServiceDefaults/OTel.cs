using System.Diagnostics;

namespace MrJB.Sample.ServiceDefaults;

/// <summary>
/// Shared OpenTelemetry names for the sample: the <see cref="ActivitySource"/> used for custom spans,
/// the meter name used by <see cref="SampleMetrics"/>, and the tag (attribute) keys both use.
/// Both names are registered with OpenTelemetry in <c>Extensions.ConfigureOpenTelemetry</c>.
/// </summary>
public static class OTel
{
    public const string ActivitySourceName = "MrJB.Sample";

    public const string MeterName = "MrJB.Sample";

    /// <summary>Source for custom spans. Span names live in <see cref="Spans"/>.</summary>
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    /// <summary>Tag keys. Database keys follow the OpenTelemetry semantic conventions.</summary>
}
