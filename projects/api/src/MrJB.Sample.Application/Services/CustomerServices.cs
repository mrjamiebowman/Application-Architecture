using Microsoft.Extensions.Logging;
using MrJB.Sample.Domain.Interfaces;
using MrJB.Sample.Domain.Interfaces.App;
using MrJB.Sample.Domain.Models;
using MrJB.Sample.ServiceDefaults;
using System.Diagnostics;

namespace MrJB.Sample.Application.Services;

public sealed partial class CustomerServices(
    ILogger<CustomerServices> logger,
    IDataService dataService,
    SampleMetrics metrics) : ICustomerServices
{
    public async Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        using var activity = OTel.ActivitySource.StartActivity($"{nameof(CustomerServices)}.{nameof(GetCustomerByIdAsync)}");
        activity?.SetTag(Spans.CustomerId, customerId);

        LogGettingCustomer(logger, customerId);

        try
        {
            var customer = await dataService.GetCustomerByIdAsync(customerId, cancellationToken);

            var result = customer is null ? Spans.Values.NotFound : Spans.Values.Found;
            activity?.SetTag(Spans.Result, result);
            metrics.CustomerLookup(result);

            if (customer is null)
            {
                LogCustomerNotFound(logger, customerId);
            }

            return customer;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // the repository already logged the failure with its exception; record it on this span and count it
            activity?.SetTag(Spans.Result, Spans.Values.Error);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            metrics.CustomerLookup(Spans.Values.Error);
            throw;
        }
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting customer {CustomerId}")]
    private static partial void LogGettingCustomer(ILogger logger, int customerId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Customer {CustomerId} was not found")]
    private static partial void LogCustomerNotFound(ILogger logger, int customerId);
}
