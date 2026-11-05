using MrJB.Sample.Domain.Models;

namespace MrJB.Sample.Data;

public interface IDataService
{
    Task<Customer?> GetCustomerByIdAsync(string customerId);
}
