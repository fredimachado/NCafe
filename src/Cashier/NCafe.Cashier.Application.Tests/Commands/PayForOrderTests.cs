using FakeItEasy;
using NCafe.Abstractions.Exceptions;
using NCafe.Abstractions.Repositories;
using NCafe.Cashier.Application.Commands;
using NCafe.Cashier.Application.Exceptions;
using NCafe.Cashier.Domain.Entities;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NCafe.Cashier.Application.Tests.Commands;

public class PayForOrderTests
{
    private readonly PayForOrderHandler sut;

    private readonly IRepository repository;

    public PayForOrderTests()
    {
        repository = A.Fake<IRepository>();

        sut = new PayForOrderHandler(repository);
    }

    [Fact]
    public async Task GivenInvalidOrderId_ShouldThrowException()
    {
        // Arrange
        var command = new PayForOrder(Guid.Empty);

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<InvalidIdException>();
    }

    [Fact]
    public async Task GivenOrderNotFound_ShouldThrowException()
    {
        // Arrange
        A.CallTo(() => repository.GetById<Order>(A<Guid>._))
            .Returns((Order)null);
        var command = new PayForOrder(Guid.NewGuid());

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<OrderNotFoundException>();
    }

    [Fact]
    public async Task GivenPlacedOrder_WhenPayingForOrder_ShouldUpdateOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        A.CallTo(() => repository.GetById<Order>(orderId))
            .Returns(new Order(orderId, Guid.NewGuid(), 1));

        var command = new PayForOrder(orderId);

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => repository.Save(A<Order>.That.Matches(o => o.Id == orderId && o.HasBeenPaid == true)))
            .MustHaveHappenedOnceExactly();
    }
}
