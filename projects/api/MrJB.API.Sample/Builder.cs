using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace MrJB.API.Sample;

public static class Builder
{
    private const string AppConfigConnectionStringName = "AzureAppConfig";
    private const string AppConfigLabelFilterKey = "AzureAppConfig:LabelFilter";
    private const string AppConfigSentinelKey = "SentinelKey";
    private static readonly TimeSpan AppConfigRefreshInterval = TimeSpan.FromSeconds(30);

    public static IConfigurationBuilder AddJsonOverrides(this IConfigurationBuilder configuration, IWebHostEnvironment environment)
    {
        if (environment.IsEnvironment("Local"))
        {
            configuration.AddJsonFile("appsettings.overrides.json", optional: true, reloadOnChange: true);
        }
        else
        {
            configuration.AddJsonFile("appsettings.overrides.local.json", optional: true, reloadOnChange: true);
        }

        return configuration;
    }

    /// <summary>
    /// Adds Azure App Configuration as a configuration source with dynamic refresh.
    /// Unlabeled keys are loaded first, then keys matching the label filter are layered on top
    /// so labeled values override the defaults.
    /// </summary>
    /// <remarks>
    /// Reads from configuration:
    /// <list type="bullet">
    /// <item><c>ConnectionStrings:AzureAppConfig</c> - the store's connection string. If missing, App Configuration is skipped.</item>
    /// <item><c>AzureAppConfig:LabelFilter</c> - optional label (e.g. "Development"). If missing, only unlabeled keys are loaded.</item>
    /// </list>
    /// Changes to the <c>SentinelKey</c> key (unlabeled) reload all settings, checked at most every 30 seconds.
    /// Call <see cref="UseAzureAppConfigRefresh"/> on the app so refreshes are triggered per request.
    /// </remarks>
    public static TBuilder ConfigureAzureAppConfig<TBuilder>(this TBuilder builder, string? configurationPrefix = null) where TBuilder : IHostApplicationBuilder
    {
        // configuration prefix
        if (!String.IsNullOrWhiteSpace(configurationPrefix))
        {
            // ensure ":" at the end of the prefix so we can append keys to it
            configurationPrefix = configurationPrefix.TrimEnd(':') + ":";
        }

        // default
        if (String.IsNullOrWhiteSpace(configurationPrefix))
        {
            configurationPrefix = String.Empty;
        }

        // vars
        var connectionString = builder.Configuration[$"{configurationPrefix}AppConfigConnectionStringName"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return builder;
        }

        var labelFilter = builder.Configuration[$"{configurationPrefix}AppConfigLabelFilterKey"];

        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(connectionString)
                   .Select(KeyFilter.Any, LabelFilter.Null);

            if (!string.IsNullOrWhiteSpace(labelFilter))
            {
                options.Select(KeyFilter.Any, labelFilter);
            }

            options.ConfigureRefresh(refresh =>
            {
                refresh.Register(AppConfigSentinelKey, refreshAll: true)
                    .SetRefreshInterval(AppConfigRefreshInterval);
            });
        });

        // registers IConfigurationRefresherProvider
        builder.Services.AddAzureAppConfiguration();

        return builder;
    }

    /// <summary>
    /// Triggers an Azure App Configuration refresh on each request (non-blocking).
    /// The provider only contacts the store once the refresh interval has elapsed.
    /// </summary>
    public static TApp UseAzureAppConfigRefresh<TApp>(this TApp app) where TApp : IApplicationBuilder
    {
        // no-op when App Configuration wasn't added (e.g. no connection string locally)
        var refreshers = app.ApplicationServices
            .GetService<IConfigurationRefresherProvider>()?
            .Refreshers;

        if (refreshers is null || !refreshers.Any())
        {
            return app;
        }

        app.Use(async (context, next) =>
        {
            foreach (var refresher in refreshers)
            {
                // fire-and-forget: TryRefreshAsync doesn't throw and is throttled by the refresh interval
                _ = refresher.TryRefreshAsync();
            }

            await next(context);
        });

        return app;
    }
}
