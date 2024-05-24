namespace NCafe.Shared.Hubs;

public sealed record Order(Guid Id, OrderItem[] OrderItems, string Customer);

public sealed record OrderItem(string Name, int Quantity);
