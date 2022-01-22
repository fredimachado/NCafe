using NCafe.Core.ReadModels;

namespace NCafe.Cashier.Domain.ReadModels;

public sealed class Product : ReadModel
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
