using FakeItEasy;
using NCafe.Abstractions.MessageBus;
using NCafe.Abstractions.MessageBus.Events;
using NCafe.Abstractions.ReadModels;
using NCafe.Abstractions.Repositories;
using NCafe.Cashier.Domain.Commands;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.ReadModels;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NCafe.Cashier.Domain.Tests.Commands;

public class PlaceOrderTests
{
    private readonly PlaceOrderHandler sut;

    private readonly IRepository repository;
    private readonly IReadModelRepository<Product> productRepository;
    private readonly IPublisher publisher;

    public PlaceOrderTests()
    {
        repository = A.Fake<IRepository>();
        productRepository = A.Fake<IReadModelRepository<Product>>();
        publisher = A.Fake<IPublisher>();

        sut = new PlaceOrderHandler(repository, productRepository, publisher);
    }

    [Fact]
    public async Task GivenProductNotFound_ShouldThrowException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => productRepository.GetById(productId))
            .Returns(null);

        var command = new PlaceOrder(productId, 1);

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<ProductNotFoundException>();
    }

    [Fact]
    public async Task GivenProductExists_ShouldSaveOrder()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => productRepository.GetById(productId))
            .Returns(new Product { Id = productId });

        var command = new PlaceOrder(productId, 1);

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => repository.Save(A<Order>.That.Matches(o => o.ProductId == productId && o.Quantity == 1)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GivenOrderSaved_ShouldPublishToMessageBus()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => productRepository.GetById(productId))
            .Returns(new Product { Id = productId });

        var command = new PlaceOrder(productId, 1);

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => publisher.Publish("orders", A<OrderPlaced>.That.Matches(o => o.ProductId == productId && o.Quantity == 1)))
            .MustHaveHappenedOnceExactly();
    }
}
