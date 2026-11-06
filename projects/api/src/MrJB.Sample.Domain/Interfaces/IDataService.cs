using MrJB.Sample.Domain.Models;

namespace MrJB.Sample.Domain.Interfaces;

public interface IDataService
{
    Task<Customer?> GetCustomerByIdAsync(string customerId);
}
