using MediatR;
using NCafe.Admin.Domain.Entities;
using NCafe.Core.Repositories;

namespace NCafe.Admin.Domain.Commands;

public record CreateProduct(string Name, decimal Price) : IRequest;

internal sealed class CreateProductHandler(IRepository repository) : IRequestHandler<CreateProduct>
{
    private readonly IRepository _repository = repository;

    public async Task Handle(CreateProduct command, CancellationToken cancellationToken)
    {
        var product = new Product(Guid.NewGuid(), command.Name, command.Price);

        await _repository.Save(product);
    }
}
