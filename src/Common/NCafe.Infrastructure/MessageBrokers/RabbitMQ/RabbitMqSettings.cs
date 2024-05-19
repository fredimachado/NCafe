namespace NCafe.Infrastructure.MessageBrokers.RabbitMQ;

internal class RabbitMqSettings
{
    public const string SectionName = "RabbitMq";

    public string ExchangeName { get; set; }

    public string QueuePrefix { get; set; }
}
