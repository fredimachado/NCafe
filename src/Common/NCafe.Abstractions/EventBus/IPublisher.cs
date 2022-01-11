namespace NCafe.Abstractions.EventBus;

public interface IPublisher
{
    Task Publish<T>(string topicName, T message) where T : class, IBusMessage;
}
