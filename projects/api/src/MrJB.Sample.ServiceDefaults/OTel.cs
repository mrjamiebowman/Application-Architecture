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
    public static class Tags
    {
        public const string CustomerId = "customer.id";

        /// <summary>found | not_found | error</summary>
        public const string LookupResult = "customer.lookup.result";

        /// <summary>success | error</summary>
        public const string Outcome = "outcome";

        public const string DbSystem = "db.system.name";
        public const string DbStoredProcedure = "db.stored_procedure.name";
    }

    public static class Values
    {
        public const string Found = "found";
        public const string NotFound = "not_found";
        public const string Error = "error";
        public const string Success = "success";

        public const string SqlServer = "microsoft.sql_server";
    }
}
