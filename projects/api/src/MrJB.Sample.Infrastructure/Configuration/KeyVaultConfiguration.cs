using TerribleSettingsAuditor.Abstractions;

namespace MrJB.Sample.Infrastructure.Configuration;

[Luggage("Key Vault Configuration")]
public class KeyVaultConfiguration
{
    public const string Position = "KeyVault";

    /// <summary>Vault address, e.g. "https://my-vault.vault.azure.net/". When empty, every feature flag is off.</summary>
    [LuggageItem(Expose = ExposeMethod.Full)]
    public string? Uri { get; set; }

    /// <summary>How long a flag's value is cached before Key Vault is asked again. Defaults to 60 seconds.</summary>
    [LuggageItem(Expose = ExposeMethod.Full)]
    public int FeatureFlagCacheSeconds { get; set; } = 60;
}
