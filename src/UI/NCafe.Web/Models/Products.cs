namespace NCafe.Web.Models;

public sealed class Products
{
    public IEnumerable<Product> Items { get; set; } = Enumerable.Empty<Product>();
}
