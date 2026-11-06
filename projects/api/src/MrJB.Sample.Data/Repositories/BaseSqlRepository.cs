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
        public const string Scheme = "[dbo]";

        // customers
        public const string CustomerGetById = $"{Scheme}.[uspCustomerGetById]";
    }

    protected DatabaseConfiguration DatabaseConfiguration { get; }

    protected BaseSqlRepository(DatabaseConfiguration databaseConfiguration)
    {
        DatabaseConfiguration = databaseConfiguration;
    }
}
