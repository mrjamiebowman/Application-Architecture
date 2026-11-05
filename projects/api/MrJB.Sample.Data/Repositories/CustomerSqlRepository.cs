using MrJB.Sample.Data.Entities;
using MrJB.Sample.Data.Repositories.Interfaces;
using MrJB.Sample.Domain.Configuration;

namespace MrJB.Sample.Data.Repositories;

public class CustomerSqlRepository : BaseSqlRepository, ICustomerRepository
{
    public CustomerSqlRepository(DatabaseConfiguration databaseConfiguration) : base(databaseConfiguration)
    {

    }

    public Task<CustomerEntity> GetCustomerByIdAsync(string customerId)
    {
        throw new NotImplementedException();
    }
}
