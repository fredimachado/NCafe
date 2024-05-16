using NCafe.Cashier.Domain.Commands;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Core.Exceptions;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Tests.Commands;

public class PayForOrderTests
{
    private readonly PayForOrderHandler _sut;
    private readonly IRepository _repository;

    public PayForOrderTests()
    {
        _repository = A.Fake<IRepository>();
        _sut = new PayForOrderHandler(_repository);
    }

    [Fact]
    public async Task GivenInvalidOrderId_ShouldThrowException()
    {
        // Arrange
        var command = new PayForOrder(Guid.Empty);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<InvalidIdException>();
    }

    [Fact]
    public async Task GivenOrderNotFound_ShouldThrowException()
    {
        // Arrange
        A.CallTo(() => _repository.GetById<Order>(A<Guid>._))
            .Returns((Order)null);
        var command = new PayForOrder(Guid.NewGuid());

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<OrderNotFoundException>();
    }

    [Fact]
    public async Task GivenPlacedOrder_WhenPayingForOrder_ShouldUpdateOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        A.CallTo(() => _repository.GetById<Order>(orderId))
            .Returns(new Order(orderId, Guid.NewGuid(), 1));

        var command = new PayForOrder(orderId);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => _repository.Save(A<Order>.That.Matches(o => o.Id == orderId && o.HasBeenPaid == true)))
            .MustHaveHappenedOnceExactly();
    }
}
