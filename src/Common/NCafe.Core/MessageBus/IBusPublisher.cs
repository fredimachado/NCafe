namespace NCafe.Core.MessageBus;

public interface IBusPublisher
{
    Task Publish<T>(T message) where T : class, IBusMessage;
}
