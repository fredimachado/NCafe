﻿using NCafe.Barista.Domain.Entities;
using NCafe.Core.Commands;
using NCafe.Core.Repositories;

namespace NCafe.Barista.Domain.Commands;

public record PlaceOrder(Guid Id, Guid ProductId, int Quantity) : ICommand;

internal sealed class PlaceOrderHandler : ICommandHandler<PlaceOrder>
{
    private readonly IRepository repository;

    public PlaceOrderHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task HandleAsync(PlaceOrder command)
    {
        var order = new BaristaOrder(command.Id, command.ProductId, command.Quantity);

        await repository.Save(order);
    }
}
