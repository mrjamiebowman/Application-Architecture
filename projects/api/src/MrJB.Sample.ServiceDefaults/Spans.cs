namespace MrJB.Sample.ServiceDefaults;

/// <summary>
/// Custom span names, started from <see cref="OTel.ActivitySource"/>.
/// The HTTP server span and the SQL client span come from instrumentation and aren't listed here.
/// </summary>
public static class Spans
{
    // application layer
    public const string CustomerServicesGetById = "CustomerServices.GetCustomerById";

    // data layer
    public const string CustomerRepositoryGetById = "CustomerSqlRepository.GetCustomerById";
}
