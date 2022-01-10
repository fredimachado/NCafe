using FakeItEasy;
using NCafe.Abstractions.Repositories;
using NCafe.Admin.Application.Commands;
using NCafe.Admin.Application.Exceptions;
using NCafe.Admin.Domain.Entities;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace NCafe.Admin.Application.Tests.Commands;

public class CreateProductTests
{
    private readonly CreateProductHandler sut;

    private readonly IRepository repository;

    public CreateProductTests()
    {
        repository = A.Fake<IRepository>();

        sut = new CreateProductHandler(repository);
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
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

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
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeOfType<InvalidProductPriceException>();
    }

    [Fact]
    public async Task GivenValidProductInformation_ShouldCreateAndStoreProduct()
    {
        // Arrange
        var command = new CreateProduct("Flat White", 3.5m);

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HandleAsync(command));

        // Assert
        exception.ShouldBeNull();
        A.CallTo(() => repository.Save(A<Product>.That.Matches(p => p.Name == command.Name && p.Price == command.Price)))
            .MustHaveHappenedOnceExactly(); ;
    }
}
