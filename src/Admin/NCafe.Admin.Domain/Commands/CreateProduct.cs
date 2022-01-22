using NCafe.Core.Commands;
using NCafe.Core.Repositories;
using NCafe.Admin.Domain.Entities;
using NCafe.Admin.Domain.Exceptions;

namespace NCafe.Admin.Domain.Commands;

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
