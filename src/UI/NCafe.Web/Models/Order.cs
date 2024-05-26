using System.ComponentModel.DataAnnotations;

namespace NCafe.Web.Models;

public class Order
{
    public Guid Id { get; set; }
    [Required]
    public string CustomerName { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total => Items.Sum(x => x.Total);
    public void Clear()
    {
        Items.Clear();
        Id = Guid.Empty;
        CustomerName = string.Empty;
    }
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total => Quantity * Price;
}
