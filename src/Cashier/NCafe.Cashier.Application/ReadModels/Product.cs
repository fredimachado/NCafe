namespace NCafe.Cashier.Application.ReadModels;

public sealed class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
