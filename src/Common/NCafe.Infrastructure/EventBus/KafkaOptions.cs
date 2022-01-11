namespace NCafe.Infrastructure.EventBus;

public class KafkaOptions
{
    public const string SectionKey = "KafkaConfig";

    public string BootstrapServers { get; set; }
}
