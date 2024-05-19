﻿using NCafe.Core.Domain;

namespace NCafe.Barista.Domain.Events;

public sealed record OrderPlaced : Event
{
    public OrderPlaced(Guid id)
    {
        Id = id;
    }

    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
