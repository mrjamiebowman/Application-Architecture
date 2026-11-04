using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MrJB.API.Sample.Controllers;

/// <summary>
/// Availability check. Excluded from the OpenAPI/Swagger document.
/// </summary>
[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class UpController : ControllerBase
{
    [HttpGet]
    [HttpHead]
    public IActionResult Get() => Content("UP", "text/plain");
}
