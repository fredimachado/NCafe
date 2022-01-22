using NCafe.Core.ReadModels;

namespace NCafe.Barista.Domain.ReadModels;

public sealed class BaristaOrder : ReadModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public bool IsCompleted { get; set; }
}
