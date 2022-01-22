namespace NCafe.Core.MessageBus;

public interface IPublisher
{
    Task Publish<T>(string topicName, T message) where T : class, IBusMessage;
}
