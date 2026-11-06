using Microsoft.Extensions.Logging;
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

    public async Task<Customer?> GetCustomerByIdAsync(string customerId)
    {
        var data = await CustomerRepository.GetCustomerByIdAsync(customerId);

        // map to domain model
        Customer? model = null;

        return model;
    }

    // save

    // delete

    // search
}
