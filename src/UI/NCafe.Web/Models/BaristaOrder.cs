namespace NCafe.Web.Models;

public class BaristaOrder
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public bool IsCompleted { get; set; }
}
