using Microsoft.Extensions.Time.Testing;
using NCafe.Cashier.Domain.Commands;
using NCafe.Cashier.Domain.Entities;
using NCafe.Core.Repositories;
using System.Threading;

namespace NCafe.Cashier.Domain.Tests.Commands;

public class CreateOrderTests
{
    private readonly CreateOrderHandler _sut;
    private readonly IRepository _repository;
    private readonly FakeTimeProvider _timeProvider;

    public CreateOrderTests()
    {
        _repository = A.Fake<IRepository>();
        _timeProvider = new FakeTimeProvider();
        _sut = new CreateOrderHandler(_repository, _timeProvider);
    }

    [Fact]
    public async Task ShouldSaveOrder()
    {
        // Arrange
        var createdAt = DateTimeOffset.UtcNow;
        _timeProvider.SetUtcNow(createdAt);
        var createdBy = "cashier-1";
        var command = new CreateOrder(createdBy);

        // Act
        var orderId = await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _repository.Save(A<Order>.That.Matches(o => o.Status == OrderStatus.New &&
                                                                   o.CreatedBy == createdBy &&
                                                                   o.CreatedAt == createdAt)))
            .MustHaveHappenedOnceExactly();
    }
}
