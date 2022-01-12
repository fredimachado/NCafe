using FakeItEasy;
using NCafe.Abstractions.Repositories;
using NCafe.Barista.Domain.Commands;
using NCafe.Barista.Domain.Entities;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NCafe.Barista.Domain.Tests.Commands;

public class PlaceOrderTests
{
    private readonly PlaceOrderHandler sut;

    private readonly IRepository repository;

    public PlaceOrderTests()
    {
        repository = A.Fake<IRepository>();

        sut = new PlaceOrderHandler(repository);
    }

    [Fact]
    public async Task ShouldSaveOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var command = new PlaceOrder(orderId, productId, 1);

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => repository.Save(A<BaristaOrder>.That.Matches(o => o.Id == orderId && o.ProductId == productId && o.Quantity == 1)))
            .MustHaveHappenedOnceExactly();
    }
}
