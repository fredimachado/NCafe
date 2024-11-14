﻿using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.EventStore;
using HealthChecks.EventStore.gRPC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspire.Hosting;

public static class EventStoreBuilderExtensions
{
    /// <summary>
    /// Adds an EventStore resource to the application model. A container is used for local development.
    /// This package defaults to the 20.10.0-buster-slim tag of the eventstore container image.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="httpPort">The port on which the EventStore HTTP endpoint will be exposed.</param>
    /// <param name="tcpPort">The port on which the EventStore TCP endpoint will be exposed.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<EventStoreResource> AddEventStore(
        this IDistributedApplicationBuilder builder, string name, int? httpPort = null, int? tcpPort = null)
    {
        var eventStoreResource = new EventStoreResource(name);

        string? connectionString = null;

        builder.Eventing.Subscribe<ConnectionStringAvailableEvent>(eventStoreResource, async (@event, cancellationToken) =>
        {
            connectionString = await eventStoreResource.ConnectionStringExpression
                .GetValueAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new DistributedApplicationException($"ConnectionStringAvailableEvent was published for the '{eventStoreResource.Name}' resource but the connection string was null.");
        });

        var healthCheckKey = $"{name}_check";
        builder.Services.AddHealthChecks()
            .Add(new HealthCheckRegistration(
                healthCheckKey,
                sp => new EventStoreHealthCheck(connectionString!),
                failureStatus: default,
                tags: default,
                timeout: default));

        return builder
            .AddResource(eventStoreResource)
            .WithEndpoint(port: tcpPort, targetPort: EventStoreResource.DefaultTcpPort, name: EventStoreResource.TcpEndpointName)
            .WithHttpEndpoint(port: httpPort, targetPort: EventStoreResource.DefaultHttpPort, name: EventStoreResource.HttpEndpointName)
            .WithImage(EventStoreContainerImageTags.Image, EventStoreContainerImageTags.Tag)
            .WithImageRegistry(EventStoreContainerImageTags.Registry)
            .WithEnvironment(ConfigureEventStoreContainer)
            .WithHealthCheck(healthCheckKey);
    }

    /// <summary>
    /// Adds a named volume for the data folder to an EventStore container resource.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the application and resource names.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<EventStoreResource> WithDataVolume(this IResourceBuilder<EventStoreResource> builder, string? name = null)
    {
        return builder.WithVolume(name ?? "eventstore-volume-data", "/var/lib/eventstore");
    }

    private static void ConfigureEventStoreContainer(EnvironmentCallbackContext context)
    {
        context.EnvironmentVariables.Add("EVENTSTORE_CLUSTER_SIZE", "1");
        context.EnvironmentVariables.Add("EVENTSTORE_RUN_PROJECTIONS", "All");
        context.EnvironmentVariables.Add("EVENTSTORE_START_STANDARD_PROJECTIONS", "true");
        context.EnvironmentVariables.Add("EVENTSTORE_EXT_TCP_PORT", $"{EventStoreResource.DefaultTcpPort}");
        context.EnvironmentVariables.Add("EVENTSTORE_HTTP_PORT", $"{EventStoreResource.DefaultHttpPort}");
        context.EnvironmentVariables.Add("EVENTSTORE_INSECURE", "true");
        context.EnvironmentVariables.Add("EVENTSTORE_DEV", "true");
        context.EnvironmentVariables.Add("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP", "true");
        context.EnvironmentVariables.Add("EVENTSTORE_ENABLE_EXTERNAL_TCP", "true");
    }
}
