using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Events;

namespace NCafe.Cashier.Domain.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void GivenNewOrder_ShouldHaveAppliedOrderPlacedEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 1;

        // Act
        var order = new Order(id, productId, quantity);

        // Assert
        var @event = order.GetPendingEvents().ShouldHaveSingleItem();
        @event.ShouldBeOfType<OrderPlaced>();

        order.Id.ShouldBe(id);
        order.ProductId.ShouldBe(productId);
        order.Quantity.ShouldBe(quantity);
    }

    [Fact]
    public void GivenOrder_WhenPaidFor_ShouldHaveAppliedOrderPaidForEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var order = new Order(id, Guid.NewGuid(), 1);

        // Act
        order.PayForOrder();

        // Assert
        var @event = order.GetPendingEvents().Last();
        @event.ShouldBeOfType<OrderPaidFor>();

        order.HasBeenPaid.ShouldBeTrue();
    }
}
