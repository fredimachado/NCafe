using NCafe.Admin.Domain.Entities;
using NCafe.Admin.Domain.Events;

namespace NCafe.Admin.Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void GivenNewProductCreated_ShouldHaveAppliedProductCreatedEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Latte";
        var price = 3m;

        // Act
        var product = new Product(id, name, price);

        // Assert
        var @event = product.GetPendingEvents().ShouldHaveSingleItem();
        @event.ShouldBeOfType<ProductCreated>();

        product.Id.ShouldBe(id);
        product.Name.ShouldBe(name);
        product.Price.ShouldBe(price);
    }
}
