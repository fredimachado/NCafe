namespace NCafe.Core.MessageBus;

public interface IPublisher
{
    Task Publish<T>(T message) where T : class, IBusMessage;
}
