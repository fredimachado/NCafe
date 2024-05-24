using NCafe.Barista.Domain.Commands;
using NCafe.Barista.Domain.Entities;
using NCafe.Core.Repositories;
using System.Threading;

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
        var orderItem = new OrderItem(productId, "Cappuccino", 1, 3.99m);
        var customer = "John Doe";
        var command = new PlaceOrder(orderId, [orderItem], customer);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _repository.Save(A<BaristaOrder>.That.Matches(o => o.Id == orderId && o.Customer == customer)))
            .MustHaveHappenedOnceExactly();
    }
}
