using Azure;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MrJB.Sample.Domain.Features;
using MrJB.Sample.Domain.Interfaces;
using MrJB.Sample.Infrastructure.Configuration;

namespace MrJB.Sample.Infrastructure.Services;

/// <summary>
/// Reads feature flags from Azure Key Vault secrets named <c>FeatureFlags--{Name}</c>.
/// Values are cached for <see cref="KeyVaultConfiguration.FeatureFlagCacheSeconds"/> so a busy
/// endpoint doesn't call Key Vault (and hit its throttling limits) on every request.
/// When a flag isn't set or can't be read, the caller's default is returned (false unless overridden).
/// </summary>
public sealed partial class FeatureService(
    ILogger<FeatureService> logger,
    IMemoryCache cache,
    KeyVaultConfiguration configuration,
    SecretClient? secretClient = null) : IFeatureService
{
    private const string CacheKeyPrefix = "feature-flag:";

    public async Task<bool> IsEnabledAsync(string featureName, bool defaultValue = false, CancellationToken cancellationToken = default)
    {
        // no vault configured or no name: use the default
        if (secretClient is null || string.IsNullOrWhiteSpace(featureName))
        {
            return defaultValue;
        }

        var cacheKey = CacheKeyPrefix + featureName;

        // the cache holds what Key Vault said (null = not set / unreadable), not the default,
        // so callers passing different defaults for the same flag each get their own
        if (!cache.TryGetValue(cacheKey, out bool? value))
        {
            value = await ReadFlagAsync(secretClient, featureName, cancellationToken);

            var cacheSeconds = configuration.FeatureFlagCacheSeconds;
            if (cacheSeconds > 0)
            {
                cache.Set(cacheKey, value, TimeSpan.FromSeconds(cacheSeconds));
            }
        }

        return value ?? defaultValue;
    }

    /// <summary>Returns the secret's boolean value, or null when it's missing, unreadable or not a valid boolean.</summary>
    private async Task<bool?> ReadFlagAsync(SecretClient client, string featureName, CancellationToken cancellationToken)
    {
        var secretName = FeatureFlags.SecretPrefix + featureName;

        try
        {
            KeyVaultSecret secret = await client.GetSecretAsync(secretName, cancellationToken: cancellationToken);

            if (bool.TryParse(secret.Value?.Trim(), out var enabled))
            {
                return enabled;
            }

            LogFlagInvalid(logger, secretName);
            return null;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            // not set: use the default
            LogFlagNotFound(logger, secretName);
            return null;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // disabled secret, no access, network failure, invalid name: use the default
            LogFlagReadFailed(logger, ex, secretName);
            return null;
        }
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Feature flag secret {SecretName} not found; using the default")]
    private static partial void LogFlagNotFound(ILogger logger, string secretName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Feature flag secret {SecretName} is not \"true\" or \"false\"; using the default")]
    private static partial void LogFlagInvalid(ILogger logger, string secretName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Could not read feature flag secret {SecretName}; using the default")]
    private static partial void LogFlagReadFailed(ILogger logger, Exception exception, string secretName);
}
