using MrJB.Sample.Domain.Models;

namespace MrJB.Sample.Domain.Interfaces;

public interface ICustomerServices
{
    Task<Customer?> GetCustomerByIdAsync(string customerId);
}
