using Ardalis.GuardClauses;

namespace NCafe.Cashier.Domain.ValueObjects;

public record OrderItem
{
    public OrderItem(Guid productId, int quantity, string name, decimal price)
    {
        Guard.Against.Default(productId, nameof(productId));
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.NegativeOrZero(price, nameof(price));

        ProductId = productId;
        Quantity = quantity;
        Name = name;
        Price = price;
    }

    public Guid ProductId { get; }
    public int Quantity { get; }
    public string Name { get; }
    public decimal Price { get; }
}
