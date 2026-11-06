using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrJB.Sample.Data.Repositories;
using MrJB.Sample.Data.Repositories.Interfaces;
using MrJB.Sample.Domain.Configuration;
using MrJB.Sample.Domain.Interfaces;

namespace MrJB.Sample.Data;

public static class Builder
{
    //public static TBuilder ConfigureAzureAppConfig<TBuilder>(this TBuilder builder, string? configurationPrefix = null) where TBuilder : IHostApplicationBuilder
    //{

    //}

    public static TBuilder ConfigureDataLayer<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // configuration
        DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration();
        builder.Configuration.GetSection(DatabaseConfiguration.Position).Bind(databaseConfiguration);
        builder.Services.AddSingleton(databaseConfiguration);

        // repositories
        builder.Services.AddTransient<ICustomerRepository, CustomerSqlRepository>();

        // services
        builder.Services.AddTransient<IDataService, DataService>();

        return builder;
    }
}
