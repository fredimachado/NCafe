using FakeItEasy;
using NCafe.Abstractions.Repositories;
using NCafe.Cashier.Application.Commands;
using NCafe.Cashier.Application.Exceptions;
using NCafe.Cashier.Application.ReadModels;
using NCafe.Cashier.Application.Services;
using NCafe.Cashier.Domain.Entities;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NCafe.Cashier.Application.Tests.Commands
{
    public class PlaceOrderTests
    {
        private readonly PlaceOrderHandler sut;

        private readonly IRepository repository;
        private readonly IProductReadService productReadService;

        public PlaceOrderTests()
        {
            repository = A.Fake<IRepository>();
            productReadService = A.Fake<IProductReadService>();

            sut = new PlaceOrderHandler(repository, productReadService);
        }

        [Fact]
        public async Task GivenProductNotFound_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            A.CallTo(() => productReadService.GetProductAsync(productId))
                .Returns((Product)null);

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
            A.CallTo(() => productReadService.GetProductAsync(productId))
                .Returns(new Product { Id = productId });

            var command = new PlaceOrder(productId, 1);

            // Act
            var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

            // Assert
            exception.ShouldBeNull();
            A.CallTo(() => repository.Save(A<Order>.That.Matches(o => o.ProductId == productId && o.Quantity == 1)))
                .MustHaveHappenedOnceExactly();
        }
    }
}
