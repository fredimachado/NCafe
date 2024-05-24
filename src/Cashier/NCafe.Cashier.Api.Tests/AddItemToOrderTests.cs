using FakeItEasy;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.ReadModels;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace NCafe.Cashier.Api.Tests;

public class AddItemToOrderTests(CashierWebApplicationFactory<Program> factory) : IClassFixture<CashierWebApplicationFactory<Program>>
{
    private readonly CashierWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task AddItemToOrder_ShouldReturnAcceptedStatus()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        A.CallTo(() => _factory.FakeRepository.GetById<Order>(orderId))
            .Returns(Task.FromResult(new Order(orderId, "cashier-1", DateTimeOffset.UtcNow)));

        A.CallTo(() => _factory.FakeProductRepository.GetById(productId))
            .Returns(new Product { Name = "Latte", Price = 5 });

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/orders/add-item",
            new { OrderId = orderId, ProductId = productId, Quantity = 1 });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
    }
}
