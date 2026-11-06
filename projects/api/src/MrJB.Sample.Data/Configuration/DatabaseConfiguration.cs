using System.ComponentModel.DataAnnotations;
using TerribleSettingsAuditor.Abstractions;

namespace MrJB.Sample.Domain.Configuration;

[Luggage(Description = "Database configuration settings for the application.")]
public class DatabaseConfiguration
{
    public const string Position = "Database";

    /// <summary>
    /// Gets or sets the Customers database connection string.
    /// </summary>
    [Required]
    [LuggageItem(Description = "Customers Database Connection String", Secret = true, Expose = ExposeMethod.Padded, ShowLeft = 25)]
    public string Customers { get; set; } = string.Empty;
}
