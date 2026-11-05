using Microsoft.AspNetCore.Mvc;

namespace MrJB.API.Sample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SampleController(ILogger<UpController> logger) : ControllerBase
{
    public ILogger<UpController> Logger { get; } = logger;


}
