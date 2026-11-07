using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrJB.API.Sample.Filters;

/// <summary>
/// Requires the <c>?key=</c> query string value to match the value of the configuration setting at <see cref="ConfigurationKey"/>.
/// Returns 404 when the key is missing, wrong, or the setting has no value, so the endpoint appears not to exist.
/// </summary>
/// <example>
/// <code>[RequireKey(ApplicationConfiguration.UpKeyPath)]</code>
/// <code>[RequireKey("Diagnostics:LogKey")]</code>
/// </example>
/// <remarks>
/// This only validates the key. Pair it with <c>[AllowAnonymous]</c> so authorization policies don't block monitors.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireKeyAttribute(string configurationKey) : Attribute, IAuthorizationFilter
{
    public const string QueryKey = "key";

    /// <summary>The configuration path that holds the expected key, e.g. "Application:UpKey".</summary>
    public string ConfigurationKey { get; } = !string.IsNullOrWhiteSpace(configurationKey)
        ? configurationKey
        : throw new ArgumentException("A configuration key is required.", nameof(configurationKey));

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // IConfiguration is read per request, so reloads (e.g. Azure App Configuration refresh) apply immediately
        var expected = context.HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()[ConfigurationKey];

        // exactly one ?key= value; repeated keys (?key=a&key=b) are rejected
        var values = context.HttpContext.Request.Query[QueryKey];
        var supplied = values.Count == 1 ? values[0] : null;

        if (!IsMatch(expected, supplied))
        {
            context.Result = new NotFoundResult();
        }
    }

    private static bool IsMatch(string? expected, string? supplied)
    {
        // no key configured = locked, never open
        if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(supplied))
        {
            return false;
        }

        // constant-time compare so response timing doesn't leak the key
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(supplied));
    }
}
