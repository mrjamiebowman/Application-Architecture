using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrJB.API.Tests.Integration.Infrastructure;
using MrJB.Sample.Domain.Models;
using Shouldly;

namespace MrJB.API.Tests.Integration.Controllers;

/// <summary>
/// GET /api/customers/{customerId} against the real API and database.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class CustomersControllerTests(ApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task GetCustomerById_ExistingCustomer_Returns200WithCustomer()
    {
        // arrange: a row only this test knows about
        var email = $"it-{Guid.NewGuid():N}@example.com";
        var customerId = await factory.SeedCustomerAsync("Grace", "Hopper", email, "555-0199", CancellationToken);

        // act
        var response = await _client.GetAsync($"/api/customers/{customerId}", CancellationToken);

        // assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var customer = await response.Content.ReadFromJsonAsync<Customer>(CancellationToken);
        customer.ShouldNotBeNull();
        customer.CustomerId.ShouldBe(customerId);
        customer.FirstName.ShouldBe("Grace");
        customer.LastName.ShouldBe("Hopper");
        customer.Email.ShouldBe(email);
        customer.PhoneNumber.ShouldBe("555-0199");
    }

    [Fact]
    public async Task GetCustomerById_UnknownCustomer_Returns404ProblemDetails()
    {
        // identity values start at 1 and won't reach int.MaxValue
        const int customerId = int.MaxValue;

        var response = await _client.GetAsync($"/api/customers/{customerId}", CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(CancellationToken);
        problem.ShouldNotBeNull();
        problem.Status.ShouldBe(StatusCodes.Status404NotFound);
        problem.Title.ShouldBe("Customer not found");
        problem.Detail.ShouldBe($"No customer has the id {customerId}.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetCustomerById_NonPositiveId_Returns400ValidationProblem(int customerId)
    {
        var response = await _client.GetAsync($"/api/customers/{customerId}", CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(CancellationToken);
        problem.ShouldNotBeNull();
        problem.Status.ShouldBe(StatusCodes.Status400BadRequest);
        problem.Errors.ShouldContainKey("customerId");
    }

    [Fact]
    public async Task GetCustomerById_NonIntegerId_Returns404()
    {
        // the {customerId:int} route constraint doesn't match, so no action is selected
        var response = await _client.GetAsync("/api/customers/abc", CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
