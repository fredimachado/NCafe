using System.Diagnostics;

namespace NCafe.Infrastructure.MessageBrokers.RabbitMQ;

internal static class RabbitMqHelper
{
    public static void AddMessagingTags<T>(Activity activity, T message, string kind, string exchangeName, string routingKey)
        where T : class
    {
        // These tags are added demonstrating the semantic conventions of the OpenTelemetry messaging specification
        // See:
        //   * https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md#messaging-attributes
        //   * https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/rabbitmq.md
        activity?.SetTag("message", message);
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination_kind", kind);
        activity?.SetTag("messaging.destination", exchangeName);
        activity?.SetTag("messaging.rabbitmq.routing_key", routingKey);
    }
}
