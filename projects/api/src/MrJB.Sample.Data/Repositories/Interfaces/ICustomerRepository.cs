using MrJB.Sample.Data.Entities;

namespace MrJB.Sample.Data.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerEntity?> GetCustomerByIdAsync(string customerId);
}
