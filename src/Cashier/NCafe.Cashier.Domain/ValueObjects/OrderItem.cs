using Ardalis.GuardClauses;

namespace NCafe.Cashier.Domain.ValueObjects;

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
    public int Quantity { get; private set; }
    public decimal Price { get; }

    public decimal Total => Quantity * Price;

    public void IncreaseQuantity(int quantity)
    {
        Quantity += quantity;
    }

    public void DecreaseQuantity(int quantity)
    {
        Quantity -= quantity;
    }
}
