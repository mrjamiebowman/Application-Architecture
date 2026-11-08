using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MrJB.Sample.Domain.Configuration;

namespace MrJB.API.Tests.Integration.Infrastructure;

/// <summary>
/// Hosts the real API in memory (TestServer) with its real service registrations: nothing is faked or replaced.
/// Requests go through the full pipeline, controllers, application and data layers to the real SQL Server database.
/// </summary>
/// <remarks>
/// <para>
/// Runs under the <c>IntegrationTests</c> environment, so the API loads <c>appsettings.IntegrationTests.json</c>
/// from this project's output. Environment variables still override it, e.g. in CI:
/// <c>Database__Customers="Server=...;Database=AppArch;..."</c>.
/// </para>
/// <para>
/// On start-up the idempotent scripts in MrJB.Sample.Data/Scripts are applied (creates dbo.Customers if missing and
/// creates or alters the stored procedures). Rows the tests insert through <see cref="SeedCustomerAsync"/> are
/// deleted when the fixture is disposed. The database itself must already exist.
/// </para>
/// </remarks>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string Environment = "IntegrationTests";

    // order matters: the procedures read the tables
    private static readonly string[] SchemaScripts = ["Customers.sql", "uspCustomerGetById.sql"];

    private readonly List<int> _seededCustomerIds = [];

    /// <summary>The connection string the API itself resolved from configuration.</summary>
    public string ConnectionString => Services.GetRequiredService<DatabaseConfiguration>().Customers;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environment);
    }

    public async ValueTask InitializeAsync()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new InvalidOperationException(
                $"Database:Customers is not set. Set it in appsettings.{Environment}.json or the Database__Customers environment variable.");
        }

        await using var connection = await OpenConnectionAsync();

        foreach (var script in SchemaScripts)
        {
            var sql = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Scripts", script));

            await using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }


    // TODO: use Entity Framework for seeding only...

    /// <summary>
    /// Inserts a customer directly into the database and returns its id. The row is deleted when the fixture is disposed.
    /// </summary>
    public async Task<int> SeedCustomerAsync(string firstName, string lastName, string email, string phoneNumber, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO dbo.Customers (FirstName, LastName, Email, PhoneNumber)
            OUTPUT INSERTED.CustomerId
            VALUES (@FirstName, @LastName, @Email, @PhoneNumber);
            """;

        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FirstName", firstName);
        command.Parameters.AddWithValue("@LastName", lastName);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

        var customerId = (int)(await command.ExecuteScalarAsync(cancellationToken))!;

        lock (_seededCustomerIds)
        {
            _seededCustomerIds.Add(customerId);
        }

        return customerId;
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            if (_seededCustomerIds.Count > 0)
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new SqlCommand(
                    $"DELETE FROM dbo.Customers WHERE CustomerId IN ({string.Join(",", _seededCustomerIds)});",
                    connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        finally
        {
            await base.DisposeAsync();
        }
    }

    private async Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}

/// <summary>
/// Shares one <see cref="ApiFactory"/> (one in-memory host, one schema setup) across every test class in the collection.
/// </summary>
[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "Api";
}
