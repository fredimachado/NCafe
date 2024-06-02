using MediatR;
using NCafe.Admin.Domain.Entities;
using NCafe.Admin.Domain.Exceptions;
using NCafe.Core.Repositories;

namespace NCafe.Admin.Domain.Commands;

public record DeleteProduct(Guid Id) : IRequest;

internal sealed class DeleteProductHandler(IRepository repository) : IRequestHandler<DeleteProduct>
{
    private readonly IRepository _repository = repository;

    public async Task Handle(DeleteProduct command, CancellationToken cancellationToken)
    {
        var product = await _repository.GetById<Product>(command.Id);
        if (product is null || product.IsDeleted)
        {
            throw new ProductNotFound(command.Id);
        }

        product.Delete();
        await _repository.Save(product);
    }
}
