using FakeItEasy;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Messages;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace NCafe.Cashier.Api.Tests;

public class PlaceOrderTests(CashierWebApplicationFactory<Program> factory) : IClassFixture<CashierWebApplicationFactory<Program>>
{
    private readonly CashierWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PlaceOrder_ShouldReturnAcceptedStatus()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, "cashier-1", DateTimeOffset.UtcNow);

        order.AddItem(new Domain.ValueObjects.OrderItem(Guid.NewGuid(), "Latte", 1, 4));

        A.CallTo(() => _factory.FakeRepository.GetById<Order>(orderId))
            .Returns(Task.FromResult(order));

        // Act
        var response = await _client.PostAsJsonAsync($"/orders/place", new { OrderId = orderId, Customer = new { Name = "John Doe" } });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

        A.CallTo(() => _factory.FakeBusPublisher.Publish(A<OrderPlacedMessage>.That.Matches(p => p.Id == orderId)))
            .MustHaveHappenedOnceExactly();
    }
}
