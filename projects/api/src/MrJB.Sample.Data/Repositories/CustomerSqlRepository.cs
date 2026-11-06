using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MrJB.Sample.Data.Entities;
using MrJB.Sample.Data.Repositories.Interfaces;
using MrJB.Sample.Domain.Configuration;
using MrJB.Sample.ServiceDefaults;
using System.Data;
using System.Diagnostics;

namespace MrJB.Sample.Data.Repositories;

public sealed partial class CustomerSqlRepository(
    ILogger<CustomerSqlRepository> logger,
    DatabaseConfiguration databaseConfiguration,
    SampleMetrics metrics) : BaseSqlRepository(databaseConfiguration), ICustomerRepository
{
    public async Task<CustomerEntity?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        const string storedProcedure = StoredProcedures.CustomerGetById;

        // parent span for the call; the SqlClient instrumentation adds the client span beneath it
        using var activity = OTel.ActivitySource.StartActivity(Spans.CustomerRepositoryGetById);
        activity?.SetTag(OTel.Tags.CustomerId, customerId);
        activity?.SetTag(OTel.Tags.DbSystem, OTel.Values.SqlServer);
        activity?.SetTag(OTel.Tags.DbStoredProcedure, storedProcedure);

        var started = Stopwatch.GetTimestamp();
        var outcome = OTel.Values.Success;

        try
        {
            await using var connection = new SqlConnection(DatabaseConfiguration.Customers);

            var command = new CommandDefinition(
                storedProcedure,
                new { CustomerId = customerId },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<CustomerEntity>(command);
        }
        catch (Exception ex)
        {
            outcome = OTel.Values.Error;
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            LogQueryFailed(logger, ex, storedProcedure, customerId);
            throw;
        }
        finally
        {
            var elapsed = Stopwatch.GetElapsedTime(started);
            metrics.DbQueryDuration(elapsed, storedProcedure, outcome);
            LogQueryCompleted(logger, storedProcedure, customerId, outcome, elapsed.TotalMilliseconds);
        }
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "{StoredProcedure} for customer {CustomerId} finished ({Outcome}) in {ElapsedMs:0.0} ms")]
    private static partial void LogQueryCompleted(ILogger logger, string storedProcedure, int customerId, string outcome, double elapsedMs);

    [LoggerMessage(Level = LogLevel.Error, Message = "{StoredProcedure} for customer {CustomerId} failed")]
    private static partial void LogQueryFailed(ILogger logger, Exception exception, string storedProcedure, int customerId);
}
