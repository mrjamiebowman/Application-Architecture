namespace MrJB.Sample.Domain.Interfaces;

/// <summary>
/// Reads feature flags from Azure Key Vault. Each flag is a secret named
/// <c>FeatureFlags--{Name}</c> whose value is "true" or "false".
/// A flag that is missing, disabled, unreadable or not a valid boolean returns the caller's default.
/// </summary>
public interface IFeatureService
{
    /// <summary>
    /// Returns the flag's value from Key Vault, or <paramref name="defaultValue"/> (false unless overridden)
    /// when the flag isn't set or can't be read.
    /// </summary>
    Task<bool> IsEnabledAsync(string featureName, bool defaultValue = false, CancellationToken cancellationToken = default);
}
