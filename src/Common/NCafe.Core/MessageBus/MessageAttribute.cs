namespace NCafe.Core.MessageBus;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class MessageAttribute(string exchangeName) : Attribute
{
    public string ExchangeName { get; } = exchangeName;
}
