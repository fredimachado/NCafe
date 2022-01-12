namespace NCafe.Cashier.Domain.ReadModels.Events;

public class ProductCreated
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
