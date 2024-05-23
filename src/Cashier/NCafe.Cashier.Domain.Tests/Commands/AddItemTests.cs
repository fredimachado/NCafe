using NCafe.Cashier.Domain.Commands;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.ReadModels;
using NCafe.Cashier.Domain.ValueObjects;
using NCafe.Core.ReadModels;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Tests.Commands;

public class AddItemTests
{
    private readonly AddItemToOrderHandler _sut;
    private readonly IRepository _repository;
    private readonly IReadModelRepository<Product> _productRepository;

    public AddItemTests()
    {
        _repository = A.Fake<IRepository>();
        _productRepository = A.Fake<IReadModelRepository<Product>>();
        _sut = new AddItemToOrderHandler(_repository, _productRepository);
    }

    [Fact]
    public async Task ShouldSaveOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 1;
        var command = new AddItemToOrder(orderId, productId, quantity);

        A.CallTo(() => _repository.GetById<Order>(orderId))
            .Returns(Task.FromResult(new Order(orderId, "cashier-1", DateTimeOffset.UtcNow)));

        A.CallTo(() => _productRepository.GetById(command.ProductId))
            .Returns(new Product { Name = "Latte", Price = 5 });

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => _repository.Save(A<Order>.That.Matches(o => o.Id == orderId &&
                                                                   o.Items.Any(i => i.ProductId == productId && i.Quantity == quantity))))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GivenOrderNotFound_ShouldThrow()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 1;
        var command = new AddItemToOrder(orderId, productId, quantity);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<OrderNotFoundException>();
    }

    [Fact]
    public async Task GivenProductNotFound_ShouldThrow()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 1;
        var command = new AddItemToOrder(orderId, productId, quantity);

        A.CallTo(() => _repository.GetById<Order>(orderId))
            .Returns(Task.FromResult(new Order(orderId, "cashier-1", DateTimeOffset.UtcNow)));

        A.CallTo(() => _productRepository.GetById(command.ProductId))
            .Returns(null);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<ProductNotFoundException>();
    }

    [Fact]
    public async Task GivenNotNewOrder_ShouldThrow()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, "cashier-1", DateTimeOffset.UtcNow);
        var productId = Guid.NewGuid();
        var quantity = 1;
        var command = new AddItemToOrder(orderId, productId, quantity);

        order.AddItem(new OrderItem(Guid.NewGuid(), 1, "Latte", 5));
        order.PlaceOrder(new Customer("John Doe"), DateTimeOffset.UtcNow);

        A.CallTo(() => _repository.GetById<Order>(orderId))
            .Returns(Task.FromResult(order));

        A.CallTo(() => _productRepository.GetById(command.ProductId))
            .Returns(new Product { Name = "Latte", Price = 5 });

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<CannotAddItemToOrderException>();
    }
}
