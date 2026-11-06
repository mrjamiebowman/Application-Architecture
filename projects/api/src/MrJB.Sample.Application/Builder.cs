using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrJB.Sample.Application.Services;
using MrJB.Sample.Data;
using MrJB.Sample.Domain.Configuration;
using MrJB.Sample.Domain.Interfaces;

namespace MrJB.Sample.Application;

public static class Builder
{
    //public static TBuilder ConfigureAzureAppConfig<TBuilder>(this TBuilder builder, string? configurationPrefix = null) where TBuilder : IHostApplicationBuilder
    //{

    //}

    public static TBuilder ConfigureDataLayer<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // configuration
        //builder.Services.Configure(builder.Configuration.GetSection(DatabaseConfiguration.Position));

        // repositories
        builder.Services.AddTransient<ICustomerServices, CustomerServices>();

        // services
        builder.Services.AddTransient<IDataService, DataService>();

        return builder;
    }
}
