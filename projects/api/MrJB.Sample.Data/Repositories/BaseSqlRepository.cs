using MrJB.Sample.Domain.Configuration;

namespace MrJB.Sample.Data.Repositories;

public abstract class BaseSqlRepository
{
    protected DatabaseConfiguration DatabaseConfiguration { get; }

    public BaseSqlRepository(DatabaseConfiguration databaseConfiguration)
    {
        DatabaseConfiguration = databaseConfiguration;
    }
}
