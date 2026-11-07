using Microsoft.Data.SqlClient;
using MrJB.Sample.Domain.Configuration;

namespace MrJB.Sample.Data.Repositories;

public abstract class BaseSqlRepository
{
    /// <summary>
    /// Names of every stored procedure the repositories call.
    /// All data access goes through stored procedures; no inline SQL.
    /// </summary>
    protected static class StoredProcedures
    {
        // customers
        public const string CustomerGetById = "dbo.Customer_GetById";
    }

    protected DatabaseConfiguration DatabaseConfiguration { get; }

    protected BaseSqlRepository(DatabaseConfiguration databaseConfiguration)
    {
        DatabaseConfiguration = databaseConfiguration;
    }

    /// <summary>
    /// New, unopened connection to the primary database. Dapper opens and closes it per call;
    /// connection pooling makes this cheap. Dispose it with <c>await using</c>.
    /// </summary>
    protected SqlConnection CreateConnection() => new(DatabaseConfiguration.Primary);
}
