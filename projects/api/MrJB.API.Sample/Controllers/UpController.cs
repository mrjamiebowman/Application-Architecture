using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrJB.API.Sample.Filters;
using MrJB.Sample.Domain.Configuration;

namespace MrJB.API.Sample.Controllers;

/// <summary>
/// Availability and logging checks. Requires the key in Application:UpKey and is excluded from the OpenAPI/Swagger document.
/// </summary>
[AllowAnonymous]
[RequireKey(ApplicationConfiguration.UpKeyPath)]
[Route("[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class UpController(ILogger<UpController> logger) : ControllerBase
{
    /// <summary>
    /// Availability check: GET/HEAD /up/{key}.
    /// </summary>
    [HttpGet("{key}")]
    [HttpHead("{key}")]
    public IActionResult Get() => Content("UP", "text/plain");

    /// <summary>
    /// Writes one log entry at every level to validate logging end to end: GET /up/log/{key}.
    /// </summary>
    [HttpGet("log/{key}")]
    public IActionResult Log()
    {
        // search the log store for this id to find exactly this run
        var checkId = Guid.NewGuid();

        logger.LogTrace("Log check {CheckId}: Trace", checkId);
        logger.LogDebug("Log check {CheckId}: Debug", checkId);
        logger.LogInformation("Log check {CheckId}: Information", checkId);
        logger.LogWarning("Log check {CheckId}: Warning", checkId);
        logger.LogError(new InvalidOperationException("Log check test exception."), "Log check {CheckId}: Error", checkId);
        logger.LogCritical("Log check {CheckId}: Critical", checkId);

        // shows which levels the current config actually emits (e.g. Trace/Debug are off by default)
        var levels = Enum.GetValues<LogLevel>()
            .Where(level => level != LogLevel.None)
            .ToDictionary(level => level.ToString(), logger.IsEnabled);

        return Ok(new { checkId, levels });
    }
}
