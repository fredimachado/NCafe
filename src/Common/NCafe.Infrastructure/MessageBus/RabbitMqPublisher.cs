using EasyNetQ;
using Microsoft.Extensions.Configuration;
using NCafe.Core.MessageBus;

namespace NCafe.Infrastructure.MessageBus;

internal class RabbitMqPublisher(IConfiguration configuration) : IPublisher
{
    private readonly IBus _bus = RabbitHutch.CreateBus(configuration.GetConnectionString("RabbitMq"));

    public async Task Publish<T>(string topicName, T message) where T : class, IBusMessage
    {
        await _bus.PubSub.PublishAsync(message, topicName);
    }
}
