using NCafe.Abstractions.ReadModels;

namespace NCafe.Cashier.Application.ReadModels;

public sealed class Product : ReadModel
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
