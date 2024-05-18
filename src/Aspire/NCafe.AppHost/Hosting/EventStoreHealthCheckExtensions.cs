using HealthChecks.EventStore.gRPC;

namespace Aspire.Hosting;

public static class EventStoreHealthCheckExtensions
{
    /// <summary>
    /// Adds a health check to the EventStore resource.
    /// </summary>
    public static IResourceBuilder<EventStoreResource> WithHealthCheck(this IResourceBuilder<EventStoreResource> builder)
    {
        return builder.WithAnnotation(
            HealthCheckAnnotation.Create(connectionString => new EventStoreHealthCheck(connectionString)));
    }
}
