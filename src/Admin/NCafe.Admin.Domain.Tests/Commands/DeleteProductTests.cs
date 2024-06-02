using NCafe.Admin.Domain.Commands;
using NCafe.Admin.Domain.Entities;
using NCafe.Admin.Domain.Exceptions;
using NCafe.Core.Repositories;
using System.Threading;

namespace NCafe.Admin.Domain.Tests.Commands;

public class DeleteProductTests
{
    private readonly DeleteProductHandler _sut;
    private readonly IRepository _repository;

    public DeleteProductTests()
    {
        _repository = A.Fake<IRepository>();
        _sut = new DeleteProductHandler(_repository);
    }

    [Fact]
    public async Task GivenNonExistingProduct_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        A.CallTo(() => _repository.GetById<Product>(id)).Returns((Product)null);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.Handle(new DeleteProduct(id), CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<ProductNotFound>();
    }

    [Fact]
    public async Task GivenExistingProduct_ShouldDeleteProduct()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product = new Product(id, "Latte", 3);
        A.CallTo(() => _repository.GetById<Product>(id)).Returns(product);

        // Act
        await _sut.Handle(new DeleteProduct(id), CancellationToken.None);

        // Assert
        A.CallTo(() => _repository.Save(A<Product>.That.Matches(p => p.Id == id && p.IsDeleted == true)))
            .MustHaveHappenedOnceExactly();
    }
}
