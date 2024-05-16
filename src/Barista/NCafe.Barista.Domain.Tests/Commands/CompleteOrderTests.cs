using NCafe.Barista.Domain.Commands;
using NCafe.Barista.Domain.Entities;
using NCafe.Barista.Domain.Exceptions;
using NCafe.Core.Repositories;

namespace NCafe.Barista.Domain.Tests.Commands;

public class CompleteOrderTests
{
    private readonly CompleteOrderHandler _sut;
    private readonly IRepository _repository;

    public CompleteOrderTests()
    {
        _repository = A.Fake<IRepository>();
        _sut = new CompleteOrderHandler(_repository);
    }

    [Fact]
    public async Task GivenOrderNotFound_ShouldThrowException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        A.CallTo(() => _repository.GetById<BaristaOrder>(orderId))
            .Returns((BaristaOrder)null);

        var command = new CompleteOrder(orderId);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<OrderNotFoundException>();
    }

    [Fact]
    public async Task ShouldSaveOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        A.CallTo(() => _repository.GetById<BaristaOrder>(orderId))
            .Returns(new BaristaOrder(orderId, Guid.NewGuid(), 1));
        var command = new CompleteOrder(orderId);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => _repository.Save(A<BaristaOrder>.That.Matches(o => o.Id == orderId && o.IsCompleted == true)))
            .MustHaveHappenedOnceExactly();
    }
}
