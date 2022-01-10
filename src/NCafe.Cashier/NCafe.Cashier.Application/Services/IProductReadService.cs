using NCafe.Cashier.Application.ReadModels;

namespace NCafe.Cashier.Application.Services;

public interface IProductReadService
{
    Task<Product> GetProductAsync(Guid id);
}
