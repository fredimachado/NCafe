using NCafe.Abstractions.Commands;
using NCafe.Abstractions.Repositories;
using NCafe.Admin.Application.Exceptions;
using NCafe.Admin.Domain.Entities;

namespace NCafe.Admin.Application.Commands;

public record CreateProduct(string Name, decimal Price) : ICommand;

internal sealed class CreateProductHandler : ICommandHandler<CreateProduct>
{
    private readonly IRepository repository;

    public CreateProductHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task HandleAsync(CreateProduct command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new InvalidProductNameException();
        }

        if (command.Price <= 0)
        {
            throw new InvalidProductPriceException(command.Price);
        }

        var product = new Product(Guid.NewGuid(), command.Name, command.Price);

        await repository.Save(product);
    }
}
