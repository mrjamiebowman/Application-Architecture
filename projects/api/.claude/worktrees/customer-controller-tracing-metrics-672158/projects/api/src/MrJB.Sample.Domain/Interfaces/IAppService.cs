using MrJB.Sample.Domain.Interfaces.App;

namespace MrJB.Sample.Domain.Interfaces;

/// <summary>
/// Single entry point to the application layer. Controllers depend on this and reach
/// each feature's services through it, e.g. <c>AppService.Customers.GetCustomerByIdAsync(...)</c>.
/// </summary>
public interface IAppService
{
    ICustomerServices Customers { get; }
}
