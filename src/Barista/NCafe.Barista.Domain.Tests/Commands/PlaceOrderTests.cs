using NCafe.Barista.Domain.Commands;
using NCafe.Barista.Domain.Entities;
using NCafe.Core.Repositories;

namespace NCafe.Barista.Domain.Tests.Commands;

public class PlaceOrderTests
{
    private readonly PlaceOrderHandler _sut;
    private readonly IRepository _repository;

    public PlaceOrderTests()
    {
        _repository = A.Fake<IRepository>();
        _sut = new PlaceOrderHandler(_repository);
    }

    [Fact]
    public async Task ShouldSaveOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var command = new PlaceOrder(orderId, productId, 1);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => _repository.Save(A<BaristaOrder>.That.Matches(o => o.Id == orderId && o.ProductId == productId && o.Quantity == 1)))
            .MustHaveHappenedOnceExactly();
    }
}
