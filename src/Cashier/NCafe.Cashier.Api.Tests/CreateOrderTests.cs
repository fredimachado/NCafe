using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace NCafe.Cashier.Api.Tests;

public class CreateOrderTests(CashierWebApplicationFactory<Program> factory) : IClassFixture<CashierWebApplicationFactory<Program>>
{
    private readonly CashierWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateOrder_ShouldReturnOrderId()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/orders", new { CreatedBy = "cashier-1" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<Guid>();
        content.ShouldNotBe(Guid.Empty);
    }
}
