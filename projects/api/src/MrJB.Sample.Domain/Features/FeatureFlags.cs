namespace MrJB.Sample.Domain.Features;

/// <summary>
/// Feature flag names. In Key Vault, create a secret named <c>FeatureFlags--{Name}</c>
/// with a value of "true" or "false", e.g. <c>FeatureFlags--NewCheckout</c>.
/// </summary>
public static class FeatureFlags
{
    /// <summary>Prefix of every feature flag secret in Key Vault.</summary>
    public const string SecretPrefix = "FeatureFlags--";

    // add flag names here, e.g.:
    // public const string NewCheckout = "NewCheckout";
}
