using NCafe.Core.ReadModels;

namespace NCafe.Barista.Domain.ReadModels;

public sealed class BaristaOrder : ReadModel
{
    public string CustomerName { get; set; }
    public BaristaOrderItem[] Items { get; set; }
    public DateTimeOffset OrderPlacedAt { get; set; }
    public DateTimeOffset? OrderPreparedAt { get; set; }
    public bool IsCompleted { get; set; }
}

public sealed class BaristaOrderItem
{
    public string Name { get; set; }
    public int Quantity { get; set; }
}
