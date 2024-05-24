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
        var productName = "Latte";
        var quantity = 1;
        var price = 4.5m;

        // Act
        var baristaOrder = new BaristaOrder(id, [new(productId, productName, quantity, price)], "John Doe");

        // Assert
        var @event = baristaOrder.GetPendingEvents().ShouldHaveSingleItem();
        @event.ShouldBeOfType<OrderPlaced>();

        baristaOrder.Id.ShouldBe(id);
        baristaOrder.Items.Count.ShouldBe(1);
        var item = baristaOrder.Items.First();
        item.ProductId.ShouldBe(productId);
        item.Name.ShouldBe(productName);
        item.Quantity.ShouldBe(quantity);
        item.Price.ShouldBe(price);
        baristaOrder.Customer.ShouldBe("John Doe");
    }

    [Fact]
    public void GivenOrderCompleted_ShouldHaveAppliedOrderPreparedEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var baristaOrder = new BaristaOrder(id, [new(Guid.NewGuid(), "Latte", 1, 4)], "John Doe");

        // Act
        baristaOrder.CompletePreparation();

        // Assert
        var @event = baristaOrder.GetPendingEvents().Last();
        @event.ShouldBeOfType<OrderPrepared>();

        baristaOrder.IsCompleted.ShouldBeTrue();
    }
}
