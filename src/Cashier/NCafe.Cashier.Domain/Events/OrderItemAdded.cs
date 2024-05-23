using NCafe.Core.Domain;

namespace NCafe.Cashier.Domain.Events;
internal sealed record OrderItemAdded : Event
{
    public OrderItemAdded(Guid id, Guid productId, int quantity, string name, decimal price)
    {
        Id = id;
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
