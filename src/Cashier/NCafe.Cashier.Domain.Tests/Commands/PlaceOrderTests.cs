using NCafe.Cashier.Domain.Commands;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.ReadModels;
using NCafe.Core.MessageBus;
using NCafe.Core.MessageBus.Events;
using NCafe.Core.ReadModels;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Tests.Commands;

public class PlaceOrderTests
{
    private readonly PlaceOrderHandler _sut;
    private readonly IRepository _repository;
    private readonly IReadModelRepository<Product> _productRepository;
    private readonly IPublisher _publisher;

    public PlaceOrderTests()
    {
        _repository = A.Fake<IRepository>();
        _productRepository = A.Fake<IReadModelRepository<Product>>();
        _publisher = A.Fake<IPublisher>();
        _sut = new PlaceOrderHandler(_repository, _productRepository, _publisher);
    }

    [Fact]
    public async Task GivenProductNotFound_ShouldThrowException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => _productRepository.GetById(productId))
            .Returns(null);

        var command = new PlaceOrder(productId, 1);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<ProductNotFoundException>();
    }

    [Fact]
    public async Task GivenProductExists_ShouldSaveOrder()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => _productRepository.GetById(productId))
            .Returns(new Product { Id = productId });

        var command = new PlaceOrder(productId, 1);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => _repository.Save(A<Order>.That.Matches(o => o.ProductId == productId && o.Quantity == 1)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GivenOrderSaved_ShouldPublishToMessageBus()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => _productRepository.GetById(productId))
            .Returns(new Product { Id = productId });

        var command = new PlaceOrder(productId, 1);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => _publisher.Publish("orders_queue", A<OrderPlaced>.That.Matches(o => o.ProductId == productId && o.Quantity == 1)))
            .MustHaveHappenedOnceExactly();
    }
}
