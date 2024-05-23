using Ardalis.GuardClauses;

namespace NCafe.Cashier.Domain.ValueObjects;

public record Customer
{
    public Customer(string name)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        Guard.Against.StringTooShort(name, 2, nameof(name));

        Name = name;
    }

    public string Name { get; }

    public static implicit operator Customer(string name) => new(name);
    public static implicit operator string(Customer customer) => customer.Name;
}
