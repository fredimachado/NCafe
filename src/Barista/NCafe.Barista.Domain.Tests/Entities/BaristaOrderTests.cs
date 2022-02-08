using NCafe.Barista.Domain.Entities;
using NCafe.Barista.Domain.Events;

namespace NCafe.Barista.Domain.Tests.Entities;

public class BaristaOrderTests
{
    [Fact]
    public void GivenOrderCreated_ShouldHaveAppliedOrderPlacedEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 1;

        // Act
        var baristaOrder = new BaristaOrder(id, productId, quantity);

        // Assert
        var @event = baristaOrder.GetPendingEvents().ShouldHaveSingleItem();
        @event.ShouldBeOfType<OrderPlaced>();

        baristaOrder.Id.ShouldBe(id);
        baristaOrder.ProductId.ShouldBe(productId);
        baristaOrder.Quantity.ShouldBe(quantity);
    }

    [Fact]
    public void GivenOrderCompleted_ShouldHaveAppliedOrderPreparedEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var baristaOrder = new BaristaOrder(id, Guid.NewGuid(), 1);

        // Act
        baristaOrder.CompletePreparation();

        // Assert
        var @event = baristaOrder.GetPendingEvents().Last();
        @event.ShouldBeOfType<OrderPrepared>();

        baristaOrder.IsCompleted.ShouldBeTrue();
    }
}
