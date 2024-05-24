using Ardalis.GuardClauses;

namespace NCafe.Barista.Domain.ValueObjects;

public record OrderItem
{
    public OrderItem(Guid productId, string name, int quantity, decimal price)
    {
        ProductId = Guard.Against.Default(productId);
        Name = Guard.Against.NullOrWhiteSpace(name);
        Quantity = Guard.Against.NegativeOrZero(quantity);
        Price = Guard.Against.NegativeOrZero(price);
    }

    public Guid ProductId { get; }
    public string Name { get; }
    public int Quantity { get; }
    public decimal Price { get; }
}
