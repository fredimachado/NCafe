using Microsoft.Extensions.Time.Testing;
using NCafe.Cashier.Domain.Commands;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.ValueObjects;
using NCafe.Core.MessageBus;
using NCafe.Core.Repositories;
using System.Threading;

namespace NCafe.Cashier.Domain.Tests.Commands;

public class PlaceOrderTests
{
    private readonly PlaceOrderHandler _sut;
    private readonly IRepository _repository;
    private readonly IBusPublisher _publisher;
    private readonly FakeTimeProvider _timeProvider;

    public PlaceOrderTests()
    {
        _repository = A.Fake<IRepository>();
        _publisher = A.Fake<IBusPublisher>();
        _timeProvider = new FakeTimeProvider();
        _sut = new PlaceOrderHandler(_repository, _publisher, _timeProvider);
    }

    [Fact]
    public async Task PlaceOrder_ShouldSaveOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, "cashier-1", DateTimeOffset.UtcNow);
        var placedAt = DateTimeOffset.UtcNow;
        var customer = new Customer("John Doe");
        var command = new PlaceOrder(orderId, customer);

        _timeProvider.SetUtcNow(placedAt);

        A.CallTo(() => _repository.GetById<Order>(orderId))
            .Returns(Task.FromResult(order));

        order.AddItem(new OrderItem(Guid.NewGuid(), "Latte", 1, 5));

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _repository.Save(A<Order>.That.Matches(o => o.Status == OrderStatus.Placed &&
                                                                   o.Customer == customer &&
                                                                   o.PlacedAt == placedAt)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GivenNotNewOrder_ShouldThrow()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, "cashier-1", DateTimeOffset.UtcNow);
        var customer = new Customer("John Doe");
        var command = new PlaceOrder(orderId, customer);

        order.AddItem(new OrderItem(Guid.NewGuid(), "Latte", 1, 5));
        order.PlaceOrder(new Customer("John Doe"), DateTimeOffset.UtcNow);

        A.CallTo(() => _repository.GetById<Order>(orderId))
            .Returns(Task.FromResult(order));

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.Handle(command, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<CannotPlaceOrderException>();
    }

    [Fact]
    public async Task GivenNoItemAdded_ShouldThrow()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, "cashier-1", DateTimeOffset.UtcNow);
        var customer = new Customer("John Doe");
        var command = new PlaceOrder(orderId, customer);

        A.CallTo(() => _repository.GetById<Order>(orderId))
            .Returns(Task.FromResult(order));

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.Handle(command, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<CannotPlaceEmptyOrderException>();
    }
}
