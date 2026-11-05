using System.ComponentModel.DataAnnotations;
using TerribleSettingsAuditor.Abstractions;

namespace MrJB.Sample.Domain.Configuration;

[Luggage("Application Configuration", Pinned = true)]
public class ApplicationConfiguration
{
    public const string Position = "Application";

    /// <summary>Configuration path of <see cref="UpKey"/> ("Application:UpKey").</summary>
    public const string UpKeyPath = $"{Position}:{nameof(UpKey)}";

    [Required]
    [LuggageItem(Secret = true, Expose = ExposeMethod.Padded, ShowLeft = 4)]
    public string? UpKey { get; set; }
}
