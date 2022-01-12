using FakeItEasy;
using NCafe.Abstractions.Repositories;
using NCafe.Barista.Domain.Commands;
using NCafe.Barista.Domain.Entities;
using NCafe.Barista.Domain.Exceptions;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NCafe.Barista.Domain.Tests.Commands;

public class CompleteOrderTests
{
    private readonly CompleteOrderHandler sut;

    private readonly IRepository repository;

    public CompleteOrderTests()
    {
        repository = A.Fake<IRepository>();

        sut = new CompleteOrderHandler(repository);
    }

    [Fact]
    public async Task GivenOrderNotFound_ShouldThrowException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        A.CallTo(() => repository.GetById<BaristaOrder>(orderId))
            .Returns((BaristaOrder)null);

        var command = new CompleteOrder(orderId);

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<OrderNotFoundException>();
    }

    [Fact]
    public async Task ShouldSaveOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        A.CallTo(() => repository.GetById<BaristaOrder>(orderId))
            .Returns(new BaristaOrder(orderId, Guid.NewGuid(), 1));
        var command = new CompleteOrder(orderId);

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => repository.Save(A<BaristaOrder>.That.Matches(o => o.Id == orderId && o.IsCompleted == true)))
            .MustHaveHappenedOnceExactly();
    }
}
