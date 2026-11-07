using Microsoft.AspNetCore.Mvc;
using MrJB.Sample.Domain.Interfaces;
using MrJB.Sample.Domain.Models;

namespace MrJB.API.Sample.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class CustomerController(ILogger<CustomerController> logger, IAppService appService) : ControllerBase
{
    public ILogger<CustomerController> Logger { get; } = logger;

    private IAppService AppService { get; } = appService;

    /// <summary>
    /// Gets a customer by id: GET /api/customer/{customerId}.
    /// </summary>
    /// <response code="200">The customer.</response>
    /// <response code="400">The id isn't a positive integer.</response>
    /// <response code="404">No customer has that id.</response>
    [HttpGet("{customerId:int}", Name = nameof(GetCustomerById))]
    [ProducesResponseType<Customer>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Customer>> GetCustomerById(int customerId, CancellationToken cancellationToken)
    {
        if (customerId <= 0)
        {
            ModelState.AddModelError(nameof(customerId), "The customer id must be a positive integer.");
            return ValidationProblem(ModelState);
        }

        // every log written below this call (app and data layers) carries CustomerId as a scope property
        using var scope = Logger.BeginScope(new Dictionary<string, object> { ["CustomerId"] = customerId });

        var customer = await AppService.Customers.GetCustomerByIdAsync(customerId, cancellationToken);

        if (customer is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Customer not found",
                detail: $"No customer has the id {customerId}.");
        }

        return Ok(customer);
    }
}
