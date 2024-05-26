namespace NCafe.Web.Models;

public record BaristaOrder(Guid Id, string CustomerName, BaristaOrderItem[] Items, DateTimeOffset OrderPlacedAt);

public record BaristaOrderItem(string Name, int Quantity);
