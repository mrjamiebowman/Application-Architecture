using MrJB.Sample.Domain.Models;

namespace MrJB.Sample.Domain.Interfaces.App;

public interface ICustomerServices
{
    Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default);
}
