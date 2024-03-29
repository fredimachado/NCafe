﻿using EasyNetQ;
using Microsoft.Extensions.Configuration;
using NCafe.Core.MessageBus;

namespace NCafe.Infrastructure.MessageBus;

internal class RabbitMqPublisher : IPublisher
{
    private readonly IBus bus;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        bus = RabbitHutch.CreateBus(configuration.GetConnectionString("RabbitMq"));
    }

    public async Task Publish<T>(string topicName, T message) where T : class, IBusMessage
    {
        await bus.PubSub.PublishAsync(message, topicName);
    }
}
