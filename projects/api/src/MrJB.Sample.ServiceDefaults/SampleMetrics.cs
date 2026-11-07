using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MrJB.Sample.ServiceDefaults;

/// <summary>
/// Custom metrics for the sample. Register as a singleton; the meter comes from <see cref="IMeterFactory"/>
/// so it's disposed with the container and isolated in tests.
/// </summary>
public sealed class SampleMetrics
{
    private readonly Counter<long> _customerLookups;
    private readonly Histogram<double> _dbQueryDuration;

    public SampleMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(OTel.MeterName);

        _customerLookups = meter.CreateCounter<long>(
            "sample.customer.lookups",
            unit: "{lookup}",
            description: "Customer lookups by result (found, not_found, error).");

        _dbQueryDuration = meter.CreateHistogram<double>(
            "sample.db.query.duration",
            unit: "s",
            description: "Duration of stored procedure calls made by the repositories.");
    }

    public void CustomerLookup(string result) =>
        _customerLookups.Add(1, new KeyValuePair<string, object?>(Spans.Result, result));

    public void DbQueryDuration(TimeSpan elapsed, string storedProcedure, string outcome)
    {
        var tags = new TagList
        {
            { Spans.DbSystem, Spans.Values.SqlServer },
            { Spans.DbStoredProcedure, storedProcedure },
            { Spans.Outcome, outcome },
        };

        _dbQueryDuration.Record(elapsed.TotalSeconds, tags);
    }
}
