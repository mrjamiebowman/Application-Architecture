using Microsoft.Extensions.Logging;
using MrJB.Sample.Data.Mappers;
using MrJB.Sample.Data.Repositories.Interfaces;
using MrJB.Sample.Domain.Interfaces;
using MrJB.Sample.Domain.Models;

namespace MrJB.Sample.Data;

public sealed class DataService(ILogger<DataService> logger, ICustomerRepository customerRepository) : IDataService
{
    // logging
    private ILogger<DataService> Logger { get; } = logger;

    // repositories
    private ICustomerRepository CustomerRepository { get; } = customerRepository;

    public async Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var entity = await CustomerRepository.GetCustomerByIdAsync(customerId, cancellationToken);

        // map to domain model
        return entity?.ToModel();
    }

    // save

    // delete

    // search
}
