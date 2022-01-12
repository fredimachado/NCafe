namespace NCafe.Infrastructure.MessageBus;

public class KafkaOptions
{
    public const string SectionKey = "KafkaConfig";

    public string BootstrapServers { get; set; }
}
