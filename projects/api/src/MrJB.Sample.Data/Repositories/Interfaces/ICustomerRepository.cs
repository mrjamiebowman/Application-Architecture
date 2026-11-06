using MrJB.Sample.Data.Entities;

namespace MrJB.Sample.Data.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerEntity?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default);
}
