using MrJB.Sample.Domain.Interfaces;
using MrJB.Sample.Domain.Interfaces.App;

namespace MrJB.Sample.Application.Services;

public sealed class AppService(ICustomerServices customers) : IAppService
{
    public ICustomerServices Customers { get; } = customers;
}
