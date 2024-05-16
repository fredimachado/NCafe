namespace Aspire.Hosting.ApplicationModel;

/// <summary>
/// A resource that represents an EventStore container.
/// </summary>
/// <param name="name">The name of the resource.</param>
public class EventStoreResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";
    internal const string TcpEndpointName = "tcp";
    internal const int DefaultHttpPort = 2113;
    internal const int DefaultTcpPort = 1113;

    private EndpointReference? _primaryEndpoint;

    /// <summary>
    /// Gets the primary endpoint for the EventStore server.
    /// </summary>
    public EndpointReference PrimaryEndpoint => _primaryEndpoint ??= new(this, HttpEndpointName);

    /// <summary>
    /// Gets the connection string for the EventStore server.
    /// </summary>
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"esdb://{PrimaryEndpoint.Property(EndpointProperty.Host)}:{PrimaryEndpoint.Property(EndpointProperty.Port)}?tls=false");
}
