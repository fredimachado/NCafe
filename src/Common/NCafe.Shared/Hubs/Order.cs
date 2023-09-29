namespace NCafe.Shared.Hubs;

public sealed record Order(Guid Id, Guid ProductId, int Quantity);
