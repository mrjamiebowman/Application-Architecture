using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MrJB.Sample.Application.Services;
using MrJB.Sample.Data;
using MrJB.Sample.Domain.Interfaces;
using MrJB.Sample.Domain.Interfaces.App;
using MrJB.Sample.ServiceDefaults;

namespace MrJB.Sample.Application;

public static class Builder
{
    /// <summary>
    /// Registers the application layer and the data layer it depends on.
    /// </summary>
    public static TBuilder ConfigureApplicationLayer<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // data layer
        builder.ConfigureDataLayer();

        // telemetry
        builder.Services.TryAddSingleton<SampleMetrics>();

        // services
        builder.Services.AddTransient<ICustomerServices, CustomerServices>();

        // app service (facade over the services above)
        builder.Services.AddTransient<IAppService, AppService>();

        return builder;
    }
}
