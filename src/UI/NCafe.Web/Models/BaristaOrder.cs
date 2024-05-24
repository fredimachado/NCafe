namespace NCafe.Web.Models;

public record BaristaOrder(Guid Id, string CustomerName, BaristaOrderItem[] Items, bool IsCompleted);

public record BaristaOrderItem(string Name, int Quantity);
