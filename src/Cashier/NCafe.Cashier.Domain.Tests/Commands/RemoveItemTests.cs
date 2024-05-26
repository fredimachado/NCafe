using NCafe.Cashier.Domain.Commands;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.ValueObjects;
using NCafe.Core.Repositories;
using System.Threading;

namespace NCafe.Cashier.Domain.Tests.Commands;

public class RemoveItemTests
{
    private readonly RemoveItemFromOrderHandler _sut;
    private readonly IRepository _repository;

    public RemoveItemTests()
    {
        _repository = A.Fake<IRepository>();
        _sut = new RemoveItemFromOrderHandler(_repository);
    }

    [Fact]
    public async Task ShouldSaveOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 1;
        var command = new RemoveItemFromOrder(orderId, productId, quantity);
        var order = new Order(orderId, "cashier-1", DateTimeOffset.UtcNow);
        order.AddItem(new OrderItem(productId, "product1", 1, 5));

        A.CallTo(() => _repository.GetById<Order>(orderId))
            .Returns(Task.FromResult(order));

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _repository.Save(
                A<Order>.That.Matches(o => o.Id == orderId &&
                                           o.Items.All(i => i.ProductId != productId))))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GivenOrderNotFound_ShouldThrow()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 1;
        var command = new RemoveItemFromOrder(orderId, productId, quantity);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.Handle(command, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<OrderNotFoundException>();
    }

    [Fact]
    public async Task GivenRemovingMoreItemsThanOrdered_ShouldThrow()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 2;
        var command = new RemoveItemFromOrder(orderId, productId, quantity);
        var order = new Order(orderId, "cashier-1", DateTimeOffset.UtcNow);
        order.AddItem(new OrderItem(productId, "product1", 1, 5));

        A.CallTo(() => _repository.GetById<Order>(orderId))
            .Returns(Task.FromResult(order));

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.Handle(command, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<CannotRemoveMoreItemsThanOrderedException>();
    }

    [Fact]
    public async Task GivenRemovingLessItemsThanOrdered_ShouldSaveOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 1;
        var command = new RemoveItemFromOrder(orderId, productId, quantity);
        var order = new Order(orderId, "cashier-1", DateTimeOffset.UtcNow);
        order.AddItem(new OrderItem(productId, "product1", 2, 5));

        A.CallTo(() => _repository.GetById<Order>(orderId))
            .Returns(Task.FromResult(order));

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _repository.Save(
                A<Order>.That.Matches(o => o.Id == orderId &&
                                           o.Items.Any(i => i.ProductId == productId && i.Quantity == 1))))
            .MustHaveHappenedOnceExactly();
    }

}
