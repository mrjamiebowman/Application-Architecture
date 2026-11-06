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

    /// <summary>Maps the OpenAPI document (/openapi/v1.json) and the Scalar UI (/scalar). Off unless set.</summary>
    [LuggageItem(Expose = ExposeMethod.Full)]
    public bool ShowOpenApiDocs { get; set; }
}
