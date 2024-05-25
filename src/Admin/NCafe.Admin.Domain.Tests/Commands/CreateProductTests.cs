using NCafe.Admin.Domain.Commands;
using NCafe.Admin.Domain.Entities;
using NCafe.Admin.Domain.Exceptions;
using NCafe.Core.Repositories;
using System.Threading;

namespace NCafe.Admin.Domain.Tests.Commands;

public class CreateProductTests
{
    private readonly CreateProductHandler _sut;
    private readonly IRepository _repository;

    public CreateProductTests()
    {
        _repository = A.Fake<IRepository>();
        _sut = new CreateProductHandler(_repository);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GivenInvalidName_ShouldThrowException(string name)
    {
        // Arrange
        var command = new CreateProduct(name, 3);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.Handle(command, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<InvalidProductNameException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GivenInvalidPrice_ShouldThrowException(decimal price)
    {
        // Arrange
        var command = new CreateProduct("Flat White", price);

        // Act
        var exception = await Record.ExceptionAsync(() => _sut.Handle(command, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<InvalidProductPriceException>();
    }

    [Fact]
    public async Task GivenValidProductInformation_ShouldCreateAndStoreProduct()
    {
        // Arrange
        var command = new CreateProduct("Flat White", 3.5m);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _repository.Save(A<Product>.That.Matches(p => p.Name == command.Name && p.Price == command.Price)))
            .MustHaveHappenedOnceExactly();
    }
}
