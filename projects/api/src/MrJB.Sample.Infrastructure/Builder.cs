using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MrJB.Sample.Domain.Interfaces;
using MrJB.Sample.Infrastructure.Configuration;
using MrJB.Sample.Infrastructure.Services;

namespace MrJB.Sample.Infrastructure;

public static class Builder
{
    /// <summary>
    /// Registers infrastructure services: Azure Key Vault and the <see cref="IFeatureService"/> that reads flags from it.
    /// </summary>
    /// <remarks>
    /// Reads <c>KeyVault:Uri</c>. When it's missing, no <see cref="SecretClient"/> is created and every flag is off.
    /// Authenticates with <see cref="DefaultAzureCredential"/> (managed identity in Azure,
    /// Visual Studio / Azure CLI sign-in locally). The identity needs the "Key Vault Secrets User" role.
    /// </remarks>
    public static TBuilder ConfigureInfrastructureLayer<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // configuration
        KeyVaultConfiguration keyVaultConfiguration = new KeyVaultConfiguration();
        builder.Configuration.GetSection(KeyVaultConfiguration.Position).Bind(keyVaultConfiguration);
        builder.Services.AddSingleton(keyVaultConfiguration);

        // key vault (thread-safe, one per app)
        if (!string.IsNullOrWhiteSpace(keyVaultConfiguration.Uri))
        {
            builder.Services.TryAddSingleton(new SecretClient(new Uri(keyVaultConfiguration.Uri), new DefaultAzureCredential()));
        }

        // feature flags
        builder.Services.AddMemoryCache();
        builder.Services.TryAddSingleton<IFeatureService>(sp => new FeatureService(
            sp.GetRequiredService<ILogger<FeatureService>>(),
            sp.GetRequiredService<IMemoryCache>(),
            sp.GetRequiredService<KeyVaultConfiguration>(),
            sp.GetService<SecretClient>()));

        return builder;
    }
}
