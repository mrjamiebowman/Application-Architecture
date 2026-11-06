using MrJB.Sample.Domain.Interfaces;
using MrJB.Sample.Domain.Models;

namespace MrJB.Sample.Application.Services;

public sealed class CustomerServices : ICustomerServices
{
    public Task<Customer?> GetCustomerByIdAsync(string customerId)
    {
        throw new NotImplementedException();
    }
}
