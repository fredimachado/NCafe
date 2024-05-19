using System.Reflection;

namespace NCafe.Infrastructure.MessageBrokers.RabbitMQ;

internal static class ExchangeNameProvider
{
    public static string Get(string exchange)
    {
        return !string.IsNullOrWhiteSpace(exchange) ? exchange : Assembly.GetEntryAssembly().GetName().Name;
    }
}
