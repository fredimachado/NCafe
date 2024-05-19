using RabbitMQ.Client;
using System.Diagnostics;

namespace NCafe.Infrastructure.MessageBus;
internal static class RabbitMqHelper
{
    public static IModel CreateModelAndDeclareTestQueue(IConnection connection, string queueName)
    {
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        return channel;
    }

    public static void AddMessagingTags(Activity activity, string exchangeName, string queueName)
    {
        // These tags are added demonstrating the semantic conventions of the OpenTelemetry messaging specification
        // See:
        //   * https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md#messaging-attributes
        //   * https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/rabbitmq.md
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination_kind", "queue");
        activity?.SetTag("messaging.destination", exchangeName);
        activity?.SetTag("messaging.rabbitmq.routing_key", queueName);
    }
}
