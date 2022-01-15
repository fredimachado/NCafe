namespace NCafe.Cashier.Api.Projections;

public record ProductCreated(Guid Id, string Name, decimal Price);
