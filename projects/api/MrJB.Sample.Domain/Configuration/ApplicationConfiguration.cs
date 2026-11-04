using TerribleSettingsAuditor.Abstractions;

namespace MrJB.Sample.Domain.Configuration;

[Luggage("Application Configuration", Pinned = 1)]
public class ApplicationConfiguration
{
    public const string Position = "Application";

    [LuggageItem(Secret = true, Expose = ExposeMethod.Padded, ShowLeft = 4)]
    public string? UpKey { get; set; }
}
