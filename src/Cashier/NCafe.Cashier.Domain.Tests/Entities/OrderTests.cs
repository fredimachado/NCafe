using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Events;

namespace NCafe.Cashier.Domain.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void GivenNewOrder_ShouldHaveAppliedOrderCreatedEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdBy = "cashier-1";
        var createdAt = DateTimeOffset.UtcNow;

        // Act
        var order = new Order(id, createdBy, createdAt);

        // Assert
        var @event = order.GetPendingEvents().ShouldHaveSingleItem();
        @event.ShouldBeOfType<OrderCreated>();

        order.Id.ShouldBe(id);
        order.CreatedBy.ShouldBe(createdBy);
        order.CreatedAt.ShouldBe(createdAt);
    }
}
